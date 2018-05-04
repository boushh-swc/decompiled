using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.Projectors
{
	public class GeometryProjector
	{
		public IProjectorRenderer Renderer;

		public int ProjectorIndex;

		public ProjectorConfig Config;

		public ProjectorAssetProcessor AssetProcessor;

		public ProjectorForceReloadHelper ReloadHelper;

		public GeometryProjector(ProjectorConfig config)
		{
			this.Config = config;
			this.ProjectorIndex = Service.ProjectorManager.AddProjector(this);
			this.AssetProcessor = new ProjectorAssetProcessor(this);
		}

		public void Destroy()
		{
			Service.ProjectorManager.RemoveProjector(this);
			if (this.AssetProcessor != null)
			{
				this.AssetProcessor.UnloadAllAssets(null);
				this.AssetProcessor = null;
			}
			if (this.Renderer != null)
			{
				this.Renderer.Destroy();
				this.Renderer = null;
			}
			if (this.Config != null)
			{
				this.Config.Destroy();
				this.Config = null;
			}
			if (this.ReloadHelper != null)
			{
				this.ReloadHelper.Destroy();
				this.ReloadHelper = null;
			}
		}
	}
}
