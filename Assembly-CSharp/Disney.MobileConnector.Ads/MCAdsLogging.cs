using System;
using UnityEngine;

namespace Disney.MobileConnector.Ads
{
	public static class MCAdsLogging
	{
		public enum LogLevel
		{
			None = 0,
			Error = 1,
			Warning = 2,
			Verbose = 3
		}

		private static MCAdsLogging.LogLevel _logLevel = MCAdsLogging.LogLevel.Verbose;

		public static void SetLogLevel(MCAdsLogging.LogLevel level)
		{
			MCAdsLogging._logLevel = level;
			MCAdsBinding.SetLogLevel(level);
			MCAdsLogging.Log("log level set to: " + level.ToString(), MCAdsLogging.LogLevel.Verbose);
		}

		public static MCAdsLogging.LogLevel GetLogLevel()
		{
			return MCAdsLogging._logLevel;
		}

		public static void Log(string log, MCAdsLogging.LogLevel level)
		{
			if (level > MCAdsLogging._logLevel || MCAdsLogging._logLevel == MCAdsLogging.LogLevel.None)
			{
				return;
			}
			if (level != MCAdsLogging.LogLevel.Error)
			{
				if (level != MCAdsLogging.LogLevel.Warning)
				{
					Debug.Log(log);
				}
				else
				{
					Debug.LogWarning(log);
				}
			}
			else
			{
				Debug.LogError(log);
			}
		}
	}
}
