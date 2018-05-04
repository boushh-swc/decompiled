using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.Projectors
{
	public class SurfaceProjectorRenderer : AbstractProjectorRenderer, IProjectorRenderer
	{
		public SurfaceProjectorRenderer(int projectorIndex) : base(projectorIndex)
		{
		}

		public override void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			if (config.RenderCallback == null)
			{
				Service.Logger.WarnFormat("Neither a sprite nor a RenderCallback was not provided for the projector {0}", new object[]
				{
					config.AssetName
				});
			}
			base.SetupCamera(config.AssetName, config.MainAsset, config.Sharpness, config.RenderWidth, config.RenderHeight, config.CameraPosition, config.CameraInterest);
			base.Render(config);
		}

		public override void PostRender(ProjectorConfig config)
		{
			base.PostRender(config);
		}

		public UITexture GetProjectorUITexture()
		{
			return null;
		}
	}
}
