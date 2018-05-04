using System;
using System.Reflection;
using UnityEngine;

namespace Disney.MobileConnector.Ads
{
	public class MCAdsManager : MonoBehaviour
	{
		public static MCAdsManager instance;

		public static event Action<string> adViewDidLoadAdEvent;

		public static event Action<string> adViewDidFailToLoadAdEvent;

		public static event Action<string> interstitialDidLoadAdEvent;

		public static event Action<string> interstitialDidFailToLoadAdEvent;

		public static event Action<string> interstitialWillAppearAdEvent;

		public static event Action<string> interstitialDidDismissEvent;

		public static event Action<string> nativeAdDidLoadAdEvent;

		public static event Action<string> nativeAdDidFailToLoadAdEvent;

		public static event Action<string> rewardedVideoAdDidLoadAdEvent;

		public static event Action<string> rewardedVideoAdDidFailToLoadEvent;

		public static event Action<string> rewardedVideoAdDidAppearEvent;

		public static event Action<string> rewardedVideoAdDidFailToPlayEvent;

		public static event Action<string> rewardedVideoAdDidDisappearEvent;

		public static event Action<string> rewardedVideoAdShouldRewardEvent;

		public static event Action<string> onAdClickedEvent;

		public static event Action<string> onAdExpandedEvent;

		public static event Action<string> onAdCollapsedEvent;

		public static event Action<string> onInterstitialClickedEvent;

		private void Awake()
		{
			if (MCAdsManager.instance)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			MCAdsManager.instance = this;
			base.gameObject.name = "MCAdsManager";
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		public void adViewDidLoadAd(string adUnitId)
		{
			if (MCAdsManager.adViewDidLoadAdEvent != null)
			{
				MCAdsManager.adViewDidLoadAdEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void adViewDidFailToLoadAd(string adUnitId)
		{
			if (MCAdsManager.adViewDidFailToLoadAdEvent != null)
			{
				MCAdsManager.adViewDidFailToLoadAdEvent(adUnitId);
			}
			MCAdsManager.LogWarning(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void interstitialDidLoadAd(string adUnitId)
		{
			if (MCAdsManager.interstitialDidLoadAdEvent != null)
			{
				MCAdsManager.interstitialDidLoadAdEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void interstitialDidFailToLoadAd(string adUnitId)
		{
			if (MCAdsManager.interstitialDidFailToLoadAdEvent != null)
			{
				MCAdsManager.interstitialDidFailToLoadAdEvent(adUnitId);
			}
			MCAdsManager.LogWarning(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void interstitialWillAppear(string adUnitId)
		{
			if (MCAdsManager.interstitialWillAppearAdEvent != null)
			{
				MCAdsManager.interstitialWillAppearAdEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void interstitialDidDismiss(string adUnitId)
		{
			if (MCAdsManager.interstitialDidDismissEvent != null)
			{
				MCAdsManager.interstitialDidDismissEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void nativeAdDidLoadAd(string adUnitId)
		{
			if (MCAdsManager.nativeAdDidLoadAdEvent != null)
			{
				MCAdsManager.nativeAdDidLoadAdEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void nativeAdDidFailToLoadAd(string adUnitId)
		{
			if (MCAdsManager.nativeAdDidFailToLoadAdEvent != null)
			{
				MCAdsManager.nativeAdDidFailToLoadAdEvent(adUnitId);
			}
			MCAdsManager.LogWarning(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void rewardedVideoAdDidLoadAd(string adUnitId)
		{
			if (MCAdsManager.rewardedVideoAdDidLoadAdEvent != null)
			{
				MCAdsManager.rewardedVideoAdDidLoadAdEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void rewardedVideoAdDidFailToLoad(string adUnitId)
		{
			if (MCAdsManager.rewardedVideoAdDidFailToLoadEvent != null)
			{
				MCAdsManager.rewardedVideoAdDidFailToLoadEvent(adUnitId);
			}
			MCAdsManager.LogWarning(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void rewardedVideoAdDidAppear(string adUnitId)
		{
			if (MCAdsManager.rewardedVideoAdDidAppearEvent != null)
			{
				MCAdsManager.rewardedVideoAdDidAppearEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void rewardedVideoAdDidFailToPlay(string adUnitId)
		{
			if (MCAdsManager.rewardedVideoAdDidFailToPlayEvent != null)
			{
				MCAdsManager.rewardedVideoAdDidFailToPlayEvent(adUnitId);
			}
			MCAdsManager.LogWarning(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void rewardedVideoAdDidDisappear(string adUnitId)
		{
			if (MCAdsManager.rewardedVideoAdDidDisappearEvent != null)
			{
				MCAdsManager.rewardedVideoAdDidDisappearEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void rewardedVideoAdShouldReward(string adUnitId)
		{
			if (MCAdsManager.rewardedVideoAdShouldRewardEvent != null)
			{
				MCAdsManager.rewardedVideoAdShouldRewardEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void onAdLoaded(string adUnitId)
		{
			this.adViewDidLoadAd(adUnitId);
		}

		public void onAdFailed(string adUnitId)
		{
			this.adViewDidFailToLoadAd(adUnitId);
		}

		public void onAdClicked(string adUnitId)
		{
			if (MCAdsManager.onAdClickedEvent != null)
			{
				MCAdsManager.onAdClickedEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void onAdExpanded(string adUnitId)
		{
			if (MCAdsManager.onAdExpandedEvent != null)
			{
				MCAdsManager.onAdExpandedEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void onAdCollapsed(string adUnitId)
		{
			if (MCAdsManager.onAdCollapsedEvent != null)
			{
				MCAdsManager.onAdCollapsedEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void onInterstitialLoaded(string adUnitId)
		{
			this.interstitialDidLoadAd(adUnitId);
		}

		public void onInterstitialFailed(string adUnitId)
		{
			this.interstitialDidFailToLoadAd(adUnitId);
		}

		public void onInterstitialShown(string adUnitId)
		{
			this.interstitialWillAppear(adUnitId);
		}

		public void onInterstitialClicked(string adUnitId)
		{
			if (MCAdsManager.onInterstitialClickedEvent != null)
			{
				MCAdsManager.onInterstitialClickedEvent(adUnitId);
			}
			MCAdsManager.LogInfo(MethodBase.GetCurrentMethod().Name + " adUnitId = " + adUnitId);
		}

		public void onInterstitialDismissed(string adUnitId)
		{
			this.interstitialDidDismiss(adUnitId);
		}

		public void onNativeAdLoaded(string adUnitId)
		{
			this.nativeAdDidLoadAd(adUnitId);
		}

		public void onNativeAdFailed(string adUnitId)
		{
			this.nativeAdDidFailToLoadAd(adUnitId);
		}

		public void onRewardedVideoLoadSuccess(string adUnitId)
		{
			this.rewardedVideoAdDidLoadAd(adUnitId);
		}

		public void onRewardedVideoLoadFailure(string adUnitId)
		{
			this.rewardedVideoAdDidFailToLoad(adUnitId);
		}

		public void onRewardedVideoStarted(string adUnitId)
		{
			this.rewardedVideoAdDidAppear(adUnitId);
		}

		public void onRewardedVideoPlaybackError(string adUnitId)
		{
			this.rewardedVideoAdDidFailToPlay(adUnitId);
		}

		public void onRewardedVideoClosed(string adUnitId)
		{
			this.rewardedVideoAdDidDisappear(adUnitId);
		}

		public void onRewardedVideoCompleted(string adUnitId)
		{
			this.rewardedVideoAdShouldReward(adUnitId);
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
