using StaRTS.Utils.Core;
using System;
using System.Globalization;
using UnityEngine;

namespace StaRTS.Utils
{
	public static class DateUtils
	{
		public const int SECONDS_IN_MINUTE = 60;

		public const int SECONDS_IN_HOUR = 3600;

		public const int SECONDS_IN_DAY = 86400;

		private const double SECOND_TO_MILLISECOND = 1000.0;

		private const double MINUTE_TO_MILLISECOND = 60000.0;

		private const double HOUR_TO_MILLISECOND = 3600000.0;

		private static readonly DateTime UnixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static int GetSecondsFromString(string dateString, int defaultValue)
		{
			if (string.IsNullOrEmpty(dateString))
			{
				Service.Logger.Warn("Missing Date Info!");
				return defaultValue;
			}
			DateTime date = DateTime.ParseExact(dateString, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			return DateUtils.GetSecondsFromEpoch(date);
		}

		public static TimeSpan GetTimeSpanSinceStartOfDate(DateTime date)
		{
			return TimeSpan.FromMilliseconds((double)date.Millisecond + (double)date.Second * 1000.0 + (double)date.Minute * 60000.0 + (double)date.Hour * 3600000.0);
		}

		public static DateTime GetDefaultDate()
		{
			return default(DateTime);
		}

		public static bool IsDefaultDate(DateTime date)
		{
			return DateUtils.GetDefaultDate().Equals(date);
		}

		public static DateTime DateFromMillis(long millis)
		{
			return new DateTime(DateUtils.UnixStart.Ticks + millis * 10000L);
		}

		public static DateTime DateFromSeconds(uint seconds)
		{
			return DateUtils.DateFromSeconds((int)seconds);
		}

		public static DateTime DateFromSeconds(int seconds)
		{
			return DateUtils.UnixStart.AddSeconds((double)seconds);
		}

		public static float GetRealTimeSinceStartUpInMilliseconds()
		{
			return Mathf.Round(UnityUtils.GetRealTimeSinceStartUp() * 1000f);
		}

		public static int GetMillisFromEpoch(DateTime date)
		{
			return (int)(date - DateUtils.UnixStart).TotalMilliseconds;
		}

		public static int GetSecondsFromEpoch(DateTime date)
		{
			return (int)(date - DateUtils.UnixStart).TotalSeconds;
		}

		public static int GetSecondsFromNow(DateTime date)
		{
			return (int)(date - DateTime.UtcNow).TotalSeconds;
		}

		public static double GetNowSecondsPrecise()
		{
			return (DateTime.UtcNow - DateUtils.UnixStart).TotalSeconds;
		}

		public static uint GetNowSeconds()
		{
			return (uint)DateUtils.GetSecondsFromEpoch(DateTime.UtcNow);
		}
	}
}
