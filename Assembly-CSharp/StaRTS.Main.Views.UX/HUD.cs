using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Performance;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Commands.Pvp;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Anchors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.Leaderboard;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Utils;
using StaRTS.Utils.Animation;
using StaRTS.Utils.Animation.Anims;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	public class HUD : UXFactory, IPerformanceObserver, IEventObserver, IViewClockTimeObserver, IViewFrameTimeObserver
	{
		public const string CURRENCY_GROUP = "Currency";

		public const string OPPONENT_GROUP = "OpponentInfo";

		public const string OPPONENT_SYMBOL = "SpriteSymbolOpponent";

		public const string OPPONENT_SYMBOL_UPGRADE_REBEL = "SpriteSymbolOpponentFactionUp";

		public const string OPPONENT_SYMBOL_UPGRADE_EMPIRE = "SpriteSymbolOpponentFactionUpEmp";

		public const string PREBATTLE_MEDALS_GROUP = "MedalInfo";

		public const string PLAYER_GROUP = "PlayerInfo";

		public const string SHIELD_GROUP = "Shield";

		public const string SPECIAL_PROMOTION_GROUP = "SpecialPromo";

		public const string BASE_RATING_BUTTON = "BaseRating";

		public const string MEDALS_BUTTON = "Medals";

		private const string MEDALS_LABEL = "LabelMedals";

		private const string CREDITS_SLIDER = "PBarCurrency";

		private const string MATERIAL_SLIDER = "PBarMaterial";

		private const string CONTRABAND_SLIDER = "PBarContraband";

		public const string CREDITS_LABEL = "LabelCurrencyValueHome";

		public const string MATERIAL_LABEL = "LabelMaterialValueHome";

		public const string CONTRABAND_LABEL = "LabelContrabandValueHome";

		public const string CRYSTALS_DROIDS_GROUP = "CrystalsDroids";

		public const string CRYSTAL_BUTTON = "Crystals";

		private const string CRYSTAL_LABEL = "LabelCrystalsValueHome";

		private const string CREDITS_ICON_ANIMATOR = "Credits";

		private const string MATERIALS_ICON_ANIMATOR = "Materials";

		private const string CONTRABAND_ICON_ANIMATOR = "Contraband";

		private const string CRYSTALS_ICON_ANIMATOR = "Crystals";

		public const string DROID_BUTTON = "Droids";

		private const string DROID_ADD_LABEL = "LabelDroidsAdd";

		private const string DROID_MAX_LABEL = "LabelDroidsMax";

		private const string DROID_ADD_GROUP = "DroidsAdd";

		private const string DROID_MAX_GROUP = "DroidsMax";

		private const string SHIELD_LABEL = "LabelShield";

		public const string NEXT_BATTLE_BUTTON = "ButtonNextBattle";

		public const string NEXT_BATTLE_COST_GROUP = "CostNextBattle";

		public const string HOME_BUTTON = "ButtonHome";

		public const string EDIT_BUTTON = "ButtonEdit";

		public const string EXIT_EDIT_BUTTON = "ButtonExitEdit";

		public const string EXIT_EDIT_ANIMATION = "ButtonExitEditHolder";

		public const string STORE_BUTTON = "ButtonStore";

		private const string CONTEXT_BUTTON_PARENT = "ButtonContextParent";

		public const string CONTEXT_BUTTON = "ButtonContext";

		private const string CONTEXT_BUTTON_DIM = "ButtonContextDim";

		private const string CONTEXT_BUTTON_LABEL = "LabelContext";

		private const string CONTEXT_BUTTON_BACKGROUND = "BackgroundContext";

		private const string CONTEXT_BUTTON_BACKGROUND_SPRITE = "context_button";

		private const string CONTEXT_DESC_LABEL = "LabelContextDescription";

		private const string STASH_CONTEXT_LOCATOR = "StashContextLocator";

		private const string CONTEXT_COST_GROUP = "Cost";

		private const string CONTEXT_HARDCOST_LABEL = "LabelHardCost";

		private const string CONTEXT_ICON_SPRITE = "SpriteContextIcon";

		private const string CONTEXT_JEWEL_CONTAINER = "ContainerJewelContext";

		private const string CONTEXT_JEWEL_COUNT_LABEL = "LabelMessageCountContext";

		public const string BATTLE_BUTTON = "ButtonBattle";

		public const string WAR_BUTTON = "ButtonWar";

		public const string WAR_BUTTON_LABEL = "LabelWar";

		public const string WAR_BUTTON_PHASE_PREP = "WarPrep";

		public const string WAR_BUTTON_PHASE_ACTION = "WarAction";

		public const string WAR_BUTTON_PHASE_COOLDOWN = "WarReward";

		public const string WAR_BUTTON_TUTORIAL = "WarTutorial";

		public const string LOG_BUTTON = "ButtonLog";

		public const string LEADERBOARD_BUTTON = "ButtonLeaderboard";

		public const string HOLONET_BUTTON = "Newspaper";

		public const string SETTINGS_BUTTON = "ButtonSettings";

		public const string SQUADS_BUTTON = "ButtonClans";

		public const string SHIELD_BUTTON = "Shield";

		public const string SQUADS_BUTTON_LABEL = "LabelClans";

		public const string END_BATTLE_BUTTON = "ButtonEndBattle";

		public const string TARGETED_BUNDLE_BUTTON = "SpecialPromo";

		public const string TARGETED_BUNDLE_TEXTURE = "TextureSpecialPromo";

		public const string TROOP_GRID = "TroopsGrid";

		private const string TROOP_TEMPLATE = "TroopTemplate";

		public const string TROOP_CHECKBOX = "CheckboxTroop";

		public const string ABILITY_PREPARED_ELEMENT = "HeroAbilityActive";

		public const string ABILITY_READY_FX_ELEMENT = "HeroReady";

		private const string TROOP_LEVEL = "LabelTroopLevel";

		private const string TROOP_QUALITY_DEFAULT = "TroopFrameBgDefault";

		private const string TROOP_QUALITY_PREFIX = "TroopFrameBgQ{0}";

		private const string PROVIDED_TROOP_LEVEL = "LabelProvidedTroopLevel";

		private const string TROOP_SPRITE_DIM = "SpriteTroopDim";

		private const string PROVIDED_TROOP_SPRITE_DIM = "SpriteProvidedTroopDim";

		private const string TROOP_PROVIDED_FRAME_GROUP = "ProvidedFrame";

		private const string TROOP_PROVIDED_FRAME_DEFAULT = "ProvidedFrameDefault";

		private const string TROOP_PROVIDED_FRAME_QUAILTY_FORMAT = "ProvidedFrameQ{0}";

		private const string TROOP_STANDARD_FRAME = "StandardFrame";

		private const string TROOP_SPRITE_PROVIDED_SELECTED = "SpriteProvidedSelected";

		private const string TROOP_GLINT_BOTTOM = "SpriteTroopGlintBottom";

		private const string TROOP_ICON_SPRITE = "SpriteTroop";

		private const string SQUAD_ICON_SPRITE = "SpriteSquad";

		private const string TROOP_COUNT_LABEL = "LabelQuantity";

		private const string PROVIDED_TROOP_COUNT_LABEL = "LabelProvidedQuantity";

		public const string TIME_LEFT_LABEL = "LabelTimeLeft";

		private const string DAMAGE_STAR_ANCHOR = "BattleStarsRewards";

		private const string DAMAGE_STAR_ANIMATOR = "RewardStarHolder";

		private const string DAMAGE_STAR_LABEL = "LabelRewardStar";

		public const string DAMAGE_STAR_GROUP = "DamageStars";

		private const string DAMAGE_STAR_1 = "SpriteStar1";

		private const string DAMAGE_STAR_2 = "SpriteStar2";

		private const string DAMAGE_STAR_3 = "SpriteStar3";

		private const string DAMAGE_VALUE_LABEL = "LabelPercent";

		public const string BASE_NAME_LABEL = "LabelBaseNameOpponent";

		private const string LOOT_GROUP = "AvailableLoot";

		public const string LOOT_CREDIT_LABEL = "LabelCurrencyValueOpponent";

		private const string LOOT_MATERIAL_LABEL = "LabelMaterialsValueOpponent";

		private const string LOOT_CONTRABAND_ICON = "SpriteOpponentContraband";

		private const string LOOT_CONTRABAND_LABEL = "LabelContrabandValueOpponent";

		public const string MEDALS_GAIN_LABEL = "LabelMedalsValueOpponent";

		public const string MEDALS_LOSE_LABEL = "LabelDefeatMedals";

		private const string TOURNAMENT_RATING_GAIN_GROUP = "TournamentMedals";

		private const string TOURNAMENT_RATING_GAIN_LABEL = "LabelDefeatTournament";

		private const string TOURNAMENT_RATING_GAIN_SPRITE = "SpriteIcoTournamentMedals";

		private const string TOURNAMENT_RATING_LOSE_GROUP = "TournamentMedalsDefeat";

		private const string TOURNAMENT_RATING_LOSE_LABEL = "LabelDefeatTournamentMedals";

		private const string TOURNAMENT_RATING_LOSE_SPRITE = "SpriteIcoTournamentMedalsDefeat";

		private const string RANK_LABEL = "LabelTrophies";

		private const string RANK_LABEL_OPPONENT = "LabelTrophiesOpponent";

		private const string RANK_SPRITE_BG = "BaseScoreBkgOpponent";

		private const string PRE_COMBAT_COUNTDOWN_GROUP = "PrecombatCountdown";

		private const string PRE_COMBAT_TIME_LABEL = "LabelCount";

		private const string PRE_COMBAT_COUNTDOWN_FILL = "CountdownFill";

		private const string PRE_COMBAT_BEGINS_LABEL = "LabelBattleBegins";

		private const string PRE_COMBAT_GOAL_LABEL = "LabelGoal";

		public const string DEPLOY_INSTRUCTIONS_LABEL = "LabelDeployInstructions";

		public const string REPLAY_CONTROLS_GROUP = "ReplayControls";

		private const string REPLAY_CHANGE_SPEED_BUTTON = "ButtonReplaySpeed";

		private const string REPLAY_CURRENT_SPEED_LABEL = "LabelReplaySpeed";

		private const string REPLAY_ENDS_IN_LABEL = "LabelReplayEndsIn";

		private const string REPLAY_TIME_LABEL = "LabelReplayTime";

		private const string PLAYER_NAME_LABEL = "LabelPlayerName";

		private const string PLAYER_CLAN_LABEL = "LabelClanName";

		private const string PLAYER_BASE_SCORE_JEWEL = "ContainerJewelBaseRating";

		private const string TARGETED_BUNDLE_LABEL = "LabelSpecialPromo";

		private const string TARGETED_BUNDLE_LABEL_TIMER = "LabelSpecialPromoTimer";

		private const string EXPIRES_IN = "expires_in";

		private const string EQUIPMENT_FX = "EquipmentFX";

		public const string PLAYER_BATTLE_WARBUFF = "BuffsYoursSquadWars";

		public const string OPPONENENT_BATTLE_WARBUFF = "BuffsOpponentsSquadWars";

		private const string LABEL_YOUR_WAR_BUFFS = "LabelBuffsYoursSquadWars";

		private const string GROUP_YOUR_WAR_BUFFS = "PanelBuffsYoursSquadWars";

		private const string GRID_YOUR_WAR_BUFFS = "GridBuffsYoursSquadWars";

		private const string TEMPLATE_SPRITE_YOUR_WAR_BUFFS = "SpriteBuffsYoursSquadWars";

		private const string GROUP_OPPONENT_WAR_BUFFS = "PanelBuffsOpponentSquadWars";

		private const string GRID_OPPONENT_WAR_BUFFS = "GridBuffsOpponentSquadWars";

		private const string TEMPLATE_SPRITE_OPPONENT_WAR_BUFFS = "SpriteBuffOpponentSquadWars";

		private const string YOUR_BUFFS_TITLE = "WAR_BATTLE_CURRENT_ADVANTAGES";

		private const string YOUR_BUFFS_BUTTON = "ContainerBuffsYoursSquadWars";

		private const string OPPONENTS_BUFFS_BUTTON = "ContainerBuffsOpponentSquadWars";

		private const string FACTION_BACKGROUND = "TrophiesBkg";

		private const string FACTION_BACKGROUND_UPGRADE_REBEL = "TrophiesBkgFactionUp";

		private const string FACTION_BACKGROUND_UPGRADE_EMPIRE = "TrophiesBkgFactionUpEmp";

		private const string FACTION_SPRITE_DEFAULT = "HudXpBg";

		private const string BUTTON_NEXT_BATTLE_HOLDER = "ButtonNextBattleHolder";

		private const string DAMAGE_STARS_HOLDER = "DamageStarsHolder";

		public const string NEIGHBOR_GROUP = "FriendInfo";

		private const string NEIGHBOR_MEDALS_LABEL = "LabelFriendMedals";

		private const string NEIGHBOR_SQUAD_NAME_LABEL = "LabelFriendClanName";

		private const string NEIGHBOR_NAME_LABEL = "LabelFriendName";

		private const string NEIGHBOR_TROPHIES_LABEL = "LabelFriendTrophies";

		private const string NEIGHBOR_FACTION_BACKGROUND = "TrophiesBkgFriend";

		private const string NEIGHBOR_FACTION_BACKGROUND_UPGRADE_REBEL = "TrophiesBkgFriendFactionUp";

		private const string NEIGHBOR_FACTION_BACKGROUND_UPGRADE_EMPIRE = "TrophiesBkgFriendFactionUpEmp";

		private const string DEPLOYABLE_BKG_SPRITE = "SpriteTroopBkg";

		private const string PROVIDED_DEPLOYABLE_BKG_SPRITE = "SpriteProvidedTroopBkg";

		private const string TROOP_BKG = "troop_bkg";

		private const string STARSHIP_BKG = "starship_bkg";

		private const string HERO_BKG = "hero_bkg";

		private const string CHAMPION_BKG = "champion_bkg";

		private const string FPS_NAME = "FPS";

		private const string FRAMETIME_NAME = "FrameTime";

		private const string FPEAK_NAME = "FPeak";

		private const string MEM_RSVD_NAME = "Rsvd";

		private const string MEM_USED_NAME = "Used";

		private const string MEM_MESH_NAME = "Mesh";

		private const string MEM_TEXTURE_NAME = "Texture";

		private const string MEM_AUDIO_NAME = "Audio";

		private const string MEM_ANIMATION_NAME = "Animation";

		private const string MEM_MATERIALS_NAME = "Material";

		private const string MEM_DEVICE_NAME = "DeviceMem";

		private const float LOOT_ICON_DEFAULT_SCALE = 32f;

		private const float LOOT_ICON_BUMP_SCALE = 40f;

		private const float LOOT_ICON_BUMP_TIME = 0.05f;

		private const float LOOT_ICON_BUMP_TIME_RESET = 0.2f;

		private const float CONTROL_ANIMATION_SPEED_DEFAULT = 1f;

		private const float CONTROL_ANIMATION_SPEED_IMMEDIATE = 100f;

		private const float CONTEXT_ANIMATION_DURATION_IN = 0.5f;

		private const float CONTEXT_ANIMATION_DURATION_OUT = 0.1f;

		private const string SHOW_PROMO_BUTTON_GLOW = "ShowEffect";

		private const string STOP_PROMO_BUTTON_GLOW = "StopEffect";

		private const string CLEAR_PROMO_BUTTON_GLOW = "ClearEffect";

		private const string SQUAD_INTRO_VIEWED_PREF = "1";

		public const string SQUAD_TROOPS_ID = "squadTroops";

		public const string SQUAD_SCREEN = "SquadScreen";

		public const string WAR_ATTACK = "WarAttack";

		public const string WAR_ATTACK_OPPONENT = "WarAttackOpponent";

		public const string WAR_ATTACK_STARTED = "WarAttackStarted";

		private const string WAR_ATTACK_BUTTON = "BtnSquadwarAttack";

		private const string WAR_ATTACK_LABEL = "LabelBtnSquadwarAttack";

		private const string WAR_ATTACK_TEXT = "WAR_START_ATTACK";

		private const string WAR_DONE_BUTTON = "BtnSquadwarBack";

		private const string WAR_DONE_LABEL = "LabelBtnSquadwarBack";

		private const string WAR_DONE_TEXT = "WAR_SCOUT_CANCEL";

		private const string WAR_UPLINKS_LABEL = "LabelAvailableUplinks";

		private const string WAR_UPLINKS_CONTAINER = "AvailableUplinks";

		private const string WAR_UPLINKS_ICON_PREFIX = "SpriteUplink";

		private const string WAR_UPLINKS_TEXT = "WAR_SCOUT_POINTS_LEFT";

		private const string WAR_EXCLAMATION = "WAR_EXCLAMATION";

		private const string WAR_BUTTON_OPEN_PHASE = "WAR_BUTTON_OPEN_PHASE";

		private const string WAR_BUTTON_PREP_PHASE = "WAR_BUTTON_PREP_PHASE";

		private const string WAR_BUTTON_ACTION_PHASE = "WAR_BUTTON_ACTION_PHASE";

		private const string WAR_BUTTON_COOLDOWN_PHASE = "WAR_BUTTON_COOLDOWN_PHASE";

		private const string WAR_ATTACK_BUFF_BASE_NOT_UNLOCKED_TITLE = "WAR_ATTACK_BUFF_BASE_NOT_UNLOCKED_TITLE";

		private const string WAR_ATTACK_BUFF_BASE_NOT_UNLOCKED_MESSAGE = "WAR_ATTACK_BUFF_BASE_NOT_UNLOCKED_MESSAGE";

		private const int MAX_COST_LABEL_WIDTH = 108;

		private const int MAX_TIMER_LABEL_WIDTH = 108;

		private const string RAID_ICON_DEFEND = "icoDefend";

		private const string RAID_DEFEND_BG = "BtnTroopBg_Gold";

		private const string RAID_ICON_INFO = "context_NightRaid";

		private const string SQUAD_ACCEPTED_MESSAGE = "SQUAD_ACCEPTED_MESSAGE";

		private const string TROOPS_RECEIVED_FROM = "TROOPS_RECEIVED_FROM";

		private const string WAR_INTERSTITIAL_TITLE = "WAR_INTERSTITIAL_TITLE";

		private const string WAR_INTERSTITIAL_MESSAGE = "WAR_INTERSTITIAL_MESSAGE";

		private const string ACTION_TYPE_BUYOUT_RESEARCH = "speed_up_research";

		private const string ACTION_TYPE_BUYOUT_BUILDING = "speed_up_building";

		private const string ACTION_TYPE_BUYOUT_BUILDING_FUE = "FUE_speed_up_building";

		private const string STICKER_SHOP_ANIM = "SwapJewels";

		private string FACTION_FLIP_ALERT_TITLE = "FACTION_ICON_FACTION_FLIP_ALERT_TITLE";

		private string FACTION_FLIP_ALERT_DESC = "FACTION_ICON_FACTION_FLIP_ALERT_DESC";

		private readonly string[] ANIMATION_TRANSITION_WHITE_LIST = new string[]
		{
			"PlayerBottomLeft",
			"PlayerBottomRight",
			"PlayerInfo",
			"Currency"
		};

		private UXElement currencyGroup;

		private UXElement opponentGroup;

		private UXSprite opponentSymbol;

		private UXSprite opponentSymbolUpgradeRebel;

		private UXSprite opponentSymbolUpgradeEmpire;

		private UXElement prebattleMedalsGroup;

		private UXElement playerGroup;

		private UXElement shieldGroup;

		private UXElement specialPromotionGroup;

		private UXButton baseRatingButton;

		private UXButton medalsButton;

		private UXSprite factionBackground;

		private UXSprite factionBackgroundUpgradeRebel;

		private UXLabel playerNameLabel;

		private UXLabel playerClanLabel;

		private UXElement crystalsDroidsGroup;

		private UXButton crystalButton;

		private UXButton droidButton;

		private UXLabel droidAddLabel;

		private UXLabel droidMaxLabel;

		private UXElement droidAddGroup;

		private UXElement droidMaxGroup;

		private UXLabel playerRankLabel;

		private UXLabel playerMedalLabel;

		private UXLabel opponentRankLabel;

		private UXSprite opponentRankBG;

		private UXLabel protectionLabel;

		private UXButton protectionButton;

		private UXButton nextBattleButton;

		private UXButton startBattleButton;

		private UXButton battleButton;

		private UXButton warButton;

		private UXButton warAttackButton;

		private UXButton targetedBundleButton;

		private bool targetedBundleGlowShown;

		private Animator targetedBundleButtonGlowAnim;

		private uint targetedBundleGlowTimerID;

		private UXLabel warAttackLabel;

		private UXButton warDoneButton;

		private UXElement warUplinks;

		private UXLabel battleButtonLabel;

		private UXButton logButton;

		private UXButton leaderboardButton;

		private UXButton holonetButton;

		private UXButton settingsButton;

		private UXButton joinSquadButton;

		private UXLabel squadsButtonLabel;

		private UXButton homeButton;

		private UXButton editButton;

		private UXButton exitEditButton;

		private Animation exitEditAnimation;

		private UXButton storeButton;

		private UXLabel storeButtonLabel;

		private UXButton contextButtonTemplate;

		private UXElement contextButtonParentTemplate;

		private List<Anim> curAnims;

		private List<UXElement> contextButtons;

		private Dictionary<string, UXElement> contextButtonPool;

		private UXLabel contextDescLabel;

		private UXButton endBattleButton;

		private UXLabel timeLeftLabel;

		private UXLabel deployInstructionsLabel;

		private UXLabel baseNameLabel;

		private UXGrid troopGrid;

		private UXElement neighborGroup;

		private UXLabel neighborNameLabel;

		private UXLabel neighborSquadLabel;

		private UXLabel neighborMedalsLabel;

		private UXLabel neighborTrophiesLabel;

		private UXSprite neighborFactionBackground;

		private UXSprite neighborFactionBackgroundUpgradeRebel;

		private JewelControl clansJewel;

		private JewelControl logJewel;

		private JewelControl storeJewel;

		private JewelControl leiSticker;

		private JewelControl battleJewel;

		private JewelControl holonetJewel;

		private JewelControl warJewel;

		private JewelControl warPrepJewel;

		private UXElement preCombatGroup;

		private UXLabel preCombatBeginsLabel;

		private UXLabel preCombatGoalLabel;

		private UXLabel preCombatTimeLabel;

		private UXSprite preCombatCountdownFill;

		private UXElement damageStarAnchor;

		private UXElement damageStarAnimator;

		private UXLabel damageStarLabel;

		private UXElement damageStarGroup;

		private UXSprite damageStar1;

		private UXSprite damageStar2;

		private UXSprite damageStar3;

		private UXLabel damageValueLabel;

		private UXElement lootGroup;

		private UXLabel lootCreditsLabel;

		private UXLabel lootMaterialLabel;

		private UXLabel lootContrabandLabel;

		private UXSprite lootContrabandIcon;

		private UXLabel medalsGainLabel;

		private UXLabel medalsLoseLabel;

		private UXElement tournamentRatingGainGroup;

		private UXSprite tournamentRatingGainSprite;

		private UXLabel tournamentRatingGainLabel;

		private UXElement tournamentRatingLoseGroup;

		private UXSprite tournamentRatingLoseSprite;

		private UXLabel tournamentRatingLoseLabel;

		private Dictionary<string, DeployableTroopControl> deployableTroops;

		private int battleControlsSelectedGroup;

		private DeployableTroopControl battleControlsSelectedCheckbox;

		private UXElement replayControlsGroup;

		private UXLabel replaySpeedLabel;

		private UXButton replayChangeSpeedButton;

		private UXLabel replayTimeLeftLabel;

		public HudConfig CurrentHudConfig;

		private List<UXElement> genericConfigElements;

		private Lang lang;

		private CurrentPlayer player;

		private DroidMoment droidMoment;

		private AssetHandle assetHandle;

		private Animation[] animations;

		private bool broughtIn;

		private bool isAnimating;

		private bool setInvisibleAfterAnimating;

		private UXAnchor performanceFPSAnchor;

		private UXAnchor performanceMemAnchor;

		private UXLabel fpsLabel;

		private UXLabel frameTimeLabel;

		private UXLabel fpeakLabel;

		private UXLabel deviceMemUsedLabel;

		private UXLabel memUsedLabel;

		private UXLabel memRsvdLabel;

		private UXLabel memMeshLabel;

		private UXLabel memTextureLabel;

		private UXLabel memAudioLabel;

		private UXLabel memAnimationLabel;

		private UXLabel memMaterialsLabel;

		private HUDResourceView creditsView;

		private HUDResourceView materialsView;

		private HUDResourceView contrabandView;

		private HUDResourceView crystalsView;

		private bool registeredFrameTimeObserver;

		private SquadSlidingScreen persistentSquadScreen;

		private bool logVisited;

		private UXSprite factionBackgroundUpgradeEmpire;

		private UXSprite neighborFactionBackgroundUpgradeEmpire;

		private bool readyToToggleVisibility;

		public HUDBaseLayoutToolView BaseLayoutToolView
		{
			get;
			private set;
		}

		public bool ReadyToToggleVisiblity
		{
			get
			{
				return this.readyToToggleVisibility && (this.persistentSquadScreen == null || this.persistentSquadScreen.IsClosed());
			}
			set
			{
				this.readyToToggleVisibility = value;
			}
		}

		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				if (this.isAnimating)
				{
					this.setInvisibleAfterAnimating = !value;
					return;
				}
				EventManager eventManager = Service.EventManager;
				if (value != base.Visible)
				{
					eventManager.SendEvent(EventId.HUDVisibilityChanging, value);
				}
				base.Visible = value;
				eventManager.SendEvent(EventId.HUDVisibilityChanged, null);
				if (!value)
				{
					if (this.registeredFrameTimeObserver)
					{
						this.RefreshAllResourceViews(false);
						this.registeredFrameTimeObserver = false;
						Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
					}
					Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				}
				else
				{
					Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				}
				Service.UXController.MiscElementsManager.SetEventTickerViewVisible(value);
				eventManager.RegisterObserver(this, EventId.NumInventoryItemsNotViewedUpdated);
				eventManager.RegisterObserver(this, EventId.HQCelebrationPlayed);
				eventManager.RegisterObserver(this, EventId.MissionActionButtonClicked);
				eventManager.RegisterObserver(this, EventId.SquadJoinInviteReceived);
				eventManager.RegisterObserver(this, EventId.SquadJoinInvitesReceived);
				eventManager.RegisterObserver(this, EventId.ContractStarted);
				eventManager.RegisterObserver(this, EventId.BuildingConstructed);
				eventManager.RegisterObserver(this, EventId.BuildingLevelUpgraded);
				eventManager.RegisterObserver(this, EventId.BuildingReplaced);
				eventManager.RegisterObserver(this, EventId.SquadUpdated);
				eventManager.RegisterObserver(this, EventId.SquadTroopsReceived);
				eventManager.RegisterObserver(this, EventId.WarPhaseChanged);
				if (value)
				{
					eventManager.RegisterObserver(this, EventId.TroopDeployed);
					eventManager.RegisterObserver(this, EventId.SpecialAttackDeployed);
					eventManager.RegisterObserver(this, EventId.HeroDeployed);
					eventManager.RegisterObserver(this, EventId.ChampionDeployed);
					eventManager.RegisterObserver(this, EventId.SquadTroopsDeployedByPlayer);
					eventManager.RegisterObserver(this, EventId.WorldLoadComplete);
					eventManager.RegisterObserver(this, EventId.GameStateChanged);
					eventManager.RegisterObserver(this, EventId.InventoryResourceUpdated);
					eventManager.RegisterObserver(this, EventId.LootEarnedUpdated);
					eventManager.RegisterObserver(this, EventId.TroopLevelUpgraded);
					eventManager.RegisterObserver(this, EventId.StarshipLevelUpgraded);
					eventManager.RegisterObserver(this, EventId.EquipmentUpgraded);
					eventManager.RegisterObserver(this, EventId.ChampionRepaired);
					eventManager.RegisterObserver(this, EventId.InventoryUnlockUpdated);
					eventManager.RegisterObserver(this, EventId.PlayerNameChanged);
					eventManager.RegisterObserver(this, EventId.PvpRatingChanged);
					eventManager.RegisterObserver(this, EventId.ScreenClosing);
					eventManager.RegisterObserver(this, EventId.SquadTroopsRequestedByCurrentPlayer);
					eventManager.RegisterObserver(this, EventId.SquadTroopsReceivedFromDonor);
					eventManager.RegisterObserver(this, EventId.SquadJoinApplicationAccepted);
					eventManager.RegisterObserver(this, EventId.SquadLeft);
					eventManager.RegisterObserver(this, EventId.WarAttackPlayerStarted);
					eventManager.RegisterObserver(this, EventId.WarAttackPlayerCompleted);
					eventManager.RegisterObserver(this, EventId.WarAttackBuffBaseStarted);
					eventManager.RegisterObserver(this, EventId.WarAttackBuffBaseCompleted);
					eventManager.RegisterObserver(this, EventId.TargetedBundleContentPrepared);
					eventManager.RegisterObserver(this, EventId.TargetedBundleRewardRedeemed);
					this.BaseLayoutToolView.RegisterObservers();
				}
				else
				{
					eventManager.UnregisterObserver(this, EventId.TroopDeployed);
					eventManager.UnregisterObserver(this, EventId.SpecialAttackDeployed);
					eventManager.UnregisterObserver(this, EventId.HeroDeployed);
					eventManager.UnregisterObserver(this, EventId.ChampionDeployed);
					eventManager.UnregisterObserver(this, EventId.SquadTroopsDeployedByPlayer);
					eventManager.UnregisterObserver(this, EventId.GameStateChanged);
					eventManager.UnregisterObserver(this, EventId.InventoryResourceUpdated);
					eventManager.UnregisterObserver(this, EventId.LootEarnedUpdated);
					eventManager.UnregisterObserver(this, EventId.TroopLevelUpgraded);
					eventManager.UnregisterObserver(this, EventId.StarshipLevelUpgraded);
					eventManager.UnregisterObserver(this, EventId.EquipmentUpgraded);
					eventManager.UnregisterObserver(this, EventId.ChampionRepaired);
					eventManager.UnregisterObserver(this, EventId.ContractStarted);
					eventManager.UnregisterObserver(this, EventId.BuildingConstructed);
					eventManager.UnregisterObserver(this, EventId.BuildingReplaced);
					eventManager.UnregisterObserver(this, EventId.BuildingLevelUpgraded);
					eventManager.UnregisterObserver(this, EventId.InventoryUnlockUpdated);
					eventManager.UnregisterObserver(this, EventId.PlayerNameChanged);
					eventManager.UnregisterObserver(this, EventId.PvpRatingChanged);
					eventManager.UnregisterObserver(this, EventId.ScreenClosing);
					eventManager.UnregisterObserver(this, EventId.SquadTroopsRequestedByCurrentPlayer);
					eventManager.UnregisterObserver(this, EventId.SquadTroopsReceivedFromDonor);
					eventManager.UnregisterObserver(this, EventId.SquadJoinApplicationAccepted);
					eventManager.UnregisterObserver(this, EventId.SquadLeft);
					eventManager.UnregisterObserver(this, EventId.WarAttackPlayerStarted);
					eventManager.UnregisterObserver(this, EventId.WarAttackPlayerCompleted);
					eventManager.UnregisterObserver(this, EventId.WarAttackBuffBaseStarted);
					eventManager.UnregisterObserver(this, EventId.WarAttackBuffBaseCompleted);
					eventManager.UnregisterObserver(this, EventId.TargetedBundleContentPrepared);
					eventManager.UnregisterObserver(this, EventId.TargetedBundleRewardRedeemed);
				}
			}
		}

		public override bool HiddenInQueue
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public HUD() : base(Service.CameraManager.UXCamera)
		{
			this.ReadyToToggleVisiblity = false;
			this.lang = Service.Lang;
			this.player = Service.CurrentPlayer;
			this.contextButtons = new List<UXElement>();
			this.contextButtonPool = new Dictionary<string, UXElement>();
			this.deployableTroops = null;
			this.animations = null;
			this.broughtIn = true;
			this.isAnimating = false;
			this.setInvisibleAfterAnimating = false;
			this.performanceFPSAnchor = null;
			this.performanceMemAnchor = null;
			this.fpsLabel = null;
			this.frameTimeLabel = null;
			this.deviceMemUsedLabel = null;
			this.fpeakLabel = null;
			this.memUsedLabel = null;
			this.memRsvdLabel = null;
			this.memMeshLabel = null;
			this.memTextureLabel = null;
			this.memAudioLabel = null;
			this.memAnimationLabel = null;
			this.memMaterialsLabel = null;
			this.registeredFrameTimeObserver = false;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.HolonetContentPrepared);
			eventManager.RegisterObserver(this, EventId.HolonetContentPrepareStarted);
			eventManager.RegisterObserver(this, EventId.TargetedBundleContentPrepared);
			eventManager.RegisterObserver(this, EventId.HUDVisibilityChanged);
			this.logVisited = false;
			this.persistentSquadScreen = null;
			this.BaseLayoutToolView = new HUDBaseLayoutToolView(this);
			base.Load(ref this.assetHandle, "gui_hud", new UXFactoryLoadDelegate(this.LoadSuccess), new UXFactoryLoadDelegate(this.LoadFailure), null);
		}

		public override void OnDestroyElement()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				base.Unload(this.assetHandle, "gui_hud");
				this.assetHandle = AssetHandle.Invalid;
			}
			base.OnDestroyElement();
		}

		private void LoadSuccess(object cookie)
		{
			base.GetElement<UXElement>("ButtonNextBattleHolder").Visible = true;
			base.GetElement<UXElement>("DamageStarsHolder").Visible = true;
			this.InitSliders();
			this.InitGrids();
			this.InitLabels();
			this.InitButtons();
			this.InitLoot();
			this.InitDamageGroup();
			this.InitReplayControls();
			this.InitCountdownGroup();
			this.InitNeighborGroup();
			this.InitTournamentRatingGroup();
			this.BaseLayoutToolView.Initialize();
			this.creditsView = new HUDResourceView("credits", base.GetElement<UXSlider>("PBarCurrency"), base.GetElement<UXLabel>("LabelCurrencyValueHome"), base.GetElement<UXElement>("Credits"));
			this.materialsView = new HUDResourceView("materials", base.GetElement<UXSlider>("PBarMaterial"), base.GetElement<UXLabel>("LabelMaterialValueHome"), base.GetElement<UXElement>("Materials"));
			this.contrabandView = new HUDResourceView("contraband", base.GetElement<UXSlider>("PBarContraband"), base.GetElement<UXLabel>("LabelContrabandValueHome"), base.GetElement<UXElement>("Contraband"));
			this.crystalsView = new HUDResourceView("crystals", null, base.GetElement<UXLabel>("LabelCrystalsValueHome"), base.GetElement<UXElement>("Crystals"));
			this.AnimateControls(false, 100f);
			Service.EventManager.SendEvent(EventId.HudComplete, this);
		}

		private void LoadFailure(object cookie)
		{
			Service.EventManager.SendEvent(EventId.HudComplete, null);
		}

		private void InitGrids()
		{
			this.troopGrid = base.GetElement<UXGrid>("TroopsGrid");
			UXElement element = base.GetElement<UXElement>("HeroAbilityActive");
			element.Visible = false;
		}

		private void InitSliders()
		{
			this.currencyGroup = base.GetElement<UXElement>("Currency");
			this.opponentGroup = base.GetElement<UXElement>("OpponentInfo");
			this.prebattleMedalsGroup = base.GetElement<UXElement>("MedalInfo");
			this.playerGroup = base.GetElement<UXElement>("PlayerInfo");
			this.shieldGroup = base.GetElement<UXElement>("Shield");
			this.specialPromotionGroup = base.GetElement<UXElement>("SpecialPromo");
			this.droidAddLabel = base.GetElement<UXLabel>("LabelDroidsAdd");
			this.droidMaxLabel = base.GetElement<UXLabel>("LabelDroidsMax");
			this.droidAddGroup = base.GetElement<UXElement>("DroidsAdd");
			this.droidMaxGroup = base.GetElement<UXElement>("DroidsMax");
			this.protectionLabel = base.GetElement<UXLabel>("LabelShield");
			this.opponentSymbol = base.GetElement<UXSprite>("SpriteSymbolOpponent");
			this.opponentSymbolUpgradeRebel = base.GetElement<UXSprite>("SpriteSymbolOpponentFactionUp");
			this.opponentSymbolUpgradeEmpire = base.GetElement<UXSprite>("SpriteSymbolOpponentFactionUpEmp");
		}

		private void InitButtons()
		{
			this.medalsButton = base.GetElement<UXButton>("Medals");
			this.medalsButton.OnClicked = new UXButtonClickedDelegate(this.OnTooltipButtonClicked);
			this.baseRatingButton = base.GetElement<UXButton>("BaseRating");
			this.baseRatingButton.OnClicked = new UXButtonClickedDelegate(this.OnBaseScoreButtonClicked);
			UXElement element = base.GetElement<UXElement>("ContainerJewelBaseRating");
			element.Visible = false;
			this.factionBackground = base.GetElement<UXSprite>("TrophiesBkg");
			this.factionBackgroundUpgradeRebel = base.GetElement<UXSprite>("TrophiesBkgFactionUp");
			this.factionBackgroundUpgradeEmpire = base.GetElement<UXSprite>("TrophiesBkgFactionUpEmp");
			this.endBattleButton = base.GetElement<UXButton>("ButtonEndBattle");
			this.endBattleButton.OnClicked = new UXButtonClickedDelegate(this.OnEndBattleButtonClicked);
			this.nextBattleButton = base.GetElement<UXButton>("ButtonNextBattle");
			this.nextBattleButton.OnClicked = new UXButtonClickedDelegate(this.OnNextBattleButtonClicked);
			this.battleButton = base.GetElement<UXButton>("ButtonBattle");
			this.battleButton.OnClicked = new UXButtonClickedDelegate(this.OnBattleButtonClicked);
			this.warButton = base.GetElement<UXButton>("ButtonWar");
			this.warButton.OnClicked = new UXButtonClickedDelegate(this.OnWarButtonClicked);
			this.logButton = base.GetElement<UXButton>("ButtonLog");
			this.logButton.OnClicked = new UXButtonClickedDelegate(this.OnLogButtonClicked);
			this.leaderboardButton = base.GetElement<UXButton>("ButtonLeaderboard");
			this.leaderboardButton.OnClicked = new UXButtonClickedDelegate(this.OnLeaderboardButtonClicked);
			this.holonetButton = base.GetElement<UXButton>("Newspaper");
			this.holonetButton.OnClicked = new UXButtonClickedDelegate(this.OnHolonetButtonClicked);
			this.settingsButton = base.GetElement<UXButton>("ButtonSettings");
			this.settingsButton.OnClicked = new UXButtonClickedDelegate(this.OnSettingsButtonClicked);
			this.joinSquadButton = base.GetElement<UXButton>("ButtonClans");
			this.joinSquadButton.OnClicked = new UXButtonClickedDelegate(this.OnSquadsButtonClicked);
			this.squadsButtonLabel = base.GetElement<UXLabel>("LabelClans");
			this.squadsButtonLabel.Text = this.lang.Get("s_Clans", new object[0]);
			if (Service.SquadController.StateManager.GetCurrentSquad() != null)
			{
				this.UpdateSquadJewelCount();
			}
			else if (GameConstants.SQUAD_INVITES_ENABLED)
			{
				this.UpdateSquadJewelCount();
			}
			this.homeButton = base.GetElement<UXButton>("ButtonHome");
			this.homeButton.OnClicked = new UXButtonClickedDelegate(this.OnHomeButtonClicked);
			this.crystalsDroidsGroup = base.GetElement<UXElement>("CrystalsDroids");
			this.crystalButton = base.GetElement<UXButton>("Crystals");
			this.crystalButton.OnClicked = new UXButtonClickedDelegate(this.OnCrystalButtonClicked);
			this.droidButton = base.GetElement<UXButton>("Droids");
			this.droidButton.OnClicked = new UXButtonClickedDelegate(this.OnDroidButtonClicked);
			this.protectionButton = base.GetElement<UXButton>("Shield");
			this.protectionButton.OnClicked = new UXButtonClickedDelegate(this.OnShieldButtonClicked);
			this.targetedBundleButton = base.GetElement<UXButton>("SpecialPromo");
			this.targetedBundleButton.OnClicked = new UXButtonClickedDelegate(this.OnSpecialPromotionButtonClicked);
			this.targetedBundleGlowShown = false;
			this.targetedBundleButtonGlowAnim = this.targetedBundleButton.Root.GetComponent<Animator>();
			this.targetedBundleButtonGlowAnim.Rebind();
			this.editButton = base.GetElement<UXButton>("ButtonEdit");
			this.editButton.OnClicked = new UXButtonClickedDelegate(this.OnEditButtonClicked);
			this.editButton.Visible = false;
			this.exitEditButton = base.GetElement<UXButton>("ButtonExitEdit");
			this.exitEditButton.OnClicked = new UXButtonClickedDelegate(this.OnHomeButtonClicked);
			this.exitEditAnimation = base.GetElement<UXElement>("ButtonExitEditHolder").Root.GetComponent<Animation>();
			this.storeButton = base.GetElement<UXButton>("ButtonStore");
			this.storeButton.OnClicked = new UXButtonClickedDelegate(this.OnStoreButtonClicked);
			this.animations = this.root.GetComponentsInChildren<Animation>(true);
			this.contextButtonTemplate = base.GetElement<UXButton>("ButtonContext");
			this.contextButtonParentTemplate = base.GetElement<UXElement>("ButtonContextParent");
			this.contextButtonParentTemplate.Visible = false;
			this.curAnims = new List<Anim>();
			base.GetElement<UXButton>("ContainerBuffsYoursSquadWars").OnClicked = new UXButtonClickedDelegate(this.OnYourBuffsButtonClicked);
			base.GetElement<UXButton>("ContainerBuffsOpponentSquadWars").OnClicked = new UXButtonClickedDelegate(this.OnOpponentBuffsButtonClicked);
		}

		private void InitLabels()
		{
			this.playerNameLabel = base.GetElement<UXLabel>("LabelPlayerName");
			this.playerClanLabel = base.GetElement<UXLabel>("LabelClanName");
			this.timeLeftLabel = base.GetElement<UXLabel>("LabelTimeLeft");
			this.baseNameLabel = base.GetElement<UXLabel>("LabelBaseNameOpponent");
			this.contextDescLabel = base.GetElement<UXLabel>("LabelContextDescription");
			this.contextDescLabel.Text = string.Empty;
			this.playerRankLabel = base.GetElement<UXLabel>("LabelTrophies");
			this.playerMedalLabel = base.GetElement<UXLabel>("LabelMedals");
			this.opponentRankLabel = base.GetElement<UXLabel>("LabelTrophiesOpponent");
			this.opponentRankBG = base.GetElement<UXSprite>("BaseScoreBkgOpponent");
			this.deployInstructionsLabel = base.GetElement<UXLabel>("LabelDeployInstructions");
			this.deployInstructionsLabel.Visible = false;
			this.clansJewel = JewelControl.Create(this, "Clans");
			this.logJewel = JewelControl.Create(this, "Log");
			JewelControl.Create(this, "Leaderboard");
			JewelControl.Create(this, "Settings");
			this.storeJewel = JewelControl.Create(this, "Store");
			this.leiSticker = JewelControl.Create(this, "StoreSpecial");
			this.battleJewel = JewelControl.Create(this, "Battle");
			this.holonetJewel = JewelControl.Create(this, "NewsHolo");
			this.warJewel = JewelControl.Create(this, "War");
			this.warPrepJewel = JewelControl.Create(this, "Prep");
			this.medalsGainLabel = base.GetElement<UXLabel>("LabelMedalsValueOpponent");
			this.medalsLoseLabel = base.GetElement<UXLabel>("LabelDefeatMedals");
		}

		private void InitLoot()
		{
			this.lootGroup = base.GetElement<UXElement>("AvailableLoot");
			this.lootCreditsLabel = base.GetElement<UXLabel>("LabelCurrencyValueOpponent");
			this.lootMaterialLabel = base.GetElement<UXLabel>("LabelMaterialsValueOpponent");
			this.lootContrabandIcon = base.GetElement<UXSprite>("SpriteOpponentContraband");
			this.lootContrabandLabel = base.GetElement<UXLabel>("LabelContrabandValueOpponent");
			this.lootGroup.Visible = false;
		}

		private void InitDamageGroup()
		{
			this.damageStarAnchor = base.GetElement<UXElement>("BattleStarsRewards");
			this.damageStarAnimator = base.GetElement<UXElement>("RewardStarHolder");
			this.damageStarLabel = base.GetElement<UXLabel>("LabelRewardStar");
			this.damageStarGroup = base.GetElement<UXElement>("DamageStars");
			this.damageStar1 = base.GetElement<UXSprite>("SpriteStar1");
			this.damageStar2 = base.GetElement<UXSprite>("SpriteStar2");
			this.damageStar3 = base.GetElement<UXSprite>("SpriteStar3");
			this.damageValueLabel = base.GetElement<UXLabel>("LabelPercent");
		}

		private void InitReplayControls()
		{
			base.GetElement<UXLabel>("LabelReplayEndsIn").Text = this.lang.Get("replay_ends_in", new object[0]);
			this.replayControlsGroup = base.GetElement<UXElement>("ReplayControls");
			this.replayChangeSpeedButton = base.GetElement<UXButton>("ButtonReplaySpeed");
			this.replaySpeedLabel = base.GetElement<UXLabel>("LabelReplaySpeed");
			this.replayTimeLeftLabel = base.GetElement<UXLabel>("LabelReplayTime");
			this.replayChangeSpeedButton.OnClicked = new UXButtonClickedDelegate(this.OnReplaySpeedChangeButtonClicked);
		}

		private void InitNeighborGroup()
		{
			this.neighborGroup = base.GetElement<UXElement>("FriendInfo");
			this.neighborNameLabel = base.GetElement<UXLabel>("LabelFriendName");
			this.neighborSquadLabel = base.GetElement<UXLabel>("LabelFriendClanName");
			this.neighborMedalsLabel = base.GetElement<UXLabel>("LabelFriendMedals");
			this.neighborTrophiesLabel = base.GetElement<UXLabel>("LabelFriendTrophies");
			this.neighborFactionBackground = base.GetElement<UXSprite>("TrophiesBkgFriend");
			this.neighborFactionBackgroundUpgradeRebel = base.GetElement<UXSprite>("TrophiesBkgFriendFactionUp");
			this.neighborFactionBackgroundUpgradeEmpire = base.GetElement<UXSprite>("TrophiesBkgFriendFactionUpEmp");
		}

		private void InitTournamentRatingGroup()
		{
			this.tournamentRatingGainGroup = base.GetElement<UXElement>("TournamentMedals");
			this.tournamentRatingGainLabel = base.GetElement<UXLabel>("LabelDefeatTournament");
			this.tournamentRatingGainSprite = base.GetElement<UXSprite>("SpriteIcoTournamentMedals");
			this.tournamentRatingLoseGroup = base.GetElement<UXElement>("TournamentMedalsDefeat");
			this.tournamentRatingLoseLabel = base.GetElement<UXLabel>("LabelDefeatTournamentMedals");
			this.tournamentRatingLoseSprite = base.GetElement<UXSprite>("SpriteIcoTournamentMedalsDefeat");
		}

		private void InitCountdownGroup()
		{
			this.preCombatGroup = base.GetElement<UXElement>("PrecombatCountdown");
			this.preCombatTimeLabel = base.GetElement<UXLabel>("LabelCount");
			this.preCombatCountdownFill = base.GetElement<UXSprite>("CountdownFill");
			this.preCombatTimeLabel.Text = Convert.ToInt32(GameConstants.PVP_MATCH_COUNTDOWN).ToString();
			UXLabel element = base.GetElement<UXLabel>("LabelBattleBegins");
			element.Text = this.lang.Get("PRECOMBAT_BATTLE_BEGINS_IN", new object[0]);
			this.preCombatGroup.Visible = false;
			this.preCombatGoalLabel = base.GetElement<UXLabel>("LabelGoal");
			this.preCombatGoalLabel.Visible = false;
		}

		public void ShowCountdown(bool enable)
		{
			this.preCombatGroup.Visible = enable;
		}

		public void SetCountdownTime(float remaining, float duration)
		{
			this.preCombatCountdownFill.FillAmount = remaining / duration;
			int num = Mathf.CeilToInt(remaining);
			this.preCombatTimeLabel.Text = num.ToString();
		}

		public void TogglePerformanceDisplay(bool isFPS)
		{
			PerformanceMonitor performanceMonitor = Service.PerformanceMonitor;
			UXController uXController = Service.UXController;
			MiscElementsManager miscElementsManager = uXController.MiscElementsManager;
			if (isFPS)
			{
				if (this.performanceFPSAnchor == null)
				{
					this.performanceFPSAnchor = uXController.PerformanceAnchor;
					float d = 0f;
					float num = (float)(Screen.height / 2);
					this.fpsLabel = miscElementsManager.CreateGameBoardLabel("FPS", this.performanceFPSAnchor);
					this.fpsLabel.Pivot = UIWidget.Pivot.BottomRight;
					this.fpsLabel.LocalPosition = Vector3.right * d + Vector3.up * num;
					num += this.fpsLabel.LineHeight;
					this.frameTimeLabel = miscElementsManager.CreateGameBoardLabel("FrameTime", this.performanceFPSAnchor);
					this.frameTimeLabel.Pivot = UIWidget.Pivot.BottomRight;
					this.frameTimeLabel.LocalPosition = Vector3.right * d + Vector3.up * num;
					num += this.frameTimeLabel.LineHeight;
					this.fpeakLabel = miscElementsManager.CreateGameBoardLabel("FPeak", this.performanceFPSAnchor);
					this.fpeakLabel.Pivot = UIWidget.Pivot.BottomRight;
					this.fpeakLabel.LocalPosition = Vector3.right * d + Vector3.up * num;
					performanceMonitor.RegisterFPSObserver(this);
				}
				else
				{
					performanceMonitor.UnregisterFPSObserver(this);
					miscElementsManager.DestroyMiscElement(this.fpeakLabel);
					this.fpeakLabel = null;
					miscElementsManager.DestroyMiscElement(this.fpsLabel);
					this.fpsLabel = null;
					miscElementsManager.DestroyMiscElement(this.frameTimeLabel);
					this.frameTimeLabel = null;
					this.performanceFPSAnchor = null;
				}
			}
			else if (this.performanceMemAnchor == null)
			{
				this.performanceMemAnchor = uXController.PerformanceAnchor;
				float d2 = 0f;
				float num2 = (float)Screen.height / 2f + 56f;
				this.deviceMemUsedLabel = miscElementsManager.CreateGameBoardLabel("DeviceMem", this.performanceMemAnchor);
				this.deviceMemUsedLabel.TextColor = Color.red;
				this.deviceMemUsedLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.deviceMemUsedLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				num2 += this.deviceMemUsedLabel.LineHeight;
				this.memRsvdLabel = miscElementsManager.CreateGameBoardLabel("Rsvd", this.performanceMemAnchor);
				this.memRsvdLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.memRsvdLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				num2 += this.memRsvdLabel.LineHeight;
				this.memUsedLabel = miscElementsManager.CreateGameBoardLabel("Used", this.performanceMemAnchor);
				this.memUsedLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.memUsedLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				num2 += this.memUsedLabel.LineHeight;
				this.memTextureLabel = miscElementsManager.CreateGameBoardLabel("Texture", this.performanceMemAnchor);
				this.memTextureLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.memTextureLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				num2 += this.memTextureLabel.LineHeight;
				this.memMeshLabel = miscElementsManager.CreateGameBoardLabel("Mesh", this.performanceMemAnchor);
				this.memMeshLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.memMeshLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				num2 += this.memMeshLabel.LineHeight;
				this.memAudioLabel = miscElementsManager.CreateGameBoardLabel("Audio", this.performanceMemAnchor);
				this.memAudioLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.memAudioLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				num2 += this.memAudioLabel.LineHeight;
				this.memAnimationLabel = miscElementsManager.CreateGameBoardLabel("Animation", this.performanceMemAnchor);
				this.memAnimationLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.memAnimationLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				num2 += this.memAnimationLabel.LineHeight;
				this.memMaterialsLabel = miscElementsManager.CreateGameBoardLabel("Material", this.performanceMemAnchor);
				this.memMaterialsLabel.Pivot = UIWidget.Pivot.BottomRight;
				this.memMaterialsLabel.LocalPosition = Vector3.right * d2 + Vector3.up * num2;
				performanceMonitor.RegisterMemObserver(this);
			}
			else
			{
				performanceMonitor.UnregisterMemObserver(this);
				miscElementsManager.DestroyMiscElement(this.memRsvdLabel);
				this.memRsvdLabel = null;
				miscElementsManager.DestroyMiscElement(this.memUsedLabel);
				this.memUsedLabel = null;
				miscElementsManager.DestroyMiscElement(this.memTextureLabel);
				this.memTextureLabel = null;
				miscElementsManager.DestroyMiscElement(this.memMeshLabel);
				this.memMeshLabel = null;
				miscElementsManager.DestroyMiscElement(this.memAudioLabel);
				this.memAudioLabel = null;
				miscElementsManager.DestroyMiscElement(this.memAnimationLabel);
				this.memAnimationLabel = null;
				miscElementsManager.DestroyMiscElement(this.memMaterialsLabel);
				this.memMaterialsLabel = null;
				miscElementsManager.DestroyMiscElement(this.deviceMemUsedLabel);
				this.deviceMemUsedLabel = null;
				this.performanceMemAnchor = null;
			}
		}

		public void OnPerformanceFPS(float fps)
		{
			this.fpsLabel.Text = string.Format("{0:F2} FPS ", fps);
			this.frameTimeLabel.Text = string.Format("{0:F2} ms", 1000.0 / (double)fps);
		}

		public void OnPerformanceFPeak(uint fpeak)
		{
			this.fpeakLabel.Text = string.Format("{0} FPeak ", fpeak);
		}

		public void OnPerformanceMemRsvd(uint memRsvd)
		{
			this.memRsvdLabel.Text = string.Format("{0} Rsvd", memRsvd / 1000000u);
		}

		public void OnPerformanceMemUsed(uint memUsd)
		{
			this.memUsedLabel.Text = string.Format("{0} Used", memUsd / 1000000u);
		}

		public void OnPerformanceMemTexture(uint mem)
		{
			this.memTextureLabel.Text = string.Format("{0} Texture", mem / 1000000u);
		}

		public void OnPerformanceMemMesh(uint mem)
		{
			this.memMeshLabel.Text = string.Format("{0} Mesh", mem / 1000000u);
		}

		public void OnPerformanceMemAudio(uint mem)
		{
			this.memAudioLabel.Text = string.Format("{0} Audio", mem / 1000000u);
		}

		public void OnPerformanceMemAnimation(uint mem)
		{
			this.memAnimationLabel.Text = string.Format("{0:F1} Animation", mem / 1000000u);
		}

		public void OnPerformanceMemMaterials(uint mem)
		{
			this.memMaterialsLabel.Text = string.Format("{0:F1} Material", mem / 1000000u);
		}

		public void OnPerformanceDeviceMemUsage(long memory)
		{
			this.deviceMemUsedLabel.Text = string.Format("{0:F2} MB", memory / 1048576L);
		}

		private void SetupDeployableTroops()
		{
			GameStateMachine gameStateMachine = Service.GameStateMachine;
			if (gameStateMachine.PreviousStateType == typeof(BattleStartState) && gameStateMachine.PreviousStateType != typeof(FueBattleStartState) && gameStateMachine.CurrentState is BattlePlayState)
			{
				return;
			}
			if (this.troopGrid.Count != 0)
			{
				this.troopGrid.RepositionItems();
				return;
			}
			this.troopGrid.Visible = true;
			this.deployableTroops = new Dictionary<string, DeployableTroopControl>();
			BattleController battleController = Service.BattleController;
			Dictionary<string, int> allPlayerDeployableTroops = battleController.GetAllPlayerDeployableTroops();
			Dictionary<string, int> allPlayerDeployableSpecialAttacks = battleController.GetAllPlayerDeployableSpecialAttacks();
			Dictionary<string, int> allPlayerDeployableHeroes = battleController.GetAllPlayerDeployableHeroes();
			Dictionary<string, int> allPlayerDeployableChampions = battleController.GetAllPlayerDeployableChampions();
			CurrentBattle currentBattle = battleController.GetCurrentBattle();
			BattleDeploymentData seededPlayerDeployableData = currentBattle.SeededPlayerDeployableData;
			if (allPlayerDeployableTroops != null)
			{
				this.CreateDeployableControls(allPlayerDeployableTroops, seededPlayerDeployableData.TroopData, new UXCheckboxSelectedDelegate(this.OnTroopCheckboxSelected), currentBattle);
			}
			if (allPlayerDeployableSpecialAttacks != null)
			{
				this.CreateDeployableControls(allPlayerDeployableSpecialAttacks, seededPlayerDeployableData.SpecialAttackData, new UXCheckboxSelectedDelegate(this.OnSpecialAttackCheckboxSelected), currentBattle);
			}
			if (allPlayerDeployableHeroes != null)
			{
				this.CreateDeployableControls(allPlayerDeployableHeroes, seededPlayerDeployableData.HeroData, new UXCheckboxSelectedDelegate(this.OnHeroCheckboxSelected), currentBattle);
			}
			if (allPlayerDeployableChampions != null)
			{
				this.CreateDeployableControls(allPlayerDeployableChampions, seededPlayerDeployableData.ChampionData, new UXCheckboxSelectedDelegate(this.OnChampionCheckboxSelected), currentBattle);
			}
			if (battleController.CanPlayerDeploySquadTroops())
			{
				this.CreateSquadDeployableControl();
			}
			int highestLevelHQ = Service.BuildingLookupController.GetHighestLevelHQ();
			int aUTOSELECT_DISABLE_HQTHRESHOLD = GameConstants.AUTOSELECT_DISABLE_HQTHRESHOLD;
			if ((highestLevelHQ < aUTOSELECT_DISABLE_HQTHRESHOLD || aUTOSELECT_DISABLE_HQTHRESHOLD < 1) && this.troopGrid.Count != 0)
			{
				UXCheckbox uXCheckbox = this.troopGrid.GetItem(0).Tag as UXCheckbox;
				uXCheckbox.Selected = true;
				uXCheckbox.OnSelected(uXCheckbox, true);
			}
			this.troopGrid.RepositionItems();
		}

		public bool IsElementProvidedTroop(UXElement element)
		{
			if (element is UXCheckbox && element.Tag is string)
			{
				UXElement optionalSubElement = this.troopGrid.GetOptionalSubElement<UXElement>((string)element.Tag, "ProvidedFrame");
				return optionalSubElement != null && optionalSubElement.Visible;
			}
			return false;
		}

		private void CreateDeployableControls(Dictionary<string, int> deployables, Dictionary<string, int> seededDeployables, UXCheckboxSelectedDelegate onSelected, CurrentBattle battle)
		{
			this.troopGrid.SetTemplateItem("TroopTemplate");
			StaticDataController staticDataController = Service.StaticDataController;
			ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
			bool flag = onSelected == new UXCheckboxSelectedDelegate(this.OnSpecialAttackCheckboxSelected);
			bool flag2 = onSelected == new UXCheckboxSelectedDelegate(this.OnHeroCheckboxSelected);
			bool flag3 = onSelected == new UXCheckboxSelectedDelegate(this.OnChampionCheckboxSelected);
			foreach (KeyValuePair<string, int> current in deployables)
			{
				string key = current.Key;
				int value = current.Value;
				if (value > 0)
				{
					IDeployableVO arg_B0_0;
					if (flag)
					{
						IDeployableVO optional = staticDataController.GetOptional<SpecialAttackTypeVO>(key);
						arg_B0_0 = optional;
					}
					else
					{
						arg_B0_0 = staticDataController.GetOptional<TroopTypeVO>(key);
					}
					IDeployableVO deployableVO = arg_B0_0;
					if (deployableVO != null)
					{
						bool flag4 = seededDeployables != null && seededDeployables.ContainsKey(current.Key);
						string text = key;
						UXElement uXElement = this.troopGrid.CloneTemplateItem(text);
						UXCheckbox subElement = this.troopGrid.GetSubElement<UXCheckbox>(text, "CheckboxTroop");
						subElement.Tag = key;
						subElement.OnSelected = onSelected;
						subElement.Selected = false;
						uXElement.Tag = subElement;
						UXSprite subElement2 = this.troopGrid.GetSubElement<UXSprite>(text, "SpriteTroop");
						UXSprite subElement3 = this.troopGrid.GetSubElement<UXSprite>(text, "SpriteTroopDim");
						UXElement subElement4 = this.troopGrid.GetSubElement<UXElement>(text, "HeroAbilityActive");
						UXElement subElement5 = this.troopGrid.GetSubElement<UXElement>(text, "HeroReady");
						UXElement subElement6 = this.troopGrid.GetSubElement<UXElement>(text, "EquipmentFX");
						UXSprite subElement7 = this.troopGrid.GetSubElement<UXSprite>(text, "SpriteSquad");
						UXSprite subElement8 = this.troopGrid.GetSubElement<UXSprite>(text, "SpriteTroopBkg");
						string defaultCardName = "TroopFrameBgDefault";
						string cardName = "TroopFrameBgQ{0}";
						if (flag4)
						{
							defaultCardName = "ProvidedFrameDefault";
							cardName = "ProvidedFrameQ{0}";
							this.troopGrid.GetSubElement<UXElement>(text, "StandardFrame").Visible = false;
							this.troopGrid.GetSubElement<UXElement>(text, "ProvidedFrame").Visible = true;
							subElement3 = this.troopGrid.GetSubElement<UXSprite>(text, "SpriteProvidedTroopDim");
							subElement8 = this.troopGrid.GetSubElement<UXSprite>(text, "SpriteProvidedTroopBkg");
							UXSprite subElement9 = this.troopGrid.GetSubElement<UXSprite>(text, "SpriteProvidedSelected");
							subElement.SetAnimationAndSprite(subElement9);
						}
						base.RevertToOriginalNameRecursively(subElement4.Root, text);
						base.RevertToOriginalNameRecursively(subElement5.Root, text);
						base.RevertToOriginalNameRecursively(subElement6.Root, text);
						subElement4.Visible = false;
						subElement5.Visible = false;
						subElement6.Visible = false;
						ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(deployableVO, subElement2);
						Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(deployableVO, projectorConfig, battle));
						projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
						GeometryProjector optionalGeometry = ProjectorUtils.GenerateProjector(projectorConfig);
						if (flag3)
						{
							FactionDecal.UpdateDeployableDecal(text, this.troopGrid, deployableVO);
							subElement8.SpriteName = "champion_bkg";
						}
						else if (flag2)
						{
							FactionDecal.UpdateDeployableDecal(text, this.troopGrid, deployableVO);
							subElement8.SpriteName = "hero_bkg";
						}
						else if (flag)
						{
							subElement8.SpriteName = "starship_bkg";
						}
						else
						{
							subElement8.SpriteName = "troop_bkg";
						}
						subElement7.Visible = false;
						UXLabel uXLabel = (!flag4) ? this.troopGrid.GetSubElement<UXLabel>(text, "LabelQuantity") : this.troopGrid.GetSubElement<UXLabel>(text, "LabelProvidedQuantity");
						uXLabel.Tag = key;
						uXLabel.Text = value.ToString();
						uXLabel.TextColor = UXUtils.GetCostColor(uXLabel, value != 0, false);
						UXLabel uXLabel2 = (!flag4) ? this.troopGrid.GetSubElement<UXLabel>(text, "LabelTroopLevel") : this.troopGrid.GetSubElement<UXLabel>(text, "LabelProvidedTroopLevel");
						uXLabel2.Text = LangUtils.GetLevelText(deployableVO.Lvl);
						string text2 = null;
						if (deployableVO is TroopTypeVO)
						{
							TroopTypeVO troop = deployableVO as TroopTypeVO;
							Service.SkinController.GetApplicableSkin(troop, activeArmory.Equipment, out text2);
						}
						int qualityInt;
						if (!string.IsNullOrEmpty(text2))
						{
							EquipmentVO equipmentVO = staticDataController.Get<EquipmentVO>(text2);
							qualityInt = (int)equipmentVO.Quality;
						}
						else
						{
							qualityInt = Service.DeployableShardUnlockController.GetUpgradeQualityForDeployable(deployableVO);
						}
						UXUtils.SetCardQuality(this, this.troopGrid, text, qualityInt, cardName, defaultCardName);
						DeployableTroopControl value2 = new DeployableTroopControl(subElement, uXLabel, subElement3, subElement4, subElement5, optionalGeometry, flag2, flag, key, subElement6);
						this.deployableTroops.Add(key, value2);
						this.troopGrid.AddItem(uXElement, deployableVO.Order);
					}
				}
			}
			this.troopGrid.Scroll(0f);
		}

		private void CreateSquadDeployableControl()
		{
			this.troopGrid.SetTemplateItem("TroopTemplate");
			UXElement uXElement = this.troopGrid.CloneTemplateItem("squadTroops");
			UXCheckbox subElement = this.troopGrid.GetSubElement<UXCheckbox>("squadTroops", "CheckboxTroop");
			subElement.Tag = "squadTroops";
			subElement.OnSelected = new UXCheckboxSelectedDelegate(this.OnSquadTroopsCheckboxSelected);
			subElement.Selected = false;
			uXElement.Tag = subElement;
			UXSprite subElement2 = this.troopGrid.GetSubElement<UXSprite>("squadTroops", "SpriteTroop");
			UXSprite subElement3 = this.troopGrid.GetSubElement<UXSprite>("squadTroops", "SpriteTroopDim");
			UXSprite subElement4 = this.troopGrid.GetSubElement<UXSprite>("squadTroops", "SpriteSquad");
			UXSprite subElement5 = this.troopGrid.GetSubElement<UXSprite>("squadTroops", "SpriteTroopBkg");
			UXElement subElement6 = this.troopGrid.GetSubElement<UXElement>("squadTroops", "HeroAbilityActive");
			UXElement subElement7 = this.troopGrid.GetSubElement<UXElement>("squadTroops", "HeroReady");
			UXElement subElement8 = this.troopGrid.GetSubElement<UXElement>("squadTroops", "EquipmentFX");
			subElement6.Visible = false;
			subElement7.Visible = false;
			subElement8.Visible = false;
			subElement.SetTweenTarget(subElement4);
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad != null)
			{
				subElement4.SpriteName = currentSquad.Symbol;
			}
			subElement5.SpriteName = "troop_bkg";
			subElement2.Visible = false;
			subElement4.Visible = true;
			UXLabel subElement9 = this.troopGrid.GetSubElement<UXLabel>("squadTroops", "LabelQuantity");
			subElement9.Tag = "squadTroops";
			subElement9.Text = "1";
			subElement9.TextColor = UXUtils.GetCostColor(subElement9, true, false);
			DeployableTroopControl value = new DeployableTroopControl(subElement, subElement9, subElement3, null, null, null, false, false, "squadTroops", null);
			this.deployableTroops.Add("squadTroops", value);
			this.troopGrid.AddItem(uXElement, 99999999);
		}

		private void CleanupDeployableTroops()
		{
			this.troopGrid.Clear();
			this.troopGrid.HideScrollArrows();
			this.troopGrid.Visible = false;
			if (this.deployableTroops != null)
			{
				foreach (DeployableTroopControl current in this.deployableTroops.Values)
				{
					current.StopObserving();
				}
				this.deployableTroops = null;
			}
		}

		private void UpdateDeployInstructionLabel()
		{
			if (!this.player.CampaignProgress.FueInProgress)
			{
				this.deployInstructionsLabel.Visible = false;
				return;
			}
			bool visible = false;
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			if (currentBattle != null && this.deployableTroops != null && this.deployableTroops.Count > 0)
			{
				if (currentBattle.Type != BattleType.PveDefend)
				{
					this.deployInstructionsLabel.Text = this.lang.Get("DEPLOY_INSTRUCTIONS", new object[0]);
				}
				else
				{
					this.deployInstructionsLabel.Text = this.lang.Get("DEPLOY_DEFENSE_INSTRUCTIONS", new object[0]);
				}
				visible = true;
			}
			this.deployInstructionsLabel.Visible = visible;
		}

		public void RefreshPlayerSocialInformation()
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			this.playerNameLabel.Text = worldOwner.PlayerName;
			this.neighborNameLabel.Text = worldOwner.PlayerName;
			if (worldOwner.Squad != null)
			{
				this.playerClanLabel.Text = worldOwner.Squad.SquadName;
				this.neighborSquadLabel.Text = worldOwner.Squad.SquadName;
			}
			else
			{
				this.playerClanLabel.Text = string.Empty;
				this.neighborSquadLabel.Text = string.Empty;
			}
			string factionUpgradeIcon = string.Empty;
			int rating = GameUtils.CalculatePlayerVictoryRating(worldOwner);
			FactionType faction = worldOwner.Faction;
			if (faction != FactionType.Empire)
			{
				if (faction != FactionType.Rebel)
				{
					factionUpgradeIcon = "HudXpBg";
				}
				else
				{
					factionUpgradeIcon = Service.FactionIconUpgradeController.GetIcon(FactionType.Rebel, rating);
				}
			}
			else
			{
				factionUpgradeIcon = Service.FactionIconUpgradeController.GetIcon(FactionType.Empire, rating);
			}
			this.RefreshFactionIconVisibility(rating, factionUpgradeIcon, worldOwner);
		}

		private void RefreshFactionIconVisibility(int rating, string factionUpgradeIcon, GamePlayer worldOwner)
		{
			bool flag = worldOwner.Faction == FactionType.Empire;
			bool flag2 = Service.FactionIconUpgradeController.UseUpgradeImage(rating);
			this.factionBackground.Visible = !flag2;
			this.neighborFactionBackground.Visible = !flag2;
			this.factionBackgroundUpgradeRebel.Visible = (flag2 && !flag);
			this.neighborFactionBackgroundUpgradeRebel.Visible = (flag2 && !flag);
			this.factionBackgroundUpgradeEmpire.Visible = (flag2 && flag);
			this.neighborFactionBackgroundUpgradeEmpire.Visible = (flag2 && flag);
			if (flag2)
			{
				if (!flag)
				{
					this.neighborFactionBackgroundUpgradeRebel.SpriteName = factionUpgradeIcon;
					this.factionBackgroundUpgradeRebel.SpriteName = factionUpgradeIcon;
				}
				else
				{
					this.factionBackgroundUpgradeEmpire.SpriteName = factionUpgradeIcon;
					this.neighborFactionBackgroundUpgradeEmpire.SpriteName = factionUpgradeIcon;
				}
			}
			else
			{
				this.neighborFactionBackground.SpriteName = factionUpgradeIcon;
				this.factionBackground.SpriteName = factionUpgradeIcon;
			}
		}

		private void RefreshAllResourceViews(bool animate)
		{
			this.RefreshResourceView("credits", animate);
			this.RefreshResourceView("materials", animate);
			this.RefreshResourceView("contraband", animate);
			this.RefreshResourceView("crystals", animate);
		}

		private void RefreshResourceView(string resourceKey, bool animate)
		{
			if (this.creditsView == null || this.materialsView == null || this.contrabandView == null || this.crystalsView == null)
			{
				return;
			}
			if (resourceKey == "credits")
			{
				this.creditsView.SetAmount(this.player.CurrentCreditsAmount, this.player.MaxCreditsAmount, animate);
			}
			else if (resourceKey == "materials")
			{
				this.materialsView.SetAmount(this.player.CurrentMaterialsAmount, this.player.MaxMaterialsAmount, animate);
			}
			else if (resourceKey == "contraband")
			{
				this.contrabandView.SetAmount(this.player.CurrentContrabandAmount, this.player.MaxContrabandAmount, animate);
			}
			else if (resourceKey == "crystals")
			{
				this.crystalsView.SetAmount(this.player.CurrentCrystalsAmount, -1, animate);
			}
			if (!this.registeredFrameTimeObserver && (this.creditsView.NeedsUpdate || this.materialsView.NeedsUpdate || this.contrabandView.NeedsUpdate || this.crystalsView.NeedsUpdate))
			{
				this.registeredFrameTimeObserver = true;
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
		}

		public void RefreshTimerView(int seconds, bool warning)
		{
			int num = seconds / 60;
			seconds -= num * 60;
			string text = Service.Lang.Get("MINUTES_SECONDS", new object[]
			{
				num,
				seconds
			});
			this.timeLeftLabel.Text = text;
			this.timeLeftLabel.TextColor = UXUtils.GetCostColor(this.timeLeftLabel, !warning || (seconds & 1) == 1, false);
		}

		public void RefreshReplayTimerView(int seconds)
		{
			BattleRecord currentBattleRecord = Service.BattlePlaybackController.CurrentBattleRecord;
			if (currentBattleRecord == null || currentBattleRecord.BattleAttributes == null)
			{
				return;
			}
			seconds -= currentBattleRecord.BattleAttributes.TimeLeft;
			int num = seconds / 60;
			seconds -= num * 60;
			string text = Service.Lang.Get("MINUTES_SECONDS", new object[]
			{
				num,
				seconds
			});
			this.replayTimeLeftLabel.Text = text;
		}

		private void SetOpponentLevelVisibility(bool vis)
		{
			this.opponentRankLabel.Visible = vis;
			this.opponentRankBG.Visible = vis;
		}

		private void RefreshBaseName()
		{
			WorldTransitioner worldTransitioner = Service.WorldTransitioner;
			this.baseNameLabel.Text = worldTransitioner.GetCurrentWorldName();
			if (!worldTransitioner.IsCurrentWorldHome())
			{
				string spriteName = string.Empty;
				bool flag = false;
				PvpTarget currentPvpTarget = Service.PvpManager.CurrentPvpTarget;
				bool flag2 = true;
				if (currentPvpTarget != null)
				{
					this.SetOpponentLevelVisibility(true);
					flag2 = (currentPvpTarget.PlayerFaction == FactionType.Empire);
					FactionIconUpgradeController factionIconUpgradeController = Service.FactionIconUpgradeController;
					int rating = currentPvpTarget.PlayerAttacksWon + currentPvpTarget.PlayerDefensesWon;
					spriteName = factionIconUpgradeController.GetIcon(currentPvpTarget.PlayerFaction, rating);
					flag = factionIconUpgradeController.UseUpgradeImage(rating);
					string text = factionIconUpgradeController.RatingToDisplayLevel(rating).ToString();
					this.opponentRankLabel.Text = text;
				}
				else
				{
					this.SetOpponentLevelVisibility(false);
					spriteName = worldTransitioner.GetCurrentWorldFactionAssetName();
				}
				this.opponentSymbol.Visible = !flag;
				this.opponentSymbolUpgradeRebel.Visible = (flag && !flag2);
				this.opponentSymbolUpgradeEmpire.Visible = (flag && flag2);
				if (flag)
				{
					if (!flag2)
					{
						this.opponentSymbolUpgradeRebel.SpriteName = spriteName;
					}
					else
					{
						this.opponentSymbolUpgradeEmpire.SpriteName = spriteName;
					}
				}
				else
				{
					this.opponentSymbol.SpriteName = spriteName;
				}
			}
		}

		private void RefreshPlayerMedals()
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			string text = Service.Lang.ThousandsSeparated(worldOwner.PlayerMedals);
			this.playerMedalLabel.Text = text;
			this.neighborMedalsLabel.Text = text;
		}

		private void RefreshCurrentPlayerLevel()
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			int rating = GameUtils.CalculatePlayerVictoryRating(worldOwner);
			FactionIconUpgradeController factionIconUpgradeController = Service.FactionIconUpgradeController;
			string text = factionIconUpgradeController.RatingToDisplayLevel(rating).ToString();
			this.playerRankLabel.Text = text;
			this.neighborTrophiesLabel.Text = text;
		}

		public UXElement GetDamageStarAnchor()
		{
			return this.damageStarAnchor;
		}

		public UXElement GetDamageStarAnimator()
		{
			return this.damageStarAnimator;
		}

		public UXLabel GetDamageStarLabel()
		{
			return this.damageStarLabel;
		}

		public void UpdateDamageValue(int percentage)
		{
			this.damageValueLabel.Text = this.lang.Get("PERCENTAGE", new object[]
			{
				percentage
			});
		}

		public void UpdateDamageStars(int numEarnedStars)
		{
			this.damageStar1.Color = ((numEarnedStars <= 0) ? Color.black : Color.white);
			this.damageStar2.Color = ((numEarnedStars <= 1) ? Color.black : Color.white);
			this.damageStar3.Color = ((numEarnedStars <= 2) ? Color.black : Color.white);
		}

		public void ResetDamageStars()
		{
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			if (currentBattle != null && currentBattle.Type == BattleType.PveDefend)
			{
				this.UpdateDamageStars(3);
			}
			else
			{
				this.UpdateDamageStars(0);
			}
		}

		public void ShowContextButtons(Entity selectedBuilding)
		{
			AnimController animController = Service.AnimController;
			int i = 0;
			int count = this.curAnims.Count;
			while (i < count)
			{
				Anim anim = this.curAnims[i];
				animController.CompleteAnim(anim);
				UXElement uXElement = (UXElement)anim.Tag;
				uXElement.Root.SetActive(false);
				this.contextButtonPool.Add(uXElement.Root.name, uXElement);
				i++;
			}
			this.curAnims.Clear();
			this.contextDescLabel.Text = string.Empty;
			this.BaseLayoutToolView.SelectedBuildingLabel.Text = string.Empty;
			IState currentState = Service.GameStateMachine.CurrentState;
			bool flag = currentState is HomeState;
			bool flag2 = GameUtils.IsVisitingBase();
			bool flag3 = currentState is WarBoardState;
			bool flag4 = currentState is EditBaseState;
			bool flag5 = Service.BaseLayoutToolController.IsActive();
			BuildingController buildingController = Service.BuildingController;
			UXLabel selectedBuildingLabel = this.contextDescLabel;
			if (flag5)
			{
				selectedBuildingLabel = this.BaseLayoutToolView.SelectedBuildingLabel;
			}
			if (selectedBuilding == null || buildingController.IsPurchasing || (!flag && !flag2 && !flag4 && !flag5 && !flag3) || this.isAnimating)
			{
				selectedBuildingLabel.Text = string.Empty;
				return;
			}
			bool isLifted = buildingController.IsLifted(selectedBuilding);
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			if (buildingComponent == null)
			{
				return;
			}
			BuildingTypeVO buildingType = buildingComponent.BuildingType;
			if (buildingType.Type == BuildingType.Clearable)
			{
				selectedBuildingLabel.Text = this.lang.Get("CLEARABLE_INFO", new object[]
				{
					LangUtils.GetClearableDisplayName(buildingType),
					buildingType.Lvl
				});
			}
			else
			{
				selectedBuildingLabel.Text = this.lang.Get("BUILDING_INFO", new object[]
				{
					LangUtils.GetBuildingDisplayName(buildingType),
					buildingType.Lvl
				});
			}
			bool visible = this.Visible;
			if (!visible)
			{
				this.Visible = true;
			}
			this.contextButtonParentTemplate.Visible = true;
			this.AddContextButtons(selectedBuilding, buildingType, flag, flag2, flag5, isLifted);
			int count2 = this.contextButtons.Count;
			float colliderWidth = this.contextButtonTemplate.ColliderWidth;
			float num = (float)(1 - count2) * 0.5f * colliderWidth;
			for (int j = 0; j < count2; j++)
			{
				UXElement uXElement2 = this.contextButtons[j];
				UXButton element = base.GetElement<UXButton>(UXUtils.FormatAppendedName("ButtonContext", uXElement2.Root.name));
				if (!element.Enabled)
				{
					element = base.GetElement<UXButton>(UXUtils.FormatAppendedName("ButtonContextDim", uXElement2.Root.name));
				}
				Vector3 localPosition = this.contextButtonParentTemplate.LocalPosition;
				localPosition.x = (float)j * colliderWidth + num;
				Vector3 localPosition2 = localPosition;
				localPosition2.y = -localPosition2.y;
				uXElement2.LocalPosition = localPosition2;
				if (flag5)
				{
					UXElement element2 = base.GetElement<UXElement>("StashContextLocator");
					localPosition.y = element2.LocalPosition.y;
				}
				AnimUXPosition animUXPosition = new AnimUXPosition(uXElement2, 0.5f, localPosition);
				animUXPosition.EaseFunctionX = new Easing.EasingDelegate(Easing.AlwaysStart);
				animUXPosition.EaseFunctionY = new Easing.EasingDelegate(Easing.ExpoEaseOut);
				animUXPosition.EaseFunctionZ = new Easing.EasingDelegate(Easing.AlwaysStart);
				animUXPosition.Tag = uXElement2;
				this.curAnims.Add(animUXPosition);
			}
			this.contextButtonParentTemplate.Visible = false;
			if (!visible)
			{
				this.Visible = false;
			}
			int k = 0;
			int count3 = this.curAnims.Count;
			while (k < count3)
			{
				animController.Animate(this.curAnims[k]);
				k++;
			}
		}

		private void AddContextButtons(Entity selectedBuilding, BuildingTypeVO buildingInfo, bool inHomeMode, bool inVisitMode, bool isBaseLayoutToolMode, bool isLifted)
		{
			RaidDefenseController raidDefenseController = Service.RaidDefenseController;
			int numSelectedBuildings = Service.BuildingController.NumSelectedBuildings;
			bool flag = Service.GameStateMachine.CurrentState is WarBaseEditorState;
			this.contextButtons.Clear();
			if (buildingInfo.Type == BuildingType.Clearable)
			{
				if (!inVisitMode)
				{
					if (ContractUtils.IsBuildingClearing(selectedBuilding))
					{
						this.AddContextButton("Cancel", 0, 0, 0, 0);
					}
					else
					{
						this.AddContextButton("Clear", buildingInfo.Credits, buildingInfo.Materials, buildingInfo.Contraband, 0);
					}
				}
				return;
			}
			if (!isLifted && numSelectedBuildings <= 1)
			{
				if (!isBaseLayoutToolMode)
				{
					this.AddContextButton("Info", 0, 0, 0, 0);
				}
				if (inHomeMode)
				{
					this.AddContextButton("Move", 0, 0, 0, 0);
				}
			}
			if (Service.PostBattleRepairController.IsEntityInRepair(selectedBuilding))
			{
				return;
			}
			if (isBaseLayoutToolMode && !isLifted)
			{
				this.AddContextButton("Stash", 0, 0, 0, 0);
			}
			if (numSelectedBuildings > 1 && buildingInfo.Type == BuildingType.Wall)
			{
				this.AddContextButton("RotateWall", 0, 0, 0, 0);
			}
			if (!inVisitMode && !isLifted && numSelectedBuildings <= 1)
			{
				if (buildingInfo.Type == BuildingType.Squad)
				{
					this.AddSquadContextButtons(selectedBuilding, buildingInfo, inHomeMode, isBaseLayoutToolMode);
				}
				if (buildingInfo.Type == BuildingType.Wall && !inHomeMode)
				{
					this.AddContextButton("SelectRow", 0, 0, 0, 0);
				}
				if (buildingInfo.Type == BuildingType.DroidHut && !flag)
				{
					if (this.player.CurrentDroidsAmount < this.player.MaxDroidsAmount)
					{
						int crystals = GameUtils.DroidCrystalCost(this.player.CurrentDroidsAmount);
						this.AddContextButton("Buy_Droid", 0, 0, 0, crystals);
					}
				}
				else if (!isBaseLayoutToolMode && ContractUtils.IsBuildingConstructing(selectedBuilding))
				{
					this.AddFinishContextButton(selectedBuilding);
				}
				else if (!isBaseLayoutToolMode && (ContractUtils.IsBuildingUpgrading(selectedBuilding) || ContractUtils.IsBuildingSwapping(selectedBuilding) || ContractUtils.IsChampionRepairing(selectedBuilding)))
				{
					this.AddContextButton("Cancel", 0, 0, 0, 0);
					this.AddFinishContextButton(selectedBuilding);
					BuildingType type = buildingInfo.Type;
					if (type != BuildingType.HQ)
					{
						if (type == BuildingType.NavigationCenter)
						{
							if (inHomeMode)
							{
								this.AddContextButton("Navigate", 0, 0, 0, 0, "context_Galaxy");
							}
						}
					}
					else
					{
						this.AddHQInventoryContextButtonIfProper();
					}
				}
				else
				{
					bool flag2 = buildingInfo.Type == BuildingType.ChampionPlatform && !Service.ChampionController.IsChampionAvailable((SmartEntity)selectedBuilding);
					if (GameUtils.IsBuildingUpgradable(buildingInfo) && !flag2 && !isBaseLayoutToolMode)
					{
						BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
						if (Service.ISupportController.FindBuildingContract(buildingComponent.BuildingTO.Key) == null)
						{
							BuildingTypeVO nextLevel = Service.BuildingUpgradeCatalog.GetNextLevel(buildingInfo);
							this.AddContextButton("Upgrade", nextLevel.UpgradeCredits, nextLevel.UpgradeMaterials, nextLevel.UpgradeContraband, 0);
						}
					}
					if (buildingInfo.Type == BuildingType.Trap && !isBaseLayoutToolMode)
					{
						if (selectedBuilding.Get<TrapComponent>().CurrentState == TrapState.Spent)
						{
							TrapTypeVO trapTypeVO = Service.StaticDataController.Get<TrapTypeVO>(buildingInfo.TrapUid);
							this.AddContextButton("Trap_Rearm", trapTypeVO.RearmCreditsCost, trapTypeVO.RearmMaterialsCost, trapTypeVO.RearmContrabandCost, 0);
						}
						List<Entity> rearmableTraps = TrapUtils.GetRearmableTraps();
						if (rearmableTraps.Count > 1 || rearmableTraps.Count == 0 || rearmableTraps[0] != selectedBuilding)
						{
							int credits;
							int materials;
							int contraband;
							TrapUtils.GetRearmAllTrapsCost(out credits, out materials, out contraband);
							this.AddContextButton("Trap_RearmAll", credits, materials, contraband, 0);
						}
					}
					if (inHomeMode)
					{
						switch (buildingInfo.Type)
						{
						case BuildingType.HQ:
							this.AddHQInventoryContextButtonIfProper();
							break;
						case BuildingType.Barracks:
							this.AddContextButton("Train", 0, 0, 0, 0);
							break;
						case BuildingType.Factory:
							this.AddContextButton("Build", 0, 0, 0, 0);
							break;
						case BuildingType.FleetCommand:
							this.AddContextButton("Commission", 0, 0, 0, 0);
							break;
						case BuildingType.HeroMobilizer:
							this.AddContextButton("Mobilize", 0, 0, 0, 0);
							break;
						case BuildingType.ChampionPlatform:
							if (flag2)
							{
								this.AddContextButton("Repair", 0, 0, 0, 0);
							}
							break;
						case BuildingType.Turret:
							if (Service.BuildingLookupController.IsTurretSwappingUnlocked())
							{
								this.AddContextButton("Swap", 0, 0, 0, 0);
							}
							break;
						case BuildingType.TroopResearch:
							if (ContractUtils.IsArmyUpgrading(selectedBuilding))
							{
								if (ContractUtils.CanCancelDeployableContract(selectedBuilding))
								{
									this.AddContextButton("Cancel", 0, 0, 0, 0);
								}
								this.AddFinishContextButton(selectedBuilding);
							}
							else if (ContractUtils.IsEquipmentUpgrading(selectedBuilding))
							{
								this.AddFinishContextButton(selectedBuilding);
							}
							this.AddContextButton("Upgrade_Troops", 0, 0, 0, 0);
							break;
						case BuildingType.DefenseResearch:
							this.AddContextButton("Upgrade_Defense", 0, 0, 0, 0);
							break;
						case BuildingType.Resource:
						{
							string contextId = null;
							switch (buildingInfo.Currency)
							{
							case CurrencyType.Credits:
								contextId = "Credits";
								break;
							case CurrencyType.Materials:
								contextId = "Materials";
								break;
							case CurrencyType.Contraband:
								contextId = "Contraband";
								break;
							}
							this.AddContextButton(contextId, 0, 0, 0, 0);
							break;
						}
						case BuildingType.Cantina:
							this.AddContextButton("Hire", 0, 0, 0, 0);
							break;
						case BuildingType.NavigationCenter:
							if (!isLifted)
							{
								this.AddContextButton("Navigate", 0, 0, 0, 0, "context_Galaxy");
							}
							break;
						case BuildingType.ScoutTower:
							if (!Service.RaidDefenseController.IsRaidAvailable())
							{
								this.AddContextButtonWithTimer("NextRaid", 0, 0, 0, 0, "context_NightRaid", new GetTimerSecondsDelegate(raidDefenseController.GetRaidTimeSeconds));
							}
							else
							{
								this.AddContextButton("RaidBriefing", 0, 0, 0, 0, "context_NightRaid");
								this.AddContextButton("RaidDefend", 0, 0, 0, 0, "icoDefend", "BtnTroopBg_Gold");
							}
							break;
						case BuildingType.Armory:
							this.AddContextButton("Armory", 0, 0, 0, 0);
							break;
						}
					}
				}
			}
		}

		private void AddHQInventoryContextButtonIfProper()
		{
			if (!this.player.CampaignProgress.FueInProgress)
			{
				int numInventoryItemsNotViewed = GameUtils.GetNumInventoryItemsNotViewed();
				this.AddContextButton("Inventory", 0, 0, 0, 0, numInventoryItemsNotViewed, null, null);
			}
		}

		private void AddSquadContextButtons(Entity selectedBuilding, BuildingTypeVO buildingInfo, bool inHomeMode, bool isBaseLayoutToolMode)
		{
			if (isBaseLayoutToolMode)
			{
				return;
			}
			if (Service.SquadController.StateManager.GetCurrentSquad() != null)
			{
				SquadController squadController = Service.SquadController;
				if (SquadUtils.GetDonatedTroopStorageUsedByCurrentPlayer() < buildingInfo.Storage)
				{
					uint serverTime = Service.ServerAPI.ServerTime;
					uint troopRequestDate = squadController.StateManager.TroopRequestDate;
					if (SquadUtils.CanSendFreeTroopRequest(serverTime, troopRequestDate))
					{
						this.AddContextButton("RequestTroops", 0, 0, 0, 0);
					}
					else
					{
						int troopRequestCrystalCost = SquadUtils.GetTroopRequestCrystalCost(serverTime, troopRequestDate);
						this.AddContextButton("RequestTroopsPaid", 0, 0, 0, troopRequestCrystalCost, "context_Finish_Now");
					}
				}
			}
			else if (inHomeMode)
			{
				this.AddContextButton("Join", 0, 0, 0, 0, "context_Squad");
			}
		}

		private void AddFinishContextButton(Entity selectedBuilding)
		{
			Contract contract = Service.ISupportController.FindCurrentContract(selectedBuilding.Get<BuildingComponent>().BuildingTO.Key);
			int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(contract);
			this.AddContextButton("Finish_Now", 0, 0, 0, crystalCostToFinishContract);
		}

		public void AddCustomContextButton(string contextId)
		{
			this.AddContextButton(contextId, 0, 0, 0, 0, 0, null, null);
		}

		private void AddContextButton(string contextId, int credits, int materials, int contraband, int crystals)
		{
			this.AddContextButton(contextId, credits, materials, contraband, crystals, 0, null, null);
		}

		private void AddContextButton(string contextId, int credits, int materials, int contraband, int crystals, string spriteName)
		{
			this.AddContextButton(contextId, credits, materials, contraband, crystals, 0, spriteName, null);
		}

		private void AddContextButtonWithTimer(string contextId, int credits, int materials, int contraband, int crystals, string spriteName, GetTimerSecondsDelegate getTimeSeconds)
		{
			ContextButtonTag contextButtonTag = this.AddContextButton(contextId, credits, materials, contraband, crystals, 0, spriteName, null);
			contextButtonTag.TimerLabel = contextButtonTag.HardCostLabel;
			contextButtonTag.TimerSecondsDelegate = getTimeSeconds;
			this.UpdateContextTimerLabel(contextButtonTag);
		}

		private void AddContextButton(string contextId, int credits, int materials, int contraband, int crystals, string spriteName, string bgSpriteName)
		{
			this.AddContextButton(contextId, credits, materials, contraband, crystals, 0, spriteName, bgSpriteName);
		}

		private UXElement GetContextButtonFromPool(UXElement template, string name, GameObject parent)
		{
			if (this.contextButtonPool.ContainsKey(name))
			{
				UXElement uXElement = this.contextButtonPool[name];
				this.contextButtonPool.Remove(name);
				uXElement.Root.SetActive(true);
				return uXElement;
			}
			return base.CloneElement<UXElement>(template, name, parent);
		}

		private ContextButtonTag AddContextButton(string contextId, int credits, int materials, int contraband, int crystals, int badgeCount, string spriteName, string bgSpriteName)
		{
			string text = string.Format("{0}_{1}", this.contextButtonParentTemplate.Root.name, contextId);
			UXElement contextButtonFromPool = this.GetContextButtonFromPool(this.contextButtonParentTemplate, text, this.contextButtonParentTemplate.Root.transform.parent.gameObject);
			ContextButtonTag contextButtonTag = new ContextButtonTag();
			contextButtonFromPool.Tag = contextButtonTag;
			contextButtonTag.ContextButton = base.GetElement<UXButton>(UXUtils.FormatAppendedName("ButtonContext", text));
			contextButtonTag.ContextButton.OnClicked = new UXButtonClickedDelegate(this.OnContextButtonClicked);
			contextButtonTag.ContextButton.Tag = contextId;
			contextButtonTag.ContextDimButton = base.GetElement<UXButton>(UXUtils.FormatAppendedName("ButtonContextDim", text));
			contextButtonTag.ContextDimButton.Enabled = false;
			contextButtonTag.ContextDimButton.Tag = contextId;
			contextButtonTag.ContextDimButton.OnClicked = new UXButtonClickedDelegate(this.OnDisabledContextButtonClicked);
			contextButtonTag.ContextId = contextId;
			contextButtonTag.ContextBackground = base.GetElement<UXSprite>(UXUtils.FormatAppendedName("BackgroundContext", text));
			if (!string.IsNullOrEmpty(bgSpriteName))
			{
				contextButtonTag.ContextBackground.SpriteName = bgSpriteName;
			}
			if (spriteName != null)
			{
				contextButtonTag.SpriteName = spriteName;
			}
			else
			{
				contextButtonTag.SpriteName = "context_" + contextButtonTag.ContextId;
			}
			contextButtonTag.ContextIconSprite = base.GetElement<UXSprite>(UXUtils.FormatAppendedName("SpriteContextIcon", text));
			contextButtonTag.ContextIconSprite.SpriteName = contextButtonTag.SpriteName;
			contextButtonTag.ContextLabel = base.GetElement<UXLabel>(UXUtils.FormatAppendedName("LabelContext", text));
			contextButtonTag.ContextLabel.Text = LangUtils.GetContextButtonText(contextButtonTag.ContextId);
			contextButtonTag.HardCostLabel = base.GetElement<UXLabel>(UXUtils.FormatAppendedName("LabelHardCost", text));
			contextButtonTag.HardCostLabel.Text = ((crystals != 0) ? this.lang.ThousandsSeparated(crystals) : string.Empty);
			contextButtonTag.HardCostLabel.TextColor = UXUtils.GetCostColor(contextButtonTag.HardCostLabel, GameUtils.CanAffordCrystals(crystals), false);
			UXElement element = base.GetElement<UXElement>(UXUtils.FormatAppendedName("ContainerJewelContext", text));
			if (badgeCount > 0)
			{
				element.Visible = true;
				base.GetElement<UXLabel>(UXUtils.FormatAppendedName("LabelMessageCountContext", text)).Text = badgeCount.ToString();
			}
			else
			{
				element.Visible = false;
			}
			UXUtils.SetupCostElements(this, "Cost", text, credits, materials, contraband, 0, false, null, 108);
			this.ToggleContextButton(contextButtonFromPool, !this.ShouldDisableContextButton(contextId, credits, materials, contraband, crystals));
			this.contextButtons.Add(contextButtonFromPool);
			return contextButtonTag;
		}

		private bool ShouldDisableContextButton(string contextId, int credits, int materials, int contraband, int crystals)
		{
			if (contextId == "Credits" || contextId == "Materials" || contextId == "Contraband")
			{
				return !Service.ICurrencyController.IsGeneratorCollectable(Service.BuildingController.SelectedBuilding);
			}
			return contextId == "Trap_RearmAll" && (credits == 0 && materials == 0) && crystals == 0;
		}

		public void ToggleContextButton(string contextId, bool enabled)
		{
			if (this.contextButtons == null)
			{
				return;
			}
			UXElement button = null;
			for (int i = 0; i < this.contextButtons.Count; i++)
			{
				if ((this.contextButtons[i].Tag as ContextButtonTag).ContextId == contextId)
				{
					button = this.contextButtons[i];
					break;
				}
			}
			this.ToggleContextButton(button, enabled);
		}

		public void ToggleContextButton(UXElement button, bool enabled)
		{
			if (button == null)
			{
				return;
			}
			ContextButtonTag contextButtonTag = button.Tag as ContextButtonTag;
			contextButtonTag.ContextDimButton.Enabled = !enabled;
			contextButtonTag.ContextDimButton.Visible = !enabled;
			contextButtonTag.ContextButton.Enabled = enabled;
			contextButtonTag.ContextLabel.TextColor = UXUtils.GetCostColor(contextButtonTag.ContextLabel, true, !enabled);
		}

		private bool IsAnimationWhiteListed(GameObject gameObject)
		{
			bool result = false;
			if (gameObject != null)
			{
				string name = gameObject.name;
				int num = this.ANIMATION_TRANSITION_WHITE_LIST.Length;
				for (int i = 0; i < num; i++)
				{
					if (name == this.ANIMATION_TRANSITION_WHITE_LIST[i])
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private void AnimateControls(bool bringIn, float speed)
		{
			if (bringIn == this.broughtIn)
			{
				return;
			}
			float num = 0f;
			int i = 0;
			int num2 = this.animations.Length;
			while (i < num2)
			{
				Animation animation = this.animations[i];
				GameObject gameObject = animation.gameObject;
				if (this.IsAnimationWhiteListed(gameObject))
				{
					foreach (AnimationState animationState in animation.gameObject.GetComponent<Animation>())
					{
						if (bringIn)
						{
							animationState.speed = speed;
							animationState.time = 0f;
						}
						else
						{
							animationState.speed = -speed;
							animationState.time = animationState.length;
						}
						if (animationState.length > num && bringIn)
						{
							num = animationState.length;
						}
					}
					this.animations[i].Play();
				}
				i++;
			}
			if (num > 0f)
			{
				this.isAnimating = true;
				Service.ViewTimerManager.CreateViewTimer(num, false, new TimerDelegate(this.OnAnimationComplete), null);
			}
			this.broughtIn = bringIn;
		}

		private void OnAnimationComplete(uint timerId, object cookie)
		{
			this.isAnimating = false;
			this.Visible = !this.setInvisibleAfterAnimating;
			this.setInvisibleAfterAnimating = false;
		}

		public void UpdateDroidCount()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is HomeState || currentState is EditBaseState)
			{
				HomeModeController homeModeController = Service.HomeModeController;
				if (homeModeController != null && homeModeController.Enabled)
				{
					int num = ContractUtils.CalculateDroidsInUse();
					int currentDroidsAmount = this.player.CurrentDroidsAmount;
					int num2 = currentDroidsAmount - num;
					string text = this.lang.Get("FRACTION", new object[]
					{
						num2,
						currentDroidsAmount
					});
					this.droidAddLabel.Text = text;
					this.droidMaxLabel.Text = text;
					bool flag = this.player.CurrentDroidsAmount < this.player.MaxDroidsAmount;
					this.droidAddGroup.Visible = flag;
					this.droidMaxGroup.Visible = !flag;
				}
			}
		}

		public void ToggleExitEditModeButton(bool show)
		{
			if (show)
			{
				this.CurrentHudConfig.Add("ButtonExitEdit");
			}
			else
			{
				this.CurrentHudConfig.Remove("ButtonExitEdit");
			}
			this.exitEditButton.Visible = show;
			if (show && this.exitEditAnimation != null)
			{
				this.exitEditAnimation.Play();
			}
		}

		public void ConfigureControls(HudConfig config)
		{
			if (!base.IsLoaded())
			{
				return;
			}
			this.CurrentHudConfig = config;
			if (this.genericConfigElements == null)
			{
				this.genericConfigElements = new List<UXElement>();
				this.genericConfigElements.Add(this.currencyGroup);
				this.genericConfigElements.Add(this.droidButton);
				this.genericConfigElements.Add(this.crystalButton);
				this.genericConfigElements.Add(this.opponentGroup);
				this.genericConfigElements.Add(this.prebattleMedalsGroup);
				this.genericConfigElements.Add(this.playerGroup);
				this.genericConfigElements.Add(this.shieldGroup);
				this.genericConfigElements.Add(this.specialPromotionGroup);
				this.genericConfigElements.Add(this.baseNameLabel);
				this.genericConfigElements.Add(this.homeButton);
				this.genericConfigElements.Add(this.editButton);
				this.genericConfigElements.Add(this.exitEditButton);
				this.genericConfigElements.Add(this.storeButton);
				this.genericConfigElements.Add(this.battleButton);
				this.genericConfigElements.Add(this.warButton);
				this.genericConfigElements.Add(this.logButton);
				this.genericConfigElements.Add(this.leaderboardButton);
				this.genericConfigElements.Add(this.settingsButton);
				this.genericConfigElements.Add(this.joinSquadButton);
				this.genericConfigElements.Add(this.endBattleButton);
				this.genericConfigElements.Add(this.damageStarGroup);
				this.genericConfigElements.Add(this.replayControlsGroup);
				this.genericConfigElements.Add(this.nextBattleButton);
				this.genericConfigElements.Add(this.deployInstructionsLabel);
				this.genericConfigElements.Add(this.neighborGroup);
				this.genericConfigElements.Add(this.holonetButton);
				this.BaseLayoutToolView.AddHUDBaseLayoutToolElements(this.genericConfigElements);
			}
			int i = 0;
			int count = this.genericConfigElements.Count;
			while (i < count)
			{
				UXElement uXElement = this.genericConfigElements[i];
				if (uXElement != null)
				{
					if (uXElement.Root != null && config.Has(uXElement.Root.name))
					{
						uXElement.Visible = true;
						uXElement.Enabled = true;
					}
					else
					{
						uXElement.Visible = false;
					}
				}
				i++;
			}
			if (this.currencyGroup.Visible)
			{
				bool isContrabandUnlocked = this.player.IsContrabandUnlocked;
				this.contrabandView.Visible = isContrabandUnlocked;
				Vector3 a = base.GetElement<UXElement>("Materials").LocalPosition - base.GetElement<UXElement>("Credits").LocalPosition;
				this.crystalsDroidsGroup.LocalPosition = ((!isContrabandUnlocked) ? (a * 2f) : (a * 3f));
			}
			if (this.exitEditButton.Visible && this.exitEditAnimation != null)
			{
				this.exitEditAnimation.Play();
			}
			if (config.Has("LabelTimeLeft"))
			{
				int timeLeft = Service.BattleController.GetCurrentBattle().TimeLeft;
				if (timeLeft > 0)
				{
					this.timeLeftLabel.Visible = true;
					this.RefreshTimerView(timeLeft, false);
				}
				else
				{
					this.timeLeftLabel.Visible = false;
				}
			}
			else
			{
				this.timeLeftLabel.Visible = false;
			}
			if (config.Has("LabelCurrencyValueOpponent"))
			{
				this.ShowLootElements();
				this.RefreshLoot();
			}
			else
			{
				this.HideLootElements();
			}
			if (config.Has("TroopsGrid"))
			{
				this.SetupDeployableTroops();
			}
			else
			{
				this.CleanupDeployableTroops();
			}
			if (config.Has("LabelDeployInstructions"))
			{
				this.UpdateDeployInstructionLabel();
			}
			if (config.Has("ReplayControls"))
			{
				this.UpdateCurrentReplaySpeedUI();
				this.replayTimeLeftLabel.Visible = false;
			}
			if (config.Has("ButtonNextBattle"))
			{
				this.nextBattleButton.Enabled = false;
				int pvpMatchCost = Service.PvpManager.GetPvpMatchCost();
				UXUtils.SetupSingleCostElement(this, "CostNextBattle", pvpMatchCost, 0, 0, 0, 0, false, string.Empty);
			}
			if (config.Has("ButtonBattle") && !this.player.CampaignProgress.FueInProgress)
			{
				int num = 0;
				num += Service.TournamentController.NumberOfTournamentsNotViewed();
				num += Service.ObjectiveManager.GetCompletedObjectivesCount();
				this.battleJewel.Value = num;
			}
			if (Service.SquadController.StateManager.GetCurrentSquad() != null)
			{
				this.joinSquadButton.Visible = false;
				IState currentState = Service.GameStateMachine.CurrentState;
				if (currentState is HomeState || config.Has("SquadScreen"))
				{
					this.CreateSquadScreen();
				}
			}
			else
			{
				this.DestroySquadScreen();
			}
			if (this.player.CampaignProgress.FueInProgress)
			{
				this.warButton.Visible = false;
			}
			else
			{
				this.UpdateWarButton();
			}
			if (config.Has("ButtonStore"))
			{
				this.UpdateStoreJewel();
			}
			this.warAttackButton = base.GetElement<UXButton>("BtnSquadwarAttack");
			this.warAttackButton.Visible = false;
			this.warAttackLabel = base.GetElement<UXLabel>("LabelBtnSquadwarAttack");
			this.warAttackLabel.Text = this.lang.Get("WAR_START_ATTACK", new object[0]);
			this.warDoneButton = base.GetElement<UXButton>("BtnSquadwarBack");
			this.warDoneButton.Visible = false;
			this.warUplinks = base.GetElement<UXElement>("AvailableUplinks");
			this.warUplinks.Visible = false;
			Service.EventManager.UnregisterObserver(this, EventId.WarAttackCommandFailed);
			if (config.Has("WarAttack"))
			{
				if (config.Has("WarAttackOpponent"))
				{
					this.warAttackButton.Visible = true;
					this.warAttackButton.OnClicked = new UXButtonClickedDelegate(this.OnWarAttackClicked);
				}
				this.warDoneButton.Visible = true;
				this.warDoneButton.Enabled = true;
				this.warDoneButton.OnClicked = new UXButtonClickedDelegate(this.OnHomeButtonClicked);
				base.GetElement<UXLabel>("LabelBtnSquadwarBack").Text = this.lang.Get("WAR_SCOUT_CANCEL", new object[0]);
				CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
				if (currentBattle.Type == BattleType.PvpAttackSquadWar)
				{
					this.warUplinks.Visible = true;
					base.GetElement<UXLabel>("LabelAvailableUplinks").Text = this.lang.Get("WAR_SCOUT_POINTS_LEFT", new object[0]);
					this.ShowScoutingUplinksAvailable();
				}
				this.UpdateWarAttackState();
				if (this.deployableTroops != null)
				{
					this.FullyDisableDeployableControls(false);
				}
			}
			else if (config.Has("WarAttackStarted") && this.deployableTroops != null)
			{
				this.FullyEnableDeployablControls();
			}
			BuffController buffController = Service.BuffController;
			UXElement element = base.GetElement<UXElement>("PanelBuffsOpponentSquadWars");
			UXLabel element2 = base.GetElement<UXLabel>("LabelBuffsYoursSquadWars");
			UXElement element3 = base.GetElement<UXElement>("PanelBuffsYoursSquadWars");
			element3.Visible = false;
			element2.Visible = false;
			element.Visible = false;
			if (config.Has("BuffsYoursSquadWars"))
			{
				List<WarBuffVO> listOfWarBuffsBasedOnTeam = buffController.GetListOfWarBuffsBasedOnTeam(TeamType.Attacker);
				int count2 = listOfWarBuffsBasedOnTeam.Count;
				if (count2 > 0)
				{
					element3.Visible = true;
					element2.Visible = true;
					element2.Text = this.lang.Get("WAR_BATTLE_CURRENT_ADVANTAGES", new object[0]);
					UXGrid element4 = base.GetElement<UXGrid>("GridBuffsYoursSquadWars");
					element4.Clear();
					element4.SetTemplateItem("SpriteBuffsYoursSquadWars");
					for (int j = 0; j < count2; j++)
					{
						UXSprite uXSprite = (UXSprite)element4.CloneTemplateItem("SpriteBuffsYoursSquadWars" + j);
						uXSprite.SpriteName = listOfWarBuffsBasedOnTeam[j].BuffIcon;
						element4.AddItem(uXSprite, j);
					}
					element4.RepositionItems();
				}
			}
			if (config.Has("BuffsOpponentsSquadWars"))
			{
				List<WarBuffVO> listOfWarBuffsBasedOnTeam2 = buffController.GetListOfWarBuffsBasedOnTeam(TeamType.Defender);
				int count3 = listOfWarBuffsBasedOnTeam2.Count;
				if (count3 > 0)
				{
					element.Visible = true;
					UXGrid element5 = base.GetElement<UXGrid>("GridBuffsOpponentSquadWars");
					element5.Clear();
					element5.SetTemplateItem("SpriteBuffOpponentSquadWars");
					for (int k = 0; k < count3; k++)
					{
						UXSprite uXSprite2 = (UXSprite)element5.CloneTemplateItem("SpriteBuffOpponentSquadWars" + k);
						uXSprite2.SpriteName = listOfWarBuffsBasedOnTeam2[k].BuffIcon;
						element5.AddItem(uXSprite2, k);
					}
					element5.RepositionItems();
				}
			}
			if (config.Has("SpecialPromo"))
			{
				this.UpdateTargetedBundleButtonVisibility();
			}
			this.RefreshAllResourceViews(true);
			this.RefreshBaseName();
			this.RefreshPlayerSocialInformation();
			this.RefreshCurrentPlayerLevel();
			this.RefreshPlayerMedals();
			this.UpdateProtectionTimeLabel();
			if (this.persistentSquadScreen != null)
			{
				this.persistentSquadScreen.RefreshVisibility();
			}
			this.AnimateControls(true, 1f);
		}

		private void UpdateWarAttackState()
		{
			if (this.warAttackButton == null || this.warAttackLabel == null || !this.warAttackButton.Visible)
			{
				return;
			}
			SquadWarScoutState squadWarScoutState = SquadWarScoutState.Invalid;
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			if (currentBattle.Type == BattleType.PvpAttackSquadWar)
			{
				squadWarScoutState = Service.SquadController.WarManager.CanAttackCurrentlyScoutedOpponent();
			}
			else if (currentBattle.Type == BattleType.PveBuffBase)
			{
				squadWarScoutState = Service.SquadController.WarManager.CanAttackCurrentlyScoutedBuffBase();
			}
			if (squadWarScoutState != SquadWarScoutState.Invalid)
			{
				this.warAttackButton.Enabled = true;
				this.warAttackButton.Tag = squadWarScoutState;
				if (squadWarScoutState != SquadWarScoutState.AttackAvailable)
				{
					this.DisableWarAttacksUI();
				}
				else
				{
					this.warAttackButton.VisuallyEnableButton();
					this.warAttackLabel.TextColor = UXUtils.COLOR_ENABLED;
				}
			}
		}

		private void ShowScoutingUplinksAvailable()
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarParticipantState currentOpponentState = warManager.GetCurrentOpponentState();
			if (currentOpponentState == null)
			{
				Service.Logger.Warn("Could not find opponent's squad war data");
				return;
			}
			int wAR_VICTORY_POINTS = GameConstants.WAR_VICTORY_POINTS;
			int victoryPointsLeft = currentOpponentState.VictoryPointsLeft;
			if (victoryPointsLeft > wAR_VICTORY_POINTS || victoryPointsLeft < 0)
			{
				Service.Logger.Warn("Invalid number of uplinks available");
			}
			int num = wAR_VICTORY_POINTS - victoryPointsLeft;
			for (int i = 1; i <= wAR_VICTORY_POINTS; i++)
			{
				UXUtils.UpdateUplinkHelper(base.GetElement<UXSprite>("SpriteUplink" + i), i > num, false);
			}
		}

		private void OnYourBuffsButtonClicked(UXButton btn)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is BattlePlayState)
			{
				return;
			}
			BuffController buffController = Service.BuffController;
			List<WarBuffVO> listOfWarBuffsBasedOnTeam = buffController.GetListOfWarBuffsBasedOnTeam(TeamType.Attacker);
			int count = listOfWarBuffsBasedOnTeam.Count;
			if (count > 0)
			{
				Service.UXController.MiscElementsManager.ShowHudBuffToolTip(btn, listOfWarBuffsBasedOnTeam, true);
			}
		}

		private void OnOpponentBuffsButtonClicked(UXButton btn)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is BattlePlayState)
			{
				return;
			}
			BuffController buffController = Service.BuffController;
			List<WarBuffVO> listOfWarBuffsBasedOnTeam = buffController.GetListOfWarBuffsBasedOnTeam(TeamType.Defender);
			int count = listOfWarBuffsBasedOnTeam.Count;
			if (count > 0)
			{
				Service.UXController.MiscElementsManager.ShowHudBuffToolTip(btn, listOfWarBuffsBasedOnTeam, false);
			}
		}

		public override void RefreshView()
		{
			if (this.CurrentHudConfig != null)
			{
				this.ConfigureControls(this.CurrentHudConfig);
			}
		}

		private void OnWarAttackClicked(UXButton button)
		{
			SquadWarScoutState squadWarScoutState = (SquadWarScoutState)((int)button.Tag);
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			if (squadWarScoutState == SquadWarScoutState.AttackAvailable)
			{
				Service.EventManager.RegisterObserver(this, EventId.WarAttackCommandFailed);
				button.Enabled = false;
				this.warDoneButton.Enabled = false;
				if (currentBattle.Type == BattleType.PvpAttackSquadWar)
				{
					Service.SquadController.WarManager.StartAttackOnScoutedWarMember();
				}
				else if (currentBattle.Type == BattleType.PveBuffBase)
				{
					Service.SquadController.WarManager.StartAttackOnScoutedBuffBase();
				}
			}
			else
			{
				this.ShowScoutAttackFailureMessage(squadWarScoutState, currentBattle);
			}
		}

		private void ShowScoutAttackFailureMessage(SquadWarScoutState state, CurrentBattle battle)
		{
			if (state == SquadWarScoutState.Invalid)
			{
				Service.Logger.Error("Attempting to start squad war battle when in invalid scouting state. Only happens if data is bad/nonexistent");
			}
			bool flag = battle.Type == BattleType.PvpAttackSquadWar;
			if (!flag && state == SquadWarScoutState.DestinationUnavailable)
			{
				string currentlyScoutedBuffBaseId = Service.SquadController.WarManager.GetCurrentlyScoutedBuffBaseId();
				WarBuffVO warBuffVO = Service.StaticDataController.Get<WarBuffVO>(currentlyScoutedBuffBaseId);
				string planetId = warBuffVO.PlanetId;
				string planetDisplayName = LangUtils.GetPlanetDisplayName(planetId);
				AlertScreen.ShowModal(false, this.lang.Get("WAR_ATTACK_BUFF_BASE_NOT_UNLOCKED_TITLE", new object[]
				{
					planetDisplayName
				}), this.lang.Get("WAR_ATTACK_BUFF_BASE_NOT_UNLOCKED_MESSAGE", new object[]
				{
					planetDisplayName
				}), null, null);
				return;
			}
			string failureStringIdByScoutState = SquadUtils.GetFailureStringIdByScoutState(state, flag);
			string instructions = this.lang.Get(failureStringIdByScoutState, new object[0]);
			Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(instructions);
		}

		public void OnSquadWarAttackResultCallback(object result, object cookie)
		{
			if (this.warAttackButton != null)
			{
				this.warAttackButton.Enabled = true;
			}
			if (this.warDoneButton != null)
			{
				this.warDoneButton.Enabled = true;
			}
		}

		public void SetSquadScreenVisibility(bool visible)
		{
			if (this.persistentSquadScreen != null)
			{
				this.persistentSquadScreen.Visible = visible;
			}
		}

		public void SetSquadScreenAlwaysOnTop(bool onTop)
		{
			if (this.persistentSquadScreen != null)
			{
				this.persistentSquadScreen.IsAlwaysOnTop = onTop;
			}
		}

		public void SlideSquadScreenOpen()
		{
			if (this.persistentSquadScreen != null)
			{
				this.persistentSquadScreen.AnimateOpen();
			}
		}

		private void CreateSquadScreen()
		{
			if (this.persistentSquadScreen == null)
			{
				this.persistentSquadScreen = new SquadSlidingScreen();
				Service.ScreenController.AddScreen(this.persistentSquadScreen, false, true);
			}
		}

		public void PrepForSquadScreenCreate()
		{
			this.persistentSquadScreen = null;
		}

		public void DestroySquadScreen()
		{
			if (this.persistentSquadScreen != null)
			{
				bool flag = this.persistentSquadScreen.IsOpen();
				if (flag)
				{
					this.persistentSquadScreen.AnimateClosed(true, null);
				}
				else
				{
					this.persistentSquadScreen.Close(null);
					this.persistentSquadScreen = null;
				}
			}
		}

		public bool IsSquadScreenOpenAndCloseable()
		{
			bool result = false;
			if (this.persistentSquadScreen != null)
			{
				result = this.persistentSquadScreen.IsOpen();
			}
			return result;
		}

		public bool IsSquadScreenOpenOrOpeningAndCloseable()
		{
			bool result = false;
			if (this.persistentSquadScreen != null)
			{
				result = (this.persistentSquadScreen.IsOpen() || this.persistentSquadScreen.IsOpening());
			}
			return result;
		}

		public void SlideSquadScreenClosed()
		{
			if (this.persistentSquadScreen != null)
			{
				this.persistentSquadScreen.AnimateClosed(false, null);
			}
		}

		public void SlideSquadScreenClosedInstantly()
		{
			if (this.persistentSquadScreen != null)
			{
				this.persistentSquadScreen.InstantClose(false, null);
			}
		}

		private bool ShouldDestroyOrHideHomeStateUI(IState state)
		{
			return state is BattleStartState || state is BattlePlaybackState || state is EditBaseState || state is WarBaseEditorState;
		}

		private void UpdateHolonetButtonVisibility(bool visible)
		{
			if (this.holonetButton != null && this.CurrentHudConfig != null && this.CurrentHudConfig.Has("Newspaper"))
			{
				this.holonetButton.Visible = visible;
				if (visible)
				{
					this.genericConfigElements.Add(this.holonetButton);
				}
				else
				{
					this.genericConfigElements.Remove(this.holonetButton);
				}
			}
		}

		private void UpdateTargetedBundleViewTimer()
		{
			if (this.targetedBundleButton == null)
			{
				return;
			}
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			if (targetedBundleController.CurrentTargetedOffer != null)
			{
				uint serverTime = Service.ServerAPI.ServerTime;
				int num = (int)(targetedBundleController.OfferExpiresAt - serverTime);
				if (num <= 0)
				{
					if (!targetedBundleController.FetchingNewOffer)
					{
						targetedBundleController.GetNewOffer();
					}
				}
				else
				{
					UXLabel element = base.GetElement<UXLabel>("LabelSpecialPromoTimer");
					element.Text = this.lang.Get("expires_in", new object[]
					{
						LangUtils.FormatTime((long)num)
					});
				}
			}
		}

		private void UpdateTargetedBundleButtonVisibility()
		{
			if (this.targetedBundleButton != null)
			{
				TargetedBundleController targetedBundleController = Service.TargetedBundleController;
				this.targetedBundleButton.Visible = targetedBundleController.CanDisplaySPDButton();
				if (UXUtils.IsVisibleInHierarchy(this.targetedBundleButton))
				{
					UXLabel element = base.GetElement<UXLabel>("LabelSpecialPromo");
					TargetedBundleVO currentTargetedOffer = targetedBundleController.CurrentTargetedOffer;
					element.Text = this.lang.Get(currentTargetedOffer.IconString, new object[0]);
					this.UpdateTargetedBundleViewTimer();
					StaticDataController staticDataController = Service.StaticDataController;
					string iconImage = currentTargetedOffer.IconImage;
					TextureVO optional = staticDataController.GetOptional<TextureVO>(iconImage);
					if (optional != null)
					{
						UXTexture element2 = base.GetElement<UXTexture>("TextureSpecialPromo");
						element2.LoadTexture(optional.AssetName);
					}
					else
					{
						Service.Logger.Error("HUD::UpdateTargetedBundleButtonVisibility Could Not find texture vo for " + iconImage + " in offer " + currentTargetedOffer.Uid);
					}
					this.targetedBundleButtonGlowAnim.SetTrigger("ClearEffect");
					if (GameConstants.PROMO_BUTTON_RESHOW_GLOW || !this.targetedBundleGlowShown)
					{
						this.ShowPromoButtonGlowEffect();
					}
				}
			}
		}

		private void ShowPromoButtonGlowEffect()
		{
			if (GameConstants.PROMO_BUTTON_GLOW_DURATION != 0)
			{
				this.targetedBundleButtonGlowAnim.SetTrigger("ShowEffect");
				this.targetedBundleGlowShown = true;
			}
			if (GameConstants.PROMO_BUTTON_GLOW_DURATION > 0)
			{
				ViewTimerManager viewTimerManager = Service.ViewTimerManager;
				viewTimerManager.KillViewTimer(this.targetedBundleGlowTimerID);
				this.targetedBundleGlowTimerID = 0u;
				this.targetedBundleGlowTimerID = viewTimerManager.CreateViewTimer((float)GameConstants.PROMO_BUTTON_GLOW_DURATION, false, new TimerDelegate(this.StopPromoButtonGlowEffect), null);
				this.targetedBundleGlowShown = true;
			}
		}

		private void StopPromoButtonGlowEffect(uint id, object cookie)
		{
			if (this.Visible)
			{
				this.targetedBundleButtonGlowAnim.SetTrigger("StopEffect");
			}
			else
			{
				this.targetedBundleButtonGlowAnim.StopPlayback();
			}
			Service.ViewTimerManager.KillViewTimer(this.targetedBundleGlowTimerID);
			this.targetedBundleGlowTimerID = 0u;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (!base.IsLoaded())
			{
				return EatResponse.NotEaten;
			}
			switch (id)
			{
			case EventId.SquadTroopsRequestedByCurrentPlayer:
			{
				SmartEntity smartEntity = (SmartEntity)Service.BuildingController.SelectedBuilding;
				if (smartEntity != null && smartEntity.SquadBuildingComp != null)
				{
					this.ShowContextButtons(smartEntity);
				}
				goto IL_746;
			}
			case EventId.SquadWarTroopsRequestStartedByCurrentPlayer:
			case EventId.SquadWarTroopsRequestedByCurrentPlayer:
			case EventId.SquadTroopsDonatedByCurrentPlayer:
			case EventId.SquadReplaySharedByCurrentPlayer:
			case EventId.CurrentPlayerMemberDataUpdated:
			case EventId.SquadJoinInviteRemoved:
			case EventId.SquadServerMessage:
			case EventId.WarBuffBaseCaptured:
			case EventId.WarVictoryPointsUpdated:
			case EventId.WarRewardClaimed:
			{
				IL_71:
				switch (id)
				{
				case EventId.TroopLevelUpgraded:
				case EventId.StarshipLevelUpgraded:
					goto IL_50A;
				case EventId.BuildingLevelUpgraded:
				case EventId.BuildingReplaced:
					goto IL_390;
				case EventId.BuildingSwapped:
				case EventId.SpecialAttackSpawned:
					IL_9B:
					switch (id)
					{
					case EventId.InventoryUnlockUpdated:
						goto IL_390;
					case EventId.InventoryPrizeUpdated:
					case EventId.LootCollected:
						IL_C0:
						switch (id)
						{
						case EventId.MissionActionButtonClicked:
							if (cookie != null)
							{
								CampaignMissionVO campaignMissionVO = (CampaignMissionVO)cookie;
								if (campaignMissionVO.MissionType == MissionType.Defend || campaignMissionVO.MissionType == MissionType.RaidDefend)
								{
									this.DestroySquadScreen();
								}
							}
							goto IL_746;
						case EventId.PlayerNameChanged:
							this.RefreshPlayerSocialInformation();
							goto IL_746;
						case EventId.PlayerFactionChanged:
							IL_DD:
							switch (id)
							{
							case EventId.HolonetContentPrepareStarted:
								Service.EventManager.RegisterObserver(this, EventId.AllHolonetContentPrepared);
								this.UpdateHolonetButtonVisibility(false);
								goto IL_746;
							case EventId.AllHolonetContentPrepared:
								this.UpdateHolonetButtonVisibility(true);
								Service.EventManager.UnregisterObserver(this, EventId.AllHolonetContentPrepared);
								goto IL_746;
							case EventId.HolonetContentPrepared:
								this.UpdateHolonetJewel();
								goto IL_746;
							case EventId.TargetedBundleContentPrepared:
								break;
							default:
							{
								if (id != EventId.SquadTroopsReceived)
								{
									if (id == EventId.SquadTroopsDeployedByPlayer)
									{
										this.OnSquadTroopsDeployed();
										goto IL_746;
									}
									if (id != EventId.SquadUpdated)
									{
										if (id == EventId.SquadLeft)
										{
											this.joinSquadButton.Visible = true;
											goto IL_746;
										}
										if (id == EventId.BuildingPurchaseSuccess)
										{
											this.RefreshAllResourceViews(true);
											goto IL_746;
										}
										if (id == EventId.TroopDeployed)
										{
											this.OnTroopPlaced(cookie as SmartEntity);
											goto IL_746;
										}
										if (id == EventId.ChampionRepaired)
										{
											goto IL_50A;
										}
										if (id == EventId.WorldLoadComplete)
										{
											Service.EventManager.UnregisterObserver(this, EventId.HolonetContentPrepared);
											this.storeButton.Enabled = true;
											if (this.CurrentHudConfig != null)
											{
												this.ConfigureControls(this.CurrentHudConfig);
											}
											this.UpdateStoreJewel();
											this.UpdateLogJewel();
											goto IL_746;
										}
										if (id == EventId.GameStateChanged)
										{
											IState currentState = Service.GameStateMachine.CurrentState;
											if (currentState is HomeState)
											{
												HomeState homeState = currentState as HomeState;
												if (homeState.ForceReloadMap)
												{
													this.storeButton.Enabled = false;
												}
												this.UpdateHolonetJewel();
												this.UpdateStoreJewel();
												this.UpdateLogJewel();
												Service.UXController.MiscElementsManager.SetEventTickerViewVisible(this.Visible);
											}
											else if (currentState is WarBaseEditorState)
											{
												Service.UXController.MiscElementsManager.SetEventTickerViewVisible(this.Visible);
											}
											else if (this.ShouldDestroyOrHideHomeStateUI(currentState))
											{
												Service.UXController.MiscElementsManager.SetEventTickerViewVisible(false);
												this.DestroySquadScreen();
											}
											goto IL_746;
										}
										if (id == EventId.ContractStarted)
										{
											ContractEventData contractEventData = (ContractEventData)cookie;
											if (contractEventData.Contract.DeliveryType == DeliveryType.Building)
											{
												this.UpdateStoreJewel();
											}
											goto IL_746;
										}
										if (id == EventId.InventoryResourceUpdated)
										{
											this.RefreshResourceView((string)cookie, !Service.BattleController.BattleInProgress);
											this.RefreshCurrentPlayerLevel();
											goto IL_746;
										}
										if (id == EventId.HUDVisibilityChanged)
										{
											bool flag = false;
											if (Service.TargetedBundleController != null)
											{
												flag = (Service.TargetedBundleController.CurrentTargetedOffer != null);
											}
											if (flag && this.Visible && UXUtils.IsVisibleInHierarchy(this.targetedBundleButton))
											{
												this.targetedBundleButtonGlowAnim.SetTrigger("ClearEffect");
												if (GameConstants.PROMO_BUTTON_RESHOW_GLOW || !this.targetedBundleGlowShown)
												{
													this.ShowPromoButtonGlowEffect();
												}
												Service.TargetedBundleController.LogTargetedBundleBI("icon_display");
											}
											else if (UXUtils.IsVisibleInHierarchy(this.targetedBundleButton) && this.targetedBundleGlowShown)
											{
												this.StopPromoButtonGlowEffect(0u, null);
											}
											goto IL_746;
										}
										if (id == EventId.HeroDeployed)
										{
											this.OnHeroDeployed(cookie as SmartEntity);
											goto IL_746;
										}
										if (id == EventId.ChampionDeployed)
										{
											this.OnChampionDeployed(cookie as SmartEntity);
											goto IL_746;
										}
										if (id == EventId.TargetedBundleRewardRedeemed)
										{
											break;
										}
										if (id != EventId.EquipmentUpgraded)
										{
											goto IL_746;
										}
										goto IL_50A;
									}
								}
								IState currentState2 = Service.GameStateMachine.CurrentState;
								if (!(currentState2 is HomeState) && !(currentState2 is EditBaseState))
								{
									goto IL_746;
								}
								this.RefreshView();
								SmartEntity smartEntity2 = (SmartEntity)Service.BuildingController.SelectedBuilding;
								if (smartEntity2 != null && smartEntity2.SquadBuildingComp != null)
								{
									this.ShowContextButtons(smartEntity2);
								}
								BuildingLookupController buildingLookupController = Service.BuildingLookupController;
								SmartEntity smartEntity3 = (SmartEntity)buildingLookupController.GetCurrentSquadBuilding();
								if (smartEntity3 != null)
								{
									Service.BuildingTooltipController.EnsureBuildingTooltip(smartEntity3);
								}
								goto IL_746;
							}
							}
							this.UpdateTargetedBundleButtonVisibility();
							goto IL_746;
						case EventId.PvpRatingChanged:
							this.RefreshPlayerMedals();
							goto IL_746;
						}
						goto IL_DD;
					case EventId.NumInventoryItemsNotViewedUpdated:
					{
						Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
						if (selectedBuilding != null)
						{
							this.ShowContextButtons(selectedBuilding);
						}
						goto IL_746;
					}
					case EventId.LootEarnedUpdated:
						this.RefreshLoot();
						goto IL_746;
					case EventId.ScreenClosing:
						if (cookie is StoreScreen)
						{
							if (!this.player.CampaignProgress.FueInProgress)
							{
								this.UpdateStoreJewel();
							}
						}
						else if (cookie is HolonetScreen)
						{
							this.UpdateHolonetJewel();
						}
						goto IL_746;
					}
					goto IL_C0;
				case EventId.BuildingConstructed:
				{
					Entity selectedBuilding2 = Service.BuildingController.SelectedBuilding;
					ContractEventData contractEventData2 = (ContractEventData)cookie;
					if (contractEventData2.BuildingVO.Currency == CurrencyType.Contraband)
					{
						this.RefreshView();
					}
					if (selectedBuilding2 != null && selectedBuilding2 == contractEventData2.Entity)
					{
						this.ShowContextButtons(selectedBuilding2);
					}
					this.UpdateStoreJewel();
					goto IL_746;
				}
				case EventId.SpecialAttackDeployed:
					this.OnSpecialAttackDeployed((SpecialAttack)cookie);
					goto IL_746;
				}
				goto IL_9B;
				IL_390:
				this.UpdateStoreJewel();
				this.UpdateLogJewel();
				goto IL_746;
				IL_50A:
				Entity selectedBuilding3 = Service.BuildingController.SelectedBuilding;
				if (selectedBuilding3 != null && selectedBuilding3 == ((ContractEventData)cookie).Entity)
				{
					this.ShowContextButtons(selectedBuilding3);
				}
				goto IL_746;
			}
			case EventId.SquadJoinApplicationAccepted:
			{
				if (ScreenUtils.IsAnySquadScreenOpen())
				{
					Service.ScreenController.CloseAll();
				}
				string text = (string)cookie;
				if (!string.IsNullOrEmpty(text))
				{
					string instructions = Service.Lang.Get("SQUAD_ACCEPTED_MESSAGE", new object[]
					{
						text
					});
					Service.UXController.MiscElementsManager.ShowPlayerInstructions(instructions);
				}
				goto IL_746;
			}
			case EventId.SquadTroopsReceivedFromDonor:
			{
				string text2 = (string)cookie;
				if (!string.IsNullOrEmpty(text2))
				{
					string instructions2 = Service.Lang.Get("TROOPS_RECEIVED_FROM", new object[]
					{
						text2
					});
					Service.UXController.MiscElementsManager.ShowPlayerInstructions(instructions2);
				}
				goto IL_746;
			}
			case EventId.SquadJoinInviteReceived:
			case EventId.SquadJoinInvitesReceived:
				this.UpdateSquadJewelCount();
				goto IL_746;
			case EventId.WarPhaseChanged:
				this.UpdateWarButton();
				goto IL_746;
			case EventId.WarAttackPlayerStarted:
			case EventId.WarAttackPlayerCompleted:
			case EventId.WarAttackBuffBaseStarted:
			case EventId.WarAttackBuffBaseCompleted:
				this.UpdateWarAttackState();
				goto IL_746;
			case EventId.WarAttackCommandFailed:
				if (this.warAttackButton != null)
				{
					this.warAttackButton.Enabled = true;
					this.warDoneButton.Enabled = true;
					this.DisableWarAttacksUI();
				}
				Service.EventManager.UnregisterObserver(this, EventId.WarAttackCommandFailed);
				goto IL_746;
			}
			goto IL_71;
			IL_746:
			return base.OnEvent(id, cookie);
		}

		private void DisableWarAttacksUI()
		{
			this.warAttackButton.VisuallyDisableButton();
			this.warAttackLabel.TextColor = UXUtils.COLOR_LABEL_DISABLED;
			this.FullyDisableDeployableControls(false);
		}

		private void FullyDisableDeployableControls(bool hideTroopCounts)
		{
			if (this.deployableTroops == null)
			{
				return;
			}
			foreach (KeyValuePair<string, DeployableTroopControl> current in this.deployableTroops)
			{
				DeployableTroopControl value = current.Value;
				value.Disable(hideTroopCounts);
				value.TroopCheckbox.Enabled = false;
			}
		}

		private void FullyEnableDeployablControls()
		{
			if (this.deployableTroops == null)
			{
				return;
			}
			foreach (KeyValuePair<string, DeployableTroopControl> current in this.deployableTroops)
			{
				DeployableTroopControl value = current.Value;
				value.TroopCheckbox.Enabled = true;
				value.Enable();
			}
		}

		private void OnTroopPlaced(SmartEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			TroopComponent troopComp = entity.TroopComp;
			if (troopComp == null)
			{
				return;
			}
			this.UpdateTroopCount(troopComp.TroopType.Uid);
		}

		private void OnSpecialAttackDeployed(SpecialAttack specialAttack)
		{
			this.UpdateSpecialAttackCount(specialAttack.VO.Uid);
		}

		private void OnHeroDeployed(SmartEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			TroopComponent troopComp = entity.TroopComp;
			if (troopComp == null)
			{
				return;
			}
			string uid = troopComp.TroopType.Uid;
			TroopAbilityVO abilityVO = troopComp.AbilityVO;
			this.UpdateHeroCount(uid);
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			bool multipleHeroDeploysAllowed = currentBattle.MultipleHeroDeploysAllowed;
			if (this.deployableTroops != null)
			{
				foreach (KeyValuePair<string, DeployableTroopControl> current in this.deployableTroops)
				{
					DeployableTroopControl value = current.Value;
					if (value.IsHero)
					{
						if (current.Key == uid && abilityVO != null && !abilityVO.Auto)
						{
							value.PrepareHeroAbility();
						}
						else if (value.Enabled && !multipleHeroDeploysAllowed)
						{
							value.Disable();
						}
					}
				}
			}
			this.battleControlsSelectedCheckbox = null;
		}

		private void OnChampionDeployed(SmartEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			TroopComponent troopComp = entity.TroopComp;
			if (troopComp == null)
			{
				return;
			}
			this.UpdateChampionCount(troopComp.TroopType.Uid);
		}

		private void OnSquadTroopsDeployed()
		{
			this.UpdateDeployableCount("squadTroops", 0);
		}

		private void UpdateTroopCount(string uid)
		{
			this.UpdateDeployableCount(uid, Service.BattleController.GetPlayerDeployableTroopCount(uid));
		}

		private void UpdateSpecialAttackCount(string uid)
		{
			this.UpdateDeployableCount(uid, Service.BattleController.GetPlayerDeployableSpecialAttackCount(uid));
		}

		private void UpdateHeroCount(string uid)
		{
			this.UpdateDeployableCount(uid, Service.BattleController.GetPlayerDeployableHeroCount(uid));
		}

		private void UpdateChampionCount(string uid)
		{
			this.UpdateDeployableCount(uid, Service.BattleController.GetPlayerDeployableChampionCount(uid));
		}

		private void UpdateDeployableCount(string uid, int count)
		{
			if (this.deployableTroops != null && this.deployableTroops.ContainsKey(uid))
			{
				DeployableTroopControl deployableTroopControl = this.deployableTroops[uid];
				UXLabel troopCountLabel = deployableTroopControl.TroopCountLabel;
				troopCountLabel.Text = count.ToString();
				troopCountLabel.TextColor = UXUtils.GetCostColor(troopCountLabel, count != 0, false);
				if (count == 0)
				{
					deployableTroopControl.Disable();
				}
			}
		}

		public void DisableHeroDeploys()
		{
			foreach (DeployableTroopControl current in this.deployableTroops.Values)
			{
				if (current.IsHero && current.AbilityState == HeroAbilityState.Dormant)
				{
					current.DisableDueToBuildingDestruction = true;
					current.Disable();
				}
			}
		}

		public void DisableSquadDeploy()
		{
			if (this.deployableTroops.ContainsKey("squadTroops"))
			{
				this.deployableTroops["squadTroops"].DisableDueToBuildingDestruction = true;
				this.deployableTroops["squadTroops"].Disable();
			}
		}

		public void DisableSpecialAttacks()
		{
			foreach (DeployableTroopControl current in this.deployableTroops.Values)
			{
				if (current.IsStarship)
				{
					current.DisableDueToBuildingDestruction = true;
					current.Disable();
				}
			}
		}

		public void EnableNextBattleButton()
		{
			if (this.nextBattleButton.Visible)
			{
				this.nextBattleButton.Enabled = true;
			}
		}

		private void DeselectAllDeployableControlers()
		{
			foreach (DeployableTroopControl current in this.deployableTroops.Values)
			{
				current.TroopCheckbox.Selected = false;
			}
		}

		private void OnNextBattleButtonClicked(UXButton button)
		{
			this.DeselectAllDeployableControlers();
			PvpManager pvpManager = Service.PvpManager;
			if (Service.PvpManager != null)
			{
				if (GameUtils.CanAffordCredits(pvpManager.GetPvpMatchCost()))
				{
					button.Enabled = false;
					Service.EventManager.SendEvent(EventId.PvpBattleSkipped, null);
					Service.CombatTriggerManager.UnregisterAllTriggers();
					pvpManager.PurchaseNextBattle();
				}
				else
				{
					pvpManager.HandleNotEnoughCreditsForNextBattle();
				}
			}
		}

		private void OnEndBattleButtonClicked(UXButton button)
		{
			Service.EventManager.SendEvent(EventId.BattleCancelRequested, null);
		}

		private void OnDroidButtonClicked(UXButton button)
		{
			this.ShowDroidPurchaseScreen();
			Service.EventManager.SendEvent(EventId.HUDDroidButtonClicked, null);
		}

		private void OnCrystalButtonClicked(UXButton button)
		{
			StoreScreen storeScreen = new StoreScreen();
			storeScreen.OpenStoreTab(StoreTab.Crystals);
			Service.ScreenController.AddScreen(storeScreen);
			Service.EventManager.SendEvent(EventId.HUDCrystalButtonClicked, null);
		}

		private void OnShieldButtonClicked(UXButton button)
		{
			StoreScreen storeScreen = new StoreScreen();
			storeScreen.OpenStoreTab(StoreTab.Protection);
			Service.ScreenController.AddScreen(storeScreen);
			Service.EventManager.SendEvent(EventId.HUDShieldButtonClicked, null);
		}

		private void OnSpecialPromotionButtonClicked(UXButton button)
		{
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			TargetedBundleVO currentTargetedOffer = targetedBundleController.CurrentTargetedOffer;
			if (currentTargetedOffer == null)
			{
				return;
			}
			CrateVO crateVOFromTargetedOffer = targetedBundleController.GetCrateVOFromTargetedOffer(currentTargetedOffer);
			ScreenBase screen;
			if (crateVOFromTargetedOffer != null)
			{
				screen = CrateInfoModalScreen.CreateForTargetedOffer(currentTargetedOffer, crateVOFromTargetedOffer);
			}
			else
			{
				screen = new TargetedBundleScreen();
			}
			Service.ScreenController.AddScreen(screen);
			Service.EventManager.SendEvent(EventId.HUDSpecialPromotionButtonClicked, null);
			Service.TargetedBundleController.LogTargetedBundleBI("icon_tap");
		}

		public void OnViewFrameTime(float dt)
		{
			bool flag = false;
			if (this.creditsView.NeedsUpdate)
			{
				this.creditsView.Update(dt);
				flag = true;
			}
			if (this.materialsView.NeedsUpdate)
			{
				this.materialsView.Update(dt);
				flag = true;
			}
			if (this.contrabandView.NeedsUpdate)
			{
				this.contrabandView.Update(dt);
				flag = true;
			}
			if (this.crystalsView.NeedsUpdate)
			{
				this.crystalsView.Update(dt);
				flag = true;
			}
			if (!flag)
			{
				this.registeredFrameTimeObserver = false;
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public void OnViewClockTime(float dt)
		{
			this.UpdateProtectionTimeLabel();
			this.UpdateTargetedBundleViewTimer();
			if (Service.BuildingController == null)
			{
				return;
			}
			SmartEntity smartEntity = (SmartEntity)Service.BuildingController.SelectedBuilding;
			if (smartEntity != null && smartEntity.BuildingComp != null && smartEntity.BuildingComp.BuildingTO != null && !string.IsNullOrEmpty(smartEntity.BuildingComp.BuildingTO.Key))
			{
				BuildingComponent buildingComp = smartEntity.BuildingComp;
				int i = 0;
				int count = this.contextButtons.Count;
				while (i < count)
				{
					if (this.contextButtons[i] != null && this.contextButtons[i].Tag != null)
					{
						ContextButtonTag contextButtonTag = this.contextButtons[i].Tag as ContextButtonTag;
						if (contextButtonTag != null)
						{
							if (contextButtonTag.ContextId == "Finish_Now")
							{
								int num = -1;
								if (contextButtonTag.ContextId == "Finish_Now")
								{
									Contract contract = Service.ISupportController.FindCurrentContract(buildingComp.BuildingTO.Key);
									if (contract != null)
									{
										num = ContractUtils.GetCrystalCostToFinishContract(contract);
									}
								}
								else if (contextButtonTag.ContextId == "RequestTroopsPaid")
								{
									uint serverTime = Service.ServerAPI.ServerTime;
									uint troopRequestDate = Service.SquadController.StateManager.TroopRequestDate;
									num = SquadUtils.GetTroopRequestCrystalCost(serverTime, troopRequestDate);
								}
								if (num >= 0 && contextButtonTag.HardCostLabel != null)
								{
									contextButtonTag.HardCostLabel.Text = this.lang.ThousandsSeparated(num);
									contextButtonTag.HardCostLabel.TextColor = UXUtils.GetCostColor(contextButtonTag.HardCostLabel, GameUtils.CanAffordCrystals(num), false);
								}
							}
							this.UpdateContextTimerLabel(contextButtonTag);
						}
					}
					i++;
				}
			}
		}

		private void UpdateContextTimerLabel(ContextButtonTag tag)
		{
			if (tag.TimerLabel != null && tag.TimerSecondsDelegate != null)
			{
				tag.TimerLabel.Text = LangUtils.FormatTime((long)tag.TimerSecondsDelegate());
				UXUtils.ClampUILabelWidth(tag.TimerLabel, 108, 0);
			}
		}

		private void UpdateProtectionTimeLabel()
		{
			if (this.protectionLabel == null)
			{
				return;
			}
			uint protectedUntil = this.player.ProtectedUntil;
			uint serverTime = Service.ServerAPI.ServerTime;
			if (protectedUntil <= serverTime)
			{
				this.protectionLabel.Text = Service.Lang.Get("PROTECTION_NONE", new object[0]);
			}
			else
			{
				this.protectionLabel.Text = GameUtils.GetTimeLabelFromSeconds(Convert.ToInt32(protectedUntil - serverTime));
			}
		}

		private void OnHomeButtonClicked(UXButton button)
		{
			button.Enabled = false;
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is BattlePlaybackState)
			{
				Service.BattleController.EndBattleRightAway();
			}
			else
			{
				if (currentState is BattleStartState)
				{
					Service.CombatTriggerManager.UnregisterAllTriggers();
					BattleType type = Service.BattleController.GetCurrentBattle().Type;
					Service.EventManager.SendEvent(EventId.BattleLeftBeforeStarting, null);
					if (type == BattleType.Pvp)
					{
						Service.PvpManager.ReleasePvpTarget();
					}
					else if (type == BattleType.PveBuffBase)
					{
						Service.SquadController.WarManager.ReleaseCurrentlyScoutedBuffBase();
					}
					Service.ShieldController.StopAllEffects();
					if (type == BattleType.PvpAttackSquadWar || type == BattleType.PveBuffBase)
					{
						Service.SquadController.WarManager.StartTranstionFromWarBaseToWarBoard();
						return;
					}
				}
				HomeState.GoToHomeState(null, false);
			}
		}

		private void OnEditButtonClicked(UXButton button)
		{
			button.Enabled = false;
			Service.GameStateMachine.SetState(new EditBaseState(false));
		}

		private void OnStoreButtonClicked(UXButton button)
		{
			Service.ScreenController.AddScreen(new StoreScreen());
			Service.EventManager.SendEvent(EventId.HUDStoreButtonClicked, null);
		}

		private BuildingType GetBuildingType(BuildingTypeVO building, bool simpleInfoScreen)
		{
			BuildingType buildingType = building.Type;
			if (simpleInfoScreen)
			{
				BuildingType buildingType2 = buildingType;
				switch (buildingType2)
				{
				case BuildingType.Barracks:
				case BuildingType.Factory:
				case BuildingType.FleetCommand:
				case BuildingType.Squad:
				case BuildingType.Starport:
					goto IL_55;
				case BuildingType.HeroMobilizer:
				case BuildingType.ChampionPlatform:
				case BuildingType.Housing:
					IL_37:
					switch (buildingType2)
					{
					case BuildingType.Cantina:
					case BuildingType.NavigationCenter:
					case BuildingType.Armory:
						goto IL_55;
					case BuildingType.ScoutTower:
						return buildingType;
					default:
						return buildingType;
					}
					break;
				}
				goto IL_37;
				IL_55:
				buildingType = BuildingType.Invalid;
			}
			return buildingType;
		}

		private void OnDisabledContextButtonClicked(UXButton button)
		{
			if (button.Tag as string == "Trap_RearmAll")
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(this.lang.Get("traps_error_all_rearmed", new object[0]));
			}
		}

		private void OnContextButtonClicked(UXButton button)
		{
			Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
			if (selectedBuilding == null)
			{
				UXElement buttonHighlight = Service.UXController.MiscElementsManager.GetButtonHighlight();
				string text = "null";
				if (buttonHighlight != null)
				{
					text = buttonHighlight.Visible.ToString();
				}
				Service.Logger.ErrorFormat("HUD.OnContextButtonClicked: SelectedBuilding is null. button.Tag is {0}. MiscElementManager.buttonHighlight visiblity is {1}.", new object[]
				{
					button.Tag,
					text
				});
				return;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			BuildingTypeVO buildingType = buildingComponent.BuildingType;
			bool simpleInfoScreen = GameUtils.IsVisitingBase();
			BuildingType buildingType2 = this.GetBuildingType(buildingType, simpleInfoScreen);
			ScreenBase screenBase = null;
			string text2 = button.Tag as string;
			string text3 = text2;
			switch (text3)
			{
			case "Info":
				switch (buildingType2)
				{
				case BuildingType.Barracks:
				case BuildingType.Factory:
				case BuildingType.Cantina:
					screenBase = new TrainingInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.FleetCommand:
					screenBase = new StarshipInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.ChampionPlatform:
					screenBase = new ChampionInfoScreen(selectedBuilding, Service.ChampionController.FindChampionTypeIfPlatform(buildingType), false);
					goto IL_44A;
				case BuildingType.Squad:
					this.OpenSquadBuildingInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.Starport:
					screenBase = new StarportInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.Turret:
					screenBase = new TurretInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.Resource:
					screenBase = new GeneratorInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.Storage:
					screenBase = new StorageInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.ShieldGenerator:
					screenBase = new ShieldGeneratorInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.Trap:
					screenBase = new TrapInfoScreen(selectedBuilding, false);
					goto IL_44A;
				case BuildingType.NavigationCenter:
					screenBase = new NavigationCenterInfoScreen(selectedBuilding);
					goto IL_44A;
				case BuildingType.Armory:
					screenBase = new ArmoryUpgradeScreen(selectedBuilding, false);
					goto IL_44A;
				}
				screenBase = new BuildingInfoScreen(selectedBuilding);
				IL_44A:
				break;
			case "Inventory":
				screenBase = this.CreatePrizeInventoryScreen();
				break;
			case "Swap":
				screenBase = new TurretUpgradeScreen(selectedBuilding, true);
				Service.BILoggingController.TrackGameAction("UI_base", "turret_swap", buildingType.Uid + "|" + this.player.PlanetId, string.Empty, 1);
				break;
			case "Move":
				Service.EventManager.SendEvent(EventId.UserWantedEditBaseState, false);
				break;
			case "Train":
			case "Build":
			case "Hire":
				screenBase = new TroopTrainingScreen(selectedBuilding);
				break;
			case "Armory":
				screenBase = new ArmoryScreen(selectedBuilding);
				break;
			case "Commission":
				screenBase = new TroopTrainingScreen(selectedBuilding);
				break;
			case "Mobilize":
				screenBase = new TroopTrainingScreen(selectedBuilding);
				break;
			case "Repair":
				Service.ChampionController.StartChampionRepair((SmartEntity)selectedBuilding);
				break;
			case "Trap_RearmAll":
				TrapUtils.RearmAllTraps();
				break;
			case "Trap_Rearm":
				TrapUtils.RearmSingleTrap(selectedBuilding);
				break;
			case "Upgrade":
				switch (buildingType.Type)
				{
				case BuildingType.HQ:
					screenBase = new HQUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.Barracks:
				case BuildingType.Factory:
				case BuildingType.FleetCommand:
				case BuildingType.HeroMobilizer:
				case BuildingType.Cantina:
					screenBase = new TrainingUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.ChampionPlatform:
					screenBase = new ChampionInfoScreen(selectedBuilding, Service.ChampionController.FindChampionTypeIfPlatform(buildingType), true);
					goto IL_65A;
				case BuildingType.Squad:
					screenBase = new SquadUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.Starport:
					screenBase = new StarportUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.Wall:
					screenBase = new WallUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.Turret:
					screenBase = new TurretUpgradeScreen(selectedBuilding, false);
					goto IL_65A;
				case BuildingType.Resource:
					screenBase = new GeneratorUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.Storage:
					screenBase = new StorageUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.ShieldGenerator:
					screenBase = new ShieldGeneratorUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.Trap:
					screenBase = new TrapInfoScreen(selectedBuilding, true);
					goto IL_65A;
				case BuildingType.NavigationCenter:
					screenBase = new NavigationCenterUpgradeScreen(selectedBuilding);
					goto IL_65A;
				case BuildingType.Armory:
					screenBase = new ArmoryUpgradeScreen(selectedBuilding, true);
					goto IL_65A;
				}
				screenBase = new BuildingInfoScreen(selectedBuilding, true);
				IL_65A:
				break;
			case "Upgrade_Troops":
				screenBase = new TroopUpgradeScreen(selectedBuilding);
				break;
			case "RequestTroops":
			case "RequestTroopsPaid":
				Service.SquadController.ShowTroopRequestScreen(null, false);
				break;
			case "Credits":
			case "Materials":
			case "Contraband":
				Service.ICurrencyController.CollectCurrency(selectedBuilding);
				break;
			case "Cancel":
				this.CancelContractOnBuilding(selectedBuilding);
				break;
			case "Buy_Droid":
				this.ShowDroidPurchaseScreen();
				break;
			case "Finish_Now":
				this.MaybeShowFinishContractScreen(selectedBuilding);
				break;
			case "Clear":
				Service.BuildingController.StartClearingSelectedBuilding();
				break;
			case "Squad":
				this.SlideSquadScreenOpen();
				break;
			case "Join":
				this.OpenJoinSquadPanel();
				break;
			case "SelectRow":
				this.SelectWallGroup(selectedBuilding);
				break;
			case "RotateWall":
				this.RotateCurrentSelection(selectedBuilding);
				break;
			case "RaidDefend":
				Service.BILoggingController.TrackGameAction("UI_raid", "start", "context", string.Empty, 1);
				Service.RaidDefenseController.StartCurrentRaidDefense();
				break;
			case "NextRaid":
			case "RaidBriefing":
				Service.BILoggingController.TrackGameAction("UI_raid_briefing", "open", "context", string.Empty, 1);
				Service.RaidDefenseController.ShowRaidInfo();
				break;
			case "Navigate":
				Service.GalaxyViewController.GoToGalaxyView();
				Service.EventManager.SendEvent(EventId.GalaxyOpenByContextButton, null);
				break;
			case "Stash":
			{
				BuildingController buildingController = Service.BuildingController;
				Service.BuildingController.EnsureLoweredLiftedBuilding();
				List<string> list = new List<string>();
				if (buildingController.NumSelectedBuildings > 1)
				{
					List<Entity> additionalSelectedBuildings = buildingController.GetAdditionalSelectedBuildings();
					int i = 0;
					int count = additionalSelectedBuildings.Count;
					while (i < count)
					{
						Entity entity = additionalSelectedBuildings[i];
						string uid = entity.Get<BuildingComponent>().BuildingType.Uid;
						if (!list.Contains(uid))
						{
							list.Add(uid);
						}
						Service.BaseLayoutToolController.StashBuilding(additionalSelectedBuildings[i]);
						i++;
					}
				}
				Service.BaseLayoutToolController.StashBuilding(selectedBuilding);
				string uid2 = selectedBuilding.Get<BuildingComponent>().BuildingTO.Uid;
				if (!list.Contains(uid2))
				{
					list.Add(uid2);
				}
				int j = 0;
				int count2 = list.Count;
				while (j < count2)
				{
					this.BaseLayoutToolView.RefreshStashedBuildingCount(list[j]);
					j++;
				}
				Service.BuildingController.EnsureDeselectSelectedBuilding();
				break;
			}
			}
			Service.EventManager.SendEvent(EventId.ContextButtonClicked, button.Tag);
			if (screenBase != null)
			{
				Service.ScreenController.AddScreen(screenBase);
			}
		}

		public ScreenBase CreatePrizeInventoryScreen()
		{
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			sharedPlayerPrefs.SetPref("HQInvLastViewTime", ServerTime.Time.ToString());
			serverPlayerPrefs.SetPref(ServerPref.NumInventoryItemsNotViewed, "0");
			Service.ServerAPI.Sync(new SetPrefsCommand(false));
			Service.EventManager.SendEvent(EventId.HUDInventoryScreenOpened, null);
			Service.EventManager.SendEvent(EventId.NumInventoryItemsNotViewedUpdated, null);
			return new PrizeInventoryScreen();
		}

		public void InitialNavigationCenterPlanetSelect(Entity entity, BuildingTypeVO buildingTypeVO, OnScreenModalResult callback)
		{
			Service.ScreenController.AddScreen(new NavigationCenterUpgradeScreen(entity, buildingTypeVO, callback));
			Service.CurrentPlayer.SetFreeRelocation();
		}

		private void SelectWallGroup(Entity selectedBuilding)
		{
			Service.BuildingController.SelectAdjacentWalls(selectedBuilding);
			this.ShowContextButtons(selectedBuilding);
		}

		private void RotateCurrentSelection(Entity selectedBuilding)
		{
			Service.BuildingController.RotateCurrentSelection(selectedBuilding);
		}

		private void CancelContractOnBuilding(Entity selectedBuilding)
		{
			CancelScreen.ShowModal(selectedBuilding, new OnScreenModalResult(this.OnCancelModalResult), null);
		}

		private void OnCancelModalResult(object result, object cookie)
		{
			Entity entity = result as Entity;
			if (entity == null)
			{
				return;
			}
			Contract contract = Service.ISupportController.FindCurrentContract(entity.Get<BuildingComponent>().BuildingTO.Key);
			if (contract != null)
			{
				if (ContractUtils.IsTroopType(ContractUtils.GetContractType(contract.DeliveryType)))
				{
					Service.ISupportController.CancelTroopTrainContract(contract.ProductUid, entity);
				}
				else
				{
					Service.ISupportController.CancelCurrentBuildingContract(contract, entity);
				}
				this.ShowContextButtons(entity);
			}
		}

		private void ShowDroidPurchaseScreen()
		{
			if (this.player.CurrentDroidsAmount >= this.player.MaxDroidsAmount)
			{
				return;
			}
			DroidMoment droidMoment = this.droidMoment;
			this.droidMoment = new DroidMoment();
			if (droidMoment != null)
			{
				droidMoment.DestroyDroidMoment();
			}
			YesNoScreen.ShowModal(LangUtils.GetBuildingVerb(BuildingType.DroidHut), this.lang.Get("PURCHASE_DROID", new object[]
			{
				GameUtils.DroidCrystalCost(this.player.CurrentDroidsAmount)
			}), true, true, false, new OnScreenModalResult(this.OnBuyDroid), null);
		}

		private void OnBuyDroid(object result, object cookie)
		{
			bool flag = result != null;
			bool happy = false;
			if (flag)
			{
				happy = GameUtils.BuyNextDroid(false);
			}
			if (this.droidMoment != null)
			{
				this.droidMoment.HideDroidMoment(happy);
			}
			if (!flag)
			{
				Service.EventManager.SendEvent(EventId.DroidPurchaseCancelled, null);
				return;
			}
			Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
			if (selectedBuilding != null)
			{
				BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
				if (buildingComponent.BuildingType.Type == BuildingType.DroidHut)
				{
					this.ShowContextButtons(selectedBuilding);
				}
			}
		}

		private void MaybeShowFinishContractScreen(Entity selectedBuilding)
		{
			Contract contract = Service.ISupportController.FindCurrentContract(selectedBuilding.Get<BuildingComponent>().BuildingTO.Key);
			int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(contract);
			if (crystalCostToFinishContract >= GameConstants.CRYSTAL_SPEND_WARNING_MINIMUM)
			{
				FinishNowScreen.ShowModal(selectedBuilding, new OnScreenModalResult(this.FinishContractOnBuilding), null);
			}
			else
			{
				this.FinishContractOnBuilding(selectedBuilding, null);
			}
		}

		private void FinishContractOnBuilding(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			Entity entity = (Entity)result;
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			if (contract == null)
			{
				return;
			}
			int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(contract);
			if (!GameUtils.SpendCrystals(crystalCostToFinishContract))
			{
				return;
			}
			if (ContractUtils.IsTroopType(ContractUtils.GetContractType(contract.DeliveryType)))
			{
				Service.ISupportController.BuyoutAllTroopTrainContracts(entity, true);
			}
			else
			{
				Service.ISupportController.BuyOutCurrentBuildingContract(entity, true);
			}
			if (entity == Service.BuildingController.SelectedBuilding)
			{
				this.ShowContextButtons(entity);
			}
			if (buildingComponent != null)
			{
				BuildingTypeVO buildingType = buildingComponent.BuildingType;
				if (buildingType != null)
				{
					int currencyAmount = -crystalCostToFinishContract;
					string itemType = string.Empty;
					string itemId = string.Empty;
					int itemCount = 1;
					string type = string.Empty;
					string context = string.Empty;
					StaticDataController staticDataController = Service.StaticDataController;
					switch (contract.DeliveryType)
					{
					case DeliveryType.UpgradeTroop:
					{
						TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(contract.ProductUid);
						type = "speed_up_research";
						itemType = StringUtils.ToLowerCaseUnderscoreSeperated(troopTypeVO.Type.ToString());
						itemId = troopTypeVO.TroopID;
						context = troopTypeVO.Lvl.ToString();
						break;
					}
					case DeliveryType.UpgradeStarship:
					{
						SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(contract.ProductUid);
						type = "speed_up_research";
						itemType = StringUtils.ToLowerCaseUnderscoreSeperated(specialAttackTypeVO.SpecialAttackName);
						itemId = specialAttackTypeVO.SpecialAttackID;
						context = specialAttackTypeVO.Lvl.ToString();
						break;
					}
					case DeliveryType.UpgradeEquipment:
					{
						EquipmentVO equipmentVO = staticDataController.Get<EquipmentVO>(contract.ProductUid);
						type = "speed_up_research";
						itemType = StringUtils.ToLowerCaseUnderscoreSeperated(equipmentVO.GetType().ToString());
						itemId = equipmentVO.EquipmentID;
						context = equipmentVO.Lvl.ToString();
						break;
					}
					default:
						itemType = StringUtils.ToLowerCaseUnderscoreSeperated(buildingType.Type.ToString());
						itemId = buildingType.BuildingID;
						type = ((!this.player.CampaignProgress.FueInProgress) ? "speed_up_building" : "FUE_speed_up_building");
						break;
					}
					string subType = "consumable";
					Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, itemId, itemCount, type, subType, context);
				}
			}
		}

		private void OnTooltipButtonClicked(UXButton button)
		{
			Service.UXController.MiscElementsManager.ShowHudTooltip(button);
		}

		private bool AttemptToShowFactionFlipInfo()
		{
			bool result = false;
			if (GameUtils.HasUserFactionFlipped(Service.CurrentPlayer))
			{
				ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
				if (serverPlayerPrefs.GetPref(ServerPref.FactionFlippingViewed) == "1")
				{
					result = true;
					serverPlayerPrefs.SetPref(ServerPref.FactionFlippingViewed, "0");
					Service.ServerAPI.Enqueue(new SetPrefsCommand(false));
					string title = this.lang.Get(this.FACTION_FLIP_ALERT_TITLE, new object[0]);
					string message = this.lang.Get(this.FACTION_FLIP_ALERT_DESC, new object[0]);
					AlertScreen.ShowModal(false, title, message, null, null);
				}
			}
			return result;
		}

		private void OnBaseScoreButtonClicked(UXButton button)
		{
			if (GameConstants.ENABLE_FACTION_ICON_UPGRADES)
			{
				if (!this.AttemptToShowFactionFlipInfo())
				{
					Service.UXController.MiscElementsManager.ShowHudFactionIconTooltip(button);
				}
			}
			else if (GameUtils.HasUserFactionFlipped(Service.CurrentPlayer))
			{
				IState currentState = Service.GameStateMachine.CurrentState;
				if (currentState is HomeState || currentState is EditBaseState)
				{
					Service.ScreenController.AddScreen(new FactionFlipScreen());
					Service.EventManager.SendEvent(EventId.UIFactionFlipOpened, "hud");
				}
			}
			else
			{
				Service.UXController.MiscElementsManager.ShowHudTooltip(button);
			}
		}

		private void OnBattleButtonClicked(UXButton button)
		{
			this.OpenPlanetViewScreen();
		}

		private void OnWarButtonClicked(UXButton button)
		{
			this.warButton.Enabled = false;
			SquadController squadController = Service.SquadController;
			SquadWarStatusType currentStatus = squadController.WarManager.GetCurrentStatus();
			string text = null;
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			if (currentSquad != null)
			{
				text = currentSquad.SquadID;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "NULL";
			}
			if (currentStatus == SquadWarStatusType.PhasePrep && squadController.WarManager.CurrentSquadWar != null)
			{
				string warId = squadController.WarManager.CurrentSquadWar.WarId;
				Service.SharedPlayerPrefs.SetPref("WarPrepBadge", warId);
			}
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			int highestLevelHQ = buildingLookupController.GetHighestLevelHQ();
			Service.BILoggingController.TrackGameAction(highestLevelHQ.ToString(), "UI_squadwar_HUD", text + "|" + ServerTime.Time.ToString(), null);
			Service.EventManager.SendEvent(EventId.WarLaunchFlow, null);
		}

		public void OpenPlanetViewScreen()
		{
			this.OpenPlanetViewScreen(CampaignScreenSection.Main);
		}

		public void OpenPlanetViewScreen(CampaignScreenSection setSection)
		{
			Service.ScreenController.CloseAll();
			Service.GalaxyViewController.GoToPlanetView(this.player.Planet.Uid, setSection);
			Service.EventManager.SendEvent(EventId.HUDBattleButtonClicked, null);
		}

		public void OpenBattleLog()
		{
			this.HideLogJewel();
			Service.ScreenController.AddScreen(new BattleLogScreen());
		}

		public void OpenLeaderBoard()
		{
			Service.ScreenController.AddScreen(new LeaderboardsScreen(true, null));
		}

		public void OpenSquadMessageScreen()
		{
			Service.SquadController.StateManager.SquadScreenState = SquadScreenState.Chat;
			this.SlideSquadScreenOpen();
		}

		public void OpenSquadAdvancementScreen()
		{
			Service.SquadController.StateManager.SquadScreenState = SquadScreenState.Advancement;
			this.SlideSquadScreenOpen();
		}

		public void OpenConflictLeaderBoardWithPlanet(string planetId)
		{
			Service.ScreenController.AddScreen(new LeaderboardsScreen(true, planetId));
		}

		private void OnLogButtonClicked(UXButton button)
		{
			this.OpenBattleLog();
			Service.EventManager.SendEvent(EventId.HUDBattleLogButtonClicked, null);
		}

		private void OnLeaderboardButtonClicked(UXButton button)
		{
			Service.ScreenController.AddScreen(new LeaderboardsScreen(true, null));
			Service.EventManager.SendEvent(EventId.HUDLeaderboardButtonClicked, null);
		}

		private void OnHolonetButtonClicked(UXButton button)
		{
			Service.HolonetController.OpenHolonet();
			Service.EventManager.SendEvent(EventId.HUDHolonetButtonClicked, null);
		}

		private void OnSettingsButtonClicked(UXButton button)
		{
			Service.ScreenController.AddScreen(new SettingsScreen());
			Service.EventManager.SendEvent(EventId.HUDSettingsButtonClicked, null);
		}

		private void OnSquadsButtonClicked(UXButton button)
		{
			this.OpenJoinSquadPanel();
			Service.EventManager.SendEvent(EventId.HUDSquadsButtonClicked, null);
		}

		public void OpenJoinSquadPanelAfterDelay()
		{
			this.Visible = false;
			Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.OpenJoinSquadPanelCallback), null);
		}

		private void OpenJoinSquadPanelCallback(uint timerId, object cookie)
		{
			this.OpenJoinSquadPanel();
		}

		public void OpenJoinSquadPanel()
		{
			Service.ScreenController.AddScreen(new SquadJoinScreen());
		}

		private void OpenSquadBuildingInfoScreen(Entity building)
		{
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			string pref = serverPlayerPrefs.GetPref(ServerPref.SquadIntroViewed);
			bool flag = pref == "1";
			Service.ScreenController.AddScreen((Service.SquadController.StateManager.GetCurrentSquad() == null && !flag) ? new SquadIntroScreen() : new SquadBuildingScreen(building));
			if (!flag)
			{
				serverPlayerPrefs.SetPref(ServerPref.SquadIntroViewed, "1");
				SetPrefsCommand command = new SetPrefsCommand(false);
				Service.ServerAPI.Enqueue(command);
			}
		}

		public void UpdateSquadJewelCount()
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (GameConstants.SQUAD_INVITES_ENABLED && currentSquad == null && this.clansJewel != null)
			{
				List<SquadInvite> squadInvites = Service.SquadController.StateManager.SquadInvites;
				this.clansJewel.Value = ((squadInvites == null) ? 0 : squadInvites.Count);
			}
		}

		private void UpdateLogJewel()
		{
			if (this.logJewel != null && !this.logVisited)
			{
				this.logJewel.Value = Service.PvpManager.GetBattlesThatHappenOffline().Count;
				this.logVisited = true;
			}
		}

		private void HideLogJewel()
		{
			if (this.logJewel != null)
			{
				this.logJewel.Value = 0;
			}
		}

		private int GetBadgeCount()
		{
			return StoreScreen.CountUnlockedUnbuiltBuildings();
		}

		private void UpdateStoreSticker()
		{
			StickerController stickerController = Service.StickerController;
			StickerVO storeStickerToDisplay = stickerController.GetStoreStickerToDisplay(StickerType.ShopButton);
			if (storeStickerToDisplay == null)
			{
				return;
			}
			this.leiSticker.Text = this.lang.Get(storeStickerToDisplay.LabelText, new object[0]);
			this.leiSticker.Icon = storeStickerToDisplay.IconAsset;
			bool value = !string.IsNullOrEmpty(storeStickerToDisplay.IconAsset);
			this.leiSticker.SetAnimParamBool("SwapJewels", value);
			if (!string.IsNullOrEmpty(storeStickerToDisplay.LabelColor))
			{
				this.leiSticker.Color = storeStickerToDisplay.LabelColor;
			}
			if (!string.IsNullOrEmpty(storeStickerToDisplay.GradientColor))
			{
				this.leiSticker.GradientColor = storeStickerToDisplay.GradientColor;
			}
		}

		private void UpdateStoreJewel()
		{
			this.leiSticker.Value = 0;
			this.UpdateStoreSticker();
			if (this.storeJewel != null)
			{
				IState currentState = Service.GameStateMachine.CurrentState;
				if (((currentState is HomeState && !((HomeState)currentState).ForceReloadMap) || currentState is EditBaseState) && Service.BuildingLookupController.GetCurrentHQ() != null)
				{
					int badgeCount = this.GetBadgeCount();
					this.storeJewel.Value = badgeCount;
				}
			}
		}

		private void HideStoreJewel()
		{
			if (this.storeJewel != null)
			{
				this.storeJewel.Value = 0;
			}
		}

		private void UpdateHolonetJewel()
		{
			if (this.holonetJewel != null)
			{
				int value = Service.HolonetController.CalculateBadgeCount();
				this.holonetJewel.Value = value;
			}
		}

		private void UpdateWarButton()
		{
			if (this.warButton != null)
			{
				UXElement element = base.GetElement<UXElement>("WarAction");
				UXElement element2 = base.GetElement<UXElement>("WarPrep");
				UXElement element3 = base.GetElement<UXElement>("WarReward");
				UXElement element4 = base.GetElement<UXElement>("WarTutorial");
				UXLabel element5 = base.GetElement<UXLabel>("LabelWar");
				bool flag = false;
				bool flag2 = Service.BuildingLookupController != null;
				if (flag2)
				{
					flag = (Service.BuildingLookupController.GetHighestLevelHQ() >= GameConstants.WAR_PARTICIPANT_MIN_LEVEL);
				}
				int pref = Service.SharedPlayerPrefs.GetPref<int>("WarTut");
				element4.Visible = (pref < 1 && flag);
				this.warButton.Enabled = true;
				element.Visible = false;
				element2.Visible = false;
				element3.Visible = false;
				this.warJewel.Value = 0;
				this.warPrepJewel.Value = 0;
				string id = string.Empty;
				SquadWarManager warManager = Service.SquadController.WarManager;
				SquadWarStatusType currentStatus = warManager.GetCurrentStatus();
				SquadWarParticipantState currentParticipantState = warManager.GetCurrentParticipantState();
				switch (currentStatus)
				{
				case SquadWarStatusType.PhaseOpen:
					id = "WAR_BUTTON_OPEN_PHASE";
					break;
				case SquadWarStatusType.PhasePrep:
				{
					SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
					string pref2 = sharedPlayerPrefs.GetPref<string>("WarPrepBadge");
					if (pref2 == warManager.CurrentSquadWar.WarId)
					{
						this.warPrepJewel.Value = 0;
					}
					else
					{
						this.warPrepJewel.Text = this.lang.Get("WAR_EXCLAMATION", new object[0]);
					}
					element2.Visible = true;
					id = "WAR_BUTTON_PREP_PHASE";
					break;
				}
				case SquadWarStatusType.PhasePrepGrace:
					element2.Visible = true;
					id = "WAR_BUTTON_PREP_PHASE";
					break;
				case SquadWarStatusType.PhaseAction:
					this.warJewel.Value = ((currentParticipantState == null) ? 0 : currentParticipantState.TurnsLeft);
					element.Visible = true;
					id = "WAR_BUTTON_ACTION_PHASE";
					break;
				case SquadWarStatusType.PhaseActionGrace:
					element.Visible = true;
					id = "WAR_BUTTON_ACTION_PHASE";
					break;
				case SquadWarStatusType.PhaseCooldown:
					this.warPrepJewel.Text = ((warManager.GetCurrentPlayerCurrentWarReward() == null) ? string.Empty : this.lang.Get("WAR_EXCLAMATION", new object[0]));
					element3.Visible = true;
					id = "WAR_BUTTON_COOLDOWN_PHASE";
					break;
				}
				element5.Text = this.lang.Get(id, new object[0]);
			}
		}

		private void OnDisabledDeployableControlSelected(DeployableTroopControl control, string errorStringId)
		{
			if (this.battleControlsSelectedCheckbox != null && this.battleControlsSelectedCheckbox.Enabled)
			{
				this.battleControlsSelectedCheckbox.TroopCheckbox.Selected = true;
			}
			control.TroopCheckbox.Selected = false;
			Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(this.lang.Get(errorStringId, new object[0]));
		}

		private void OnTroopCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			string text = checkbox.Tag as string;
			if (!string.IsNullOrEmpty(text) && this.deployableTroops != null && this.deployableTroops.ContainsKey(text))
			{
				TroopTypeVO troopType = Service.StaticDataController.Get<TroopTypeVO>(text);
				DeployableTroopControl deployableTroopControl = this.deployableTroops[text];
				if (deployableTroopControl.Enabled)
				{
					this.SelectCheckbox(deployableTroopControl);
					Service.DeployerController.EnterTroopPlacementMode(troopType);
				}
				else
				{
					this.OnDisabledDeployableControlSelected(deployableTroopControl, "TROOP_INVALID");
				}
			}
		}

		private void OnSpecialAttackCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			string text = checkbox.Tag as string;
			if (!string.IsNullOrEmpty(text) && this.deployableTroops != null && this.deployableTroops.ContainsKey(text))
			{
				SpecialAttackTypeVO specialAttackType = Service.StaticDataController.Get<SpecialAttackTypeVO>(text);
				DeployableTroopControl deployableTroopControl = this.deployableTroops[text];
				if (deployableTroopControl.Enabled)
				{
					this.SelectCheckbox(deployableTroopControl);
					Service.DeployerController.EnterSpecialAttackPlacementMode(specialAttackType);
				}
				else if (deployableTroopControl.DisableDueToBuildingDestruction)
				{
					this.OnDisabledDeployableControlSelected(deployableTroopControl, "STARSHIP_TRAINER_DESTROYED");
				}
				else
				{
					this.OnDisabledDeployableControlSelected(deployableTroopControl, "TROOP_INVALID");
				}
			}
		}

		private void OnHeroCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			string text = checkbox.Tag as string;
			if (!string.IsNullOrEmpty(text) && this.deployableTroops != null && this.deployableTroops.ContainsKey(text))
			{
				TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(text);
				DeployableTroopControl deployableTroopControl = this.deployableTroops[text];
				if (deployableTroopControl.Enabled)
				{
					if (deployableTroopControl.AbilityState == HeroAbilityState.Prepared)
					{
						deployableTroopControl.UseHeroAbility();
						Service.TroopAbilityController.UserActivateAbility(deployableTroopControl.HeroEntityID);
					}
					else
					{
						this.SelectCheckbox(deployableTroopControl);
						Service.DeployerController.EnterHeroDeployMode(troopTypeVO);
					}
				}
				else
				{
					string errorStringId;
					if (deployableTroopControl.AbilityState == HeroAbilityState.InUse)
					{
						errorStringId = "HERO_ABILITY_ACTIVE";
					}
					else if (deployableTroopControl.AbilityState == HeroAbilityState.CoolingDown)
					{
						errorStringId = "HERO_ABILITY_COOLDOWN";
					}
					else if (deployableTroopControl.DisableDueToBuildingDestruction)
					{
						errorStringId = "HERO_TRAINER_DESTROYED";
					}
					else
					{
						int playerDeployableHeroCount = Service.BattleController.GetPlayerDeployableHeroCount(text);
						if (playerDeployableHeroCount > 0)
						{
							errorStringId = "CANNOT_DEPLOY_MULTIPLE_HEROES";
						}
						else
						{
							errorStringId = "CANNOT_DEPLOY_THIS_HERO";
						}
					}
					this.OnDisabledDeployableControlSelected(deployableTroopControl, errorStringId);
					Service.EventManager.SendEvent(EventId.HeroNotActivated, troopTypeVO);
				}
			}
		}

		private void OnChampionCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			string text = checkbox.Tag as string;
			if (!string.IsNullOrEmpty(text) && this.deployableTroops != null && this.deployableTroops.ContainsKey(text))
			{
				TroopTypeVO troopType = Service.StaticDataController.Get<TroopTypeVO>(text);
				DeployableTroopControl deployableTroopControl = this.deployableTroops[text];
				if (deployableTroopControl.Enabled)
				{
					this.SelectCheckbox(deployableTroopControl);
					Service.DeployerController.EnterChampionDeployMode(troopType);
				}
				else if (deployableTroopControl.DisableDueToBuildingDestruction)
				{
					this.OnDisabledDeployableControlSelected(deployableTroopControl, "SQUAD_CENTER_DESTROYED");
				}
				else
				{
					this.OnDisabledDeployableControlSelected(deployableTroopControl, "TROOP_INVALID");
				}
			}
		}

		private void OnSquadTroopsCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			if (this.deployableTroops != null && this.deployableTroops.ContainsKey("squadTroops"))
			{
				DeployableTroopControl deployableTroopControl = this.deployableTroops["squadTroops"];
				if (deployableTroopControl.Enabled)
				{
					this.SelectCheckbox(deployableTroopControl);
					Service.DeployerController.EnterSquadTroopPlacementMode();
				}
				else
				{
					this.OnDisabledDeployableControlSelected(deployableTroopControl, "TROOP_INVALID");
				}
			}
		}

		private void SelectCheckbox(DeployableTroopControl control)
		{
			if (control.TroopCheckbox.RadioGroup != this.battleControlsSelectedGroup && this.battleControlsSelectedCheckbox != null)
			{
				this.battleControlsSelectedCheckbox.TroopCheckbox.Selected = false;
			}
			this.battleControlsSelectedGroup = control.TroopCheckbox.RadioGroup;
			this.battleControlsSelectedCheckbox = control;
		}

		public void UpdateMedalsAvailable(int gain, int lose)
		{
			this.medalsGainLabel.Text = this.lang.ThousandsSeparated(gain);
			this.medalsLoseLabel.Text = this.lang.ThousandsSeparated(lose);
		}

		public void UpdateTournamentRatingBattleDelta(int gain, int lose, string planetId)
		{
			TournamentController tournamentController = Service.TournamentController;
			bool flag = tournamentController.IsThisTournamentLive(tournamentController.CurrentPlanetActiveTournament);
			if (flag)
			{
				this.tournamentRatingGainSprite.SpriteName = GameUtils.GetTournamentPointIconName(planetId);
				this.tournamentRatingLoseSprite.SpriteName = GameUtils.GetTournamentPointIconName(planetId);
				this.tournamentRatingGainLabel.Text = gain.ToString();
				this.tournamentRatingLoseLabel.Text = lose.ToString();
			}
			this.tournamentRatingGainGroup.Visible = flag;
			this.tournamentRatingLoseGroup.Visible = flag;
		}

		private void ShowLootElements()
		{
			this.lootGroup.Visible = true;
			this.lootContrabandIcon.Visible = this.player.IsContrabandUnlocked;
			this.lootContrabandLabel.Visible = this.player.IsContrabandUnlocked;
			LootController lootController = Service.BattleController.LootController;
			if (lootController != null)
			{
				string text = this.lang.ThousandsSeparated(lootController.GetTotalLootAvailable(CurrencyType.Credits));
				string text2 = this.lang.ThousandsSeparated(lootController.GetTotalLootAvailable(CurrencyType.Materials));
				string text3 = this.lang.ThousandsSeparated(lootController.GetTotalLootAvailable(CurrencyType.Contraband));
				this.lootCreditsLabel.Text = text;
				this.lootMaterialLabel.Text = text2;
				this.lootContrabandLabel.Text = text3;
			}
		}

		private void HideLootElements()
		{
			this.lootGroup.Visible = false;
			this.lootCreditsLabel.Text = "0";
			this.lootMaterialLabel.Text = "0";
			this.lootContrabandLabel.Text = "0";
		}

		private void RefreshLoot()
		{
			LootController lootController = Service.BattleController.LootController;
			if (lootController != null)
			{
				string text = this.lang.ThousandsSeparated((int)lootController.GetRemainingLoot(CurrencyType.Credits));
				string text2 = this.lang.ThousandsSeparated((int)lootController.GetRemainingLoot(CurrencyType.Materials));
				string text3 = this.lang.ThousandsSeparated((int)lootController.GetRemainingLoot(CurrencyType.Contraband));
				this.lootCreditsLabel.Text = text;
				this.lootMaterialLabel.Text = text2;
				this.lootContrabandLabel.Text = text3;
			}
		}

		private void OnReplaySpeedChangeButtonClicked(UXButton button)
		{
			Service.BattlePlaybackController.FastForward();
			this.UpdateCurrentReplaySpeedUI();
		}

		public void UpdateCurrentReplaySpeedUI()
		{
			int currentPlaybackScale = (int)Service.BattlePlaybackController.CurrentPlaybackScale;
			this.replaySpeedLabel.Text = this.lang.Get("replay_playback_speed", new object[]
			{
				currentPlaybackScale
			});
		}

		public void ShowReplayTimer()
		{
			this.replayTimeLeftLabel.Visible = true;
			this.RefreshReplayTimerView(Service.BattleController.GetCurrentBattle().TimeLeft);
		}

		public bool AreBattleStarsVisible()
		{
			return this.damageStarGroup.Visible;
		}
	}
}
