using System;
using UnityEngine;

namespace StaRTS.Utils
{
	public static class CameraUtils
	{
		private const float DRAG_THRESHOLD_SCREEN_WIDTH_PERCENTAGE = 0.01f;

		public static bool HasDragged(Vector2 screenPosition, Vector2 lastScreenPosition)
		{
			float num = (float)Screen.width * 0.01f;
			num *= num;
			float num2 = lastScreenPosition.x - screenPosition.x;
			float num3 = lastScreenPosition.y - screenPosition.y;
			float num4 = num2 * num2 + num3 * num3;
			return num4 >= num;
		}

		public static float CalculateDistanceFromEyeToScreen(Camera unityCamera)
		{
			return (float)Screen.height * 0.5f / Mathf.Tan(0.5f * unityCamera.fieldOfView * 3.14159274f / 180f);
		}

		public static bool GetGroundPositionHelper(Camera unityCamera, Vector3 screenPosition, Vector3 rayOrigin, float distanceFromEyeToScreen, float groundOffset, ref Vector3 groundPosition)
		{
			Vector3 a;
			CameraUtils.ScreenToRay(unityCamera, screenPosition, distanceFromEyeToScreen, out a);
			if (a.y < 0f)
			{
				float d = (groundOffset - rayOrigin.y) / a.y;
				groundPosition = rayOrigin + a * d;
				groundPosition.y = 0f;
				return true;
			}
			return false;
		}

		private static void ScreenToRay(Camera unityCamera, Vector2 screenPosition, float distanceFromEyeToScreen, out Vector3 outRayDirection)
		{
			Vector4 v = new Vector4(screenPosition.x - 0.5f * (float)Screen.width, screenPosition.y - 0.5f * (float)Screen.height, -distanceFromEyeToScreen, 0f);
			Matrix4x4 cameraToWorldMatrix = unityCamera.cameraToWorldMatrix;
			v = cameraToWorldMatrix * v;
			outRayDirection = new Vector3(v.x, v.y, v.z);
			outRayDirection = outRayDirection.normalized;
		}
	}
}
