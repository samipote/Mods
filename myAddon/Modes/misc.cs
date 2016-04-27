using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using Settings = AddonTemplate.Config.Modes.Misc;
namespace AddonTemplate.Modes
{
	/// <summary>
	/// Description of misc.
	/// </summary>
	public sealed class Misc : ModeBase
	{
		private float QMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
		private float WMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
		private float RMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
		private float MyMana = Player.Instance.Mana;                                                            
		   public override bool ShouldBeExecuted()
        {
		   	return !Player.HasBuff("pyromania_particle");
        }
		   public override void Execute()
        {
		   	var target = TargetSelector.GetTarget(10000, DamageType.Magical);
		   	if (Settings.UseE && E.IsReady() && Mana() == true)
            {
            	E.Cast();
            }
		   	if (Settings.UseE && E.IsReady() && Player.Instance.IsInShopRange())
		   	{
		   		E.Cast();
		   	}
            if (Settings.AutoStacks && Player.Instance.IsInShopRange() && W.IsReady())
            {
            	W.Cast();
            }
            if (Settings.AutoStacks && target==null && W.IsReady() && Player.Instance.ManaPercent > 90 && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
            	W.Cast();
            }
        }
		   private bool Mana()
		   {
		   	if (Q.IsReady() && !W.IsReady() && !R.IsReady() && QMana + 20  > MyMana)
		   	{
		   		return true;
		   	}
		   	if (Q.IsReady() && W.IsReady() && !R.IsReady() && QMana + WMana + 20 > MyMana)
		   	{
		   		return true;
		   	}
		   	if (Q.IsReady() && W.IsReady() && R.IsReady() && QMana + WMana + RMana + 20 > MyMana)
		   	{
		   		return true;
		   	}
		   	return false;
		   }
	}
}
