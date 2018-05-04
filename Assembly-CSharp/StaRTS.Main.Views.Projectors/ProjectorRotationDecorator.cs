using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorRotationDecorator : IProjectorRenderer, IViewFrameTimeObserver
	{
		private IProjectorRenderer baseRenderer;

		private GameObject targetObject;

		private float iconRotationSpeed;

		public ProjectorRotationDecorator(IProjectorRenderer baseRenderer)
		{
			this.baseRenderer = baseRenderer;
		}

		public void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			this.targetObject = config.MainAsset.gameObject;
			this.iconRotationSpeed = config.IconRotationSpeed;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.baseRenderer.Render(config);
		}

		public void PostRender(ProjectorConfig config)
		{
			this.baseRenderer.PostRender(config);
		}

		public void OnViewFrameTime(float dt)
		{
			this.targetObject.transform.Rotate(Vector3.up, this.iconRotationSpeed * dt);
		}

		public void Destroy()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.baseRenderer.Destroy();
			this.baseRenderer = null;
		}

		public bool DoesRenderTextureNeedReload()
		{
			return this.baseRenderer.DoesRenderTextureNeedReload();
		}

		public UITexture GetProjectorUITexture()
		{
			return this.baseRenderer.GetProjectorUITexture();
		}
	}
}
