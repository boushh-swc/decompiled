using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class FactionChangedStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private const int FACTION_ARG = 0;

		private FactionType faction;

		public FactionChangedStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			this.faction = StringUtils.ParseEnum<FactionType>(this.prepareArgs[0]);
			Service.EventManager.RegisterObserver(this, EventId.ScreenClosing, EventPriority.Default);
		}

		private void RemoveListeners()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ScreenClosing);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenClosing)
			{
				ChooseFactionScreen chooseFactionScreen = cookie as ChooseFactionScreen;
				if (chooseFactionScreen != null)
				{
					FactionType factionType = Service.CurrentPlayer.Faction;
					if (factionType == this.faction)
					{
						this.RemoveListeners();
						this.parent.SatisfyTrigger(this);
					}
				}
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
