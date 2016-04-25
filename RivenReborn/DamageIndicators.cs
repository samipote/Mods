using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using EloBuddy.SDK.Rendering;
using RivenReborn;

namespace RivenReborn
{
    internal class DamageIndicators
    {


        public static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                float passivenhan;
                if (Riven.myHero.Level >= 18)
                {
                    passivenhan = 0.5f;
                }
                else if (Riven.myHero.Level >= 15)
                {
                    passivenhan = 0.45f;
                }
                else if (Riven.myHero.Level >= 12)
                {
                    passivenhan = 0.4f;
                }
                else if (Riven.myHero.Level >= 9)
                {
                    passivenhan = 0.35f;
                }
                else if (Riven.myHero.Level >= 6)
                {
                    passivenhan = 0.3f;
                }
                else if (Riven.myHero.Level >= 3)
                {
                    passivenhan = 0.25f;
                }
                else
                {
                    passivenhan = 0.2f;
                }
                if (Riven.W.IsReady()) damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.W);
                if (Riven.Q.IsReady())
                {
                    var qnhan = 4 - Riven.QNum;
                    damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q) * qnhan +
                             Riven.myHero.GetAutoAttackDamage(enemy) * qnhan * (1 + passivenhan);
                }
                damage = damage + Riven.myHero.GetAutoAttackDamage(enemy) * (1 + passivenhan);
                if (Riven.R1.IsReady())
                {
                    return damage * 1.2f + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.R);
                }

                return damage;
            }
            return 0;
        }

        private static double basicdmg(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan;
                if (Riven.myHero.Level >= 18)
                {
                    passivenhan = 0.5;
                }
                else if (Riven.myHero.Level >= 15)
                {
                    passivenhan = 0.45;
                }
                else if (Riven.myHero.Level >= 12)
                {
                    passivenhan = 0.4;
                }
                else if (Riven.myHero.Level >= 9)
                {
                    passivenhan = 0.35;
                }
                else if (Riven.myHero.Level >= 6)
                {
                    passivenhan = 0.3;
                }
                else if (Riven.myHero.Level >= 3)
                {
                    passivenhan = 0.25;
                }
                else
                {
                    passivenhan = 0.2;
                }

                if (Riven.W.IsReady()) dmg = dmg + Riven.myHero.GetSpellDamage(target, SpellSlot.W);
                if (Riven.Q.IsReady())
                {
                    var qnhan = 4 - Riven.QNum;

                    dmg = dmg + ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) * qnhan + Riven.myHero.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Riven.myHero.GetAutoAttackDamage(target) * (1 + passivenhan);
                return dmg;
            }
            return 0;
        }



        public static double Rdmg(Obj_AI_Base target, double health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75 ? 0.75 : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * (8 / 3);
                var rawdmg = new double[] { 80, 120, 160 }[Riven.R1.Level - 1] + 0.6 * Riven.myHero.FlatPhysicalDamageMod;
                return Riven.myHero.CalculateDamageOnUnit(target, DamageType.Physical, (float)(rawdmg * (1 + pluspercent)));
            }
            return 0;
        }

        public static double totaldame(Obj_AI_Base target)
        {
            if (target != null)
            {
                float dmg = 0;
                float passivenhan;
                if (Riven.myHero.Level >= 18)
                {
                    passivenhan = 0.5f;
                }
                else if (Riven.myHero.Level >= 15)
                {
                    passivenhan = 0.45f;
                }
                else if (Riven.myHero.Level >= 12)
                {
                    passivenhan = 0.4f;
                }
                else if (Riven.myHero.Level >= 9)
                {
                    passivenhan = 0.35f;
                }
                else if (Riven.myHero.Level >= 6)
                {
                    passivenhan = 0.3f;
                }
                else if (Riven.myHero.Level >= 3)
                {
                    passivenhan = 0.25f;
                }
                else
                {
                    passivenhan = 0.2f;
                }
                if (Riven.W.IsReady()) dmg = dmg + Riven.myHero.GetSpellDamage(target, SpellSlot.W);
                if (Riven.Q.IsReady())
                {
                    var qnhan = 4 - Riven.QNum;
                    dmg = dmg + ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) * qnhan + Riven.myHero.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Riven.myHero.GetAutoAttackDamage(target) * (1 + passivenhan);
                if (Riven.R1.IsReady())
                {
                    var rdmg = Rdmg(target, target.Health - dmg * 1.2f);
                    return dmg * 1.2 + rdmg;
                }
                return dmg;
            }
            return 0;
        }
        private static double RDamage(Obj_AI_Base target)
        {

            if (target != null && Riven.R1.IsReady())
            {
                float missinghealth = (target.MaxHealth - target.Health) / target.MaxHealth > 0.75f
                    ? 0.75f
                    : (target.MaxHealth - target.Health) / target.MaxHealth;
                float pluspercent = missinghealth * (2.666667F); // 8/3
                float rawdmg = new float[] { 80, 120, 160 }[Riven.R1.Level - 1] + 0.6f * Riven.myHero.FlatPhysicalDamageMod;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            return 0;
        }
    }
}