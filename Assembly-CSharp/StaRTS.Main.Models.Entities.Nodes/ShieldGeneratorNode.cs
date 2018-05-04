using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class ShieldGeneratorNode : Node<ShieldGeneratorNode>
	{
		public BuildingComponent BuildingComp
		{
			get;
			set;
		}

		public ShieldGeneratorComponent ShieldGenComp
		{
			get;
			set;
		}

		public BoardItemComponent BoardItem
		{
			get;
			set;
		}

		public GameObjectViewComponent View
		{
			get;
			set;
		}
	}
}
