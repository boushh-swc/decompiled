using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class UserPlayerPrefsController
	{
		public static void SetString(string key, string value)
		{
			UserPlayerPrefsController.ClearOriginalKey(key);
			string prefix = UserPlayerPrefsController.GetPrefix();
			PlayerPrefs.SetString(prefix + key, value);
		}

		public static void SetFloat(string key, float value)
		{
			UserPlayerPrefsController.ClearOriginalKey(key);
			string prefix = UserPlayerPrefsController.GetPrefix();
			PlayerPrefs.SetFloat(prefix + key, value);
		}

		public static void SetInt(string key, int value)
		{
			UserPlayerPrefsController.ClearOriginalKey(key);
			string prefix = UserPlayerPrefsController.GetPrefix();
			PlayerPrefs.SetInt(prefix + key, value);
		}

		public static string GetString(string key, string fallback)
		{
			string prefix = UserPlayerPrefsController.GetPrefix();
			if (PlayerPrefs.HasKey(prefix + key))
			{
				return PlayerPrefs.GetString(prefix + key);
			}
			if (PlayerPrefs.HasKey(key))
			{
				return PlayerPrefs.GetString(key);
			}
			return fallback;
		}

		public static float GetFloat(string key, float fallback)
		{
			string prefix = UserPlayerPrefsController.GetPrefix();
			if (PlayerPrefs.HasKey(prefix + key))
			{
				return PlayerPrefs.GetFloat(prefix + key);
			}
			if (PlayerPrefs.HasKey(key))
			{
				return PlayerPrefs.GetFloat(key);
			}
			return fallback;
		}

		public static int GetInt(string key, int fallback)
		{
			string prefix = UserPlayerPrefsController.GetPrefix();
			if (PlayerPrefs.HasKey(prefix + key))
			{
				return PlayerPrefs.GetInt(prefix + key);
			}
			if (PlayerPrefs.HasKey(key))
			{
				return PlayerPrefs.GetInt(key);
			}
			return fallback;
		}

		private static string GetPrefix()
		{
			if (Service.CurrentPlayer != null)
			{
				return Service.CurrentPlayer.PlayerId;
			}
			if (PlayerPrefs.HasKey("prefPlayerId"))
			{
				return PlayerPrefs.GetString("prefPlayerId");
			}
			return string.Empty;
		}

		private static void ClearOriginalKey(string key)
		{
			if (PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}
	}
}
