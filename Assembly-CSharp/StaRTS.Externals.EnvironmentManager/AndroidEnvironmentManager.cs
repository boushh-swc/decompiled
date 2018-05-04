using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Externals.EnvironmentManager
{
	public class AndroidEnvironmentManager : IEnvironmentManager
	{
		private AndroidJavaObject environmentManagerBridge;

		private AndroidJavaObject pluginActivity;

		public void Init()
		{
			this.environmentManagerBridge = new AndroidJavaClass("com.disney.glendale.environmentmanager.EnvironmentManagerBridge");
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.starts.PluginActivity");
			if (androidJavaClass != null)
			{
				this.pluginActivity = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
				if (this.environmentManagerBridge != null)
				{
					this.environmentManagerBridge.CallStatic("setContext", new object[]
					{
						this.pluginActivity
					});
				}
			}
		}

		public void SetAutoMuteOnBackgroundMute(bool value)
		{
			if (this.pluginActivity == null)
			{
				return;
			}
			this.pluginActivity.CallStatic("SetAutoMuteOnBackgroundMute", new object[]
			{
				value
			});
		}

		public string GetDeviceId()
		{
			string result = string.Empty;
			if (this.pluginActivity != null)
			{
				result = this.pluginActivity.Call<string>("GetAdvertisingId", new object[0]);
			}
			return result;
		}

		public string GetDeviceIdForEvent2()
		{
			string result;
			if (this.IsDeviceIdValid())
			{
				result = this.GetDeviceId();
			}
			else
			{
				result = PlayerPrefs.GetString("prefPlayerId", "UnknownPlayerId");
			}
			return result;
		}

		public bool IsDeviceIdValid()
		{
			string deviceId = this.GetDeviceId();
			return StringUtils.IsGuidValid(deviceId);
		}

		public string GetLocale()
		{
			string result = null;
			if (this.environmentManagerBridge != null)
			{
				result = this.environmentManagerBridge.CallStatic<string>("getLocale", new object[0]);
			}
			return result;
		}

		public string GetCurrencyCode()
		{
			string result = "NONE";
			if (this.environmentManagerBridge != null)
			{
				result = this.environmentManagerBridge.CallStatic<string>("getCurrencyCode", new object[0]);
			}
			return result;
		}

		public string GetMachine()
		{
			string result = string.Empty;
			if (this.pluginActivity != null)
			{
				result = this.pluginActivity.CallStatic<string>("GetManufacturer", new object[0]);
			}
			return result;
		}

		public string GetModel()
		{
			string result = string.Empty;
			if (this.pluginActivity != null)
			{
				result = this.pluginActivity.CallStatic<string>("GetModel", new object[0]);
			}
			return result;
		}

		public int GetAPILevel()
		{
			int result = 0;
			if (this.environmentManagerBridge != null)
			{
				result = this.environmentManagerBridge.CallStatic<int>("getAPILevel", new object[0]);
			}
			return result;
		}

		public bool IsAutoRotationEnabled()
		{
			bool result = true;
			if (this.pluginActivity != null)
			{
				result = this.pluginActivity.Call<bool>("IsAutoRotationEnabled", new object[0]);
			}
			return result;
		}

		public bool IsMusicPlaying()
		{
			return false;
		}

		public bool IsRestrictedProfile()
		{
			bool result = false;
			if (this.environmentManagerBridge != null)
			{
				result = this.environmentManagerBridge.CallStatic<bool>("getIsRestrictedProfile", new object[0]);
			}
			return result;
		}

		public bool IsTablet()
		{
			bool result = false;
			if (this.environmentManagerBridge != null)
			{
				result = this.environmentManagerBridge.CallStatic<bool>("isTablet", new object[0]);
			}
			return result;
		}

		public bool AreHeadphonesConnected()
		{
			bool result = false;
			if (this.environmentManagerBridge != null)
			{
				result = this.environmentManagerBridge.CallStatic<bool>("getAreHeadphonesConnected", new object[0]);
			}
			return result;
		}

		public void GainAudioFocus()
		{
			if (this.environmentManagerBridge != null)
			{
				this.environmentManagerBridge.CallStatic("gainAudioFocus", new object[0]);
			}
		}

		public void ShowAlert(string titleText, string messageText, string yesButtonText, string noButtonText)
		{
			if (this.environmentManagerBridge != null)
			{
				this.environmentManagerBridge.CallStatic("showAlert", new object[]
				{
					titleText,
					messageText,
					yesButtonText,
					noButtonText
				});
			}
		}

		public string GetOS()
		{
			return "ga";
		}

		public string GetPlatform()
		{
			return "and";
		}

		public string GetDeviceIdType()
		{
			if (!this.IsDeviceIdValid())
			{
				return "ply";
			}
			return "gida";
		}
	}
}
