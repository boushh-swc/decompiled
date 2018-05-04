using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class HideHoloInfoPanelStoryAction : AbstractStoryAction
	{
		public HideHoloInfoPanelStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			Service.EventManager.SendEvent(EventId.HideInfoPanel, this);
			this.parent.ChildComplete(this);
		}
	}
}
