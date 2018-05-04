using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class CooldownState : AttackFSMState
	{
		public CooldownState(AttackFSM owner) : base(owner, owner.ShooterComp.ShooterVO.CooldownDelay)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			base.AttackFSMOwner.StateComponent.CurState = EntityState.CoolingDown;
		}

		public override void OnExit(IState nextState)
		{
			if (base.AttackFSMOwner.ShooterComp.ShooterVO.ClipRetargeting)
			{
				Service.TargetingController.ReevaluateTarget(base.AttackFSMOwner.ShooterComp);
			}
			base.OnExit(nextState);
			Service.ShooterController.OnCooldownExit(base.ShooterComp);
		}
	}
}
