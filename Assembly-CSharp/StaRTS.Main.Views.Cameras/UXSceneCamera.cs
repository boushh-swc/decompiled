using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class UXSceneCamera : UXCamera
	{
		private const string UX_SCENE_ROOT_NAME = "UX {0} Root";

		private const string UX_SCENE_CAMERA_NAME = "UX {0} Camera";

		private const string UX_SCENE_ANCHOR_NAME = "UX {0} Anchor";

		private const string UX_SCENE_NAME = "Scene";

		private const float SCENE_ROOT_CLIP = 10f;

		public override void Init(float offset)
		{
			this.InitHelper(offset, "Scene", 9, 2);
		}

		protected void InitHelper(float offset, string name, int layer, int depth)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.uxRoot);
			gameObject.name = string.Format("UX {0} Root", name);
			gameObject.transform.position = this.uxRoot.transform.position + Vector3.right * offset;
			this.uxRoot = gameObject;
			Camera componentInChildren = gameObject.GetComponentInChildren<Camera>();
			componentInChildren.nearClipPlane = -10f;
			componentInChildren.farClipPlane = 10f;
			GameObject gameObject2 = UnityUtils.FindGameObject(this.uxRoot, "UX Camera");
			if (gameObject2 == null)
			{
				throw new Exception("Unable to find cloned " + name + " camera");
			}
			gameObject2.name = string.Format("UX {0} Camera", name);
			this.unityCamera = gameObject2.GetComponent<Camera>();
			this.nguiCamera = this.unityCamera.gameObject.GetComponent<UICamera>();
			this.unityCamera.eventMask = 0;
			this.unityCamera.renderingPath = RenderingPath.Forward;
			this.uxAnchor = UnityUtils.FindGameObject(this.unityCamera.gameObject, "UX Anchor");
			if (this.uxAnchor == null)
			{
				throw new Exception("Unable to find cloned " + name + " anchor");
			}
			this.uxAnchor.name = string.Format("UX {0} Anchor", name);
			this.scale = gameObject2.transform.localScale.x;
			UnityUtils.SetLayerRecursively(this.uxRoot, layer);
			this.unityCamera.cullingMask = 1 << layer;
			this.unityCamera.depth = (float)depth;
			this.unityCamera.enabled = false;
			base.ReceiveEvents = false;
		}
	}
}
