using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class UpdateClientScreen : AlertScreen
	{
		private UpdateClientScreen(string title, string message) : base(true, title, message, null, false)
		{
		}

		public static void ShowModal(string title, string message, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			UpdateClientScreen updateClientScreen = new UpdateClientScreen(title, message);
			updateClientScreen.OnModalResult = onModalResult;
			updateClientScreen.ModalResultCookie = modalResultCookie;
		}

		protected override void SetupControls()
		{
			base.SetupControls();
			this.primaryLabel.Text = this.lang.Get("FORCED_UPDATE_BUTTON", new object[0]);
		}
	}
}
