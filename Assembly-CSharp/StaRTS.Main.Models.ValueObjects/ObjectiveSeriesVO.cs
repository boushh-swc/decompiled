using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Globalization;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ObjectiveSeriesVO : IValueObject
	{
		public static int COLUMN_planetUid
		{
			get;
			private set;
		}

		public static int COLUMN_objCount
		{
			get;
			private set;
		}

		public static int COLUMN_objBucket
		{
			get;
			private set;
		}

		public static int COLUMN_objBucket2
		{
			get;
			private set;
		}

		public static int COLUMN_objBucket3
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_periodHours
		{
			get;
			private set;
		}

		public static int COLUMN_graceHours
		{
			get;
			private set;
		}

		public static int COLUMN_endDate
		{
			get;
			private set;
		}

		public static int COLUMN_specialEvent
		{
			get;
			private set;
		}

		public static int COLUMN_eventIcon
		{
			get;
			private set;
		}

		public static int COLUMN_eventPlayart
		{
			get;
			private set;
		}

		public static int COLUMN_eventDetailsart
		{
			get;
			private set;
		}

		public static int COLUMN_bundleName
		{
			get;
			private set;
		}

		public static int COLUMN_headerString
		{
			get;
			private set;
		}

		public static int COLUMN_objectiveString
		{
			get;
			private set;
		}

		public static int COLUMN_objectiveExpiringString
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string PlanetUid
		{
			get;
			set;
		}

		public int ObjCount
		{
			get;
			set;
		}

		public string ObjBucket
		{
			get;
			set;
		}

		public string ObjBucket2
		{
			get;
			set;
		}

		public string ObjBucket3
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

		public int PeriodHours
		{
			get;
			set;
		}

		public int GraceHours
		{
			get;
			set;
		}

		public bool SpecialEvent
		{
			get;
			set;
		}

		public string EventIcon
		{
			get;
			set;
		}

		public string EventPlayArt
		{
			get;
			set;
		}

		public string EventDetailsArt
		{
			get;
			set;
		}

		public string BundleName
		{
			get;
			set;
		}

		public string HeaderString
		{
			get;
			set;
		}

		public string ObjectiveString
		{
			get;
			set;
		}

		public string ObjectiveExpiringString
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.PlanetUid = row.TryGetString(ObjectiveSeriesVO.COLUMN_planetUid);
			this.ObjCount = row.TryGetInt(ObjectiveSeriesVO.COLUMN_objCount);
			this.ObjBucket = row.TryGetString(ObjectiveSeriesVO.COLUMN_objBucket);
			this.ObjBucket2 = row.TryGetString(ObjectiveSeriesVO.COLUMN_objBucket2);
			this.ObjBucket3 = row.TryGetString(ObjectiveSeriesVO.COLUMN_objBucket3);
			this.PeriodHours = row.TryGetInt(ObjectiveSeriesVO.COLUMN_periodHours);
			this.GraceHours = row.TryGetInt(ObjectiveSeriesVO.COLUMN_graceHours);
			this.SpecialEvent = row.TryGetBool(ObjectiveSeriesVO.COLUMN_specialEvent);
			this.EventIcon = row.TryGetString(ObjectiveSeriesVO.COLUMN_eventIcon);
			this.EventPlayArt = row.TryGetString(ObjectiveSeriesVO.COLUMN_eventPlayart);
			this.EventDetailsArt = row.TryGetString(ObjectiveSeriesVO.COLUMN_eventDetailsart);
			this.BundleName = row.TryGetString(ObjectiveSeriesVO.COLUMN_bundleName);
			this.HeaderString = row.TryGetString(ObjectiveSeriesVO.COLUMN_headerString);
			this.ObjectiveString = row.TryGetString(ObjectiveSeriesVO.COLUMN_objectiveString);
			this.ObjectiveExpiringString = row.TryGetString(ObjectiveSeriesVO.COLUMN_objectiveExpiringString);
			string text = row.TryGetString(ObjectiveSeriesVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				DateTime date = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				this.StartTime = DateUtils.GetSecondsFromEpoch(date);
			}
			else
			{
				this.StartTime = 0;
				Service.Logger.Warn("ObjectiveSeries VO Start Date Not Specified");
			}
			string text2 = row.TryGetString(ObjectiveSeriesVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				DateTime date2 = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				this.EndTime = DateUtils.GetSecondsFromEpoch(date2);
			}
			else
			{
				this.EndTime = 2147483647;
				Service.Logger.Warn("ObjectiveSeries VO End Date Not Specified");
			}
		}
	}
}
