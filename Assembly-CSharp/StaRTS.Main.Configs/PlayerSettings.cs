using StaRTS.Main.Controllers;
using System;

namespace StaRTS.Main.Configs
{
	public static class PlayerSettings
	{
		private const string PREF_MUSIC_VOLUME = "prefMusicVolume";

		private const string PREF_SFX_VOLUME = "prefSfxVolume";

		private const string PREF_NOTIFICATIONS_LEVEL = "prefNotificationsLevel2";

		private const string PREF_LOCALE_COPY = "prefLocaleCopy";

		private const string PREF_HARDWARE_PROFILE = "prefHardwareProfile";

		private const string PREF_SKIP_RAID_WAIT = "prefSkipRaidWait";

		private const string PREF_SKIP_RAID_DEFEND = "prefSkipRaidDefend";

		private const string PREF_SHARD_SHOP_EXPIRATION = "prefShardShopExpiration";

		public const float DEFAULT_AUDIO_VOLUME = 1f;

		public const int DEFAULT_NOTIFICATIONS_LEVEL = 100;

		public const int HARDWARE_PROFILE_DEFAULT = 0;

		public const int HARDWARE_PROFILE_FORCE_HIGH_END = 1;

		public const int HARDWARE_PROFILE_FORCE_LOW_END = 2;

		public const int HARDWARE_PROFILE_COUNT = 3;

		public static float GetMusicVolume()
		{
			return UserPlayerPrefsController.GetFloat("prefMusicVolume", 1f);
		}

		public static void SetMusicVolume(float volume)
		{
			UserPlayerPrefsController.SetFloat("prefMusicVolume", volume);
		}

		public static float GetSfxVolume()
		{
			return UserPlayerPrefsController.GetFloat("prefSfxVolume", 1f);
		}

		public static void SetSfxVolume(float volume)
		{
			UserPlayerPrefsController.SetFloat("prefSfxVolume", volume);
		}

		public static int GetNotificationsLevel()
		{
			return UserPlayerPrefsController.GetInt("prefNotificationsLevel2", 0);
		}

		public static void SetNotificationsLevel(int level)
		{
			UserPlayerPrefsController.SetInt("prefNotificationsLevel2", level);
		}

		public static void SetSkipRaidWaitConfirmation(bool skip)
		{
			UserPlayerPrefsController.SetInt("prefSkipRaidWait", (!skip) ? 0 : 1);
		}

		public static bool GetSkipRaidWaitConfirmation()
		{
			return UserPlayerPrefsController.GetInt("prefSkipRaidWait", 0) == 1;
		}

		public static void SetSkipRaidDefendConfirmation(bool skip)
		{
			UserPlayerPrefsController.SetInt("prefSkipRaidDefend", (!skip) ? 0 : 1);
		}

		public static bool GetSkipRaidDefendConfirmation()
		{
			return UserPlayerPrefsController.GetInt("prefSkipRaidDefend", 0) == 1;
		}

		public static string GetLocaleCopy()
		{
			return UserPlayerPrefsController.GetString("prefLocaleCopy", null);
		}

		public static void SetLocaleCopy(string locale)
		{
			UserPlayerPrefsController.SetString("prefLocaleCopy", locale);
		}

		public static int GetHardwareProfile()
		{
			return UserPlayerPrefsController.GetInt("prefHardwareProfile", 0);
		}

		public static void SetHardwareProfile(int hardwareProfile)
		{
			UserPlayerPrefsController.SetInt("prefHardwareProfile", hardwareProfile);
		}

		public static string GetShardShopExpiration()
		{
			return UserPlayerPrefsController.GetString("prefShardShopExpiration", null);
		}

		public static void SetShardShopExpiration(string expirationTime)
		{
			UserPlayerPrefsController.SetString("prefShardShopExpiration", expirationTime);
		}

		public static void ResetAllSettingsToDefault()
		{
			UserPlayerPrefsController.SetInt("prefHardwareProfile", 0);
			UserPlayerPrefsController.SetString("prefLocaleCopy", null);
			UserPlayerPrefsController.SetInt("prefSkipRaidDefend", 0);
			UserPlayerPrefsController.SetInt("prefSkipRaidWait", 0);
			UserPlayerPrefsController.SetInt("prefNotificationsLevel2", 0);
			UserPlayerPrefsController.SetFloat("prefSfxVolume", 1f);
			UserPlayerPrefsController.SetFloat("prefMusicVolume", 1f);
		}
	}
}
