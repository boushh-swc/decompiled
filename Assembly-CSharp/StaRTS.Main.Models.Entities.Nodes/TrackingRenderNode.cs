using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class TrackingRenderNode : Node<TrackingRenderNode>
	{
		public GameObjectViewComponent View
		{
			get;
			set;
		}

		public TrackingGameObjectViewComponent TrackingView
		{
			get;
			set;
		}

		public TrackingComponent TrackingComp
		{
			get;
			set;
		}
	}
}
