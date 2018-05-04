using System;
using UnityEngine;

public static class SwrveLog
{
	public enum LogLevel
	{
		Verbose = 0,
		Info = 1,
		Warning = 2,
		Error = 3,
		Disabled = 4
	}

	public delegate void SwrveLogEventHandler(SwrveLog.LogLevel level, object message, string tag);

	public static SwrveLog.LogLevel Level;

	public static event SwrveLog.SwrveLogEventHandler OnLog;

	public static void Log(object message)
	{
		SwrveLog.Log(message, "activity");
	}

	public static void LogInfo(object message)
	{
		SwrveLog.LogInfo(message, "activity");
	}

	public static void LogWarning(object message)
	{
		SwrveLog.LogWarning(message, "activity");
	}

	public static void LogError(object message)
	{
		SwrveLog.LogError(message, "activity");
	}

	public static void Log(object message, string tag)
	{
		if (SwrveLog.Level == SwrveLog.LogLevel.Verbose)
		{
			Debug.Log(message);
			if (SwrveLog.OnLog != null)
			{
				SwrveLog.OnLog(SwrveLog.LogLevel.Verbose, message, tag);
			}
		}
	}

	public static void LogInfo(object message, string tag)
	{
		if (SwrveLog.Level == SwrveLog.LogLevel.Verbose || SwrveLog.Level == SwrveLog.LogLevel.Info)
		{
			Debug.Log(message);
			if (SwrveLog.OnLog != null)
			{
				SwrveLog.OnLog(SwrveLog.LogLevel.Info, message, tag);
			}
		}
	}

	public static void LogWarning(object message, string tag)
	{
		if (SwrveLog.Level == SwrveLog.LogLevel.Verbose || SwrveLog.Level == SwrveLog.LogLevel.Info || SwrveLog.Level == SwrveLog.LogLevel.Warning)
		{
			Debug.LogWarning(message);
			if (SwrveLog.OnLog != null)
			{
				SwrveLog.OnLog(SwrveLog.LogLevel.Warning, message, tag);
			}
		}
	}

	public static void LogError(object message, string tag)
	{
		if (SwrveLog.Level == SwrveLog.LogLevel.Verbose || SwrveLog.Level == SwrveLog.LogLevel.Info || SwrveLog.Level == SwrveLog.LogLevel.Warning || SwrveLog.Level == SwrveLog.LogLevel.Error)
		{
			Debug.LogError(message);
			if (SwrveLog.OnLog != null)
			{
				SwrveLog.OnLog(SwrveLog.LogLevel.Error, message, tag);
			}
		}
	}
}
