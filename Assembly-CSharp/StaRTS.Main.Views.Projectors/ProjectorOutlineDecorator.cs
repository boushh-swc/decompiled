using System;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorOutlineDecorator : IProjectorRenderer
	{
		private const string OUTLINE_OUTER_PARAM = "_Outline";

		private const string OUTLINE_INNER_PARAM = "_OutlineInnerWidth";

		private const float NO_OUTLINE = 0f;

		private IProjectorRenderer baseRenderer;

		public ProjectorOutlineDecorator(IProjectorRenderer baseRenderer)
		{
			this.baseRenderer = baseRenderer;
		}

		public void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			float value = (!config.Outline) ? 0f : config.OutlineInner;
			float value2 = (!config.Outline) ? 0f : config.OutlineOuter;
			Renderer[] componentsInChildren = config.MainAsset.GetComponentsInChildren<Renderer>();
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				Renderer renderer = componentsInChildren[i];
				if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_OutlineInnerWidth") && renderer.sharedMaterial.HasProperty("_Outline"))
				{
					renderer.sharedMaterial.SetFloat("_Outline", value2);
					renderer.sharedMaterial.SetFloat("_OutlineInnerWidth", value);
				}
				i++;
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
