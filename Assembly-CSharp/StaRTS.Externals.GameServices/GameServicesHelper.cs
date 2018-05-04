using System;

namespace StaRTS.Externals.GameServices
{
	public class GameServicesHelper : IGameServicesHelper
	{
		private const string USER_DOMAIN = "gp";

		private const string EXTERNAL_NETWORK_CODE = "googleplus";

		private const string USER_ID = "1234567890";

		private bool mUserIsAuthenticated;

		public void OnReady()
		{
			this.SignIn();
		}

		public void SignIn()
		{
			this.mUserIsAuthenticated = true;
		}

		public void SignOut()
		{
			this.mUserIsAuthenticated = false;
		}

		public bool IsUserAuthenticated()
		{
			return this.mUserIsAuthenticated;
		}

		public bool HasBeenPromptedForSignIn()
		{
			return true;
		}

		public string GetUserId()
		{
			return "1234567890";
		}

		public string GetAuthToken()
		{
			return null;
		}

		public string GetUserDomain()
		{
			return "gp";
		}

		public string GetExternalNetworkCode()
		{
			return "googleplus";
		}

		public void UnlockAchievement(string achievementId)
		{
			if (this.IsUserAuthenticated())
			{
			}
		}

		public void AddScoreToLeaderboard(int score, string leaderboardId)
		{
			if (this.IsUserAuthenticated())
			{
			}
		}

		public void ShowAchievements()
		{
			if (this.IsUserAuthenticated())
			{
			}
		}

		public void ShowLeaderboard(string leaderboardId)
		{
			if (this.IsUserAuthenticated())
			{
			}
		}

		public void Share(string text, string contentURL, string thumbnailURL)
		{
			if (this.IsUserAuthenticated())
			{
			}
		}

		public void HandleUserIdCallback(string userId)
		{
		}

		public void HandleAuthTokenCallback(string authToken)
		{
		}
	}
}
