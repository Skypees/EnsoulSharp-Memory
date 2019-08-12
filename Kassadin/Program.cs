using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace kassadinfix
{
    class Program
    {
        public static Menu Config { get; set; }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static Spell Q, W, E, R;
        private static SpellSlot Ignite;
        public static int tickNum = 4, tickIndex = 0;
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += GameEventOnOnGameLoad;
        }

        private static void GameEventOnOnGameLoad()
        {
            if (Player.CharacterName != "Kassadin") return;
            CreateSpells();
            CreateMenu();
            CreateEvents();
            Chat.Print("<font color=\"#008aff\"> xDreamms Kassadin </font> ver 1.0 by <font color=\"#FF0000\"> xDreamms</font> - <font color=\"#00BFFF\">Loaded</font>");

        }

        private static void CreateEvents()
        {
            Game.OnUpdate += GameOnOnUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            Interrupter.OnInterrupterSpell += InterrupterOnOnInterrupterSpell;
        }

        private static void InterrupterOnOnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (sender.IsEnemy && sender.Distance(Player) < Q.Range)
            {
                Q.CastOnUnit(sender);
            }
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Green);
            }
            else
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Crimson);
            }
            if (R.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Green);
            }
            else
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Crimson);
            }
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                LaneClear();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                LastHit();
            }
            if (FleeKey)
            {
                Flee();
            }
            KillSteal();
            tickIndex++;
            if (tickIndex > 4)
                tickIndex = 0;
        }

        private static void LastHit()
        {
            if (LastHitUseQ && Q.IsReady() && LagFree(4))
            {
                var minion = GameObjects.EnemyMinions.Where(x =>
                    x.IsValidTarget(Q.Range) && x.IsEnemy && Player.GetSpellDamage(x, SpellSlot.Q) > x.Health).FirstOrDefault();
                Q.CastOnUnit(minion);
            }
        }

        private static void LaneClear()
        {
            if (LaneClearUseQ && Q.IsReady() && LagFree(4))
            {
                var qMinions = GameObjects.Minions.Where(x => x.IsValidTarget(Q.Range) && !x.IsAlly);
                Q.CastOnUnit(qMinions.FirstOrDefault());
            }
            if (LaneClearUseW && W.IsReady() && LagFree(1))
            {
                var wMinions = GameObjects.Minions.Where(x => x.Distance(Player) <180 && !x.IsAlly).Count();
                if (wMinions > 0)
                {
                    W.Cast();
                }
            }
            if (LaneClearUseE && E.IsReady() && LagFree(2))
            {
                var eMinions = GameObjects.Minions.Where(x => x.IsValidTarget(E.Range) && !x.IsAlly).FirstOrDefault();
                E.Cast(eMinions.Position);
            }
            if (LaneClearUseR && R.IsReady() && Player.CountEnemyHeroesInRange(900) == 0 && LagFree(1))
            {
                var rMinions = GameObjects.Minions.Where(x => x.IsValidTarget(R.Range) && x.IsEnemy).ToList();
               
                    var minionLocation = R.GetCircularFarmLocation(rMinions);
                    R.Cast(minionLocation.Position);
            }
        }

        private static void KillSteal()
        {
            if (MiscUseIgnite && Ignite.IsReady())
            {
                var target = TargetSelector.GetTarget(600, DamageType.True);
                if (target != null && CastIgnite(target))
                {
                    return;
                }
            }
        }
        public static bool CastIgnite(AIHeroClient target)
        {
            return Ignite.IsReady() && target.IsValidTarget(600)
                   && target.Health + 5 < Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite)
                   && Player.Spellbook.CastSpell(Ignite, target);
        }
        private static void Flee()
        {
            if (!FleeUseR)
                return;

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosCenter);

            if (R.IsReady() && LagFree(1))
            {
                R.Cast(Game.CursorPosCenter);
            }
        }

        private static void Harass()
        {
            if (Q.IsReady() && HarassUseQ)
            {
                AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (target.IsValidTarget(Q.Range) && LagFree(4))
                {
                    Q.CastOnUnit(target);
                }
            }

            if (W.IsReady() && HarassUseW)
            {
                int enemies = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.Distance(Player) < 180).Count();

                if (enemies > 0 && LagFree(1))
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && HarassUseE)
            {
                AIHeroClient target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                if (target.IsValidTarget(E.Range) && LagFree(3))
                {
                    E.Cast(target.Position);
                }
            }
        }

        private static void Combo()
        {
            if (Q.IsReady() && CombouseQ)
            {
                AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (target.IsValidTarget(Q.Range) && LagFree(2))
                {
                    Q.CastOnUnit(target);
                }
            }

            if (W.IsReady() && CombouseW)
            {
                int enemies = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.Distance(Player) < 280).Count();

                if (enemies > 0 && LagFree(1))
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && CombouseE)
            {
                AIHeroClient target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

                if (target.IsValidTarget(E.Range) && LagFree(3))
                {
                    E.Cast(target.Position);
                }
            }
            if (R.IsReady() && CombouseR)
            {
                AIHeroClient target = TargetSelector.GetTarget(R.Range, DamageType.Magical);

                var extraEnemies =
                    ObjectManager.Get<AIHeroClient>().Where(x => x != target && x.IsEnemy && x.Distance(target) < 800).Count();
                if (target.IsValidTarget(R.Range) && extraEnemies < ComboDontR && LagFree(4))
                {
                    R.Cast(target.Position);
                }
            }
        }
        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            else
                return false;
        }
        private static void CreateMenu()
        {
            Config = new Menu("xDreammsKassadin","xDreamms Kassadin",true);

            //Combo Menu
            var comboMenu = new Menu("Combo","Combo");
            Helper.AddMenuBool(comboMenu, "CombouseQ", "Use Q");
            Helper.AddMenuBool(comboMenu, "CombouseW", "Use W");
            Helper.AddMenuBool(comboMenu, "CombouseE", "Use E");
            Helper.AddMenuBool(comboMenu, "CombouseR", "Use R");
            Helper.AddMenuSlider(comboMenu, "ComboDontR", "Don't R if >= X Enemies", 3,1,5);
            Config.Add(comboMenu);

            //Harass Menu
            var harassMenu = new Menu("Harass", "Harass");
            Helper.AddMenuBool(harassMenu, "HarassUseQ", "Use Q");
            Helper.AddMenuBool(harassMenu, "HarassUseW", "Use W");
            Helper.AddMenuBool(harassMenu, "HarassUseE", "Use E");
            Config.Add(harassMenu);

            //Lane Clear Menu
            var lClearMenu = new Menu("LaneClear", "LaneClear");
            Helper.AddMenuBool(lClearMenu, "LaneClearUseQ", "Use Q");
            Helper.AddMenuBool(lClearMenu, "LaneClearUseW", "Use W");
            Helper.AddMenuBool(lClearMenu, "LaneClearUseE", "Use E");
            Helper.AddMenuBool(lClearMenu, "LaneClearUseR", "Use R if no enemies arround");

            Config.Add(lClearMenu);

            //Last Hit
            var lastHitMenu = new Menu("LastHit","LastHit");
            Helper.AddMenuBool(lastHitMenu, "LastHitUseQ", "Use Q");
            Config.Add(lastHitMenu);

            //Flee Menu
            var fleeMenu = new Menu("Flee","Flee");
            Helper.AddMenuBool(fleeMenu, "FleeUseR", "Use R");
            Helper.AddMenuKeyBind(fleeMenu, "FleeKey", "Flee !", Keys.Z,KeyBindType.Press);
            Config.Add(fleeMenu);

            //Misc Menu
            var miscMenu = new Menu("Misc", "Misc");
            Helper.AddMenuBool(miscMenu, "MiscUseIgnite", "KillSteal with Ignite.");
            Config.Add(miscMenu);
            Config.Attach();
           
        }

        private static void CreateSpells()
        {
            Q = new Spell(SpellSlot.Q, 650f);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 580f);
            R = new Spell(SpellSlot.R, 550f);
            Ignite = Player.GetSpellSlot("summonerdot");
            Q.SetTargetted(0.5f, 1400f);
            E.SetSkillshot(0.5f, 10f, float.MaxValue, false, SkillshotType.Cone);
            R.SetSkillshot(0.5f, 150f, float.MaxValue, false, SkillshotType.Circle);
        }

        public static bool CombouseQ { get { return Helper.GetMenuBoolValue(Config, "Combo", "CombouseQ"); }}
        public static bool CombouseW { get { return Helper.GetMenuBoolValue(Config, "Combo", "CombouseW"); } }
        public static bool CombouseE { get { return Helper.GetMenuBoolValue(Config, "Combo", "CombouseE"); } }
        public static bool CombouseR { get { return Helper.GetMenuBoolValue(Config, "Combo", "CombouseR"); } }
        public static int ComboDontR { get { return Helper.GetMenuSliderValue(Config, "Combo", "ComboDontR"); } }
        public static bool HarassUseQ { get { return Helper.GetMenuBoolValue(Config, "Harass", "HarassUseQ"); } }
        public static bool HarassUseW { get { return Helper.GetMenuBoolValue(Config, "Harass", "HarassUseW"); } }
        public static bool HarassUseE { get { return Helper.GetMenuBoolValue(Config, "Harass", "HarassUseE"); } }
        public static bool LaneClearUseQ { get { return Helper.GetMenuBoolValue(Config, "LaneClear", "LaneClearUseQ"); } }
        public static bool LaneClearUseW { get { return Helper.GetMenuBoolValue(Config, "LaneClear", "LaneClearUseW"); } }
        public static bool LaneClearUseE { get { return Helper.GetMenuBoolValue(Config, "LaneClear", "LaneClearUseE"); } }
        public static bool LaneClearUseR { get { return Helper.GetMenuBoolValue(Config, "LaneClear", "LaneClearUseR"); } }

        public static bool LastHitUseQ { get { return Helper.GetMenuBoolValue(Config, "LastHit", "LastHitUseQ"); } }
        public static bool FleeUseR { get { return Helper.GetMenuBoolValue(Config, "Flee", "FleeUseR"); } }
        public static bool FleeKey { get { return Helper.GetMenuKeyBindValue(Config, "Flee", "FleeKey"); } }
        public static bool MiscUseIgnite { get { return Helper.GetMenuBoolValue(Config, "Misc", "MiscUseIgnite"); } }

    }
}
