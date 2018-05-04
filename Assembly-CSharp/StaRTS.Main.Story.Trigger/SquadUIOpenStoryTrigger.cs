using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class SquadUIOpenStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		public SquadUIOpenStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			if (this.IsSatisfied())
			{
				this.parent.SatisfyTrigger(this);
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.SquadScreenOpenedOrClosed, EventPriority.BeforeDefault);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SquadScreenOpenedOrClosed)
			{
				bool flag = (bool)cookie;
				if (this.IsSatisfied() || flag)
				{
					this.UnregisterObservers();
					this.parent.SatisfyTrigger(this);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			this.UnregisterObservers();
			base.Destroy();
		}

		private void UnregisterObservers()
		{
			Service.EventManager.UnregisterObserver(this, EventId.SquadScreenOpenedOrClosed);
		}

		private bool IsSatisfied()
		{
			bool result = false;
			if (Service.UXController != null)
			{
				HUD hUD = Service.UXController.HUD;
				if (hUD != null)
				{
					result = hUD.IsSquadScreenOpenAndCloseable();
				}
			}
			return result;
		}
	}
}
