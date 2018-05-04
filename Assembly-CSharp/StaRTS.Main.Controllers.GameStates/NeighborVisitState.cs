using StaRTS.Main.Models;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class NeighborVisitState : IGameState, IState
	{
		public void OnEnter()
		{
			Service.UXController.HUD.Visible = true;
			Service.UXController.HUD.ConfigureControls(new HudConfig(new string[]
			{
				"FriendInfo",
				"ButtonHome"
			}));
			Service.BuildingController.EnterSelectMode();
		}

		public void OnExit(IState nextState)
		{
		}

		public bool CanUpdateHomeContracts()
		{
			return false;
		}
	}
}
