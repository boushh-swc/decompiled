using StaRTS.Main.Models;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorShaderSwapDecorator : IProjectorRenderer
	{
		private static readonly string TOD_SHADER_SWAP_SRC = "UnlitTexture_Index0";

		private const string MISSING_MESH_RENDERER_SHARED_MATERIAL = "sharedMaterial missing on {0}";

		private const string MISSING_SKINNED_RENDERER_SHARED_MATERIAL = "skinnedMeshRenderer missing on {0}";

		private static readonly string[] TOD_SHADER_SWAP_LIST = new string[]
		{
			"Custom/Unlit_Texture_Planet_1Color",
			"Custom/Unlit_Texture_Planet_2Color",
			"Custom/Indexed/ShadowMultiplyPlanetColor_Index-1",
			"Custom/Unlit_Texture_Planet_2Color_Mask",
			"Custom/Flag",
			"Custom/TimeOffsetBlend",
			"Custom/Terrain/GroundPL_Index0",
			"Custom/Terrain/GroundPlanetaryVertexColor_Index0",
			"Custom/Terrain/GroundPlanetaryVertexColor_Index-10",
			"Custom/Indexed/ShadowMultiplyPlanetColor_Index0",
			"Custom/PL_2Color_Vertex_Alpha"
		};

		private List<Material> unsharedMaterials;

		private Shader todSwapShaderSrc;

		private Shader holoCrateSwapShaderSrc;

		private IProjectorRenderer baseRenderer;

		private bool initialized;

		private ProjectorConfig config;

		public ProjectorShaderSwapDecorator(IProjectorRenderer baseRenderer)
		{
			this.baseRenderer = baseRenderer;
			this.unsharedMaterials = new List<Material>();
		}

		public void Render(ProjectorConfig config)
		{
			if (!config.AssetReady)
			{
				return;
			}
			this.Initialize(config);
			this.baseRenderer.Render(config);
		}

		public void PostRender(ProjectorConfig config)
		{
			if (config.EnableCrateHoloShaderSwap)
			{
				UITexture projectorUITexture = this.GetProjectorUITexture();
				if (projectorUITexture != null)
				{
					projectorUITexture.shader = this.holoCrateSwapShaderSrc;
				}
			}
			this.baseRenderer.PostRender(config);
		}

		private void Initialize(ProjectorConfig config)
		{
			if (this.initialized)
			{
				return;
			}
			this.config = config;
			this.EnsureSwapShaderSrc();
			this.CreateNonSharedMaterials(config.MainAsset);
			this.initialized = true;
		}

		private void CreateNonSharedMaterials(GameObject assetGameObject)
		{
			MeshRenderer[] componentsInChildren = assetGameObject.GetComponentsInChildren<MeshRenderer>(true);
			SkinnedMeshRenderer[] componentsInChildren2 = assetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				if (componentsInChildren[i].sharedMaterial == null)
				{
					Service.Logger.ErrorFormat("sharedMaterial missing on {0}", new object[]
					{
						assetGameObject.name
					});
				}
				else if (dictionary.ContainsKey(componentsInChildren[i].sharedMaterial))
				{
					componentsInChildren[i].sharedMaterial = dictionary[componentsInChildren[i].sharedMaterial];
				}
				else
				{
					Material material = UnityUtils.CreateMaterial(componentsInChildren[i].sharedMaterial.shader);
					material.CopyPropertiesFromMaterial(componentsInChildren[i].sharedMaterial);
					dictionary.Add(componentsInChildren[i].sharedMaterial, material);
					componentsInChildren[i].material = material;
					this.unsharedMaterials.Add(material);
					this.AttemptShaderSwapIfNeeded(material);
				}
				i++;
			}
			int j = 0;
			int num2 = componentsInChildren2.Length;
			while (j < num2)
			{
				if (componentsInChildren2[j].sharedMaterial == null)
				{
					Service.Logger.ErrorFormat("skinnedMeshRenderer missing on {0}", new object[]
					{
						assetGameObject.name
					});
				}
				else if (dictionary.ContainsKey(componentsInChildren2[j].sharedMaterial))
				{
					componentsInChildren2[j].sharedMaterial = dictionary[componentsInChildren2[j].sharedMaterial];
				}
				else
				{
					Material material2 = UnityUtils.CreateMaterial(componentsInChildren2[j].sharedMaterial.shader);
					material2.CopyPropertiesFromMaterial(componentsInChildren2[j].sharedMaterial);
					dictionary.Add(componentsInChildren2[j].sharedMaterial, material2);
					componentsInChildren2[j].material = material2;
					this.unsharedMaterials.Add(material2);
					this.AttemptShaderSwapIfNeeded(material2);
				}
				j++;
			}
			dictionary.Clear();
		}

		public void Destroy()
		{
			this.baseRenderer.Destroy();
			this.baseRenderer = null;
			int i = 0;
			int count = this.unsharedMaterials.Count;
			while (i < count)
			{
				UnityUtils.DestroyMaterial(this.unsharedMaterials[i]);
				i++;
			}
			this.unsharedMaterials.Clear();
		}

		private void EnsureSwapShaderSrc()
		{
			if (this.todSwapShaderSrc == null)
			{
				this.todSwapShaderSrc = (Resources.Load(ProjectorShaderSwapDecorator.TOD_SHADER_SWAP_SRC) as Shader);
			}
			if (this.config.EnableCrateHoloShaderSwap && this.holoCrateSwapShaderSrc == null)
			{
				this.holoCrateSwapShaderSrc = Service.AssetManager.Shaders.GetShader("HologramScanlines");
			}
		}

		private void AttemptShaderSwapIfNeeded(Material material)
		{
			if (GameConstants.TIME_OF_DAY_ENABLED)
			{
				GameUtils.SwapShaderIfNeeded(ProjectorShaderSwapDecorator.TOD_SHADER_SWAP_LIST, this.todSwapShaderSrc, material);
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
