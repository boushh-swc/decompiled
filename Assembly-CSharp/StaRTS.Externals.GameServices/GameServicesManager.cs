using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Externals.GameServices
{
	public class GameServicesManager : MonoBehaviour, IEventObserver
	{
		private static IGameServicesHelper mGameServicesHelper;

		private static bool isWindowOpen;

		public static void StaticReset()
		{
			GameServicesManager.mGameServicesHelper = null;
			GameServicesManager.isWindowOpen = false;
		}

		public static void Startup()
		{
			GameObject gameObject = GameObject.Find("Game Services Manager");
			if (gameObject == null)
			{
				Service.Logger.Error("Unable to find Game Services Manager object");
			}
			else
			{
				GameServicesManager component = gameObject.GetComponent<GameServicesManager>();
				if (component == null)
				{
					Service.Logger.Error("Missing GameServicesManager component");
				}
				else
				{
					component.enabled = true;
					GameServicesManager.Init();
					Service.EventManager.RegisterObserver(component, EventId.ApplicationPauseToggled, EventPriority.AfterDefault);
				}
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ApplicationPauseToggled)
			{
				this.HandleApplicationPause((bool)cookie);
			}
			return EatResponse.NotEaten;
		}

		public static void AttemptAutomaticSignInPrompt()
		{
			string derivedAccountProviderId = Service.IAccountSyncController.GetDerivedAccountProviderId(AccountProvider.GOOGLEPLAY);
			bool flag = GameServicesManager.mGameServicesHelper.HasBeenPromptedForSignIn();
			if (string.IsNullOrEmpty(derivedAccountProviderId) && !flag)
			{
				GameServicesManager.mGameServicesHelper.SignIn();
			}
		}

		private void HandleApplicationPause(bool paused)
		{
			if (paused)
			{
				return;
			}
			if (GameServicesManager.isWindowOpen)
			{
				GameServicesManager.isWindowOpen = false;
				GameServicesManager.ToggleIdleTimerAndSounds(true);
			}
		}

		private static void Init()
		{
			GameServicesManager.mGameServicesHelper = new GameServicesHelperAndroid();
		}

		private static void ToggleIdleTimerAndSounds(bool enabled)
		{
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = enabled;
			}
			if (Service.AudioManager != null)
			{
				Service.AudioManager.ToggleAllSounds(enabled);
			}
		}

		public static void OnReady()
		{
			GameServicesManager.mGameServicesHelper.OnReady();
		}

		public static void SignIn()
		{
			GameServicesManager.ToggleIdleTimerAndSounds(false);
			GameServicesManager.isWindowOpen = true;
			GameServicesManager.mGameServicesHelper.SignIn();
		}

		public static void SignOut()
		{
			GameServicesManager.mGameServicesHelper.SignOut();
		}

		public static void UnlockAchievement(string achievementId)
		{
			if (GameServicesManager.mGameServicesHelper != null)
			{
				GameServicesManager.mGameServicesHelper.UnlockAchievement(achievementId);
			}
		}

		public static void AddScoreToLeaderboard(int score, string leaderboardId)
		{
			GameServicesManager.mGameServicesHelper.AddScoreToLeaderboard(score, leaderboardId);
		}

		public static void ShowAchievements()
		{
			GameServicesManager.ToggleIdleTimerAndSounds(false);
			GameServicesManager.isWindowOpen = true;
			GameServicesManager.mGameServicesHelper.ShowAchievements();
		}

		public static void ShowLeaderboard(string leaderboardId)
		{
			GameServicesManager.ToggleIdleTimerAndSounds(false);
			GameServicesManager.isWindowOpen = true;
			GameServicesManager.mGameServicesHelper.ShowLeaderboard(leaderboardId);
		}

		public static void Share(string text, string contentURL, string thumbnailURL)
		{
			GameServicesManager.ToggleIdleTimerAndSounds(false);
			GameServicesManager.isWindowOpen = true;
			GameServicesManager.mGameServicesHelper.Share(text, contentURL, thumbnailURL);
		}

		public static bool IsUserAuthenticated()
		{
			return GameServicesManager.mGameServicesHelper.IsUserAuthenticated();
		}

		public static string GetUserId()
		{
			return GameServicesManager.mGameServicesHelper.GetUserId();
		}

		public static string GetAuthToken()
		{
			return GameServicesManager.mGameServicesHelper.GetAuthToken();
		}

		public static string GetUserDomain()
		{
			return GameServicesManager.mGameServicesHelper.GetUserDomain();
		}

		public static string GetExternalNetworkCode()
		{
			return GameServicesManager.mGameServicesHelper.GetExternalNetworkCode();
		}

		public void GameServicesUserIdCallback(string userId)
		{
			GameServicesManager.ToggleIdleTimerAndSounds(true);
			GameServicesManager.isWindowOpen = false;
			if (GameServicesManager.mGameServicesHelper != null)
			{
				GameServicesManager.mGameServicesHelper.HandleUserIdCallback(userId);
			}
		}

		public void GameServicesSignInFailedCallback(string errorCode)
		{
			if (Service.EventManager != null)
			{
				Service.EventManager.SendEvent(EventId.GameServicesSignedOut, AccountProvider.GOOGLEPLAY);
			}
		}

		public void GameServicesAuthTokenCallback(string authToken)
		{
			if (GameServicesManager.mGameServicesHelper != null)
			{
				GameServicesManager.mGameServicesHelper.HandleAuthTokenCallback(authToken);
			}
		}
	}
}
