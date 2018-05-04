using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class WorldTransitionCompleteStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		public WorldTransitionCompleteStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			Service.EventManager.RegisterObserver(this, EventId.WorldInTransitionComplete, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.WorldInTransitionComplete)
			{
				this.parent.SatisfyTrigger(this);
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.WorldInTransitionComplete);
			base.Destroy();
		}
	}
}
