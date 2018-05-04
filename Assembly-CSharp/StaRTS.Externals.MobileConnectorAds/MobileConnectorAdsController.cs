using Disney.MobileConnector.Ads;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Cheats;
using StaRTS.Main.Models.Commands.MobileConnectorAds;
using StaRTS.Main.Models.MobileConnectorAds;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Externals.MobileConnectorAds
{
	public class MobileConnectorAdsController : IEventObserver
	{
		private bool isPendingReward;

		public bool IsVideoAdActive
		{
			get;
			private set;
		}

		public MobileConnectorAdsController()
		{
			Service.MobileConnectorAdsController = this;
			this.isPendingReward = false;
		}

		public bool IsEnabled()
		{
			return GameConstants.MOBILE_CONNECTOR_ADS_ENABLED;
		}

		public bool IsMobileConnectorAdAvailable()
		{
			if (!this.CanInitialize())
			{
				return false;
			}
			string value = "a";
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			string videoAdUnitId = this.GetVideoAdUnitId();
			return MCAdsBinding.IsRewardedVideoReady(videoAdUnitId);
		}

		public void Destroy()
		{
			if (Service.EventManager != null)
			{
				EventManager eventManager = Service.EventManager;
				eventManager.UnregisterObserver(this, EventId.ApplicationPauseToggled);
				eventManager.UnregisterObserver(this, EventId.InAppPurchaseMade);
			}
			MCAdsManager.rewardedVideoAdDidAppearEvent -= new Action<string>(this.VideoAdAppeared);
			MCAdsManager.rewardedVideoAdDidDisappearEvent -= new Action<string>(this.VideoAdDisappeared);
			MCAdsManager.rewardedVideoAdShouldRewardEvent -= new Action<string>(this.ShouldRewardVideoAd);
			MCAdsManager.rewardedVideoAdDidFailToPlayEvent -= new Action<string>(this.VideoAdFailToPlay);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ApplicationPauseToggled)
			{
				if (id == EventId.InAppPurchaseMade)
				{
					MCAdsBinding.SetGuestIsPayer(true);
				}
			}
			else
			{
				this.HandleApplicationPause((bool)cookie);
			}
			return EatResponse.NotEaten;
		}

		public void HandleApplicationPause(bool isPaused)
		{
			if (isPaused)
			{
				MCAdsBinding.PauseRewardedVideo();
			}
			else
			{
				MCAdsBinding.ResumeRewardedVideo();
			}
		}

		public bool CanInitialize()
		{
			if (!this.IsEnabled())
			{
				return false;
			}
			uint daysSinceInstall = GameUtils.GetDaysSinceInstall();
			if ((ulong)daysSinceInstall < (ulong)((long)GameConstants.MOBILE_CONNECTOR_ADS_MIN_DAYS_INSTALL))
			{
				return false;
			}
			MobileConnectorAdsInfo mobileConnectorAdsInfo = Service.CurrentPlayer.MobileConnectorAdsInfo;
			return mobileConnectorAdsInfo != null && !(Service.ServerAPI.ServerDateTime < mobileConnectorAdsInfo.nextAvailableDate);
		}

		public void InitializeAdUnits()
		{
			string videoAdUnitId = this.GetVideoAdUnitId();
			MCAdsBinding.ReportApplicationOpen("847985808");
			string[] adUnits = new string[]
			{
				videoAdUnitId
			};
			MCAdsBinding.SetLogLevel(MCAdsLogging.LogLevel.Error);
			MCAdsLogging.SetLogLevel(MCAdsLogging.LogLevel.Error);
			string guestPreferredLanguage = Service.Lang.ExtractLanguageFromLocale();
			MCAdsBinding.SetGuestPreferredLanguage(guestPreferredLanguage);
			uint lastPaymentTime = Service.CurrentPlayer.LastPaymentTime;
			MCAdsBinding.SetGuestIsPayer(lastPaymentTime > 0u);
			MCAdsBinding.SetGuestAge(99);
			MCAdsBinding.InitializeRewardedVideo(adUnits);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ApplicationPauseToggled);
			eventManager.RegisterObserver(this, EventId.InAppPurchaseMade);
			MCAdsManager.rewardedVideoAdDidAppearEvent += new Action<string>(this.VideoAdAppeared);
			MCAdsManager.rewardedVideoAdDidDisappearEvent += new Action<string>(this.VideoAdDisappeared);
			MCAdsManager.rewardedVideoAdShouldRewardEvent += new Action<string>(this.ShouldRewardVideoAd);
			MCAdsManager.rewardedVideoAdDidFailToPlayEvent += new Action<string>(this.VideoAdFailToPlay);
		}

		public void ClearMobileConnectorAdsRecords()
		{
			CheatClearMobileConnectorAdsCommand command = new CheatClearMobileConnectorAdsCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			Service.ServerAPI.Sync(command);
		}

		public void ShowRewardedVideoAd()
		{
			if (!this.IsMobileConnectorAdAvailable())
			{
				Service.Logger.Warn("Not Available Yet");
				return;
			}
			ProcessingScreen.Show();
			this.HandleBackgrounding();
			string videoAdUnitId = this.GetVideoAdUnitId();
			this.BILogEvent("video_ad", "start", videoAdUnitId);
			MCAdsBinding.ShowRewardedVideo(videoAdUnitId);
		}

		public void VideoAdAppeared(string adUnit)
		{
			Service.Logger.Debug("VideoAd Appeared: " + adUnit);
		}

		public void VideoAdDisappeared(string adUnit)
		{
			this.HandleForegrounding();
			ProcessingScreen.Hide();
			Service.Logger.Debug("VideoAd Disappeared: " + adUnit);
			this.BILogEvent("video_ad", "closed", adUnit);
			if (this.isPendingReward)
			{
				this.RequestReward();
			}
		}

		public void VideoAdFailToPlay(string adUnit)
		{
			this.BILogEvent("video_ad", "closed", adUnit);
			this.HandleForegrounding();
			ProcessingScreen.Hide();
			Service.Logger.Error("VideoAd Fail to Play: " + adUnit);
		}

		public void ShouldRewardVideoAd(string adUnit)
		{
			this.BILogEvent("video_ad", "complete", adUnit);
			this.isPendingReward = true;
		}

		private void RequestReward()
		{
			this.isPendingReward = false;
			ClaimMobileConnectorAdsRewardCommand claimMobileConnectorAdsRewardCommand = new ClaimMobileConnectorAdsRewardCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			claimMobileConnectorAdsRewardCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, ClaimMobileConnectorAdsRewardResponse>.OnSuccessCallback(this.OnRewardSuccess));
			Service.ServerAPI.Sync(claimMobileConnectorAdsRewardCommand);
		}

		private void OnRewardSuccess(ClaimMobileConnectorAdsRewardResponse response, object cookie)
		{
			if (response.CrateDataTO != null)
			{
				ProcessingScreen.Show();
				Service.EventManager.SendEvent(EventId.OpeningEpisodeTaskCrate, null);
				CrateData crateDataTO = response.CrateDataTO;
				List<string> resolvedSupplyIdList = GameUtils.GetResolvedSupplyIdList(crateDataTO);
				Service.InventoryCrateRewardController.GrantInventoryCrateReward(resolvedSupplyIdList, crateDataTO);
			}
		}

		private string GetVideoAdUnitId()
		{
			return "97f642ad52904742b48973ab67f75e0c";
		}

		private void BILogEvent(string context, string action, string message)
		{
			if (Service.BILoggingController != null)
			{
				Service.BILoggingController.TrackGameAction(context, action, message, null);
			}
		}

		private void HandleBackgrounding()
		{
			this.IsVideoAdActive = true;
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = false;
			}
			if (Service.AudioManager != null)
			{
				Service.AudioManager.ToggleAllSounds(false);
			}
			if (Service.EnvironmentController != null)
			{
				Service.EnvironmentController.SetAutoMuteOnBackgroundMute(false);
			}
		}

		private void HandleForegrounding()
		{
			this.IsVideoAdActive = false;
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.ForceResetInactivityTimer();
				Service.GameIdleController.Enabled = true;
			}
			if (Service.AudioManager != null)
			{
				Service.AudioManager.ToggleAllSounds(true);
			}
			if (Service.EnvironmentController != null)
			{
				Service.EnvironmentController.SetAutoMuteOnBackgroundMute(true);
			}
		}
	}
}
