using System;

namespace StaRTS.Externals.GameServices
{
	public interface IGameServicesHelper
	{
		void OnReady();

		void SignIn();

		void SignOut();

		void ShowAchievements();

		void AddScoreToLeaderboard(int score, string leaderboardId);

		void ShowLeaderboard(string leaderboardId);

		void UnlockAchievement(string achievementId);

		void Share(string text, string contentURL, string thumbnailURL);

		void HandleUserIdCallback(string userId);

		void HandleAuthTokenCallback(string authToken);

		bool IsUserAuthenticated();

		bool HasBeenPromptedForSignIn();

		string GetUserId();

		string GetAuthToken();

		string GetUserDomain();

		string GetExternalNetworkCode();
	}
}
