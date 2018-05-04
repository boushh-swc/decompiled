using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Globalization;
using System.Xml;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodeDataVO : IValueObject, IEpisodeTimeVO
	{
		public static int COLUMN_uid
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

		public static int COLUMN_panel
		{
			get;
			private set;
		}

		public static int COLUMN_priority
		{
			get;
			private set;
		}

		public static int COLUMN_tasks
		{
			get;
			private set;
		}

		public static int COLUMN_grindTask
		{
			get;
			private set;
		}

		public static int COLUMN_pointScale
		{
			get;
			private set;
		}

		public string Uid
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

		private TimeSpan Duration
		{
			get;
			set;
		}

		public string Panel
		{
			get;
			private set;
		}

		public string[] Tasks
		{
			get;
			private set;
		}

		public string GrindTask
		{
			get;
			private set;
		}

		public string PointScale
		{
			get;
			private set;
		}

		public int Priority
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			string text = row.TryGetString(EpisodeDataVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				this.StartTime = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				this.StartTime = DateTime.MinValue;
				Service.Logger.Warn("EpisodeDataVO Start Date Not Specified");
			}
			string text2 = row.TryGetString(EpisodeDataVO.COLUMN_duration);
			if (!string.IsNullOrEmpty(text2))
			{
				this.Duration = XmlConvert.ToTimeSpan(text2);
				this.EndTime = this.StartTime.AddSeconds(this.Duration.TotalSeconds);
			}
			else
			{
				this.EndTime = this.StartTime;
				Service.Logger.Warn("Duration not defined for EpisodeData: " + this.Uid);
			}
			this.Panel = row.TryGetString(EpisodeDataVO.COLUMN_panel);
			this.Priority = row.TryGetInt(EpisodeDataVO.COLUMN_priority);
			this.Tasks = row.TryGetStringArray(EpisodeDataVO.COLUMN_tasks);
			this.GrindTask = row.TryGetString(EpisodeDataVO.COLUMN_grindTask);
			this.PointScale = row.TryGetString(EpisodeDataVO.COLUMN_pointScale);
		}
	}
}
