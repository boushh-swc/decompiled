using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class ArmoryNode : Node<ArmoryNode>
	{
		public ArmoryComponent ArmoryComp
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
