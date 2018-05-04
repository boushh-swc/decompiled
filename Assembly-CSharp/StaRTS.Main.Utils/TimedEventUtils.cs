using StaRTS.Externals.Manimal;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Globalization;

namespace StaRTS.Main.Utils
{
	public static class TimedEventUtils
	{
		public static bool IsTimedEventLive(ITimedEventVO eventVO)
		{
			TimedEventState state = TimedEventUtils.GetState(eventVO);
			return state == TimedEventState.Live;
		}

		public static bool IsTimedEventLive(ITimedEventVO eventVO, uint timeToCheck)
		{
			TimedEventState state = TimedEventUtils.GetState(eventVO, timeToCheck);
			return state == TimedEventState.Live;
		}

		public static bool IsTimedEventLiveOrClosing(ITimedEventVO eventVO)
		{
			TimedEventState state = TimedEventUtils.GetState(eventVO);
			return state == TimedEventState.Live || state == TimedEventState.Closing;
		}

		public static bool IsTimedEventActive(ITimedEventVO eventVO)
		{
			TimedEventState state = TimedEventUtils.GetState(eventVO);
			return state != TimedEventState.Invalid && state != TimedEventState.Hidden;
		}

		public static bool IsTimedEventClosing(ITimedEventVO eventVO, uint timeToCheck)
		{
			TimedEventState state = TimedEventUtils.GetState(eventVO, timeToCheck);
			return state == TimedEventState.Closing;
		}

		public static int GetTimestamp(string timedEventUid, string dateString)
		{
			int result = 0;
			try
			{
				DateTime date = DateTime.ParseExact(dateString, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				result = DateUtils.GetSecondsFromEpoch(date);
			}
			catch
			{
				Service.Logger.ErrorFormat("Failed to parse date string {0} for uid {1}. Must be valid and in format {2}", new object[]
				{
					dateString,
					timedEventUid,
					"HH:mm,dd-MM-yyyy"
				});
			}
			return result;
		}

		public static TimedEventState GetState(ITimedEventVO vo)
		{
			int nowTimeWithEventOffset = TimedEventUtils.GetNowTimeWithEventOffset(vo);
			return TimedEventUtils.GetState(vo, (uint)nowTimeWithEventOffset);
		}

		public static TimedEventState GetState(ITimedEventVO vo, uint nowSeconds)
		{
			int upcomingDurationSeconds = vo.GetUpcomingDurationSeconds();
			int closingDurationSeconds = vo.GetClosingDurationSeconds();
			if ((ulong)nowSeconds < (ulong)((long)(vo.StartTimestamp - upcomingDurationSeconds)) || (ulong)nowSeconds > (ulong)((long)(vo.EndTimestamp + closingDurationSeconds)))
			{
				return TimedEventState.Hidden;
			}
			if ((ulong)nowSeconds < (ulong)((long)vo.StartTimestamp))
			{
				return TimedEventState.Upcoming;
			}
			if ((ulong)nowSeconds < (ulong)((long)vo.EndTimestamp))
			{
				return TimedEventState.Live;
			}
			if ((ulong)nowSeconds < (ulong)((long)(vo.EndTimestamp + closingDurationSeconds)))
			{
				return TimedEventState.Closing;
			}
			return TimedEventState.Hidden;
		}

		public static int GetSecondsRemaining(ITimedEventVO vo)
		{
			int nowTimeWithEventOffset = TimedEventUtils.GetNowTimeWithEventOffset(vo);
			switch (TimedEventUtils.GetState(vo))
			{
			case TimedEventState.Upcoming:
				return vo.StartTimestamp - nowTimeWithEventOffset;
			case TimedEventState.Live:
				return vo.EndTimestamp - nowTimeWithEventOffset;
			case TimedEventState.Closing:
				return vo.EndTimestamp - nowTimeWithEventOffset;
			default:
				return 0;
			}
		}

		public static int GetSecondsRemainingUntilClosing(ITimedEventVO vo)
		{
			int nowTimeWithEventOffset = TimedEventUtils.GetNowTimeWithEventOffset(vo);
			return vo.EndTimestamp - nowTimeWithEventOffset;
		}

		public static int GetStoreSecondsRemaining(ITimedEventVO vo)
		{
			return TimedEventUtils.GetSecondsRemaining(vo) + vo.GetClosingDurationSeconds();
		}

		private static int GetNowTimeWithEventOffset(ITimedEventVO vo)
		{
			return (int)(ServerTime.Time + (uint)Service.CurrentPlayer.CampaignProgress.GetOffsetSeconds(vo));
		}
	}
}
