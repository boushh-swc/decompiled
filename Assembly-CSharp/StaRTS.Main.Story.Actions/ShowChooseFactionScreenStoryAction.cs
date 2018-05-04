using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowChooseFactionScreenStoryAction : AbstractStoryAction, IEventObserver
	{
		public ShowChooseFactionScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
			Service.ScreenController.AddScreen(new ChooseFactionScreen());
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				if (cookie is ChooseFactionScreen)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
					this.parent.ChildComplete(this);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
