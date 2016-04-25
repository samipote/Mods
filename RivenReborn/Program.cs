namespace RivenReborn
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Rendering;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    internal class Riven
    {
        public static Spell.Skillshot Q { get; set; }

        public static Spell.Targeted ignite { get; set; }

        public static Spell.Skillshot R2 { get; set; }

        public static Spell.Skillshot E { get; set; }

        public static Spell.Active W { get; set; }

        public static Spell.Skillshot Flash { get; set; }

        public static Spell.Active R1 { get; set; }

        public static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        public static Menu ComboMenu { get; private set; }

        public static Menu DelayMenu { get; private set; }

        public static Menu HarassMenu { get; private set; }

        public static Menu BurstMenu { get; private set; }

        public static Menu LaneMenu { get; private set; }

        public static Menu JungleMenu { get; private set; }

        public static Menu MiscMenu { get; private set; }

        public static Menu FleeMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        public static bool CastR2;

        private static Menu Menu;

        private static float LastQ;


        private static int QStack = 0;


        private static bool forceQ;

        private static bool forceW;

        private static bool forceR;

        private static bool forceR2;


        private const string IsFirstR = "RivenFengShuiEngine";

        private const string IsSecondR = "RivenIzunaBlade";

        private static AttackableUnit QTarget;


        public static int ComboBox(Menu m, string s)
        {
            return m[s].Cast<ComboBox>().SelectedIndex;
        }


        private static AIHeroClient R2Target;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.BaseSkinName != "Riven")
            {
                return;
            }

            Menu = MainMenu.AddMenu("Riven Reborn", "RivenReborn");
            Menu.AddGroupLabel("Riven Reborn Revamped Rewritten ReKappa!");


            ComboMenu = Menu.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddLabel("Sick Burst combo try it !");
            ComboMenu.Add("ComboW", new CheckBox("use W in Combo"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("RForce", new KeyBind("R Force Key", false, KeyBind.BindTypes.PressToggle, 'G'));
            ComboMenu.Add("UseRType", new ComboBox("Use R2 :", 1, "Killable", "Max Damage", "Instant Cast", "Disable"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("ComboE", new CheckBox("use E in Combo"));
            ComboMenu.AddLabel("Q Delays : ");
            ComboMenu.AddSeparator();
            ComboMenu.Add("q1delay", new Slider("Q1 animation delay in ms default 293", 291, 0, 500));
            ComboMenu.Add("q2delay", new Slider("Q2 animation delay in ms default 293", 291, 0, 500));
            ComboMenu.Add("q3delay", new Slider("Q3 animation delay in ms default 393", 393, 0, 500));
            ComboMenu.Add("wdelay", new Slider("W animation delay in ms default 170", 170, 0, 500));
            ComboMenu.AddSeparator();
            ComboMenu.AddSeparator();
            ComboMenu.Add("manualcancel", new CheckBox("Cancel animation from manual Qs"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("UseItems", new CheckBox("Use Items"));


            BurstMenu = Menu.AddSubMenu("Burst");
            BurstMenu.AddGroupLabel("Burst Settingsz");
            BurstMenu.Add("burstcombo", new KeyBind("Activate Burst", false, KeyBind.BindTypes.HoldActive, 'T'));
            BurstMenu.AddSeparator();
            BurstMenu.AddLabel("Please Make sure you have Force R enable or it will not use R in burst (will fix)");

            HarassMenu = Menu.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Qharass", new CheckBox("Use Q"));
            HarassMenu.Add("Wharass", new CheckBox("Use W"));
            HarassMenu.AddLabel("It will use E away from enemy");
            HarassMenu.AddSeparator();



            LaneMenu = Menu.AddSubMenu("Farm");
            LaneMenu.AddGroupLabel("LaneClear Settings");
            LaneMenu.Add("LaneQ", new CheckBox("Use Q in Laneclear"));
            LaneMenu.Add("LaneW", new CheckBox("Use Q in Laneclear"));
            LaneMenu.Add("LaneE", new CheckBox("Use E in Laneclear"));
            LaneMenu.Add("Lanemin", new Slider("Use W if hit {0} minions", 3, 1, 5));
            LaneMenu.AddSeparator();


            JungleMenu = Menu.AddSubMenu("Jungle");
            JungleMenu.AddGroupLabel("Jungle Clear");
            JungleMenu.Add("jungleQ", new CheckBox("Use Q"));
            JungleMenu.Add("jungleW", new CheckBox("Use W"));
            JungleMenu.Add("jungleE", new CheckBox("Use E"));
            JungleMenu.AddSeparator();


            MiscMenu = Menu.AddSubMenu("Misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("KillStealQ", new CheckBox("Use Q KS"));
            MiscMenu.Add("KillStealW", new CheckBox("Use W KS"));
            MiscMenu.Add("KillStealE", new CheckBox("Use E KS"));
            ComboMenu.AddLabel("Killsteal with R is disable (was causing random r2 behind enemy cast)");
            MiscMenu.Add("SaveW", new CheckBox("Dont W if target killable with AA", false));
            MiscMenu.Add("AutoW", new Slider("Auto W When X Enemy", 5, 0, 5));
            MiscMenu.Add("AutoShield", new CheckBox("Auto E")); ;
            MiscMenu.Add("Winterrupt", new CheckBox("W interrupt"));
            MiscMenu.Add("gapcloser", new CheckBox("Stun on enemy gapcloser"));
            MiscMenu.AddSeparator();

            FleeMenu = Menu.AddSubMenu("Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.AddGroupLabel("Flee");
            FleeMenu.Add("qflee", new CheckBox("Use Q"));
            FleeMenu.Add("wflee", new CheckBox("Use W on enemy"));
            FleeMenu.Add("eflee", new CheckBox("Use E"));
            FleeMenu.Add("useitemf", new CheckBox("Use Yoummu"));

            FleeMenu.AddSeparator();

            DrawMenu = Menu.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.Add("DrawAlwaysR", new CheckBox("Draw R Status"));
            DrawMenu.Add("damagein", new CheckBox("Draw HP bar damage"));
            DrawMenu.Add("ER", new CheckBox("Draw Combo Engage Range"));
            DrawMenu.Add("BER", new CheckBox("Draw Burst Engage Range"));

            Q = new Spell.Skillshot(SpellSlot.Q, 220, SkillShotType.Circular, 250, 2200, 100);
            W = new Spell.Active(SpellSlot.W, 252);
            E = new Spell.Skillshot(SpellSlot.E, 465, SkillShotType.Linear);
            R1 = new Spell.Active(SpellSlot.R, (uint)myHero.GetAutoAttackRange());
            R2 = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 125)
            {
                AllowedCollisionCount = int.MaxValue
            };

            var slot = Player.Instance.GetSpellSlotFromName("summonerflash");

            if (slot != SpellSlot.Unknown)
            {
                Flash = new Spell.Skillshot(slot, 680, SkillShotType.Linear);
            }

            var ign = Player.Spells.FirstOrDefault(o => o.SData.Name == "SummonerDot");


            if (ign != null)
            {
                SpellSlot igslot = EloBuddy.SDK.Extensions.GetSpellSlotFromName(myHero, "SummonerDot");

                ignite = new Spell.Targeted(igslot, 600);
            }

            Game.OnTick += OnTick;
            Obj_AI_Base.OnSpellCast += AfterAAQLogic;
            Obj_AI_Base.OnPlayAnimation += OnPlay;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Orbwalker.OnPostAttack += JungleClearELogic;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;


        }


        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == myHero.Type && MiscMenu["AutoShield"].Cast<CheckBox>().CurrentValue)
            {
                var epos = myHero.ServerPosition + (myHero.ServerPosition - sender.ServerPosition).Normalized() * 300;

                if (myHero.Distance(sender.ServerPosition) <= args.SData.CastRange)
                {
                    switch (args.SData.TargettingType)
                    {
                        case SpellDataTargetType.Unit:

                            if (args.Target.NetworkId == myHero.NetworkId)
                            {
                                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !args.SData.Name.Contains("NasusW"))
                                {
                                    if (E.IsReady()) E.Cast(epos);
                                }
                            }

                            break;
                        case SpellDataTargetType.SelfAoe:

                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                            {
                                if (E.IsReady()) E.Cast(epos);
                            }

                            break;
                    }
                    if (args.SData.Name.Contains("IreliaEquilibriumStrike"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady() && W.IsInRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TalonCutthroat"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RenektonPreExecute"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("GarenRPreCast"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }

                    if (args.SData.Name.Contains("GarenQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }

                    if (args.SData.Name.Contains("XenZhaoThrust3"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarQ"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDash"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDashAADummy"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TwitchEParticle"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("FizzPiercingStrike"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("HungeringStrike"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaRTrigger"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady() && W.IsInRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaE"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingSpinToWin"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                            else if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == myHero.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                }
            }
        }

        private static int lastQ;
        private static int lastQDelay;
        public static int QNum = 0;
        private static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (myHero.IsDead) return;
            if (!sender.IsMe) return;
            int delay = 0;
            switch (args.Animation)
            {
                case "Spell1a":
                    delay = ComboMenu["Q1Delay"].Cast<Slider>().CurrentValue;
                    lastQ = Core.GameTickCount;
                    QNum = 1;
                    break;
                case "Spell1b":
                    delay = ComboMenu["Q2Delay"].Cast<Slider>().CurrentValue;
                    lastQ = Core.GameTickCount;
                    QNum = 2;
                    break;
                case "Spell1c":
                    delay = ComboMenu["Q3Delay"].Cast<Slider>().CurrentValue;
                    lastQ = Core.GameTickCount;
                    QNum = 3;
                    break;
                case "Dance":
                    if (lastQ > Core.GameTickCount - 500)
                    {

                        //Orbwalker.ResetAutoAttack();
                        //Utils.Debug("reset");
                    }

                    break;
            }

            if (delay != 0 && (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None || ComboMenu["manualcancel"].Cast<CheckBox>().CurrentValue))
            {
                lastQDelay = delay;
                Orbwalker.ResetAutoAttack();
                Core.DelayAction(DanceIfNotAborted, delay - Game.Ping);
            }


        }

        private static void DanceIfNotAborted()
        {
            Player.DoEmote(Emote.Dance);

        }

        private static bool InWRange(GameObject target)
        {
            if (target == null || !target.IsValid) return false;
            return (myHero.HasBuff("RivenFengShuiEngine"))
            ? 330 >= myHero.Distance(target.Position)
            : 265 >= myHero.Distance(target.Position);

        }

        private static void Interrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsEnemy && W.IsReady() && sender.IsValidTarget() && !sender.IsZombie && ComboMenu["Winterrupt"].Cast<CheckBox>().CurrentValue)
            {
                if (sender.IsValidTarget(125 + myHero.BoundingRadius + sender.BoundingRadius)) W.Cast();
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (myHero.IsDead || !sender.IsEnemy || !sender.IsValidTarget(W.Range) || !W.IsReady() || !MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue) return;

            W.Cast();
        }

        private static int lastAA;
        private static AIHeroClient ComboTarget;
        private static void AfterAAQLogic(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            var t = args.Target;

            if (t == null)
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Q.IsReady())
                {
                    if (t is AIHeroClient)
                        Q.Cast(t.Position);
                }
            }

            if (BurstMenu["burstcombo"].Cast<KeyBind>().CurrentValue)
            {
                if (Q.IsReady())
                {
                    if (t is AIHeroClient)
                        Q.Cast(t.Position);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (Q.IsReady())
                {
                    if (HarassMenu["Qharass"].Cast<CheckBox>().CurrentValue)
                        if (t is AIHeroClient)
                            Q.Cast(t.Position);
                }
            }



            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (t is Obj_AI_Minion)
                {
                    if (Q.IsReady())
                    {
                        if (LaneMenu["LaneQ"].Cast<CheckBox>().CurrentValue)
                        {
                            foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, E.Range))
                            {
                                Q.Cast(minion);
                            }
                        }
                        if (JungleMenu["jungleQ"].Cast<CheckBox>().CurrentValue)
                        {
                            foreach (var camp in EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, E.Range))
                            {
                                Q.Cast(camp);
                            }
                        }
                    }
                }
            }
        }


        internal static void UseItems2(Obj_AI_Base target)
        {
            var RivenServerPosition = myHero.ServerPosition.To2D();
            var targetServerPosition = target.ServerPosition.To2D();

            if (Item.CanUseItem(ItemId.Youmuus_Ghostblade) && myHero.GetAutoAttackRange() > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Youmuus_Ghostblade);
            }
        }


        internal static void UseItems(Obj_AI_Base target)
        {
            var RivenServerPosition = myHero.ServerPosition.To2D();
            var targetServerPosition = target.ServerPosition.To2D();

            if (Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
            }
            if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Tiamat_Melee_Only);
            }
            if (Item.CanUseItem(ItemId.Titanic_Hydra) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Titanic_Hydra);
            }
            if (Item.CanUseItem(ItemId.Blade_of_the_Ruined_King) && 550 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Blade_of_the_Ruined_King);
            }

            if (Item.CanUseItem(ItemId.Bilgewater_Cutlass) && 550 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Bilgewater_Cutlass);
            }
        }

        private static int TickLimiter = 1;
        private static int LastGameTick = 0;
        private static void OnTick(EventArgs args)
        {

            if (lastQ + 3650 < Core.GameTickCount)


                Killsteal();
            AutoUseW();
            ;
            if (BurstMenu["burstcombo"].Cast<KeyBind>().CurrentValue) Burst();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
                CastYoumoo();
            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Flee();


        }

        private static void Combo()
        {

            if (ComboMenu["ComboW"].Cast<CheckBox>().CurrentValue)
            {
                var t = EntityManager.Heroes.Enemies.Find(x => x.IsValidTarget(W.Range) && !x.HasBuffOfType(BuffType.SpellShield));

                if (t != null)
                {
                    if (W.IsReady() && !Orbwalker.CanAutoAttack)


                    {

                        W.Cast();
                    }

                    UseItems(t);
                    UseItems2(t);

                }
            }

            if (E.IsReady())
            {
                var t = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range + myHero.GetAutoAttackRange()));

                if (t == null)
                {
                    return;
                }

                if (ComboMenu["ComboE"].Cast<CheckBox>().CurrentValue)
                {
                    var t1 = t.OrderByDescending(e => TargetSelector.GetPriority(e)).FirstOrDefault();

                    if (t1 != null)
                        E.Cast(t1.ServerPosition);
                }

            }

            {

                if (ComboMenu["RForce"].Cast<KeyBind>().CurrentValue)
                {
                    if (R1.IsReady())
                    {
                        if (ComboMenu["RForce"].Cast<KeyBind>().CurrentValue && !myHero.HasBuff("RivenFengShuiEngine"))
                        {
                            var t = TargetSelector.GetTarget(700, DamageType.Physical);
                            if (t == null)
                            {
                                return;
                            }
                            if (t.Distance(myHero.ServerPosition) < E.Range + myHero.AttackRange && myHero.CountEnemiesInRange(500) >= 1)
                                R1.Cast();
                        }

                        if (myHero.HasBuff("RivenFengShuiEngine"))
                        {
                            var t = TargetSelector.GetTarget(900, DamageType.Physical);
                            if (t == null)
                            {
                                return;
                            }

                            {

                                if (ComboBox(ComboMenu, "UseRType") == 0)
                                {
                                    var target = TargetSelector.SelectedTarget;
                                    if (DamageIndicators.Rdmg(t, t.Health) > t.Health && t.IsValidTarget(R2.Range) && t.Distance(myHero.ServerPosition) < 600)
                                    {
                                        R2.Cast(t.ServerPosition);
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                }
                                else if (ComboBox(ComboMenu, "UseRType") == 1)
                                {
                                    var prediction = R2.GetPrediction(t);
                                    if (t.HealthPercent < 50 && t.Health > DamageIndicators.Rdmg(t, t.Health) + Damage.GetAutoAttackDamage(myHero, t) * 2)
                                    {
                                        R2.Cast(t.ServerPosition);
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                }

                                else if (ComboBox(ComboMenu, "UseRType") == 2)
                                {
                                    if (t.IsValidTarget(R2.Range) && t.Distance(myHero.ServerPosition) < 200)
                                    {
                                        R2.Cast(t.ServerPosition);
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                }
                                else if (ComboBox(ComboMenu, "UseRType") == 3)
                                {
                                    CastR2 = false;
                                }
                            }

                            if (CastR2)
                            {
                                R2.Cast(t);
                            }
                        }
                    }
                }
            }
        }
        private static void AutoUseW()
        {
            if (MiscMenu["AutoW"].Cast<Slider>().CurrentValue > 0)
            {
                if (myHero.CountEnemiesInRange(W.Range) >= MiscMenu["AutoW"].Cast<Slider>().CurrentValue)
                {
                    W.Cast();
                }
            }
        }

        private static void Harass()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (t != null && t.IsValidTarget())
            {
                if (QStack == 2)
                {
                    if (E.IsReady())
                    {
                        E.Cast(myHero.ServerPosition + (myHero.ServerPosition - t.ServerPosition).Normalized() * E.Range);
                    }

                    if (!E.IsReady())
                    {
                        Q.Cast(myHero.ServerPosition + (myHero.ServerPosition - t.ServerPosition).Normalized() * E.Range);
                    }
                }

                if (W.IsReady())
                {
                    if (t.IsValidTarget(W.Range) && QStack == 1)
                    {
                        W.Cast();

                    }
                }

                if (Q.IsReady())
                {
                    if (QStack == 0)
                    {
                        if (t.IsValidTarget(myHero.AttackRange + myHero.BoundingRadius + 150))
                        {
                            Q.Cast(t.Position);
                        }
                    }
                }
            }
        }


        private static void Burst()
        {
            var target = TargetSelector.SelectedTarget;
            Orbwalker.ForcedTarget = target;
            Orbwalker.OrbwalkTo(target.ServerPosition);
            if (target == null || target.IsZombie || target.IsInvulnerable) return;
            if (target.IsValidTarget(800))

            {
                if (E.IsReady())
                {
                    UseItems2(target);
                    Player.CastSpell(SpellSlot.E, target.ServerPosition);
                }
                UseItems2(target);

                if (R1.IsReady() && BurstMenu["burstcombo"].Cast<KeyBind>().CurrentValue && forceR == false)
                {
                    R1.Cast();
                }

                if (Flash.IsReady() && (myHero.Distance(target.Position) <= 680))
                {
                    Flash.Cast(target.ServerPosition);
                }

                UseItems(target);

                if (target.IsValidTarget(W.Range))
                {
                    if (W.IsReady())

                    {
                        W.Cast();
                    }

                    if (R2.IsReady())

                    {
                        R2.Cast(target.ServerPosition);
                    }

                }
            }
        }

        private static void LaneClear()
        {
            if (LaneMenu["LaneW"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady())
                {
                    var WMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, W.Range).ToList();

                    if (WMinions != null)
                        if (WMinions.FirstOrDefault().IsValidTarget(W.Range))
                            if (WMinions.Count >= 3)
                                W.Cast();
                }

                if (LaneMenu["LaneE"].Cast<CheckBox>().CurrentValue)
                {
                    var Mob = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.ServerPosition, E.Range).ToList();

                    if (Mob.FirstOrDefault().IsValidTarget(E.Range))
                    {
                        if (Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun) && !W.IsReady())
                        {
                            E.Cast(Game.CursorPos);
                        }
                        else if (!Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun))
                        {
                            E.Cast(Game.CursorPos);
                        }
                    }
                }
            }
        }

        private static void Flee()
        {
            var enemy =
                EntityManager.Heroes.Enemies.Where(
                    hero =>
                        hero.IsValidTarget(WRange) && W.IsReady());
            var x = myHero.Position.Extend(Game.CursorPos, 300);
            if (W.IsReady() && FleeMenu["wflee"].Cast<CheckBox>().CurrentValue && enemy.Any()) W.Cast();
            if (Q.IsReady() && FleeMenu["qflee"].Cast<CheckBox>().CurrentValue && !myHero.IsDashing()) Player.CastSpell(SpellSlot.Q, Game.CursorPos);
            if (E.IsReady() && FleeMenu["eflee"].Cast<CheckBox>().CurrentValue && !myHero.IsDashing()) Player.CastSpell(SpellSlot.E, x.To3D());
        }

        public static uint WRange
        {
            get
            {
                return (uint)
                        (70 + ObjectManager.Player.BoundingRadius +
                         (ObjectManager.Player.HasBuff("RivenFengShuiEngine") ? 195 : 120));
            }
        }
        private static void Killsteal()
        {
            foreach (var e in EntityManager.Heroes.Enemies.Where(e => !e.IsZombie && !e.HasBuff("KindredrNoDeathBuff") && !e.HasBuff("Undying Rage") && !e.HasBuff("JudicatorIntervention") && e.IsValidTarget()))
            {
                if (Q.IsReady() && MiscMenu["KillStealQ"].Cast<CheckBox>().CurrentValue)
                {
                    if (myHero.HasBuff("RivenFengShuiEngine"))
                    {
                        if (e.Distance(myHero.ServerPosition) < myHero.AttackRange + myHero.BoundingRadius + 50 && myHero.GetSpellDamage(e, SpellSlot.Q) > e.Health + e.HPRegenRate)
                            Q.Cast(e.Position);
                    }
                    else if (!myHero.HasBuff("RivenFengShuiEngine"))
                    {
                        if (e.Distance(myHero.ServerPosition) < myHero.AttackRange + myHero.BoundingRadius && myHero.GetSpellDamage(e, SpellSlot.Q) > e.Health + e.HPRegenRate)
                            Q.Cast(e.Position);
                    }
                }

                if (MiscMenu["KillStealW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    var targets = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(R2.Range) && !x.IsZombie);
                    foreach (var target in targets)
                    {
                        if (target.Health < myHero.GetSpellDamage(target, SpellSlot.W) && InWRange(target))
                            W.Cast();
                    }


                }
            }
        }

        private static void ForceR()
        {
            forceR = (R1.IsReady() && R1.Name == IsFirstR);
            Core.DelayAction(() => forceR = false, 700);
        }

        private static void ForceR2()
        {
            forceR2 = R2.IsReady() && R2.Name == IsSecondR;
            Core.DelayAction(() => forceR2 = false, 500);
        }


        private static void JungleClear()
        {
            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, E.Range).OrderBy(x => x.MaxHealth).ToList();

            if (JungleMenu["jungleW"].Cast<CheckBox>().CurrentValue)
            {
                if (Mob != null)
                    if (Mob.FirstOrDefault().IsValidTarget(W.Range))
                        W.Cast();

            }
        }

        private static void JungleClearELogic(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (target is Obj_AI_Minion)
                {
                    if (JungleMenu["jungleE"].Cast<CheckBox>().CurrentValue)
                    {
                        var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.ServerPosition, E.Range).OrderBy(x => x.MaxHealth).ToList();

                        if (Mob.FirstOrDefault().IsValidTarget(E.Range))
                        {
                            if (Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun) && !W.IsReady())
                            {
                                E.Cast(Game.CursorPos);
                            }
                            else if (!Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun))
                            {
                                E.Cast(Game.CursorPos);
                            }
                        }
                    }
                }
            }
        }

        private static void CastYoumoo()
        {
            var youmu = ObjectManager.Player.InventoryItems.FirstOrDefault(it => it.Id == ItemId.Youmuus_Ghostblade);

            if (youmu != null && youmu.CanUseItem() && FleeMenu["useitemf"].Cast<CheckBox>().CurrentValue) youmu.Cast();
        }


        private static float Cooldown(SpellDataInst spell)
        {
            return Player.Spells[0].CooldownExpires - Game.Time;
        }

        private static void ForceSkill()
        {
            if (QTarget == null || !QTarget.IsValidTarget()) return;
            if (forceR && R1.Name == IsFirstR)
            {
                Player.CastSpell(SpellSlot.R);
                return;
            }
            if (forceQ && QTarget != null && QTarget.IsValidTarget(E.Range + myHero.BoundingRadius + 70) && Q.IsReady())
                Player.CastSpell(SpellSlot.Q, ((Obj_AI_Base)QTarget).ServerPosition);
            if (forceW) W.Cast();

            if (forceR2 && R2.Name == IsSecondR)
            {
                var target = TargetSelector.SelectedTarget;

                if (target == null || !target.IsValidTarget()) target = TargetSelector.GetTarget(450 + myHero.AttackRange + 170, DamageType.Physical);
                if (target == null || !target.IsValidTarget()) return;
                R2.Cast(TargetSelector.SelectedTarget);
            }
        }


        public static AIHeroClient Qtarget;
        private static void ForceQ()
        {
            if (Q.IsReady())
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
        }

        private static double RDamage(Obj_AI_Base target)
        {

            if (target != null && R2.IsReady())
            {
                float missinghealth = (target.MaxHealth - target.Health) / target.MaxHealth > 0.75f
                    ? 0.75f
                    : (target.MaxHealth - target.Health) / target.MaxHealth;
                float pluspercent = missinghealth * (2.666667F);
                float rawdmg = new float[] { 80, 120, 160 }[R2.Level - 1] + 0.6f * myHero.FlatPhysicalDamageMod;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            return 0;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (myHero.IsDead)
                return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var green = Color.LimeGreen;
            var red = Color.IndianRed;
            if (DrawMenu["ER"].Cast<CheckBox>().CurrentValue)
                Circle.Draw(E.IsReady() ? green : red, 250 + myHero.AttackRange + 70, ObjectManager.Player.Position);
            if (DrawMenu["BER"].Cast<CheckBox>().CurrentValue && Flash != null && Flash.Slot != SpellSlot.Unknown)
                Circle.Draw(R1.IsReady() ? green : red, 800, ObjectManager.Player.Position);

            if (DrawMenu["DrawAlwaysR"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawText(heropos.X - 40, heropos.Y + 20, System.Drawing.Color.FloralWhite, "ForceR");
                Drawing.DrawText(heropos.X + 10, heropos.Y + 20,
                    ComboMenu["RForce"].Cast<KeyBind>().CurrentValue ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red,
                    ComboMenu["RForce"].Cast<KeyBind>().CurrentValue ? "On" : "Off");
            }

        }
        private static readonly float _barLength = 104;
        private static readonly float _xOffset = 2;
        private static readonly float _yOffset = 9;
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (myHero.IsDead)
                return;
            if (!DrawMenu["damagein"].Cast<CheckBox>().CurrentValue) return;
            foreach (var aiHeroClient in EntityManager.Heroes.Enemies)
            {
                if (!aiHeroClient.IsHPBarRendered || !aiHeroClient.VisibleOnScreen) continue;

                var pos = new Vector2(aiHeroClient.HPBarPosition.X + _xOffset, aiHeroClient.HPBarPosition.Y + _yOffset);
                var fullbar = (_barLength) * (aiHeroClient.HealthPercent / 100);
                var damage = (_barLength) *
                                 ((DamageIndicators.getComboDamage(aiHeroClient) / aiHeroClient.MaxHealth) > 1
                                     ? 1
                                     : (DamageIndicators.getComboDamage(aiHeroClient) / aiHeroClient.MaxHealth));
                Line.DrawLine(System.Drawing.Color.DarkRed, 9f, new Vector2(pos.X, pos.Y),
                    new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));
                Line.DrawLine(System.Drawing.Color.Black, 9, new Vector2(pos.X + (damage > fullbar ? fullbar : damage) - 2, pos.Y), new Vector2(pos.X + (damage > fullbar ? fullbar : damage) + 2, pos.Y));
            }
        }
    }
}