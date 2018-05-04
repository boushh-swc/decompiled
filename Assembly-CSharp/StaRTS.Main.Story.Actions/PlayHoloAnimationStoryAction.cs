using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class PlayHoloAnimationStoryAction : AbstractStoryAction
	{
		private const int ANIM_NAME_ARG = 0;

		public string AnimName
		{
			get
			{
				return this.prepareArgs[0];
			}
		}

		public PlayHoloAnimationStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(2);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.EventManager.SendEvent(EventId.PlayHologramAnimation, this);
			this.parent.ChildComplete(this);
		}
	}
}
