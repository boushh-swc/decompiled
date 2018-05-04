using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class TransportNode : Node<TransportNode>
	{
		public StateComponent State
		{
			get;
			set;
		}

		public TransportComponent Transport
		{
			get;
			set;
		}
	}
}
