using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class HealthViewNode : Node<HealthViewNode>
	{
		public HealthViewComponent HealthView
		{
			get;
			set;
		}
	}
}
