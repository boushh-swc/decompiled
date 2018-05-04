using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodeTaskVO : IValueObject
	{
		public bool IsSignificant;

		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_actions
		{
			get;
			private set;
		}

		public static int COLUMN_crateId
		{
			get;
			private set;
		}

		public static int COLUMN_crateIconAsset
		{
			get;
			private set;
		}

		public static int COLUMN_timeGate
		{
			get;
			private set;
		}

		public static int COLUMN_storyId
		{
			get;
			private set;
		}

		public static int COLUMN_isSignificant
		{
			get;
			private set;
		}

		public static int COLUMN_empireHeaderString
		{
			get;
			private set;
		}

		public static int COLUMN_rebelHeaderString
		{
			get;
			private set;
		}

		public static int COLUMN_empireBodyString
		{
			get;
			private set;
		}

		public static int COLUMN_rebelBodyString
		{
			get;
			private set;
		}

		public static int COLUMN_empireRewardItemIds
		{
			get;
			private set;
		}

		public static int COLUMN_rebelRewardItemIds
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] Actions
		{
			get;
			set;
		}

		public string CrateId
		{
			get;
			set;
		}

		public string CrateIconAsset
		{
			get;
			set;
		}

		public int TimeGate
		{
			get;
			set;
		}

		public string StoryID
		{
			get;
			private set;
		}

		public string EmpireHeaderString
		{
			get;
			set;
		}

		public string RebelHeaderString
		{
			get;
			set;
		}

		public string EmpireBodyString
		{
			get;
			set;
		}

		public string RebelBodyString
		{
			get;
			set;
		}

		public string[] EmpireRewardItemIds
		{
			get;
			set;
		}

		public string[] RebelRewardItemIds
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Actions = row.TryGetStringArray(EpisodeTaskVO.COLUMN_actions);
			this.CrateId = row.TryGetString(EpisodeTaskVO.COLUMN_crateId);
			this.CrateIconAsset = row.TryGetString(EpisodeTaskVO.COLUMN_crateIconAsset);
			this.TimeGate = row.TryGetInt(EpisodeTaskVO.COLUMN_timeGate);
			this.StoryID = row.TryGetString(EpisodeTaskVO.COLUMN_storyId);
			this.IsSignificant = row.TryGetBool(EpisodeTaskVO.COLUMN_isSignificant);
			this.EmpireHeaderString = row.TryGetString(EpisodeTaskVO.COLUMN_empireHeaderString);
			this.RebelHeaderString = row.TryGetString(EpisodeTaskVO.COLUMN_rebelHeaderString);
			this.EmpireBodyString = row.TryGetString(EpisodeTaskVO.COLUMN_empireBodyString);
			this.RebelBodyString = row.TryGetString(EpisodeTaskVO.COLUMN_rebelBodyString);
			this.EmpireRewardItemIds = row.TryGetStringArray(EpisodeTaskVO.COLUMN_empireRewardItemIds);
			this.RebelRewardItemIds = row.TryGetStringArray(EpisodeTaskVO.COLUMN_rebelRewardItemIds);
		}

		public string GetHeaderString(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireHeaderString;
			}
			if (faction == FactionType.Rebel)
			{
				return this.RebelHeaderString;
			}
			return string.Empty;
		}

		public string GetBodyString(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireBodyString;
			}
			if (faction == FactionType.Rebel)
			{
				return this.RebelBodyString;
			}
			return string.Empty;
		}

		public string[] GetRewardItemIds(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireRewardItemIds;
			}
			if (faction == FactionType.Rebel)
			{
				return this.RebelRewardItemIds;
			}
			return null;
		}
	}
}
