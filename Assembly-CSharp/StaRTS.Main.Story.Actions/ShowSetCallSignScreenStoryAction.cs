using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowSetCallSignScreenStoryAction : AbstractStoryAction, IEventObserver
	{
		private bool doBackendAuth;

		public ShowSetCallSignScreenStoryAction(bool doBackendAuth, StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
			this.doBackendAuth = doBackendAuth;
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
			if (Service.ScreenController.GetHighestLevelScreen<SetCallsignScreen>() == null)
			{
				Service.ScreenController.AddScreen(new SetCallsignScreen(this.doBackendAuth));
			}
			else
			{
				this.parent.ChildComplete(this);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				if (cookie is SetCallsignScreen)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
					this.parent.ChildComplete(this);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
