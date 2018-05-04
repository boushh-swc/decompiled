using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class DeactivateTriggerStoryAction : AbstractStoryAction
	{
		private const int TRIGGER_UID_ARG = 0;

		public DeactivateTriggerStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			string uid = this.prepareArgs[0];
			Service.QuestController.KillTrigger(uid);
			this.parent.ChildComplete(this);
		}
	}
}
