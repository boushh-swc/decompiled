using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class ScoutPlanetTrigger : AbstractStoryTrigger, IEventObserver
	{
		public ScoutPlanetTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.PlanetScoutingStart && (string.IsNullOrEmpty(this.vo.PrepareString) || this.vo.PrepareString.Equals((string)cookie)))
			{
				Service.EventManager.UnregisterObserver(this, EventId.PlanetScoutingStart);
				this.parent.SatisfyTrigger(this);
			}
			return EatResponse.NotEaten;
		}

		public override void Activate()
		{
			base.Activate();
			Service.EventManager.RegisterObserver(this, EventId.PlanetScoutingStart, EventPriority.Default);
		}
	}
}
