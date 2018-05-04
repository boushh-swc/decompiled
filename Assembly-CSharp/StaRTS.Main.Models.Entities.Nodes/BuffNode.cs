using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class BuffNode : Node<BuffNode>
	{
		public BuffComponent BuffComp
		{
			get;
			set;
		}
	}
}
