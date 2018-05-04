using StaRTS.Main.Configs;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class AbstractProjectorRenderer : IViewFrameTimeObserver
	{
		private const float CAMERA_OFFSET = 500f;

		private const float GEOMETRY_FOV = 10f;

		private const string CAMERA_FORMAT = "Projector Camera ({0})";

		private const float INDEX_OFFSET = 250f;

		protected int projectorIndex;

		protected GameObject cameraGameObject;

		protected RenderTexture renderTexture;

		private Action<RenderTexture, ProjectorConfig> renderCallback;

		protected bool snapshot;

		private int snapshotFrameCount;

		private ProjectorConfig config;

		public AbstractProjectorRenderer(int projectorIndex)
		{
			this.projectorIndex = projectorIndex;
		}

		public bool DoesRenderTextureNeedReload()
		{
			return !(this.renderTexture == null) && !this.renderTexture.IsCreated();
		}

		protected void SetupCamera(string name, GameObject subject, float sharpness, float width, float height, Vector3 cameraPosition, Vector3 cameraInterest)
		{
			this.DestroyCamera();
			this.cameraGameObject = new GameObject(string.Format("Projector Camera ({0})", name));
			Camera camera = this.cameraGameObject.AddComponent<Camera>();
			camera.fieldOfView = 10f;
			camera.clearFlags = CameraClearFlags.Color;
			camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			camera.depth = 0f;
			int num = 13;
			this.cameraGameObject.layer = num;
			camera.cullingMask = 1 << num;
			UnityUtils.SetLayerRecursively(subject, this.cameraGameObject.layer);
			camera.aspect = 1f;
			camera.eventMask = 0;
			int num2 = Math.Min((int)(width * sharpness), (int)(height * sharpness));
			this.renderTexture = Service.CameraManager.GetRenderTexture(num2, num2);
			Vector3 vector = (Vector3.up + Vector3.right) * 500f;
			vector.y += (float)this.projectorIndex * 250f;
			this.cameraGameObject.transform.position = vector;
			subject.transform.position = vector - cameraPosition;
			Vector3 worldPosition = subject.transform.position + cameraInterest;
			this.cameraGameObject.transform.LookAt(worldPosition);
			camera.targetTexture = this.renderTexture;
		}

		public virtual void Render(ProjectorConfig config)
		{
			this.config = config;
			bool flag = config.AnimPreference == AnimationPreference.NoAnimation;
			bool flag2 = config.AnimPreference == AnimationPreference.AnimationPreferred && HardwareProfile.IsLowEndDevice();
			this.snapshot = (flag || flag2);
			if (this.snapshot)
			{
				this.renderCallback = config.RenderCallback;
				this.snapshotFrameCount = config.SnapshotFrameDelay;
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
			else if (config.RenderCallback != null)
			{
				config.RenderCallback(this.renderTexture, config);
			}
		}

		public virtual void PostRender(ProjectorConfig config)
		{
		}

		public void OnViewFrameTime(float dt)
		{
			this.snapshotFrameCount--;
			if (this.snapshotFrameCount > 0)
			{
				return;
			}
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.FinishSnapshot();
		}

		protected void FinishSnapshot()
		{
			if (this.renderCallback != null)
			{
				this.renderCallback(this.renderTexture, this.config);
			}
			this.DestroyCamera();
		}

		public virtual void Destroy()
		{
			this.DestroyCamera();
			Service.CameraManager.ReleaseRenderTexture(this.renderTexture, true);
			this.renderTexture = null;
		}

		public void DestroyCamera()
		{
			if (this.cameraGameObject != null)
			{
				this.cameraGameObject.GetComponent<Camera>().targetTexture = null;
				UnityEngine.Object.Destroy(this.cameraGameObject);
			}
		}
	}
}
