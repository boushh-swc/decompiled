using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class SquadSizeStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private const int ARG_MIN_SQUAD_SIZE = 0;

		private const int ARG_MAX_SQUAD_SIZE = 1;

		public SquadSizeStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
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
				Service.EventManager.RegisterObserver(this, EventId.SquadJoinedByCurrentPlayer);
				Service.EventManager.RegisterObserver(this, EventId.SquadUpdated);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SquadJoinedByCurrentPlayer || id == EventId.SquadUpdated)
			{
				if (this.IsSatisfied())
				{
					this.UnregisterObservers();
					this.parent.SatisfyTrigger(this);
				}
			}
			return EatResponse.NotEaten;
		}

		private bool IsSatisfied()
		{
			bool result = false;
			int num = Convert.ToInt32(this.prepareArgs[0]);
			int num2 = Convert.ToInt32(this.prepareArgs[1]);
			SquadController squadController = Service.SquadController;
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				result = false;
			}
			else if (currentSquad.MemberCount >= num && currentSquad.MemberCount <= num2)
			{
				result = true;
			}
			return result;
		}

		public override void Destroy()
		{
			this.UnregisterObservers();
			base.Destroy();
		}

		private void UnregisterObservers()
		{
			Service.EventManager.UnregisterObserver(this, EventId.SquadJoinedByCurrentPlayer);
			Service.EventManager.UnregisterObserver(this, EventId.SquadUpdated);
		}
	}
}
