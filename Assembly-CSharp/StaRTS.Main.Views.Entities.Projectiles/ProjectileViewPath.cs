using StaRTS.Main.Views.World;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities.Projectiles
{
	public class ProjectileViewPath
	{
		public float AgeSeconds;

		protected ProjectileView view;

		public Vector3 CurrentLocation
		{
			get;
			protected set;
		}

		public ProjectileViewPath(ProjectileView view)
		{
			this.view = view;
			this.CurrentLocation = view.StartLocation;
		}

		public virtual void OnUpdate(float dt)
		{
		}
	}
}
