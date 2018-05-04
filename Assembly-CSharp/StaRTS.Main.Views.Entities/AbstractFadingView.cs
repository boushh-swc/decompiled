using StaRTS.Main.Models.Entities.Components;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class AbstractFadingView
	{
		protected object fadingObject;

		private float delay;

		private float fadeTime;

		private FadingDelegate onStart;

		private FadingDelegate onComplete;

		private bool startedFade;

		private float curTime;

		private List<Material> fadingMaterials;

		private Dictionary<int, Material> oldMaterials;

		private GameObject renderableGameObject;

		private MeterShaderComponent meter;

		public AbstractFadingView(object fadingObject, float delay, float fadeTime, FadingDelegate onStart, FadingDelegate onComplete)
		{
			this.fadingObject = fadingObject;
			this.delay = ((delay >= 0f) ? delay : 0f);
			this.fadeTime = ((fadeTime >= 0f) ? fadeTime : 0f);
			this.onStart = onStart;
			this.onComplete = onComplete;
			this.startedFade = false;
			this.curTime = 0f;
			this.fadingMaterials = null;
			this.oldMaterials = null;
		}

		protected void InitData(GameObject renderableGameObject, MeterShaderComponent meter)
		{
			this.renderableGameObject = renderableGameObject;
			this.meter = meter;
		}

		public bool Fade(float dt)
		{
			this.curTime += dt;
			float num = this.curTime - this.delay;
			if (num < 0f)
			{
				return false;
			}
			this.HandleStarted();
			bool flag = num >= this.fadeTime;
			num = ((!flag) ? (1f - num / this.fadeTime) : 0f);
			Color color = new Color(1f, 1f, 1f, num);
			if (!this.EnsureMaterials())
			{
				return true;
			}
			int i = 0;
			int count = this.fadingMaterials.Count;
			while (i < count)
			{
				this.SetColor(i, color);
				i++;
			}
			return flag;
		}

		private void SetColor(int i, Color color)
		{
			if (this.fadingMaterials[i] != null)
			{
				this.fadingMaterials[i].color = color;
			}
		}

		private void HandleStarted()
		{
			if (!this.startedFade)
			{
				this.startedFade = true;
				if (this.onStart != null)
				{
					this.onStart(this.fadingObject);
					this.onStart = null;
				}
			}
		}

		public void Complete()
		{
			this.HandleStarted();
			this.RestoreMaterials();
			if (this.fadingMaterials != null)
			{
				int i = 0;
				int count = this.fadingMaterials.Count;
				while (i < count)
				{
					UnityUtils.DestroyMaterial(this.fadingMaterials[i]);
					i++;
				}
				this.fadingMaterials = null;
			}
			if (this.onComplete != null)
			{
				this.onComplete(this.fadingObject);
				this.onComplete = null;
			}
		}

		private bool EnsureMaterials()
		{
			if (this.fadingMaterials != null)
			{
				return true;
			}
			string shaderName = "UnlitTexture_Fade";
			Shader shader = Service.AssetManager.Shaders.GetShader(shaderName);
			if (shader == null)
			{
				return false;
			}
			if (this.renderableGameObject == null)
			{
				return false;
			}
			Renderer[] componentsInChildren = this.renderableGameObject.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					Renderer renderer = componentsInChildren[i];
					if (renderer != null && (this.meter == null || !this.meter.GameObjectEquals(renderer.gameObject)))
					{
						Material sharedMaterial = renderer.sharedMaterial;
						Material material = UnityUtils.EnsureMaterialCopy(renderer);
						if (material != null)
						{
							if (this.oldMaterials == null)
							{
								this.oldMaterials = new Dictionary<int, Material>();
							}
							this.oldMaterials.Add(renderer.gameObject.GetInstanceID(), sharedMaterial);
							material.shader = shader;
							if (this.fadingMaterials == null)
							{
								this.fadingMaterials = new List<Material>();
							}
							this.fadingMaterials.Add(material);
						}
					}
					i++;
				}
			}
			return this.fadingMaterials != null;
		}

		private void RestoreMaterials()
		{
			if (this.oldMaterials == null)
			{
				return;
			}
			if (this.renderableGameObject == null)
			{
				return;
			}
			Renderer[] componentsInChildren = this.renderableGameObject.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					Renderer renderer = componentsInChildren[i];
					if (renderer != null && (this.meter == null || !this.meter.GameObjectEquals(renderer.gameObject)))
					{
						int instanceID = renderer.gameObject.GetInstanceID();
						if (this.oldMaterials.ContainsKey(instanceID))
						{
							renderer.sharedMaterial = this.oldMaterials[instanceID];
						}
					}
					i++;
				}
			}
			this.oldMaterials = null;
		}
	}
}
