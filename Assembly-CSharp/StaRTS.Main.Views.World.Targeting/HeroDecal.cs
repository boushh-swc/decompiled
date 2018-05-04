using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World.Targeting
{
	public class HeroDecal : IViewFrameTimeObserver
	{
		private const string VIEW_NAME = "HeroDecal {0}";

		private const float FADE_TIME = 1.5f;

		private TroopTypeVO hero;

		private AssetHandle heroHandle;

		private string name;

		private float scale;

		private Material material;

		private GameObject view;

		private bool loaded;

		private bool fading;

		private float fadingTime;

		public Entity Entity
		{
			get;
			private set;
		}

		public HeroDecal(Entity entity)
		{
			this.Entity = entity;
			string uid = ((SmartEntity)entity).TroopComp.TroopType.Uid;
			this.hero = Service.StaticDataController.Get<TroopTypeVO>(uid);
			this.name = string.Format("HeroDecal {0}", this.hero.Uid);
			this.scale = this.hero.DecalSize;
			this.material = null;
			this.view = null;
			this.loaded = false;
			this.fading = false;
			this.fadingTime = 0f;
			if (!string.IsNullOrEmpty(this.hero.DecalAssetName))
			{
				Service.AssetManager.Load(ref this.heroHandle, this.hero.DecalAssetName, new AssetSuccessDelegate(this.OnLoad), null, null);
			}
		}

		public void Cleanup()
		{
			if (this.Entity == null)
			{
				return;
			}
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			GameObjectViewComponent gameObjectViewComponent = this.Entity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent != null)
			{
				gameObjectViewComponent.DetachGameObject(this.name);
			}
			if (this.material != null)
			{
				UnityUtils.DestroyMaterial(this.material);
				this.material = null;
			}
			if (this.view != null)
			{
				UnityEngine.Object.Destroy(this.view);
				this.view = null;
			}
			if (this.heroHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.heroHandle);
				this.heroHandle = AssetHandle.Invalid;
			}
			this.Entity = null;
			this.hero = null;
			this.name = null;
			this.loaded = false;
			this.fading = false;
		}

		private void OnLoad(object asset, object cookie)
		{
			if (asset is GameObject)
			{
				this.view = UnityEngine.Object.Instantiate<GameObject>(asset as GameObject);
				this.view.name = this.name;
				this.view.SetActive(false);
				this.TrySetupView();
			}
		}

		public static void SetupDecal(GameObject decal, float scale)
		{
			float num = scale * 3f;
			decal.transform.localScale = new Vector3(num, num, 1f);
			decal.transform.localEulerAngles = new Vector3(90f, 45f, 0f);
		}

		public void TrySetupView()
		{
			if (this.loaded)
			{
				return;
			}
			if (this.view == null)
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComponent = this.Entity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null)
			{
				return;
			}
			HeroDecal.SetupDecal(this.view, this.scale);
			this.view.SetActive(true);
			gameObjectViewComponent.AttachGameObject(this.name, this.view, Vector3.zero, true, true);
			this.loaded = true;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			if (this.fading)
			{
				this.FadeToGray();
			}
		}

		private bool TrySetMaterial()
		{
			Renderer[] componentsInChildren = this.view.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					Renderer renderer = componentsInChildren[i];
					if (renderer != null)
					{
						Material x = UnityUtils.EnsureMaterialCopy(renderer);
						if (x != null)
						{
							this.material = x;
							this.SetMaterialSaturation(1f);
							return true;
						}
					}
					i++;
				}
			}
			return false;
		}

		private void SetMaterialSaturation(float saturation)
		{
			this.material.SetFloat("_Saturation", saturation);
		}

		public void FadeToGray()
		{
			this.fading = true;
			this.fadingTime = 0f;
		}

		public void OnViewFrameTime(float dt)
		{
			GameObjectViewComponent gameObjectViewComponent = this.Entity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent != null)
			{
				gameObjectViewComponent.UpdateAttachment(this.name);
			}
			if (this.material == null)
			{
				this.TrySetMaterial();
			}
			if (this.fading && this.material != null)
			{
				this.fadingTime += dt;
				bool flag = this.fadingTime >= 1.5f;
				if (flag)
				{
					this.fadingTime = 1.5f;
				}
				float num = this.fadingTime / 1.5f;
				float materialSaturation = 1f - num;
				this.SetMaterialSaturation(materialSaturation);
				if (flag)
				{
					this.fading = false;
					Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				}
			}
		}
	}
}
