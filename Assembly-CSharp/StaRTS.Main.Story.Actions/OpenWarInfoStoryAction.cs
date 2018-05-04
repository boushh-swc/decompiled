using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class OpenWarInfoStoryAction : AbstractStoryAction, IEventObserver
	{
		private const int HELP_STATE_ARG = 0;

		private int pageIndex;

		public OpenWarInfoStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			if (this.prepareArgs.Length == 0)
			{
				this.pageIndex = -1;
			}
			else
			{
				this.pageIndex = Convert.ToInt32(this.prepareArgs[0]);
			}
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			ScreenController screenController = Service.ScreenController;
			SquadWarInfoScreen squadWarInfoScreen = screenController.GetHighestLevelScreen<SquadWarInfoScreen>();
			if (squadWarInfoScreen != null)
			{
				if (squadWarInfoScreen.IsLoaded())
				{
					squadWarInfoScreen.ShowPage(this.pageIndex);
					this.parent.ChildComplete(this);
				}
				else
				{
					Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded);
				}
			}
			else
			{
				squadWarInfoScreen = new SquadWarInfoScreen(this.pageIndex);
				Service.ScreenController.AddScreen(squadWarInfoScreen);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				if (cookie is SquadWarInfoScreen)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
					((SquadWarInfoScreen)cookie).ShowPage(this.pageIndex);
					this.parent.ChildComplete(this);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
