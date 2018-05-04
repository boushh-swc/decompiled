using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class MovementNode : Node<MovementNode>
	{
		public BoardItemComponent BoardItemComp
		{
			get;
			set;
		}

		public PathingComponent Path
		{
			get;
			set;
		}

		public TransformComponent Transform
		{
			get;
			set;
		}

		public StateComponent State
		{
			get;
			set;
		}
	}
}
