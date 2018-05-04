using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TroopUpgradeScreen : SelectedBuildingScreen, IViewFrameTimeObserver
	{
		private class DeployableUpgradeData
		{
			public IDeployableVO UpgradeType;

			public IDeployableVO CurrentType;
		}

		private const string RESEARCH_LAB_TROOPS_TAB = "RESEARCH_LAB_TROOPS_TAB";

		private const string RESEARCH_LAB_EQUIPMENT_TAB = "RESEARCH_LAB_EQUIPMENT_TAB";

		private const string UPGRADE_UPGRADE_TROOPS = "UPGRADE_UPGRADE_TROOPS";

		private const string UPGRADE_EQUIPMENT = "UPGRADE_EQUIPMENT";

		private const string OBJECTIVE_PROGRESS = "OBJECTIVE_PROGRESS";

		private const string BUILDING_REQUIREMENT = "BUILDING_REQUIREMENT";

		private const string EQUIPMENT_TAB_PREFIX = "EQUIPMENT_TAB_";

		private const string RESEARCH_EQUIPMENT_TAB_NEW = "RESEARCH_EQUIPMENT_TAB_NEW";

		private const string EQUIPMENT_RESEARCH_LOCKED = "EQUIPMENT_RESEARCH_LOCKED";

		private const string TROOP_TAB_ALL = "TROOP_TAB_ALL";

		private const string TROOP_TAB_HEROES = "TROOP_TAB_HEROES";

		private const string TROOP_TAB_INFANTRY = "TROOP_TAB_INFANTRY";

		private const string TROOP_TAB_VEHICLES = "TROOP_TAB_VEHICLES";

		private const string TROOP_TAB_STARSHIPS = "TROOP_TAB_STARSHIPS";

		private const string TROOP_TAB_MERCENARIES = "TROOP_TAB_MERCENARIES";

		private const string MAX_LEVEL = "MAX_LEVEL";

		private const string LABEL_TITLE = "LabelSelectTroop";

		private const string TROOP_GRID = "TroopCapacityGrid";

		private const string EQUIPMENT_GRID = "EquipmentGrid";

		private const string EQUIPMENT_ITEM_TEMPLATE = "EquipmentItemTemplate";

		private const string TROOP_BADGE = "ContainerJewel1";

		private const string TROOP_BADGE_LABEL = "LabelMessage1";

		private const string TROOP_CARD_DEFAULT = "CardDefault";

		private const string TROOP_CARD_QUALITY = "CardQuality";

		private const string TROOP_CARD_Q_10 = "CardQ10";

		private const string TROOP_ITEM_TEMPLATE = "TroopItemTemplate";

		private const string TROOP_ITEM_ICON = "SpriteTroopItemImage";

		private const string TROOP_ITEM_LABEL_REQ = "LabelRequirement";

		private const string TROOP_ITEM_LABEL_LEVEL = "LabelTroopLevel";

		private const string TROOP_ITEM_BUTTON = "ButtonTroopItemCard";

		private const string TROOP_ITEM_DIMMER = "SpriteDim";

		private const string TROOP_ITEM_SPRITE_LOCKED = "SpriteIconLockedTroop";

		private const string TROOP_ITEM_SLIDER = "pBarUpgradeTime";

		private const string TROOP_ITEM_LABEL_UPGRADE_TIME = "LabelpBarUpgradeTime";

		private const string TROOP_QUALITY_POSTFIX = "Quality";

		private const string TROOP_SHARD_PROGRESS_BAR = "pBarQuality";

		private const string TROOP_SHARD_PROGRESS_BAR_SPRITE = "SpritepBarQuality";

		private const string TROOP_SHARD_NUMBER_LABEL = "LabelNumberQuality";

		private const string TROOP_SHARD_UPGRADE_ICON = "IconUpgradeQuality";

		private const string STRING_FRACTION = "FRACTION";

		private const string TROOP_FRAGMENT_ICON = "SpriteFragmentTroop";

		private const string LABEL_EQUIPMENT_REQUIREMENT = "LabelEquipmentRequirement";

		private const string LABEL_EQUIPMENT_NAME = "LabelEquipmentName";

		private const string LABEL_EQUIPMENT_LEVEL = "LabelEquipmentLevel";

		private const string BTN_EQUIPMENT_ITEM_CARD = "BtnEquipmentItemCard";

		private const string SPRITE_EQUIPMENT_ITEM_IMAGE = "SpriteEquipmentItemImage";

		private const string LABEL_EQUIPMENT_NUMBER = "LabelEquipmentNumber";

		private const string EQUIPMENT_SPRITE_DIM = "SpriteDim";

		private const string EQUIPMENT_ITEM_SPRITE_LOCKED = "SpriteIconLockedEquip";

		private const string EQUIPMENT_CARD = "EquipmentItemCardQ{0}";

		private const string SHARD_PROGRESS_BAR = "pBarFrag";

		private const string SHARD_PROGRESS_BAR_SPRITE = "SpritepBarFrag";

		private const string EQUIPMENT_BADGE = "ContainerJewel";

		private const string EQUIPMENT_BADGE_LABEL = "LabelMessage";

		private const string EQUIPMENT_ITEM_SLIDER = "pBarUpgradeTimeEquipment";

		private const string EQUIPMENT_ITEM_LABEL_UPGRADE_TIME = "LabelpBarUpgradeTimeEquipment";

		private const string ICON_UPGRADE = "IconUpgrade";

		private const string EQUIPMENT_FRAG_ICON = "SpriteFragmentEquip";

		private const string SPACER = "spacer";

		private const string TAB_BTN1 = "Btn1";

		private const string TAB_LABEL_BTN1 = "LabelBtn1";

		private const string TAB_BTN2 = "Btn2";

		private const string TAB_LABEL_BTN2 = "LabelBtn2";

		private const string TAB_BTN2_DIM = "Btn2Dim";

		private const string TAB_LABEL_BTN2_DIM = "LabelBtn2Dim";

		private const int TROOP_ROWS_COUNT = 2;

		private const int EQUIPMENT_ROWS_COUNT = 2;

		private UXGrid troopGrid;

		private UXGrid equipmentGrid;

		private UXCheckbox troopTabButton;

		private UXLabel troopTabLabel;

		private UXCheckbox equipmentTabButton;

		private UXLabel equipmentTabLabel;

		private UXButton equipmentTabButtonDim;

		private UXLabel equipmentTabLabelDim;

		private TroopUpgradeScreenMode researchMode;

		private TroopUpgradeCatalog troopUpgradeCatalog;

		private StarshipUpgradeCatalog starshipUpgradeCatalog;

		private Contract activeContract;

		private float accumulatedUpdateDt;

		private bool timerActive;

		private bool contractHidden;

		private AbstractTabHelper filterHelper;

		private List<TroopUpgradeScreen.DeployableUpgradeData> eligibleDeployables;

		private HashSet<TroopType> eligibleTroopTypes;

		private bool hasStarships;

		private List<EquipmentVO> equipmentToDisplay;

		private StaticDataController dataController;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public TroopUpgradeScreen(Entity trainingBuilding) : base("gui_upgrade_troops", trainingBuilding)
		{
			this.dataController = Service.StaticDataController;
			this.troopUpgradeCatalog = Service.TroopUpgradeCatalog;
			this.starshipUpgradeCatalog = Service.StarshipUpgradeCatalog;
			this.accumulatedUpdateDt = 0f;
			this.timerActive = false;
			this.eligibleDeployables = new List<TroopUpgradeScreen.DeployableUpgradeData>();
			this.eligibleTroopTypes = new HashSet<TroopType>();
			this.hasStarships = false;
			this.equipmentToDisplay = new List<EquipmentVO>();
			this.RefreshDeployableData();
		}

		protected override void OnScreenLoaded()
		{
			if (this.selectedBuilding == null)
			{
				base.DestroyScreen();
				return;
			}
			this.researchMode = TroopUpgradeScreenMode.Troops;
			base.GetElement<UXLabel>("LabelSelectTroop").Text = this.lang.Get("UPGRADE_UPGRADE_TROOPS", new object[0]);
			this.troopGrid = base.GetElement<UXGrid>("TroopCapacityGrid");
			this.troopGrid.SetTemplateItem("TroopItemTemplate");
			this.equipmentGrid = base.GetElement<UXGrid>("EquipmentGrid");
			this.equipmentGrid.SetTemplateItem("EquipmentItemTemplate");
			this.equipmentGrid.DupeOrdersAllowed = true;
			this.InitButtons();
			this.troopTabButton = base.GetElement<UXCheckbox>("Btn1");
			this.troopTabButton.OnSelected = new UXCheckboxSelectedDelegate(this.OnTroopsTabClicked);
			this.troopTabLabel = base.GetElement<UXLabel>("LabelBtn1");
			this.troopTabLabel.Text = this.lang.Get("RESEARCH_LAB_TROOPS_TAB", new object[0]);
			this.equipmentTabButton = base.GetElement<UXCheckbox>("Btn2");
			this.equipmentTabButton.OnSelected = new UXCheckboxSelectedDelegate(this.OnEquipmentTabClicked);
			this.equipmentTabLabel = base.GetElement<UXLabel>("LabelBtn2");
			this.equipmentTabLabel.Text = this.lang.Get("RESEARCH_LAB_EQUIPMENT_TAB", new object[0]);
			this.equipmentTabButtonDim = base.GetElement<UXButton>("Btn2Dim");
			this.equipmentTabButtonDim.OnClicked = new UXButtonClickedDelegate(this.OnEquipmentTabDimClicked);
			this.equipmentTabLabelDim = base.GetElement<UXLabel>("LabelBtn2Dim");
			this.equipmentTabLabelDim.Text = this.lang.Get("RESEARCH_LAB_EQUIPMENT_TAB", new object[0]);
			if (ArmoryUtils.PlayerHasArmory())
			{
				this.equipmentTabButton.Enabled = true;
				this.equipmentTabButton.Visible = true;
				this.equipmentTabButtonDim.Enabled = false;
				this.equipmentTabButtonDim.Visible = false;
			}
			else
			{
				this.equipmentTabButton.Enabled = false;
				this.equipmentTabButton.Visible = false;
				this.equipmentTabButtonDim.Enabled = true;
				this.equipmentTabButtonDim.Visible = true;
			}
			base.GetElement<UXElement>("ContainerJewel1").Visible = false;
			base.GetElement<UXLabel>("LabelMessage1").Text = this.lang.Get("RESEARCH_EQUIPMENT_TAB_NEW", new object[0]);
			ArmoryController armoryController = Service.ArmoryController;
			bool visible = armoryController.AllowShowEquipmentTabBadge && armoryController.DoesUserHaveAnyUpgradableEquipment();
			base.GetElement<UXElement>("ContainerJewel").Visible = visible;
			base.GetElement<UXLabel>("LabelMessage").Text = this.lang.Get("RESEARCH_EQUIPMENT_TAB_NEW", new object[0]);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ContractCompleted);
			eventManager.RegisterObserver(this, EventId.InventoryUnlockUpdated);
			this.RefreshFilterTabs();
			this.RefreshGrids();
			armoryController.AllowShowEquipmentTabBadge = false;
			Service.DeployableShardUnlockController.AllowResearchBuildingBadging = false;
			eventManager.SendEvent(EventId.TroopUpgradeScreenOpened, null);
		}

		private void OnTroopsTabClicked(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			this.SetResearchMode(TroopUpgradeScreenMode.Troops);
		}

		private void OnEquipmentTabClicked(UXCheckbox checkbox, bool selected)
		{
			if (!selected)
			{
				return;
			}
			base.GetElement<UXElement>("ContainerJewel").Visible = false;
			this.SetResearchMode(TroopUpgradeScreenMode.Equipment);
		}

		private void OnEquipmentTabDimClicked(UXButton button)
		{
			MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
			miscElementsManager.ShowPlayerInstructionsError(this.lang.Get("EQUIPMENT_RESEARCH_LOCKED", new object[0]));
		}

		private void SetResearchMode(TroopUpgradeScreenMode researchMode)
		{
			this.researchMode = researchMode;
			base.GetElement<UXLabel>("LabelSelectTroop").Text = ((researchMode != TroopUpgradeScreenMode.Troops) ? this.lang.Get("UPGRADE_EQUIPMENT", new object[0]) : this.lang.Get("UPGRADE_UPGRADE_TROOPS", new object[0]));
			this.RefreshFilterTabs();
			this.RefreshGrids();
		}

		protected override void SetupPerksButton()
		{
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ContractCompleted)
			{
				if (id == EventId.InventoryUnlockUpdated)
				{
					this.RefreshGrids();
				}
			}
			else
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				ContractType contractType = contractEventData.Contract.ContractTO.ContractType;
				if (contractType == ContractType.Build || contractType == ContractType.Research || contractType == ContractType.Upgrade)
				{
					this.RefreshDeployableData();
					this.RefreshGrids();
				}
			}
			return base.OnEvent(id, cookie);
		}

		public void OnViewFrameTime(float dt)
		{
			this.accumulatedUpdateDt += dt;
			if (this.accumulatedUpdateDt >= 0.1f)
			{
				this.UpdateContractTimers();
				this.accumulatedUpdateDt = 0f;
			}
		}

		public void RefreshGrids()
		{
			this.equipmentGrid.Clear();
			this.troopGrid.Clear();
			Service.Engine.ForceGarbageCollection(null);
			TroopUpgradeScreenMode troopUpgradeScreenMode = this.researchMode;
			if (troopUpgradeScreenMode != TroopUpgradeScreenMode.Troops)
			{
				if (troopUpgradeScreenMode == TroopUpgradeScreenMode.Equipment)
				{
					this.PopulateEquipmentGrid();
				}
			}
			else
			{
				this.PopulateTroopGrid();
			}
		}

		private void DisableTimers()
		{
			this.timerActive = false;
			this.activeContract = null;
			this.accumulatedUpdateDt = 0f;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		private void RefreshFilterTabs()
		{
			if (this.filterHelper != null)
			{
				this.filterHelper.Destroy();
				this.filterHelper = null;
			}
			this.troopTabLabel.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			this.equipmentTabLabel.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
			TroopUpgradeScreenMode troopUpgradeScreenMode = this.researchMode;
			if (troopUpgradeScreenMode != TroopUpgradeScreenMode.Troops)
			{
				if (troopUpgradeScreenMode == TroopUpgradeScreenMode.Equipment)
				{
					this.equipmentTabLabel.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
					this.SetFilterToEquipment();
				}
			}
			else
			{
				this.troopTabLabel.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
				this.SetFilterToTroops();
			}
		}

		private void SetFilterToTroops()
		{
			Dictionary<TroopTab, string> dictionary = new Dictionary<TroopTab, string>();
			dictionary.Add(TroopTab.All, this.lang.Get("TROOP_TAB_ALL", new object[0]));
			if (this.eligibleTroopTypes.Contains(TroopType.Hero))
			{
				dictionary.Add(TroopTab.Hero, this.lang.Get("TROOP_TAB_HEROES", new object[0]));
			}
			if (this.eligibleTroopTypes.Contains(TroopType.Infantry))
			{
				dictionary.Add(TroopTab.Infantry, this.lang.Get("TROOP_TAB_INFANTRY", new object[0]));
			}
			if (this.eligibleTroopTypes.Contains(TroopType.Vehicle))
			{
				dictionary.Add(TroopTab.Vehicle, this.lang.Get("TROOP_TAB_VEHICLES", new object[0]));
			}
			if (this.hasStarships)
			{
				dictionary.Add(TroopTab.Starship, this.lang.Get("TROOP_TAB_STARSHIPS", new object[0]));
			}
			if (this.eligibleTroopTypes.Contains(TroopType.Mercenary))
			{
				dictionary.Add(TroopTab.Mercenary, this.lang.Get("TROOP_TAB_MERCENARIES", new object[0]));
			}
			this.filterHelper = new TroopTabHelper();
			Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
			foreach (KeyValuePair<TroopTab, string> current in dictionary)
			{
				dictionary2[(int)current.Key] = current.Value;
			}
			this.filterHelper.CreateTabs(this, new Action(this.RefreshGrids), dictionary2, 0);
		}

		private void SetFilterToEquipment()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			IEnumerator enumerator = Enum.GetValues(typeof(EquipmentTab)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					EquipmentTab equipmentTab = (EquipmentTab)((int)enumerator.Current);
					string text = equipmentTab.ToString();
					StringBuilder stringBuilder = new StringBuilder("EQUIPMENT_TAB_");
					stringBuilder.Append(text.ToUpper());
					dictionary.Add((int)equipmentTab, this.lang.Get(stringBuilder.ToString(), new object[0]));
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			this.filterHelper = new EquipmentTabHelper();
			this.filterHelper.CreateTabs(this, new Action(this.RefreshGrids), dictionary, 0);
		}

		private void RefreshDeployableData()
		{
			this.eligibleDeployables.Clear();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			foreach (string current in this.troopUpgradeCatalog.AllUpgradeGroups())
			{
				int nextLevel = currentPlayer.UnlockedLevels.Troops.GetNextLevel(current);
				TroopTypeVO troopTypeVO = this.troopUpgradeCatalog.GetByLevel(current, nextLevel);
				TroopTypeVO byLevel = this.troopUpgradeCatalog.GetByLevel(current, nextLevel - 1);
				if (troopTypeVO == null)
				{
					troopTypeVO = this.troopUpgradeCatalog.GetMaxLevel(current);
				}
				if (troopTypeVO.PlayerFacing || (byLevel != null && byLevel.PlayerFacing))
				{
					if (troopTypeVO.Type != TroopType.Champion)
					{
						if (troopTypeVO.Faction == this.buildingInfo.Faction)
						{
							TroopUpgradeScreen.DeployableUpgradeData deployableUpgradeData = new TroopUpgradeScreen.DeployableUpgradeData();
							deployableUpgradeData.UpgradeType = troopTypeVO;
							deployableUpgradeData.CurrentType = byLevel;
							this.eligibleDeployables.Add(deployableUpgradeData);
							if (!this.eligibleTroopTypes.Contains(troopTypeVO.Type))
							{
								this.eligibleTroopTypes.Add(troopTypeVO.Type);
							}
						}
					}
				}
			}
			foreach (string current2 in this.starshipUpgradeCatalog.AllUpgradeGroups())
			{
				int nextLevel2 = currentPlayer.UnlockedLevels.Starships.GetNextLevel(current2);
				SpecialAttackTypeVO specialAttackTypeVO = this.starshipUpgradeCatalog.GetByLevel(current2, nextLevel2);
				SpecialAttackTypeVO byLevel2 = this.starshipUpgradeCatalog.GetByLevel(current2, nextLevel2 - 1);
				if (specialAttackTypeVO == null)
				{
					specialAttackTypeVO = this.starshipUpgradeCatalog.GetMaxLevel(current2);
				}
				if (specialAttackTypeVO.PlayerFacing || (byLevel2 != null && byLevel2.PlayerFacing))
				{
					if (specialAttackTypeVO.Faction == this.buildingInfo.Faction)
					{
						TroopUpgradeScreen.DeployableUpgradeData deployableUpgradeData2 = new TroopUpgradeScreen.DeployableUpgradeData();
						deployableUpgradeData2.UpgradeType = specialAttackTypeVO;
						deployableUpgradeData2.CurrentType = byLevel2;
						this.eligibleDeployables.Add(deployableUpgradeData2);
						this.hasStarships = true;
					}
				}
			}
		}

		private void PopulateTroopGrid()
		{
			List<TroopUpgradeScreen.DeployableUpgradeData> list = new List<TroopUpgradeScreen.DeployableUpgradeData>();
			this.contractHidden = (this.activeContract != null);
			string a = string.Empty;
			int i = 0;
			int count = this.eligibleDeployables.Count;
			while (i < count)
			{
				TroopUpgradeScreen.DeployableUpgradeData deployableUpgradeData = this.eligibleDeployables[i];
				TroopTab currentTab = (TroopTab)this.filterHelper.CurrentTab;
				if (deployableUpgradeData.CurrentType is SpecialAttackTypeVO)
				{
					if (currentTab == TroopTab.All || currentTab == TroopTab.Starship)
					{
						if (this.contractHidden)
						{
							SpecialAttackTypeVO nextLevel = Service.StarshipUpgradeCatalog.GetNextLevel((SpecialAttackTypeVO)deployableUpgradeData.CurrentType);
							if (nextLevel != null && nextLevel.PlayerFacing)
							{
								a = nextLevel.Uid;
							}
						}
						goto IL_13F;
					}
				}
				else if (currentTab != TroopTab.Starship)
				{
					if (currentTab != TroopTab.All)
					{
						TroopType type = ((TroopTypeVO)deployableUpgradeData.CurrentType).Type;
						TroopTabHelper troopTabHelper = this.filterHelper as TroopTabHelper;
						TroopTab troopTab = troopTabHelper.ConvertTroopTypeToTab(type);
						if (currentTab != troopTab)
						{
							goto IL_16F;
						}
					}
					if (!this.contractHidden)
					{
						goto IL_13F;
					}
					TroopTypeVO nextLevel2 = Service.TroopUpgradeCatalog.GetNextLevel((TroopTypeVO)deployableUpgradeData.CurrentType);
					if (nextLevel2 != null && nextLevel2.PlayerFacing)
					{
						a = nextLevel2.Uid;
						goto IL_13F;
					}
					goto IL_13F;
				}
				IL_16F:
				i++;
				continue;
				IL_13F:
				if (this.contractHidden && a == this.activeContract.ProductUid)
				{
					this.contractHidden = false;
				}
				list.Add(deployableUpgradeData);
				goto IL_16F;
			}
			list.Sort(new Comparison<TroopUpgradeScreen.DeployableUpgradeData>(this.CompareUpgradeDataBySortOrder));
			int num = 2;
			int count2 = list.Count;
			int num2 = (count2 % num != 0) ? (count2 / num + 1) : (count2 / num);
			this.troopGrid.MaxItemsPerLine = num2;
			if (count2 % num == 1)
			{
				int sortOrder = 2 * num2 - 1;
				this.AddSpacerElement(sortOrder, this.troopGrid);
			}
			int j = 0;
			int count3 = list.Count;
			while (j < count3)
			{
				int num3 = j % num;
				int sortOrder2 = num3 * num2 + j / num;
				TroopUpgradeScreen.DeployableUpgradeData deployableUpgradeData2 = list[j];
				this.AddUpgradeableItemToTroopGrid(deployableUpgradeData2.UpgradeType, deployableUpgradeData2.CurrentType, sortOrder2);
				j++;
			}
			this.troopGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.FinishTroopGridSetup), 1);
			this.CheckActiveContract();
		}

		private void FindAllUpgradableEquipment(CurrentPlayer player, EquipmentUpgradeCatalog catalog)
		{
			this.equipmentToDisplay.Clear();
			foreach (string current in catalog.AllUpgradeGroups())
			{
				int level = player.UnlockedLevels.Equipment.GetLevel(current);
				EquipmentVO equipmentVO = catalog.GetByLevel(current, level);
				if (equipmentVO == null)
				{
					equipmentVO = catalog.GetMaxLevel(current);
				}
				if (equipmentVO.PlayerFacing)
				{
					if (equipmentVO.Faction == player.Faction)
					{
						this.equipmentToDisplay.Add(equipmentVO);
					}
				}
			}
			this.equipmentToDisplay.Sort(new Comparison<EquipmentVO>(this.CompareBySortOrder));
		}

		private UXElement FetchEquipmentCardGridItem(CurrentPlayer player, EquipmentVO equipmentVO, EquipmentUpgradeCatalog catalog)
		{
			UXElement result = this.equipmentGrid.CloneTemplateItem(equipmentVO.Uid);
			string uid = equipmentVO.Uid;
			EquipmentVO nextLevel = catalog.GetNextLevel(equipmentVO);
			ShardQuality quality = equipmentVO.Quality;
			UXUtils.SetCardQuality(this, this.equipmentGrid, uid, (int)quality, "EquipmentItemCardQ{0}");
			UXLabel subElement = this.equipmentGrid.GetSubElement<UXLabel>(uid, "LabelEquipmentName");
			subElement.Text = this.lang.Get(equipmentVO.EquipmentName, new object[0]);
			UXLabel subElement2 = this.equipmentGrid.GetSubElement<UXLabel>(uid, "LabelEquipmentLevel");
			subElement2.Text = LangUtils.GetLevelText(equipmentVO.Lvl);
			UXButton subElement3 = this.equipmentGrid.GetSubElement<UXButton>(uid, "BtnEquipmentItemCard");
			subElement3.Tag = equipmentVO;
			subElement3.OnClicked = new UXButtonClickedDelegate(this.OnEquipmentButtonClicked);
			this.troopGrid.GetSubElement<UXSlider>(uid, "pBarUpgradeTimeEquipment").Visible = false;
			this.troopGrid.GetSubElement<UXLabel>(uid, "LabelpBarUpgradeTimeEquipment").Visible = false;
			UXSprite subElement4 = this.equipmentGrid.GetSubElement<UXSprite>(uid, "SpriteFragmentEquip");
			UXUtils.SetupFragmentIconSprite(subElement4, (int)quality);
			UXSprite subElement5 = this.equipmentGrid.GetSubElement<UXSprite>(uid, "SpriteEquipmentItemImage");
			ProjectorConfig config = ProjectorUtils.GenerateEquipmentConfig(equipmentVO, subElement5, true);
			ProjectorUtils.GenerateProjector(config);
			UXLabel subElement6 = this.equipmentGrid.GetSubElement<UXLabel>(uid, "LabelEquipmentNumber");
			UXSlider subElement7 = this.equipmentGrid.GetSubElement<UXSlider>(uid, "pBarFrag");
			UXSprite subElement8 = this.equipmentGrid.GetSubElement<UXSprite>(uid, "SpritepBarFrag");
			subElement8.Color = UXUtils.COLOR_SHARD_INPROGRESS;
			UXLabel subElement9 = this.equipmentGrid.GetSubElement<UXLabel>(uid, "LabelEquipmentRequirement");
			UXSprite subElement10 = this.equipmentGrid.GetSubElement<UXSprite>(uid, "SpriteDim");
			UXSprite subElement11 = this.equipmentGrid.GetSubElement<UXSprite>(uid, "SpriteIconLockedEquip");
			UXElement subElement12 = this.equipmentGrid.GetSubElement<UXElement>(uid, "IconUpgrade");
			if (!player.UnlockedLevels.Equipment.Has(equipmentVO))
			{
				subElement9.Text = LangUtils.GetShardLockedEquipmentString(equipmentVO);
				subElement10.Visible = true;
				subElement11.Visible = true;
				subElement2.Visible = false;
				int shards = player.GetShards(equipmentVO.EquipmentID);
				subElement6.Text = this.lang.Get("OBJECTIVE_PROGRESS", new object[]
				{
					shards,
					equipmentVO.UpgradeShards
				});
				subElement7.Value = this.CalculateProgress(shards, equipmentVO);
				subElement12.Visible = false;
			}
			else if (nextLevel == null)
			{
				subElement6.Visible = false;
				subElement10.Visible = true;
				subElement11.Visible = false;
				subElement6.Visible = false;
				subElement9.Text = this.lang.Get("MAX_LEVEL", new object[0]);
				subElement7.Visible = false;
				subElement12.Visible = false;
			}
			else
			{
				int shards2 = player.GetShards(equipmentVO.EquipmentID);
				subElement6.Text = this.lang.Get("OBJECTIVE_PROGRESS", new object[]
				{
					shards2,
					nextLevel.UpgradeShards
				});
				subElement7.Value = this.CalculateProgress(shards2, nextLevel);
				subElement12.Visible = (shards2 >= nextLevel.UpgradeShards);
				if (subElement12.Visible)
				{
					subElement8.Color = UXUtils.COLOR_SHARD_COMPLETE;
				}
				if (ArmoryUtils.IsBuildingRequirementMet(nextLevel))
				{
					subElement9.Visible = false;
					subElement10.Visible = false;
					subElement11.Visible = false;
				}
				else
				{
					BuildingTypeVO buildingInfo = this.dataController.Get<BuildingTypeVO>(nextLevel.BuildingRequirement);
					subElement9.Text = this.lang.Get("BUILDING_REQUIREMENT", new object[]
					{
						nextLevel.Lvl,
						LangUtils.GetBuildingDisplayName(buildingInfo)
					});
					subElement10.Visible = true;
					subElement11.Visible = true;
				}
			}
			return result;
		}

		private void PopulateEquipmentGrid()
		{
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.FindAllUpgradableEquipment(currentPlayer, equipmentUpgradeCatalog);
			int maxItemsPerLine = (this.equipmentToDisplay.Count + 1) / 2;
			this.equipmentGrid.MaxItemsPerLine = maxItemsPerLine;
			EquipmentTabHelper equipmentTabHelper = this.filterHelper as EquipmentTabHelper;
			EquipmentTab currentTab = (EquipmentTab)equipmentTabHelper.CurrentTab;
			List<UXElement> list = new List<UXElement>();
			int i = 0;
			int count = this.equipmentToDisplay.Count;
			while (i < count)
			{
				EquipmentVO equipmentVO = this.equipmentToDisplay[i];
				if (equipmentTabHelper.IsEquipmentValidForTab(equipmentVO, currentTab))
				{
					if (this.contractHidden)
					{
						EquipmentVO nextLevel = equipmentUpgradeCatalog.GetNextLevel(equipmentVO);
						if (nextLevel.Uid == this.activeContract.ProductUid)
						{
							this.contractHidden = false;
						}
					}
					UXElement item = this.FetchEquipmentCardGridItem(currentPlayer, equipmentVO, equipmentUpgradeCatalog);
					list.Add(item);
				}
				i++;
			}
			UXUtils.SortListForTwoRowGrids(list, this.equipmentGrid);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				this.equipmentGrid.AddItem(list[j], j);
				j++;
			}
			list.Clear();
			this.equipmentGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.FinishEquipmentGridSetup), 1);
			this.CheckActiveContract();
		}

		private float CalculateProgress(int currentShards, EquipmentVO equipment)
		{
			int upgradeShards = equipment.UpgradeShards;
			if (upgradeShards == 0)
			{
				Service.Logger.ErrorFormat("CMS Error: Shards required for {0} is zero", new object[]
				{
					equipment.Uid
				});
				return 0f;
			}
			float num = (float)currentShards / (float)upgradeShards;
			return (num <= 1f) ? num : 1f;
		}

		private void OnEquipmentButtonClicked(UXButton button)
		{
			EquipmentVO selectedEquipment = button.Tag as EquipmentVO;
			Service.ScreenController.AddScreen(new EquipmentInfoScreen(selectedEquipment, this.equipmentToDisplay, this.selectedBuilding, true, false));
		}

		private void FinishTroopGridSetup(AbstractUXList list)
		{
			this.troopGrid.Scroll(0f);
		}

		private void FinishEquipmentGridSetup(AbstractUXList list)
		{
			this.equipmentGrid.Scroll(0f);
		}

		private void CheckActiveContract()
		{
			BuildingComponent buildingComponent = this.selectedBuilding.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			if (contract != null)
			{
				this.activeContract = contract;
				if (!this.timerActive)
				{
					this.timerActive = true;
					Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				}
			}
			else
			{
				this.activeContract = null;
				this.DisableTimers();
			}
		}

		private void UpdateContractTimers()
		{
			if (this.activeContract == null || this.contractHidden)
			{
				return;
			}
			int remainingTimeForView = this.activeContract.GetRemainingTimeForView();
			if (remainingTimeForView > 0)
			{
				UXSlider subElement;
				UXLabel subElement2;
				if (this.activeContract.DeliveryType == DeliveryType.UpgradeEquipment)
				{
					if (this.researchMode != TroopUpgradeScreenMode.Equipment)
					{
						return;
					}
					EquipmentVO vo = this.dataController.Get<EquipmentVO>(this.activeContract.ProductUid);
					EquipmentVO prevLevel = Service.EquipmentUpgradeCatalog.GetPrevLevel(vo);
					EquipmentTabHelper equipmentTabHelper = this.filterHelper as EquipmentTabHelper;
					if (!equipmentTabHelper.IsEquipmentValidForTab(vo, (EquipmentTab)equipmentTabHelper.CurrentTab))
					{
						return;
					}
					subElement = this.equipmentGrid.GetSubElement<UXSlider>(prevLevel.Uid, "pBarUpgradeTimeEquipment");
					subElement2 = this.equipmentGrid.GetSubElement<UXLabel>(prevLevel.Uid, "LabelpBarUpgradeTimeEquipment");
					this.equipmentGrid.GetSubElement<UXLabel>(prevLevel.Uid, "LabelEquipmentName").Visible = false;
					this.equipmentGrid.GetSubElement<UXLabel>(prevLevel.Uid, "LabelEquipmentNumber").Visible = false;
					this.equipmentGrid.GetSubElement<UXSlider>(prevLevel.Uid, "pBarFrag").Visible = false;
					this.equipmentGrid.GetSubElement<UXElement>(prevLevel.Uid, "IconUpgrade").Visible = false;
				}
				else
				{
					if (this.researchMode != TroopUpgradeScreenMode.Troops)
					{
						return;
					}
					UXButton optionalSubElement = this.troopGrid.GetOptionalSubElement<UXButton>(this.activeContract.ProductUid, "ButtonTroopItemCard");
					if (optionalSubElement == null)
					{
						return;
					}
					TroopUpgradeTag troopUpgradeTag = (TroopUpgradeTag)optionalSubElement.Tag;
					string text = "pBarUpgradeTime";
					string text2 = "LabelpBarUpgradeTime";
					if (!string.IsNullOrEmpty(troopUpgradeTag.Troop.UpgradeShardUid))
					{
						text += "Quality";
						text2 += "Quality";
					}
					subElement = this.troopGrid.GetSubElement<UXSlider>(this.activeContract.ProductUid, text);
					subElement2 = this.troopGrid.GetSubElement<UXLabel>(this.activeContract.ProductUid, text2);
				}
				subElement.Value = 1f - (float)remainingTimeForView / (float)this.activeContract.TotalTime;
				if (!subElement.Visible)
				{
					subElement.Visible = true;
				}
				subElement2.Text = GameUtils.GetTimeLabelFromSeconds(remainingTimeForView);
				if (!subElement2.Visible)
				{
					subElement2.Visible = true;
				}
			}
			else
			{
				this.activeContract = null;
				this.DisableTimers();
				this.RefreshGrids();
			}
		}

		private int CompareTagBySortOrder(TroopUpgradeTag a, TroopUpgradeTag b)
		{
			return this.CompareBySortOrder(a.Troop, b.Troop);
		}

		private int CompareUpgradeDataBySortOrder(TroopUpgradeScreen.DeployableUpgradeData a, TroopUpgradeScreen.DeployableUpgradeData b)
		{
			return this.CompareBySortOrder(a.UpgradeType, b.UpgradeType);
		}

		private int CompareBySortOrder(IUpgradeableVO a, IUpgradeableVO b)
		{
			if (a == b)
			{
				return 0;
			}
			int num = a.Order - b.Order;
			if (num == 0)
			{
				Service.Logger.WarnFormat("Upgradable {0} matches order ({1}) of {2}", new object[]
				{
					a.Uid,
					a.Order,
					b.Uid
				});
			}
			return num;
		}

		private void AddSpacerElement(int sortOrder, UXGrid grid)
		{
			UXElement item = grid.CloneTemplateItem("spacer");
			grid.GetSubElement<UXButton>("spacer", "ButtonTroopItemCard").Visible = false;
			grid.GetSubElement<UXLabel>("spacer", "LabelTroopLevel").Visible = false;
			grid.GetSubElement<UXLabel>("spacer", "LabelRequirement").Visible = false;
			grid.GetSubElement<UXSprite>("spacer", "SpriteIconLockedTroop").Visible = false;
			grid.GetSubElement<UXSprite>("spacer", "SpriteFragmentTroop").Visible = false;
			grid.AddItem(item, sortOrder);
		}

		private void AddUpgradeableItemToTroopGrid(IDeployableVO nextTroop, IDeployableVO currentTroop, int sortOrder)
		{
			string uid = nextTroop.Uid;
			UXElement uXElement = this.troopGrid.CloneTemplateItem(uid);
			UXLabel subElement = this.troopGrid.GetSubElement<UXLabel>(uid, "LabelTroopLevel");
			subElement.Text = LangUtils.GetLevelText(currentTroop.Lvl);
			UXElement subElement2 = this.troopGrid.GetSubElement<UXElement>(uid, "CardDefault");
			UXElement subElement3 = this.troopGrid.GetSubElement<UXElement>(uid, "CardQuality");
			string text = null;
			string text2 = null;
			bool flag = !nextTroop.PlayerFacing || nextTroop.Lvl == currentTroop.Lvl;
			bool flag2;
			if (flag)
			{
				flag2 = false;
				text = this.lang.Get("MAX_LEVEL", new object[0]);
				text2 = text;
			}
			else
			{
				flag2 = Service.UnlockController.CanDeployableBeUpgraded(currentTroop, nextTroop, out text, out text2);
			}
			this.troopGrid.GetSubElement<UXSprite>(uid, "SpriteDim").Visible = !flag2;
			this.troopGrid.GetSubElement<UXSprite>(uid, "SpriteIconLockedTroop").Visible = (!flag2 && !flag);
			UXSprite subElement4 = this.troopGrid.GetSubElement<UXSprite>(uid, "SpriteFragmentTroop");
			bool flag3 = !string.IsNullOrEmpty(currentTroop.UpgradeShardUid);
			if (flag3)
			{
				subElement2.Visible = false;
				subElement3.Visible = true;
				subElement4.Visible = true;
				ShardVO shardVO = this.dataController.Get<ShardVO>(currentTroop.UpgradeShardUid);
				int quality = (int)shardVO.Quality;
				string name = string.Format("CardQ10", quality);
				UXElement optionalSubElement = this.troopGrid.GetOptionalSubElement<UXElement>(uid, name);
				if (optionalSubElement != null)
				{
					base.RevertToOriginalNameRecursively(optionalSubElement.Root, uid);
					optionalSubElement.Visible = true;
				}
				UXUtils.SetupFragmentIconSprite(subElement4, quality);
				IDeployableVO troopType = currentTroop;
				if (Service.UnlockController.IsMinLevelUnlocked(currentTroop))
				{
					troopType = nextTroop;
				}
				this.SetupTroopShardProgressBar(uid, troopType, flag);
			}
			else
			{
				subElement4.Visible = false;
				subElement2.Visible = true;
				subElement3.Visible = false;
			}
			UXLabel subElement5 = this.troopGrid.GetSubElement<UXLabel>(uid, "LabelRequirement");
			subElement5.Visible = !flag2;
			if (!flag2)
			{
				subElement5.Text = text2;
			}
			UXSlider subElement6 = this.troopGrid.GetSubElement<UXSlider>(uid, "pBarUpgradeTime");
			subElement6.Visible = false;
			UXLabel subElement7 = this.troopGrid.GetSubElement<UXLabel>(uid, "LabelpBarUpgradeTime");
			subElement7.Visible = false;
			UXSlider subElement8 = this.troopGrid.GetSubElement<UXSlider>(uid, "pBarUpgradeTimeQuality");
			subElement8.Visible = false;
			UXLabel subElement9 = this.troopGrid.GetSubElement<UXLabel>(uid, "LabelpBarUpgradeTimeQuality");
			subElement9.Visible = false;
			string text3 = "SpriteTroopItemImage";
			if (flag3)
			{
				text3 += "Quality";
			}
			UXSprite subElement10 = this.troopGrid.GetSubElement<UXSprite>(uid, text3);
			ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(nextTroop, subElement10, true);
			ProjectorUtils.GenerateProjector(config);
			FactionDecal.SetDeployableDecalVisibiliy(uid, this.troopGrid, false);
			TroopUpgradeTag troopUpgradeTag = new TroopUpgradeTag(nextTroop, flag2);
			troopUpgradeTag.IsMaxLevel = flag;
			troopUpgradeTag.RequirementText = text;
			troopUpgradeTag.ShortRequirementText = text2;
			UXButton subElement11 = this.troopGrid.GetSubElement<UXButton>(uid, "ButtonTroopItemCard");
			subElement11.Tag = troopUpgradeTag;
			subElement11.OnClicked = new UXButtonClickedDelegate(this.OnTroopCardClicked);
			uXElement.Tag = troopUpgradeTag;
			this.troopGrid.AddItem(uXElement, sortOrder);
		}

		private void SetupTroopShardProgressBar(string itemUid, IDeployableVO troopType, bool isMaxLevel)
		{
			UXSlider subElement = this.troopGrid.GetSubElement<UXSlider>(itemUid, "pBarQuality");
			UXSprite subElement2 = this.troopGrid.GetSubElement<UXSprite>(itemUid, "SpritepBarQuality");
			UXLabel subElement3 = this.troopGrid.GetSubElement<UXLabel>(itemUid, "LabelNumberQuality");
			if (isMaxLevel)
			{
				subElement3.Text = string.Empty;
				subElement.Value = 0f;
				this.troopGrid.GetSubElement<UXElement>(itemUid, "IconUpgradeQuality").Visible = false;
				return;
			}
			int shardAmount = Service.DeployableShardUnlockController.GetShardAmount(troopType.UpgradeShardUid);
			int upgradeShardCount = troopType.UpgradeShardCount;
			subElement3.Text = this.lang.Get("FRACTION", new object[]
			{
				shardAmount,
				upgradeShardCount
			});
			if (upgradeShardCount == 0)
			{
				Service.Logger.ErrorFormat("CMS Error: Shards required for {0} is zero", new object[]
				{
					troopType.Uid
				});
				return;
			}
			float num = (float)shardAmount / (float)upgradeShardCount;
			subElement.Value = ((num <= 1f) ? num : 1f);
			UXElement subElement4 = this.troopGrid.GetSubElement<UXElement>(itemUid, "IconUpgradeQuality");
			subElement4.Visible = (shardAmount >= upgradeShardCount);
			if (subElement4.Visible)
			{
				subElement2.Color = UXUtils.COLOR_SHARD_COMPLETE;
			}
			else
			{
				subElement2.Color = UXUtils.COLOR_SHARD_INPROGRESS;
			}
		}

		private void OnTroopCardClicked(UXButton button)
		{
			TroopUpgradeTag tag = (TroopUpgradeTag)button.Tag;
			List<TroopUpgradeTag> list = new List<TroopUpgradeTag>();
			for (int i = 0; i < this.troopGrid.Count; i++)
			{
				if (this.troopGrid.GetItem(i).Tag != null)
				{
					list.Add(this.GetPrevLevel(this.troopGrid.GetItem(i).Tag as TroopUpgradeTag));
				}
			}
			list.Sort(new Comparison<TroopUpgradeTag>(this.CompareTagBySortOrder));
			Service.ScreenController.AddScreen(new DeployableInfoScreen(this.GetPrevLevel(tag), list, true, this.selectedBuilding));
		}

		private TroopUpgradeTag GetPrevLevel(TroopUpgradeTag tag)
		{
			if (tag.IsMaxLevel && tag.Troop.PlayerFacing)
			{
				return tag;
			}
			IDeployableVO deployableVO = null;
			if (tag.Troop is SpecialAttackTypeVO)
			{
				deployableVO = Service.StarshipUpgradeCatalog.GetPrevLevel(tag.Troop as SpecialAttackTypeVO);
			}
			else if (tag.Troop is TroopTypeVO)
			{
				deployableVO = Service.TroopUpgradeCatalog.GetPrevLevel(tag.Troop as TroopTypeVO);
			}
			string requirementText = null;
			string shortRequirementText = null;
			bool reqMet = Service.UnlockController.CanDeployableBeUpgraded(deployableVO, tag.Troop, out requirementText, out shortRequirementText);
			return new TroopUpgradeTag(deployableVO, reqMet)
			{
				RequirementText = requirementText,
				ShortRequirementText = shortRequirementText
			};
		}

		protected override void RefreshScreen()
		{
			this.RefreshFilterTabs();
			this.RefreshGrids();
		}

		public override void OnDestroyElement()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ContractCompleted);
			Service.EventManager.UnregisterObserver(this, EventId.InventoryUnlockUpdated);
			this.DisableTimers();
			if (this.troopGrid != null)
			{
				this.troopGrid.Clear();
				this.troopGrid = null;
			}
			if (this.equipmentGrid != null)
			{
				this.equipmentGrid.Clear();
				this.equipmentGrid = null;
			}
			if (this.filterHelper != null)
			{
				this.filterHelper.Destroy();
				this.filterHelper = null;
			}
			this.eligibleDeployables.Clear();
			this.eligibleDeployables = null;
			this.eligibleTroopTypes.Clear();
			this.eligibleTroopTypes = null;
			this.activeContract = null;
			base.OnDestroyElement();
		}
	}
}
