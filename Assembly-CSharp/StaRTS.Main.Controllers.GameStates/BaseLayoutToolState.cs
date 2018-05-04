using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class BaseLayoutToolState : IGameState, IState
	{
		private const string STRING_INFO_TITLE = "blt_popup_title";

		private const string STRING_INFO_BODY = "blt_popup_body";

		private const string SKIP_CONFIRMATION = "SKIP_FUTURE_CONFIRMATION";

		public void OnEnter()
		{
			HUDBaseLayoutToolView baseLayoutToolView = Service.UXController.HUD.BaseLayoutToolView;
			baseLayoutToolView.ConfigureBaseLayoutToolStateHUD();
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			baseLayoutToolController.EnterBaseLayoutTool();
			baseLayoutToolController.PauseContractsOnAllBuildings();
			baseLayoutToolController.UpdateLastSavedMap();
			Service.DroidController.HideAllNonClearableDroids();
			string pref = Service.SharedPlayerPrefs.GetPref<string>("SkipBLTIntro");
			if (pref != "1")
			{
				Lang lang = Service.Lang;
				string title = lang.Get("blt_popup_title", new object[0]);
				string message = lang.Get("blt_popup_body", new object[0]);
				AlertWithCheckBoxScreen screen = new AlertWithCheckBoxScreen(title, message, "SKIP_FUTURE_CONFIRMATION", new AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult(this.OnInfoPopupClosed));
				Service.ScreenController.AddScreen(screen);
			}
			baseLayoutToolView.RefreshStashModeCheckBox();
			baseLayoutToolView.RefreshSaveLayoutButtonStatus();
			this.SaveBLTSeenSharedPref();
			Service.ChampionController.DestroyAllChampionEntities();
		}

		public void OnExit(IState nextState)
		{
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			if (baseLayoutToolController.ShouldRevertMap)
			{
				Service.BuildingController.EnsureLoweredLiftedBuilding();
				baseLayoutToolController.RevertToPreviousMapLayout();
			}
			baseLayoutToolController.ClearStashedBuildings();
			Service.UXController.HUD.BaseLayoutToolView.ClearStashedBuildingTray();
			baseLayoutToolController.ResumeContractsOnAllBuildings();
			Service.DroidController.ShowAllDroids();
			Service.EditBaseController.Enable(false);
			Service.WorldInitializer.View.SetEditModeVantage(false);
			Service.WorldInitializer.View.DestroyWorldGrid();
			Service.BuildingController.ExitAllModes();
			baseLayoutToolController.ExitBaseLayoutTool();
			Service.EventManager.SendEvent(EventId.ExitBaseLayoutToolMode, null);
		}

		private void OnInfoPopupClosed(object result, bool selected)
		{
			if (result != null && selected)
			{
				Service.SharedPlayerPrefs.SetPref("SkipBLTIntro", "1");
			}
		}

		private void SaveBLTSeenSharedPref()
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			string pref = sharedPlayerPrefs.GetPref<string>("BLTSeen");
			if (pref != "1")
			{
				sharedPlayerPrefs.SetPref("BLTSeen", "1");
			}
		}

		public bool CanUpdateHomeContracts()
		{
			return true;
		}
	}
}
