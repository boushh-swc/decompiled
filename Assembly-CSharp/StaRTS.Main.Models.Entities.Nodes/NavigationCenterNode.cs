using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class NavigationCenterNode : Node<NavigationCenterNode>
	{
		public NavigationCenterComponent NavigationCenterComp
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
