using System;

namespace StaRTS.Externals.EnvironmentManager
{
	public interface IEnvironmentManager
	{
		void Init();

		void SetAutoMuteOnBackgroundMute(bool value);

		string GetDeviceId();

		string GetDeviceIdForEvent2();

		bool IsDeviceIdValid();

		string GetDeviceIdType();

		string GetLocale();

		string GetCurrencyCode();

		bool IsAutoRotationEnabled();

		bool IsRestrictedProfile();

		bool IsMusicPlaying();

		bool IsTablet();

		bool AreHeadphonesConnected();

		int GetAPILevel();

		string GetMachine();

		string GetModel();

		void GainAudioFocus();

		void ShowAlert(string titleText, string messageText, string yesButtonText, string noButtonText);

		string GetOS();

		string GetPlatform();
	}
}
