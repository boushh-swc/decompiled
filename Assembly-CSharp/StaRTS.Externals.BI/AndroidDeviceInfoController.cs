using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Externals.BI
{
	public class AndroidDeviceInfoController : IDeviceInfoController
	{
		private AndroidJavaObject pluginActivity;

		public AndroidDeviceInfoController()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.starts.PluginActivity");
			this.pluginActivity = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		}

		public string GetDeviceId()
		{
			return this.GetDeviceAdvertisingId();
		}

		public string GetAndroidID()
		{
			return this.pluginActivity.Call<string>("GetDeviceId", new object[0]);
		}

		public string GetDeviceAdvertisingId()
		{
			return this.pluginActivity.Call<string>("GetAdvertisingId", new object[0]);
		}

		public string GetIMEI()
		{
			return this.pluginActivity.Call<string>("GetIMEI", new object[0]);
		}

		public void AddDeviceSpecificInfo(BILog log)
		{
			string deviceAdvertisingId = this.GetDeviceAdvertisingId();
			log.AddParam("google_advertising_id", EncryptionUtil.EncryptString(deviceAdvertisingId));
		}
	}
}
