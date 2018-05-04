using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class BuildingNode : Node<BuildingNode>
	{
		public TransformComponent Transform
		{
			get;
			set;
		}

		public HealthComponent HealthComp
		{
			get;
			set;
		}

		public BuildingComponent BuildingComp
		{
			get;
			set;
		}

		public TeamComponent TeamComp
		{
			get;
			set;
		}
	}
}
