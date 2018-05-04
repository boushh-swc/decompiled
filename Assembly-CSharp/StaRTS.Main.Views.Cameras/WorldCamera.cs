using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public abstract class WorldCamera : CameraBase
	{
		public float GroundOffset
		{
			get;
			set;
		}

		public abstract bool GetGroundPosition(Vector3 screenPosition, ref Vector3 groundPosition);

		public Vector3 WorldPositionToScreenPoint(Vector3 worldPoint)
		{
			return this.unityCamera.WorldToScreenPoint(worldPoint);
		}
	}
}
