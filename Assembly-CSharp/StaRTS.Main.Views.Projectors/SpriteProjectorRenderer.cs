using StaRTS.Main.Views.UX.Elements;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class SpriteProjectorRenderer : AbstractProjectorRenderer, IProjectorRenderer
	{
		private const string OBJECT_FORMAT = "Projector Quad ({0})";

		private GameObject quadGameObject;

		private UXSprite frameSprite;

		private UITexture uiTexture;

		public SpriteProjectorRenderer(UXSprite frameSprite, int projectorIndex) : base(projectorIndex)
		{
			this.frameSprite = frameSprite;
		}

		public override void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			base.SetupCamera(this.frameSprite.Root.name, config.MainAsset, config.Sharpness, this.frameSprite.Width, this.frameSprite.Height, config.CameraPosition, config.CameraInterest);
			this.SetupScreen(config);
			base.Render(config);
		}

		public override void PostRender(ProjectorConfig config)
		{
			base.PostRender(config);
		}

		public override void Destroy()
		{
			base.Destroy();
			this.DestroyQuad();
		}

		public UITexture GetProjectorUITexture()
		{
			return this.uiTexture;
		}

		private void DestroyQuad()
		{
			if (this.quadGameObject != null)
			{
				UnityEngine.Object.Destroy(this.quadGameObject);
			}
		}

		private void SetupScreen(ProjectorConfig config)
		{
			this.DestroyQuad();
			this.quadGameObject = new GameObject(string.Format("Projector Quad ({0})", this.frameSprite.Root.name));
			this.quadGameObject.layer = this.frameSprite.Root.layer;
			this.quadGameObject.transform.parent = this.frameSprite.Root.transform;
			this.quadGameObject.transform.localPosition = Vector3.zero;
			this.quadGameObject.transform.localScale = Vector3.one;
			this.uiTexture = this.quadGameObject.AddComponent<UITexture>();
			this.uiTexture.mainTexture = this.renderTexture;
			UXElement uXElement = new UXElement(this.frameSprite.UXCamera, this.quadGameObject, null);
			uXElement.Width = this.frameSprite.Width;
			uXElement.Height = this.frameSprite.Height;
			uXElement.WidgetDepth = this.frameSprite.WidgetDepth;
			if (config.Tint != Color.white)
			{
				this.uiTexture.color = config.Tint;
			}
		}

		virtual bool DoesRenderTextureNeedReload()
		{
			return base.DoesRenderTextureNeedReload();
		}
	}
}
