using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class BuildingTypeVO : IAssetVO, IAudioVO, IGeometryVO, IHealthVO, IUnlockableVO, IUpgradeableVO, IValueObject
	{
		public ArmorType ArmorType;

		public string BuildingID;

		public int Credits;

		public int Materials;

		public int Contraband;

		public int SwapCredits;

		public int SwapMaterials;

		public int SwapContraband;

		public int SwapTime;

		public int CycleTime;

		public int Produce;

		public int SizeX;

		public int SizeY;

		public int Storage;

		public int Xp;

		public CurrencyType Currency;

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

		public static int COLUMN_armorType
		{
			get;
			private set;
		}

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_buildingID
		{
			get;
			private set;
		}

		public static int COLUMN_type
		{
			get;
			private set;
		}

		public static int COLUMN_subType
		{
			get;
			private set;
		}

		public static int COLUMN_storeTab
		{
			get;
			private set;
		}

		public static int COLUMN_credits
		{
			get;
			private set;
		}

		public static int COLUMN_materials
		{
			get;
			private set;
		}

		public static int COLUMN_contraband
		{
			get;
			private set;
		}

		public static int COLUMN_crossCredits
		{
			get;
			private set;
		}

		public static int COLUMN_crossMaterials
		{
			get;
			private set;
		}

		public static int COLUMN_crossContraband
		{
			get;
			private set;
		}

		public static int COLUMN_crossTime
		{
			get;
			private set;
		}

		public static int COLUMN_cycleTime
		{
			get;
			private set;
		}

		public static int COLUMN_collectNotify
		{
			get;
			private set;
		}

		public static int COLUMN_time
		{
			get;
			private set;
		}

		public static int COLUMN_health
		{
			get;
			private set;
		}

		public static int COLUMN_shieldHealthPoints
		{
			get;
			private set;
		}

		public static int COLUMN_shieldRangePoints
		{
			get;
			private set;
		}

		public static int COLUMN_produce
		{
			get;
			private set;
		}

		public static int COLUMN_hideIfLocked
		{
			get;
			private set;
		}

		public static int COLUMN_requirements
		{
			get;
			private set;
		}

		public static int COLUMN_requirements2
		{
			get;
			private set;
		}

		public static int COLUMN_unlockedByEvent
		{
			get;
			private set;
		}

		public static int COLUMN_linkedUnit
		{
			get;
			private set;
		}

		public static int COLUMN_maxQuantity
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

		public static int COLUMN_order
		{
			get;
			private set;
		}

		public static int COLUMN_stashOrder
		{
			get;
			private set;
		}

		public static int COLUMN_sizex
		{
			get;
			private set;
		}

		public static int COLUMN_sizey
		{
			get;
			private set;
		}

		public static int COLUMN_spawnProtect
		{
			get;
			private set;
		}

		public static int COLUMN_allowDefensiveSpawn
		{
			get;
			private set;
		}

		public static int COLUMN_xp
		{
			get;
			private set;
		}

		public static int COLUMN_storage
		{
			get;
			private set;
		}

		public static int COLUMN_currency
		{
			get;
			private set;
		}

		public static int COLUMN_audioDeath
		{
			get;
			private set;
		}

		public static int COLUMN_audioPlacement
		{
			get;
			private set;
		}

		public static int COLUMN_audioCharge
		{
			get;
			private set;
		}

		public static int COLUMN_audioAttack
		{
			get;
			private set;
		}

		public static int COLUMN_audioImpact
		{
			get;
			private set;
		}

		public static int COLUMN_turretId
		{
			get;
			private set;
		}

		public static int COLUMN_trapID
		{
			get;
			private set;
		}

		public static int COLUMN_activationRadius
		{
			get;
			private set;
		}

		public static int COLUMN_fillStateAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_fillStateBundleName
		{
			get;
			private set;
		}

		public static int COLUMN_destructFX
		{
			get;
			private set;
		}

		public static int COLUMN_tooltipHeightOffset
		{
			get;
			private set;
		}

		public static int COLUMN_buffAssetOffset
		{
			get;
			private set;
		}

		public static int COLUMN_buffAssetBaseOffset
		{
			get;
			private set;
		}

		public static int COLUMN_lvl
		{
			get;
			private set;
		}

		public static int COLUMN_connectors
		{
			get;
			private set;
		}

		public static int COLUMN_forceShowReticle
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public FactionType Faction
		{
			get;
			set;
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

		public string TrackerName
		{
			get;
			set;
		}

		public BuildingType Type
		{
			get;
			private set;
		}

		public BuildingSubType SubType
		{
			get;
			private set;
		}

		public StoreTab StoreTab
		{
			get;
			set;
		}

		public int CollectNotify
		{
			get;
			set;
		}

		public int Time
		{
			get;
			set;
		}

		public int Health
		{
			get;
			set;
		}

		public int ShieldHealthPoints
		{
			get;
			set;
		}

		public int ShieldRangePoints
		{
			get;
			set;
		}

		public bool HideIfLocked
		{
			get;
			private set;
		}

		public bool PlayerFacing
		{
			get
			{
				return true;
			}
		}

		public string BuildingRequirement
		{
			get;
			set;
		}

		public string BuildingRequirement2
		{
			get;
			private set;
		}

		public bool UnlockedByEvent
		{
			get;
			set;
		}

		public int MaxQuantity
		{
			get;
			private set;
		}

		public string LinkedUnit
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

		public float IconRotationSpeed
		{
			get;
			set;
		}

		public List<StrIntPair> AudioPlacement
		{
			get;
			set;
		}

		public List<StrIntPair> AudioDeath
		{
			get;
			set;
		}

		public List<StrIntPair> AudioMovement
		{
			get;
			set;
		}

		public List<StrIntPair> AudioMovementAway
		{
			get;
			set;
		}

		public List<StrIntPair> AudioTrain
		{
			get;
			set;
		}

		public List<StrIntPair> AudioCharge
		{
			get;
			set;
		}

		public List<StrIntPair> AudioAttack
		{
			get;
			set;
		}

		public List<StrIntPair> AudioImpact
		{
			get;
			set;
		}

		public string DestructFX
		{
			get;
			set;
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

		public int StashOrder
		{
			get;
			set;
		}

		public int Size
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public int UpgradeCredits
		{
			get
			{
				return this.Credits;
			}
		}

		public int UpgradeMaterials
		{
			get
			{
				return this.Materials;
			}
		}

		public int UpgradeContraband
		{
			get
			{
				return this.Contraband;
			}
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
				return this.BuildingID;
			}
		}

		public BuildingConnectorTypeVO Connectors
		{
			get;
			set;
		}

		public bool IsLootable
		{
			get;
			private set;
		}

		public int SpawnProtection
		{
			get;
			set;
		}

		public bool AllowDefensiveSpawn
		{
			get;
			private set;
		}

		public string TurretUid
		{
			get;
			private set;
		}

		public string TrapUid
		{
			get;
			private set;
		}

		public uint ActivationRadius
		{
			get;
			private set;
		}

		public string FillStateAssetName
		{
			get;
			private set;
		}

		public string FillStateBundleName
		{
			get;
			private set;
		}

		public float TooltipHeightOffset
		{
			get;
			private set;
		}

		public Vector3 BuffAssetOffset
		{
			get;
			set;
		}

		public Vector3 BuffAssetBaseOffset
		{
			get;
			set;
		}

		public bool ShowReticleWhenTargeted
		{
			get;
			private set;
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

		public void ReadRow(Row row)
		{
			this.AssetName = row.TryGetString(BuildingTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(BuildingTypeVO.COLUMN_bundleName);
			this.IconAssetName = row.TryGetString(BuildingTypeVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(BuildingTypeVO.COLUMN_iconBundleName, this.BundleName);
			this.ArmorType = StringUtils.ParseEnum<ArmorType>(row.TryGetString(BuildingTypeVO.COLUMN_armorType));
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(BuildingTypeVO.COLUMN_faction));
			this.BuildingID = row.TryGetString(BuildingTypeVO.COLUMN_buildingID);
			this.Type = StringUtils.ParseEnum<BuildingType>(row.TryGetString(BuildingTypeVO.COLUMN_type));
			this.SubType = StringUtils.ParseEnum<BuildingSubType>(row.TryGetString(BuildingTypeVO.COLUMN_subType));
			this.StoreTab = StringUtils.ParseEnum<StoreTab>(row.TryGetString(BuildingTypeVO.COLUMN_storeTab));
			this.Credits = row.TryGetInt(BuildingTypeVO.COLUMN_credits);
			this.Materials = row.TryGetInt(BuildingTypeVO.COLUMN_materials);
			this.Contraband = row.TryGetInt(BuildingTypeVO.COLUMN_contraband);
			this.SwapCredits = row.TryGetInt(BuildingTypeVO.COLUMN_crossCredits);
			this.SwapMaterials = row.TryGetInt(BuildingTypeVO.COLUMN_crossMaterials);
			this.SwapContraband = row.TryGetInt(BuildingTypeVO.COLUMN_crossContraband);
			this.SwapTime = row.TryGetInt(BuildingTypeVO.COLUMN_crossTime);
			this.CycleTime = row.TryGetInt(BuildingTypeVO.COLUMN_cycleTime);
			this.CollectNotify = row.TryGetInt(BuildingTypeVO.COLUMN_collectNotify);
			this.Time = row.TryGetInt(BuildingTypeVO.COLUMN_time);
			this.Health = row.TryGetInt(BuildingTypeVO.COLUMN_health);
			this.ShieldHealthPoints = row.TryGetInt(BuildingTypeVO.COLUMN_shieldHealthPoints);
			this.ShieldRangePoints = row.TryGetInt(BuildingTypeVO.COLUMN_shieldRangePoints);
			this.Produce = row.TryGetInt(BuildingTypeVO.COLUMN_produce);
			this.HideIfLocked = row.TryGetBool(BuildingTypeVO.COLUMN_hideIfLocked);
			string[] array = row.TryGetStringArray(BuildingTypeVO.COLUMN_requirements);
			this.BuildingRequirement = ((array != null && array.Length != 0) ? array[0] : null);
			this.BuildingRequirement2 = row.TryGetString(BuildingTypeVO.COLUMN_requirements2);
			this.UnlockedByEvent = row.TryGetBool(BuildingTypeVO.COLUMN_unlockedByEvent);
			this.LinkedUnit = row.TryGetString(BuildingTypeVO.COLUMN_linkedUnit);
			this.MaxQuantity = row.TryGetInt(BuildingTypeVO.COLUMN_maxQuantity);
			this.IconCameraPosition = row.TryGetVector3(BuildingTypeVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(BuildingTypeVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(BuildingTypeVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(BuildingTypeVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.Order = row.TryGetInt(BuildingTypeVO.COLUMN_order);
			this.StashOrder = row.TryGetInt(BuildingTypeVO.COLUMN_stashOrder);
			this.SizeX = row.TryGetInt(BuildingTypeVO.COLUMN_sizex);
			this.SizeY = row.TryGetInt(BuildingTypeVO.COLUMN_sizey);
			this.SpawnProtection = row.TryGetInt(BuildingTypeVO.COLUMN_spawnProtect);
			this.AllowDefensiveSpawn = row.TryGetBool(BuildingTypeVO.COLUMN_allowDefensiveSpawn);
			this.Xp = row.TryGetInt(BuildingTypeVO.COLUMN_xp);
			this.Storage = row.TryGetInt(BuildingTypeVO.COLUMN_storage);
			this.Currency = StringUtils.ParseEnum<CurrencyType>(row.TryGetString(BuildingTypeVO.COLUMN_currency));
			ValueObjectController valueObjectController = Service.ValueObjectController;
			this.AudioDeath = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(BuildingTypeVO.COLUMN_audioDeath));
			this.AudioPlacement = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(BuildingTypeVO.COLUMN_audioPlacement));
			this.AudioCharge = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(BuildingTypeVO.COLUMN_audioCharge));
			this.AudioAttack = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(BuildingTypeVO.COLUMN_audioAttack));
			this.AudioImpact = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(BuildingTypeVO.COLUMN_audioImpact));
			this.TurretUid = row.TryGetString(BuildingTypeVO.COLUMN_turretId);
			this.TrapUid = row.TryGetString(BuildingTypeVO.COLUMN_trapID);
			this.ActivationRadius = row.TryGetUint(BuildingTypeVO.COLUMN_activationRadius);
			this.FillStateAssetName = row.TryGetString(BuildingTypeVO.COLUMN_fillStateAssetName);
			this.FillStateBundleName = row.TryGetString(BuildingTypeVO.COLUMN_fillStateBundleName);
			this.DestructFX = row.TryGetString(BuildingTypeVO.COLUMN_destructFX);
			this.TooltipHeightOffset = row.TryGetFloat(BuildingTypeVO.COLUMN_tooltipHeightOffset);
			this.BuffAssetOffset = row.TryGetVector3(BuildingTypeVO.COLUMN_buffAssetOffset);
			this.BuffAssetBaseOffset = row.TryGetVector3(BuildingTypeVO.COLUMN_buffAssetBaseOffset, Vector3.zero);
			this.ShowReticleWhenTargeted = row.TryGetBool(BuildingTypeVO.COLUMN_forceShowReticle);
			this.Lvl = row.TryGetInt(BuildingTypeVO.COLUMN_lvl);
			this.UpgradeTime = ((this.Lvl != 1) ? this.Time : 0);
			BuildingType type = this.Type;
			if (type != BuildingType.Resource && type != BuildingType.Storage && type != BuildingType.HQ)
			{
				this.IsLootable = false;
			}
			else
			{
				this.IsLootable = true;
			}
			string text = row.TryGetString(BuildingTypeVO.COLUMN_connectors);
			if (!string.IsNullOrEmpty(text))
			{
				this.Connectors = Service.StaticDataController.Get<BuildingConnectorTypeVO>(text);
			}
			this.IconUnlockScale = Vector3.one;
			this.IconUnlockRotation = Vector3.zero;
			this.IconUnlockPosition = Vector3.zero;
		}
	}
}
