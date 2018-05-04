using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TrapComponent : ComponentBase
	{
		public TrapTypeVO Type;

		private TrapState state;

		public TrapState PreviousState
		{
			get;
			private set;
		}

		public TrapState CurrentState
		{
			get
			{
				return this.state;
			}
			set
			{
				this.PreviousState = this.state;
				this.state = value;
			}
		}

		public TrapComponent(TrapTypeVO type, TrapState state)
		{
			this.Type = type;
			this.state = state;
			this.PreviousState = TrapState.Spent;
		}
	}
}
