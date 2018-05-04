using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class OpenEpisodeInfoScreenStoryAction : AbstractStoryAction, IEventObserver
	{
		public OpenEpisodeInfoScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			EpisodeInfoScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<EpisodeInfoScreen>();
			if (highestLevelScreen != null && highestLevelScreen.IsLoaded())
			{
				this.parent.ChildComplete(this);
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
				if (highestLevelScreen == null)
				{
					bool flag = Service.EpisodeController.PlayIntroStoryAction();
					if (flag)
					{
						return;
					}
					Service.EpisodeController.AttemptToShowActiveEpisodeInfoScreen();
				}
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				if (cookie is EpisodeInfoScreen)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
					this.parent.ChildComplete(this);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
