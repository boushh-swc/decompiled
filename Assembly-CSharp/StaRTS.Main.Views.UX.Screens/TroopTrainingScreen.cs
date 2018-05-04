using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TroopTrainingScreen : SelectedBuildingScreen, IViewFrameTimeObserver
	{
		private const string LABEL_TITLE = "DialogTrainingTitle";

		private const string LABEL_CAPACITY = "LabelCurrentlyTraining";

		private const string LABEL_BUILDING_CAPACITY = "LabelBarracksCapacity";

		private const string STATIC_TIME = "LabelTimeIntro";

		private const string LABEL_TIME = "LabelTimeCount";

		private const string STATIC_FINISH = "LabelFinishIntro";

		private const string BUTTON_FINISH = "BtnFinish";

		private const string COST_FINISH = "FinishCost";

		private const string LABEL_INSTRUCTIONS = "LabelInstructions";

		private const string LABEL_CLASS_HINT = "LabelUnitClassHint";

		private const string TRAINING_GROUP = "TroopTraining";

		private const string PREV_BUTTON = "BtnHeroPrev";

		private const string NEXT_BUTTON = "BtnHeroNext";

		private const string QUEUE_ITEM_GRID = "QueueGrid";

		private const string QUEUE_ITEM_TEMPLATE = "QueueItemTemplate";

		private const string QUEUE_ITEM_BUTTON = "BtnRemove";

		private const string QUEUE_ITEM_ICON = "SpriteQueueItemImage";

		private const string QUEUE_ITEM_LABEL_COUNT = "LabelQueueCount";

		private const string ACTIVE_ITEM = "ButtonActiveItemCard";

		private const string ACTIVE_ITEM_BUTTON = "BtnRemoveActive";

		private const string ACTIVE_ITEM_ICON = "SpriteActiveItemImage";

		private const string ACTIVE_ITEM_LABEL_COUNT = "LabelActiveCount";

		private const string ACTIVE_ITEM_SLIDER_TIME = "pBarTrainTimeActive";

		private const string ACTIVE_ITEM_LABEL_TIME = "LabelpBarTrainTimeActive";

		private const string TROOP_ITEM_PANEL = "TroopSelect";

		private const string TROOP_ITEM_GRID = "TroopGrid";

		private const string TROOP_ITEM_SCROLL = "TroopScroll";

		private const string TROOP_ITEM_TEMPLATE = "TroopItemTemplate";

		private const string TROOP_ITEM_BUTTON = "ButtonTroopItemCard";

		private const string TROOP_ITEM_ICON = "SpriteTroopItemImage";

		private const string TROOP_ITEM_LABEL_REQ = "LabelRequirement";

		private const string TROOP_ITEM_COST = "Cost";

		private const string TROOP_ITEM_COST_LABEL = "CostLabel";

		private const string TROOP_ITEM_INFO = "BtnInfo";

		private const string TROOP_ITEM_LABEL_LVL = "LabelTroopLevel";

		private const string TROOP_ITEM_DIMMER = "SpriteDim";

		private const string TROOP_PERK_GROUP = "PerkEffectTroop";

		private const string TROOP_PERK_BUTTON = "btnPerksTroopTraining";

		private const string STARSHIP_ITEM_PANEL = "StarshipSelect";

		private const string STARSHIP_ITEM_GRID = "StarshipGrid";

		private const string STARSHIP_ITEM_SCROLL = "StarshipScroll";

		private const string HERO_TRAINING_GROUP = "HeroTraining";

		private const string HERO_COST_FINISH = "FinishCostHeroes";

		private const string HERO_LABEL_TITLE = "DialogTrainingTitleHeroes";

		private const string HERO_LABEL_CAPACITY = "LabelCurrentlyTrainingHeroes";

		private const string HERO_LABEL_TIME = "LabelTimeCountHeroes";

		private const string HERO_STATIC_TIME = "LabelTimeIntroHeroes";

		private const string HERO_STATIC_FINISH = "LabelFinishIntroHeroes";

		private const string HERO_LABEL_INSTRUCTIONS = "LabelInstructionsHeroes";

		private const string HERO_LABEL_AVAILABLE = "LabelAvailableHeroes";

		private const string HERO_FINISH_BUTTON = "BtnFinishHeroes";

		private const string HERO_PERK_GROUP = "PerkEffectHero";

		private const string HERO_SLOT_GROUP = "HeroSlot{0}";

		private const string HERO_SLOT_CANCEL = "BtnCancel{0}";

		private const string HERO_SLOT_LABEL = "LabelpBarTrainTime{0}";

		private const string HERO_SLOT_SLIDER = "pBarTrainTime{0}";

		private const string HERO_SLOT_SPRITE = "HeroSlotFrame{0}";

		private const string HERO_SLOT_LOCK = "SpriteLockedHeroSlot{0}";

		private const string HERO_SLOT_LABEL_LOCK = "LabelLocked{0}";

		private const string HERO_SLOT_DECAL = "SpriteHeroDecal{0}";

		private const string TROOP_TAB_ALL = "TROOP_TAB_ALL";

		private const string TROOP_CLASS_PREFIX = "trp_class_";

		private const string EQUIPMENT_LOCKED = "EQUIPMENT_LOCKED";

		private const string REBEL_DECAL = "HeroDecalRebel";

		private const string EMPIRE_DECAL = "HeroDecalEmpire";

		private const string HERO_ITEM_GRID = "HeroGrid";

		private const string HERO_ITEM_TEMPLATE = "HeroTemplate";

		private const string HERO_ITEM_BUTTON = "ButtonHeroItemCard";

		private const string HERO_ITEM_INFO = "HeroBtnInfo";

		private const string HERO_ITEM_ICON = "SpriteHeroItemImage";

		private const string HERO_ITEM_COST = "CostHero";

		private const string HERO_ITEM_COST_LABEL = "CostHeroLabel";

		private const string HERO_ITEM_LABEL_REQ = "LabelLockedHero";

		private const string HERO_ITEM_LABEL_LVL = "LabelHeroLevel";

		private const string HERO_ITEM_DIMMER = "SpriteDimHero";

		private const string CARD_DEFAULT = "CardDefault";

		private const string CARD_HERO_DEFAULT = "CardHeroDefault";

		private const string CARD_HERO_QUALITY_PREFIX = "CardHeroQ{0}";

		private const string CARD_QUALITY_PREFIX = "CardQ{0}";

		private const string PERKS_TITLE_LABEL = "DialogTrainingTitlePerks";

		private const string TROOP_SELECT_SCROLL_RIGHT = "SpriteTroopSelectScrollRight";

		private const string TROOP_SELECT_SCROLL_LEFT = "SpriteTroopSelectScrollLeft";

		private const string CLASS_CONTAINER_SPRITE = "SpriteUnitClassContainer";

		private const string NPC = "TutorialCharacter";

		private const string NPC_TEXTURE = "TutorialTexture";

		private const string NPC_LABEL = "LabelTutorialText";

		private const string TEXTURE_NAME_CANTINA = "NPC_Cantina";

		private const float BUTTON_PRESS_DELAY_TIME = 0.4f;

		private const float BUTTON_PRESS_REPEAT_TIME = 0.1f;

		private const int NUM_QUEUE_SLOTS = 5;

		private const int NUM_HERO_SLOTS = 3;

		private UXGrid queueItemGrid;

		private UXElement queueItemTemplate;

		private UXGrid troopItemGrid;

		private UXLabel titleLabel;

		private UXLabel perksTitleLabel;

		private UXLabel buildingCapactiyLabel;

		private UXLabel capacityLabel;

		private UXLabel timeStatic;

		private UXLabel timeLabel;

		private UXLabel finishStatic;

		private UXButton finishButton;

		private UXButton backButton;

		private UXLabel instructionsLabel;

		private UXElement trainingGroup;

		private UXLabel classHintLabel;

		private UXElement activeItem;

		private UXButton itemButtonActive;

		private UXSprite itemIconSpriteActive;

		private UXLabel itemCountLabelActive;

		private UXSlider itemTimeSliderActive;

		private UXLabel itemTimeLabelActive;

		private GeometryProjector activeItemGeometry;

		private UXButton nextButton;

		private UXButton prevButton;

		private UXButton troopPerkButton;

		private int trainingSpace;

		private int trainingSpaceTotal;

		private int housingSpace;

		private int housingSpaceTotal;

		private int totalSecondsLeft;

		private ISupportController supportController;

		private float accumulatedUpdateDt;

		private bool blockedContract;

		private uint buttonDelayTimer;

		private uint buttonRepeatTimer;

		private DeliveryType deliveryType;

		private bool timersRunning;

		private bool repositionNextFrame;

		private TroopTrainingTag confirmingTraining;

		private int specialInstructionsCounterMax;

		private int specialInstructionsCounter;

		private string specialInstructionsTroopUid;

		private string regularInstructions;

		private List<SmartEntity> availableTrainerBuildings;

		private List<QueuedUnitTrainingTag> queuedUnits;

		private Color dimColor;

		private TroopTabHelper tabHelper;

		private List<IDeployableVO> eligibleDeployables;

		private HashSet<TroopRole> eligibleTroopRoles;

		[CompilerGenerated]
		private static Comparison<UXElement> <>f__mg$cache0;

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

		public TroopTrainingScreen(SmartEntity selectedBuilding) : base("gui_troop_training", selectedBuilding)
		{
			this.buttonDelayTimer = 0u;
			this.buttonRepeatTimer = 0u;
			this.availableTrainerBuildings = new List<SmartEntity>();
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			NodeList<BarracksNode> barracksNodeList = buildingLookupController.BarracksNodeList;
			NodeList<FactoryNode> factoryNodeList = buildingLookupController.FactoryNodeList;
			NodeList<CantinaNode> cantinaNodeList = buildingLookupController.CantinaNodeList;
			NodeList<FleetCommandNode> fleetCommandNodeList = buildingLookupController.FleetCommandNodeList;
			NodeList<TacticalCommandNode> tacticalCommandNodeList = buildingLookupController.TacticalCommandNodeList;
			for (BarracksNode barracksNode = barracksNodeList.Head; barracksNode != null; barracksNode = barracksNode.Next)
			{
				this.TryAddTrainerBuilding((SmartEntity)barracksNode.Entity);
			}
			for (FactoryNode factoryNode = factoryNodeList.Head; factoryNode != null; factoryNode = factoryNode.Next)
			{
				this.TryAddTrainerBuilding((SmartEntity)factoryNode.Entity);
			}
			for (CantinaNode cantinaNode = cantinaNodeList.Head; cantinaNode != null; cantinaNode = cantinaNode.Next)
			{
				this.TryAddTrainerBuilding((SmartEntity)cantinaNode.Entity);
			}
			for (FleetCommandNode fleetCommandNode = fleetCommandNodeList.Head; fleetCommandNode != null; fleetCommandNode = fleetCommandNode.Next)
			{
				this.TryAddTrainerBuilding((SmartEntity)fleetCommandNode.Entity);
			}
			for (TacticalCommandNode tacticalCommandNode = tacticalCommandNodeList.Head; tacticalCommandNode != null; tacticalCommandNode = tacticalCommandNode.Next)
			{
				this.TryAddTrainerBuilding((SmartEntity)tacticalCommandNode.Entity);
			}
			this.timersRunning = false;
			this.supportController = Service.ISupportController;
			this.accumulatedUpdateDt = 0f;
			this.repositionNextFrame = false;
			this.dimColor = new Color(1f, 1f, 1f, 0.5f);
			this.tabHelper = new TroopTabHelper();
			this.eligibleDeployables = new List<IDeployableVO>();
			this.eligibleTroopRoles = new HashSet<TroopRole>();
		}

		private void TryAddTrainerBuilding(SmartEntity entity)
		{
			if (!ContractUtils.IsBuildingConstructing(entity) && !ContractUtils.IsBuildingUpgrading(entity))
			{
				this.availableTrainerBuildings.Add(entity);
			}
		}

		protected override void OnScreenLoaded()
		{
			if (this.selectedBuilding == null)
			{
				base.DestroyScreen();
				return;
			}
			this.InitScreen();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.TroopRecruited, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.HeroMobilized, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.StarshipMobilized, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ContractInvalidForStorage, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.InventoryTroopUpdated, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.InventoryHeroUpdated, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.InventorySpecialAttackUpdated, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.DenyUserInput, EventPriority.Default);
		}

		public override void OnDestroyElement()
		{
			this.ResetScreen();
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.TroopRecruited);
			eventManager.UnregisterObserver(this, EventId.HeroMobilized);
			eventManager.UnregisterObserver(this, EventId.StarshipMobilized);
			eventManager.UnregisterObserver(this, EventId.ContractInvalidForStorage);
			eventManager.UnregisterObserver(this, EventId.InventoryTroopUpdated);
			eventManager.UnregisterObserver(this, EventId.InventoryHeroUpdated);
			eventManager.UnregisterObserver(this, EventId.InventorySpecialAttackUpdated);
			eventManager.UnregisterObserver(this, EventId.DenyUserInput);
			this.tabHelper = null;
			this.eligibleDeployables = null;
			this.eligibleTroopRoles = null;
			base.OnDestroyElement();
		}

		private void ResetScreen()
		{
			this.KillTrainingTimers();
			this.KillButtonTimers();
			if (this.troopItemGrid != null)
			{
				this.troopItemGrid.Clear();
				this.troopItemGrid = null;
			}
			if (this.queueItemGrid != null)
			{
				this.queueItemGrid.Clear();
				this.queueItemGrid = null;
			}
			this.tabHelper.Destroy();
			this.eligibleDeployables.Clear();
			this.eligibleTroopRoles.Clear();
			if (this.activeItemGeometry != null)
			{
				this.activeItemGeometry.Destroy();
				this.activeItemGeometry = null;
			}
		}

		private void InitScreen()
		{
			if (this.buildingInfo.Type == BuildingType.HeroMobilizer)
			{
				this.trainingGroup = base.GetElement<UXElement>("HeroTraining");
				base.GetElement<UXElement>("TroopTraining").Visible = false;
			}
			else
			{
				this.trainingGroup = base.GetElement<UXElement>("TroopTraining");
				base.GetElement<UXElement>("HeroTraining").Visible = false;
				base.GetElement<UXSprite>("SpriteTroopSelectScrollRight").Visible = false;
				base.GetElement<UXSprite>("SpriteTroopSelectScrollLeft").Visible = false;
			}
			this.InitVariables();
			this.InitEligibleDeployables();
			this.InitTabs();
			this.InitLabels();
			this.InitButtons();
			this.InitTroopItemGrid();
			this.InitQueueItemGrid();
			this.InitNPC();
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.repositionNextFrame = true;
		}

		private void InitVariables()
		{
			this.trainingSpace = 0;
			this.trainingSpaceTotal = this.buildingInfo.Storage;
			this.deliveryType = ContractUtils.GetTroopContractTypeByBuilding(this.buildingInfo);
			if (this.deliveryType == DeliveryType.Hero)
			{
				foreach (KeyValuePair<string, InventoryEntry> current in Service.CurrentPlayer.GetAllHeroes())
				{
					this.trainingSpaceTotal -= current.Value.Scale * current.Value.Amount;
				}
			}
			this.housingSpace = 0;
			this.housingSpaceTotal = 0;
			this.totalSecondsLeft = 0;
			this.blockedContract = false;
			this.queuedUnits = new List<QueuedUnitTrainingTag>();
		}

		private void InitLabels()
		{
			if (this.deliveryType == DeliveryType.Hero)
			{
				base.GetElement<UXLabel>("LabelAvailableHeroes").Text = this.lang.Get("AVAILABLE_HEROES", new object[0]);
				this.titleLabel = base.GetElement<UXLabel>("DialogTrainingTitleHeroes");
				this.capacityLabel = base.GetElement<UXLabel>("LabelCurrentlyTrainingHeroes");
				this.timeLabel = base.GetElement<UXLabel>("LabelTimeCountHeroes");
				this.timeStatic = base.GetElement<UXLabel>("LabelTimeIntroHeroes");
				this.finishStatic = base.GetElement<UXLabel>("LabelFinishIntroHeroes");
				this.instructionsLabel = base.GetElement<UXLabel>("LabelInstructionsHeroes");
			}
			else
			{
				this.itemIconSpriteActive = base.GetElement<UXSprite>("SpriteActiveItemImage");
				this.itemCountLabelActive = base.GetElement<UXLabel>("LabelActiveCount");
				this.itemTimeSliderActive = base.GetElement<UXSlider>("pBarTrainTimeActive");
				this.itemTimeLabelActive = base.GetElement<UXLabel>("LabelpBarTrainTimeActive");
				this.titleLabel = base.GetElement<UXLabel>("DialogTrainingTitle");
				this.capacityLabel = base.GetElement<UXLabel>("LabelCurrentlyTraining");
				this.buildingCapactiyLabel = base.GetElement<UXLabel>("LabelBarracksCapacity");
				this.timeLabel = base.GetElement<UXLabel>("LabelTimeCount");
				this.timeStatic = base.GetElement<UXLabel>("LabelTimeIntro");
				this.finishStatic = base.GetElement<UXLabel>("LabelFinishIntro");
				this.instructionsLabel = base.GetElement<UXLabel>("LabelInstructions");
				this.classHintLabel = base.GetElement<UXLabel>("LabelUnitClassHint");
				this.classHintLabel.Visible = false;
			}
			this.perksTitleLabel = base.GetElement<UXLabel>("DialogTrainingTitlePerks");
			this.perksTitleLabel.Text = this.titleLabel.Text;
			string buildingVerb = LangUtils.GetBuildingVerb(this.buildingInfo.Type);
			DeliveryType deliveryType = this.deliveryType;
			switch (deliveryType)
			{
			case DeliveryType.Infantry:
				this.instructionsLabel.Text = this.lang.Get("TRAIN_INSTRUCTIONS", new object[]
				{
					this.lang.Get("TROOP", new object[0]),
					buildingVerb
				});
				break;
			case DeliveryType.Vehicle:
				this.instructionsLabel.Text = this.lang.Get("TRAIN_INSTRUCTIONS", new object[]
				{
					this.lang.Get("VEHICLE", new object[0]),
					buildingVerb
				});
				break;
			case DeliveryType.Starship:
				this.instructionsLabel.Text = this.lang.Get("SELECT_STARSHIP", new object[0]);
				break;
			case DeliveryType.Hero:
				this.instructionsLabel.Text = this.lang.Get("s_SelectHeroTitle", new object[0]);
				break;
			default:
				if (deliveryType == DeliveryType.Mercenary)
				{
					this.instructionsLabel.Text = this.lang.Get("TRAIN_INSTRUCTIONS", new object[]
					{
						this.lang.Get("MERCENARY", new object[0]),
						buildingVerb
					});
				}
				break;
			}
			this.regularInstructions = this.instructionsLabel.Text;
			this.specialInstructionsCounterMax = 0;
			this.specialInstructionsTroopUid = null;
			this.trainingGroup.Visible = true;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			if (this.buildingInfo.Type == BuildingType.HeroMobilizer)
			{
				this.finishButton = base.GetElement<UXButton>("BtnFinishHeroes");
			}
			else
			{
				this.finishButton = base.GetElement<UXButton>("BtnFinish");
				this.activeItem = base.GetElement<UXElement>("ButtonActiveItemCard");
				this.itemButtonActive = base.GetElement<UXButton>("BtnRemoveActive");
				this.itemButtonActive.OnClicked = new UXButtonClickedDelegate(this.OnQueueItemButtonClicked);
				this.itemButtonActive.OnPressed = new UXButtonPressedDelegate(this.OnQueueItemButtonPressed);
				this.itemButtonActive.OnReleased = new UXButtonReleasedDelegate(this.OnItemButtonReleased);
				this.itemButtonActive.Tag = this.activeItem;
			}
			this.finishButton.OnClicked = new UXButtonClickedDelegate(this.OnFinishButtonClicked);
			this.prevButton = base.GetElement<UXButton>("BtnHeroPrev");
			this.prevButton.OnClicked = new UXButtonClickedDelegate(this.OnPrevOrNextButtonClicked);
			this.prevButton.Tag = -1;
			this.nextButton = base.GetElement<UXButton>("BtnHeroNext");
			this.nextButton.OnClicked = new UXButtonClickedDelegate(this.OnPrevOrNextButtonClicked);
			this.nextButton.Tag = 1;
			if (this.availableTrainerBuildings.Count <= 1)
			{
				this.prevButton.Visible = false;
				this.nextButton.Visible = false;
			}
		}

		protected override void SetupPerksButton()
		{
			this.titleLabel.Visible = true;
			UXElement element = base.GetElement<UXElement>("TitleGroupPerks");
			element.Visible = false;
			this.troopPerkButton = base.GetElement<UXButton>("btnPerksTroopTraining");
			this.troopPerkButton.Tag = this.buildingInfo;
			this.troopPerkButton.OnClicked = new UXButtonClickedDelegate(base.OnPerksButtonClicked);
			bool flag = Service.PerkManager.IsPerkAppliedToBuilding(this.buildingInfo);
			if (flag)
			{
				element.Visible = true;
				this.titleLabel.Visible = false;
			}
		}

		private void InitNPC()
		{
			UXElement element = base.GetElement<UXElement>("TutorialCharacter");
			if (this.buildingInfo.Type == BuildingType.Cantina)
			{
				element.Visible = true;
				base.GetElement<UXLabel>("LabelTutorialText").Text = this.lang.Get("BLURB_CANTINA", new object[0]);
				base.GetElement<UXTexture>("TutorialTexture").LoadTexture("NPC_Cantina");
			}
			else
			{
				element.Visible = false;
			}
		}

		private void InitEligibleDeployables()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			List<IDeployableVO> list = new List<IDeployableVO>();
			if (this.deliveryType == DeliveryType.Starship)
			{
				foreach (SpecialAttackTypeVO current in staticDataController.GetAll<SpecialAttackTypeVO>())
				{
					list.Add(current);
				}
			}
			else
			{
				foreach (TroopTypeVO current2 in staticDataController.GetAll<TroopTypeVO>())
				{
					list.Add(current2);
				}
			}
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				IDeployableVO deployableVO = list[i];
				if (this.UnitIsEligibleForThisScreen(deployableVO, currentPlayer))
				{
					this.eligibleDeployables.Add(deployableVO);
					if (deployableVO is TroopTypeVO)
					{
						TroopRole troopRole = ((TroopTypeVO)deployableVO).TroopRole;
						if (!this.eligibleTroopRoles.Contains(troopRole))
						{
							this.eligibleTroopRoles.Add(troopRole);
						}
					}
				}
				i++;
			}
		}

		private bool UnitIsEligibleForThisScreen(IDeployableVO unit, CurrentPlayer player)
		{
			if (unit.Lvl <= 0 || unit.Size <= 0)
			{
				return false;
			}
			if (!unit.PlayerFacing)
			{
				return false;
			}
			if (unit.Faction != this.buildingInfo.Faction)
			{
				return false;
			}
			if (unit is TroopTypeVO)
			{
				TroopTypeVO troopTypeVO = unit as TroopTypeVO;
				if ((this.deliveryType == DeliveryType.Infantry && troopTypeVO.Type != TroopType.Infantry) || (this.deliveryType == DeliveryType.Vehicle && troopTypeVO.Type != TroopType.Vehicle) || (this.deliveryType == DeliveryType.Mercenary && troopTypeVO.Type != TroopType.Mercenary) || (this.deliveryType == DeliveryType.Hero && troopTypeVO.Type != TroopType.Hero))
				{
					return false;
				}
				if (player.UnlockedLevels.Troops.GetLevel(troopTypeVO.UpgradeGroup) != troopTypeVO.Lvl)
				{
					return false;
				}
			}
			else if (unit is SpecialAttackTypeVO && player.UnlockedLevels.Starships.GetLevel(unit.UpgradeGroup) != unit.Lvl)
			{
				return false;
			}
			return true;
		}

		private void InitTabs()
		{
			if (this.deliveryType == DeliveryType.Starship || this.deliveryType == DeliveryType.Hero)
			{
				this.tabHelper.HideTabs(this);
				return;
			}
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(0, this.lang.Get("TROOP_TAB_ALL", new object[0]));
			IEnumerator enumerator = Enum.GetValues(typeof(TroopRole)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					TroopRole troopRole = (TroopRole)enumerator.Current;
					if (this.eligibleTroopRoles.Contains(troopRole))
					{
						TroopTab key = this.tabHelper.ConvertTroopRoleToTab(troopRole);
						dictionary.Add((int)key, this.lang.Get("trp_class_" + troopRole.ToString(), new object[0]));
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.tabHelper.CreateTabs(this, new Action(this.OnTroopTabChanged), dictionary, 0);
		}

		private void SetupItemQuality(string itemUid, int quality, bool hero)
		{
			string cardName = (!hero) ? "CardQ{0}" : "CardHeroQ{0}";
			string name = (!hero) ? "CardDefault" : "CardHeroDefault";
			this.troopItemGrid.GetSubElement<UXElement>(itemUid, name).Visible = false;
			UXElement uXElement = UXUtils.SetCardQuality(this, this.troopItemGrid, itemUid, quality, cardName);
			if (uXElement != null)
			{
				uXElement.SkipBoundsCalculations(true);
			}
		}

		private void InitTroopItemGrid()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			bool isPerkApplied = Service.PerkManager.IsPerkAppliedToBuilding(this.buildingInfo);
			string perkGroupName = string.Empty;
			bool flag = this.deliveryType == DeliveryType.Hero;
			if (flag)
			{
				this.troopItemGrid = base.GetElement<UXGrid>("HeroGrid");
				this.troopItemGrid.SetTemplateItem("HeroTemplate");
				perkGroupName = "PerkEffectHero";
			}
			else
			{
				if (this.deliveryType == DeliveryType.Starship)
				{
					this.troopItemGrid = base.GetElement<UXGrid>("StarshipGrid");
					this.UpdateItemPanel(true);
				}
				else
				{
					this.troopItemGrid = base.GetElement<UXGrid>("TroopGrid");
					this.UpdateItemPanel(false);
				}
				perkGroupName = "PerkEffectTroop";
				this.troopItemGrid.SetTemplateItem("TroopItemTemplate");
			}
			this.troopItemGrid.OnDrag = new UXDragDelegate(this.OnGridDrag);
			UnlockController unlockController = Service.UnlockController;
			List<UXElement> list = new List<UXElement>();
			int i = 0;
			int count = this.eligibleDeployables.Count;
			while (i < count)
			{
				IDeployableVO deployableVO = this.eligibleDeployables[i];
				TroopTab currentTab = (TroopTab)this.tabHelper.CurrentTab;
				if (currentTab == TroopTab.All || !(deployableVO is TroopTypeVO))
				{
					goto IL_146;
				}
				TroopRole troopRole = ((TroopTypeVO)deployableVO).TroopRole;
				TroopTab troopTab = this.tabHelper.ConvertTroopRoleToTab(troopRole);
				if (currentTab == troopTab)
				{
					goto IL_146;
				}
				IL_561:
				i++;
				continue;
				IL_146:
				BuildingTypeVO buildingTypeVO = null;
				bool flag2 = unlockController.IsUnlocked(deployableVO, 0, out buildingTypeVO);
				TroopTrainingTag troopTrainingTag = new TroopTrainingTag(deployableVO, flag2);
				string uid = deployableVO.Uid;
				UXElement uXElement = this.troopItemGrid.CloneTemplateItem(uid);
				uXElement.Tag = troopTrainingTag;
				UXLabel subElement = this.troopItemGrid.GetSubElement<UXLabel>(uid, (!flag) ? "CostLabel" : "CostHeroLabel");
				troopTrainingTag.CostLabel = subElement;
				int credits = deployableVO.Credits;
				int materials = deployableVO.Materials;
				int contraband = deployableVO.Contraband;
				this.UpdateCostWithAppliedPerks(ref credits, ref materials, ref contraband);
				UXUtils.SetupCostElements(this, (!flag) ? "Cost" : "CostHero", uid, credits, materials, contraband, 0, false, null);
				ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
				UXSprite subElement2 = this.troopItemGrid.GetSubElement<UXSprite>(uid, (!flag) ? "SpriteTroopItemImage" : "SpriteHeroItemImage");
				ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(deployableVO, subElement2, true);
				Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(deployableVO, projectorConfig, activeArmory));
				projectorConfig.AnimPreference = AnimationPreference.NoAnimation;
				ProjectorUtils.GenerateProjector(projectorConfig);
				string text = null;
				if (deployableVO is TroopTypeVO)
				{
					TroopTypeVO troop = deployableVO as TroopTypeVO;
					Service.SkinController.GetApplicableSkin(troop, activeArmory.Equipment, out text);
				}
				UXLabel subElement3 = this.troopItemGrid.GetSubElement<UXLabel>(uid, (!flag) ? "LabelTroopLevel" : "LabelHeroLevel");
				subElement3.Text = LangUtils.GetLevelText(deployableVO.Lvl);
				string cardName = (!flag) ? "CardQ{0}" : "CardHeroQ{0}";
				UXUtils.HideGridQualityCards(this.troopItemGrid, uid, cardName);
				string name = (!flag) ? "CardDefault" : "CardHeroDefault";
				this.troopItemGrid.GetSubElement<UXElement>(uid, name).Visible = true;
				int upgradeQualityForDeployable = Service.DeployableShardUnlockController.GetUpgradeQualityForDeployable(deployableVO);
				if (upgradeQualityForDeployable > 0)
				{
					this.SetupItemQuality(uid, upgradeQualityForDeployable, flag);
				}
				else if (!string.IsNullOrEmpty(text))
				{
					EquipmentVO optional = staticDataController.GetOptional<EquipmentVO>(text);
					if (optional != null)
					{
						int quality = (int)optional.Quality;
						this.SetupItemQuality(uid, quality, flag);
					}
				}
				UXButton subElement4 = this.troopItemGrid.GetSubElement<UXButton>(uid, (!flag) ? "BtnInfo" : "HeroBtnInfo");
				subElement4.Tag = uXElement;
				subElement4.OnClicked = new UXButtonClickedDelegate(this.OnTroopItemInfoClicked);
				if (flag2 && !string.IsNullOrEmpty(deployableVO.BuildingRequirement))
				{
					BuildingTypeVO minBuildingRequirement = Service.BuildingLookupController.GetMinBuildingRequirement(deployableVO);
					flag2 = (minBuildingRequirement.Type == this.buildingInfo.Type && minBuildingRequirement.Lvl <= this.buildingInfo.Lvl);
					if (!flag2)
					{
						buildingTypeVO = minBuildingRequirement;
					}
				}
				troopTrainingTag.ReqMet = flag2;
				UXSprite subElement5 = this.troopItemGrid.GetSubElement<UXSprite>(uid, (!flag) ? "SpriteDim" : "SpriteDimHero");
				subElement5.Visible = !flag2;
				troopTrainingTag.Dimmer = subElement5;
				UXLabel subElement6 = this.troopItemGrid.GetSubElement<UXLabel>(uid, (!flag) ? "LabelRequirement" : "LabelLockedHero");
				if (!flag2)
				{
					if (deployableVO.UnlockedByEvent)
					{
						subElement6.Text = LangUtils.GetShardLockedDeployableString(deployableVO);
					}
					else
					{
						subElement6.Text = this.lang.Get("BUILDING_REQUIREMENT", new object[]
						{
							buildingTypeVO.Lvl,
							LangUtils.GetBuildingDisplayName(buildingTypeVO)
						});
					}
				}
				else
				{
					subElement6.Text = string.Empty;
				}
				UXButton subElement7 = this.troopItemGrid.GetSubElement<UXButton>(uid, (!flag) ? "ButtonTroopItemCard" : "ButtonHeroItemCard");
				subElement7.Tag = uXElement;
				subElement7.OnPressed = new UXButtonPressedDelegate(this.OnTroopItemButtonPressed);
				subElement7.OnReleased = new UXButtonReleasedDelegate(this.OnItemButtonReleased);
				troopTrainingTag.TroopButton = subElement7;
				this.UpdateTroopButtonEnabled(troopTrainingTag);
				this.UpdateGridItemPerkElement(troopTrainingTag, perkGroupName, isPerkApplied, flag2);
				list.Add(uXElement);
				goto IL_561;
			}
			List<UXElement> arg_58F_0 = list;
			if (TroopTrainingScreen.<>f__mg$cache0 == null)
			{
				TroopTrainingScreen.<>f__mg$cache0 = new Comparison<UXElement>(TroopTrainingScreen.CompareTroopItem);
			}
			arg_58F_0.Sort(TroopTrainingScreen.<>f__mg$cache0);
			if (this.buildingInfo.Type != BuildingType.HeroMobilizer)
			{
				UXUtils.SortListForTwoRowGrids(list, this.troopItemGrid);
			}
			else
			{
				this.troopItemGrid.MaxItemsPerLine = list.Count;
			}
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				this.troopItemGrid.AddItem(list[j], j);
				j++;
			}
			this.troopItemGrid.RepositionItemsFrameDelayed(null, 2);
			this.troopItemGrid.IsScrollable = true;
			if (this.tabHelper != null)
			{
				this.tabHelper.SetSelectable(true);
			}
		}

		private void UpdateGridItemPerkElement(TroopTrainingTag tag, string perkGroupName, bool isPerkApplied, bool reqMet)
		{
			if (string.IsNullOrEmpty(perkGroupName))
			{
				return;
			}
			UXElement optionalSubElement = this.troopItemGrid.GetOptionalSubElement<UXElement>(tag.Troop.Uid, perkGroupName);
			if (optionalSubElement == null || optionalSubElement.Root == null)
			{
				return;
			}
			base.RevertToOriginalNameRecursively(optionalSubElement.Root, tag.Troop.Uid);
			optionalSubElement.Visible = (isPerkApplied && reqMet);
			if (optionalSubElement.Visible)
			{
				Animation component = optionalSubElement.Root.GetComponent<Animation>();
				if (component != null)
				{
					component.Play();
				}
			}
		}

		public void DisableTroopItemScrolling()
		{
			if (this.troopItemGrid != null)
			{
				this.troopItemGrid.IsScrollable = false;
			}
		}

		public void DisableTabSelection()
		{
			if (this.tabHelper != null)
			{
				this.tabHelper.SetSelectable(false);
			}
		}

		private void UpdateItemPanel(bool forStarships)
		{
			base.GetElement<UXElement>("TroopSelect").Visible = !forStarships;
			base.GetElement<UXElement>("TroopScroll").Visible = !forStarships;
			base.GetElement<UXElement>("StarshipSelect").Visible = forStarships;
			base.GetElement<UXElement>("StarshipScroll").Visible = forStarships;
			base.GetElement<UXElement>("SpriteUnitClassContainer").Visible = !forStarships;
		}

		private static int CompareTroopItem(UXElement a, UXElement b)
		{
			if (a == b)
			{
				return 0;
			}
			IDeployableVO troop = ((TroopTrainingTag)a.Tag).Troop;
			IDeployableVO troop2 = ((TroopTrainingTag)b.Tag).Troop;
			int num = troop.Order - troop2.Order;
			if (num == 0)
			{
				Service.Logger.WarnFormat("Troop {0} matches order ({1}) of {2}", new object[]
				{
					troop.Uid,
					troop.Order,
					troop2.Uid
				});
			}
			return num;
		}

		private void OnTroopTabChanged()
		{
			this.troopItemGrid.Clear();
			Service.Engine.ForceGarbageCollection(null);
			this.InitTroopItemGrid();
			TroopTab currentTab = (TroopTab)this.tabHelper.CurrentTab;
			if (currentTab != TroopTab.All)
			{
				this.classHintLabel.Visible = true;
				this.classHintLabel.Text = this.lang.Get("trp_class_hint_" + this.tabHelper.CurrentTab.ToString(), new object[0]);
			}
			else
			{
				this.classHintLabel.Visible = false;
			}
		}

		private void AddNewQueueListItem(int index)
		{
			TroopTrainingTag troopTrainingTag = new TroopTrainingTag(null, false);
			string itemUid = index.ToString();
			UXElement uXElement = this.queueItemGrid.CloneTemplateItem(itemUid);
			uXElement.Tag = troopTrainingTag;
			UXLabel subElement = this.queueItemGrid.GetSubElement<UXLabel>(itemUid, "LabelQueueCount");
			subElement.Visible = false;
			troopTrainingTag.QueueCountLabel = subElement;
			if (troopTrainingTag.Projector != null)
			{
				troopTrainingTag.Projector.Destroy();
			}
			UXButton subElement2 = this.queueItemGrid.GetSubElement<UXButton>(itemUid, "BtnRemove");
			subElement2.Tag = uXElement;
			subElement2.OnClicked = new UXButtonClickedDelegate(this.OnQueueItemButtonClicked);
			subElement2.OnPressed = new UXButtonPressedDelegate(this.OnQueueItemButtonPressed);
			subElement2.OnReleased = new UXButtonReleasedDelegate(this.OnItemButtonReleased);
			subElement2.Visible = false;
			troopTrainingTag.QueueButton = subElement2;
			int order = index;
			this.queueItemGrid.AddItem(uXElement, order);
		}

		private void InitQueueItemGrid()
		{
			if (this.buildingInfo.Type != BuildingType.HeroMobilizer)
			{
				this.queueItemGrid = base.GetElement<UXGrid>("QueueGrid");
				this.queueItemGrid.OnDrag = new UXDragDelegate(this.OnGridDrag);
				this.queueItemGrid.SetTemplateItem("QueueItemTemplate");
			}
			List<Contract> list = this.supportController.FindAllTroopContractsForBuilding(this.selectedBuilding.Get<BuildingComponent>().BuildingTO.Key);
			for (int i = 0; i < list.Count; i++)
			{
				int timeLeft;
				if (i == 0)
				{
					timeLeft = list[i].GetRemainingTimeForView();
					this.blockedContract = !this.supportController.IsContractValidForStorage(list[i]);
				}
				else
				{
					timeLeft = list[i].TotalTime;
				}
				this.HandleContract(list[i], timeLeft);
			}
		}

		private void HandleContract(Contract contract, int timeLeft)
		{
			IDeployableVO deployableVO;
			if (contract.DeliveryType == DeliveryType.Starship)
			{
				deployableVO = Service.StaticDataController.Get<SpecialAttackTypeVO>(contract.ProductUid);
			}
			else
			{
				deployableVO = Service.StaticDataController.Get<TroopTypeVO>(contract.ProductUid);
			}
			QueuedUnitTrainingTag queuedUnitTrainingTag = this.GetQueuedItemDataFromUpgradeGroup(deployableVO.UpgradeGroup);
			if (queuedUnitTrainingTag == null)
			{
				queuedUnitTrainingTag = new QueuedUnitTrainingTag();
				queuedUnitTrainingTag.UnitVO = deployableVO;
				this.queuedUnits.Add(queuedUnitTrainingTag);
			}
			this.QueueTroop(queuedUnitTrainingTag, timeLeft, contract);
		}

		private void QueueTroop(QueuedUnitTrainingTag tag, int timeLeft, Contract contract)
		{
			this.AdjustTrainingSpace(tag.UnitVO.Size);
			this.AdjustTotalSecondsLeft(timeLeft);
			tag.Contracts.Add(contract);
			tag.TimeLeftFloat = (float)tag.TimeLeft;
			this.RedrawQueueItems();
			if (this.specialInstructionsTroopUid == contract.ProductUid)
			{
				this.specialInstructionsCounter++;
				if (this.specialInstructionsCounter >= this.specialInstructionsCounterMax)
				{
					Service.UXController.MiscElementsManager.HideTroopCounter();
					this.specialInstructionsCounterMax = 0;
					this.specialInstructionsCounter = 0;
					this.specialInstructionsTroopUid = null;
				}
				else
				{
					TroopTrainingTag troopTrainingTagFromUpgradeGroup = this.GetTroopTrainingTagFromUpgradeGroup(tag.UnitVO.UpgradeGroup);
					Service.UXController.MiscElementsManager.ShowTroopCounter(troopTrainingTagFromUpgradeGroup.TroopButton, this.specialInstructionsCounter, this.specialInstructionsCounterMax);
				}
			}
		}

		private void RedrawQueueItems()
		{
			if (this.buildingInfo.Type == BuildingType.HeroMobilizer)
			{
				this.RedrawHeroQueueItems();
			}
			else
			{
				this.RedrawTroopQueueItems();
				this.RepositionAndScroll();
			}
		}

		private void RedrawTroopQueueItems()
		{
			if (this.queuedUnits.Count == 0)
			{
				if (this.activeItemGeometry != null)
				{
					this.activeItemGeometry.Destroy();
				}
				this.itemTimeLabelActive.Text = string.Empty;
				this.itemCountLabelActive.Text = string.Empty;
				this.itemTimeSliderActive.Visible = false;
				this.itemButtonActive.Visible = false;
			}
			else
			{
				QueuedUnitTrainingTag queuedUnitTrainingTag = this.queuedUnits[0];
				this.activeItem.Tag = this.GetTroopTrainingTagFromUpgradeGroup(queuedUnitTrainingTag.UnitVO.UpgradeGroup);
				this.itemButtonActive.Visible = true;
				this.itemTimeSliderActive.Visible = true;
				this.itemCountLabelActive.Text = LangUtils.GetMultiplierText(queuedUnitTrainingTag.Contracts.Count);
				ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(queuedUnitTrainingTag.UnitVO, this.itemIconSpriteActive);
				Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(queuedUnitTrainingTag.UnitVO, projectorConfig, Service.CurrentPlayer.ActiveArmory));
				projectorConfig.AnimPreference = AnimationPreference.AnimationAlways;
				projectorConfig.AnimState = ((!(queuedUnitTrainingTag.UnitVO is TroopTypeVO)) ? AnimState.Idle : AnimState.Walk);
				if (this.activeItemGeometry == null || !projectorConfig.IsEquivalentTo(this.activeItemGeometry.Config))
				{
					this.activeItemGeometry = ProjectorUtils.GenerateProjector(projectorConfig);
				}
				this.UpdateTimeLeftLabels(queuedUnitTrainingTag);
				this.UpdateTimeLeftSlider(queuedUnitTrainingTag);
			}
			while (this.queueItemGrid.Count < 5 || this.queuedUnits.Count - 1 > this.queueItemGrid.Count)
			{
				this.AddNewQueueListItem(this.queueItemGrid.Count);
			}
			while (this.queuedUnits.Count - 1 <= 5 && this.queueItemGrid.Count > 5)
			{
				UXElement item = this.queueItemGrid.GetItem(this.queueItemGrid.Count - 1);
				this.queueItemGrid.RemoveItem(item);
				base.DestroyElement(item);
			}
			for (int i = 0; i < this.queueItemGrid.Count; i++)
			{
				int i2 = this.queueItemGrid.Count - i - 1;
				TroopTrainingTag troopTrainingTag = this.queueItemGrid.GetItem(i2).Tag as TroopTrainingTag;
				UXSprite subElement = this.queueItemGrid.GetSubElement<UXSprite>(i2.ToString(), "SpriteQueueItemImage");
				if (i + 1 < this.queuedUnits.Count)
				{
					QueuedUnitTrainingTag queuedUnitTrainingTag2 = this.queuedUnits[i + 1];
					if (troopTrainingTag.Troop != queuedUnitTrainingTag2.UnitVO)
					{
						ProjectorConfig projectorConfig2 = ProjectorUtils.GenerateGeometryConfig(queuedUnitTrainingTag2.UnitVO, subElement);
						Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(queuedUnitTrainingTag2.UnitVO, projectorConfig2, Service.CurrentPlayer.ActiveArmory));
						projectorConfig2.AnimPreference = AnimationPreference.AnimationPreferred;
						troopTrainingTag.Projector = ProjectorUtils.GenerateProjector(projectorConfig2);
					}
					troopTrainingTag.Troop = queuedUnitTrainingTag2.UnitVO;
					troopTrainingTag.QueueCountLabel.Visible = true;
					troopTrainingTag.QueueButton.Visible = true;
					string multiplierText = LangUtils.GetMultiplierText(queuedUnitTrainingTag2.Contracts.Count);
					troopTrainingTag.QueueCountLabel.Text = multiplierText;
				}
				else
				{
					if (troopTrainingTag.Projector != null)
					{
						troopTrainingTag.Projector.Destroy();
					}
					troopTrainingTag.QueueCountLabel.Visible = false;
					troopTrainingTag.QueueButton.Visible = false;
					troopTrainingTag.Troop = null;
				}
			}
		}

		private void RedrawHeroQueueItems()
		{
			List<string> list = new List<string>();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			IEnumerable<KeyValuePair<string, InventoryEntry>> allHeroes = currentPlayer.GetAllHeroes();
			foreach (KeyValuePair<string, InventoryEntry> current in allHeroes)
			{
				if (current.Value.Amount > 0)
				{
					list.Add(current.Key);
				}
			}
			int num = 0;
			int num2 = 0;
			bool flag = currentPlayer.Faction == FactionType.Rebel;
			for (int i = 1; i <= 3; i++)
			{
				UXElement element = base.GetElement<UXElement>(string.Format("HeroSlot{0}", i));
				UXButton element2 = base.GetElement<UXButton>(string.Format("BtnCancel{0}", i));
				UXSprite element3 = base.GetElement<UXSprite>(string.Format("HeroSlotFrame{0}", i));
				UXSprite element4 = base.GetElement<UXSprite>(string.Format("SpriteLockedHeroSlot{0}", i));
				UXLabel element5 = base.GetElement<UXLabel>(string.Format("LabelLocked{0}", i));
				UXSlider element6 = base.GetElement<UXSlider>(string.Format("pBarTrainTime{0}", i));
				UXLabel element7 = base.GetElement<UXLabel>(string.Format("LabelpBarTrainTime{0}", i));
				TroopTrainingTag troopTrainingTag;
				if (element.Tag == null)
				{
					troopTrainingTag = new TroopTrainingTag(null, false);
					element.Tag = troopTrainingTag;
					element2.Tag = element;
					element2.OnClicked = new UXButtonClickedDelegate(this.OnQueueItemButtonClicked);
					element2.OnPressed = new UXButtonPressedDelegate(this.OnQueueItemButtonPressed);
					element2.OnReleased = new UXButtonReleasedDelegate(this.OnItemButtonReleased);
				}
				else
				{
					troopTrainingTag = (element.Tag as TroopTrainingTag);
				}
				UXSprite element8 = base.GetElement<UXSprite>(string.Format("SpriteHeroDecal{0}", i));
				element8.SpriteName = ((!flag) ? "HeroDecalEmpire" : "HeroDecalRebel");
				element8.Color = Color.white;
				QueuedUnitTrainingTag queuedUnitTrainingTag = null;
				if (num < this.queuedUnits.Count)
				{
					queuedUnitTrainingTag = this.queuedUnits[num++];
				}
				element5.Text = string.Empty;
				if (queuedUnitTrainingTag != null)
				{
					bool flag2 = i == 1;
					troopTrainingTag.Troop = queuedUnitTrainingTag.UnitVO;
					element2.Visible = true;
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(queuedUnitTrainingTag.UnitVO, element3);
					Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(queuedUnitTrainingTag.UnitVO, projectorConfig, Service.CurrentPlayer.ActiveArmory));
					projectorConfig.AnimState = ((!flag2) ? AnimState.Idle : AnimState.Walk);
					projectorConfig.AnimPreference = ((!flag2) ? AnimationPreference.AnimationPreferred : AnimationPreference.AnimationAlways);
					troopTrainingTag.Projector = ProjectorUtils.GenerateProjector(projectorConfig);
					if (flag2)
					{
						this.activeItem = element;
						this.itemTimeLabelActive = element7;
						this.itemTimeSliderActive = element6;
						element6.Visible = true;
						element7.Visible = true;
					}
					else
					{
						element6.Visible = false;
						element7.Visible = true;
						element7.Text = this.lang.Get("WAITING", new object[0]);
					}
					element4.Visible = false;
				}
				else if (num2 < list.Count)
				{
					string uid = list[num2++];
					TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(uid);
					troopTrainingTag.Troop = troopTypeVO;
					ProjectorConfig projectorConfig2 = ProjectorUtils.GenerateGeometryConfig(troopTypeVO, element3);
					Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(troopTypeVO, projectorConfig2, Service.CurrentPlayer.ActiveArmory));
					projectorConfig2.AnimPreference = AnimationPreference.AnimationPreferred;
					troopTrainingTag.Projector = ProjectorUtils.GenerateProjector(projectorConfig2);
					element6.Visible = false;
					element7.Visible = true;
					element7.Text = this.lang.Get("IN_YOUR_CAMP", new object[0]);
					element2.Visible = false;
					element4.Visible = false;
				}
				else
				{
					bool flag3 = i - 1 < this.housingSpaceTotal;
					element6.Visible = false;
					element7.Visible = false;
					element2.Visible = false;
					element4.Visible = !flag3;
					element8.Color = this.dimColor;
					if (!flag3)
					{
						BuildingTypeVO buildingType = this.selectedBuilding.Get<BuildingComponent>().BuildingType;
						BuildingTypeVO heroSlotUnlockRequirement = Service.BuildingLookupController.GetHeroSlotUnlockRequirement(buildingType, i);
						if (heroSlotUnlockRequirement != null)
						{
							element5.Text = this.lang.Get("BUILDING_REQUIREMENT", new object[]
							{
								heroSlotUnlockRequirement.Lvl,
								LangUtils.GetBuildingDisplayName(heroSlotUnlockRequirement)
							});
						}
						else
						{
							element5.Text = this.lang.Get("s_HeroLocked", new object[0]);
						}
					}
					if (troopTrainingTag.Projector != null)
					{
						troopTrainingTag.Projector.Destroy();
					}
				}
			}
			if (this.queuedUnits.Count != 0)
			{
				this.UpdateTimeLeftLabels(this.queuedUnits[0]);
				this.UpdateTimeLeftSlider(this.queuedUnits[0]);
			}
		}

		private void RepositionAndScroll()
		{
			if (this.queueItemGrid == null)
			{
				return;
			}
			this.queueItemGrid.IsScrollable = true;
			bool flag = false;
			TroopTrainingTag troopTrainingTag = null;
			if (this.queueItemGrid.Count > 0)
			{
				troopTrainingTag = (this.queueItemGrid.GetItem(this.queueItemGrid.Count - 1).Tag as TroopTrainingTag);
				flag = troopTrainingTag.QueueButton.Visible;
				troopTrainingTag.QueueButton.Visible = false;
			}
			this.queueItemGrid.RepositionItems(false);
			this.queueItemGrid.Scroll(1f);
			if (flag)
			{
				troopTrainingTag.QueueButton.Visible = true;
			}
			if (this.queuedUnits.Count <= 5)
			{
				this.queueItemGrid.IsScrollable = false;
			}
		}

		private void DequeueTroop(QueuedUnitTrainingTag tag, bool fromLast)
		{
			this.AdjustTrainingSpace(-tag.UnitVO.Size);
			int num;
			if (fromLast)
			{
				int index = tag.Contracts.Count - 1;
				num = tag.Contracts[index].GetRemainingTimeForView();
				tag.Contracts.RemoveAt(index);
			}
			else
			{
				num = tag.TimeLeft;
				tag.Contracts.RemoveAt(0);
			}
			tag.TimeLeftFloat = (float)tag.TimeLeft;
			this.AdjustTotalSecondsLeft(-num);
			if (tag.Contracts.Count == 0)
			{
				this.KillButtonTimers();
				this.queuedUnits.Remove(tag);
			}
			this.blockedContract = false;
			if (this.queuedUnits.Count != 0)
			{
				tag = this.queuedUnits[0];
				this.blockedContract = !this.supportController.IsContractValidForStorage(tag.Contracts[0]);
			}
			this.RedrawQueueItems();
		}

		public override void RefreshView()
		{
			if (!base.IsLoaded())
			{
				return;
			}
			UXLabel arg_57_0 = this.titleLabel;
			string text = this.lang.Get("BUILDING_INFO", new object[]
			{
				LangUtils.GetBuildingDisplayName(this.buildingInfo),
				this.buildingInfo.Lvl
			});
			this.titleLabel.Text = text;
			arg_57_0.Text = text;
			this.perksTitleLabel.Text = this.titleLabel.Text;
			bool flag = this.trainingSpace >= this.trainingSpaceTotal;
			bool flag2 = this.trainingSpace <= 0;
			DeliveryType deliveryType = this.deliveryType;
			switch (deliveryType)
			{
			case DeliveryType.Infantry:
				if (flag)
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("BARRACKS_FULL", new object[0]);
				}
				else if (flag2)
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("BARRACKS_EMPTY", new object[0]);
				}
				else
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("BARRACKS_CAPACITY", new object[]
					{
						this.trainingSpace,
						this.trainingSpaceTotal
					});
				}
				break;
			case DeliveryType.Vehicle:
				if (flag)
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("FACTORY_FULL", new object[0]);
				}
				else if (flag2)
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("FACTORY_EMPTY", new object[0]);
				}
				else
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("FACTORY_CAPACITY", new object[]
					{
						this.trainingSpace,
						this.trainingSpaceTotal
					});
				}
				break;
			case DeliveryType.Starship:
				if (flag)
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("STARSHIP_COMMAND_FULL", new object[0]);
				}
				else if (flag2)
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("STARSHIP_COMMAND_EMPTY", new object[0]);
				}
				else
				{
					this.buildingCapactiyLabel.Text = this.lang.Get("STARSHIP_COMMAND_CAPACITY", new object[]
					{
						this.trainingSpace,
						this.trainingSpaceTotal
					});
				}
				break;
			default:
				if (deliveryType == DeliveryType.Mercenary)
				{
					if (flag)
					{
						this.buildingCapactiyLabel.Text = this.lang.Get("CANTINA_FULL", new object[0]);
					}
					else if (flag2)
					{
						this.buildingCapactiyLabel.Text = this.lang.Get("CANTINA_EMPTY", new object[0]);
					}
					else
					{
						this.buildingCapactiyLabel.Text = this.lang.Get("CANTINA_CAPACITY", new object[]
						{
							this.trainingSpace,
							this.trainingSpaceTotal
						});
					}
				}
				break;
			}
			this.UpdateHousingSpace();
			this.UpdateAllTroopButtonStates();
			this.UpdateTrainingLabels();
			this.RedrawQueueItems();
		}

		public void SetSpecialInstructions(string instructionsUid, int maxCount)
		{
			TroopTrainingTag troopTrainingTag = null;
			for (int i = 0; i < this.troopItemGrid.Count; i++)
			{
				TroopTrainingTag troopTrainingTag2 = this.troopItemGrid.GetItem(i).Tag as TroopTrainingTag;
				if (troopTrainingTag2.Troop.Uid == instructionsUid)
				{
					troopTrainingTag = troopTrainingTag2;
					break;
				}
			}
			if (troopTrainingTag != null)
			{
				this.specialInstructionsTroopUid = instructionsUid;
				this.specialInstructionsCounter = 0;
				this.specialInstructionsCounterMax = maxCount;
				QueuedUnitTrainingTag queuedItemDataFromUpgradeGroup = this.GetQueuedItemDataFromUpgradeGroup(troopTrainingTag.Troop.UpgradeGroup);
				Service.UXController.MiscElementsManager.ShowTroopCounter(troopTrainingTag.TroopButton, (queuedItemDataFromUpgradeGroup != null) ? queuedItemDataFromUpgradeGroup.Contracts.Count : 0, this.specialInstructionsCounterMax);
			}
		}

		private void UpdateHousingSpace()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			BuildingType type = this.buildingInfo.Type;
			switch (type)
			{
			case BuildingType.Barracks:
			case BuildingType.Factory:
				break;
			case BuildingType.FleetCommand:
				this.housingSpace = currentPlayer.Inventory.SpecialAttack.GetTotalStorageAmount();
				this.housingSpaceTotal = currentPlayer.Inventory.SpecialAttack.GetTotalStorageCapacity();
				goto IL_AF;
			case BuildingType.HeroMobilizer:
				this.housingSpace = currentPlayer.Inventory.Hero.GetTotalStorageAmount();
				this.housingSpaceTotal = currentPlayer.Inventory.Hero.GetTotalStorageCapacity();
				goto IL_AF;
			default:
				if (type != BuildingType.Cantina)
				{
					goto IL_AF;
				}
				break;
			}
			GameUtils.GetStarportTroopCounts(out this.housingSpace, out this.housingSpaceTotal);
			IL_AF:
			bool enabled = this.housingSpace + ContractUtils.CalculateSpaceOccupiedByQueuedTroops(this.selectedBuilding) <= this.housingSpaceTotal;
			int i = 0;
			int count = this.availableTrainerBuildings.Count;
			while (i < count)
			{
				SmartEntity smartEntity = this.availableTrainerBuildings[i];
				BuildingComponent buildingComp = smartEntity.BuildingComp;
				bool flag = false;
				if (this.buildingInfo.Type == BuildingType.Factory || this.buildingInfo.Type == BuildingType.Barracks || this.buildingInfo.Type == BuildingType.Cantina)
				{
					if (buildingComp.BuildingType.Type == BuildingType.Factory || buildingComp.BuildingType.Type == BuildingType.Barracks || buildingComp.BuildingType.Type == BuildingType.Cantina)
					{
						flag = true;
					}
				}
				else if (buildingComp.BuildingType.Type == this.buildingInfo.Type)
				{
					flag = true;
				}
				if (flag)
				{
					this.housingSpace += ContractUtils.CalculateSpaceOccupiedByQueuedTroops(smartEntity);
				}
				i++;
			}
			string text = string.Empty;
			BuildingType type2 = this.buildingInfo.Type;
			switch (type2)
			{
			case BuildingType.Barracks:
				text = this.lang.Get("HOUSING_CAPACITY", new object[]
				{
					this.housingSpace,
					this.housingSpaceTotal
				});
				break;
			case BuildingType.Factory:
				text = this.lang.Get("VEHICLE_CAPACITY", new object[]
				{
					this.housingSpace,
					this.housingSpaceTotal
				});
				break;
			case BuildingType.FleetCommand:
				text = this.lang.Get("STARSHIP_CAPACITY", new object[]
				{
					this.housingSpace,
					this.housingSpaceTotal
				});
				break;
			case BuildingType.HeroMobilizer:
				text = this.lang.Get("HERO_CAPACITY", new object[]
				{
					this.housingSpace,
					this.housingSpaceTotal
				});
				break;
			default:
				if (type2 == BuildingType.Cantina)
				{
					text = this.lang.Get("MERCENARY_CAPACITY", new object[]
					{
						this.housingSpace,
						this.housingSpaceTotal
					});
				}
				break;
			}
			this.capacityLabel.Text = text;
			this.finishButton.Enabled = enabled;
		}

		private void UpdateTimeLeftSlider(QueuedUnitTrainingTag tag)
		{
			if (this.blockedContract)
			{
				this.itemTimeSliderActive.Value = 1f;
			}
			else
			{
				this.itemTimeSliderActive.Value = 1f - tag.TimeLeftFloat / (float)tag.TimeTotal;
			}
		}

		private void UpdateTimeLeftLabels(QueuedUnitTrainingTag tag)
		{
			if (this.blockedContract)
			{
				this.timeLabel.Text = this.lang.Get("STARPORT_FULL", new object[0]);
				this.itemTimeLabelActive.Text = this.lang.Get("STOPPED", new object[0]);
			}
			else
			{
				this.timeLabel.Text = GameUtils.GetTimeLabelFromSeconds(this.totalSecondsLeft);
				this.itemTimeLabelActive.Text = GameUtils.GetTimeLabelFromSeconds(tag.TimeLeft);
			}
		}

		private void UpdateTrainingLabels()
		{
			bool flag = this.queuedUnits.Count > 0;
			this.instructionsLabel.Text = this.regularInstructions;
			this.finishButton.Visible = (flag && !this.blockedContract);
			int crystals = this.CalculateBuyoutCost();
			string costElementName = (this.deliveryType != DeliveryType.Hero) ? "FinishCost" : "FinishCostHeroes";
			UXUtils.SetupCostElements(this, costElementName, null, 0, 0, 0, crystals, false, null);
			this.timeStatic.Visible = (flag && !this.blockedContract);
			this.timeLabel.Visible = flag;
			this.finishStatic.Visible = flag;
			if (this.deliveryType == DeliveryType.Hero && this.housingSpace == this.housingSpaceTotal)
			{
				this.instructionsLabel.Visible = false;
			}
			else
			{
				this.instructionsLabel.Visible = !flag;
			}
		}

		private int CalculateBuyoutCost()
		{
			int num = 0;
			int i = 0;
			int count = this.queuedUnits.Count;
			while (i < count)
			{
				QueuedUnitTrainingTag queuedUnitTrainingTag = this.queuedUnits[i];
				int num2;
				if (i == 0)
				{
					num2 = queuedUnitTrainingTag.Contracts[0].GetRemainingTimeForSim();
				}
				else
				{
					num2 = queuedUnitTrainingTag.TimeTotal;
				}
				int seconds = num2 + queuedUnitTrainingTag.TimeTotal * (queuedUnitTrainingTag.Contracts.Count - 1);
				num += GameUtils.SecondsToCrystals(seconds);
				i++;
			}
			return num;
		}

		private int CalculateTotalSecondsLeftForView()
		{
			int num = 0;
			int i = 0;
			int count = this.queuedUnits.Count;
			while (i < count)
			{
				QueuedUnitTrainingTag queuedUnitTrainingTag = this.queuedUnits[i];
				int num2;
				if (i == 0)
				{
					num2 = queuedUnitTrainingTag.Contracts[0].GetRemainingTimeForView();
				}
				else
				{
					num2 = queuedUnitTrainingTag.TimeTotal;
				}
				num += num2 + queuedUnitTrainingTag.TimeTotal * (queuedUnitTrainingTag.Contracts.Count - 1);
				i++;
			}
			return num;
		}

		private void AdjustTotalSecondsLeft(int seconds)
		{
			this.totalSecondsLeft += seconds;
			if (this.totalSecondsLeft <= 0)
			{
				this.totalSecondsLeft = 0;
				this.KillTrainingTimers();
			}
			else if (!this.timersRunning)
			{
				this.timersRunning = true;
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
		}

		private void AdjustTrainingSpace(int space)
		{
			this.trainingSpace += space;
			this.UpdateAllTroopButtonStates();
		}

		private void UpdateAllTroopButtonStates()
		{
			int i = 0;
			int count = this.troopItemGrid.Count;
			while (i < count)
			{
				TroopTrainingTag tag = this.troopItemGrid.GetItem(i).Tag as TroopTrainingTag;
				this.UpdateTroopButtonEnabled(tag);
				i++;
			}
		}

		private void UpdateTroopButtonEnabled(TroopTrainingTag tag)
		{
			tag.Dimmer.Visible = (this.trainingSpace + tag.Troop.Size > this.trainingSpaceTotal || !tag.ReqMet);
			int credits = tag.Troop.Credits;
			int materials = tag.Troop.Materials;
			int contraband = tag.Troop.Contraband;
			this.UpdateCostWithAppliedPerks(ref credits, ref materials, ref contraband);
			UXUtils.UpdateCostColor(tag.CostLabel, null, credits, materials, contraband, 0, 0, false);
		}

		private void KillTrainingTimers()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.timersRunning = false;
		}

		private void EnsureTimerKilled(ref uint timer)
		{
			if (timer != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(timer);
				timer = 0u;
			}
		}

		private void CheckTroopContract(bool finishedContract)
		{
			if (this.queuedUnits.Count == 0)
			{
				return;
			}
			QueuedUnitTrainingTag tag = this.queuedUnits[0];
			if (this.blockedContract)
			{
				this.KillTrainingTimers();
				this.RefreshView();
			}
			else if (finishedContract)
			{
				this.DequeueTroop(tag, false);
				this.RefreshView();
			}
		}

		public void OnViewFrameTime(float dt)
		{
			QueuedUnitTrainingTag queuedUnitTrainingTag = null;
			if (this.repositionNextFrame)
			{
				this.RepositionAndScroll();
				this.repositionNextFrame = false;
				this.troopItemGrid.RepositionItems();
			}
			if (this.queuedUnits.Count != 0)
			{
				queuedUnitTrainingTag = this.queuedUnits[0];
				queuedUnitTrainingTag.TimeLeftFloat -= dt;
				this.UpdateTimeLeftSlider(queuedUnitTrainingTag);
			}
			this.accumulatedUpdateDt += dt;
			if (this.accumulatedUpdateDt >= 0.1f)
			{
				int num = this.CalculateTotalSecondsLeftForView();
				if (num < this.totalSecondsLeft)
				{
					this.AdjustTotalSecondsLeft(num - this.totalSecondsLeft);
					this.UpdateTrainingLabels();
					if (queuedUnitTrainingTag != null && queuedUnitTrainingTag.TimeLeft != 0)
					{
						this.UpdateTimeLeftLabels(queuedUnitTrainingTag);
					}
				}
				this.accumulatedUpdateDt = 0f;
			}
		}

		private void OnFinishButtonClicked(UXButton button)
		{
			if (this.totalSecondsLeft <= 0)
			{
				return;
			}
			int crystals = this.CalculateBuyoutCost();
			if (!GameUtils.CanAffordCrystals(crystals))
			{
				GameUtils.SpendCrystals(crystals);
				return;
			}
			this.queuedUnits.Clear();
			this.AdjustTotalSecondsLeft(-this.totalSecondsLeft);
			Service.EventManager.SendEvent(EventId.InitiatedBuyout, null);
			this.supportController.BuyoutAllTroopTrainContracts(this.selectedBuilding);
			this.AdjustTrainingSpace(-this.trainingSpace);
			this.RefreshView();
		}

		private void OnQueueItemButtonClicked(UXButton button)
		{
			this.KillButtonTimers();
			if (button == null)
			{
				return;
			}
			UXElement uXElement = button.Tag as UXElement;
			if (uXElement == null)
			{
				return;
			}
			TroopTrainingTag troopTrainingTag = uXElement.Tag as TroopTrainingTag;
			if (troopTrainingTag != null)
			{
				QueuedUnitTrainingTag queuedItemDataFromUpgradeGroup = this.GetQueuedItemDataFromUpgradeGroup(troopTrainingTag.Troop.UpgradeGroup);
				this.OnQueueItemButtonClicked(queuedItemDataFromUpgradeGroup);
			}
		}

		private void OnQueueItemButtonClicked(QueuedUnitTrainingTag tag)
		{
			if (tag == null)
			{
				Service.Logger.Error("QueuedUnitTrainingTag:tag is empty");
				return;
			}
			if (tag.Contracts == null || tag.Contracts.Count < 1)
			{
				Service.Logger.Error("Contract is empty");
				return;
			}
			string productUid = tag.Contracts[tag.Contracts.Count - 1].ProductUid;
			if (string.IsNullOrEmpty(productUid))
			{
				Service.Logger.Error("productUID is not valid");
				return;
			}
			this.supportController.CancelTroopTrainContract(productUid, this.selectedBuilding);
			this.DequeueTroop(tag, true);
			this.RefreshView();
		}

		private QueuedUnitTrainingTag GetQueuedItemDataFromUpgradeGroup(string upgradeGroup)
		{
			for (int i = 0; i < this.queuedUnits.Count; i++)
			{
				if (this.queuedUnits[i].UnitVO.UpgradeGroup == upgradeGroup)
				{
					return this.queuedUnits[i];
				}
			}
			return null;
		}

		private TroopTrainingTag GetTroopTrainingTagFromUpgradeGroup(string upgradeGroup)
		{
			for (int i = 0; i < this.troopItemGrid.Count; i++)
			{
				TroopTrainingTag troopTrainingTag = this.troopItemGrid.GetItem(i).Tag as TroopTrainingTag;
				if (troopTrainingTag.Troop.UpgradeGroup == upgradeGroup)
				{
					return troopTrainingTag;
				}
			}
			return null;
		}

		private void OnPrevOrNextButtonClicked(UXButton button)
		{
			int num = (int)button.Tag;
			int i = 0;
			int count = this.availableTrainerBuildings.Count;
			while (i < count)
			{
				SmartEntity smartEntity = this.availableTrainerBuildings[i];
				if (smartEntity == this.selectedBuilding)
				{
					SmartEntity selectedBuilding = this.availableTrainerBuildings[(i + num + count) % count];
					this.ResetScreen();
					Service.Engine.ForceGarbageCollection(null);
					this.SetSelectedBuilding(selectedBuilding);
					this.InitScreen();
					this.RefreshView();
					break;
				}
				i++;
			}
		}

		private void OnTroopItemInfoClicked(UXButton button)
		{
			UXElement uXElement = button.Tag as UXElement;
			TroopUpgradeTag troopUpgradeTag = uXElement.Tag as TroopUpgradeTag;
			bool showUpgradeControls = !string.IsNullOrEmpty(troopUpgradeTag.Troop.UpgradeShardUid);
			SmartEntity availableTroopResearchLab = Service.BuildingLookupController.GetAvailableTroopResearchLab();
			List<TroopUpgradeTag> list = new List<TroopUpgradeTag>();
			for (int i = 0; i < this.troopItemGrid.Count; i++)
			{
				list.Add(this.troopItemGrid.GetItem(i).Tag as TroopUpgradeTag);
			}
			Service.ScreenController.AddScreen(new DeployableInfoScreen(troopUpgradeTag, list, showUpgradeControls, availableTroopResearchLab));
		}

		private void OnTroopItemButtonClicked(UXButton button)
		{
			this.KillButtonTimers();
			UXElement uXElement = button.Tag as UXElement;
			TroopTrainingTag tag = uXElement.Tag as TroopTrainingTag;
			this.OnTroopItemButtonClicked(tag);
		}

		private void OnTroopItemButtonClicked(TroopTrainingTag tag)
		{
			if (this.trainingSpace + tag.Troop.Size > this.trainingSpaceTotal)
			{
				string id = string.Empty;
				DeliveryType deliveryType = this.deliveryType;
				switch (deliveryType)
				{
				case DeliveryType.Infantry:
					id = "s_SlotsFullTroopsDesc";
					break;
				case DeliveryType.Vehicle:
					id = "s_SlotsFullVehicleDesc";
					break;
				case DeliveryType.Starship:
					id = "s_SlotsFullShipsDesc";
					break;
				case DeliveryType.Hero:
					id = "s_SlotsFullBody";
					break;
				default:
					if (deliveryType == DeliveryType.Mercenary)
					{
						id = "s_SlotsFullMercenaryDesc";
					}
					break;
				}
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(this.lang.Get(id, new object[0]));
				return;
			}
			if (this.deliveryType != DeliveryType.Starship)
			{
				TroopTypeVO troopTypeVO = tag.Troop as TroopTypeVO;
				if (troopTypeVO.Type == TroopType.Hero)
				{
					if (this.housingSpace + tag.Troop.Size > this.housingSpaceTotal)
					{
						return;
					}
					if (this.GetQueuedItemDataFromUpgradeGroup(tag.Troop.UpgradeGroup) != null)
					{
						return;
					}
					if (GameUtils.GetDeployableCountForUpgradeGroupTroop(troopTypeVO) > 0)
					{
						return;
					}
				}
			}
			string value;
			string value2;
			if (this.deliveryType == DeliveryType.Starship)
			{
				value = StringUtils.ToLowerCaseUnderscoreSeperated((tag.Troop as SpecialAttackTypeVO).SpecialAttackID);
				value2 = (tag.Troop as SpecialAttackTypeVO).SpecialAttackID;
			}
			else
			{
				value = StringUtils.ToLowerCaseUnderscoreSeperated((tag.Troop as TroopTypeVO).Type.ToString());
				value2 = (tag.Troop as TroopTypeVO).TroopID;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value);
			stringBuilder.Append("|");
			stringBuilder.Append(value2);
			stringBuilder.Append("|");
			stringBuilder.Append(tag.Troop.Lvl);
			stringBuilder.Append("|train");
			this.confirmingTraining = tag;
			int credits = tag.Troop.Credits;
			int materials = tag.Troop.Materials;
			int contraband = tag.Troop.Contraband;
			this.UpdateCostWithAppliedPerks(ref credits, ref materials, ref contraband);
			if (PayMeScreen.ShowIfNotEnoughCurrency(credits, materials, contraband, stringBuilder.ToString(), new OnScreenModalResult(this.OnPayMeForCurrencyResult)))
			{
				this.KillButtonTimers();
			}
			else
			{
				this.ConfirmTraining();
			}
		}

		private void UpdateCostWithAppliedPerks(ref int credits, ref int materials, ref int contraband)
		{
			BuildingTypeVO buildingType = this.selectedBuilding.Get<BuildingComponent>().BuildingType;
			float contractCostMultiplier = Service.PerkManager.GetContractCostMultiplier(buildingType);
			GameUtils.MultiplyCurrency(contractCostMultiplier, ref credits, ref materials, ref contraband);
		}

		private void ConfirmTraining()
		{
			TroopTrainingTag troopTrainingTag = this.confirmingTraining;
			this.confirmingTraining = null;
			Contract contract;
			if (this.buildingInfo.Type == BuildingType.FleetCommand)
			{
				contract = this.supportController.StartStarshipMobilization(troopTrainingTag.Troop as SpecialAttackTypeVO, this.selectedBuilding);
			}
			else if (this.buildingInfo.Type == BuildingType.HeroMobilizer)
			{
				contract = this.supportController.StartHeroMobilization(troopTrainingTag.Troop as TroopTypeVO, this.selectedBuilding);
			}
			else
			{
				contract = this.supportController.StartTroopTrainContract(troopTrainingTag.Troop as TroopTypeVO, this.selectedBuilding);
			}
			this.HandleContract(contract, contract.GetRemainingTimeForView());
			if (this.queuedUnits.Count == 1)
			{
				this.blockedContract = !this.supportController.IsContractValidForStorage(contract);
			}
			this.RefreshView();
		}

		private void OnPayMeForCurrencyResult(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				this.ConfirmTraining();
			}
			else
			{
				this.confirmingTraining = null;
			}
		}

		private void OnQueueItemButtonPressed(UXButton button)
		{
			UXElement uXElement = button.Tag as UXElement;
			if (uXElement != null)
			{
				TroopTrainingTag troopTrainingTag = uXElement.Tag as TroopTrainingTag;
				if (troopTrainingTag != null)
				{
					troopTrainingTag.AutoQueuing = false;
					this.OnItemButtonPressed(troopTrainingTag);
				}
			}
		}

		private void OnTroopItemButtonPressed(UXButton button)
		{
			UXElement uXElement = button.Tag as UXElement;
			TroopTrainingTag troopTrainingTag = uXElement.Tag as TroopTrainingTag;
			if (!troopTrainingTag.ReqMet)
			{
				return;
			}
			troopTrainingTag.AutoQueuing = true;
			this.OnItemButtonPressed(troopTrainingTag);
			troopTrainingTag.TroopButton.OnClicked = new UXButtonClickedDelegate(this.OnTroopItemButtonClicked);
		}

		private void OnItemButtonPressed(TroopTrainingTag tag)
		{
			if (this.buttonDelayTimer == 0u && this.buttonRepeatTimer == 0u)
			{
				this.buttonDelayTimer = Service.ViewTimerManager.CreateViewTimer(0.4f, false, new TimerDelegate(this.OnButtonDelayTimer), tag);
			}
		}

		private void OnItemButtonReleased(UXButton button)
		{
			this.KillButtonTimers();
		}

		private void OnGridDrag(AbstractUXList grid)
		{
			this.KillButtonTimers();
		}

		private void KillButtonTimers()
		{
			this.EnsureTimerKilled(ref this.buttonDelayTimer);
			this.EnsureTimerKilled(ref this.buttonRepeatTimer);
		}

		private void OnButtonDelayTimer(uint id, object cookie)
		{
			if (id == this.buttonDelayTimer)
			{
				this.KillButtonTimers();
				this.buttonRepeatTimer = Service.ViewTimerManager.CreateViewTimer(0.1f, true, new TimerDelegate(this.OnButtonRepeatTimer), cookie);
			}
		}

		private void OnButtonRepeatTimer(uint id, object cookie)
		{
			TroopTrainingTag troopTrainingTag = cookie as TroopTrainingTag;
			if (troopTrainingTag.AutoQueuing)
			{
				this.OnTroopItemButtonClicked(troopTrainingTag);
				troopTrainingTag.TroopButton.OnClicked = null;
			}
			else
			{
				this.OnQueueItemButtonClicked(this.GetQueuedItemDataFromUpgradeGroup(troopTrainingTag.Troop.UpgradeGroup));
			}
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.TroopRecruited:
			case EventId.StarshipMobilized:
			case EventId.HeroMobilized:
				if ((cookie as ContractEventData).Entity == this.selectedBuilding)
				{
					this.CheckTroopContract(true);
				}
				goto IL_AD;
			case EventId.TroopLoadingIntoStarport:
				IL_19:
				switch (id)
				{
				case EventId.InventoryTroopUpdated:
				case EventId.InventorySpecialAttackUpdated:
				case EventId.InventoryHeroUpdated:
					this.UpdateHousingSpace();
					goto IL_AD;
				default:
					if (id == EventId.ContractInvalidForStorage)
					{
						if ((cookie as ContractEventData).Entity == this.selectedBuilding)
						{
							this.blockedContract = true;
							this.CheckTroopContract(false);
						}
						goto IL_AD;
					}
					if (id != EventId.DenyUserInput)
					{
						goto IL_AD;
					}
					this.KillButtonTimers();
					goto IL_AD;
				}
				break;
			}
			goto IL_19;
			IL_AD:
			return base.OnEvent(id, cookie);
		}
	}
}
