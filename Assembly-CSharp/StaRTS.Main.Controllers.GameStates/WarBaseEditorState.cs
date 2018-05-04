using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Commands.Squads;
using StaRTS.Main.Models.Commands.Squads.Responses;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class WarBaseEditorState : IGameState, IState
	{
		private Map warBaseMap;

		public WarBaseEditorState()
		{
			HudConfig config = new HudConfig(new string[0]);
			HUD hUD = Service.UXController.HUD;
			hUD.ConfigureControls(config);
		}

		public static void GoToWarBaseEditorState()
		{
			WarBaseEditorState warBaseEditorState = new WarBaseEditorState();
			warBaseEditorState.StartTransitionToWarBase();
		}

		private void StartTransitionToWarBase()
		{
			string warPlanetUid = Service.SquadController.WarManager.GetCurrentWarScheduleData().WarPlanetUid;
			Service.WorldTransitioner.StartWipe(new WarboardToWarbaseTransition(null, new UserWarBaseMapDataLoader(), null, false, false), Service.StaticDataController.Get<PlanetVO>(warPlanetUid));
			this.QueryBaseData();
		}

		private void QueryBaseData()
		{
			PlayerIdChecksumRequest request = new PlayerIdChecksumRequest();
			GetSquadMemberSyncedWarDataCommand getSquadMemberSyncedWarDataCommand = new GetSquadMemberSyncedWarDataCommand(request);
			getSquadMemberSyncedWarDataCommand.AddSuccessCallback(new AbstractCommand<PlayerIdChecksumRequest, SquadMemberWarDataResponse>.OnSuccessCallback(this.OnWarBaseDataSuccess));
			Service.ServerAPI.Sync(getSquadMemberSyncedWarDataCommand);
			SquadController squadController = Service.SquadController;
			string warId = squadController.WarManager.CurrentSquadWar.WarId;
			string squadID = squadController.StateManager.GetCurrentSquad().SquadID;
			Service.BILoggingController.TrackGameAction(warId, "edit_warbase", squadID, null);
			Service.UXController.MiscElementsManager.HideEventsTickerView();
		}

		private void SetupBaseEditing()
		{
			Service.EditBaseController.Enable(true);
			Service.BuildingController.EnterMoveMode(false);
			Service.WorldInitializer.View.SetEditModeVantage(true);
			Service.WorldInitializer.View.DrawWorldGrid();
			Service.EventManager.SendEvent(EventId.EnterEditMode, null);
			Service.ChampionController.DestroyAllChampionEntities();
		}

		private void SetupEditingMode()
		{
			this.SetupBaseEditing();
			HUDBaseLayoutToolView baseLayoutToolView = Service.UXController.HUD.BaseLayoutToolView;
			baseLayoutToolView.ConfigureBaseLayoutToolStateHUD();
			baseLayoutToolView.RegisterObservers();
			WarBaseEditController warBaseEditController = Service.WarBaseEditController;
			warBaseEditController.EnterWarBaseEditing(this.warBaseMap);
			this.warBaseMap = null;
			Service.UXController.MiscElementsManager.AddSquadWarTickerStatus();
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			baseLayoutToolController.UpdateLastSavedMap();
			Service.DroidController.HideAllNonClearableDroids();
			baseLayoutToolView.RefreshStashModeCheckBox();
			baseLayoutToolView.RefreshSaveLayoutButtonStatus();
			Service.ChampionController.DestroyAllChampionEntities();
			warBaseEditController.CheckForNewBuildings();
		}

		private void OnWarBaseDataSuccess(SquadMemberWarDataResponse response, object cookie)
		{
			this.warBaseMap = response.MemberWarData.BaseMap;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			UserWarBaseMapDataLoader userWarBaseMapDataLoader = new UserWarBaseMapDataLoader();
			userWarBaseMapDataLoader.Initialize(this.warBaseMap, currentPlayer.PlayerName, currentPlayer.Faction);
			Service.WorldTransitioner.ContinueWipe(this, userWarBaseMapDataLoader, new TransitionCompleteDelegate(this.OnTransitionComplete));
		}

		private void OnTransitionComplete()
		{
			this.SetupEditingMode();
		}

		private void CleanupBaseEditing()
		{
			Service.EditBaseController.Enable(false);
			Service.WorldInitializer.View.SetEditModeVantage(false);
			Service.WorldInitializer.View.DestroyWorldGrid();
			Service.BuildingController.ExitAllModes();
			Service.EventManager.SendEvent(EventId.ExitEditMode, null);
		}

		public void OnEnter()
		{
		}

		public void OnExit(IState nextState)
		{
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			baseLayoutToolController.ClearStashedBuildings();
			HUDBaseLayoutToolView baseLayoutToolView = Service.UXController.HUD.BaseLayoutToolView;
			baseLayoutToolView.ClearStashedBuildingTray();
			baseLayoutToolView.UnregisterObservers();
			WarBaseEditController warBaseEditController = Service.WarBaseEditController;
			warBaseEditController.ExitWarBaseEditing();
			this.CleanupBaseEditing();
			Service.UXController.MiscElementsManager.HideEventsTickerView();
		}

		public bool CanUpdateHomeContracts()
		{
			return false;
		}
	}
}
