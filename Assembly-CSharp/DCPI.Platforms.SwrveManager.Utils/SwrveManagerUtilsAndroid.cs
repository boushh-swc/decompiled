using System;
using UnityEngine;

namespace DCPI.Platforms.SwrveManager.Utils
{
	public static class SwrveManagerUtilsAndroid
	{
		private static AndroidJavaObject _plugin;

		private static AndroidJavaObject playerActivityContext;

		static SwrveManagerUtilsAndroid()
		{
			if (Application.platform == RuntimePlatform.Android && (SwrveManagerUtilsAndroid.playerActivityContext == null || SwrveManagerUtilsAndroid._plugin == null))
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.starts.PluginActivity"))
				{
					SwrveManagerUtilsAndroid.playerActivityContext = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
					if (SwrveManagerUtilsAndroid.playerActivityContext != null)
					{
						Debug.Log("SwrveManagerUtilsAndroid:SwrveManagerUtilsAndroid() created playerActivityContext");
					}
					else
					{
						Debug.LogError("SwrveManagerUtilsAndroid:SwrveManagerUtilsAndroid() failed to create playerActivityContext!");
					}
				}
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.disney.dcpi.swrveutils.SwrveUtilsPlugin"))
				{
					if (androidJavaClass2 != null)
					{
						androidJavaClass2.CallStatic("setContext", new object[]
						{
							SwrveManagerUtilsAndroid.playerActivityContext
						});
						SwrveManagerUtilsAndroid._plugin = androidJavaClass2.CallStatic<AndroidJavaObject>("instance", new object[0]);
						if (SwrveManagerUtilsAndroid._plugin != null)
						{
							Debug.Log("### Successfully set up the SwrveManagerUtilsAndroid instance");
						}
						else
						{
							Debug.LogError("#### still not able to get SwrveUtilsPlugin instance for some reason.");
						}
					}
				}
			}
		}

		public static string GetIsJailBroken()
		{
			if (SwrveManagerUtilsAndroid._plugin != null)
			{
				return SwrveManagerUtilsAndroid._plugin.Call<int>("isJailBroken", new object[0]).ToString();
			}
			Debug.LogError("SwrveManagerUtilsAndroid::GetIsJailBroken - no plugin!!");
			return "-1";
		}

		public static int GetIsLat()
		{
			if (SwrveManagerUtilsAndroid._plugin != null)
			{
				return SwrveManagerUtilsAndroid._plugin.Call<int>("isLat", new object[0]);
			}
			Debug.LogError("SwrveManagerUtilsAndroid::GetIsLat - no plugin!!");
			return 2;
		}

		public static string GetGIDA()
		{
			if (SwrveManagerUtilsAndroid._plugin != null)
			{
				return SwrveManagerUtilsAndroid._plugin.Call<string>("gida", new object[0]);
			}
			Debug.LogError("SwrveManagerUtilsAndroid::GetGIDA - no plugin!!");
			return string.Empty;
		}

		public static bool RegisterGCMDevice(string gameObject, string senderId, string appTitle, string iconId, string materialIconId, string largeIconId, int accentColor, string appGroupName)
		{
			if (SwrveManagerUtilsAndroid._plugin != null)
			{
				return SwrveManagerUtilsAndroid._plugin.Call<bool>("setGcmDeviceRegistration", new object[]
				{
					gameObject,
					senderId,
					appTitle,
					iconId,
					materialIconId,
					largeIconId,
					accentColor,
					appGroupName
				});
			}
			Debug.LogError("SwrveManagerUtilsAndroid::RegisterGCMDevice - no plugin!!");
			return false;
		}
	}
}
