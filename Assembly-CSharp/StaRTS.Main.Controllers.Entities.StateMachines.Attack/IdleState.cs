using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class IdleState : AttackFSMState
	{
		public IdleState(AttackFSM owner) : base(owner, 0u)
		{
		}

		public override void OnEnter()
		{
			base.NextUnlockTime = 0u;
		}
	}
}
