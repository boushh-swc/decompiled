using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class TrackingSystem : SimSystemBase
	{
		private NodeList<TrackingNode> nodeList;

		public override void AddToGame(Game game)
		{
			this.nodeList = Service.EntityController.GetNodeList<TrackingNode>();
		}

		public override void RemoveFromGame(Game game)
		{
			this.nodeList = null;
		}

		protected override void Update(uint dt)
		{
			ShooterController shooterController = Service.ShooterController;
			for (TrackingNode trackingNode = this.nodeList.Head; trackingNode != null; trackingNode = trackingNode.Next)
			{
				TrackingComponent trackingComp = trackingNode.TrackingComp;
				Entity turretTarget = shooterController.GetTurretTarget(trackingNode.ShooterComp);
				if (turretTarget != null && turretTarget != trackingComp.TargetEntity)
				{
					trackingComp.TargetEntity = turretTarget;
					trackingComp.Mode = TrackingMode.Entity;
				}
				if (trackingComp.Mode != TrackingMode.Disabled)
				{
					if (trackingComp.Mode == TrackingMode.Entity)
					{
						float num = (float)trackingNode.TrackingComp.TargetTransform.X - trackingNode.Transform.CenterX();
						float num2 = (float)trackingNode.TrackingComp.TargetTransform.Z - trackingNode.Transform.CenterZ();
						trackingComp.Yaw = Mathf.Atan2(num2, num);
						if (trackingNode.TrackingComp.TrackPitch)
						{
							float num3 = Mathf.Sqrt(num * num + num2 * num2);
							float num4 = Mathf.Sqrt(trackingNode.ShooterComp.MinAttackRangeSquared);
							float num5 = Mathf.Sqrt(trackingNode.ShooterComp.MaxAttackRangeSquared);
							num3 = Mathf.Clamp(num3, num4, num5);
							trackingComp.Pitch = 0.2617994f * (num5 - num3) / (num5 - num4);
						}
					}
					else if (trackingComp.Mode == TrackingMode.Location)
					{
						float x = (float)trackingComp.TargetX - trackingNode.Transform.CenterX();
						float y = (float)trackingComp.TargetZ - trackingNode.Transform.CenterZ();
						trackingComp.Yaw = Mathf.Atan2(y, x);
					}
					else if (trackingComp.Mode == TrackingMode.Angle)
					{
						trackingComp.Yaw = trackingComp.TargetYaw;
					}
				}
			}
		}
	}
}
