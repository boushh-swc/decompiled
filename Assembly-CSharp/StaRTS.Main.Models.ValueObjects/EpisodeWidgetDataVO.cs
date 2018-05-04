using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Globalization;
using System.Xml;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodeWidgetDataVO : IValueObject, IEpisodeTimeVO
	{
		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_stateId
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

		public string StateId
		{
			get;
			private set;
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
			this.StateId = row.TryGetString(EpisodeWidgetDataVO.COLUMN_stateId);
			string text = row.TryGetString(EpisodeWidgetDataVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				this.StartTime = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				this.StartTime = DateTime.MinValue;
				Service.Logger.Warn("EpisodeWidgetDataVO Start Date Not Specified");
			}
			string text2 = row.TryGetString(EpisodeWidgetDataVO.COLUMN_duration);
			if (!string.IsNullOrEmpty(text2))
			{
				this.Duration = XmlConvert.ToTimeSpan(text2);
				this.EndTime = this.StartTime.AddSeconds(this.Duration.TotalSeconds);
			}
			else
			{
				this.EndTime = this.StartTime;
				Service.Logger.Warn("Duration not defined for EpisodeWidgetDataVO: " + this.Uid);
			}
			this.Priority = row.TryGetInt(EpisodeWidgetDataVO.COLUMN_priority);
		}
	}
}
