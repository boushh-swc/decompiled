using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class YesNoScreen : AlertScreen
	{
		private bool textOnRight;

		private static string yesString = string.Empty;

		private static string noString = string.Empty;

		public bool useKoreanFont;

		private YesNoScreen(string title, string message, bool textOnRight, bool alwaysOnTop) : base(false, title, message, null, false)
		{
			this.textOnRight = textOnRight;
			base.IsAlwaysOnTop = alwaysOnTop;
		}

		public static void StaticReset()
		{
			YesNoScreen.yesString = string.Empty;
			YesNoScreen.noString = string.Empty;
		}

		public static void ShowModal(string title, string message, bool textOnRight, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			YesNoScreen.ShowModal(title, message, textOnRight, false, false, onModalResult, modalResultCookie);
		}

		public static void ShowModal(string title, string message, bool textOnRight, bool allowFUEBackButton, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			YesNoScreen.yesString = string.Empty;
			YesNoScreen.noString = string.Empty;
			YesNoScreen.CommonShowModal(title, message, textOnRight, false, allowFUEBackButton, false, onModalResult, modalResultCookie);
		}

		public static void ShowModal(string title, string message, bool textOnRight, bool centerTitle, bool allowFUEBackButton, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			YesNoScreen.yesString = string.Empty;
			YesNoScreen.noString = string.Empty;
			YesNoScreen.CommonShowModal(title, message, textOnRight, centerTitle, allowFUEBackButton, false, onModalResult, modalResultCookie);
		}

		public static void ShowModal(string title, string message, bool textOnRight, bool centerTitle, bool allowFUEBackButton, bool alwaysOnTop, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			YesNoScreen.yesString = string.Empty;
			YesNoScreen.noString = string.Empty;
			YesNoScreen.CommonShowModal(title, message, textOnRight, centerTitle, allowFUEBackButton, alwaysOnTop, onModalResult, modalResultCookie);
		}

		public static void ShowModal(string title, string message, bool textOnRight, string confirmString, string gobackString, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			YesNoScreen.yesString = confirmString;
			YesNoScreen.noString = gobackString;
			YesNoScreen.CommonShowModal(title, message, textOnRight, false, false, false, onModalResult, modalResultCookie);
		}

		private static void CommonShowModal(string title, string message, bool textOnRight, bool centerTitle, bool allowFUEBackButton, bool alwaysOnTop, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			YesNoScreen yesNoScreen = new YesNoScreen(title, message, textOnRight, alwaysOnTop);
			yesNoScreen.centerTitle = centerTitle;
			yesNoScreen.OnModalResult = onModalResult;
			yesNoScreen.ModalResultCookie = modalResultCookie;
			yesNoScreen.AllowFUEBackButton = allowFUEBackButton;
			Service.ScreenController.AddScreen(yesNoScreen);
		}

		protected override void SetupControls()
		{
			base.GetElement<UXLabel>("TickerDialogSmall").Visible = false;
			this.primary2OptionButton.Visible = true;
			this.primary2OptionButton.Tag = true;
			this.primary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnYesOrNoButtonClicked);
			this.secondary2OptionButton.Visible = true;
			this.secondary2OptionButton.Tag = null;
			this.secondary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnYesOrNoButtonClicked);
			if (!string.IsNullOrEmpty(YesNoScreen.yesString))
			{
				this.primary2Option.Text = YesNoScreen.yesString;
			}
			else
			{
				this.primary2Option.Text = this.lang.Get("YES", new object[0]);
			}
			if (!string.IsNullOrEmpty(YesNoScreen.noString))
			{
				this.secondary2Option.Text = YesNoScreen.noString;
			}
			else
			{
				this.secondary2Option.Text = this.lang.Get("NO", new object[0]);
			}
			this.titleLabel.Text = this.title;
			if (this.textOnRight)
			{
				this.rightLabel.Text = this.message;
				if (this.useKoreanFont)
				{
					this.rightLabel.Font = Service.Lang.CustomKoreanFont;
				}
			}
			else
			{
				this.centerLabel.Text = this.message;
				if (this.useKoreanFont)
				{
					this.centerLabel.Font = Service.Lang.CustomKoreanFont;
				}
			}
			if (!base.IsFatal && Service.UserInputInhibitor != null)
			{
				Service.UserInputInhibitor.AddToAllow(this.primary2OptionButton);
				Service.UserInputInhibitor.AddToAllow(this.secondary2OptionButton);
			}
		}

		private void OnYesOrNoButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.Close(button.Tag);
		}
	}
}
