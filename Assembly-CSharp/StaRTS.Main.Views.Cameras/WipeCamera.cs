using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class WipeCamera : QuadCamera, IViewFrameTimeObserver
	{
		public const float LEFT_TO_RIGHT = 0f;

		public const float BOTTOM_TO_TOP = 1.57079637f;

		public const float RIGHT_TO_LEFT = 3.14159274f;

		public const float TOP_TO_BOTTOM = 4.712389f;

		public const float LOWER_LEFT_TO_UPPER_RIGHT = 0.7853982f;

		public const float LOWER_RIGHT_TO_UPPER_LEFT = 2.3561945f;

		public const float UPPER_RIGHT_TO_LOWER_LEFT = 3.926991f;

		public const float UPPER_LEFT_TO_LOWER_RIGHT = 5.49778748f;

		private const string WIPE_NAME = "Wipe";

		private const float WIPE_THICKNESS = 0.1f;

		private const float WIPE_TIME = 1f;

		private const int FRAMES_FOR_SNAPSHOT = 4;

		private CameraManager cameraManager;

		private float wipeTime;

		private WipeCompleteDelegate completeCallback;

		private object completeCookie;

		private WipeTransition transition;

		private bool takingSnapshot;

		private int snapshotFramesToWait = 4;

		private bool continueWipeRequested;

		public WipeCamera(Vector3 position) : base("Wipe", position, 12, 3)
		{
			this.cameraManager = Service.CameraManager;
		}

		public void StartLinearWipe(WipeTransition transition, float wipeAngle, WipeCompleteDelegate completeCallback, object completeCookie)
		{
			this.StartWipe("Wipe_Linear", transition, wipeAngle, 0f, 0f, completeCallback, completeCookie);
		}

		public void StartEllipticalWipe(WipeTransition transition, float centerX, float centerY, WipeCompleteDelegate completeCallback, object completeCookie)
		{
			this.StartWipe("Wipe_Elliptical", transition, 0f, centerX, centerY, completeCallback, completeCookie);
		}

		private void StartWipe(string shaderName, WipeTransition transition, float wipeAngle, float centerX, float centerY, WipeCompleteDelegate completeCallback, object completeCookie)
		{
			if (base.IsRendering())
			{
				this.StopWipe();
			}
			this.completeCallback = completeCallback;
			this.completeCookie = completeCookie;
			this.transition = transition;
			Service.UserInputManager.Enable(false);
			switch (transition)
			{
			case WipeTransition.FromIntroToBase:
				this.SetupWipeFromIntroToBase();
				break;
			case WipeTransition.FromBaseToBase:
				this.SetupWipeFromBaseToBase();
				break;
			case WipeTransition.FromStoryToLoadingScreen:
				this.SetupWipeFromStoryToLoadingScreen();
				break;
			case WipeTransition.FromLoadingScreenToBase:
				this.SetupWipeFromLoadingScreenToBase();
				break;
			case WipeTransition.FromGalaxyToHyperspace:
				this.SetupWipeFromGalaxyToHyperspace();
				break;
			case WipeTransition.FromHyperspaceToBase:
				this.SetupWipeFromHyperspaceToBase();
				break;
			case WipeTransition.FromBaseToGalaxy:
				this.SetupWipeFromBaseToGalaxy();
				break;
			case WipeTransition.FromGalaxyToBase:
				this.SetupWipeFromGalaxyToBase();
				break;
			case WipeTransition.FromGalaxyToLoadingScreen:
				this.SetupWipeFromGalaxyToLoadingScreen();
				break;
			case WipeTransition.FromLoadingScreenToWarboard:
				this.SetupWipeFromLoadingScreenToWarboard();
				break;
			case WipeTransition.FromWarboardToLoadingScreen:
				this.SetupWipeFromWarboardToLoadingScreen();
				break;
			case WipeTransition.FromBaseToWarboard:
				this.SetupWipeFromBaseToWarboard();
				break;
			case WipeTransition.FromWarboardToBase:
				this.SetupWipeFromWarboardToBase();
				break;
			}
			if (this.srcCameras.Count > 0)
			{
				this.srcRenderTexture = base.PrepareCameras(this.srcCameras);
			}
			if (this.dstCameras.Count > 0)
			{
				this.dstRenderTexture = base.PrepareCameras(this.dstCameras);
			}
			base.CreateMaterial(shaderName);
			base.StartRendering(true);
			this.quadMaterial.SetTexture("_from", this.srcRenderTexture);
			this.quadMaterial.SetTexture("_to", this.dstRenderTexture);
			this.quadMaterial.SetFloat("_thickness", 0.1f);
			float num = Mathf.Cos(wipeAngle);
			float num2 = Mathf.Sin(wipeAngle);
			float num3 = Mathf.Abs(num) + Mathf.Abs(num2);
			this.quadMaterial.SetFloat("_cos_sum", num / num3);
			this.quadMaterial.SetFloat("_sin_sum", num2 / num3);
			this.quadMaterial.SetFloat("_xedge", (num >= 0f) ? 0f : 1f);
			this.quadMaterial.SetFloat("_yedge", (num2 >= 0f) ? 0f : 1f);
			this.quadMaterial.SetFloat("_cx", centerX);
			this.quadMaterial.SetFloat("_cy", centerY);
			this.quadMaterial.SetFloat("_aspect", (float)Screen.width / (float)Screen.height);
			this.wipeTime = 0f;
			this.OnViewFrameTime(0f);
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		private void SetupWipeFromIntroToBase()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.UXSceneCamera.Camera);
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.MainCamera.Camera);
			this.dstCameras.Add(this.cameraManager.UXCamera.Camera);
			this.DstCamerasAddHoloCamera();
		}

		private void SetupWipeFromBaseToBase()
		{
			this.SetupStandardSourceCameras();
			this.dstCameras.Clear();
			GameObject gameObject = new GameObject("Temp Wipe Camera");
			Transform transform = gameObject.transform;
			transform.position = new Vector3(0f, 50f, 0f);
			transform.rotation = Quaternion.AngleAxis(50f, Vector3.up);
			this.dstCameras.Add(gameObject.AddComponent<Camera>());
		}

		private void SetupWipeFromStoryToLoadingScreen()
		{
			this.SetupStandardSourceCameras();
			this.srcCameras.Add(this.cameraManager.StarsCamera);
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.UXSceneCamera.Camera);
		}

		private void SetupWipeFromGalaxyToBase()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.UXSceneCamera.Camera);
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.MainCamera.Camera);
			this.dstCameras.Add(this.cameraManager.UXCamera.Camera);
		}

		private void SetupWipeFromGalaxyToLoadingScreen()
		{
			this.SetupCamerasForGalaxy(this.srcCameras);
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.UXSceneCamera.Camera);
		}

		private void SetupWipeFromBaseToGalaxy()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.UXSceneCamera.Camera);
			this.SrcCamerasAddHoloCamera();
			this.SetupCamerasForGalaxy(this.dstCameras);
		}

		private void SetupWipeFromLoadingScreenToBase()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.UXSceneCamera.Camera);
			this.cameraManager.MainCamera.Camera.clearFlags = CameraClearFlags.Color;
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.MainCamera.Camera);
			this.dstCameras.Add(this.cameraManager.UXCamera.Camera);
			this.DstCamerasAddHoloCamera();
		}

		private void SetupWipeFromGalaxyToHyperspace()
		{
			this.SetupCamerasForGalaxy(this.srcCameras);
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.HyperspaceCamera);
		}

		private void SetupWipeFromHyperspaceToBase()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.UXSceneCamera.Camera);
			this.srcCameras.Add(this.cameraManager.HyperspaceCamera);
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.MainCamera.Camera);
		}

		private void SetupWipeFromLoadingScreenToWarboard()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.UXSceneCamera.Camera);
			this.SetupCamerasForWarBoard(this.dstCameras);
		}

		private void SetupWipeFromWarboardToLoadingScreen()
		{
			this.SetupCamerasForWarBoard(this.srcCameras);
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.UXSceneCamera.Camera);
		}

		private void SetupWipeFromBaseToWarboard()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.MainCamera.Camera);
			this.srcCameras.Add(this.cameraManager.UXCamera.Camera);
			this.dstCameras.Clear();
		}

		private void SetupWipeFromWarboardToBase()
		{
			this.SetupCamerasForWarBoard(this.srcCameras);
			this.dstCameras.Clear();
		}

		private void SetupStandardSourceCameras()
		{
			this.srcCameras.Clear();
			this.srcCameras.Add(this.cameraManager.MainCamera.Camera);
			this.srcCameras.Add(this.cameraManager.UXCamera.Camera);
			this.SrcCamerasAddHoloCamera();
		}

		private void SetupCamerasForGalaxy(List<Camera> cameras)
		{
			cameras.Clear();
			cameras.Add(this.cameraManager.MainCamera.Camera);
			cameras.Add(this.cameraManager.UXCamera.Camera);
			cameras.Add(this.cameraManager.StarsCamera);
		}

		private void SetupCamerasForWarBoard(List<Camera> cameras)
		{
			cameras.Clear();
			cameras.Add(this.cameraManager.WarBoardCamera.Camera);
			cameras.Add(this.cameraManager.UXCamera.Camera);
		}

		private void SrcCamerasAddHoloCamera()
		{
			Camera activeCamera = Service.HoloController.GetActiveCamera();
			if (activeCamera != null)
			{
				this.srcCameras.Add(activeCamera);
			}
		}

		private void DstCamerasAddHoloCamera()
		{
			Camera activeCamera = Service.HoloController.GetActiveCamera();
			if (activeCamera != null)
			{
				this.dstCameras.Add(activeCamera);
			}
		}

		public void TakeSnapshotForDelayedWipe(WipeTransition transition, float wipeAngle, WipeCompleteDelegate completeCallback, object completeCookie)
		{
			this.StartLinearWipe(transition, wipeAngle, completeCallback, null);
			this.takingSnapshot = true;
			this.snapshotFramesToWait = 4;
		}

		private void OnSnapShotTaken()
		{
			base.RestoreCameras(this.srcCameras, true, false);
			if (this.dstRenderTexture != null)
			{
				UnityUtils.ReleaseTemporaryRenderTexture(this.dstRenderTexture);
				this.dstRenderTexture = null;
			}
			WipeTransition wipeTransition = this.transition;
			if (wipeTransition != WipeTransition.FromBaseToWarboard)
			{
				if (wipeTransition == WipeTransition.FromWarboardToBase)
				{
					this.SetupDestinationWipeFromWarboardToBase();
				}
			}
			else
			{
				this.SetupDestinationWipeFromBaseToWarboard();
			}
			this.dstRenderTexture = base.PrepareCameras(this.dstCameras);
			this.quadMaterial.SetTexture("_to", this.dstRenderTexture);
			this.takingSnapshot = false;
			if (this.continueWipeRequested)
			{
				this.continueWipeRequested = false;
			}
			else
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
			Service.EventManager.SendEvent(EventId.WipeCameraSnapshotTaken, null);
		}

		private void SetupDestinationWipeFromBaseToWarboard()
		{
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.WarBoardCamera.Camera);
			this.dstCameras.Add(this.cameraManager.UXCamera.Camera);
		}

		private void SetupDestinationWipeFromWarboardToBase()
		{
			this.dstCameras.Clear();
			this.dstCameras.Add(this.cameraManager.MainCamera.Camera);
			this.dstCameras.Add(this.cameraManager.UXCamera.Camera);
		}

		public void ContinueWipe()
		{
			if (this.takingSnapshot)
			{
				this.continueWipeRequested = true;
				return;
			}
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void StopWipe()
		{
			if (!base.IsRendering())
			{
				return;
			}
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.wipeTime = 0f;
			this.continueWipeRequested = false;
			this.takingSnapshot = false;
			bool flag = this.transition == WipeTransition.FromBaseToBase;
			base.RestoreCameras(this.srcCameras, flag, false);
			this.srcCameras.Clear();
			base.RestoreCameras(this.dstCameras, true, flag);
			this.dstCameras.Clear();
			base.DestroyRenderObjects();
			Service.UserInputManager.Enable(true);
			if (this.completeCallback != null)
			{
				WipeCompleteDelegate wipeCompleteDelegate = this.completeCallback;
				object cookie = this.completeCookie;
				this.completeCallback = null;
				this.completeCookie = null;
				wipeCompleteDelegate(cookie);
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.takingSnapshot)
			{
				this.snapshotFramesToWait--;
				if (this.snapshotFramesToWait <= 0)
				{
					this.OnSnapShotTaken();
				}
				return;
			}
			this.wipeTime += dt;
			float num = this.wipeTime / 1f;
			bool flag = num >= 1f;
			if (flag)
			{
				num = 1f;
			}
			num += 0.1f * (num - 1f);
			this.quadMaterial.SetFloat("_travel", num);
			if (flag)
			{
				this.StopWipe();
			}
		}
	}
}
