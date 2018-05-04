using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class TrackingRenderSystem : ViewSystemBase
	{
		private NodeList<TrackingRenderNode> nodeList;

		public override void AddToGame(Game game)
		{
			this.nodeList = Service.EntityController.GetNodeList<TrackingRenderNode>();
		}

		public override void RemoveFromGame(Game game)
		{
			this.nodeList = null;
		}

		protected override void Update(float dt)
		{
			for (TrackingRenderNode trackingRenderNode = this.nodeList.Head; trackingRenderNode != null; trackingRenderNode = trackingRenderNode.Next)
			{
				TrackingComponent trackingComp = trackingRenderNode.TrackingComp;
				TrackingGameObjectViewComponent trackingView = trackingRenderNode.TrackingView;
				if (trackingView.Speed != 0f)
				{
					float target = MathUtils.MinAngle(trackingView.Yaw, trackingComp.Yaw);
					trackingView.Yaw = Mathf.SmoothDampAngle(trackingView.Yaw, target, ref trackingView.YawVelocity, trackingComp.MaxVelocity / trackingView.Speed);
					if (trackingView.YawVelocity != 0f)
					{
						trackingView.Yaw = MathUtils.WrapAngle(trackingView.Yaw);
					}
					trackingView.YawRotate(trackingView.Yaw);
					if (trackingComp.TrackPitch)
					{
						float target2 = MathUtils.MinAngle(trackingView.Pitch, trackingComp.Pitch);
						trackingView.Pitch = Mathf.SmoothDampAngle(trackingView.Pitch, target2, ref trackingView.PitchVelocity, trackingComp.MaxVelocity / trackingView.Speed);
						if (trackingView.PitchVelocity != 0f)
						{
							trackingView.Pitch = MathUtils.WrapAngle(trackingView.Pitch);
						}
						trackingView.PitchRotate(trackingView.Pitch);
					}
				}
			}
		}
	}
}
