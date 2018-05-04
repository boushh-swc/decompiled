using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class BattleLogScreen : ClosableScreen, IEventObserver
	{
		private const string TAB_DEFENSE = "TabDefense";

		private const string TAB_ATTACK = "TabAttack";

		private const string ITEM_GRID = "BattleLogGrid";

		private const string ITEM_TEMPLATE = "BattleLogItem";

		private const string ITEM_REPLAY_BUTTON = "BtnReplay";

		private const string ITEM_REVENGE_BUTTON = "BtnRevenge";

		private const string ITEM_SHARE_BUTTON = "BtnShare";

		private const string ITEM_CREDITS_LABEL = "LabelCredits";

		private const string ITEM_MATERIALS_LABEL = "LabelMaterials";

		private const string ITEM_CONTRABAND = "Contraband";

		private const string ITEM_CONTRABAND_LABEL = "LabelContraband";

		private const string ITEM_MEDALS_LABEL = "LabelBasePoints";

		private const string ITEM_DAMAGE_LABEL = "LabelDamage";

		private const string ITEM_RESULT_LABEL = "LabelResult";

		private const string ITEM_TIMESTAMP_LABEL = "LabelTimeStamp";

		private const string ITEM_PLAYER_NAME = "LabelPlayerName";

		private const string ITEM_PLAYER_GUILD = "LabelFactionName";

		private const string ITEM_PLAYER_ICON = "SpriteFaction";

		private const string ITEM_FACTION_PREFIX = "Faction";

		private const string ITEM_EXPENDED_GRID = "TroopsExpendedGrid";

		private const string ITEM_EXPENDED_TEMPLATE = "TroopsExpendedTemplate";

		private const string ITEM_EXPENDED_CARD = "TroopsExpendedCard";

		private const string ITEM_EXPENDED_COUNT = "LabelTroopsExpended";

		private const string ITEM_EXPENDED_ICON = "SpriteTroopsImage";

		private const string ITEM_EXPENDED_LEVEL = "LabelTroopLevel";

		private const string ITEM_TROOPS_EXPENDED_DEFAULT = "TroopsExpendBgDefault";

		private const string ITEM_TROOPS_EXPENDED_PREFIX = "TroopsExpendBgQ{0}";

		private const string ITEM_TOURNAMENT_RATING = "TournamentRating";

		private const string ITEM_TOURNAMENT_RATING_LABEL = "LabelTournamentRating";

		private const string ITEM_TOURNAMENT_RATING_SPRITE = "SpriteTournamentRating";

		private const string LABEL_TAB_ATTACK = "LabelTabAttack";

		private const string LABEL_TAB_DEFENSE = "LabelTabDefense";

		private const string LABEL_SHARE_BUTTON = "LabelBtnShare";

		private const string LABEL_REPLAY_BUTTON = "LabelBtnReplay";

		private const string LABEL_REVENGE_BUTTON = "LabelBtnRevenge";

		private const string LABEL_TITLE = "DialogTrainingTitle";

		private const string BATTLE_LOG_PLANET_BG = "TexturePlanetBg";

		private const int MAX_TROOPS = 8;

		private const int MAX_SHOWN_ATTACKS = 10;

		private const int MAX_SHOWN_DEFENCES = 20;

		private UXGrid itemGrid;

		private List<UXCheckbox> tabs;

		private UXLabel tabAttackLabel;

		private UXLabel tabDefenseLabel;

		private List<UXButton> revengeButtons;

		private BattleLogTab curTab;

		private TroopTooltipHelper troopTooltipHelper;

		private StaticDataController sdc;

		[CompilerGenerated]
		private static Comparison<BattleEntry> <>f__mg$cache0;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public BattleLogScreen() : base("gui_battle_log")
		{
			this.troopTooltipHelper = new TroopTooltipHelper();
			this.sdc = Service.StaticDataController;
			this.SetTab(BattleLogTab.Defense);
		}

		public override void OnDestroyElement()
		{
			if (this.itemGrid != null)
			{
				this.itemGrid.Clear();
				this.itemGrid = null;
			}
			this.troopTooltipHelper.Destroy();
			this.troopTooltipHelper = null;
			if (this.revengeButtons != null)
			{
				this.revengeButtons.Clear();
				this.revengeButtons = null;
			}
			base.OnDestroyElement();
		}

		protected override void OnScreenLoaded()
		{
			Service.BuildingController.CancelEditModeTimer();
			this.InitButtons();
			this.SetTab(this.curTab);
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.tabs = new List<UXCheckbox>();
			this.SetupTab(BattleLogTab.Defense, "TabDefense");
			this.SetupTab(BattleLogTab.Attack, "TabAttack");
			this.itemGrid = base.GetElement<UXGrid>("BattleLogGrid");
			this.itemGrid.SetTemplateItem("BattleLogItem");
			this.tabAttackLabel = base.GetElement<UXLabel>("LabelTabAttack");
			this.tabAttackLabel.Text = this.lang.Get("BUTTON_BATTLE_LOG_ATTACK", new object[0]);
			this.tabDefenseLabel = base.GetElement<UXLabel>("LabelTabDefense");
			this.tabDefenseLabel.Text = this.lang.Get("s_Defense", new object[0]);
			UXLabel element = base.GetElement<UXLabel>("DialogTrainingTitle");
			element.Text = this.lang.Get("s_BattleLog", new object[0]);
		}

		private void SetupTab(BattleLogTab tab, string tabName)
		{
			UXCheckbox element = base.GetElement<UXCheckbox>(tabName);
			element.OnSelected = new UXCheckboxSelectedDelegate(this.OnTabCheckboxSelected);
			element.Tag = tab;
			this.tabs.Add(element);
		}

		public void SetTab(BattleLogTab tab)
		{
			this.curTab = tab;
			if (!base.IsLoaded())
			{
				return;
			}
			int i = 0;
			int count = this.tabs.Count;
			while (i < count)
			{
				UXCheckbox uXCheckbox = this.tabs[i];
				uXCheckbox.Selected = (this.curTab == (BattleLogTab)uXCheckbox.Tag);
				i++;
			}
			this.SetupCurTabElements();
		}

		private void SetupCurTabElements()
		{
			this.itemGrid.Clear();
			List<BattleEntry> list = null;
			this.tabAttackLabel.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			this.tabDefenseLabel.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			int num = 20;
			BattleLogTab battleLogTab = this.curTab;
			if (battleLogTab != BattleLogTab.Defense)
			{
				if (battleLogTab == BattleLogTab.Attack)
				{
					list = this.GetBattleLogEntries(true);
					num = 10;
					this.tabAttackLabel.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
				}
			}
			else
			{
				list = this.GetBattleLogEntries(false);
				num = 20;
				this.tabDefenseLabel.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
			}
			if (list != null)
			{
				List<BattleEntry> arg_AD_0 = list;
				if (BattleLogScreen.<>f__mg$cache0 == null)
				{
					BattleLogScreen.<>f__mg$cache0 = new Comparison<BattleEntry>(BattleLogScreen.CompareBattleLogEntry);
				}
				arg_AD_0.Sort(BattleLogScreen.<>f__mg$cache0);
				if (list.Count > num)
				{
					int num2 = list.Count - num;
					list.RemoveRange(list.Count - num2, num2);
				}
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.AddBattleLogItem(list[i]);
					i++;
				}
			}
			this.itemGrid.RepositionItems();
		}

		private static int CompareBattleLogEntry(BattleEntry a, BattleEntry b)
		{
			if (a == b)
			{
				return 0;
			}
			return (a.EndBattleServerTime >= b.EndBattleServerTime) ? -1 : 1;
		}

		private void OnTabCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			BattleLogTab battleLogTab = (BattleLogTab)checkbox.Tag;
			if (battleLogTab != this.curTab)
			{
				this.SetTab(battleLogTab);
				Service.EventManager.SendEvent(EventId.BattleLogScreenTabSelected, battleLogTab);
			}
		}

		private void AddBattleLogItem(BattleEntry entry)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			bool flag = currentPlayer.PlayerId == entry.AttackerID;
			BattleParticipant battleParticipant;
			BattleParticipant battleParticipant2;
			if (flag)
			{
				battleParticipant = entry.Attacker;
				battleParticipant2 = entry.Defender;
			}
			else
			{
				battleParticipant = entry.Defender;
				battleParticipant2 = entry.Attacker;
			}
			string playerName = battleParticipant2.PlayerName;
			string guildName = battleParticipant2.GuildName;
			FactionType factionType = battleParticipant2.PlayerFaction;
			if (factionType == FactionType.Invalid)
			{
				factionType = GameUtils.GetOppositeFaction(currentPlayer.Faction);
			}
			int num = GameUtils.CalcuateMedals(battleParticipant.AttackRating, battleParticipant.DefenseRating);
			int num2 = GameUtils.CalcuateMedals(battleParticipant.AttackRating + battleParticipant.AttackRatingDelta, battleParticipant.DefenseRating + battleParticipant.DefenseRatingDelta);
			int value = num2 - num;
			int tournamentRatingDelta = battleParticipant.TournamentRatingDelta;
			bool flag2 = GameUtils.IsBattleVersionSupported(entry.CmsVersion, entry.BattleVersion);
			bool flag3 = Service.SquadController.StateManager.GetCurrentSquad() != null;
			bool visible = flag2;
			bool visible2 = !flag && entry.IsPvP() && !entry.Revenged && entry.PlanetId == currentPlayer.PlanetId;
			bool visible3 = flag2 && flag3;
			uint time = ServerTime.Time;
			int totalSeconds = (int)(time - entry.EndBattleServerTime);
			int count = this.itemGrid.Count;
			string text = string.Format("{0}{1}", "BattleLogItem", count);
			UXElement item = this.itemGrid.CloneTemplateItem(text);
			bool won = entry.Won;
			Color winResultColor = UXUtils.GetWinResultColor(won);
			UXButton subElement = this.itemGrid.GetSubElement<UXButton>(text, "BtnReplay");
			subElement.Tag = entry;
			subElement.OnClicked = new UXButtonClickedDelegate(this.OnReplayButtonClicked);
			subElement.Visible = visible;
			UXButton subElement2 = this.itemGrid.GetSubElement<UXButton>(text, "BtnRevenge");
			subElement2.Tag = entry;
			subElement2.OnClicked = new UXButtonClickedDelegate(this.OnRevengeButtonClicked);
			subElement2.Visible = visible2;
			if (this.revengeButtons == null)
			{
				this.revengeButtons = new List<UXButton>();
			}
			this.revengeButtons.Add(subElement2);
			UXButton subElement3 = this.itemGrid.GetSubElement<UXButton>(text, "BtnShare");
			subElement3.Tag = entry;
			subElement3.OnClicked = new UXButtonClickedDelegate(this.OnShareButtonClicked);
			subElement3.Visible = visible3;
			UXLabel subElement4 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelBtnShare");
			subElement4.Text = this.lang.Get("s_Share", new object[0]);
			UXLabel subElement5 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelBtnReplay");
			subElement5.Text = this.lang.Get("BUTTON_REPLAY", new object[0]);
			UXLabel subElement6 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelBtnRevenge");
			subElement6.Text = this.lang.Get("s_Revenge", new object[0]);
			bool flag4 = this.curTab == BattleLogTab.Attack;
			int value2 = (!flag4) ? entry.LootCreditsDeducted : entry.LootCreditsEarned;
			int value3 = (!flag4) ? entry.LootMaterialsDeducted : entry.LootMaterialsEarned;
			int value4 = (!flag4) ? entry.LootContrabandDeducted : entry.LootContrabandEarned;
			UXLabel subElement7 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelCredits");
			subElement7.Text = this.lang.ThousandsSeparated(value2);
			UXLabel subElement8 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelMaterials");
			subElement8.Text = this.lang.ThousandsSeparated(value3);
			UXElement subElement9 = this.itemGrid.GetSubElement<UXElement>(text, "Contraband");
			if (currentPlayer.IsContrabandUnlocked)
			{
				subElement9.Visible = true;
				UXLabel subElement10 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelContraband");
				subElement10.Text = this.lang.ThousandsSeparated(value4);
			}
			else
			{
				subElement9.Visible = false;
			}
			UXLabel subElement11 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelBasePoints");
			subElement11.Text = this.lang.ThousandsSeparated(value);
			subElement11.TextColor = winResultColor;
			UXLabel subElement12 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelDamage");
			subElement12.Text = this.lang.Get("DAMAGE_PERCENT", new object[]
			{
				entry.DamagePercent
			});
			UXLabel subElement13 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelResult");
			subElement13.Text = this.lang.Get((!won) ? "YOU_LOST" : "YOU_WON", new object[0]);
			subElement13.TextColor = winResultColor;
			UXLabel subElement14 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelTimeStamp");
			subElement14.Text = this.lang.Get("TIME_AGO", new object[]
			{
				GameUtils.GetTimeLabelFromSeconds(totalSeconds)
			});
			UXLabel subElement15 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelPlayerName");
			subElement15.Text = playerName;
			UXLabel subElement16 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelFactionName");
			subElement16.Text = guildName;
			UXSprite subElement17 = this.itemGrid.GetSubElement<UXSprite>(text, "SpriteFaction");
			subElement17.SpriteName = "Faction" + factionType.ToString();
			if (!string.IsNullOrEmpty(entry.PlanetId))
			{
				PlanetVO planetVO = this.sdc.Get<PlanetVO>(entry.PlanetId);
				UXTexture subElement18 = this.itemGrid.GetSubElement<UXTexture>(text, "TexturePlanetBg");
				subElement18.LoadTexture(planetVO.LeaderboardTileTexture);
			}
			UXGrid subElement19 = this.itemGrid.GetSubElement<UXGrid>(text, "TroopsExpendedGrid");
			UXElement subElement20 = this.itemGrid.GetSubElement<UXElement>(text, "TroopsExpendedTemplate");
			subElement19.SetTemplateItem(subElement20.Root.name);
			this.AddExpendedItems(entry, text, subElement19);
			subElement19.RepositionItems();
			bool flag5 = tournamentRatingDelta != 0;
			UXElement subElement21 = this.itemGrid.GetSubElement<UXElement>(text, "TournamentRating");
			subElement21.Visible = flag5;
			if (!string.IsNullOrEmpty(entry.PlanetId))
			{
				string tournamentPointIconName = GameUtils.GetTournamentPointIconName(entry.PlanetId);
				if (flag5 && !string.IsNullOrEmpty(tournamentPointIconName))
				{
					UXSprite subElement22 = this.itemGrid.GetSubElement<UXSprite>(text, "SpriteTournamentRating");
					subElement22.SpriteName = tournamentPointIconName;
					UXLabel subElement23 = this.itemGrid.GetSubElement<UXLabel>(text, "LabelTournamentRating");
					subElement23.Text = tournamentRatingDelta.ToString();
					subElement23.TextColor = winResultColor;
				}
			}
			this.itemGrid.AddItem(item, count);
		}

		private int GetItemQuality(TroopTypeVO troop, List<string> activeEquipment, DeployableShardUnlockController shardUnlockCtrl)
		{
			string text = null;
			Service.SkinController.GetApplicableSkin(troop, activeEquipment, out text);
			int result;
			if (!string.IsNullOrEmpty(text))
			{
				EquipmentVO equipmentVO = this.sdc.Get<EquipmentVO>(text);
				result = (int)equipmentVO.Quality;
			}
			else
			{
				result = shardUnlockCtrl.GetUpgradeQualityForDeployable(troop);
			}
			return result;
		}

		private void AddExpendedItems(BattleEntry entry, string entryItemUid, UXGrid expendedGrid)
		{
			BattleDeploymentData attackerDeployedData = entry.AttackerDeployedData;
			DeployableShardUnlockController deployableShardUnlockController = Service.DeployableShardUnlockController;
			if (attackerDeployedData == null)
			{
				return;
			}
			Dictionary<string, int> dictionary = null;
			if (attackerDeployedData.SquadData != null)
			{
				dictionary = new Dictionary<string, int>(attackerDeployedData.SquadData);
			}
			if (attackerDeployedData.TroopData != null)
			{
				foreach (KeyValuePair<string, int> current in attackerDeployedData.TroopData)
				{
					string key = current.Key;
					int num = 0;
					if (dictionary != null && dictionary.ContainsKey(key))
					{
						num = dictionary[key];
						dictionary.Remove(key);
					}
					num += current.Value;
					TroopTypeVO troop = this.sdc.Get<TroopTypeVO>(key);
					int unitQuality = this.GetItemQuality(troop, entry.AttackerEquipment, deployableShardUnlockController);
					this.AddExpendedItem(expendedGrid, entryItemUid, key, troop, num, unitQuality, entry);
				}
			}
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, int> current2 in dictionary)
				{
					if (current2.Value >= 1)
					{
						TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(current2.Key);
						int unitQuality = deployableShardUnlockController.GetUpgradeQualityForDeployable(troopTypeVO);
						this.AddExpendedItem(expendedGrid, entryItemUid, current2.Key, troopTypeVO, current2.Value, unitQuality, entry);
					}
				}
			}
			if (attackerDeployedData.SpecialAttackData != null)
			{
				foreach (KeyValuePair<string, int> current3 in attackerDeployedData.SpecialAttackData)
				{
					string key2 = current3.Key;
					int value = current3.Value;
					SpecialAttackTypeVO specialAttackTypeVO = this.sdc.Get<SpecialAttackTypeVO>(key2);
					int unitQuality = deployableShardUnlockController.GetUpgradeQualityForDeployable(specialAttackTypeVO);
					this.AddExpendedItem(expendedGrid, entryItemUid, key2, specialAttackTypeVO, value, unitQuality, entry);
				}
			}
			if (attackerDeployedData.HeroData != null)
			{
				foreach (KeyValuePair<string, int> current4 in attackerDeployedData.HeroData)
				{
					string key3 = current4.Key;
					int value2 = current4.Value;
					TroopTypeVO troop2 = this.sdc.Get<TroopTypeVO>(key3);
					int unitQuality = this.GetItemQuality(troop2, entry.AttackerEquipment, deployableShardUnlockController);
					this.AddExpendedItem(expendedGrid, entryItemUid, key3, troop2, value2, unitQuality, entry);
				}
			}
			if (attackerDeployedData.ChampionData != null)
			{
				foreach (KeyValuePair<string, int> current5 in attackerDeployedData.ChampionData)
				{
					string key4 = current5.Key;
					int value3 = current5.Value;
					TroopTypeVO troop3 = this.sdc.Get<TroopTypeVO>(key4);
					int unitQuality = 0;
					this.AddExpendedItem(expendedGrid, entryItemUid, key4, troop3, value3, unitQuality, entry);
				}
			}
		}

		private void AddExpendedItem(UXGrid expendedGrid, string entryItemUid, string troopUid, IUpgradeableVO troop, int count, int unitQuality, BattleEntry battleEntry)
		{
			if (expendedGrid.Count == 8)
			{
				return;
			}
			if (count <= 0)
			{
				return;
			}
			UXElement uXElement = expendedGrid.CloneTemplateItem(troopUid);
			uXElement.Root.name = UXUtils.FormatAppendedName(uXElement.Root.name, entryItemUid);
			UXLabel subElement = expendedGrid.GetSubElement<UXLabel>(troopUid, UXUtils.FormatAppendedName("LabelTroopsExpended", entryItemUid));
			subElement.Text = ((count <= 0) ? string.Empty : this.lang.Get("TROOP_MULTIPLIER", new object[]
			{
				count
			}));
			UXLabel subElement2 = expendedGrid.GetSubElement<UXLabel>(troopUid, UXUtils.FormatAppendedName("LabelTroopLevel", entryItemUid));
			subElement2.Text = LangUtils.GetLevelText(troop.Lvl);
			UXSprite subElement3 = expendedGrid.GetSubElement<UXSprite>(troopUid, UXUtils.FormatAppendedName("SpriteTroopsImage", entryItemUid));
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(troop, subElement3);
			Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(troop, projectorConfig, battleEntry));
			UXUtils.SetupGeometryForIcon(subElement3, projectorConfig);
			string defaultCardName = UXUtils.FormatAppendedName("TroopsExpendBgDefault", entryItemUid);
			string cardName = UXUtils.FormatAppendedName("TroopsExpendBgQ{0}", entryItemUid);
			UXUtils.SetCardQuality(this, expendedGrid, troopUid, unitQuality, cardName, defaultCardName);
			FactionDecal.UpdateDeployableDecal(troopUid, entryItemUid, expendedGrid, (IDeployableVO)troop);
			UXButton subElement4 = expendedGrid.GetSubElement<UXButton>(troopUid, UXUtils.FormatAppendedName("TroopsExpendedCard", entryItemUid));
			this.troopTooltipHelper.RegisterButtonTooltip(subElement4, troop, battleEntry);
			expendedGrid.AddItem(uXElement, troop.Order);
		}

		private void OnReplayButtonClicked(UXButton button)
		{
			GameUtils.ExitEditState();
			BattleEntry battleEntry = button.Tag as BattleEntry;
			Service.PvpManager.ReplayBattle(battleEntry.RecordID, battleEntry.Defender, null);
			Service.EventManager.SendEvent(EventId.BattleLogScreenReplayButtonClicked, null);
		}

		private void ToggleRevengeButtons(bool enabled)
		{
			if (this.revengeButtons != null)
			{
				int i = 0;
				int count = this.revengeButtons.Count;
				while (i < count)
				{
					this.revengeButtons[i].Enabled = enabled;
					i++;
				}
			}
		}

		private void OnRevengeButtonClicked(UXButton button)
		{
			this.ToggleRevengeButtons(false);
			if (Service.CurrentPlayer.Inventory.Troop.GetTotalStorageAmount() <= 0 && Service.CurrentPlayer.Inventory.SpecialAttack.GetTotalStorageAmount() <= 0 && Service.CurrentPlayer.Inventory.Hero.GetTotalStorageAmount() <= 0 && Service.CurrentPlayer.Inventory.Champion.GetTotalStorageAmount() <= 0)
			{
				AlertScreen.ShowModal(false, this.lang.Get("NOT_ENOUGH_TROOPS_TITLE", new object[0]), this.lang.Get("NOT_ENOUGH_TROOPS_FOR_ATTACK", new object[0]), null, null);
				Service.EventManager.SendEvent(EventId.UIAttackScreenSelection, new ActionMessageBIData("PvP", "no_troops"));
				return;
			}
			if (Service.CurrentPlayer.ProtectedUntil > ServerTime.Time)
			{
				DisableProtectionAlertScreen.ShowModal(new OnScreenModalResult(this.OnConfirmInvalidation), button.Tag);
			}
			else
			{
				GameUtils.ExitEditState();
				this.StartRevenge(button.Tag as BattleEntry);
			}
		}

		private void OnConfirmInvalidation(object result, object cookie)
		{
			if (result != null)
			{
				this.StartRevenge(cookie as BattleEntry);
			}
			else
			{
				this.ToggleRevengeButtons(true);
			}
		}

		private void StartRevenge(BattleEntry entry)
		{
			Service.EventManager.RegisterObserver(this, EventId.PvpRevengeOpponentNotFound);
			Service.EventManager.SendEvent(EventId.BattleLogScreenRevengeButtonClicked, null);
			Service.PvpManager.StartRevenge(entry.Attacker.PlayerId);
		}

		private void OnShareButtonClicked(UXButton button)
		{
			BattleEntry battleEntry = button.Tag as BattleEntry;
			Service.EventManager.SendEvent(EventId.BattleLogScreenShareButtonClicked, null);
			Service.ScreenController.AddScreen(new BattleReplayShareScreen(battleEntry));
		}

		private List<BattleEntry> GetBattleLogEntries(bool playerAttacked)
		{
			List<BattleEntry> list = new List<BattleEntry>();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			List<BattleEntry> battleHistory = currentPlayer.BattleHistory.GetBattleHistory();
			int i = 0;
			int count = battleHistory.Count;
			while (i < count)
			{
				BattleEntry battleEntry = battleHistory[i];
				if (battleEntry.IsPvP() && playerAttacked == (currentPlayer.PlayerId == battleEntry.AttackerID))
				{
					list.Add(battleEntry);
				}
				i++;
			}
			return list;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.PvpRevengeOpponentNotFound)
			{
				Service.EventManager.UnregisterObserver(this, EventId.PvpRevengeOpponentNotFound);
				this.ToggleRevengeButtons(true);
			}
			return base.OnEvent(id, cookie);
		}
	}
}
