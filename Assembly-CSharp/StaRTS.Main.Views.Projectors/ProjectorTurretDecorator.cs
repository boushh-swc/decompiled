using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorTurretDecorator : IProjectorRenderer
	{
		private static readonly Quaternion DEFAULT_ROTATION = Quaternion.Euler(0f, -90f, 0f);

		private IProjectorRenderer baseRenderer;

		public ProjectorTurretDecorator(IProjectorRenderer baseRenderer)
		{
			this.baseRenderer = baseRenderer;
		}

		public void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			GameObject gameObject = UnityUtils.FindGameObject(config.MainAsset, config.TrackerName);
			if (gameObject != null)
			{
				gameObject.transform.localRotation = ProjectorTurretDecorator.DEFAULT_ROTATION;
			}
			this.baseRenderer.Render(config);
		}

		public void PostRender(ProjectorConfig config)
		{
			this.baseRenderer.PostRender(config);
		}

		public void Destroy()
		{
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
