using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class StoreLookupStoryAction : AbstractStoryAction, IEventObserver
	{
		private const int TAB_ARG = 0;

		private const int ITEM_ARG = 1;

		private StoreTab tab;

		public StoreLookupStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(2);
			this.tab = StringUtils.ParseEnum<StoreTab>(this.prepareArgs[0]);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
			if (highestLevelScreen != null && highestLevelScreen.IsLoaded())
			{
				Service.EventManager.RegisterObserver(this, EventId.StoreScreenReady, EventPriority.Default);
				this.PerformStoreLookup(highestLevelScreen);
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.StoreScreenReady)
			{
				if (id == EventId.ScreenLoaded)
				{
					if (cookie is StoreScreen)
					{
						Service.EventManager.RegisterObserver(this, EventId.StoreScreenReady, EventPriority.Default);
						this.PerformStoreLookup(cookie as StoreScreen);
					}
				}
			}
			else
			{
				this.RemoveListeners();
				this.parent.ChildComplete(this);
			}
			return EatResponse.NotEaten;
		}

		private void PerformStoreLookup(StoreScreen store)
		{
			store.OpenStoreTab(this.tab);
			store.ScrollToItem(this.prepareArgs[1]);
			store.EnableScrollListMovement(false);
		}

		private void RemoveListeners()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
			Service.EventManager.UnregisterObserver(this, EventId.StoreScreenReady);
		}
	}
}
