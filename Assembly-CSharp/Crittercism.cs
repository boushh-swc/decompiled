using System;
using System.Net;
using UnityEngine;

public class Crittercism : MonoBehaviour
{
	public string CrittercismiOSAppID = "YOUR IOS APP ID GOES HERE";

	public string CrittercismAndroidAppID = "YOUR ANDROID APP ID GOES HERE";

	public string CrittercismAmazonAppID = "YOUR AMAZON APP ID GOES HERE";

	public string CrittercismiOSNonProdAppID = "YOUR IOS NON PROD APP ID GOES HERE";

	public string CrittercismAndroidNonProdAppID = "YOUR ANDROID NON PROD APP ID GOES HERE";

	public string CrittercismAmazonNonProdAppID = "YOUR AMAZON NON PROD APP ID GOES HERE";

	private void Awake()
	{
		CrittercismAndroid.Init(this.CrittercismAndroidAppID);
	}

	public static void LogHandledException(Exception e)
	{
		CrittercismAndroid.LogHandledException(e);
	}

	public static bool GetOptOut()
	{
		return CrittercismAndroid.GetOptOut();
	}

	public static void SetOptOut(bool isOptedOut)
	{
		CrittercismAndroid.SetOptOut(isOptedOut);
	}

	public static void SetUsername(string username)
	{
		CrittercismAndroid.SetUsername(username);
	}

	public static void SetValue(string key, string value)
	{
		CrittercismAndroid.SetMetadata(new string[]
		{
			key
		}, new string[]
		{
			value
		});
	}

	public static void SetMetadata(string[] keys, string[] values)
	{
		CrittercismAndroid.SetMetadata(keys, values);
	}

	public static void LeaveBreadcrumb(string breadcrumb)
	{
		CrittercismAndroid.LeaveBreadcrumb(breadcrumb);
	}

	public static void LogNetworkRequest(string method, string uriString, double latencyInSeconds, int bytesRead, int bytesSent, HttpStatusCode responseCode, WebExceptionStatus exceptionStatus)
	{
		CrittercismAndroid.LogNetworkRequest(method, uriString, (long)latencyInSeconds * 1000L, (long)bytesRead, (long)bytesSent, responseCode, exceptionStatus);
	}

	public static void LogNetworkRequest(string method, string uriString, long latencyInMilliseconds, int bytesRead, int bytesSent, HttpStatusCode responseCode, WebExceptionStatus exceptionStatus)
	{
		CrittercismAndroid.LogNetworkRequest(method, uriString, latencyInMilliseconds, (long)bytesRead, (long)bytesSent, responseCode, exceptionStatus);
	}

	public static bool DidCrashOnLastLoad()
	{
		return CrittercismAndroid.DidCrashOnLastLoad();
	}

	public static void BeginUserflow(string name)
	{
		CrittercismAndroid.BeginUserflow(name);
	}

	public static void BeginUserflow(string name, int value)
	{
		CrittercismAndroid.BeginUserflow(name);
		CrittercismAndroid.SetUserflowValue(name, value);
	}

	public static void CancelUserflow(string name)
	{
		CrittercismAndroid.CancelUserflow(name);
	}

	public static void EndUserflow(string name)
	{
		CrittercismAndroid.EndUserflow(name);
	}

	public static void FailUserflow(string name)
	{
		CrittercismAndroid.FailUserflow(name);
	}

	public static void SetUserflowValue(string name, int value)
	{
		CrittercismAndroid.SetUserflowValue(name, value);
	}

	public static int GetUserflowValue(string name)
	{
		return CrittercismAndroid.GetUserflowValue(name);
	}

	public static void SetLogUnhandledExceptionAsCrash(bool value)
	{
		CrittercismAndroid.SetLogUnhandledExceptionAsCrash(value);
	}

	public static bool GetLogUnhandledExceptionAsCrash()
	{
		return CrittercismAndroid.GetLogUnhandledExceptionAsCrash();
	}
}
