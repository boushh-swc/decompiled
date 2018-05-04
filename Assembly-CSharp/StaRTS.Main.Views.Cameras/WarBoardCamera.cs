using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class WarBoardCamera : WorldCamera
	{
		private const string STR_WAR_BOARD_CAMERA = "WarboardCamera";

		private GameObject cameraGameObject;

		private float distanceFromEyeToScreen;

		private Vector3 warboardCameraPosFromUnity = new Vector3(0f, 160f, -50f);

		private Vector3 warboardCameraRotation = new Vector3(18f, 0f, 0f);

		public WarBoardCamera()
		{
			this.cameraGameObject = new GameObject("WarboardCamera");
			this.cameraGameObject.transform.position = this.warboardCameraPosFromUnity;
			this.cameraGameObject.transform.position += new Vector3(-10000f, -10000f, 0f);
			this.cameraGameObject.transform.localEulerAngles = this.warboardCameraRotation;
			this.unityCamera = this.cameraGameObject.AddComponent<Camera>();
			this.unityCamera.clearFlags = CameraClearFlags.Color;
			this.unityCamera.backgroundColor = Color.black;
			this.unityCamera.depth = -1f;
			this.unityCamera.orthographic = false;
			this.unityCamera.fieldOfView = 20f;
			this.unityCamera.nearClipPlane = 20f;
			this.unityCamera.farClipPlane = 1500f;
			this.unityCamera.useOcclusionCulling = false;
			this.unityCamera.hdr = false;
			base.GroundOffset = 0f;
			this.distanceFromEyeToScreen = CameraUtils.CalculateDistanceFromEyeToScreen(this.unityCamera);
			base.Disable();
		}

		public override bool GetGroundPosition(Vector3 screenPosition, ref Vector3 groundPosition)
		{
			if (this.unityCamera.enabled)
			{
				Vector3 position = this.unityCamera.transform.position;
				return CameraUtils.GetGroundPositionHelper(this.unityCamera, screenPosition, position, this.distanceFromEyeToScreen, base.GroundOffset, ref groundPosition);
			}
			return false;
		}
	}
}
