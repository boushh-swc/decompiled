using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Utils.Chat
{
	public static class ChatTimeConversionUtils
	{
		private const string CHAT_TIME_STR = "{0} {1} GMT";

		private const string S_ACTIVE_DAYS_AGO = "s_ActiveDaysAgo";

		private const string ACTIVE_DAY_AGO = "ACTIVE_DAY_AGO";

		private const string S_ACTIVE_HOURS_AGO = "s_ActiveHoursAgo";

		private const string ACTIVE_HOUR_AGO = "ACTIVE_HOUR_AGO";

		private const string S_ACTIVE_MINS_AGO = "s_ActiveMinsAgo";

		private const string ACTIVE_MIN_AGO = "ACTIVE_MIN_AGO";

		private const string ACTIVE_LESS_MIN_AGO = "ACTIVE_LESS_MIN_AGO";

		private const string S_DAYS_AGO = "s_DaysAgo";

		private const string DAY_AGO = "DAY_AGO";

		private const string S_HOURS_AGO = "s_HoursAgo";

		private const string HOUR_AGO = "HOUR_AGO";

		private const string S_MINS_AGO = "s_MinsAgo";

		private const string MIN_AGO = "MIN_AGO";

		private const string LESS_MIN_AGO = "LESS_MIN_AGO";

		public static uint GetUnixTimestamp()
		{
			return Service.ServerAPI.ServerTime;
		}

		public static string GetFormattedAgeSinceLogin(uint lastLoginTime, Lang lang)
		{
			uint unixTimestamp = ChatTimeConversionUtils.GetUnixTimestamp();
			if (unixTimestamp < lastLoginTime || lastLoginTime == 0u)
			{
				return string.Empty;
			}
			uint num = unixTimestamp - lastLoginTime;
			int num2 = (int)(num / 86400u);
			if (num2 > 0)
			{
				if (num2 > 1)
				{
					return lang.Get("s_ActiveDaysAgo", new object[]
					{
						num2
					});
				}
				return lang.Get("ACTIVE_DAY_AGO", new object[0]);
			}
			else
			{
				int num3 = (int)(num / 3600u);
				if (num3 > 0)
				{
					if (num3 > 1)
					{
						return lang.Get("s_ActiveHoursAgo", new object[]
						{
							num3
						});
					}
					return lang.Get("ACTIVE_HOUR_AGO", new object[0]);
				}
				else
				{
					int num4 = (int)(num / 60u);
					if (num4 <= 0)
					{
						return lang.Get("ACTIVE_LESS_MIN_AGO", new object[0]);
					}
					if (num4 > 1)
					{
						return lang.Get("s_ActiveMinsAgo", new object[]
						{
							num4
						});
					}
					return lang.Get("ACTIVE_MIN_AGO", new object[0]);
				}
			}
		}

		public static string GetFormattedAge(uint timestamp, Lang lang)
		{
			uint unixTimestamp = ChatTimeConversionUtils.GetUnixTimestamp();
			if (unixTimestamp < timestamp || timestamp == 0u)
			{
				return string.Empty;
			}
			uint num = unixTimestamp - timestamp;
			int num2 = (int)(num / 86400u);
			if (num2 > 0)
			{
				if (num2 > 1)
				{
					return lang.Get("s_DaysAgo", new object[]
					{
						num2
					});
				}
				return lang.Get("DAY_AGO", new object[0]);
			}
			else
			{
				int num3 = (int)(num / 3600u);
				if (num3 > 0)
				{
					if (num3 > 1)
					{
						return lang.Get("s_HoursAgo", new object[]
						{
							num3
						});
					}
					return lang.Get("HOUR_AGO", new object[0]);
				}
				else
				{
					int num4 = (int)(num / 60u);
					if (num4 <= 0)
					{
						return lang.Get("LESS_MIN_AGO", new object[0]);
					}
					if (num4 > 1)
					{
						return lang.Get("s_MinsAgo", new object[]
						{
							num4
						});
					}
					return lang.Get("MIN_AGO", new object[0]);
				}
			}
		}
	}
}
