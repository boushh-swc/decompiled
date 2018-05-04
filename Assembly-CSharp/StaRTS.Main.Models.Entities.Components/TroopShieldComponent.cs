using Net.RichardLord.Ash.Core;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TroopShieldComponent : ComponentBase
	{
		private bool activiated;

		private uint timerID;

		private uint coolDownInterval;

		private SmartEntity troop;

		public TroopShieldComponent(SmartEntity entity, uint cooldown)
		{
			this.troop = entity;
			this.coolDownInterval = cooldown * 1000u;
			this.Activiate();
		}

		public void CoolDownTimedOut(uint id, object cookie)
		{
			this.Activiate();
		}

		public void Activiate()
		{
			if (this.troop.HealthComp.IsDead())
			{
				return;
			}
			this.timerID = 0u;
			this.troop.TroopShieldHealthComp.Health = this.troop.TroopShieldHealthComp.MaxHealth;
			if (this.troop.TroopShieldViewComp != null)
			{
				this.troop.TroopShieldViewComp.PlayActivateAnimation();
				Service.EventManager.SendEvent(EventId.ChampionShieldActivated, null);
			}
			this.activiated = true;
		}

		public void Deactiviate()
		{
			if (this.troop.TroopShieldViewComp != null)
			{
				this.troop.TroopShieldViewComp.PlayDeactivateAnimation();
				Service.EventManager.SendEvent(EventId.ChampionShieldDestroyed, null);
			}
			this.activiated = false;
			this.troop.TroopShieldHealthComp.Health = 0;
			this.timerID = Service.SimTimerManager.CreateSimTimer(this.coolDownInterval, false, new TimerDelegate(this.CoolDownTimedOut), null);
		}

		public override void OnRemove()
		{
			TroopShieldViewComponent troopShieldViewComp = ((SmartEntity)this.Entity).TroopShieldViewComp;
			if (troopShieldViewComp != null)
			{
				troopShieldViewComp.PlayDeactivateAnimation();
				Service.EventManager.SendEvent(EventId.ChampionShieldDeactivated, null);
			}
			if (this.timerID != 0u)
			{
				Service.SimTimerManager.KillSimTimer(this.timerID);
			}
		}

		public bool IsActive()
		{
			return this.activiated;
		}
	}
}
