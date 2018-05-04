using System;

namespace StaRTS.Main.Views.Projectors
{
	public interface IProjectorRenderer
	{
		void Render(ProjectorConfig config);

		void Destroy();

		bool DoesRenderTextureNeedReload();

		UITexture GetProjectorUITexture();

		void PostRender(ProjectorConfig config);
	}
}
