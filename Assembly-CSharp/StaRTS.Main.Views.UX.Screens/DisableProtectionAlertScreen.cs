using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Text;

namespace StaRTS.Main.Views.UX.Screens
{
	public class DisableProtectionAlertScreen : AlertScreen, IViewClockTimeObserver
	{
		protected DisableProtectionAlertScreen() : base(false, null, DisableProtectionAlertScreen.GetProtectionTimeRemaining(), UXUtils.GetCurrencyItemAssetName("protection"), false)
		{
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		public static void ShowModal(OnScreenModalResult onModalResult, object modalResultCookie)
		{
			DisableProtectionAlertScreen disableProtectionAlertScreen = new DisableProtectionAlertScreen();
			disableProtectionAlertScreen.OnModalResult = onModalResult;
			disableProtectionAlertScreen.ModalResultCookie = modalResultCookie;
			Service.ScreenController.AddScreen(disableProtectionAlertScreen);
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			base.OnDestroyElement();
		}

		public void OnViewClockTime(float dt)
		{
			if (this.rightLabel != null)
			{
				this.rightLabel.Text = DisableProtectionAlertScreen.GetProtectionTimeRemaining();
			}
		}

		private static string GetProtectionTimeRemaining()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int protectionTimeRemaining = GameUtils.GetProtectionTimeRemaining();
			if (protectionTimeRemaining > 0)
			{
				stringBuilder.Append(Service.Lang.Get("PROTECTION_REMAINING", new object[0]));
				stringBuilder.Append(GameUtils.GetTimeLabelFromSeconds(protectionTimeRemaining));
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Service.Lang.Get("PROTECTION_INVALIDATE", new object[0]));
			return stringBuilder.ToString();
		}

		protected override void SetupControls()
		{
			base.GetElement<UXLabel>("TickerDialogSmall").Visible = false;
			this.rightLabel.Text = DisableProtectionAlertScreen.GetProtectionTimeRemaining();
			this.titleLabel.Text = this.lang.Get("ALERT", new object[0]);
			this.primary2Option.Text = this.lang.Get("YES", new object[0]);
			this.primary2OptionButton.Visible = true;
			this.primary2OptionButton.Tag = true;
			this.primary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnYesOrNoButtonClicked);
			this.secondary2Option.Text = this.lang.Get("NO", new object[0]);
			this.secondary2OptionButton.Visible = true;
			this.secondary2OptionButton.Tag = null;
			this.secondary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnYesOrNoButtonClicked);
			if (!string.IsNullOrEmpty(this.spriteName))
			{
				UXUtils.SetupGeometryForIcon(this.sprite, this.spriteName);
			}
		}

		private void OnYesOrNoButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.Close(button.Tag);
		}
	}
}
