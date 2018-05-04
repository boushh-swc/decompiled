using StaRTS.Main.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TournamentVO : ITimedEventVO, IValueObject
	{
		public static int COLUMN_planetId
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_endDate
		{
			get;
			private set;
		}

		public static int COLUMN_backgroundTextureName
		{
			get;
			private set;
		}

		public static int COLUMN_startingRating
		{
			get;
			private set;
		}

		public static int COLUMN_rewardGroupId
		{
			get;
			private set;
		}

		public static int COLUMN_rewardBanner
		{
			get;
			private set;
		}

		public static int COLUMN_titleString
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

		public int StartTimestamp
		{
			get;
			set;
		}

		public int EndTimestamp
		{
			get;
			set;
		}

		public string BackgroundTextureName
		{
			get;
			set;
		}

		public int StartingRating
		{
			get;
			set;
		}

		public string RewardGroupId
		{
			get;
			set;
		}

		public int RewardBanner
		{
			get;
			set;
		}

		public string TitleString
		{
			get;
			set;
		}

		public bool UseTimeZoneOffset
		{
			get
			{
				return false;
			}
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.PlanetId = row.TryGetString(TournamentVO.COLUMN_planetId);
			string dateString = row.TryGetString(TournamentVO.COLUMN_startDate);
			string dateString2 = row.TryGetString(TournamentVO.COLUMN_endDate);
			this.BackgroundTextureName = row.TryGetString(TournamentVO.COLUMN_backgroundTextureName);
			this.StartingRating = row.TryGetInt(TournamentVO.COLUMN_startingRating);
			this.RewardGroupId = row.TryGetString(TournamentVO.COLUMN_rewardGroupId);
			this.RewardBanner = row.TryGetInt(TournamentVO.COLUMN_rewardBanner);
			this.TitleString = row.TryGetString(TournamentVO.COLUMN_titleString);
			this.StartTimestamp = TimedEventUtils.GetTimestamp(this.Uid, dateString);
			this.EndTimestamp = TimedEventUtils.GetTimestamp(this.Uid, dateString2);
		}

		public int GetUpcomingDurationSeconds()
		{
			return GameConstants.TOURNAMENT_HOURS_UPCOMING * 3600;
		}

		public int GetClosingDurationSeconds()
		{
			return GameConstants.TOURNAMENT_HOURS_CLOSING * 3600;
		}
	}
}
