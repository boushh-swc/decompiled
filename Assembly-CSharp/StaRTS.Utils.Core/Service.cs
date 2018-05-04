using Midcore.Resources.ContentManagement;
using StaRTS.Assets;
using StaRTS.Audio;
using StaRTS.Externals.BI;
using StaRTS.Externals.DMOAnalytics;
using StaRTS.Externals.EnvironmentManager;
using StaRTS.Externals.IAP;
using StaRTS.Externals.Manimal;
using StaRTS.Externals.MobileConnectorAds;
using StaRTS.FX;
using StaRTS.GameBoard.Pathfinding;
using StaRTS.Main.Bot;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.CombineMesh;
using StaRTS.Main.Controllers.Entities;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Holonet;
using StaRTS.Main.Controllers.Notifications;
using StaRTS.Main.Controllers.Objectives;
using StaRTS.Main.Controllers.Performance;
using StaRTS.Main.Controllers.Planets;
using StaRTS.Main.Controllers.ShardShop;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Controllers.SquadWar;
using StaRTS.Main.Controllers.VictoryConditions;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Static;
using StaRTS.Main.RUF;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.World;
using StaRTS.Utils.Animation;
using StaRTS.Utils.Diagnostics;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Utils.Core
{
	public static class Service
	{
		private static ShieldEffects shieldEffects;

		private static ShieldController shieldController;

		private static AssetManager assetManager;

		private static AudioManager audioManager;

		private static BILoggingController bILoggingController;

		private static StepTimingController stepTimingController;

		private static ContentManager contentManager;

		private static DMOAnalyticsController dMOAnalyticsController;

		private static EnvironmentController environmentController;

		private static InAppPurchaseController inAppPurchaseController;

		private static ServerAPI serverAPI;

		private static MobileConnectorAdsController mobileConnectorAdsController;

		private static AnimationEventManager animationEventManager;

		private static CurrencyEffects currencyEffects;

		private static FXManager fXManager;

		private static MobilizationEffectsManager mobilizationEffectsManager;

		private static PlanetEffectController planetEffectController;

		private static StorageEffects storageEffects;

		private static TerrainBlendController terrainBlendController;

		private static PathingManager pathingManager;

		private static BotRunner botRunner;

		private static IAccountSyncController iAccountSyncController;

		private static AchievementController achievementController;

		private static AppServerEnvironmentController appServerEnvironmentController;

		private static ArmoryController armoryController;

		private static BaseLayoutToolController baseLayoutToolController;

		private static BattleController battleController;

		private static BattlePlaybackController battlePlaybackController;

		private static BattleRecordController battleRecordController;

		private static BoardController boardController;

		private static BuffController buffController;

		private static BuildingAnimationController buildingAnimationController;

		private static BuildingController buildingController;

		private static BuildingLookupController buildingLookupController;

		private static BuildingTooltipController buildingTooltipController;

		private static CampaignController campaignController;

		private static ChampionController championController;

		private static CombatEncounterController combatEncounterController;

		private static CombatTriggerManager combatTriggerManager;

		private static CombineMeshManager combineMeshManager;

		private static ICurrencyController iCurrencyController;

		private static DefensiveBattleController defensiveBattleController;

		private static DeployableShardUnlockController deployableShardUnlockController;

		private static DeployerController deployerController;

		private static DroidController droidController;

		private static EditBaseController editBaseController;

		private static Engine engine;

		private static EntityFactory entityFactory;

		private static EntityController entityController;

		private static EntityIdleController entityIdleController;

		private static EntityRenderController entityRenderController;

		private static EpisodeController episodeController;

		private static EpisodeTaskManager episodeTaskManager;

		private static EpisodeWidgetViewController episodeWidgetViewController;

		private static GameIdleController gameIdleController;

		private static GameStateMachine gameStateMachine;

		private static HealthController healthController;

		private static HoloController holoController;

		private static HolonetController holonetController;

		private static HomeModeController homeModeController;

		private static InventoryCrateRewardController inventoryCrateRewardController;

		private static LeaderboardController leaderboardController;

		private static LightingEffectsController lightingEffectsController;

		private static LimitedEditionItemController limitedEditionItemController;

		private static MainController mainController;

		private static NeighborVisitManager neighborVisitManager;

		private static NotificationController notificationController;

		private static SocialPushNotificationController socialPushNotificationController;

		private static ObjectiveController objectiveController;

		private static ObjectiveManager objectiveManager;

		private static PerformanceMonitor performanceMonitor;

		private static PerformanceSampler performanceSampler;

		private static PerkManager perkManager;

		private static PerkViewController perkViewController;

		private static GalaxyPlanetController galaxyPlanetController;

		private static GalaxyViewController galaxyViewController;

		private static PlanetRelocationController planetRelocationController;

		private static PlayerIdentityController playerIdentityController;

		private static PlayerValuesController playerValuesController;

		private static PopupsManager popupsManager;

		private static PostBattleRepairController postBattleRepairController;

		private static ProfanityController profanityController;

		private static ProjectileController projectileController;

		private static PromoPopupManager promoPopupManager;

		private static PvpManager pvpManager;

		private static QuestController questController;

		private static QuietCorrectionController quietCorrectionController;

		private static RaidDefenseController raidDefenseController;

		private static RewardManager rewardManager;

		private static ScreenController screenController;

		private static ScreenSizeController screenSizeController;

		private static ServerController serverController;

		private static ShardShopController shardShopController;

		private static ShooterController shooterController;

		private static ShuttleController shuttleController;

		private static SkinController skinController;

		private static ISocialDataController iSocialDataController;

		private static SpatialIndexController spatialIndexController;

		private static SpecialAttackController specialAttackController;

		private static SquadTroopAttackController squadTroopAttackController;

		private static WarBoardBuildingController warBoardBuildingController;

		private static WarBoardViewController warBoardViewController;

		private static SquadController squadController;

		private static TroopDonationTrackController troopDonationTrackController;

		private static StaticDataController staticDataController;

		private static StunController stunController;

		private static SummonController summonController;

		private static ISupportController iSupportController;

		private static TargetedBundleController targetedBundleController;

		private static TargetingController targetingController;

		private static TournamentController tournamentController;

		private static TransportController transportController;

		private static TrapController trapController;

		private static TrapViewController trapViewController;

		private static TroopAbilityController troopAbilityController;

		private static TroopAttackController troopAttackController;

		private static TroopController troopController;

		private static TurretAttackController turretAttackController;

		private static UXController uXController;

		private static UnlockController unlockController;

		private static ValueObjectController valueObjectController;

		private static VictoryConditionController victoryConditionController;

		private static WarBaseEditController warBaseEditController;

		private static HomeMapDataLoader homeMapDataLoader;

		private static NpcMapDataLoader npcMapDataLoader;

		private static PvpMapDataLoader pvpMapDataLoader;

		private static ReplayMapDataLoader replayMapDataLoader;

		private static WarBaseMapDataLoader warBaseMapDataLoader;

		private static WorldController worldController;

		private static WorldInitializer worldInitializer;

		private static WorldPreloader worldPreloader;

		private static WorldTransitioner worldTransitioner;

		private static CurrentPlayer currentPlayer;

		private static ServerPlayerPrefs serverPlayerPrefs;

		private static SharedPlayerPrefs sharedPlayerPrefs;

		private static BuildingUpgradeCatalog buildingUpgradeCatalog;

		private static EquipmentUpgradeCatalog equipmentUpgradeCatalog;

		private static StarshipUpgradeCatalog starshipUpgradeCatalog;

		private static TroopUpgradeCatalog troopUpgradeCatalog;

		private static RUFManager rUFManager;

		private static EventManager eventManager;

		private static FactionIconUpgradeController factionIconUpgradeController;

		private static CameraManager cameraManager;

		private static DerivedTransformationManager derivedTransformationManager;

		private static EntityViewManager entityViewManager;

		private static ProjectorManager projectorManager;

		private static IBackButtonManager iBackButtonManager;

		private static UserInputInhibitor userInputInhibitor;

		private static UserInputManager userInputManager;

		private static ProjectileViewManager projectileViewManager;

		private static AnimController animController;

		private static WWWManager wWWManager;

		private static Logger logger;

		private static Lang lang;

		private static Rand rand;

		private static SimTimeEngine simTimeEngine;

		private static SimTimerManager simTimerManager;

		private static ViewTimeEngine viewTimeEngine;

		private static ViewTimerManager viewTimerManager;

		private static StickerController stickerController;

		public static ShieldEffects ShieldEffects
		{
			get
			{
				return Service.shieldEffects;
			}
			set
			{
				if (Service.shieldEffects != null)
				{
					throw new Exception("An instance of the ShieldEffects service class has already been set!");
				}
				Service.shieldEffects = value;
			}
		}

		public static ShieldController ShieldController
		{
			get
			{
				return Service.shieldController;
			}
			set
			{
				if (Service.shieldController != null)
				{
					throw new Exception("An instance of the ShieldController service class has already been set!");
				}
				Service.shieldController = value;
			}
		}

		public static AssetManager AssetManager
		{
			get
			{
				return Service.assetManager;
			}
			set
			{
				if (Service.assetManager != null)
				{
					throw new Exception("An instance of the AssetManager service class has already been set!");
				}
				Service.assetManager = value;
			}
		}

		public static AudioManager AudioManager
		{
			get
			{
				return Service.audioManager;
			}
			set
			{
				if (Service.audioManager != null)
				{
					throw new Exception("An instance of the AudioManager service class has already been set!");
				}
				Service.audioManager = value;
			}
		}

		public static BILoggingController BILoggingController
		{
			get
			{
				return Service.bILoggingController;
			}
			set
			{
				if (Service.bILoggingController != null)
				{
					throw new Exception("An instance of the BILoggingController service class has already been set!");
				}
				Service.bILoggingController = value;
			}
		}

		public static StepTimingController StepTimingController
		{
			get
			{
				return Service.stepTimingController;
			}
			set
			{
				if (Service.stepTimingController != null)
				{
					throw new Exception("An instance of the StepTimingController service class has already been set!");
				}
				Service.stepTimingController = value;
			}
		}

		public static ContentManager ContentManager
		{
			get
			{
				return Service.contentManager;
			}
			set
			{
				if (Service.contentManager != null)
				{
					throw new Exception("An instance of the ContentManager service class has already been set!");
				}
				Service.contentManager = value;
			}
		}

		public static DMOAnalyticsController DMOAnalyticsController
		{
			get
			{
				return Service.dMOAnalyticsController;
			}
			set
			{
				if (Service.dMOAnalyticsController != null)
				{
					throw new Exception("An instance of the DMOAnalyticsController service class has already been set!");
				}
				Service.dMOAnalyticsController = value;
			}
		}

		public static EnvironmentController EnvironmentController
		{
			get
			{
				return Service.environmentController;
			}
			set
			{
				if (Service.environmentController != null)
				{
					throw new Exception("An instance of the EnvironmentController service class has already been set!");
				}
				Service.environmentController = value;
			}
		}

		public static InAppPurchaseController InAppPurchaseController
		{
			get
			{
				return Service.inAppPurchaseController;
			}
			set
			{
				if (Service.inAppPurchaseController != null)
				{
					throw new Exception("An instance of the InAppPurchaseController service class has already been set!");
				}
				Service.inAppPurchaseController = value;
			}
		}

		public static ServerAPI ServerAPI
		{
			get
			{
				return Service.serverAPI;
			}
			set
			{
				if (Service.serverAPI != null)
				{
					throw new Exception("An instance of the ServerAPI service class has already been set!");
				}
				Service.serverAPI = value;
			}
		}

		public static MobileConnectorAdsController MobileConnectorAdsController
		{
			get
			{
				return Service.mobileConnectorAdsController;
			}
			set
			{
				if (Service.mobileConnectorAdsController != null)
				{
					throw new Exception("An instance of the MobileConnectorAdsController service class has already been set!");
				}
				Service.mobileConnectorAdsController = value;
			}
		}

		public static AnimationEventManager AnimationEventManager
		{
			get
			{
				return Service.animationEventManager;
			}
			set
			{
				if (Service.animationEventManager != null)
				{
					throw new Exception("An instance of the AnimationEventManager service class has already been set!");
				}
				Service.animationEventManager = value;
			}
		}

		public static CurrencyEffects CurrencyEffects
		{
			get
			{
				return Service.currencyEffects;
			}
			set
			{
				if (Service.currencyEffects != null)
				{
					throw new Exception("An instance of the CurrencyEffects service class has already been set!");
				}
				Service.currencyEffects = value;
			}
		}

		public static FXManager FXManager
		{
			get
			{
				return Service.fXManager;
			}
			set
			{
				if (Service.fXManager != null)
				{
					throw new Exception("An instance of the FXManager service class has already been set!");
				}
				Service.fXManager = value;
			}
		}

		public static MobilizationEffectsManager MobilizationEffectsManager
		{
			get
			{
				return Service.mobilizationEffectsManager;
			}
			set
			{
				if (Service.mobilizationEffectsManager != null)
				{
					throw new Exception("An instance of the MobilizationEffectsManager service class has already been set!");
				}
				Service.mobilizationEffectsManager = value;
			}
		}

		public static PlanetEffectController PlanetEffectController
		{
			get
			{
				return Service.planetEffectController;
			}
			set
			{
				if (Service.planetEffectController != null)
				{
					throw new Exception("An instance of the PlanetEffectController service class has already been set!");
				}
				Service.planetEffectController = value;
			}
		}

		public static StorageEffects StorageEffects
		{
			get
			{
				return Service.storageEffects;
			}
			set
			{
				if (Service.storageEffects != null)
				{
					throw new Exception("An instance of the StorageEffects service class has already been set!");
				}
				Service.storageEffects = value;
			}
		}

		public static TerrainBlendController TerrainBlendController
		{
			get
			{
				return Service.terrainBlendController;
			}
			set
			{
				if (Service.terrainBlendController != null)
				{
					throw new Exception("An instance of the TerrainBlendController service class has already been set!");
				}
				Service.terrainBlendController = value;
			}
		}

		public static PathingManager PathingManager
		{
			get
			{
				return Service.pathingManager;
			}
			set
			{
				if (Service.pathingManager != null)
				{
					throw new Exception("An instance of the PathingManager service class has already been set!");
				}
				Service.pathingManager = value;
			}
		}

		public static BotRunner BotRunner
		{
			get
			{
				return Service.botRunner;
			}
			set
			{
				if (Service.botRunner != null)
				{
					throw new Exception("An instance of the BotRunner service class has already been set!");
				}
				Service.botRunner = value;
			}
		}

		public static IAccountSyncController IAccountSyncController
		{
			get
			{
				return Service.iAccountSyncController;
			}
			set
			{
				if (Service.iAccountSyncController != null)
				{
					throw new Exception("An instance of the IAccountSyncController service class has already been set!");
				}
				Service.iAccountSyncController = value;
			}
		}

		public static AchievementController AchievementController
		{
			get
			{
				return Service.achievementController;
			}
			set
			{
				if (Service.achievementController != null)
				{
					throw new Exception("An instance of the AchievementController service class has already been set!");
				}
				Service.achievementController = value;
			}
		}

		public static AppServerEnvironmentController AppServerEnvironmentController
		{
			get
			{
				return Service.appServerEnvironmentController;
			}
			set
			{
				if (Service.appServerEnvironmentController != null)
				{
					throw new Exception("An instance of the AppServerEnvironmentController service class has already been set!");
				}
				Service.appServerEnvironmentController = value;
			}
		}

		public static ArmoryController ArmoryController
		{
			get
			{
				return Service.armoryController;
			}
			set
			{
				if (Service.armoryController != null)
				{
					throw new Exception("An instance of the ArmoryController service class has already been set!");
				}
				Service.armoryController = value;
			}
		}

		public static BaseLayoutToolController BaseLayoutToolController
		{
			get
			{
				return Service.baseLayoutToolController;
			}
			set
			{
				if (Service.baseLayoutToolController != null)
				{
					throw new Exception("An instance of the BaseLayoutToolController service class has already been set!");
				}
				Service.baseLayoutToolController = value;
			}
		}

		public static BattleController BattleController
		{
			get
			{
				return Service.battleController;
			}
			set
			{
				if (Service.battleController != null)
				{
					throw new Exception("An instance of the BattleController service class has already been set!");
				}
				Service.battleController = value;
			}
		}

		public static BattlePlaybackController BattlePlaybackController
		{
			get
			{
				return Service.battlePlaybackController;
			}
			set
			{
				if (Service.battlePlaybackController != null)
				{
					throw new Exception("An instance of the BattlePlaybackController service class has already been set!");
				}
				Service.battlePlaybackController = value;
			}
		}

		public static BattleRecordController BattleRecordController
		{
			get
			{
				return Service.battleRecordController;
			}
			set
			{
				if (Service.battleRecordController != null)
				{
					throw new Exception("An instance of the BattleRecordController service class has already been set!");
				}
				Service.battleRecordController = value;
			}
		}

		public static BoardController BoardController
		{
			get
			{
				return Service.boardController;
			}
			set
			{
				if (Service.boardController != null)
				{
					throw new Exception("An instance of the BoardController service class has already been set!");
				}
				Service.boardController = value;
			}
		}

		public static BuffController BuffController
		{
			get
			{
				return Service.buffController;
			}
			set
			{
				if (Service.buffController != null)
				{
					throw new Exception("An instance of the BuffController service class has already been set!");
				}
				Service.buffController = value;
			}
		}

		public static BuildingAnimationController BuildingAnimationController
		{
			get
			{
				return Service.buildingAnimationController;
			}
			set
			{
				if (Service.buildingAnimationController != null)
				{
					throw new Exception("An instance of the BuildingAnimationController service class has already been set!");
				}
				Service.buildingAnimationController = value;
			}
		}

		public static BuildingController BuildingController
		{
			get
			{
				return Service.buildingController;
			}
			set
			{
				if (Service.buildingController != null)
				{
					throw new Exception("An instance of the BuildingController service class has already been set!");
				}
				Service.buildingController = value;
			}
		}

		public static BuildingLookupController BuildingLookupController
		{
			get
			{
				return Service.buildingLookupController;
			}
			set
			{
				if (Service.buildingLookupController != null)
				{
					throw new Exception("An instance of the BuildingLookupController service class has already been set!");
				}
				Service.buildingLookupController = value;
			}
		}

		public static BuildingTooltipController BuildingTooltipController
		{
			get
			{
				return Service.buildingTooltipController;
			}
			set
			{
				if (Service.buildingTooltipController != null)
				{
					throw new Exception("An instance of the BuildingTooltipController service class has already been set!");
				}
				Service.buildingTooltipController = value;
			}
		}

		public static CampaignController CampaignController
		{
			get
			{
				return Service.campaignController;
			}
			set
			{
				if (Service.campaignController != null)
				{
					throw new Exception("An instance of the CampaignController service class has already been set!");
				}
				Service.campaignController = value;
			}
		}

		public static ChampionController ChampionController
		{
			get
			{
				return Service.championController;
			}
			set
			{
				if (Service.championController != null)
				{
					throw new Exception("An instance of the ChampionController service class has already been set!");
				}
				Service.championController = value;
			}
		}

		public static CombatEncounterController CombatEncounterController
		{
			get
			{
				return Service.combatEncounterController;
			}
			set
			{
				if (Service.combatEncounterController != null)
				{
					throw new Exception("An instance of the CombatEncounterController service class has already been set!");
				}
				Service.combatEncounterController = value;
			}
		}

		public static CombatTriggerManager CombatTriggerManager
		{
			get
			{
				return Service.combatTriggerManager;
			}
			set
			{
				if (Service.combatTriggerManager != null)
				{
					throw new Exception("An instance of the CombatTriggerManager service class has already been set!");
				}
				Service.combatTriggerManager = value;
			}
		}

		public static CombineMeshManager CombineMeshManager
		{
			get
			{
				return Service.combineMeshManager;
			}
			set
			{
				if (Service.combineMeshManager != null)
				{
					throw new Exception("An instance of the CombineMeshManager service class has already been set!");
				}
				Service.combineMeshManager = value;
			}
		}

		public static ICurrencyController ICurrencyController
		{
			get
			{
				return Service.iCurrencyController;
			}
			set
			{
				if (Service.iCurrencyController != null)
				{
					throw new Exception("An instance of the ICurrencyController service class has already been set!");
				}
				Service.iCurrencyController = value;
			}
		}

		public static DefensiveBattleController DefensiveBattleController
		{
			get
			{
				return Service.defensiveBattleController;
			}
			set
			{
				if (Service.defensiveBattleController != null)
				{
					throw new Exception("An instance of the DefensiveBattleController service class has already been set!");
				}
				Service.defensiveBattleController = value;
			}
		}

		public static DeployableShardUnlockController DeployableShardUnlockController
		{
			get
			{
				return Service.deployableShardUnlockController;
			}
			set
			{
				if (Service.deployableShardUnlockController != null)
				{
					throw new Exception("An instance of the DeployableShardUnlockController service class has already been set!");
				}
				Service.deployableShardUnlockController = value;
			}
		}

		public static DeployerController DeployerController
		{
			get
			{
				return Service.deployerController;
			}
			set
			{
				if (Service.deployerController != null)
				{
					throw new Exception("An instance of the DeployerController service class has already been set!");
				}
				Service.deployerController = value;
			}
		}

		public static DroidController DroidController
		{
			get
			{
				return Service.droidController;
			}
			set
			{
				if (Service.droidController != null)
				{
					throw new Exception("An instance of the DroidController service class has already been set!");
				}
				Service.droidController = value;
			}
		}

		public static EditBaseController EditBaseController
		{
			get
			{
				return Service.editBaseController;
			}
			set
			{
				if (Service.editBaseController != null)
				{
					throw new Exception("An instance of the EditBaseController service class has already been set!");
				}
				Service.editBaseController = value;
			}
		}

		public static Engine Engine
		{
			get
			{
				return Service.engine;
			}
			set
			{
				if (Service.engine != null)
				{
					throw new Exception("An instance of the Engine service class has already been set!");
				}
				Service.engine = value;
			}
		}

		public static EntityFactory EntityFactory
		{
			get
			{
				return Service.entityFactory;
			}
			set
			{
				if (Service.entityFactory != null)
				{
					throw new Exception("An instance of the EntityFactory service class has already been set!");
				}
				Service.entityFactory = value;
			}
		}

		public static EntityController EntityController
		{
			get
			{
				return Service.entityController;
			}
			set
			{
				if (Service.entityController != null)
				{
					throw new Exception("An instance of the EntityController service class has already been set!");
				}
				Service.entityController = value;
			}
		}

		public static EntityIdleController EntityIdleController
		{
			get
			{
				return Service.entityIdleController;
			}
			set
			{
				if (Service.entityIdleController != null)
				{
					throw new Exception("An instance of the EntityIdleController service class has already been set!");
				}
				Service.entityIdleController = value;
			}
		}

		public static EntityRenderController EntityRenderController
		{
			get
			{
				return Service.entityRenderController;
			}
			set
			{
				if (Service.entityRenderController != null)
				{
					throw new Exception("An instance of the EntityRenderController service class has already been set!");
				}
				Service.entityRenderController = value;
			}
		}

		public static EpisodeController EpisodeController
		{
			get
			{
				return Service.episodeController;
			}
			set
			{
				if (Service.episodeController != null)
				{
					throw new Exception("An instance of the EpisodeController service class has already been set!");
				}
				Service.episodeController = value;
			}
		}

		public static EpisodeTaskManager EpisodeTaskManager
		{
			get
			{
				return Service.episodeTaskManager;
			}
			set
			{
				if (Service.episodeTaskManager != null)
				{
					throw new Exception("An instance of the EpisodeTaskManager service class has already been set!");
				}
				Service.episodeTaskManager = value;
			}
		}

		public static EpisodeWidgetViewController EpisodeWidgetViewController
		{
			get
			{
				return Service.episodeWidgetViewController;
			}
			set
			{
				if (Service.episodeWidgetViewController != null)
				{
					throw new Exception("An instance of the EpisodeWidgetViewController service class has already been set!");
				}
				Service.episodeWidgetViewController = value;
			}
		}

		public static GameIdleController GameIdleController
		{
			get
			{
				return Service.gameIdleController;
			}
			set
			{
				if (Service.gameIdleController != null)
				{
					throw new Exception("An instance of the GameIdleController service class has already been set!");
				}
				Service.gameIdleController = value;
			}
		}

		public static GameStateMachine GameStateMachine
		{
			get
			{
				return Service.gameStateMachine;
			}
			set
			{
				if (Service.gameStateMachine != null)
				{
					throw new Exception("An instance of the GameStateMachine service class has already been set!");
				}
				Service.gameStateMachine = value;
			}
		}

		public static HealthController HealthController
		{
			get
			{
				return Service.healthController;
			}
			set
			{
				if (Service.healthController != null)
				{
					throw new Exception("An instance of the HealthController service class has already been set!");
				}
				Service.healthController = value;
			}
		}

		public static HoloController HoloController
		{
			get
			{
				return Service.holoController;
			}
			set
			{
				if (Service.holoController != null)
				{
					throw new Exception("An instance of the HoloController service class has already been set!");
				}
				Service.holoController = value;
			}
		}

		public static HolonetController HolonetController
		{
			get
			{
				return Service.holonetController;
			}
			set
			{
				if (Service.holonetController != null)
				{
					throw new Exception("An instance of the HolonetController service class has already been set!");
				}
				Service.holonetController = value;
			}
		}

		public static HomeModeController HomeModeController
		{
			get
			{
				return Service.homeModeController;
			}
			set
			{
				if (Service.homeModeController != null)
				{
					throw new Exception("An instance of the HomeModeController service class has already been set!");
				}
				Service.homeModeController = value;
			}
		}

		public static InventoryCrateRewardController InventoryCrateRewardController
		{
			get
			{
				return Service.inventoryCrateRewardController;
			}
			set
			{
				if (Service.inventoryCrateRewardController != null)
				{
					throw new Exception("An instance of the InventoryCrateRewardController service class has already been set!");
				}
				Service.inventoryCrateRewardController = value;
			}
		}

		public static LeaderboardController LeaderboardController
		{
			get
			{
				return Service.leaderboardController;
			}
			set
			{
				if (Service.leaderboardController != null)
				{
					throw new Exception("An instance of the LeaderboardController service class has already been set!");
				}
				Service.leaderboardController = value;
			}
		}

		public static LightingEffectsController LightingEffectsController
		{
			get
			{
				return Service.lightingEffectsController;
			}
			set
			{
				if (Service.lightingEffectsController != null)
				{
					throw new Exception("An instance of the LightingEffectsController service class has already been set!");
				}
				Service.lightingEffectsController = value;
			}
		}

		public static LimitedEditionItemController LimitedEditionItemController
		{
			get
			{
				return Service.limitedEditionItemController;
			}
			set
			{
				if (Service.limitedEditionItemController != null)
				{
					throw new Exception("An instance of the LimitedEditionItemController service class has already been set!");
				}
				Service.limitedEditionItemController = value;
			}
		}

		public static MainController MainController
		{
			get
			{
				return Service.mainController;
			}
			set
			{
				if (Service.mainController != null)
				{
					throw new Exception("An instance of the MainController service class has already been set!");
				}
				Service.mainController = value;
			}
		}

		public static NeighborVisitManager NeighborVisitManager
		{
			get
			{
				return Service.neighborVisitManager;
			}
			set
			{
				if (Service.neighborVisitManager != null)
				{
					throw new Exception("An instance of the NeighborVisitManager service class has already been set!");
				}
				Service.neighborVisitManager = value;
			}
		}

		public static NotificationController NotificationController
		{
			get
			{
				return Service.notificationController;
			}
			set
			{
				if (Service.notificationController != null)
				{
					throw new Exception("An instance of the NotificationController service class has already been set!");
				}
				Service.notificationController = value;
			}
		}

		public static SocialPushNotificationController SocialPushNotificationController
		{
			get
			{
				return Service.socialPushNotificationController;
			}
			set
			{
				if (Service.socialPushNotificationController != null)
				{
					throw new Exception("An instance of the SocialPushNotificationController service class has already been set!");
				}
				Service.socialPushNotificationController = value;
			}
		}

		public static ObjectiveController ObjectiveController
		{
			get
			{
				return Service.objectiveController;
			}
			set
			{
				if (Service.objectiveController != null)
				{
					throw new Exception("An instance of the ObjectiveController service class has already been set!");
				}
				Service.objectiveController = value;
			}
		}

		public static ObjectiveManager ObjectiveManager
		{
			get
			{
				return Service.objectiveManager;
			}
			set
			{
				if (Service.objectiveManager != null)
				{
					throw new Exception("An instance of the ObjectiveManager service class has already been set!");
				}
				Service.objectiveManager = value;
			}
		}

		public static PerformanceMonitor PerformanceMonitor
		{
			get
			{
				return Service.performanceMonitor;
			}
			set
			{
				if (Service.performanceMonitor != null)
				{
					throw new Exception("An instance of the PerformanceMonitor service class has already been set!");
				}
				Service.performanceMonitor = value;
			}
		}

		public static PerformanceSampler PerformanceSampler
		{
			get
			{
				return Service.performanceSampler;
			}
			set
			{
				if (Service.performanceSampler != null)
				{
					throw new Exception("An instance of the PerformanceSampler service class has already been set!");
				}
				Service.performanceSampler = value;
			}
		}

		public static PerkManager PerkManager
		{
			get
			{
				return Service.perkManager;
			}
			set
			{
				if (Service.perkManager != null)
				{
					throw new Exception("An instance of the PerkManager service class has already been set!");
				}
				Service.perkManager = value;
			}
		}

		public static PerkViewController PerkViewController
		{
			get
			{
				return Service.perkViewController;
			}
			set
			{
				if (Service.perkViewController != null)
				{
					throw new Exception("An instance of the PerkViewController service class has already been set!");
				}
				Service.perkViewController = value;
			}
		}

		public static GalaxyPlanetController GalaxyPlanetController
		{
			get
			{
				return Service.galaxyPlanetController;
			}
			set
			{
				if (Service.galaxyPlanetController != null)
				{
					throw new Exception("An instance of the GalaxyPlanetController service class has already been set!");
				}
				Service.galaxyPlanetController = value;
			}
		}

		public static GalaxyViewController GalaxyViewController
		{
			get
			{
				return Service.galaxyViewController;
			}
			set
			{
				if (Service.galaxyViewController != null)
				{
					throw new Exception("An instance of the GalaxyViewController service class has already been set!");
				}
				Service.galaxyViewController = value;
			}
		}

		public static PlanetRelocationController PlanetRelocationController
		{
			get
			{
				return Service.planetRelocationController;
			}
			set
			{
				if (Service.planetRelocationController != null)
				{
					throw new Exception("An instance of the PlanetRelocationController service class has already been set!");
				}
				Service.planetRelocationController = value;
			}
		}

		public static PlayerIdentityController PlayerIdentityController
		{
			get
			{
				return Service.playerIdentityController;
			}
			set
			{
				if (Service.playerIdentityController != null)
				{
					throw new Exception("An instance of the PlayerIdentityController service class has already been set!");
				}
				Service.playerIdentityController = value;
			}
		}

		public static PlayerValuesController PlayerValuesController
		{
			get
			{
				return Service.playerValuesController;
			}
			set
			{
				if (Service.playerValuesController != null)
				{
					throw new Exception("An instance of the PlayerValuesController service class has already been set!");
				}
				Service.playerValuesController = value;
			}
		}

		public static PopupsManager PopupsManager
		{
			get
			{
				return Service.popupsManager;
			}
			set
			{
				if (Service.popupsManager != null)
				{
					throw new Exception("An instance of the PopupsManager service class has already been set!");
				}
				Service.popupsManager = value;
			}
		}

		public static PostBattleRepairController PostBattleRepairController
		{
			get
			{
				return Service.postBattleRepairController;
			}
			set
			{
				if (Service.postBattleRepairController != null)
				{
					throw new Exception("An instance of the PostBattleRepairController service class has already been set!");
				}
				Service.postBattleRepairController = value;
			}
		}

		public static ProfanityController ProfanityController
		{
			get
			{
				return Service.profanityController;
			}
			set
			{
				if (Service.profanityController != null)
				{
					throw new Exception("An instance of the ProfanityController service class has already been set!");
				}
				Service.profanityController = value;
			}
		}

		public static ProjectileController ProjectileController
		{
			get
			{
				return Service.projectileController;
			}
			set
			{
				if (Service.projectileController != null)
				{
					throw new Exception("An instance of the ProjectileController service class has already been set!");
				}
				Service.projectileController = value;
			}
		}

		public static PromoPopupManager PromoPopupManager
		{
			get
			{
				return Service.promoPopupManager;
			}
			set
			{
				if (Service.promoPopupManager != null)
				{
					throw new Exception("An instance of the PromoPopupManager service class has already been set!");
				}
				Service.promoPopupManager = value;
			}
		}

		public static PvpManager PvpManager
		{
			get
			{
				return Service.pvpManager;
			}
			set
			{
				if (Service.pvpManager != null)
				{
					throw new Exception("An instance of the PvpManager service class has already been set!");
				}
				Service.pvpManager = value;
			}
		}

		public static QuestController QuestController
		{
			get
			{
				return Service.questController;
			}
			set
			{
				if (Service.questController != null)
				{
					throw new Exception("An instance of the QuestController service class has already been set!");
				}
				Service.questController = value;
			}
		}

		public static QuietCorrectionController QuietCorrectionController
		{
			get
			{
				return Service.quietCorrectionController;
			}
			set
			{
				if (Service.quietCorrectionController != null)
				{
					throw new Exception("An instance of the QuietCorrectionController service class has already been set!");
				}
				Service.quietCorrectionController = value;
			}
		}

		public static RaidDefenseController RaidDefenseController
		{
			get
			{
				return Service.raidDefenseController;
			}
			set
			{
				if (Service.raidDefenseController != null)
				{
					throw new Exception("An instance of the RaidDefenseController service class has already been set!");
				}
				Service.raidDefenseController = value;
			}
		}

		public static RewardManager RewardManager
		{
			get
			{
				return Service.rewardManager;
			}
			set
			{
				if (Service.rewardManager != null)
				{
					throw new Exception("An instance of the RewardManager service class has already been set!");
				}
				Service.rewardManager = value;
			}
		}

		public static ScreenController ScreenController
		{
			get
			{
				return Service.screenController;
			}
			set
			{
				if (Service.screenController != null)
				{
					throw new Exception("An instance of the ScreenController service class has already been set!");
				}
				Service.screenController = value;
			}
		}

		public static ScreenSizeController ScreenSizeController
		{
			get
			{
				return Service.screenSizeController;
			}
			set
			{
				if (Service.screenSizeController != null)
				{
					throw new Exception("An instance of the ScreenSizeController service class has already been set!");
				}
				Service.screenSizeController = value;
			}
		}

		public static ServerController ServerController
		{
			get
			{
				return Service.serverController;
			}
			set
			{
				if (Service.serverController != null)
				{
					throw new Exception("An instance of the ServerController service class has already been set!");
				}
				Service.serverController = value;
			}
		}

		public static ShardShopController ShardShopController
		{
			get
			{
				return Service.shardShopController;
			}
			set
			{
				if (Service.shardShopController != null)
				{
					throw new Exception("An instance of the ShardShopController service class has already been set!");
				}
				Service.shardShopController = value;
			}
		}

		public static ShooterController ShooterController
		{
			get
			{
				return Service.shooterController;
			}
			set
			{
				if (Service.shooterController != null)
				{
					throw new Exception("An instance of the ShooterController service class has already been set!");
				}
				Service.shooterController = value;
			}
		}

		public static ShuttleController ShuttleController
		{
			get
			{
				return Service.shuttleController;
			}
			set
			{
				if (Service.shuttleController != null)
				{
					throw new Exception("An instance of the ShuttleController service class has already been set!");
				}
				Service.shuttleController = value;
			}
		}

		public static SkinController SkinController
		{
			get
			{
				return Service.skinController;
			}
			set
			{
				if (Service.skinController != null)
				{
					throw new Exception("An instance of the SkinController service class has already been set!");
				}
				Service.skinController = value;
			}
		}

		public static ISocialDataController ISocialDataController
		{
			get
			{
				return Service.iSocialDataController;
			}
			set
			{
				if (Service.iSocialDataController != null)
				{
					throw new Exception("An instance of the ISocialDataController service class has already been set!");
				}
				Service.iSocialDataController = value;
			}
		}

		public static SpatialIndexController SpatialIndexController
		{
			get
			{
				return Service.spatialIndexController;
			}
			set
			{
				if (Service.spatialIndexController != null)
				{
					throw new Exception("An instance of the SpatialIndexController service class has already been set!");
				}
				Service.spatialIndexController = value;
			}
		}

		public static SpecialAttackController SpecialAttackController
		{
			get
			{
				return Service.specialAttackController;
			}
			set
			{
				if (Service.specialAttackController != null)
				{
					throw new Exception("An instance of the SpecialAttackController service class has already been set!");
				}
				Service.specialAttackController = value;
			}
		}

		public static SquadTroopAttackController SquadTroopAttackController
		{
			get
			{
				return Service.squadTroopAttackController;
			}
			set
			{
				if (Service.squadTroopAttackController != null)
				{
					throw new Exception("An instance of the SquadTroopAttackController service class has already been set!");
				}
				Service.squadTroopAttackController = value;
			}
		}

		public static WarBoardBuildingController WarBoardBuildingController
		{
			get
			{
				return Service.warBoardBuildingController;
			}
			set
			{
				if (Service.warBoardBuildingController != null)
				{
					throw new Exception("An instance of the WarBoardBuildingController service class has already been set!");
				}
				Service.warBoardBuildingController = value;
			}
		}

		public static WarBoardViewController WarBoardViewController
		{
			get
			{
				return Service.warBoardViewController;
			}
			set
			{
				if (Service.warBoardViewController != null)
				{
					throw new Exception("An instance of the WarBoardViewController service class has already been set!");
				}
				Service.warBoardViewController = value;
			}
		}

		public static SquadController SquadController
		{
			get
			{
				return Service.squadController;
			}
			set
			{
				if (Service.squadController != null)
				{
					throw new Exception("An instance of the SquadController service class has already been set!");
				}
				Service.squadController = value;
			}
		}

		public static TroopDonationTrackController TroopDonationTrackController
		{
			get
			{
				return Service.troopDonationTrackController;
			}
			set
			{
				if (Service.troopDonationTrackController != null)
				{
					throw new Exception("An instance of the TroopDonationTrackController service class has already been set!");
				}
				Service.troopDonationTrackController = value;
			}
		}

		public static StaticDataController StaticDataController
		{
			get
			{
				return Service.staticDataController;
			}
			set
			{
				if (Service.staticDataController != null)
				{
					throw new Exception("An instance of the StaticDataController service class has already been set!");
				}
				Service.staticDataController = value;
			}
		}

		public static StunController StunController
		{
			get
			{
				return Service.stunController;
			}
			set
			{
				if (Service.stunController != null)
				{
					throw new Exception("An instance of the StunController service class has already been set!");
				}
				Service.stunController = value;
			}
		}

		public static SummonController SummonController
		{
			get
			{
				return Service.summonController;
			}
			set
			{
				if (Service.summonController != null)
				{
					throw new Exception("An instance of the SummonController service class has already been set!");
				}
				Service.summonController = value;
			}
		}

		public static ISupportController ISupportController
		{
			get
			{
				return Service.iSupportController;
			}
			set
			{
				if (Service.iSupportController != null)
				{
					throw new Exception("An instance of the ISupportController service class has already been set!");
				}
				Service.iSupportController = value;
			}
		}

		public static TargetedBundleController TargetedBundleController
		{
			get
			{
				return Service.targetedBundleController;
			}
			set
			{
				if (Service.targetedBundleController != null)
				{
					throw new Exception("An instance of the TargetedBundleController service class has already been set!");
				}
				Service.targetedBundleController = value;
			}
		}

		public static TargetingController TargetingController
		{
			get
			{
				return Service.targetingController;
			}
			set
			{
				if (Service.targetingController != null)
				{
					throw new Exception("An instance of the TargetingController service class has already been set!");
				}
				Service.targetingController = value;
			}
		}

		public static TournamentController TournamentController
		{
			get
			{
				return Service.tournamentController;
			}
			set
			{
				if (Service.tournamentController != null)
				{
					throw new Exception("An instance of the TournamentController service class has already been set!");
				}
				Service.tournamentController = value;
			}
		}

		public static TransportController TransportController
		{
			get
			{
				return Service.transportController;
			}
			set
			{
				if (Service.transportController != null)
				{
					throw new Exception("An instance of the TransportController service class has already been set!");
				}
				Service.transportController = value;
			}
		}

		public static TrapController TrapController
		{
			get
			{
				return Service.trapController;
			}
			set
			{
				if (Service.trapController != null)
				{
					throw new Exception("An instance of the TrapController service class has already been set!");
				}
				Service.trapController = value;
			}
		}

		public static TrapViewController TrapViewController
		{
			get
			{
				return Service.trapViewController;
			}
			set
			{
				if (Service.trapViewController != null)
				{
					throw new Exception("An instance of the TrapViewController service class has already been set!");
				}
				Service.trapViewController = value;
			}
		}

		public static TroopAbilityController TroopAbilityController
		{
			get
			{
				return Service.troopAbilityController;
			}
			set
			{
				if (Service.troopAbilityController != null)
				{
					throw new Exception("An instance of the TroopAbilityController service class has already been set!");
				}
				Service.troopAbilityController = value;
			}
		}

		public static TroopAttackController TroopAttackController
		{
			get
			{
				return Service.troopAttackController;
			}
			set
			{
				if (Service.troopAttackController != null)
				{
					throw new Exception("An instance of the TroopAttackController service class has already been set!");
				}
				Service.troopAttackController = value;
			}
		}

		public static TroopController TroopController
		{
			get
			{
				return Service.troopController;
			}
			set
			{
				if (Service.troopController != null)
				{
					throw new Exception("An instance of the TroopController service class has already been set!");
				}
				Service.troopController = value;
			}
		}

		public static TurretAttackController TurretAttackController
		{
			get
			{
				return Service.turretAttackController;
			}
			set
			{
				if (Service.turretAttackController != null)
				{
					throw new Exception("An instance of the TurretAttackController service class has already been set!");
				}
				Service.turretAttackController = value;
			}
		}

		public static UXController UXController
		{
			get
			{
				return Service.uXController;
			}
			set
			{
				if (Service.uXController != null)
				{
					throw new Exception("An instance of the UXController service class has already been set!");
				}
				Service.uXController = value;
			}
		}

		public static UnlockController UnlockController
		{
			get
			{
				return Service.unlockController;
			}
			set
			{
				if (Service.unlockController != null)
				{
					throw new Exception("An instance of the UnlockController service class has already been set!");
				}
				Service.unlockController = value;
			}
		}

		public static ValueObjectController ValueObjectController
		{
			get
			{
				return Service.valueObjectController;
			}
			set
			{
				if (Service.valueObjectController != null)
				{
					throw new Exception("An instance of the ValueObjectController service class has already been set!");
				}
				Service.valueObjectController = value;
			}
		}

		public static VictoryConditionController VictoryConditionController
		{
			get
			{
				return Service.victoryConditionController;
			}
			set
			{
				if (Service.victoryConditionController != null)
				{
					throw new Exception("An instance of the VictoryConditionController service class has already been set!");
				}
				Service.victoryConditionController = value;
			}
		}

		public static WarBaseEditController WarBaseEditController
		{
			get
			{
				return Service.warBaseEditController;
			}
			set
			{
				if (Service.warBaseEditController != null)
				{
					throw new Exception("An instance of the WarBaseEditController service class has already been set!");
				}
				Service.warBaseEditController = value;
			}
		}

		public static HomeMapDataLoader HomeMapDataLoader
		{
			get
			{
				return Service.homeMapDataLoader;
			}
			set
			{
				if (Service.homeMapDataLoader != null)
				{
					throw new Exception("An instance of the HomeMapDataLoader service class has already been set!");
				}
				Service.homeMapDataLoader = value;
			}
		}

		public static NpcMapDataLoader NpcMapDataLoader
		{
			get
			{
				return Service.npcMapDataLoader;
			}
			set
			{
				if (Service.npcMapDataLoader != null)
				{
					throw new Exception("An instance of the NpcMapDataLoader service class has already been set!");
				}
				Service.npcMapDataLoader = value;
			}
		}

		public static PvpMapDataLoader PvpMapDataLoader
		{
			get
			{
				return Service.pvpMapDataLoader;
			}
			set
			{
				if (Service.pvpMapDataLoader != null)
				{
					throw new Exception("An instance of the PvpMapDataLoader service class has already been set!");
				}
				Service.pvpMapDataLoader = value;
			}
		}

		public static ReplayMapDataLoader ReplayMapDataLoader
		{
			get
			{
				return Service.replayMapDataLoader;
			}
			set
			{
				if (Service.replayMapDataLoader != null)
				{
					throw new Exception("An instance of the ReplayMapDataLoader service class has already been set!");
				}
				Service.replayMapDataLoader = value;
			}
		}

		public static WarBaseMapDataLoader WarBaseMapDataLoader
		{
			get
			{
				return Service.warBaseMapDataLoader;
			}
			set
			{
				if (Service.warBaseMapDataLoader != null)
				{
					throw new Exception("An instance of the WarBaseMapDataLoader service class has already been set!");
				}
				Service.warBaseMapDataLoader = value;
			}
		}

		public static WorldController WorldController
		{
			get
			{
				return Service.worldController;
			}
			set
			{
				if (Service.worldController != null)
				{
					throw new Exception("An instance of the WorldController service class has already been set!");
				}
				Service.worldController = value;
			}
		}

		public static WorldInitializer WorldInitializer
		{
			get
			{
				return Service.worldInitializer;
			}
			set
			{
				if (Service.worldInitializer != null)
				{
					throw new Exception("An instance of the WorldInitializer service class has already been set!");
				}
				Service.worldInitializer = value;
			}
		}

		public static WorldPreloader WorldPreloader
		{
			get
			{
				return Service.worldPreloader;
			}
			set
			{
				if (Service.worldPreloader != null)
				{
					throw new Exception("An instance of the WorldPreloader service class has already been set!");
				}
				Service.worldPreloader = value;
			}
		}

		public static WorldTransitioner WorldTransitioner
		{
			get
			{
				return Service.worldTransitioner;
			}
			set
			{
				if (Service.worldTransitioner != null)
				{
					throw new Exception("An instance of the WorldTransitioner service class has already been set!");
				}
				Service.worldTransitioner = value;
			}
		}

		public static CurrentPlayer CurrentPlayer
		{
			get
			{
				return Service.currentPlayer;
			}
			set
			{
				if (Service.currentPlayer != null)
				{
					throw new Exception("An instance of the CurrentPlayer service class has already been set!");
				}
				Service.currentPlayer = value;
			}
		}

		public static ServerPlayerPrefs ServerPlayerPrefs
		{
			get
			{
				return Service.serverPlayerPrefs;
			}
			set
			{
				if (Service.serverPlayerPrefs != null)
				{
					throw new Exception("An instance of the ServerPlayerPrefs service class has already been set!");
				}
				Service.serverPlayerPrefs = value;
			}
		}

		public static SharedPlayerPrefs SharedPlayerPrefs
		{
			get
			{
				return Service.sharedPlayerPrefs;
			}
			set
			{
				if (Service.sharedPlayerPrefs != null)
				{
					throw new Exception("An instance of the SharedPlayerPrefs service class has already been set!");
				}
				Service.sharedPlayerPrefs = value;
			}
		}

		public static BuildingUpgradeCatalog BuildingUpgradeCatalog
		{
			get
			{
				return Service.buildingUpgradeCatalog;
			}
			set
			{
				if (Service.buildingUpgradeCatalog != null)
				{
					throw new Exception("An instance of the BuildingUpgradeCatalog service class has already been set!");
				}
				Service.buildingUpgradeCatalog = value;
			}
		}

		public static EquipmentUpgradeCatalog EquipmentUpgradeCatalog
		{
			get
			{
				return Service.equipmentUpgradeCatalog;
			}
			set
			{
				if (Service.equipmentUpgradeCatalog != null)
				{
					throw new Exception("An instance of the EquipmentUpgradeCatalog service class has already been set!");
				}
				Service.equipmentUpgradeCatalog = value;
			}
		}

		public static StarshipUpgradeCatalog StarshipUpgradeCatalog
		{
			get
			{
				return Service.starshipUpgradeCatalog;
			}
			set
			{
				if (Service.starshipUpgradeCatalog != null)
				{
					throw new Exception("An instance of the StarshipUpgradeCatalog service class has already been set!");
				}
				Service.starshipUpgradeCatalog = value;
			}
		}

		public static TroopUpgradeCatalog TroopUpgradeCatalog
		{
			get
			{
				return Service.troopUpgradeCatalog;
			}
			set
			{
				if (Service.troopUpgradeCatalog != null)
				{
					throw new Exception("An instance of the TroopUpgradeCatalog service class has already been set!");
				}
				Service.troopUpgradeCatalog = value;
			}
		}

		public static RUFManager RUFManager
		{
			get
			{
				return Service.rUFManager;
			}
			set
			{
				if (Service.rUFManager != null)
				{
					throw new Exception("An instance of the RUFManager service class has already been set!");
				}
				Service.rUFManager = value;
			}
		}

		public static EventManager EventManager
		{
			get
			{
				return Service.eventManager;
			}
			set
			{
				if (Service.eventManager != null)
				{
					throw new Exception("An instance of the EventManager service class has already been set!");
				}
				Service.eventManager = value;
			}
		}

		public static FactionIconUpgradeController FactionIconUpgradeController
		{
			get
			{
				return Service.factionIconUpgradeController;
			}
			set
			{
				if (Service.factionIconUpgradeController != null)
				{
					throw new Exception("An instance of the FactionIconUpgradeController service class has already been set!");
				}
				Service.factionIconUpgradeController = value;
			}
		}

		public static CameraManager CameraManager
		{
			get
			{
				return Service.cameraManager;
			}
			set
			{
				if (Service.cameraManager != null)
				{
					throw new Exception("An instance of the CameraManager service class has already been set!");
				}
				Service.cameraManager = value;
			}
		}

		public static DerivedTransformationManager DerivedTransformationManager
		{
			get
			{
				return Service.derivedTransformationManager;
			}
			set
			{
				if (Service.derivedTransformationManager != null)
				{
					throw new Exception("An instance of the DerivedTransformationManager service class has already been set!");
				}
				Service.derivedTransformationManager = value;
			}
		}

		public static EntityViewManager EntityViewManager
		{
			get
			{
				return Service.entityViewManager;
			}
			set
			{
				if (Service.entityViewManager != null)
				{
					throw new Exception("An instance of the EntityViewManager service class has already been set!");
				}
				Service.entityViewManager = value;
			}
		}

		public static ProjectorManager ProjectorManager
		{
			get
			{
				return Service.projectorManager;
			}
			set
			{
				if (Service.projectorManager != null)
				{
					throw new Exception("An instance of the ProjectorManager service class has already been set!");
				}
				Service.projectorManager = value;
			}
		}

		public static IBackButtonManager IBackButtonManager
		{
			get
			{
				return Service.iBackButtonManager;
			}
			set
			{
				if (Service.iBackButtonManager != null)
				{
					throw new Exception("An instance of the IBackButtonManager service class has already been set!");
				}
				Service.iBackButtonManager = value;
			}
		}

		public static UserInputInhibitor UserInputInhibitor
		{
			get
			{
				return Service.userInputInhibitor;
			}
			set
			{
				if (Service.userInputInhibitor != null)
				{
					throw new Exception("An instance of the UserInputInhibitor service class has already been set!");
				}
				Service.userInputInhibitor = value;
			}
		}

		public static UserInputManager UserInputManager
		{
			get
			{
				return Service.userInputManager;
			}
			set
			{
				if (Service.userInputManager != null)
				{
					throw new Exception("An instance of the UserInputManager service class has already been set!");
				}
				Service.userInputManager = value;
			}
		}

		public static ProjectileViewManager ProjectileViewManager
		{
			get
			{
				return Service.projectileViewManager;
			}
			set
			{
				if (Service.projectileViewManager != null)
				{
					throw new Exception("An instance of the ProjectileViewManager service class has already been set!");
				}
				Service.projectileViewManager = value;
			}
		}

		public static AnimController AnimController
		{
			get
			{
				return Service.animController;
			}
			set
			{
				if (Service.animController != null)
				{
					throw new Exception("An instance of the AnimController service class has already been set!");
				}
				Service.animController = value;
			}
		}

		public static WWWManager WWWManager
		{
			get
			{
				return Service.wWWManager;
			}
			set
			{
				if (Service.wWWManager != null)
				{
					throw new Exception("An instance of the WWWManager service class has already been set!");
				}
				Service.wWWManager = value;
			}
		}

		public static Logger Logger
		{
			get
			{
				return Service.logger;
			}
			set
			{
				if (Service.logger != null)
				{
					throw new Exception("An instance of the Logger service class has already been set!");
				}
				Service.logger = value;
			}
		}

		public static Lang Lang
		{
			get
			{
				return Service.lang;
			}
			set
			{
				if (Service.lang != null)
				{
					throw new Exception("An instance of the Lang service class has already been set!");
				}
				Service.lang = value;
			}
		}

		public static Rand Rand
		{
			get
			{
				return Service.rand;
			}
			set
			{
				if (Service.rand != null)
				{
					throw new Exception("An instance of the Rand service class has already been set!");
				}
				Service.rand = value;
			}
		}

		public static SimTimeEngine SimTimeEngine
		{
			get
			{
				return Service.simTimeEngine;
			}
			set
			{
				if (Service.simTimeEngine != null)
				{
					throw new Exception("An instance of the SimTimeEngine service class has already been set!");
				}
				Service.simTimeEngine = value;
			}
		}

		public static SimTimerManager SimTimerManager
		{
			get
			{
				return Service.simTimerManager;
			}
			set
			{
				if (Service.simTimerManager != null)
				{
					throw new Exception("An instance of the SimTimerManager service class has already been set!");
				}
				Service.simTimerManager = value;
			}
		}

		public static ViewTimeEngine ViewTimeEngine
		{
			get
			{
				return Service.viewTimeEngine;
			}
			set
			{
				if (Service.viewTimeEngine != null)
				{
					throw new Exception("An instance of the ViewTimeEngine service class has already been set!");
				}
				Service.viewTimeEngine = value;
			}
		}

		public static ViewTimerManager ViewTimerManager
		{
			get
			{
				return Service.viewTimerManager;
			}
			set
			{
				if (Service.viewTimerManager != null)
				{
					throw new Exception("An instance of the ViewTimerManager service class has already been set!");
				}
				Service.viewTimerManager = value;
			}
		}

		public static StickerController StickerController
		{
			get
			{
				return Service.stickerController;
			}
			set
			{
				if (Service.stickerController != null)
				{
					throw new Exception("An instance of the StickerController service class has already been set!");
				}
				Service.stickerController = value;
			}
		}

		public static void ResetAll()
		{
			Service.shieldEffects = null;
			Service.shieldController = null;
			Service.assetManager = null;
			Service.audioManager = null;
			Service.bILoggingController = null;
			Service.stepTimingController = null;
			Service.contentManager = null;
			Service.dMOAnalyticsController = null;
			Service.environmentController = null;
			Service.inAppPurchaseController = null;
			Service.serverAPI = null;
			Service.mobileConnectorAdsController = null;
			Service.animationEventManager = null;
			Service.currencyEffects = null;
			Service.fXManager = null;
			Service.mobilizationEffectsManager = null;
			Service.planetEffectController = null;
			Service.storageEffects = null;
			Service.terrainBlendController = null;
			Service.pathingManager = null;
			Service.botRunner = null;
			Service.iAccountSyncController = null;
			Service.achievementController = null;
			Service.appServerEnvironmentController = null;
			Service.armoryController = null;
			Service.baseLayoutToolController = null;
			Service.battleController = null;
			Service.battlePlaybackController = null;
			Service.battleRecordController = null;
			Service.boardController = null;
			Service.buffController = null;
			Service.buildingAnimationController = null;
			Service.buildingController = null;
			Service.buildingLookupController = null;
			Service.buildingTooltipController = null;
			Service.campaignController = null;
			Service.championController = null;
			Service.combatEncounterController = null;
			Service.combatTriggerManager = null;
			Service.combineMeshManager = null;
			Service.iCurrencyController = null;
			Service.defensiveBattleController = null;
			Service.deployableShardUnlockController = null;
			Service.deployerController = null;
			Service.droidController = null;
			Service.editBaseController = null;
			Service.engine = null;
			Service.entityFactory = null;
			Service.entityController = null;
			Service.entityIdleController = null;
			Service.entityRenderController = null;
			Service.episodeController = null;
			Service.episodeTaskManager = null;
			Service.episodeWidgetViewController = null;
			Service.gameIdleController = null;
			Service.gameStateMachine = null;
			Service.healthController = null;
			Service.holoController = null;
			Service.holonetController = null;
			Service.homeModeController = null;
			Service.inventoryCrateRewardController = null;
			Service.leaderboardController = null;
			Service.lightingEffectsController = null;
			Service.limitedEditionItemController = null;
			Service.mainController = null;
			Service.neighborVisitManager = null;
			Service.notificationController = null;
			Service.socialPushNotificationController = null;
			Service.objectiveController = null;
			Service.objectiveManager = null;
			Service.performanceMonitor = null;
			Service.performanceSampler = null;
			Service.perkManager = null;
			Service.perkViewController = null;
			Service.galaxyPlanetController = null;
			Service.galaxyViewController = null;
			Service.planetRelocationController = null;
			Service.playerIdentityController = null;
			Service.playerValuesController = null;
			Service.popupsManager = null;
			Service.postBattleRepairController = null;
			Service.profanityController = null;
			Service.projectileController = null;
			Service.promoPopupManager = null;
			Service.pvpManager = null;
			Service.questController = null;
			Service.quietCorrectionController = null;
			Service.raidDefenseController = null;
			Service.rewardManager = null;
			Service.screenController = null;
			Service.screenSizeController = null;
			Service.serverController = null;
			Service.shardShopController = null;
			Service.shooterController = null;
			Service.shuttleController = null;
			Service.skinController = null;
			Service.iSocialDataController = null;
			Service.spatialIndexController = null;
			Service.specialAttackController = null;
			Service.squadTroopAttackController = null;
			Service.warBoardBuildingController = null;
			Service.warBoardViewController = null;
			Service.squadController = null;
			Service.troopDonationTrackController = null;
			Service.staticDataController = null;
			Service.stunController = null;
			Service.summonController = null;
			Service.iSupportController = null;
			Service.targetedBundleController = null;
			Service.targetingController = null;
			Service.tournamentController = null;
			Service.transportController = null;
			Service.trapController = null;
			Service.trapViewController = null;
			Service.troopAbilityController = null;
			Service.troopAttackController = null;
			Service.troopController = null;
			Service.turretAttackController = null;
			Service.uXController = null;
			Service.unlockController = null;
			Service.valueObjectController = null;
			Service.victoryConditionController = null;
			Service.warBaseEditController = null;
			Service.homeMapDataLoader = null;
			Service.npcMapDataLoader = null;
			Service.pvpMapDataLoader = null;
			Service.replayMapDataLoader = null;
			Service.warBaseMapDataLoader = null;
			Service.worldController = null;
			Service.worldInitializer = null;
			Service.worldPreloader = null;
			Service.worldTransitioner = null;
			Service.currentPlayer = null;
			Service.serverPlayerPrefs = null;
			Service.sharedPlayerPrefs = null;
			Service.buildingUpgradeCatalog = null;
			Service.equipmentUpgradeCatalog = null;
			Service.starshipUpgradeCatalog = null;
			Service.troopUpgradeCatalog = null;
			Service.rUFManager = null;
			Service.eventManager = null;
			Service.factionIconUpgradeController = null;
			Service.cameraManager = null;
			Service.derivedTransformationManager = null;
			Service.entityViewManager = null;
			Service.projectorManager = null;
			Service.iBackButtonManager = null;
			Service.userInputInhibitor = null;
			Service.userInputManager = null;
			Service.projectileViewManager = null;
			Service.animController = null;
			Service.wWWManager = null;
			Service.logger = null;
			Service.lang = null;
			Service.rand = null;
			Service.simTimeEngine = null;
			Service.simTimerManager = null;
			Service.viewTimeEngine = null;
			Service.viewTimerManager = null;
			Service.stickerController = null;
		}
	}
}
