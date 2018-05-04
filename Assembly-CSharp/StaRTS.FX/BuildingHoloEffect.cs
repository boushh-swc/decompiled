using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class BuildingHoloEffect
	{
		private const string CONE_ASSET_NAME = "fx_summon_hologram";

		private const float FX_BASE_SCALE = 6.59f;

		private const float FX_FLOWBITS_SCALE1 = 0.63f;

		private const float FX_FLOWBITS_SCALE2 = 0.67f;

		private const string LOCATOR_NAME = "locator_hologram";

		private static readonly Quaternion HOLO_ROTATION = Quaternion.Euler(new Vector3(90f, 0f, 0f));

		private Entity building;

		private GameObject coneObj;

		private AssetHandle coneAssetHandle;

		private GameObject holoObj;

		private AssetHandle holoAssetHandle;

		private int unitOrder;

		private bool useHoloRotation;

		private string locatorName;

		public bool WaitingForBuildingView
		{
			get;
			private set;
		}

		public BuildingHoloEffect(Entity building, string unitUid, bool isStarship, bool isNavCenter) : this(building)
		{
			this.CreateMobilizationHolo(unitUid, isStarship, isNavCenter);
		}

		public BuildingHoloEffect(Entity building)
		{
			this.locatorName = "locator_hologram";
			this.useHoloRotation = false;
			this.building = building;
		}

		public void CreateMobilizationHolo(string unitUid, bool isStarship, bool isNavCenter)
		{
			AssetManager assetManager = Service.AssetManager;
			if (this.coneAssetHandle == AssetHandle.Invalid && !isNavCenter)
			{
				assetManager.Load(ref this.coneAssetHandle, "fx_summon_hologram", new AssetSuccessDelegate(this.OnEffectLoaded), null, true);
			}
			if (unitUid == null)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			IDeployableVO geometry = null;
			int num;
			string text;
			if (isStarship)
			{
				SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(unitUid);
				num = specialAttackTypeVO.Order;
				text = specialAttackTypeVO.HologramUid;
				geometry = specialAttackTypeVO;
			}
			else if (isNavCenter)
			{
				text = unitUid;
				num = 2147483647;
			}
			else
			{
				TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(unitUid);
				num = troopTypeVO.Order;
				text = troopTypeVO.HologramUid;
				geometry = troopTypeVO;
			}
			if (num <= this.unitOrder)
			{
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			this.DestroyHolo();
			this.unitOrder = num;
			this.useHoloRotation = true;
			MobilizationHologramVO mobilizationHologramVO = staticDataController.Get<MobilizationHologramVO>(text);
			GeometryTag geometryTag = new GeometryTag(geometry, mobilizationHologramVO.AssetName);
			Service.EventManager.SendEvent(EventId.HologramCreated, geometryTag);
			assetManager.Load(ref this.holoAssetHandle, geometryTag.assetName, new AssetSuccessDelegate(this.OnEffectLoaded), new AssetFailureDelegate(this.OnEffectLoadFailed), false);
		}

		public void CreateGenericHolo(string holoAssetName)
		{
			this.CreateGenericHolo(holoAssetName, null);
		}

		public void CreateGenericHolo(string holoAssetName, string locatorOverride)
		{
			AssetManager assetManager = Service.AssetManager;
			this.unitOrder = 2147483647;
			if (!string.IsNullOrEmpty(locatorOverride))
			{
				this.locatorName = locatorOverride;
			}
			if (string.IsNullOrEmpty(holoAssetName))
			{
				return;
			}
			this.DestroyHolo();
			assetManager.Load(ref this.holoAssetHandle, holoAssetName, new AssetSuccessDelegate(this.OnEffectLoaded), new AssetFailureDelegate(this.OnEffectLoadFailed), false);
		}

		private void UnloadAssetHandle(ref AssetHandle assetHandle)
		{
			Service.AssetManager.Unload(assetHandle);
			assetHandle = AssetHandle.Invalid;
		}

		private void OnEffectLoaded(object asset, object cookie)
		{
			bool flag = (bool)cookie;
			GameObjectViewComponent gameObjectViewComponent = this.building.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null || gameObjectViewComponent.MainGameObject == null)
			{
				if (flag)
				{
					this.UnloadAssetHandle(ref this.coneAssetHandle);
				}
				else
				{
					this.UnloadAssetHandle(ref this.holoAssetHandle);
				}
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(asset as GameObject);
			Transform transform = GameUtils.FindAssetMetaDataTransform(gameObjectViewComponent.MainGameObject, this.locatorName);
			Transform transform2 = gameObject.transform;
			transform2.parent = transform;
			transform2.localPosition = Vector3.zero;
			if (flag)
			{
				if (transform == null)
				{
					Service.Logger.Error("Cone Holo Effect Needs Building Transform");
					return;
				}
				this.coneObj = gameObject;
				Transform child = gameObject.transform.GetChild(0);
				if (child != null)
				{
					ParticleSystem component = child.GetComponent<ParticleSystem>();
					if (component != null)
					{
						component.startSize = transform.localScale.x * 6.59f;
					}
					if (child.childCount >= 2)
					{
						Transform child2 = child.GetChild(0);
						if (child2 != null)
						{
							ParticleSystem component2 = child2.GetComponent<ParticleSystem>();
							if (component2 != null)
							{
								component2.startSize = transform.localScale.x;
							}
						}
						Transform child3 = child.GetChild(1);
						if (child3 != null)
						{
							ParticleSystem component3 = child3.GetComponent<ParticleSystem>();
							if (component3 != null)
							{
								Rand rand = Service.Rand;
								component3.startSize = transform.localScale.x * rand.ViewRangeFloat(0.63f, 0.67f);
							}
						}
					}
				}
			}
			else
			{
				this.holoObj = gameObject;
				if (this.useHoloRotation)
				{
					transform2.localRotation = BuildingHoloEffect.HOLO_ROTATION;
				}
			}
			this.WaitingForBuildingView = false;
		}

		private void OnEffectLoadFailed(object cookie)
		{
			this.unitOrder = 0;
			this.WaitingForBuildingView = false;
		}

		public void TransferEffect(Entity newBuilding)
		{
			this.building = newBuilding;
			if (this.coneObj == null)
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComponent = newBuilding.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null || gameObjectViewComponent.MainGameObject == null)
			{
				this.WaitingForBuildingView = true;
				this.OrphanEffects();
			}
			else
			{
				this.UpdateEffect();
			}
		}

		public void UpdateEffect()
		{
			if (this.coneObj == null)
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComponent = this.building.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null || gameObjectViewComponent.MainGameObject == null)
			{
				return;
			}
			this.WaitingForBuildingView = false;
			Transform transform = gameObjectViewComponent.MainTransform.FindChild("locator_hologram");
			if (!this.coneObj.activeSelf)
			{
				this.coneObj.SetActive(true);
			}
			Transform transform2 = this.coneObj.transform;
			transform2.parent = transform;
			transform2.localPosition = Vector3.zero;
			if (this.holoObj != null)
			{
				if (!this.holoObj.activeSelf)
				{
					this.holoObj.SetActive(true);
				}
				Transform transform3 = this.holoObj.transform;
				Vector3 localScale = transform3.localScale;
				transform3.parent = transform;
				transform3.localPosition = Vector3.zero;
				transform3.localRotation = BuildingHoloEffect.HOLO_ROTATION;
				Vector3 localScale2 = transform.localScale;
				transform3.localScale = new Vector3(localScale.x / localScale2.x, localScale.y / localScale2.y, localScale.z / localScale2.z);
			}
		}

		private void OrphanEffects()
		{
			if (this.coneObj != null)
			{
				Transform transform = this.coneObj.transform;
				transform.parent = null;
				this.coneObj.SetActive(false);
			}
			if (this.holoObj != null)
			{
				Transform transform2 = this.holoObj.transform;
				transform2.parent = null;
				this.holoObj.SetActive(false);
			}
		}

		private void DestroyHoloCone()
		{
			if (this.coneObj != null)
			{
				UnityEngine.Object.Destroy(this.coneObj);
				this.coneObj = null;
			}
			if (this.coneAssetHandle != AssetHandle.Invalid)
			{
				this.UnloadAssetHandle(ref this.coneAssetHandle);
			}
		}

		private void DestroyHolo()
		{
			if (this.holoObj != null)
			{
				UnityEngine.Object.Destroy(this.holoObj);
				this.holoObj = null;
			}
			if (this.holoAssetHandle != AssetHandle.Invalid)
			{
				this.UnloadAssetHandle(ref this.holoAssetHandle);
			}
		}

		public void Cleanup()
		{
			this.DestroyHoloCone();
			this.DestroyHolo();
			this.unitOrder = 0;
		}
	}
}
