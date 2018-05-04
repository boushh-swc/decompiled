using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class WarBoardState : IGameState, IState
	{
		public void OnEnter()
		{
			UXController uXController = Service.UXController;
			uXController.MiscElementsManager.HideEventsTickerView();
			uXController.HUD.ConfigureControls(new HudConfig(new string[]
			{
				"SquadScreen"
			}));
			Service.UXController.HUD.SetSquadScreenAlwaysOnTop(true);
			ScreenController screenController = Service.ScreenController;
			screenController.CloseAll();
			screenController.AddScreen(new SquadWarScreen(), false);
			Service.WarBoardViewController.ShowWarBoard();
		}

		public void OnExit(IState nextState)
		{
			Service.WarBoardViewController.HideWarBoard();
			Service.UXController.HUD.SetSquadScreenVisibility(true);
			Service.UXController.MiscElementsManager.ShowEventsTickerView();
			Service.UXController.HUD.SetSquadScreenAlwaysOnTop(false);
		}

		public bool CanUpdateHomeContracts()
		{
			return true;
		}
	}
}
