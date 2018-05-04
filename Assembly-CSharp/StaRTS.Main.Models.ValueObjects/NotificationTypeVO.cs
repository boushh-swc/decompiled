using StaRTS.Main.Controllers;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class NotificationTypeVO : IValueObject
	{
		public static int COLUMN_soundName
		{
			get;
			private set;
		}

		public static int COLUMN_desc
		{
			get;
			private set;
		}

		public static int COLUMN_minCompletionTime
		{
			get;
			private set;
		}

		public static int COLUMN_repeatTime
		{
			get;
			private set;
		}

		public static int COLUMN_validTimeRange
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Desc
		{
			get;
			set;
		}

		public string SoundName
		{
			get;
			set;
		}

		public int MinCompletionTime
		{
			get;
			set;
		}

		public int RepeatTime
		{
			get;
			set;
		}

		public int EarliestValidTime
		{
			get;
			private set;
		}

		public int LatestValidTime
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.SoundName = row.TryGetString(NotificationTypeVO.COLUMN_soundName);
			if (this.SoundName.Equals(string.Empty))
			{
				this.SoundName = null;
			}
			this.Desc = row.TryGetString(NotificationTypeVO.COLUMN_desc);
			this.MinCompletionTime = row.TryGetInt(NotificationTypeVO.COLUMN_minCompletionTime);
			this.RepeatTime = row.TryGetInt(NotificationTypeVO.COLUMN_repeatTime);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			List<StrIntPair> strIntPairs = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(NotificationTypeVO.COLUMN_validTimeRange));
			int earliestValidTime = 10;
			int latestValidTime = 21;
			if (strIntPairs != null)
			{
				int i = 0;
				int count = strIntPairs.Count;
				while (i < count)
				{
					StrIntPair strIntPair = strIntPairs[i];
					string strKey = strIntPair.StrKey;
					if (strKey == "earliest")
					{
						earliestValidTime = strIntPair.IntVal;
					}
					else if (strKey == "latest")
					{
						latestValidTime = strIntPair.IntVal;
					}
					i++;
				}
			}
			this.EarliestValidTime = earliestValidTime;
			this.LatestValidTime = latestValidTime;
		}
	}
}
