using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class CloseScreenStoryAction : AbstractStoryAction
	{
		private const string PLANET_DETAILS_SCREEN = "PlanetDetailsScreen";

		private const string SQUAD_SCREEN_OPEN = "";

		public CloseScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			ScreenController screenController = Service.ScreenController;
			UXController uXController = Service.UXController;
			ScreenBase highestLevelScreen = screenController.GetHighestLevelScreen<ClosableScreen>();
			if (highestLevelScreen != null)
			{
				string name = highestLevelScreen.GetType().Name;
				if (name == "PlanetDetailsScreen")
				{
					if (!Service.BuildingLookupController.HasNavigationCenter())
					{
						Service.GalaxyViewController.GoToHome();
					}
					else
					{
						Service.GalaxyViewController.TranstionPlanetToGalaxy();
					}
					highestLevelScreen.Close(null);
				}
				else
				{
					highestLevelScreen.Close(null);
				}
			}
			if (uXController.HUD.IsSquadScreenOpenAndCloseable())
			{
				SquadController squadController = Service.SquadController;
				if (squadController.StateManager.SquadScreenState == SquadScreenState.Donation)
				{
					SquadSlidingScreen highestLevelScreen2 = screenController.GetHighestLevelScreen<SquadSlidingScreen>();
					highestLevelScreen2.CloseDonationView();
				}
				else
				{
					uXController.HUD.SlideSquadScreenClosed();
				}
			}
			this.parent.ChildComplete(this);
		}
	}
}
