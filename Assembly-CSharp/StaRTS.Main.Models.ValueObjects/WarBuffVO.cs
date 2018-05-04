using StaRTS.Main.Models.Squads.War;
using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class WarBuffVO : IValueObject
	{
		public static int COLUMN_planetId
		{
			get;
			private set;
		}

		public static int COLUMN_masterNeutralBuildingUid
		{
			get;
			private set;
		}

		public static int COLUMN_masterEmpireBuildingUid
		{
			get;
			private set;
		}

		public static int COLUMN_masterRebelBuildingUid
		{
			get;
			private set;
		}

		public static int COLUMN_troopBuffUid
		{
			get;
			private set;
		}

		public static int COLUMN_buildingBuffUid
		{
			get;
			private set;
		}

		public static int COLUMN_buffType
		{
			get;
			private set;
		}

		public static int COLUMN_buffBaseName
		{
			get;
			private set;
		}

		public static int COLUMN_buffStringTitle
		{
			get;
			private set;
		}

		public static int COLUMN_buffStringDesc
		{
			get;
			private set;
		}

		public static int COLUMN_buffIcon
		{
			get;
			private set;
		}

		public static int COLUMN_empireBattlesByLevel
		{
			get;
			private set;
		}

		public static int COLUMN_rebelBattlesByLevel
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string PlanetId
		{
			get;
			set;
		}

		public string MasterNeutralBuildingUid
		{
			get;
			set;
		}

		public string MasterEmpireBuildingUid
		{
			get;
			set;
		}

		public string MasterRebelBuildingUid
		{
			get;
			set;
		}

		public string TroopBuffUid
		{
			get;
			set;
		}

		public string BuildingBuffUid
		{
			get;
			set;
		}

		public SquadWarBuffType BuffType
		{
			get;
			set;
		}

		public string BuffBaseName
		{
			get;
			set;
		}

		public string BuffStringTitle
		{
			get;
			set;
		}

		public string BuffStringDesc
		{
			get;
			set;
		}

		public string BuffIcon
		{
			get;
			set;
		}

		public string[] EmpireBattlesByLevel
		{
			get;
			set;
		}

		public string[] RebelBattlesByLevel
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.PlanetId = row.TryGetString(WarBuffVO.COLUMN_planetId);
			this.MasterNeutralBuildingUid = row.TryGetString(WarBuffVO.COLUMN_masterNeutralBuildingUid);
			this.MasterEmpireBuildingUid = row.TryGetString(WarBuffVO.COLUMN_masterEmpireBuildingUid);
			this.MasterRebelBuildingUid = row.TryGetString(WarBuffVO.COLUMN_masterRebelBuildingUid);
			this.TroopBuffUid = row.TryGetString(WarBuffVO.COLUMN_troopBuffUid);
			this.BuildingBuffUid = row.TryGetString(WarBuffVO.COLUMN_buildingBuffUid);
			this.BuffType = StringUtils.ParseEnum<SquadWarBuffType>(row.TryGetString(WarBuffVO.COLUMN_buffType));
			this.BuffBaseName = row.TryGetString(WarBuffVO.COLUMN_buffBaseName);
			this.BuffStringDesc = row.TryGetString(WarBuffVO.COLUMN_buffStringDesc);
			this.BuffStringTitle = row.TryGetString(WarBuffVO.COLUMN_buffStringTitle);
			this.BuffIcon = row.TryGetString(WarBuffVO.COLUMN_buffIcon);
			string text = row.TryGetString(WarBuffVO.COLUMN_empireBattlesByLevel);
			if (!string.IsNullOrEmpty(text))
			{
				this.EmpireBattlesByLevel = text.Split(new char[0]);
			}
			string text2 = row.TryGetString(WarBuffVO.COLUMN_rebelBattlesByLevel);
			if (!string.IsNullOrEmpty(text2))
			{
				this.RebelBattlesByLevel = text2.Split(new char[0]);
			}
		}
	}
}
