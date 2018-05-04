using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class EditBaseState : IGameState, IState
	{
		private bool autoLiftSelectedBuilding;

		public EditBaseState(bool autoLiftSelectedBuilding)
		{
			this.autoLiftSelectedBuilding = autoLiftSelectedBuilding;
		}

		public void OnEnter()
		{
			Service.EditBaseController.Enable(true);
			Service.BuildingController.EnterMoveMode(this.autoLiftSelectedBuilding);
			Service.WorldInitializer.View.SetEditModeVantage(true);
			Service.WorldInitializer.View.DrawWorldGrid();
			HudConfig hudConfig = new HudConfig(new string[]
			{
				"PlayerInfo",
				"Currency",
				"Droids",
				"Crystals",
				"ButtonExitEdit",
				"ButtonStore"
			});
			HUD hUD = Service.UXController.HUD;
			if (!GameConstants.DISABLE_BASE_LAYOUT_TOOL && !hUD.BaseLayoutToolView.IsBuildingPendingPurchase)
			{
				hudConfig.Add("BtnActivateStash");
			}
			hUD.ConfigureControls(hudConfig);
			hUD.BaseLayoutToolView.RefreshNewJewelStatus();
			Service.EventManager.SendEvent(EventId.EnterEditMode, null);
			Service.ChampionController.DestroyAllChampionEntities();
		}

		public void OnExit(IState nextState)
		{
			if (!(nextState is BaseLayoutToolState))
			{
				Service.EditBaseController.Enable(false);
				Service.WorldInitializer.View.SetEditModeVantage(false);
				Service.WorldInitializer.View.DestroyWorldGrid();
				Service.BuildingController.ExitAllModes();
			}
			else
			{
				Service.BuildingController.EnsureLoweredLiftedBuilding();
			}
			Service.EventManager.SendEvent(EventId.ExitEditMode, null);
		}

		public bool CanUpdateHomeContracts()
		{
			return true;
		}
	}
}
