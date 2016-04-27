using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;

// Using the config like this makes your life easier, trust me
using Settings = AddonTemplate.Config.Modes.Combo;
namespace AddonTemplate.Modes
{
    public sealed class Combo : ModeBase
    {
    	private	float Stacks = Player.GetBuff("pyromania").Count;
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            // TODO: Add combo logic here
            // See how I used the Settings.UseQ here, this is why I love my way of using
            // the menu in the Config class!
            if (Settings.UseQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                var predW  = W.GetPrediction(target).CastPosition;
                var predR  = R.GetPrediction(target).CastPosition;
                if (target != null && Player.Instance.Distance(target) < 600 && W.IsReady())
                {
                	W.Cast(predW);
                }
                else if (target != null)
                {
                	if (Player.Instance.Distance(target) < 600 && R.IsReady()  && ComboDmg(target) > target.Health  && Stacks > 2)
                	{
                		R.Cast(predR);
                	}
                }
            }
        }
        
        private float ComboDmg(Obj_AI_Base Target)
        {
        	if (Target != null)
        	{
        		return Player.Instance.GetSpellDamage(Target, SpellSlot.Q)+Player.Instance.GetSpellDamage(Target, SpellSlot.W)+Player.Instance.GetSpellDamage(Target, SpellSlot.R);
        	}
        	return 0;
        }
        private sattic int Cd(spell)
        {
        	if (Spell.IsReady())
        	{
        		return 1;
        	}
        return 0;
        }
    }
}
