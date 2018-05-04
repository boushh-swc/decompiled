using StaRTS.Main.Models.Player;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class CallsignRUFTask : AbstractRUFTask
	{
		private bool continueProcessing;

		public CallsignRUFTask()
		{
			base.Priority = 30;
			base.ShouldProcess = false;
			base.ShouldPurgeQueue = false;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.CampaignProgress.FueInProgress && currentPlayer.NumIdentities == 1)
			{
				base.ShouldProcess = false;
				return;
			}
			if (currentPlayer.PlayerNameInvalid)
			{
				base.ShouldProcess = true;
			}
		}

		public override void Process(bool continueProcessing)
		{
			this.continueProcessing = continueProcessing;
			if (base.ShouldProcess)
			{
				Service.ViewTimerManager.CreateViewTimer(2f, false, new TimerDelegate(this.ShowCallsignOnTimer), null);
			}
			else
			{
				base.Process(continueProcessing);
			}
		}

		private void ShowCallsignOnTimer(uint timerId, object data)
		{
			SetCallsignScreen setCallsignScreen = new SetCallsignScreen(true);
			setCallsignScreen.OnModalResult = new OnScreenModalResult(this.OnScreenModalResult);
			Service.ScreenController.AddScreen(setCallsignScreen);
		}

		private void OnScreenModalResult(object result, object cookie)
		{
			base.Process(this.continueProcessing);
		}
	}
}
