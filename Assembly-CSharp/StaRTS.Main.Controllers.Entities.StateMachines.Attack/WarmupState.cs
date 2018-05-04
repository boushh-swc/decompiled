using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class WarmupState : AttackFSMState
	{
		public WarmupState(AttackFSM owner) : base(owner, owner.ShooterComp.ShooterVO.WarmupDelay)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			base.AttackFSMOwner.StateComponent.CurState = EntityState.WarmingUp;
			Service.EventManager.SendEvent(EventId.ShooterWarmingUp, base.AttackFSMOwner.Entity);
		}
	}
}
