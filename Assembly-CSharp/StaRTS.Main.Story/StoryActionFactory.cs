using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story.Actions;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story
{
	public static class StoryActionFactory
	{
		public const string ACTIVATE_TRIGGER = "ActivateTrigger";

		public const string ACTIVATE_SAVE_TRIGGER = "ActivateSaveTrigger";

		public const string CLUSTER_AND = "ClusterAND";

		public const string PLAY_AUDIO = "PlayAudio";

		public const string TRANSITION_TO_WORLD = "TransitionToWorld";

		public const string TRANSITION_TO_HOME = "TransitionToHome";

		public const string DELAY = "Delay";

		public const string SHOW_HOLO = "ShowHolo";

		public const string PLAY_HOLO_ANIM = "PlayHoloAnim";

		public const string SHOW_TRANSCRIPT = "ShowTranscript";

		public const string DISPLAY_BUTTON = "DisplayButton";

		public const string HIDE_HOLO = "HideHolo";

		public const string HIDE_TRANSCRIPT = "HideTranscript";

		public const string CONFIGURE_CONTROLS = "ConfigureControls";

		public const string PAUSE_BATTLE = "PauseBattle";

		public const string RESUME_BATTLE = "ResumeBattle";

		public const string ACTIVATE_MISSION = "ActivateMission";

		public const string DEPLOY_BUILDING = "DeployBuilding";

		public const string REMOVE_BUILDING = "RemoveBuilding";

		public const string PRESS_HERE = "PressHere";

		public const string PRESS_HERE_SCREEN = "PressHereScreen";

		public const string CLEAR_PRESS_HERE = "ClearPressHere";

		public const string SHOW_INSTRUCTION = "ShowInstruction";

		public const string MOVE_CAMERA = "MoveCamera";

		public const string DEPLOY_STARFIGHTER = "DeployStarfighter";

		public const string CIRCLE_BUTTON = "CircleButton";

		public const string CLEAR_CIRCLE_BUTTON = "ClearCircleButton";

		public const string DEFEND_BASE = "DefendBase";

		public const string CIRCLE_REGION = "CircleRegion";

		public const string CIRCLE_BUILDING = "CircleBuilding";

		public const string STORE_LOOKUP = "StoreLookup";

		public const string EXIT_EDIT_MODE = "ExitEditMode";

		public const string DESELECT_BUILDING = "DeselectBuilding";

		public const string DISABLE_CLICKS = "DisableClicks";

		public const string DISABLE_GRID_SCROLLING = "DisableGridScrolling";

		public const string ENABLE_CLICKS = "EnableClicks";

		public const string ENABLE_GRID_SCROLLING = "EnableGridScrolling";

		public const string ALLOW_DEPLOY = "AllowDeploy";

		public const string SHOW_CHOOSE_FACTION_SCREEN = "ShowChooseFactionScreen";

		public const string TRAINING_INSTRUCTIONS = "TrainingInstructions";

		public const string HIGHLIGHT_AREA = "HighlightAreaRectangle";

		public const string HIGHLIGHT_BUTTON = "HighlightButton";

		public const string HIGHLIGHT_REGION = "HighlightRegion";

		public const string HIGHLIGHT_BUILDING = "HighlightBuilding";

		public const string CLEAR_HIGHLIGHT = "ClearHighlight";

		public const string SAVE_PROGRESS = "SaveProgress";

		public const string CLEAR_PROGRESS = "ClearProgress";

		public const string DEACTIVATE_TRIGGER = "DeactivateTrigger";

		public const string ZOOM_CAMERA = "ZoomCamera";

		public const string SET_BUILDING_REPAIR_LEVEL_TYPE = "SetBuildingTypeRepairLevel";

		public const string SET_BUILDING_REPAIR_LEVEL_TYPE_AND_AREA = "SetBuildingTypeRepairLevelInArea";

		public const string SET_BUILDING_REPAIR_LEVEL_ALL = "SetAllBuildingsRepairLevel";

		public const string PAUSE_BUILDING_REPAIR = "PauseBuildingRepair";

		public const string UNPAUSE_BUILDING_REPAIR = "UnpauseBuildingRepair";

		public const string HIDE_BUILDING_TOOLTIPS = "HideBuildingTooltips";

		public const string SHOW_BUILDING_TOOLTIPS = "ShowBuildingTooltips";

		public const string DELAY_BLOCKING = "DelayBlocking";

		public const string HIDE_INSTRUCTION = "HideInstruction";

		public const string END_CHAIN = "EndChain";

		public const string SHOW_UI_ELEMENT = "ShowUIElement";

		public const string HIDE_UI_ELEMENT = "HideUIElement";

		public const string SHOW_BUILDING_TOOLTIP_BY_TYPE = "ShowBuildingTooltipByType";

		public const string HIDE_BUILDING_TOOLTIP_BY_TYPE = "HideBuildingTooltipByType";

		public const string START_PLACE_BUILDING = "StartPlaceBuilding";

		public const string SHOW_HOLO_INFO_PANEL = "ShowInfoPanel";

		public const string SHOW_PLANET_INFO_PANEL = "ShowPlanetPanel";

		public const string HIDE_HOLO_INFO_PANEL = "HideInfoPanel";

		public const string SHOW_WHATS_NEXT_SCREEN = "ShowWhatsNextScreen";

		public const string SHOW_SET_CALLSIGN_SCREEN = "ShowSetCallSignScreen";

		public const string SHOW_SET_CALLSIGN_SCREEN_NO_AUTH = "ShowSetCallSignScreenHackNoAuth";

		public const string SET_MUSIC_VOLUME = "SetMusicVolume";

		public const string OPEN_EPISODE_INFO_SCREEN = "OpenEpisodeInfoScreen";

		public const string OPEN_STORE_SCREEN = "OpenStoreScreen";

		public const string DISABLE_CANCEL_BUILDING_PLACEMENT = "DisableCancelBuildingPlacement";

		public const string ENABLE_CANCEL_BUILDING_PLACEMENT = "EnableCancelBuildingPlacement";

		public const string END_FUE = "EndFue";

		public const string SPAWN_DEFENSIVE_TROOP = "SpawnDefensiveTroop";

		public const string PROMPT_PVP = "PromptPvp";

		public const string SHOW_PUSH_NOTIFICATION_SETTINGS_SCREEN = "ShowPushNotificationSettingsScreen";

		public const string SHOW_RATE_MY_APP_SCREEN = "ShowRateMyAppScreen";

		public const string CLEAR_BUILDING_HIGHLIGHT = "ClearBuildingHighlight";

		public const string SHOW_TEXT_CRAWL = "ShowTextCrawl";

		public const string SPIN_PLANET_FORWARD = "SpinPlanetForward";

		public const string REBEL_EMPIRE_FORKING = "RebelEmpireFork";

		public const string SHOW_SCREEN = "ShowScreen";

		public const string CLOSE_SCREEN = "CloseScreen";

		public const string PAN_TO_PLANET = "PanToPlanet";

		public const string EDIT_PREF = "EditPref";

		public const string IF_PREF_GATE = "IfPrefGate";

		public const string IF_MAIN_FUE_GATE = "IfMainFUEGate";

		public const string PLAY_PLANET_INTRO = "PlayPlanetIntro";

		public const string SHOW_WAR_HELP = "ShowWarHelp";

		public static IStoryAction GenerateStoryAction(StoryActionVO vo, IStoryReactor parent)
		{
			string actionType = vo.ActionType;
			switch (actionType)
			{
			case "ActivateTrigger":
			case "ActivateSaveTrigger":
				return new ActivateTriggerStoryAction(vo, parent);
			case "ClusterAND":
				return new ClusterANDStoryAction(vo, parent);
			case "PlayAudio":
				return new PlayAudioStoryAction(vo, parent);
			case "TransitionToWorld":
				return new TransitionToWorldStoryAction(vo, parent);
			case "TransitionToHome":
				return new TransitionToHomeStoryAction(vo, parent);
			case "Delay":
				return new DelayStoryAction(vo, parent);
			case "ShowHolo":
				return new ShowHologramStoryAction(vo, parent);
			case "PlayHoloAnim":
				return new PlayHoloAnimationStoryAction(vo, parent);
			case "ShowTranscript":
				return new ShowTranscriptStoryAction(vo, parent);
			case "DisplayButton":
				return new DisplayButtonStoryAction(vo, parent);
			case "HideHolo":
				return new HideHologramStoryAction(vo, parent);
			case "HideTranscript":
				return new HideTranscriptStoryAction(vo, parent);
			case "ConfigureControls":
				return new ConfigureControlsStoryAction(vo, parent);
			case "PauseBattle":
				return new PauseBattleStoryAction(vo, parent);
			case "ResumeBattle":
				return new ResumeBattleStoryAction(vo, parent);
			case "ActivateMission":
				return new ActivateMissionStoryAction(vo, parent);
			case "MoveCamera":
				return new MoveCameraStoryAction(vo, parent);
			case "DeployStarfighter":
				return new DeployStarshipAttackStoryAction(vo, parent);
			case "CircleButton":
			case "HighlightButton":
				return new CircleButtonStoryAction(vo, parent);
			case "PressHere":
				return new PressHereStoryAction(vo, parent, false);
			case "PressHereScreen":
				return new PressHereStoryAction(vo, parent, true);
			case "ClearPressHere":
				return new ClearPressHereStoryAction(vo, parent);
			case "ShowInstruction":
				return new ShowInstructionStoryAction(vo, parent);
			case "ClearCircleButton":
			case "ClearHighlight":
				return new ClearButtonCircleStoryAction(vo, parent);
			case "DeployBuilding":
				return new DeployBuildingStoryAction(vo, parent);
			case "RemoveBuilding":
				return new RemoveBuildingStoryAction(vo, parent);
			case "DefendBase":
				return new DefendBaseStoryAction(vo, parent);
			case "HighlightAreaRectangle":
				return new HighlightAreaStoryAction(vo, parent);
			case "CircleRegion":
			case "HighlightRegion":
				return new CircleRegionStoryAction(vo, parent);
			case "CircleBuilding":
			case "HighlightBuilding":
				return new CircleBuildingStoryAction(vo, parent);
			case "StoreLookup":
				return new StoreLookupStoryAction(vo, parent);
			case "ExitEditMode":
				return new ExitEditModeStoryAction(vo, parent);
			case "DeselectBuilding":
				return new DeselectBuildingStoryAction(vo, parent);
			case "DisableClicks":
				return new DisableClicksStoryAction(vo, parent);
			case "EnableClicks":
				return new EnableClicksStoryAction(vo, parent);
			case "DisableGridScrolling":
				return new DisableGridScrollingStoryAction(vo, parent);
			case "EnableGridScrolling":
				return new EnableGridScrollingStoryAction(vo, parent);
			case "AllowDeploy":
				return new AllowDeployStoryAction(vo, parent);
			case "ShowChooseFactionScreen":
				return new ShowChooseFactionScreenStoryAction(vo, parent);
			case "TrainingInstructions":
				return new TrainingInstructionsStoryAction(vo, parent);
			case "SaveProgress":
				return new SaveProgressStoryAction(vo, parent);
			case "ClearProgress":
				return new ClearProgressStoryAction(vo, parent);
			case "DeactivateTrigger":
				return new DeactivateTriggerStoryAction(vo, parent);
			case "ZoomCamera":
				return new ZoomCameraStoryAction(vo, parent);
			case "SetAllBuildingsRepairLevel":
			case "SetBuildingTypeRepairLevel":
			case "SetBuildingTypeRepairLevelInArea":
				return new SetBuildingRepairStateStoryAction(vo, parent);
			case "PauseBuildingRepair":
				return new PauseBuildingRepairStoryAction(vo, parent);
			case "UnpauseBuildingRepair":
				return new UnpauseBuildingRepairStoryAction(vo, parent);
			case "HideBuildingTooltips":
				return new ShowBuildingTooltipsStoryAction(vo, parent, false);
			case "ShowBuildingTooltips":
				return new ShowBuildingTooltipsStoryAction(vo, parent, true);
			case "DelayBlocking":
				return new DelayBlockingStoryAction(vo, parent);
			case "HideInstruction":
				return new HideInstructionStoryAction(vo, parent);
			case "EndChain":
				return new EndChainStoryAction(vo, parent);
			case "ShowUIElement":
				return new ShowUIElementStoryAction(vo, parent, true);
			case "HideUIElement":
				return new ShowUIElementStoryAction(vo, parent, false);
			case "ShowBuildingTooltipByType":
				return new ShowBuildingTooltipByTypeStoryAction(vo, parent, true);
			case "HideBuildingTooltipByType":
				return new ShowBuildingTooltipByTypeStoryAction(vo, parent, false);
			case "StartPlaceBuilding":
				return new StartPlaceBuildingStoryAction(vo, parent);
			case "ShowInfoPanel":
				return new ShowHologramInfoStoryAction(vo, parent, false);
			case "ShowPlanetPanel":
				return new ShowHologramInfoStoryAction(vo, parent, true);
			case "HideInfoPanel":
				return new HideHoloInfoPanelStoryAction(vo, parent);
			case "ShowSetCallSignScreen":
				return new ShowSetCallSignScreenStoryAction(true, vo, parent);
			case "ShowSetCallSignScreenHackNoAuth":
				return new ShowSetCallSignScreenStoryAction(false, vo, parent);
			case "ShowWhatsNextScreen":
				return new ShowWhatsNextScreenStoryAction(vo, parent);
			case "SetMusicVolume":
				return new SetMusicVolumeStoryAction(vo, parent);
			case "OpenEpisodeInfoScreen":
				return new OpenEpisodeInfoScreenStoryAction(vo, parent);
			case "OpenStoreScreen":
				return new OpenStoreScreenStoryAction(vo, parent);
			case "DisableCancelBuildingPlacement":
				return new EnableCancelBuildingPlacementStoryAction(vo, parent, false);
			case "EnableCancelBuildingPlacement":
				return new EnableCancelBuildingPlacementStoryAction(vo, parent, true);
			case "EndFue":
				return new EndFueStoryAction(vo, parent);
			case "SpawnDefensiveTroop":
				return new SpawnDefensiveTroopStoryAction(vo, parent);
			case "PromptPvp":
				return new PromptPvpStoryAction(vo, parent);
			case "ShowPushNotificationSettingsScreen":
				return new ShowPushNotificationsSettingsScreenStoryAction(vo, parent);
			case "ShowRateMyAppScreen":
				return new ShowRateAppScreenStoryAction(vo, parent);
			case "ClearBuildingHighlight":
				return new ClearBuildingHighlightStoryAction(vo, parent);
			case "ShowTextCrawl":
				return new TextCrawlStoryAction(vo, parent);
			case "SpinPlanetForward":
				return new SpinPlanetForwardStoryAction(vo, parent);
			case "RebelEmpireFork":
				return new RebelEmpireForkingStoryAction(vo, parent);
			case "ShowScreen":
				return new ShowScreenStoryAction(vo, parent);
			case "CloseScreen":
				return new CloseScreenStoryAction(vo, parent);
			case "PanToPlanet":
				return new PanToPlanetStoryAction(vo, parent);
			case "EditPref":
				return new EditPrefStoryAction(vo, parent);
			case "IfPrefGate":
				return new IfPrefGateStoryAction(vo, parent);
			case "IfMainFUEGate":
				return new MainFUEGateStoryAction(vo, parent);
			case "PlayPlanetIntro":
				return new PlayPlanetIntroStoryAction(vo, parent);
			case "ShowWarHelp":
				return new OpenWarInfoStoryAction(vo, parent);
			}
			Service.Logger.ErrorFormat("There is no entry in the StoryActionFactory for {0}", new object[]
			{
				vo.ActionType
			});
			return new DegenerateStoryAction(vo, parent);
		}
	}
}
