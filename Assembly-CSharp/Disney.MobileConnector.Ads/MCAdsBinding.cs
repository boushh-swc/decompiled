using System;
using System.Reflection;
using UnityEngine;

namespace Disney.MobileConnector.Ads
{
	public static class MCAdsBinding
	{
		private static AndroidJavaObject _plugin;

		static MCAdsBinding()
		{
			if (Application.isEditor)
			{
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mopub.mobileads.MoPubUnityPlugin"))
			{
				MCAdsBinding._plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}

		public static void CreateBanner(string adUnitId, MCAdsAdPlacement position = MCAdsAdPlacement.Custom, MCAdsBannerType size = MCAdsBannerType.Size320x50, string keywords = "", int marginTop = 0, int marginLeft = 0, int customBannerWidth = 0, int customBannerHeight = 0)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("showBanner", new object[]
			{
				(int)size,
				(int)position,
				adUnitId,
				keywords,
				marginTop,
				marginLeft,
				customBannerWidth,
				customBannerHeight
			});
		}

		public static void ShowBanner(string adUnitId, bool shouldShow = true)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("hideBanner", new object[]
			{
				adUnitId,
				!shouldShow
			});
		}

		public static void HideBanner(string adUnitId)
		{
			MCAdsBinding.ShowBanner(adUnitId, false);
		}

		public static void RefreshBannerWithKeywords(string keywords)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("setBannerKeywords", new object[]
			{
				keywords
			});
		}

		public static void DestroyBanner(string adUnitId)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("destroyBanner", new object[]
			{
				adUnitId
			});
		}

		public static void SetAutoRefreshBannerEnabled(string adUnitId, bool enabled)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("setAutorefreshEnabled", new object[]
			{
				adUnitId,
				enabled
			});
		}

		[Obsolete("This functionality is now automatically handled behind the scenes.")]
		public static void RequestInterstitialAd(string adUnitId, string keywords = null)
		{
		}

		[Obsolete("Please use InitializeInterstitialAdSystem instead.")]
		public static void InitializeInterstitials(string[] adUnits)
		{
			MCAdsBinding.InitializeInterstitialAdSystem(adUnits);
		}

		public static void InitializeInterstitialAdSystem(string[] adUnits)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (adUnits == null || adUnits.Length == 0)
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			if (adUnits.Length > 0)
			{
				string text = adUnits[0];
				for (int i = 1; i < adUnits.Length; i++)
				{
					text = text + "," + adUnits[i];
				}
				MCAdsBinding._plugin.Call("initializeInterstitialAdSystem", new object[]
				{
					text
				});
			}
		}

		public static void TerminateInterstitialAdSystem()
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("terminateInterstitialAdSystem", new object[0]);
		}

		public static void SetInterstitialKeywords(string keywords)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("setInterstitialKeywords", new object[]
			{
				keywords
			});
		}

		public static void ShowInterstitialAd(string adUnitId)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("showInterstitial", new object[]
			{
				adUnitId
			});
		}

		public static void RequestNativeAd(string adUnitId, string keywords = null)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("requestNativeAd", new object[]
			{
				adUnitId,
				keywords
			});
		}

		public static string GetNativeAdTitle(string adUnitId)
		{
			if (Application.isEditor)
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			return MCAdsBinding._plugin.Call<string>("getNativeAdTitle", new object[]
			{
				adUnitId
			});
		}

		public static string GetNativeAdImageURL(string adUnitId)
		{
			if (Application.isEditor)
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			return MCAdsBinding._plugin.Call<string>("getNativeAdImageURL", new object[]
			{
				adUnitId
			});
		}

		public static string GetNativeAdClickURL(string adUnitId)
		{
			if (Application.isEditor)
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			return MCAdsBinding._plugin.Call<string>("getNativeAdClickURL", new object[]
			{
				adUnitId
			});
		}

		public static void CallNativeAdTrackImpression(string adUnitId)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("callNativeAdTrackImpression", new object[]
			{
				adUnitId
			});
		}

		public static void CallNativeAdTrackClick(string adUnitId)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("callNativeAdTrackClick", new object[]
			{
				adUnitId
			});
		}

		public static void ReportApplicationOpen(string iTunesAppId = "")
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("reportApplicationOpen", new object[0]);
		}

		public static bool IsInterstitalAdReady(string adUnitId)
		{
			if (Application.isEditor)
			{
				return false;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			return MCAdsBinding._plugin.Call<bool>("hasInterstitial", new object[]
			{
				adUnitId
			});
		}

		[Obsolete("Please use InitializeRewardedAdSystem instead.")]
		public static void InitializeRewardedVideo(string[] adUnits)
		{
			MCAdsBinding.InitializeRewardedAdSystem(adUnits);
		}

		public static void InitializeRewardedAdSystem(string[] adUnits)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding.LogInfo(MethodBase.GetCurrentMethod().Name);
			MCAdsBinding.GetAdvertisingId();
			if (adUnits.Length < 1)
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
				return;
			}
			if (adUnits.Length > 0)
			{
				string text = adUnits[0];
				for (int i = 1; i < adUnits.Length; i++)
				{
					text = text + "," + adUnits[i];
				}
				MCAdsBinding._plugin.Call("initializeRewardedAdSystem", new object[]
				{
					text
				});
			}
		}

		public static void TerminateRewardedAdSystem()
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("terminateRewardedAdSystem", new object[0]);
		}

		public static void PauseRewardedVideo()
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("pauseRewardedVideo", new object[0]);
		}

		public static void ResumeRewardedVideo()
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("resumeRewardedVideo", new object[0]);
		}

		public static void SetCustomSegmentationKeywords(string keywords)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("setCustomSegmentationKeywords", new object[]
			{
				keywords
			});
		}

		[Obsolete("This functionality is now automatically handled behind the scenes.")]
		public static void RequestRewardedVideo(string adUnitId, string keywords)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("requestRewardedVideo", new object[]
			{
				adUnitId,
				keywords
			});
		}

		public static void ShowRewardedVideo(string adUnitId)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			MCAdsBinding._plugin.Call("showRewardedVideo", new object[]
			{
				adUnitId
			});
		}

		public static bool IsRewardedVideoReady(string adUnitId)
		{
			if (Application.isEditor)
			{
				return false;
			}
			if (string.IsNullOrEmpty(adUnitId))
			{
				MCAdsBinding.LogMissingAdUnitID(MethodBase.GetCurrentMethod().Name);
			}
			return MCAdsBinding._plugin.Call<bool>("isRewardedVideoReady", new object[]
			{
				adUnitId
			});
		}

		public static void SetRewardedVideoParentalGateType(MPRewardedVideoParentalGateType type)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding.LogInfo("SetRewardedVideoParentalGateType() type set to: " + type.ToString());
		}

		public static bool CheckRewardedVideoParentalGateAnswer(string answer)
		{
			return Application.isEditor && false;
		}

		public static void SetGuestAge(int age)
		{
			if (Application.isEditor)
			{
				return;
			}
			if (age < 0)
			{
				MCAdsBinding.LogWarning("SetGuestAge() called with negative age: " + age.ToString());
			}
			else
			{
				MCAdsBinding.LogInfo("SetGuestAge() age set to: " + age.ToString());
			}
			MCAdsBinding._plugin.Call("setGuestAge", new object[]
			{
				age
			});
		}

		public static void SetGuestPreferredLanguage(string languageCode)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding.LogInfo("SetGuestPreferredLanguage() languageCode set to: " + languageCode);
			MCAdsBinding._plugin.Call("setGuestPreferredLanguage", new object[]
			{
				languageCode
			});
		}

		public static void SetGuestIsPayer(bool isPayer)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding.LogInfo("SetGuestIsPayer() isPayer set to: " + isPayer.ToString());
			MCAdsBinding._plugin.Call("setGuestIsPayer", new object[]
			{
				isPayer
			});
		}

		public static string GetAutomaticSegmentationKeywords()
		{
			if (Application.isEditor)
			{
				return string.Empty;
			}
			return MCAdsBinding._plugin.Call<string>("getAutomaticSegmentationKeywords", new object[0]);
		}

		public static void UseImmersiveModeForInterstitials(bool enabled)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("useImmersiveModeForInterstitials", new object[]
			{
				enabled
			});
		}

		public static string GetMCAdsUnityPluginVersion()
		{
			if (Application.isEditor)
			{
				return "MC ADS UNITY PLUGIN VERSION";
			}
			return MCAdsBinding.GetMCAdsSDKVersion();
		}

		public static string GetMoPubSDKVersion()
		{
			if (Application.isEditor)
			{
				return "MOPUB SDK VERSION";
			}
			return MCAdsBinding._plugin.Call<string>("getMoPubSDKVersion", new object[0]);
		}

		public static string GetSuperAwesomeSDKVersion()
		{
			if (Application.isEditor)
			{
				return "SUPERAWESOME SDK VERSION";
			}
			return MCAdsBinding._plugin.Call<string>("getSuperAwesomeSDKVersion", new object[0]);
		}

		public static string GetUnityAdsSDKVersion()
		{
			if (Application.isEditor)
			{
				return "UNITY ADS SDK VERSION";
			}
			return MCAdsBinding._plugin.Call<string>("getUnityAdsSDKVersion", new object[0]);
		}

		public static string GetMoPubDirectSDKVersion()
		{
			if (Application.isEditor)
			{
				return "MOPUB DIRECT SDK VERSION";
			}
			return MCAdsBinding._plugin.Call<string>("getMoPubDirectSDKVersion", new object[0]);
		}

		public static string GetMCAdsSDKVersion()
		{
			if (Application.isEditor)
			{
				return "MC ADS SDK VERSION";
			}
			return MCAdsBinding._plugin.Call<string>("getMCAdsSDKVersion", new object[0]);
		}

		public static string GetAdColonySDKVersion()
		{
			if (Application.isEditor)
			{
				return "ADCOLONY SDK VERSION";
			}
			return "2.3.6";
		}

		[Obsolete("Tapjoy is no longer part of the Mobile Connector Ads SDK")]
		public static string GetTapjoySDKVersion()
		{
			return string.Empty;
		}

		public static string GetIDFA()
		{
			return MCAdsBinding.GetAdvertisingId();
		}

		public static string GetGIDA()
		{
			return MCAdsBinding.GetAdvertisingId();
		}

		public static string GetAdvertisingId()
		{
			if (Application.isEditor)
			{
				return string.Empty;
			}
			MCAdsBinding.LogInfo(MethodBase.GetCurrentMethod().Name);
			return MCAdsBinding._plugin.Call<string>("getGIDA", new object[0]);
		}

		public static string GetDeviceType()
		{
			if (Application.isEditor)
			{
				return "EDITOR";
			}
			MCAdsBinding.LogInfo(MethodBase.GetCurrentMethod().Name);
			return MCAdsBinding._plugin.Call<string>("getDeviceType", new object[0]);
		}

		public static bool IsTablet()
		{
			return !Application.isEditor && MCAdsBinding._plugin.Call<bool>("isTablet", new object[0]);
		}

		public static string GetLanguageCode()
		{
			if (Application.isEditor)
			{
				return "en";
			}
			return MCAdsBinding._plugin.Call<string>("getLanguageCode", new object[0]);
		}

		public static void SetIsGeneralAudienceApp()
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("setIsGeneralAudienceApp", new object[0]);
		}

		public static void EnableLocationSupport(bool shouldUseLocation)
		{
			if (Application.isEditor)
			{
				return;
			}
		}

		public static void ReadjustAdPosition(string adUnitId)
		{
			if (Application.isEditor)
			{
				return;
			}
		}

		[Obsolete("Tapjoy is no longer part of the Mobile Connector Ads SDK")]
		public static void InitializeTapjoy(string appID, string secretKey)
		{
		}

		[Obsolete("Tapjoy is no longer part of the Mobile Connector Ads SDK")]
		public static void ShowOfferWall(string currencyId = null)
		{
		}

		public static void SetLogLevel(MCAdsLogging.LogLevel level)
		{
			if (Application.isEditor)
			{
				return;
			}
			MCAdsBinding._plugin.Call("setLogLevel", new object[]
			{
				(int)level
			});
		}

		private static void LogMissingAdUnitID(string function)
		{
			MCAdsBinding.LogError(function + "() missing ad unit ID!");
		}

		private static void LogError(string log)
		{
			MCAdsLogging.Log(log, MCAdsLogging.LogLevel.Error);
		}

		private static void LogWarning(string log)
		{
			MCAdsLogging.Log(log, MCAdsLogging.LogLevel.Warning);
		}

		private static void LogInfo(string log)
		{
			MCAdsLogging.Log(log, MCAdsLogging.LogLevel.Verbose);
		}
	}
}
