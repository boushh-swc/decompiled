using StaRTS.Main.Views.World;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities.Projectiles
{
	public class ProjectileViewLinearPath : ProjectileViewPath
	{
		private Vector3 delta;

		public ProjectileViewLinearPath(ProjectileView view) : base(view)
		{
			this.delta = view.TargetLocation - view.StartLocation;
		}

		public override void OnUpdate(float dt)
		{
			float num = this.AgeSeconds / this.view.LifetimeSeconds;
			Vector3 vector;
			if (this.view.TargetTracker == null)
			{
				vector = this.view.StartLocation + this.delta * num;
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
			}
			else
			{
				vector = Vector3.Lerp(this.view.StartLocation, this.view.TargetTracker.position, num);
				if (this.view.EmitterTracker != null)
				{
					this.view.EmitterTracker.position = vector;
					this.view.EmitterTracker.LookAt(this.view.TargetTracker.position);
				}
				if (this.view.DefaultTracker != null)
				{
					this.view.DefaultTracker.position = vector;
					this.view.DefaultTracker.LookAt(this.view.TargetTracker.position);
				}
				if (this.view.MeshTracker != null)
				{
					this.view.MeshTracker.position = vector;
					this.view.MeshTracker.LookAt(this.view.TargetTracker.position);
				}
			}
			base.CurrentLocation = vector;
		}
	}
}
