using StaRTS.Main.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SaleTypeVO : ITimedEventVO, IValueObject
	{
		public static int COLUMN_title
		{
			get;
			private set;
		}

		public static int COLUMN_saleItems
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

		public string Title
		{
			get;
			set;
		}

		public string[] SaleItems
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
			this.Title = row.TryGetString(SaleTypeVO.COLUMN_title);
			this.SaleItems = row.TryGetStringArray(SaleTypeVO.COLUMN_saleItems);
			string dateString = row.TryGetString(SaleTypeVO.COLUMN_startDate);
			string dateString2 = row.TryGetString(SaleTypeVO.COLUMN_endDate);
			this.StartTimestamp = TimedEventUtils.GetTimestamp(this.Uid, dateString);
			this.EndTimestamp = TimedEventUtils.GetTimestamp(this.Uid, dateString2);
		}

		public int GetUpcomingDurationSeconds()
		{
			return 0;
		}

		public int GetClosingDurationSeconds()
		{
			return 0;
		}
	}
}
