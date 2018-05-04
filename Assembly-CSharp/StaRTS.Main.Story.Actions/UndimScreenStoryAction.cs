using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class UndimScreenStoryAction : AbstractStoryAction
	{
		public UndimScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			this.parent.ChildComplete(this);
		}
	}
}
