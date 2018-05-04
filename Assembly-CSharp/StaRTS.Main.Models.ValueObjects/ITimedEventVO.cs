using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface ITimedEventVO : IValueObject
	{
		int StartTimestamp
		{
			get;
			set;
		}

		int EndTimestamp
		{
			get;
			set;
		}

		bool UseTimeZoneOffset
		{
			get;
		}

		int GetUpcomingDurationSeconds();

		int GetClosingDurationSeconds();
	}
}
