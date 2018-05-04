using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class PreFireState : AttackFSMState
	{
		public PreFireState(AttackFSM owner) : base(owner, owner.ShooterComp.ShooterVO.AnimationDelay)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			base.AttackFSMOwner.StateComponent.CurState = EntityState.Attacking;
		}

		public override void OnExit(IState nextState)
		{
			base.OnExit(nextState);
			base.AttackFSMOwner.Fire();
		}
	}
}
