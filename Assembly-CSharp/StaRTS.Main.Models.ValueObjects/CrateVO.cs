using StaRTS.Utils.MetaData;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CrateVO : IValueObject, IGeometryVO
	{
		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_bundleName
		{
			get;
			private set;
		}

		public static int COLUMN_vfxAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_iconCameraPosition
		{
			get;
			private set;
		}

		public static int COLUMN_iconLookatPosition
		{
			get;
			private set;
		}

		public static int COLUMN_iconCloseupCameraPosition
		{
			get;
			private set;
		}

		public static int COLUMN_iconCloseupLookatPosition
		{
			get;
			private set;
		}

		public static int COLUMN_iconAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_iconBundleName
		{
			get;
			private set;
		}

		public static int COLUMN_crystals
		{
			get;
			private set;
		}

		public static int COLUMN_purchasable
		{
			get;
			private set;
		}

		public static int COLUMN_supplyPoolUid
		{
			get;
			private set;
		}

		public static int COLUMN_expirationTime
		{
			get;
			private set;
		}

		public static int COLUMN_storeVisibilityConditions
		{
			get;
			private set;
		}

		public static int COLUMN_storePurchasableConditions
		{
			get;
			private set;
		}

		public static int COLUMN_flyoutEmpireItems
		{
			get;
			private set;
		}

		public static int COLUMN_flyoutRebelItems
		{
			get;
			private set;
		}

		public static int COLUMN_rewardAnimAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_crateLandTime
		{
			get;
			private set;
		}

		public static int COLUMN_empireLEIUid
		{
			get;
			private set;
		}

		public static int COLUMN_rebelLEIUid
		{
			get;
			private set;
		}

		public static int COLUMN_holoCameraPosition
		{
			get;
			private set;
		}

		public static int COLUMN_holoLookatPosition
		{
			get;
			private set;
		}

		public static int COLUMN_holoParticleEffect
		{
			get;
			private set;
		}

		public static int COLUMN_holoCrateShadow
		{
			get;
			private set;
		}

		public static int COLUMN_uiColor
		{
			get;
			private set;
		}

		public static int COLUMN_titleString
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			private set;
		}

		public string BundleName
		{
			get;
			private set;
		}

		public string VfxAssetName
		{
			get;
			private set;
		}

		public Vector3 IconCameraPosition
		{
			get;
			set;
		}

		public Vector3 IconLookatPosition
		{
			get;
			set;
		}

		public Vector3 IconCloseupCameraPosition
		{
			get;
			set;
		}

		public Vector3 IconCloseupLookatPosition
		{
			get;
			set;
		}

		public Vector3 HoloNetIconCameraPostion
		{
			get;
			set;
		}

		public Vector3 HoloNetIconLookAtPostion
		{
			get;
			set;
		}

		public string IconAssetName
		{
			get;
			set;
		}

		public string IconBundleName
		{
			get;
			set;
		}

		public float IconRotationSpeed
		{
			get;
			set;
		}

		public int Crystals
		{
			get;
			private set;
		}

		public bool Purchasable
		{
			get;
			private set;
		}

		public string[] SupplyPoolUIDs
		{
			get;
			private set;
		}

		public uint ExpirationTimeSec
		{
			get;
			private set;
		}

		public string[] StoreVisibilityConditions
		{
			get;
			set;
		}

		public string[] StorePurchasableConditions
		{
			get;
			set;
		}

		public string[] FlyoutEmpireItems
		{
			get;
			private set;
		}

		public string[] FlyoutRebelItems
		{
			get;
			private set;
		}

		public string RewardAnimationAssetName
		{
			get;
			private set;
		}

		public float CrateRewardAnimLandTime
		{
			get;
			private set;
		}

		public string EmpireLEIUid
		{
			get;
			private set;
		}

		public string RebelLEIUid
		{
			get;
			private set;
		}

		public string HoloParticleEffectId
		{
			get;
			private set;
		}

		public string HoloCrateShadowTextureName
		{
			get;
			private set;
		}

		public string UIColor
		{
			get;
			private set;
		}

		public string TitleString
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(CrateVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(CrateVO.COLUMN_bundleName);
			this.VfxAssetName = row.TryGetString(CrateVO.COLUMN_vfxAssetName);
			this.IconAssetName = row.TryGetString(CrateVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(CrateVO.COLUMN_iconBundleName, this.BundleName);
			this.IconCameraPosition = row.TryGetVector3(CrateVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(CrateVO.COLUMN_iconLookatPosition);
			this.HoloNetIconCameraPostion = row.TryGetVector3(CrateVO.COLUMN_holoCameraPosition);
			this.HoloNetIconLookAtPostion = row.TryGetVector3(CrateVO.COLUMN_holoLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(CrateVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(CrateVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.Crystals = row.TryGetInt(CrateVO.COLUMN_crystals);
			this.Purchasable = row.TryGetBool(CrateVO.COLUMN_purchasable);
			this.SupplyPoolUIDs = row.TryGetStringArray(CrateVO.COLUMN_supplyPoolUid);
			this.ExpirationTimeSec = Convert.ToUInt32(row.TryGetInt(CrateVO.COLUMN_expirationTime) * 60);
			this.StoreVisibilityConditions = row.TryGetStringArray(CrateVO.COLUMN_storeVisibilityConditions);
			this.StorePurchasableConditions = row.TryGetStringArray(CrateVO.COLUMN_storePurchasableConditions);
			this.FlyoutEmpireItems = row.TryGetStringArray(CrateVO.COLUMN_flyoutEmpireItems);
			this.FlyoutRebelItems = row.TryGetStringArray(CrateVO.COLUMN_flyoutRebelItems);
			this.RewardAnimationAssetName = row.TryGetString(CrateVO.COLUMN_rewardAnimAssetName);
			this.CrateRewardAnimLandTime = row.TryGetFloat(CrateVO.COLUMN_crateLandTime);
			this.EmpireLEIUid = row.TryGetString(CrateVO.COLUMN_empireLEIUid);
			this.RebelLEIUid = row.TryGetString(CrateVO.COLUMN_rebelLEIUid);
			this.HoloParticleEffectId = row.TryGetString(CrateVO.COLUMN_holoParticleEffect);
			this.HoloCrateShadowTextureName = row.TryGetString(CrateVO.COLUMN_holoCrateShadow);
			this.UIColor = row.TryGetHexValueString(CrateVO.COLUMN_uiColor);
			this.TitleString = row.TryGetString(CrateVO.COLUMN_titleString);
		}
	}
}
