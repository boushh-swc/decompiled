using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Objectives;
using StaRTS.Main.Controllers.Planets;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX.Anchors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	public class MiscElementsManager : UXFactory, IViewFrameTimeObserver, IUserInputObserver, IEventObserver
	{
		private const float HEIGHT_OFFSET = 4f;

		private const float FINGER_ANIMATION_DURATION = 1f;

		private const float CONFIRM_BUTTON_SPACING_FACTOR = 0.75f;

		private const float TOOL_TIP_OFFSET = 4f;

		private const float SQUAD_LEVEL_TOOLTIP_OFFSET = 0.3f;

		private static readonly Vector3 OFF_SCREEN_OFFSET = new Vector3(10000f, 10000f, 0f);

		private const int FIRST_TICKER_INDEX = 0;

		private const float INSTRUCTIONS_SHOWTIME = 4f;

		private const float INSTRUCTIONS_FADETIME = 0.5f;

		private const string INSTRUCTIONS_PANEL_NAME = "Instructions Panel {0}";

		private const string INSTRUCTIONS_NAME = "{0} ({1})";

		public const string HEALTH_SLIDER_NAME = "HealthSlider";

		public const string SHIELD_SLIDER_NAME = "ShieldSlider";

		private const string SHOW = "Show";

		private const string SHOW_HUD = "ShowHud";

		private const string HIDE = "Hide";

		private const string HIDE_HUD = "HideHud";

		private const string OFF = "Off";

		private const string ACTIVE_CONFLICT_COLOR = "cc0000";

		private const string DEFAULT_CONFLICT_COLOR = "10b2e9";

		private const string CONFLICT_GAL_ATTACK_NOW = "CONFLICT_GAL_ATTACK_NOW";

		private const string CONFLICT_GAL_TOP = "CONFLICT_GAL_TOP";

		private const string CONFLICT_GAL_ENDED = "CONFLICT_GAL_ENDED";

		private const string CONFLICT_STATUS_TITLE_TEXT = "CONFLICT_STATUS_TITLE";

		private const string FACTION_ICON_DESCRIPTION = "FACTION_ICON_DESCRIPTION";

		private const string FACTION_ICON_LEVEL = "FACTION_ICON_LEVEL";

		private const string RAID_TIME_REMAINING_ACTIVE = "RAID_TIME_REMAINING_TICKER_ACTIVE";

		private const string RAID_TIME_REMAINING_INACTIVE = "RAID_TIME_REMAINING_TICKER_INACTIVE";

		private const int RAID_TICKER_INDEX = 0;

		private const string SQUAD_WAR_PREP_TIME_REMAINING = "WAR_BASE_PREP_TIME_REMAINING";

		private const string PERK_SQUAD_LEVEL = "PERK_SQUAD_LEVEL_TOOLTIP";

		private const string PERK_SQUAD_LEVEL_LABEL = "squadLevelToolTip";

		private const string SAME_FACTION_MM_LABEL = "sameFactionMatchMakingToolTip";

		private const int HEALTH_SLIDER_POOL_INITIAL_SIZE = 40;

		private const int SHIELD_SLIDER_POOL_INITIAL_SIZE = 10;

		private const string SCRIM_ELEMENT = "Scrim";

		private const string SCRIM_BUTTON = "ButtonScrim";

		private const string SCRIM_SPRITE = "SpriteBkgScrim";

		private const string SCREEN_TRANSITIONER = "DialogTransition";

		private const string HEALTH_SLIDER = "pBarHealth";

		private const string HEALTH_SHIELD_SLIDER = "pBarHealthShield";

		private const string GENERAL_TOOLTIP = "Info";

		private const string BUBBLE_TOOLTIP = "Tooltip";

		private const string BUBBLE_REPAIR_TOOLTIP = "RepairTooltip";

		private const string PROGRESS_TOOLTIP = "Progress";

		private const string ICONPROGRESS_TOOLTIP = "Training";

		private const string TOOLTIP_LABEL = "TooltipLabel";

		private const string HUD_TOOLTIP = "HudToolTip";

		private const string HUD_TOOLTIP_PANEL = "HudToolTipPanel";

		private const string HUD_TOOLTIP_LABEL = "LabelHudToolTip";

		private const string HUD_FACTION_ICON_TOOLTIP = "HudToolTipFactionUp";

		private const string HUD_FACTION_ICON_DESC_LABEL = "LabelVictoriesToNextFactionUp";

		private const string HUD_FACTION_ICON_PROG_LABEL = "LabelHudToolTipFactionUp";

		private const string HUD_FACTION_ICON_NEXT_LEVEL_SPRITE = "SpriteFactionIconUp";

		private const string OBJECTIVE_TOAST = "Toaster";

		private const string OBJECTIVE_WIDGET_TOASTER = "WidgetToaster";

		private const string OBJECTIVE_TOAST_TITLE_LABEL = "LabelObjective";

		private const string OBJECTIVE_TOAST_CRATE_INFO_LABEL = "LabelObjectiveSupplyCrate";

		private const string OBJECTIVE_TOAST_STATUS_LABEL = "LabelObjectiveStatus";

		private const string OBJECTIVE_TOAST_DETAIL_BUTTON = "BtnDetails";

		private const string OBJECTIVE_TOAST_CLOSE_BUTTON = "BtnClose";

		private const string OBJECTIVE_TOAST_PLANET_BACKGROUND = "TexturePlanetBg";

		private const string CANCEL_BUTTON = "buttonCancel";

		private const string ACCEPT_BUTTON = "buttonAccept";

		private const string CANCEL_BUTTON_PARENT = "objectCancelPlacement";

		private const string ACCEPT_BUTTON_PARENT = "objectAcceptPlacement";

		private const string ACCEPT_DIM_SPRITE = "SpriteDimAccept";

		private const string CANCEL_DIM_SPRITE = "SpriteDimCancel";

		private const string FINGER_ANIMATION = "FingerAnimation";

		private const string BUTTON_HIGHLIGHT = "FUETileHighlight";

		private const string PLAYER_INSTRUCTIONS = "LabelInstructions";

		private const string PLAYER_INSTRUCTIONS_PANEL = "PanelLabelInstructions";

		private const string PLAYER_INSTRUCTIONS_WAR_BOARD = "LabelWarboardText";

		private const string PLAYER_INSTRUCTIONS_WAR_BOARD_PANEL = "WarboardText";

		private const string LONG_PRESS = "LongPressAnimation";

		private const string DEBUG_CURSOR = "tapTarget";

		private const string COLLECTION_LABEL = "CollectionLabel";

		private const string TROOP_TOOLTIP = "TroopTooltip";

		private const string TROOP_TOOLTIP_LABEL = "LabelTroopTooltip";

		private const string INNER_SPINNER = "SpriteInnerCircler";

		private const string CONFIRM_GROUP_NAME = "Confirm Group";

		private const string NAME_ARROW_HOLDER = "OuterArrowHolder";

		private const string NAME_BORDER = "Border";

		private const string CIRCLE_BORDER = "BorderCircular";

		private const string OBJECTIVE_BORDER = "BorderCircle";

		private const string NAME_CUSTOM_HIGHLIGHT = "{0}Highlight";

		private const float HIDE_ANIMATION_LENGTH = 0.5f;

		public const string ARROW_TOP_RIGHT = "topright";

		public const string ARROW_BOTTOM_RIGHT = "bottomright";

		public const string ARROW_BOTTOM_LEFT = "bottomleft";

		private bool startedHighlightAnim;

		private uint hideHighlightTimerId;

		private RegionHighlight regionHighlightMover;

		private Transform buttonHighlightArrow;

		private const string CURRENCY_TRAY = "CurrencyTray";

		private const string CURRENCY_TRAY_BG = "CurrencyTrayBg";

		private const string CURRENCY_TRAY_TABLE = "CurrencyTable";

		private const string CURRENCY_TRAY_TEMPLATE = "CurrencyTemplate";

		private const string CURRENCY_TRAY_ICON = "CurrencyIcon{0}";

		private const string CURRENCY_TRAY_LABEL = "CurrencyLabel";

		private const int CURRENCY_TRAY_PADDING_X = 50;

		private const string CURRENCY_CREDITS = "Credit";

		private const string CURRENCY_MATERIALS = "Alloy";

		private const string CURRENCY_CRYSTALS = "Crystal";

		private const string CURRENCY_CONTRABAND = "Contraband";

		private const string CURRENCY_CAMPAIGN_POINTS = "Points";

		private const string CURRENCY_REPUTATION = "Reputation";

		public static readonly string[] CURRENCY_LIST = new string[]
		{
			"Credit",
			"Alloy",
			"Contraband",
			"Points",
			"Crystal",
			"Reputation"
		};

		private const string NAME_TROOP_COUNTER = "TroopCount";

		private const string NAME_TROOP_COUNTER_LABEL = "LabelTroopCount";

		private UXElement troopCounter;

		private UXLabel troopCounterLabel;

		private const string PLANET_ANCHOR_TOP_RIGHT = "AnchorTopRight";

		private const string PLANET_BTN_CLOSE_HOLDER = "BtnCloseGalaxyHolder";

		private const string GALAXY_BTN_CLOSE = "BtnCloseGalaxy";

		private const float OBJECTIVE_TOAST_DISPLAY_TIME = 3f;

		private const float OBJECTIVE_TOAST_HIDE_TIME = 0.5f;

		private const float TOURNAMENT_STATUS_TITLE_ANIMATION_LENGTH = 0.5f;

		private const float TOURNAMENT_STATUS_SHOW_DELAY = 1f;

		private uint ConflictStatusTitleTimerId;

		private uint ConflictStatusShowTimerId;

		private const string PLANET_HIGHLIGHT_BACK = "PlanetHighlightBack";

		private const string PLANET_BACK_EVENT_HOLDER = "EventHolder";

		private const string PLANET_BACK_EVENT_TITLE = "LabelEventTitleBack";

		private const string PLANET_BACK_EVENT_TIMER = "LabelEventTimerBack";

		private const string PLANET_BACK_GROUP_NAME = "PlanetBackGroup";

		private const string PLANET_BACK_SPRITE_INNER_CIRCLE = "PlanetSize";

		private const string PLANET_CONFLICT_BACK = "PLANETS_CONFLICT_BACK";

		private const string PLANET_HIGHLIGHT_IMMINENT = "PlanetHighlightImminent";

		private const string PLANET_IMMINENT_EVENT_HOLDER = "EventHolderImminent";

		private const string PLANET_IMMINENT_EVENT_TITLE = "LabelEventTitleBackImminent";

		private const string PLANET_IMMINENT_EVENT_TIMER = "LabelEventTimerBackImminent";

		private const string PLANET_IMMINENT_GROUP_NAME = "PlanetImminentGroup";

		private const string PLANET_IMMINENT_SPRITE_INNER_CIRCLE = "PlanetSizeImminent";

		private const string PLANET_HIGHLIGHT_FRONT = "PlanetHighlightFront";

		private const string PLANET_FRONT_PLANET_NAME = "LabelPlanetName";

		private const string PLANET_FRONT_SIZE = "PlanetSizeFront";

		private const string PLANET_IMMINENT_CURRENT = "LabelCurrentBackImminent";

		private const string PLANET_BACK_CURRENT = "LabelCurrentBack";

		private const string HIDE_BACK_CURRENT_TRIGGER = "CurrentNoEventHide";

		private const string SHOW_BACK_CURRENT_TRIGGER = "CurrentNoEventShow";

		private const string PLANET_FRONT_CURRENT = "LabelCurrentFront";

		private const string PLANET_LOCKED_GROUP = "PlanetLockedGroup";

		private const string PLANET_LOCKED = "PlanetLockedIcon";

		private const string PLANET_LOCKED_SIZE = "PlanetSizeLocked";

		private const string PLANET_FRIENDS_GROUP = "Friends";

		private const string PLANET_FRIENDS_GRID = "GridInfoFriends";

		private const string PLANET_FRIENDS_GRID_TEMPLATE = "FriendThumb";

		private const string PLANET_FRIENDS_PIC_TEXTURE = "FriendPic";

		private const int PLANET_FRIENDS_MAX_THUMBS = 5;

		private const string PLANET_FRONT_CIRCLE_FULL = "SpriteCircleFull";

		private const string PLANET_FRONT_CIRCLE_PIECE1 = "SpriteCirclePiece1";

		private const string PLANET_FRONT_CIRCLE_PIECE2 = "SpriteCirclePiece2";

		private const string PLANET_FRONT_CIRCLE_PIECE3 = "SpriteCirclePiece3";

		private const string PLANET_FRONT_LINE_LEFT = "SpritePlanetLineL";

		private const string PLANET_FRONT_LINE_RIGHT = "SpritePlanetLineR";

		private const string PLANET_FRONT_STATS_COUNT = "LabelPlanetStatsCount";

		private const string PLANET_FRONT_EVENT = "EventInfo";

		private const string PLANET_FRONT_EVENT_TITLE = "LabelEventTitleFront";

		private const string PLANET_FRONT_EVENT_TIMER = "LabelEventTimerFront";

		private const string PLANET_FRONT_GROUP_NAME = "PlanetFrontGroup";

		private const string PLANET_FRONT_NO_FRIENDS = "NoFriendsHolder";

		private const string PLANET_FRONT_SQUAD_COUNT = "LabelSquadmatesCount";

		private const string HUD_BUFF_TOOLTIP_GROUP = "BuffsInfo";

		private const string HUD_BUFF_TOOLTIP_TABLE = "TableBuffsInfo";

		private const string HUD_BUFF_TOOLTIP_TEMPLATE = "CardBuffsInfo";

		private const string HUD_BUFF_TOOLTIP_ITEM_ICON = "SpriteIconBuffsInfo";

		private const string HUD_BUFF_TOOLTIP_ITEM_TITLE = "LabelTitleBuffsInfo";

		private const string HUD_BUFF_TOOLTIP_ITEM_BODY = "LabelBodyBuffsInfo";

		private const string HUD_BUFF_TOOLTIP_BG = "SpriteBgBuffsInfo";

		private AssetsCompleteDelegate onCompleteCallback;

		private object onCompleteCookie;

		private UXElement scrimElement;

		private UXButton scrimButton;

		private UXSprite scrimSprite;

		private UXSlider healthSlider;

		private UXSlider healthShieldSlider;

		private UXElement generalTooltip;

		private UXElement bubbleTooltip;

		private UXElement bubbleRepairTooltip;

		private UXElement progressTooltip;

		private UXElement iconProgressTooltip;

		private UXElement hudTooltip;

		private UXLabel hudTooltipLabel;

		private bool wasHudToolTipVisible;

		private bool wasHudBuffToolTipVisible;

		private bool wasHudFactionToolTipVisible;

		private UXElement hudFactionIconTooltip;

		private UXSprite hudFactionIconTooltipSprite;

		private UXLabel hudFactionIconDesc;

		private UXLabel hudFactionIconProg;

		private UXLabel gameBoardLabel;

		private UXLabel collectionLabel;

		private UXButton cancelButton;

		private UXButton acceptButton;

		private UXElement fingerAnimation;

		private UXElement buttonHighlight;

		private UXElement buttonHighlightTarget;

		private float buttonHightlightAge;

		private UXLabel playerInstructions;

		private UXElement playerInstructionsPanel;

		private UXLabel playerInstructionsWarBoard;

		private UXElement playerInstructionsWarBoardPanel;

		private List<LabelFader> playerInstructionsList;

		private int playerInstructionsAutoInc;

		private UXElement longPress;

		private UXElement debugCursor;

		private UXElement troopTooltip;

		private UXLabel troopTooltipLabel;

		private UXElement squadLevelTooltip;

		private UXLabel squadLevelTooltipLabel;

		private bool wasSquadLevelToolTipVisible;

		private UIPanel squadLevelTooltipPanel;

		private UXElement sameFactionMatchMakingTooltip;

		private UXLabel sameFactionMatchMakingTooltipLabel;

		private bool wasSameFactionMatchMakingToolTipVisible;

		private UXElement hudBuffToolTip;

		private UXTable hudBuffToolTipTable;

		private UXSprite hudBuffToolTipBG;

		private float hudBuffToolTipBGHeight;

		private UXElement objectiveToast;

		private UXElement objectiveWidgetToaster;

		private UXLabel objectiveToastLabel;

		private UXLabel objectiveToastCrateLabel;

		private UXLabel objectiveToastStatusLabel;

		private UXButton objectiveToastClose;

		private uint objectiveDisplayTimerId;

		private uint objectiveDisplayHideTimerId;

		private bool isObjectiveToastVisible;

		private bool checkPendingObjectiveToast;

		private GameObject confirmGroup;

		private UXElement cancelButtonParent;

		private UXElement acceptButtonParent;

		private UXSprite cancelButtonDimSprite;

		private UXSprite acceptButtonDimSprite;

		private UXElement currencyTray;

		private UXTable currencyTrayTable;

		private UXSprite currencyTrayBackground;

		private CurrencyTrayType currencyTrayType;

		private Action closePlanetsGalaxyCallback;

		private UXElement planetTopRightAnchor;

		private UXButton galaxyCloseBtn;

		private EventTickerView eventTickerView;

		private bool raidAvailable;

		private bool isShowingRaidTicker;

		private bool isShowingSquadWarTicker;

		private string raidTimerText;

		private uint raidTimerId;

		private RaidDefenseController raidDefenseController;

		private UXElement planetHighlightFront;

		private UXSprite spinner;

		private UXLabel planetFrontPlanetName;

		private Vector2 planetFrontSize;

		private Vector3 planetScale;

		private Vector3 planetFrontPos;

		private UXLabel planetFrontStatsCount;

		private UXElement planetFrontEventInfo;

		private UXLabel planetFrontEventTitle;

		private UXLabel planetFrontEventTimer;

		private UXLabel planetFrontCurrent;

		private UXElement planetNoFriends;

		private UXLabel planetSquadCount;

		private UXSprite planetFrontFullCircle;

		private UXSprite planetFrontCirclePiece1;

		private UXSprite planetFrontCirclePiece2;

		private UXSprite planetFrontCirclePiece3;

		private UXSprite planetFrontLineLeft;

		private UXSprite planetFrontLineRight;

		private Color planetFrontDefaultTextColor;

		private Color planetFrontUpcomingTextColor;

		private Color planetFrontActiveTextColor;

		private Color planetFrontDefaultSpriteColor;

		private Color planetFrontUpcomingSpriteColor;

		private Color planetFrontActiveSpriteColor;

		private UXElement planetFriendsGroup;

		private UXGrid planetFriendsGrid;

		private List<Texture2D> planetFriendPicTextures;

		private Animator planetFrontAnim;

		private GameObject planetFrontGroup;

		private TimedEventCountdownHelper frontPlanetEventCountdown;

		private Planet frontPlanetData;

		private GalaxyFrontPlanetUIState frontPlanetUIState;

		private GameObject attachedPlanetFrontObject;

		private List<GameObjectElementPair> planetBackUIList;

		private Vector2 planetBackCircleSize;

		private Vector2 planetImminentCircleSize;

		private Vector3 planetBackScale;

		private Vector3 planetBackPos;

		private List<TimedEventCountdownHelper> eventCountdowns;

		private Vector2 planetLockedSize;

		private Vector3 planetLockedPos;

		private Vector3 planetLockedScale;

		private List<GameObjectElementPair> planetLockedUIList;

		private Queue<GameObject>[] tooltipPool;

		private Queue<UXSlider> healthSliderPool;

		private Queue<UXSlider> shieldSliderPool;

		private Entity confirmBuilding;

		private MiscConfirmDelegate confirmCallback;

		private float heightOffGround;

		private float fingerX;

		private float fingerZ;

		private float fingerAge;

		private bool fingerScreen;

		private GameObject transitioner;

		private MainCamera mainCamera;

		private AssetHandle assetHandle;

		private int observerCount;

		private int tournamentDisplayCount;

		private bool tournamentDisplayActive;

		private Vector3 hudBuffToolTipLocalOffsetPos;

		public bool EnableCancelBuildingPlacement
		{
			get;
			set;
		}

		public MiscElementsManager(AssetsCompleteDelegate onCompleteCallback, object onCompleteCookie) : base(Service.CameraManager.UXCamera)
		{
			this.onCompleteCallback = onCompleteCallback;
			this.onCompleteCookie = onCompleteCookie;
			this.EnableCancelBuildingPlacement = true;
			this.confirmBuilding = null;
			this.confirmCallback = null;
			this.heightOffGround = -1f;
			this.observerCount = 0;
			this.planetScale = Vector3.zero;
			this.planetFrontPos = Vector3.zero;
			this.planetBackScale = Vector3.zero;
			this.planetBackPos = Vector3.zero;
			this.planetLockedScale = Vector3.zero;
			this.planetLockedPos = Vector3.zero;
			this.playerInstructionsList = new List<LabelFader>();
			this.playerInstructionsAutoInc = 0;
			this.hudBuffToolTipLocalOffsetPos = Vector3.zero;
			this.mainCamera = Service.CameraManager.MainCamera;
			this.tooltipPool = new Queue<GameObject>[7];
			this.planetBackUIList = new List<GameObjectElementPair>();
			this.planetLockedUIList = new List<GameObjectElementPair>();
			this.eventCountdowns = new List<TimedEventCountdownHelper>();
			this.planetFriendPicTextures = new List<Texture2D>();
			this.planetFrontDefaultTextColor = new Color(0.0623f, 0.698f, 0.937f);
			this.planetFrontUpcomingTextColor = new Color(1f, 0.753f, 0f);
			this.planetFrontActiveTextColor = new Color(0.937f, 0f, 0f);
			this.planetFrontDefaultSpriteColor = this.planetFrontDefaultTextColor;
			this.planetFrontUpcomingSpriteColor = this.planetFrontUpcomingTextColor;
			this.planetFrontActiveSpriteColor = this.planetFrontActiveTextColor;
			this.raidTimerId = 0u;
			this.hudFactionIconTooltip = null;
			base.Load(ref this.assetHandle, "gui_misc", new UXFactoryLoadDelegate(this.LoadSuccess), new UXFactoryLoadDelegate(this.LoadFailure), null);
		}

		public override void OnDestroyElement()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				base.Unload(this.assetHandle, "gui_misc");
				this.assetHandle = AssetHandle.Invalid;
			}
			Service.UserInputManager.UnregisterObserver(this, UserInputLayer.Screen);
			Service.EventManager.UnregisterObserver(this, EventId.SquadAdvancementTabSelected);
			this.CancelWaitForRaidTime();
			base.OnDestroyElement();
		}

		public void DestroyMiscElement(UXElement element)
		{
			base.DestroyElement(element);
		}

		private void LoadSuccess(object cookie)
		{
			this.currencyTray = base.GetElement<UXElement>("CurrencyTray");
			this.currencyTrayBackground = base.GetElement<UXSprite>("CurrencyTrayBg");
			this.currencyTrayTable = base.GetElement<UXTable>("CurrencyTable");
			this.currencyTrayTable.SetTemplateItem("CurrencyTemplate");
			for (int i = 0; i < MiscElementsManager.CURRENCY_LIST.Length; i++)
			{
				UXElement uXElement = this.currencyTrayTable.CloneTemplateItem(MiscElementsManager.CURRENCY_LIST[i]);
				uXElement.Tag = MiscElementsManager.CURRENCY_LIST[i];
				this.currencyTrayTable.AddItem(uXElement, i);
				for (int j = 0; j < MiscElementsManager.CURRENCY_LIST.Length; j++)
				{
					this.currencyTrayTable.GetSubElement<UXSprite>(MiscElementsManager.CURRENCY_LIST[i], string.Format("CurrencyIcon{0}", MiscElementsManager.CURRENCY_LIST[j])).Visible = (i == j);
				}
			}
			this.currencyTrayTable.RepositionItemsFrameDelayed();
			this.scrimElement = base.GetElement<UXElement>("Scrim");
			this.scrimButton = base.GetElement<UXButton>("ButtonScrim");
			this.scrimSprite = base.GetElement<UXSprite>("SpriteBkgScrim");
			this.healthSlider = base.GetElement<UXSlider>("pBarHealth");
			this.healthShieldSlider = base.GetElement<UXSlider>("pBarHealthShield");
			this.generalTooltip = base.GetElement<UXElement>("Info");
			this.bubbleTooltip = base.GetElement<UXElement>("Tooltip");
			this.bubbleRepairTooltip = base.GetElement<UXElement>("RepairTooltip");
			this.progressTooltip = base.GetElement<UXElement>("Progress");
			this.iconProgressTooltip = base.GetElement<UXElement>("Training");
			this.hudTooltip = base.GetElement<UXElement>("HudToolTip");
			this.hudTooltip.Visible = false;
			this.hudTooltipLabel = base.GetElement<UXLabel>("LabelHudToolTip");
			this.InitSquadLevelTooltip();
			this.InitSameFactionMatchMakingTooltip();
			this.hudFactionIconTooltip = base.GetElement<UXElement>("HudToolTipFactionUp");
			this.hudFactionIconTooltip.Visible = false;
			Service.EventManager.SendEvent(EventId.HUDFactionTooltipVisible, this.hudFactionIconTooltip.Visible);
			this.hudFactionIconTooltipSprite = base.GetElement<UXSprite>("SpriteFactionIconUp");
			this.hudFactionIconDesc = base.GetElement<UXLabel>("LabelVictoriesToNextFactionUp");
			this.hudFactionIconProg = base.GetElement<UXLabel>("LabelHudToolTipFactionUp");
			this.objectiveToast = base.GetElement<UXElement>("Toaster");
			this.objectiveToastLabel = base.GetElement<UXLabel>("LabelObjective");
			this.objectiveToastCrateLabel = base.GetElement<UXLabel>("LabelObjectiveSupplyCrate");
			this.objectiveToastStatusLabel = base.GetElement<UXLabel>("LabelObjectiveStatus");
			this.objectiveToastClose = base.GetElement<UXButton>("BtnClose");
			this.objectiveToast.Root.transform.parent = Service.UXController.WorldAnchor.Root.transform;
			this.objectiveToastClose.OnClicked = new UXButtonClickedDelegate(this.HideObjectiveToastButtonClicked);
			this.objectiveWidgetToaster = base.GetElement<UXElement>("WidgetToaster");
			this.objectiveWidgetToaster.Visible = false;
			Service.EventManager.RegisterObserver(this, EventId.ShowObjectiveToast, EventPriority.Default);
			this.gameBoardLabel = base.GetElement<UXLabel>("TooltipLabel");
			this.collectionLabel = base.GetElement<UXLabel>("CollectionLabel");
			this.cancelButton = base.GetElement<UXButton>("buttonCancel");
			this.cancelButton.OnClicked = new UXButtonClickedDelegate(this.OnConfirmButtonClicked);
			this.acceptButton = base.GetElement<UXButton>("buttonAccept");
			this.acceptButton.OnClicked = new UXButtonClickedDelegate(this.OnConfirmButtonClicked);
			this.fingerAnimation = base.GetElement<UXElement>("FingerAnimation");
			this.fingerAnimation.Visible = false;
			this.fingerAnimation.Parent = Service.UXController.WorldAnchor;
			this.buttonHighlight = base.GetElement<UXElement>("FUETileHighlight");
			this.buttonHighlightArrow = this.buttonHighlight.Root.transform.Find("OuterArrowHolder");
			this.buttonHighlight.WidgetDepth = 9999;
			this.buttonHighlight.Visible = false;
			this.buttonHighlightTarget = null;
			this.longPress = base.GetElement<UXElement>("LongPressAnimation");
			this.longPress.Visible = false;
			this.InitPlayerInstructionElements();
			this.troopCounter = base.GetElement<UXElement>("TroopCount");
			this.troopCounterLabel = base.GetElement<UXLabel>("LabelTroopCount");
			this.troopCounter.Visible = false;
			this.troopTooltip = base.GetElement<UXElement>("TroopTooltip");
			this.troopTooltipLabel = base.GetElement<UXLabel>("LabelTroopTooltip");
			this.troopTooltip.Visible = false;
			this.cancelButtonParent = base.GetElement<UXElement>("objectCancelPlacement");
			this.acceptButtonParent = base.GetElement<UXElement>("objectAcceptPlacement");
			this.debugCursor = base.GetElement<UXElement>("tapTarget");
			this.SetupScrim();
			this.SetupConfirmGroup();
			this.transitioner = base.GetElement<UXElement>("DialogTransition").Root;
			Service.UserInputManager.RegisterObserver(this, UserInputLayer.Screen);
			Service.EventManager.RegisterObserver(this, EventId.InventoryResourceUpdated, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.SquadAdvancementTabSelected, EventPriority.Default);
			this.planetFrontFullCircle = base.GetElement<UXSprite>("SpriteCircleFull");
			this.planetFrontCirclePiece1 = base.GetElement<UXSprite>("SpriteCirclePiece1");
			this.planetFrontCirclePiece2 = base.GetElement<UXSprite>("SpriteCirclePiece2");
			this.planetFrontCirclePiece3 = base.GetElement<UXSprite>("SpriteCirclePiece3");
			this.planetFrontLineLeft = base.GetElement<UXSprite>("SpritePlanetLineL");
			this.planetFrontLineRight = base.GetElement<UXSprite>("SpritePlanetLineR");
			this.planetTopRightAnchor = base.GetElement<UXElement>("AnchorTopRight");
			this.galaxyCloseBtn = base.GetElement<UXButton>("BtnCloseGalaxy");
			this.galaxyCloseBtn.OnClicked = new UXButtonClickedDelegate(this.OnPlanetsGalaxyCloseClicked);
			this.planetHighlightFront = base.GetElement<UXElement>("PlanetHighlightFront");
			this.planetFrontPlanetName = base.GetElement<UXLabel>("LabelPlanetName");
			this.planetFrontStatsCount = base.GetElement<UXLabel>("LabelPlanetStatsCount");
			this.planetFrontCurrent = base.GetElement<UXLabel>("LabelCurrentFront");
			this.planetFrontEventInfo = base.GetElement<UXElement>("EventInfo");
			this.planetFrontEventTitle = base.GetElement<UXLabel>("LabelEventTitleFront");
			this.planetFrontEventTimer = base.GetElement<UXLabel>("LabelEventTimerFront");
			UXElement element = base.GetElement<UXElement>("PlanetSizeFront");
			this.planetFrontSize = new Vector2(element.Width, element.Height);
			this.planetFrontAnim = this.planetHighlightFront.Root.GetComponent<Animator>();
			this.planetFriendsGroup = base.GetElement<UXElement>("Friends");
			this.planetFriendsGrid = base.GetElement<UXGrid>("GridInfoFriends");
			this.planetFriendsGrid.SetTemplateItem("FriendThumb");
			this.planetNoFriends = base.GetElement<UXElement>("NoFriendsHolder");
			this.planetSquadCount = base.GetElement<UXLabel>("LabelSquadmatesCount");
			UXElement element2 = base.GetElement<UXElement>("PlanetSize");
			this.planetBackCircleSize = new Vector2(element2.Width, element2.Height);
			UXElement element3 = base.GetElement<UXElement>("PlanetSizeLocked");
			this.planetLockedSize = new Vector2(element3.Width, element3.Height);
			UXElement element4 = base.GetElement<UXElement>("PlanetSizeImminent");
			this.planetImminentCircleSize = new Vector2(element4.Width, element4.Height);
			this.SetupPlanetGalaxyUI();
			this.hudBuffToolTip = base.GetElement<UXElement>("BuffsInfo");
			this.hudBuffToolTipTable = base.GetElement<UXTable>("TableBuffsInfo");
			this.hudBuffToolTipBG = base.GetElement<UXSprite>("SpriteBgBuffsInfo");
			this.hudBuffToolTipBGHeight = (float)this.hudBuffToolTipBG.GetUIWidget.height;
			this.Loaded();
		}

		private void InitSquadLevelTooltip()
		{
			this.squadLevelTooltip = base.CloneElement<UXElement>(this.hudTooltip, "squadLevelToolTip", this.hudTooltip.Root);
			this.squadLevelTooltip.Visible = false;
			string text = UXUtils.FormatAppendedName("LabelHudToolTip", "squadLevelToolTip");
			this.squadLevelTooltipLabel = base.GetElement<UXLabel>(text);
			if (this.squadLevelTooltipLabel == null)
			{
				Service.Logger.WarnFormat("Could not set up squad level tooltip label {0}", new object[]
				{
					text
				});
			}
			string text2 = UXUtils.FormatAppendedName("HudToolTipPanel", "squadLevelToolTip");
			UXElement element = base.GetElement<UXElement>(text2);
			this.squadLevelTooltipPanel = element.Root.GetComponent<UIPanel>();
			if (this.squadLevelTooltipPanel == null)
			{
				Service.Logger.WarnFormat("Could not set up squad level tooltip panel {0}", new object[]
				{
					text2
				});
			}
		}

		private void InitSameFactionMatchMakingTooltip()
		{
			this.sameFactionMatchMakingTooltip = base.CloneElement<UXElement>(this.hudTooltip, "sameFactionMatchMakingToolTip", this.hudTooltip.Root);
			this.sameFactionMatchMakingTooltip.Visible = false;
			string text = UXUtils.FormatAppendedName("LabelHudToolTip", "sameFactionMatchMakingToolTip");
			this.sameFactionMatchMakingTooltipLabel = base.GetElement<UXLabel>(text);
			if (this.sameFactionMatchMakingTooltipLabel == null)
			{
				Service.Logger.WarnFormat("Could not set up same faction MM tooltip label {0}", new object[]
				{
					text
				});
			}
		}

		private void InitPlayerInstructionElements()
		{
			this.playerInstructions = base.GetElement<UXLabel>("LabelInstructions");
			this.playerInstructionsPanel = base.GetElement<UXElement>("PanelLabelInstructions");
			this.SetupPlayerInstructionElements(this.playerInstructions, this.playerInstructionsPanel);
			this.playerInstructionsWarBoard = base.GetElement<UXLabel>("LabelWarboardText");
			this.playerInstructionsWarBoardPanel = base.GetElement<UXElement>("WarboardText");
			this.SetupPlayerInstructionElements(this.playerInstructionsWarBoard, this.playerInstructionsWarBoardPanel);
		}

		private void SetupPlayerInstructionElements(UXLabel label, UXElement panel)
		{
			label.Pivot = UIWidget.Pivot.Bottom;
			panel.Visible = false;
			panel.WidgetDepth = 9999;
			Service.CameraManager.UXCamera.AttachToMainAnchor(panel.Root);
		}

		private UXLabel GetPlayerInstructionsLabelBasedOnGameState()
		{
			return (!(Service.GameStateMachine.CurrentState is WarBoardState)) ? this.playerInstructions : this.playerInstructionsWarBoard;
		}

		private UXElement GetPlayerInstructionsPanelBasedOnGameState()
		{
			return (!(Service.GameStateMachine.CurrentState is WarBoardState)) ? this.playerInstructionsPanel : this.playerInstructionsWarBoardPanel;
		}

		private void SetFrontUIColorBasedOnEventState(TimedEventState state)
		{
			Color white = Color.white;
			Color white2 = Color.white;
			this.planetFrontStatsCount.TextColor = white;
			if (state != TimedEventState.Upcoming)
			{
				if (state != TimedEventState.Live)
				{
					white = this.planetFrontDefaultTextColor;
					white2 = this.planetFrontDefaultSpriteColor;
				}
				else
				{
					white = this.planetFrontActiveTextColor;
					white2 = this.planetFrontActiveSpriteColor;
				}
			}
			else
			{
				white = this.planetFrontUpcomingTextColor;
				white2 = this.planetFrontUpcomingSpriteColor;
			}
			this.planetFrontFullCircle.Color = white2;
			this.planetFrontCirclePiece1.Color = white2;
			this.planetFrontCirclePiece2.Color = white2;
			this.planetFrontCirclePiece3.Color = white2;
			this.planetFrontLineLeft.Color = white2;
			this.planetFrontLineRight.Color = white2;
			this.planetFrontEventTimer.TextColor = white;
		}

		public void SetGalaxyCloseButtonVisible(bool visible)
		{
			if (this.galaxyCloseBtn != null)
			{
				this.galaxyCloseBtn.Visible = visible;
			}
		}

		public void SetEventTickerViewVisible(bool visible)
		{
			if (visible)
			{
				this.ShowEventsTickerView();
			}
			else
			{
				this.HideEventsTickerView();
			}
		}

		public void ShowEventsTickerView()
		{
			if (Service.GameStateMachine.CurrentState is NeighborVisitState)
			{
				this.HideEventsTickerView();
				return;
			}
			if (Service.EpisodeController != null && Service.EpisodeController.IsEpisodeWidgetActive())
			{
				this.HideEventsTickerView();
				return;
			}
			if (this.eventTickerView == null)
			{
				this.eventTickerView = new EventTickerView(this);
			}
			this.HideGalaxyTournamentStatus();
			this.ShowGalaxyTournamentStatus();
			this.HideRaidsTickerStatus();
			this.ShowRaidsTickerStatus();
			this.HideSquadWarBeginStatus();
			this.ShowSquadWarBeginStatus();
		}

		public void HideEventsTickerView()
		{
			if (this.eventTickerView != null)
			{
				this.eventTickerView.DestroyElements();
				this.eventTickerView = null;
			}
			this.HideGalaxyTournamentStatus();
			this.HideRaidsTickerStatus();
			this.HideSquadWarBeginStatus();
		}

		private bool CanShowGalaxyTournamentStatus()
		{
			bool result = false;
			string pref = Service.SharedPlayerPrefs.GetPref<string>(GameConstants.PLANET_RELOCATED_TUTORIAL_ID);
			if (!string.IsNullOrEmpty(pref))
			{
				IState currentState = Service.GameStateMachine.CurrentState;
				result = (currentState is HomeState || currentState is GalaxyState);
			}
			return result;
		}

		private void ShowGalaxyTournamentStatus()
		{
			if (this.eventTickerView != null)
			{
				this.LoadConflictMedalsData();
			}
			if (!this.CanShowGalaxyTournamentStatus())
			{
				return;
			}
			if (this.tournamentDisplayActive && this.ConflictStatusTitleTimerId == 0u)
			{
				this.ConflictStatusTitleTimerId = Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.ShowGalaxyTournamentStatusCallBack), null);
			}
		}

		private void HideGalaxyTournamentStatus()
		{
			this.ResetAllConflictStatusTimers();
			this.tournamentDisplayActive = false;
		}

		private void ShowGalaxyTournamentStatusCallBack(uint timerId, object cookie)
		{
			this.ConflictStatusTitleTimerId = 0u;
			if (this.tournamentDisplayActive)
			{
				this.eventTickerView.AnimateStatusTitleText();
				this.ConflictStatusShowTimerId = Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.AnimateGalaxyTournamentStatus), null);
			}
		}

		private void AnimateGalaxyTournamentStatus(uint timerId, object cookie)
		{
			if (this.tournamentDisplayActive)
			{
				this.eventTickerView.AnimateTickerAtIndex(this.GetTotalTickerDisplayCount());
			}
		}

		public void ResetAllConflictStatusTimers()
		{
			Service.ViewTimerManager.KillViewTimer(this.ConflictStatusTitleTimerId);
			Service.ViewTimerManager.KillViewTimer(this.ConflictStatusShowTimerId);
			this.ConflictStatusTitleTimerId = 0u;
			this.ConflictStatusShowTimerId = 0u;
		}

		public void AddGalaxyTournamentStatus()
		{
			if (this.eventTickerView == null)
			{
				this.eventTickerView = new EventTickerView(this);
			}
			this.LoadConflictMedalsData();
		}

		public void RemoveGalaxyTournamentStatus()
		{
			this.HideEventsTickerView();
		}

		public bool IsGalaxyTournamentStatusVisible()
		{
			return this.tournamentDisplayActive;
		}

		private void OnGalaxyConflictStatusClicked(UXButton button)
		{
			if (button != null && button.Tag != null)
			{
				string text = (string)button.Tag;
				if (!string.IsNullOrEmpty(text))
				{
					IState currentState = Service.GameStateMachine.CurrentState;
					PlanetVO planetVO = Service.StaticDataController.Get<PlanetVO>(text);
					if (planetVO != null)
					{
						if (currentState is GalaxyState)
						{
							Service.EventManager.SendEvent(EventId.UIConflictStatusClicked, new ActionMessageBIData("galaxy", planetVO.PlanetBIName));
							Service.GalaxyViewController.OpenPlanetDetailsForPlanet(text);
						}
						else if (currentState is HomeState)
						{
							Service.EventManager.SendEvent(EventId.UIConflictStatusClicked, new ActionMessageBIData("base", planetVO.PlanetBIName));
							Service.GalaxyViewController.GoToPlanetView(text, CampaignScreenSection.Main);
						}
					}
				}
			}
		}

		private int GetTotalTickerDisplayCount()
		{
			int num = this.GetActiveTournamentDisplayCount() + this.GetTotalRaidsDisplayCount() + this.GetTotalSquadWarDisplayCount();
			return (num <= 4) ? num : 4;
		}

		private int GetTotalNonConflictTickerDisplayCount()
		{
			return this.GetTotalRaidsDisplayCount() + this.GetTotalSquadWarDisplayCount();
		}

		private int GetActiveTournamentDisplayCount()
		{
			return (!this.CanShowGalaxyTournamentStatus()) ? 0 : this.tournamentDisplayCount;
		}

		private void LoadConflictMedalsData()
		{
			TournamentProgress tournamentProgress = Service.CurrentPlayer.TournamentProgress;
			List<TournamentVO> tournamentVOs = TournamentController.GetTournamentVOs(TournamentFilter.Live);
			Lang lang = Service.Lang;
			this.tournamentDisplayCount = 0;
			this.eventTickerView.SetTitleText(Service.Lang.Get("CONFLICT_STATUS_TITLE", new object[0]));
			this.tournamentDisplayActive = true;
			if (this.GetTotalRaidsDisplayCount() == 1)
			{
				EventTickerObject obj = new EventTickerObject();
				this.eventTickerView.StoreTickerObject(obj, 0);
			}
			if (!this.CanShowGalaxyTournamentStatus())
			{
				return;
			}
			int count = tournamentVOs.Count;
			for (int i = 0; i < count; i++)
			{
				TournamentVO tournamentVO = tournamentVOs[i];
				if (tournamentVO != null)
				{
					Tournament tournament = tournamentProgress.GetTournament(tournamentVO.Uid);
					int index = this.GetTotalNonConflictTickerDisplayCount() + this.tournamentDisplayCount;
					EventTickerObject eventTickerObject = new EventTickerObject();
					eventTickerObject.planet = tournamentVO.PlanetId;
					eventTickerObject.onClickFunction = new UXButtonClickedDelegate(this.OnGalaxyConflictStatusClicked);
					string message = string.Empty;
					if (tournament != null && tournament.CurrentRank != null && !string.IsNullOrEmpty(tournament.CurrentRank.TierUid))
					{
						if (!tournament.Collected)
						{
							TournamentTierVO tournamentTierVO = Service.StaticDataController.Get<TournamentTierVO>(tournament.CurrentRank.TierUid);
							message = lang.Get("CONFLICT_GAL_TOP", new object[]
							{
								LangUtils.GetPlanetDisplayName(tournamentVO.PlanetId),
								Math.Round(tournament.CurrentRank.Percentile, 2),
								lang.Get(tournamentTierVO.RankName, new object[0]),
								lang.Get(tournamentTierVO.DivisionSmall, new object[0])
							});
							this.tournamentDisplayCount++;
						}
					}
					else
					{
						string planetDisplayName = LangUtils.GetPlanetDisplayName(tournamentVO.PlanetId);
						message = lang.Get("CONFLICT_GAL_ATTACK_NOW", new object[]
						{
							planetDisplayName
						});
						this.tournamentDisplayCount++;
					}
					eventTickerObject.message = message;
					eventTickerObject.textColor = Color.white;
					eventTickerObject.bgColor = this.eventTickerView.DefaultTickerBgColor;
					this.eventTickerView.StoreTickerObject(eventTickerObject, index);
				}
			}
		}

		private void SetupPlanetGalaxyUI()
		{
			this.planetFrontGroup = new GameObject("PlanetFrontGroup");
			this.planetFrontGroup.transform.parent = Service.UXController.WorldAnchor.Root.transform;
			this.planetFrontGroup.transform.localScale = Vector3.one;
			this.planetFrontGroup.transform.localPosition = Vector3.zero;
			this.planetHighlightFront.Root.transform.parent = this.planetFrontGroup.transform;
			this.planetHighlightFront.LocalPosition = Vector3.zero;
			this.planetFrontGroup.SetActive(false);
		}

		public void AddRaidsTickerStatus(RaidDefenseController raidController)
		{
			if (this.eventTickerView == null)
			{
				this.eventTickerView = new EventTickerView(this);
			}
			this.raidDefenseController = raidController;
			this.LoadRaidsData();
		}

		public bool LoadRaidsData()
		{
			if (this.raidDefenseController.AreRaidsAccessible())
			{
				this.SetupRaidData();
				this.eventTickerView.UpdateTickerObject(0, 0);
				this.eventTickerView.SetTitleText(string.Empty);
				this.eventTickerView.AnimateStatusTitleText();
				this.eventTickerView.AnimateTickerAtIndex(this.GetTotalTickerDisplayCount());
				return true;
			}
			return false;
		}

		private bool IsRaidCountdownReady()
		{
			int num = GameConstants.RAIDS_UPCOMING_TICKER_THROTTLE * 60;
			return this.raidDefenseController != null && this.raidDefenseController.GetRaidTimeSeconds() <= num;
		}

		private bool IsRaidTickerAvailable()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			bool flag = currentState is HomeState || currentState is GalaxyState;
			flag = (flag && this.raidDefenseController != null);
			flag = (flag && this.eventTickerView != null);
			return flag && this.raidDefenseController.AreRaidsAccessible();
		}

		private bool CanShowRaidTickerStatus()
		{
			return this.IsRaidTickerAvailable() && this.IsRaidCountdownReady();
		}

		private void TryWaitForRaidTime()
		{
			if (!this.IsRaidTickerAvailable())
			{
				return;
			}
			this.CancelWaitForRaidTime();
			int num = this.raidDefenseController.GetRaidTimeSeconds() - GameConstants.RAIDS_UPCOMING_TICKER_THROTTLE * 60;
			if (num > 0)
			{
				this.raidTimerId = Service.ViewTimerManager.CreateViewTimer((float)num, true, new TimerDelegate(this.OnShowRaidTimer), null);
			}
		}

		private void CancelWaitForRaidTime()
		{
			if (this.raidTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.raidTimerId);
				this.raidTimerId = 0u;
			}
		}

		private void OnShowRaidTimer(uint id, object cookie)
		{
			this.CancelWaitForRaidTime();
			if (this.CanShowRaidTickerStatus())
			{
				this.HideEventsTickerView();
				this.ShowEventsTickerView();
			}
		}

		private void ShowRaidsTickerStatus()
		{
			if (!this.CanShowRaidTickerStatus())
			{
				this.TryWaitForRaidTime();
				return;
			}
			bool flag = this.LoadRaidsData();
			if (flag)
			{
				this.EnableFrameTimeObserving(true);
				this.isShowingRaidTicker = true;
			}
		}

		private void HideRaidsTickerStatus()
		{
			this.CancelWaitForRaidTime();
			if (this.isShowingRaidTicker)
			{
				this.EnableFrameTimeObserving(false);
				this.isShowingRaidTicker = false;
			}
		}

		private void OnRaidTickerStatusClicked(UXButton button)
		{
			Service.BILoggingController.TrackGameAction("UI_raid_briefing", "open", "ticker", string.Empty, 1);
			this.raidDefenseController.ShowRaidInfo();
		}

		private void SetupRaidData()
		{
			this.raidAvailable = this.raidDefenseController.IsRaidAvailable();
			this.SetupRaidInfo();
		}

		private void SetupRaidStateColors()
		{
			Color textColor;
			Color bgColor;
			if (this.raidAvailable)
			{
				textColor = this.raidDefenseController.ActiveRaidColor;
				bgColor = this.raidDefenseController.ActiveRaidColor;
			}
			else
			{
				textColor = this.raidDefenseController.InactiveTickerColor;
				bgColor = this.eventTickerView.DefaultTickerBgColor;
			}
			EventTickerObject tickerObject = this.eventTickerView.GetTickerObject(0);
			if (tickerObject != null)
			{
				tickerObject.textColor = textColor;
				tickerObject.bgColor = bgColor;
			}
		}

		private void SetupRaidInfo()
		{
			this.SetupRaidStateColors();
			this.raidTimerText = ((!this.raidAvailable) ? "RAID_TIME_REMAINING_TICKER_INACTIVE" : "RAID_TIME_REMAINING_TICKER_ACTIVE");
			this.UpdateRaidTimer();
			EventTickerObject tickerObject = this.eventTickerView.GetTickerObject(0);
			if (tickerObject != null)
			{
				tickerObject.onClickFunction = new UXButtonClickedDelegate(this.OnRaidTickerStatusClicked);
			}
		}

		private void UpdateRaidTickerView()
		{
			if (this.raidDefenseController != null && this.raidDefenseController.AreRaidsAccessible() && this.eventTickerView != null)
			{
				if (this.raidAvailable != this.raidDefenseController.IsRaidAvailable())
				{
					this.raidAvailable = this.raidDefenseController.IsRaidAvailable();
					this.SetupRaidInfo();
				}
				this.UpdateRaidTimer();
			}
		}

		private void UpdateRaidTimer()
		{
			if (!this.CanShowRaidTickerStatus())
			{
				return;
			}
			string text = LangUtils.FormatTime((long)this.raidDefenseController.GetRaidTimeSeconds());
			string message = Service.Lang.Get(this.raidTimerText, new object[]
			{
				text
			});
			EventTickerObject tickerObject = this.eventTickerView.GetTickerObject(0);
			if (tickerObject != null)
			{
				tickerObject.message = message;
			}
		}

		private int GetTotalRaidsDisplayCount()
		{
			return (!this.CanShowRaidTickerStatus()) ? 0 : 1;
		}

		public void AddSquadWarTickerStatus()
		{
			if (this.eventTickerView == null)
			{
				this.eventTickerView = new EventTickerView(this);
			}
			this.HideSquadWarBeginStatus();
			this.ShowSquadWarBeginStatus();
			if (this.CanShowSquadWarPrepPhaseStatus())
			{
				this.eventTickerView.UpdateTickerObject(0, 0);
				this.eventTickerView.SetTitleText(string.Empty);
				this.eventTickerView.AnimateStatusTitleText();
				this.eventTickerView.AnimateTickerAtIndex(this.GetTotalTickerDisplayCount());
			}
		}

		private bool CanShowSquadWarPrepPhaseStatus()
		{
			bool flag = Service.GameStateMachine.CurrentState is WarBaseEditorState;
			return flag && this.eventTickerView != null;
		}

		private void ShowSquadWarBeginStatus()
		{
			if (!this.CanShowSquadWarPrepPhaseStatus())
			{
				return;
			}
			if (this.eventTickerView != null)
			{
				this.LoadSquadWarData();
			}
			this.EnableFrameTimeObserving(true);
			this.isShowingSquadWarTicker = true;
		}

		private void LoadSquadWarData()
		{
			if (this.GetTotalSquadWarDisplayCount() == 1)
			{
				EventTickerObject obj = new EventTickerObject();
				this.eventTickerView.StoreTickerObject(obj, this.GetTotalRaidsDisplayCount());
				this.SetupSquadWarColors();
			}
		}

		private void HideSquadWarBeginStatus()
		{
			if (this.isShowingSquadWarTicker)
			{
				this.EnableFrameTimeObserving(false);
				this.isShowingSquadWarTicker = false;
			}
		}

		private void SetupSquadWarColors()
		{
			EventTickerObject tickerObject = this.eventTickerView.GetTickerObject(this.GetTotalRaidsDisplayCount());
			if (tickerObject != null)
			{
				tickerObject.textColor = Color.white;
				tickerObject.bgColor = Color.white;
			}
		}

		private void UpdateSquadWarTickerView()
		{
			if (this.isShowingSquadWarTicker)
			{
				this.UpdateSquadWarTimer();
			}
		}

		private void UpdateSquadWarTimer()
		{
			SquadWarData currentSquadWar = Service.SquadController.WarManager.CurrentSquadWar;
			if (currentSquadWar == null)
			{
				Service.Logger.Warn("Trying to update squad war timer with no squad data");
				return;
			}
			uint serverTime = Service.ServerAPI.ServerTime;
			string text = LangUtils.FormatTime((long)(currentSquadWar.PrepGraceStartTimeStamp - (int)serverTime));
			string text2 = Service.Lang.Get("WAR_BASE_PREP_TIME_REMAINING", new object[]
			{
				text
			});
			int totalRaidsDisplayCount = this.GetTotalRaidsDisplayCount();
			EventTickerObject tickerObject = this.eventTickerView.GetTickerObject(totalRaidsDisplayCount);
			if (tickerObject != null && text2 != tickerObject.message)
			{
				tickerObject.message = text2;
				this.eventTickerView.UpdateTickerObject(totalRaidsDisplayCount, totalRaidsDisplayCount);
			}
		}

		private int GetTotalSquadWarDisplayCount()
		{
			return (!this.CanShowSquadWarPrepPhaseStatus()) ? 0 : 1;
		}

		public void AttachCurrencyTrayToScreen(UXElement screenRoot, CurrencyTrayType type)
		{
			this.currencyTray.Parent = screenRoot;
			this.currencyTray.Visible = true;
			this.currencyTrayType = type;
			this.UpdateCurrencyTrayContents(screenRoot);
		}

		private void UpdateCurrencyTrayContents(UXElement screenRoot)
		{
			if (!this.currencyTray.Visible)
			{
				return;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			for (int i = 0; i < this.currencyTrayTable.Count; i++)
			{
				int num = -1;
				string text = (string)this.currencyTrayTable.GetItem(i).Tag;
				if (text == "Credit")
				{
					num = currentPlayer.CurrentCreditsAmount;
				}
				else if (text == "Alloy")
				{
					num = currentPlayer.CurrentMaterialsAmount;
				}
				else if (text == "Crystal")
				{
					num = currentPlayer.CurrentCrystalsAmount;
				}
				else if (text == "Contraband")
				{
					if (currentPlayer.MaxContrabandAmount > 0)
					{
						num = currentPlayer.CurrentContrabandAmount;
					}
				}
				else if (text == "Reputation" && currentPlayer.MaxReputationAmount > 0)
				{
					num = currentPlayer.CurrentReputationAmount;
				}
				bool flag = num >= 0;
				if (this.currencyTrayType == CurrencyTrayType.Reputation && text != "Reputation")
				{
					flag = false;
				}
				else if (this.currencyTrayType == CurrencyTrayType.Default && text == "Reputation")
				{
					flag = false;
				}
				this.currencyTrayTable.GetItem(i).Visible = flag;
				if (flag)
				{
					this.currencyTrayTable.GetSubElement<UXLabel>(text, "CurrencyLabel").Text = Service.Lang.ThousandsSeparated(num);
				}
			}
			this.currencyTrayTable.RepositionItems();
			this.CurrencyTrayResize(screenRoot);
		}

		private void CurrencyTrayResize(UXElement screenRoot)
		{
			Vector3 vector = UXUtils.CalculateElementSize(this.currencyTrayTable);
			this.currencyTrayBackground.Width = vector.x + 50f;
			if (screenRoot != null)
			{
				this.currencyTray.LocalPosition = new Vector3(0f, -screenRoot.ColliderHeight / 2f, 0f);
			}
		}

		public void DetachCurrencyTray()
		{
			this.currencyTray.Parent = this;
			this.currencyTray.Visible = false;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.InventoryResourceUpdated)
			{
				if (id != EventId.SquadAdvancementTabSelected)
				{
					if (id == EventId.ShowObjectiveToast)
					{
						this.checkPendingObjectiveToast = true;
						this.EnableFrameTimeObserving(true);
					}
				}
				else if (cookie != null)
				{
					SquadScreenAdvancementView squadScreenAdvancementView = cookie as SquadScreenAdvancementView;
					this.currencyTrayType = squadScreenAdvancementView.GetDisplayCurrencyTrayType();
					this.UpdateCurrencyTrayContents(null);
				}
			}
			else
			{
				this.UpdateCurrencyTrayContents(null);
			}
			return base.OnEvent(id, cookie);
		}

		private void SetupScrim()
		{
			GameObject root = this.scrimElement.Root;
			root.transform.parent = this.uxCamera.Anchor.transform;
			UnityUtils.EnsureScreenFillingComponent(this.scrimButton.Root, 0, this.uxCamera.Scale);
			this.scrimElement.Visible = false;
			this.scrimButton.OnClicked = new UXButtonClickedDelegate(this.OnScrimButtonClicked);
		}

		private void SetupConfirmGroup()
		{
			this.confirmGroup = new GameObject("Confirm Group");
			this.confirmGroup.transform.parent = Service.UXController.WorldAnchor.Root.transform;
			this.confirmGroup.transform.localScale = Vector3.one;
			this.confirmGroup.transform.localPosition = Vector3.zero;
			float d = this.acceptButton.ColliderWidth * 0.75f;
			this.cancelButtonParent.Root.transform.parent = this.confirmGroup.transform;
			this.cancelButtonParent.LocalPosition = Vector3.left * d;
			this.acceptButtonParent.Root.transform.parent = this.confirmGroup.transform;
			this.acceptButtonParent.LocalPosition = Vector3.right * d;
			this.cancelButtonParent.WidgetDepth = -1;
			this.acceptButtonParent.WidgetDepth = -1;
			UnityUtils.SetLayerRecursively(this.confirmGroup, this.uxCamera.Camera.gameObject.layer);
			this.acceptButtonDimSprite = base.GetElement<UXSprite>("SpriteDimAccept");
			this.cancelButtonDimSprite = base.GetElement<UXSprite>("SpriteDimCancel");
			this.acceptButtonDimSprite.Visible = false;
			this.cancelButtonDimSprite.Visible = false;
			this.HideConfirmGroup();
		}

		private void LoadFailure(object cookie)
		{
			this.Loaded();
		}

		private void Loaded()
		{
			if (this.onCompleteCallback != null)
			{
				this.onCompleteCallback(this.onCompleteCookie);
			}
		}

		public Animation AddScreenTransitionAnimation(GameObject gameObject)
		{
			Animation animation = null;
			if (this.transitioner != null)
			{
				Animation component = this.transitioner.GetComponent<Animation>();
				if (component != null)
				{
					AnimatedAlpha animatedAlpha = gameObject.AddComponent<AnimatedAlpha>();
					animatedAlpha.alpha = 0f;
					animatedAlpha.enabled = false;
					animatedAlpha.enabled = true;
					animation = gameObject.AddComponent<Animation>();
					animation.playAutomatically = false;
					IEnumerator enumerator = component.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							AnimationState animationState = (AnimationState)enumerator.Current;
							AnimationClip animationClip = UnityEngine.Object.Instantiate<AnimationClip>(animationState.clip);
							animationClip.name = animationState.name;
							animation.AddClip(animationClip, animationClip.name);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			return animation;
		}

		public UXElement ShowScrim(bool show)
		{
			return this.ShowScrim(show, true);
		}

		public UXElement ShowScrim(bool show, bool visibleBG)
		{
			this.scrimElement.Visible = show;
			if (show)
			{
				this.HideEventsTickerView();
			}
			else
			{
				this.ShowEventsTickerView();
			}
			if (!visibleBG)
			{
				this.scrimSprite.Alpha = 0f;
			}
			else
			{
				this.scrimSprite.Alpha = 1f;
			}
			return this.scrimElement;
		}

		private void OnScrimButtonClicked(UXButton button)
		{
			ScreenBase highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<ScreenBase>();
			if (highestLevelScreen is SquadSlidingScreen && Service.UXController.HUD.IsSquadScreenOpenAndCloseable())
			{
				Service.UXController.HUD.SlideSquadScreenClosed();
			}
			else
			{
				ClosableScreen nonFatalClosableScreen = this.GetNonFatalClosableScreen();
				if (nonFatalClosableScreen != null && !(nonFatalClosableScreen is PlanetDetailsScreen))
				{
					nonFatalClosableScreen.Close(null);
				}
			}
		}

		public void TryCloseNonFatalAlertScreen()
		{
			ScreenController screenController = Service.ScreenController;
			AlertScreen highestLevelScreen = screenController.GetHighestLevelScreen<AlertScreen>();
			if (highestLevelScreen != null && !highestLevelScreen.IsFatal)
			{
				highestLevelScreen.Close(null);
			}
		}

		public ClosableScreen GetNonFatalClosableScreen()
		{
			ScreenController screenController = Service.ScreenController;
			ClosableScreen highestLevelScreen = screenController.GetHighestLevelScreen<ClosableScreen>();
			if (highestLevelScreen != null && highestLevelScreen.AllowClose && highestLevelScreen.TransitionedIn && highestLevelScreen == screenController.GetHighestLevelScreen<UXElement>() && (!(highestLevelScreen is AlertScreen) || !(highestLevelScreen as AlertScreen).IsFatal))
			{
				return highestLevelScreen;
			}
			return null;
		}

		public void ShowTroopCounter(UXElement troopButton, int currentCount, int maxCount)
		{
			this.PositionElementRelatively(this.troopCounter, troopButton);
			this.troopCounter.WidgetDepth = 9999;
			this.troopCounter.Visible = true;
			this.troopCounterLabel.Text = Service.Lang.Get("FUE_TRAIN_TROOPS", new object[]
			{
				currentCount,
				maxCount
			});
		}

		public void HideTroopCounter()
		{
			this.troopCounter.Visible = false;
		}

		public void ShowTroopTooltip(UXElement troopButton, string localizedText)
		{
			this.PositionElementRelatively(this.troopTooltip, troopButton);
			this.troopTooltipLabel.Text = localizedText;
			this.troopTooltip.WidgetDepth = 9999;
			this.troopTooltip.Visible = true;
		}

		public void HideTroopTooltip()
		{
			this.troopTooltip.Visible = false;
		}

		private void PositionElementRelatively(UXElement element, UXElement targetElement)
		{
			UXCamera uXCamera = Service.CameraManager.UXCamera;
			uXCamera.AttachToMainAnchor(element.Root);
			element.UXCamera = uXCamera;
			Vector3 position = targetElement.Root.transform.position + Vector3.up * (targetElement.Root.GetComponent<Collider>().bounds.size.y / 2f);
			Vector3 position2 = uXCamera.Camera.WorldToViewportPoint(position);
			if (position2.x >= 0f && position2.x <= 1f && position2.y >= 0f && position2.y <= 1f)
			{
				element.Root.transform.position = uXCamera.Camera.ViewportToWorldPoint(position2);
			}
		}

		public void AlignArrow(Vector3 position)
		{
			UXCamera uXCamera = Service.CameraManager.UXCamera;
			this.buttonHighlight.Root.transform.parent = uXCamera.Anchor.transform;
			this.buttonHighlight.UXCamera = uXCamera;
			this.buttonHighlight.Position = position;
			this.buttonHighlightArrow.localPosition = Vector3.zero;
			UXSprite element = base.GetElement<UXSprite>("Border");
			element.Visible = false;
			this.buttonHighlight.Visible = true;
			Animator component = this.buttonHighlight.Root.GetComponent<Animator>();
			component.SetTrigger("Show");
		}

		public void ShowArrowOnScreen(float x, float y)
		{
			UXCamera uXCamera = Service.CameraManager.UXCamera;
			this.buttonHighlight.Root.transform.parent = uXCamera.Anchor.transform;
			this.buttonHighlight.UXCamera = uXCamera;
			Vector3 position = new Vector3(x, y, 0f);
			Vector3 position2 = uXCamera.Camera.ViewportToWorldPoint(position);
			this.buttonHighlight.Root.transform.position = position2;
			this.buttonHighlightArrow.localPosition = Vector3.zero;
			UXSprite element = base.GetElement<UXSprite>("Border");
			element.Visible = false;
			this.buttonHighlight.Visible = true;
			Animator component = this.buttonHighlight.Root.GetComponent<Animator>();
			component.SetTrigger("Show");
		}

		public void HideArrow()
		{
			this.buttonHighlight.Visible = false;
		}

		public void HighlightButton(UXElement button, string arrowPosition)
		{
			if (this.buttonHighlight.Root == null || button.Root == null || button.Root.GetComponent<Collider>() == null)
			{
				return;
			}
			button.Visible = true;
			Transform transform = null;
			if (!button.Root.activeInHierarchy)
			{
				transform = button.Root.transform;
				while (transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
				{
					transform = transform.parent;
				}
			}
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
			}
			UXCamera uXCamera = Service.CameraManager.UXCamera;
			uXCamera.AttachToMainAnchor(this.buttonHighlight.Root);
			this.buttonHighlight.UXCamera = uXCamera;
			UXSprite element = base.GetElement<UXSprite>("Border");
			UXSprite element2 = base.GetElement<UXSprite>("BorderCircular");
			UXSprite element3 = base.GetElement<UXSprite>("BorderCircle");
			IEnumerator enumerator = element.Root.transform.parent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform2 = (Transform)enumerator.Current;
					transform2.gameObject.SetActive(false);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			string name = string.Format("{0}Highlight", button.Root.name);
			GameObject gameObject = UnityUtils.FindGameObject(button.Root, name);
			if (gameObject == null)
			{
				gameObject = UnityUtils.FindGameObject(element.Root.transform.parent.gameObject, name);
			}
			if (gameObject != null)
			{
				gameObject.layer = element.Root.layer;
				gameObject.transform.parent = element.Root.transform.parent;
				if (gameObject.GetComponent<UIWidget>().isAnchored)
				{
					Transform anchor = null;
					gameObject.GetComponent<UIWidget>().SetAnchor(anchor);
				}
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.SetActive(true);
				element.Visible = false;
				gameObject.transform.localScale = Vector3.one;
			}
			else
			{
				UXSprite uXSprite;
				if (Service.UXController.HUD.IsElementProvidedTroop(button))
				{
					element.Visible = false;
					element2.Visible = true;
					element3.Visible = false;
					uXSprite = element2;
				}
				else if (UXUtils.IsElementObjective(button))
				{
					element.Visible = false;
					element2.Visible = false;
					element3.Visible = true;
					uXSprite = element3;
				}
				else
				{
					element.Visible = true;
					element2.Visible = false;
					element3.Visible = false;
					uXSprite = element;
				}
				uXSprite.Width = button.ColliderWidth;
				uXSprite.Height = button.ColliderHeight;
				uXSprite.Root.transform.localScale = Vector3.one;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (arrowPosition == "topright")
			{
				zero.x = button.ColliderWidth / uXCamera.Scale / 2f - 10f / uXCamera.Scale;
				zero.y = button.ColliderHeight / uXCamera.Scale / 2f - 10f / uXCamera.Scale;
				zero2.y = 180f;
			}
			else if (arrowPosition == "bottomleft")
			{
				zero.x = -button.ColliderWidth / uXCamera.Scale / 2f + 10f / uXCamera.Scale;
				zero.y = -button.ColliderHeight / uXCamera.Scale / 2f + 10f / uXCamera.Scale;
				zero2.y = 180f;
				zero2.z = 180f;
			}
			else if (arrowPosition == "bottomright")
			{
				zero.x = button.ColliderWidth / uXCamera.Scale / 2f - 10f / uXCamera.Scale;
				zero.y = -button.ColliderHeight / uXCamera.Scale / 2f + 10f / uXCamera.Scale;
				zero2.z = 180f;
			}
			else
			{
				zero.x = -button.ColliderWidth / uXCamera.Scale / 2f + 10f / uXCamera.Scale;
				zero.y = button.ColliderHeight / uXCamera.Scale / 2f - 10f / uXCamera.Scale;
			}
			this.buttonHighlightArrow.localPosition = zero;
			this.buttonHighlightArrow.localEulerAngles = zero2;
			this.buttonHighlight.Visible = false;
			this.startedHighlightAnim = false;
			if (this.hideHighlightTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.hideHighlightTimerId);
				this.hideHighlightTimerId = 0u;
				Service.EventManager.SendEvent(EventId.ButtonHighlightHidden, null);
			}
			if (this.regionHighlightMover != null)
			{
				this.regionHighlightMover.Destroy();
				this.regionHighlightMover = null;
			}
			this.buttonHighlightTarget = button;
			this.EnableFrameTimeObserving(true);
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			Service.EventManager.SendEvent(EventId.ButtonHighlightActivated, null);
		}

		public void HighlightRegion(float boardX, float boardZ, int width, int depth)
		{
			this.buttonHighlight.Visible = true;
			Vector3 zero = Vector3.zero;
			zero.x = -75f / this.uxCamera.Scale;
			zero.y = 75f / this.uxCamera.Scale;
			this.buttonHighlightArrow.localPosition = zero;
			this.buttonHighlightArrow.localEulerAngles = Vector3.zero;
			UXSprite element = base.GetElement<UXSprite>("Border");
			IEnumerator enumerator = element.Root.transform.parent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					transform.gameObject.SetActive(false);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (this.regionHighlightMover != null)
			{
				this.regionHighlightMover.Destroy();
				this.regionHighlightMover = null;
			}
			this.regionHighlightMover = new RegionHighlight(this.buttonHighlight, boardX, boardZ, width, depth);
		}

		public void HideHighlight()
		{
			if (this.buttonHighlight.Visible)
			{
				if (this.hideHighlightTimerId == 0u)
				{
					Animator component = this.buttonHighlight.Root.GetComponent<Animator>();
					component.SetTrigger("Hide");
					this.hideHighlightTimerId = Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.OnHideHighlightFinish), null);
				}
			}
			else
			{
				Service.EventManager.SendEvent(EventId.ButtonHighlightHidden, null);
			}
		}

		private void OnHideHighlightFinish(uint timerId, object cookie)
		{
			if (this.regionHighlightMover != null)
			{
				this.regionHighlightMover.Destroy();
				this.regionHighlightMover = null;
			}
			this.hideHighlightTimerId = 0u;
			this.buttonHighlightTarget = null;
			this.EnableFrameTimeObserving(false);
			this.buttonHighlight.Visible = false;
			Service.EventManager.SendEvent(EventId.ButtonHighlightHidden, null);
		}

		public void ShowFingerAnimation(float boardX, float boardZ)
		{
			this.fingerX = boardX;
			this.fingerZ = boardZ;
			this.fingerAge = 0f;
			this.UpdateFingerPosition(0f);
			if (!this.fingerAnimation.Visible)
			{
				this.fingerAnimation.Visible = true;
				this.EnableFrameTimeObserving(true);
			}
			this.fingerAnimation.Root.GetComponent<Animation>().Play();
		}

		private void UpdateFingerPosition(float dt)
		{
			this.fingerAge += dt;
			if (this.fingerAge > 1f)
			{
				this.fingerAnimation.Root.GetComponent<Animation>().Play();
				this.fingerAge = 0f;
			}
			Vector3 worldPoint = new Vector3(Units.BoardToWorldX((int)this.fingerX), 0f, Units.BoardToWorldX((int)this.fingerZ));
			Vector3 localPosition = Service.CameraManager.MainCamera.WorldPositionToScreenPoint(worldPoint);
			this.fingerAnimation.LocalPosition = localPosition;
		}

		public void HideFingerAnimation()
		{
			if (this.fingerAnimation.Visible)
			{
				this.fingerAnimation.Visible = false;
				this.EnableFrameTimeObserving(false);
			}
		}

		public void ShowHudTooltip(UXElement element)
		{
			if (!this.wasHudToolTipVisible)
			{
				this.hudTooltip.Visible = true;
				this.hudTooltip.Root.transform.parent = element.Root.transform;
				this.hudTooltip.LocalPosition = new Vector3(element.Width, -element.Height * 0.5f, 0f);
				this.hudTooltipLabel.Text = Service.Lang.Get("Tooltip_" + element.Root.name, new object[0]);
			}
		}

		public void ShowSquadLevelTooltip(UXElement element)
		{
			if (!this.wasSquadLevelToolTipVisible)
			{
				this.squadLevelTooltip.Visible = true;
				this.squadLevelTooltip.Root.transform.parent = element.Root.transform;
				this.squadLevelTooltipLabel.Text = Service.Lang.Get("PERK_SQUAD_LEVEL_TOOLTIP", new object[0]);
				float x = -element.Width * 0.3f;
				this.squadLevelTooltip.LocalPosition = new Vector3(x, 0f, 0f);
				float x2 = this.squadLevelTooltip.UXCamera.Camera.WorldToScreenPoint(this.squadLevelTooltip.WorldPosition).x;
				float width = this.squadLevelTooltip.Width;
				if (x2 + width >= (float)Screen.width)
				{
					this.squadLevelTooltip.LocalPosition = new Vector3(-this.squadLevelTooltip.Width - element.Width, -element.Height * 0.5f, 0f);
				}
				int depth = element.Root.GetComponent<UIWidget>().depth;
				ScreenBase highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<ScreenBase>();
				int val = (highestLevelScreen != null) ? highestLevelScreen.GetRootPanelDepth() : 0;
				int num = Math.Max(depth, val);
				this.squadLevelTooltipPanel.depth = Math.Max(this.squadLevelTooltipPanel.depth, num + 1);
			}
		}

		public void ShowSameFactionMatchMakingTooltip(UXElement element, string sameFactionString)
		{
			if (!this.wasSameFactionMatchMakingToolTipVisible)
			{
				this.PositionElementRelatively(this.sameFactionMatchMakingTooltip, element);
				this.sameFactionMatchMakingTooltip.Visible = true;
				this.sameFactionMatchMakingTooltip.WidgetDepth = 9999;
				this.sameFactionMatchMakingTooltipLabel.Text = sameFactionString;
			}
		}

		public bool IsHudFactionIconTooltipVisible()
		{
			return this.hudFactionIconTooltip != null && this.hudFactionIconTooltip.Visible;
		}

		public void ShowHudFactionIconTooltip(UXElement element)
		{
			if (!this.wasHudFactionToolTipVisible)
			{
				this.hudFactionIconTooltip.Visible = true;
				Service.EventManager.SendEvent(EventId.HUDFactionTooltipVisible, this.hudFactionIconTooltip.Visible);
				this.hudFactionIconTooltip.Root.transform.parent = element.Root.transform;
				this.hudFactionIconTooltip.LocalPosition = new Vector3(element.Width + 4f, 0f, 0f);
				Lang lang = Service.Lang;
				int totalVictoryRatingToNextLevel = Service.FactionIconUpgradeController.GetTotalVictoryRatingToNextLevel();
				int currentPlayerVictoryRating = Service.FactionIconUpgradeController.GetCurrentPlayerVictoryRating();
				int num = totalVictoryRatingToNextLevel - currentPlayerVictoryRating;
				num = Math.Max(num, 0);
				FactionIconUpgradeController factionIconUpgradeController = Service.FactionIconUpgradeController;
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				int currentPlayerDisplayLevel = factionIconUpgradeController.GetCurrentPlayerDisplayLevel();
				this.hudFactionIconProg.Text = lang.Get("FACTION_ICON_LEVEL", new object[]
				{
					currentPlayerDisplayLevel
				});
				this.hudFactionIconTooltipSprite.SpriteName = factionIconUpgradeController.GetIcon(currentPlayer.Faction, totalVictoryRatingToNextLevel);
				int currentPlayerDisplayNextLevel = factionIconUpgradeController.GetCurrentPlayerDisplayNextLevel();
				string text = lang.ThousandsSeparated(num);
				this.hudFactionIconDesc.Text = lang.Get("FACTION_ICON_DESCRIPTION", new object[]
				{
					text,
					currentPlayerDisplayNextLevel
				});
			}
		}

		public void ShowHudBuffToolTip(UXElement element, List<WarBuffVO> buffs, bool rightSide)
		{
			if (this.wasHudBuffToolTipVisible)
			{
				return;
			}
			if (element == null || buffs == null)
			{
				Service.Logger.Error("ShowHudBuffToolTip, sent null param(s)");
				return;
			}
			int count = buffs.Count;
			if (count <= 0)
			{
				Service.Logger.Error("ShowHudBuffToolTip, sent empty buff list");
				return;
			}
			if (!rightSide)
			{
				this.hudBuffToolTipLocalOffsetPos.Set(element.ColliderWidth + 4f, 0f, 0f);
			}
			else
			{
				float newX = -((float)this.hudBuffToolTipBG.GetUIWidget.width - element.ColliderWidthUnscaled + element.ColliderXUnscaled + 4f);
				this.hudBuffToolTipLocalOffsetPos.Set(newX, 0f, 0f);
			}
			this.hudBuffToolTip.Visible = true;
			this.hudBuffToolTip.Root.transform.parent = element.Root.transform;
			this.hudBuffToolTip.Root.transform.localPosition = MiscElementsManager.OFF_SCREEN_OFFSET;
			this.hudBuffToolTip.WidgetDepth = 9999;
			this.hudBuffToolTipTable.Clear();
			this.hudBuffToolTipTable.SetTemplateItem("CardBuffsInfo");
			for (int i = 0; i < count; i++)
			{
				string itemUid = "CardBuffsInfo" + i;
				UXElement item = this.hudBuffToolTipTable.CloneTemplateItem(itemUid);
				this.hudBuffToolTipTable.GetSubElement<UXSprite>(itemUid, "SpriteIconBuffsInfo").SpriteName = buffs[i].BuffIcon;
				this.hudBuffToolTipTable.GetSubElement<UXLabel>(itemUid, "LabelTitleBuffsInfo").Text = Service.Lang.Get(buffs[i].BuffStringTitle, new object[0]);
				this.hudBuffToolTipTable.GetSubElement<UXLabel>(itemUid, "LabelBodyBuffsInfo").Text = Service.Lang.Get(buffs[i].BuffStringDesc, new object[0]);
				this.hudBuffToolTipTable.AddItem(item, i);
			}
			this.hudBuffToolTipTable.RepositionCallback = new Action(this.OnBuffToolTipTableRepositioned);
			this.hudBuffToolTipTable.RepositionItems();
		}

		private void OnBuffToolTipTableRepositioned()
		{
			this.hudBuffToolTip.Root.transform.localPosition = this.hudBuffToolTipLocalOffsetPos;
			List<UXElement> elementList = this.hudBuffToolTipTable.GetElementList();
			int count = elementList.Count;
			float num = this.hudBuffToolTipBGHeight;
			for (int i = 1; i < count; i++)
			{
				num += elementList[i].GetUIWidget.CalculateBounds().size.y + this.hudBuffToolTipTable.Padding.y;
			}
			this.hudBuffToolTipBG.GetUIWidget.height = (int)Mathf.Round(num);
		}

		public void HideHudBuffToolTip()
		{
			if (this.hudBuffToolTip.Visible)
			{
				this.hudBuffToolTipTable.Clear();
				this.hudBuffToolTip.Visible = false;
			}
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			this.wasHudFactionToolTipVisible = this.hudFactionIconTooltip.Visible;
			this.wasHudToolTipVisible = this.hudTooltip.Visible;
			this.wasSquadLevelToolTipVisible = this.squadLevelTooltip.Visible;
			this.wasSameFactionMatchMakingToolTipVisible = this.sameFactionMatchMakingTooltip.Visible;
			this.wasHudBuffToolTipVisible = this.hudBuffToolTip.Visible;
			this.HideHudFactionIconToolTip();
			this.HideHudTooltip();
			this.HideHudBuffToolTip();
			this.HideSquadLevelToolTip();
			this.HideSameFactionMatchMakingToolTip();
			return EatResponse.NotEaten;
		}

		private void HideHudTooltip()
		{
			if (this.hudTooltip.Visible)
			{
				this.hudTooltip.Visible = false;
			}
		}

		private void HideHudFactionIconToolTip()
		{
			if (this.hudFactionIconTooltip.Visible)
			{
				this.hudFactionIconTooltip.Visible = false;
				Service.EventManager.SendEvent(EventId.HUDFactionTooltipVisible, this.hudFactionIconTooltip.Visible);
			}
		}

		public void HideSquadLevelToolTip()
		{
			if (this.squadLevelTooltip.Visible)
			{
				this.squadLevelTooltip.Visible = false;
			}
		}

		public void HideSameFactionMatchMakingToolTip()
		{
			if (this.sameFactionMatchMakingTooltip.Visible)
			{
				this.sameFactionMatchMakingTooltip.Visible = false;
			}
		}

		public void ShowObjectiveToast(ObjectiveProgress objProgress)
		{
			if (this.objectiveToast != null && !this.isObjectiveToastVisible)
			{
				Lang lang = Service.Lang;
				string toast = lang.Get("OBJECTIVE_GRACE_UNLOCKED", new object[0]);
				string status = lang.Get(objProgress.VO.ObjString, new object[]
				{
					objProgress.Count
				});
				CrateVO crateVO = Service.StaticDataController.Get<CrateVO>(objProgress.VO.CrateRewardUid);
				string crateDisplayName = LangUtils.GetCrateDisplayName(crateVO);
				this.ShowToast(toast, status, crateDisplayName);
			}
		}

		public void ShowToast(string toast, string status, string crateName)
		{
			this.objectiveWidgetToaster.Visible = true;
			this.isObjectiveToastVisible = true;
			this.ResetObjectiveHideTimer();
			this.objectiveToastStatusLabel.Text = status;
			this.objectiveToastLabel.Text = toast;
			this.objectiveToastCrateLabel.Text = crateName;
			Animator component = this.objectiveToast.Root.GetComponent<Animator>();
			component.SetTrigger("ShowHud");
			this.objectiveDisplayTimerId = Service.ViewTimerManager.CreateViewTimer(3f, false, new TimerDelegate(this.HideToastByTimerTrigger), null);
		}

		private void HideToastByTimerTrigger(uint timerId, object cookie)
		{
			this.HideObjectiveToast();
		}

		private void HideObjectiveToastButtonClicked(UXButton button)
		{
			this.HideObjectiveToast();
		}

		private void HideObjectiveToast()
		{
			if (this.objectiveDisplayTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.objectiveDisplayTimerId);
				this.objectiveDisplayTimerId = 0u;
			}
			if (this.objectiveToast != null && this.isObjectiveToastVisible)
			{
				Animator component = this.objectiveToast.Root.GetComponent<Animator>();
				component.SetTrigger("HideHud");
				this.ResetObjectiveHideTimer();
				this.objectiveDisplayHideTimerId = Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.DisableToast), null);
			}
		}

		private void DisableToast(uint timerId, object cookie)
		{
			this.objectiveDisplayHideTimerId = 0u;
			if (this.objectiveWidgetToaster != null)
			{
				this.objectiveWidgetToaster.Visible = false;
				this.isObjectiveToastVisible = false;
			}
		}

		private void ResetObjectiveHideTimer()
		{
			if (this.objectiveDisplayHideTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.objectiveDisplayHideTimerId);
				this.objectiveDisplayHideTimerId = 0u;
			}
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			return EatResponse.NotEaten;
		}

		public EatResponse OnRelease(int id)
		{
			return EatResponse.NotEaten;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			return EatResponse.NotEaten;
		}

		public void ShowPlayerInstructions(string instructions, float showTime, float fadeTime)
		{
			this.ShowPlayerInstructions(string.Empty, instructions, showTime, fadeTime);
		}

		public void ShowPlayerInstructions(string id, string instructions, float showTime, float fadeTime)
		{
			if (string.IsNullOrEmpty(instructions))
			{
				Service.Logger.WarnFormat("Trying to show an empty player message", new object[0]);
				return;
			}
			if (id != string.Empty)
			{
				int num = -1;
				int i = 0;
				int count = this.playerInstructionsList.Count;
				while (i < count)
				{
					if (id.Equals(this.playerInstructionsList[i].Label.Tag))
					{
						num = i;
						break;
					}
					i++;
				}
				if (num > -1)
				{
					LabelFader labelFader = this.playerInstructionsList[num];
					int lineCount = labelFader.LineCount;
					this.playerInstructionsList.RemoveAt(num);
					labelFader.Destroy();
					for (int j = 0; j < num; j++)
					{
						this.playerInstructionsList[j].MoveUp(lineCount);
					}
				}
			}
			UXElement playerInstructionsPanelBasedOnGameState = this.GetPlayerInstructionsPanelBasedOnGameState();
			UXLabel playerInstructionsLabelBasedOnGameState = this.GetPlayerInstructionsLabelBasedOnGameState();
			string text = string.Format("Instructions Panel {0}", this.playerInstructionsAutoInc);
			string name = string.Format("{0} ({1})", playerInstructionsLabelBasedOnGameState.Root.name, text);
			this.playerInstructionsAutoInc++;
			GameObject gameObject = playerInstructionsPanelBasedOnGameState.Root.transform.parent.gameObject;
			UXElement objectToDestroy = base.CloneElement<UXElement>(playerInstructionsPanelBasedOnGameState, text, gameObject);
			UXLabel element = base.GetElement<UXLabel>(name);
			element.Visible = true;
			element.Text = instructions;
			element.Tag = id;
			int num2 = element.CalculateLineCount();
			int num3 = num2;
			if (num3 > 1 || (this.playerInstructionsList.Count > 0 && this.playerInstructionsList[0].LineCount > 1))
			{
				num3++;
			}
			for (int k = this.playerInstructionsList.Count - 1; k >= 0; k--)
			{
				if (k >= this.playerInstructionsList.Count)
				{
					k = this.playerInstructionsList.Count - 1;
				}
				this.playerInstructionsList[k].MoveUp(num3);
			}
			LabelFader item = new LabelFader(element, this, showTime, fadeTime, new LabelFaderCompleteDelegate(this.OnLabelFaderComplete), num2, objectToDestroy);
			this.playerInstructionsList.Insert(0, item);
		}

		public void ShowPlayerInstructions(string instructions)
		{
			this.ShowPlayerInstructions(instructions, 4f, 0.5f);
		}

		public void ShowPlayerInstructionsError(string instructions)
		{
			UXLabel playerInstructionsLabelBasedOnGameState = this.GetPlayerInstructionsLabelBasedOnGameState();
			Color costColor = UXUtils.GetCostColor(playerInstructionsLabelBasedOnGameState, false, false);
			this.ShowPlayerInstructionsWithColor(instructions, costColor);
		}

		public void ShowPlayerInstructionsWithColor(string instructions, Color color)
		{
			UXLabel playerInstructionsLabelBasedOnGameState = this.GetPlayerInstructionsLabelBasedOnGameState();
			Color textColor = playerInstructionsLabelBasedOnGameState.TextColor;
			playerInstructionsLabelBasedOnGameState.TextColor = color;
			this.ShowPlayerInstructions(instructions, 4f, 0.5f);
			playerInstructionsLabelBasedOnGameState.TextColor = textColor;
		}

		public void HidePlayerInstructions()
		{
			for (int i = this.playerInstructionsList.Count - 1; i >= 0; i--)
			{
				this.playerInstructionsList[i].Destroy();
			}
		}

		private void OnLabelFaderComplete(LabelFader fader)
		{
			this.playerInstructionsList.Remove(fader);
		}

		public UXTexture CloneUXTexture(UXElement text, string name, GameObject parent)
		{
			return base.CloneElement<UXTexture>(text, name, parent);
		}

		public UXLabel CloneUXLabel(UXElement text, string name, GameObject parent)
		{
			return base.CloneElement<UXLabel>(text, name, parent);
		}

		public UXButton CloneUXButton(UXElement text, string name, GameObject parent)
		{
			return base.CloneElement<UXButton>(text, name, parent);
		}

		public UXElement CreateLongPress(string name, UXAnchor anchor)
		{
			return base.CloneElement<UXElement>(this.longPress, name, anchor.Root);
		}

		public UXLabel CreateGameBoardLabel(string name, UXElement parent)
		{
			return base.CloneElement<UXLabel>(this.gameBoardLabel, name, parent.Root);
		}

		public UXSprite GetHolonetLoader(UXElement newParent)
		{
			return base.CloneElement<UXSprite>(base.GetElement<UXSprite>("HolonetLoader"), string.Format("HolonetLoader ({0})", 1), newParent.Root);
		}

		public UXLabel CreateCollectionLabel(string name, UXElement parent)
		{
			return base.CloneElement<UXLabel>(this.collectionLabel, name, parent.Root);
		}

		public UXSprite GetChildElementSprite(UXElement parentElement, string spriteName)
		{
			spriteName = UXUtils.FormatAppendedName(spriteName, parentElement.Root.name);
			return base.GetOptionalElement<UXSprite>(spriteName);
		}

		public UXLabel GetChildElementLabel(UXElement parentElement, string labelName)
		{
			labelName = UXUtils.FormatAppendedName(labelName, parentElement.Root.name);
			return base.GetOptionalElement<UXLabel>(labelName);
		}

		public void ShowConfirmGroup(Entity building, MiscConfirmDelegate callback)
		{
			if (this.confirmBuilding == null)
			{
				this.EnableFrameTimeObserving(true);
			}
			this.confirmBuilding = building;
			this.confirmCallback = callback;
			this.heightOffGround = -1f;
			this.confirmGroup.SetActive(false);
			this.EnableConfirmGroupAcceptButton(true);
		}

		public void HideConfirmGroup()
		{
			this.confirmGroup.SetActive(false);
			if (this.confirmBuilding != null)
			{
				this.EnableFrameTimeObserving(false);
			}
			this.confirmBuilding = null;
			this.confirmCallback = null;
			this.heightOffGround = -1f;
		}

		public void EnableConfirmGroupAcceptButton(bool enable)
		{
			this.acceptButton.Enabled = enable;
			this.acceptButtonDimSprite.Visible = !enable;
			if (enable)
			{
				Service.UserInputInhibitor.AddToAllow(this.acceptButton);
			}
		}

		public void EnsureHealthSliderPool()
		{
			int size = 40;
			int size2 = 10;
			this.CreatePool(ref this.healthSliderPool, this.healthSlider, size, "HealthSlider");
			this.CreatePool(ref this.shieldSliderPool, this.healthShieldSlider, size2, "ShieldSlider");
		}

		private void CreatePool(ref Queue<UXSlider> pool, UXSlider sliderPrefab, int size, string sliderName)
		{
			if (pool == null)
			{
				pool = new Queue<UXSlider>(size);
				for (int i = 0; i < size; i++)
				{
					string name = sliderName + i;
					UXSlider uXSlider = base.CloneElement<UXSlider>(sliderPrefab, name, base.Root);
					uXSlider.Visible = false;
					pool.Enqueue(uXSlider);
				}
			}
		}

		public void ReleaseHealthSliderPool()
		{
			this.ReleasePool(ref this.healthSliderPool);
			this.ReleasePool(ref this.shieldSliderPool);
		}

		private void ReleasePool(ref Queue<UXSlider> pool)
		{
			if (pool != null)
			{
				int count = pool.Count;
				for (int i = 0; i < count; i++)
				{
					this.DestroyMiscElement(pool.Dequeue());
				}
				pool = null;
			}
		}

		public void UpdateFrontPlanetEventTimer()
		{
			if (this.frontPlanetData != null)
			{
				TimedEventState tournamentState = this.frontPlanetData.TournamentState;
				if (tournamentState == TimedEventState.Invalid || tournamentState == TimedEventState.Closing)
				{
					this.planetFrontEventInfo.Visible = false;
				}
				this.SetFrontUIColorBasedOnEventState(tournamentState);
			}
		}

		private void ScalePlanet(Planet planet)
		{
			GameObject planetGameObject = planet.PlanetGameObject;
			if (planet.IsForegrounded)
			{
				Vector3 position = planetGameObject.transform.position;
				float screenRadiusFor3DVolume = UXUtils.GetScreenRadiusFor3DVolume(position, planet.ObjectExtents);
				float num = 1f;
				if (screenRadiusFor3DVolume > 0f)
				{
					num = this.planetFrontSize.x / screenRadiusFor3DVolume;
				}
				num = Mathf.Lerp(1f, num, Service.GalaxyViewController.GetPlanetScaleFactor(planet));
				this.planetScale.Set(num, num, num);
				planetGameObject.transform.localScale = this.planetScale;
				UnityUtils.ScaleParticleSize(planet.ParticleRings, planet.OriginalRingSize, num);
			}
			else
			{
				UnityUtils.ScaleParticleSize(planet.ParticleRings, planet.OriginalRingSize, 1f);
				planetGameObject.transform.localScale = Vector3.one;
			}
		}

		private void UpdatePlanetUIPosition()
		{
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			if (this.attachedPlanetFrontObject != null)
			{
				vector2 = this.attachedPlanetFrontObject.transform.position;
				vector = this.mainCamera.WorldPositionToScreenPoint(vector2);
				this.planetFrontPos.Set(vector.x, vector.y, 0f);
				if (this.frontPlanetData != null)
				{
					this.planetFrontStatsCount.Text = Service.Lang.ThousandsSeparated(this.frontPlanetData.ThrashingPopulation);
				}
				this.planetFrontGroup.transform.localPosition = this.planetFrontPos / this.uxCamera.Scale;
			}
			int count = this.planetBackUIList.Count;
			float x = this.planetBackCircleSize.x;
			for (int i = 0; i < count; i++)
			{
				GameObjectElementPair gameObjectElementPair = this.planetBackUIList[i];
				GameObject pairObject = gameObjectElementPair.PairObject;
				UXElement pairElement = gameObjectElementPair.PairElement;
				Planet planet = pairObject.GetComponent<PlanetRef>().Planet;
				this.ScalePlanet(planet);
				Transform parent = gameObjectElementPair.PairElement.Root.transform.parent;
				vector2 = gameObjectElementPair.PairObject.transform.position;
				if (planet.Tournament != null && planet.TournamentState == TimedEventState.Upcoming)
				{
					x = this.planetImminentCircleSize.x;
				}
				if (!Service.GalaxyViewController.IsPlanetDetailsScreenOpen())
				{
					if (Service.GalaxyViewController.ShouldAnimateCurrent(planet) && planet != this.frontPlanetData && planet.TournamentState != TimedEventState.Live)
					{
						planet.CurrentBackUIShown = true;
						Animator component = pairElement.Root.GetComponent<Animator>();
						component.ResetTrigger("CurrentNoEventHide");
						component.SetTrigger("CurrentNoEventShow");
					}
				}
				else
				{
					this.TryToHideCurrentPlanetText(planet);
				}
				float screenRadiusFor3DVolume = UXUtils.GetScreenRadiusFor3DVolume(vector2, planet.ObjectExtents);
				float num = screenRadiusFor3DVolume / x;
				this.planetBackScale.Set(num, num, num);
				parent.localScale = this.planetBackScale;
				vector = this.mainCamera.WorldPositionToScreenPoint(vector2);
				this.planetBackPos.Set(vector.x, vector.y, 0f);
				parent.localPosition = this.planetBackPos / this.uxCamera.Scale;
			}
			count = this.planetLockedUIList.Count;
			x = this.planetLockedSize.x;
			for (int j = 0; j < count; j++)
			{
				GameObjectElementPair gameObjectElementPair2 = this.planetLockedUIList[j];
				GameObject pairObject2 = gameObjectElementPair2.PairObject;
				Planet planet2 = pairObject2.GetComponent<PlanetRef>().Planet;
				Transform parent2 = gameObjectElementPair2.PairElement.Root.transform.parent;
				vector2 = gameObjectElementPair2.PairObject.transform.position;
				float screenRadiusFor3DVolume2 = UXUtils.GetScreenRadiusFor3DVolume(vector2, planet2.ObjectExtents);
				float num2 = screenRadiusFor3DVolume2 / x;
				this.planetLockedScale.Set(num2, num2, num2);
				parent2.localScale = this.planetLockedScale;
				vector = this.mainCamera.WorldPositionToScreenPoint(vector2);
				this.planetLockedPos.Set(vector.x, vector.y, 0f);
				parent2.localPosition = this.planetLockedPos / this.uxCamera.Scale;
			}
		}

		private UXElement GetPlanetBackUIElement(GameObject planet)
		{
			int count = this.planetBackUIList.Count;
			for (int i = 0; i < count; i++)
			{
				GameObjectElementPair gameObjectElementPair = this.planetBackUIList[i];
				if (gameObjectElementPair.PairObject == planet)
				{
					return gameObjectElementPair.PairElement;
				}
			}
			return null;
		}

		public void CreatePlanetLockedUI(Planet planet)
		{
			GameObject planetGameObject = planet.PlanetGameObject;
			int count = this.planetLockedUIList.Count;
			GameObject gameObject = new GameObject("PlanetLockedGroup" + count);
			string name = planetGameObject.name;
			gameObject.transform.parent = Service.UXController.WorldAnchor.Root.transform;
			UXElement element = base.GetElement<UXElement>("PlanetLockedIcon");
			UXElement uXElement = base.CloneElement<UXElement>(element, name + count, gameObject);
			uXElement.Root.layer = element.Root.layer;
			uXElement.Root.transform.position = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			this.planetLockedUIList.Add(new GameObjectElementPair(planetGameObject, uXElement));
		}

		public void HidePlanetLockedUI()
		{
			int count = this.planetLockedUIList.Count;
			for (int i = 0; i < count; i++)
			{
				this.planetLockedUIList[i].PairElement.Visible = false;
			}
		}

		public void ShowPlanetLockedUI()
		{
			int count = this.planetLockedUIList.Count;
			for (int i = 0; i < count; i++)
			{
				this.planetLockedUIList[i].PairElement.Visible = true;
			}
		}

		public void CreatePlanetBackgroundUI(Planet planet)
		{
			string arg = "PlanetBackGroup";
			string name = "PlanetHighlightBack";
			string labelName = "LabelEventTimerBack";
			string labelName2 = "LabelCurrentBack";
			TournamentVO tournament = planet.Tournament;
			if (tournament != null)
			{
				TimedEventState state = TimedEventUtils.GetState(planet.Tournament);
				if (state == TimedEventState.Upcoming || state == TimedEventState.Closing)
				{
					labelName2 = "LabelCurrentBackImminent";
					arg = "PlanetImminentGroup";
					name = "PlanetHighlightImminent";
					labelName = "LabelEventTimerBackImminent";
				}
			}
			GameObject planetGameObject = planet.PlanetGameObject;
			int count = this.planetBackUIList.Count;
			GameObject gameObject = new GameObject(arg + count);
			string name2 = planetGameObject.name;
			gameObject.transform.parent = Service.UXController.WorldAnchor.Root.transform;
			UXElement element = base.GetElement<UXElement>(name);
			UXElement uXElement = base.CloneElement<UXElement>(element, name2 + count, gameObject);
			uXElement.Root.layer = element.Root.layer;
			uXElement.Root.transform.position = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			UXLabel childElementLabel = this.GetChildElementLabel(uXElement, labelName2);
			childElementLabel.Visible = planet.IsCurrentPlanet;
			TimedEventCountdownHelper timedEventCountdownHelper = new TimedEventCountdownHelper(this.GetChildElementLabel(uXElement, labelName), tournament);
			this.eventCountdowns.Add(timedEventCountdownHelper);
			planet.TournamentCountdown = timedEventCountdownHelper;
			this.planetBackUIList.Add(new GameObjectElementPair(planetGameObject, uXElement));
		}

		public void PrepareGalaxyPlanetUI()
		{
			this.planetBackUIList.Clear();
			this.planetLockedUIList.Clear();
			this.planetFrontGroup.SetActive(true);
			this.planetHighlightFront.Visible = true;
			this.EnableFrameTimeObserving(true);
			this.frontPlanetData = null;
		}

		public void HideGalaxyPlanetUI()
		{
			this.planetFrontAnim.ResetTrigger("Show");
			this.planetFrontAnim.ResetTrigger("Hide");
			this.planetFrontAnim.SetTrigger("Off");
			this.EnableFrameTimeObserving(false);
		}

		public void PanBetweenPlanets()
		{
			if (this.frontPlanetData != null && this.attachedPlanetFrontObject != null)
			{
				this.frontPlanetUIState = GalaxyFrontPlanetUIState.Hidden;
				this.frontPlanetData = null;
				this.planetFrontAnim.ResetTrigger("Off");
				this.planetFrontAnim.ResetTrigger("Show");
				this.planetFrontAnim.SetTrigger("Hide");
				this.ClearFriendsPicGrid();
			}
		}

		public void DestroyPlanetBackgroundUI(Planet planetToDestroy)
		{
			int count = this.planetBackUIList.Count;
			int num = -1;
			for (int i = 0; i < count; i++)
			{
				GameObjectElementPair gameObjectElementPair = this.planetBackUIList[i];
				GameObject pairObject = gameObjectElementPair.PairObject;
				Planet planet = pairObject.GetComponent<PlanetRef>().Planet;
				if (planet == planetToDestroy)
				{
					GameObject gameObject = gameObjectElementPair.PairElement.Root.transform.parent.gameObject;
					gameObjectElementPair.PairElement.Root.transform.parent = null;
					UnityEngine.Object.Destroy(gameObject);
					base.DestroyElement(gameObjectElementPair.PairElement);
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				this.planetBackUIList.RemoveAt(num);
			}
			count = this.eventCountdowns.Count;
			num = -1;
			for (int j = 0; j < count; j++)
			{
				if (this.eventCountdowns[j] == planetToDestroy.TournamentCountdown)
				{
					this.eventCountdowns[j].Destroy();
					num = j;
					break;
				}
			}
			if (num >= 0)
			{
				this.eventCountdowns.RemoveAt(num);
			}
		}

		public void DestroyAllPlanetBackgroundUI()
		{
			int count = this.planetBackUIList.Count;
			for (int i = 0; i < count; i++)
			{
				GameObjectElementPair gameObjectElementPair = this.planetBackUIList[i];
				GameObject gameObject = gameObjectElementPair.PairElement.Root.transform.parent.gameObject;
				gameObjectElementPair.PairElement.Root.transform.parent = null;
				UnityEngine.Object.Destroy(gameObject);
				base.DestroyElement(gameObjectElementPair.PairElement);
			}
			count = this.eventCountdowns.Count;
			for (int j = 0; j < count; j++)
			{
				this.eventCountdowns[j].Destroy();
			}
			this.eventCountdowns.Clear();
			this.planetBackUIList.Clear();
		}

		public void DestroyPlanetLockedUI()
		{
			int count = this.planetLockedUIList.Count;
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = this.planetLockedUIList[i].PairElement.Root.transform.parent.gameObject;
				this.planetLockedUIList[i].PairElement.Root.transform.parent = null;
				UnityEngine.Object.Destroy(gameObject);
				base.DestroyElement(this.planetLockedUIList[i].PairElement);
			}
			this.planetLockedUIList.Clear();
		}

		public void ShowPlanetBackUI(Planet planet)
		{
			UXElement planetBackUIElement = this.GetPlanetBackUIElement(planet.PlanetGameObject);
			if (planetBackUIElement != null)
			{
				string labelName = "LabelEventTitleBack";
				TournamentVO tournament = planet.Tournament;
				Animator component = planetBackUIElement.Root.GetComponent<Animator>();
				TimedEventState tournamentState = planet.TournamentState;
				if (tournamentState == TimedEventState.Upcoming || tournamentState == TimedEventState.Closing)
				{
					labelName = "LabelEventTitleBackImminent";
				}
				this.GetChildElementLabel(planetBackUIElement, labelName).Text = Service.Lang.Get("PLANETS_CONFLICT_BACK", new object[]
				{
					LangUtils.GetPlanetDisplayName(planet.VO.Uid)
				});
				planet.TournamentCountdown.Campaign = tournament;
				component.ResetTrigger("Hide");
				component.SetTrigger("Show");
			}
		}

		public void HidePlanetBackUI(Planet planet)
		{
			UXElement planetBackUIElement = this.GetPlanetBackUIElement(planet.PlanetGameObject);
			if (planetBackUIElement != null && UXUtils.IsVisibleInHierarchy(planetBackUIElement))
			{
				Animator component = planetBackUIElement.Root.GetComponent<Animator>();
				if (planet.IsCurrentPlanet)
				{
					planet.CurrentBackUIShown = false;
					component.ResetTrigger("CurrentNoEventShow");
					component.SetTrigger("CurrentNoEventHide");
				}
				component.ResetTrigger("Show");
				component.SetTrigger("Hide");
			}
		}

		public void ClearPlanetFrontUI()
		{
			if (this.frontPlanetData != null && this.attachedPlanetFrontObject != null)
			{
				this.frontPlanetUIState = GalaxyFrontPlanetUIState.Hidden;
				this.frontPlanetData = null;
				this.planetFrontAnim.ResetTrigger("Hide");
				this.planetFrontAnim.ResetTrigger("Show");
				this.planetFrontAnim.SetTrigger("Off");
				this.ClearFriendsPicGrid();
			}
		}

		public void ShowGalaxyCloseButton(Action callback)
		{
			this.planetTopRightAnchor.Parent = Service.UXController.WorldAnchor;
			this.closePlanetsGalaxyCallback = callback;
		}

		public void HideGalaxyCloseButton()
		{
			this.closePlanetsGalaxyCallback = null;
			this.planetTopRightAnchor.Parent = this;
			this.SetGalaxyCloseButtonVisible(false);
		}

		public void OnPlanetsGalaxyCloseClicked(UXButton btn)
		{
			if (this.closePlanetsGalaxyCallback != null)
			{
				this.closePlanetsGalaxyCallback();
				Service.EventManager.SendEvent(EventId.GalaxyScreenClosed, null);
			}
		}

		private void TryToHideCurrentPlanetText(Planet planet)
		{
			if (planet.CurrentBackUIShown)
			{
				UXElement planetBackUIElement = this.GetPlanetBackUIElement(planet.PlanetGameObject);
				Animator component = planetBackUIElement.Root.GetComponent<Animator>();
				planet.CurrentBackUIShown = false;
				component.ResetTrigger("CurrentNoEventShow");
				component.SetTrigger("CurrentNoEventHide");
			}
		}

		private void AttachFrontUIInternal(Planet planet)
		{
			this.planetFrontPlanetName.Text = LangUtils.GetPlanetDisplayName(planet.VO);
			this.planetFrontStatsCount.Text = Service.Lang.ThousandsSeparated(planet.ThrashingPopulation);
			if (this.attachedPlanetFrontObject != null)
			{
				this.attachedPlanetFrontObject.transform.localScale = Vector3.one;
			}
			TimedEventState frontUIColorBasedOnEventState = TimedEventState.Invalid;
			if (planet.Tournament == null)
			{
				this.planetFrontEventInfo.Visible = false;
			}
			else
			{
				if (this.frontPlanetEventCountdown == null)
				{
					this.frontPlanetEventCountdown = new TimedEventCountdownHelper(this.planetFrontEventTimer, planet.Tournament);
				}
				else
				{
					this.frontPlanetEventCountdown.Campaign = planet.Tournament;
				}
				TimedEventState tournamentState = planet.TournamentState;
				this.planetFrontEventInfo.Visible = (tournamentState == TimedEventState.Live || tournamentState == TimedEventState.Upcoming);
				this.planetFrontEventTitle.Text = LangUtils.GetTournamentTitle(planet.Tournament);
				this.planetFrontEventTimer = base.GetElement<UXLabel>("LabelEventTimerFront");
				frontUIColorBasedOnEventState = tournamentState;
			}
			this.TryToHideCurrentPlanetText(planet);
			this.planetFrontAnim.ResetTrigger("Hide");
			this.planetFrontAnim.ResetTrigger("Off");
			this.planetFrontAnim.SetTrigger("Show");
			this.frontPlanetData = planet;
			this.planetFrontCurrent.Visible = planet.IsCurrentPlanet;
			this.attachedPlanetFrontObject = planet.PlanetGameObject;
			this.planetSquadCount.Text = Service.Lang.ThousandsSeparated(planet.NumSquadmatesOnPlanet);
			this.ClearFriendsPicGrid();
			this.LoadFriendPictures(planet);
			this.SetFrontUIColorBasedOnEventState(frontUIColorBasedOnEventState);
		}

		public void AttachPlanetFrontUI(Planet planet)
		{
			if (this.frontPlanetData != planet)
			{
				this.frontPlanetData = planet;
				this.attachedPlanetFrontObject = planet.PlanetGameObject;
			}
		}

		public void ShowPlanetFrontUI(Planet planet)
		{
			if (this.frontPlanetUIState != GalaxyFrontPlanetUIState.Shown)
			{
				this.frontPlanetUIState = GalaxyFrontPlanetUIState.CanBeShown;
				Service.EventManager.SendEvent(EventId.GalaxyUIPlanetFocus, null);
			}
		}

		private void LoadFriendPictures(Planet planet)
		{
			if (planet.FriendsOnPlanet.Count < 1)
			{
				this.planetFriendsGroup.Visible = true;
				this.planetFriendsGrid.Visible = false;
				this.planetNoFriends.Visible = true;
			}
			else
			{
				this.planetFriendsGroup.Visible = true;
				this.planetFriendsGrid.Visible = true;
				this.planetNoFriends.Visible = false;
				int num = 0;
				while (num < planet.FriendsOnPlanet.Count && num < 5)
				{
					SocialFriendData socialFriendData = planet.FriendsOnPlanet[num];
					UXElement item = this.planetFriendsGrid.CloneTemplateItem(socialFriendData.Id);
					UXTexture subElement = this.planetFriendsGrid.GetSubElement<UXTexture>(socialFriendData.Id, "FriendPic");
					Service.ISocialDataController.GetFriendPicture(socialFriendData, new OnGetProfilePicture(this.OnGetFriendPicture), subElement);
					this.planetFriendsGrid.AddItem(item, num);
					num++;
				}
			}
		}

		public void ClearFriendsPicGrid()
		{
			this.planetFriendsGrid.Clear();
			for (int i = 0; i < this.planetFriendPicTextures.Count; i++)
			{
				Service.ISocialDataController.DestroyFriendPicture(this.planetFriendPicTextures[i]);
			}
			this.planetFriendPicTextures.Clear();
		}

		private void OnGetFriendPicture(Texture2D tex, object cookie)
		{
			UXTexture uXTexture = cookie as UXTexture;
			uXTexture.MainTexture = tex;
			this.planetFriendPicTextures.Add(tex);
		}

		public UXSlider CreateHealthSlider(string name, GameObject parentGameObject, bool forShields)
		{
			Queue<UXSlider> queue = (!forShields) ? this.healthSliderPool : this.shieldSliderPool;
			if (queue == null)
			{
				UXSlider template = (!forShields) ? this.healthSlider : this.healthShieldSlider;
				return base.CloneElement<UXSlider>(template, name, parentGameObject);
			}
			if (queue.Count == 0)
			{
				return null;
			}
			UXSlider uXSlider = queue.Dequeue();
			uXSlider.Root.transform.parent = parentGameObject.transform;
			uXSlider.Visible = true;
			return uXSlider;
		}

		public void DestroyHealthSlider(UXSlider slider, bool forShields)
		{
			Queue<UXSlider> queue = (!forShields) ? this.healthSliderPool : this.shieldSliderPool;
			if (queue != null)
			{
				slider.Visible = false;
				queue.Enqueue(slider);
			}
			else
			{
				this.DestroyMiscElement(slider);
			}
		}

		public BuildingTooltip CreateGeneralTooltip(string name, GameObject parentGameObject)
		{
			return this.CreateBuildingTooltip(this.generalTooltip, name, parentGameObject, BuildingTooltipKind.General);
		}

		public BuildingTooltip CreateBubbleRepairTooltip(string name, GameObject parentGameObject)
		{
			return this.CreateBuildingTooltip(this.bubbleRepairTooltip, name, parentGameObject, BuildingTooltipKind.RepairBubble);
		}

		public BuildingTooltip CreateBubbleTooltip(string name, GameObject parentGameObject)
		{
			return this.CreateBuildingTooltip(this.bubbleTooltip, name, parentGameObject, BuildingTooltipKind.Bubble);
		}

		public BuildingTooltip CreateHQBubbleTooltip(string name, GameObject parentGameObject)
		{
			return this.CreateBuildingTooltip(this.bubbleTooltip, name, parentGameObject, BuildingTooltipKind.HQBubble);
		}

		public BuildingTooltip CreateShardUpgradeTooltip(string name, GameObject parentGameObject)
		{
			return this.CreateBuildingTooltip(this.bubbleTooltip, name, parentGameObject, BuildingTooltipKind.ShardUpgradeBubble);
		}

		public BuildingTooltip CreateProgressTooltip(string name, GameObject parentGameObject)
		{
			return this.CreateBuildingTooltip(this.progressTooltip, name, parentGameObject, BuildingTooltipKind.Progress);
		}

		public BuildingTooltip CreateIconProgressTooltip(string name, GameObject parentGameObject)
		{
			return this.CreateBuildingTooltip(this.iconProgressTooltip, name, parentGameObject, BuildingTooltipKind.IconProgress);
		}

		private BuildingTooltip CreateBuildingTooltip(UXElement tooltip, string name, GameObject parentObject, BuildingTooltipKind kind)
		{
			Queue<GameObject> queue = this.tooltipPool[(int)kind];
			UXElement uXElement;
			if (queue != null && queue.Count > 0)
			{
				GameObject gameObject = queue.Dequeue();
				gameObject.transform.parent = parentObject.transform;
				UXUtils.AppendNameRecursively(gameObject, name, true);
				uXElement = base.CreateElements(gameObject);
				uXElement.Visible = true;
			}
			else
			{
				uXElement = base.CloneElement<UXElement>(tooltip, name, parentObject);
			}
			return new BuildingTooltip(this, uXElement, tooltip.Root.name, name, kind);
		}

		public void DestroyBuildingTooltip(BuildingTooltip buildingTooltip)
		{
			buildingTooltip.TooltipElement.Visible = false;
			int kind = (int)buildingTooltip.Kind;
			Queue<GameObject> queue = this.tooltipPool[kind];
			if (queue == null)
			{
				queue = new Queue<GameObject>();
				this.tooltipPool[kind] = queue;
			}
			GameObject root = buildingTooltip.TooltipElement.Root;
			base.RemoveElementsRecursively(root, true, false);
			base.RevertToOriginalNameRecursively(root, root.name);
			queue.Enqueue(root);
		}

		public UXElement CreateDebugCursor(string name, UXAnchor anchor)
		{
			return base.CloneElement<UXElement>(this.debugCursor, name, anchor.Root);
		}

		private void OnConfirmButtonClicked(UXButton button)
		{
			if (this.confirmCallback != null && (this.EnableCancelBuildingPlacement || (button != this.cancelButton && !this.EnableCancelBuildingPlacement)))
			{
				this.confirmCallback(button == this.acceptButton);
			}
		}

		private void EnableFrameTimeObserving(bool enable)
		{
			if (enable)
			{
				if (this.observerCount++ == 0)
				{
					Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				}
			}
			else if (this.observerCount > 0 && --this.observerCount == 0)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public bool CanShowToast(IState currentState)
		{
			WorldTransitioner worldTransitioner = Service.WorldTransitioner;
			ScreenController screenController = Service.ScreenController;
			return (currentState is HomeState || currentState is GalaxyState || currentState is EditBaseState) && !worldTransitioner.IsTransitioning() && !worldTransitioner.IsSoftWiping() && !Service.PlanetRelocationController.IsRelocationInProgress() && screenController.GetHighestLevelScreen<HQCelebScreen>() == null && screenController.GetHighestLevelScreen<MissionCompleteScreen>() == null && screenController.GetHighestLevelScreen<CampaignCompleteScreen>() == null && screenController.GetHighestLevelScreen<TournamentTierChangedScreen>() == null;
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.fingerAnimation.Visible)
			{
				this.UpdateFingerPosition(dt);
			}
			IState currentState = Service.GameStateMachine.CurrentState;
			if (!this.CanShowToast(currentState))
			{
				this.HideObjectiveToast();
			}
			else if (this.checkPendingObjectiveToast && !this.isObjectiveToastVisible)
			{
				ObjectiveController objectiveController = Service.ObjectiveController;
				ObjectiveProgress nextCompletedObjective = objectiveController.GetNextCompletedObjective();
				if (nextCompletedObjective != null)
				{
					this.ShowObjectiveToast(nextCompletedObjective);
				}
				else
				{
					this.checkPendingObjectiveToast = false;
					this.EnableFrameTimeObserving(false);
				}
			}
			if (this.buttonHighlightTarget != null)
			{
				if (this.buttonHighlightTarget.Root == null || this.buttonHighlightTarget.Root.GetComponent<Collider>() == null)
				{
					Service.ViewTimerManager.KillViewTimer(this.hideHighlightTimerId);
					this.OnHideHighlightFinish(0u, null);
				}
				else
				{
					this.buttonHighlight.Visible = this.buttonHighlightTarget.Root.activeInHierarchy;
					if (!this.startedHighlightAnim)
					{
						this.startedHighlightAnim = true;
						this.buttonHighlight.Visible = true;
						Animator component = this.buttonHighlight.Root.GetComponent<Animator>();
						component.enabled = true;
						component.SetTrigger("Show");
					}
					UXCamera uXCamera = Service.CameraManager.UXCamera;
					Vector3 position = uXCamera.Camera.WorldToViewportPoint(this.buttonHighlightTarget.Root.GetComponent<Collider>().bounds.center);
					if (position.x >= 0f && position.x <= 1f && position.y >= 0f && position.y <= 1f)
					{
						this.buttonHighlight.Root.transform.position = uXCamera.Camera.ViewportToWorldPoint(position);
					}
				}
			}
			if (this.frontPlanetData != null && this.frontPlanetUIState == GalaxyFrontPlanetUIState.CanBeShown)
			{
				this.frontPlanetUIState = GalaxyFrontPlanetUIState.Shown;
				this.AttachFrontUIInternal(this.frontPlanetData);
			}
			this.UpdateRaidTickerView();
			this.UpdateSquadWarTickerView();
			if (this.attachedPlanetFrontObject != null || this.planetBackUIList.Count > 0)
			{
				this.UpdatePlanetUIPosition();
				return;
			}
			if (this.confirmBuilding == null)
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComponent = this.confirmBuilding.Get<GameObjectViewComponent>();
			bool flag = gameObjectViewComponent != null;
			if (flag != this.confirmGroup.activeSelf)
			{
				this.confirmGroup.SetActive(flag);
			}
			if (flag)
			{
				GameObject mainGameObject = gameObjectViewComponent.MainGameObject;
				if (this.heightOffGround < 0f)
				{
					this.heightOffGround = UnityUtils.GetGameObjectBounds(mainGameObject).max.y + 4f;
				}
				Vector3 worldPoint = Vector3.zero;
				BoxCollider component2 = mainGameObject.GetComponent<BoxCollider>();
				if (component2 != null)
				{
					worldPoint = mainGameObject.transform.position + (component2.size.x / 2f + 4f) * Vector3.right + (component2.size.z / 2f + 4f) * Vector3.forward;
				}
				else
				{
					worldPoint = mainGameObject.transform.position + Vector3.up * this.heightOffGround;
				}
				Vector3 a = this.mainCamera.WorldPositionToScreenPoint(worldPoint);
				this.confirmGroup.transform.localPosition = a / this.uxCamera.Scale;
			}
		}

		public UXElement GetButtonHighlight()
		{
			return this.buttonHighlight;
		}
	}
}
