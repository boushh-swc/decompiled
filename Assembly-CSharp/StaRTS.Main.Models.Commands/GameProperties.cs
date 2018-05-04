using StaRTS.Externals.Manimal.TransferObjects;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class GameProperties : Properties
	{
		public const string ACCESS_TOKEN = "accessToken";

		public const string ACTION_END_TIME = "actionEndTime";

		public const string ACTION_GRACE_START_TIME = "actionGraceStartTime";

		public const string ACTION_UID = "actionUID";

		public const string ACTIVE_ARMORY = "activeArmory";

		public const string AD_UNIT_ID = "auid";

		public const string AMOUNT = "amount";

		public const string APP_CODE = "appCode";

		public const string ARMORY_INFO = "armoryInfo";

		public const string ATTACK_EXPIRATION_DATE = "attackExpirationDate";

		public const string ATTACKER_ID = "attackerId";

		public const string ATTACKER = "attacker";

		public const string ATTACKER_GUILD_TROOPS_SPENT = "attackerGuildTroopsSpent";

		public const string ATTACK_RATING = "attackRating";

		public const string ATTACKER_EQUIPMENT = "attackerEquipment";

		public const string ATTACKER_WAR_BUFFS = "attackerWarBuffs";

		public const string POTENTIAL_MEDAL_COUNT = "potentialMedalGain";

		public const string GUILD_TROOPS_SPENT = "guildTroopsSpent";

		public const string BATTLE = "battle";

		public const string BATTLE_ID = "battleId";

		public const string BATTLE_UID = "battleUid";

		public const string BATTLE_HISTORY = "battleLogs";

		public const string BATTLE_LOG = "battleLog";

		public const string BATTLE_SCRIPT = "battleScript";

		public const string BATTLE_TYPE = "battleType";

		public const string BASE_DAMAGE_PERCENT = "baseDamagePercent";

		public const string NUM_VISITORS = "numVisitors";

		public const string EVENT_2_BI = "event2BiLogging";

		public const string EVENT_2_NO_PROXY_BI = "event2NoProxyBiLogging";

		public const string SQUAD_ATTACKER_TROOPS_EXPENDED = "attackerGuildTroopsExpended";

		public const string SQUAD_DEFENDER_TROOPS_EXPENDED = "defenderGuildTroopsExpended";

		public const string TROOPS_EXPENDED = "troopsExpended";

		public const string BASE_LEVEL = "baseLvl";

		public const string ATTACK_RATING_DELTA = "attackRatingDelta";

		public const string DEFENSE_RATING_DELTA = "defenseRatingDelta";

		public const string ATTACKER_DEPLOYABLES = "attackerDeployables";

		public const string ATTACKER_TOURNAMENT = "attackerTournament";

		public const string ACCEPTOR = "acceptor";

		public const string ACCOUNT = "account";

		public const string AMOUNT_REMAINING = "amountRemaining";

		public const string ATTACKS_WON = "attacksWon";

		public const string CREATED = "created";

		public const string DATE = "date";

		public const string DEFENSES_WON = "defensesWon";

		public const string DESC = "description";

		public const string DONATED_TROOPS = "donatedTroops";

		public const string FRIEND_IDS = "friendIds";

		public const string GUILD_ID = "guildId";

		public const string GUILD_INFO = "guildInfo";

		public const string GUILD_LEVEL = "level";

		public const string GUILD_NAME = "guildName";

		public const string ICON = "icon";

		public const string ID = "id";

		public const string ID_FEATURED = "_id";

		public const string IS_OFFICER = "isOfficer";

		public const string IS_OWNER = "isOwner";

		public const string JOIN_DATE = "joinDate";

		public const string MANIMAL = "manimal";

		public const string MAX_SIZE = "maxSize";

		public const string MEMBER_COUNT = "memberCount";

		public const string ACTIVE_MEMBER_COUNT = "activeMemberCount";

		public const string WAR_HISTORY = "warHistory";

		public const string MEMBER_ID = "memberId";

		public const string MEMBERS = "members";

		public const string MEMBERSHIP_RESTRICT = "membershipRestrictions";

		public const string MESSAGE = "message";

		public const string MIN_SCORE = "minScore";

		public const string MIN_TROPHY = "minScoreAtEnrollment";

		public const string NAME = "name";

		public const string OPEN_ENROLLMENT = "openEnrollment";

		public const string OPPONENT_NAME = "opponentName";

		public const string PAY_TO_SKIP = "payToSkip";

		public const string PERCENTILE = "percentile";

		public const string RANK = "rank";

		public const string HIGHEST_RANK = "highestRankAchieved";

		public const string RECIPIENT_ID = "recipientId";

		public const string SCORE = "score";

		public const string SEARCH_TERM = "searchTerm";

		public const string SINCE = "since";

		public const string SNID = "snid";

		public const int SQUAD_OPEN = 1;

		public const int SQUAD_PRIVATE = 0;

		public const string TO_RANK = "toRank";

		public const string NEW_RANK = "newRank";

		public const string TOTAL_CAPACITY = "totalCapacity";

		public const string TROOPS_DONATED = "troopsDonated";

		public const string TROOPS_RECEIVED = "troopsReceived";

		public const string REPUTATION_INVESTED = "reputationInvested";

		public const string SQUAD_ATTACKS_WON = "attacksWon";

		public const string SQUAD_DEFENSES_WON = "defensesWon";

		public const string SQUAD_TOURNAMENT_SCORE = "tournamentScores";

		public const string SQUAD_LAST_LOGIN_TIME = "lastLoginTime";

		public const string LAST_TROOP_REQUEST_TIME = "lastTroopRequestTime";

		public const string LAST_WAR_TROOP_REQUEST_TIME = "lastWarTroopRequestTime";

		public const string BATTLE_VERSION = "battleVersion";

		public const string BUFF_BASE_UID = "buffBaseUid";

		public const string BUILDING = "building";

		public const string BUILDINGS = "buildings";

		public const string BUILDING_ID = "buildingId";

		public const string BUILDING_IDS = "buildingIds";

		public const string BUILDING_CONTRACT_UID = "buildingContractUid";

		public const string BUILDING_TYPE = "buildingType";

		public const string BUILDING_UID = "buildingUid";

		public const string BUILDING_UIDS = "buildingUids";

		public const string BUILDING_START_TIME = "buildingStartTime";

		public const string CAMPAIGN_UID = "campaignUid";

		public const string CAMPAIGNS = "campaigns";

		public const string CAPACITY = "capacity";

		public const string CAPTURED = "captured";

		public const string CLAIMED = "claimed";

		public const string CLIENT_VERSION = "clientVersion";

		public const string CMS_VERSION = "cmsVersion";

		public const string CODE = "code";

		public const string COLLECTED = "collected";

		public const string COMPLETED = "completed";

		public const string CONSTRUCTOR = "constructor";

		public const string CONTENT = "content";

		public const string CONTRACT_UIDS = "contractUids";

		public const string CONTRACTS = "contracts";

		public const string COOLDOWN_END_TIME = "cooldownEndTime";

		public const string REWARDS_PROCESSED = "rewardsProcessed";

		public const string COUNTERS = "counters";

		public const string CS = "cs";

		public const string CURRENT_BATTLE = "currentBattle";

		public const string CURRENT_CONTRACT_UID = "currentContractUid";

		public const string CURRENT_QUEST = "currentQuest";

		public const string CURRENT_STORAGE = "currentStorage";

		public const string COUNT = "count";

		public const string CHECKSUM = "cs";

		public const string CURRENCY = "currency";

		public const string CURRENT_TASK_INDEX = "currentTaskIndex";

		public const string CURRENT_TASK_UID = "currentTaskUid";

		public const string CURRENT_TASK_ACTION_UID = "currentTaskActionUid";

		public const string CURRENT_EPISODE_UID = "currentEpisodeUid";

		public const string CREDITS = "credits";

		public const string CONTRABAND = "contraband";

		public const string CRYSTALS = "crystals";

		public const string REPUTATION = "reputation";

		public const string CURRENTLY_DEFENDING = "currentlyDefending";

		public const string CURRENTLY_DEFENDING_EXPIRE_TIME = "expiration";

		public const string IAP_UID = "iapUid";

		public const string LAST_PAYMENT_TIME = "lastPaymentTime";

		public const string DELTA_BATTLE_SCORE = "battleScoreDelta";

		public const string DAMAGE_PERCENT = "damagePercent";

		public const string DEFENDER_ID = "defenderId";

		public const string DEFENDER = "defender";

		public const string DEFENDER_GUILD_TROOPS_SPENT = "defenderGuildTroopsSpent";

		public const string DEFENSE_ENCOUNTER_PROFILE = "defenseEncounterProfile";

		public const string DEFENSE_RATING = "defenseRating";

		public const string DEFENDER_EQUIPMENT = "defenderEquipment";

		public const string DEFENDER_WAR_BUFFS = "defenderWarBuffs";

		public const string DEATH_LOG = "deathLog";

		public const string DEPLOYED_TROOPS = "deployedTroops";

		public const string DEPLOYED_HEROES = "deployedHeroes";

		public const string DEPLOYED_SPECIAL_ATTACKS = "deployedSpecialAttacks";

		public const string DAMAGED_BUILDINGS = "damagedBuildings";

		public const string DAMAGED_BUILDINGS_PLAYER = "DamagedBuildings";

		public const string DEFENDING_UNITS_KILLED = "defendingUnitsKilled";

		public const string ATTACKING_UNITS_KILLED = "attackingUnitsKilled";

		public const string DERIVED_EXTERNAL_ACCOUNT_ID = "derivedExternalAccountId";

		public const string EXTERNAL_ACCOUNT_REWARD = "registrationReward";

		public const string DEVICE_ID = "deviceId";

		public const string DEVICE_ID_TYPE = "deviceIdType";

		public const string DEVICE_TOKEN = "deviceToken";

		public const string DEVICE_TYPE = "deviceType";

		public const string DEVICE_INFO = "deviceInfo";

		public const string DIFFICULTY = "difficulty";

		public const string DONOR_ID = "donorId";

		public const string DURATION = "duration";

		public const string EARNED = "earned";

		public const string EARNED_STARS = "earnedStars";

		public const string EMPIRE_NAME = "empireName";

		public const string EMPIRE_SCORE = "empireScore";

		public const string END_TIME = "endTime";

		public const string END_TIME_SECS = "endTimeSecs";

		public const string ENVIRONMENT = "environment";

		public const string EPISODE_PROGRESS_INFO = "episodeProgressInfo";

		public const string EPISODE_TASK_PROGRESS_INFO = "currentTask";

		public const string EPISODE_TASK_INTRO_STORY_VIEWED = "introStoryViewed";

		public const string EQUIPMENT = "equipment";

		public const string EQUIPMENT_ID = "equipmentId";

		public const string EQUIPMENT_UID = "equipmentUid";

		public const string SHARDS_TO_INVEST = "shardsToInvest";

		public const string EVENT = "event";

		public const string EXPIRATION = "expiration";

		public const string EXTRA_PARAMS = "extraParams";

		public const string EQUIPMENT_LEVEL = "equipmentLevel";

		public const string FACTION = "faction";

		public const string FAILED = "failed";

		public const string FB_FRIEND_ID = "fbFriendId";

		public const string FINISHED_TASKS = "finishedTasks";

		public const string FIRST_CRATE_PURCHASED = "firstCratePurchased";

		public const string FIRST_TIME_PLAYER = "firstTimePlayer";

		public const string FROZEN_BUILDINGS = "frozenBuildings";

		public const string FUE_ID = "FueUid";

		public const string FUE_IN_PROGRESS = "isFueInProgress";

		public const string GENERATOR = "generator";

		public const string SEEDED_TROOPS_DEPLOYED = "seededTroopsDeployed";

		public const string GRIND_MISSION_RETRIES = "grindMissionRetries";

		public const string GENERATE_NEW_LIST = "generateNew";

		public const string GRACE_TIME = "graceTime";

		public const string GRIND = "grind";

		public const string GROUPS = "groups";

		public const string GUILD_TROOPS = "guildTroops";

		public const string HERO = "hero";

		public const string HERO_LIST = "heroList";

		public const string HQ = "hq";

		public const string HQ_LEVEL = "hqLevel";

		public const string CHAMPION = "champion";

		public const string CHAMPIONS = "champions";

		public const string CHAMPION_LIST = "championList";

		public const string IDENTITIES = "identities";

		public const string IDENTITY_INDEX = "identityIndex";

		public const string IDENTITY_SWITCH_TIMES = "identitySwitchTimes";

		public const string INSTANCE_ID = "instanceId";

		public const string INTERNAL_STORAGE = "storage";

		public const string INTROS = "intros";

		public const string INVENTORY = "inventory";

		public const string IS_CONNECTED_ACCOUNT = "isConnectedAccount";

		public const string IS_RATE_INCENTIVIZED = "isRateIncentivized";

		public const string IS_PUSH_INCENTIVIZED = "pushRewarded";

		public const string IS_USER_ENDED = "isUserEnded";

		public const string ITEMS = "items";

		public const string LEADERS = "leaders";

		public const string SURROUNDING = "surrounding";

		public const string KEY = "key";

		public const string LIVENESS = "liveness";

		public const string LAST_COLLECT_TIME = "lastCollectTime";

		public const string LAST_START_TIME = "lastStartTime";

		public const string LE_UID = "leUid";

		public const string LEVEL = "level";

		public const string INSTALLDATE = "installDate";

		public const string AVAILABLE = "available";

		public const string IN_PROGRESS = "inProgress";

		public const string LOCALE_PREFERENCE = "locale";

		public const string FACEBOOK_ID = "facebookId";

		public const string FACEBOOK_AUTH_TOKEN = "facebookAuthToken";

		public const string LOCKED = "locked";

		public const string LOOT_CREDITS_EARNED = "lootCreditsEarned";

		public const string LOOT_CONTRABAND_EARNED = "lootContrabandEarned";

		public const string LOOT = "loot";

		public const string LOOT_MATERIALS_EARNED = "lootMaterialsEarned";

		public const string LOOT_REMAINING = "lootRemaining";

		public const string LOOTED = "looted";

		public const string CLIENT_PREFS = "clientPrefs";

		public const string MANIFEST = "manifest";

		public const string MANIFEST_VERSION = "manifestVersion";

		public const string MAP = "map";

		public const string MATERIALS = "materials";

		public const string MAX_LOOTABLE = "maxLootable";

		public const string MISSIONS = "missions";

		public const string MISSION_UID = "missionUid";

		public const string MOBILE_CONNECTOR_ADS_INFO = "mcaInfo";

		public const string MODEL = "model";

		public const string RAID_MISSION_UID = "raidMissionId";

		public const string RAID_POOL_ID = "raidPoolId";

		public const string RAID_ID = "raidId";

		public const string LAST_RAID_STARS = "lastRaidStars";

		public const string LAST_RAID_ID = "lastRaidId";

		public const string LAST_RAID_POOL_ID = "lastRaidPoolId";

		public const string LAST_RAID_MISSION_ID = "lastRaidMissionId";

		public const string LAST_RAID_CRATE_REWARD = "lastCrateReward";

		public const string LAST_REWARD_DATE = "lastRewardDate";

		public const string NEXT_RAID_START_TIME = "nextRaidStartTime";

		public const string RAIDS = "raids";

		public const string RAID_DATA = "raidData";

		public const string WAVE_ID = "waveId";

		public const string NEIGHBOR_ID = "neighborId";

		public const string NEXT_AVAILABLE_DATE = "nextAvailableDate";

		public const string OVERRIDE = "override";

		public const string OPPONENT_ID = "opponentId";

		public const string OPPONENT_FACTION = "opponentFaction";

		public const string OS = "os";

		public const string OS_VERSION = "osVersion";

		public const string OTHER_LINKED_PROVIDER_ID = "otherLinkedProviderId";

		public const string PARTICIPANT_ID = "participantId";

		public const string PATCH = "patch";

		public const string PATCHES = "patches";

		public const string PAY_WITH_HARD_CURRENCY = "payWithHardCurrency";

		public const string PLANET_ID = "planetId";

		public const string PLANET_UID = "planetUid";

		public const string PLANET_RELOCATE_ID = "planet";

		public const string PLANET_STATS_ID = "planets";

		public const string UNLOCKED_PLANETS = "unlockedPlanets";

		public const string HOLONET_REWARDS = "holonetRewards";

		public const string RELOCATION_START_COUNT = "relocationStarCount";

		public const string PURCHASE_CONTEXT = "purchaseContext";

		public const string MISSION_ID = "missionId";

		public const string PLATFORM = "platform";

		public const string PLAYER = "player";

		public const string PLAYER_MODEL = "playerModel";

		public const string PLAYER_NAME = "playerName";

		public const string PLAYER_OBJECTIVES = "playerObjectives";

		public const string OBJECTIVE_ID = "objectiveId";

		public const string POINTS = "points";

		public const string REWARD = "reward";

		public const string POSITION = "position";

		public const string POSITIONS = "positions";

		public const string PREP_END_TIME = "prepEndTime";

		public const string PREP_GRACE_START_TIME = "prepGraceStartTime";

		public const string PRICE = "price";

		public const string PRIZES = "prizes";

		public const string PROTECTION = "protection";

		public const string PROTECTED_UNTIL = "protectedUntil";

		public const string PROTECTION_FROM = "protectionFrom";

		public const string PROTECTION_COOLDOWN_UNTIL = "protectionCooldownUntil";

		public const string PRODUCT_ID = "productId";

		public const string PROGRESS = "progress";

		public const string QUANTITY = "quantity";

		public const string QUEST_TRIGGERS = "triggers";

		public const string RATING = "rating";

		public const string REBEL_NAME = "rebelName";

		public const string REBEL_SCORE = "rebelScore";

		public const string RECEIPT = "receipt";

		public const string RECIPIENT_PLAYER_ID = "recipientPlayerId";

		public const string RECORDS = "records";

		public const string RECORD_ID = "battleId";

		public const string REDEEMED_REWARDS = "redeemedRewards";

		public const string REGISTRATION_TIME = "registrationTime";

		public const string REJECTOR = "rejector";

		public const string REPLAY_DATA = "replayData";

		public const string RESOURCES = "resources";

		public const string REVENGED = "revenged";

		public const string REWARD_CONTEXT = "rewardContext";

		public const string REWARDS = "rewards";

		public const string BEST_TIER = "bestTier";

		public const string SECRET = "secret";

		public const string SECURED_CDN_ROOTS = "secureCdnRoots";

		public const string SENDER_ID = "senderId";

		public const string SENDER_NAME = "senderName";

		public const string SESSION_COUNT_TODAY = "sessionCountToday";

		public const string SCALE = "scale";

		public const string SCALARS = "scalars";

		public const string SESSION_ID = "sessionId";

		public const string SHARD_ACTIVE_SERIES_ID = "activeSeriesId";

		public const string SHARD_OFFERS = "shardOffers";

		public const string SHARD_SERIES_ID = "seriesId";

		public const string SHARD_SHOP_DATA = "shardShopData";

		public const string SHARD_OFFER_EXPIRATION = "offerExpiration";

		public const string SHARD_POOL_ID = "poolSlotId";

		public const string SHARD_POOL_PREFIX = "pool_";

		public const string SHARD_OFFSET_MINUTES = "offsetMinutes";

		public const string SHARDS = "shards";

		public const string SHARED_PREFS = "sharedPrefs";

		public const string SPECIAL_ATTACK = "specialAttack";

		public const string SPECIAL_ATTACK_LIST = "specialAttackList";

		public const string GROUP_ID = "groupId";

		public const string START_TIME = "startTime";

		public const string NEXT_START_TIME = "nextRaidStartTime";

		public const string ATTACK_DATE = "attackDate";

		public const string STARTED = "started";

		public const string STATE = "state";

		public const string STATUS_CODE = "statusCode";

		public const string STATUS_REASON = "statusReason";

		public const string SUBSTORAGE = "subStorage";

		public const string SUBTYPE = "subtype";

		public const string SUPPORT = "support";

		public const string STARS = "stars";

		public const string AB_TESTS = "abTests";

		public const string TAG = "tag";

		public const string TARGET = "target";

		public const string TARGET_ID = "targetId";

		public const string TEXT = "text";

		public const string TIER = "tier";

		public const string TIME_LEFT = "timeLeft";

		public const string TIME_ZONE = "timeZone";

		public const string TIME_ZONE_OFFSET = "timeZoneOffset";

		public const string TIMESTAMP = "timestamp";

		public const string TOTAL_STORAGE_CAPACITY = "capacity";

		public const string TROOP = "troop";

		public const string TROOP_LIST = "troopList";

		public const string TOURNAMENT = "tournament";

		public const string TOURNAMENTS = "tournaments";

		public const string TOURNAMENT_UID = "tournamentId";

		public const string TOURNAMENT_RATING = "tournamentRating";

		public const string TOURNAMENT_RATING_DELTA = "tournamentRatingDelta";

		public const string TROOP_UID = "troopUid";

		public const string TROOP_START_TIME = "troopStartTime";

		public const string TRIGGERS = "triggers";

		public const string TYPE = "type";

		public const string UID = "uid";

		public const string UIDS = "uids";

		public const string UNARMED_TRAPS = "unarmedTraps";

		public const string UNIT_TYPE_ID = "unitTypeId";

		public const string UNITS = "units";

		public const string UPGRADES = "upgrades";

		public const string USER_NAME = "userName";

		public const string OFFSET = "offset";

		public const string PARAM_ATTACKS_WON = "attacksWon";

		public const string PARAM_ATTACKS_LOST = "attacksLost";

		public const string PARAM_DEFENSES_WON = "defensesWon";

		public const string PARAM_DEFENSES_LOST = "defensesLost";

		public const string VALUE = "value";

		public const string VENDOR_KEY = "vendorKey";

		public const string WAR_ID = "warId";

		public const string WAR_MAP = "warMap";

		public const string WON = "won";

		public const string SQUAD_WAR_LAST_PARTICIPATION_TIME = "lastWarParticipationTime";

		public const string WAR_REWARD_CRATE_TIER = "crateTier";

		public const string WAR_REWARD_CRATE_ID = "crateId";

		public const string WAR_REWARD_EXPIRATION_DATE = "expiry";

		public const string WAR_REWARD_END_MSG_EMPIRE_CRATE_TIER = "empireCrateTier";

		public const string WAR_REWARD_END_MSG_REBEL_CRATE_TIER = "rebelCrateTier";

		public const string WAR_REWARD_END_MSG_EMPIRE_CRATE_ID = "empireCrateId";

		public const string WAR_REWARD_END_MSG_REBEL_CRATE_ID = "rebelCrateId";

		public const string X = "x";

		public const string XP = "xp";

		public const string Z = "z";

		public const string SIM_SEED_A = "simSeedA";

		public const string SIM_SEED_B = "simSeedB";

		public const string ACCOUNT_PROVIDER = "providerId";

		public const string EXTERNAL_ACCOUNT_ID = "externalAccountId";

		public const string EXTERNAL_ACCOUNT_SECURITY_TOKEN = "externalAccountSecurityToken";

		public const string OVERRIDE_EXISTING_ACCOUNT_REGISTRATION = "overrideExistingAccountRegistration";

		public const string PLAYER_GUILD = "guild";

		public const string RIVAL_GUILD = "rival";

		public const string BUFF_BASES = "buffBases";

		public const string PARTICIPANTS = "participants";

		public const string TURNS = "turns";

		public const string VICTORYPOINTS = "victoryPoints";

		public const string PLAYERID = "playerId";

		public const string OWNER_ID = "ownerId";

		public const string BUFF_UID = "buffUid";

		public const string UNDERATTACK = "underAttack";

		public const string CURRENT_WAR_ID = "currentWarId";

		public const string CHAT_MESSAGE_ENCRYPTION_KEY = "chatMessageEncryptionKey";

		public const string WAR_SIGN_UP_TIME = "warSignUpTime";

		public const string IS_SAME_FACTION_WAR_ALLOWED = "isSameFactionWarAllowed";

		public const string WAR_PARTY = "warParty";

		public const string PARTICIPANT_IDS = "participantIds";

		public const string WAR_PLAYER_SCOUT_STATUS = "scoutingStatus";

		public const string OPPONENT_ICON = "opponentIcon";

		public const string END_DATE = "endDate";

		public const string OPPONENT_SCORE = "opponentScore";

		public const string AVAIALBLE_OFFER = "availableOffer";

		public const string GLOBAL_COOLDOWN_EXPIRES_AT = "globalCooldownExpiresAt";

		public const string NEXT_OFFER_AVAILABLE_AT = "nextOfferAvailableAt";

		public const string OFFER_UID = "offerUid";

		public const string OPEN_OFFER = "openOffer";

		public const string TRIGGER_DATE = "triggerDate";

		public const string TARGETED_OFFER = "targetedOffer";

		public const string TARGETED_OFFER_RESULT = "targetedOfferResult";

		public const string OFFER_PRODUCT_ID = "offerProductId";

		public const char HOLONET_TRANSMISSION_MULTIPART_DATA_SEPARATOR = '\\';

		public const string MEDAL_COUNT = "medalCount";

		public const string PERKS_INFO = "perksInfo";

		public const string TROOP_DONATION_PROGRESS = "troopDonationProgress";

		public const string DONATION_COUNT = "donationCount";

		public const string LAST_TRACKED_DONATION_TIME = "lastTrackedDonationTime";

		public const string PERKS = "perks";

		public const string ACTIVATED_PERKS = "activatedPerks";

		public const string COOLDOWNS = "cooldowns";

		public const string HAS_ACTIVATED_FIRST_PERK = "hasActivatedFirstPerk";

		public const string PERK_ID = "perkId";

		public const string PERK_IDS = "perkIds";

		public const string PERK_INVEST_AMT = "perkInvestAmt";

		public const string PERK_DEACTIVATE = "deactivate";

		public const string GENERATOR_EFFECT_TYPE = "generator";

		public const string CONTRACT_COST_EFFECT_TYPE = "contractCost";

		public const string CONTRACT_TIME_EFFECT_TYPE = "contractTime";

		public const string TROOP_DONATION_LIMIT = "troopDonationLimit";

		public const string TROOP_REQUEST_EFFECT_TYPE = "troopRequestTime";

		public const string RELOCATION_COST_EFFECT_TYPE = "relocation";

		public const string INVESTED_REP = "investedRep";

		public const string SQUAD_LEVEL_PREFIX = "SquadLevel";

		public const string TOTAL_REP_INVESTED = "totalRepInvested";

		public const string REP_TO_INVEST = "repToInvest";

		public const string REPUTATION_AWARDED = "reputationAwarded";

		public const string REP_DONATION_COOLDOWN_END = "repDonationCooldownEndTime";

		public const string CRATES = "crates";

		public const string CRATE_ID = "crateId";

		public const string CONTEXT = "context";

		public const string PLANET = "planet";

		public const string RECEIVED = "received";

		public const string NEXT = "next";

		public const string CRATE_UID = "crateUid";

		public const string EXPIRES_IN_MIN = "expiresInMinutes";

		public const string AWARDED_CRATE_UID = "awardedCrateUid";

		public const string CRATE_DATA = "crateData";

		public const string SUPPLY_ID = "supplyId";

		public const string SUPPLY_POOL_ID = "supplyPoolId";

		public const string RESOLVED_SUPPLIES = "resolvedSupplies";

		public const string NEXT_DAILY_CRATE_TIME = "nextDailyCrateTime";

		public const string REWARD_HOUR = "rewardHour";

		public const string REWARD_MINUTE = "rewardMinute";
	}
}
