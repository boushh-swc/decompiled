using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class GeneratorViewNode : Node<GeneratorViewNode>
	{
		public GeneratorComponent GeneratorComp
		{
			get;
			set;
		}

		public GeneratorViewComponent GeneratorView
		{
			get;
			set;
		}

		public BuildingComponent BuildingComp
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
