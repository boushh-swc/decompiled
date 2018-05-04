using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class ShowLoadingScreenPopupsStartupTask : StartupTask
	{
		private const string COUNTRY_CODE_ALL = "ALL";

		private bool checkedDeviceCompatibilty;

		private bool checkedIAPDisclaimer;

		private bool checkedUnderAttack;

		public ShowLoadingScreenPopupsStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			this.TryComplete();
		}

		private void TryComplete()
		{
			bool flag = Service.PopupsManager.DisplayAdminMessagesOnQueue(true);
			if (flag)
			{
				return;
			}
			if (!this.checkedDeviceCompatibilty)
			{
				this.CheckDeviceCompatibility();
			}
			else if (!this.checkedIAPDisclaimer)
			{
				this.CheckIAPDisclaimer();
			}
			else if (!this.checkedUnderAttack)
			{
				this.CheckUnderAttack();
			}
			else
			{
				base.Complete();
			}
		}

		private void CheckUnderAttack()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.CurrentlyDefending && !currentPlayer.CampaignProgress.FueInProgress)
			{
				Service.ScreenController.AddScreen(new UnderAttackScreen(currentPlayer.CurrentlyDefendingExpireTime));
			}
			else
			{
				this.checkedUnderAttack = true;
				this.TryComplete();
			}
		}

		private void CheckIAPDisclaimer()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			bool flag = currentPlayer.FirstTimePlayer && currentPlayer.NumIdentities == 1;
			if (flag)
			{
				string iAP_DISCLAIMER_WHITELIST = GameConstants.IAP_DISCLAIMER_WHITELIST;
				flag = (iAP_DISCLAIMER_WHITELIST == "ALL" || GameUtils.IsDeviceCountryInList(iAP_DISCLAIMER_WHITELIST));
			}
			if (flag)
			{
				Service.ScreenController.AddScreen(new IAPDisclaimerScreen(new OnScreenModalResult(this.OnIAPDisclaimerViewed)));
			}
			else
			{
				this.checkedIAPDisclaimer = true;
				this.TryComplete();
			}
		}

		private void OnIAPDisclaimerViewed(object result, object cookie)
		{
			this.checkedIAPDisclaimer = true;
			this.TryComplete();
		}

		private void CheckDeviceCompatibility()
		{
			if (!Service.CurrentPlayer.HasNotCompletedFirstFueStep())
			{
				this.checkedDeviceCompatibilty = true;
				this.TryComplete();
				return;
			}
			bool flag = false;
			if (flag)
			{
				Service.UserInputManager.Enable(true);
				Service.CameraManager.SetCameraOrderForPreloadScreens();
				Lang lang = Service.Lang;
				AlertScreen.ShowModal(false, lang.Get("ALERT", new object[0]), lang.Get("DEVICE_NOT_SUPPORTED", new object[0]), new OnScreenModalResult(this.OnDeviceCompatibilityWarningClosed), null);
			}
			else
			{
				this.checkedDeviceCompatibilty = true;
				this.TryComplete();
			}
		}

		private void OnDeviceCompatibilityWarningClosed(object result, object cookie)
		{
			Service.CameraManager.SetRegularCameraOrder();
			Service.UserInputManager.Enable(false);
			this.checkedDeviceCompatibilty = true;
			this.TryComplete();
		}
	}
}
