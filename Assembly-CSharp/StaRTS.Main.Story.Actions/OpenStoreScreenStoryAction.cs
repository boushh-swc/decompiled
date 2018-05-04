using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class OpenStoreScreenStoryAction : AbstractStoryAction, IEventObserver
	{
		public OpenStoreScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
			if (highestLevelScreen != null && highestLevelScreen.IsLoaded())
			{
				this.parent.ChildComplete(this);
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
				if (highestLevelScreen == null)
				{
					Service.ScreenController.AddScreen(new StoreScreen());
				}
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				if (cookie is StoreScreen)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
					this.parent.ChildComplete(this);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
