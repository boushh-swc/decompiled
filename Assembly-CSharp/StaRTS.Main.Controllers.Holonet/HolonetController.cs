using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Holonet;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Holonet
{
	public class HolonetController : IEventObserver
	{
		public delegate void HolonetPreparedDelegate();

		private const string HOLONET_RELOCATE_NO_PC = "HOLONET_RELOCATE_NO_PC";

		private const string HOLONET_REWARD_REDEEMED = "hn_reward_redeemed";

		private const string HOLONET_REWARD_CLAIMED = "hn_reward_claimed_desc";

		private const string HOLONET_NO_PC = "hn_noPC";

		private const string HOLONET_UPGRADE_REQUIRED = "hn_upgrade_required";

		private const int NO_NEW_ENTRIES = 0;

		private const string PLANET = "planet";

		private const string GALAXY = "galaxy";

		private const string HOME = "home";

		private const string WEB = "web";

		public const string REWARD = "reward";

		private const string STORE = "store";

		private const string BATTLE = "battle";

		private const string LEADER = "leader";

		private const string CONFLICT = "conflict";

		private const string CONFLICT_END = "conflictend";

		private const string HOLONET = "holonet";

		private const string STORY = "story";

		private const string RELOCATE = "relocate";

		private const string SQUADCHAT = "squadchat";

		private const string SQUADLIST = "squadlist";

		private const string WAR_BOARD = "gotowarboard";

		private const string SQUAD_LEVELUP = "squadlevelup";

		private const string DAILY_CRATE_REWARD = "dailycratereward";

		private HolonetController.HolonetPreparedDelegate preparedHandler;

		public CommandCenterHolonetController CommandCenterController;

		public DevNotesHolonetController DevNotesController;

		public TransmissionsHolonetController TransmissionsController;

		private Dictionary<HolonetControllerType, int> tabCounts;

		private Dictionary<HolonetControllerType, int> tabStamps;

		private HolonetScreen holonetScreen;

		public HolonetController()
		{
			Service.HolonetController = this;
			this.tabCounts = new Dictionary<HolonetControllerType, int>();
			this.tabStamps = new Dictionary<HolonetControllerType, int>();
			this.CommandCenterController = new CommandCenterHolonetController();
			this.DevNotesController = new DevNotesHolonetController();
			this.TransmissionsController = new TransmissionsHolonetController();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.PlanetRelocate, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.SquadJoinedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.SquadLeft);
		}

		public void PrepareContent(HolonetController.HolonetPreparedDelegate preparedHandler)
		{
			Service.EventManager.SendEvent(EventId.HolonetContentPrepareStarted, null);
			this.preparedHandler = preparedHandler;
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			this.tabStamps[HolonetControllerType.CommandCenter] = sharedPlayerPrefs.GetPref<int>("hn_ts_c");
			this.tabStamps[HolonetControllerType.DevNotes] = sharedPlayerPrefs.GetPref<int>("hn_ts_d");
			this.tabStamps[HolonetControllerType.Transmissions] = sharedPlayerPrefs.GetPref<int>("hn_ts_t");
			this.tabCounts.Clear();
			this.CommandCenterController.PrepareContent(this.tabStamps[this.CommandCenterController.ControllerType]);
			this.DevNotesController.PrepareContent(this.tabStamps[this.DevNotesController.ControllerType]);
			this.TransmissionsController.PrepareContent(this.tabStamps[this.TransmissionsController.ControllerType]);
		}

		public void ContentPrepared(IHolonetContoller controller, int newEntries)
		{
			this.tabCounts[controller.ControllerType] = newEntries;
			if (this.tabCounts.Count >= 3)
			{
				if (this.preparedHandler != null)
				{
					this.preparedHandler();
				}
				Service.EventManager.SendEvent(EventId.AllHolonetContentPrepared, null);
			}
			Service.EventManager.SendEvent(EventId.HolonetContentPrepared, null);
		}

		public void SetTabCount(IHolonetContoller controller, int newEntries)
		{
			this.tabCounts[controller.ControllerType] = newEntries;
		}

		public void InitBattlesTransmission(List<BattleEntry> battles)
		{
			this.TransmissionsController.InitBattlesTransmission(battles);
		}

		public bool HasNewBattles()
		{
			return this.TransmissionsController.HasNewBattles();
		}

		public int CalculateBadgeCount()
		{
			int num = 0;
			foreach (int current in this.tabCounts.Values)
			{
				num += current;
			}
			return num;
		}

		public void OpenHolonet()
		{
			this.OpenHolonet(false);
		}

		public void CloseHolonet()
		{
			this.holonetScreen.Close(null);
		}

		public void OpenHolonet(bool autoDisplayMCACrate)
		{
			if (!(Service.GameStateMachine.CurrentState is HomeState))
			{
				return;
			}
			this.holonetScreen = new HolonetScreen(this.tabCounts[HolonetControllerType.CommandCenter], this.tabCounts[HolonetControllerType.DevNotes], this.tabCounts[HolonetControllerType.Transmissions]);
			if (autoDisplayMCACrate)
			{
				this.holonetScreen.AutoDisplayMCACrate = true;
			}
			Service.ScreenController.AddScreen(this.holonetScreen);
		}

		public void RegisterForHolonetToReopenAfterCrateReward()
		{
			Service.EventManager.RegisterObserver(this, EventId.InventoryCrateCollectionClosed);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			EventManager eventManager = Service.EventManager;
			switch (id)
			{
			case EventId.SquadLeft:
			case EventId.SquadJoinedByCurrentPlayer:
				goto IL_53;
			case EventId.SquadChatFilterUpdated:
				IL_20:
				if (id == EventId.PlanetRelocate)
				{
					goto IL_53;
				}
				if (id != EventId.InventoryCrateCollectionClosed)
				{
					return EatResponse.NotEaten;
				}
				eventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
				this.OpenHolonet(true);
				return EatResponse.NotEaten;
			}
			goto IL_20;
			IL_53:
			this.PrepareContent(null);
			return EatResponse.NotEaten;
		}

		public void SetLastViewed(AbstractHolonetTab tab)
		{
			string text = string.Empty;
			if (tab is CommandCenterHolonetTab)
			{
				text = "hn_ts_c";
			}
			else if (tab is DevNotesHolonetTab)
			{
				text = "hn_ts_d";
			}
			else if (tab is TransmissionsHolonetTab)
			{
				text = "hn_ts_t";
			}
			if (string.IsNullOrEmpty(text))
			{
				Service.Logger.ErrorFormat("Cannot find a prefname for tab {0}", new object[]
				{
					tab.GetType().ToString()
				});
				return;
			}
			int serverTime = (int)Service.ServerAPI.ServerTime;
			Service.SharedPlayerPrefs.SetPref(text, serverTime.ToString());
			tab.SetBadgeCount(0);
			this.tabCounts[tab.HolonetControllerType] = 0;
		}

		public void EnableAllHolonetTabButtons()
		{
			this.holonetScreen.EnableAllTabButtons();
		}

		public void DisableAllHolonetTabButtons()
		{
			this.holonetScreen.DisableAllTabButtons();
		}

		public void UpdateIncomingTransmission(int index)
		{
			this.TransmissionsController.UpdateIncomingTransmission(index);
		}

		public void DismissIncomingTransmission(int index)
		{
			this.TransmissionsController.DismissIncomingTransmission(index);
		}

		public int CompareTimestamps(ITimestamped a, ITimestamped b)
		{
			return b.StartTime - a.StartTime;
		}

		public void ClearTimestamps()
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			sharedPlayerPrefs.SetPref("hn_ts_c", null);
			sharedPlayerPrefs.SetPref("hn_ts_d", null);
			sharedPlayerPrefs.SetPref("hn_ts_t", null);
		}

		private void GoToPlanet(string uid)
		{
			if (string.IsNullOrEmpty(uid))
			{
				uid = Service.CurrentPlayer.PlanetId;
			}
			Service.GalaxyViewController.GoToPlanetView(uid, CampaignScreenSection.Main);
		}

		private void GoToBattleLog()
		{
			this.holonetScreen.Close(null);
			Service.UXController.HUD.OpenBattleLog();
		}

		private void GoToLeaderBoard()
		{
			this.holonetScreen.Close(null);
			Service.UXController.HUD.OpenLeaderBoard();
		}

		private void GoToWarBoard()
		{
			this.holonetScreen.Close(null);
			Service.EventManager.SendEvent(EventId.WarLaunchFlow, null);
		}

		public void HandleCallToActionButton(string action, string data)
		{
			this.HandleCallToActionButton(action, data, null);
		}

		public void GoToSquadMessage()
		{
			this.holonetScreen.Close(null);
			Service.UXController.HUD.OpenSquadMessageScreen();
		}

		public void GoToFeatureSquad()
		{
			this.holonetScreen.Close(null);
			Service.UXController.HUD.OpenJoinSquadPanel();
			Service.EventManager.SendEvent(EventId.UISquadScreenTabShown, "holonet");
		}

		public void GoToSquadAdvancement()
		{
			this.holonetScreen.Close(null);
			Service.UXController.HUD.OpenSquadAdvancementScreen();
		}

		public void HandleCallToActionButton(string action, string data, object cookie)
		{
			if (Service.ScreenController.GetHighestLevelScreen<HolonetScreen>() == null)
			{
				Service.Logger.Warn("CTA actions only work in Holonet.");
				return;
			}
			switch (action)
			{
			case "conflict":
				this.OpenPlanetView(data);
				return;
			case "planet":
				this.OpenPlanetView(data);
				return;
			case "battle":
				this.GoToBattleLog();
				return;
			case "leader":
				this.GoToLeaderBoard();
				return;
			case "galaxy":
				this.GoToGalaxyView(data);
				return;
			case "home":
				this.holonetScreen.Close(null);
				return;
			case "gotowarboard":
				this.GoToWarBoard();
				return;
			case "web":
				GameUtils.OpenURL(data);
				return;
			case "reward":
			{
				string contextUid = (string)cookie;
				this.ClaimReward(data, contextUid);
				return;
			}
			case "conflictend":
				this.OpenPrizeInventoryScreen();
				return;
			case "store":
				Service.ScreenController.CloseAll();
				this.OpenStore(action, data);
				return;
			case "holonet":
				if (this.holonetScreen != null && this.holonetScreen.IsLoaded())
				{
					HolonetControllerType tabType = StringUtils.ParseEnum<HolonetControllerType>(data);
					this.holonetScreen.OpenTab(tabType);
				}
				return;
			case "story":
				this.holonetScreen.Close(null);
				new ActionChain(data);
				return;
			case "relocate":
				this.OpenRelocateView(data);
				return;
			case "squadchat":
				this.GoToSquadMessage();
				return;
			case "squadlist":
				this.GoToFeatureSquad();
				return;
			case "squadlevelup":
				this.GoToSquadAdvancement();
				return;
			case "dailycratereward":
				this.OpenPrizeInventoryScreen();
				return;
			}
			Service.Logger.Warn("Unknown button1 action: " + action + " data: " + data);
		}

		private void ClaimReward(string limitedTimeRewardUid, string contextUid)
		{
			if (!string.IsNullOrEmpty(contextUid))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				LimitedTimeRewardVO optional = staticDataController.GetOptional<LimitedTimeRewardVO>(limitedTimeRewardUid);
				if (optional == null)
				{
					Service.Logger.WarnFormat("No LimitedTimeReward data found for uid: {0}", new object[]
					{
						limitedTimeRewardUid
					});
					return;
				}
				RewardVO optional2 = staticDataController.GetOptional<RewardVO>(optional.RewardUid);
				if (optional2 == null)
				{
					Service.Logger.WarnFormat("LimitedTimeReward {0} points to reward {1} but it was not found", new object[]
					{
						limitedTimeRewardUid,
						optional.RewardUid
					});
					return;
				}
				HolonetClaimRewardRequest request = new HolonetClaimRewardRequest(limitedTimeRewardUid, contextUid);
				HolonetClaimRewardCommand command = new HolonetClaimRewardCommand(request);
				Service.ServerAPI.Sync(command);
				GameUtils.AddRewardToInventory(optional2);
				Service.CurrentPlayer.AddHolonetReward(limitedTimeRewardUid);
				Lang lang = Service.Lang;
				string text = lang.Get(optional.Title, new object[0]);
				AlertScreen.ShowModal(false, lang.Get("hn_reward_redeemed", new object[0]), lang.Get("hn_reward_claimed_desc", new object[]
				{
					text
				}), null, null);
			}
		}

		private void OpenPlanetView(string data)
		{
			string text = Service.CurrentPlayer.PlanetId;
			CampaignScreenSection section = CampaignScreenSection.Main;
			bool flag = Service.BuildingLookupController.HasNavigationCenter();
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array.Length >= 1)
				{
					text = array[0];
				}
			}
			if (flag || text == "planet1")
			{
				this.holonetScreen.Close(null);
				Service.GalaxyViewController.GoToPlanetView(text, section);
			}
			else
			{
				AlertScreen.ShowModal(false, Service.Lang.Get("hn_upgrade_required", new object[0]), Service.Lang.Get("hn_noPC", new object[0]), null, null);
			}
		}

		private void GoToGalaxyView(string data)
		{
			bool flag = Service.BuildingLookupController.HasNavigationCenter();
			if (flag)
			{
				this.holonetScreen.Close(null);
				Service.GalaxyViewController.GoToGalaxyView(data);
			}
			else
			{
				AlertScreen.ShowModal(false, Service.Lang.Get("hn_upgrade_required", new object[0]), Service.Lang.Get("hn_noPC", new object[0]), null, null);
			}
		}

		private void OpenRelocateView(string data)
		{
			string text = Service.CurrentPlayer.PlanetId;
			CampaignScreenSection section = CampaignScreenSection.Main;
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array.Length >= 1)
				{
					text = array[0];
				}
			}
			bool flag = Service.BuildingLookupController.HasNavigationCenter();
			if (flag)
			{
				if (this.holonetScreen != null)
				{
					this.holonetScreen.Close(null);
				}
				Service.GalaxyViewController.GoToPlanetView(text, section);
				PlanetVO planetVO = Service.StaticDataController.Get<PlanetVO>(text);
				ConfirmRelocateScreen.ShowModal(planetVO, null, null);
			}
			else
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(Service.Lang.Get("HOLONET_RELOCATE_NO_PC", new object[0]));
			}
		}

		private void OpenStore(string action, string data)
		{
			StoreScreen storeScreen = new StoreScreen();
			string[] array = data.Split(new char[]
			{
				'|'
			});
			if (array.Length > 0)
			{
				StoreTab tab = StringUtils.ParseEnum<StoreTab>(array[0]);
				storeScreen.OpenStoreTab(tab);
			}
			Service.ScreenController.AddScreen(storeScreen);
			if (array.Length > 1)
			{
				storeScreen.ScrollToItem(array[1]);
			}
		}

		private void OpenPrizeInventoryScreen()
		{
			this.holonetScreen.Close(null);
			ScreenBase screenBase = Service.UXController.HUD.CreatePrizeInventoryScreen();
			if (screenBase != null)
			{
				Service.ScreenController.AddScreen(screenBase);
			}
		}
	}
}
