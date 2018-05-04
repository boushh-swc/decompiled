using StaRTS.Main.Models.Entities.Components;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorMeterDecorator : IProjectorRenderer
	{
		private IProjectorRenderer baseRenderer;

		private MeterShaderComponent shaderMeterComp;

		public ProjectorMeterDecorator(IProjectorRenderer baseRenderer)
		{
			this.baseRenderer = baseRenderer;
		}

		public void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			this.shaderMeterComp = Service.EntityViewManager.CreateMeterShaderComponentIfApplicable(config.MainAsset);
			this.shaderMeterComp.UpdatePercentage(config.MeterValue);
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
			this.shaderMeterComp = null;
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
