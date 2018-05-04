using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CommandCenterVO : ITimestamped, IValueObject, ICallToAction
	{
		public List<AudienceCondition> AudienceConditions;

		public static int COLUMN_layout
		{
			get;
			private set;
		}

		public static int COLUMN_image
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

		public static int COLUMN_priority
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

		public static int COLUMN_carouselAutoSwipe
		{
			get;
			private set;
		}

		public static int COLUMN_audienceConditions
		{
			get;
			private set;
		}

		public static int COLUMN_isPromo
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int Layout
		{
			get;
			set;
		}

		public string Image
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

		public int Priority
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

		public float CarouselAutoSwipe
		{
			get;
			set;
		}

		public bool IsPromo
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Layout = row.TryGetInt(CommandCenterVO.COLUMN_layout);
			this.Image = row.TryGetString(CommandCenterVO.COLUMN_image);
			this.TitleText = row.TryGetString(CommandCenterVO.COLUMN_titleText);
			this.BodyText = row.TryGetString(CommandCenterVO.COLUMN_bodyText);
			this.Btn1 = row.TryGetString(CommandCenterVO.COLUMN_btn1);
			this.Btn1Action = row.TryGetString(CommandCenterVO.COLUMN_btn1action);
			this.Btn1Data = row.TryGetString(CommandCenterVO.COLUMN_btn1data);
			this.Btn2 = row.TryGetString(CommandCenterVO.COLUMN_btn2);
			this.Btn2Action = row.TryGetString(CommandCenterVO.COLUMN_btn2action);
			this.Btn2Data = row.TryGetString(CommandCenterVO.COLUMN_btn2data);
			if (string.IsNullOrEmpty(this.Btn1Data))
			{
				this.Btn1Data = string.Empty;
			}
			if (string.IsNullOrEmpty(this.Btn2Data))
			{
				this.Btn2Data = string.Empty;
			}
			this.Priority = row.TryGetInt(CommandCenterVO.COLUMN_priority, 0);
			string text = row.TryGetString(CommandCenterVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				DateTime date = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				this.StartTime = DateUtils.GetSecondsFromEpoch(date);
			}
			else
			{
				this.StartTime = 0;
				Service.Logger.Warn("CommandCenter VO Start Date Not Specified");
			}
			string text2 = row.TryGetString(CommandCenterVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				DateTime date2 = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				this.EndTime = DateUtils.GetSecondsFromEpoch(date2);
			}
			else
			{
				this.EndTime = 2147483647;
			}
			this.CarouselAutoSwipe = row.TryGetFloat(CommandCenterVO.COLUMN_carouselAutoSwipe);
			this.AudienceConditions = new List<AudienceCondition>();
			string[] array = row.TryGetStringArray(CommandCenterVO.COLUMN_audienceConditions);
			if (array != null)
			{
				int i = 0;
				int num = array.Length;
				while (i < num)
				{
					this.AudienceConditions.Add(new AudienceCondition(array[i]));
					i++;
				}
			}
			this.IsPromo = row.TryGetBool(CommandCenterVO.COLUMN_isPromo);
		}
	}
}
