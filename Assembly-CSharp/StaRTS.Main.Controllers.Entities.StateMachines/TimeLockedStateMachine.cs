using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.Entities.StateMachines
{
	public class TimeLockedStateMachine : StateMachine, ISimTimeProvider
	{
		private ISimTimeProvider time;

		private TimeLockedState curTimeLockedState;

		private Type prevTimeLockedStateType;

		public uint Now
		{
			get
			{
				return this.time.Now;
			}
		}

		public ISimTimeProvider TimeProvider
		{
			get
			{
				return this.time;
			}
			set
			{
				this.time = value;
			}
		}

		public new TimeLockedState CurrentState
		{
			get
			{
				return this.curTimeLockedState;
			}
		}

		public new Type PreviousStateType
		{
			get
			{
				return this.prevTimeLockedStateType;
			}
		}

		public TimeLockedStateMachine(ISimTimeProvider timeProvider)
		{
			this.time = timeProvider;
		}

		public virtual bool SetState(TimeLockedState timeLockedState)
		{
			TimeLockedState timeLockedState2 = this.curTimeLockedState;
			if (base.SetState(timeLockedState))
			{
				this.prevTimeLockedStateType = ((timeLockedState2 != null) ? timeLockedState2.GetType() : null);
				this.curTimeLockedState = timeLockedState;
				return true;
			}
			return false;
		}
	}
}
