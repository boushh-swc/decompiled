using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class MissionVictoryStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private const int MISSION_UID_ARG = 0;

		public MissionVictoryStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			Service.EventManager.RegisterObserver(this, EventId.VictoryConditionSuccess, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.VictoryConditionSuccess)
			{
				ConditionVO conditionVO = (ConditionVO)cookie;
				if (conditionVO.Uid.ToLower() == this.prepareArgs[0].ToLower())
				{
					this.parent.SatisfyTrigger(this);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.VictoryConditionSuccess);
			base.Destroy();
		}
	}
}
