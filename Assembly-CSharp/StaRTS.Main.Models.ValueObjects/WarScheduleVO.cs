using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Globalization;

namespace StaRTS.Main.Models.ValueObjects
{
	public class WarScheduleVO : IValueObject
	{
		public static int COLUMN_warPlanetUid
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

		public string Uid
		{
			get;
			set;
		}

		public string WarPlanetUid
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

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.WarPlanetUid = row.TryGetString(WarScheduleVO.COLUMN_warPlanetUid);
			string text = row.TryGetString(WarScheduleVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					DateTime date = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
					this.StartTime = DateUtils.GetSecondsFromEpoch(date);
				}
				catch (Exception)
				{
					this.StartTime = 0;
					Service.Logger.Warn("WarScheduleVO:: War Schedule CMS Start Date Format Error: " + this.Uid);
				}
			}
			else
			{
				this.StartTime = 0;
				Service.Logger.Warn("WarScheduleVO:: War Schedule CMS Start Date Not Specified For: " + this.Uid);
			}
			string text2 = row.TryGetString(WarScheduleVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				try
				{
					DateTime date2 = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
					this.EndTime = DateUtils.GetSecondsFromEpoch(date2);
				}
				catch (Exception)
				{
					this.EndTime = 2147483647;
					Service.Logger.Warn("WarScheduleVO:: War Schedule CMS End Date Format Error: " + this.Uid);
				}
			}
			else
			{
				this.EndTime = 2147483647;
			}
		}
	}
}
