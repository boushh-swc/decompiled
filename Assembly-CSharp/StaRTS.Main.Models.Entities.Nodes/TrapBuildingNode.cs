using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class TrapBuildingNode : Node<TrapBuildingNode>
	{
		public TrapComponent TrapComp
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
