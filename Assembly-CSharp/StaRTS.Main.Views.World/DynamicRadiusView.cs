using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class DynamicRadiusView : IViewFrameTimeObserver
	{
		private const string RADIUS_KEY = "dynamicRadius";

		private const float AVERAGE_MAX_RADIUS = 15f;

		private const float TRIGGER_WIDTH = 0.4f;

		private const float DEPTH_DELTA = 0.01f;

		private const float ROTATION_FACTOR = 4f;

		private const string EFFECT_TEXTURE_UID = "effect_radius";

		private const float EFFECT_ANIMATION_SPEED = 0f;

		private const float EFFECT_TINT = 0.9f;

		private const float EFFECT_MATERIAL_TILING = 1f;

		private const string TRIGGER_TEXTURE_UID = "trigger_radius";

		private const float TRIGGER_ANIMATION_SPEED = 0f;

		private const float TRIGGER_TINT = 1f;

		private const float TRIGGER_TILING = 1f;

		private const float TRIGGER_SCALE = 0.8f;

		private const float TRIGGER_OFFSET = 0.1f;

		private float inverseSpeedFactor;

		private Color EFFECT_COLOR = new Color(1f, 0f, 0f, 0.9f);

		private Color TRIGGER_COLOR = new Color(0.4f, 0f, 0f, 0.7f);

		private static int LastIdentity;

		private int identity;

		private GameObject radiusParent;

		private GameObject triggerRadius;

		private GameObject effectRadius;

		private Material effectMaterial;

		private Texture2D effectTexture;

		private AssetHandle effectHandle;

		private Material triggerMaterial;

		private Texture2D triggerTexture;

		private AssetHandle triggerHandle;

		private GameObjectViewComponent govc;

		private bool showing;

		private RadiusView particleView;

		public DynamicRadiusView()
		{
			this.identity = ++DynamicRadiusView.LastIdentity;
			this.particleView = new RadiusView();
			this.radiusParent = new GameObject("RadiusView_" + this.identity);
			this.triggerRadius = new GameObject("Trigger_" + this.identity);
			this.effectRadius = new GameObject("Effect_" + this.identity);
			this.triggerRadius.transform.parent = this.radiusParent.transform;
			this.effectRadius.transform.parent = this.radiusParent.transform;
			this.HideHighlight();
		}

		public static void StaticReset()
		{
			DynamicRadiusView.LastIdentity = 0;
		}

		public void ShowHighlight(Entity entity)
		{
			SmartEntity smartEntity = (SmartEntity)entity;
			if (smartEntity.GameObjectViewComp == null)
			{
				return;
			}
			uint num = 0u;
			uint minRange = 0u;
			this.DetermineEffectRange(smartEntity, ref num, ref minRange);
			this.GenerateEffectRadius(num, minRange);
			uint num2 = 0u;
			this.DetermineTriggerRange(smartEntity, ref num2);
			this.GenerateTriggerRadius(num2);
			this.inverseSpeedFactor = ((num2 <= 0u) ? 0f : (15f / num2));
			this.UpdateDepths(num, num2);
			this.radiusParent.SetActive(true);
			this.govc = smartEntity.GameObjectViewComp;
			this.govc.AttachGameObject("dynamicRadius", this.radiusParent, new Vector3(0f, 0.04f, 0f), true, false);
			this.showing = true;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.particleView.ShowHighlight(entity);
		}

		private void GenerateEffectRadius(uint maxRange, uint minRange)
		{
			float num = (15f - minRange) / 15f;
			num = Mathf.Max(num, 0.1f);
			CircleMeshUtils.AddCircleMesh(this.effectRadius, Units.BoardToWorldX(minRange), Units.BoardToWorldX(maxRange - minRange), num);
			this.GenerateEffectMaterial();
		}

		private void GenerateTriggerRadius(uint range)
		{
			CircleMeshUtils.AddCircleMesh(this.triggerRadius, Units.BoardToWorldX(range), 0.4f, 1f);
			this.GenerateTriggerMaterial();
		}

		private void GenerateEffectMaterial()
		{
			if (this.effectMaterial == null)
			{
				string shaderName = "Scroll_XY_Alpha";
				Shader shader = Service.AssetManager.Shaders.GetShader(shaderName);
				if (shader == null)
				{
					return;
				}
				this.effectMaterial = UnityUtils.CreateMaterial(shader);
				this.effectRadius.GetComponent<Renderer>().sharedMaterial = this.effectMaterial;
				this.effectMaterial.color = this.EFFECT_COLOR;
				this.effectMaterial.SetFloat("_tint", 0.9f);
				this.effectMaterial.SetFloat("_SpeedY", 0f);
				if (this.effectTexture != null)
				{
					this.AssignEffectMaterial();
				}
				else if (this.effectHandle == AssetHandle.Invalid)
				{
					this.effectRadius.SetActive(false);
					TextureVO textureVO = Service.StaticDataController.Get<TextureVO>("effect_radius");
					Service.AssetManager.Load(ref this.effectHandle, textureVO.AssetName, new AssetSuccessDelegate(this.OnTextureLoaded), null, this.effectMaterial);
				}
			}
		}

		private void GenerateTriggerMaterial()
		{
			if (this.triggerMaterial == null)
			{
				string shaderName = "Scroll_XY_Alpha";
				Shader shader = Service.AssetManager.Shaders.GetShader(shaderName);
				if (shader == null)
				{
					return;
				}
				this.triggerMaterial = UnityUtils.CreateMaterial(shader);
				this.triggerRadius.GetComponent<Renderer>().sharedMaterial = this.triggerMaterial;
				this.triggerMaterial.color = this.TRIGGER_COLOR;
				this.triggerMaterial.SetFloat("_tint", 1f);
				this.triggerMaterial.SetFloat("_SpeedX", 0f);
				if (this.triggerTexture != null)
				{
					this.AssignTriggerMaterial();
				}
				else if (this.triggerHandle == AssetHandle.Invalid)
				{
					this.triggerRadius.SetActive(false);
					TextureVO textureVO = Service.StaticDataController.Get<TextureVO>("trigger_radius");
					Service.AssetManager.Load(ref this.triggerHandle, textureVO.AssetName, new AssetSuccessDelegate(this.OnTextureLoaded), null, this.triggerMaterial);
				}
			}
		}

		private void OnTextureLoaded(object asset, object cookie)
		{
			Texture2D texture2D = (Texture2D)asset;
			Material x = (Material)cookie;
			if (x == this.effectMaterial)
			{
				this.effectTexture = texture2D;
				this.effectRadius.SetActive(true);
				this.AssignEffectMaterial();
			}
			else if (x == this.triggerMaterial)
			{
				this.triggerTexture = texture2D;
				this.triggerRadius.SetActive(true);
				this.AssignTriggerMaterial();
			}
		}

		private void AssignEffectMaterial()
		{
			this.effectMaterial.mainTexture = this.effectTexture;
			this.effectMaterial.mainTextureScale = new Vector2(1f, 1f);
		}

		private void AssignTriggerMaterial()
		{
			this.triggerMaterial.mainTexture = this.triggerTexture;
			this.triggerMaterial.mainTextureScale = new Vector2(1f, 0.8f);
			this.triggerMaterial.mainTextureOffset = new Vector2(0f, 0.1f);
		}

		private void UpdateDepths(uint effectRange, uint triggerRange)
		{
			if (effectRange > triggerRange)
			{
				this.effectRadius.transform.localPosition = Vector3.zero;
				this.triggerRadius.transform.localPosition = new Vector3(0f, 0.01f, 0f);
			}
			else
			{
				this.effectRadius.transform.localPosition = new Vector3(0f, 0.01f, 0f);
				this.triggerRadius.transform.localPosition = Vector3.zero;
			}
		}

		public void HideHighlight()
		{
			if (!this.showing)
			{
				return;
			}
			this.showing = false;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			if (this.govc != null)
			{
				this.govc.DetachGameObject("dynamicRadius");
				this.govc = null;
			}
			this.radiusParent.SetActive(false);
			this.particleView.HideHighlight();
		}

		public void HideHighlight(Entity highlightedEntity)
		{
			if (this.govc != null && this.govc.Entity == highlightedEntity)
			{
				this.HideHighlight();
			}
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.effectRadius);
			UnityEngine.Object.Destroy(this.triggerRadius);
			UnityEngine.Object.Destroy(this.radiusParent);
			this.particleView.Destroy();
			this.particleView = null;
			UnityEngine.Object.Destroy(this.effectTexture);
			UnityEngine.Object.Destroy(this.triggerTexture);
			UnityEngine.Object.Destroy(this.effectMaterial);
			UnityEngine.Object.Destroy(this.triggerMaterial);
			AssetManager assetManager = Service.AssetManager;
			if (this.effectHandle != AssetHandle.Invalid)
			{
				assetManager.Unload(this.effectHandle);
				this.effectHandle = AssetHandle.Invalid;
			}
			if (this.triggerHandle != AssetHandle.Invalid)
			{
				assetManager.Unload(this.triggerHandle);
				this.triggerHandle = AssetHandle.Invalid;
			}
		}

		private void DetermineEffectRange(SmartEntity entity, ref uint maxRange, ref uint minRange)
		{
			if (entity.BuildingComp == null)
			{
				return;
			}
			if (entity.ShooterComp != null)
			{
				maxRange = entity.ShooterComp.ShooterVO.MaxAttackRange;
				minRange = entity.ShooterComp.ShooterVO.MinAttackRange;
				return;
			}
			if (entity.TrapComp != null)
			{
				maxRange = TrapUtils.GetTrapAttackRadius(entity.TrapComp.Type);
				return;
			}
		}

		private void DetermineTriggerRange(SmartEntity entity, ref uint triggerRange)
		{
			if (entity.BuildingComp == null)
			{
				return;
			}
			BuildingTypeVO buildingType = entity.BuildingComp.BuildingType;
			if (buildingType.Type == BuildingType.Squad || buildingType.Type == BuildingType.ChampionPlatform)
			{
				triggerRange = buildingType.ActivationRadius;
				return;
			}
			if (entity.ShooterComp != null)
			{
				triggerRange = entity.ShooterComp.ShooterVO.MaxAttackRange;
				return;
			}
			if (entity.TrapComp != null)
			{
				triggerRange = TrapUtils.GetTrapMaxRadius(entity.TrapComp.Type);
				return;
			}
		}

		public void OnViewFrameTime(float dt)
		{
			Vector3 eulerAngles = new Vector3(0f, dt * (4f * this.inverseSpeedFactor), 0f);
			this.effectRadius.transform.Rotate(eulerAngles);
			this.triggerRadius.transform.Rotate(eulerAngles);
		}
	}
}
