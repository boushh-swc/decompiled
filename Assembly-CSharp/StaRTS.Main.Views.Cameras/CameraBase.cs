using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class CameraBase
	{
		protected Camera unityCamera;

		public Camera Camera
		{
			get
			{
				return this.unityCamera;
			}
		}

		public CameraBase()
		{
			this.unityCamera = null;
		}

		public Ray ScreenPointToRay(Vector3 screenPoint)
		{
			return this.unityCamera.ScreenPointToRay(screenPoint);
		}

		public Vector3 ViewportPositionToScreenPoint(Vector3 viewportPosition)
		{
			return this.unityCamera.ViewportToScreenPoint(viewportPosition);
		}

		public void Enable()
		{
			this.unityCamera.enabled = true;
		}

		public void Disable()
		{
			this.unityCamera.enabled = false;
		}
	}
}
