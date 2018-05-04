using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class RaidMissionPoolVO : IValueObject
	{
		public static int COLUMN_HQ
		{
			get;
			private set;
		}

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_campaignMissions
		{
			get;
			private set;
		}

		public static int COLUMN_crate1
		{
			get;
			private set;
		}

		public static int COLUMN_crate2
		{
			get;
			private set;
		}

		public static int COLUMN_crate3
		{
			get;
			private set;
		}

		public static int COLUMN_descCondition1
		{
			get;
			private set;
		}

		public static int COLUMN_descCondition2
		{
			get;
			private set;
		}

		public static int COLUMN_descCondition3
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] Missions
		{
			get;
			private set;
		}

		public FactionType Faction
		{
			get;
			private set;
		}

		public string Crate1StarRewardId
		{
			get;
			private set;
		}

		public string Crate2StarRewardId
		{
			get;
			private set;
		}

		public string Crate3StarRewardId
		{
			get;
			private set;
		}

		public string Condition1StarRewardStringId
		{
			get;
			private set;
		}

		public string Condition2StarRewardStringId
		{
			get;
			private set;
		}

		public string Condition3StarRewardStringId
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(RaidMissionPoolVO.COLUMN_faction));
			this.Missions = row.TryGetStringArray(RaidMissionPoolVO.COLUMN_campaignMissions);
			this.Crate1StarRewardId = row.TryGetString(RaidMissionPoolVO.COLUMN_crate1);
			this.Crate2StarRewardId = row.TryGetString(RaidMissionPoolVO.COLUMN_crate2);
			this.Crate3StarRewardId = row.TryGetString(RaidMissionPoolVO.COLUMN_crate3);
			this.Condition1StarRewardStringId = row.TryGetString(RaidMissionPoolVO.COLUMN_descCondition1);
			this.Condition2StarRewardStringId = row.TryGetString(RaidMissionPoolVO.COLUMN_descCondition2);
			this.Condition3StarRewardStringId = row.TryGetString(RaidMissionPoolVO.COLUMN_descCondition3);
		}
	}
}
