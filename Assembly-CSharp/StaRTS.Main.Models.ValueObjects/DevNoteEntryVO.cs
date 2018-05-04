using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Globalization;

namespace StaRTS.Main.Models.ValueObjects
{
	public class DevNoteEntryVO : ITimestamped, ICallToAction, IValueObject
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

		public static int COLUMN_titleText
		{
			get;
			private set;
		}

		public static int COLUMN_bodyText
		{
			get;
			private set;
		}

		public static int COLUMN_image
		{
			get;
			private set;
		}

		public static int COLUMN_btn1
		{
			get;
			private set;
		}

		public static int COLUMN_btn1action
		{
			get;
			private set;
		}

		public static int COLUMN_btn1data
		{
			get;
			private set;
		}

		public static int COLUMN_btn2
		{
			get;
			private set;
		}

		public static int COLUMN_btn2action
		{
			get;
			private set;
		}

		public static int COLUMN_btn2data
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

		public string TitleText
		{
			get;
			set;
		}

		public string BodyText
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public string Btn1
		{
			get;
			set;
		}

		public string Btn1Action
		{
			get;
			set;
		}

		public string Btn1Data
		{
			get;
			set;
		}

		public string Btn2
		{
			get;
			set;
		}

		public string Btn2Action
		{
			get;
			set;
		}

		public string Btn2Data
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.TitleText = row.TryGetString(DevNoteEntryVO.COLUMN_titleText);
			this.BodyText = row.TryGetString(DevNoteEntryVO.COLUMN_bodyText);
			this.Image = row.TryGetString(DevNoteEntryVO.COLUMN_image);
			string text = row.TryGetString(DevNoteEntryVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				DateTime date = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				this.StartTime = DateUtils.GetSecondsFromEpoch(date);
			}
			else
			{
				this.StartTime = 0;
				Service.Logger.Warn("DevNoteEntry VO Start Date Not Specified");
			}
			string text2 = row.TryGetString(DevNoteEntryVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				DateTime date2 = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				this.EndTime = DateUtils.GetSecondsFromEpoch(date2);
			}
			else
			{
				this.EndTime = 2147483647;
			}
			this.Btn1 = row.TryGetString(DevNoteEntryVO.COLUMN_btn1);
			this.Btn1Action = row.TryGetString(DevNoteEntryVO.COLUMN_btn1action);
			this.Btn1Data = row.TryGetString(DevNoteEntryVO.COLUMN_btn1data);
			if (string.IsNullOrEmpty(this.Btn1Data))
			{
				this.Btn1Data = string.Empty;
			}
			this.Btn2 = row.TryGetString(DevNoteEntryVO.COLUMN_btn2);
			this.Btn2Action = row.TryGetString(DevNoteEntryVO.COLUMN_btn2action);
			this.Btn2Data = row.TryGetString(DevNoteEntryVO.COLUMN_btn2data);
			if (string.IsNullOrEmpty(this.Btn2Data))
			{
				this.Btn2Data = string.Empty;
			}
		}
	}
}
