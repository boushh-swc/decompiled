using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class DelayStoryAction : AbstractStoryAction
	{
		private const int TIME_SECONDS_ARG = 0;

		public DelayStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			float delay = Convert.ToSingle(this.prepareArgs[0]);
			Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.OnComplete), null);
		}

		private void OnComplete(uint id, object cookie)
		{
			this.parent.ChildComplete(this);
		}
	}
}
