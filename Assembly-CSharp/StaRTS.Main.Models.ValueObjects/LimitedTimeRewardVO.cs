using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class LimitedTimeRewardVO : IValueObject
	{
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

		public static int COLUMN_title
		{
			get;
			private set;
		}

		public static int COLUMN_rewardData
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int StartTime
		{
			get;
			set;
		}

		public int EndTime
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string RewardUid
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Title = row.TryGetString(LimitedTimeRewardVO.COLUMN_title);
			this.RewardUid = row.TryGetString(LimitedTimeRewardVO.COLUMN_rewardData);
			this.StartTime = DateUtils.GetSecondsFromString(row.TryGetString(LimitedTimeRewardVO.COLUMN_startDate), 0);
			this.EndTime = DateUtils.GetSecondsFromString(row.TryGetString(LimitedTimeRewardVO.COLUMN_endDate), 2147483647);
		}
	}
}
