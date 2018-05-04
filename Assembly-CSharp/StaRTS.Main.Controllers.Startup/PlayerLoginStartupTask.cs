using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class PlayerLoginStartupTask : StartupTask
	{
		public PlayerLoginStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			LoginCommand loginCommand = new LoginCommand(new LoginRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId,
				LocalePreference = Service.Lang.Locale,
				DeviceToken = Service.NotificationController.GetDeviceToken(),
				TimeZoneOffset = Service.EnvironmentController.GetTimezoneOffset()
			});
			loginCommand.AddSuccessCallback(new AbstractCommand<LoginRequest, LoginResponse>.OnSuccessCallback(this.OnLoginComplete));
			Service.ServerAPI.Async(loginCommand);
		}

		private void OnLoginComplete(LoginResponse response, object cookie)
		{
			Service.Logger.Debug("Player Logged In Successfully.");
			Service.EventManager.SendEvent(EventId.PlayerLoginSuccess, null);
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Service.ServerAPI.StartSession(currentPlayer.LoginTime);
			if (!Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				Service.NotificationController.TryEnableNotifications();
			}
			if (Service.CurrentPlayer.SessionCountToday == 1)
			{
				Kochava.FireEvent("dayPlayed", "1");
			}
			currentPlayer.Prizes.Crates.UpdateBadgingBasedOnAvailableCrates();
			base.Complete();
			Service.IAccountSyncController.UpdateExternalAccountInfo(new OnUpdateExternalAccountInfoResponseReceived(this.OnUpdateExternalAccountInfoResponseReceived));
		}

		private void OnUpdateExternalAccountInfoResponseReceived()
		{
			Service.ISocialDataController.PopulateFacebookData();
		}
	}
}
