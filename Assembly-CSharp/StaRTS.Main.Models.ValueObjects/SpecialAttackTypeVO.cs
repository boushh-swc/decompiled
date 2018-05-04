using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SpecialAttackTypeVO : ISpeedVO, IAudioVO, IDeployableVO, IUpgradeableVO, IValueObject, IAssetVO, IGeometryVO, IUnlockableVO
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

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_specialAttackID
		{
			get;
			private set;
		}

		public static int COLUMN_specialAttackName
		{
			get;
			private set;
		}

		public static int COLUMN_size
		{
			get;
			private set;
		}

		public static int COLUMN_trainingTime
		{
			get;
			private set;
		}

		public static int COLUMN_xp
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

		public static int COLUMN_acceleration
		{
			get;
			private set;
		}

		public static int COLUMN_maxSpeed
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

		public static int COLUMN_audioPlacement
		{
			get;
			private set;
		}

		public static int COLUMN_audioMovement
		{
			get;
			private set;
		}

		public static int COLUMN_audioMovementAway
		{
			get;
			private set;
		}

		public static int COLUMN_audioImpact
		{
			get;
			private set;
		}

		public static int COLUMN_damage
		{
			get;
			private set;
		}

		public static int COLUMN_dps
		{
			get;
			private set;
		}

		public static int COLUMN_infoUIType
		{
			get;
			private set;
		}

		public static int COLUMN_requirements
		{
			get;
			private set;
		}

		public static int COLUMN_unlockedByEvent
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

		public static int COLUMN_lvl
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeTime
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeCredits
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeMaterials
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeContraband
		{
			get;
			private set;
		}

		public static int COLUMN_shotCount
		{
			get;
			private set;
		}

		public static int COLUMN_shotDelay
		{
			get;
			private set;
		}

		public static int COLUMN_impactDelay
		{
			get;
			private set;
		}

		public static int COLUMN_animationDelay
		{
			get;
			private set;
		}

		public static int COLUMN_destroyDelay
		{
			get;
			private set;
		}

		public static int COLUMN_projectileType
		{
			get;
			private set;
		}

		public static int COLUMN_reticleDuration
		{
			get;
			private set;
		}

		public static int COLUMN_reticleAsset
		{
			get;
			private set;
		}

		public static int COLUMN_reticleScale
		{
			get;
			private set;
		}

		public static int COLUMN_playerFacing
		{
			get;
			private set;
		}

		public static int COLUMN_hologramUid
		{
			get;
			private set;
		}

		public static int COLUMN_numberOfAttackers
		{
			get;
			private set;
		}

		public static int COLUMN_attackerDelay
		{
			get;
			private set;
		}

		public static int COLUMN_attackerOffset
		{
			get;
			private set;
		}

		public static int COLUMN_attackerOffsetVariance
		{
			get;
			private set;
		}

		public static int COLUMN_attackFormation
		{
			get;
			private set;
		}

		public static int COLUMN_angleOfAttack
		{
			get;
			private set;
		}

		public static int COLUMN_angleOfAttackVariance
		{
			get;
			private set;
		}

		public static int COLUMN_angleOfRoll
		{
			get;
			private set;
		}

		public static int COLUMN_angleOfRollVariance
		{
			get;
			private set;
		}

		public static int COLUMN_linkedUnit
		{
			get;
			private set;
		}

		public static int COLUMN_unitCount
		{
			get;
			private set;
		}

		public static int COLUMN_favoriteTargetType
		{
			get;
			private set;
		}

		public static int COLUMN_attachmentAsset
		{
			get;
			private set;
		}

		public static int COLUMN_attachmentAssetBundle
		{
			get;
			private set;
		}

		public static int COLUMN_unlockPlanet
		{
			get;
			private set;
		}

		public static int COLUMN_eventFeaturesString
		{
			get;
			private set;
		}

		public static int COLUMN_eventButtonAction
		{
			get;
			private set;
		}

		public static int COLUMN_eventButtonData
		{
			get;
			private set;
		}

		public static int COLUMN_eventButtonString
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeShards
		{
			get;
			private set;
		}

		public static int COLUMN_upgradeShardUid
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

		public string SpecialAttackID
		{
			get;
			set;
		}

		public string SpecialAttackName
		{
			get;
			set;
		}

		public int TrainingTime
		{
			get;
			set;
		}

		public int Xp
		{
			get;
			set;
		}

		public int Credits
		{
			get;
			set;
		}

		public int Materials
		{
			get;
			set;
		}

		public int Contraband
		{
			get;
			set;
		}

		public int Acceleration
		{
			get;
			set;
		}

		public int MaxSpeed
		{
			get;
			set;
		}

		public int RotationSpeed
		{
			get;
			set;
		}

		public uint NumberOfAttackers
		{
			get;
			set;
		}

		public int AttackerDelay
		{
			get;
			set;
		}

		public int AttackerOffset
		{
			get;
			set;
		}

		public int AttackerOffsetVariance
		{
			get;
			set;
		}

		public AttackFormation AttackFormation
		{
			get;
			set;
		}

		public int AngleOfAttack
		{
			get;
			set;
		}

		public int AngleOfAttackVariance
		{
			get;
			set;
		}

		public int AngleOfRoll
		{
			get;
			set;
		}

		public int AngleOfRollVariance
		{
			get;
			set;
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

		public string LinkedUnit
		{
			get;
			set;
		}

		public uint UnitCount
		{
			get;
			set;
		}

		public bool IsDropship
		{
			get
			{
				return !string.IsNullOrEmpty(this.LinkedUnit) && this.UnitCount != 0u;
			}
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
				return this.SpecialAttackID;
			}
		}

		public List<StrIntPair> AudioPlacement
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

		public List<StrIntPair> AudioImpact
		{
			get;
			set;
		}

		public List<StrIntPair> AudioTrain
		{
			get;
			set;
		}

		public int Damage
		{
			get;
			set;
		}

		public int DPS
		{
			get;
			set;
		}

		public InfoUIType InfoUIType
		{
			get;
			private set;
		}

		public uint ShotCount
		{
			get;
			set;
		}

		public uint ShotDelay
		{
			get;
			set;
		}

		public uint HitDelay
		{
			get;
			set;
		}

		public uint AnimationDelay
		{
			get;
			set;
		}

		public float DestroyDelay
		{
			get;
			set;
		}

		public float ReticleDuration
		{
			get;
			set;
		}

		public string ReticleAsset
		{
			get;
			set;
		}

		public float ReticleScale
		{
			get;
			set;
		}

		public ProjectileTypeVO ProjectileType
		{
			get;
			set;
		}

		public bool PlayerFacing
		{
			get;
			private set;
		}

		public string HologramUid
		{
			get;
			private set;
		}

		public string FavoriteTargetType
		{
			get;
			private set;
		}

		public string DropoffAttachedAssetName
		{
			get;
			private set;
		}

		public string DropoffAttachedBundleName
		{
			get;
			private set;
		}

		public bool HasDropoff
		{
			get
			{
				return !string.IsNullOrEmpty(this.DropoffAttachedAssetName);
			}
		}

		public string EventFeaturesString
		{
			get;
			set;
		}

		public string EventButtonAction
		{
			get;
			set;
		}

		public string EventButtonData
		{
			get;
			set;
		}

		public string EventButtonString
		{
			get;
			set;
		}

		public int UpgradeShardCount
		{
			get;
			set;
		}

		public string UpgradeShardUid
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

		public void ReadRow(Row row)
		{
			this.AssetName = row.TryGetString(SpecialAttackTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(SpecialAttackTypeVO.COLUMN_bundleName);
			this.IconAssetName = row.TryGetString(SpecialAttackTypeVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(SpecialAttackTypeVO.COLUMN_iconBundleName, this.BundleName);
			this.DropoffAttachedAssetName = row.TryGetString(SpecialAttackTypeVO.COLUMN_attachmentAsset);
			this.DropoffAttachedBundleName = row.TryGetString(SpecialAttackTypeVO.COLUMN_attachmentAssetBundle);
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(SpecialAttackTypeVO.COLUMN_faction));
			this.SpecialAttackID = row.TryGetString(SpecialAttackTypeVO.COLUMN_specialAttackID);
			this.SpecialAttackName = row.TryGetString(SpecialAttackTypeVO.COLUMN_specialAttackName);
			this.Size = row.TryGetInt(SpecialAttackTypeVO.COLUMN_size);
			this.TrainingTime = row.TryGetInt(SpecialAttackTypeVO.COLUMN_trainingTime);
			this.Xp = row.TryGetInt(SpecialAttackTypeVO.COLUMN_xp);
			this.Credits = row.TryGetInt(SpecialAttackTypeVO.COLUMN_credits);
			this.Materials = row.TryGetInt(SpecialAttackTypeVO.COLUMN_materials);
			this.Contraband = row.TryGetInt(SpecialAttackTypeVO.COLUMN_contraband);
			this.Acceleration = row.TryGetInt(SpecialAttackTypeVO.COLUMN_acceleration);
			this.MaxSpeed = row.TryGetInt(SpecialAttackTypeVO.COLUMN_maxSpeed);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			this.AudioCharge = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SpecialAttackTypeVO.COLUMN_audioCharge));
			this.AudioAttack = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SpecialAttackTypeVO.COLUMN_audioAttack));
			this.AudioPlacement = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SpecialAttackTypeVO.COLUMN_audioPlacement));
			this.AudioMovement = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SpecialAttackTypeVO.COLUMN_audioMovement));
			this.AudioMovementAway = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SpecialAttackTypeVO.COLUMN_audioMovementAway));
			this.AudioImpact = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SpecialAttackTypeVO.COLUMN_audioImpact));
			this.Damage = row.TryGetInt(SpecialAttackTypeVO.COLUMN_damage);
			this.DPS = row.TryGetInt(SpecialAttackTypeVO.COLUMN_dps);
			this.InfoUIType = StringUtils.ParseEnum<InfoUIType>(row.TryGetString(SpecialAttackTypeVO.COLUMN_infoUIType));
			string[] array = row.TryGetStringArray(SpecialAttackTypeVO.COLUMN_requirements);
			this.BuildingRequirement = ((array != null && array.Length != 0) ? array[0] : null);
			this.UnlockedByEvent = row.TryGetBool(SpecialAttackTypeVO.COLUMN_unlockedByEvent);
			this.IconCameraPosition = row.TryGetVector3(SpecialAttackTypeVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(SpecialAttackTypeVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(SpecialAttackTypeVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(SpecialAttackTypeVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.Order = row.TryGetInt(SpecialAttackTypeVO.COLUMN_order);
			this.Lvl = row.TryGetInt(SpecialAttackTypeVO.COLUMN_lvl);
			this.UpgradeTime = row.TryGetInt(SpecialAttackTypeVO.COLUMN_upgradeTime);
			this.UpgradeCredits = row.TryGetInt(SpecialAttackTypeVO.COLUMN_upgradeCredits);
			this.UpgradeMaterials = row.TryGetInt(SpecialAttackTypeVO.COLUMN_upgradeMaterials);
			this.UpgradeContraband = row.TryGetInt(SpecialAttackTypeVO.COLUMN_upgradeContraband);
			this.ShotCount = row.TryGetUint(SpecialAttackTypeVO.COLUMN_shotCount);
			this.ShotDelay = row.TryGetUint(SpecialAttackTypeVO.COLUMN_shotDelay);
			this.HitDelay = row.TryGetUint(SpecialAttackTypeVO.COLUMN_impactDelay);
			this.AnimationDelay = row.TryGetUint(SpecialAttackTypeVO.COLUMN_animationDelay);
			this.DestroyDelay = row.TryGetFloat(SpecialAttackTypeVO.COLUMN_destroyDelay);
			this.ProjectileType = Service.StaticDataController.Get<ProjectileTypeVO>(row.TryGetString(SpecialAttackTypeVO.COLUMN_projectileType));
			this.ReticleDuration = row.TryGetFloat(SpecialAttackTypeVO.COLUMN_reticleDuration, 3f);
			this.ReticleAsset = row.TryGetString(SpecialAttackTypeVO.COLUMN_reticleAsset);
			this.ReticleScale = row.TryGetFloat(SpecialAttackTypeVO.COLUMN_reticleScale, 2f);
			this.PlayerFacing = row.TryGetBool(SpecialAttackTypeVO.COLUMN_playerFacing);
			this.HologramUid = row.TryGetString(SpecialAttackTypeVO.COLUMN_hologramUid);
			this.NumberOfAttackers = Math.Max(row.TryGetUint(SpecialAttackTypeVO.COLUMN_numberOfAttackers), 1u);
			this.AttackerDelay = row.TryGetInt(SpecialAttackTypeVO.COLUMN_attackerDelay);
			this.AttackerOffset = row.TryGetInt(SpecialAttackTypeVO.COLUMN_attackerOffset);
			this.AttackerOffsetVariance = row.TryGetInt(SpecialAttackTypeVO.COLUMN_attackerOffsetVariance);
			this.AttackFormation = StringUtils.ParseEnum<AttackFormation>(row.TryGetString(SpecialAttackTypeVO.COLUMN_attackFormation));
			this.AngleOfAttack = row.TryGetInt(SpecialAttackTypeVO.COLUMN_angleOfAttack);
			this.AngleOfAttackVariance = row.TryGetInt(SpecialAttackTypeVO.COLUMN_angleOfAttackVariance);
			this.AngleOfRoll = row.TryGetInt(SpecialAttackTypeVO.COLUMN_angleOfRoll);
			this.AngleOfRollVariance = row.TryGetInt(SpecialAttackTypeVO.COLUMN_angleOfRollVariance);
			this.LinkedUnit = row.TryGetString(SpecialAttackTypeVO.COLUMN_linkedUnit);
			this.UnitCount = row.TryGetUint(SpecialAttackTypeVO.COLUMN_unitCount);
			this.FavoriteTargetType = row.TryGetString(SpecialAttackTypeVO.COLUMN_favoriteTargetType);
			this.EventFeaturesString = row.TryGetString(SpecialAttackTypeVO.COLUMN_eventFeaturesString);
			this.EventButtonAction = row.TryGetString(SpecialAttackTypeVO.COLUMN_eventButtonAction);
			this.EventButtonData = row.TryGetString(SpecialAttackTypeVO.COLUMN_eventButtonData);
			this.EventButtonString = row.TryGetString(SpecialAttackTypeVO.COLUMN_eventButtonString);
			this.UpgradeShardCount = row.TryGetInt(SpecialAttackTypeVO.COLUMN_upgradeShards);
			this.UpgradeShardUid = row.TryGetString(SpecialAttackTypeVO.COLUMN_upgradeShardUid);
			this.IconUnlockScale = row.TryGetVector3(SpecialAttackTypeVO.COLUMN_iconUnlockScale, Vector3.one);
			this.IconUnlockRotation = row.TryGetVector3(SpecialAttackTypeVO.COLUMN_iconUnlockRotation, Vector3.zero);
			this.IconUnlockPosition = row.TryGetVector3(SpecialAttackTypeVO.COLUMN_iconUnlockPosition, Vector3.zero);
		}
	}
}
