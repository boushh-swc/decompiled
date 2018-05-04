using StaRTS.Main.Views.World;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities.Projectiles
{
	public class ProjectileViewArcPath : ProjectileViewPath
	{
		private const float g = 9.8f;

		private float spinSpeed;

		private Vector3 velocityXZ;

		private float velocityY;

		public ProjectileViewArcPath(ProjectileView view) : base(view)
		{
			this.velocityY = view.LifetimeSeconds / 2f * 9.8f;
			if (view.LifetimeSeconds != 0f)
			{
				this.velocityY -= view.StartLocation.y / view.LifetimeSeconds;
			}
			this.velocityXZ = view.TargetLocation - view.StartLocation;
			this.velocityXZ.y = 0f;
			this.velocityXZ.Normalize();
			this.velocityXZ *= (float)(view.ProjectileType.MaxSpeed * 3);
			this.spinSpeed = view.ProjectileType.SpinSpeed;
		}

		public override void OnUpdate(float dt)
		{
			Vector3 vector = this.view.StartLocation;
			vector.y += this.velocityY * this.AgeSeconds - 9.8f * this.AgeSeconds * this.AgeSeconds / 2f;
			vector += this.velocityXZ * this.AgeSeconds;
			if (this.view.EmitterTracker != null)
			{
				this.view.EmitterTracker.position = vector;
				this.view.EmitterTracker.Rotate(this.spinSpeed, 0f, 0f);
			}
			if (this.view.DefaultTracker != null)
			{
				this.view.DefaultTracker.position = vector;
				this.view.DefaultTracker.Rotate(this.spinSpeed, 0f, 0f);
			}
			if (this.view.MeshTracker != null)
			{
				this.view.MeshTracker.position = new Vector3(vector.x, 0f, vector.z);
			}
			base.CurrentLocation = vector;
		}
	}
}
