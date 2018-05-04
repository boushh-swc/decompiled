using Midcore.Resources.ContentManagement;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class PlayerContentStartupTask : StartupTask
	{
		private const string SERVER_FAILED_MESSAGE = "The server was unable to deliver content initialization data. Please try again soon.";

		private const string CONTENT_FAILED_MESSAGE = "Content Manager failed to initialize.";

		private const string STRINGS_PATH_PREFIX = "strings/";

		private const int NUM_CONCURRENT_PATCHES = 3;

		private GetContentResponse response;

		private Catalog catalog;

		private int currentPatchIndex;

		private ContentManagerOptions options;

		private int numPatchesLoaded;

		public PlayerContentStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.SendEvent(EventId.InitialLoadStart, null);
			GetContentCommand getContentCommand = new GetContentCommand(new GetContentRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getContentCommand.AddSuccessCallback(new AbstractCommand<GetContentRequest, GetContentResponse>.OnSuccessCallback(this.OnCommandComplete));
			Service.ServerAPI.Async(getContentCommand);
		}

		private void OnCommandComplete(GetContentResponse response, object cookie)
		{
			this.response = response;
			this.options = new ContentManagerOptions();
			if (response.CdnRoots.Count > 0 && !string.IsNullOrEmpty(response.CdnRoots[0]) && !string.IsNullOrEmpty(response.ManifestVersion) && !string.IsNullOrEmpty(response.ManifestPath))
			{
				this.options.ManifestVersion = int.Parse(response.ManifestVersion);
				this.options.ManifestPath = response.ManifestPath;
				this.options.ContentBaseUrl = response.CdnRoots[0];
				this.options.Env = response.Environment;
				ContentManagerMode mode = ContentManagerMode.Remote;
				this.options.Mode = mode;
				this.LoadContentManager();
				Service.CurrentPlayer.Patches = response.Patches;
				return;
			}
			Service.Logger.Error("Content is not initialized");
			AlertScreen.ShowModalWithBI(true, null, "The server was unable to deliver content initialization data. Please try again soon.", "Connection error!");
		}

		private void OnTokenRequestComplete(string token)
		{
			this.options.AccessToken = token;
			this.LoadContentManager();
		}

		private void LoadContentManager()
		{
			ContentManager contentManager = new ContentManager(this.options);
			contentManager.Load(new ManagerLoadDelegate(this.OnContentManagerLoadSuccess), new ManagerLoadDelegate(this.OnContentManagerLoadFailure), Service.Engine.WebManager);
		}

		private void OnContentManagerLoadSuccess(ContentManager manager)
		{
			Service.EventManager.SendEvent(EventId.MetaDataLoadStart, null);
			this.catalog = new Catalog();
			this.currentPatchIndex = -1;
			this.numPatchesLoaded = 0;
			int num = 0;
			while (num < 3 && num < this.response.Patches.Count)
			{
				this.ProcessNextPatch();
				num++;
			}
		}

		private void OnContentManagerLoadFailure(ContentManager manager)
		{
			Service.Logger.Error("Content manager failed to initialize.");
			AlertScreen.ShowModalWithBI(true, null, "Content Manager failed to initialize.", "CONTENT ERROR");
		}

		private void VerifyCurrentPatch()
		{
			while (this.currentPatchIndex < this.response.Patches.Count)
			{
				string text = this.response.Patches[this.currentPatchIndex];
				if (!text.StartsWith("strings/"))
				{
					break;
				}
				this.currentPatchIndex++;
				this.numPatchesLoaded++;
			}
		}

		private void ProcessNextPatch()
		{
			this.currentPatchIndex++;
			this.VerifyCurrentPatch();
			if (this.numPatchesLoaded == this.response.Patches.Count)
			{
				this.OnAllPatchesComplete();
				return;
			}
			if (this.currentPatchIndex >= this.response.Patches.Count)
			{
				return;
			}
			this.catalog.PatchData(this.response.Patches[this.currentPatchIndex], new Catalog.CatalogDelegate(this.OnPatchComplete));
		}

		private void OnPatchComplete(bool success, string file)
		{
			this.numPatchesLoaded++;
			this.ProcessNextPatch();
		}

		private void SetupStaticDataController()
		{
			new ValueObjectController();
			StaticDataController staticDataController = new StaticDataController();
			staticDataController.Load<AudioTypeVO>(this.catalog, "AudioData");
			staticDataController.Load<BattleTypeVO>(this.catalog, "BattleData");
			staticDataController.Load<BuffTypeVO>(this.catalog, "BuffData");
			staticDataController.Load<ProjectileTypeVO>(this.catalog, "ProjectileData");
			staticDataController.Load<TroopAbilityVO>(this.catalog, "HeroAbilities");
			staticDataController.Load<BuildingConnectorTypeVO>(this.catalog, "BuildingConnectorData");
			staticDataController.Load<AchievementVO>(this.catalog, "AchievementData");
			staticDataController.Load<CommonAssetVO>(this.catalog, "CommonAssetData");
			staticDataController.Load<PlanetVO>(this.catalog, "PlanetData");
			staticDataController.Load<BuildingTypeVO>(this.catalog, "BuildingData");
			staticDataController.Load<TrapTypeVO>(this.catalog, "TrapData");
			staticDataController.Load<EffectsTypeVO>(this.catalog, "EffectsData");
			staticDataController.Load<ShaderTypeVO>(this.catalog, "ShaderData");
			staticDataController.Load<AssetTypeVO>(this.catalog, "AssetData");
			staticDataController.Load<UITypeVO>(this.catalog, "UIData");
			staticDataController.Load<GameConstantsVO>(this.catalog, "GameConstants");
			staticDataController.Load<ProfanityVO>(this.catalog, "Profanity");
			staticDataController.Load<TroopTypeVO>(this.catalog, "TroopData");
			staticDataController.Load<DefaultLightingVO>(this.catalog, "PlanetaryLighting");
			staticDataController.Load<CivilianTypeVO>(this.catalog, "CivilianData");
			staticDataController.Load<TransportTypeVO>(this.catalog, "TransportData");
			staticDataController.Load<SpecialAttackTypeVO>(this.catalog, "SpecialAttackData");
			staticDataController.Load<StoryActionVO>(this.catalog, "StoryActions");
			staticDataController.Load<StoryTriggerVO>(this.catalog, "StoryTriggers");
			staticDataController.Load<ConditionVO>(this.catalog, "Conditions");
			staticDataController.Load<DefenseEncounterVO>(this.catalog, "DefenseEncounters");
			staticDataController.Load<CampaignVO>(this.catalog, "CampaignData");
			staticDataController.Load<CampaignMissionVO>(this.catalog, "CampaignMissionData");
			staticDataController.Load<CharacterVO>(this.catalog, "CharacterData");
			staticDataController.Load<TextureVO>(this.catalog, "TextureData");
			staticDataController.Load<TournamentVO>(this.catalog, "TournamentData");
			staticDataController.Load<TournamentTierVO>(this.catalog, "TournamentTierData");
			staticDataController.Load<NotificationTypeVO>(this.catalog, "Notifications");
			staticDataController.Load<InAppPurchaseTypeVO>(this.catalog, "InAppPurchases");
			staticDataController.Load<SaleItemTypeVO>(this.catalog, "SaleItems");
			staticDataController.Load<SaleTypeVO>(this.catalog, "Sales");
			staticDataController.Load<RewardVO>(this.catalog, "RewardData");
			staticDataController.Load<TournamentRewardsVO>(this.catalog, "TournamentRewards");
			staticDataController.Load<TurretTypeVO>(this.catalog, "TurretData");
			staticDataController.Load<EncounterProfileVO>(this.catalog, "DefenseEncountersProfiles");
			staticDataController.Load<MobilizationHologramVO>(this.catalog, "MobilizationHologram");
			staticDataController.Load<BattleScriptVO>(this.catalog, "BattleScripts");
			staticDataController.Load<DevNoteEntryVO>(this.catalog, "DevNotes");
			staticDataController.Load<CommandCenterVO>(this.catalog, "CommandCenterEntries");
			staticDataController.Load<TransmissionVO>(this.catalog, "Transmissions");
			staticDataController.Load<TransmissionCharacterVO>(this.catalog, "TransmissionCharacters");
			staticDataController.Load<LimitedTimeRewardVO>(this.catalog, "LimitedTimeRewards");
			staticDataController.Load<IconUpgradeVO>(this.catalog, "FactionIcons");
			staticDataController.Load<ObjectiveVO>(this.catalog, "ObjTable");
			staticDataController.Load<ObjectiveSeriesVO>(this.catalog, "ObjSeries");
			staticDataController.Load<CrateTierVO>(this.catalog, "CrateTiers");
			staticDataController.Load<DataCardTierVO>(this.catalog, "DataCardTiers");
			staticDataController.Load<CrateFlyoutItemVO>(this.catalog, "CrateFlyoutItem");
			staticDataController.Load<CurrencyIconVO>(this.catalog, "CurrencyType");
			staticDataController.Load<RaidMissionPoolVO>(this.catalog, "RaidMissionPool");
			staticDataController.Load<RaidVO>(this.catalog, "Raid");
			staticDataController.Load<WarBuffVO>(this.catalog, "WarBuffData");
			staticDataController.Load<WarScheduleVO>(this.catalog, "WarSchedule");
			staticDataController.Load<SquadLevelVO>(this.catalog, "SquadLevel");
			staticDataController.Load<PerkEffectVO>(this.catalog, "PerkEffects");
			staticDataController.Load<PerkVO>(this.catalog, "Perks");
			staticDataController.Load<TargetedBundleVO>(this.catalog, "SpecialPromo");
			staticDataController.Load<EquipmentVO>(this.catalog, "EquipmentData");
			staticDataController.Load<EquipmentEffectVO>(this.catalog, "EquipmentEffectData");
			staticDataController.Load<SkinOverrideTypeVO>(this.catalog, "SkinOverrideData");
			staticDataController.Load<SkinTypeVO>(this.catalog, "SkinData");
			staticDataController.Load<CrateSupplyVO>(this.catalog, "CrateSupply");
			staticDataController.Load<CrateVO>(this.catalog, "Crate");
			staticDataController.Load<CrateSupplyScaleVO>(this.catalog, "CrateSupplyScale");
			staticDataController.Load<ShardVO>(this.catalog, "Shard");
			staticDataController.Load<PlanetLootVO>(this.catalog, "PlanetLoot");
			staticDataController.Load<PlanetLootEntryVO>(this.catalog, "PlanetLootEntry");
			staticDataController.Load<LimitedEditionItemVO>(this.catalog, "LimitedEditionItemData");
			staticDataController.Load<SummonDetailsVO>(this.catalog, "SummonDetails");
			staticDataController.Load<VFXProfileVO>(this.catalog, "VFXProfileData");
			staticDataController.Load<PlanetAttachmentVO>(this.catalog, "PlanetAttachmentData");
			staticDataController.Load<EpisodeDataVO>(this.catalog, "EpisodeData");
			staticDataController.Load<EpisodeTaskVO>(this.catalog, "EpisodeTask");
			staticDataController.Load<EpisodeTaskActionVO>(this.catalog, "EpisodeTaskAction");
			staticDataController.Load<EpisodeTaskScaleVO>(this.catalog, "EpisodeTaskScale");
			staticDataController.Load<EpisodePointScheduleVO>(this.catalog, "EpisodePointSchedule");
			staticDataController.Load<EpisodePointScaleVO>(this.catalog, "EpisodePointScale");
			staticDataController.Load<EpisodePanelVO>(this.catalog, "EpisodePanel");
			staticDataController.Load<EpisodeWidgetDataVO>(this.catalog, "EpisodeWidgetData");
			staticDataController.Load<EpisodeWidgetStateVO>(this.catalog, "EpisodeWidgetState");
			staticDataController.Load<TroopUniqueAbilityDescVO>(this.catalog, "UISupplemental");
			staticDataController.Load<ShardShopSeriesVO>(this.catalog, "ShardShopSeries");
			staticDataController.Load<ShardShopPoolVO>(this.catalog, "ShardShopPool");
			staticDataController.Load<CostVO>(this.catalog, "Cost");
			staticDataController.Load<StickerVO>(this.catalog, "Stickers");
			this.catalog = null;
		}

		private void OnAllPatchesComplete()
		{
			this.SetupStaticDataController();
			new AchievementController();
			GameConstants.Initialize();
			Service.ServerAPI.SetKeepAlive(new KeepAliveCommand(new KeepAliveRequest()), GameConstants.KEEP_ALIVE_DISPATCH_WAIT_TIME);
			Service.EventManager.SendEvent(EventId.MetaDataLoadEnd, null);
			Service.LeaderboardController.InitLeaderBoardListForPlanet();
			Service.CurrentPlayer.DoPostContentInitialization();
			base.Complete();
		}
	}
}
