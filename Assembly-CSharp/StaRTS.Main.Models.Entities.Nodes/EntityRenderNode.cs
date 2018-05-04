using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class EntityRenderNode : Node<EntityRenderNode>
	{
		public TransformComponent Transform
		{
			get;
			set;
		}

		public SizeComponent Size
		{
			get;
			set;
		}

		public PathingComponent Path
		{
			get;
			set;
		}

		public GameObjectViewComponent View
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
