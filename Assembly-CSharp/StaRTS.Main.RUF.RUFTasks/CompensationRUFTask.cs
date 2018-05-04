using StaRTS.Main.Views.UX;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class CompensationRUFTask : AbstractRUFTask
	{
		private bool continueProcessing;

		public CompensationRUFTask()
		{
			base.Priority = 110;
			base.ShouldProcess = true;
		}

		public override void Process(bool continueProcessing)
		{
			this.continueProcessing = continueProcessing;
			if (base.ShouldProcess)
			{
				Service.ViewTimerManager.CreateViewTimer(2f, false, new TimerDelegate(this.ShowCompensationOnTimer), null);
			}
		}

		public void ShowCompensationOnTimer(uint timerId, object data)
		{
			if (!Service.PerkManager.WillShowPerkTutorial())
			{
				UXUtils.ShowCompensationMessage();
			}
			base.Process(this.continueProcessing);
		}
	}
}
