using StaRTS.Main.Story.Trigger;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class AutoTriggerRUFTask : AbstractRUFTask
	{
		private AutoStoryTrigger trigger;

		public AutoTriggerRUFTask(AutoStoryTrigger trigger)
		{
			this.trigger = trigger;
			base.Priority = 40;
			base.ShouldProcess = trigger.IsPreSatisfied();
			base.ShouldPlayFromLoadState = true;
			base.ShouldPurgeQueue = false;
		}

		public override void Process(bool continueProcessing)
		{
			if (base.ShouldProcess)
			{
				Service.QuestController.ActivateTrigger(this.trigger, false);
			}
			base.Process(continueProcessing);
		}
	}
}
