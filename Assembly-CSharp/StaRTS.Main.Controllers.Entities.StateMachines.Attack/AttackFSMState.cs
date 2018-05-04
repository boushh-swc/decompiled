using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public abstract class AttackFSMState : TimeLockedState
	{
		public AttackFSM AttackFSMOwner
		{
			get;
			protected set;
		}

		public ShooterComponent ShooterComp
		{
			get;
			protected set;
		}

		public AttackFSMState(AttackFSM owner, uint lockDuration) : base(owner, lockDuration)
		{
			this.AttackFSMOwner = owner;
			this.ShooterComp = owner.ShooterComp;
		}

		public override void OnEnter()
		{
			base.NextUnlockTime = base.Owner.Now + base.LockDuration;
		}
	}
}
