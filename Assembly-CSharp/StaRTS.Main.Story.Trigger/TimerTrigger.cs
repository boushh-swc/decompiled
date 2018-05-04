using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class TimerTrigger : AbstractStoryTrigger
	{
		private const int TIME_SECONDS_ARG = 0;

		private uint timerId;

		public TimerTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			float delay = Convert.ToSingle(this.prepareArgs[0]);
			this.timerId = Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.OnComplete), null);
		}

		public override void Destroy()
		{
			Service.ViewTimerManager.KillViewTimer(this.timerId);
			base.Destroy();
		}

		private void OnComplete(uint id, object cookie)
		{
			this.parent.SatisfyTrigger(this);
		}
	}
}
