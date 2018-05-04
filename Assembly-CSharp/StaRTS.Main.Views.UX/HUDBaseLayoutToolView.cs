using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX
{
	public class HUDBaseLayoutToolView : IEventObserver
	{
		private const string BUTTON_CANCEL = "BtnCancelStash";

		public const string BUTTON_ENTER_BLT = "BtnActivateStash";

		private const string BUTTON_SAVE = "BtnSaveLayout";

		private const string BUTTON_STASH_ALL = "BtnStashAll";

		private const string BUTTON_STASH_MODE = "BtnStashMode";

		private const string BUTTON_INFO_WARBE = "BtnInfoWarBE";

		private const string ELEMENT_STASH_PANEL = "StashPanelHolder";

		private const string ELEMENT_JEWEL_CONTAINER = "ContainerJewelBattleActivateStash";

		private const string SPRITE_STASH_TRAY = "SpriteStashTray";

		private const string SPRITE_STASH_MODE_CHECK = "SpriteCheckStashMode";

		private const string LABEL_BLT_TOP_LEFT = "LabelTitleTopLeft";

		private const string LABEL_BTN_ENTER_BLT = "LabelActivateStash";

		private const string LABEL_BTN_CANCEL = "LabelBtnCancelStash";

		private const string LABEL_BTN_SAVE = "LabelBtnSaveLayout";

		private const string LABEL_BTN_STASH_ALL = "LabelBtnStashAll";

		private const string LABEL_BTN_STASH_MODE = "LabelStashMode";

		private const string LABEL_JEWEL_NEW = "LabelMessageCountBattleActivateStash";

		private const string LABEL_STASH_INSTRUCTIONS = "LabelStashInstructions";

		private const string LABEL_SELECTED_BUILDING_DESC = "LabelContextDescriptionStash";

		private const string STASHED_BUILDING_CHECKBOX = "CheckboxStash";

		private const string STASHED_BUILDINGS_GRID = "StashGrid";

		private const string STASHED_BUILDING_LABEL_LEVEL = "LabelBldgLevelStash";

		private const string STASHED_BUILDING_LABEL_COUNT = "LabelQuantityStash";

		private const string STASHED_BUILDING_SPRITE = "SpriteBldgStash";

		private const string STASHED_BUILDING_TEMPLATE = "StashTemplate";

		private const string STRING_BLT_BTN = "blt_btn";

		private const string STRING_BLT_TITLE = "blt_title";

		private const string STRING_WBE_TITLE = "WAR_BASE_LAYOUT_TITLE";

		private const string STRING_BLT_STASH_INSTRUCTIONS = "blt_tray_instructions";

		private const string STRING_BLT_QUICKSTASH_ON_INSTRUCTIONS = "blt_tray_quickmode_on";

		private const string STRING_BTN_CANCEL = "btn_Cancel";

		private const string STRING_BTN_SAVE = "btn_Save";

		private const string STRING_BTN_STASH_ALL = "blt_stash_all_btn";

		private const string STRING_BTN_STASH_ONE = "blt_stash_one_btn";

		private const string STRING_BTN_STASH_MODE = "blt_quick_stash";

		private const string STRING_BTN_STASH_MODE_OFF = "blt_stash_toggle_off_btn";

		private const string STRING_BTN_STASH_MODE_ON = "blt_stash_toggle_on_btn";

		private const string STRING_JEWEL_NEW = "s_New";

		private const string TABLE_STASHMODE_BUTTON = "StashModeBtnTable";

		private const string STRING_CANCEL_TITLE = "blt_cancel_title";

		private const string STRING_CANCEL_BODY = "blt_cancel_body";

		private const string SKIP_CONFIRMATION = "SKIP_FUTURE_CONFIRMATION";

		private const string STRING_SAVE_TITLE = "blt_confirm_title";

		private const string STRING_SAVE_BODY = "blt_confirm_body";

		private const string STRING_LAYOUT_INCOMPLETE_TITLE = "blt_unfinished_base_title";

		private const string STRING_LAYOUT_INCOMPLETE_DESC = "blt_unfinished_base_body";

		private const string STRING_WARBE_FORCE_EXIT_TITLE = "WAR_BASE_PREP_END_TITLE";

		private const string STRING_WARBE_FORCE_EXIT_TEXT = "WAR_BASE_PREP_END_TEXT";

		private const string STRING_WARBE_FORCE_EXIT_CTA = "WAR_BASE_PREP_END_CTA";

		private HUD hud;

		private UXElement stashPanel;

		private UXSprite stashTray;

		private UXSprite stashModeCheck;

		private UXButton cancelButton;

		private UXButton enterBltButton;

		private UXButton infoWarBEButton;

		private UXButton saveLayoutButton;

		private UXButton stashAllButton;

		private UXButton toggleStashModeButton;

		private UXTable stashModeTable;

		private UXGrid stashedBuildingsGrid;

		private UXElement stashedBuildingTemplate;

		private UXElement newJewelContainer;

		private UXLabel bltTitleLabel;

		private UXLabel stashInstructionsLabel;

		private float lastScrollPosition;

		private string addedBuildingCardUID;

		public UXLabel SelectedBuildingLabel
		{
			get;
			private set;
		}

		public bool IsBuildingPendingPurchase
		{
			get;
			private set;
		}

		public HUDBaseLayoutToolView(HUD hud)
		{
			this.hud = hud;
			this.lastScrollPosition = 0f;
			this.addedBuildingCardUID = null;
		}

		public void Initialize()
		{
			this.stashPanel = this.hud.GetElement<UXElement>("StashPanelHolder");
			this.stashTray = this.hud.GetElement<UXSprite>("SpriteStashTray");
			this.newJewelContainer = this.hud.GetElement<UXElement>("ContainerJewelBattleActivateStash");
			this.InitLabels();
			this.InitButtons();
			this.InitGrid();
		}

		public void RegisterObservers()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.BuildingDeselected);
			eventManager.RegisterObserver(this, EventId.BuildingSelected);
			eventManager.RegisterObserver(this, EventId.BuildingPurchaseModeEnded);
			eventManager.RegisterObserver(this, EventId.BuildingPurchaseModeStarted);
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				eventManager.RegisterObserver(this, EventId.WarPhaseChanged, EventPriority.AfterDefault);
			}
		}

		public void UnregisterObservers()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.BuildingDeselected);
			eventManager.UnregisterObserver(this, EventId.BuildingSelected);
			eventManager.UnregisterObserver(this, EventId.WarPhaseChanged);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			bool flag = false;
			if (Service.BaseLayoutToolController != null)
			{
				flag = Service.BaseLayoutToolController.IsBaseLayoutModeActive;
			}
			switch (id)
			{
			case EventId.BuildingPurchaseModeStarted:
				this.IsBuildingPendingPurchase = true;
				this.hud.CurrentHudConfig.Remove("BtnActivateStash");
				this.hud.RefreshView();
				return EatResponse.NotEaten;
			case EventId.BuildingPurchaseModeEnded:
				this.IsBuildingPendingPurchase = false;
				if (!GameConstants.DISABLE_BASE_LAYOUT_TOOL)
				{
					this.hud.CurrentHudConfig.Add("BtnActivateStash");
					this.hud.RefreshView();
				}
				return EatResponse.NotEaten;
			case EventId.BuildingStartedUpgrading:
			case EventId.BuildingSelectedFromStore:
			case EventId.BuildingSelectedSound:
			{
				IL_3C:
				if (id != EventId.WarPhaseChanged)
				{
					return EatResponse.NotEaten;
				}
				if (!(Service.GameStateMachine.CurrentState is WarBaseEditorState))
				{
					Service.Logger.Warn("Not in War Base Editor when responding to Squad War phase change");
					return EatResponse.NotEaten;
				}
				SquadWarStatusType squadWarStatusType = (SquadWarStatusType)cookie;
				if (squadWarStatusType != SquadWarStatusType.PhasePrep)
				{
					this.ShowForceExitAlert();
				}
				return EatResponse.NotEaten;
			}
			case EventId.BuildingSelected:
				if (!flag)
				{
					return EatResponse.NotEaten;
				}
				this.RefreshInstructionLabel();
				this.RefreshCurrencyTray(true);
				return EatResponse.NotEaten;
			case EventId.BuildingDeselected:
				if (!flag)
				{
					return EatResponse.NotEaten;
				}
				this.RefreshInstructionLabel();
				this.RefreshCurrencyTray(false);
				return EatResponse.NotEaten;
			}
			goto IL_3C;
		}

		private void ShowForceExitAlert()
		{
			string title = Service.Lang.Get("WAR_BASE_PREP_END_TITLE", new object[0]);
			string message = Service.Lang.Get("WAR_BASE_PREP_END_TEXT", new object[0]);
			string primaryLabelText = Service.Lang.Get("WAR_BASE_PREP_END_CTA", new object[0]);
			AlertScreen alertScreen = AlertScreen.ShowModal(false, title, message, null, new OnScreenModalResult(this.OnForceExitScreenClosed), null, true, false, null, true);
			alertScreen.SetPrimaryLabelText(primaryLabelText);
		}

		private void OnForceExitScreenClosed(object result, object cookie)
		{
			this.ExitBaseLayoutTool(false);
		}

		public void ConfigureBaseLayoutToolStateHUD()
		{
			HudConfig hudConfig = new HudConfig(new string[]
			{
				"BtnCancelStash",
				"BtnSaveLayout",
				"BtnStashAll",
				"BtnStashMode",
				"StashPanelHolder",
				"SpriteStashTray",
				"LabelTitleTopLeft"
			});
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				this.bltTitleLabel.Text = Service.Lang.Get("WAR_BASE_LAYOUT_TITLE", new object[0]);
				hudConfig.Add("BtnInfoWarBE");
			}
			else
			{
				this.bltTitleLabel.Text = Service.Lang.Get("blt_title", new object[0]);
			}
			this.hud.ConfigureControls(hudConfig);
		}

		public void AddHUDBaseLayoutToolElements(List<UXElement> genericConfigElements)
		{
			genericConfigElements.Add(this.enterBltButton);
			genericConfigElements.Add(this.stashPanel);
			genericConfigElements.Add(this.stashTray);
			genericConfigElements.Add(this.cancelButton);
			genericConfigElements.Add(this.saveLayoutButton);
			genericConfigElements.Add(this.stashAllButton);
			genericConfigElements.Add(this.toggleStashModeButton);
			genericConfigElements.Add(this.bltTitleLabel);
			genericConfigElements.Add(this.stashInstructionsLabel);
			genericConfigElements.Add(this.infoWarBEButton);
		}

		public void ClearStashedBuildingTray()
		{
			if (this.stashedBuildingsGrid != null)
			{
				this.stashedBuildingsGrid.Clear();
				this.stashedBuildingsGrid.RepositionItems();
				this.stashedBuildingsGrid.HideScrollArrows();
			}
		}

		public float GetStashedBuildingTrayHeight()
		{
			return this.stashTray.Height;
		}

		private void InitButtons()
		{
			Lang lang = Service.Lang;
			this.cancelButton = this.hud.GetElement<UXButton>("BtnCancelStash");
			this.cancelButton.OnClicked = new UXButtonClickedDelegate(this.OnCancelButtonClicked);
			this.hud.GetElement<UXLabel>("LabelBtnCancelStash").Text = lang.Get("btn_Cancel", new object[0]);
			this.enterBltButton = this.hud.GetElement<UXButton>("BtnActivateStash");
			this.enterBltButton.OnClicked = new UXButtonClickedDelegate(this.OnEnterBLTButtonClicked);
			this.hud.GetElement<UXLabel>("LabelActivateStash").Text = lang.Get("blt_btn", new object[0]);
			this.infoWarBEButton = this.hud.GetElement<UXButton>("BtnInfoWarBE");
			this.infoWarBEButton.OnClicked = new UXButtonClickedDelegate(Service.SquadController.WarManager.ShowInfoScreen);
			this.saveLayoutButton = this.hud.GetElement<UXButton>("BtnSaveLayout");
			this.saveLayoutButton.OnClicked = new UXButtonClickedDelegate(this.OnSaveButtonClicked);
			this.hud.GetElement<UXLabel>("LabelBtnSaveLayout").Text = lang.Get("btn_Save", new object[0]);
			this.stashAllButton = this.hud.GetElement<UXButton>("BtnStashAll");
			this.stashAllButton.OnClicked = new UXButtonClickedDelegate(this.OnStashAllButtonClicked);
			this.hud.GetElement<UXLabel>("LabelBtnStashAll").Text = lang.Get("blt_stash_all_btn", new object[0]);
			this.toggleStashModeButton = this.hud.GetElement<UXButton>("BtnStashMode");
			this.hud.GetElement<UXLabel>("LabelStashMode").Text = lang.Get("blt_quick_stash", new object[0]);
			this.toggleStashModeButton.OnClicked = new UXButtonClickedDelegate(this.OnStashModeClicked);
			this.stashModeCheck = this.hud.GetElement<UXSprite>("SpriteCheckStashMode");
			this.stashModeTable = this.hud.GetElement<UXTable>("StashModeBtnTable");
		}

		private void InitGrid()
		{
			this.stashedBuildingsGrid = this.hud.GetElement<UXGrid>("StashGrid");
			this.stashedBuildingsGrid.SetTemplateItem("StashTemplate");
		}

		private void InitLabels()
		{
			Lang lang = Service.Lang;
			this.bltTitleLabel = this.hud.GetElement<UXLabel>("LabelTitleTopLeft");
			this.bltTitleLabel.Text = lang.Get("blt_title", new object[0]);
			this.stashInstructionsLabel = this.hud.GetElement<UXLabel>("LabelStashInstructions");
			this.stashInstructionsLabel.Text = lang.Get("blt_tray_instructions", new object[0]);
			this.SelectedBuildingLabel = this.hud.GetElement<UXLabel>("LabelContextDescriptionStash");
			this.SelectedBuildingLabel.Text = string.Empty;
			this.hud.GetElement<UXLabel>("LabelMessageCountBattleActivateStash").Text = lang.Get("s_New", new object[0]);
		}

		private void OnEnterBLTButtonClicked(UXButton button)
		{
			if (Service.BILoggingController != null)
			{
				Service.BILoggingController.TrackGameAction("UI_edit_mode", "enter", null, null);
			}
			Service.GameStateMachine.SetState(new BaseLayoutToolState());
		}

		private void OnCancelButtonClicked(UXButton button)
		{
			this.CancelBaseLayoutTool();
		}

		public void CancelBaseLayoutTool()
		{
			string pref = Service.SharedPlayerPrefs.GetPref<string>("SkipBLTCancel");
			if (pref != "1" && Service.BaseLayoutToolController.ShouldRevertMap)
			{
				Lang lang = Service.Lang;
				string title = lang.Get("blt_cancel_title", new object[0]);
				string message = lang.Get("blt_cancel_body", new object[0]);
				AlertWithCheckBoxScreen alertWithCheckBoxScreen = new AlertWithCheckBoxScreen(title, message, "SKIP_FUTURE_CONFIRMATION", new AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult(this.OnCancelConfirmationPopupClosed));
				alertWithCheckBoxScreen.Set2ButtonGroupEnabledState(true);
				Service.ScreenController.AddScreen(alertWithCheckBoxScreen, false);
				return;
			}
			this.LayoutCanceled();
		}

		private void ExitBaseLayoutTool(bool afterSave)
		{
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				HomeMapDataLoader homeMapDataLoader = Service.HomeMapDataLoader;
				Service.WorldTransitioner.StartTransition(new WarbaseToWarboardTransition(new WarBoardState(), homeMapDataLoader, null, false, false));
			}
			else if (afterSave)
			{
				HomeState.GoToHomeState(null, false);
			}
			else
			{
				Service.GameStateMachine.SetState(new EditBaseState(false));
			}
		}

		private void LayoutCanceled()
		{
			this.LogCancelLayoutButton();
			this.ExitBaseLayoutTool(false);
		}

		private void OnCancelConfirmationPopupClosed(object result, bool selected)
		{
			if (result != null)
			{
				this.LayoutCanceled();
				if (selected)
				{
					Service.SharedPlayerPrefs.SetPref("SkipBLTCancel", "1");
				}
			}
		}

		private void LogSaveLayoutButton()
		{
			string action = "exit";
			string message = "save";
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				action = "WarBE_save";
				message = this.GetSquadID();
			}
			Service.BILoggingController.TrackGameAction(this.GetBaseEditingBILoggingContext(), action, message, null);
		}

		private void LogCancelLayoutButton()
		{
			string action = "exit";
			string message = "cancel";
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				action = "WarBE_cancel";
				message = this.GetSquadID();
			}
			Service.BILoggingController.TrackGameAction(this.GetBaseEditingBILoggingContext(), action, message, null);
		}

		private void LogStashAllButton()
		{
			string action = "stash_all";
			string message = null;
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				action = "WarBE_stash_all";
				message = this.GetSquadID();
			}
			Service.BILoggingController.TrackGameAction(this.GetBaseEditingBILoggingContext(), action, message, null);
		}

		private string GetBaseEditingBILoggingContext()
		{
			string result = "UI_edit_mode";
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				result = Service.SquadController.WarManager.CurrentSquadWar.WarId;
			}
			return result;
		}

		private string GetSquadID()
		{
			return Service.SquadController.StateManager.GetCurrentSquad().SquadID;
		}

		private void OnSaveButtonClicked(UXButton button)
		{
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			if (!baseLayoutToolController.IsStashedBuildingListEmpty())
			{
				Lang lang = Service.Lang;
				string title = lang.Get("blt_unfinished_base_title", new object[0]);
				string message = lang.Get("blt_unfinished_base_body", new object[0]);
				AlertScreen.ShowModal(false, title, message, null, null, null, false, false, null, false);
				return;
			}
			string pref = Service.SharedPlayerPrefs.GetPref<string>("SkipBLTSave");
			if (pref != "1")
			{
				Lang lang2 = Service.Lang;
				string title2 = lang2.Get("blt_confirm_title", new object[0]);
				string message2 = lang2.Get("blt_confirm_body", new object[0]);
				AlertWithCheckBoxScreen alertWithCheckBoxScreen = new AlertWithCheckBoxScreen(title2, message2, "SKIP_FUTURE_CONFIRMATION", new AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult(this.OnSaveConfirmationPopupClosed));
				alertWithCheckBoxScreen.Set2ButtonGroupEnabledState(true);
				Service.ScreenController.AddScreen(alertWithCheckBoxScreen, false);
				return;
			}
			baseLayoutToolController.SaveMap();
			this.LogSaveLayoutButton();
			this.ExitBaseLayoutTool(true);
		}

		private void OnSaveConfirmationPopupClosed(object result, bool selected)
		{
			if (result != null)
			{
				Service.BaseLayoutToolController.SaveMap();
				this.LogSaveLayoutButton();
				this.ExitBaseLayoutTool(true);
				if (selected)
				{
					Service.SharedPlayerPrefs.SetPref("SkipBLTSave", "1");
				}
			}
		}

		private void OnStashAllButtonClicked(UXButton button)
		{
			Service.BaseLayoutToolController.StashAllBuildings();
			this.LogStashAllButton();
			this.RefreshWholeStashTray();
		}

		private void OnStashModeClicked(UXButton button)
		{
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			baseLayoutToolController.IsQuickStashModeEnabled = !baseLayoutToolController.IsQuickStashModeEnabled;
			this.RefreshStashModeCheckBox();
		}

		public void RefreshStashModeCheckBox()
		{
			if (Service.BaseLayoutToolController.IsQuickStashModeEnabled)
			{
				this.stashModeCheck.Visible = true;
				BuildingController buildingController = Service.BuildingController;
				buildingController.EnsureLoweredLiftedBuilding();
				buildingController.EnsureDeselectSelectedBuilding();
			}
			else
			{
				this.stashModeCheck.Visible = false;
			}
			this.stashModeTable.RepositionItems();
			this.RefreshInstructionLabel();
		}

		public void RefreshWholeStashTray()
		{
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			if (baseLayoutToolController.stashedBuildingMap == null || baseLayoutToolController.stashedBuildingMap.Count < 1)
			{
				this.stashedBuildingsGrid.Clear();
				return;
			}
			foreach (KeyValuePair<string, List<SmartEntity>> current in baseLayoutToolController.stashedBuildingMap)
			{
				List<SmartEntity> value = current.Value;
				if (value.Count >= 1)
				{
					Building buildingTO = value[0].BuildingComp.BuildingTO;
					this.RefreshStashedBuildingCount(buildingTO.Uid);
				}
			}
			this.stashedBuildingsGrid.SetAnimateSmoothly(false);
			this.stashedBuildingsGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.ResetScrollViewAfterFullRefresh));
		}

		private void ResetScrollViewAfterFullRefresh(AbstractUXList list)
		{
			this.stashedBuildingsGrid.SetAnimateSmoothly(true);
			this.stashedBuildingsGrid.UpdateScrollArrows();
		}

		private void CreateBuildingTrayItem(BuildingTypeVO vo, int count)
		{
			UXElement uXElement = this.stashedBuildingsGrid.CloneTemplateItem(vo.Uid);
			UXCheckbox subElement = this.stashedBuildingsGrid.GetSubElement<UXCheckbox>(vo.Uid, "CheckboxStash");
			subElement.Tag = vo.Uid;
			subElement.OnSelected = new UXCheckboxSelectedDelegate(this.OnStashedBuildingClicked);
			subElement.Selected = false;
			uXElement.Tag = vo;
			UXLabel subElement2 = this.stashedBuildingsGrid.GetSubElement<UXLabel>(vo.Uid, "LabelQuantityStash");
			subElement2.Tag = vo.Uid;
			subElement2.Text = count.ToString();
			UXLabel subElement3 = this.stashedBuildingsGrid.GetSubElement<UXLabel>(vo.Uid, "LabelBldgLevelStash");
			subElement3.Text = LangUtils.GetLevelText(vo.Lvl);
			UXSprite subElement4 = this.stashedBuildingsGrid.GetSubElement<UXSprite>(vo.Uid, "SpriteBldgStash");
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateBuildingConfig(vo, subElement4);
			projectorConfig.AnimPreference = AnimationPreference.NoAnimation;
			ProjectorUtils.GenerateProjector(projectorConfig);
			this.stashedBuildingsGrid.AddItem(uXElement, vo.Lvl + vo.StashOrder);
		}

		private void OnStashedBuildingClicked(UXCheckbox checkBox, bool selected)
		{
			if (selected)
			{
				this.UnstashBuilding((string)checkBox.Tag);
				BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
				if (baseLayoutToolController.IsQuickStashModeEnabled)
				{
					baseLayoutToolController.IsQuickStashModeEnabled = false;
					this.RefreshStashModeCheckBox();
				}
			}
			checkBox.Selected = false;
		}

		private void UnstashBuilding(string buildingUID)
		{
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			string a = null;
			Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
			if (selectedBuilding != null)
			{
				a = selectedBuilding.Get<BuildingComponent>().BuildingType.Uid;
			}
			if (a == buildingUID && baseLayoutToolController.IsBuildingStampable(selectedBuilding))
			{
				baseLayoutToolController.StampUnstashBuildingByUID(buildingUID);
			}
			else
			{
				baseLayoutToolController.UnstashBuildingByUID(buildingUID, false, true, true, true);
			}
			this.RefreshStashedBuildingCount(buildingUID);
			Service.UXController.HUD.ShowContextButtons(Service.BuildingController.SelectedBuilding);
		}

		public void RefreshStashedBuildingCount(string buildingUID)
		{
			UXElement uXElement = null;
			int i = 0;
			int count = this.stashedBuildingsGrid.Count;
			while (i < count)
			{
				BuildingTypeVO buildingTypeVO = this.stashedBuildingsGrid.GetItem(i).Tag as BuildingTypeVO;
				if (buildingTypeVO.Uid == buildingUID)
				{
					uXElement = this.stashedBuildingsGrid.GetItem(i);
					break;
				}
				i++;
			}
			int num = 0;
			this.addedBuildingCardUID = null;
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			if (baseLayoutToolController.stashedBuildingMap.ContainsKey(buildingUID))
			{
				num = baseLayoutToolController.stashedBuildingMap[buildingUID].Count;
			}
			bool flag = false;
			this.lastScrollPosition = this.stashedBuildingsGrid.GetCurrentScrollPosition(true);
			if (num > 0)
			{
				if (uXElement == null)
				{
					BuildingTypeVO vo = Service.StaticDataController.Get<BuildingTypeVO>(buildingUID);
					this.CreateBuildingTrayItem(vo, num);
					flag = true;
				}
				UXLabel subElement = this.stashedBuildingsGrid.GetSubElement<UXLabel>(buildingUID, "LabelQuantityStash");
				subElement.Text = num.ToString();
				this.addedBuildingCardUID = buildingUID;
			}
			else if (uXElement != null)
			{
				if (this.stashedBuildingsGrid.GetSortedIndex(uXElement) == 0)
				{
					this.stashedBuildingsGrid.SetAnimateSmoothly(false);
				}
				this.stashedBuildingsGrid.RemoveItem(uXElement);
				this.hud.DestroyElement(uXElement);
				flag = true;
			}
			this.RefreshSaveLayoutButtonStatus();
			if (!flag)
			{
				return;
			}
			this.stashedBuildingsGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.OnRepositionComplete));
		}

		private void OnRepositionComplete(AbstractUXList list)
		{
			this.stashedBuildingsGrid.UpdateScrollArrows();
			this.stashedBuildingsGrid.SetAnimateSmoothly(true);
			if (list == null)
			{
				return;
			}
			if (!this.stashedBuildingsGrid.IsGridComponentScrollable())
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.addedBuildingCardUID))
			{
				this.stashedBuildingsGrid.ScrollToItem(this.GetGridIndexOfBuildingUID(this.addedBuildingCardUID));
				this.addedBuildingCardUID = null;
				return;
			}
			this.stashedBuildingsGrid.Scroll(this.lastScrollPosition);
		}

		private int GetGridIndexOfBuildingUID(string buildingUID)
		{
			int result = -1;
			if (this.stashedBuildingsGrid == null)
			{
				return result;
			}
			int i = 0;
			int count = this.stashedBuildingsGrid.Count;
			while (i < count)
			{
				BuildingTypeVO buildingTypeVO = (BuildingTypeVO)this.stashedBuildingsGrid.GetItem(i).Tag;
				if (buildingTypeVO.Uid == buildingUID)
				{
					result = i;
					break;
				}
				i++;
			}
			return result;
		}

		public void RefreshSaveLayoutButtonStatus()
		{
			if (this.stashedBuildingsGrid != null && this.stashedBuildingsGrid.Count > 0)
			{
				this.saveLayoutButton.VisuallyDisableButton();
			}
			else
			{
				this.saveLayoutButton.VisuallyEnableButton();
			}
		}

		public void RefreshNewJewelStatus()
		{
			string pref = Service.SharedPlayerPrefs.GetPref<string>("BLTSeen");
			if (pref != "1")
			{
				this.newJewelContainer.Visible = true;
			}
			else
			{
				this.newJewelContainer.Visible = false;
			}
		}

		private void RefreshInstructionLabel()
		{
			if (Service.BuildingController.SelectedBuilding != null)
			{
				this.stashInstructionsLabel.Visible = false;
			}
			else
			{
				this.stashInstructionsLabel.Visible = true;
				bool isQuickStashModeEnabled = Service.BaseLayoutToolController.IsQuickStashModeEnabled;
				Lang lang = Service.Lang;
				if (isQuickStashModeEnabled)
				{
					this.stashInstructionsLabel.Text = lang.Get("blt_tray_quickmode_on", new object[0]);
				}
				else
				{
					this.stashInstructionsLabel.Text = lang.Get("blt_tray_instructions", new object[0]);
				}
			}
		}

		private void RefreshCurrencyTray(bool selected)
		{
			Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
			if (selectedBuilding == null || !selected)
			{
				this.hud.CurrentHudConfig.Remove("Currency");
				this.hud.RefreshView();
				return;
			}
			BuildingType type = selectedBuilding.Get<BuildingComponent>().BuildingType.Type;
			bool flag = type == BuildingType.Clearable;
			if (type == BuildingType.DroidHut)
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				if (currentPlayer.CurrentDroidsAmount < currentPlayer.MaxDroidsAmount)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.hud.CurrentHudConfig.Add("Currency");
				this.hud.CurrentHudConfig.Add("CrystalsDroids");
				this.hud.CurrentHudConfig.Add("Droids");
				this.hud.CurrentHudConfig.Add("Crystals");
				this.hud.RefreshView();
			}
			else
			{
				this.hud.CurrentHudConfig.Remove("Currency");
				this.hud.CurrentHudConfig.Remove("CrystalsDroids");
				this.hud.CurrentHudConfig.Remove("Droids");
				this.hud.CurrentHudConfig.Remove("Crystals");
				this.hud.RefreshView();
			}
		}
	}
}
