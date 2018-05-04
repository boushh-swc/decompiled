using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class QuadCamera : CameraBase
	{
		private const string CAMERA_FORMAT = "{0} Camera";

		private const string OBJECT_FORMAT = "{0} Quad";

		private const float FAR_CLIP = 1f;

		private GameObject quadGameObject;

		protected Material quadMaterial;

		private Mesh quadMesh;

		protected List<Camera> srcCameras;

		protected List<Camera> dstCameras;

		protected RenderTexture srcRenderTexture;

		protected RenderTexture dstRenderTexture;

		private string name;

		protected QuadCamera(string name, Vector3 position, int layer, int depth)
		{
			this.name = name;
			this.srcCameras = new List<Camera>();
			this.dstCameras = new List<Camera>();
			GameObject gameObject = new GameObject(string.Format("{0} Camera", name));
			gameObject.layer = layer;
			Transform transform = gameObject.transform;
			transform.position = position;
			transform.rotation = Quaternion.AngleAxis(90f, Vector3.right);
			this.unityCamera = gameObject.AddComponent<Camera>();
			this.unityCamera.enabled = false;
			this.unityCamera.orthographic = true;
			this.unityCamera.orthographicSize = (float)(Screen.height / 2);
			this.unityCamera.clearFlags = CameraClearFlags.Color;
			this.unityCamera.backgroundColor = Color.black;
			this.unityCamera.nearClipPlane = -1f;
			this.unityCamera.farClipPlane = 1f;
			this.unityCamera.cullingMask = 1 << layer;
			this.unityCamera.depth = (float)depth;
			this.unityCamera.useOcclusionCulling = false;
			this.unityCamera.eventMask = 0;
			this.unityCamera.renderingPath = RenderingPath.VertexLit;
		}

		public bool IsRendering()
		{
			return this.unityCamera.enabled;
		}

		protected RenderTexture PrepareCameras(List<Camera> cameras)
		{
			RenderTexture temporaryRenderTexture = UnityUtils.GetTemporaryRenderTexture(Screen.width, Screen.height);
			int i = 0;
			int count = cameras.Count;
			while (i < count)
			{
				Camera camera = cameras[i];
				camera.targetTexture = temporaryRenderTexture;
				camera.enabled = true;
				i++;
			}
			return temporaryRenderTexture;
		}

		protected void RestoreCameras(List<Camera> cameras, bool enable, bool destroy)
		{
			int i = 0;
			int count = cameras.Count;
			while (i < count)
			{
				Camera camera = cameras[i];
				if (!(camera == null))
				{
					camera.targetTexture = null;
					camera.enabled = enable;
					if (destroy)
					{
						UnityEngine.Object.Destroy(camera.gameObject);
					}
				}
				i++;
			}
		}

		protected void CreateMaterial(string shaderName)
		{
			Shader shader = Service.AssetManager.Shaders.GetShader(shaderName);
			this.quadMaterial = UnityUtils.CreateMaterial(shader);
		}

		protected void StartRendering(bool needQuad)
		{
			if (needQuad)
			{
				this.quadGameObject = new GameObject(string.Format("{0} Quad", this.name));
				this.quadGameObject.transform.position = this.unityCamera.gameObject.transform.position - new Vector3((float)Screen.width * 0.5f, 0.5f, (float)Screen.height * 0.5f);
				this.quadGameObject.transform.localScale = new Vector3((float)Screen.width, 0f, (float)Screen.height);
				this.quadMesh = UnityUtils.CreateQuadMesh(0f);
				UnityUtils.SetupMeshMaterial(this.quadGameObject, this.quadMesh, this.quadMaterial);
				UnityUtils.SetLayerRecursively(this.quadGameObject, this.unityCamera.gameObject.layer);
			}
			else
			{
				this.quadGameObject = null;
			}
			this.unityCamera.enabled = true;
		}

		protected void DestroyRenderObjects()
		{
			this.unityCamera.enabled = false;
			if (this.srcRenderTexture != null)
			{
				UnityUtils.ReleaseTemporaryRenderTexture(this.srcRenderTexture);
				this.srcRenderTexture = null;
			}
			if (this.dstRenderTexture != null)
			{
				UnityUtils.ReleaseTemporaryRenderTexture(this.dstRenderTexture);
				this.dstRenderTexture = null;
			}
			if (this.quadMesh != null)
			{
				UnityUtils.DestroyMesh(this.quadMesh);
				this.quadMesh = null;
			}
			if (this.quadMaterial != null)
			{
				UnityUtils.DestroyMaterial(this.quadMaterial);
				this.quadMaterial = null;
			}
			if (this.quadGameObject != null)
			{
				UnityEngine.Object.Destroy(this.quadGameObject);
				this.quadGameObject = null;
				this.quadMaterial = null;
			}
		}
	}
}
