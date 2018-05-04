using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TroopAbilityVO : IValueObject, ITroopShooterVO, ISpeedVO, IShooterVO
	{
		public static int COLUMN_selfBuff
		{
			get;
			private set;
		}

		public static int COLUMN_targetSelf
		{
			get;
			private set;
		}

		public static int COLUMN_duration
		{
			get;
			private set;
		}

		public static int COLUMN_persistentEffect
		{
			get;
			private set;
		}

		public static int COLUMN_persistentScaling
		{
			get;
			private set;
		}

		public static int COLUMN_gunSequence
		{
			get;
			private set;
		}

		public static int COLUMN_damage
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

		public static int COLUMN_projectileType
		{
			get;
			private set;
		}

		public static int COLUMN_overWalls
		{
			get;
			private set;
		}

		public static int COLUMN_favoriteTargetType
		{
			get;
			private set;
		}

		public static int COLUMN_targetLocking
		{
			get;
			private set;
		}

		public static int COLUMN_cooldownTime
		{
			get;
			private set;
		}

		public static int COLUMN_armingDelay
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

		public static int COLUMN_shotCount
		{
			get;
			private set;
		}

		public static int COLUMN_targetPreferenceStrength
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

		public static int COLUMN_heroBruiserVehicle
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

		public static int COLUMN_healerInfantry
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

		public static int COLUMN_audioAbilityActivate
		{
			get;
			private set;
		}

		public static int COLUMN_audioAbilityAttack
		{
			get;
			private set;
		}

		public static int COLUMN_audioAbilityLoop
		{
			get;
			private set;
		}

		public static int COLUMN_maxSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_newRotationSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_auto
		{
			get;
			private set;
		}

		public static int COLUMN_clipCount
		{
			get;
			private set;
		}

		public static int COLUMN_cooldownOnSpawn
		{
			get;
			private set;
		}

		public static int COLUMN_weaponTrailFxParams
		{
			get;
			private set;
		}

		public static int COLUMN_retargetingOffset
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

		public static int COLUMN_strictCoolDown
		{
			get;
			private set;
		}

		public static int COLUMN_dps
		{
			get;
			private set;
		}

		public static int COLUMN_altGunLocators
		{
			get;
			private set;
		}

		public static int COLUMN_recastAbility
		{
			get;
			private set;
		}

		public static int COLUMN_troopRole
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

		public string Uid
		{
			get;
			set;
		}

		public string SelfBuff
		{
			get;
			set;
		}

		public uint CoolDownTime
		{
			get;
			set;
		}

		public uint Duration
		{
			get;
			set;
		}

		public string PersistentEffect
		{
			get;
			set;
		}

		public float PersistentScaling
		{
			get;
			set;
		}

		public bool Auto
		{
			get;
			private set;
		}

		public int ClipCount
		{
			get;
			private set;
		}

		public bool CooldownOnSpawn
		{
			get;
			private set;
		}

		public float[] WeaponTrailFxParams
		{
			get;
			private set;
		}

		public int[] AltGunLocators
		{
			get;
			private set;
		}

		public int[] GunSequence
		{
			get;
			set;
		}

		public int Damage
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

		public ProjectileTypeVO ProjectileType
		{
			get;
			set;
		}

		public bool OverWalls
		{
			get;
			set;
		}

		public string FavoriteTargetType
		{
			get;
			set;
		}

		public bool TargetLocking
		{
			get;
			set;
		}

		public uint ArmingDelay
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

		public int[] Preference
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

		public List<StrIntPair> AudioAbilityActivate
		{
			get;
			set;
		}

		public List<StrIntPair> AudioAbilityAttack
		{
			get;
			set;
		}

		public List<StrIntPair> AudioAbilityLoop
		{
			get;
			set;
		}

		public bool TargetSelf
		{
			get;
			set;
		}

		public Dictionary<int, int> Sequences
		{
			get;
			private set;
		}

		public uint RetargetingOffset
		{
			get;
			set;
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

		public bool StrictCooldown
		{
			get;
			set;
		}

		public int DPS
		{
			get;
			set;
		}

		public bool RecastAbility
		{
			get;
			private set;
		}

		public TroopRole TroopRole
		{
			get;
			private set;
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

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.SelfBuff = row.TryGetString(TroopAbilityVO.COLUMN_selfBuff);
			this.TargetSelf = row.TryGetBool(TroopAbilityVO.COLUMN_targetSelf);
			this.Duration = row.TryGetUint(TroopAbilityVO.COLUMN_duration);
			this.PersistentEffect = row.TryGetString(TroopAbilityVO.COLUMN_persistentEffect);
			this.PersistentScaling = (float)(100 + row.TryGetInt(TroopAbilityVO.COLUMN_persistentScaling)) * 0.01f;
			this.Damage = row.TryGetInt(TroopAbilityVO.COLUMN_damage);
			this.ViewRange = row.TryGetUint(TroopAbilityVO.COLUMN_viewRange);
			this.MinAttackRange = row.TryGetUint(TroopAbilityVO.COLUMN_minAttackRange);
			this.MaxAttackRange = row.TryGetUint(TroopAbilityVO.COLUMN_maxAttackRange);
			string text = row.TryGetString(TroopAbilityVO.COLUMN_projectileType);
			if (!string.IsNullOrEmpty(text))
			{
				this.ProjectileType = Service.StaticDataController.Get<ProjectileTypeVO>(text);
			}
			this.OverWalls = row.TryGetBool(TroopAbilityVO.COLUMN_overWalls);
			this.FavoriteTargetType = row.TryGetString(TroopAbilityVO.COLUMN_favoriteTargetType);
			this.TargetLocking = row.TryGetBool(TroopAbilityVO.COLUMN_targetLocking);
			this.CoolDownTime = row.TryGetUint(TroopAbilityVO.COLUMN_cooldownTime);
			this.ArmingDelay = row.TryGetUint(TroopAbilityVO.COLUMN_armingDelay);
			this.WarmupDelay = row.TryGetUint(TroopAbilityVO.COLUMN_chargeTime);
			this.AnimationDelay = row.TryGetUint(TroopAbilityVO.COLUMN_animationDelay);
			this.ShotDelay = row.TryGetUint(TroopAbilityVO.COLUMN_shotDelay);
			this.CooldownDelay = row.TryGetUint(TroopAbilityVO.COLUMN_reload);
			this.ShotCount = row.TryGetUint(TroopAbilityVO.COLUMN_shotCount);
			this.PreferencePercentile = row.TryGetInt(TroopAbilityVO.COLUMN_targetPreferenceStrength);
			this.NearnessPercentile = 100 - this.PreferencePercentile;
			this.Preference = new int[24];
			int i = 0;
			int num = 24;
			while (i < num)
			{
				this.Preference[i] = 0;
				i++;
			}
			this.Preference[1] = row.TryGetInt(TroopAbilityVO.COLUMN_wall);
			this.Preference[2] = row.TryGetInt(TroopAbilityVO.COLUMN_building);
			this.Preference[3] = row.TryGetInt(TroopAbilityVO.COLUMN_storage);
			this.Preference[4] = row.TryGetInt(TroopAbilityVO.COLUMN_resource);
			this.Preference[5] = row.TryGetInt(TroopAbilityVO.COLUMN_turret);
			this.Preference[6] = row.TryGetInt(TroopAbilityVO.COLUMN_HQ);
			this.Preference[7] = row.TryGetInt(TroopAbilityVO.COLUMN_shield);
			this.Preference[8] = row.TryGetInt(TroopAbilityVO.COLUMN_shieldGenerator);
			this.Preference[9] = row.TryGetInt(TroopAbilityVO.COLUMN_infantry);
			this.Preference[10] = row.TryGetInt(TroopAbilityVO.COLUMN_bruiserInfantry);
			this.Preference[11] = row.TryGetInt(TroopAbilityVO.COLUMN_vehicle);
			this.Preference[12] = row.TryGetInt(TroopAbilityVO.COLUMN_bruiserVehicle);
			this.Preference[13] = row.TryGetInt(TroopAbilityVO.COLUMN_heroInfantry);
			this.Preference[14] = row.TryGetInt(TroopAbilityVO.COLUMN_heroVehicle);
			this.Preference[15] = row.TryGetInt(TroopAbilityVO.COLUMN_heroBruiserInfantry);
			this.Preference[16] = row.TryGetInt(TroopAbilityVO.COLUMN_heroBruiserVehicle);
			this.Preference[17] = row.TryGetInt(TroopAbilityVO.COLUMN_flierInfantry);
			this.Preference[18] = row.TryGetInt(TroopAbilityVO.COLUMN_flierVehicle);
			this.Preference[19] = row.TryGetInt(TroopAbilityVO.COLUMN_healerInfantry);
			this.Preference[20] = row.TryGetInt(TroopAbilityVO.COLUMN_trap);
			this.Preference[21] = row.TryGetInt(TroopAbilityVO.COLUMN_champion);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			this.AudioAbilityActivate = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopAbilityVO.COLUMN_audioAbilityActivate));
			this.AudioAbilityAttack = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopAbilityVO.COLUMN_audioAbilityAttack));
			this.AudioAbilityLoop = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(TroopAbilityVO.COLUMN_audioAbilityLoop));
			this.RecastAbility = row.TryGetBool(TroopAbilityVO.COLUMN_recastAbility);
			SequencePair gunSequences = valueObjectController.GetGunSequences(this.Uid, row.TryGetString(TroopAbilityVO.COLUMN_gunSequence));
			this.GunSequence = gunSequences.GunSequence;
			this.Sequences = gunSequences.Sequences;
			this.MaxSpeed = row.TryGetInt(TroopAbilityVO.COLUMN_maxSpeed);
			this.RotationSpeed = row.TryGetInt(TroopAbilityVO.COLUMN_newRotationSpeed);
			this.Auto = row.TryGetBool(TroopAbilityVO.COLUMN_auto);
			this.ClipCount = row.TryGetInt(TroopAbilityVO.COLUMN_clipCount);
			this.CooldownOnSpawn = row.TryGetBool(TroopAbilityVO.COLUMN_cooldownOnSpawn);
			this.WeaponTrailFxParams = row.TryGetFloatArray(TroopAbilityVO.COLUMN_weaponTrailFxParams);
			this.RetargetingOffset = row.TryGetUint(TroopAbilityVO.COLUMN_retargetingOffset);
			this.ClipRetargeting = row.TryGetBool(TroopAbilityVO.COLUMN_clipRetargeting);
			this.NewTargetOnReload = row.TryGetBool(TroopAbilityVO.COLUMN_newTargetOnReload);
			this.StrictCooldown = row.TryGetBool(TroopAbilityVO.COLUMN_strictCoolDown);
			this.DPS = row.TryGetInt(TroopAbilityVO.COLUMN_dps);
			this.AltGunLocators = row.TryGetIntArray(TroopAbilityVO.COLUMN_altGunLocators);
			string name = row.TryGetString(TroopAbilityVO.COLUMN_troopRole, "None");
			this.TroopRole = StringUtils.ParseEnum<TroopRole>(name);
			this.CrushesWalls = row.TryGetBool(TroopAbilityVO.COLUMN_crushesWalls);
			this.IgnoresWalls = row.TryGetBool(TroopAbilityVO.COLUMN_ignoresWalls);
		}
	}
}
