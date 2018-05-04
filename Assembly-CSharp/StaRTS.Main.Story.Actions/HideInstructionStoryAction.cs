using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class HideInstructionStoryAction : AbstractStoryAction
	{
		public HideInstructionStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(0);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.UXController.MiscElementsManager.HidePlayerInstructions();
			this.parent.ChildComplete(this);
		}
	}
}
