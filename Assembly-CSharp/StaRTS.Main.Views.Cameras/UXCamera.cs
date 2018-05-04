using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class UXCamera : CameraBase
	{
		protected const string UX_ROOT_NAME = "UX Root";

		protected const string UX_CAMERA_NAME = "UX Camera";

		protected const string UX_ANCHOR_NAME = "UX Anchor";

		protected GameObject uxRoot;

		protected GameObject uxAnchor;

		protected UICamera nguiCamera;

		protected float scale;

		public bool ReceiveEvents
		{
			get
			{
				return this.nguiCamera.eventReceiverMask == this.unityCamera.cullingMask;
			}
			set
			{
				this.nguiCamera.eventReceiverMask = ((!value) ? 0 : this.unityCamera.cullingMask);
			}
		}

		public GameObject Anchor
		{
			get
			{
				return this.uxAnchor;
			}
		}

		public float Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
				this.unityCamera.gameObject.transform.localScale = Vector3.one * this.scale;
			}
		}

		public UXCamera()
		{
			this.uxRoot = GameObject.Find("UX Root");
			if (this.uxRoot == null)
			{
				throw new Exception("Unable to find UX Root");
			}
		}

		public virtual void Init(float offset)
		{
			UIRoot component = this.uxRoot.GetComponent<UIRoot>();
			if (component != null)
			{
				component.enabled = true;
			}
			this.unityCamera = UnityUtils.FindCamera("UX Camera");
			if (this.unityCamera == null)
			{
				throw new Exception("Unable to find UX Camera");
			}
			this.unityCamera.eventMask = 0;
			this.unityCamera.renderingPath = RenderingPath.Forward;
			this.nguiCamera = this.unityCamera.gameObject.GetComponent<UICamera>();
			if (this.nguiCamera == null)
			{
				throw new Exception("Unable to find NGUI component on UX Camera");
			}
			this.uxAnchor = UnityUtils.FindGameObject(this.unityCamera.gameObject, "UX Anchor");
			if (this.uxAnchor == null)
			{
				throw new Exception("Unable to find UX Anchor");
			}
			UIAnchor component2 = this.uxAnchor.GetComponent<UIAnchor>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
			this.Scale = 1f;
			this.nguiCamera.useMouse = false;
		}

		public float ScaleColliderHorizontally(float x)
		{
			return Mathf.Round(x / this.uxRoot.transform.localScale.x);
		}

		public float ScaleColliderVertically(float y)
		{
			return Mathf.Round(y / this.uxRoot.transform.localScale.y);
		}

		public float ScaleColliderInDepth(float z)
		{
			return Mathf.Round(z / this.uxRoot.transform.localScale.z);
		}

		public void AddNewAnchor(GameObject gameObject)
		{
			gameObject.name = string.Format("{0} ({1})", this.uxAnchor.name, gameObject.name);
			this.SetParent(gameObject, this.unityCamera.gameObject);
		}

		public void AttachToMainAnchor(GameObject gameObject)
		{
			this.SetParent(gameObject, this.uxAnchor);
		}

		private void SetParent(GameObject gameObject, GameObject parent)
		{
			UnityUtils.SetLayerRecursively(gameObject, parent.layer);
			gameObject.transform.parent = parent.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
		}
	}
}
