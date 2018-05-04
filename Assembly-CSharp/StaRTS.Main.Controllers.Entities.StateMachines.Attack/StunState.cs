using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class StunState : AttackFSMState
	{
		public StunState(AttackFSM owner) : base(owner, 0u)
		{
		}

		public void SetDuration(int millisecondsToStun)
		{
			base.LockDuration = (uint)millisecondsToStun;
		}
	}
}
