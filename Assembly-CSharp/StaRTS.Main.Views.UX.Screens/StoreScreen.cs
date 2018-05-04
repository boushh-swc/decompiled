using Net.RichardLord.Ash.Core;
using StaRTS.Externals.IAP;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.ShardShop;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class StoreScreen : ClosableScreen, IEventObserver, IViewClockTimeObserver
	{
		public delegate void AddShardItemHandler(string name, UXElement item, UXGrid grid, ShardShopViewTO vto);

		public delegate void AddBuildingItemDelegate(List<UXElement> list, BuildingTypeVO buildingInfo, BuildingTypeVO reqBuilding, bool reqMet, int curQuantity, int maxQuantity, UXGrid grid);

		private const string STORE_NAME = "gui_store_screen";

		private const string STORE_WIDGET = "gui_store_screen_main_widget";

		private const string PRIMARY_PAGE = "TabPage";

		private const string SECONDARY_PAGE = "CategoryPage";

		private const string BACK_BUTTON = "BtnBack";

		private const string PROMO_GROUP = "PromoContainer";

		private const string ITEM_GRID_FILTER_PANEL = "FilterPanel";

		private const string ITEM_GRID_TWO_ROWS_PARENT = "StoreItems2row";

		private const string ITEM_GRID_TWO_ROWS = "StoreGrid2row";

		private const string ITEM_GRID_ONE_ROW = "StoreGrid1row";

		private const string ITEM_GRID_TWO_ROWS_PARENT_FILTERED = "StoreItems2rowFiltered";

		private const string ITEM_GRID_TWO_ROWS_FILTERED = "StoreGrid2rowFiltered";

		private const string ITEM_TEMPLATE = "StoreItemTemplate";

		private const string ITEM_HEIGHT_GUIDE = "ItemHeightGuide";

		private const string ITEM_CELL_HEIGHT_GUIDE = "ItemCellHeightGuide";

		private const string ITEM_BUTTON = "ButtonItemCard";

		private const string ITEM_ICON = "SpriteItemImage";

		private const string ITEM_MAIN_ELEMENT = "ItemInfo";

		private const string ITEM_LABEL_NAME = "LabelName";

		private const string ITEM_LABEL_LEI_NAME = "LabelNameSpecial";

		private const string ITEM_LEI_TIMER_LABEL = "LabelTimerSpecial";

		private const string ITEM_LABEL_TIME = "LabelBuildTime";

		private const string ITEM_ICON_TIME = "SpriteItemTimeIcon";

		private const string ITEM_LABEL_COUNT = "LabelItemCount";

		private const string ITEM_LABEL_INFO = "LabelItemInfo";

		private const string ITEM_LABEL_REQ = "LabelItemRequirement";

		private const string ITEM_LABEL_REWARD = "LabelCurrencyAmount";

		private const string ITEM_GROUP_COUNTS = "CountAndBuildTime";

		private const string ITEM_BUTTON_INFO = "BtnItemInfo";

		protected const string ITEM_COST_GROUP = "Cost";

		private const string ITEM_LOCKED = "SpriteDim";

		private const string FRAGMENT_ITEM_TEMPLATE = "FragmentItemTemplate";

		private const string EXPIRATION_LABEL = "LabelExpiration";

		private const string MODAL_PURCHASE = "ModalPurchase";

		private const string FRAGMENT_ITEM_BUTTON = "ButtonFragmentCard";

		private const string TABS_PARENT = "StoreTabs";

		private const string TAB_CRYSTALS = "TabCrystals";

		private const string TAB_FRAGMENTS = "TabFragments";

		private const string TAB_TREASURE = "TabTreasure";

		private const string TAB_STRUCTURES = "TabStructures";

		private const string TAB_TURRETS = "TabTurrets";

		protected const string TAB_TITLE = "DialogStoreTitle";

		private const string CATEGORY_TITLE = "DialogStoreCategoryTitle";

		private const string TURRET_LABEL = "LabelTurretCount";

		private const string TURRET_LABEL_GROUP = "TurretCount";

		private const string STRUCTURE_TAB_ALL = "TROOP_TAB_ALL";

		private const string STRUCTURE_TAB_ARMY = "s_army";

		private const string STRUCTURE_TAB_RESOURCES = "s_resources";

		private const string STRUCTURE_TAB_DEFENSES = "s_defenses";

		private const string FRAGMENTS_LOCK_ELEMENT = "WidgetTabFragmentsLocked";

		private const string FRAGMENTS_LOCK_LABEL = "LabelTabFragmentsLocked";

		private const string CRYSTALS_LOCK_ELEMENT = "WidgetTabFCrystalsLocked";

		private const string CRYSTALS_LOCK_LABEL = "LabelTabCrystalsLocked";

		private const string FRAGMENTS_LOCK_MIN_HQ_MSG = "shardShopMinHqMessage";

		private const string TEXTURE_HOLDER_CRYSTALS = "TextureCrystalsIcon";

		private const string TEXTURE_HOLDER_FRAGMENTS = "TextureFragmentsIcon";

		private const string TEXTURE_HOLDER_TREASURE = "TextureTreasureIcon";

		private const string TEXTURE_HOLDER_STRUCTURES = "TextureStructuresIcon";

		private const string TEXTURE_HOLDER_TURRETS = "TextureTurretsIcon";

		private const string TEXTURE_CRYSTALS = "storeicon_lg_crystals";

		private const string TEXTURE_FRAGMENTS = "storeicon_lg_fragments";

		private const string TEXTURE_TREASURE = "storeicon_treasure";

		private const string TEXTURE_STRUCTURES = "storeicon_structures_{0}";

		private const string TEXTURE_TURRETS = "storeicon_turrets_{0}";

		private const string CRYSTAL_SALE_TITLE_CONTAINER = "CrystalBonusTitleContainer";

		private const string CRYSTAL_SALE_TITLE = "CrystalBonusLabelTitle";

		private const string CRYSTAL_SALE_TIMER = "CyrstalBonusLabelExpire";

		protected const string CRYSTAL_SALE_INFO = "CrystalBonus";

		private const string CRYSTAL_SALE_ITEM_PERCENT = "CrystalBonusLabel";

		private const string CRYSTAL_SALE_ITEM_TOTAL = "CrystalBonusLabelAmount";

		private const string CRYSTAL_SALE_ITEM_BONUS = "CrystalBonusLabelBonusAmount";

		private const string CRYSTAL_SALE_TIMER_TEXT = "crystal_bonus_ends_in";

		private const string CRYSTAL_SALE_PERCENT_TEXT = "crystal_percent_bonus";

		private const string CRYSTAL_SALE_BONUS_TEXT = "crystal_amount_bonus";

		private const string CURRENCY_NAME_TEXT = "CURRENCY_VALUE_NAME";

		private const string PURCHASE_CRATE = "PURCHASE_CRATE";

		private const string PURCHASE_PROTECTION = "PURCHASE_PROTECTION";

		private const string PURCHASE_SOFT_CURRENCY = "PURCHASE_SOFT_CURRENCY";

		private const string ALL_CRATES_ALREADY_PURCHASED = "ALL_CRATES_ALREADY_PURCHASED";

		private const string LIMITED_EDITION_CATEGORY_BANNER = "LIMITED_EDITION_CATEGORY_BANNER";

		private const string CRATE_STORE_LEI_EXPIRATION_TIMER = "CRATE_STORE_LEI_EXPIRATION_TIMER";

		private const string SHARD_SHOP_OFFER_EXPIRES = "shard_shop_offer_expires";

		private const string CRYSTALS_LOCKED_NO_IAP = "crystals_tab_locked";

		private const string BACK_BUTTON_LABEL = "LabelBack";

		private const string DEFAULT_BACK = "s_Back";

		private const string BACK_TO_CATEGORIES = "back_to_shop_categories";

		private const string ITEM_ICON_PROTECTION = "protection";

		private const string ITEM_PACK_CRYSTALS = "PACK_CRYSTALS{0}";

		private const string ITEM_PACK_CURRENCY = "PACK_CURRENCY{0}";

		private const string ITEM_PACK_PROTECTION = "PACK_PROTECTION{0}";

		protected const string ITEM_JEWEL = "Items";

		private const string ITEM_BG = "TemplateBg";

		private const string ITEM_LEI_BG = "TemplateBgSpecial";

		private const string SUFFIX_EMPIRE = "emp";

		private const string SUFFIX_REBEL = "rbl";

		public const string IAP_TITLE_PREFIX = "iap_title_";

		public const string IAP_DESC_PREFIX = "iap_desc_";

		protected const string BADGE_GROUP = "PackageBadge";

		private const string BADGE_TOP_SPRITE = "SpritePackageBg";

		private const string BADGE_TOP_LABEL = "LabelPackageTop";

		private const string BADGE_BOTTOM_LABEL = "LabelPackageBottom";

		protected const string EVENT_STORE_TITLE = "EventStoreTitle";

		private const string LEI_TREASURE_HEADER_LABEL = "LabelHeaderTreasureSpecial";

		private const string LEI_TREASURE_BG = "BgTabTreasureSpecial";

		private const string TREASURE_HEADER_LABEL = "LabelTreasure";

		private const string TREASURE_BG = "BgTabTreasure";

		private const string ITEM_FRAGMENTS_AVAILABLE_LABEL = "LabelMesageCountFragmentItems";

		private const string ITEM_FRAGMENTS_INFO_BUTTON = "BtnFragmentItemInfo";

		private const string ITEM_FRAGMENTS_COST = "WidgetFragmentCost";

		private const string ITEM_FRAGMENTS_NAME_LABEL = "LabelFragmentName";

		private const string ITEM_FRAGMENTS_REQUIREMENTS_LABEL = "LabelFragmentRequirement";

		private const string ITEM_FRAGMENTS_REWARD_BOX = "WidgetRewardBox";

		private const string ITEM_FRAGMENTS_PROGRESS = "pBarFragmentCount";

		private const string ITEM_FRAGMENTS_ICON_SPRITE = "SpriteFragmentImage";

		private const string ITEM_FRAGMENTS_COST_ICON = "SpriteFragmentCurrencyIcon";

		private const string ITEM_FRAGMENTS_COST_LABEL = "LabelFragmentCost";

		private const string ITEM_FRAGMENTS_PROGRESS_LABEL = "LabelFragProgress";

		private const string ITEM_FRAGMENTS_PROGRESS_MESSAGE_1_LABEL = "LabelFragProgressMessage1";

		private const string ITEM_FRAGMENTS_PROGRESS_MESSAGE_2_LABEL = "LabelFragProgressMessage2";

		private const string ITEM_FRAGMENTS_REQ_LABEL = "LabelFragmentRequirement";

		private const string ITEM_FRAGMENTS_PROGRESS_BAR = "pBarFragmentCount";

		private const string ITEM_FRAGMENTS_PROGRESS_BAR_DELTA = "pBarFragmentCountDelta";

		private const string SHARD_SHOP_MAXED_OUT = "shard_shop_maxed_out";

		private const string SHARD_SHOP_SOLD_OUT = "shard_shop_sold_out";

		private const string SHARD_SHOP_REQ_CANTINA = "shard_shop_cantina_req";

		private const string SHARD_SHOP_REQ_ARMORY = "shard_shop_armory_req";

		private const string ICON_QUALITY_INT = "Quality";

		private const string ICON_MODAL_STATE = "InModal";

		private const string ICON_SHOW = "Visible";

		private const string ICON_LOCKED = "Locked";

		private const string ICON_UPGRADEABLE = "Upgradeable";

		private const string SHARD_PROGRESS_UPGRADE_SEQ1 = "CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ1";

		private const string SHARD_PROGRESS_UPGRADE_SEQ2 = "CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ2";

		private static readonly int[] SOFT_CURRENCY_PERCENTS = new int[]
		{
			10,
			50,
			100
		};

		private UXLabel titleLabel;

		private UXLabel categoryLabel;

		private UXElement filterPanel;

		protected UXGrid itemGrid2Rows;

		protected UXGrid itemGrid1Row;

		private UXElement itemGridParent;

		protected UXGrid itemGridFiltered;

		private UXElement itemGridFilteredParent;

		private UXElement tabsParent;

		private UXElement primaryPage;

		private UXElement secondaryPage;

		private UXElement promoGroup;

		private UXLabel turretLabel;

		private UXElement turretLabelGroup;

		protected UXButton backButton;

		private UXLabel expirationLabel;

		private StoreTab curTab;

		private bool changingInventory;

		private bool showingPromo;

		private bool movedDown;

		private bool turretSwappingUnlocked;

		private bool enableTimer;

		private string curItem;

		private Dictionary<StoreTab, JewelControl> tabJewels;

		protected bool requiresRefresh;

		private SaleTypeVO visibleSale;

		private bool gridInitialized;

		private bool gridScrollable = true;

		private uint delayedInitTimerId;

		private bool resetCurrentTabOnVisible;

		private string crateToReOpenInFlyout;

		private StructuresTabHelper tabHelper;

		private ShardShopViewModule ssView;

		private ShopStickerViewModule stickersViewModule;

		private string backButtonStringId;

		public override bool ShowCurrencyTray
		{
			get
			{
				return true;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
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
				base.Visible = value;
				if (value && this.resetCurrentTabOnVisible)
				{
					this.ResetCurrentTab();
				}
			}
		}

		public StoreScreen() : base("gui_store")
		{
			this.backButtonStringId = "s_Back";
			this.delayedInitTimerId = 0u;
			this.changingInventory = false;
			this.enableTimer = false;
			this.resetCurrentTabOnVisible = false;
			this.showingPromo = true;
			this.movedDown = false;
			this.turretSwappingUnlocked = Service.BuildingLookupController.IsTurretSwappingUnlocked();
			this.curItem = null;
			this.SetTab(StoreTab.NotInStore);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.PlayerFactionChanged);
			eventManager.RegisterObserver(this, EventId.ButtonHighlightActivated);
			eventManager.RegisterObserver(this, EventId.EquipmentUnlockCelebrationPlayed);
			eventManager.RegisterObserver(this, EventId.ShardOfferChanged);
			eventManager.RegisterObserver(this, EventId.ShardViewClosed);
			this.RegisterForEventsWithoutModal();
			this.tabJewels = new Dictionary<StoreTab, JewelControl>();
			this.tabHelper = new StructuresTabHelper();
			this.stickersViewModule = new ShopStickerViewModule(this);
			this.requiresRefresh = false;
			this.visibleSale = null;
			base.OnTransitionInComplete = new OnTransInComplete(this.onScreenTransitionInComplete);
		}

		private void onScreenTransitionInComplete()
		{
			Service.EventManager.SendEvent(EventId.ScreenTransitionInComplete, this);
		}

		protected override void OnScreenLoaded()
		{
			base.Root.name = "gui_store_screen";
			this.InitLabels();
			this.InitButtons();
			base.GetElement<UXElement>("EventStoreTitle").Visible = false;
			base.GetElement<UXElement>("ModalPurchase").Visible = false;
			this.ssView = new ShardShopViewModule(this);
			this.expirationLabel.Text = string.Empty;
			this.expirationLabel.Visible = false;
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			this.delayedInitTimerId = Service.ViewTimerManager.CreateViewTimer(0.01f, false, new TimerDelegate(this.InitializeDelayed), null);
		}

		private void InitializeDelayed(uint id, object cookie)
		{
			this.delayedInitTimerId = 0u;
			this.filterPanel = base.GetElement<UXElement>("FilterPanel");
			this.itemGrid2Rows = base.GetElement<UXGrid>("StoreGrid2row");
			this.InitGrids(this.itemGrid2Rows);
			this.itemGrid1Row = base.GetElement<UXGrid>("StoreGrid1row");
			this.InitGrids(this.itemGrid1Row);
			this.itemGridFiltered = base.GetElement<UXGrid>("StoreGrid2rowFiltered");
			this.InitGrids(this.itemGridFiltered);
			this.InitStructuresFilter();
			this.SetTab(this.curTab);
			this.ShowPromos(this.showingPromo);
			this.ScrollToItem(this.curItem);
		}

		public override void OnDestroyElement()
		{
			if (this.delayedInitTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.delayedInitTimerId);
				this.delayedInitTimerId = 0u;
			}
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			this.visibleSale = null;
			this.ssView.Destroy();
			this.EnableScrollListMovement(true);
			if (this.itemGrid2Rows != null)
			{
				this.itemGrid2Rows.Clear();
				this.itemGrid2Rows = null;
			}
			if (this.itemGrid1Row != null)
			{
				this.itemGrid1Row.Clear();
				this.itemGrid1Row = null;
			}
			if (this.itemGridFiltered != null)
			{
				this.itemGridFiltered.Clear();
				this.itemGridFiltered = null;
			}
			if (this.tabHelper != null)
			{
				this.tabHelper.Destroy();
				this.tabHelper = null;
			}
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.PlayerFactionChanged);
			eventManager.UnregisterObserver(this, EventId.ButtonHighlightActivated);
			eventManager.UnregisterObserver(this, EventId.EquipmentUnlockCelebrationPlayed);
			eventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
			eventManager.UnregisterObserver(this, EventId.ShardOfferChanged);
			eventManager.UnregisterObserver(this, EventId.ShardViewClosed);
			this.UnregisterForEventsWithoutModal();
			this.RemoveProtectionCooldownTimer();
			base.OnDestroyElement();
		}

		private void RegisterForEventsWithoutModal()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.InventoryResourceUpdated);
		}

		private void UnregisterForEventsWithoutModal()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.InventoryResourceUpdated);
		}

		private void AddProtectionCooldownTimer()
		{
			if (this.enableTimer)
			{
				return;
			}
			this.enableTimer = true;
		}

		private void RemoveProtectionCooldownTimer()
		{
			this.enableTimer = false;
		}

		private void InitLabels()
		{
			this.titleLabel = base.GetElement<UXLabel>("DialogStoreTitle");
			this.categoryLabel = base.GetElement<UXLabel>("DialogStoreCategoryTitle");
			this.turretLabel = base.GetElement<UXLabel>("LabelTurretCount");
			this.turretLabelGroup = base.GetElement<UXElement>("TurretCount");
			this.turretLabel.Text = string.Empty;
			this.turretLabelGroup.Visible = false;
			this.expirationLabel = base.GetElement<UXLabel>("LabelExpiration");
			this.expirationLabel.Visible = false;
		}

		private void RefreshProtectionCooldownTimer()
		{
			int[] array;
			int[] array2;
			GameUtils.GetProtectionPacks(out array, out array2);
			bool flag = true;
			for (int i = 1; i <= array.Length; i++)
			{
				string text = string.Format("{0}_{1}", "protection", i);
				UXButton subElement = this.itemGrid2Rows.GetSubElement<UXButton>(text, "ButtonItemCard");
				StoreItemTag storeItemTag = subElement.Tag as StoreItemTag;
				if (!storeItemTag.CanPurchase)
				{
					UXLabel subElement2 = this.itemGrid2Rows.GetSubElement<UXLabel>(text, "LabelItemRequirement");
					int protectionCooldownRemainingInSeconds = this.GetProtectionCooldownRemainingInSeconds(i);
					if (protectionCooldownRemainingInSeconds > 0)
					{
						flag = false;
						subElement2.Text = this.lang.Get("PROTECTION_COOLDOWN_TIMER", new object[0]) + GameUtils.GetTimeLabelFromSeconds(protectionCooldownRemainingInSeconds);
					}
					else
					{
						StoreItemTag arg_BC_0 = storeItemTag;
						bool flag2 = true;
						storeItemTag.ReqMet = flag2;
						arg_BC_0.CanPurchase = flag2;
						subElement2.Text = string.Empty;
						UXSprite subElement3 = this.itemGrid2Rows.GetSubElement<UXSprite>(text, "SpriteDim");
						subElement3.Visible = false;
						UXUtils.SetupCostElements(this, "Cost", text, 0, 0, 0, storeItemTag.Price, false, null);
					}
				}
			}
			if (flag)
			{
				this.RemoveProtectionCooldownTimer();
			}
		}

		private void InitGrids(UXGrid grid)
		{
			grid.CellHeight = base.GetElement<UXElement>("ItemCellHeightGuide").Height;
			base.GetElement<UXElement>("StoreItemTemplate").Height = base.GetElement<UXElement>("ItemHeightGuide").Height;
			UXElement element = base.GetElement<UXElement>("ButtonItemCard");
			UIDragScrollView component = element.Root.GetComponent<UIDragScrollView>();
			component.scrollView = grid.Root.GetComponent<UIScrollView>();
			base.GetElement<UXLabel>("LabelItemRequirement").Text = string.Empty;
			grid.SetTemplateItem("StoreItemTemplate");
			grid.IsScrollable = this.gridScrollable;
			this.gridInitialized = true;
		}

		private void InitStructuresFilter()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(0, this.lang.Get("TROOP_TAB_ALL", new object[0]));
			dictionary.Add(1, this.lang.Get("s_army", new object[0]));
			dictionary.Add(2, this.lang.Get("s_defenses", new object[0]));
			dictionary.Add(3, this.lang.Get("s_resources", new object[0]));
			this.tabHelper.CreateTabs(this, new Action(this.OnStructuresTabChanged), dictionary, 0);
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.tabsParent = base.GetElement<UXElement>("StoreTabs");
			this.primaryPage = base.GetElement<UXElement>("TabPage");
			this.secondaryPage = base.GetElement<UXElement>("CategoryPage");
			this.promoGroup = base.GetElement<UXElement>("PromoContainer");
			this.backButton = base.GetElement<UXButton>("BtnBack");
			this.BackButtons.Add(this.backButton);
			this.SetupTab(StoreTab.NotInStore, this.backButton);
			this.SetupTab(StoreTab.Crystals, "TabCrystals", "TextureCrystalsIcon", "storeicon_lg_crystals");
			this.SetupTab(StoreTab.Fragments, "TabFragments", "TextureFragmentsIcon", "storeicon_lg_fragments");
			this.SetupTab(StoreTab.Treasure, "TabTreasure", "TextureTreasureIcon", "storeicon_treasure");
			this.SetupTabForFaction(StoreTab.Structures, "TabStructures", "TextureStructuresIcon", "storeicon_structures_{0}");
			this.SetupTabForFaction(StoreTab.Turrets, "TabTurrets", "TextureTurretsIcon", "storeicon_turrets_{0}");
			this.stickersViewModule.SetupStickers();
		}

		public static int CountUnlockedUnbuiltBuildings()
		{
			return StoreScreen.AddOrCountBuildingItems(null, StoreTab.Resources, null, null) + StoreScreen.AddOrCountBuildingItems(null, StoreTab.Army, null, null) + StoreScreen.AddOrCountBuildingItems(null, StoreTab.Defenses, null, null) + StoreScreen.AddOrCountBuildingItems(null, StoreTab.Decorations, null, null);
		}

		private void SetupLimitedEditionTab(StoreTab tab, string leHeaderName, string leBgName, string bgName)
		{
			UXLabel element = base.GetElement<UXLabel>(leHeaderName);
			UXElement element2 = base.GetElement<UXElement>(leBgName);
			element.Visible = false;
			element2.Visible = false;
			LimitedEditionItemController limitedEditionItemController = Service.LimitedEditionItemController;
			int i = 0;
			int count = limitedEditionItemController.ValidLEIs.Count;
			while (i < count)
			{
				if (((LimitedEditionItemVO)limitedEditionItemController.ValidLEIs[i]).StoreTab == tab)
				{
					element.Text = this.lang.Get("LIMITED_EDITION_CATEGORY_BANNER", new object[0]);
					element.Visible = true;
					element2.Visible = true;
					base.GetElement<UXElement>(bgName).Visible = false;
					break;
				}
				i++;
			}
		}

		private void SetupTab(StoreTab tab, string tabName, string textureHolderName, string textureAssetName)
		{
			base.GetElement<UXTexture>(textureHolderName).LoadTexture(textureAssetName);
			UXButton element = base.GetElement<UXButton>(tabName);
			this.SetupTab(tab, element);
			if (tab != StoreTab.Crystals)
			{
				if (tab == StoreTab.Fragments)
				{
					bool flag = Service.ShardShopController.IsShardShopUnlocked();
					if (flag)
					{
						base.GetElement<UXElement>("WidgetTabFragmentsLocked").Visible = false;
					}
					else
					{
						int sHARD_SHOP_MINIMUM_HQ = GameConstants.SHARD_SHOP_MINIMUM_HQ;
						base.GetElement<UXLabel>("LabelTabFragmentsLocked").Text = this.lang.Get("shardShopMinHqMessage", new object[]
						{
							sHARD_SHOP_MINIMUM_HQ
						});
						element.OnClicked = null;
					}
				}
			}
			else if (this.HasInAppPurchaseItems())
			{
				base.GetElement<UXElement>("WidgetTabFCrystalsLocked").Visible = false;
			}
			else
			{
				base.GetElement<UXLabel>("LabelTabCrystalsLocked").Text = this.GetLockedCrystalLabelString();
			}
		}

		public bool HasInAppPurchaseItems()
		{
			int numberOfValidIapItems = Service.InAppPurchaseController.GetNumberOfValidIapItems();
			return numberOfValidIapItems > 0;
		}

		private string GetLockedCrystalLabelString()
		{
			return this.lang.Get("crystals_tab_locked", new object[0]);
		}

		private void SetupTabForFaction(StoreTab tab, string tabName, string textureHolderName, string textureAssetNameFormat)
		{
			string assetName = null;
			FactionType faction = Service.CurrentPlayer.Faction;
			if (faction != FactionType.Empire)
			{
				if (faction == FactionType.Rebel)
				{
					assetName = string.Format(textureAssetNameFormat, "rbl");
				}
			}
			else
			{
				assetName = string.Format(textureAssetNameFormat, "emp");
			}
			base.GetElement<UXTexture>(textureHolderName).LoadTexture(assetName);
			this.SetupTab(tab, base.GetElement<UXButton>(tabName));
		}

		private void SetupTab(StoreTab tab, UXButton tabButton)
		{
			tabButton.OnClicked = new UXButtonClickedDelegate(this.OnTabButtonClicked);
			tabButton.Tag = tab;
		}

		private void SetupJewel(StoreTab tab)
		{
			JewelControl jewelControl;
			if (this.tabJewels.ContainsKey(tab))
			{
				jewelControl = this.tabJewels[tab];
			}
			else
			{
				bool showCount = tab != StoreTab.Fragments;
				jewelControl = JewelControl.Create(this, tab.ToString(), null, showCount, true);
				if (jewelControl == null)
				{
					return;
				}
				this.tabJewels.Add(tab, jewelControl);
			}
			int value;
			if (tab == StoreTab.Structures)
			{
				value = StoreScreen.AddOrCountBuildingItems(null, StoreTab.Army, null, null) + StoreScreen.AddOrCountBuildingItems(null, StoreTab.Defenses, null, null) + StoreScreen.AddOrCountBuildingItems(null, StoreTab.Resources, null, null);
			}
			else if (tab == StoreTab.Turrets)
			{
				value = StoreScreen.AddOrCountBuildingItems(null, StoreTab.Decorations, null, null);
			}
			else if (tab == StoreTab.Fragments)
			{
				value = 0;
				ShardShopController shardShopController = Service.ShardShopController;
				if (shardShopController.IsShardShopUnlocked())
				{
					ShardShopData currentShopData = shardShopController.CurrentShopData;
					string shardShopExpiration = PlayerSettings.GetShardShopExpiration();
					if (currentShopData != null && currentShopData.Expiration.ToString("D") != shardShopExpiration)
					{
						value = 1;
					}
				}
			}
			else
			{
				value = StoreScreen.AddOrCountBuildingItems(null, tab, null, null);
			}
			jewelControl.Value = value;
		}

		private void ShowPromos(bool show)
		{
			this.showingPromo = show;
			if (!base.IsLoaded())
			{
				return;
			}
			if (this.showingPromo == this.movedDown)
			{
				this.promoGroup.Visible = this.showingPromo;
				this.movedDown = !this.showingPromo;
				float amount = (float)((!show) ? -1 : 1) * this.promoGroup.Height * 0.5f;
				this.MoveUp(this.itemGridParent, amount);
				this.MoveUp(this.tabsParent, amount);
			}
		}

		private void MoveUp(UXElement element, float amount)
		{
			Vector3 localPosition = element.LocalPosition;
			localPosition.y += amount;
			element.LocalPosition = localPosition;
		}

		public void OpenStoreTab(StoreTab tab)
		{
			this.backButtonStringId = "back_to_shop_categories";
			this.SetTab(tab);
		}

		private void SetTab(StoreTab tab)
		{
			if (this.IsTabLocked(tab))
			{
				tab = StoreTab.NotInStore;
			}
			this.curTab = tab;
			if (!base.IsLoaded() || !this.gridInitialized)
			{
				return;
			}
			bool flag = tab != StoreTab.NotInStore;
			this.primaryPage.Visible = !flag;
			this.secondaryPage.Visible = flag;
			this.titleLabel.Visible = !flag;
			this.categoryLabel.Visible = flag;
			this.filterPanel.Visible = false;
			base.GetElement<UXLabel>("LabelBack").Text = this.lang.Get(this.backButtonStringId, new object[0]);
			if (flag)
			{
				this.SetupCurTabElements();
				if (this.tabJewels.ContainsKey(tab))
				{
					int value = StoreScreen.AddOrCountBuildingItems(null, tab, null, null);
					this.tabJewels[tab].Value = value;
				}
			}
			else
			{
				this.titleLabel.Text = this.lang.Get("s_Store", new object[0]);
				this.itemGrid2Rows.Clear();
				this.itemGrid1Row.Clear();
				this.SetupJewel(StoreTab.Fragments);
				this.SetupJewel(StoreTab.Treasure);
				this.SetupJewel(StoreTab.Structures);
				this.SetupJewel(StoreTab.Turrets);
			}
			StoreTab storeTab = tab;
			if (storeTab != StoreTab.Fragments)
			{
				this.RegisterForEventsWithoutModal();
				if (this.ssView.IsModalVisible())
				{
					this.ssView.Hide();
				}
			}
			else
			{
				Service.ShardShopController.SaveShardShopExpiration();
			}
			this.SetupBackButtonDelegate();
		}

		public void ResetCurrentTab()
		{
			if (this.curTab == StoreTab.EventPrizes || this.curTab == StoreTab.Protection || this.curTab == StoreTab.Treasure)
			{
				if (this.Visible)
				{
					this.SetupCurTabElements();
				}
				else
				{
					this.resetCurrentTabOnVisible = true;
				}
			}
		}

		public void AddShardItemView(string name, UXElement item, UXGrid grid, ShardShopViewTO vto)
		{
			bool flag = vto.State != "unlocked";
			bool upgradeable = vto.Upgradeable;
			UXButton subElement = grid.GetSubElement<UXButton>(name, "ButtonFragmentCard");
			if (!flag)
			{
				subElement.OnClicked = new UXButtonClickedDelegate(this.OnShardShopItemButtonClicked);
				subElement.Tag = vto;
			}
			Animator component = subElement.Root.GetComponent<Animator>();
			if (!flag)
			{
				UXLabel subElement2 = grid.GetSubElement<UXLabel>(name, "LabelMesageCountFragmentItems");
				subElement2.Text = vto.RemainingShardsForSale.ToString();
			}
			UXButton subElement3 = grid.GetSubElement<UXButton>(name, "BtnFragmentItemInfo");
			subElement3.OnClicked = new UXButtonClickedDelegate(this.OnShardShopInfoButtonClicked);
			subElement3.Tag = vto;
			if (flag)
			{
				UXLabel subElement4 = grid.GetSubElement<UXLabel>(name, "LabelFragmentRequirement");
				string text = string.Empty;
				if (vto.State == "maxedOut")
				{
					text = Service.Lang.Get("shard_shop_maxed_out", new object[0]);
				}
				else if (vto.State == "soldOut")
				{
					text = Service.Lang.Get("shard_shop_sold_out", new object[0]);
				}
				else if (vto.State == "requiresArmory")
				{
					text = Service.Lang.Get("shard_shop_armory_req", new object[0]);
				}
				else if (vto.State == "requiresCantina")
				{
					text = Service.Lang.Get("shard_shop_cantina_req", new object[0]);
				}
				subElement4.Text = text;
			}
			UXSprite subElement5 = grid.GetSubElement<UXSprite>(name, "SpriteFragmentImage");
			CrateSupplyVO supplyVO = vto.SupplyVO;
			IGeometryVO geometryVO = GameUtils.GetIconVOFromCrateSupply(supplyVO, vto.PlayerHQLevel);
			if (supplyVO.Type == SupplyType.Shard)
			{
				geometryVO = ProjectorUtils.DetermineVOForEquipment((EquipmentVO)geometryVO);
			}
			ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(geometryVO, subElement5);
			ProjectorUtils.GenerateProjector(config);
			if (!flag)
			{
				UXLabel subElement6 = grid.GetSubElement<UXLabel>(name, "LabelFragmentCost");
				UXSprite subElement7 = grid.GetSubElement<UXSprite>(name, "SpriteFragmentCurrencyIcon");
				UXUtils.SetupSingleResourceUI(vto.CostOfNextShard, subElement6, subElement7);
			}
			UXLabel subElement8 = grid.GetSubElement<UXLabel>(name, "LabelFragProgress");
			float value = 0f;
			if (vto.UpgradeShardsRequired > 0)
			{
				subElement8.Visible = true;
				subElement8.Text = vto.UpgradeShardsEarned + "/" + vto.UpgradeShardsRequired;
				value = (float)vto.UpgradeShardsEarned / (float)vto.UpgradeShardsRequired;
			}
			else
			{
				subElement8.Visible = false;
			}
			if (upgradeable)
			{
				UXLabel subElement9 = grid.GetSubElement<UXLabel>(name, "LabelFragProgressMessage1");
				subElement9.Text = this.lang.Get("CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ1", new object[0]);
				UXLabel subElement10 = grid.GetSubElement<UXLabel>(name, "LabelFragProgressMessage2");
				subElement10.Text = this.lang.Get("CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ2", new object[0]);
			}
			UXSlider subElement11 = grid.GetSubElement<UXSlider>(name, "pBarFragmentCount");
			UXSlider subElement12 = grid.GetSubElement<UXSlider>(name, "pBarFragmentCountDelta");
			subElement11.Value = value;
			subElement12.Value = value;
			UXLabel subElement13 = grid.GetSubElement<UXLabel>(name, "LabelFragmentName");
			subElement13.Text = vto.ItemName;
			base.RevertToOriginalNameRecursively(item.Root, name);
			component.SetInteger("Quality", vto.Quality);
			component.SetBool("Visible", true);
			component.SetBool("Locked", vto.State != "unlocked");
			component.SetBool("Upgradeable", upgradeable);
		}

		public static int AddShardItems(List<UXElement> list, StoreTab tab, UXGrid grid, StoreScreen.AddShardItemHandler OnItemAdded)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			ShardShopController shardShopController = Service.ShardShopController;
			ShardShopData currentShopData = shardShopController.CurrentShopData;
			int count = currentShopData.ShardOffers.Count;
			for (int i = 0; i < count; i++)
			{
				ShardShopViewTO shardShopViewTO = shardShopController.GenerateViewTO(i, currentPlayer, currentShopData);
				string shardSlotId = GameUtils.GetShardSlotId(i);
				UXElement uXElement = grid.CloneTemplateItem(shardSlotId);
				OnItemAdded(shardSlotId, uXElement, grid, shardShopViewTO);
				uXElement.Tag = shardShopViewTO;
				list.Add(uXElement);
			}
			return count;
		}

		public static int AddOrCountBuildingItems(List<UXElement> list, StoreTab tab, StoreScreen.AddBuildingItemDelegate onAddBuildingItem, UXGrid grid)
		{
			int num = 0;
			bool flag = list == null;
			bool flag2 = false;
			int num2 = 0;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			FactionType faction = currentPlayer.Faction;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			StaticDataController staticDataController = Service.StaticDataController;
			UnlockController unlockController = Service.UnlockController;
			foreach (BuildingTypeVO current in staticDataController.GetAll<BuildingTypeVO>())
			{
				if (current.Faction == faction && current.StoreTab == tab)
				{
					BuildingTypeVO reqBuilding = null;
					bool flag3 = unlockController.IsUnlocked(current, 0, out reqBuilding);
					if (flag3 || !current.HideIfLocked)
					{
						if (flag && tab == StoreTab.Decorations && current.Type == BuildingType.Turret)
						{
							if (num2 == 0)
							{
								num2 = buildingLookupController.GetBuildingMaxPurchaseQuantity(current, 0);
								flag2 = true;
							}
						}
						else
						{
							int buildingPurchasedQuantity = buildingLookupController.GetBuildingPurchasedQuantity(current);
							int buildingMaxPurchaseQuantity = buildingLookupController.GetBuildingMaxPurchaseQuantity(current, 0);
							if (buildingMaxPurchaseQuantity > 0 && buildingPurchasedQuantity < buildingMaxPurchaseQuantity)
							{
								num += buildingMaxPurchaseQuantity - buildingPurchasedQuantity;
							}
							if (!flag)
							{
								if (tab == StoreTab.Decorations && current.Type == BuildingType.Turret && flag3 && StoreScreen.IsTurretMax(current))
								{
									Entity currentHQ = buildingLookupController.GetCurrentHQ();
									BuildingTypeVO buildingType = currentHQ.Get<BuildingComponent>().BuildingType;
									int lvl = Service.BuildingUpgradeCatalog.GetMaxLevel(buildingType.UpgradeGroup).Lvl;
									if (buildingType.Lvl < lvl)
									{
										onAddBuildingItem(list, current, reqBuilding, true, 1, 1, grid);
									}
									else
									{
										onAddBuildingItem(list, current, reqBuilding, true, 0, 0, grid);
									}
								}
								else
								{
									onAddBuildingItem(list, current, reqBuilding, flag3, buildingPurchasedQuantity, buildingMaxPurchaseQuantity, grid);
								}
							}
						}
					}
				}
			}
			if (flag2)
			{
				int num3 = buildingLookupController.TurretBuildingNodeList.CalculateCount();
				if (num2 > 0 && num3 < num2)
				{
					num += num2 - num3;
				}
			}
			if (list != null)
			{
				list.Sort(new Comparison<UXElement>(StoreScreen.CompareBuildingItem));
			}
			return num;
		}

		private void AddBuildingItem(List<UXElement> list, BuildingTypeVO buildingInfo, BuildingTypeVO reqBuilding, bool reqMet, int curQuantity, int maxQuantity, UXGrid grid)
		{
			StoreItemTag storeItemTag = new StoreItemTag();
			storeItemTag.Uid = buildingInfo.Uid;
			storeItemTag.BuildingInfo = buildingInfo;
			storeItemTag.ReqMet = reqMet;
			storeItemTag.ReqBuilding = reqBuilding;
			storeItemTag.CurQuantity = curQuantity;
			storeItemTag.MaxQuantity = maxQuantity;
			string uid = buildingInfo.Uid;
			UXElement uXElement = grid.CloneTemplateItem(uid);
			uXElement.Tag = storeItemTag;
			storeItemTag.MainElement = grid.GetSubElement<UXElement>(uid, "ItemInfo");
			UXLabel subElement = grid.GetSubElement<UXLabel>(uid, "LabelName");
			subElement.Text = LangUtils.GetBuildingDisplayName(buildingInfo);
			storeItemTag.InfoLabel = grid.GetSubElement<UXLabel>(uid, "LabelItemInfo");
			storeItemTag.InfoLabel.Text = LangUtils.GetBuildingDescription(buildingInfo);
			storeItemTag.InfoLabel.Visible = false;
			UXButton subElement2 = grid.GetSubElement<UXButton>(uid, "BtnItemInfo");
			subElement2.OnClicked = new UXButtonClickedDelegate(this.OnInfoButtonClicked);
			subElement2.Tag = storeItemTag;
			UXSprite subElement3 = grid.GetSubElement<UXSprite>(uid, "SpriteItemImage");
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateBuildingConfig(buildingInfo, subElement3);
			projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			ProjectorUtils.GenerateProjector(projectorConfig);
			string name = UXUtils.FormatAppendedName("Items", uid);
			JewelControl jewelControl = JewelControl.Create(this, name);
			if (jewelControl != null)
			{
				int value = 0;
				if (buildingInfo.Type != BuildingType.Turret && storeItemTag.ReqMet && storeItemTag.MaxQuantity > 0)
				{
					value = storeItemTag.MaxQuantity - storeItemTag.CurQuantity;
				}
				jewelControl.Value = value;
			}
			int credits = buildingInfo.Credits;
			int materials = buildingInfo.Materials;
			int contraband = buildingInfo.Contraband;
			UXUtils.SetupCostElements(this, "Cost", uid, credits, materials, contraband, 0, false, null);
			UXLabel subElement4 = grid.GetSubElement<UXLabel>(uid, "LabelItemRequirement");
			if (storeItemTag.ReqMet && storeItemTag.CurQuantity < storeItemTag.MaxQuantity)
			{
				storeItemTag.CanPurchase = true;
				UXLabel subElement5 = grid.GetSubElement<UXLabel>(uid, "LabelBuildTime");
				subElement5.Text = GameUtils.GetTimeLabelFromSeconds(buildingInfo.Time);
			}
			else
			{
				storeItemTag.CanPurchase = false;
				if (storeItemTag.ReqMet && storeItemTag.CurQuantity >= storeItemTag.MaxQuantity)
				{
					if (storeItemTag.CurQuantity == 0 && storeItemTag.MaxQuantity == 0)
					{
						subElement4.Text = this.lang.Get("BUILDING_MAX", new object[0]);
					}
					else
					{
						int lvl = Service.BuildingUpgradeCatalog.GetMaxLevel(reqBuilding.UpgradeGroup).Lvl;
						int buildingMaxPurchaseQuantity = Service.BuildingLookupController.GetBuildingMaxPurchaseQuantity(storeItemTag.BuildingInfo, lvl);
						if (buildingMaxPurchaseQuantity > maxQuantity)
						{
							subElement4.Text = this.lang.Get("BUILDING_UPGRADE_REQUIREMENT", new object[]
							{
								LangUtils.GetBuildingDisplayName(storeItemTag.ReqBuilding)
							});
						}
						else
						{
							subElement4.Text = this.lang.Get("BUILDING_MAX", new object[0]);
						}
					}
				}
				else
				{
					subElement4.Text = this.lang.Get("BUILDING_REQUIREMENT", new object[]
					{
						storeItemTag.ReqBuilding.Lvl,
						LangUtils.GetBuildingDisplayName(storeItemTag.ReqBuilding)
					});
				}
			}
			subElement4.Visible = !storeItemTag.CanPurchase;
			UXSprite subElement6 = grid.GetSubElement<UXSprite>(uid, "SpriteDim");
			subElement6.Visible = !storeItemTag.CanPurchase;
			UXElement subElement7 = grid.GetSubElement<UXElement>(uid, "CountAndBuildTime");
			subElement7.Visible = storeItemTag.CanPurchase;
			if (storeItemTag.CanPurchase)
			{
				UXLabel subElement8 = grid.GetSubElement<UXLabel>(uid, "LabelItemCount");
				subElement8.Text = ((!this.turretSwappingUnlocked || buildingInfo.Type != BuildingType.Turret) ? this.lang.Get("FRACTION", new object[]
				{
					storeItemTag.CurQuantity,
					storeItemTag.MaxQuantity
				}) : this.lang.Get("TROOP_MULTIPLIER", new object[]
				{
					storeItemTag.CurQuantity
				}));
			}
			UXLabel subElement9 = grid.GetSubElement<UXLabel>(uid, "LabelCurrencyAmount");
			subElement9.Visible = false;
			UXButton subElement10 = grid.GetSubElement<UXButton>(uid, "ButtonItemCard");
			subElement10.OnClicked = new UXButtonClickedDelegate(this.OnBuildingItemButtonClicked);
			subElement10.Tag = storeItemTag;
			storeItemTag.MainButton = subElement10;
			grid.GetSubElement<UXElement>(uid, "PackageBadge").Visible = false;
			grid.GetSubElement<UXElement>(uid, "CrystalBonus").Visible = false;
			this.HideLEIElements(uid);
			list.Add(uXElement);
		}

		private void AddSupplyCrates(List<UXElement> list)
		{
			List<CrateVO> list2 = new List<CrateVO>();
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (CrateVO current in staticDataController.GetAll<CrateVO>())
			{
				if (CrateUtils.IsVisibleInStore(current))
				{
					list2.Add(current);
				}
			}
			list2.Sort(new Comparison<CrateVO>(this.CompareCrates));
			int i = 0;
			int count = list2.Count;
			while (i < count)
			{
				CrateVO crateVO = list2[i];
				StoreItemTag storeItemTag = new StoreItemTag();
				storeItemTag.Amount = 1;
				storeItemTag.Price = crateVO.Crystals;
				storeItemTag.Currency = CurrencyType.Crystals;
				storeItemTag.ReqMet = true;
				storeItemTag.CanPurchase = true;
				string uid = crateVO.Uid;
				storeItemTag.Uid = uid;
				UXElement uXElement = this.itemGrid2Rows.CloneTemplateItem(uid);
				uXElement.Tag = storeItemTag;
				storeItemTag.MainElement = this.itemGrid2Rows.GetSubElement<UXElement>(uid, "ItemInfo");
				UXLabel subElement = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelName");
				subElement.Text = LangUtils.GetCrateDisplayName(crateVO);
				this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemInfo").Visible = false;
				this.itemGrid2Rows.GetSubElement<UXButton>(uid, "BtnItemInfo").Visible = false;
				UXSprite subElement2 = this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteItemImage");
				subElement2.SpriteName = "bkgClear";
				RewardUtils.SetCrateIcon(subElement2, crateVO, AnimState.Closed);
				string name = UXUtils.FormatAppendedName("Items", uid);
				JewelControl.Create(this, name);
				UXLabel subElement3 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemRequirement");
				subElement3.Visible = false;
				UXSprite subElement4 = this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteDim");
				subElement4.Visible = !storeItemTag.ReqMet;
				int crystals = (!storeItemTag.ReqMet) ? 0 : storeItemTag.Price;
				UXUtils.SetupCostElements(this, "Cost", uid, 0, 0, 0, crystals, false, null);
				this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelBuildTime").Visible = false;
				this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteItemTimeIcon").Visible = false;
				this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemCount").Visible = false;
				this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelCurrencyAmount").Visible = false;
				UXButton subElement5 = this.itemGrid2Rows.GetSubElement<UXButton>(uid, "ButtonItemCard");
				subElement5.OnClicked = new UXButtonClickedDelegate(this.OnSupplyCrateItemButtonClicked);
				subElement5.Tag = storeItemTag;
				this.itemGrid2Rows.GetSubElement<UXElement>(uid, "PackageBadge").Visible = false;
				this.itemGrid2Rows.GetSubElement<UXElement>(uid, "CrystalBonus").Visible = false;
				this.HideLEIElements(uid);
				list.Add(uXElement);
				i++;
			}
		}

		private void HideLEIElements(string itemUid)
		{
			this.itemGrid2Rows.GetSubElement<UXElement>(itemUid, "TemplateBgSpecial").Visible = false;
			this.itemGrid2Rows.GetSubElement<UXLabel>(itemUid, "LabelNameSpecial").Visible = false;
			this.itemGrid2Rows.GetSubElement<UXLabel>(itemUid, "LabelTimerSpecial").Visible = false;
		}

		private void AddLEItems(List<UXElement> list, StoreTab currentTab, List<ILimitedEditionItemVO> leItems)
		{
			int i = 0;
			int count = leItems.Count;
			while (i < count)
			{
				LimitedEditionItemVO limitedEditionItemVO = (LimitedEditionItemVO)leItems[i];
				if (limitedEditionItemVO.StoreTab == currentTab)
				{
					StoreItemTag storeItemTag = new StoreItemTag();
					storeItemTag.Amount = 1;
					storeItemTag.ReqMet = true;
					storeItemTag.CanPurchase = true;
					if (limitedEditionItemVO.Credits > 0)
					{
						storeItemTag.Currency = CurrencyType.Credits;
						storeItemTag.Price = limitedEditionItemVO.Credits;
					}
					else if (limitedEditionItemVO.Materials > 0)
					{
						storeItemTag.Currency = CurrencyType.Materials;
						storeItemTag.Price = limitedEditionItemVO.Materials;
					}
					else if (limitedEditionItemVO.Contraband > 0)
					{
						storeItemTag.Currency = CurrencyType.Contraband;
						storeItemTag.Price = limitedEditionItemVO.Contraband;
					}
					else if (limitedEditionItemVO.Crystals > 0)
					{
						storeItemTag.Currency = CurrencyType.Crystals;
						storeItemTag.Price = limitedEditionItemVO.Crystals;
					}
					string uid = limitedEditionItemVO.Uid;
					storeItemTag.Uid = uid;
					UXElement uXElement = this.itemGrid2Rows.CloneTemplateItem(uid);
					uXElement.Tag = storeItemTag;
					storeItemTag.MainElement = this.itemGrid2Rows.GetSubElement<UXElement>(uid, "ItemInfo");
					UXLabel subElement = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelNameSpecial");
					subElement.Text = LangUtils.GetLEIDisplayName(limitedEditionItemVO.Uid);
					this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemInfo").Visible = false;
					this.itemGrid2Rows.GetSubElement<UXButton>(uid, "BtnItemInfo").Visible = false;
					UXSprite subElement2 = this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteItemImage");
					subElement2.SpriteName = "bkgClear";
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(limitedEditionItemVO, subElement2);
					projectorConfig.AnimPreference = AnimationPreference.NoAnimation;
					projectorConfig.AnimState = AnimState.Closed;
					ProjectorUtils.GenerateProjector(projectorConfig);
					string name = UXUtils.FormatAppendedName("Items", uid);
					JewelControl.Create(this, name);
					UXLabel subElement3 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemRequirement");
					subElement3.Visible = false;
					UXSprite subElement4 = this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteDim");
					subElement4.Visible = !storeItemTag.ReqMet;
					UXUtils.SetupCostElements(this, "Cost", uid, limitedEditionItemVO.Credits, limitedEditionItemVO.Materials, limitedEditionItemVO.Contraband, limitedEditionItemVO.Crystals, false, null);
					this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelBuildTime").Visible = false;
					this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteItemTimeIcon").Visible = false;
					this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemCount").Visible = false;
					this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelCurrencyAmount").Visible = false;
					UXButton subElement5 = this.itemGrid2Rows.GetSubElement<UXButton>(uid, "ButtonItemCard");
					subElement5.OnClicked = new UXButtonClickedDelegate(this.OnLEItemButtonClicked);
					subElement5.Tag = storeItemTag;
					this.itemGrid2Rows.GetSubElement<UXElement>(uid, "PackageBadge").Visible = false;
					this.itemGrid2Rows.GetSubElement<UXElement>(uid, "CrystalBonus").Visible = false;
					this.itemGrid2Rows.GetSubElement<UXElement>(uid, "TemplateBg").Visible = false;
					this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelName").Visible = false;
					UXLabel subElement6 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelTimerSpecial");
					subElement6.Visible = true;
					subElement6.TextColor = UXUtils.COLOR_CRATE_EXPIRE_LABEL_NORMAL;
					CountdownControl countdownControl = new CountdownControl(subElement6, this.lang.Get("CRATE_STORE_LEI_EXPIRATION_TIMER", new object[0]), limitedEditionItemVO.EndTime);
					countdownControl.SetThreshold(GameConstants.CRATE_INVENTORY_LEI_EXPIRATION_TIMER_WARNING, UXUtils.COLOR_CRATE_EXPIRE_LABEL_WARNING);
					list.Add(uXElement);
				}
				i++;
			}
		}

		private int CompareCrates(CrateVO a, CrateVO b)
		{
			return a.Crystals - b.Crystals;
		}

		private void AddCurrencyOrProtectionItems(List<UXElement> list, bool forProtection, CurrencyType currencyType)
		{
			string[] array = null;
			bool[] array2 = null;
			int[] array3;
			int[] array4;
			if (forProtection)
			{
				GameUtils.GetProtectionPacks(out array3, out array4);
			}
			else
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				int num = StoreScreen.SOFT_CURRENCY_PERCENTS.Length;
				array3 = new int[num];
				array4 = new int[num];
				array = new string[num];
				array2 = new bool[num];
				int num2 = 0;
				int num3 = 0;
				switch (currencyType)
				{
				case CurrencyType.Credits:
					num2 = currentPlayer.CurrentCreditsAmount;
					num3 = currentPlayer.MaxCreditsAmount;
					break;
				case CurrencyType.Materials:
					num2 = currentPlayer.CurrentMaterialsAmount;
					num3 = currentPlayer.MaxMaterialsAmount;
					break;
				case CurrencyType.Contraband:
					num2 = currentPlayer.CurrentContrabandAmount;
					num3 = currentPlayer.MaxContrabandAmount;
					break;
				}
				for (int i = 0; i < num; i++)
				{
					int num4 = StoreScreen.SOFT_CURRENCY_PERCENTS[i];
					int num5 = (num4 != 100) ? (num3 * num4 / 100) : (num3 - num2);
					array3[i] = num5;
					int num6 = 0;
					switch (currencyType)
					{
					case CurrencyType.Credits:
						array[i] = "Credits" + num4;
						num6 = GameUtils.CreditsCrystalCost(num5);
						break;
					case CurrencyType.Materials:
						array[i] = "Materials" + num4;
						num6 = GameUtils.MaterialsCrystalCost(num5);
						break;
					case CurrencyType.Contraband:
						array[i] = "Contraband" + num4;
						num6 = GameUtils.ContrabandCrystalCost(num5);
						break;
					}
					array4[i] = num6;
					array2[i] = (num5 > 0 && num3 > 0 && num2 + num5 <= num3);
				}
			}
			int j = 0;
			int num7 = array3.Length;
			while (j < num7)
			{
				int num8 = j + 1;
				StoreItemTag storeItemTag = new StoreItemTag();
				storeItemTag.Amount = ((!forProtection) ? array3[j] : num8);
				storeItemTag.Price = array4[j];
				storeItemTag.Currency = currencyType;
				storeItemTag.ReqMet = (forProtection || array2[j]);
				if (forProtection)
				{
					StoreItemTag arg_21C_0 = storeItemTag;
					bool flag = this.IsProtectionPackAvailable(num8);
					storeItemTag.ReqMet = flag;
					arg_21C_0.CanPurchase = flag;
				}
				if (!forProtection && array != null)
				{
					storeItemTag.Uid = array[j];
				}
				string text = string.Format("{0}_{1}", (!forProtection) ? currencyType.ToString() : "protection", num8);
				UXElement uXElement = this.itemGrid2Rows.CloneTemplateItem(text);
				uXElement.Tag = storeItemTag;
				storeItemTag.MainElement = this.itemGrid2Rows.GetSubElement<UXElement>(text, "ItemInfo");
				UXLabel subElement = this.itemGrid2Rows.GetSubElement<UXLabel>(text, "LabelName");
				string format;
				if (forProtection)
				{
					format = "PACK_PROTECTION{0}";
				}
				else
				{
					format = "PACK_CURRENCY{0}";
				}
				subElement.Text = this.lang.Get(string.Format(format, num8), new object[]
				{
					this.lang.Get(currencyType.ToString().ToUpper(), new object[0])
				});
				storeItemTag.InfoLabel = this.itemGrid2Rows.GetSubElement<UXLabel>(text, "LabelItemInfo");
				storeItemTag.InfoLabel.Visible = false;
				UXButton subElement2 = this.itemGrid2Rows.GetSubElement<UXButton>(text, "BtnItemInfo");
				subElement2.Visible = false;
				UXSprite subElement3 = this.itemGrid2Rows.GetSubElement<UXSprite>(text, "SpriteItemImage");
				string text2 = (!forProtection) ? currencyType.ToString() : "protection";
				UXUtils.SetupGeometryForIcon(subElement3, text2, num8);
				storeItemTag.IconName = UXUtils.GetCurrencyItemAssetName(text2, num8);
				string name = UXUtils.FormatAppendedName("Items", text);
				JewelControl.Create(this, name);
				UXLabel subElement4 = this.itemGrid2Rows.GetSubElement<UXLabel>(text, "LabelItemRequirement");
				if (!storeItemTag.ReqMet && !forProtection)
				{
					subElement4.Text = this.lang.Get("STORE_TREASURE_LIMIT", new object[0]);
				}
				else if (!storeItemTag.ReqMet && forProtection)
				{
					this.AddProtectionCooldownTimer();
				}
				else
				{
					subElement4.Visible = false;
				}
				UXSprite subElement5 = this.itemGrid2Rows.GetSubElement<UXSprite>(text, "SpriteDim");
				subElement5.Visible = !storeItemTag.ReqMet;
				if (forProtection && !storeItemTag.CanPurchase)
				{
					subElement5.Visible = true;
				}
				int crystals = (!storeItemTag.ReqMet) ? 0 : storeItemTag.Price;
				UXUtils.SetupCostElements(this, "Cost", text, 0, 0, 0, crystals, false, null);
				UXLabel subElement6 = this.itemGrid2Rows.GetSubElement<UXLabel>(text, "LabelBuildTime");
				subElement6.Visible = false;
				UXSprite subElement7 = this.itemGrid2Rows.GetSubElement<UXSprite>(text, "SpriteItemTimeIcon");
				subElement7.Visible = false;
				UXLabel subElement8 = this.itemGrid2Rows.GetSubElement<UXLabel>(text, "LabelItemCount");
				subElement8.Visible = false;
				UXLabel subElement9 = this.itemGrid2Rows.GetSubElement<UXLabel>(text, "LabelCurrencyAmount");
				if (!forProtection && storeItemTag.Amount > 0)
				{
					subElement9.Text = this.lang.Get("CURRENCY_VALUE_NAME", new object[]
					{
						this.lang.ThousandsSeparated(storeItemTag.Amount),
						this.lang.Get(currencyType.ToString().ToUpper(), new object[0])
					});
				}
				else
				{
					subElement9.Visible = false;
				}
				UXButton subElement10 = this.itemGrid2Rows.GetSubElement<UXButton>(text, "ButtonItemCard");
				if (forProtection)
				{
					subElement10.OnClicked = new UXButtonClickedDelegate(this.OnProtectionItemButtonClicked);
				}
				else
				{
					subElement10.OnClicked = new UXButtonClickedDelegate(this.OnSoftCurrencyItemButtonClicked);
				}
				subElement10.Tag = storeItemTag;
				this.itemGrid2Rows.GetSubElement<UXElement>(text, "PackageBadge").Visible = false;
				this.itemGrid2Rows.GetSubElement<UXElement>(text, "CrystalBonus").Visible = false;
				this.HideLEIElements(text);
				list.Add(uXElement);
				j++;
			}
		}

		protected virtual void AddEventPrizeItems(List<UXElement> list)
		{
		}

		private List<SaleItemTypeVO> GetCurrentItemsOnSale()
		{
			SaleTypeVO currentActiveSale = SaleUtils.GetCurrentActiveSale();
			List<SaleItemTypeVO> list = null;
			if (this.curTab == StoreTab.Treasure && currentActiveSale != null)
			{
				list = SaleUtils.GetSaleItems(currentActiveSale.SaleItems);
				if (list.Count > 0)
				{
					UXLabel element = base.GetElement<UXLabel>("CrystalBonusLabelTitle");
					element.Text = this.lang.Get(currentActiveSale.Title, new object[0]);
					UXLabel element2 = base.GetElement<UXLabel>("CyrstalBonusLabelExpire");
					int secondsRemaining = TimedEventUtils.GetSecondsRemaining(currentActiveSale);
					element2.Text = this.lang.Get("crystal_bonus_ends_in", new object[]
					{
						LangUtils.FormatTime((long)secondsRemaining)
					});
				}
			}
			return list;
		}

		private void SetupSaleItem(string itemUid, SaleItemTypeVO saleItem, StoreItemTag tag)
		{
			if (saleItem != null)
			{
				this.itemGrid2Rows.GetSubElement<UXElement>(itemUid, "PackageBadge").Visible = false;
				base.GetElement<UXElement>("CrystalBonusTitleContainer").Visible = true;
				this.visibleSale = SaleUtils.GetCurrentActiveSale();
				this.itemGrid2Rows.GetSubElement<UXElement>(itemUid, "CrystalBonus").Visible = true;
				this.itemGrid2Rows.GetSubElement<UXLabel>(itemUid, "LabelCurrencyAmount").Visible = false;
				UXLabel subElement = this.itemGrid2Rows.GetSubElement<UXLabel>(itemUid, "CrystalBonusLabel");
				subElement.Text = this.lang.Get("crystal_percent_bonus", new object[]
				{
					Math.Round((saleItem.BonusMultiplier - 1.0) * 100.0)
				});
				UXLabel subElement2 = this.itemGrid2Rows.GetSubElement<UXLabel>(itemUid, "CrystalBonusLabelAmount");
				int num = (int)Math.Round((double)tag.Amount * saleItem.BonusMultiplier);
				subElement2.Text = this.lang.Get("CURRENCY_VALUE_NAME", new object[]
				{
					this.lang.ThousandsSeparated(num),
					this.lang.Get(tag.Currency.ToString().ToUpper(), new object[0])
				});
				UXLabel subElement3 = this.itemGrid2Rows.GetSubElement<UXLabel>(itemUid, "CrystalBonusLabelBonusAmount");
				subElement3.Text = this.lang.Get("crystal_amount_bonus", new object[]
				{
					num - tag.Amount
				});
			}
			else
			{
				this.itemGrid2Rows.GetSubElement<UXElement>(itemUid, "CrystalBonus").Visible = false;
			}
		}

		private void RemoveSaleTitle()
		{
			base.GetElement<UXElement>("CrystalBonusTitleContainer").Visible = false;
			this.visibleSale = null;
		}

		private void AddIAPItems(List<UXElement> list)
		{
			InAppPurchaseController inAppPurchaseController = Service.InAppPurchaseController;
			List<InAppPurchaseTypeVO> validIAPTypes = inAppPurchaseController.GetValidIAPTypes();
			CurrencyType currencyType = CurrencyType.Crystals;
			List<SaleItemTypeVO> currentItemsOnSale = this.GetCurrentItemsOnSale();
			int count = validIAPTypes.Count;
			for (int i = 0; i < count; i++)
			{
				InAppPurchaseTypeVO inAppPurchaseTypeVO = validIAPTypes[i];
				InAppPurchaseProductInfo iAPProduct = inAppPurchaseController.GetIAPProduct(inAppPurchaseTypeVO.ProductId);
				string text = this.lang.Get("iap_title_" + inAppPurchaseTypeVO.Uid, new object[0]);
				string text2 = this.lang.Get("iap_desc_" + inAppPurchaseTypeVO.Uid, new object[0]);
				StoreItemTag storeItemTag = new StoreItemTag();
				storeItemTag.Amount = inAppPurchaseTypeVO.Amount;
				storeItemTag.Currency = currencyType;
				storeItemTag.IAPType = inAppPurchaseTypeVO;
				storeItemTag.IAPProduct = iAPProduct;
				string uid = inAppPurchaseTypeVO.Uid;
				UXElement uXElement = this.itemGrid2Rows.CloneTemplateItem(uid);
				uXElement.Tag = storeItemTag;
				storeItemTag.MainElement = this.itemGrid2Rows.GetSubElement<UXElement>(uid, "ItemInfo");
				UXLabel subElement = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelName");
				subElement.Text = text;
				storeItemTag.InfoLabel = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemInfo");
				storeItemTag.InfoLabel.Text = text2;
				storeItemTag.InfoLabel.Visible = false;
				UXButton subElement2 = this.itemGrid2Rows.GetSubElement<UXButton>(uid, "BtnItemInfo");
				subElement2.Visible = false;
				UXSprite subElement3 = this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteItemImage");
				Service.InAppPurchaseController.SetIAPRewardIcon(subElement3, inAppPurchaseTypeVO.Uid);
				string name = UXUtils.FormatAppendedName("Items", uid);
				JewelControl.Create(this, name);
				UXLabel subElement4 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemRequirement");
				subElement4.Visible = false;
				UXUtils.SetupCostElements(this, "Cost", uid, 0, 0, 0, 0, false, iAPProduct.FormattedRealCost);
				UXLabel subElement5 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelBuildTime");
				subElement5.Visible = false;
				UXSprite subElement6 = this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteItemTimeIcon");
				subElement6.Visible = false;
				UXLabel subElement7 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelItemCount");
				subElement7.Visible = false;
				UXLabel subElement8 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelCurrencyAmount");
				subElement8.Text = this.lang.Get("CURRENCY_VALUE_NAME", new object[]
				{
					this.lang.ThousandsSeparated(storeItemTag.Amount),
					this.lang.Get(currencyType.ToString().ToUpper(), new object[0])
				});
				UXSprite subElement9 = this.itemGrid2Rows.GetSubElement<UXSprite>(uid, "SpriteDim");
				subElement9.Visible = false;
				UXButton subElement10 = this.itemGrid2Rows.GetSubElement<UXButton>(uid, "ButtonItemCard");
				subElement10.OnClicked = new UXButtonClickedDelegate(this.OnIAPItemButtonClicked);
				subElement10.Tag = storeItemTag;
				subElement10.Enabled = true;
				this.itemGrid2Rows.GetSubElement<UXElement>(uid, "PackageBadge").Visible = true;
				this.itemGrid2Rows.GetSubElement<UXElement>(uid, "SpritePackageBg").Visible = (inAppPurchaseTypeVO.TopBadgeString != null);
				UXLabel subElement11 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelPackageTop");
				UXLabel subElement12 = this.itemGrid2Rows.GetSubElement<UXLabel>(uid, "LabelPackageBottom");
				subElement11.Visible = (inAppPurchaseTypeVO.TopBadgeString != null);
				subElement12.Visible = (inAppPurchaseTypeVO.BottomBadgeString != null);
				if (inAppPurchaseTypeVO.TopBadgeString != null)
				{
					subElement11.Text = this.lang.Get(inAppPurchaseTypeVO.TopBadgeString, new object[0]);
				}
				if (inAppPurchaseTypeVO.BottomBadgeString != null)
				{
					subElement12.Text = this.lang.Get(inAppPurchaseTypeVO.BottomBadgeString, new object[0]);
				}
				SaleItemTypeVO saleItem = null;
				if (currentItemsOnSale != null)
				{
					foreach (SaleItemTypeVO current in currentItemsOnSale)
					{
						if (current.ProductId == inAppPurchaseTypeVO.ProductId)
						{
							saleItem = current;
							break;
						}
					}
				}
				this.SetupSaleItem(uid, saleItem, storeItemTag);
				this.HideLEIElements(uid);
				list.Add(uXElement);
			}
		}

		private UXGrid GetActiveGrid()
		{
			if (this.curTab == StoreTab.Structures)
			{
				return this.itemGridFiltered;
			}
			if (this.curTab == StoreTab.Fragments)
			{
				return this.itemGrid1Row;
			}
			return this.itemGrid2Rows;
		}

		private void SetupCurTabElements()
		{
			this.itemGrid2Rows.Clear();
			this.itemGrid1Row.Clear();
			this.itemGridFiltered.Clear();
			UXGrid activeGrid = this.GetActiveGrid();
			this.tabHelper.HideTabs(this);
			if (this.curTab == StoreTab.Fragments)
			{
				activeGrid.SetTemplateItem("FragmentItemTemplate");
				UXElement element = base.GetElement<UXElement>("FragmentItemTemplate");
				element.Visible = true;
			}
			else
			{
				activeGrid.SetTemplateItem("StoreItemTemplate");
			}
			this.categoryLabel.Text = this.lang.Get("s_" + this.curTab.ToString().ToLower(), new object[0]);
			List<UXElement> list = new List<UXElement>();
			this.RemoveProtectionCooldownTimer();
			this.RemoveSaleTitle();
			base.GetElement<UXButton>("BtnBack").Visible = true;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			List<ILimitedEditionItemVO> validLEIs = Service.LimitedEditionItemController.ValidLEIs;
			this.expirationLabel.Visible = false;
			switch (this.curTab)
			{
			case StoreTab.Treasure:
				this.AddLEItems(list, this.curTab, validLEIs);
				this.AddSupplyCrates(list);
				if (buildingLookupController.IsContrabandUnlocked())
				{
					this.AddCurrencyOrProtectionItems(list, false, CurrencyType.Contraband);
				}
				this.AddCurrencyOrProtectionItems(list, false, CurrencyType.Credits);
				this.AddCurrencyOrProtectionItems(list, false, CurrencyType.Materials);
				goto IL_2DF;
			case StoreTab.Protection:
				base.GetElement<UXButton>("BtnBack").Visible = false;
				this.AddLEItems(list, this.curTab, validLEIs);
				this.AddCurrencyOrProtectionItems(list, true, CurrencyType.Crystals);
				goto IL_2DF;
			case StoreTab.EventPrizes:
				this.AddLEItems(list, this.curTab, validLEIs);
				this.AddEventPrizeItems(list);
				goto IL_2DF;
			case StoreTab.Turrets:
				this.AddLEItems(list, this.curTab, validLEIs);
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Decorations, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), activeGrid);
				goto IL_2DF;
			case StoreTab.Crystals:
				this.AddIAPItems(list);
				this.AddLEItems(list, this.curTab, validLEIs);
				goto IL_2DF;
			case StoreTab.Fragments:
			{
				ShardShopData currentShopData = Service.ShardShopController.CurrentShopData;
				if (currentShopData != null && currentShopData.ActiveSeries != null)
				{
					StoreScreen.AddShardItems(list, this.curTab, activeGrid, new StoreScreen.AddShardItemHandler(this.AddShardItemView));
					this.CreateShardShopCountdown(currentShopData);
					this.expirationLabel.Visible = true;
				}
				goto IL_2DF;
			}
			case StoreTab.Structures:
				this.filterPanel.Visible = true;
				this.itemGridFiltered.Clear();
				this.AddLEItems(list, this.curTab, validLEIs);
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Army, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), activeGrid);
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Resources, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), activeGrid);
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Defenses, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), activeGrid);
				goto IL_2DF;
			}
			this.AddLEItems(list, this.curTab, validLEIs);
			StoreScreen.AddOrCountBuildingItems(list, this.curTab, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), activeGrid);
			IL_2DF:
			if (this.curTab == StoreTab.Turrets && list.Count > 0 && this.turretSwappingUnlocked)
			{
				StoreItemTag storeItemTag = (StoreItemTag)list[0].Tag;
				int num = buildingLookupController.TurretBuildingNodeList.CalculateCount();
				int buildingMaxPurchaseQuantity = buildingLookupController.GetBuildingMaxPurchaseQuantity(storeItemTag.BuildingInfo, 0);
				this.turretLabelGroup.Visible = true;
				this.turretLabel.Text = this.lang.Get("TURRET_COUNT", new object[]
				{
					num,
					buildingMaxPurchaseQuantity
				});
				if (num >= buildingMaxPurchaseQuantity)
				{
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						storeItemTag = (StoreItemTag)list[i].Tag;
						storeItemTag.CanPurchase = false;
						storeItemTag.MaxQuantity = storeItemTag.CurQuantity;
						i++;
					}
				}
			}
			else
			{
				this.turretLabelGroup.Visible = false;
				this.turretLabel.Text = string.Empty;
			}
			if (this.curTab != StoreTab.Fragments)
			{
				UXUtils.SortListForTwoRowGrids(list, activeGrid);
			}
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				activeGrid.AddItem(list[j], j);
				j++;
			}
			activeGrid.RepositionItems();
			Service.EventManager.SendEvent(EventId.StoreScreenReady, this);
		}

		private static bool IsTurretMax(BuildingTypeVO buildingInfo)
		{
			if (Service.BuildingLookupController.IsTurretSwappingUnlocked())
			{
				BuildingLookupController buildingLookupController = Service.BuildingLookupController;
				int num = buildingLookupController.TurretBuildingNodeList.CalculateCount();
				int buildingMaxPurchaseQuantity = buildingLookupController.GetBuildingMaxPurchaseQuantity(buildingInfo, 0);
				if (num >= buildingMaxPurchaseQuantity)
				{
					return true;
				}
			}
			return false;
		}

		private static int CompareBuildingItem(UXElement a, UXElement b)
		{
			if (a == b)
			{
				return 0;
			}
			BuildingTypeVO buildingInfo = ((StoreItemTag)a.Tag).BuildingInfo;
			BuildingTypeVO buildingInfo2 = ((StoreItemTag)b.Tag).BuildingInfo;
			int num = buildingInfo.Order - buildingInfo2.Order;
			if (num == 0)
			{
				Service.Logger.WarnFormat("Building {0} matches order ({1}) of {2}", new object[]
				{
					buildingInfo.Uid,
					buildingInfo.Order,
					buildingInfo2.Uid
				});
			}
			return num;
		}

		public void ScrollToItem(string itemUid)
		{
			this.curItem = itemUid;
			if (string.IsNullOrEmpty(this.curItem) || !base.IsLoaded() || !this.gridInitialized)
			{
				return;
			}
			int i = 0;
			int count = this.itemGrid2Rows.Count;
			while (i < count)
			{
				UXElement item = this.itemGrid2Rows.GetItem(i);
				StoreItemTag storeItemTag = item.Tag as StoreItemTag;
				if (storeItemTag.Uid == this.curItem && this.itemGrid2Rows.IsGridComponentScrollable())
				{
					this.itemGrid2Rows.RepositionItems();
					this.itemGrid2Rows.ScrollToItem(i);
					break;
				}
				i++;
			}
		}

		public void EnableScrollListMovement(bool enable)
		{
			this.gridScrollable = enable;
			if (this.gridInitialized)
			{
				this.itemGrid2Rows.IsScrollable = enable;
			}
		}

		public bool IsShardShopModalVisible()
		{
			return this.ssView.IsModalVisible();
		}

		private void OnTabButtonClicked(UXButton button)
		{
			StoreTab storeTab = (StoreTab)((int)button.Tag);
			this.backButtonStringId = "s_Back";
			if (storeTab != this.curTab)
			{
				if (this.IsTabLocked(storeTab))
				{
					return;
				}
				if (storeTab != StoreTab.Treasure)
				{
					Service.EventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
				}
				this.itemGrid2Rows.Scroll(0f);
				this.SetTab(storeTab);
				if (storeTab == StoreTab.NotInStore)
				{
					Service.EventManager.SendEvent(EventId.BackButtonClicked, null);
				}
				else
				{
					Service.EventManager.SendEvent(EventId.StoreCategorySelected, storeTab);
				}
			}
		}

		public void SetupBackButtonDelegate()
		{
			if (this.curTab == StoreTab.NotInStore || this.curTab == StoreTab.Protection)
			{
				base.InitDefaultBackDelegate();
			}
			else
			{
				base.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnTabButtonClicked);
				base.CurrentBackButton = this.backButton;
			}
		}

		public bool IsTabLocked(StoreTab tab)
		{
			if (tab != StoreTab.Crystals)
			{
				return tab == StoreTab.Fragments && !Service.ShardShopController.IsShardShopUnlocked();
			}
			return !this.HasInAppPurchaseItems();
		}

		private void OnInfoButtonClicked(UXButton button)
		{
			StoreItemTag storeItemTag = button.Tag as StoreItemTag;
			storeItemTag.MainElement.Visible = false;
			storeItemTag.InfoLabel.Visible = true;
			Service.EventManager.SendEvent(EventId.InfoButtonClicked, null);
		}

		private void OnShardShopInfoButtonClicked(UXButton button)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			ShardShopViewTO shardShopViewTO = (ShardShopViewTO)button.Tag;
			if (shardShopViewTO.SupplyVO.Type == SupplyType.Shard)
			{
				string rewardUid = shardShopViewTO.SupplyVO.RewardUid;
				int level = currentPlayer.UnlockedLevels.Equipment.GetLevel(rewardUid);
				EquipmentVO byLevel = Service.EquipmentUpgradeCatalog.GetByLevel(rewardUid, level);
				Service.ScreenController.AddScreen(new EquipmentInfoScreen(byLevel, null, null, false, false));
			}
			else
			{
				ShardVO shardVO = staticDataController.Get<ShardVO>(shardShopViewTO.SupplyVO.RewardUid);
				string targetGroupId = shardVO.TargetGroupId;
				int level2 = currentPlayer.UnlockedLevels.Troops.GetLevel(targetGroupId);
				IDeployableVO byLevel2;
				if (shardVO.TargetType == "specialAttack")
				{
					byLevel2 = Service.StarshipUpgradeCatalog.GetByLevel(shardVO.TargetGroupId, level2);
				}
				else
				{
					byLevel2 = Service.TroopUpgradeCatalog.GetByLevel(shardVO.TargetGroupId, level2);
				}
				TroopUpgradeTag selectedTroop = new TroopUpgradeTag(byLevel2, true);
				Service.ScreenController.AddScreen(new DeployableInfoScreen(selectedTroop, null, false, null));
			}
		}

		private void OnShardShopItemButtonClicked(UXButton button)
		{
			ShardShopViewTO shardShopViewTO = (ShardShopViewTO)button.Tag;
			ShardShopData currentShopData = Service.ShardShopController.CurrentShopData;
			int num = 0;
			foreach (int current in currentShopData.Purchases[shardShopViewTO.SlotIndex].Values)
			{
				num += current;
			}
			string cookie = string.Concat(new string[]
			{
				(shardShopViewTO.SlotIndex + 1).ToString(),
				"|",
				shardShopViewTO.SupplyVO.RewardUid,
				"||",
				num.ToString(),
				"|",
				shardShopViewTO.RemainingShardsForSale.ToString()
			});
			Service.EventManager.SendEvent(EventId.ShardItemSelectedFromStore, cookie);
			this.UnregisterForEventsWithoutModal();
			this.ssView.Show(shardShopViewTO);
		}

		private void OnBuildingItemButtonClicked(UXButton button)
		{
			StoreItemTag storeItemTag = button.Tag as StoreItemTag;
			if (storeItemTag.InfoLabel.Visible)
			{
				storeItemTag.InfoLabel.Visible = false;
				storeItemTag.MainElement.Visible = true;
				return;
			}
			if (!storeItemTag.CanPurchase)
			{
				string text = null;
				BuildingTypeVO reqBuilding = storeItemTag.ReqBuilding;
				if (reqBuilding != null)
				{
					if (!storeItemTag.ReqMet)
					{
						text = this.lang.Get("STORE_MESSAGE_UNLOCK", new object[]
						{
							reqBuilding.Lvl,
							LangUtils.GetBuildingDisplayName(reqBuilding)
						});
					}
					else if (storeItemTag.CurQuantity == storeItemTag.MaxQuantity)
					{
						BuildingLookupController buildingLookupController = Service.BuildingLookupController;
						BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
						int num = 0;
						List<Entity> buildingListByType = buildingLookupController.GetBuildingListByType(reqBuilding.Type);
						int i = 0;
						int count = buildingListByType.Count;
						while (i < count)
						{
							num = Math.Max(num, buildingListByType[i].Get<BuildingComponent>().BuildingType.Lvl);
							i++;
						}
						int lvl = buildingUpgradeCatalog.GetMaxLevel(reqBuilding.UpgradeGroup).Lvl;
						if (num < lvl)
						{
							int buildingMaxPurchaseQuantity = buildingLookupController.GetBuildingMaxPurchaseQuantity(storeItemTag.BuildingInfo, num + 1);
							if (buildingMaxPurchaseQuantity > storeItemTag.MaxQuantity)
							{
								text = this.lang.Get("STORE_MESSAGE_UPGRADE", new object[]
								{
									LangUtils.GetBuildingDisplayName(reqBuilding)
								});
							}
						}
					}
				}
				if (text == null)
				{
					text = this.lang.Get("STORE_MESSAGE_MAX", new object[0]);
				}
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(text);
				return;
			}
			button.Enabled = false;
			BuildingController buildingController = Service.BuildingController;
			buildingController.PrepareAndPurchaseNewBuilding(storeItemTag.BuildingInfo);
			Service.EventManager.SendEvent(EventId.BuildingSelectedFromStore, null);
			this.Close(storeItemTag.BuildingInfo.Uid);
		}

		public virtual void OnViewClockTime(float dt)
		{
			switch (this.curTab)
			{
			case StoreTab.NotInStore:
				this.stickersViewModule.CheckForStickerExpiration();
				break;
			case StoreTab.Treasure:
				if (this.visibleSale != null)
				{
					if (!TimedEventUtils.IsTimedEventActive(this.visibleSale))
					{
						this.requiresRefresh = true;
						this.RefreshView();
					}
					else
					{
						UXLabel element = base.GetElement<UXLabel>("CyrstalBonusLabelExpire");
						int secondsRemaining = TimedEventUtils.GetSecondsRemaining(this.visibleSale);
						element.Text = this.lang.Get("crystal_bonus_ends_in", new object[]
						{
							LangUtils.FormatTime((long)secondsRemaining)
						});
					}
				}
				break;
			case StoreTab.Protection:
				if (this.enableTimer)
				{
					this.RefreshProtectionCooldownTimer();
				}
				break;
			}
		}

		public override void RefreshView()
		{
			if (base.IsLoaded() && this.Visible && this.requiresRefresh)
			{
				this.SetTab(this.curTab);
				this.requiresRefresh = false;
			}
			base.RefreshView();
		}

		public void RegisterForCrateFlyoutReOpen(string crateUId)
		{
			if (this.curTab == StoreTab.Treasure)
			{
				this.crateToReOpenInFlyout = crateUId;
				Service.EventManager.RegisterObserver(this, EventId.InventoryCrateCollectionClosed);
				Service.EventManager.RegisterObserver(this, EventId.EquipmentUnlocked);
				Service.EventManager.RegisterObserver(this, EventId.ShardUnitUpgraded);
			}
		}

		private void UnregisterForCrateFlyoutReOpen()
		{
			this.crateToReOpenInFlyout = null;
			Service.EventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
			Service.EventManager.UnregisterObserver(this, EventId.EquipmentUnlocked);
			Service.EventManager.UnregisterObserver(this, EventId.ShardUnitUpgraded);
		}

		private void OnIAPItemButtonClicked(UXButton button)
		{
			if (GameConstants.IAP_DISABLED_ANDROID)
			{
				AlertScreen.ShowModal(false, this.lang.Get("IAP_DISABLED_ANDROID_TITLE", new object[0]), this.lang.Get("IAP_DISABLED_ANDROID_DESCRIPTION", new object[0]), null, null, null, true, false);
				return;
			}
			StoreItemTag storeItemTag = button.Tag as StoreItemTag;
			Service.EventManager.SendEvent(EventId.InAppPurchaseSelect, storeItemTag.IAPType.ProductId);
			Service.InAppPurchaseController.PurchaseProduct(storeItemTag.IAPType.ProductId);
		}

		private void OnSoftCurrencyItemButtonClicked(UXButton button)
		{
			StoreItemTag storeItemTag = button.Tag as StoreItemTag;
			Service.EventManager.SendEvent(EventId.SoftCurrencyPurchaseSelect, storeItemTag.Uid);
			if (storeItemTag.ReqMet)
			{
				string message = this.lang.Get("PURCHASE_SOFT_CURRENCY", new object[]
				{
					this.lang.ThousandsSeparated(storeItemTag.Amount),
					this.lang.Get(storeItemTag.Currency.ToString().ToUpper(), new object[0]),
					storeItemTag.Price
				});
				AlertScreen.ShowModalWithImage(false, null, message, storeItemTag.IconName, new OnScreenModalResult(this.OnPurchaseSoftCurrency), storeItemTag);
			}
			else
			{
				GameUtils.ShowNotEnoughStorageMessage(storeItemTag.Currency);
			}
		}

		private void OnProtectionItemButtonClicked(UXButton button)
		{
			StoreItemTag storeItemTag = button.Tag as StoreItemTag;
			if (!storeItemTag.ReqMet)
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(this.lang.Get("PROTECTION_COOLDOWN", new object[0]));
				return;
			}
			string message = this.lang.Get("PURCHASE_PROTECTION", new object[]
			{
				this.lang.Get(string.Format("PACK_PROTECTION{0}", storeItemTag.Amount), new object[0]),
				storeItemTag.Price
			});
			AlertScreen.ShowModalWithImage(false, null, message, storeItemTag.IconName, new OnScreenModalResult(this.OnPurchaseProtection), storeItemTag);
		}

		private void OnLEItemButtonClicked(UXButton button)
		{
			StoreItemTag storeItemTag = button.Tag as StoreItemTag;
			LimitedEditionItemVO limitedEditionItemVO = Service.StaticDataController.Get<LimitedEditionItemVO>(storeItemTag.Uid);
			CrateVO vo = Service.StaticDataController.Get<CrateVO>(limitedEditionItemVO.CrateId);
			this.EnterCratePurchaseFlow(vo);
		}

		private void OnSupplyCrateItemButtonClicked(UXButton button)
		{
			StoreItemTag storeItemTag = button.Tag as StoreItemTag;
			CrateVO crateVO = Service.StaticDataController.Get<CrateVO>(storeItemTag.Uid);
			if (CrateUtils.IsPurchasableInStore(crateVO))
			{
				this.EnterCratePurchaseFlow(crateVO);
			}
			else
			{
				AlertScreen.ShowModal(false, null, this.lang.Get("ALL_CRATES_ALREADY_PURCHASED", new object[0]), null, null);
			}
		}

		private void OpenCrateModalFlyoutForStore(string crateId, string planetID)
		{
			CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForStore(crateId, planetID);
			crateInfoModalScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(crateInfoModalScreen, true, false);
		}

		private void EnterCratePurchaseFlow(CrateVO vo)
		{
			string uid = vo.Uid;
			string planetId = Service.CurrentPlayer.PlanetId;
			this.OpenCrateModalFlyoutForStore(uid, planetId);
			Service.EventManager.SendEvent(EventId.CrateStoreOpen, vo.Uid);
		}

		private void OnPurchaseSoftCurrency(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			StoreItemTag storeItemTag = cookie as StoreItemTag;
			this.changingInventory = true;
			try
			{
				GameUtils.BuySoftCurrencyWithCrystals(storeItemTag.Currency, storeItemTag.Amount, storeItemTag.Price, "soft_currency_pack|" + storeItemTag.Uid, false);
			}
			finally
			{
				this.changingInventory = false;
				this.RefreshAfterCurrencyChange();
			}
		}

		private void OnPurchaseProtection(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			StoreItemTag storeItemTag = cookie as StoreItemTag;
			this.changingInventory = true;
			try
			{
				GameUtils.BuyProtectionPackWithCrystals(storeItemTag.Amount);
			}
			finally
			{
				this.changingInventory = false;
				this.RefreshAfterCurrencyChange();
			}
		}

		private bool IsProtectionPackAvailable(int packNumber)
		{
			int protectionPurchaseCooldown = (int)Service.CurrentPlayer.GetProtectionPurchaseCooldown(packNumber);
			if (protectionPurchaseCooldown < 1)
			{
				return true;
			}
			int time = (int)ServerTime.Time;
			return time >= protectionPurchaseCooldown;
		}

		private int GetProtectionCooldownRemainingInSeconds(int packNumber)
		{
			int num = (int)(GameUtils.GetNowJavaEpochTime() / 1000L);
			int protectionPurchaseCooldown = (int)Service.CurrentPlayer.GetProtectionPurchaseCooldown(packNumber);
			if (protectionPurchaseCooldown < 1)
			{
				return 0;
			}
			return protectionPurchaseCooldown - num;
		}

		private int GetProtectionPackDuration(int packNumber)
		{
			int[] array;
			int[] array2;
			GameUtils.GetProtectionPacks(out array, out array2);
			return array[packNumber - 1];
		}

		private void RefreshAfterCurrencyChange()
		{
			if (!Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				this.SetTab(this.curTab);
				Service.EventManager.SendEvent(EventId.ScreenRefreshed, this);
			}
		}

		private void OnStructuresTabChanged()
		{
			this.itemGridFiltered.Clear();
			Service.Engine.ForceGarbageCollection(null);
			this.InitGrids(this.itemGridFiltered);
			List<UXElement> list = new List<UXElement>();
			StructuresTab currentTab = (StructuresTab)this.tabHelper.CurrentTab;
			switch (currentTab)
			{
			case StructuresTab.All:
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Army, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), this.itemGridFiltered);
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Resources, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), this.itemGridFiltered);
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Defenses, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), this.itemGridFiltered);
				break;
			case StructuresTab.Army:
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Army, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), this.itemGridFiltered);
				break;
			case StructuresTab.Defenses:
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Defenses, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), this.itemGridFiltered);
				break;
			case StructuresTab.Resources:
				StoreScreen.AddOrCountBuildingItems(list, StoreTab.Resources, new StoreScreen.AddBuildingItemDelegate(this.AddBuildingItem), this.itemGridFiltered);
				break;
			default:
				Service.Logger.WarnFormat("Unknown Structures tab {0}", new object[]
				{
					currentTab
				});
				break;
			}
			UXUtils.SortListForTwoRowGrids(list, this.itemGridFiltered);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				this.itemGridFiltered.AddItem(list[i], i);
				i++;
			}
			this.itemGridFiltered.RepositionItems();
		}

		private CountdownControl CreateShardShopCountdown(ShardShopData ssd)
		{
			CountdownControl countdownControl = new CountdownControl(this.expirationLabel, this.lang.Get("shard_shop_offer_expires", new object[0]), (int)ssd.Expiration);
			countdownControl.SetOffsetMinutes((int)ssd.OffsetMinutes);
			return countdownControl;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ShardOfferChanged)
			{
				if (id != EventId.ShardViewClosed)
				{
					if (id != EventId.ShardUnitUpgraded)
					{
						if (id != EventId.DeployableUnlockCelebrationPlayed)
						{
							if (id == EventId.InventoryResourceUpdated)
							{
								if (!this.changingInventory)
								{
									this.RefreshAfterCurrencyChange();
								}
								goto IL_1DC;
							}
							if (id == EventId.ButtonHighlightActivated)
							{
								base.GetOptionalElement<UXElement>("ContainerJewel" + StoreTab.Fragments.ToString()).Visible = false;
								base.GetOptionalElement<UXElement>("ContainerJewel" + StoreTab.Treasure.ToString()).Visible = false;
								base.GetOptionalElement<UXElement>("ContainerJewel" + StoreTab.Structures.ToString()).Visible = false;
								base.GetOptionalElement<UXElement>("ContainerJewel" + StoreTab.Turrets.ToString()).Visible = false;
								goto IL_1DC;
							}
							if (id == EventId.PlayerFactionChanged)
							{
								this.SetTab(this.curTab);
								goto IL_1DC;
							}
							if (id == EventId.EquipmentUnlocked)
							{
								if (Service.ArmoryController.AllowUnlockCelebration)
								{
									this.UnregisterForCrateFlyoutReOpen();
								}
								goto IL_1DC;
							}
							if (id != EventId.EquipmentUnlockCelebrationPlayed)
							{
								if (id != EventId.InventoryCrateCollectionClosed)
								{
									goto IL_1DC;
								}
								string planetId = Service.CurrentPlayer.PlanetId;
								this.OpenCrateModalFlyoutForStore(this.crateToReOpenInFlyout, planetId);
								this.UnregisterForCrateFlyoutReOpen();
								goto IL_1DC;
							}
						}
						if (!this.ssView.IsModalVisible())
						{
							this.Close(null);
						}
					}
					else
					{
						IDeployableVO deployableVO = (IDeployableVO)cookie;
						if (deployableVO != null && deployableVO.Lvl > 1)
						{
							this.UnregisterForCrateFlyoutReOpen();
						}
					}
				}
				else if (this.curTab == StoreTab.Fragments)
				{
					this.SetTab(this.curTab);
				}
			}
			else if (this.curTab == StoreTab.Fragments)
			{
				this.SetTab(this.curTab);
			}
			IL_1DC:
			return base.OnEvent(id, cookie);
		}
	}
}
