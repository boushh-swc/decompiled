using System;

namespace StaRTS.Externals.DMOAnalytics
{
	public interface IDMOAnalyticsManager
	{
		void LogEvent(string appEvent);

		void LogAppStart();

		void LogAppEnd();

		void LogAppForeground();

		void LogAppBackground();

		void LogEventWithContext(string eventName, string parameters);

		void FlushAnalyticsQueue();

		void SetDebugLogging(bool isEnable);

		void SetCanUseNetwork(bool isEnable);
	}
}
