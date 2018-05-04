using StaRTS.Main.Controllers;
using StaRTS.Main.Views.UX.Screens.Leaderboard;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public static class ScreenUtils
	{
		public static ClosableScreen GetDeployableInfoParentScreen()
		{
			ScreenController screenController = Service.ScreenController;
			ClosableScreen highestLevelScreen = screenController.GetHighestLevelScreen<BuildingInfoScreen>();
			if (highestLevelScreen != null)
			{
				return highestLevelScreen;
			}
			highestLevelScreen = screenController.GetHighestLevelScreen<TroopTrainingScreen>();
			if (highestLevelScreen != null)
			{
				return highestLevelScreen;
			}
			highestLevelScreen = screenController.GetHighestLevelScreen<TroopUpgradeScreen>();
			if (highestLevelScreen != null)
			{
				return highestLevelScreen;
			}
			highestLevelScreen = screenController.GetHighestLevelScreen<PrizeInventoryScreen>();
			if (highestLevelScreen != null)
			{
				return highestLevelScreen;
			}
			return null;
		}

		public static bool IsAnySquadScreenOpen()
		{
			ScreenController screenController = Service.ScreenController;
			return screenController.GetHighestLevelScreen<SquadBuildingScreen>() != null || screenController.GetHighestLevelScreen<SquadCreateScreen>() != null || screenController.GetHighestLevelScreen<SquadIntroScreen>() != null || screenController.GetHighestLevelScreen<SquadJoinRequestScreen>() != null || screenController.GetHighestLevelScreen<SquadJoinScreen>() != null || screenController.GetHighestLevelScreen<AbstractSquadRequestScreen>() != null || screenController.GetHighestLevelScreen<SquadSlidingScreen>() != null || screenController.GetHighestLevelScreen<SquadTroopRequestScreen>() != null || screenController.GetHighestLevelScreen<SquadUpgradeScreen>() != null;
		}
	}
}
