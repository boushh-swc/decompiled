using StaRTS.Main.Configs;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorEquipmentBuildingDecorator : IProjectorRenderer
	{
		private const string MISSING_SHARED_MATERIAL = "sharedMaterial missing on {0}";

		private IProjectorRenderer baseRenderer;

		private List<Material> materialCopies;

		public ProjectorEquipmentBuildingDecorator(IProjectorRenderer baseRenderer)
		{
			this.baseRenderer = baseRenderer;
		}

		public void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			if (string.IsNullOrEmpty(config.buildingEquipmentShaderName) || config.AnimPreference == AnimationPreference.NoAnimation || (config.AnimPreference == AnimationPreference.AnimationPreferred && HardwareProfile.IsLowEndDevice()))
			{
				this.baseRenderer.Render(config);
				return;
			}
			this.materialCopies = new List<Material>();
			Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
			Shader shader = Service.AssetManager.Shaders.GetShader(config.buildingEquipmentShaderName);
			AssetMeshDataMonoBehaviour component = config.MainAsset.GetComponent<AssetMeshDataMonoBehaviour>();
			if (component != null)
			{
				int count = component.SelectableGameObjects.Count;
				for (int i = 0; i < count; i++)
				{
					Renderer component2 = component.SelectableGameObjects[i].GetComponent<Renderer>();
					if (component2.sharedMaterial == null)
					{
						Service.Logger.ErrorFormat("sharedMaterial missing on {0}", new object[]
						{
							component.SelectableGameObjects[i].name
						});
					}
					else if (dictionary.ContainsKey(component2.sharedMaterial))
					{
						component2.sharedMaterial = dictionary[component2.sharedMaterial];
					}
					else
					{
						Material material = UnityUtils.EnsureMaterialCopy(component2);
						this.materialCopies.Add(material);
						dictionary.Add(component2.sharedMaterial, material);
						material.shader = shader;
					}
				}
			}
			dictionary.Clear();
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
			if (this.materialCopies != null)
			{
				int i = 0;
				int count = this.materialCopies.Count;
				while (i < count)
				{
					UnityUtils.DestroyMaterial(this.materialCopies[i]);
					i++;
				}
				this.materialCopies.Clear();
			}
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
