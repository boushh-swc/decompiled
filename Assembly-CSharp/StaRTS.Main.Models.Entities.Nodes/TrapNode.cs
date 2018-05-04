using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class TrapNode : Node<TrapNode>
	{
		public TrapComponent TrapComp
		{
			get;
			set;
		}

		public TrapViewComponent TrapViewComp
		{
			get;
			set;
		}

		public BuildingComponent BuildingComp
		{
			get;
			set;
		}

		public TransformComponent TransformComp
		{
			get;
			set;
		}

		public HealthComponent HealthComp
		{
			get;
			set;
		}
	}
}
