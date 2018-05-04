using StaRTS.Main.Controllers;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TurretTypeVO : IShooterVO, ITrackerVO, IValueObject
	{
		public static int COLUMN_trackerName
		{
			get;
			private set;
		}

		public static int COLUMN_gunSequence
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

		public static int COLUMN_strictCoolDown
		{
			get;
			private set;
		}

		public static int COLUMN_clipRetargeting
		{
			get;
			private set;
		}

		public static int COLUMN_projectileType
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string TrackerName
		{
			get;
			set;
		}

		public int[] GunSequence
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

		public bool OverWalls
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

		public TroopRole TroopRole
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

		public ProjectileTypeVO ProjectileType
		{
			get;
			set;
		}

		public Dictionary<int, int> Sequences
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.TrackerName = row.TryGetString(TurretTypeVO.COLUMN_trackerName);
			this.Preference = new int[24];
			int i = 0;
			int num = 24;
			while (i < num)
			{
				this.Preference[i] = 0;
				i++;
			}
			this.Preference[1] = row.TryGetInt(TurretTypeVO.COLUMN_wall);
			this.Preference[2] = row.TryGetInt(TurretTypeVO.COLUMN_building);
			this.Preference[3] = row.TryGetInt(TurretTypeVO.COLUMN_storage);
			this.Preference[4] = row.TryGetInt(TurretTypeVO.COLUMN_resource);
			this.Preference[5] = row.TryGetInt(TurretTypeVO.COLUMN_turret);
			this.Preference[6] = row.TryGetInt(TurretTypeVO.COLUMN_HQ);
			this.Preference[7] = row.TryGetInt(TurretTypeVO.COLUMN_shield);
			this.Preference[8] = row.TryGetInt(TurretTypeVO.COLUMN_shieldGenerator);
			this.Preference[9] = row.TryGetInt(TurretTypeVO.COLUMN_infantry);
			this.Preference[10] = row.TryGetInt(TurretTypeVO.COLUMN_bruiserInfantry);
			this.Preference[11] = row.TryGetInt(TurretTypeVO.COLUMN_vehicle);
			this.Preference[12] = row.TryGetInt(TurretTypeVO.COLUMN_bruiserVehicle);
			this.Preference[13] = row.TryGetInt(TurretTypeVO.COLUMN_heroInfantry);
			this.Preference[14] = row.TryGetInt(TurretTypeVO.COLUMN_heroVehicle);
			this.Preference[15] = row.TryGetInt(TurretTypeVO.COLUMN_heroBruiserInfantry);
			this.Preference[16] = row.TryGetInt(TurretTypeVO.COLUMN_heroBruiserVehicle);
			this.Preference[17] = row.TryGetInt(TurretTypeVO.COLUMN_flierInfantry);
			this.Preference[18] = row.TryGetInt(TurretTypeVO.COLUMN_flierVehicle);
			this.Preference[19] = row.TryGetInt(TurretTypeVO.COLUMN_healerInfantry);
			this.Preference[20] = row.TryGetInt(TurretTypeVO.COLUMN_trap);
			this.Preference[21] = row.TryGetInt(TurretTypeVO.COLUMN_champion);
			this.PreferencePercentile = row.TryGetInt(TurretTypeVO.COLUMN_targetPreferenceStrength);
			this.NearnessPercentile = 100 - this.PreferencePercentile;
			this.Damage = row.TryGetInt(TurretTypeVO.COLUMN_damage);
			this.DPS = row.TryGetInt(TurretTypeVO.COLUMN_dps);
			this.FavoriteTargetType = row.TryGetString(TurretTypeVO.COLUMN_favoriteTargetType);
			this.MinAttackRange = row.TryGetUint(TurretTypeVO.COLUMN_minAttackRange);
			this.MaxAttackRange = row.TryGetUint(TurretTypeVO.COLUMN_maxAttackRange);
			this.ShotCount = row.TryGetUint(TurretTypeVO.COLUMN_shotCount);
			this.WarmupDelay = row.TryGetUint(TurretTypeVO.COLUMN_chargeTime);
			this.AnimationDelay = row.TryGetUint(TurretTypeVO.COLUMN_animationDelay);
			this.ShotDelay = row.TryGetUint(TurretTypeVO.COLUMN_shotDelay);
			this.CooldownDelay = row.TryGetUint(TurretTypeVO.COLUMN_reload);
			this.StrictCooldown = row.TryGetBool(TurretTypeVO.COLUMN_strictCoolDown);
			this.ClipRetargeting = row.TryGetBool(TurretTypeVO.COLUMN_clipRetargeting);
			this.ProjectileType = Service.StaticDataController.Get<ProjectileTypeVO>(row.TryGetString(TurretTypeVO.COLUMN_projectileType));
			if (this.ProjectileType.IsBeam && (long)this.ProjectileType.BeamDamageLength < (long)((ulong)this.MaxAttackRange))
			{
				Service.Logger.WarnFormat("Turret {0} can target something it can't damage", new object[]
				{
					this.Uid
				});
			}
			ValueObjectController valueObjectController = Service.ValueObjectController;
			SequencePair gunSequences = valueObjectController.GetGunSequences(this.Uid, row.TryGetString(TurretTypeVO.COLUMN_gunSequence));
			this.GunSequence = gunSequences.GunSequence;
			this.Sequences = gunSequences.Sequences;
			this.TroopRole = TroopRole.None;
		}
	}
}
