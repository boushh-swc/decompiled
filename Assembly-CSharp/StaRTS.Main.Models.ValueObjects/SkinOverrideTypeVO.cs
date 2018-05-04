using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SkinOverrideTypeVO : IValueObject
	{
		public static int COLUMN_projectileType
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

		public static int COLUMN_gunSequence
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

		public static int COLUMN_role
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

		public string Uid
		{
			get;
			set;
		}

		public ProjectileTypeVO ProjectileType
		{
			get;
			set;
		}

		public uint ShotCount
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

		public int[] GunSequence
		{
			get;
			set;
		}

		public Dictionary<int, int> Sequences
		{
			get;
			private set;
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

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			StaticDataController staticDataController = Service.StaticDataController;
			string value = row.TryGetString(SkinOverrideTypeVO.COLUMN_projectileType);
			if (!string.IsNullOrEmpty(value))
			{
				this.ProjectileType = staticDataController.Get<ProjectileTypeVO>(row.TryGetString(SkinOverrideTypeVO.COLUMN_projectileType));
			}
			this.WarmupDelay = row.TryGetUint(SkinOverrideTypeVO.COLUMN_chargeTime);
			this.AnimationDelay = row.TryGetUint(SkinOverrideTypeVO.COLUMN_animationDelay);
			this.ShotDelay = row.TryGetUint(SkinOverrideTypeVO.COLUMN_shotDelay);
			this.CooldownDelay = row.TryGetUint(SkinOverrideTypeVO.COLUMN_reload);
			this.ShotCount = row.TryGetUint(SkinOverrideTypeVO.COLUMN_shotCount);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			SequencePair gunSequences = valueObjectController.GetGunSequences(this.Uid, row.TryGetString(SkinOverrideTypeVO.COLUMN_gunSequence));
			this.GunSequence = gunSequences.GunSequence;
			this.Sequences = gunSequences.Sequences;
			this.ViewRange = row.TryGetUint(SkinOverrideTypeVO.COLUMN_viewRange);
			this.MinAttackRange = row.TryGetUint(SkinOverrideTypeVO.COLUMN_minAttackRange);
			this.MaxAttackRange = row.TryGetUint(SkinOverrideTypeVO.COLUMN_maxAttackRange);
			this.TroopRole = StringUtils.ParseEnum<TroopRole>(row.TryGetString(SkinOverrideTypeVO.COLUMN_role));
			this.CrushesWalls = row.TryGetBool(SkinOverrideTypeVO.COLUMN_crushesWalls);
			this.IgnoresWalls = row.TryGetBool(SkinOverrideTypeVO.COLUMN_ignoresWalls);
			this.Preference = new int[24];
			int i = 0;
			int num = 24;
			while (i < num)
			{
				this.Preference[i] = -1;
				i++;
			}
			this.Preference[1] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_wall, -1);
			this.Preference[2] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_building, -1);
			this.Preference[3] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_storage, -1);
			this.Preference[4] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_resource, -1);
			this.Preference[5] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_turret, -1);
			this.Preference[6] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_HQ, -1);
			this.Preference[7] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_shield, -1);
			this.Preference[8] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_shieldGenerator, -1);
			this.Preference[9] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_infantry, -1);
			this.Preference[10] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_bruiserInfantry, -1);
			this.Preference[11] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_vehicle, -1);
			this.Preference[12] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_bruiserVehicle, -1);
			this.Preference[13] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_heroInfantry, -1);
			this.Preference[14] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_heroVehicle, -1);
			this.Preference[15] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_heroBruiserInfantry, -1);
			this.Preference[16] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_heroBruiserVechicle, -1);
			this.Preference[17] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_flierInfantry, -1);
			this.Preference[18] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_flierVehicle, -1);
			this.Preference[19] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_healerInfantry, -1);
			this.Preference[20] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_trap, -1);
			this.Preference[21] = row.TryGetInt(SkinOverrideTypeVO.COLUMN_champion, -1);
			this.PreferencePercentile = row.TryGetInt(SkinOverrideTypeVO.COLUMN_targetPreferenceStrength, -1);
		}
	}
}
