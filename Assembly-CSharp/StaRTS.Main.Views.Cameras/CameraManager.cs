using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class CameraManager
	{
		public const float UX_CAMERA_OFFSET_X = -10f;

		private const float QUAD_CAMERA_OFFSET_X = 1000f;

		private const float QUAD_CAMERA_OFFSET_Y = -500f;

		private MainCamera mainCamera;

		private UXCamera uxCamera;

		private UXSceneCamera uxSceneCamera;

		private WipeCamera wipeCamera;

		private WarBoardCamera warBoardCamera;

		public Camera HyperspaceCamera;

		public Camera StarsCamera;

		private List<RenderTextureItem> renderTextures;

		public MainCamera MainCamera
		{
			get
			{
				return this.mainCamera;
			}
		}

		public UXCamera UXCamera
		{
			get
			{
				return this.uxCamera;
			}
		}

		public UXSceneCamera UXSceneCamera
		{
			get
			{
				return this.uxSceneCamera;
			}
		}

		public WipeCamera WipeCamera
		{
			get
			{
				return this.wipeCamera;
			}
		}

		public WarBoardCamera WarBoardCamera
		{
			get
			{
				return this.warBoardCamera;
			}
		}

		public CameraManager()
		{
			Service.CameraManager = this;
			this.renderTextures = new List<RenderTextureItem>();
			this.mainCamera = new MainCamera();
			this.uxCamera = new UXCamera();
			this.uxCamera.Init(0f);
			this.uxCamera.Scale = Splash.CalculateScale();
			this.uxSceneCamera = new UXSceneCamera();
			this.uxSceneCamera.Init(-10f);
			this.uxCamera.Camera.clearFlags = CameraClearFlags.Depth;
			Vector3 position = Vector3.right * 1000f + Vector3.up * -500f;
			this.wipeCamera = new WipeCamera(position);
			this.warBoardCamera = new WarBoardCamera();
		}

		public void SetCameraOrderForPreloadScreens()
		{
			this.uxCamera.Camera.depth = 2f;
			this.uxSceneCamera.Camera.depth = 0f;
		}

		public void SetRegularCameraOrder()
		{
			this.uxCamera.Camera.depth = 0f;
			this.uxSceneCamera.Camera.depth = 2f;
		}

		public RenderTexture GetRenderTexture(int width, int height)
		{
			RenderTexture renderTexture = null;
			List<RenderTextureItem> list = new List<RenderTextureItem>();
			for (int i = this.renderTextures.Count - 1; i >= 0; i--)
			{
				RenderTextureItem renderTextureItem = this.renderTextures[i];
				if (!renderTextureItem.InUse)
				{
					if (renderTextureItem.Width != width || renderTextureItem.Height != height)
					{
						list.Add(renderTextureItem);
					}
					else if (renderTexture == null)
					{
						renderTextureItem.InUse = true;
						renderTexture = renderTextureItem.RenderTexture;
					}
				}
			}
			int count = list.Count;
			for (int j = 0; j < count; j++)
			{
				RenderTextureItem renderTextureItem2 = list[j];
				this.renderTextures.Remove(renderTextureItem2);
				renderTextureItem2.RenderTexture.DiscardContents();
				renderTextureItem2.RenderTexture.Release();
				UnityEngine.Object.Destroy(renderTextureItem2.RenderTexture);
			}
			list.Clear();
			if (renderTexture == null)
			{
				int depth = 16;
				renderTexture = new RenderTexture(width, height, depth);
				renderTexture.autoGenerateMips = false;
				renderTexture.filterMode = FilterMode.Point;
				RenderTextureItem renderTextureItem3 = new RenderTextureItem(renderTexture, width, height);
				renderTextureItem3.InUse = true;
				this.renderTextures.Add(renderTextureItem3);
			}
			return renderTexture;
		}

		public void ReleaseRenderTexture(RenderTexture renderTexture, bool destroy)
		{
			int i = 0;
			int count = this.renderTextures.Count;
			while (i < count)
			{
				RenderTextureItem renderTextureItem = this.renderTextures[i];
				if (renderTextureItem.RenderTexture == renderTexture)
				{
					renderTextureItem.RenderTexture.DiscardContents();
					if (destroy)
					{
						renderTextureItem.RenderTexture.Release();
						UnityEngine.Object.Destroy(renderTextureItem.RenderTexture);
						this.renderTextures.RemoveAt(i);
					}
					else
					{
						renderTextureItem.InUse = false;
					}
					break;
				}
				i++;
			}
		}
	}
}
