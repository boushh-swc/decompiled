using StaRTS.Utils.Core;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace StaRTS.Externals.EnvironmentManager
{
	public class EnvironmentController
	{
		public const string UNKNOWN_PLAYER_ID = "UnknownPlayerId";

		private IEnvironmentManager environmentManager;

		public EnvironmentController()
		{
			Service.EnvironmentController = this;
			this.environmentManager = new AndroidEnvironmentManager();
			this.environmentManager.Init();
		}

		public string GetLocale()
		{
			string text = this.environmentManager.GetLocale();
			if (string.IsNullOrEmpty(text))
			{
				text = "en_US";
			}
			return this.GetBIAppropriateLocale(text);
		}

		public string GetRawLocale()
		{
			return this.environmentManager.GetLocale();
		}

		public string GetCurrencyCode()
		{
			return this.environmentManager.GetCurrencyCode();
		}

		public bool IsTablet()
		{
			return this.environmentManager.IsTablet();
		}

		public string GetDeviceCountryCode()
		{
			string locale = this.environmentManager.GetLocale();
			string[] array = locale.Split(new char[]
			{
				'_'
			});
			string result = "US";
			if (array.Length > 1)
			{
				result = array[1];
			}
			return result;
		}

		private string GetBIAppropriateLocale(string deviceLocale)
		{
			string text = deviceLocale.Substring(0, 2);
			string text2 = text;
			string result;
			switch (text2)
			{
			case "de":
				result = "de_DE";
				return result;
			case "en":
				result = "en_US";
				return result;
			case "es":
				result = "es_LA";
				return result;
			case "fr":
				result = "fr_FR";
				return result;
			case "it":
				result = "it_IT";
				return result;
			case "ja":
				result = "ja_JP";
				return result;
			case "ko":
				result = "ko_KR";
				return result;
			case "pt":
				result = "pt_BR";
				return result;
			case "ru":
				result = "ru_RU";
				return result;
			case "zh":
				if (deviceLocale.Equals("zh_CN") || deviceLocale.Equals("zh_SG") || deviceLocale.Contains("Hans"))
				{
					result = "zh_CN";
				}
				else if (deviceLocale.Contains("Hant") || deviceLocale.Contains("TW"))
				{
					result = "zh_TW";
				}
				else
				{
					result = "en_US";
				}
				return result;
			}
			result = "en_US";
			return result;
		}

		public void SetAutoMuteOnBackgroundMute(bool value)
		{
			this.environmentManager.SetAutoMuteOnBackgroundMute(value);
		}

		public string GetDeviceId()
		{
			return this.environmentManager.GetDeviceId();
		}

		public string GetDeviceIDForEvent2()
		{
			return this.environmentManager.GetDeviceIdForEvent2();
		}

		public string GetDeviceIdType()
		{
			return this.environmentManager.GetDeviceIdType();
		}

		public string GetMachine()
		{
			return this.environmentManager.GetMachine();
		}

		public string GetModel()
		{
			return this.environmentManager.GetModel();
		}

		public int GetAPILevel()
		{
			return this.environmentManager.GetAPILevel();
		}

		public string GetOSVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			return Regex.Replace(operatingSystem, "[^0-9.]", string.Empty);
		}

		public string GetOS()
		{
			return this.environmentManager.GetOS();
		}

		public string GetPlatform()
		{
			return this.environmentManager.GetPlatform();
		}

		public bool IsMusicPlaying()
		{
			return this.environmentManager.IsMusicPlaying();
		}

		public bool AreHeadphonesConnected()
		{
			return this.environmentManager.AreHeadphonesConnected();
		}

		public bool IsRestrictedProfile()
		{
			return this.environmentManager.IsRestrictedProfile();
		}

		public double GetTimezoneOffset()
		{
			TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
			return currentTimeZone.GetUtcOffset(DateTime.Now).TotalHours;
		}

		public int GetTimezoneOffsetSeconds()
		{
			return (int)(this.GetTimezoneOffset() * 3600.0);
		}

		public void GainAudioFocus()
		{
			this.environmentManager.GainAudioFocus();
		}

		public void SetupAutoRotation()
		{
			if (!this.environmentManager.IsAutoRotationEnabled())
			{
				if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
				{
					Screen.orientation = ScreenOrientation.LandscapeRight;
				}
				else
				{
					Screen.orientation = ScreenOrientation.LandscapeLeft;
				}
			}
			else
			{
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
		}

		public void ShowAlert(string titleText, string messageText, string yesButtonText)
		{
			this.ShowAlert(titleText, messageText, yesButtonText, string.Empty);
		}

		public void ShowAlert(string titleText, string messageText, string yesButtonText, string noButtonText)
		{
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = false;
			}
			this.environmentManager.ShowAlert(titleText, messageText, yesButtonText, noButtonText);
		}
	}
}
