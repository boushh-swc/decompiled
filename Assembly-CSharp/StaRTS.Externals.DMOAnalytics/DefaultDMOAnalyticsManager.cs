using System;

namespace StaRTS.Externals.DMOAnalytics
{
	public class DefaultDMOAnalyticsManager : IDMOAnalyticsManager
	{
		public void LogEvent(string appEvent)
		{
		}

		public void LogAppStart()
		{
		}

		public void LogAppEnd()
		{
		}

		public void LogAppForeground()
		{
		}

		public void LogAppBackground()
		{
		}

		public void LogEventWithContext(string eventName, string parameters)
		{
		}

		public void FlushAnalyticsQueue()
		{
		}

		public void LogGameAction(string parameters)
		{
		}

		public void LogMoneyAction(string parameters)
		{
		}

		public void SetDebugLogging(bool isEnable)
		{
		}

		public void SetCanUseNetwork(bool isEnable)
		{
		}
	}
}
