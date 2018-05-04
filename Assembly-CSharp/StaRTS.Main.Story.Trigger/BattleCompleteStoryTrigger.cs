using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class BattleCompleteStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		public BattleCompleteStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			Service.EventManager.RegisterObserver(this, EventId.BattleEndFullyProcessed, EventPriority.Default);
		}

		private void RemoveListeners()
		{
			Service.EventManager.UnregisterObserver(this, EventId.BattleEndFullyProcessed);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.BattleEndFullyProcessed)
			{
				this.RemoveListeners();
				this.parent.SatisfyTrigger(this);
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			this.RemoveListeners();
			base.Destroy();
		}
	}
}
