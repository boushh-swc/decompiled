using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowInstructionStoryAction : AbstractStoryAction
	{
		private const int INSTRUCTION_KEY_ARG = 0;

		private const int DELAY_KEY_ARG = 1;

		private const int DURATION_KEY_ARG = 2;

		private string msg;

		private float delay;

		private float duration;

		public ShowInstructionStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(new int[]
			{
				1,
				3
			});
			this.msg = Service.Lang.Get(this.prepareArgs[0], new object[0]);
			if (this.prepareArgs.Length > 1)
			{
				float.TryParse(this.prepareArgs[1], out this.delay);
				float.TryParse(this.prepareArgs[2], out this.duration);
			}
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.UXController.MiscElementsManager.ShowPlayerInstructions(this.msg, this.delay, this.duration);
			this.parent.ChildComplete(this);
		}
	}
}
