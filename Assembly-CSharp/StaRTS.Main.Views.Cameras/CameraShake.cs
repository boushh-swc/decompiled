using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class CameraShake : IViewPhysicsTimeObserver
	{
		private GameObject harness;

		private Vector3 originalPosition;

		private float elapsed;

		private float magnitude;

		private float duration;

		private OnCameraShake onCameraShake;

		public const float MAGNITUDE_SMALL = 0.2f;

		public const float MAGNITUDE_MEDIUM = 0.25f;

		public const float MAGNITUDE_LARGE = 0.75f;

		public const float DURATION_SMALL = 0.5f;

		public const float DURATION_MEDIUM = 0.5f;

		public const float DURATION_LARGE = 1f;

		public CameraShake(OnCameraShake onCameraShake)
		{
			this.harness = GameObject.Find("Main Camera Harness");
			this.originalPosition = this.harness.transform.position;
			this.onCameraShake = onCameraShake;
		}

		public void Shake()
		{
			this.Shake(0.5f, 0.2f);
		}

		public void Shake(float duration, float magnitude)
		{
			if (this.elapsed > 0f && magnitude <= this.magnitude)
			{
				return;
			}
			Service.ViewTimeEngine.RegisterPhysicsTimeObserver(this);
			this.elapsed = 0f;
			this.duration = duration;
			this.magnitude = magnitude;
		}

		public void OnViewPhysicsTime(float dt)
		{
			this.elapsed += dt;
			if (this.elapsed > this.duration)
			{
				this.SetHarnessPosition(this.originalPosition);
				this.elapsed = 0f;
				Service.ViewTimeEngine.UnregisterPhysicsTimeObserver(this);
				return;
			}
			float num = this.elapsed / this.duration;
			float num2 = 1f - Mathf.Clamp(4f * num - 3f, 0f, 1f);
			Rand rand = Service.Rand;
			float num3 = rand.ViewValue * 2f - 1f;
			float num4 = rand.ViewValue * 2f - 1f;
			float num5 = rand.ViewValue * 2f - 1f;
			float num6 = this.magnitude * num2;
			num3 *= num6;
			num4 *= num6;
			num5 *= num6;
			Vector3 harnessPosition = new Vector3(this.originalPosition.x + num3, this.originalPosition.y + num4, this.originalPosition.z + num5);
			this.SetHarnessPosition(harnessPosition);
		}

		private void SetHarnessPosition(Vector3 newPosition)
		{
			this.harness.transform.position = newPosition;
			if (this.onCameraShake != null)
			{
				this.onCameraShake(newPosition);
			}
		}
	}
}
