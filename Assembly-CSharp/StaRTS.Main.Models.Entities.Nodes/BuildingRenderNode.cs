using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class BuildingRenderNode : Node<BuildingRenderNode>
	{
		public StateComponent State
		{
			get;
			set;
		}

		public GameObjectViewComponent View
		{
			get;
			set;
		}

		public BuildingComponent BuildingComp
		{
			get;
			set;
		}

		public BuildingAnimationComponent AnimComp
		{
			get;
			set;
		}
	}
}
