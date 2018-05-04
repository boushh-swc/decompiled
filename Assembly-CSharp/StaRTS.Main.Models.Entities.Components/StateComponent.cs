using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Shared;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Entities.Components
{
	public class StateComponent : ComponentBase
	{
		public EntityState RawState;

		private Queue<EntityState> prevStates;

		public bool IsRunning
		{
			get;
			set;
		}

		public bool ForceUpdateAnimation
		{
			get;
			set;
		}

		public int DeathAnimationID
		{
			get;
			set;
		}

		public bool Dirty
		{
			get
			{
				return this.prevStates.Count != 0;
			}
		}

		public EntityState CurState
		{
			get
			{
				return this.RawState;
			}
			set
			{
				if (value != EntityState.Moving)
				{
					this.IsRunning = false;
				}
				if (this.RawState != value)
				{
					this.prevStates.Enqueue(this.RawState);
					this.RawState = value;
				}
			}
		}

		public StateComponent(Entity entity)
		{
			this.prevStates = new Queue<EntityState>();
			this.Entity = entity;
			this.Reset();
		}

		public void Reset()
		{
			this.RawState = EntityState.Idle;
			this.IsRunning = false;
			this.ForceUpdateAnimation = false;
			this.prevStates.Clear();
			this.DeathAnimationID = 2;
		}

		public EntityState DequeuePrevState()
		{
			return this.prevStates.Dequeue();
		}
	}
}
