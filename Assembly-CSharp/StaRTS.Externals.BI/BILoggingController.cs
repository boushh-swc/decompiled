using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Performance;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace StaRTS.Externals.BI
{
	public class BILoggingController : IEventObserver
	{
		private const string TWO_VAR_STR = "{0}|{1}";

		private const string THREE_VAR_STR = "{0}|{1}|{2}";

		private const string NO_PLAYER_ID = "NO_PLAYER_ID";

		private uint sampleDelayTimerID;

		private const string REASON_LOW_FPS = "Low FPS";

		private const string REASON_ERROR = "Critical error";

		private const string REASON_WARNING = "Severe warning";

		private DateTime epochDate;

		private MonoBehaviour engine;

		private string locale = string.Empty;

		private IDeviceInfoController deviceInfoController;

		private BIFrameMonitor biFrameMonitor;

		private int pageLoadStepCounter;

		private int iapActionCounter;

		private BILog log;

		private StepTimingController stepTiming;

		private Event2LogCreator event2LogCreator;

		public BILoggingController()
		{
			Service.BILoggingController = this;
			this.Initialize();
		}

		public void Initialize()
		{
			this.event2LogCreator = new Event2LogCreator(AppServerEnvironmentController.GetEvent2ClientBILoggingUrl(), "https://di-dtaectolog-us-prod-1.appspot.com/events2/mob");
			this.engine = Service.Engine;
			this.epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			this.log = new BILog();
			this.stepTiming = new StepTimingController();
			this.biFrameMonitor = new BIFrameMonitor();
			EventManager eventManager = Service.EventManager;
			this.deviceInfoController = new AndroidDeviceInfoController();
			eventManager.RegisterObserver(this, EventId.AssetLoadEnd);
			eventManager.RegisterObserver(this, EventId.AssetLoadStart);
			eventManager.RegisterObserver(this, EventId.BattleLoadEnd);
			eventManager.RegisterObserver(this, EventId.BattleLoadStart);
			eventManager.RegisterObserver(this, EventId.ContractAdded);
			eventManager.RegisterObserver(this, EventId.ContractCanceled);
			eventManager.RegisterObserver(this, EventId.ContractCompleted);
			eventManager.RegisterObserver(this, EventId.EpisodeInfoScreenGotoStore);
			eventManager.RegisterObserver(this, EventId.EpisodeInfoScreenOpened);
			eventManager.RegisterObserver(this, EventId.EpisodeInfoScreenStoryAction);
			eventManager.RegisterObserver(this, EventId.EpisodePointsHelpScreenOpened);
			eventManager.RegisterObserver(this, EventId.FacebookLoggedIn);
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
			eventManager.RegisterObserver(this, EventId.HUDBattleButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDBattleLogButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDCrystalButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDDroidButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDLeaderboardButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDHolonetButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDSettingsButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDShieldButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDSquadsButtonClicked);
			eventManager.RegisterObserver(this, EventId.HUDStoreButtonClicked);
			eventManager.RegisterObserver(this, EventId.FactionIconUpgraded);
			eventManager.RegisterObserver(this, EventId.InAppPurchaseSelect);
			eventManager.RegisterObserver(this, EventId.InitialLoadStart);
			eventManager.RegisterObserver(this, EventId.InitializeAudioEnd);
			eventManager.RegisterObserver(this, EventId.InitializeAudioStart);
			eventManager.RegisterObserver(this, EventId.InitializeBoardEnd);
			eventManager.RegisterObserver(this, EventId.InitializeBoardStart);
			eventManager.RegisterObserver(this, EventId.InitializeGameDataStart);
			eventManager.RegisterObserver(this, EventId.InitializeGameDataEnd);
			eventManager.RegisterObserver(this, EventId.InitializeGeneralSystemsEnd);
			eventManager.RegisterObserver(this, EventId.InitializeGeneralSystemsStart);
			eventManager.RegisterObserver(this, EventId.InitializeWorldEnd);
			eventManager.RegisterObserver(this, EventId.InitializeWorldStart);
			eventManager.RegisterObserver(this, EventId.LogStoryActionExecuted);
			eventManager.RegisterObserver(this, EventId.MetaDataLoadEnd);
			eventManager.RegisterObserver(this, EventId.MetaDataLoadStart);
			eventManager.RegisterObserver(this, EventId.PlayerLoginSuccess);
			eventManager.RegisterObserver(this, EventId.PreloadAssetsEnd);
			eventManager.RegisterObserver(this, EventId.PreloadAssetsStart);
			eventManager.RegisterObserver(this, EventId.PvpBattleSkipped);
			eventManager.RegisterObserver(this, EventId.PvpOpponentNotFound);
			eventManager.RegisterObserver(this, EventId.PvpRevengeOpponentNotFound);
			eventManager.RegisterObserver(this, EventId.SettingsAboutButtonClicked);
			eventManager.RegisterObserver(this, EventId.SettingsFacebookLoggedIn);
			eventManager.RegisterObserver(this, EventId.SettingsHelpButtonClicked);
			eventManager.RegisterObserver(this, EventId.SettingsMusicCheckboxSelected);
			eventManager.RegisterObserver(this, EventId.SettingsFanForumsButtonClicked);
			eventManager.RegisterObserver(this, EventId.SettingsSfxCheckboxSelected);
			eventManager.RegisterObserver(this, EventId.ShowOffers);
			eventManager.RegisterObserver(this, EventId.SoftCurrencyPurchaseSelect);
			eventManager.RegisterObserver(this, EventId.StoreCategorySelected);
			eventManager.RegisterObserver(this, EventId.ShardItemSelectedFromStore);
			eventManager.RegisterObserver(this, EventId.GoToShardShopClickedFromHolonet);
			eventManager.RegisterObserver(this, EventId.StringsLoadEnd);
			eventManager.RegisterObserver(this, EventId.StringsLoadStart);
			eventManager.RegisterObserver(this, EventId.SquadChatSent);
			eventManager.RegisterObserver(this, EventId.SquadJoinedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.SquadJoinApplicationAcceptedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.SquadReplaySharedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.UIAttackScreenSelection);
			eventManager.RegisterObserver(this, EventId.UIConflictStatusClicked);
			eventManager.RegisterObserver(this, EventId.UIIAPDisclaimerClosed);
			eventManager.RegisterObserver(this, EventId.UIIAPDisclaimerViewed);
			eventManager.RegisterObserver(this, EventId.UIFactionFlipAction);
			eventManager.RegisterObserver(this, EventId.UIFactionFlipConfirmAction);
			eventManager.RegisterObserver(this, EventId.UIFactionFlipOpened);
			eventManager.RegisterObserver(this, EventId.UIPvEMissionStart);
			eventManager.RegisterObserver(this, EventId.UILeaderboardExpand);
			eventManager.RegisterObserver(this, EventId.UILeaderboardFriendsTabShown);
			eventManager.RegisterObserver(this, EventId.UILeaderboardInfo);
			eventManager.RegisterObserver(this, EventId.UILeaderboardPlayersTabShown);
			eventManager.RegisterObserver(this, EventId.UILeaderboardTournamentTabShown);
			eventManager.RegisterObserver(this, EventId.UILeaderboardSquadTabShown);
			eventManager.RegisterObserver(this, EventId.UILeaderboardVisit);
			eventManager.RegisterObserver(this, EventId.UINotEnoughDroidBuy);
			eventManager.RegisterObserver(this, EventId.UINotEnoughDroidClose);
			eventManager.RegisterObserver(this, EventId.UINotEnoughDroidSpeedUp);
			eventManager.RegisterObserver(this, EventId.UINotEnoughHardCurrencyBuy);
			eventManager.RegisterObserver(this, EventId.UINotEnoughHardCurrencyClose);
			eventManager.RegisterObserver(this, EventId.UINotEnoughSoftCurrencyBuy);
			eventManager.RegisterObserver(this, EventId.UINotEnoughSoftCurrencyClose);
			eventManager.RegisterObserver(this, EventId.UIPvESelection);
			eventManager.RegisterObserver(this, EventId.UISquadJoinTabShown);
			eventManager.RegisterObserver(this, EventId.InventoryCrateTapped);
			eventManager.RegisterObserver(this, EventId.InventoryCrateStoreOpen);
			eventManager.RegisterObserver(this, EventId.InventoryCrateOpened);
			eventManager.RegisterObserver(this, EventId.HUDInventoryScreenOpened);
			eventManager.RegisterObserver(this, EventId.LootTableButtonTapped);
			eventManager.RegisterObserver(this, EventId.LootTableUnitInfoTapped);
			eventManager.RegisterObserver(this, EventId.LootTableRelocateTapped);
			eventManager.RegisterObserver(this, EventId.UnitInfoGoToGalaxy);
			eventManager.RegisterObserver(this, EventId.UISquadScreenTabShown);
			eventManager.RegisterObserver(this, EventId.UITournamentEndSelection);
			eventManager.RegisterObserver(this, EventId.UITournamentTierSelection);
			eventManager.RegisterObserver(this, EventId.UserIsIdle);
			eventManager.RegisterObserver(this, EventId.UILeaderboardJoin);
			eventManager.RegisterObserver(this, EventId.VisitPlayer);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete);
			eventManager.RegisterObserver(this, EventId.GalaxyOpenByContextButton);
			eventManager.RegisterObserver(this, EventId.GalaxyOpenByInfoScreen);
			eventManager.RegisterObserver(this, EventId.GalaxyOpenByPlayScreen);
			eventManager.RegisterObserver(this, EventId.GalaxyScreenClosed);
			eventManager.RegisterObserver(this, EventId.GalaxyPlanetTapped);
			eventManager.RegisterObserver(this, EventId.GalaxyPlanetInfoButton);
			eventManager.RegisterObserver(this, EventId.PlanetRelocateButtonPressed);
			eventManager.RegisterObserver(this, EventId.HolonetCommandCenterTab);
			eventManager.RegisterObserver(this, EventId.HolonetCommandCenterFeature);
			eventManager.RegisterObserver(this, EventId.HolonetVideoTab);
			eventManager.RegisterObserver(this, EventId.HolonetDevNotes);
			eventManager.RegisterObserver(this, EventId.HolonetTransmissionLog);
			eventManager.RegisterObserver(this, EventId.HolonetIncomingTransmission);
			eventManager.RegisterObserver(this, EventId.HolonetClosed);
			eventManager.RegisterObserver(this, EventId.HolonetOpened);
			eventManager.RegisterObserver(this, EventId.HolonetTabClosed);
			eventManager.RegisterObserver(this, EventId.HolonetTabOpened);
			eventManager.RegisterObserver(this, EventId.ObjectiveLockedCrateClicked);
			eventManager.RegisterObserver(this, EventId.ObjectiveDetailsClicked);
			eventManager.RegisterObserver(this, EventId.UISquadJoinScreenShown);
			eventManager.RegisterObserver(this, EventId.UISquadLeaveConfirmation);
			eventManager.RegisterObserver(this, EventId.CrateStoreOpen);
			eventManager.RegisterObserver(this, EventId.CrateStoreCancel);
			eventManager.RegisterObserver(this, EventId.CrateStorePurchase);
			eventManager.RegisterObserver(this, EventId.CrateStoreNotEnoughCurrency);
		}

		public void Destroy()
		{
			if (this.log != null)
			{
				this.log.Reset();
				this.log = null;
			}
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.AssetLoadEnd);
			eventManager.UnregisterObserver(this, EventId.AssetLoadStart);
			eventManager.UnregisterObserver(this, EventId.BattleLoadEnd);
			eventManager.UnregisterObserver(this, EventId.BattleLoadStart);
			eventManager.UnregisterObserver(this, EventId.ContractAdded);
			eventManager.UnregisterObserver(this, EventId.ContractCanceled);
			eventManager.UnregisterObserver(this, EventId.ContractCompleted);
			eventManager.UnregisterObserver(this, EventId.EpisodeInfoScreenGotoStore);
			eventManager.UnregisterObserver(this, EventId.EpisodeInfoScreenOpened);
			eventManager.UnregisterObserver(this, EventId.EpisodeInfoScreenStoryAction);
			eventManager.UnregisterObserver(this, EventId.EpisodePointsHelpScreenOpened);
			eventManager.UnregisterObserver(this, EventId.FacebookLoggedIn);
			eventManager.UnregisterObserver(this, EventId.GameStateChanged);
			eventManager.UnregisterObserver(this, EventId.HUDBattleButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDBattleLogButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDCrystalButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDDroidButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDLeaderboardButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDHolonetButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDSettingsButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDShieldButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDSquadsButtonClicked);
			eventManager.UnregisterObserver(this, EventId.HUDStoreButtonClicked);
			eventManager.UnregisterObserver(this, EventId.FactionIconUpgraded);
			eventManager.UnregisterObserver(this, EventId.InAppPurchaseSelect);
			eventManager.UnregisterObserver(this, EventId.InitialLoadStart);
			eventManager.UnregisterObserver(this, EventId.InitializeAudioEnd);
			eventManager.UnregisterObserver(this, EventId.InitializeAudioStart);
			eventManager.UnregisterObserver(this, EventId.InitializeBoardEnd);
			eventManager.UnregisterObserver(this, EventId.InitializeBoardStart);
			eventManager.UnregisterObserver(this, EventId.InitializeGameDataStart);
			eventManager.UnregisterObserver(this, EventId.InitializeGameDataEnd);
			eventManager.UnregisterObserver(this, EventId.InitializeGeneralSystemsEnd);
			eventManager.UnregisterObserver(this, EventId.InitializeGeneralSystemsStart);
			eventManager.UnregisterObserver(this, EventId.InitializeWorldEnd);
			eventManager.UnregisterObserver(this, EventId.InitializeWorldStart);
			eventManager.UnregisterObserver(this, EventId.LogStoryActionExecuted);
			eventManager.UnregisterObserver(this, EventId.MetaDataLoadEnd);
			eventManager.UnregisterObserver(this, EventId.MetaDataLoadStart);
			eventManager.UnregisterObserver(this, EventId.PlayerLoginSuccess);
			eventManager.UnregisterObserver(this, EventId.PreloadAssetsEnd);
			eventManager.UnregisterObserver(this, EventId.PreloadAssetsStart);
			eventManager.UnregisterObserver(this, EventId.PvpBattleSkipped);
			eventManager.UnregisterObserver(this, EventId.PvpOpponentNotFound);
			eventManager.UnregisterObserver(this, EventId.PvpRevengeOpponentNotFound);
			eventManager.UnregisterObserver(this, EventId.SettingsAboutButtonClicked);
			eventManager.UnregisterObserver(this, EventId.SettingsFacebookLoggedIn);
			eventManager.UnregisterObserver(this, EventId.SettingsHelpButtonClicked);
			eventManager.UnregisterObserver(this, EventId.SettingsMusicCheckboxSelected);
			eventManager.UnregisterObserver(this, EventId.SettingsFanForumsButtonClicked);
			eventManager.UnregisterObserver(this, EventId.SettingsSfxCheckboxSelected);
			eventManager.UnregisterObserver(this, EventId.ShowOffers);
			eventManager.UnregisterObserver(this, EventId.SoftCurrencyPurchaseSelect);
			eventManager.UnregisterObserver(this, EventId.StoreCategorySelected);
			eventManager.UnregisterObserver(this, EventId.ShardItemSelectedFromStore);
			eventManager.UnregisterObserver(this, EventId.GoToShardShopClickedFromHolonet);
			eventManager.UnregisterObserver(this, EventId.StringsLoadEnd);
			eventManager.UnregisterObserver(this, EventId.StringsLoadStart);
			eventManager.UnregisterObserver(this, EventId.SquadChatSent);
			eventManager.UnregisterObserver(this, EventId.SquadJoinedByCurrentPlayer);
			eventManager.UnregisterObserver(this, EventId.SquadJoinApplicationAcceptedByCurrentPlayer);
			eventManager.UnregisterObserver(this, EventId.SquadReplaySharedByCurrentPlayer);
			eventManager.UnregisterObserver(this, EventId.UIAttackScreenSelection);
			eventManager.UnregisterObserver(this, EventId.UIConflictStatusClicked);
			eventManager.UnregisterObserver(this, EventId.UIIAPDisclaimerClosed);
			eventManager.UnregisterObserver(this, EventId.UIIAPDisclaimerViewed);
			eventManager.UnregisterObserver(this, EventId.UIFactionFlipAction);
			eventManager.UnregisterObserver(this, EventId.UIFactionFlipConfirmAction);
			eventManager.UnregisterObserver(this, EventId.UIFactionFlipOpened);
			eventManager.UnregisterObserver(this, EventId.UIPvEMissionStart);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardExpand);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardFriendsTabShown);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardInfo);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardPlayersTabShown);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardTournamentTabShown);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardSquadTabShown);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardVisit);
			eventManager.UnregisterObserver(this, EventId.UINotEnoughDroidBuy);
			eventManager.UnregisterObserver(this, EventId.UINotEnoughDroidClose);
			eventManager.UnregisterObserver(this, EventId.UINotEnoughDroidSpeedUp);
			eventManager.UnregisterObserver(this, EventId.UINotEnoughHardCurrencyBuy);
			eventManager.UnregisterObserver(this, EventId.UINotEnoughHardCurrencyClose);
			eventManager.UnregisterObserver(this, EventId.UINotEnoughSoftCurrencyBuy);
			eventManager.UnregisterObserver(this, EventId.UINotEnoughSoftCurrencyClose);
			eventManager.UnregisterObserver(this, EventId.UIPvESelection);
			eventManager.UnregisterObserver(this, EventId.UISquadJoinTabShown);
			eventManager.UnregisterObserver(this, EventId.InventoryCrateTapped);
			eventManager.UnregisterObserver(this, EventId.InventoryCrateStoreOpen);
			eventManager.UnregisterObserver(this, EventId.InventoryCrateOpened);
			eventManager.UnregisterObserver(this, EventId.HUDInventoryScreenOpened);
			eventManager.UnregisterObserver(this, EventId.LootTableButtonTapped);
			eventManager.UnregisterObserver(this, EventId.LootTableUnitInfoTapped);
			eventManager.UnregisterObserver(this, EventId.LootTableRelocateTapped);
			eventManager.UnregisterObserver(this, EventId.UnitInfoGoToGalaxy);
			eventManager.UnregisterObserver(this, EventId.UISquadScreenTabShown);
			eventManager.UnregisterObserver(this, EventId.UITournamentEndSelection);
			eventManager.UnregisterObserver(this, EventId.UITournamentTierSelection);
			eventManager.UnregisterObserver(this, EventId.UserIsIdle);
			eventManager.UnregisterObserver(this, EventId.UILeaderboardJoin);
			eventManager.UnregisterObserver(this, EventId.VisitPlayer);
			eventManager.UnregisterObserver(this, EventId.WorldLoadComplete);
			eventManager.UnregisterObserver(this, EventId.GalaxyOpenByContextButton);
			eventManager.UnregisterObserver(this, EventId.GalaxyOpenByInfoScreen);
			eventManager.UnregisterObserver(this, EventId.GalaxyOpenByPlayScreen);
			eventManager.UnregisterObserver(this, EventId.GalaxyScreenClosed);
			eventManager.UnregisterObserver(this, EventId.GalaxyPlanetTapped);
			eventManager.UnregisterObserver(this, EventId.GalaxyPlanetInfoButton);
			eventManager.UnregisterObserver(this, EventId.PlanetRelocateButtonPressed);
			eventManager.UnregisterObserver(this, EventId.HolonetCommandCenterTab);
			eventManager.UnregisterObserver(this, EventId.HolonetCommandCenterFeature);
			eventManager.UnregisterObserver(this, EventId.HolonetVideoTab);
			eventManager.UnregisterObserver(this, EventId.HolonetDevNotes);
			eventManager.UnregisterObserver(this, EventId.HolonetTransmissionLog);
			eventManager.UnregisterObserver(this, EventId.HolonetIncomingTransmission);
			eventManager.UnregisterObserver(this, EventId.HolonetClosed);
			eventManager.UnregisterObserver(this, EventId.HolonetOpened);
			eventManager.UnregisterObserver(this, EventId.HolonetTabClosed);
			eventManager.UnregisterObserver(this, EventId.HolonetTabOpened);
			eventManager.UnregisterObserver(this, EventId.ObjectiveLockedCrateClicked);
			eventManager.UnregisterObserver(this, EventId.ObjectiveDetailsClicked);
			eventManager.UnregisterObserver(this, EventId.UISquadJoinScreenShown);
			eventManager.UnregisterObserver(this, EventId.UISquadLeaveConfirmation);
			eventManager.UnregisterObserver(this, EventId.CrateStoreOpen);
			eventManager.UnregisterObserver(this, EventId.CrateStoreCancel);
			eventManager.UnregisterObserver(this, EventId.CrateStorePurchase);
			eventManager.UnregisterObserver(this, EventId.CrateStoreNotEnoughCurrency);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.InitialLoadStart:
				this.TrackStepTiming("page_load", "start", "default", StepTimingType.Start);
				return EatResponse.NotEaten;
			case EventId.MetaDataLoadStart:
				this.TrackStepTiming("page_load", "metadata_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.MetaDataLoadEnd:
				this.TrackStepTiming("page_load", "metadata_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.AssetLoadStart:
				this.TrackStepTiming("page_load", "assetload_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.AssetLoadEnd:
				this.TrackStepTiming("page_load", "assetload_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.StringsLoadStart:
				this.TrackStepTiming("page_load", "string_data_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.StringsLoadEnd:
				this.TrackStepTiming("page_load", "string_data_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.PreloadAssetsStart:
				this.TrackStepTiming("page_load", "preload_assets_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.PreloadAssetsEnd:
				this.TrackStepTiming("page_load", "preload_assets_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeGameDataStart:
				this.TrackStepTiming("page_load", "init_gamedata_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeGameDataEnd:
				this.TrackStepTiming("page_load", "init_gamedata_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeAudioStart:
				this.TrackStepTiming("page_load", "init_audio_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeAudioEnd:
				this.TrackStepTiming("page_load", "init_audio_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.PlayedLoadedOnDemandAudio:
			case EventId.PreloadedAudioSuccess:
			case EventId.PreloadedAudioFailure:
			case EventId.SuccessfullyResumed:
			case EventId.AllUXElementsCreated:
			case EventId.ElementDestroyed:
			case EventId.UIFilterSelected:
			case EventId.EnterEditMode:
			case EventId.ExitEditMode:
			case EventId.ExitBaseLayoutToolMode:
			case EventId.BattleEndProcessing:
			case EventId.BattleEndRecorded:
			case EventId.BattleEndFullyProcessed:
			case EventId.BattleCancelRequested:
			case EventId.BattleCanceled:
			case EventId.BattleNextRequested:
			case EventId.BattleReplayRequested:
			case EventId.BattleReplaySetup:
			case EventId.BattleRecordRetrieved:
			case EventId.BattleLeftBeforeStarting:
			case EventId.BattleLoadedForDefend:
			case EventId.BattleReplayEnded:
			case EventId.MissionStarted:
			case EventId.PvpBattleStarting:
			case EventId.PvpBattleWon:
				IL_ED:
				switch (id)
				{
				case EventId.HolonetCommandCenterTab:
					this.TrackGameAction("UI_holonet", "command_center", cookie as string, null, 1);
					return EatResponse.NotEaten;
				case EventId.HolonetCommandCenterFeature:
					this.TrackGameAction("UI_holonet", "command_center", cookie as string, null, 1);
					return EatResponse.NotEaten;
				case EventId.HolonetVideoTab:
					this.TrackGameAction("UI_holonet", "video", cookie as string, null, 1);
					return EatResponse.NotEaten;
				case EventId.HolonetDevNotes:
					this.TrackGameAction("UI_holonet", "dev_notes", cookie as string, null, 1);
					return EatResponse.NotEaten;
				case EventId.HolonetTransmissionLog:
					this.TrackGameAction("UI_holonet", "transmission_log", null, null, 1);
					return EatResponse.NotEaten;
				case EventId.HolonetIncomingTransmission:
					this.TrackGameAction("UI_holonet", "incoming_transmission", cookie as string, null, 1);
					return EatResponse.NotEaten;
				case EventId.HolonetOpened:
					this.TrackStepTiming("holonet", "start", "holonet", StepTimingType.Intermediary);
					return EatResponse.NotEaten;
				case EventId.HolonetClosed:
					this.TrackGameAction("UI_holonet", "close", cookie as string, null, 1);
					this.TrackStepTiming("holonet", "end", "holonet", StepTimingType.Intermediary);
					return EatResponse.NotEaten;
				case EventId.HolonetTabOpened:
					this.TrackStepTiming("holonet_tab", "start", cookie as string, StepTimingType.Intermediary);
					return EatResponse.NotEaten;
				case EventId.HolonetTabClosed:
					this.TrackStepTiming("holonet_tab", "end", cookie as string, StepTimingType.Intermediary);
					return EatResponse.NotEaten;
				case EventId.ObjectiveLockedCrateClicked:
					this.TrackGameAction("UI_objectives", "locked_crate", cookie as string, null, 1);
					return EatResponse.NotEaten;
				case EventId.ObjectiveDetailsClicked:
					this.TrackGameAction("UI_objectives", "objective_details", cookie as string, null, 1);
					return EatResponse.NotEaten;
				case EventId.ObjectiveCrateInfoScreenOpened:
				case EventId.ObjectiveCompleted:
				case EventId.ObjectiveRewardDataCardRevealed:
				case EventId.SquadScreenOpenedOrClosed:
				case EventId.SquadUpdated:
				case EventId.SquadLeft:
				case EventId.SquadChatFilterUpdated:
				case EventId.SquadJoinInviteAcceptedByCurrentPlayer:
				case EventId.SquadTroopsRequestedByCurrentPlayer:
				case EventId.SquadWarTroopsRequestStartedByCurrentPlayer:
				case EventId.SquadWarTroopsRequestedByCurrentPlayer:
				case EventId.SquadTroopsDonatedByCurrentPlayer:
					IL_165:
					switch (id)
					{
					case EventId.SquadChatSent:
						this.TrackGameAction("squad_action", "chat", string.Empty, this.GetSquadID(), 1);
						return EatResponse.NotEaten;
					case EventId.SquadDetailsUpdated:
					case EventId.SquadUpdateCompleted:
					case EventId.SquadEdited:
					case EventId.SquadSelect:
					case EventId.SquadSend:
					case EventId.SquadNext:
					case EventId.SquadMore:
					case EventId.SquadFB:
					case EventId.SquadCredits:
						IL_1CA:
						switch (id)
						{
						case EventId.UIAttackScreenSelection:
						{
							ActionMessageBIData actionMessageBIData = (ActionMessageBIData)cookie;
							this.TrackGameAction("UI_attack", actionMessageBIData.Action, actionMessageBIData.Message, string.Empty, 1);
							return EatResponse.NotEaten;
						}
						case EventId.UISquadWarScreen:
						{
							ActionMessageBIData actionMessageBIData2 = (ActionMessageBIData)cookie;
							this.TrackGameAction("UI_squadwar_attack", actionMessageBIData2.Action, actionMessageBIData2.Message, string.Empty);
							return EatResponse.NotEaten;
						}
						case EventId.UIConflictStatusClicked:
						{
							ActionMessageBIData actionMessageBIData3 = (ActionMessageBIData)cookie;
							this.TrackGameAction("UI_conflict_ticker", actionMessageBIData3.Action, actionMessageBIData3.Message, string.Empty, 1);
							return EatResponse.NotEaten;
						}
						case EventId.UIPvESelection:
						{
							ActionMessageBIData actionMessageBIData4 = (ActionMessageBIData)cookie;
							this.TrackGameAction("UI_PvE_mission", actionMessageBIData4.Action, actionMessageBIData4.Message, string.Empty, 1);
							return EatResponse.NotEaten;
						}
						case EventId.UIPvEMissionStart:
							this.TrackGameAction("PvE", "start", cookie as string, string.Empty, 1);
							return EatResponse.NotEaten;
						case EventId.UITournamentTierSelection:
						{
							ActionMessageBIData actionMessageBIData5 = (ActionMessageBIData)cookie;
							this.TrackGameAction("UI_tournament_tiers", actionMessageBIData5.Action, actionMessageBIData5.Message, string.Empty, 1);
							return EatResponse.NotEaten;
						}
						case EventId.UITournamentEndSelection:
						{
							ActionMessageBIData actionMessageBIData6 = (ActionMessageBIData)cookie;
							this.TrackGameAction("UI_tournament_end", actionMessageBIData6.Action, actionMessageBIData6.Message, string.Empty, 1);
							return EatResponse.NotEaten;
						}
						case EventId.ObjectivesUpdated:
						case EventId.UpdateObjectiveToastData:
						case EventId.ShowObjectiveToast:
						case EventId.ClaimObjectiveFailed:
						case EventId.HoloEvent:
						case EventId.StoryTranscriptDisplayed:
						case EventId.HolocommScreenLoadComplete:
						case EventId.HoloCommScreenDestroyed:
						case EventId.StoryNextButtonClicked:
						case EventId.StoryAttackButtonClicked:
						case EventId.StorySkipButtonClicked:
						case EventId.StoryChainCompleted:
							IL_236:
							switch (id)
							{
							case EventId.HUDBattleButtonClicked:
								this.TrackGameAction("UI_HUD", "attack", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDBattleLogButtonClicked:
								this.TrackGameAction("UI_HUD", "battle_log", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDCrystalButtonClicked:
								this.TrackGameAction("UI_HUD", "add_crystals", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDDroidButtonClicked:
								this.TrackGameAction("UI_HUD", "add_droid", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDLeaderboardButtonClicked:
								this.TrackGameAction("UI_HUD", "leaderboard", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDHolonetButtonClicked:
								this.TrackGameAction("UI_HUD", "holonet", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDSettingsButtonClicked:
								this.TrackGameAction("UI_HUD", "settings", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDShieldButtonClicked:
								this.TrackGameAction("UI_HUD", "damage_protection", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDSquadsButtonClicked:
								this.TrackGameAction("UI_HUD", "squad", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDStoreButtonClicked:
								this.TrackGameAction("UI_HUD", "shop", null, null, 1);
								return EatResponse.NotEaten;
							case EventId.HUDSpecialPromotionButtonClicked:
							case EventId.HUDFactionTooltipVisible:
							case EventId.HQCelebrationPlayed:
								IL_27A:
								switch (id)
								{
								case EventId.HUDInventoryScreenOpened:
									this.TrackGameAction("UI_crate_inventory", "inventory_tap", string.Empty, string.Empty, 1);
									return EatResponse.NotEaten;
								case EventId.InventoryCrateTapped:
								{
									string message = Convert.ToString(cookie);
									this.TrackGameAction("UI_crate_inventory", "crate_tap", message, string.Empty, 1);
									return EatResponse.NotEaten;
								}
								case EventId.InventoryCrateOpened:
								{
									string message2 = Convert.ToString(cookie);
									this.TrackGameAction("UI_crate_inventory", "crate_open", message2, string.Empty, 1);
									return EatResponse.NotEaten;
								}
								case EventId.InventoryCrateStoreOpen:
								{
									string message3 = Convert.ToString(cookie);
									this.TrackGameAction("UI_crate_inventory", "crate_store", message3, string.Empty, 1);
									return EatResponse.NotEaten;
								}
								case EventId.LootTableButtonTapped:
									this.TrackGameAction("UI_prize_table", "prize_table_tap", string.Empty, string.Empty, 1);
									return EatResponse.NotEaten;
								case EventId.LootTableUnitInfoTapped:
								{
									string message4 = Convert.ToString(cookie);
									this.TrackGameAction("UI_prize_table", "unit_info_tap", message4, string.Empty, 1);
									return EatResponse.NotEaten;
								}
								case EventId.LootTableRelocateTapped:
								{
									CurrentPlayer currentPlayer = Service.CurrentPlayer;
									string planetId = currentPlayer.PlanetId;
									int rawRelocationStarsCount = currentPlayer.GetRawRelocationStarsCount();
									string value = Convert.ToString(cookie);
									string value2 = currentPlayer.IsRelocationRequirementMet().ToString();
									StringBuilder stringBuilder = new StringBuilder(rawRelocationStarsCount.ToString());
									stringBuilder.Append("|");
									stringBuilder.Append(value2);
									stringBuilder.Append("|");
									stringBuilder.Append(value);
									stringBuilder.Append("|");
									stringBuilder.Append(planetId);
									this.TrackGameAction("UI_prize_table", "relocate", stringBuilder.ToString(), string.Empty, 1);
									return EatResponse.NotEaten;
								}
								case EventId.UnitInfoGoToGalaxy:
								{
									string message5 = Convert.ToString(cookie);
									this.TrackGameAction("UI_unit_info", "galaxy_map", message5, string.Empty, 1);
									return EatResponse.NotEaten;
								}
								default:
									switch (id)
									{
									case EventId.FacebookLoggedIn:
										this.TrackGameAction("facebook_connect", "allow", cookie as string, string.Empty);
										return EatResponse.NotEaten;
									case EventId.SettingsAboutButtonClicked:
										this.TrackGameAction("UI_settings", "about", null, null, 1);
										return EatResponse.NotEaten;
									case EventId.SettingsFacebookLoggedIn:
										this.HandleSettingsScreenFacebookLogin((bool)cookie);
										return EatResponse.NotEaten;
									case EventId.SettingsHelpButtonClicked:
										this.TrackGameAction("UI_settings", "help", null, null, 1);
										return EatResponse.NotEaten;
									case EventId.SettingsMusicCheckboxSelected:
										this.HandleSettingsScreenMusicSetting((bool)cookie);
										return EatResponse.NotEaten;
									case EventId.SettingsFanForumsButtonClicked:
										this.TrackGameAction("UI_settings", "fan_forums", null, null, 1);
										return EatResponse.NotEaten;
									case EventId.SettingsSfxCheckboxSelected:
										this.HandleSettingsScreenSfxSetting((bool)cookie);
										return EatResponse.NotEaten;
									default:
										switch (id)
										{
										case EventId.GalaxyOpenByInfoScreen:
											this.TrackGameAction("UI_galaxy_map", "open", "info_screen", string.Empty, 1);
											return EatResponse.NotEaten;
										case EventId.GalaxyOpenByContextButton:
											this.TrackGameAction("UI_galaxy_map", "open", "context_button", string.Empty, 1);
											return EatResponse.NotEaten;
										case EventId.GalaxyOpenByPlayScreen:
											this.TrackGameAction("UI_galaxy_map", "open", "play_screen", string.Empty, 1);
											return EatResponse.NotEaten;
										case EventId.GalaxyScreenClosed:
											this.TrackGameAction("UI_galaxy_map", "close", string.Empty, string.Empty, 1);
											return EatResponse.NotEaten;
										case EventId.GalaxyPlanetTapped:
											this.TrackGameAction("UI_galaxy_map", "planet", cookie as string, string.Empty, 1);
											return EatResponse.NotEaten;
										case EventId.GalaxyPlanetInfoButton:
											this.TrackGameAction("UI_attack", "info", cookie as string, string.Empty, 1);
											return EatResponse.NotEaten;
										default:
											switch (id)
											{
											case EventId.UIIAPDisclaimerClosed:
												this.TrackGameAction("UI_IAP_disclaimer", "close", string.Empty, string.Empty, 0);
												return EatResponse.NotEaten;
											case EventId.UIIAPDisclaimerViewed:
												this.TrackGameAction("UI_IAP_disclaimer", "view", string.Empty, string.Empty, 0);
												return EatResponse.NotEaten;
											case EventId.UIFactionFlipAction:
												this.TrackGameAction("UI_faction_flip", "flip", cookie as string, string.Empty, 1);
												return EatResponse.NotEaten;
											case EventId.UIFactionFlipConfirmAction:
												this.TrackGameAction("UI_faction_flip", "confirmation", cookie as string, string.Empty, 1);
												return EatResponse.NotEaten;
											case EventId.UIFactionFlipOpened:
												this.TrackGameAction("UI_faction_flip", "menu", cookie as string, string.Empty, 1);
												return EatResponse.NotEaten;
											case EventId.TrapTriggered:
											case EventId.TrapDisarmed:
											case EventId.TrapDestroyed:
												IL_322:
												switch (id)
												{
												case EventId.CrateStoreOpen:
													this.TrackGameAction("UI_shop_treasure", cookie as string, "open", string.Empty, 1);
													return EatResponse.NotEaten;
												case EventId.CrateStoreCancel:
													this.TrackGameAction("UI_shop_treasure", cookie as string, "cancel", string.Empty, 1);
													return EatResponse.NotEaten;
												case EventId.CrateStorePurchase:
													this.TrackGameAction("UI_shop_treasure", cookie as string, "purchase", string.Empty, 1);
													return EatResponse.NotEaten;
												case EventId.CrateStoreNotEnoughCurrency:
													this.TrackGameAction("UI_shop_treasure", cookie as string, "not_enough_currency", string.Empty, 1);
													return EatResponse.NotEaten;
												default:
													switch (id)
													{
													case EventId.EpisodeInfoScreenOpened:
														this.TrackGameAction("UI_event_widget", "open", cookie as string, null, 1);
														return EatResponse.NotEaten;
													case EventId.EpisodeInfoScreenGotoStore:
													case EventId.EpisodeInfoScreenStoryAction:
													case EventId.EpisodePointsHelpScreenOpened:
														this.TrackGameAction("UI_event_widget", "button_tap", cookie as string, null, 1);
														return EatResponse.NotEaten;
													default:
														switch (id)
														{
														case EventId.StoreCategorySelected:
															this.HandleStoreCategorySelection((StoreTab)cookie);
															return EatResponse.NotEaten;
														case EventId.ShardItemSelectedFromStore:
															this.HandleStoreShardItemSelection((string)cookie);
															return EatResponse.NotEaten;
														case EventId.GoToShardShopClickedFromHolonet:
															this.HandleHolonetGoToShardShopClick();
															return EatResponse.NotEaten;
														default:
															switch (id)
															{
															case EventId.ContractCompleted:
															case EventId.ContractCanceled:
																this.TrackBuildingContractStepTiming(StepTimingType.End, cookie as ContractEventData);
																return EatResponse.NotEaten;
															case EventId.ContractCompletedForStoryAction:
															case EventId.ContractsCompletedWhileOffline:
																IL_38B:
																if (id == EventId.WorldLoadComplete)
																{
																	IState currentState = Service.GameStateMachine.CurrentState;
																	if (currentState is ApplicationLoadState)
																	{
																		this.TrackStepTiming("page_load", "end", "default", StepTimingType.End);
																		this.TrackUserInfo();
																		this.TrackPlayerInfo();
																		this.TrackFaction();
																	}
																	return EatResponse.NotEaten;
																}
																if (id == EventId.ContractAdded)
																{
																	this.TrackBuildingContractStepTiming(StepTimingType.Start, cookie as ContractEventData);
																	return EatResponse.NotEaten;
																}
																if (id != EventId.PlayerLoginSuccess)
																{
																	return EatResponse.NotEaten;
																}
																this.TrackLogin();
																this.TrackDeviceInfo();
																this.TrackGeo();
																return EatResponse.NotEaten;
															}
															goto IL_38B;
														}
														break;
													}
													break;
												}
												break;
											case EventId.PlanetRelocateButtonPressed:
												this.TrackGameAction("UI_attack", "relocate", cookie as string, string.Empty, 1);
												return EatResponse.NotEaten;
											}
											goto IL_322;
										}
										break;
									}
									break;
								}
								break;
							case EventId.FactionIconUpgraded:
								this.TrackGameAction("faction_icon", "icon_level", null, null, 1);
								return EatResponse.NotEaten;
							}
							goto IL_27A;
						case EventId.ShowOffers:
							this.TrackGameAction("UI_show_offers", cookie as string, string.Empty, string.Empty);
							return EatResponse.NotEaten;
						case EventId.InAppPurchaseSelect:
						case EventId.SoftCurrencyPurchaseSelect:
							this.TrackGameAction("UI_shop_treasure", cookie as string, string.Empty, string.Empty, 1);
							return EatResponse.NotEaten;
						case EventId.UserIsIdle:
							this.TrackGameAction("UI_idlepop", "idlepop", string.Empty, string.Empty);
							return EatResponse.NotEaten;
						case EventId.LogStoryActionExecuted:
							this.HandleStoryAction(cookie as StoryActionVO);
							return EatResponse.NotEaten;
						}
						goto IL_236;
					case EventId.UISquadJoinTabShown:
						this.TrackGameAction("UI_squad", cookie as string, this.GetMemberOrNonMember(), string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UISquadJoinScreenShown:
						this.TrackGameAction("UI_squad", "featured_access", (cookie as string) + "|" + this.GetMemberOrNonMember(), string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UISquadLeaveConfirmation:
						this.TrackGameAction("squad_action", "leave_prompt", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UISquadScreenTabShown:
						this.TrackGameAction("UI_squad", cookie as string, this.GetMemberOrNonMember(), string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardSquadTabShown:
						this.TrackGameAction("UI_leaderboard", "squads", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardFriendsTabShown:
						this.TrackGameAction("UI_leaderboard", "friends", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardPlayersTabShown:
						this.TrackGameAction("UI_leaderboard", "players", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardTournamentTabShown:
						this.TrackGameAction("UI_leaderboard", "tournament", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardExpand:
						this.TrackGameAction("UI_leaderboard", "expand", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardVisit:
						this.TrackGameAction("UI_leaderboard_expand", "visit", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardInfo:
						this.TrackGameAction("UI_leaderboard_expand", "info", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.UILeaderboardJoin:
						this.TrackGameAction("UI_leaderboard_expand", "join", cookie as string, string.Empty, 1);
						return EatResponse.NotEaten;
					case EventId.VisitPlayer:
					{
						PlayerVisitTag playerVisitTag = cookie as PlayerVisitTag;
						this.TrackPlayerVisit(playerVisitTag.IsSquadMate, playerVisitTag.IsFriend, playerVisitTag.TabName, playerVisitTag.PlayerId);
						return EatResponse.NotEaten;
					}
					}
					goto IL_1CA;
				case EventId.SquadJoinedByCurrentPlayer:
					this.TrackSquadSocialGameAction("squad_membership_social", "join", cookie as string, true);
					return EatResponse.NotEaten;
				case EventId.SquadJoinApplicationAcceptedByCurrentPlayer:
					this.TrackSquadSocialGameAction("squad_action", "join_accept", cookie as string, false);
					return EatResponse.NotEaten;
				case EventId.SquadReplaySharedByCurrentPlayer:
				{
					SqmReplayData sqmReplayData = (SqmReplayData)cookie;
					string action = (sqmReplayData.BattleType != SquadBattleReplayType.Defense) ? "attack" : "defense";
					this.TrackGameAction("share_replay", action, string.Empty, sqmReplayData.BattleId, 1);
					return EatResponse.NotEaten;
				}
				}
				goto IL_165;
			case EventId.InitializeBoardStart:
				this.TrackStepTiming("page_load", "init_board_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeBoardEnd:
				this.TrackStepTiming("page_load", "init_board_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeGeneralSystemsStart:
				this.TrackStepTiming("page_load", "init_general_sys_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeGeneralSystemsEnd:
				this.TrackStepTiming("page_load", "init_general_sys_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeWorldStart:
				this.TrackStepTiming("page_load", "init_world_start", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.InitializeWorldEnd:
				this.TrackStepTiming("page_load", "init_world_end", "default", StepTimingType.Intermediary);
				return EatResponse.NotEaten;
			case EventId.UINotEnoughSoftCurrencyBuy:
				this.TrackGameAction("UI_money_flow", "buy", "not_enough_soft_currency", string.Empty, 1);
				return EatResponse.NotEaten;
			case EventId.UINotEnoughSoftCurrencyClose:
				this.TrackGameAction("UI_money_flow", "close", "not_enough_soft_currency", string.Empty, 1);
				return EatResponse.NotEaten;
			case EventId.UINotEnoughHardCurrencyBuy:
				this.TrackGameAction("UI_money_flow", "buy", "not_enough_hard_currency", string.Empty, 1);
				return EatResponse.NotEaten;
			case EventId.UINotEnoughHardCurrencyClose:
				this.TrackGameAction("UI_money_flow", "close", "not_enough_hard_currency", string.Empty, 1);
				return EatResponse.NotEaten;
			case EventId.UINotEnoughDroidBuy:
				this.TrackGameAction("UI_money_flow", "buy", "all_droids_busy", string.Empty, 1);
				return EatResponse.NotEaten;
			case EventId.UINotEnoughDroidClose:
				this.TrackGameAction("UI_money_flow", "close", "all_droids_busy", string.Empty, 1);
				return EatResponse.NotEaten;
			case EventId.UINotEnoughDroidSpeedUp:
				this.TrackGameAction("UI_money_flow", "speed_up", "all_droids_busy", string.Empty, 1);
				return EatResponse.NotEaten;
			case EventId.BattleLoadStart:
				this.TrackBattleLoadStepTiming(StepTimingType.Start);
				return EatResponse.NotEaten;
			case EventId.BattleLoadEnd:
				this.TrackBattleLoadStepTiming(StepTimingType.End);
				return EatResponse.NotEaten;
			case EventId.PvpOpponentNotFound:
				this.TrackGameAction("UI_PvP", "no_opponent_found", cookie as string, string.Empty, 0);
				return EatResponse.NotEaten;
			case EventId.PvpRevengeOpponentNotFound:
				this.TrackGameAction("UI_Revenge", "no_opponent_found", cookie as string, string.Empty, 0);
				return EatResponse.NotEaten;
			case EventId.PvpBattleSkipped:
				this.TrackPvpGameAction("skip");
				return EatResponse.NotEaten;
			case EventId.GameStateChanged:
				this.HandleGameStateChanged();
				return EatResponse.NotEaten;
			}
			goto IL_ED;
		}

		public void SchedulePerformanceLogging(bool home)
		{
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			if (this.sampleDelayTimerID != 0u)
			{
				viewTimerManager.KillViewTimer(this.sampleDelayTimerID);
			}
			this.sampleDelayTimerID = viewTimerManager.CreateViewTimer((float)((!home) ? GameConstants.PERFORMANCE_SAMPLE_DELAY_BATTLE : GameConstants.PERFORMANCE_SAMPLE_DELAY_HOME), false, new TimerDelegate(this.PerformanceLogCallback), 0);
		}

		public void UnschedulePerformanceLogging()
		{
			if (this.sampleDelayTimerID != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.sampleDelayTimerID);
				this.sampleDelayTimerID = 0u;
			}
		}

		private void PerformanceLogCallback(uint id, object cookie)
		{
			this.sampleDelayTimerID = 0u;
			this.biFrameMonitor.LogFPS();
		}

		public void SetBIUrl(string event2Url, string event2SecondaryUrl)
		{
			this.event2LogCreator.SetURL(event2Url, event2SecondaryUrl);
		}

		private void AddCommonParameters(BILog biLog)
		{
			biLog.AddParam("app", "starts");
			biLog.AddParam("c_app_version", "6.0.0.10394");
			biLog.AddParam("app_locale", this.GetLangLocale());
			biLog.AddParam("network", this.GetNetwork());
			biLog.AddParam("view_network", this.GetViewNetwork());
			biLog.AddParam("user_id", this.GetPlayerId());
		}

		private void AddLocaleParameter(BILog biLog)
		{
			biLog.AddParam("locale", this.GetDeviceLocale());
		}

		private void AddLangParameter(BILog biLog)
		{
			biLog.AddParam("lang", this.GetLangLocale());
		}

		private void StartCallBICoroutine(BILog biLog)
		{
			BILogData event2LogData = null;
			if (GameConstants.EVENT_2_BI_ENABLED)
			{
				event2LogData = this.event2LogCreator.CreateWWWDataFromBILog(biLog);
			}
			this.engine.StartCoroutine(this.CallBI(event2LogData));
		}

		public void TrackAuthorization(string step, string type)
		{
			this.log.Reset();
			this.AddCommonParameters(this.log);
			this.log.AddParam("tag", "authorization");
			this.log.AddParam("step", step);
			this.log.AddParam("type", type);
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackLogin()
		{
			this.log.Reset();
			this.log.AddParam("tag", "clicked_link");
			this.AddCommonParameters(this.log);
			this.AddLangParameter(this.log);
			this.AddLocaleParameter(this.log);
			this.log.AddParam("is_new_user", this.IsNewUser());
			this.log.AddParam("tracking_code", "mobile");
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
			this.log.AddParam("tag", "clicked_link");
			this.AddCommonParameters(this.log);
			this.AddLangParameter(this.log);
			this.AddLocaleParameter(this.log);
			this.log.AddParam("app", "click_track");
			this.log.AddParam("is_new_user", this.IsNewUser());
			this.log.AddParam("log_app", "starts");
			this.log.AddParam("tracking_code", "mobile");
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackUserInfo()
		{
			this.log.Reset();
			this.log.AddParam("tag", "user_info");
			this.AddCommonParameters(this.log);
			this.AddLangParameter(this.log);
			this.log.AddParam("device_id", this.deviceInfoController.GetDeviceId());
			this.log.AddParam("device_type", this.GetDeviceType());
			this.log.AddParam("os_version", Service.EnvironmentController.GetOSVersion());
			this.log.AddParam("level", this.GetHQLevel());
			if (Service.ISocialDataController != null && Service.ISocialDataController.HaveSelfData)
			{
				this.log.AddParam("gender", Service.ISocialDataController.Gender);
			}
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackPlayerInfo()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			CampaignController campaignController = Service.CampaignController;
			this.log.Reset();
			this.log.AddParam("tag", "player_info");
			this.AddCommonParameters(this.log);
			this.log.AddParam("level", this.GetHQLevel());
			this.log.AddParam("credit_balance", currentPlayer.CurrentCreditsAmount.ToString());
			this.log.AddParam("alloy_balance", currentPlayer.CurrentMaterialsAmount.ToString());
			this.log.AddParam("crystal_balance", currentPlayer.CurrentCrystalsAmount.ToString());
			this.log.AddParam("stars_earned", campaignController.GetTotalStarsEarned().ToString());
			this.log.AddParam("droids_available", currentPlayer.CurrentDroidsAmount.ToString());
			this.log.AddParam("faction", currentPlayer.Faction.ToString());
			this.log.AddParam("squad_name", this.GetSquadName());
			this.log.AddParam("squad_id", this.GetSquadID());
			this.log.AddParam("clearable_units", Service.BuildingLookupController.GetNumberOfClearables().ToString());
			this.log.AddParam("trophy_balance", "5");
			this.log.AddParam("furthest_mission_complete", "-1");
			this.log.AddParam("shield_timer", "-1");
			this.log.AddParam("lifetime_spend", "-1");
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackFaction()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.Faction == FactionType.Empire || currentPlayer.Faction == FactionType.Rebel)
			{
				this.TrackGameAction("faction_choice", currentPlayer.Faction.ToString(), null, null, 1);
			}
		}

		public void TrackDeviceInfo()
		{
			this.log.Reset();
			this.log.AddParam("tag", "device_info");
			this.AddCommonParameters(this.log);
			this.AddLangParameter(this.log);
			this.log.AddParam("machine", this.GetDeviceType());
			this.log.AddParam("model", this.GetDeviceModel());
			this.log.AddParam("os_version", Service.EnvironmentController.GetOSVersion());
			this.log.AddParam("timestamp", this.GetTimeStampInMilliseconds());
			this.deviceInfoController.AddDeviceSpecificInfo(this.log);
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackIAPGameAction(string context, string action, string message)
		{
			this.TrackGameAction(context, action, message, string.Empty + this.iapActionCounter++);
		}

		private void TrackSquadSocialGameAction(string context, string action, string source, bool addFriendIds)
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			int num = 0;
			string arg = string.Empty;
			List<string> friendIdsInSquad = SquadUtils.GetFriendIdsInSquad(currentSquad.SquadID, Service.LeaderboardController);
			if (friendIdsInSquad != null)
			{
				num = friendIdsInSquad.Count;
				if (addFriendIds)
				{
					arg = SquadUtils.GetFriendIdsString(friendIdsInSquad);
				}
			}
			string arg2 = "lower";
			if (SquadUtils.IsPlayerMedalCountHigherThanSquadAvg(currentSquad, Service.CurrentPlayer.PlayerMedals))
			{
				arg2 = "higher";
			}
			string message = string.Format("{0}|{1}|{2}", num.ToString(), arg2, source.ToLower());
			string otherData = (!addFriendIds) ? currentSquad.SquadID : string.Format("{0}|{1}", currentSquad.SquadID, arg);
			this.TrackGameAction(context, action, message, otherData, 1);
		}

		public void TrackGameAction(string context, string action, string message, string otherData)
		{
			this.TrackGameAction(context, action, message, otherData, 0);
		}

		private void AddGameActionParams(string context, string action, string message, string otherData, int engaged)
		{
			this.log.AddParam("tag", "game_action");
			this.AddCommonParameters(this.log);
			this.AddLangParameter(this.log);
			this.log.AddParam("context", context);
			this.log.AddParam("action", action);
			this.log.AddParam("engaged", engaged.ToString());
			if (!string.IsNullOrEmpty(message))
			{
				this.log.AddParam("message", message);
			}
			if (!string.IsNullOrEmpty(otherData))
			{
				this.log.AddParam("other_key", otherData);
			}
		}

		public void TrackGameAction(string context, string action, string message, string otherData, int engaged)
		{
			this.log.Reset();
			this.AddGameActionParams(context, action, message, otherData, engaged);
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackStepTiming(string context, string location, string pathName, StepTimingType type)
		{
			string location2 = location;
			if (context.Equals("page_load"))
			{
				if (type != StepTimingType.Start && type != StepTimingType.End)
				{
					location2 = this.GetPageLoadStepCounter() + location;
				}
				this.pageLoadStepCounter++;
				if (type == StepTimingType.End)
				{
					this.pageLoadStepCounter = 1;
				}
			}
			this.TrackStepTiming(context, location2, pathName, type, 0);
		}

		public void TrackStepTiming(string context, string location, string pathName, StepTimingType type, int engaged)
		{
			this.log.Reset();
			this.log.AddParam("tag", "step_timing");
			this.AddCommonParameters(this.log);
			this.log.AddParam("context", context);
			this.log.AddParam("location", location);
			this.log.AddParam("path_name", pathName);
			this.log.AddParam("timestamp_ms", this.GetTimeStampInSeconds());
			this.log.AddParam("engaged", engaged.ToString());
			if (type != StepTimingType.Start)
			{
				if (type != StepTimingType.Intermediary)
				{
					if (type == StepTimingType.End)
					{
						this.stepTiming.EndStep(context, this.log);
					}
				}
				else
				{
					this.stepTiming.IntermediaryStep(context, this.log);
				}
			}
			else
			{
				this.stepTiming.StartStep(context);
			}
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackStepTiming(string context, string location, string pathName, int timeElapsed)
		{
			this.log.Reset();
			this.log.AddParam("tag", "step_timing");
			this.AddCommonParameters(this.log);
			this.log.AddParam("context", context);
			this.log.AddParam("location", location);
			this.log.AddParam("path_name", pathName);
			this.log.AddParam("timestamp_ms", this.GetTimeStampInSeconds());
			this.log.AddParam("elapsed_time_ms", timeElapsed.ToString());
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackGeo()
		{
			this.log.Reset();
			this.log.UseSecondaryUrl = true;
			this.log.AddParam("tag", "geo");
			this.AddCommonParameters(this.log);
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackPerformance(float fps, float memoryUsed)
		{
			this.log.Reset();
			double num = Math.Round((double)fps, 2);
			double num2 = Math.Round((double)memoryUsed, 2);
			string value = "battle";
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is HomeState)
			{
				value = "home";
			}
			this.log.AddParam("tag", "performance");
			this.AddCommonParameters(this.log);
			this.log.AddParam("time_since_start", DateUtils.GetRealTimeSinceStartUpInMilliseconds().ToString());
			this.log.AddParam("fps", num.ToString());
			this.log.AddParam("memory_used", num2.ToString());
			this.log.AddParam("display_state", value);
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackLowFPS(string errorMessage)
		{
			this.TrackError("Low FPS", errorMessage);
		}

		public void TrackError(LogLevel logLevel, string errorMessage)
		{
			this.TrackError((logLevel != LogLevel.Error) ? "Severe warning" : "Critical error", errorMessage);
		}

		private void TrackError(string reason, string message)
		{
			this.log.Reset();
			this.log.AddParam("tag", "error");
			this.AddCommonParameters(this.log);
			this.log.AddParam("reason", reason);
			string text = Service.GameStateMachine.CurrentState.ToString();
			text = text.Substring(text.LastIndexOf(".") + 1);
			this.log.AddParam("context", "Client: " + text);
			this.log.AddParam("message", message);
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackSendMessage(string trackingCode, string recipientIds, int numRecipients)
		{
			this.log.Reset();
			this.AddCommonParameters(this.log);
			this.log.AddParam("tag", "send_message");
			this.log.AddParam("tracking_code", trackingCode);
			this.log.AddParam("send_timestamp", this.GetTimeStampInSeconds());
			this.log.AddParam("target_user_id", recipientIds);
			this.log.AddParam("num_sent", numRecipients.ToString());
			this.StartCallBICoroutine(this.log);
			this.log.Reset();
		}

		public void TrackAssetBundleCacheClean(int version, bool isSuccess)
		{
			string message = (!isSuccess) ? "failure" : "success";
			this.TrackGameAction("asset_bundle_cache_clean", version.ToString(), message, null);
		}

		public void TrackAccountDiscard(AccountProvider provider, string playerId, string accountId)
		{
			string message = string.Format("{0}|{1}|{2}", provider.ToString(), accountId, playerId);
			this.TrackGameAction("account_id_sync", "discard", message, null);
		}

		public void TrackAccountRestore(AccountProvider provider, string playerId, string accountId)
		{
			string message = string.Format("{0}|{1}|{2}", provider.ToString(), accountId, playerId);
			this.TrackGameAction("account_id_sync", "restore", message, null);
		}

		[DebuggerHidden]
		private IEnumerator CallBI(BILogData event2LogData)
		{
			if (event2LogData != null)
			{
				WWW wWW = new WWW(event2LogData.url, event2LogData.postData, event2LogData.headers);
				WWWManager.Add(wWW);
				yield return wWW;
				if (WWWManager.Remove(wWW))
				{
					wWW.Dispose();
				}
			}
			yield break;
		}

		private void HandleStoryAction(StoryActionVO vo)
		{
			string logType = vo.LogType;
			if (logType != null)
			{
				if (logType == "start")
				{
					this.TrackStepTiming(vo.LogTag, "start", vo.LogPath, StepTimingType.Start, 1);
					return;
				}
				if (logType == "end")
				{
					this.TrackStepTiming(vo.LogTag, "end", vo.LogPath, StepTimingType.End, 1);
					this.TrackGameAction(vo.LogTag, vo.LogPath, string.Empty, vo.Uid, 1);
					return;
				}
			}
			if (!this.stepTiming.IsStepStarted(vo.LogTag))
			{
				this.TrackStepTiming(vo.LogTag, "start", vo.LogPath, StepTimingType.Start, 1);
			}
			this.TrackStepTiming(vo.LogTag, vo.Uid, vo.LogPath, StepTimingType.Intermediary, 1);
		}

		private void TrackBuildingContractStepTiming(StepTimingType type, ContractEventData contractData)
		{
			if (contractData.SendBILog)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int timeElapsed = 0;
				stringBuilder.Append(StringUtils.ToLowerCaseUnderscoreSeperated(contractData.BuildingVO.Type.ToString()));
				string location = string.Empty;
				if (type == StepTimingType.Start)
				{
					location = "start";
				}
				else if (type == StepTimingType.End)
				{
					location = "end";
					timeElapsed = (contractData.Contract.TotalTime - contractData.Contract.GetRemainingTimeForSim()) * 1000;
				}
				switch (contractData.Contract.DeliveryType)
				{
				case DeliveryType.Building:
				case DeliveryType.UpgradeBuilding:
					if (!string.IsNullOrEmpty(contractData.BuildingVO.TurretUid))
					{
						stringBuilder.Append("_upgrade");
					}
					stringBuilder.Append("|");
					stringBuilder.Append(contractData.BuildingVO.BuildingID);
					stringBuilder.Append("|");
					stringBuilder.Append(Service.StaticDataController.Get<BuildingTypeVO>(contractData.Contract.ProductUid).Lvl);
					break;
				case DeliveryType.SwapBuilding:
					stringBuilder.Append("_cross");
					stringBuilder.Append("|");
					stringBuilder.Append(contractData.BuildingVO.BuildingID);
					stringBuilder.Append("|");
					stringBuilder.Append(contractData.BuildingVO.Lvl);
					break;
				case DeliveryType.ClearClearable:
					stringBuilder.Append("|");
					stringBuilder.Append(contractData.BuildingVO.Uid);
					stringBuilder.Append("|");
					stringBuilder.Append(contractData.BuildingVO.SizeX * contractData.BuildingVO.SizeY);
					break;
				}
				switch (contractData.Contract.DeliveryType)
				{
				case DeliveryType.Building:
				case DeliveryType.UpgradeBuilding:
				case DeliveryType.SwapBuilding:
				case DeliveryType.ClearClearable:
					this.TrackStepTiming("droid", location, stringBuilder.ToString(), timeElapsed);
					break;
				}
			}
		}

		private void HandleStoreCategorySelection(StoreTab tab)
		{
			string text = null;
			switch (tab)
			{
			case StoreTab.Turrets:
				text = "turrets";
				break;
			case StoreTab.Crystals:
				text = "crystals";
				break;
			case StoreTab.Fragments:
				text = "datacard_shop";
				break;
			case StoreTab.Structures:
				text = "buildings";
				break;
			default:
				if (tab == StoreTab.Treasure)
				{
					text = "treasure";
				}
				break;
			}
			if (text != null)
			{
				this.TrackGameAction("UI_shop", text, null, null, 1);
			}
		}

		private void HandleStoreShardItemSelection(string message)
		{
			this.TrackGameAction("UI_datacard_shop", "datacard_shop", message, null, 1);
		}

		private void HandleHolonetGoToShardShopClick()
		{
			this.TrackGameAction("UI_holonet", "datacard_shop", null, null, 1);
		}

		private void HandleSettingsScreenFacebookLogin(bool loggedIn)
		{
			string action = "fb_connect";
			string action2 = "allow";
			if (!loggedIn)
			{
				action = "fb_disconnect";
				action2 = "disallow";
			}
			this.TrackGameAction("UI_settings", action, null, null, 1);
			this.TrackGameAction("facebook_connect", action2, "UI_settings", string.Empty);
		}

		private void HandleSettingsScreenMusicSetting(bool musicEnabled)
		{
			string action = "music_on";
			if (!musicEnabled)
			{
				action = "music_off";
			}
			this.TrackGameAction("UI_settings", action, null, null, 1);
		}

		private void HandleSettingsScreenSfxSetting(bool sfxEnabled)
		{
			string action = "sfx_on";
			if (!sfxEnabled)
			{
				action = "sfx_off";
			}
			this.TrackGameAction("UI_settings", action, null, null, 1);
		}

		private void TrackPlayerVisit(bool isSquadMate, bool isFriend, string message, string playerId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (isSquadMate)
			{
				stringBuilder.Append("squadmate");
			}
			else
			{
				stringBuilder.Append("not_squadmate");
			}
			stringBuilder.Append("|");
			if (isFriend)
			{
				stringBuilder.Append("friend");
			}
			else
			{
				stringBuilder.Append("not_friend");
			}
			stringBuilder.Append("|");
			stringBuilder.Append(this.GetHQLevel());
			this.TrackGameAction("visit_player", stringBuilder.ToString(), message, playerId, 1);
		}

		private void TrackBattleLoadStepTiming(StepTimingType type)
		{
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			BattleType type2 = currentBattle.Type;
			string context = string.Empty;
			switch (type2)
			{
			case BattleType.Pvp:
				context = "load_PvP";
				break;
			case BattleType.PveDefend:
			case BattleType.PveAttack:
			case BattleType.PveFue:
			case BattleType.PveBuffBase:
			case BattleType.PvpAttackSquadWar:
				if (currentBattle.IsSpecOps())
				{
					context = "load_campaign";
				}
				else
				{
					context = "load_PvE";
				}
				break;
			}
			this.TrackBattleStepTiming(type, context);
		}

		private void HandleGameStateChanged()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			BattleType type = currentBattle.Type;
			string context = string.Empty;
			if (currentState is BattlePlayState || currentState is BattleEndState)
			{
				switch (type)
				{
				case BattleType.Pvp:
					context = "PvP";
					break;
				case BattleType.PveDefend:
				case BattleType.PveAttack:
				case BattleType.PveFue:
				case BattleType.PveBuffBase:
				case BattleType.PvpAttackSquadWar:
					if (currentBattle.IsSpecOps())
					{
						context = "campaign";
					}
					else
					{
						context = "PvE";
					}
					break;
				}
				if (currentState is BattlePlayState)
				{
					this.TrackBattleStepTiming(StepTimingType.Start, context);
				}
				else if (currentState is BattleEndState)
				{
					this.TrackBattleStepTiming(StepTimingType.End, context);
				}
			}
		}

		private void TrackBattleStepTiming(StepTimingType type, string context)
		{
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			BattleType type2 = currentBattle.Type;
			string text = string.Empty;
			string location = string.Empty;
			if (type == StepTimingType.Start)
			{
				location = "start";
			}
			else if (type == StepTimingType.End)
			{
				location = "end";
			}
			switch (type2)
			{
			case BattleType.Pvp:
				text = currentBattle.AttackerID;
				if (text == Service.CurrentPlayer.PlayerId)
				{
					text = currentBattle.DefenderID;
				}
				break;
			case BattleType.PveDefend:
			case BattleType.PveAttack:
			case BattleType.PveFue:
			case BattleType.PveBuffBase:
			case BattleType.PvpAttackSquadWar:
			{
				StringBuilder stringBuilder = new StringBuilder();
				string missionId = currentBattle.MissionId;
				if (!string.IsNullOrEmpty(missionId))
				{
					CampaignMissionVO mission = this.GetMission(missionId);
					stringBuilder.Append(mission.BIChapterId);
					stringBuilder.Append("|");
					stringBuilder.Append(mission.BIMissionId);
					stringBuilder.Append("|");
					stringBuilder.Append(mission.BIMissionName);
					stringBuilder.Append("|");
					stringBuilder.Append(mission.Uid);
					text = stringBuilder.ToString();
				}
				else
				{
					stringBuilder.Append("null|null|null|null");
					text = stringBuilder.ToString();
				}
				break;
			}
			}
			this.TrackStepTiming(context, location, text, type, 1);
		}

		private void TrackPvpGameAction(string action)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			string recordID = currentBattle.RecordID;
			string value = (currentBattle.Defender == null) ? string.Empty : currentBattle.Defender.PlayerId;
			int defenderBaseScore = currentBattle.DefenderBaseScore;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(currentPlayer.CurrentXPAmount);
			stringBuilder.Append("|");
			stringBuilder.Append(defenderBaseScore);
			stringBuilder.Append("|");
			stringBuilder.Append(this.GetHQLevel());
			stringBuilder.Append("|");
			stringBuilder.Append(value);
			stringBuilder.Append("|");
			stringBuilder.Append("|");
			stringBuilder.Append("|");
			stringBuilder.Append(currentPlayer.Planet.PlanetBIName);
			this.TrackGameAction("PvP", action, stringBuilder.ToString(), recordID, 1);
		}

		private CampaignMissionVO GetMission(string uid)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			return staticDataController.Get<CampaignMissionVO>(uid);
		}

		private string GetPageLoadStepCounter()
		{
			StringBuilder stringBuilder = new StringBuilder("load_");
			if (this.pageLoadStepCounter < 10)
			{
				stringBuilder.Append("0");
			}
			stringBuilder.Append(this.pageLoadStepCounter);
			stringBuilder.Append("_");
			return stringBuilder.ToString();
		}

		private string GetPlayerId()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return (currentPlayer == null || currentPlayer.PlayerId == null) ? "NO_PLAYER_ID" : currentPlayer.PlayerId;
		}

		private string GetHQLevel()
		{
			if (Service.BuildingLookupController != null)
			{
				return Service.BuildingLookupController.GetHighestLevelHQ().ToString();
			}
			return "0";
		}

		private string GetSquadName()
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad != null && !string.IsNullOrEmpty(currentSquad.SquadName))
			{
				return currentSquad.SquadName;
			}
			return string.Empty;
		}

		private string GetSquadID()
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad != null && !string.IsNullOrEmpty(currentSquad.SquadID))
			{
				return currentSquad.SquadID;
			}
			return string.Empty;
		}

		private string GetNetwork()
		{
			return "u";
		}

		private string GetViewNetwork()
		{
			return "a";
		}

		private string GetDeviceType()
		{
			return Service.EnvironmentController.GetMachine();
		}

		private string GetDeviceModel()
		{
			return Service.EnvironmentController.GetModel();
		}

		private string GetTimeStampInSeconds()
		{
			DateTime utcNow = DateTime.UtcNow;
			return ((int)(utcNow - this.epochDate).TotalSeconds).ToString();
		}

		private string GetTimeStampInMilliseconds()
		{
			DateTime utcNow = DateTime.UtcNow;
			return ((long)(utcNow - this.epochDate).TotalMilliseconds).ToString();
		}

		private string IsNewUser()
		{
			if (Service.CurrentPlayer.FirstTimePlayer)
			{
				return "1";
			}
			return "0";
		}

		private string GetDeviceLocale()
		{
			if (string.IsNullOrEmpty(this.locale))
			{
				this.locale = Service.EnvironmentController.GetLocale();
			}
			return this.locale;
		}

		private string GetMemberOrNonMember()
		{
			if (Service.CurrentPlayer.Squad != null)
			{
				return "member";
			}
			return "nonmember";
		}

		private string GetLangLocale()
		{
			return Service.Lang.Locale;
		}
	}
}
