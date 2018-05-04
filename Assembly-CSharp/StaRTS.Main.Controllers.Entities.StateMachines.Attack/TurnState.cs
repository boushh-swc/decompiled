using StaRTS.Main.Models.Entities.Shared;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class TurnState : AttackFSMState
	{
		public TurnState(AttackFSM owner) : base(owner, 2147483647u)
		{
		}

		public uint GetDuration()
		{
			return base.LockDuration;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			base.AttackFSMOwner.StateComponent.CurState = EntityState.Turning;
		}
	}
}
