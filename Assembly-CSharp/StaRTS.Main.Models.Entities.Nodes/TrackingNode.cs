using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class TrackingNode : Node<TrackingNode>
	{
		public TransformComponent Transform
		{
			get;
			set;
		}

		public TrackingComponent TrackingComp
		{
			get;
			set;
		}

		public ShooterComponent ShooterComp
		{
			get;
			set;
		}

		public BuildingComponent BuildingComp
		{
			get;
			set;
		}
	}
}
