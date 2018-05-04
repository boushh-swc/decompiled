using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Reflection;

namespace StaRTS.Main.Models
{
	public static class GameConstants
	{
		private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty;

		public static string ALL_LOCALES
		{
			get;
			private set;
		}

		public static int COEF_EXP_ACCURACY
		{
			get;
			private set;
		}

		public static int CREDITS_COEFFICIENT
		{
			get;
			private set;
		}

		public static int CREDITS_EXPONENT
		{
			get;
			private set;
		}

		public static int ALLOY_COEFFICIENT
		{
			get;
			private set;
		}

		public static int ALLOY_EXPONENT
		{
			get;
			private set;
		}

		public static int CONTRABAND_COEFFICIENT
		{
			get;
			private set;
		}

		public static int CONTRABAND_EXPONENT
		{
			get;
			private set;
		}

		public static int CRYSTALS_SPEED_UP_COEFFICIENT
		{
			get;
			private set;
		}

		public static int CRYSTALS_SPEED_UP_EXPONENT
		{
			get;
			private set;
		}

		public static int SQUADPERK_CRYSTALS_SPEED_UP_COEFFICIENT
		{
			get;
			private set;
		}

		public static int SQUADPERK_CRYSTALS_SPEED_UP_EXPONENT
		{
			get;
			private set;
		}

		public static int CRYSTALS_SPEED_UP_TIME_GATE_COEFFICIENT
		{
			get;
			private set;
		}

		public static int CRYSTALS_SPEED_UP_TIME_GATE_EXPONENT
		{
			get;
			private set;
		}

		public static string CRYSTAL_PACK_AMOUNT
		{
			get;
			private set;
		}

		public static string CRYSTAL_PACK_COST_USD
		{
			get;
			private set;
		}

		public static string PROTECTION_DURATION
		{
			get;
			private set;
		}

		public static string PROTECTION_CRYSTAL_COSTS
		{
			get;
			private set;
		}

		public static int RESPAWN_TIME_NATURAL_RESOURCE
		{
			get;
			private set;
		}

		public static int INVENTORY_INITIAL_CAP
		{
			get;
			private set;
		}

		public static int INVENTORY_LIMIT
		{
			get;
			private set;
		}

		public static int SECONDS_PER_CRYSTAL
		{
			get;
			private set;
		}

		public static string DROID_CRYSTAL_COSTS
		{
			get;
			private set;
		}

		public static string FUE_BATTLE
		{
			get;
			private set;
		}

		public static float EDIT_LONG_PRESS_PRE_FADE
		{
			get;
			private set;
		}

		public static float EDIT_LONG_PRESS_FADE
		{
			get;
			private set;
		}

		public static string POST_FUE_REBEL_BASE
		{
			get;
			private set;
		}

		public static string POST_FUE_EMPIRE_BASE
		{
			get;
			private set;
		}

		public static string NEW_PLAYER_FACTION
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_CREDITS_AMOUNT
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_MATERIALS_AMOUNT
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_CONTRABAND_AMOUNT
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_CREDITS_CAPACITY
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_MATERIALS_CAPACITY
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_CONTRABAND_CAPACITY
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_REPUTATION_AMOUNT
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_CRYSTALS_AMOUNT
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_XP_AMOUNT
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_DROIDS_AMOUNT
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_DROIDS_CAPACITY
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_TROOP_CAPACITY
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_HERO_CAPACITY
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_CHAMPION_CAPACITY
		{
			get;
			private set;
		}

		public static int NEW_PLAYER_STARSHIP_CAPACITY
		{
			get;
			private set;
		}

		public static string NEW_PLAYER_INITIAL_MISSION_REBEL
		{
			get;
			private set;
		}

		public static string NEW_PLAYER_INITIAL_MISSION_EMPIRE
		{
			get;
			private set;
		}

		public static string FUE_QUEST_UID
		{
			get;
			private set;
		}

		public static int PVP_MATCH_DURATION
		{
			get;
			private set;
		}

		public static float PVP_MATCH_COUNTDOWN
		{
			get;
			private set;
		}

		public static string PVP_SEARCH_COST_BY_HQ_LEVEL
		{
			get;
			private set;
		}

		public static int PVP_SEARCH_TIMEOUT_DURATION
		{
			get;
			private set;
		}

		public static int HQ_LOOTABLE_CREDITS
		{
			get;
			private set;
		}

		public static int HQ_LOOTABLE_MATERIALS
		{
			get;
			private set;
		}

		public static int HQ_LOOTABLE_CONTRABAND
		{
			get;
			private set;
		}

		public static int TURRET_SWAP_HQ_UNLOCK
		{
			get;
			private set;
		}

		public static float CAMPAIGN_STORY_INTRO_DELAY
		{
			get;
			private set;
		}

		public static float CAMPAIGN_STORY_SUCCESS_DELAY
		{
			get;
			private set;
		}

		public static float CAMPAIGN_STORY_FAILURE_DELAY
		{
			get;
			private set;
		}

		public static float CAMPAIGN_STORY_GoalFailure_DELAY
		{
			get;
			private set;
		}

		public static string SHIELD_HEALTH_PER_POINT
		{
			get;
			private set;
		}

		public static string SHIELD_RANGE_PER_POINT
		{
			get;
			private set;
		}

		public static int CRYSTAL_SPEND_WARNING_MINIMUM
		{
			get;
			private set;
		}

		public static int DEFAULT_BATTLE_LENGTH
		{
			get;
			private set;
		}

		public static int BATTLE_WARNING_TIME
		{
			get;
			private set;
		}

		public static int BATTLE_END_DELAY
		{
			get;
			private set;
		}

		public static int IDLE_RELOAD_TIME
		{
			get;
			private set;
		}

		public static int PAUSED_RELOAD_TIME
		{
			get;
			private set;
		}

		public static int MAX_TROOP_DONATIONS
		{
			get;
			private set;
		}

		public static int MAX_PER_USER_TROOP_DONATION
		{
			get;
			private set;
		}

		public static int SQUAD_MEMBER_LIMIT
		{
			get;
			private set;
		}

		public static int SQUAD_CREATE_MIN_TROPHY_REQ
		{
			get;
			private set;
		}

		public static int SQUAD_CREATE_MAX_TROPHY_REQ
		{
			get;
			private set;
		}

		public static int SQUAD_CREATE_COST
		{
			get;
			private set;
		}

		public static uint SQUAD_TROOP_REQUEST_THROTTLE_MINUTES
		{
			get;
			private set;
		}

		public static int SQUAD_NAME_LENGTH_MIN
		{
			get;
			private set;
		}

		public static int SQUAD_NAME_LENGTH_MAX
		{
			get;
			private set;
		}

		public static bool SQUAD_INVITES_ENABLED
		{
			get;
			private set;
		}

		public static bool SQUAD_INVITES_TO_LEADERS_ENABLED
		{
			get;
			private set;
		}

		public static int CONTRACT_REFUND_PERCENTAGE_BUILDINGS
		{
			get;
			private set;
		}

		public static int CONTRACT_REFUND_PERCENTAGE_TROOPS
		{
			get;
			private set;
		}

		public static int ATTACK_RATING_WEIGHT
		{
			get;
			private set;
		}

		public static int DEFENSE_RATING_WEIGHT
		{
			get;
			private set;
		}

		public static float PVP_MATCH_BONUS_ATTACKER_SLOPE
		{
			get;
			private set;
		}

		public static float PVP_MATCH_BONUS_ATTACKER_Y_INTERCEPT
		{
			get;
			private set;
		}

		public static bool START_FUE_IN_BATTLE_MODE
		{
			get;
			private set;
		}

		public static int USER_NAME_MAX_CHARACTERS
		{
			get;
			private set;
		}

		public static int USER_NAME_MIN_CHARACTERS
		{
			get;
			private set;
		}

		public static float KEEP_ALIVE_DISPATCH_WAIT_TIME
		{
			get;
			private set;
		}

		public static int UNDER_ATTACK_STATUS_CHECK_INTERVAL
		{
			get;
			private set;
		}

		public static int CAMPAIGN_HOURS_UPCOMING
		{
			get;
			private set;
		}

		public static int CAMPAIGN_HOURS_CLOSING
		{
			get;
			private set;
		}

		public static int TOURNAMENT_HOURS_UPCOMING
		{
			get;
			private set;
		}

		public static int TOURNAMENT_HOURS_SHOW_BADGE
		{
			get;
			private set;
		}

		public static int TOURNAMENT_HOURS_CLOSING
		{
			get;
			private set;
		}

		public static string TOURNAMENT_RATING_DELTAS_ATTACKER
		{
			get;
			private set;
		}

		public static uint TOURNAMENT_TIER_CHANGE_VIEW_THROTTLE
		{
			get;
			private set;
		}

		public static uint CONFLICT_REWARD_PREVIEW_MULT
		{
			get;
			private set;
		}

		public static uint CONFLICT_SHOW_MULTIPLIER
		{
			get;
			private set;
		}

		public static bool REFUND_SURVIVORS
		{
			get;
			private set;
		}

		public static uint SQUAD_TROOP_DEPLOY_STAGGER
		{
			get;
			private set;
		}

		public static string SQUAD_TROOP_DEPLOY_FLAG_EMPIRE_ASSET
		{
			get;
			private set;
		}

		public static string SQUAD_TROOP_DEPLOY_FLAG_REBEL_ASSET
		{
			get;
			private set;
		}

		public static bool RATE_MY_APP_ENABLED
		{
			get;
			private set;
		}

		public static int FB_CONNECT_REWARD
		{
			get;
			private set;
		}

		public static int CREDITS_2_THRESHOLD
		{
			get;
			private set;
		}

		public static int CREDITS_3_THRESHOLD
		{
			get;
			private set;
		}

		public static int MATERIALS_2_THRESHOLD
		{
			get;
			private set;
		}

		public static int MATERIALS_3_THRESHOLD
		{
			get;
			private set;
		}

		public static int CONTRABAND_2_THRESHOLD
		{
			get;
			private set;
		}

		public static int CONTRABAND_3_THRESHOLD
		{
			get;
			private set;
		}

		public static int CRYSTALS_2_THRESHOLD
		{
			get;
			private set;
		}

		public static int CRYSTALS_3_THRESHOLD
		{
			get;
			private set;
		}

		public static string VO_BLACKLIST
		{
			get;
			private set;
		}

		public static int SPAWN_HEALTH_PERCENT
		{
			get;
			private set;
		}

		public static uint SPAWN_DELAY
		{
			get;
			private set;
		}

		public static int GRIND_MISSION_MAXIMUM
		{
			get;
			private set;
		}

		public static bool FORUMS_ENABLED
		{
			get;
			private set;
		}

		public static bool IAP_DISABLED_ANDROID
		{
			get;
			private set;
		}

		public static string IAP_DISCLAIMER_WHITELIST
		{
			get;
			private set;
		}

		public static bool PROMO_UNIT_TEST_ENABLED
		{
			get;
			private set;
		}

		public static bool IAP_FORCE_POPUP_ENABLED
		{
			get;
			private set;
		}

		public static bool FACTION_CHOICE_CONFIRM_SCREEN_ENABLED
		{
			get;
			private set;
		}

		public static bool SET_CALLSIGN_CONFIRM_SCREEN_ENABLED
		{
			get;
			private set;
		}

		public static float HUD_RESOURCE_TICKER_MAX_DURATION
		{
			get;
			private set;
		}

		public static float HUD_RESOURCE_TICKER_MIN_DURATION
		{
			get;
			private set;
		}

		public static int HUD_RESOURCE_TICKER_CRYSTAL_THRESHOLD
		{
			get;
			private set;
		}

		public static bool PROMO_BUTTON_RESHOW_GLOW
		{
			get;
			private set;
		}

		public static int PROMO_BUTTON_GLOW_DURATION
		{
			get;
			private set;
		}

		public static bool TARGETED_OFFERS_ENABLED
		{
			get;
			private set;
		}

		public static int TARGETED_OFFERS_FREQUENCY_LIMIT
		{
			get;
			private set;
		}

		public static int RATE_APP_INCENTIVE_CRYSTALS
		{
			get;
			private set;
		}

		public static bool PVP_LOSE_ON_PAUSE
		{
			get;
			private set;
		}

		public static bool PVP_LOSE_ON_QUIT
		{
			get;
			private set;
		}

		public static bool NO_FB_FACTION_CHOICE_ANDROID
		{
			get;
			private set;
		}

		public static bool RATE_APP_INCENTIVE_SHOW_IOS
		{
			get;
			private set;
		}

		public static bool RATE_APP_INCENTIVE_GRANT_IOS
		{
			get;
			private set;
		}

		public static bool RATE_APP_INCENTIVE_SHOW_ANDROID
		{
			get;
			private set;
		}

		public static bool RATE_APP_INCENTIVE_GRANT_ANDROID
		{
			get;
			private set;
		}

		public static bool RATE_APP_INCENTIVE_SHOW_WINDOWS
		{
			get;
			private set;
		}

		public static bool RATE_APP_INCENTIVE_GRANT_WINDOWS
		{
			get;
			private set;
		}

		public static bool QUIET_CORRECTION_ENABLED
		{
			get;
			private set;
		}

		public static string QUIET_CORRECTION_WHITELIST
		{
			get;
			private set;
		}

		public static bool ENABLE_INSTANT_BUY
		{
			get;
			private set;
		}

		public static bool ENABLE_UPGRADE_ALL_WALLS
		{
			get;
			private set;
		}

		public static float UPGRADE_ALL_WALLS_CONVENIENCE_TAX
		{
			get;
			private set;
		}

		public static int UPGRADE_ALL_WALLS_COEFFICIENT
		{
			get;
			private set;
		}

		public static int UPGRADE_ALL_WALL_EXPONENT
		{
			get;
			private set;
		}

		public static bool MESH_COMBINE_DISABLED
		{
			get;
			private set;
		}

		public static bool ASSET_BUNDLE_CACHE_CLEAN_DISABLED
		{
			get;
			private set;
		}

		public static int ASSET_BUNDLE_CACHE_CLEAN_VERSION
		{
			get;
			private set;
		}

		public static int MAX_CONCURRENT_ASSET_LOADS
		{
			get;
			private set;
		}

		public static int DEFLECTION_VELOCITY_PERCENT
		{
			get;
			private set;
		}

		public static int DEFLECTION_DURATION_MS
		{
			get;
			private set;
		}

		public static int FACTION_FLIPPING_UNLOCK_LEVEL
		{
			get;
			private set;
		}

		public static int CONTRABAND_UNLOCK_LEVEL
		{
			get;
			private set;
		}

		public static int PERFORMANCE_SAMPLE_DELAY_HOME
		{
			get;
			private set;
		}

		public static int PERFORMANCE_SAMPLE_DELAY_BATTLE
		{
			get;
			private set;
		}

		public static int FPS_THRESHOLD
		{
			get;
			private set;
		}

		public static string RAID_DEFENSE_TRAINER_BINDINGS
		{
			get;
			private set;
		}

		public static int AUTOSELECT_DISABLE_HQTHRESHOLD
		{
			get;
			private set;
		}

		public static float PUSH_NOTIFICATION_SQUAD_JOIN_COOLDOWN
		{
			get;
			private set;
		}

		public static float PUSH_NOTIFICATIONS_TROOP_REQUEST_COOLDOWN
		{
			get;
			private set;
		}

		public static bool FACEBOOK_INVITES_ENABLED
		{
			get;
			private set;
		}

		public static string PLANET_RELOCATED_TUTORIAL_ID
		{
			get;
			private set;
		}

		public static float GALAXY_AUTO_ROTATE_SPEED
		{
			get;
			private set;
		}

		public static float GALAXY_AUTO_ROTATE_DELAY
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_FOREGROUND_THRESHOLD
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_FOREGROUND_PLATEAU_THRESHOLD
		{
			get;
			private set;
		}

		public static float GALAXY_CAMERA_HEIGHT_OFFSET
		{
			get;
			private set;
		}

		public static float GALAXY_CAMERA_DISTANCE_OFFSET
		{
			get;
			private set;
		}

		public static float GALAXY_EASE_ROTATION_TIME
		{
			get;
			private set;
		}

		public static float GALAXY_EASE_ROTATION_TRANSITION_TIME
		{
			get;
			private set;
		}

		public static float GALAXY_INITIAL_GALAXY_ZOOM_DIST
		{
			get;
			private set;
		}

		public static float GALAXY_INITIAL_GALAXY_ZOOM_TIME
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_VIEW_HEIGHT
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_GALAXY_ZOOM_TIME
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_POPULATION_COUNT_PERCENTAGE
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_POPULATION_UPDATE_TIME
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_FOREGROUND_UI_THRESHOLD
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_SWIPE_MIN_MOVE
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_SWIPE_MAX_TIME
		{
			get;
			private set;
		}

		public static float GALAXY_PLANET_SWIPE_TIME
		{
			get;
			private set;
		}

		private static string CRYSTALS_PER_RELOCATION_STAR
		{
			get;
			set;
		}

		public static int[] CrystalsPerRelocationStar
		{
			get;
			private set;
		}

		private static string STARS_PER_RELOCATION
		{
			get;
			set;
		}

		public static int[] StarsPerRelocation
		{
			get;
			private set;
		}

		public static bool PUSH_NOTIFICATION_ENABLE_INCENTIVE
		{
			get;
			private set;
		}

		public static int PUSH_NOTIFICATION_CRYSTAL_REWARD_AMOUNT
		{
			get;
			private set;
		}

		public static int PUSH_NOTIFICATION_MAX_REACHED
		{
			get;
			private set;
		}

		public static float FADE_OUT_CONSTANT_LENGTH
		{
			get;
			private set;
		}

		public static float GALAXY_UI_PLANET_FOCUS_THROTTLE
		{
			get;
			private set;
		}

		public static bool TIME_OF_DAY_ENABLED
		{
			get;
			private set;
		}

		public static double TOD_MID_DAY_PERCENTAGE
		{
			get;
			private set;
		}

		public static int RAIDS_HQ_UNLOCK_LEVEL
		{
			get;
			private set;
		}

		public static int RAIDS_UPCOMING_TICKER_THROTTLE
		{
			get;
			private set;
		}

		public static bool LOG_STACK_TRACE_TO_BI
		{
			get;
			private set;
		}

		public static bool EVENT_2_BI_ENABLED
		{
			get;
			private set;
		}

		public static bool DISABLE_BASE_LAYOUT_TOOL
		{
			get;
			private set;
		}

		public static int WAR_MAX_BUFF_BASES
		{
			get;
			private set;
		}

		public static int SQUADPERK_MAX_SQUAD_LEVEL_CELEBRATIONS_SHOWN
		{
			get;
			private set;
		}

		public static string WAR_HELP_OVERVIEW_REBEL
		{
			get;
			private set;
		}

		public static string WAR_HELP_OVERVIEW_EMPIRE
		{
			get;
			private set;
		}

		public static string WAR_HELP_BASEEDIT_REBEL
		{
			get;
			private set;
		}

		public static string WAR_HELP_BASEEDIT_EMPIRE
		{
			get;
			private set;
		}

		public static string WAR_HELP_PREPARATION_REBEL
		{
			get;
			private set;
		}

		public static string WAR_HELP_PREPARATION_EMPIRE
		{
			get;
			private set;
		}

		public static string WAR_HELP_WAR_REBEL
		{
			get;
			private set;
		}

		public static string WAR_HELP_WAR_EMPIRE
		{
			get;
			private set;
		}

		public static string WAR_HELP_REWARD_REBEL
		{
			get;
			private set;
		}

		public static string WAR_HELP_REWARD_EMPIRE
		{
			get;
			private set;
		}

		public static int WAR_NOTIF_ACTION_TURNS_REMINDER
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_REBEL_OPEN
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_REBEL_PREP
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_REBEL_ACTION
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_REBEL_COOLDOWN
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_EMPIRE_OPEN
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_EMPIRE_PREP
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_EMPIRE_ACTION
		{
			get;
			private set;
		}

		public static string HOLONET_TEXTURE_WAR_EMPIRE_COOLDOWN
		{
			get;
			private set;
		}

		public static string TFA_PLANET_UID
		{
			get;
			private set;
		}

		public static string HOTH_PLANET_UID
		{
			get;
			private set;
		}

		public static string ERKIT_PLANET_UID
		{
			get;
			private set;
		}

		public static string YAVIN_PLANET_UID
		{
			get;
			private set;
		}

		public static string DANDORAN_PLANET_UID
		{
			get;
			private set;
		}

		public static string TATOOINE_PLANET_UID
		{
			get;
			private set;
		}

		public static int WWW_MAX_RETRY
		{
			get;
			private set;
		}

		public static int PUBLISH_TIMER_DELAY
		{
			get;
			private set;
		}

		public static float PULL_FREQUENCY_CHAT_OPEN
		{
			get;
			private set;
		}

		public static float PULL_FREQUENCY_CHAT_CLOSED
		{
			get;
			private set;
		}

		public static int SQUAD_MESSAGE_LIMIT
		{
			get;
			private set;
		}

		public static int SQUADPERK_MAX_PERK_CARD_BADGES
		{
			get;
			private set;
		}

		public static int CRATE_EXPIRATION_WARNING_NOTIF
		{
			get;
			private set;
		}

		public static int CRATE_EXPIRATION_WARNING_NOTIF_MINIMUM
		{
			get;
			private set;
		}

		public static int CRATE_EXPIRATION_WARNING_TOAST
		{
			get;
			private set;
		}

		public static bool CRATE_DAILY_CRATE_ENABLED
		{
			get;
			private set;
		}

		public static int CRATE_DAILY_CRATE_NOTIF_OFFSET
		{
			get;
			private set;
		}

		public static int CRATE_INVENTORY_TO_STORE_LINK_SORT
		{
			get;
			private set;
		}

		public static string CRATE_INVENTORY_TO_STORE_LINK_CRATE_ASSET
		{
			get;
			private set;
		}

		public static float CRATE_FLYOUT_ITEM_AUTO_SELECT_DURATION
		{
			get;
			private set;
		}

		public static float CRATE_FLYOUT_ITEM_AUTO_SELECT_RESUME
		{
			get;
			private set;
		}

		public static int PLANET_REWARDS_ITEM_THROTTLE
		{
			get;
			private set;
		}

		public static string CRATE_DAY_OF_THE_WEEK_REWARD
		{
			get;
			private set;
		}

		public static bool MOBILE_CONNECTOR_ADS_ENABLED
		{
			get;
			private set;
		}

		public static string MOBILE_CONNECTOR_VIDEO_REWARD_CRATE
		{
			get;
			private set;
		}

		public static float DELAY_EPISODES_PLAY_TASK_STORY_SECONDS
		{
			get;
			private set;
		}

		public static int MOBILE_CONNECTOR_ADS_MIN_DAYS_INSTALL
		{
			get;
			private set;
		}

		public static int HOLONET_MAX_INCOMING_TRANSMISSIONS
		{
			get;
			private set;
		}

		public static int HOLONET_EVENT_MESSAGE_MAX_COUNT
		{
			get;
			private set;
		}

		public static float HOLONET_FEATURE_CAROUSEL_AUTO_SWIPE
		{
			get;
			private set;
		}

		public static bool HOLONET_FEATURE_SHARE_ENABLED
		{
			get;
			private set;
		}

		public static string IOS_PROMO_END_DATE
		{
			get;
			private set;
		}

		public static bool ENABLE_FACTION_ICON_UPGRADES
		{
			get;
			private set;
		}

		public static int OBJECTIVES_UNLOCKED
		{
			get;
			private set;
		}

		public static float CRATE_OUTLINE_OUTER
		{
			get;
			private set;
		}

		public static float CRATE_OUTLINE_INNER
		{
			get;
			private set;
		}

		public static int WAR_PREP_DURATION
		{
			get;
			private set;
		}

		public static int WAR_ACTION_DURATION
		{
			get;
			private set;
		}

		public static int WAR_ATTACK_COUNT
		{
			get;
			private set;
		}

		public static int WAR_PARTICIPANT_COUNT
		{
			get;
			private set;
		}

		public static int WAR_PARTICIPANT_MIN_LEVEL
		{
			get;
			private set;
		}

		public static int WAR_VICTORY_POINTS
		{
			get;
			private set;
		}

		public static string WARBOARD_LABEL_OFFSET
		{
			get;
			private set;
		}

		public static bool WAR_ALLOW_MATCHMAKING
		{
			get;
			private set;
		}

		public static bool WAR_ALLOW_SAME_FACTION_MATCHMAKING
		{
			get;
			private set;
		}

		private static string WAR_BUFF_BASE_HQ_LEVEL_MAP
		{
			get;
			set;
		}

		public static int[] WAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING
		{
			get;
			private set;
		}

		public static string SQUADPERK_TUTORIAL_ACTIVE_PREF
		{
			get;
			set;
		}

		public static int SQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD
		{
			get;
			set;
		}

		public static int SQUADPERK_DONATION_REPUTATION_AWARD
		{
			get;
			set;
		}

		public static string SQUADPERK_REPUTATION_MAX_LIMIT
		{
			get;
			set;
		}

		public static int NEW_PLAYER_REPUTATION_CAPACITY
		{
			get;
			set;
		}

		public static int CRATE_INVENTORY_EXPIRATION_TIMER_WARNING
		{
			get;
			private set;
		}

		public static int CRATE_INVENTORY_LEI_EXPIRATION_TIMER_WARNING
		{
			get;
			private set;
		}

		public static int CRYSTAL_STORE_SALE_EXPIRATION_TIMER_WARNING
		{
			get;
			private set;
		}

		public static bool CRATE_SHOW_VFX
		{
			get;
			set;
		}

		public static int EQUIPMENT_SHADER_DELAY
		{
			get;
			set;
		}

		public static int EQUIPMENT_SHADER_DELAY_REPLAY
		{
			get;
			set;
		}

		public static int EQUIPMENT_SHADER_DELAY_DEFENSE
		{
			get;
			set;
		}

		public static bool SEND_RESET_EVENT_ON_ENTITY_STOP_FLASHING
		{
			get;
			private set;
		}

		public static bool SAME_FACTION_MATCHMAKING_DEFAULT
		{
			get;
			set;
		}

		public static int MAX_SUMMONS_PER_BATTLE
		{
			get;
			private set;
		}

		public static bool ALLOW_SUMMON
		{
			get;
			private set;
		}

		public static bool PHOTON_CHAT_DISABLED
		{
			get;
			private set;
		}

		public static float PHOTON_CHAT_KEEP_ALIVE_TICK
		{
			get;
			private set;
		}

		public static string PHOTON_CHAT_APP_VERSION
		{
			get;
			private set;
		}

		public static int PHOTON_CHAT_HISTORY_LENGTH
		{
			get;
			private set;
		}

		public static bool PHOTON_CHAT_COMPLEX_COMPARE_ENABLED
		{
			get;
			private set;
		}

		public static int SHARD_SHOP_CREDITS_COEFFICIENT
		{
			get;
			private set;
		}

		public static int SHARD_SHOP_CREDITS_EXPONENT
		{
			get;
			private set;
		}

		public static int SHARD_SHOP_CONTRABAND_COEFFICIENT
		{
			get;
			private set;
		}

		public static int SHARD_SHOP_CONTRABAND_EXPONENT
		{
			get;
			private set;
		}

		public static int SHARD_SHOP_ALLOY_COEFFICIENT
		{
			get;
			private set;
		}

		public static int SHARD_SHOP_ALLOY_EXPONENT
		{
			get;
			private set;
		}

		public static int SHARD_SHOP_MINIMUM_HQ
		{
			get;
			private set;
		}

		public static void Initialize()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			PropertyInfo[] properties = typeof(GameConstants).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
			int i = 0;
			int num = properties.Length;
			while (i < num)
			{
				PropertyInfo propertyInfo = properties[i];
				string uid = propertyInfo.Name.ToLower();
				GameConstantsVO optional = staticDataController.GetOptional<GameConstantsVO>(uid);
				if (optional != null)
				{
					string value = optional.Value;
					propertyInfo.SetValue(null, Convert.ChangeType(value, propertyInfo.PropertyType), null);
				}
				i++;
			}
			staticDataController.Unload<GameConstantsVO>();
			GameConstants.InitRelocationCrystalsAndStarCost();
			GameConstants.InitWarBaseLevelMapping();
		}

		private static void InitWarBaseLevelMapping()
		{
			if (string.IsNullOrEmpty(GameConstants.WAR_BUFF_BASE_HQ_LEVEL_MAP))
			{
				Service.Logger.Error("GameConstants.WAR_BUFF_BASE_HQ_LEVEL_MAP is null or empty");
				return;
			}
			string[] array = GameConstants.WAR_BUFF_BASE_HQ_LEVEL_MAP.Split(new char[]
			{
				','
			});
			int num = array.Length;
			GameConstants.WAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING = new int[num];
			for (int i = 0; i < num; i++)
			{
				GameConstants.WAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING[i] = Convert.ToInt32(array[i]);
			}
		}

		private static void InitRelocationCrystalsAndStarCost()
		{
			if (string.IsNullOrEmpty(GameConstants.CRYSTALS_PER_RELOCATION_STAR))
			{
				Service.Logger.Error("GameConstants.CRYSTALS_PER_RELOCATION_STAR is null or empty");
				return;
			}
			string[] array = GameConstants.CRYSTALS_PER_RELOCATION_STAR.Split(new char[]
			{
				','
			});
			string[] array2 = GameConstants.STARS_PER_RELOCATION.Split(new char[]
			{
				','
			});
			int num = array.Length;
			if (num != array2.Length)
			{
				Service.Logger.Error("RelocationCrystalsAndStarCost Invalid Lengths");
				GameConstants.CrystalsPerRelocationStar = new int[0];
				GameConstants.StarsPerRelocation = new int[0];
				return;
			}
			GameConstants.CrystalsPerRelocationStar = new int[num];
			GameConstants.StarsPerRelocation = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2;
				int num3;
				if (!int.TryParse(array[i], out num2) || !int.TryParse(array2[i], out num3))
				{
					Service.Logger.Error("RelocationCrystalsAndStarCost Parse Error");
					GameConstants.CrystalsPerRelocationStar = new int[0];
					GameConstants.StarsPerRelocation = new int[0];
					return;
				}
				GameConstants.CrystalsPerRelocationStar[i] = num2;
				GameConstants.StarsPerRelocation[i] = num3;
			}
		}
	}
}
