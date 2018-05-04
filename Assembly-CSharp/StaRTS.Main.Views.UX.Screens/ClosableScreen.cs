using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ClosableScreen : ScreenBase
	{
		protected const string CLOSE_BUTTON = "BtnClose";

		public UXButton CloseButton;

		protected bool allowClose;

		public bool AllowClose
		{
			get
			{
				return this.allowClose;
			}
			set
			{
				this.allowClose = value;
				if (this.CloseButton != null)
				{
					this.CloseButton.Visible = this.allowClose;
				}
			}
		}

		protected ClosableScreen(string assetName) : base(assetName)
		{
			this.allowClose = true;
			this.InitDefaultBackDelegate();
		}

		public void InitDefaultBackDelegate()
		{
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.HandleClose);
			base.CurrentBackButton = this.CloseButton;
		}

		protected virtual void RefreshScreen()
		{
		}

		public void SetVisibilityAndRefresh(bool visible, bool doRefresh)
		{
			this.Visible = visible;
			if (this.Visible && doRefresh)
			{
				this.RefreshScreen();
			}
		}

		protected virtual void InitButtons()
		{
			this.CloseButton = base.GetElement<UXButton>("BtnClose");
			this.CloseButton.OnClicked = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
			this.CloseButton.Enabled = true;
			base.CurrentBackButton = this.CloseButton;
		}

		protected virtual void HandleClose(UXButton button)
		{
			if (!this.allowClose)
			{
				return;
			}
			this.Close(null);
		}

		protected virtual void OnCloseButtonClicked(UXButton button)
		{
			if (!this.allowClose)
			{
				return;
			}
			if (button != null)
			{
				button.Enabled = false;
			}
			this.HandleClose(button);
		}
	}
}
