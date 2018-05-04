using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines
{
	public abstract class TimeLockedState : IState
	{
		public TimeLockedStateMachine Owner
		{
			get;
			protected set;
		}

		public uint LockDuration
		{
			get;
			protected set;
		}

		public uint NextUnlockTime
		{
			get;
			protected set;
		}

		public TimeLockedState(TimeLockedStateMachine owner, uint lockDuration)
		{
			this.Owner = owner;
			this.SetDefaultLockDuration(lockDuration);
		}

		public void ResetLock()
		{
			this.ForceUnlock();
		}

		public virtual void OnEnter()
		{
			this.NextUnlockTime = this.Owner.Now + this.LockDuration;
		}

		public virtual void OnExit(IState nextState)
		{
			this.ResetLock();
		}

		public void SetDefaultLockDuration(uint lockDuration)
		{
			this.LockDuration = lockDuration;
		}

		public bool IsUnlocked()
		{
			return this.Owner.Now >= this.NextUnlockTime;
		}

		public void ForceUnlock()
		{
			this.NextUnlockTime = 0u;
		}
	}
}
