using StaRTS.Assets;
using StaRTS.Main.Views.Cameras;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXMeshRenderer : UXElement
	{
		private MeshRenderer component;

		private AssetManager assetManager;

		private string assetName;

		private AssetHandle assetHandle;

		private Material material;

		public Texture MainTexture
		{
			get
			{
				if (this.material == null && this.component.sharedMaterial)
				{
					return this.component.sharedMaterial.mainTexture;
				}
				if (this.material != null)
				{
					return this.material.mainTexture;
				}
				return null;
			}
			set
			{
				if (this.material == null)
				{
					this.material = UnityUtils.EnsureMaterialCopy(this.component);
				}
				this.material.mainTexture = value;
			}
		}

		public UXMeshRenderer(UXCamera uxCamera, MeshRenderer component) : base(uxCamera, component.gameObject, null)
		{
			this.assetManager = Service.AssetManager;
			this.component = component;
		}

		public override void InternalDestroyComponent()
		{
			this.UnloadCurrentTexture();
			if (this.material != null)
			{
				this.material.mainTexture = null;
				UnityUtils.DestroyMaterial(this.material);
				this.material = null;
			}
			this.component = null;
		}

		public void LoadTexture(string assetName)
		{
			if (assetName == this.assetName)
			{
				return;
			}
			this.component.gameObject.SetActive(false);
			this.UnloadCurrentTexture();
			this.assetName = assetName;
			this.assetManager.Load(ref this.assetHandle, assetName, new AssetSuccessDelegate(this.OnLoadSuccess), null, null);
		}

		private void OnLoadSuccess(object asset, object cookie)
		{
			this.MainTexture = (Texture2D)asset;
			this.component.gameObject.SetActive(true);
		}

		private void UnloadCurrentTexture()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				this.assetManager.Unload(this.assetHandle);
				this.assetHandle = AssetHandle.Invalid;
				this.assetName = null;
			}
		}

		public void SetShader(string shaderName)
		{
			if (this.material == null)
			{
				this.material = UnityUtils.EnsureMaterialCopy(this.component);
			}
			Shader shader = Service.AssetManager.Shaders.GetShader(shaderName);
			if (shader == null)
			{
				shader = Shader.Find(shaderName);
			}
			if (shader == null)
			{
				Service.Logger.Error("Shader missing: '" + shaderName + "'");
				return;
			}
			this.material.shader = shader;
		}

		public void SetShaderFloat(string nameID, float value)
		{
			if (this.material == null)
			{
				this.material = UnityUtils.EnsureMaterialCopy(this.component);
			}
			this.material.SetFloat(nameID, value);
		}
	}
}
