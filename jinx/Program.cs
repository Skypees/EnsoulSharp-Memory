namespace MemoryJinx
{
    // namespace
    using System;
    using System.Linq;

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;

    using Color = System.Drawing.Color;

    public class Program
    {
        private static Menu MainMenu;

        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        public static int tickNum = 4, tickIndex = 0;
        private static void Main(string[] args)
        {
            // start init script when game already load
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            Notifications.Add(new Notification("Memory Jinx", "credit  Memory"));
            // judge champion Name
            if (ObjectManager.Player.CharacterName != "Jinx")
            {
                return;
            }

            // create spell
            Q = new Spell(SpellSlot.Q); //skillshot
            Q.SetSkillshot(0.25f, 80f, float.MaxValue, false, SkillshotType.Line);

            W = new Spell(SpellSlot.W, 1500f); //charge spell
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.Line);

            E = new Spell(SpellSlot.E, 900f); //self cast
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.Circle);

            R = new Spell(SpellSlot.R, 2500f); //target
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.Line);
            // create menu
            MainMenu = new Menu("Mjinx", "Mjinx", true);

            // combo menu
            var comboMenu = new Menu("Combo", "Combo Settings");
            comboMenu.Add(new MenuBool("comboQ", "Use Q", true));
            comboMenu.Add(new MenuBool("comboW", "Use W", true));
            comboMenu.Add(new MenuBool("comboE", "Use E", true));
            comboMenu.Add(new MenuBool("comboR", "Use R", true));
            MainMenu.Add(comboMenu);

            // draw menu 
            var drawMenu = new Menu("Draw", "Draw Settings");
            drawMenu.Add(new MenuBool("drawQ", "Draw Q Range", true));
            MainMenu.Add(drawMenu);

            //example boolean on MainMenu
            MainMenu.Add(new MenuBool("isDead", "if Player is Dead not Draw Range", true));

            // init MainMenu
            MainMenu.Attach();

            // events
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void Combo()
        {
            // cast q (is skillshot spell)
            // if menuitem enabled + Q ready
           if (MainMenu["Combo"]["comboQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
                 {
            foreach (var t in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(t => t.IsValidTarget(GetRealPowPowRange(t) + QAddRange + 20f)))
                {
                    var swapDistance = true;
                    var swapAoe = true;
                    var distance = GetRealDistance(t);
                    var powPowRange = GetRealPowPowRange(t);

                    if (swapDistance && Q.IsReady())
                    {
                        if (distance > powPowRange && !FishBoneActive)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                        else if (distance < powPowRange && FishBoneActive)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                    }

                    if (swapAoe && Q.IsReady())
                    {
                        if (distance > powPowRange && PowPowStacks > 2 && !FishBoneActive && CountEnemies(t, 150) > 1)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                    }
                }
            }

            // cast w (is charge spell)
            // if menuitem enabled + Q ready
            if (MainMenu["Combo"]["comboW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                // get target
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                // or like this
                target = TargetSelector.GetTarget(W.Range);
                // both work for get target

                // judge target is valid
                if (target != null && target.IsValidTarget(W.Range) && GetRealDistance(target) >= 400 && LagFree(2))
                {
                    // get pred
                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        // cast skillshot
                        W.Cast(pred.CastPosition);
                    }
                }
            }


            // cast e (is selfcast spell)
            // if menuitem enabled + E ready
            if (MainMenu["Combo"]["comboE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                // or like this
                target = TargetSelector.GetTarget(E.Range);
                // both work for get target

                // judge target is valid
                if (target != null && target.IsValidTarget(E.Range) && LagFree(3))
                {
                    // get pred
                    var pred = E.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        // cast skillshot
                        E.Cast(pred.CastPosition);
                    }
                }
            }

            // cast r (is target spell)
            // if menuitem enabled + Q ready
            if (MainMenu["Combo"]["comboR"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                 var target = TargetSelector.GetTarget(R.Range);
                // or like this
                target = TargetSelector.GetTarget(R.Range);
                // both work for get target

                // judge target is valid
                if (target != null && target.IsValidTarget(R.Range) && GetRealDistance(target) >= 900 && ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health && LagFree(4))
                {
                    // get pred
                    var pred = R.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        // cast skillshot
                        R.Cast(pred.CastPosition);
                    }
                }
            }
        }
        private static int CountAlliesNearTarget(AIBaseClient target, float range)
        {
            return ObjectManager.Get<AIHeroClient>().Count(hero => hero.Team == ObjectManager.Player.Team &&
            hero.Position.Distance(target.Position) <= range);
        }
        private static void Clear()
        {
            // check out Ashe or Kalista
            // it already have example

            // get Minion
            var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range) && x.IsMinion());

            // get Mob
            var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range));

            // get Legendary Mob (Dragon, Baron, ect)
            var lMobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() == JungleType.Legendary);

            // get Large Mob (Red Buff, Blue Buff, ect)
            var bMobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() == JungleType.Large);
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    Clear();
                    break;
            }
            tickIndex++;
            if (tickIndex > 4)
                tickIndex = 0;
        }
        private static int CountEnemies(AIBaseClient target, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                            hero.IsValidTarget() && hero.Team != ObjectManager.Player.Team &&
                            hero.Position.Distance(target.Position) <= range);
        }
        private static float GetRealPowPowRange(GameObject target)
        {
            return 525f + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }
        private static float GetRealDistance(GameObject target)
        {
            return ObjectManager.Player.Position.Distance(target.Position) + ObjectManager.Player.BoundingRadius +
            target.BoundingRadius;
        }
        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            else
                return false;
        }
        public static float QAddRange
        {
            get { return 50 + 25 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level; }
        }

        private static bool FishBoneActive
        {
            get { return ObjectManager.Player.AttackRange > 565f; }
        }

        private static int PowPowStacks
        {
            get
            {
                return ObjectManager.Player.Buffs.Where(buff => buff.Name.ToLower() == "jinxqramp").Select(buff => buff.Count)
                    .FirstOrDefault();
            }
        }
        private static void OnDraw(EventArgs args)
        {
            // if Player is Dead not Draw Range
            if (MainMenu["isDead"].GetValue<MenuBool>().Enabled)
            {
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }
            }

            // draw Q Range
            if (MainMenu["Draw"]["drawQ"].GetValue<MenuBool>().Enabled)
            {
                // Draw Circle
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua);
            }
        }
    }
}
