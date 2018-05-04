using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class SupportViewNode : Node<SupportViewNode>
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

		public SupportViewComponent SupportView
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
