namespace EnsoulSharp.Yasuo
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using EnsoulSharp.SDK.Utility;
    using EnsoulSharp.SDK.Prediction;
    using Color = System.Drawing.Color;
    using SharpDX;
    internal class Yasuo
    {
        private static readonly Menu MenuYasuo;
        public static Spell Q, Q2, W, E, R;
        private const int QRange = 550, Q2Range = 1150, QCirWidth = 275, cirqwdthmin = 250, RWidth = 400;
        private static SpellSlot Ignite;
        public static int tickNum = 4, tickIndex = 0;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static AIBaseClient target;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public class ComboYasuo
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool E = new MenuBool("e", "Use E");
            public static readonly MenuBool R = new MenuBool("r", "Use R");
            public static readonly MenuKeyBind usemin = new MenuKeyBind("useminion", "E Cast Minion(Go To Enemy Heroes)", System.Windows.Forms.Keys.J, KeyBindType.Toggle);
            public static readonly MenuSlider cmboegprange = new MenuSlider("egpclsr", "Gapcloser", 300, 1, 475);
            public static readonly MenuKeyBind cmboegptower = new MenuKeyBind("eg4ptuwer", "E Under Turret", System.Windows.Forms.Keys.A, KeyBindType.Toggle);
            public static readonly MenuBool cmbeg4p = new MenuBool("cmbegps", "G4p Cl0ser");
            public static readonly MenuKeyBind stackq = new MenuKeyBind("stack", "Q Stack Farm Lasthit", System.Windows.Forms.Keys.G, KeyBindType.Toggle);
        }
        public class flee
        {
            public static readonly MenuKeyBind fl33Z = new MenuKeyBind("fleeze", "Flying minion", System.Windows.Forms.Keys.Z, KeyBindType.Press);
            public static readonly MenuBool fl33Est4ckq = new MenuBool("fl33Est4ckq", "fl33 EQ Stack Q");
        }
        public class OtoR
        {
            public static readonly MenuKeyBind autoRactive = new MenuKeyBind("auR", "Combo Use Auto R", System.Windows.Forms.Keys.T, KeyBindType.Toggle);
        }
        public class HaraSs
        {
            public static readonly MenuBool hQ = new MenuBool("useq", "Use Q");
            public static readonly MenuBool hW = new MenuBool("usew", "Use W");
            public static readonly MenuBool hE = new MenuBool("usee", "Use E");
        }
        public class Junglemenu
        {
            public static readonly MenuBool jQ = new MenuBool("useq", "Use Q");
            public static readonly MenuBool jW = new MenuBool("usew", "Use W");
            public static readonly MenuBool jE = new MenuBool("usee", "Use E");
        }
        public class LaneclearMenu
        {
            public static readonly MenuBool lQ = new MenuBool("useq", "Use Q");
            public static readonly MenuBool lE = new MenuBool("usee", "Use E");
        }
        public class LastHit
        {
            public static readonly MenuBool lastq = new MenuBool("DQ", "Use Q to Last Hit Minion");
            public static readonly MenuBool lastfq = new MenuBool("DFQ", "Use Q to Last Hit Jungle");
        }
        public class KSYasuo
        {


            public static readonly MenuBool KE = new MenuBool("DE", "Killsteal with E");
            public static readonly MenuBool KQ = new MenuBool("DQ", "Killsteal with Q");
            public static readonly MenuBool KR = new MenuBool("DR", "Killsteal with R");
        }
        public class Draws
        {
            public static readonly MenuBool DQ = new MenuBool("Drawq", "Draw Q Range");
            public static readonly MenuBool DW = new MenuBool("Draww", "Draw W Range");
            public static readonly MenuBool DE = new MenuBool("Drawe", "Draw E Range");
            public static readonly MenuBool DR = new MenuBool("Drawr", "Draw R Range");
        }
        public static bool ignitecast(AIHeroClient target)
        {
            return Ignite.IsReady() && target.IsValidTarget(600)
                   && target.Health + 5 < Player.GetSummonerSpellDamage(target, SummonerSpell.Ignite)
                   && Player.Spellbook.CastSpell(Ignite, target);
        }
        private static float g3tq2d3l4y
        {
            get
            {
                return 0.5f * (1 - Math.Min((Player.AttackSpeedMod - 1) * 0.58f, 0.66f));
            }
        }
        private static float g3tqd3lay
        {
            get
            {
                return 0.4f * (1 - Math.Min((Player.AttackSpeedMod - 1) * 0.58f, 0.66f));
            }
        }
        private static bool q3h4ve
        {
            get { return Q.Name == "YasuoQ3Wrapper"; }
        }
        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            Q2 = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 1300);
            R.SetSkillshot(0.70f, 125f, float.MaxValue, false, SkillshotType.Circle);
            Ignite = Player.GetSpellSlot("summonerdot");
            Q.SetSkillshot(g3tqd3lay, 20, float.MaxValue, false, SkillshotType.Line);

            Q2.SetSkillshot(g3tq2d3l4y, 90, 1500, false, SkillshotType.Line);
            E.SetTargetted(0.05f, 1000);
            var MenuYasuo = new Menu("Memory.Yasuo", "Memory.Yasuo", true);
            var combomenu = new Menu("combo", "Combo");
            combomenu.Add(ComboYasuo.Q);
            combomenu.Add(ComboYasuo.W);
            combomenu.Add(ComboYasuo.E);
            combomenu.Add(ComboYasuo.R);
            combomenu.Add(ComboYasuo.usemin).Permashow();
            combomenu.Add(ComboYasuo.cmboegptower).Permashow();
            combomenu.Add(ComboYasuo.stackq).Permashow();
            MenuYasuo.Add(combomenu);
            var HarassMenu = new Menu("harass", "Harass");
            HarassMenu.Add(HaraSs.hQ);
            HarassMenu.Add(HaraSs.hW);
            HarassMenu.Add(HaraSs.hE);
            MenuYasuo.Add(HarassMenu);
            var fleemenu = new Menu("flee", "fl33");
            fleemenu.Add(flee.fl33Z).Permashow();
            MenuYasuo.Add(fleemenu);
            var otomaticR = new Menu("otoR", "Oto R");
            otomaticR.Add(OtoR.autoRactive).Permashow();
            MenuYasuo.Add(otomaticR);
            var junglem = new Menu("jungle", "JungleCl3aring");
            junglem.Add(Junglemenu.jQ);
            junglem.Add(Junglemenu.jW);
            junglem.Add(Junglemenu.jE);


            MenuYasuo.Add(junglem);
            var laneclm = new Menu("lane", "LaneCl3aring");
            laneclm.Add(LaneclearMenu.lQ);
            laneclm.Add(LaneclearMenu.lE);
            MenuYasuo.Add(laneclm);
            var KSMenu = new Menu("killsteal", "Killsteal");
            KSMenu.Add(KSYasuo.KE);
            KSMenu.Add(KSYasuo.KQ);
            KSMenu.Add(KSYasuo.KR);
            MenuYasuo.Add(KSMenu);
            var DrawMenu = new Menu("drawings", "Drawings");
            DrawMenu.Add(Draws.DQ);
            DrawMenu.Add(Draws.DE);
            DrawMenu.Add(Draws.DR);
            MenuYasuo.Add(DrawMenu);
            MenuYasuo.Attach();
            Game.OnUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
        }
        private static void Killsteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && !x.IsInvulnerable && x.Health < Q.GetDamage(x)))
            {
                if (Q.IsReady() && KSYasuo.KQ.Enabled)
                {
                    if (GameObjects.Player.GetSpellDamage(target, SpellSlot.Q) >= target.Health && target.IsValidTarget(E.Range) && LagFree(4))
                    {
                        Q.CastOnUnit(target);
                    }
                }
            }
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && !x.IsInvulnerable && x.Health < E.GetDamage(x)))
            {
                if (E.IsReady() && KSYasuo.KE.Enabled)
                {
                    if (GameObjects.Player.GetSpellDamage(target, SpellSlot.E) >= target.Health && target.IsValidTarget(E.Range) && LagFree(3))
                    {
                        E.CastOnUnit(target);
                    }
                }
            }
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && !x.IsInvulnerable && x.Health < R.GetDamage(x)))
            {
                if (R.IsReady() && KSYasuo.KR.Enabled)
                {
                    if (GameObjects.Player.GetSpellDamage(target, SpellSlot.R) >= target.Health && target.IsValidTarget(R.Range) && LagFree(1))
                    {
                        R.CastOnUnit(target);
                    }
                }

            }
        }
        private static void Harass()
        {
            bool useQ = HaraSs.hQ.Enabled;
            bool useW = HaraSs.hW.Enabled;
            bool useE = HaraSs.hE.Enabled;
            var target = TargetSelector.GetTarget(E.Range);
            if (target != null && target.IsValidTarget(E.Range))
            {
                if (!target.IsValidTarget())
                {
                    return;
                }
                if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
                {
                    if (target != null && LagFree(4))
                    {
                        Q.CastOnUnit(target);
                    }
                }

            }
        }
        private static void OnTick(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || MenuGUI.IsChatOpen || ObjectManager.Player.IsWindingUp)
            {
                return;
            }
            if (Player.HasBuff("YasuoQ2"))
            {
                Q2.IsReady();
            }
            if (flee.fl33Z.Active == true)
            {
                fl33();
            }
            Killsteal();
            st4ckq();



            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combat();
                    Killsteal();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LastHit:

                    break;
                case OrbwalkerMode.LaneClear:
                    Cl3aring();
                    Jungle();
                    break;
            }
            tickIndex++;
            if (tickIndex > 4)
                tickIndex = 0;
        }
        public static List<AIMinionClient> GetGenericJungleMinionsTargets()
        {
            return GetGenericJungleMinionsTargetsInRange(float.MaxValue);
        }
        public static List<AIMinionClient> GetGenericJungleMinionsTargetsInRange(float range)
        {
            return GameObjects.Jungle.Where(m => !GameObjects.JungleSmall.Contains(m) && m.IsValidTarget(range)).ToList();
        }
        public static List<AIMinionClient> GetEnemyLaneMinionsTargets()
        {
            return GetEnemyLaneMinionsTargetsInRange(float.MaxValue);
        }
        public static List<AIMinionClient> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range)).ToList();
        }
        private static void Jungle()
        {
            bool useQ = Junglemenu.jQ.Enabled;


            bool useW = Junglemenu.jW.Enabled;
            bool useE = Junglemenu.jE.Enabled;
            var mob = GameObjects.Jungle
                .Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() != JungleType.Unknown)
                .OrderByDescending(x => x.MaxHealth).FirstOrDefault();

            if (E.IsReady() && useE && mob.IsValidTarget(E.Range))
                E.Cast(mob);
            if (mob != null)
            {
                if (Q.IsReady() && useQ && mob.IsValidTarget(400) && LagFree(2))
                    Q.Cast(mob);

                if (E.IsReady() && useE && mob.IsValidTarget(E.Range) && LagFree(4))
                    E.Cast(mob);
                E.CastOnUnit(mob);

            }
        }
        private static double g3tdmg(AIBaseClient target)
        {
            return Player.CalculateDamage(
                target,


        DamageType.Magical,
                (50 + 20 * E.Level) * (1 + Math.Max(0, Player.GetBuffCount("YasuoDashScalar") * 0.25))
                + 0.6 * Player.FlatMagicDamageMod);
        }
        private static double g3tqdmg(AIBaseClient target)
        {
            return Player.GetSpellDamage(target, SpellSlot.Q);
        }
        private static void Cl3aring()
        {
            if (E.IsReady() && LagFree(4))
            {
                var mniobj = ObjectManager.Get<AIMinionClient>().Where(x => x.IsValidTarget(E.Range) && x.IsEnemy)
                    .Where(i => c4nc4stE(i) && (!turretunder(PosAfterE(i))))
                    .ToList();
                if (mniobj.Any())
                {
                    var obj = mniobj.FirstOrDefault(i => CanKill(i, g3tdmg(i)));
                    if (obj == null && Q.IsReady(50)
                        && (!q3h4ve))
                    {
                        obj = (from i in mniobj
                               let sub = ObjectManager.Get<AIMinionClient>().Where(x =>
                                   x.IsValidTarget(QCirWidth, true, PosAfterE(x)) && x.IsEnemy)
                               where
                                             i.Team == GameObjectTeam.Neutral
                                                                           || (i.Distance(PosAfterE(i)) < cirqwdthmin && CanKill(i, g3tdmg(i) + g3tqdmg(i)))
                                   || sub.Any(a => CanKill(a, g3tqdmg(a))) || sub.Count() > 1
                               select i).MaxOrDefault(




                            i => ObjectManager.Get<AIMinionClient>()
                                .Where(x => x.IsValidTarget(QCirWidth, true, PosAfterE(x)) && x.IsEnemy).Count());
                    }
                    if (obj != null && E.CastOnUnit(obj))
                    {
                        return;
                    }
                }
            }
            if (Q.IsReady() && (!q3h4ve) && LagFree(1))
            {
                if (Player.IsDashing())
                {
                    var mniobj = ObjectManager.Get<AIMinionClient>()
                        .Where(x => x.IsValidTarget(QCirWidth, true, Player.GetDashInfo().EndPos.ToVector3()) &&
                                    x.IsEnemy);

                    if ((mniobj.Any(i => CanKill(i, g3tqdmg(i)) || i.Team == GameObjectTeam.Neutral)
                         || mniobj.Count() > 1) && Player.Distance(Player.GetDashInfo().EndPos) < 150
                        && cqcir(mniobj.MinOrDefault(i => i.Distance(Player))))
                    {
                        return;
                    }
                }
                else
                {
                    var mniobj = ObjectManager.Get<AIMinionClient>()
                        .Where(x => x.IsValidTarget(!q3h4ve ? QRange : Q2Range) && x.IsEnemy).OrderBy(a => a.MaxHealth);

                    if (mniobj.Any() && LagFree(1))
                    {
                        if (!q3h4ve)
                        {


                            var obj = mniobj.FirstOrDefault(i => CanKill(i, g3tqdmg(i)));
                            if (obj != null && Q.Cast(obj))
                            {
                                return;
                            }
                        }
                        var qMinHit = Q.MinHitChance;
                        Q.MinHitChance = HitChance.High;
                        var pos = (!q3h4ve ? Q : Q2).GetLineFarmLocation(mniobj.Cast<AIBaseClient>().ToList());
                        Q.MinHitChance = qMinHit;
                        if (pos.MinionsHit > 0 && (!q3h4ve ? Q : Q2).Cast(pos.Position) && LagFree(4))
                        {
                            return;
                        }
                    }
                }
            }
        }
        private static AIHeroClient QCirTarget
        {
            get
            {
                var pos = Player.GetDashInfo().EndPos.ToVector3();
                var target = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(QCirWidth, true, pos) && !x.IsAlly && !x.IsZombie).FirstOrDefault();
                return target != null && Player.Distance(pos) < 150 ? target : null;
            }
        }
        private static void fl33()
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosCenter);




            if (!flee.fl33Z.Active)
            {
                return;
            }
            if (flee.fl33Est4ckq && Q.IsReady() && Player.IsDashing())
            {
                if (QCirTarget != null && cqcir(QCirTarget))
                {
                    return;
                }
                var minionObj = ObjectManager.Get<AIMinionClient>().Where(x =>
                    x.IsValidTarget(QCirWidth, true, Player.GetDashInfo().EndPos.ToVector3()) && x.IsEnemy);

                if (minionObj.Any() && LagFree(2) && Player.Distance(Player.GetDashInfo().EndPos) < 150
                    && cqcir(minionObj.MinOrDefault(i => i.Distance(Player))))
                {
                    return;
                }
            }
            var obj = GetNearObj();
            if (obj == null || !E.IsReady())
            {
                return;
            }
            E.CastOnUnit(obj);
        }
        private static bool cqcir(AIBaseClient target)
        {
            return target.IsValidTarget(cirqwdthmin - target.BoundingRadius) && Q.Cast(Game.CursorPosCenter);
        }
        private static void st4ckq()
        {
            var targetm = TargetSelector.GetTarget(1200);
            if (ComboYasuo.stackq.Active && targetm.IsValidTarget(600) == false)
            {
                if (Player.HasBuff("YasuoQ2"))
                {
                    if (Q.IsReady() && targetm.IsValidTarget(Q.Range))
                    {
                        if (targetm != null && LagFree(4))
                        {
                            Q.Cast(targetm);
                        }
                    }
                }
                if (Player.HasBuff("YasuoQ2"))
                {
                    if (Q.IsReady() && targetm.IsValidTarget(Q.Range) == false)
                    {
                        foreach (var minion in GetEnemyLaneMinionsTargetsInRange(Q.Range))
                            if (minion.Health <= GameObjects.Player.GetSpellDamage(minion, SpellSlot.Q) && LagFree(2))
                            {
                                Q.CastIfHitchanceEquals(minion, HitChance.VeryHigh);
                            }
                    }
                }
                if (!Q.IsReady() || Player.IsDashing())
                {
                    return;
                }
                var target = Q.GetTarget();
                if (target != null && (!turretunder(Player.Position) || !turretunder(target.Position)) && LagFree(4))
                {

                    {
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }


                }
                else
                {
                    var mniobj = ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && i.IsEnemy)
                        .OrderBy(a => a.MaxHealth);
                    if (!mniobj.Any())
                    {
                        return;
                    }
                    var obj = mniobj.FirstOrDefault(i => CanKill(i, g3tqdmg(i)))
                              ?? mniobj.MinOrDefault(i => i.Distance(Player));
                    if (obj != null)
                    {
                        foreach (var minion in GetEnemyLaneMinionsTargetsInRange(E.Range))
                            if (minion.Health <= GameObjects.Player.GetSpellDamage(minion, SpellSlot.Q) && LagFree(3))
                            {
                                Q.CastIfHitchanceEquals(minion, HitChance.VeryHigh);
                            }
                    }
                }
            }
        }
        public static bool CanKill(AIBaseClient target, double subDmg)
        {
            return target.Health < subDmg;
        }
        private static void Cl3aringing()
        {
            bool useQ = LaneclearMenu.lQ.Enabled;
            bool useE = LaneclearMenu.lE.Enabled;
            {
                var target = TargetSelector.GetTarget(1200);
                if (Player.HasBuff("YasuoQ2"))
                {
                    if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
                    {
                        if (target != null && LagFree(1))
                        {
                            Q.Cast(target);
                        }
                    }
                }
                if (Q.IsReady())
                {
                    foreach (var minion in GetEnemyLaneMinionsTargetsInRange(E.Range))
                    {
                        if (useQ && minion.IsValidTarget(E.Range) && LagFree(3))
                        {
                            Q.Cast(minion);
                        }

                    }
                }
                foreach (var minion in GetEnemyLaneMinionsTargetsInRange(E.Range))
                {

                    if (minion.Health <= GameObjects.Player.GetSpellDamage(minion, SpellSlot.E) && LagFree(4))
                    {
                        if (useE)
                        {


                            if (minion.Distance(GameObjects.Player) > E.Range && LagFree(1))
                            {
                                E.Cast(minion);
                            }
                        }
                        if (useE && LagFree(2))
                        {
                            E.Cast(minion);
                        }

                    }
                }
            }
        }

        private static bool c4nc4stE(AIBaseClient target)
        {
            return !target.HasBuff("YasuoDashWrapper");
        }
        private static bool turretunder(Vector3 pos)
        {
            return
                ObjectManager.Get<AITurretClient>()
                                                    .Any(i => i.IsEnemy && !i.IsDead && i.Distance(pos) < 850 + Player.BoundingRadius);
        }
        private static AIBaseClient GetNearObj(AIBaseClient target = null, bool inQCir = false)
        {
            var pos = target != null
                          ? SpellPrediction.GetPrediction(target, E.Delay, 0, E.Speed).UnitPosition
                          : Game.CursorPosCenter;
            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));


            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            return
                obj.Where(
                    i =>
                    c4nc4stE(i) && pos.Distance(PosAfterE(i)) < (inQCir ? cirqwdthmin : Player.Distance(pos))
                    )
                      .MinOrDefault(i => pos.Distance(PosAfterE(i)));
        }
        private static Vector3 PosAfterE(AIBaseClient target)
        {
            return Player.Position.Extend(
                target.Position,
                Player.Distance(target) < 410 ? E.Range : Player.Distance(target) + 65);
        }

        private static void Combat()
        {
            var target = TargetSelector.GetTarget(1200);
            var targetd = TargetSelector.GetTarget(Q2.Range);
            var t = TargetSelector.GetTarget(R.Range);
            bool useQ = ComboYasuo.Q.Enabled;
            bool useW = ComboYasuo.W.Enabled;
            bool useE = ComboYasuo.E.Enabled;
            bool useR = ComboYasuo.R.Enabled;
            bool jumpmin = ComboYasuo.usemin.Active;
            if (jumpmin && E.IsReady())
            {
                if (q3h4ve && Q.IsReady(50))
                {
                    var targetz = TargetSelector.GetTarget(QRange, DamageType.Physical);
                    if (targetz != null)
                    {
                        var obj = GetNearObj(targetz, true);
                        if (obj != null && E.CastOnUnit(obj))
                        {
                            return;
                        }
                    }
                }
                if (ComboYasuo.cmbeg4p.Enabled && jumpmin)
                {
                    var targetz = TargetSelector.GetTarget(QRange, DamageType.Physical)
                                 ?? TargetSelector.GetTarget(Q2Range, DamageType.Physical);
                    if (targetz != null)
                    {
                        var obj = GetNearObj(targetz);
                        if (obj != null
                            && (obj.NetworkId != targetz.NetworkId
                                    ? Player.Distance(targetz) > ComboYasuo.cmboegprange.Value
                                    : !targetz.InAutoAttackRange())
                            && (!turretunder(PosAfterE(obj)) || ComboYasuo.cmboegptower.Active)
                            && E.CastOnUnit(obj))
                        {
                            return;
                        }
                    }
                }
            }
            if (E.IsReady() && target.IsValidTarget(E.Range) && ComboYasuo.usemin.Active == false)
            {


                if (target != null && LagFree(4))
                {
                    E.Cast(target);
                }
            }
            if (Player.HasBuff("YasuoRSpellPassive") && Q.IsReady() == false)
            {
                var etarget = TargetSelector.GetTarget(2000f);
                if (t == null)
                    return;
                bool oR = OtoR.autoRactive.Active;
                if (R.IsReady() && Q.IsReady() == false && oR && t.IsValidTarget(R.Range) && LagFree(1))
                {
                    R.Cast(t);
                }
            }
            if (Player.HasBuff("YasuoQ2"))
            {
                if (useQ && target.IsValidTarget(1000))
                {
                    if (target != null && LagFree(2))
                    {
                        Q.Cast(target);
                    }
                }
            }

            if (Q.IsReady() && useQ && target.IsValidTarget(E.Range))
            {
                if (target != null && LagFree(3))
                {
                    Q.Cast(target);
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
        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }
            if (Draws.DQ.Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range, Color.Crimson);
            }
            if (Player.HasBuff("YasuoQ2"))
            {
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Q2.Range, Color.Crimson);
                }
            }
            if (Draws.DW.Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.Crimson);
            }

            if (Draws.DE.Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range, Color.Blue);
            }
            if (Draws.DR.Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, R.Range, Color.Crimson);
            }
        }
    }
}
