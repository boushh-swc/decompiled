using StaRTS.Main.Models.Player;
using StaRTS.Main.RUF;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class PlayerIdentityStartupTask : StartupTask
	{
		public PlayerIdentityStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new PlayerIdentityController();
			new QuestController();
			new SupportController();
			new RUFManager();
			new PopupsManager();
			new AccountSyncController();
			new SocialDataController();
			new SharedPlayerPrefs();
			new LeaderboardController();
			base.Complete();
		}
	}
}
