using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class DegenerateStoryAction : AbstractStoryAction
	{
		public DegenerateStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			this.parent.ChildComplete(this);
		}
	}
}
