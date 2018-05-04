using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarScreen : ClosableScreen, IEventObserver, IViewClockTimeObserver
	{
		private const int SORT_FACTOR = 10000;

		private const int PLANET_GEO_FRAME_DELAY = 3;

		private const string PREP_PHASE_TIME_FORMAT = "WAR_BOARD_PREP_PHASE_TIME_REMAINING";

		private const string ACTION_PHASE_TIME_FORMAT = "WAR_BOARD_ACTION_PHASE_TIME_REMAINING";

		private const string COOLDOWN_PHASE_TIME_FORMAT = "WAR_BOARD_COOLDOWN_PHASE_TIME_REMAINING";

		private const string WAR_BOARD_PREP_GRACE_PHASE = "WAR_BOARD_PREP_GRACE_PHASE";

		private const string WAR_BOARD_PREP_GRACE_PHASE_DESC = "WAR_BOARD_PREP_GRACE_PHASE_DESC";

		private const string WAR_BOARD_ACTION_GRACE_PHASE = "WAR_BOARD_ACTION_GRACE_PHASE";

		private const string WAR_BOARD_ACTION_GRACE_PHASE_DESC = "WAR_BOARD_ACTION_GRACE_PHASE_DESC";

		private const string GRACE_POPUP_OKAY_BUTTON_LABEL = "WAR_OK";

		private const string WAR_BOARD_DROPDOWN_TOTALS = "WAR_BOARD_DROPDOWN_TOTALS";

		private const string EDIT_WAR_BASE = "WAR_BOARD_EDIT_BASE";

		private const string REQUEST_TROOPS = "WAR_BOARD_REQUEST_TROOPS";

		private const string WAR_BOARD_REQUEST_TROOPS_MAX_CAPACITY = "WAR_BOARD_REQUEST_TROOPS_MAX_CAPACITY";

		private const string WAR_BOARD_TOTAL_ATTACKS_REMAINING = "WAR_BOARD_TOTAL_ATTACKS_REMAINING";

		private const string WAR_BOARD_HQ_LABEL = "WAR_BOARD_HQ_LABEL";

		private const string BUFF_BASE_INFO_LEVEL_SHORT = "BUFF_BASE_INFO_LEVEL_SHORT";

		private const string LANG_TURNS_LEFT = "WAR_PLAYER_DETAILS_TURNS_LEFT";

		private const string LANG_POINTS_LEFT = "WAR_PLAYER_DETAILS_POINTS_LEFT";

		private const string PREP_PHASE_NEUTRAL_BASES = "WAR_BOARD_PREP_PHASE_NEUTRAL_BASES";

		private const string ACTION_PHASE_OPPONENT_CAPTURED_BASES = "WAR_BOARD_OPPONENT_CAPTURED_BASES_ACTION_PHASE";

		private const string ACTION_PHASE_PLAYER_CAPTURED_BASES = "WAR_BOARD_PLAYER_CAPTURED_BASES_ACTION_PHASE";

		private const string ACTION_PHASE_NEUTRAL_BASES = "WAR_BOARD_NEUTRAL_BASES_ACTION_PHASE";

		private const string COOLDOWN_PHASE_OPPONENT_CAPTURED_BASES = "WAR_BOARD_COOLDOWN_PHASE_OPPONENT_CAPTURED_BASES";

		private const string COOLDOWN_PHASE_PLAYER_CAPTURED_BASES = "WAR_BOARD_COOLDOWN_PHASE_PLAYER_CAPTURED_BASES";

		private const string COOLDOWN_PHASE_NEUTRAL_BASES = "WAR_BOARD_COOLDOWN_PHASE_NEUTRAL_BASES";

		private const string WAR_BOARD_START_NEW_WAR = "WAR_BOARD_START_NEW_WAR";

		private const string FACTION_EMPIRE = "Empire";

		private const string FACTION_REBEL = "Rebel";

		private const string SQUAD_PLAYER = "Player";

		private const string SQUAD_OPPONENT = "Opponent";

		private const string EMPIRE_ICON_NAME = "FactionEmpire";

		private const string REBEL_ICON_NAME = "FactionRebel";

		private const string FACTION_ICON_SPRITE_PREFIX = "SpriteIcon";

		private const string FACTION_ICON_DEFAULT_SPRITE_PREFIX = "SpriteIconZero";

		private const string FACTION_EMPIRE_SUFFIX = "Emp";

		private const string FACTION_REBEL_SUFFIX = "Reb";

		private const string SQUAD_WAR_CLOSE_BUTTON = "BtnCloseSquadWar";

		private const string BUTTON_INFO = "BtnInfo";

		private const string GROUP_PREP_PHASE = "PrepPhase";

		private const string BUTTON_EDIT_WAR_BASE = "ButtonBattle";

		private const string LABEL_EDIT_WAR_BASE = "LabelBattle";

		private const string GROUP_MESSAGE_COUNT_BATTLE = "ContainerJewelBattle";

		private const string LABEL_MESSAGE_COUNT_BATTLE = "LabelMessageCountBattle";

		private const string BUTTON_REQUEST_TROOPS = "ButtonStore";

		private const string LABEL_REQUEST_TROOPS = "LabelStore";

		private const string LABEL_PREP_TIMER = "LabelPrepTimer";

		private const string GROUP_WAR_TIMER = "WarTimer";

		private const string LABEL_ACTION_AND_COOLDOWN_TIMER = "LabelActionTimer";

		private static readonly Color TEXT_ACTION_TIMER_COLOR = new Color(0.847058833f, 0f, 0f);

		private static readonly Color TEXT_COOLDOWN_TIMER_COLOR = new Color(0.06666667f, 0.7058824f, 0.9254902f);

		private const string GROUP_FILTER = "GroupFilter";

		private const string DROPDOWN_PLAYER_GROUP = "DropdownPlayerGroup";

		private const string DROPDOWN_OPPONENT_GROUP = "DropdownOpponentGroup";

		private const string DROPDOWN_BUTTON_PLAYER = "DropdownButtonPlayer";

		private const string DROPDOWN_BUTTON_OPPONENT = "DropdownButtonOpponent";

		private const string BUTTON_PLAYER_LIST = "GroupPlayerScore";

		private const string BUTTON_OPPONENT_LIST = "GroupOpponentScore";

		private const string PANEL_PLAYER_LIST = "PanelPlayerList";

		private const string PANEL_OPPONENT_LIST = "PanelOpponentList";

		private const string LABEL_PLAYER_STATS_TOTAL_ATTACKS_PREFIX = "LabelPlayerStatsTotalAttacks";

		private const string LABEL_PLAYER_STATS_TOTAL_STARS_PREFIX = "LabelPlayerStatsTotalStars";

		private const string LABEL_DROPDOWN_ATTACKS_PREFIX = "LabelDropdownAttacks";

		private const string ITEM_OPPONENT_LIST = "ItemOpponentList";

		private const string ITEM_PLAYER_LIST = "ItemPlayerList";

		private const string LABEL_PLAYER_STATS_TOTAL_PLAYER = "LabelPlayerStatsTotalPlayer";

		private const string LABEL_PLAYER_STATS_TOTAL_OPPONENT = "LabelPlayerStatsTotalOpponent";

		private const string LABEL_OWN_ATTACKS_REMAINING = "LabelOwnAttacksRemaining";

		private const string SPRITE_ATTACKS_REMAINING_BG = "SpriteAttacksRemainingBg";

		private const string BUTTON_FILTER_PLAYER = "BtnFilterPlayer";

		private const string BUTTON_FILTER_OPPONENT = "BtnFilterOpponent";

		private const string LABEL_FILTER_PREFIX = "LabelBtnFilter";

		private const string GRID_PLAYER_LIST = "GridPlayerList";

		private const string GRID_OPPONENT_LIST = "GridOpponentList";

		private const string LABEL_PLAYER_NAME_PREFIX = "LabelPlayerName";

		private const string LABEL_STARS_AVAILABLE_PREFIX = "LabelStarsAvailable";

		private const string LABEL_ATTACKS_REMAINING_PREFIX = "LabelAttacksRemaining";

		private const string SPRITE_HIGHLIGHT_PREFIX = "SpritePlayerHighlight";

		private const string LABEL_SCORE_PREFIX = "LabelScore";

		private const string SPRITE_SQUAD_ICON_PREFIX = "SpriteScoreIcon";

		private const string SPRITE_SQUAD_BG_GLOW_PREFIX = "SpriteBgGlow";

		private const string LABEL_NAME_PREFIX = "LabelName";

		private const string SPRITE_ARROW_PREFIX = "Sprite";

		private const string SPRITE_ARROW_POSTFIX = "Arrow";

		private const string SPRITE_WINNING_PREFIX = "SpriteWinning";

		private const string LABEL_LIST_STARS_AVAILABLE_PREFIX = "LabelListAvailableStars";

		private const string LABEL_LIST_ATTACKS_REMAINING_PREFIX = "LabelListAttacksRemaining";

		private const string SPRITE_BG_BTN_FILTER_PREFIX = "SpriteBgBtnFilter";

		private const string BTN_FILTER_PREFIX = "BtnFilter";

		private const string GRID_BUFF_PLAYER_PLANETS = "GridBuffPlanetsPlayer";

		private const string GRID_BUFF_PLAYER_TEMPLATE = "BuffPlanetPlayer";

		private const string GRID_BUFF_PLAYER_SPRITE = "SpritePlanetPlayer";

		private const string SPRITE_LOCK_PLAYER = "SpriteLockedPlayer";

		private const string SPRITE_CAPTURED_BG_GLOW_PREFIX = "SpriteCapturedBgGlow";

		private const string GRID_BUFF_NEUTRAL_PLANETS = "GridBuffPlanetsNeutral";

		private const string GRID_BUFF_NEUTRAL_TEMPLATE = "BuffPlanetNeutral";

		private const string GRID_BUFF_NEUTRAL_SPRITE = "SpritePlanetNeutral";

		private const string SPRITE_BUFF_ICON_NEUTRAL = "SpriteBuffIconNeutral";

		private const string LABEL_BASE_INFO_NEUTRAL = "LabelBaseInfoNeutral";

		private const string SPRITE_LOCK_NEUTRAL = "SpriteLockedNeutral";

		private const string GRID_BUFF_OPPONENT_PLANETS = "GridBuffPlanetsOpponent";

		private const string GRID_BUFF_OPPONENT_TEMPLATE = "BuffPlanetOpponent";

		private const string GRID_BUFF_OPPONENT_SPRITE = "SpritePlanetOpponent";

		private const string SPRITE_BUFF_ICON_OPPONENT = "SpriteBuffIconOpponent";

		private const string LABEL_BASE_INFO_OPPONENT = "LabelBaseInfoOpponent";

		private const string SPRITE_LOCK_OPPONENT = "SpriteLockedOpponent";

		private const string BTN_START_NEW_WAR = "BtnStartNewWar";

		private const string LABEL_BTN_START_NEW_WAR = "LabelBtnStartNewWar";

		private const string CAPTURED_PREFIX = "Captured";

		private const string CAPTURED_GLOW_PREFIX = "CapturedGlow";

		private const string LABEL_CAPTURED_PREFIX = "LabelCaptured";

		private const string LABEL_CAPTURED_NEUTRAL = "LabelCapturedNeutral";

		private const string SHOW = "show";

		private const string HIDE = "hide";

		private const float BUFF_BASE_RENDER_SIZE_MULTIPLIER = 3f;

		private const string CONTAINER_PLAYER_COLLAPSED = "ContainerPlayerCollapsed";

		private const string CONTAINER_OPPONENT_COLLAPSED = "ContainerOpponentCollapsed";

		private UXGrid gridBuffPlayerPlanets;

		private UXGrid gridBuffNeutralPlanets;

		private UXGrid gridBuffOpponentPlanets;

		private UXLabel labelBuffPlayerRebelPlanets;

		private UXLabel labelBuffPlayerEmpirePlanets;

		private UXLabel labelBuffNeutralPlanets;

		private UXLabel labelBuffOpponentRebelPlanets;

		private UXLabel labelBuffOpponentEmpirePlanets;

		private UXLabel labelSquadScore;

		private UXSprite spriteSquadWinning;

		private UXSprite spriteBuffPlayerRebelPlanets;

		private UXSprite spriteBuffPlayerEmpirePlanets;

		private UXSprite spriteBuffOpponentRebelPlanets;

		private UXSprite spriteBuffOpponentEmpirePlanets;

		private UXElement groupCapturedPlayerRebel;

		private UXElement groupCapturedPlayerEmpire;

		private UXElement groupCapturedOpponentRebel;

		private UXElement groupCapturedOpponentEmpire;

		private UXElement groupCapturedGlowPlayerRebel;

		private UXElement groupCapturedGlowPlayerEmpire;

		private UXElement groupCapturedGlowOpponentRebel;

		private UXElement groupCapturedGlowOpponentEmpire;

		private UXElement groupPrepPhase;

		private UXElement groupWarTimer;

		private UXElement groupFilter;

		private UXElement groupBattleMessageCount;

		private UXLabel labelBattleMessageCount;

		private UXLabel labelPrepTimer;

		private UXLabel labelActionAndCooldownTimer;

		private int phaseTimeRemaining;

		private UXGrid playerGrid;

		private UXGrid opponentGrid;

		private UXElement dropdownPlayerGroup;

		private UXElement dropdownOpponentGroup;

		private UXButton dropdownButtonRebel;

		private UXButton dropdownButtonEmpire;

		private UXElement panelPlayerList;

		private UXElement panelOpponentList;

		private UXCheckbox buttonFilterPlayer;

		private UXCheckbox buttonFilterOpponent;

		private UXButton containerRebelCollapsed;

		private UXButton containerEmpireCollapsed;

		private UXLabel labelPlayerStatsTotalEmpire;

		private UXLabel labelPlayerStatsTotalRebel;

		private UXLabel labelAttacksRemaining;

		private UXSprite attacksRemainingBg;

		private UXButton btnStartNewWar;

		private UXLabel labelBtnStartNewWar;

		private List<GeometryProjector> projectors;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public SquadWarScreen() : base("gui_squadwar")
		{
		}

		public override void OnDestroyElement()
		{
			if (this.playerGrid != null)
			{
				this.playerGrid.Scroll(0f);
				this.playerGrid.Clear();
				this.playerGrid = null;
			}
			if (this.opponentGrid != null)
			{
				this.opponentGrid.Scroll(0f);
				this.opponentGrid.Clear();
				this.opponentGrid = null;
			}
			this.DestroyOldBuffElements();
			this.gridBuffPlayerPlanets = null;
			this.gridBuffNeutralPlanets = null;
			this.gridBuffOpponentPlanets = null;
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.WarPhaseChanged);
			eventManager.UnregisterObserver(this, EventId.SquadScreenOpenedOrClosed);
			eventManager.UnregisterObserver(this, EventId.WarBuffBaseCaptured);
			eventManager.UnregisterObserver(this, EventId.CurrentPlayerMemberDataUpdated);
			eventManager.UnregisterObserver(this, EventId.WarBoardFlyoutHidden);
			eventManager.UnregisterObserver(this, EventId.WarBoardParticipantBuildingSelected);
			eventManager.UnregisterObserver(this, EventId.WarVictoryPointsUpdated);
			eventManager.UnregisterObserver(this, EventId.SquadWarTroopsRequestStartedByCurrentPlayer);
			base.OnDestroyElement();
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.WarBuffBaseCaptured)
			{
				if (id != EventId.WarVictoryPointsUpdated)
				{
					switch (id)
					{
					case EventId.WarBoardParticipantBuildingSelected:
					case EventId.WarBoardFlyoutHidden:
						this.DeselectAllBuffBases();
						goto IL_1A7;
					case EventId.WarBoardBuffBaseBuildingSelected:
					case EventId.WarBoardBuildingDeselected:
						IL_32:
						if (id == EventId.SquadScreenOpenedOrClosed)
						{
							bool flag = (bool)cookie;
							if (this.Visible && flag)
							{
								this.Visible = false;
							}
							else if (!this.Visible && !flag)
							{
								this.Visible = true;
								if (this.gridBuffPlayerPlanets != null)
								{
									this.gridBuffPlayerPlanets.CenterElementsInPanel();
								}
								if (this.gridBuffNeutralPlanets != null)
								{
									this.gridBuffNeutralPlanets.CenterElementsInPanel();
								}
								if (this.gridBuffOpponentPlanets != null)
								{
									this.gridBuffOpponentPlanets.CenterElementsInPanel();
								}
							}
							goto IL_1A7;
						}
						if (id == EventId.CurrentPlayerMemberDataUpdated)
						{
							SquadWarStatusType currentStatus = Service.SquadController.WarManager.GetCurrentStatus();
							if (currentStatus == SquadWarStatusType.PhasePrep || currentStatus == SquadWarStatusType.PhasePrepGrace)
							{
								this.UpdateRequestTroopsLabel();
							}
							else if (currentStatus == SquadWarStatusType.PhaseCooldown)
							{
								this.TryShowWarEndedScreen();
							}
							goto IL_1A7;
						}
						if (id != EventId.WarPhaseChanged)
						{
							goto IL_1A7;
						}
						switch ((SquadWarStatusType)cookie)
						{
						case SquadWarStatusType.PhaseOpen:
							this.Close(null);
							HomeState.GoToHomeState(null, false);
							break;
						case SquadWarStatusType.PhasePrepGrace:
							this.ShowPrepGracePhase();
							break;
						case SquadWarStatusType.PhaseAction:
							this.ShowActionPhase();
							break;
						case SquadWarStatusType.PhaseActionGrace:
							this.ShowActionGracePhase();
							break;
						case SquadWarStatusType.PhaseCooldown:
							this.ShowCooldownPhase();
							break;
						}
						goto IL_1A7;
					}
					goto IL_32;
				}
				this.UpdateWarElements();
			}
			else
			{
				this.UpdateBuffOwnership(true);
			}
			IL_1A7:
			return base.OnEvent(id, cookie);
		}

		public void OnViewClockTime(float dt)
		{
			this.phaseTimeRemaining = Mathf.Max(0, this.phaseTimeRemaining - 1);
			SquadWarStatusType currentStatus = Service.SquadController.WarManager.GetCurrentStatus();
			this.SetTimeRemaining(this.phaseTimeRemaining, currentStatus);
		}

		protected override void HandleClose(UXButton button)
		{
			this.CloseSquadWarScreen(null);
		}

		protected override void OnCloseButtonClicked(UXButton button)
		{
			if (Service.WorldTransitioner.IsTransitioning())
			{
				return;
			}
			base.OnCloseButtonClicked(button);
		}

		public void CloseSquadWarScreen(TransitionCompleteDelegate callback)
		{
			if (!this.allowClose)
			{
				return;
			}
			Service.SquadController.WarManager.ExitWarBoardMode(callback);
			this.Close(null);
		}

		protected override void OnScreenLoaded()
		{
			this.projectors = new List<GeometryProjector>();
			this.labelAttacksRemaining = base.GetElement<UXLabel>("LabelOwnAttacksRemaining");
			this.labelAttacksRemaining.Visible = false;
			this.attacksRemainingBg = base.GetElement<UXSprite>("SpriteAttacksRemainingBg");
			this.attacksRemainingBg.Visible = false;
			this.labelBtnStartNewWar = base.GetElement<UXLabel>("LabelBtnStartNewWar");
			this.btnStartNewWar = base.GetElement<UXButton>("BtnStartNewWar");
			this.btnStartNewWar.Visible = false;
			this.btnStartNewWar.OnClicked = new UXButtonClickedDelegate(this.OnStartNewWarClicked);
			this.InitButtons();
			this.InitPrepPhase();
			this.InitActionAndCooldownPhase();
			this.InitBuffBaseUI();
			this.UpdateWarElements();
			switch (Service.SquadController.WarManager.GetCurrentStatus())
			{
			case SquadWarStatusType.PhasePrep:
				this.ShowPrepPhase();
				break;
			case SquadWarStatusType.PhasePrepGrace:
				this.ShowPrepGracePhase();
				break;
			case SquadWarStatusType.PhaseAction:
				this.ShowActionPhase();
				break;
			case SquadWarStatusType.PhaseActionGrace:
				this.ShowActionGracePhase();
				break;
			case SquadWarStatusType.PhaseCooldown:
				this.ShowCooldownPhase();
				break;
			}
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.SquadScreenOpenedOrClosed);
			eventManager.RegisterObserver(this, EventId.WarPhaseChanged);
			eventManager.RegisterObserver(this, EventId.WarBuffBaseCaptured);
			eventManager.RegisterObserver(this, EventId.CurrentPlayerMemberDataUpdated);
			eventManager.RegisterObserver(this, EventId.WarBoardFlyoutHidden);
			eventManager.RegisterObserver(this, EventId.WarBoardParticipantBuildingSelected);
			eventManager.RegisterObserver(this, EventId.WarVictoryPointsUpdated);
			eventManager.RegisterObserver(this, EventId.SquadWarTroopsRequestStartedByCurrentPlayer);
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		protected override void InitButtons()
		{
			this.CloseButton = base.GetElement<UXButton>("BtnCloseSquadWar");
			this.CloseButton.OnClicked = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
			this.CloseButton.Enabled = true;
			base.CurrentBackButton = this.CloseButton;
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
			base.GetElement<UXButton>("BtnInfo").OnClicked = new UXButtonClickedDelegate(Service.SquadController.WarManager.ShowInfoScreen);
		}

		private void InitPrepPhase()
		{
			this.groupPrepPhase = base.GetElement<UXElement>("PrepPhase");
			this.groupPrepPhase.Visible = false;
			UXButton element = base.GetElement<UXButton>("ButtonBattle");
			element.OnClicked = new UXButtonClickedDelegate(this.OnEditWarBaseButtonClicked);
			UXLabel element2 = base.GetElement<UXLabel>("LabelBattle");
			element2.Text = this.lang.Get("WAR_BOARD_EDIT_BASE", new object[0]);
			UXButton element3 = base.GetElement<UXButton>("ButtonStore");
			element3.OnClicked = new UXButtonClickedDelegate(this.OnRequestTroopsButtonClicked);
			UXLabel element4 = base.GetElement<UXLabel>("LabelStore");
			element4.Text = this.lang.Get("WAR_BOARD_REQUEST_TROOPS", new object[0]);
			this.groupBattleMessageCount = base.GetElement<UXElement>("ContainerJewelBattle");
			this.labelBattleMessageCount = base.GetElement<UXLabel>("LabelMessageCountBattle");
			this.SetBattleMessageCount(0);
			this.labelPrepTimer = base.GetElement<UXLabel>("LabelPrepTimer");
			this.SetTimeRemaining(0, SquadWarStatusType.PhasePrep);
		}

		private void InitActionAndCooldownPhase()
		{
			this.groupFilter = base.GetElement<UXElement>("GroupFilter");
			this.groupWarTimer = base.GetElement<UXElement>("WarTimer");
			this.groupWarTimer.Visible = false;
			this.playerGrid = base.GetElement<UXGrid>("GridPlayerList");
			this.playerGrid.SetTemplateItem("ItemPlayerList");
			this.opponentGrid = base.GetElement<UXGrid>("GridOpponentList");
			this.opponentGrid.SetTemplateItem("ItemOpponentList");
			this.dropdownButtonRebel = base.GetElement<UXButton>("DropdownButtonPlayer");
			this.dropdownButtonRebel.OnClicked = new UXButtonClickedDelegate(this.TogglePlayerList);
			this.dropdownButtonEmpire = base.GetElement<UXButton>("DropdownButtonOpponent");
			this.dropdownButtonEmpire.OnClicked = new UXButtonClickedDelegate(this.ToggleOpponentList);
			UXButton element = base.GetElement<UXButton>("GroupPlayerScore");
			element.OnClicked = new UXButtonClickedDelegate(this.TogglePlayerList);
			UXButton element2 = base.GetElement<UXButton>("GroupOpponentScore");
			element2.OnClicked = new UXButtonClickedDelegate(this.ToggleOpponentList);
			this.containerRebelCollapsed = base.GetElement<UXButton>("ContainerPlayerCollapsed");
			this.containerRebelCollapsed.OnClicked = new UXButtonClickedDelegate(this.TogglePlayerList);
			this.containerEmpireCollapsed = base.GetElement<UXButton>("ContainerOpponentCollapsed");
			this.containerEmpireCollapsed.OnClicked = new UXButtonClickedDelegate(this.ToggleOpponentList);
			this.dropdownPlayerGroup = base.GetElement<UXElement>("DropdownPlayerGroup");
			this.dropdownPlayerGroup.InitAnimator();
			this.dropdownOpponentGroup = base.GetElement<UXElement>("DropdownOpponentGroup");
			this.dropdownOpponentGroup.InitAnimator();
			this.labelPlayerStatsTotalEmpire = base.GetElement<UXLabel>("LabelPlayerStatsTotalPlayer");
			this.labelPlayerStatsTotalEmpire.Text = this.lang.Get("WAR_BOARD_DROPDOWN_TOTALS", new object[0]);
			this.labelPlayerStatsTotalRebel = base.GetElement<UXLabel>("LabelPlayerStatsTotalOpponent");
			this.labelPlayerStatsTotalRebel.Text = this.lang.Get("WAR_BOARD_DROPDOWN_TOTALS", new object[0]);
			this.panelPlayerList = base.GetElement<UXElement>("PanelPlayerList");
			this.panelPlayerList.Visible = false;
			this.panelOpponentList = base.GetElement<UXElement>("PanelOpponentList");
			this.panelOpponentList.Visible = false;
			this.labelActionAndCooldownTimer = base.GetElement<UXLabel>("LabelActionTimer");
			this.SetTimeRemaining(0, SquadWarStatusType.PhaseAction);
			this.InitDynamicElements(SquadWarSquadType.PLAYER_SQUAD);
			this.InitDynamicElements(SquadWarSquadType.OPPONENT_SQUAD);
			SquadWarSquadType currentDisplaySquad = Service.WarBoardViewController.GetCurrentDisplaySquad();
			this.RefreshFilterCheckboxForSquad(currentDisplaySquad);
		}

		private void InitDynamicElements(SquadWarSquadType squadType)
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarSquadData squadData = warManager.GetSquadData(squadType);
			string squadTypeString = this.GetSquadTypeString(squadType);
			bool flag = squadData.Faction == FactionType.Empire;
			string text = squadTypeString + "Emp";
			this.UpdateDynamicElements(text, flag);
			string text2 = squadTypeString + "Reb";
			this.UpdateDynamicElements(text2, !flag);
			UXCheckbox element = base.GetElement<UXCheckbox>("BtnFilter" + text);
			UXCheckbox element2 = base.GetElement<UXCheckbox>("BtnFilter" + text2);
			element.Visible = flag;
			element2.Visible = !flag;
			if (squadType == SquadWarSquadType.PLAYER_SQUAD)
			{
				this.buttonFilterPlayer = ((!flag) ? element2 : element);
				this.buttonFilterPlayer.OnSelected = new UXCheckboxSelectedDelegate(this.OnFilterPlayerCheckboxSelected);
			}
			else
			{
				this.buttonFilterOpponent = ((!flag) ? element2 : element);
				this.buttonFilterOpponent.OnSelected = new UXCheckboxSelectedDelegate(this.OnFilterOpponentCheckboxSelected);
			}
		}

		private void UpdateDynamicElements(string squadNameFaction, bool isEmpire)
		{
			UXLabel element = base.GetElement<UXLabel>("LabelScore" + squadNameFaction);
			UXLabel element2 = base.GetElement<UXLabel>("LabelDropdownAttacks" + squadNameFaction);
			UXLabel element3 = base.GetElement<UXLabel>("LabelName" + squadNameFaction);
			UXLabel element4 = base.GetElement<UXLabel>("LabelCaptured" + squadNameFaction);
			UXSprite element5 = base.GetElement<UXSprite>("SpriteScoreIcon" + squadNameFaction);
			UXSprite element6 = base.GetElement<UXSprite>("SpriteBgGlow" + squadNameFaction);
			UXSprite element7 = base.GetElement<UXSprite>("SpriteWinning" + squadNameFaction);
			UXSprite element8 = base.GetElement<UXSprite>("SpriteBgBtnFilter" + squadNameFaction);
			element.Visible = isEmpire;
			element7.Visible = isEmpire;
			element2.Visible = isEmpire;
			element5.Visible = isEmpire;
			element6.Visible = isEmpire;
			element3.Visible = isEmpire;
			element8.Visible = isEmpire;
			element4.Visible = isEmpire;
		}

		private void InitBuffBaseUI()
		{
			this.gridBuffPlayerPlanets = base.GetElement<UXGrid>("GridBuffPlanetsPlayer");
			this.gridBuffPlayerPlanets.SetTemplateItem("BuffPlanetPlayer");
			this.gridBuffNeutralPlanets = base.GetElement<UXGrid>("GridBuffPlanetsNeutral");
			this.gridBuffNeutralPlanets.SetTemplateItem("BuffPlanetNeutral");
			this.gridBuffOpponentPlanets = base.GetElement<UXGrid>("GridBuffPlanetsOpponent");
			this.gridBuffOpponentPlanets.SetTemplateItem("BuffPlanetOpponent");
			string str = "PlayerReb";
			string str2 = "PlayerEmp";
			string str3 = "OpponentReb";
			string str4 = "OpponentEmp";
			this.labelBuffNeutralPlanets = base.GetElement<UXLabel>("LabelCapturedNeutral");
			this.labelBuffPlayerRebelPlanets = this.GetLabelWithName("LabelCaptured" + str);
			this.labelBuffPlayerEmpirePlanets = this.GetLabelWithName("LabelCaptured" + str2);
			this.labelBuffOpponentRebelPlanets = this.GetLabelWithName("LabelCaptured" + str3);
			this.labelBuffOpponentEmpirePlanets = this.GetLabelWithName("LabelCaptured" + str4);
			this.spriteBuffPlayerRebelPlanets = this.GetSpriteWithName("SpriteCapturedBgGlow" + str);
			this.spriteBuffPlayerEmpirePlanets = this.GetSpriteWithName("SpriteCapturedBgGlow" + str2);
			this.spriteBuffOpponentRebelPlanets = this.GetSpriteWithName("SpriteCapturedBgGlow" + str3);
			this.spriteBuffOpponentEmpirePlanets = this.GetSpriteWithName("SpriteCapturedBgGlow" + str4);
			this.groupCapturedPlayerRebel = this.GetElementWithName("Captured" + str);
			this.groupCapturedPlayerEmpire = this.GetElementWithName("Captured" + str2);
			this.groupCapturedOpponentRebel = this.GetElementWithName("Captured" + str3);
			this.groupCapturedOpponentEmpire = this.GetElementWithName("Captured" + str4);
			this.groupCapturedGlowPlayerRebel = this.GetElementWithName("CapturedGlow" + str);
			this.groupCapturedGlowPlayerEmpire = this.GetElementWithName("CapturedGlow" + str2);
			this.groupCapturedGlowOpponentRebel = this.GetElementWithName("CapturedGlow" + str3);
			this.groupCapturedGlowOpponentEmpire = this.GetElementWithName("CapturedGlow" + str4);
			this.UpdateBuffOwnership(false);
		}

		private UXLabel GetLabelWithName(string labelName)
		{
			return base.GetElement<UXLabel>(labelName);
		}

		private UXSprite GetSpriteWithName(string labelName)
		{
			return base.GetElement<UXSprite>(labelName);
		}

		private UXElement GetElementWithName(string labelName)
		{
			return base.GetElement<UXElement>(labelName);
		}

		private void SetBuffBaseUI(SquadWarStatusType status)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			switch (status)
			{
			case SquadWarStatusType.PhasePrep:
			case SquadWarStatusType.PhasePrepGrace:
				text3 = "WAR_BOARD_PREP_PHASE_NEUTRAL_BASES";
				break;
			case SquadWarStatusType.PhaseAction:
			case SquadWarStatusType.PhaseActionGrace:
				text2 = "WAR_BOARD_OPPONENT_CAPTURED_BASES_ACTION_PHASE";
				text = "WAR_BOARD_PLAYER_CAPTURED_BASES_ACTION_PHASE";
				text3 = "WAR_BOARD_NEUTRAL_BASES_ACTION_PHASE";
				break;
			case SquadWarStatusType.PhaseCooldown:
				text2 = "WAR_BOARD_COOLDOWN_PHASE_OPPONENT_CAPTURED_BASES";
				text = "WAR_BOARD_COOLDOWN_PHASE_PLAYER_CAPTURED_BASES";
				text3 = "WAR_BOARD_COOLDOWN_PHASE_NEUTRAL_BASES";
				this.DisableAllBuffBaseGrids();
				break;
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				SquadWarSquadData squadData = Service.SquadController.WarManager.GetSquadData(SquadWarSquadType.PLAYER_SQUAD);
				SquadWarSquadData squadData2 = Service.SquadController.WarManager.GetSquadData(SquadWarSquadType.OPPONENT_SQUAD);
				this.labelBuffPlayerRebelPlanets.Text = this.lang.Get(text, new object[0]);
				this.labelBuffPlayerEmpirePlanets.Text = this.lang.Get(text, new object[0]);
				this.labelBuffOpponentRebelPlanets.Text = this.lang.Get(text2, new object[0]);
				this.labelBuffOpponentEmpirePlanets.Text = this.lang.Get(text2, new object[0]);
				bool flag = squadData.Faction == FactionType.Empire;
				bool flag2 = squadData2.Faction == FactionType.Empire;
				this.labelBuffPlayerRebelPlanets.Visible = !flag;
				this.labelBuffPlayerEmpirePlanets.Visible = flag;
				this.labelBuffOpponentRebelPlanets.Visible = !flag2;
				this.labelBuffOpponentEmpirePlanets.Visible = flag2;
			}
			if (!string.IsNullOrEmpty(text3))
			{
				this.labelBuffNeutralPlanets.Text = this.lang.Get(text3, new object[0]);
			}
		}

		private void UpdateBuffOwnership(bool CleanUpOldElements)
		{
			if (CleanUpOldElements)
			{
				this.DestroyOldBuffElements();
			}
			SquadController squadController = Service.SquadController;
			SquadWarManager warManager = squadController.WarManager;
			SquadWarSquadData squadData = warManager.GetSquadData(SquadWarSquadType.OPPONENT_SQUAD);
			SquadWarSquadData squadData2 = warManager.GetSquadData(SquadWarSquadType.PLAYER_SQUAD);
			for (int i = 0; i < GameConstants.WAR_MAX_BUFF_BASES; i++)
			{
				SquadWarBuffBaseData buffBaseData = warManager.GetBuffBaseData(i);
				string ownerId = buffBaseData.OwnerId;
				WarBuffVO warBuffVO = Service.StaticDataController.Get<WarBuffVO>(buffBaseData.BuffBaseId);
				SquadWarSquadData squadWarSquadData = null;
				bool flag = false;
				if (ownerId == squadData.SquadId)
				{
					squadWarSquadData = squadData;
				}
				else if (ownerId == squadData2.SquadId)
				{
					squadWarSquadData = squadData2;
					flag = true;
				}
				if (squadWarSquadData == null)
				{
					this.CreateBuffBaseElement(this.gridBuffNeutralPlanets, warBuffVO, "SpritePlanetNeutral", buffBaseData, "SpriteBuffIconNeutral", "LabelBaseInfoNeutral", "SpriteLockedNeutral", null);
				}
				else if (!flag)
				{
					this.CreateBuffBaseElement(this.gridBuffOpponentPlanets, warBuffVO, "SpritePlanetOpponent", buffBaseData, "SpriteBuffIconOpponent", "LabelBaseInfoOpponent", "SpriteLockedOpponent", squadWarSquadData);
				}
				else
				{
					this.CreateBuffBaseElement(this.gridBuffPlayerPlanets, warBuffVO, "SpritePlanetPlayer", buffBaseData, "SpriteBuffIconNeutral", "LabelBaseInfoNeutral", "SpriteLockedPlayer", squadWarSquadData);
				}
			}
			bool flag2 = squadData2.Faction == FactionType.Empire;
			bool flag3 = squadData.Faction == FactionType.Empire;
			bool flag4 = this.gridBuffPlayerPlanets.Count > 0;
			bool flag5 = this.gridBuffOpponentPlanets.Count > 0;
			bool visible = flag4 && flag2;
			bool visible2 = flag4 && !flag2;
			this.gridBuffPlayerPlanets.CenterElementsInPanel();
			this.labelBuffPlayerRebelPlanets.Visible = visible2;
			this.labelBuffPlayerEmpirePlanets.Visible = visible;
			this.spriteBuffPlayerRebelPlanets.Visible = visible2;
			this.spriteBuffPlayerEmpirePlanets.Visible = visible;
			this.gridBuffNeutralPlanets.CenterElementsInPanel();
			bool visible3 = flag5 && flag3;
			bool visible4 = flag5 && !flag3;
			this.gridBuffOpponentPlanets.CenterElementsInPanel();
			this.labelBuffOpponentRebelPlanets.Visible = visible4;
			this.labelBuffOpponentEmpirePlanets.Visible = visible3;
			this.spriteBuffOpponentRebelPlanets.Visible = visible4;
			this.spriteBuffOpponentEmpirePlanets.Visible = visible3;
			this.groupCapturedPlayerRebel.Visible = visible2;
			this.groupCapturedPlayerEmpire.Visible = visible;
			this.groupCapturedOpponentRebel.Visible = visible4;
			this.groupCapturedOpponentEmpire.Visible = visible3;
			this.groupCapturedGlowPlayerRebel.Visible = visible2;
			this.groupCapturedGlowPlayerEmpire.Visible = visible;
			this.groupCapturedGlowOpponentRebel.Visible = visible4;
			this.groupCapturedGlowOpponentEmpire.Visible = visible3;
		}

		private void DestroyOldBuffElements()
		{
			int i = 0;
			int count = this.projectors.Count;
			while (i < count)
			{
				this.projectors[i].Destroy();
				i++;
			}
			this.projectors.Clear();
			if (this.gridBuffPlayerPlanets != null)
			{
				this.gridBuffPlayerPlanets.Clear();
			}
			if (this.gridBuffNeutralPlanets != null)
			{
				this.gridBuffNeutralPlanets.Clear();
			}
			if (this.gridBuffOpponentPlanets != null)
			{
				this.gridBuffOpponentPlanets.Clear();
			}
		}

		private void ShowPrepPhase()
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarData currentSquadWar = warManager.CurrentSquadWar;
			uint serverTime = Service.ServerAPI.ServerTime;
			bool visible = warManager.GetCurrentParticipantState() != null;
			this.groupPrepPhase.Visible = true;
			UXButton element = base.GetElement<UXButton>("ButtonBattle");
			element.Visible = visible;
			UXButton element2 = base.GetElement<UXButton>("ButtonStore");
			element2.Visible = visible;
			this.UpdateRequestTroopsLabel();
			this.groupFilter.Visible = true;
			this.groupWarTimer.Visible = false;
			this.dropdownPlayerGroup.Visible = true;
			this.dropdownOpponentGroup.Visible = true;
			this.UpdateWarElements();
			this.phaseTimeRemaining = currentSquadWar.PrepGraceStartTimeStamp - (int)serverTime;
			this.SetTimeRemaining(this.phaseTimeRemaining, SquadWarStatusType.PhasePrep);
			this.SetBuffBaseUI(SquadWarStatusType.PhasePrep);
		}

		private void ShowPrepGracePhase()
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarData currentSquadWar = warManager.CurrentSquadWar;
			uint serverTime = Service.ServerAPI.ServerTime;
			this.groupPrepPhase.Visible = true;
			UXButton element = base.GetElement<UXButton>("ButtonBattle");
			element.Visible = false;
			UXButton element2 = base.GetElement<UXButton>("ButtonStore");
			element2.Visible = false;
			this.UpdateRequestTroopsLabel();
			this.groupFilter.Visible = true;
			this.groupWarTimer.Visible = false;
			this.dropdownPlayerGroup.Visible = true;
			this.dropdownOpponentGroup.Visible = true;
			this.UpdateWarElements();
			this.phaseTimeRemaining = currentSquadWar.PrepEndTimeStamp - (int)serverTime;
			this.SetTimeRemaining(this.phaseTimeRemaining, SquadWarStatusType.PhasePrepGrace);
			this.SetBuffBaseUI(SquadWarStatusType.PhasePrepGrace);
			this.ShowGracePeriodAlertScreen(SquadWarStatusType.PhasePrepGrace, currentSquadWar.PrepEndTimeStamp);
		}

		private void UpdateRequestTroopsLabel()
		{
			UXButton element = base.GetElement<UXButton>("ButtonStore");
			UXLabel element2 = base.GetElement<UXLabel>("LabelStore");
			if (element.Visible)
			{
				if (SquadUtils.IsPlayerSquadWarTroopsAtMaxCapacity())
				{
					element2.Text = this.lang.Get("WAR_BOARD_REQUEST_TROOPS_MAX_CAPACITY", new object[0]);
				}
				else
				{
					element2.Text = this.lang.Get("WAR_BOARD_REQUEST_TROOPS", new object[0]);
				}
			}
		}

		private void ShowActionPhase()
		{
			this.ShowActionAndCooldownPhaseCommonUI();
			SquadWarParticipantState currentParticipantState = Service.SquadController.WarManager.GetCurrentParticipantState();
			if (currentParticipantState != null)
			{
				this.labelAttacksRemaining.Text = this.lang.Get("WAR_PLAYER_DETAILS_TURNS_LEFT", new object[]
				{
					currentParticipantState.TurnsLeft
				});
				this.labelAttacksRemaining.Visible = true;
				this.attacksRemainingBg.Visible = true;
			}
			SquadWarData currentSquadWar = Service.SquadController.WarManager.CurrentSquadWar;
			uint serverTime = Service.ServerAPI.ServerTime;
			this.phaseTimeRemaining = currentSquadWar.ActionGraceStartTimeStamp - (int)serverTime;
			this.labelActionAndCooldownTimer.TextColor = SquadWarScreen.TEXT_ACTION_TIMER_COLOR;
			this.SetTimeRemaining(this.phaseTimeRemaining, SquadWarStatusType.PhaseAction);
			this.SetBuffBaseUI(SquadWarStatusType.PhaseAction);
			this.CloseGracePeriodAlertScreen();
		}

		private void ShowActionGracePhase()
		{
			this.ShowActionAndCooldownPhaseCommonUI();
			SquadWarParticipantState currentParticipantState = Service.SquadController.WarManager.GetCurrentParticipantState();
			if (currentParticipantState != null)
			{
				this.labelAttacksRemaining.Text = this.lang.Get("WAR_PLAYER_DETAILS_TURNS_LEFT", new object[]
				{
					currentParticipantState.TurnsLeft
				});
				this.labelAttacksRemaining.Visible = true;
				this.attacksRemainingBg.Visible = true;
			}
			SquadWarData currentSquadWar = Service.SquadController.WarManager.CurrentSquadWar;
			uint serverTime = Service.ServerAPI.ServerTime;
			this.phaseTimeRemaining = currentSquadWar.ActionEndTimeStamp - (int)serverTime;
			this.labelActionAndCooldownTimer.TextColor = SquadWarScreen.TEXT_ACTION_TIMER_COLOR;
			this.SetTimeRemaining(this.phaseTimeRemaining, SquadWarStatusType.PhaseActionGrace);
			this.SetBuffBaseUI(SquadWarStatusType.PhaseActionGrace);
			this.ShowGracePeriodAlertScreen(SquadWarStatusType.PhaseActionGrace, currentSquadWar.ActionEndTimeStamp);
		}

		private void ShowCooldownPhase()
		{
			this.ShowActionAndCooldownPhaseCommonUI();
			SquadWarData currentSquadWar = Service.SquadController.WarManager.CurrentSquadWar;
			uint serverTime = Service.ServerAPI.ServerTime;
			this.phaseTimeRemaining = currentSquadWar.CooldownEndTimeStamp - (int)serverTime;
			this.labelActionAndCooldownTimer.TextColor = SquadWarScreen.TEXT_COOLDOWN_TIMER_COLOR;
			this.SetTimeRemaining(this.phaseTimeRemaining, SquadWarStatusType.PhaseCooldown);
			SquadController squadController = Service.SquadController;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			if (SquadUtils.CanStartMatchmakingPrep(squadController, buildingLookupController))
			{
				this.btnStartNewWar.Visible = true;
				this.labelBtnStartNewWar.Text = this.lang.Get("WAR_BOARD_START_NEW_WAR", new object[0]);
			}
			this.SetBuffBaseUI(SquadWarStatusType.PhaseCooldown);
			this.CloseGracePeriodAlertScreen();
			this.TryShowWarEndedScreen();
		}

		private void ShowActionAndCooldownPhaseCommonUI()
		{
			this.groupPrepPhase.Visible = false;
			this.groupFilter.Visible = true;
			this.groupWarTimer.Visible = true;
			this.dropdownPlayerGroup.Visible = true;
			this.dropdownOpponentGroup.Visible = true;
			this.UpdateWarElements();
		}

		private void OnGracePeriodAlertScreenClosed(object result, object cookie)
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarStatusType currentStatus = warManager.GetCurrentStatus();
			if (currentStatus == SquadWarStatusType.PhaseActionGrace || currentStatus == SquadWarStatusType.PhasePrepGrace)
			{
				this.CloseSquadWarScreen(null);
			}
		}

		private void CloseGracePeriodAlertScreen()
		{
			TimerAlertScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<TimerAlertScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.Close(null);
			}
		}

		private void TryShowWarEndedScreen()
		{
			Service.SquadController.WarManager.ShowWarEndedScreen();
		}

		private void UpdateWarElements()
		{
			this.UpdateSquadMemberGrids();
			this.UpdateSquadWarStats();
		}

		private string GetFactionString(SquadWarSquadData squadData)
		{
			if (squadData.Faction == FactionType.Empire)
			{
				return "Empire";
			}
			if (squadData.Faction == FactionType.Rebel)
			{
				return "Rebel";
			}
			return null;
		}

		private string GetSquadTypeString(SquadWarSquadType squadType)
		{
			if (squadType == SquadWarSquadType.PLAYER_SQUAD)
			{
				return "Player";
			}
			return "Opponent";
		}

		private void UpdateSquadMemberGrids()
		{
			this.PopulateSquadMemberGrid(SquadWarSquadType.PLAYER_SQUAD);
			this.PopulateSquadMemberGrid(SquadWarSquadType.OPPONENT_SQUAD);
		}

		private void PopulateSquadMemberGrid(SquadWarSquadType squadType)
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarSquadData squadData = warManager.GetSquadData(squadType);
			if (squadData == null)
			{
				return;
			}
			string factionString = this.GetFactionString(squadData);
			if (string.IsNullOrEmpty(factionString))
			{
				return;
			}
			string squadTypeString = this.GetSquadTypeString(squadType);
			UXGrid uXGrid;
			if (squadType == SquadWarSquadType.PLAYER_SQUAD)
			{
				uXGrid = this.playerGrid;
			}
			else
			{
				uXGrid = this.opponentGrid;
			}
			uXGrid.Clear();
			uXGrid.DupeOrdersAllowed = true;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < squadData.Participants.Count; i++)
			{
				SquadWarParticipantState participantState = warManager.GetParticipantState(i, squadType);
				UXElement item = this.CreateSquadMemberGridItem(uXGrid, participantState, factionString, squadTypeString);
				num += participantState.VictoryPointsLeft;
				num2 += participantState.TurnsLeft;
				uXGrid.AddItem(item, 10000 / participantState.HQLevel + i);
			}
			uXGrid.RepositionItems();
			UXLabel element = base.GetElement<UXLabel>("LabelPlayerStatsTotalAttacks" + squadTypeString);
			element.Text = num2.ToString();
			UXLabel element2 = base.GetElement<UXLabel>("LabelPlayerStatsTotalStars" + squadTypeString);
			element2.Text = num.ToString();
			string text = squadTypeString;
			if (squadData.Faction == FactionType.Empire)
			{
				text += "Emp";
			}
			else if (squadData.Faction == FactionType.Rebel)
			{
				text += "Reb";
			}
			UXLabel element3 = base.GetElement<UXLabel>("LabelDropdownAttacks" + text);
			element3.Text = this.lang.Get("WAR_BOARD_TOTAL_ATTACKS_REMAINING", new object[]
			{
				num2
			});
		}

		private UXElement CreateSquadMemberGridItem(UXGrid grid, SquadWarParticipantState participantData, string faction, string squadType)
		{
			string squadMemberId = participantData.SquadMemberId;
			UXElement uXElement = grid.CloneTemplateItem(squadMemberId);
			grid.AddItem(uXElement, 0);
			uXElement.Tag = squadMemberId;
			UXLabel subElement = grid.GetSubElement<UXLabel>(squadMemberId, "LabelPlayerName" + squadType);
			subElement.Text = this.lang.Get("WAR_BOARD_HQ_LABEL", new object[]
			{
				participantData.SquadMemberName,
				participantData.HQLevel
			});
			UXLabel subElement2 = grid.GetSubElement<UXLabel>(squadMemberId, "LabelAttacksRemaining" + squadType);
			subElement2.Text = participantData.TurnsLeft.ToString();
			UXLabel subElement3 = grid.GetSubElement<UXLabel>(squadMemberId, "LabelStarsAvailable" + squadType);
			subElement3.Text = participantData.VictoryPointsLeft.ToString();
			string playerId = Service.CurrentPlayer.PlayerId;
			UXSprite subElement4 = grid.GetSubElement<UXSprite>(squadMemberId, "SpritePlayerHighlight" + squadType);
			subElement4.Visible = (participantData.SquadMemberId == playerId);
			string text = "SpriteIcon" + squadType;
			UXSprite subElement5 = grid.GetSubElement<UXSprite>(squadMemberId, text);
			text = "SpriteIconZero" + squadType;
			string name = text + "Emp";
			string name2 = text + "Reb";
			UXSprite subElement6 = grid.GetSubElement<UXSprite>(squadMemberId, name);
			UXSprite subElement7 = grid.GetSubElement<UXSprite>(squadMemberId, name2);
			int rating = GameUtils.CalculateVictoryRating(participantData.AttacksWon, participantData.DefensesWon);
			FactionIconUpgradeController factionIconUpgradeController = Service.FactionIconUpgradeController;
			string icon = factionIconUpgradeController.GetIcon(faction, rating);
			if (factionIconUpgradeController.UseUpgradeImage(rating))
			{
				subElement5.Visible = true;
				subElement6.Visible = false;
				subElement7.Visible = false;
				subElement5.SpriteName = icon;
			}
			else
			{
				subElement5.Visible = false;
				bool flag = faction == FactionType.Empire.ToString();
				subElement6.Visible = flag;
				subElement7.Visible = !flag;
				subElement6.SpriteName = icon;
				subElement7.SpriteName = icon;
			}
			UXButton uXButton = uXElement as UXButton;
			if (uXButton != null)
			{
				uXButton.OnClicked = new UXButtonClickedDelegate(this.OnSquadMemberItemButtonClicked);
			}
			return uXElement;
		}

		private void CreateBuffBaseElement(UXGrid grid, WarBuffVO warBuffVO, string renderSpriteName, SquadWarBuffBaseData buffBaseData, string buffIconName, string infoLableName, string spriteLockName, SquadWarSquadData squad)
		{
			UXCheckbox uXCheckbox = (UXCheckbox)grid.CloneTemplateItem(warBuffVO.Uid);
			grid.AddItem(uXCheckbox, grid.Count);
			uXCheckbox.Tag = buffBaseData;
			uXCheckbox.Enabled = true;
			uXCheckbox.OnSelected = new UXCheckboxSelectedDelegate(this.OnBuffBaseClicked);
			PlanetVO vo = Service.StaticDataController.Get<PlanetVO>(warBuffVO.PlanetId);
			UXSprite subElement = grid.GetSubElement<UXSprite>(warBuffVO.Uid, buffIconName);
			subElement.SpriteName = warBuffVO.BuffIcon;
			UXLabel subElement2 = grid.GetSubElement<UXLabel>(warBuffVO.Uid, infoLableName);
			subElement2.Text = this.lang.Get("BUFF_BASE_INFO_LEVEL_SHORT", new object[]
			{
				buffBaseData.GetDisplayBaseLevel()
			});
			UXSprite subElement3 = grid.GetSubElement<UXSprite>(warBuffVO.Uid, spriteLockName);
			subElement3.Visible = !Service.CurrentPlayer.IsPlanetUnlocked(warBuffVO.PlanetId);
			UXSprite subElement4 = grid.GetSubElement<UXSprite>(warBuffVO.Uid, renderSpriteName);
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(vo, subElement4);
			projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			this.projectors.Add(ProjectorUtils.GenerateProjector(projectorConfig));
			if (squad != null)
			{
				bool flag = squad.Faction == FactionType.Empire;
				if (grid == this.gridBuffOpponentPlanets)
				{
					this.groupCapturedGlowOpponentRebel = grid.GetSubElement<UXElement>(warBuffVO.Uid, "CapturedGlowOpponentReb");
					this.groupCapturedGlowOpponentEmpire = grid.GetSubElement<UXElement>(warBuffVO.Uid, "CapturedGlowOpponentEmp");
					this.groupCapturedGlowOpponentRebel.Visible = !flag;
					this.groupCapturedGlowOpponentEmpire.Visible = flag;
				}
				else if (grid == this.gridBuffPlayerPlanets)
				{
					this.groupCapturedGlowPlayerRebel = grid.GetSubElement<UXElement>(warBuffVO.Uid, "CapturedGlowPlayerReb");
					this.groupCapturedGlowPlayerEmpire = grid.GetSubElement<UXElement>(warBuffVO.Uid, "CapturedGlowPlayerEmp");
					this.groupCapturedGlowPlayerRebel.Visible = !flag;
					this.groupCapturedGlowPlayerEmpire.Visible = flag;
				}
			}
		}

		private void DisableAllBuffBaseGrids()
		{
			this.DisableBuffBasesInGrid(this.gridBuffNeutralPlanets);
			this.DisableBuffBasesInGrid(this.gridBuffOpponentPlanets);
			this.DisableBuffBasesInGrid(this.gridBuffPlayerPlanets);
		}

		private void DisableBuffBasesInGrid(UXGrid grid)
		{
			List<UXElement> elementList = grid.GetElementList();
			int i = 0;
			int count = elementList.Count;
			while (i < count)
			{
				UXCheckbox uXCheckbox = elementList[i] as UXCheckbox;
				uXCheckbox.Enabled = false;
				i++;
			}
		}

		private void DeselectAllBuffBases()
		{
			this.DeselectBuffBasesInGrid(this.gridBuffNeutralPlanets);
			this.DeselectBuffBasesInGrid(this.gridBuffOpponentPlanets);
			this.DeselectBuffBasesInGrid(this.gridBuffPlayerPlanets);
		}

		private void DeselectBuffBasesInGrid(UXGrid grid)
		{
			List<UXElement> elementList = grid.GetElementList();
			int i = 0;
			int count = elementList.Count;
			while (i < count)
			{
				UXCheckbox uXCheckbox = elementList[i] as UXCheckbox;
				uXCheckbox.Selected = false;
				i++;
			}
		}

		private void OnBuffBaseClicked(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			Service.EventManager.SendEvent(EventId.WarBoardBuffBaseBuildingSelected, checkbox);
		}

		private void UpdateSquadWarStats()
		{
			this.UpdateSquadWarStats(SquadWarSquadType.PLAYER_SQUAD);
			this.UpdateSquadWarStats(SquadWarSquadType.OPPONENT_SQUAD);
		}

		private void UpdateSquadWarStats(SquadWarSquadType squadType)
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarSquadData squadData = warManager.GetSquadData(squadType);
			if (squadData == null)
			{
				return;
			}
			SquadWarStatusType currentStatus = warManager.GetCurrentStatus();
			bool visible = false;
			bool visible2 = false;
			int num = 0;
			if (currentStatus == SquadWarStatusType.PhasePrep || currentStatus == SquadWarStatusType.PhaseAction || currentStatus == SquadWarStatusType.PhaseCooldown)
			{
				SquadWarSquadType squadType2 = (squadType != SquadWarSquadType.PLAYER_SQUAD) ? SquadWarSquadType.PLAYER_SQUAD : SquadWarSquadType.OPPONENT_SQUAD;
				if (warManager.GetSquadData(squadType2) == null)
				{
					return;
				}
				visible = true;
				num = warManager.GetCurrentSquadScore(squadType);
				int currentSquadScore = warManager.GetCurrentSquadScore(squadType2);
				visible2 = (num > currentSquadScore);
			}
			string factionString = this.GetFactionString(squadData);
			if (string.IsNullOrEmpty(factionString))
			{
				return;
			}
			string squadTypeString = this.GetSquadTypeString(squadType);
			string text = squadTypeString;
			if (squadData.Faction == FactionType.Empire)
			{
				text += "Emp";
			}
			else if (squadData.Faction == FactionType.Rebel)
			{
				text += "Reb";
			}
			UXLabel element = base.GetElement<UXLabel>("LabelName" + text);
			element.Text = squadData.SquadName;
			UXLabel element2 = base.GetElement<UXLabel>("LabelBtnFilter" + text);
			element2.Text = squadData.SquadName;
			UXLabel element3 = base.GetElement<UXLabel>("LabelScore" + text);
			element3.Text = num.ToString();
			element3.Visible = visible;
			UXSprite element4 = base.GetElement<UXSprite>("SpriteWinning" + text);
			element4.Visible = visible2;
			UXLabel element5 = base.GetElement<UXLabel>("LabelListAvailableStars" + squadTypeString);
			element5.Text = this.lang.Get("WAR_PLAYER_DETAILS_POINTS_LEFT", new object[]
			{
				string.Empty
			});
			UXLabel element6 = base.GetElement<UXLabel>("LabelListAttacksRemaining" + squadTypeString);
			element6.Text = this.lang.Get("WAR_PLAYER_DETAILS_TURNS_LEFT", new object[]
			{
				string.Empty
			});
		}

		private void SetBattleMessageCount(int count)
		{
			if (count == 0)
			{
				this.groupBattleMessageCount.Visible = false;
			}
			else
			{
				this.groupBattleMessageCount.Visible = true;
				this.labelBattleMessageCount.Text = count.ToString();
			}
		}

		private void ShowGracePeriodAlertScreen(SquadWarStatusType status, int endTimeStamp)
		{
			string text = string.Empty;
			string timerTextUID = string.Empty;
			string messageUID = string.Empty;
			if (status != SquadWarStatusType.PhasePrepGrace)
			{
				if (status == SquadWarStatusType.PhaseActionGrace)
				{
					text = "WAR_BOARD_ACTION_GRACE_PHASE";
					timerTextUID = "WAR_BOARD_ACTION_PHASE_TIME_REMAINING";
					messageUID = "WAR_BOARD_ACTION_GRACE_PHASE_DESC";
				}
			}
			else
			{
				text = "WAR_BOARD_PREP_GRACE_PHASE";
				timerTextUID = "WAR_BOARD_PREP_PHASE_TIME_REMAINING";
				messageUID = "WAR_BOARD_PREP_GRACE_PHASE_DESC";
			}
			if (string.IsNullOrEmpty(text))
			{
				Service.Logger.Warn("Attempting to display Grace Period Alert Screen at inappropriate phase!!!");
				return;
			}
			TimerAlertScreen screen = new TimerAlertScreen(text, timerTextUID, messageUID, "WAR_OK", endTimeStamp, new OnScreenModalResult(this.OnGracePeriodAlertScreenClosed));
			Service.ScreenController.AddScreen(screen);
		}

		private void SetTimeRemaining(int seconds, SquadWarStatusType status)
		{
			seconds = Mathf.Max(0, seconds);
			UXLabel uXLabel = null;
			string id = string.Empty;
			switch (status)
			{
			case SquadWarStatusType.PhasePrep:
				id = "WAR_BOARD_PREP_PHASE_TIME_REMAINING";
				uXLabel = this.labelPrepTimer;
				break;
			case SquadWarStatusType.PhasePrepGrace:
				id = "WAR_BOARD_PREP_GRACE_PHASE";
				uXLabel = this.labelPrepTimer;
				break;
			case SquadWarStatusType.PhaseAction:
				id = "WAR_BOARD_ACTION_PHASE_TIME_REMAINING";
				uXLabel = this.labelActionAndCooldownTimer;
				break;
			case SquadWarStatusType.PhaseActionGrace:
				id = "WAR_BOARD_ACTION_GRACE_PHASE";
				uXLabel = this.labelActionAndCooldownTimer;
				break;
			case SquadWarStatusType.PhaseCooldown:
				id = "WAR_BOARD_COOLDOWN_PHASE_TIME_REMAINING";
				uXLabel = this.labelActionAndCooldownTimer;
				break;
			}
			if (uXLabel != null)
			{
				uXLabel.Text = this.lang.Get(id, new object[]
				{
					GameUtils.GetTimeLabelFromSeconds(seconds)
				});
			}
		}

		private void OnEditWarBaseButtonClicked(UXButton button)
		{
			WarBaseEditorState.GoToWarBaseEditorState();
		}

		private void OnRequestTroopsButtonClicked(UXButton button)
		{
			Service.SquadController.ShowTroopRequestScreen(null, true);
		}

		private void OnSquadMemberItemButtonClicked(UXButton button)
		{
			string squadMemberId = (string)button.Tag;
			Service.AudioManager.PlayAudio("sfx_button_next");
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarSquadType participantSquad = warManager.GetParticipantSquad(squadMemberId);
			this.RefreshFilterCheckboxForSquad(participantSquad);
			Service.WarBoardBuildingController.ShowWarBuildings(participantSquad, false);
			Service.WarBoardViewController.SelectAndCenterOn(squadMemberId);
		}

		private void OnFilterPlayerCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			Service.WarBoardBuildingController.ShowWarBuildings(SquadWarSquadType.PLAYER_SQUAD, true);
		}

		private void OnFilterOpponentCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			Service.WarBoardBuildingController.ShowWarBuildings(SquadWarSquadType.OPPONENT_SQUAD, true);
		}

		private void RefreshFilterCheckboxForSquad(SquadWarSquadType squadType)
		{
			bool flag = squadType == SquadWarSquadType.PLAYER_SQUAD;
			if (this.buttonFilterPlayer != null && this.buttonFilterOpponent != null)
			{
				this.buttonFilterPlayer.SetSelected(flag);
				this.buttonFilterOpponent.SetSelected(!flag);
			}
		}

		private void ToggleOpponentList(UXButton button)
		{
			this.panelOpponentList.Visible = true;
			if (this.dropdownOpponentGroup.IsCurrentAnimatorState("Show"))
			{
				Service.AudioManager.PlayAudio("sfx_ui_squadwar_dropdown_close");
				this.dropdownOpponentGroup.SetTrigger("hide");
			}
			else
			{
				Service.AudioManager.PlayAudio("sfx_ui_squadwar_dropdown_open");
				this.dropdownOpponentGroup.SetTrigger("show");
			}
		}

		private void TogglePlayerList(UXButton button)
		{
			this.panelPlayerList.Visible = true;
			if (this.dropdownPlayerGroup.IsCurrentAnimatorState("Show"))
			{
				Service.AudioManager.PlayAudio("sfx_ui_squadwar_dropdown_close");
				this.dropdownPlayerGroup.SetTrigger("hide");
			}
			else
			{
				Service.AudioManager.PlayAudio("sfx_ui_squadwar_dropdown_open");
				this.dropdownPlayerGroup.SetTrigger("show");
			}
		}

		private void OnStartNewWarClicked(UXButton button)
		{
			Service.SquadController.WarManager.StartMatchMakingPreparation();
		}
	}
}
