using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TroopTypeVO : IAssetVO, IAudioVO, IDeployableVO, IGeometryVO, IHealthVO, IShooterVO, ISpeedVO, ITroopDeployableVO, ITroopShooterVO, IUnlockableVO, IUpgradeableVO, IValueObject
	{
		public const int MELEE_RANGE = 4;

		public int Xp;

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_shieldAssetName
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

		public static int COLUMN_decalAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_uiDecalAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_decalBundleName
		{
			get;
			private set;
		}

		public static int COLUMN_decalSize
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

		public static int COLUMN_role
		{
			get;
			private set;
		}

		public static int COLUMN_unitID
		{
			get;
			private set;
		}

		public static int COLUMN_gunSequence
		{
			get;
			private set;
		}

		public static int COLUMN_type
		{
			get;
			private set;
		}

		public static int COLUMN_overWalls
		{
			get;
			private set;
		}

		public static int COLUMN_crushesWalls
		{
			get;
			private set;
		}

		public static int COLUMN_ignoresWalls
		{
			get;
			private set;
		}

		public static int COLUMN_attackShieldBorder
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

		public static int COLUMN_health
		{
			get;
			private set;
		}

		public static int COLUMN_shieldHealth
		{
			get;
			private set;
		}

		public static int COLUMN_shieldCooldown
		{
			get;
			private set;
		}

		public static int COLUMN_maxSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_runSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_runThreshold
		{
			get;
			private set;
		}

		public static int COLUMN_newRotationSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_size
		{
			get;
			private set;
		}

		public static int COLUMN_isFlying
		{
			get;
			private set;
		}

		public static int COLUMN_targetLocking
		{
			get;
			private set;
		}

		public static int COLUMN_retargetingOffset
		{
			get;
			private set;
		}

		public static int COLUMN_supportFollowDistance
		{
			get;
			private set;
		}

		public static int COLUMN_clipRetargeting
		{
			get;
			private set;
		}

		public static int COLUMN_newTargetOnReload
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

		public static int COLUMN_ability
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

		public static int COLUMN_audioImpact
		{
			get;
			private set;
		}

		public static int COLUMN_audioTrain
		{
			get;
			private set;
		}

		public static int COLUMN_wall
		{
			get;
			private set;
		}

		public static int COLUMN_building
		{
			get;
			private set;
		}

		public static int COLUMN_storage
		{
			get;
			private set;
		}

		public static int COLUMN_resource
		{
			get;
			private set;
		}

		public static int COLUMN_turret
		{
			get;
			private set;
		}

		public static int COLUMN_HQ
		{
			get;
			private set;
		}

		public static int COLUMN_shield
		{
			get;
			private set;
		}

		public static int COLUMN_shieldGenerator
		{
			get;
			private set;
		}

		public static int COLUMN_infantry
		{
			get;
			private set;
		}

		public static int COLUMN_bruiserInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_vehicle
		{
			get;
			private set;
		}

		public static int COLUMN_bruiserVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_healerInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_heroInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_heroVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_heroBruiserInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_heroBruiserVechicle
		{
			get;
			private set;
		}

		public static int COLUMN_flierInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_flierVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_trap
		{
			get;
			private set;
		}

		public static int COLUMN_champion
		{
			get;
			private set;
		}

		public static int COLUMN_targetPreferenceStrength
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

		public static int COLUMN_favoriteTargetType
		{
			get;
			private set;
		}

		public static int COLUMN_viewRange
		{
			get;
			private set;
		}

		public static int COLUMN_minAttackRange
		{
			get;
			private set;
		}

		public static int COLUMN_maxAttackRange
		{
			get;
			private set;
		}

		public static int COLUMN_shotCount
		{
			get;
			private set;
		}

		public static int COLUMN_pathSearchWidth
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

		public static int COLUMN_chargeTime
		{
			get;
			private set;
		}

		public static int COLUMN_animationDelay
		{
			get;
			private set;
		}

		public static int COLUMN_shotDelay
		{
			get;
			private set;
		}

		public static int COLUMN_reload
		{
			get;
			private set;
		}

		public static int COLUMN_playerFacing
		{
			get;
			private set;
		}

		public static int COLUMN_targetSelf
		{
			get;
			private set;
		}

		public static int COLUMN_hologramUid
		{
			get;
			private set;
		}

		public static int COLUMN_factoryScaleFactor
		{
			get;
			private set;
		}

		public static int COLUMN_factoryRotation
		{
			get;
			private set;
		}

		public static int COLUMN_strictCoolDown
		{
			get;
			private set;
		}

		public static int COLUMN_autoSpawnSpreadingScale
		{
			get;
			private set;
		}

		public static int COLUMN_autoSpawnRateScale
		{
			get;
			private set;
		}

		public static int COLUMN_projectileType
		{
			get;
			private set;
		}

		public static int COLUMN_deathProjectile
		{
			get;
			private set;
		}

		public static int COLUMN_deathProjectileDelay
		{
			get;
			private set;
		}

		public static int COLUMN_deathProjectileDistance
		{
			get;
			private set;
		}

		public static int COLUMN_deathProjectileDamage
		{
			get;
			private set;
		}

		public static int COLUMN_deathAnimation
		{
			get;
			private set;
		}

		public static int COLUMN_spawnApplyBuffs
		{
			get;
			private set;
		}

		public static int COLUMN_spawnEffectUid
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

		public static int COLUMN_infoUIType
		{
			get;
			private set;
		}

		public static int COLUMN_unlockPlanet
		{
			get;
			private set;
		}

		public static int COLUMN_targetInRangeModifier
		{
			get;
			private set;
		}

		public static int COLUMN_preventDonation
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

		public static int COLUMN_planetAttachmentId
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
			set;
		}

		public string ShieldAssetName
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

		public string DecalAssetName
		{
			get;
			set;
		}

		public string UIDecalAssetName
		{
			get;
			set;
		}

		public string DecalBundleName
		{
			get;
			set;
		}

		public float DecalSize
		{
			get;
			set;
		}

		public ArmorType ArmorType
		{
			get;
			private set;
		}

		public FactionType Faction
		{
			get;
			set;
		}

		public TroopRole TroopRole
		{
			get;
			set;
		}

		public string TroopID
		{
			get;
			private set;
		}

		public int[] GunSequence
		{
			get;
			set;
		}

		public TroopType Type
		{
			get;
			private set;
		}

		public InfoUIType InfoUIType
		{
			get;
			private set;
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

		public int Health
		{
			get;
			set;
		}

		public int ShieldHealth
		{
			get;
			set;
		}

		public uint ShieldCooldown
		{
			get;
			set;
		}

		public int MaxSpeed
		{
			get;
			set;
		}

		public int RunSpeed
		{
			get;
			set;
		}

		public int RotationSpeed
		{
			get;
			set;
		}

		public int TrainingTime
		{
			get;
			set;
		}

		public int RunThreshold
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

		public string Ability
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

		public int SizeX
		{
			get;
			private set;
		}

		public int SizeY
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
				return this.TroopID;
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

		public int[] Preference
		{
			get;
			set;
		}

		public int PreferencePercentile
		{
			get;
			set;
		}

		public int NearnessPercentile
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

		public string FavoriteTargetType
		{
			get;
			set;
		}

		public uint ViewRange
		{
			get;
			set;
		}

		public uint MinAttackRange
		{
			get;
			set;
		}

		public uint MaxAttackRange
		{
			get;
			set;
		}

		public bool AttackShieldBorder
		{
			get;
			set;
		}

		public bool OverWalls
		{
			get;
			set;
		}

		public bool CrushesWalls
		{
			get;
			set;
		}

		public bool IgnoresWalls
		{
			get;
			set;
		}

		public uint PathSearchWidth
		{
			get;
			set;
		}

		public bool IsHealer
		{
			get;
			set;
		}

		public bool IsFlying
		{
			get;
			set;
		}

		public bool TargetLocking
		{
			get;
			set;
		}

		public bool TargetSelf
		{
			get;
			set;
		}

		public uint RetargetingOffset
		{
			get;
			set;
		}

		public uint SupportFollowDistance
		{
			get;
			private set;
		}

		public bool ClipRetargeting
		{
			get;
			set;
		}

		public bool NewTargetOnReload
		{
			get;
			set;
		}

		public uint WarmupDelay
		{
			get;
			set;
		}

		public uint AnimationDelay
		{
			get;
			set;
		}

		public uint ShotDelay
		{
			get;
			set;
		}

		public uint CooldownDelay
		{
			get;
			set;
		}

		public uint ShotCount
		{
			get;
			set;
		}

		public ProjectileTypeVO ProjectileType
		{
			get;
			set;
		}

		public ProjectileTypeVO DeathProjectileType
		{
			get;
			private set;
		}

		public uint DeathProjectileDelay
		{
			get;
			private set;
		}

		public int DeathProjectileDistance
		{
			get;
			private set;
		}

		public int DeathProjectileDamage
		{
			get;
			private set;
		}

		public List<KeyValuePair<string, int>> DeathAnimations
		{
			get;
			private set;
		}

		public string[] SpawnApplyBuffs
		{
			get;
			private set;
		}

		public TroopUniqueAbilityDescVO UniqueAbilityDescVO
		{
			get;
			set;
		}

		public uint AutoSpawnSpreadingScale
		{
			get;
			set;
		}

		public uint AutoSpawnRateScale
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

		public float FactoryScaleFactor
		{
			get;
			private set;
		}

		public float FactoryRotation
		{
			get;
			private set;
		}

		public string SpawnEffectUid
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

		public Dictionary<int, int> Sequences
		{
			get;
			private set;
		}

		public bool StrictCooldown
		{
			get;
			set;
		}

		public uint TargetInRangeModifier
		{
			get;
			set;
		}

		public bool PreventDonation
		{
			get;
			set;
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

		public string PlanetAttachmentId
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.AssetName = row.TryGetString(TroopTypeVO.COLUMN_assetName);
			this.ShieldAssetName = row.TryGetString(TroopTypeVO.COLUMN_shieldAssetName);
			this.BundleName = row.TryGetString(TroopTypeVO.COLUMN_bundleName);
			this.IconAssetName = row.TryGetString(TroopTypeVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(TroopTypeVO.COLUMN_iconBundleName, this.BundleName);
			this.DecalAssetName = row.TryGetString(TroopTypeVO.COLUMN_decalAssetName);
			this.UIDecalAssetName = row.TryGetString(TroopTypeVO.COLUMN_uiDecalAssetName);
			this.DecalBundleName = row.TryGetString(TroopTypeVO.COLUMN_decalBundleName);
			this.DecalSize = (float)row.TryGetInt(TroopTypeVO.COLUMN_decalSize) * 0.01f;
			this.ArmorType = StringUtils.ParseEnum<ArmorType>(row.TryGetString(TroopTypeVO.COLUMN_armorType));
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(TroopTypeVO.COLUMN_faction));
			this.TroopRole = StringUtils.ParseEnum<TroopRole>(row.TryGetString(TroopTypeVO.COLUMN_role));
			this.TroopID = row.TryGetString(TroopTypeVO.COLUMN_unitID);
			this.Type = StringUtils.ParseEnum<TroopType>(row.TryGetString(TroopTypeVO.COLUMN_type));
			this.InfoUIType = StringUtils.ParseEnum<InfoUIType>(row.TryGetString(TroopTypeVO.COLUMN_infoUIType));
			this.OverWalls = row.TryGetBool(TroopTypeVO.COLUMN_overWalls);
			this.CrushesWalls = row.TryGetBool(TroopTypeVO.COLUMN_crushesWalls);
			this.IgnoresWalls = row.TryGetBool(TroopTypeVO.COLUMN_ignoresWalls);
			this.AttackShieldBorder = row.TryGetBool(TroopTypeVO.COLUMN_attackShieldBorder);
			this.Credits = row.TryGetInt(TroopTypeVO.COLUMN_credits);
			this.Materials = row.TryGetInt(TroopTypeVO.COLUMN_materials);
			this.Contraband = row.TryGetInt(TroopTypeVO.COLUMN_contraband);
			this.Health = row.TryGetInt(TroopTypeVO.COLUMN_health);
			this.ShieldHealth = row.TryGetInt(TroopTypeVO.COLUMN_shieldHealth);
			this.ShieldCooldown = row.TryGetUint(TroopTypeVO.COLUMN_shieldCooldown);
			this.MaxSpeed = row.TryGetInt(TroopTypeVO.COLUMN_maxSpeed);
			this.RunSpeed = row.TryGetInt(TroopTypeVO.COLUMN_runSpeed);
			this.RunThreshold = row.TryGetInt(TroopTypeVO.COLUMN_runThreshold);
			this.RotationSpeed = row.TryGetInt(TroopTypeVO.COLUMN_newRotationSpeed);
			this.Size = row.TryGetInt(TroopTypeVO.COLUMN_size);
			this.IsFlying = row.TryGetBool(TroopTypeVO.COLUMN_isFlying);
			this.TargetLocking = row.TryGetBool(TroopTypeVO.COLUMN_targetLocking);
			this.RetargetingOffset = row.TryGetUint(TroopTypeVO.COLUMN_retargetingOffset);
			this.SupportFollowDistance = row.TryGetUint(TroopTypeVO.COLUMN_supportFollowDistance);
			this.ClipRetargeting = row.TryGetBool(TroopTypeVO.COLUMN_clipRetargeting);
			this.NewTargetOnReload = row.TryGetBool(TroopTypeVO.COLUMN_newTargetOnReload);
			this.TargetInRangeModifier = row.TryGetUint(TroopTypeVO.COLUMN_targetInRangeModifier, 1u);
			this.PreventDonation = row.TryGetBool(TroopTypeVO.COLUMN_preventDonation);
			this.PlanetAttachmentId = row.TryGetString(TroopTypeVO.COLUMN_planetAttachmentId, string.Empty);
			if (this.TargetInRangeModifier == 0u)
			{
				this.TargetInRangeModifier = 1u;
			}
			if ((this.Type == TroopType.Hero || this.Type == TroopType.Champion) && this.Size != 1)
			{
				Service.Logger.Warn(this.Uid + " must have size 1.  Please fix CMS.");
				this.Size = 1;
			}
			this.TrainingTime = row.TryGetInt(TroopTypeVO.COLUMN_trainingTime);
			this.Xp = row.TryGetInt(TroopTypeVO.COLUMN_xp);
			string[] array = row.TryGetStringArray(TroopTypeVO.COLUMN_requirements);
			this.BuildingRequirement = ((array != null && array.Length != 0) ? array[0] : null);
			this.UnlockedByEvent = row.TryGetBool(TroopTypeVO.COLUMN_unlockedByEvent);
			this.Ability = row.TryGetString(TroopTypeVO.COLUMN_ability);
			this.IconCameraPosition = row.TryGetVector3(TroopTypeVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(TroopTypeVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(TroopTypeVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(TroopTypeVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.Order = row.TryGetInt(TroopTypeVO.COLUMN_order);
			this.SizeX = row.TryGetInt(TroopTypeVO.COLUMN_sizex);
			this.SizeY = row.TryGetInt(TroopTypeVO.COLUMN_sizey);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			this.AudioCharge = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopTypeVO.COLUMN_audioCharge));
			this.AudioAttack = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopTypeVO.COLUMN_audioAttack));
			this.AudioDeath = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopTypeVO.COLUMN_audioDeath));
			this.AudioPlacement = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopTypeVO.COLUMN_audioPlacement));
			this.AudioImpact = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopTypeVO.COLUMN_audioImpact));
			this.AudioTrain = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopTypeVO.COLUMN_audioTrain));
			this.Preference = new int[24];
			int i = 0;
			int num = 24;
			while (i < num)
			{
				this.Preference[i] = 0;
				i++;
			}
			this.Preference[1] = row.TryGetInt(TroopTypeVO.COLUMN_wall);
			this.Preference[2] = row.TryGetInt(TroopTypeVO.COLUMN_building);
			this.Preference[3] = row.TryGetInt(TroopTypeVO.COLUMN_storage);
			this.Preference[4] = row.TryGetInt(TroopTypeVO.COLUMN_resource);
			this.Preference[5] = row.TryGetInt(TroopTypeVO.COLUMN_turret);
			this.Preference[6] = row.TryGetInt(TroopTypeVO.COLUMN_HQ);
			this.Preference[7] = row.TryGetInt(TroopTypeVO.COLUMN_shield);
			this.Preference[8] = row.TryGetInt(TroopTypeVO.COLUMN_shieldGenerator);
			this.Preference[9] = row.TryGetInt(TroopTypeVO.COLUMN_infantry);
			this.Preference[10] = row.TryGetInt(TroopTypeVO.COLUMN_bruiserInfantry);
			this.Preference[11] = row.TryGetInt(TroopTypeVO.COLUMN_vehicle);
			this.Preference[12] = row.TryGetInt(TroopTypeVO.COLUMN_bruiserVehicle);
			this.Preference[13] = row.TryGetInt(TroopTypeVO.COLUMN_heroInfantry);
			this.Preference[14] = row.TryGetInt(TroopTypeVO.COLUMN_heroVehicle);
			this.Preference[15] = row.TryGetInt(TroopTypeVO.COLUMN_heroBruiserInfantry);
			this.Preference[16] = row.TryGetInt(TroopTypeVO.COLUMN_heroBruiserVechicle);
			this.Preference[17] = row.TryGetInt(TroopTypeVO.COLUMN_flierInfantry);
			this.Preference[18] = row.TryGetInt(TroopTypeVO.COLUMN_flierVehicle);
			this.Preference[19] = row.TryGetInt(TroopTypeVO.COLUMN_healerInfantry);
			this.Preference[20] = row.TryGetInt(TroopTypeVO.COLUMN_trap);
			this.Preference[21] = row.TryGetInt(TroopTypeVO.COLUMN_champion);
			this.PreferencePercentile = row.TryGetInt(TroopTypeVO.COLUMN_targetPreferenceStrength);
			this.NearnessPercentile = 100 - this.PreferencePercentile;
			this.Damage = row.TryGetInt(TroopTypeVO.COLUMN_damage);
			this.DPS = row.TryGetInt(TroopTypeVO.COLUMN_dps);
			this.FavoriteTargetType = row.TryGetString(TroopTypeVO.COLUMN_favoriteTargetType);
			this.ViewRange = row.TryGetUint(TroopTypeVO.COLUMN_viewRange);
			this.MinAttackRange = row.TryGetUint(TroopTypeVO.COLUMN_minAttackRange);
			this.MaxAttackRange = row.TryGetUint(TroopTypeVO.COLUMN_maxAttackRange);
			this.ShotCount = row.TryGetUint(TroopTypeVO.COLUMN_shotCount);
			this.PathSearchWidth = row.TryGetUint(TroopTypeVO.COLUMN_pathSearchWidth);
			this.Lvl = row.TryGetInt(TroopTypeVO.COLUMN_lvl);
			this.UpgradeTime = row.TryGetInt(TroopTypeVO.COLUMN_upgradeTime);
			this.UpgradeCredits = row.TryGetInt(TroopTypeVO.COLUMN_upgradeCredits);
			this.UpgradeMaterials = row.TryGetInt(TroopTypeVO.COLUMN_upgradeMaterials);
			this.UpgradeContraband = row.TryGetInt(TroopTypeVO.COLUMN_upgradeContraband);
			this.IsHealer = (this.TroopRole == TroopRole.Healer);
			this.WarmupDelay = row.TryGetUint(TroopTypeVO.COLUMN_chargeTime);
			this.AnimationDelay = row.TryGetUint(TroopTypeVO.COLUMN_animationDelay);
			this.ShotDelay = row.TryGetUint(TroopTypeVO.COLUMN_shotDelay);
			this.CooldownDelay = row.TryGetUint(TroopTypeVO.COLUMN_reload);
			this.PlayerFacing = row.TryGetBool(TroopTypeVO.COLUMN_playerFacing);
			this.TargetSelf = row.TryGetBool(TroopTypeVO.COLUMN_targetSelf);
			this.HologramUid = row.TryGetString(TroopTypeVO.COLUMN_hologramUid);
			this.FactoryScaleFactor = row.TryGetFloat(TroopTypeVO.COLUMN_factoryScaleFactor);
			this.FactoryRotation = row.TryGetFloat(TroopTypeVO.COLUMN_factoryRotation);
			this.StrictCooldown = row.TryGetBool(TroopTypeVO.COLUMN_strictCoolDown);
			this.AutoSpawnSpreadingScale = row.TryGetUint(TroopTypeVO.COLUMN_autoSpawnSpreadingScale, 1u);
			this.AutoSpawnRateScale = row.TryGetUint(TroopTypeVO.COLUMN_autoSpawnRateScale, 1u);
			StaticDataController staticDataController = Service.StaticDataController;
			this.ProjectileType = staticDataController.Get<ProjectileTypeVO>(row.TryGetString(TroopTypeVO.COLUMN_projectileType));
			if (this.ProjectileType.IsBeam && (long)this.ProjectileType.BeamDamageLength < (long)((ulong)this.MaxAttackRange))
			{
				Service.Logger.WarnFormat("Troop {0} can target something it can't damage", new object[]
				{
					this.Uid
				});
			}
			string text = row.TryGetString(TroopTypeVO.COLUMN_deathProjectile);
			this.DeathProjectileType = ((!string.IsNullOrEmpty(text)) ? staticDataController.Get<ProjectileTypeVO>(text) : null);
			this.DeathProjectileDelay = row.TryGetUint(TroopTypeVO.COLUMN_deathProjectileDelay, 0u);
			this.DeathProjectileDistance = row.TryGetInt(TroopTypeVO.COLUMN_deathProjectileDistance, 0);
			this.DeathProjectileDamage = row.TryGetInt(TroopTypeVO.COLUMN_deathProjectileDamage, this.Damage);
			string text2 = row.TryGetString(TroopTypeVO.COLUMN_deathAnimation);
			if (!string.IsNullOrEmpty(text2))
			{
				string[] array2 = text2.Split(new char[]
				{
					','
				});
				int num2 = array2.Length;
				this.DeathAnimations = new List<KeyValuePair<string, int>>(num2);
				for (int j = 0; j < num2; j++)
				{
					string[] array3 = array2[j].Split(new char[]
					{
						':'
					});
					int value;
					if (array3.Length == 2 && int.TryParse(array3[1], out value))
					{
						string key = array3[0];
						this.DeathAnimations.Add(new KeyValuePair<string, int>(key, value));
					}
				}
			}
			this.SpawnApplyBuffs = null;
			string text3 = row.TryGetString(TroopTypeVO.COLUMN_spawnApplyBuffs);
			if (!string.IsNullOrEmpty(text3))
			{
				this.SpawnApplyBuffs = text3.Split(new char[]
				{
					','
				});
			}
			this.SpawnEffectUid = row.TryGetString(TroopTypeVO.COLUMN_spawnEffectUid);
			this.TooltipHeightOffset = row.TryGetFloat(TroopTypeVO.COLUMN_tooltipHeightOffset);
			this.BuffAssetOffset = row.TryGetVector3(TroopTypeVO.COLUMN_buffAssetOffset);
			this.BuffAssetBaseOffset = row.TryGetVector3(TroopTypeVO.COLUMN_buffAssetBaseOffset, Vector3.zero);
			SequencePair gunSequences = valueObjectController.GetGunSequences(this.Uid, row.TryGetString(TroopTypeVO.COLUMN_gunSequence));
			this.GunSequence = gunSequences.GunSequence;
			this.Sequences = gunSequences.Sequences;
			this.EventFeaturesString = row.TryGetString(TroopTypeVO.COLUMN_eventFeaturesString);
			this.EventButtonAction = row.TryGetString(TroopTypeVO.COLUMN_eventButtonAction);
			this.EventButtonData = row.TryGetString(TroopTypeVO.COLUMN_eventButtonData);
			this.EventButtonString = row.TryGetString(TroopTypeVO.COLUMN_eventButtonString);
			this.UpgradeShardCount = row.TryGetInt(TroopTypeVO.COLUMN_upgradeShards);
			this.UpgradeShardUid = row.TryGetString(TroopTypeVO.COLUMN_upgradeShardUid);
			this.IconUnlockScale = row.TryGetVector3(TroopTypeVO.COLUMN_iconUnlockScale, Vector3.one);
			this.IconUnlockRotation = row.TryGetVector3(TroopTypeVO.COLUMN_iconUnlockRotation, Vector3.zero);
			this.IconUnlockPosition = row.TryGetVector3(TroopTypeVO.COLUMN_iconUnlockPosition, Vector3.zero);
			if (this.RotationSpeed == 0)
			{
				Service.Logger.ErrorFormat("Missing rotation speed for troopTypeVO {0}", new object[]
				{
					this.Uid
				});
			}
		}
	}
}
