using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CrittercismAndroid
{
	private static bool isInitialized;

	private static readonly string CRITTERCISM_CLASS = "com.crittercism.app.Crittercism";

	private static AndroidJavaClass mCrittercismsPlugin;

	private static volatile bool logUnhandledExceptionAsCrash;

	[CompilerGenerated]
	private static Application.LogCallback <>f__mg$cache0;

	public static void Init(string appID)
	{
		CrittercismAndroid.Init(appID, new CrittercismConfig());
	}

	public static void Init(string appID, CrittercismConfig config)
	{
		if (CrittercismAndroid.isInitialized)
		{
			Debug.Log("CrittercismAndroid is already initialized.");
			return;
		}
		Debug.Log("Initializing Crittercism with app id " + appID);
		CrittercismAndroid.mCrittercismsPlugin = new AndroidJavaClass(CrittercismAndroid.CRITTERCISM_CLASS);
		if (CrittercismAndroid.mCrittercismsPlugin == null)
		{
			Debug.Log("CrittercismAndroid failed to initialize.  Unable to find class " + CrittercismAndroid.CRITTERCISM_CLASS);
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				CrittercismAndroid.PluginCallStatic("initialize", new object[]
				{
					@static,
					appID,
					config.GetAndroidConfig()
				});
			}
		}
		if (CrittercismAndroid.<>f__mg$cache0 == null)
		{
			CrittercismAndroid.<>f__mg$cache0 = new Application.LogCallback(CrittercismAndroid.OnLogMessageReceived);
		}
		Application.logMessageReceived += CrittercismAndroid.<>f__mg$cache0;
		CrittercismAndroid.isInitialized = true;
	}

	private static string StackTrace(Exception e)
	{
		string text = e.StackTrace;
		List<Exception> list = new List<Exception>();
		list.Add(e);
		if (text != null)
		{
			text = string.Concat(new string[]
			{
				e.GetType().FullName,
				" : ",
				e.Message,
				"\r\n",
				text
			});
			Exception innerException = e.InnerException;
			while (innerException != null && list.IndexOf(innerException) < 0)
			{
				list.Add(innerException);
				text = string.Concat(new string[]
				{
					innerException.GetType().FullName,
					" : ",
					innerException.Message,
					"\r\n",
					innerException.StackTrace,
					"\r\n",
					text
				});
				innerException = innerException.InnerException;
			}
		}
		else
		{
			text = string.Empty;
		}
		return text;
	}

	public static void LogHandledException(Exception e)
	{
		string fullName = e.GetType().FullName;
		string message = e.Message;
		string text = CrittercismAndroid.StackTrace(e);
		CrittercismAndroid.PluginCallStatic("_logHandledException", new object[]
		{
			fullName,
			message,
			text
		});
	}

	private static void LogUnhandledException(Exception e)
	{
		string fullName = e.GetType().FullName;
		string message = e.Message;
		string text = CrittercismAndroid.StackTrace(e);
		CrittercismAndroid.PluginCallStatic((!CrittercismAndroid.logUnhandledExceptionAsCrash) ? "_logHandledException" : "_logCrashException", new object[]
		{
			fullName,
			message,
			text
		});
	}

	public static void LogNetworkRequest(string method, string uriString, long latency, long bytesRead, long bytesSent, HttpStatusCode responseCode, WebExceptionStatus exceptionStatus)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("logNetworkRequest", new object[]
		{
			method,
			uriString,
			latency,
			bytesRead,
			bytesSent,
			(int)responseCode,
			(int)exceptionStatus
		});
	}

	public static bool GetOptOut()
	{
		return CrittercismAndroid.isInitialized && CrittercismAndroid.PluginCallStatic<bool>("getOptOutStatus", new object[0]);
	}

	public static void SetOptOut(bool optOutStatus)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("setOptOutStatus", new object[]
		{
			optOutStatus
		});
	}

	public static bool DidCrashOnLastLoad()
	{
		return CrittercismAndroid.isInitialized && CrittercismAndroid.PluginCallStatic<bool>("didCrashOnLastLoad", new object[0]);
	}

	public static void SetUsername(string username)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("setUsername", new object[]
		{
			username
		});
	}

	public static void SetMetadata(string[] keys, string[] values)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		if (keys.Length != values.Length)
		{
			Debug.Log("Crittercism.SetMetadata given arrays of different lengths");
			return;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			CrittercismAndroid.SetValue(keys[i], values[i]);
		}
	}

	public static void SetValue(string key, string value)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("org.json.JSONObject", new object[0]))
		{
			androidJavaObject.Call<AndroidJavaObject>("put", new object[]
			{
				key,
				value
			});
			CrittercismAndroid.PluginCallStatic("setMetadata", new object[]
			{
				androidJavaObject
			});
		}
	}

	public static void LeaveBreadcrumb(string breadcrumb)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("leaveBreadcrumb", new object[]
		{
			breadcrumb
		});
	}

	public static void BeginUserflow(string userflowName)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("beginTransaction", new object[]
		{
			userflowName
		});
	}

	[Obsolete("BeginTransaction is deprecated, please use BeginUserflow instead.")]
	public static void BeginTransaction(string userflowName)
	{
		CrittercismAndroid.BeginUserflow(userflowName);
	}

	public static void CancelUserflow(string userflowName)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("cancelTransaction", new object[]
		{
			userflowName
		});
	}

	[Obsolete("CancelTransaction is deprecated, please use CancelUserflow instead.")]
	public static void CancelTransaction(string userflowName)
	{
		CrittercismAndroid.CancelUserflow(userflowName);
	}

	public static void EndUserflow(string userflowName)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("endTransaction", new object[]
		{
			userflowName
		});
	}

	[Obsolete("EndTransaction is deprecated, please use EndUserflow instead.")]
	public static void EndTransaction(string userflowName)
	{
		CrittercismAndroid.EndUserflow(userflowName);
	}

	public static void FailUserflow(string userflowName)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("failTransaction", new object[]
		{
			userflowName
		});
	}

	[Obsolete("FailTransaction is deprecated, please use FailUserflow instead.")]
	public static void FailTransaction(string userflowName)
	{
		CrittercismAndroid.FailUserflow(userflowName);
	}

	public static void SetUserflowValue(string userflowName, int value)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		CrittercismAndroid.PluginCallStatic("setTransactionValue", new object[]
		{
			userflowName,
			value
		});
	}

	[Obsolete("SetTransactionValue is deprecated, please use SetUserflowValue instead.")]
	public static void SetTransactionValue(string userflowName, int value)
	{
		CrittercismAndroid.SetUserflowValue(userflowName, value);
	}

	public static int GetUserflowValue(string userflowName)
	{
		if (!CrittercismAndroid.isInitialized)
		{
			return -1;
		}
		return CrittercismAndroid.PluginCallStatic<int>("getTransactionValue", new object[]
		{
			userflowName
		});
	}

	[Obsolete("GetTransactionValue is deprecated, please use GetUserflowValue instead.")]
	public static int GetTransactionValue(string userflowName)
	{
		return CrittercismAndroid.GetUserflowValue(userflowName);
	}

	private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
	{
		if (!CrittercismAndroid.isInitialized || args == null || args.ExceptionObject == null)
		{
			return;
		}
		Exception e = args.ExceptionObject as Exception;
		CrittercismAndroid.LogUnhandledException(e);
	}

	public static void SetLogUnhandledExceptionAsCrash(bool value)
	{
		CrittercismAndroid.logUnhandledExceptionAsCrash = value;
	}

	public static bool GetLogUnhandledExceptionAsCrash()
	{
		return CrittercismAndroid.logUnhandledExceptionAsCrash;
	}

	private static void OnLogMessageReceived(string name, string stack, LogType type)
	{
		if (type != LogType.Exception)
		{
			return;
		}
		if (!CrittercismAndroid.isInitialized)
		{
			return;
		}
		if (CrittercismAndroid.logUnhandledExceptionAsCrash)
		{
			CrittercismAndroid.PluginCallStatic("_logCrashException", new object[]
			{
				name,
				name,
				stack
			});
		}
		else
		{
			stack = new Regex("\r\n").Replace(stack, "\n\tat");
			CrittercismAndroid.PluginCallStatic("_logHandledException", new object[]
			{
				name,
				name,
				stack
			});
		}
	}

	private static void PluginCallStatic(string methodName, params object[] args)
	{
		CrittercismAndroid.mCrittercismsPlugin.CallStatic(methodName, args);
	}

	private static RetType PluginCallStatic<RetType>(string methodName, params object[] args)
	{
		return CrittercismAndroid.mCrittercismsPlugin.CallStatic<RetType>(methodName, args);
	}
}
