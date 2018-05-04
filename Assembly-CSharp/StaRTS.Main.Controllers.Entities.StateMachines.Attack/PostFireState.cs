using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class PostFireState : AttackFSMState
	{
		public PostFireState(AttackFSM owner) : base(owner, owner.ShooterComp.ShooterVO.ShotDelay)
		{
		}

		public override void OnExit(IState nextState)
		{
			base.OnExit(nextState);
		}
	}
}
