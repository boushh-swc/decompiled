using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Contracts;
using StaRTS.Main.Models.Commands.Player.Building.Upgrade;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
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
	public class BuildingInfoScreen : SelectedBuildingScreen, IViewClockTimeObserver, IEventObserver
	{
		protected const string GROUP_BUILDING_INFO = "BuildingInfo";

		protected const string GROUP_INFO = "Info";

		protected const string GROUP_NO_SQUAD = "NoSquad";

		protected const string GROUP_STORAGE = "BuildingInfoStorage";

		protected const string GROUP_SWAP = "SwapGroup";

		protected const string GROUP_TURRET = "InfoTurret";

		protected const string GROUP_UNLOCK_ITEMS = "UnlockItems";

		protected const string GROUP_UPGRADE_TIME = "UpgradeTime";

		protected const string GROUP_UPGRADE_TIME_STORAGE = "UpgradeTimeStorage";

		protected const string GROUP_NAVIGATION_CENTER = "NavigationCenter";

		protected const string GROUP_LOW_LAYOUT_GROUP = "LowLayoutGroup";

		protected const string GROUP_SELECT_PLANET = "SelectPlanet";

		protected const string LABEL_BUTTON_COST = "CostLabel";

		protected const string LABEL_HQ_UPGRADE_DESC = "LabelUpgradeHQStorage";

		protected const string LABEL_INFO = "LabelInfo";

		protected const string LABEL_ITEM_INFO = "LabelItemInfo";

		protected const string LABEL_ITEM_INFO_STORAGE = "LabelItemInfoStorage";

		protected const string LABEL_STORAGE = "LabelStorage";

		protected const string LABEL_TITLE = "DialogBldgUpgradeTitle";

		protected const string LABEL_TITLE_PERKED = "DialogBldgUpgradeTitlePerks";

		protected const string LABEL_UNLOCK = "LabelUnlock";

		protected const string LABEL_UNLOCK_DESC = "LabelUnlockDescription";

		protected const string LABEL_UNLOCK_DESC_STORAGE = "LabelUnlockDescriptionStorage";

		protected const string LABEL_UPGRADE = "LabelUpgrade";

		protected const string LABEL_UPGRADE_STORAGE = "LabelUpgradeStorage";

		protected const string LABEL_UPGRADE_TIME = "LabelUpgradeTime";

		protected const string LABEL_UPGRADE_TIME_STORAGE = "LabelUpgradeTimeStorage";

		protected const string LABEL_PRIMARY_ACTION = "LabelButtonPrimaryInfo";

		protected const string LABEL_SECONDARY_ACTION = "LabelButtonSecondaryInfo";

		protected const string LABEL_INSTANT_BUY = "CostInstantBuyValue";

		protected const string LABEL_INSTANT_COST = "CostInstantBuyLabel";

		protected const string LABEL_UPGRADE_UNLOCK_PLANET = "LabelUpgradeUnlockPlanet";

		protected const string LABEL_SELECT_PLANET = "LabelSelectPlanet";

		protected const string LABEL_SELECT_PLANET_NAME = "LabelSelectPlanetName";

		protected const string LABEL_TOP_UNLOCK_ITEMS_NAME = "LabelTopUnlockItemsName";

		protected const string LABEL_TOP_UNLOCK_ITEMS_LOCK = "LabelTopUnlockItemsLock";

		protected const string LABEL_TOP_UNLOCK_ITEMS_CURRENT = "LabelTopUnlockItemsCurrent";

		protected const string LABEL_INFO_BOTTOM = "LabelInfoBottom";

		protected const string LABEL_VIEW_GALAXY_MAP = "LabelViewGalaxyMap";

		protected const string BUTTON_IMAGE = "BuildingImage";

		protected const string BUTTON_IMAGE_STORAGE = "BuildingImageStorage";

		protected const string BUTTON_INFO = "BtnItemInfo";

		protected const string BUTTON_INFO_STORAGE = "BtnItemInfoStorage";

		protected const string BUTTON_PRIMARY_ACTION = "ButtonPrimary";

		protected const string BUTTON_REQUEST_TROOPS = "BtnRequestTroops";

		protected const string BUTTON_STORAGE_ITEM_CARD = "StorageItemsCard";

		public const string BUTTON_SWAP = "BtnSwap";

		protected const string BUTTON_INSTANT_BUY = "BtnInstantBuy";

		protected const string BUTTON_UPGRADE_ALL_WALLS = "ButtonSecondary";

		protected const string BUTTON_TROOP_CARD_ITEM = "BuildingUnlockCard";

		protected const string BUTTON_SELECT_PLANET_CARD = "SelectPlanetCard";

		protected const string BUTTON_TUTORIAL_CONFIRM = "ButtonTutorialConfirm";

		protected const string BUTTON_VIEW_GALAXY_MAP = "BtnViewGalaxyMap";

		protected const string BUTTON_UPGRADE = "BtnTurretUpgrade";

		protected const string IMAGE_FRAME = "BldgImageFrame";

		protected const string IMAGE_FRAME_STORAGE = "BldgImageFrameStorage";

		protected const string ICON_SQUAD = "SpriteSquadSymbol";

		protected const string ICON_INSTANT_BUY_CRYSTAL = "CostInstantBuyIconCrystal";

		protected const string ICON_SELECT_PLANET_IMAGE = "SpriteSelectPlanetImagePlanet";

		protected const string SPRITE_BLDG_IMAGE_BOTTOM_FRAME = "BldgImageBottomFrame";

		protected const string TEXTURE_TOP_UNLOCK_ITEMS_IMAGE = "TextureTopUnlockItemsImage";

		protected const string SPRITE_TOP_UNLOCK_ITEMS_LOCK = "SpriteTopUnlockItemsLock";

		protected const string GRID_STORAGE = "StorageItemsGrid";

		protected const string GRID_ITEM_COUNT = "LabelStorageCount";

		protected const string GRID_ITEM_ICON = "SpriteStorageItemImage";

		protected const string GRID_ITEM_LEVEL = "LabelTroopLevel";

		protected const string GRID_ITEM_TEMPLATE = "StorageItemsTemplate";

		protected const string GRID_ITEM_TROOP_ICON = "SpriteStorageItemImageTroops";

		protected const string GRID_UNLOCK = "BuildingUnlockGrid";

		protected const string UNLOCK_ITEM_LABEL_AMOUNT = "LabelUnlockCount";

		protected const string UNLOCK_ITEM_SPRITE = "SpriteItemImage";

		protected const string UNLOCK_ITEM_TEMPLATE = "BuildingUnlockTemplate";

		protected const string UNLOCK_TROOP_SPRITE = "SpriteItemImageTroops";

		protected const string SELECT_PLANET_GRID = "SelectPlanetGrid";

		protected const string SELECT_PLANET_TEMPLATE_ROOT = "SelectPlanetTemplate";

		protected const string SELECT_PLANET_TEMPLATE_ITEM = "SelectPlanetCard";

		protected const string TOP_UNLOCK_ITEMS_GRID = "TopUnlockItemsGrid";

		protected const string TOP_UNLOCK_ITEMS_TEMPLATE = "TopUnlockItemsTemplate";

		protected const string CONTEXT_COST_GROUP = "Cost";

		protected const string PLUS = "PLUS";

		protected const int SLIDER_MAX = 6;

		protected const string BOTTOM_BAR_ONE_POSTFIX = "1Bottom";

		protected const string BOTTOM_BAR_TWO_POSTFIX = "2Bottom";

		protected const string BOTTOM_BAR_THREE_POSTFIX = "3Bottom";

		protected const int INFO_SLIDER_HITPOINTS = 0;

		protected const int INFO_SLIDER_COUNT = 1;

		protected const int BOTTOM_BAR_1_INDEX = 3;

		protected const int BOTTOM_BAR_2_INDEX = 4;

		protected const int BOTTOM_BAR_3_INDEX = 5;

		protected SliderControl[] sliders;

		protected UXButton buttonImage;

		protected UXButton buttonImageStorage;

		protected UXButton buttonInfo;

		protected UXButton buttonInfoStorage;

		protected UXButton buttonPrimaryAction;

		protected UXButton buttonSwap;

		protected UXButton buttonInstantBuy;

		protected UXButton buttonUpgradeAllWalls;

		protected UXButton buttonViewGalaxyMap;

		protected UXButton upgradeButton;

		protected UXLabel labelHQUpgradeDesc;

		protected UXLabel labelUpgradeUnlockPlanet;

		protected UXLabel labelInfo;

		protected UXLabel labelItemInfo;

		protected UXLabel labelItemInfoStorage;

		protected UXLabel labelStorage;

		protected UXLabel labelUnlock;

		protected UXLabel labelUnlockDesc;

		protected UXLabel labelUnlockDescStorage;

		protected UXLabel labelUpgrade;

		protected UXLabel labelUpgradeTime;

		protected UXLabel labelUpgradeTimeStorage;

		protected UXLabel labelPrimaryAction;

		protected UXLabel labelSecondaryAction;

		protected UXLabel labelViewGalaxyMap;

		protected UXGrid storageItemGrid;

		protected UXGrid selectPlanetGrid;

		private int hitpointSliderIndex;

		protected bool observingClockViewTime;

		protected bool useUpgradeGroup;

		protected bool useStorageGroup;

		protected bool useTurretGroup;

		protected BuildingTypeVO nextBuildingInfo;

		protected BuildingTypeVO maxBuildingInfo;

		protected BuildingTypeVO reqBuildingInfo;

		protected bool reqMet;

		protected string instantUpgradeBuildingKey;

		protected string instantUpgradeBuildingUid;

		protected GeometryProjector projector;

		private TroopTooltipHelper troopTooltipHelper;

		private List<TroopUpgradeTag> troopList;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public override bool ShowCurrencyTray
		{
			get
			{
				return this.useUpgradeGroup;
			}
		}

		public BuildingInfoScreen(SmartEntity selectedBuilding) : this(selectedBuilding, false)
		{
		}

		public BuildingInfoScreen(SmartEntity selectedBuilding, bool useUpgradeGroup) : base("gui_building", selectedBuilding)
		{
			this.useUpgradeGroup = useUpgradeGroup;
			this.projector = null;
			this.troopTooltipHelper = new TroopTooltipHelper();
		}

		protected override void SetSelectedBuilding(SmartEntity newSelectedBuilding)
		{
			base.SetSelectedBuilding(newSelectedBuilding);
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			this.nextBuildingInfo = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo);
			this.maxBuildingInfo = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup);
			this.reqMet = Service.UnlockController.IsUnlocked(this.nextBuildingInfo, 1, out this.reqBuildingInfo);
		}

		protected override void OnScreenLoaded()
		{
			if (this.selectedBuilding == null)
			{
				base.DestroyScreen();
				return;
			}
			this.InitGroups();
			this.InitLabels();
			this.InitButtons();
			this.InitImages();
			this.OnLoaded();
		}

		protected virtual void InitGroups()
		{
			if (this.useStorageGroup)
			{
				base.GetElement<UXElement>("BuildingInfoStorage").Visible = true;
				base.GetElement<UXElement>("BuildingInfo").Visible = false;
				base.GetElement<UXElement>("NoSquad").Visible = false;
				if (!this.useUpgradeGroup)
				{
					base.GetElement<UXElement>("UpgradeTimeStorage").Visible = false;
				}
			}
			else
			{
				base.GetElement<UXElement>("BuildingInfo").Visible = true;
				base.GetElement<UXElement>("BuildingInfoStorage").Visible = false;
				base.GetElement<UXElement>("SwapGroup").Visible = false;
				base.GetElement<UXElement>("InfoTurret").Visible = this.useTurretGroup;
				base.GetElement<UXElement>("UnlockItems").Visible = false;
				if (this.useUpgradeGroup)
				{
					base.GetElement<UXElement>("Info").Visible = !this.useTurretGroup;
				}
				else
				{
					base.GetElement<UXElement>("UpgradeTime").Visible = false;
				}
			}
			base.GetElement<UXElement>("NavigationCenter").Visible = false;
		}

		protected virtual void InitLabels()
		{
			UXLabel element = base.GetElement<UXLabel>("DialogBldgUpgradeTitle");
			UXLabel element2 = base.GetElement<UXLabel>("DialogBldgUpgradeTitlePerks");
			if (this.useUpgradeGroup && this.nextBuildingInfo != null)
			{
				element.Text = this.lang.Get("BUILDING_UPGRADE", new object[]
				{
					LangUtils.GetBuildingDisplayName(this.nextBuildingInfo),
					this.nextBuildingInfo.Lvl
				});
			}
			else
			{
				element.Text = this.lang.Get("BUILDING_INFO", new object[]
				{
					LangUtils.GetBuildingDisplayName(this.buildingInfo),
					this.buildingInfo.Lvl
				});
			}
			element2.Text = element.Text;
			string buildingDescription = LangUtils.GetBuildingDescription(this.buildingInfo);
			if (this.useStorageGroup)
			{
				this.labelStorage = base.GetElement<UXLabel>("LabelStorage");
				this.labelItemInfoStorage = base.GetElement<UXLabel>("LabelItemInfoStorage");
				this.labelItemInfoStorage.Text = buildingDescription;
				this.labelItemInfoStorage.Visible = false;
				this.labelUnlockDescStorage = base.GetElement<UXLabel>("LabelUnlockDescriptionStorage");
				this.labelUnlockDescStorage.Visible = false;
			}
			else
			{
				this.labelItemInfo = base.GetElement<UXLabel>("LabelItemInfo");
				this.labelItemInfo.Text = buildingDescription;
				this.labelItemInfo.Visible = false;
				this.labelUnlockDesc = base.GetElement<UXLabel>("LabelUnlockDescription");
				this.labelUnlockDesc.Visible = false;
			}
			this.labelInfo = base.GetElement<UXLabel>("LabelInfo");
			if (this.useUpgradeGroup)
			{
				this.labelInfo.Visible = false;
			}
			else
			{
				this.labelInfo.Text = buildingDescription;
			}
			this.labelHQUpgradeDesc = base.GetElement<UXLabel>("LabelUpgradeHQStorage");
			if (!this.useUpgradeGroup || this.reqMet || this.reqBuildingInfo == null)
			{
				this.labelHQUpgradeDesc.Visible = false;
			}
			else
			{
				this.labelHQUpgradeDesc.Visible = true;
				this.labelHQUpgradeDesc.Text = this.lang.Get("BUILDING_REQUIREMENT", new object[]
				{
					this.reqBuildingInfo.Lvl,
					LangUtils.GetBuildingDisplayName(this.reqBuildingInfo)
				});
			}
			this.buttonPrimaryAction = base.GetElement<UXButton>("ButtonPrimary");
			if (this.useUpgradeGroup && this.nextBuildingInfo != null)
			{
				UXLabel uXLabel = (!this.useStorageGroup) ? base.GetElement<UXLabel>("LabelUpgrade") : base.GetElement<UXLabel>("LabelUpgradeStorage");
				uXLabel.Text = this.lang.Get("s_upgradeTime", new object[0]);
				UXLabel uXLabel2 = (!this.useStorageGroup) ? base.GetElement<UXLabel>("LabelUpgradeTime") : base.GetElement<UXLabel>("LabelUpgradeTimeStorage");
				uXLabel2.Text = GameUtils.GetTimeLabelFromSeconds(this.nextBuildingInfo.UpgradeTime);
				this.buttonPrimaryAction.Visible = true;
				UXUtils.SetupCostElements(this, "Cost", null, this.nextBuildingInfo.UpgradeCredits, this.nextBuildingInfo.UpgradeMaterials, this.nextBuildingInfo.UpgradeContraband, 0, !this.reqMet, null);
			}
			if (this.useUpgradeGroup)
			{
				this.buttonPrimaryAction.Visible = true;
				this.buttonPrimaryAction.Enabled = this.reqMet;
				this.buttonPrimaryAction.OnClicked = new UXButtonClickedDelegate(this.OnUpgradeButtonClicked);
			}
			else
			{
				this.buttonPrimaryAction.Visible = false;
			}
			this.labelPrimaryAction = base.GetElement<UXLabel>("LabelButtonPrimaryInfo");
			this.labelSecondaryAction = base.GetElement<UXLabel>("LabelButtonSecondaryInfo");
			this.labelViewGalaxyMap = base.GetElement<UXLabel>("LabelViewGalaxyMap");
			this.labelPrimaryAction.Visible = false;
			this.labelSecondaryAction.Visible = false;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			if (this.useStorageGroup)
			{
				this.buttonImageStorage = base.GetElement<UXButton>("BuildingImageStorage");
				this.buttonInfoStorage = base.GetElement<UXButton>("BtnItemInfoStorage");
				if (this.useUpgradeGroup)
				{
					this.buttonInfoStorage.OnClicked = new UXButtonClickedDelegate(this.OnInfoStorageClicked);
					this.buttonImageStorage.OnClicked = new UXButtonClickedDelegate(this.OnInfoStorageClicked);
				}
				else
				{
					this.buttonInfoStorage.Visible = false;
				}
			}
			else
			{
				this.buttonImage = base.GetElement<UXButton>("BuildingImage");
				this.buttonInfo = base.GetElement<UXButton>("BtnItemInfo");
				if (this.useUpgradeGroup)
				{
					this.buttonInfo.OnClicked = new UXButtonClickedDelegate(this.OnInfoClicked);
					this.buttonImage.OnClicked = new UXButtonClickedDelegate(this.OnInfoClicked);
				}
				else
				{
					this.buttonInfo.Visible = false;
				}
			}
			this.buttonUpgradeAllWalls = base.GetElement<UXButton>("ButtonSecondary");
			this.buttonSwap = base.GetElement<UXButton>("BtnSwap");
			this.buttonInstantBuy = base.GetElement<UXButton>("BtnInstantBuy");
			this.buttonViewGalaxyMap = base.GetElement<UXButton>("BtnViewGalaxyMap");
			this.buttonSwap.Visible = false;
			this.buttonUpgradeAllWalls.Visible = false;
			this.buttonInstantBuy.Visible = (this.useUpgradeGroup && this.nextBuildingInfo != null);
			this.upgradeButton = base.GetElement<UXButton>("BtnTurretUpgrade");
			if (!GameConstants.ENABLE_INSTANT_BUY)
			{
				this.buttonInstantBuy.Visible = GameConstants.ENABLE_INSTANT_BUY;
			}
			if (this.useUpgradeGroup && this.nextBuildingInfo != null)
			{
				int num = GameUtils.CrystalCostToInstantUpgrade(this.nextBuildingInfo);
				UXLabel element = base.GetElement<UXLabel>("CostInstantBuyLabel");
				UXLabel element2 = base.GetElement<UXLabel>("CostInstantBuyValue");
				element.Text = this.lang.ThousandsSeparated(num);
				element2.Text = this.lang.Get("BUILDING_UPGRADE_INSTANT", new object[0]);
				element.TextColor = UXUtils.GetCostColor(element, GameUtils.CanAffordCrystals(num), !this.reqMet);
				element2.TextColor = UXUtils.GetCostColor(element2, true, !this.reqMet);
				this.buttonInstantBuy.OnClicked = new UXButtonClickedDelegate(this.OnInstantUpgradeButtonClicked);
				this.buttonInstantBuy.Enabled = (this.reqMet && GameConstants.ENABLE_INSTANT_BUY);
				UXSprite element3 = base.GetElement<UXSprite>("CostInstantBuyIconCrystal");
				if (!this.reqMet)
				{
					element3.Color = UXUtils.COLOR_COST_LOCKED;
				}
				else
				{
					element3.Color = Color.white;
				}
			}
		}

		protected virtual IGeometryVO GetImageGeometryConfig()
		{
			IGeometryVO buildingInfo;
			if (this.useUpgradeGroup && this.nextBuildingInfo != null)
			{
				buildingInfo = this.nextBuildingInfo;
			}
			else
			{
				buildingInfo = this.buildingInfo;
			}
			return buildingInfo;
		}

		protected string GetAssetNameFromGeometryConfig(IGeometryVO config)
		{
			return (!(config is IAssetVO)) ? null : ((IAssetVO)config).AssetName;
		}

		protected virtual void InitImages()
		{
			IGeometryVO imageGeometryConfig = this.GetImageGeometryConfig();
			UXSprite element = base.GetElement<UXSprite>((!this.useStorageGroup) ? "BldgImageFrame" : "BldgImageFrameStorage");
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateBuildingConfig(imageGeometryConfig as BuildingTypeVO, element);
			projectorConfig.AnimPreference = AnimationPreference.AnimationAlways;
			this.projector = ProjectorUtils.GenerateProjector(projectorConfig);
			base.GetElement<UXSprite>("SpriteSquadSymbol").Visible = false;
		}

		protected virtual void OnLoaded()
		{
			this.InitControls(1);
			this.InitHitpoints(0);
		}

		protected void InitControls(int slidersUsed)
		{
			this.sliders = new SliderControl[6];
			for (int i = 0; i < 6; i++)
			{
				if (3 > i || i > 5 || (!this.useTurretGroup && !this.useStorageGroup))
				{
					string num;
					switch (i)
					{
					case 3:
						num = "1Bottom";
						break;
					case 4:
						num = "2Bottom";
						break;
					case 5:
						num = "3Bottom";
						break;
					default:
						num = (i + 1).ToString();
						break;
					}
					SliderControl sliderControl = new SliderControl(this, num, this.useStorageGroup, this.useTurretGroup, this.useUpgradeGroup);
					if (i >= slidersUsed)
					{
						sliderControl.HideAll();
					}
					this.sliders[i] = sliderControl;
				}
			}
		}

		protected virtual void InitHitpoints(int sliderIndex)
		{
			this.hitpointSliderIndex = sliderIndex;
			this.sliders[sliderIndex].CurrentLabel.Visible = true;
			this.sliders[sliderIndex].CurrentSlider.Visible = true;
			this.sliders[sliderIndex].DescLabel.Visible = true;
			this.sliders[sliderIndex].Background.Visible = true;
			string text;
			if (this.useUpgradeGroup && this.nextBuildingInfo != null)
			{
				text = this.lang.Get("BUILDING_HITPOINTS", new object[0]);
				BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
				int health = this.buildingInfo.Health;
				int health2 = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo).Health;
				int health3 = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup).Health;
				this.sliders[sliderIndex].CurrentLabel.Text = this.lang.ThousandsSeparated(health);
				this.sliders[sliderIndex].NextLabel.Text = this.lang.Get("PLUS", new object[]
				{
					this.lang.ThousandsSeparated(health2 - health)
				});
				this.sliders[sliderIndex].CurrentSlider.Value = ((health3 != 0) ? ((float)health / (float)health3) : 0f);
				this.sliders[sliderIndex].NextSlider.Value = ((health3 != 0) ? ((float)health2 / (float)health3) : 0f);
			}
			else
			{
				text = this.lang.Get("BUILDING_HITPOINTS", new object[0]);
				this.UpdateHitpoints();
			}
			this.sliders[this.hitpointSliderIndex].DescLabel.Text = text;
			if (!this.observingClockViewTime && Service.PostBattleRepairController.IsEntityInRepair(this.selectedBuilding))
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				this.observingClockViewTime = true;
			}
		}

		protected virtual void UpdateHitpoints()
		{
			if (this.useUpgradeGroup)
			{
				return;
			}
			int health = this.buildingInfo.Health;
			int num = health;
			HealthViewComponent healthViewComp = this.selectedBuilding.HealthViewComp;
			if (healthViewComp != null && healthViewComp.IsInitialized)
			{
				num = healthViewComp.HealthAmount;
			}
			UXLabel currentLabel = this.sliders[this.hitpointSliderIndex].CurrentLabel;
			currentLabel.Text = this.lang.Get("FRACTION", new object[]
			{
				this.lang.ThousandsSeparated(num),
				this.lang.ThousandsSeparated(health)
			});
			UXSlider currentSlider = this.sliders[this.hitpointSliderIndex].CurrentSlider;
			currentSlider.Value = ((health != 0) ? ((float)num / (float)health) : 0f);
		}

		public void InitStorage(int sliderIndex, string description)
		{
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			int storage = this.buildingInfo.Storage;
			BuildingTypeVO nextLevel = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo);
			int num = (nextLevel != null) ? nextLevel.Storage : storage;
			int storage2 = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup).Storage;
			SliderControl sliderControl = this.sliders[sliderIndex];
			sliderControl.DescLabel.Text = this.lang.Get(description, new object[0]);
			sliderControl.CurrentLabel.Text = this.lang.ThousandsSeparated(storage);
			sliderControl.CurrentSlider.Value = ((storage2 != 0) ? ((float)storage / (float)storage2) : 0f);
			sliderControl.NextLabel.Text = this.lang.Get("PLUS", new object[]
			{
				this.lang.ThousandsSeparated(num - storage)
			});
			sliderControl.NextSlider.Value = ((storage2 != 0) ? ((float)num / (float)storage2) : 0f);
		}

		public virtual void OnViewClockTime(float dt)
		{
			if (Service.PostBattleRepairController.IsEntityInRepair(this.selectedBuilding))
			{
				this.UpdateHitpoints();
			}
			else
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				this.observingClockViewTime = false;
			}
		}

		public override void OnDestroyElement()
		{
			if (this.projector != null)
			{
				this.projector.Destroy();
				this.projector = null;
			}
			if (this.observingClockViewTime)
			{
				this.observingClockViewTime = false;
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			}
			if (this.sliders != null)
			{
				for (int i = 0; i < 6; i++)
				{
					this.sliders[i] = null;
				}
			}
			if (this.storageItemGrid != null)
			{
				this.storageItemGrid.Clear();
				this.storageItemGrid = null;
			}
			this.troopTooltipHelper.Destroy();
			this.troopTooltipHelper = null;
			this.troopList = null;
			base.OnDestroyElement();
		}

		private void OnInfoClicked(UXButton button)
		{
			this.labelItemInfo.Visible = !this.labelItemInfo.Visible;
			this.buttonInfo.Visible = !this.labelItemInfo.Visible;
			UXSprite element = base.GetElement<UXSprite>((!this.useStorageGroup) ? "BldgImageFrame" : "BldgImageFrameStorage");
			element.Visible = !this.labelItemInfo.Visible;
		}

		private void OnInfoStorageClicked(UXButton button)
		{
			this.labelItemInfoStorage.Visible = !this.labelItemInfoStorage.Visible;
			this.buttonInfoStorage.Visible = !this.labelItemInfoStorage.Visible;
			UXSprite element = base.GetElement<UXSprite>("BldgImageFrameStorage");
			element.Visible = !this.labelItemInfoStorage.Visible;
		}

		protected void InitGrid()
		{
			this.storageItemGrid = base.GetElement<UXGrid>("StorageItemsGrid");
			this.storageItemGrid.SetTemplateItem("StorageItemsTemplate");
			this.storageItemGrid.Clear();
			this.troopList = new List<TroopUpgradeTag>();
		}

		protected void AddTroopItem(IUpgradeableVO troop, int troopCount, string tooltipText)
		{
			TroopUpgradeTag troopUpgradeTag = new TroopUpgradeTag(troop as IDeployableVO, true);
			this.troopList.Add(troopUpgradeTag);
			string uid = troop.Uid;
			UXElement item = this.storageItemGrid.CloneTemplateItem(uid);
			UXLabel subElement = this.storageItemGrid.GetSubElement<UXLabel>(uid, "LabelStorageCount");
			subElement.Text = LangUtils.GetMultiplierText(troopCount);
			UXSprite subElement2 = this.storageItemGrid.GetSubElement<UXSprite>(uid, "SpriteStorageItemImageTroops");
			ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(troop as IDeployableVO, subElement2);
			Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(troop, config, Service.CurrentPlayer.ActiveArmory));
			ProjectorUtils.GenerateProjector(config);
			UXLabel subElement3 = this.storageItemGrid.GetSubElement<UXLabel>(uid, "LabelTroopLevel");
			subElement3.Text = LangUtils.GetLevelText(troop.Lvl);
			UXElement subElement4 = this.storageItemGrid.GetSubElement<UXElement>(uid, "BtnRequestTroops");
			subElement4.Visible = false;
			UXButton subElement5 = this.storageItemGrid.GetSubElement<UXButton>(uid, "StorageItemsCard");
			if (tooltipText != null)
			{
				this.troopTooltipHelper.RegisterButtonTooltip(subElement5, tooltipText);
			}
			else
			{
				subElement5.Tag = troopUpgradeTag;
				subElement5.OnClicked = new UXButtonClickedDelegate(this.OnTroopItemClicked);
			}
			this.storageItemGrid.AddItem(item, troop.Order);
		}

		private void OnTroopItemClicked(UXButton button)
		{
			SmartEntity availableTroopResearchLab = Service.BuildingLookupController.GetAvailableTroopResearchLab();
			TroopUpgradeTag troopUpgradeTag = button.Tag as TroopUpgradeTag;
			bool showUpgradeControls = !string.IsNullOrEmpty(troopUpgradeTag.Troop.UpgradeShardUid);
			Service.ScreenController.AddScreen(new DeployableInfoScreen(troopUpgradeTag, this.troopList, showUpgradeControls, availableTroopResearchLab));
		}

		protected void RepositionGridItems()
		{
			this.storageItemGrid.RepositionItems();
		}

		protected virtual void OnPayMeForCurrencyResult(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie) && !PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
			{
				this.ConfirmUpgrade();
			}
		}

		protected virtual void OnPayMeForDroidResult(object result, object cookie)
		{
			if (result != null)
			{
				this.ConfirmUpgrade();
			}
		}

		protected virtual void ConfirmUpgrade()
		{
			Service.ISupportController.StartBuildingUpgrade(this.nextBuildingInfo, this.selectedBuilding, false);
			if (this.nextBuildingInfo.Time > 0)
			{
				Service.BuildingController.EnsureDeselectSelectedBuilding();
				Service.BuildingController.SelectedBuilding = this.selectedBuilding;
			}
			this.Close(this.selectedBuilding.ID);
		}

		protected virtual void OnUpgradeButtonClicked(UXButton button)
		{
			int upgradeCredits = this.nextBuildingInfo.UpgradeCredits;
			int upgradeMaterials = this.nextBuildingInfo.UpgradeMaterials;
			int upgradeContraband = this.nextBuildingInfo.UpgradeContraband;
			string buildingPurchaseContext = GameUtils.GetBuildingPurchaseContext(this.nextBuildingInfo, this.buildingInfo, true, false);
			if (PayMeScreen.ShowIfNotEnoughCurrency(upgradeCredits, upgradeMaterials, upgradeContraband, buildingPurchaseContext, new OnScreenModalResult(this.OnPayMeForCurrencyResult)))
			{
				return;
			}
			if (PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
			{
				return;
			}
			this.ConfirmUpgrade();
		}

		protected virtual void OnInstantUpgradeButtonClicked(UXButton button)
		{
			if (!this.HasEnoughResourceCapacityToUpgrade(this.nextBuildingInfo))
			{
				CurrencyType currencyType = GameUtils.GetCurrencyType(this.nextBuildingInfo.UpgradeCredits, this.nextBuildingInfo.UpgradeMaterials, this.nextBuildingInfo.UpgradeContraband);
				Service.ICurrencyController.HandleUnableToCollect(currencyType);
				return;
			}
			int num = GameUtils.CrystalCostToInstantUpgrade(this.nextBuildingInfo);
			if (num >= GameConstants.CRYSTAL_SPEND_WARNING_MINIMUM)
			{
				FinishNowScreen.ShowModalWithNoContract(this.selectedBuilding, new OnScreenModalResult(this.ConfirmInstantUpgrade), null, num);
				return;
			}
			this.ConfirmInstantUpgrade(true, null);
		}

		protected void ConfirmInstantUpgrade(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			int num = GameUtils.CrystalCostToInstantUpgrade(this.nextBuildingInfo);
			if (!GameUtils.SpendCrystals(num))
			{
				return;
			}
			this.buttonPrimaryAction.Enabled = false;
			this.buttonInstantBuy.Enabled = false;
			this.buttonSwap.Enabled = false;
			this.upgradeButton.Enabled = false;
			this.allowClose = false;
			ProcessingScreen.Show();
			int currencyAmount = -num;
			string itemType = this.buildingInfo.Type.ToString();
			string uid = this.buildingInfo.Uid;
			int itemCount = 1;
			string type = "instant_building";
			string subType = "consumable";
			Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, uid, itemCount, type, subType);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.MissionCollecting, EventPriority.Default);
			this.instantUpgradeBuildingKey = this.selectedBuilding.BuildingComp.BuildingTO.Key;
			this.instantUpgradeBuildingUid = this.nextBuildingInfo.Uid;
			try
			{
				ISupportController iSupportController = Service.ISupportController;
				iSupportController.StartBuildingUpgrade(this.nextBuildingInfo, this.selectedBuilding, true);
				iSupportController.BuyOutCurrentBuildingContract(this.selectedBuilding, false);
			}
			finally
			{
				eventManager.UnregisterObserver(this, EventId.MissionCollecting);
				this.HandleInstantUpgradeRequest();
			}
		}

		public override void Close(object modalResult)
		{
			ProcessingScreen.Hide();
			base.Close(modalResult);
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.MissionCollecting)
			{
				this.HandleInstantUpgradeRequest();
			}
			return base.OnEvent(id, cookie);
		}

		protected virtual void HandleInstantUpgradeRequest()
		{
			if (this.instantUpgradeBuildingKey != null && this.instantUpgradeBuildingUid != null)
			{
				BuildingInstantUpgradeRequest request = new BuildingInstantUpgradeRequest(this.instantUpgradeBuildingKey, this.instantUpgradeBuildingUid, string.Empty);
				BuildingInstantUpgradeCommand command = new BuildingInstantUpgradeCommand(request);
				Service.ServerAPI.Enqueue(command);
				this.instantUpgradeBuildingKey = null;
				this.instantUpgradeBuildingUid = null;
			}
		}

		protected bool HasEnoughResourceCapacityToUpgrade(BuildingTypeVO vo)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return vo.UpgradeCredits <= currentPlayer.MaxCreditsAmount && vo.UpgradeMaterials <= currentPlayer.MaxMaterialsAmount && vo.UpgradeContraband <= currentPlayer.MaxContrabandAmount;
		}
	}
}
