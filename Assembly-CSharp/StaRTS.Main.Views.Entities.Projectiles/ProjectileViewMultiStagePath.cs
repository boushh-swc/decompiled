using StaRTS.Main.Views.World;
using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities.Projectiles
{
	public class ProjectileViewMultiStagePath : ProjectileViewPath
	{
		private Vector3 stage1EndPos;

		private float stage1DurS;

		private float stageTransDurS;

		private float stage2DurS;

		private float stage2TransKeyFrameS;

		private float stage2KeyFrameS;

		private Vector3 deltaS1;

		private Vector3 deltaS2;

		public ProjectileViewMultiStagePath(ProjectileView view) : base(view)
		{
			this.stage1DurS = view.ProjectileType.Stage1Duration / 1000f;
			this.stageTransDurS = view.ProjectileType.StageTransitionDuration / 1000f;
			this.stage2DurS = view.ProjectileType.Stage2Duration / 1000f;
			this.stage2TransKeyFrameS = this.stage1DurS;
			this.stage2KeyFrameS = this.stage1DurS + this.stageTransDurS;
			Vector3 vector = view.TargetLocation - view.StartLocation;
			vector.y = 0f;
			vector.Normalize();
			vector *= (float)view.ProjectileType.ArcRange;
			this.stage1EndPos = view.StartLocation + vector;
			this.stage1EndPos.y = (float)view.ProjectileType.ArcHeight;
			this.deltaS1 = this.stage1EndPos - view.StartLocation;
			this.deltaS2 = view.TargetLocation - this.stage1EndPos;
		}

		public override void OnUpdate(float dt)
		{
			if (this.AgeSeconds <= this.stage2TransKeyFrameS)
			{
				Vector3 zero = Vector3.zero;
				zero.x = Easing.Linear(this.AgeSeconds, this.view.StartLocation.x, this.deltaS1.x, this.stage1DurS);
				zero.z = Easing.Linear(this.AgeSeconds, this.view.StartLocation.z, this.deltaS1.z, this.stage1DurS);
				zero.y = Easing.CubicEaseOut(this.AgeSeconds, this.view.StartLocation.y, this.deltaS1.y, this.stage1DurS);
				if (this.view.EmitterTracker != null)
				{
					this.view.EmitterTracker.position = zero;
				}
				if (this.view.MeshTracker != null)
				{
					this.view.MeshTracker.position = zero;
				}
				if (this.view.DefaultTracker != null)
				{
					this.view.DefaultTracker.position = zero;
				}
				base.CurrentLocation = zero;
			}
			else if (this.AgeSeconds >= this.stage2KeyFrameS)
			{
				Vector3 vector = this.stage1EndPos;
				float t = this.AgeSeconds - this.stage2KeyFrameS;
				vector.x = Easing.ExpoEaseIn(t, this.stage1EndPos.x, this.deltaS2.x, this.stage2DurS);
				vector.z = Easing.ExpoEaseIn(t, this.stage1EndPos.z, this.deltaS2.z, this.stage2DurS);
				vector.y = Easing.ExpoEaseIn(t, this.stage1EndPos.y, this.deltaS2.y, this.stage2DurS);
				if (this.view.EmitterTracker != null)
				{
					this.view.EmitterTracker.position = vector;
				}
				if (this.view.MeshTracker != null)
				{
					this.view.MeshTracker.position = vector;
				}
				if (this.view.DefaultTracker != null)
				{
					this.view.DefaultTracker.position = vector;
				}
				base.CurrentLocation = vector;
			}
		}
	}
}
