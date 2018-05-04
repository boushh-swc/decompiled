using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class AreaTriggerBuildingNode : Node<AreaTriggerBuildingNode>
	{
		public TransformComponent TransformComp
		{
			get;
			set;
		}

		public BuildingComponent BuildingComp
		{
			get;
			set;
		}

		public AreaTriggerComponent AreaTriggerComp
		{
			get;
			set;
		}
	}
}
