using System;
using UnityEngine;

public class CrittercismConfig
{
	private static readonly string CRITTERCISM_CONFIG_CLASS = "com.crittercism.app.CrittercismConfig";

	private AndroidJavaObject mCrittercismConfig;

	public CrittercismConfig()
	{
		this.mCrittercismConfig = new AndroidJavaObject(CrittercismConfig.CRITTERCISM_CONFIG_CLASS, new object[0]);
	}

	public AndroidJavaObject GetAndroidConfig()
	{
		return this.mCrittercismConfig;
	}

	public string GetCustomVersionName()
	{
		return this.CallConfigMethod<string>("getCustomVersionName", new object[0]);
	}

	public void SetCustomVersionName(string customVersionName)
	{
		this.CallConfigMethod("setCustomVersionName", new object[]
		{
			customVersionName
		});
	}

	public bool IsLogcatReportingEnabled()
	{
		return this.CallConfigMethod<bool>("isLogcatReportingEnabled", new object[0]);
	}

	public void SetLogcatReportingEnabled(bool shouldCollectLogcat)
	{
		this.CallConfigMethod("setLogcatReportingEnabled", new object[]
		{
			shouldCollectLogcat
		});
	}

	public bool IsServiceMonitoringEnabled()
	{
		return this.CallConfigMethod<bool>("isServiceMonitoringEnabled", new object[0]);
	}

	public void SetServiceMonitoringEnabled(bool isServiceMonitoringEnabled)
	{
		this.CallConfigMethod("setServiceMonitoringEnabled", new object[]
		{
			isServiceMonitoringEnabled
		});
	}

	private void CallConfigMethod(string methodName, params object[] args)
	{
		this.mCrittercismConfig.Call(methodName, args);
	}

	private RetType CallConfigMethod<RetType>(string methodName, params object[] args)
	{
		return this.mCrittercismConfig.Call<RetType>(methodName, args);
	}
}
