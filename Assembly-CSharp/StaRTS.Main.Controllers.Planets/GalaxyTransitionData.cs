using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Planets
{
	public class GalaxyTransitionData
	{
		public float StartViewDistance
		{
			get;
			private set;
		}

		public float EndViewDistance
		{
			get;
			private set;
		}

		public float StartViewHeight
		{
			get;
			private set;
		}

		public float EndViewHeight
		{
			get;
			private set;
		}

		public float StartViewRotation
		{
			get;
			private set;
		}

		public float EndViewRotation
		{
			get;
			private set;
		}

		public Vector3 StartViewLookAt
		{
			get;
			private set;
		}

		public Vector3 EndViewLookAt
		{
			get;
			private set;
		}

		public bool Instant
		{
			get;
			set;
		}

		public float TransitionDuration
		{
			get;
			set;
		}

		public void SetTransitionDistance(float startDistance, float endDistance)
		{
			this.StartViewDistance = startDistance;
			this.EndViewDistance = endDistance;
		}

		public void SetTransitionHeight(float startHeight, float endHeight)
		{
			this.StartViewHeight = startHeight;
			this.EndViewHeight = endHeight;
		}

		public void SetTransitionRotation(float startRotation, float endRotation)
		{
			this.StartViewRotation = startRotation;
			this.EndViewRotation = endRotation;
		}

		public void SetTransitionLookAt(Vector3 startlook, Vector3 endLook)
		{
			this.StartViewLookAt = startlook;
			this.EndViewLookAt = endLook;
		}

		public void Reset()
		{
			this.StartViewDistance = 0f;
			this.EndViewDistance = 0f;
			this.StartViewHeight = 0f;
			this.EndViewHeight = 0f;
			this.StartViewRotation = 0f;
			this.EndViewRotation = 0f;
			this.StartViewLookAt = Vector3.zero;
			this.EndViewLookAt = Vector3.zero;
			this.TransitionDuration = 0f;
			this.Instant = false;
		}
	}
}
