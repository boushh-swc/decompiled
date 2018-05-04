using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class BlockingWarStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		public BlockingWarStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			Service.EventManager.RegisterObserver(this, EventId.WarLaunchFlow, EventPriority.BeforeDefault);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.WarLaunchFlow)
			{
				return EatResponse.NotEaten;
			}
			this.parent.SatisfyTrigger(this);
			return EatResponse.Eaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.WarLaunchFlow);
			base.Destroy();
		}
	}
}
