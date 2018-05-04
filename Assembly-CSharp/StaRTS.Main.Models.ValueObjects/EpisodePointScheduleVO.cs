using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;
using System.Globalization;
using System.Xml;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodePointScheduleVO : IValueObject
	{
		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_scaleId
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_duration
		{
			get;
			private set;
		}

		public static int COLUMN_priority
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string ScaleId
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public int StartTimeEpochSecs
		{
			get;
			private set;
		}

		public int EndTimeEpochSecs
		{
			get;
			private set;
		}

		private TimeSpan Duration
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.ScaleId = row.TryGetString(EpisodePointScheduleVO.COLUMN_scaleId);
			this.Priority = row.TryGetInt(EpisodePointScheduleVO.COLUMN_priority);
			string text = row.TryGetString(EpisodePointScheduleVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				this.StartTime = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				this.StartTime = DateTime.MinValue;
			}
			string text2 = row.TryGetString(EpisodePointScheduleVO.COLUMN_duration);
			if (!string.IsNullOrEmpty(text2))
			{
				this.Duration = XmlConvert.ToTimeSpan(text2);
				this.EndTime = this.StartTime.AddSeconds(this.Duration.TotalSeconds);
			}
			else
			{
				this.EndTime = this.StartTime;
			}
			this.StartTimeEpochSecs = DateUtils.GetSecondsFromEpoch(this.StartTime);
			this.EndTimeEpochSecs = DateUtils.GetSecondsFromEpoch(this.EndTime);
		}
	}
}
