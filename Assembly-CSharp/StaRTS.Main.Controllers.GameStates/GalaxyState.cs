using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class GalaxyState : IGameState, IState
	{
		public void OnEnter()
		{
			Service.UXController.HUD.Visible = false;
			Service.UXController.HUD.SetSquadScreenAlwaysOnTop(true);
			Service.UXController.HUD.SetSquadScreenVisibility(true);
			Action callback = new Action(Service.GalaxyViewController.GoToHome);
			Service.UXController.MiscElementsManager.ShowGalaxyCloseButton(callback);
			Service.UXController.MiscElementsManager.AddGalaxyTournamentStatus();
		}

		public void OnExit(IState nextState)
		{
			Service.UXController.HUD.SetSquadScreenAlwaysOnTop(false);
			Service.UXController.MiscElementsManager.RemoveGalaxyTournamentStatus();
			Service.UXController.MiscElementsManager.HideGalaxyCloseButton();
			Service.UXController.HUD.Visible = true;
		}

		public bool CanUpdateHomeContracts()
		{
			return true;
		}
	}
}
