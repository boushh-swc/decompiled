using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowWhatsNextScreenStoryAction : AbstractStoryAction, IEventObserver
	{
		public ShowWhatsNextScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			if (Service.ScreenController.GetHighestLevelScreen<WhatsNextFUEScreen>() == null)
			{
				Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
				Service.ScreenController.AddScreen(new WhatsNextFUEScreen());
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				if (cookie is WhatsNextFUEScreen)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
					this.parent.ChildComplete(this);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
