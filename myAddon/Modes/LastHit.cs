using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;
using Settings = AddonTemplate.Config.Modes.LastHit;
namespace AddonTemplate.Modes
{
    public sealed class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on lasthit mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }
        public override void Execute()
        {
            if (Q.IsReady() && Settings.UseQ)
            {
            	var target = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range).Where(minion => minion.Health <= Player.Instance.GetSpellDamage(minion,SpellSlot.Q));
            	if (target == null)
            	{
            		return;
            	}
            	if (!Player.HasBuff("pyromania_particle"))
            	{
            	Q.Cast(target.First());
            	}
            }
        }
    }
}
