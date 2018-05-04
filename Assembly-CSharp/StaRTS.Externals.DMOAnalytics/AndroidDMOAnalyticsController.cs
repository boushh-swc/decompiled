using System;
using UnityEngine;

namespace StaRTS.Externals.DMOAnalytics
{
	public class AndroidDMOAnalyticsController : IDMOAnalyticsManager
	{
		private static AndroidDMOAnalyticsController Instance;

		private AndroidJavaObject analyticsPlugin;

		private AndroidDMOAnalyticsController()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.starts.PluginActivity");
			AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.dismo.AnalyticsPlugin");
			this.analyticsPlugin = androidJavaClass2.CallStatic<AndroidJavaObject>("instance", new object[0]);
			this.analyticsPlugin.Call("setContext", new object[]
			{
				androidJavaObject
			});
		}

		public static AndroidDMOAnalyticsController CreateAndInitializeAndroidDMOAnalyticsController(string key, string secret)
		{
			if (AndroidDMOAnalyticsController.Instance == null)
			{
				AndroidDMOAnalyticsController.Instance = new AndroidDMOAnalyticsController();
				AndroidDMOAnalyticsController.Instance.Init(key, secret);
			}
			return AndroidDMOAnalyticsController.Instance;
		}

		private void Init(string key, string secret)
		{
			this.analyticsPlugin.Call("init", new object[]
			{
				key,
				secret
			});
		}

		public void LogEvent(string appEvent)
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogEvent", new object[]
			{
				appEvent
			});
		}

		public void LogAppStart()
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogAppStart", new object[0]);
		}

		public void LogAppEnd()
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogAppEnd", new object[0]);
		}

		public void LogAppForeground()
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogAppForeground", new object[0]);
		}

		public void LogAppBackground()
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogAppBackground", new object[0]);
		}

		public void LogEventWithContext(string eventName, string parameters)
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogEventWithContext", new object[]
			{
				eventName,
				parameters
			});
		}

		public void FlushAnalyticsQueue()
		{
			this.analyticsPlugin.Call("dmoAnalyticsflushAnalyticsQueue", new object[0]);
		}

		public void LogGameAction(string parameters)
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogGameAction", new object[]
			{
				parameters
			});
		}

		public void LogMoneyAction(string parameters)
		{
			this.analyticsPlugin.Call("dmoAnalyticsLogMoneyAction", new object[]
			{
				parameters
			});
		}

		public void SetDebugLogging(bool isEnable)
		{
			this.analyticsPlugin.Call("dmoAnalyticsSetDebugLogging", new object[]
			{
				isEnable
			});
		}

		public void SetCanUseNetwork(bool isEnable)
		{
			this.analyticsPlugin.Call("dmoAnalyticsSetCanUseNetwork", new object[]
			{
				isEnable
			});
		}
	}
}
