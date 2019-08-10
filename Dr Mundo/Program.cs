namespace MDrMundo
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

        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "DrMundo")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 975f);
            Q.SetSkillshot(0.25f, 100f, 1850f, true, SkillshotType.Line);

            W = new Spell(SpellSlot.W, 325f);

            E = new Spell(SpellSlot.E, 225f);

          
            MainMenu = new Menu("MDrMundo", "Memory DrMundo", true);

            var comboMenu = new Menu("Combo", "Combo Settings");
            comboMenu.Add(new MenuBool("comboQ", "Use Q", true));
            comboMenu.Add(new MenuBool("comboW", "Use W", true));
            comboMenu.Add(new MenuBool("comboE", "Use E", true));
           // comboMenu.Add(new MenuBool("comboR", "Use R", true));
            MainMenu.Add(comboMenu);
            var laneclearMenu = new Menu("Lane Clear", "LaneClear");
            laneclearMenu.Add(new MenuBool("clearQ", "Use Q", true));
            laneclearMenu.Add(new MenuBool("clearW", "Use W", true));
            laneclearMenu.Add(new MenuBool("clearE", "Use E", true));
            MainMenu.Add(laneclearMenu);

            var jungleclearMenu = new Menu("Jungle Clear", "JungleClear");
            jungleclearMenu.Add(new MenuBool("jungleQ", "Use Q", true));
            jungleclearMenu.Add(new MenuBool("jungleW", "Use W", true));
            jungleclearMenu.Add(new MenuBool("jungleE", "Use E", true));
            MainMenu.Add(jungleclearMenu);

            var drawMenu = new Menu("Draw", "Draw Settings");
            drawMenu.Add(new MenuBool("drawQ", "Draw Q Range", true));
            MainMenu.Add(drawMenu);

            MainMenu.Add(new MenuBool("isDead", "if Player is Dead not Draw Range", true));

            MainMenu.Attach();

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void Combo()
        {

            if (MainMenu["Combo"]["comboQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                target = TargetSelector.GetTarget(Q.Range);

                if (target != null && target.IsValidTarget(Q.Range))
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
           if (MainMenu["Combo"]["comboW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                target = TargetSelector.GetTarget(W.Range);
                if (target != null && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
                }
            if (MainMenu["Combo"]["comboE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {

                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                target = TargetSelector.GetTarget(E.Range);
                 if (target != null && target.IsValidTarget(E.Range))
                {
                E.Cast();
                }
            }

            
        }

        static void JungleClear()
        {
            var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range));
            if (MainMenu["Jungle Clear"]["jungleQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                foreach (var minion in mobs)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.GetPrediction(minion);
                        Q.CastIfHitchanceEquals(minion, HitChance.High);
                    }
                }
            }
                 if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {     
                if (MainMenu["Lane Clear"]["jungleW"].GetValue<MenuBool>().Enabled && W.IsReady() && ObjectManager.Get<AIMinionClient>().Any(minion => minion.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
                }
            if (MainMenu["Lane Clear"]["jungleE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                foreach (var minion in mobs)
                {
                    if (minion.IsValidTarget())
                    {
                        E.Cast();
                    }

                }
            }

        }
        private static void Clear()
        {
            var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range) && x.IsMinion());
            if (MainMenu["Lane Clear"]["clearQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.GetPrediction(minion);
                        Q.CastIfHitchanceEquals(minion, HitChance.High);
                    }
                }
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {     

                if (MainMenu["Lane Clear"]["clearW"].GetValue<MenuBool>().Enabled && W.IsReady() && !IsBurning() && ObjectManager.Get<AIMinionClient>().Any(minion => minion.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
                }
            if (MainMenu["Lane Clear"]["clearE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.CastOnUnit(minion);
                    }

                }
            }

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
               case OrbwalkerMode.None:
                    BurningManager();
                    break;
            }
        }
        private static bool IsBurning()
        {
            return Player.HasBuff("BurningAgony");
        }
        private static void BurningManager()
        {
           if (IsBurning()){
               W.Cast();
           }
        }

        private static void OnDraw(EventArgs args)
        {
            if (MainMenu["isDead"].GetValue<MenuBool>().Enabled)
            {
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }
            }

            if (MainMenu["Draw"]["drawQ"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua);
            }
        }
    }
}
