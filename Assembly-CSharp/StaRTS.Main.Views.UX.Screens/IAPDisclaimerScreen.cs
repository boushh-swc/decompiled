using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class IAPDisclaimerScreen : AlertScreen
	{
		public IAPDisclaimerScreen(OnScreenModalResult onModalResult) : base(true, null, null, null, false)
		{
			base.OnModalResult = onModalResult;
			this.title = this.lang.Get("IAP_DISCLAIMER_TITLE", new object[0]);
			this.message = this.lang.Get("IAP_DISCLAIMER_DESC", new object[0]);
		}

		protected override void SetupControls()
		{
			base.SetupControls();
			this.primaryButton.OnClicked = new UXButtonClickedDelegate(this.OnButtonClicked);
			this.primaryLabel.Text = this.lang.Get("OK", new object[0]);
			Service.EventManager.SendEvent(EventId.UIIAPDisclaimerViewed, null);
		}

		private void OnButtonClicked(UXButton button)
		{
			this.Close(true);
			Service.EventManager.SendEvent(EventId.UIIAPDisclaimerClosed, null);
		}
	}
}
