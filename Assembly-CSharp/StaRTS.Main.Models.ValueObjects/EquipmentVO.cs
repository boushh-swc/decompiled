using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EquipmentVO : IAssetVO, IGeometryVO, IUnlockableVO, IUpgradeableVO, IValueObject
	{
		public static int COLUMN_equipmentID
		{
			get;
			private set;
		}

		public static int COLUMN_lvl
		{
			get;
			private set;
		}

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_planetIDs
		{
			get;
			private set;
		}

		public static int COLUMN_effectUids
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeShards
		{
			get;
			private set;
		}

		public static int COLUMN_size
		{
			get;
			private set;
		}

		public static int COLUMN_equipmentName
		{
			get;
			private set;
		}

		public static int COLUMN_equipmentDescription
		{
			get;
			private set;
		}

		public static int COLUMN_skins
		{
			get;
			private set;
		}

		public static int COLUMN_iconUnlockScale
		{
			get;
			private set;
		}

		public static int COLUMN_iconUnlockRotation
		{
			get;
			private set;
		}

		public static int COLUMN_iconUnlockPosition
		{
			get;
			private set;
		}

		public static int COLUMN_quality
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeTime
		{
			get;
			private set;
		}

		public static int COLUMN_buildingID
		{
			get;
			private set;
		}

		public static int COLUMN_order
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string EquipmentID
		{
			get;
			private set;
		}

		public FactionType Faction
		{
			get;
			private set;
		}

		public string[] PlanetIDs
		{
			get;
			private set;
		}

		public string[] EffectUids
		{
			get;
			private set;
		}

		public int UpgradeShards
		{
			get;
			protected set;
		}

		public string EquipmentName
		{
			get;
			set;
		}

		public string EquipmentDescription
		{
			get;
			set;
		}

		public string[] Skins
		{
			get;
			set;
		}

		public ShardQuality Quality
		{
			get;
			set;
		}

		public string BuildingID
		{
			get;
			set;
		}

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

		public static int COLUMN_iconRotationSpeed
		{
			get;
			private set;
		}

		public string AssetName
		{
			get;
			set;
		}

		public string BundleName
		{
			get;
			set;
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

		public Vector3 IconUnlockScale
		{
			get;
			set;
		}

		public Vector3 IconUnlockRotation
		{
			get;
			set;
		}

		public Vector3 IconUnlockPosition
		{
			get;
			set;
		}

		public string IconBundleName
		{
			get;
			set;
		}

		public string IconAssetName
		{
			get;
			set;
		}

		public float IconRotationSpeed
		{
			get;
			set;
		}

		public static int COLUMN_playerFacing
		{
			get;
			private set;
		}

		public static int COLUMN_requirements
		{
			get;
			private set;
		}

		public int Lvl
		{
			get;
			set;
		}

		public int Order
		{
			get;
			set;
		}

		public int Size
		{
			get;
			set;
		}

		public int UpgradeCredits
		{
			get;
			set;
		}

		public int UpgradeMaterials
		{
			get;
			set;
		}

		public int UpgradeContraband
		{
			get;
			set;
		}

		public int UpgradeTime
		{
			get;
			set;
		}

		public string UpgradeGroup
		{
			get
			{
				return this.EquipmentID;
			}
		}

		public bool PlayerFacing
		{
			get;
			private set;
		}

		public string BuildingRequirement
		{
			get;
			set;
		}

		public bool UnlockedByEvent
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.EquipmentID = row.TryGetString(EquipmentVO.COLUMN_equipmentID);
			this.Lvl = row.TryGetInt(EquipmentVO.COLUMN_lvl);
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(EquipmentVO.COLUMN_faction));
			this.PlanetIDs = row.TryGetStringArray(EquipmentVO.COLUMN_planetIDs);
			this.EffectUids = row.TryGetStringArray(EquipmentVO.COLUMN_effectUids);
			this.UpgradeShards = row.TryGetInt(EquipmentVO.COLUMN_upgradeShards);
			this.Size = row.TryGetInt(EquipmentVO.COLUMN_size);
			this.Quality = StringUtils.ParseEnum<ShardQuality>(row.TryGetString(EquipmentVO.COLUMN_quality));
			this.IconUnlockScale = row.TryGetVector3(EquipmentVO.COLUMN_iconUnlockScale, Vector3.one);
			this.IconUnlockRotation = row.TryGetVector3(EquipmentVO.COLUMN_iconUnlockRotation, Vector3.zero);
			this.IconUnlockPosition = row.TryGetVector3(EquipmentVO.COLUMN_iconUnlockPosition, Vector3.zero);
			this.UpgradeTime = row.TryGetInt(EquipmentVO.COLUMN_upgradeTime);
			this.BuildingID = row.TryGetString(EquipmentVO.COLUMN_buildingID);
			this.AssetName = row.TryGetString(EquipmentVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(EquipmentVO.COLUMN_bundleName);
			this.IconAssetName = row.TryGetString(EquipmentVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(EquipmentVO.COLUMN_iconBundleName, this.BundleName);
			this.IconCameraPosition = row.TryGetVector3(EquipmentVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(EquipmentVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(EquipmentVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(EquipmentVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.IconRotationSpeed = row.TryGetFloat(EquipmentVO.COLUMN_iconRotationSpeed);
			this.PlayerFacing = row.TryGetBool(EquipmentVO.COLUMN_playerFacing);
			string[] array = row.TryGetStringArray(EquipmentVO.COLUMN_requirements);
			this.BuildingRequirement = ((array != null && array.Length != 0) ? array[0] : null);
			this.EquipmentName = row.TryGetString(EquipmentVO.COLUMN_equipmentName, this.BundleName);
			this.EquipmentDescription = row.TryGetString(EquipmentVO.COLUMN_equipmentDescription, this.BundleName);
			this.Skins = row.TryGetStringArray(EquipmentVO.COLUMN_skins);
			this.Order = row.TryGetInt(EquipmentVO.COLUMN_order);
		}
	}
}
