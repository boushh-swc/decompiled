using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorAnimationDecorator : IProjectorRenderer
	{
		private IProjectorRenderer baseRenderer;

		public ProjectorAnimationDecorator(IProjectorRenderer baseRenderer)
		{
			this.baseRenderer = baseRenderer;
		}

		public void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			Animator component = config.MainAsset.GetComponent<Animator>();
			Animation component2 = config.MainAsset.GetComponent<Animation>();
			if (component != null)
			{
				if (component.hasRootMotion)
				{
					component.SetInteger("Motivation", (int)config.AnimState);
				}
				else
				{
					Service.Logger.ErrorFormat("{0} needs re-rigging, its Animator does not have a root motion.", new object[]
					{
						config.AssetName
					});
				}
			}
			else if (component2 != null)
			{
				string text = config.AnimationName;
				if (string.IsNullOrEmpty(text))
				{
					text = config.AnimState.ToString();
				}
				if (component2.GetClip(text) != null)
				{
					component2.Play(text);
				}
			}
			else if (config.MainAsset != null)
			{
				AssetMeshDataMonoBehaviour component3 = config.MainAsset.transform.GetComponent<AssetMeshDataMonoBehaviour>();
				if (component3 != null)
				{
					List<GameObject> selectableGameObjects = component3.SelectableGameObjects;
					int i = 0;
					int count = selectableGameObjects.Count;
					while (i < count)
					{
						if (!(selectableGameObjects[i] == null))
						{
							component = selectableGameObjects[i].GetComponent<Animator>();
							if (component != null && component.hasRootMotion)
							{
								component.SetInteger("Motivation", (int)config.AnimState);
							}
						}
						i++;
					}
				}
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
