using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class SupportNode : Node<SupportNode>
	{
		public BuildingComponent BuildingComp
		{
			get;
			set;
		}

		public SupportComponent SupportComp
		{
			get;
			set;
		}
	}
}
