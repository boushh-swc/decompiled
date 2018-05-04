using StaRTS.Main.Controllers;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story
{
	public class SessionStartTriggerParent : ITriggerReactor
	{
		private List<IStoryTrigger> triggerBuffer;

		private bool buffering;

		public SessionStartTriggerParent()
		{
			this.triggerBuffer = new List<IStoryTrigger>();
			this.buffering = true;
		}

		public void SatisfyTrigger(IStoryTrigger trigger)
		{
			if (this.buffering)
			{
				this.triggerBuffer.Add(trigger);
			}
			else
			{
				Service.QuestController.SatisfyTrigger(trigger);
			}
		}

		public void ReleaseSatisfiedTriggers()
		{
			this.buffering = false;
			QuestController questController = Service.QuestController;
			int i = 0;
			int count = this.triggerBuffer.Count;
			while (i < count)
			{
				questController.SatisfyTrigger(this.triggerBuffer[i]);
				i++;
			}
			this.triggerBuffer.Clear();
		}

		public void KillAllTriggers()
		{
			this.buffering = false;
			this.triggerBuffer.Clear();
		}
	}
}
