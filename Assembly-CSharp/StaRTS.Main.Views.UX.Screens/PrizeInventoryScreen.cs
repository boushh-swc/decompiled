using Net.RichardLord.Ash.Core;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class PrizeInventoryScreen : ClosableScreen, IEventObserver, IViewClockTimeObserver
	{
		private const string GO_TO_STORE_ITEM_ID = "Id_Store";

		private const int GO_TO_STORE_DONT_SHOW = 0;

		private const int GO_TO_STORE_INSERT_FIRST = 1;

		private const int GO_TO_STORE_INSERT_LAST = 2;

		private const string INVENTORY_NO_ITEM = "INVENTORY_NO_ITEM";

		private const string INVENTORY_TAB_TROOPS = "INVENTORY_TAB_TROOPS";

		private const string INVENTORY_TAB_BUILDINGS = "INVENTORY_TAB_BUILDINGS";

		private const string INVENTORY_TAB_CURRENCY = "INVENTORY_TAB_CURRENCY";

		private const string INVENTORY_TAB_CRATES = "INVENTORY_TAB_CRATES";

		private const string CRATE_INVENTORY_CRATE_TITLE = "CRATE_INVENTORY_CRATE_TITLE";

		private const string CRATE_INVENTORY_TO_STORE_TITLE = "CRATE_INVENTORY_TO_STORE_TITLE";

		private const string CRATE_INVENTORY_TO_STORE_DESC = "CRATE_INVENTORY_TO_STORE_DESC";

		private const string CRATE_BADGE_NEW = "CRATE_INVENTORY_CRATE_NEW_BADGE";

		private const string ITEM_GRID = "InventoryGrid";

		private const string ITEM_GRID_PARENT = "InventoryItems";

		private const string ITEM_GROUP_COUNTS = "CountAndBuildTime";

		private const string ITEM_TEMPLATE = "InventoryTemplate";

		private const string ITEM_MAIN_ELEMENT = "ItemInfo";

		private const string ITEM_BUTTON = "InventoryCard";

		private const string ITEM_BUTTON_INFO = "BtnItemInfo";

		private const string ITEM_BADGE_NEW = "ContainerJewelNew";

		private const string ITEM_BADGE_NEW_LABEL = "LabelMessageCountNew";

		private const string LABEL_INVENTORY_TITLE = "LabelInventoryTitle";

		private const string LABEL_INVENTORY_EMPTY_CATEGORY = "LabelEmptyCategory";

		private const string ITEM_LABEL_BUILD_TIME = "LabelBuildTime";

		private const string ITEM_LABEL_COUNT = "LabelItemCount";

		private const string ITEM_LABEL_EXPIRATION = "LabelItemExpiration";

		private const string ITEM_LABEL_INFO = "LabelItemInfo";

		private const string ITEM_LABEL_NAME = "LabelName";

		private const string ITEM_LABEL_REQ = "LabelItemRequirement";

		private const string ITEM_LABEL_REWARD = "LabelCurrencyAmount";

		private const string ITEM_ICON = "SpriteItemImage";

		private const string ITEM_SPRITE_ITEM_TIME_ICON = "SpriteItemTimeIcon";

		private const string BUTTON_BUILDINGS_LABEL = "LabelBuildings";

		private const string BUTTON_CURRENCY_LABEL = "LabelCurrency";

		private const string BUTTON_TROOPS_LABEL = "LabelTroops";

		private const string BUTTON_CRATES_LABEL = "LabelCrates";

		private const string TAB_BUTTON_BUILDING = "BtnBuildings";

		private const string TAB_BUTTON_CURRENCY = "BtnCurrency";

		private const string TAB_BUTTON_TROOP = "BtnTroops";

		private const string TAB_BUTTON_CRATE = "BtnCrates";

		private const string ITEM_PACK_CURRENCY = "PACK_CURRENCY{0}";

		private const string REWARD_COUNT_TEXT = "x{0}";

		private const string CRATE_BG_PLANET = "TexturePlanet";

		private const int INVENTORY_QUALITY_MAX = 3;

		private const string INVENTORY_QUALITY_PREFIX = "QualityCardQ{0}";

		private const string INVENTORY_DEFAULT_QUALITY = "Default";

		private SharedPlayerPrefs prefs;

		private uint crateTabLastViewedTime;

		private UXGrid itemGrid;

		private List<UXCheckbox> tabs;

		private InventoryTab curTab;

		private UXLabel emptyCategoryLabel;

		private UXLabel labelBuildings;

		private UXLabel labelCurrency;

		private UXLabel labelTroops;

		private UXLabel labelCrates;

		private JewelControl cratesJewel;

		private JewelControl buildingsJewel;

		private JewelControl troopsJewel;

		private JewelControl currencyJewel;

		private CurrentPlayer cp;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public PrizeInventoryScreen() : base("gui_inventory_2row")
		{
			this.cp = Service.CurrentPlayer;
		}

		protected override void OnScreenLoaded()
		{
			this.InitLabels();
			this.InitTabJewels();
			this.InitButtons();
			this.prefs = Service.SharedPlayerPrefs;
			this.crateTabLastViewedTime = this.prefs.GetPref<uint>("crtInvLastViewed");
			this.SetTab(InventoryTab.Crate);
		}

		private void InitLabels()
		{
			this.emptyCategoryLabel = base.GetElement<UXLabel>("LabelEmptyCategory");
			this.labelBuildings = base.GetElement<UXLabel>("LabelBuildings");
			this.labelBuildings.Text = this.lang.Get("INVENTORY_TAB_BUILDINGS", new object[0]);
			this.labelCurrency = base.GetElement<UXLabel>("LabelCurrency");
			this.labelCurrency.Text = this.lang.Get("INVENTORY_TAB_CURRENCY", new object[0]);
			this.labelTroops = base.GetElement<UXLabel>("LabelTroops");
			this.labelTroops.Text = this.lang.Get("INVENTORY_TAB_TROOPS", new object[0]);
			this.labelCrates = base.GetElement<UXLabel>("LabelCrates");
			this.labelCrates.Text = this.lang.Get("INVENTORY_TAB_CRATES", new object[0]);
			base.GetElement<UXLabel>("LabelInventoryTitle").Text = this.lang.Get("context_Inventory", new object[0]);
		}

		private void InitTabJewels()
		{
			this.cratesJewel = JewelControl.Create(this, "Crates");
			this.buildingsJewel = JewelControl.Create(this, "Buildings");
			this.troopsJewel = JewelControl.Create(this, "Troops");
			this.currencyJewel = JewelControl.Create(this, "Currency");
			this.UpdateTabJewels();
		}

		private void UpdateTabJewels()
		{
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			this.cratesJewel.Value = Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryCratesNotViewed));
			this.buildingsJewel.Value = 0;
			this.troopsJewel.Value = Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryTroopsNotViewed));
			this.currencyJewel.Value = Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryCurrencyNotViewed));
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.tabs = new List<UXCheckbox>();
			this.itemGrid = base.GetElement<UXGrid>("InventoryGrid");
			this.itemGrid.SetTemplateItem("InventoryTemplate");
			this.SetupTab(InventoryTab.Building, "BtnBuildings");
			this.SetupTab(InventoryTab.Treasure, "BtnCurrency");
			this.SetupTab(InventoryTab.Troop, "BtnTroops");
			this.SetupTab(InventoryTab.Crate, "BtnCrates");
		}

		private void SetupTab(InventoryTab tab, string tabName)
		{
			UXCheckbox element = base.GetElement<UXCheckbox>(tabName);
			this.SetupTab(tab, element);
		}

		private void SetupTab(InventoryTab tab, UXCheckbox tabButton)
		{
			tabButton.OnSelected = new UXCheckboxSelectedDelegate(this.OnTabCheckboxSelected);
			tabButton.Tag = tab;
			this.tabs.Add(tabButton);
		}

		private void OnTabCheckboxSelected(UXCheckbox checkbox, bool selected)
		{
			InventoryTab inventoryTab = (InventoryTab)((int)checkbox.Tag);
			if (!selected)
			{
				return;
			}
			if (inventoryTab != this.curTab)
			{
				if (this.curTab == InventoryTab.Crate)
				{
					this.prefs.SetPref("crtInvLastViewed", ServerTime.Time.ToString());
					this.crateTabLastViewedTime = ServerTime.Time;
				}
				this.SetTab(inventoryTab);
			}
		}

		public void SetTab(InventoryTab tab)
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
				uXCheckbox.Selected = (this.curTab == (InventoryTab)((int)uXCheckbox.Tag));
				i++;
			}
			this.SetupCurTabElements();
		}

		private void SetupCurTabElements()
		{
			this.itemGrid.Clear();
			this.labelBuildings.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			this.labelCurrency.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			this.labelTroops.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			this.labelCrates.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			List<UXElement> list = new List<UXElement>();
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			Service.EventManager.UnregisterObserver(this, EventId.InventoryCrateOpenedAndGranted);
			Service.EventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
			Service.EventManager.UnregisterObserver(this, EventId.CrateInventoryUpdated);
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			switch (this.curTab)
			{
			case InventoryTab.Crate:
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				Service.EventManager.RegisterObserver(this, EventId.InventoryCrateOpenedAndGranted);
				Service.EventManager.RegisterObserver(this, EventId.InventoryCrateCollectionClosed);
				Service.EventManager.RegisterObserver(this, EventId.CrateInventoryUpdated);
				this.AddCrates(list);
				this.AddGoToStoreItem(list);
				this.emptyCategoryLabel.Text = this.lang.Get("INVENTORY_NO_ITEM", new object[]
				{
					this.lang.Get("INVENTORY_TAB_CRATES", new object[0])
				});
				this.labelCrates.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
				serverPlayerPrefs.SetPref(ServerPref.NumInventoryCratesNotViewed, "0");
				break;
			case InventoryTab.Treasure:
				this.AddCurrencyItems(list);
				this.emptyCategoryLabel.Text = this.lang.Get("INVENTORY_NO_ITEM", new object[]
				{
					this.lang.Get("INVENTORY_TAB_CURRENCY", new object[0])
				});
				this.labelCurrency.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
				serverPlayerPrefs.SetPref(ServerPref.NumInventoryCurrencyNotViewed, "0");
				break;
			case InventoryTab.Building:
				this.AddBuildings(list);
				this.emptyCategoryLabel.Text = this.lang.Get("INVENTORY_NO_ITEM", new object[]
				{
					this.lang.Get("INVENTORY_TAB_BUILDINGS", new object[0])
				});
				this.labelBuildings.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
				break;
			case InventoryTab.Troop:
				this.AddTroops(list);
				this.emptyCategoryLabel.Text = this.lang.Get("INVENTORY_NO_ITEM", new object[]
				{
					this.lang.Get("INVENTORY_TAB_TROOPS", new object[0])
				});
				this.labelTroops.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
				serverPlayerPrefs.SetPref(ServerPref.NumInventoryTroopsNotViewed, "0");
				break;
			}
			Service.ServerAPI.Enqueue(new SetPrefsCommand(false));
			this.UpdateTabJewels();
			UXUtils.SortListForTwoRowGrids(list, this.itemGrid);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				this.itemGrid.AddItem(list[i], i);
				i++;
			}
			this.itemGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.OnGridRepositioned), 2);
			this.ToggleEmptyTabMessage();
			Service.EventManager.SendEvent(EventId.StoreScreenReady, this);
		}

		private void OnGridRepositioned(AbstractUXList list)
		{
			list.Scroll(0f);
		}

		private void ToggleEmptyTabMessage()
		{
			if (this.itemGrid.Count > 0)
			{
				this.emptyCategoryLabel.Visible = false;
			}
			else
			{
				this.emptyCategoryLabel.Visible = true;
			}
		}

		private void AddTroops(List<UXElement> list)
		{
			TroopUpgradeCatalog troopUpgradeCatalog = Service.TroopUpgradeCatalog;
			foreach (KeyValuePair<string, int> current in this.cp.Prizes.Troops)
			{
				if (current.Value > 0)
				{
					PrizeType prizeType = PrizeType.Infantry;
					TroopTypeVO minLevel = troopUpgradeCatalog.GetMinLevel(current.Key);
					switch (minLevel.Type)
					{
					case TroopType.Vehicle:
						prizeType = PrizeType.Vehicle;
						break;
					case TroopType.Mercenary:
						prizeType = PrizeType.Mercenary;
						break;
					case TroopType.Hero:
						prizeType = PrizeType.Hero;
						break;
					}
					UXElement item = this.CreateTileWithPrizeInfo(current.Key, current.Value, prizeType);
					list.Add(item);
				}
			}
			foreach (KeyValuePair<string, int> current2 in this.cp.Prizes.SpecialAttacks)
			{
				if (current2.Value > 0)
				{
					UXElement item2 = this.CreateTileWithPrizeInfo(current2.Key, current2.Value, PrizeType.SpecialAttack);
					list.Add(item2);
				}
			}
		}

		private void AddCurrencyItems(List<UXElement> list)
		{
			foreach (KeyValuePair<string, int> current in this.cp.Prizes.CurrencyResources)
			{
				if (current.Value > 0)
				{
					UXElement item = this.CreateTileWithPrizeInfo(current.Key, current.Value, PrizeType.Currency);
					list.Add(item);
				}
			}
		}

		private void AddCrates(List<UXElement> list)
		{
			uint time = ServerTime.Time;
			foreach (KeyValuePair<string, CrateData> current in this.cp.Prizes.Crates.Available)
			{
				CrateData value = current.Value;
				if (!value.Claimed && (!value.DoesExpire || value.ExpiresTimeStamp > time))
				{
					UXElement item = this.CreateTileWithPrizeInfo(current.Key, 1, PrizeType.Crate);
					list.Add(item);
				}
			}
			list.Sort(new Comparison<UXElement>(this.SortCrateListByExpiration));
		}

		private void AddGoToStoreItem(List<UXElement> list)
		{
			int cRATE_INVENTORY_TO_STORE_LINK_SORT = GameConstants.CRATE_INVENTORY_TO_STORE_LINK_SORT;
			if (cRATE_INVENTORY_TO_STORE_LINK_SORT == 0 || !CrateUtils.HasVisibleCrateStoreItems())
			{
				return;
			}
			string cRATE_INVENTORY_TO_STORE_LINK_CRATE_ASSET = GameConstants.CRATE_INVENTORY_TO_STORE_LINK_CRATE_ASSET;
			IGeometryVO vo = Service.StaticDataController.Get<CrateVO>(cRATE_INVENTORY_TO_STORE_LINK_CRATE_ASSET);
			string title = this.lang.Get("CRATE_INVENTORY_TO_STORE_TITLE", new object[0]);
			string description = this.lang.Get("CRATE_INVENTORY_TO_STORE_DESC", new object[0]);
			UXElement item = this.CreateInventoryGridItem("Id_Store", PrizeType.None, title, description, 0, vo, 0);
			if (cRATE_INVENTORY_TO_STORE_LINK_SORT == 1)
			{
				list.Insert(0, item);
			}
			else if (cRATE_INVENTORY_TO_STORE_LINK_SORT == 2)
			{
				list.Add(item);
			}
		}

		private int SortCrateListByExpiration(UXElement a, UXElement b)
		{
			PrizeInventoryItemTag prizeInventoryItemTag = (PrizeInventoryItemTag)a.Tag;
			CrateData crateData = this.cp.Prizes.Crates.Available[prizeInventoryItemTag.PrizeID];
			PrizeInventoryItemTag prizeInventoryItemTag2 = (PrizeInventoryItemTag)b.Tag;
			CrateData crateData2 = this.cp.Prizes.Crates.Available[prizeInventoryItemTag2.PrizeID];
			if (!crateData.DoesExpire && !crateData2.DoesExpire)
			{
				return 0;
			}
			if (!crateData.DoesExpire)
			{
				return 1;
			}
			if (!crateData2.DoesExpire)
			{
				return -1;
			}
			int num = (int)(crateData.ExpiresTimeStamp - crateData2.ExpiresTimeStamp);
			if (num < 0)
			{
				return -1;
			}
			if (num > 0)
			{
				return 1;
			}
			return 0;
		}

		private UXElement CreateTileWithPrizeInfo(string id, int count, PrizeType prizeType)
		{
			string title = null;
			string description = null;
			DeployableShardUnlockController deployableShardUnlockController = Service.DeployableShardUnlockController;
			IGeometryVO geometryVO = TimedEventPrizeUtils.GetFinalUnitFromPrize(prizeType, id);
			int quality = 0;
			switch (prizeType)
			{
			case PrizeType.Currency:
				title = this.lang.Get(id.ToUpper(), new object[0]);
				geometryVO = UXUtils.GetDefaultCurrencyIconVO(id);
				break;
			case PrizeType.Infantry:
			case PrizeType.Hero:
			case PrizeType.Vehicle:
			case PrizeType.Mercenary:
			{
				TroopTypeVO troopTypeVO = (TroopTypeVO)geometryVO;
				quality = deployableShardUnlockController.GetUpgradeQualityForDeployable(troopTypeVO);
				title = LangUtils.GetTroopDisplayName(troopTypeVO);
				description = LangUtils.GetTroopDescription(troopTypeVO);
				break;
			}
			case PrizeType.SpecialAttack:
			{
				SpecialAttackTypeVO specialAttackTypeVO = (SpecialAttackTypeVO)geometryVO;
				quality = deployableShardUnlockController.GetUpgradeQualityForDeployable(specialAttackTypeVO);
				title = LangUtils.GetStarshipDisplayName(specialAttackTypeVO);
				description = LangUtils.GetStarshipDescription(specialAttackTypeVO);
				break;
			}
			case PrizeType.Crate:
			{
				CrateData crateData = this.cp.Prizes.Crates.Available[id];
				string crateDisplayName = LangUtils.GetCrateDisplayName(crateData.CrateId);
				title = this.lang.Get("CRATE_INVENTORY_CRATE_TITLE", new object[]
				{
					LangUtils.GetPlanetDisplayName(crateData.PlanetId),
					crateDisplayName
				});
				geometryVO = Service.StaticDataController.Get<CrateVO>(crateData.CrateId);
				break;
			}
			}
			return this.CreateInventoryGridItem(id, prizeType, title, description, count, geometryVO, quality);
		}

		private void SetupGridItemQuality(string itemUID, int quality)
		{
			UXElement subElement = this.itemGrid.GetSubElement<UXElement>(itemUID, "Default");
			if (quality == 0)
			{
				UXUtils.HideGridQualityCards(this.itemGrid, itemUID, "QualityCardQ{0}");
				subElement.Visible = true;
				return;
			}
			subElement.Visible = false;
			UXUtils.SetCardQuality(this, this.itemGrid, itemUID, quality, "QualityCardQ{0}");
		}

		private UXElement CreateInventoryGridItem(string id, PrizeType prizeType, string title, string description, int count, IGeometryVO vo, int quality)
		{
			PrizeInventoryItemTag prizeInventoryItemTag = new PrizeInventoryItemTag();
			UXElement uXElement = this.itemGrid.CloneTemplateItem(id);
			prizeInventoryItemTag.TileElement = uXElement;
			prizeInventoryItemTag.PrizeID = id;
			prizeInventoryItemTag.PrizeType = prizeType;
			uXElement.Tag = prizeInventoryItemTag;
			this.SetupGridItemQuality(id, quality);
			prizeInventoryItemTag.MainElement = this.itemGrid.GetSubElement<UXElement>(id, "ItemInfo");
			UXLabel subElement = this.itemGrid.GetSubElement<UXLabel>(id, "LabelName");
			subElement.Text = title;
			UXButton subElement2 = this.itemGrid.GetSubElement<UXButton>(id, "BtnItemInfo");
			prizeInventoryItemTag.InfoLabel = this.itemGrid.GetSubElement<UXLabel>(id, "LabelItemInfo");
			if (!string.IsNullOrEmpty(description) && prizeType != PrizeType.None)
			{
				prizeInventoryItemTag.InfoLabel.Text = description;
				subElement2.Visible = true;
				subElement2.OnClicked = new UXButtonClickedDelegate(this.OnInfoButtonClicked);
				subElement2.Tag = prizeInventoryItemTag;
			}
			else
			{
				subElement2.Visible = false;
			}
			prizeInventoryItemTag.CountLabel = this.itemGrid.GetSubElement<UXLabel>(id, "LabelItemCount");
			UXSprite subElement3 = this.itemGrid.GetSubElement<UXSprite>(id, "SpriteItemImage");
			if (prizeType == PrizeType.None)
			{
				RewardUtils.SetCrateIcon(subElement3, (CrateVO)vo, AnimState.Idle);
			}
			else if (prizeType == PrizeType.Crate)
			{
				RewardUtils.SetCrateIcon(subElement3, (CrateVO)vo, AnimState.Closed);
			}
			else
			{
				RewardUtils.SetRewardIcon(subElement3, vo, AnimationPreference.NoAnimation);
			}
			prizeInventoryItemTag.IconAssetName = vo.IconAssetName;
			UXButton subElement4 = this.itemGrid.GetSubElement<UXButton>(id, "InventoryCard");
			subElement4.OnClicked = new UXButtonClickedDelegate(this.OnTileClicked);
			subElement4.Tag = prizeInventoryItemTag;
			this.itemGrid.GetSubElement<UXElement>(id, "CountAndBuildTime").Visible = true;
			this.itemGrid.GetSubElement<UXElement>(id, "ContainerJewelNew").Visible = false;
			UXLabel subElement5 = this.itemGrid.GetSubElement<UXLabel>(id, "LabelItemExpiration");
			UXLabel subElement6 = this.itemGrid.GetSubElement<UXLabel>(id, "LabelCurrencyAmount");
			subElement6.Visible = false;
			if (prizeType == PrizeType.None)
			{
				prizeInventoryItemTag.CountLabel.Visible = false;
				subElement5.Visible = false;
				subElement6.Text = description;
				subElement6.Visible = true;
			}
			else if (prizeType == PrizeType.Crate)
			{
				prizeInventoryItemTag.CountLabel.Visible = false;
				CrateData crateData = this.cp.Prizes.Crates.Available[id];
				subElement5.Visible = true;
				UXUtils.SetCrateExpirationTimerLabel(crateData, subElement5, this.lang);
				if (!string.IsNullOrEmpty(crateData.PlanetId))
				{
					PlanetVO planetVO = Service.StaticDataController.Get<PlanetVO>(crateData.PlanetId);
					UXTexture subElement7 = this.itemGrid.GetSubElement<UXTexture>(id, "TexturePlanet");
					subElement7.LoadTexture(planetVO.LeaderboardTileTexture);
				}
				if (this.crateTabLastViewedTime < crateData.ReceivedTimeStamp)
				{
					this.itemGrid.GetSubElement<UXElement>(id, "ContainerJewelNew").Visible = true;
					this.itemGrid.GetSubElement<UXLabel>(id, "LabelMessageCountNew").Text = this.lang.Get("CRATE_INVENTORY_CRATE_NEW_BADGE", new object[0]);
				}
				subElement4.Tag = prizeInventoryItemTag;
			}
			else
			{
				prizeInventoryItemTag.CountLabel.Text = string.Format("x{0}", count);
				prizeInventoryItemTag.CountLabel.Visible = true;
				subElement5.Visible = false;
			}
			this.itemGrid.GetSubElement<UXLabel>(id, "LabelItemInfo").Visible = false;
			this.itemGrid.GetSubElement<UXLabel>(id, "LabelItemRequirement").Visible = false;
			this.itemGrid.GetSubElement<UXSprite>(id, "SpriteItemTimeIcon").Visible = false;
			this.itemGrid.GetSubElement<UXLabel>(id, "LabelBuildTime").Visible = false;
			uXElement.Tag = prizeInventoryItemTag;
			return uXElement;
		}

		private void AddBuildings(List<UXElement> list)
		{
		}

		private void OnTileClicked(UXButton button)
		{
			PrizeInventoryItemTag prizeInventoryItemTag = button.Tag as PrizeInventoryItemTag;
			if (prizeInventoryItemTag.InfoLabel.Visible)
			{
				prizeInventoryItemTag.InfoLabel.Visible = false;
				prizeInventoryItemTag.MainElement.Visible = true;
				return;
			}
			if (prizeInventoryItemTag.PrizeType == PrizeType.None)
			{
				Service.EventManager.SendEvent(EventId.InventoryCrateStoreOpen, prizeInventoryItemTag.IconAssetName);
				Service.ScreenController.CloseAll();
				GameUtils.OpenStoreWithTab(StoreTab.Treasure);
			}
			else if (prizeInventoryItemTag.PrizeType == PrizeType.Crate)
			{
				button.Enabled = false;
				CrateData crateData = this.cp.Prizes.Crates.Available[prizeInventoryItemTag.PrizeID];
				GameUtils.OpenInventoryCrateModal(crateData, new OnScreenModalResult(this.OnCrateInfoModalClosed));
				Service.EventManager.SendEvent(EventId.InventoryCrateTapped, crateData.CrateId + "|" + crateData.UId);
			}
			else
			{
				int num = TimedEventPrizeUtils.TransferPrizeFromInventory(prizeInventoryItemTag.PrizeType, prizeInventoryItemTag.PrizeID);
				if (num > 0)
				{
					prizeInventoryItemTag.CountLabel.Text = string.Format("x{0}", num);
				}
				else if (num == 0)
				{
					this.itemGrid.RemoveItem(prizeInventoryItemTag.TileElement);
					base.DestroyElement(prizeInventoryItemTag.TileElement);
					this.itemGrid.RepositionItems();
				}
				this.ToggleEmptyTabMessage();
			}
		}

		public override void OnDestroyElement()
		{
			EventManager eventManager = Service.EventManager;
			if (this.itemGrid != null)
			{
				this.itemGrid.Clear();
				this.itemGrid = null;
			}
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			eventManager.UnregisterObserver(this, EventId.InventoryCrateOpenedAndGranted);
			eventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
			eventManager.UnregisterObserver(this, EventId.CrateInventoryUpdated);
			this.prefs.SetPref("crtInvLastViewed", ServerTime.Time.ToString());
			eventManager.SendEvent(EventId.NumInventoryItemsNotViewedUpdated, null);
			base.OnDestroyElement();
		}

		private void OnInfoButtonClicked(UXButton button)
		{
			PrizeInventoryItemTag prizeInventoryItemTag = button.Tag as PrizeInventoryItemTag;
			IUpgradeableVO finalUnitFromPrize = TimedEventPrizeUtils.GetFinalUnitFromPrize(prizeInventoryItemTag.PrizeType, prizeInventoryItemTag.PrizeID);
			if (finalUnitFromPrize != null)
			{
				Entity availableTroopResearchLab = Service.BuildingLookupController.GetAvailableTroopResearchLab();
				TroopUpgradeTag troopUpgradeTag = new TroopUpgradeTag(finalUnitFromPrize as IDeployableVO, true);
				bool showUpgradeControls = !string.IsNullOrEmpty(troopUpgradeTag.Troop.UpgradeShardUid);
				Service.ScreenController.AddScreen(new DeployableInfoScreen(troopUpgradeTag, null, showUpgradeControls, availableTroopResearchLab));
			}
			else
			{
				prizeInventoryItemTag.MainElement.Visible = false;
				prizeInventoryItemTag.InfoLabel.Visible = true;
			}
			Service.EventManager.SendEvent(EventId.InfoButtonClicked, null);
		}

		public void OnViewClockTime(float dt)
		{
			int i = 0;
			int count = this.itemGrid.Count;
			while (i < count)
			{
				PrizeInventoryItemTag prizeInventoryItemTag = (PrizeInventoryItemTag)this.itemGrid.GetItem(i).Tag;
				if (prizeInventoryItemTag.PrizeType == PrizeType.Crate)
				{
					CrateData crateData = this.cp.Prizes.Crates.Available[prizeInventoryItemTag.PrizeID];
					if (crateData.DoesExpire)
					{
						if (crateData.ExpiresTimeStamp <= ServerTime.Time)
						{
							this.SetupCurTabElements();
							return;
						}
					}
				}
				i++;
			}
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.InventoryCrateOpenedAndGranted || id == EventId.CrateInventoryUpdated || id == EventId.InventoryCrateCollectionClosed)
			{
				if (this.curTab == InventoryTab.Crate)
				{
					this.SetupCurTabElements();
				}
			}
			return base.OnEvent(id, cookie);
		}

		private void OnCrateInfoModalClosed(object result, object cookie)
		{
			if (cookie == null)
			{
				int i = 0;
				int count = this.itemGrid.Count;
				while (i < count)
				{
					UXElement item = this.itemGrid.GetItem(i);
					PrizeInventoryItemTag prizeInventoryItemTag = (PrizeInventoryItemTag)item.Tag;
					UXButton subElement = this.itemGrid.GetSubElement<UXButton>(prizeInventoryItemTag.PrizeID, "InventoryCard");
					subElement.Enabled = true;
					i++;
				}
				return;
			}
		}
	}
}
