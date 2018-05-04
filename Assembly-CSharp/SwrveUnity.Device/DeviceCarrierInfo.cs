using System;
using UnityEngine;

namespace SwrveUnity.Device
{
	public class DeviceCarrierInfo : ICarrierInfo
	{
		private AndroidJavaObject androidTelephonyManager;

		public DeviceCarrierInfo()
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Context"))
				{
					using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
					{
						using (AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity"))
						{
							string static2 = androidJavaClass.GetStatic<string>("TELEPHONY_SERVICE");
							this.androidTelephonyManager = @static.Call<AndroidJavaObject>("getSystemService", new object[]
							{
								static2
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't get access to TelephonyManager: " + ex.ToString());
			}
		}

		private string AndroidGetTelephonyManagerAttribute(string method)
		{
			if (this.androidTelephonyManager != null)
			{
				try
				{
					return this.androidTelephonyManager.Call<string>(method, new object[0]);
				}
				catch (Exception ex)
				{
					SwrveLog.LogWarning("Problem accessing the TelephonyManager - " + method + ": " + ex.ToString());
				}
			}
			return null;
		}

		public string GetName()
		{
			return this.AndroidGetTelephonyManagerAttribute("getSimOperatorName");
		}

		public string GetIsoCountryCode()
		{
			return this.AndroidGetTelephonyManagerAttribute("getSimCountryIso");
		}

		public string GetCarrierCode()
		{
			return this.AndroidGetTelephonyManagerAttribute("getSimOperator");
		}
	}
}
