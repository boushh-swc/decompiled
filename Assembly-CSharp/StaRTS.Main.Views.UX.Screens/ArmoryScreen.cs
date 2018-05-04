using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
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
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ArmoryScreen : ClosableScreen, IEventObserver
	{
		private const string BUILDING_INFO = "BUILDING_INFO";

		private const string ARMORY_CAPACITY = "ARMORY_CAPACITY";

		private const string ARMORY_INSTRUCTIONS = "ARMORY_CTA";

		private const string EQUIPMENT_LOCKED = "EQUIPMENT_LOCKED";

		private const string NOT_ENOUGH_CAPACITY = "ARMORY_FULL";

		private const string BASE_ON_INCORRECT_PLANET = "BASE_ON_INCORRECT_PLANET";

		private const string BUILDING_REQUIREMENT = "BUILDING_REQUIREMENT";

		private const string ACTIVATE_INSTRUCTION = "ACTIVATE_INSTRUCTION";

		private const string OBJECTIVE_PROGRESS = "OBJECTIVE_PROGRESS";

		private const string MAX_LEVEL = "MAX_LEVEL";

		private const string ARMORY_INACTIVE_CAPACITY_REACHED = "ARMORY_INACTIVE_CAPACITY_REACHED";

		private const string ARMORY_UPGRADE_NOW = "ARMORY_UPGRADE_NOW";

		private const string EQUIPMENT_TAB_PREFIX = "EQUIPMENT_TAB_";

		private const string LABEL_ARMORY_TITLE = "LabelTitle";

		private const string LABEL_CURRENT_CAPACITY = "LabelEquipmentActive";

		private const string LABEL_ARMORY_INSTRUCTIONS = "LabelEquipment";

		private const string LABEL_ACTIVATE_EQUIPMENT_INSTRUCTIONS = "LabelEquipmentActiveInstructions";

		private const string GRID_ACTIVE_EQUIPMENT = "GridEquipmentActive";

		private const string TEMPLATE_ACTIVE_EQUIPMENT = "EquipmentActiveItemTemplate";

		private const string GRID_EQUIPMENT = "EquipmentGrid";

		private const string TEMPLATE_EQUIPMENT_ITEM = "EquipmentItemTemplate";

		private const string EQUIPMENT_FRAGMENT_ICON = "SpriteIconFragment";

		private const string LABEL_EQUIPMENT_ACTIVE_NAME = "LabelEquipmentActiveName";

		private const string LABEL_EQUIPMENT_ACTIVE_LEVEL = "LabelEquipmentActiveLevel";

		private const string EQUIPMENT_ITEM_ACTIVE_ICON = "SpriteEquipmentActiveItemImage";

		private const string EQUIPMENT_ACTIVE_PLANET_ICON = "SpriteEquipmentActivePlanet";

		private const string BUTTON_EQUIPMENT_ACTIVE_CARD = "BtnEquipmentActiveItemCard";

		private const string BUTTON_EQUIPMENT_ACTIVE_CANCEL = "BtnEquipmentActiveCancel";

		private const string ACTIVE_EMPTY_CARD_BG_OUTLINE = "SpriteEquipmentActiveImageEmptySlot";

		private const string ACTIVE_CARD_BACKGROUND = "SpriteEquipmentActiveImageBkg";

		private const string ACTIVE_CARD_GRADIENT_BOTTOM = "SpriteEquipmentActiveGradientBottom";

		private const string SPRITE_EQUIPMENT_ACTIVE_IMAGE_BKG_STROKE = "SpriteEquipmentActiveImageBkgStroke";

		private const string SPRITE_EQUIPMENT_ACTIVE_IMAGE_BKG_GLOW = "SpriteEquipmentActiveImageBkgGlow";

		private const string ACTIVE_CARD = "EquipmentActiveItemCardQ{0}";

		private const string LABEL_EQUIPMENT_NAME = "LabelEquipmentName";

		private const string LABEL_EQUIPMENT_LEVEL = "LabelEquipmentLevel";

		private const string LABEL_EQUIPMENT_REQUIREMENT = "LabelEquipmentRequirement";

		private const string EQUIPMENT_ITEM_ICON = "SpriteEquipmentItemImage";

		private const string EQUIPMENT_PLANET_ICON = "SpriteEquipmentPlanet";

		private const string BUTTON_EQUIPMENT_CARD = "BtnEquipmentItemCard";

		private const string BUTTON_EQUIPMENT_INFO = "BtnEquipmentInfo";

		private const string SPRITE_BUTTON_DIMMER = "SpriteDim";

		private const string SPRITE_BUTTON_DIM_FULL = "SpriteDimFull";

		private const string LABEL_FRAG_PROGRESS = "LabelFragProgress";

		private const string SPRITE_LOCK_ICON = "SpriteLockIcon";

		private const string PLANET_LOCKED = "PlanetLocked";

		private const string ICON_UPGRADE = "IconUpgrade";

		private const string SPRITE_EQUIPMENT_IMAGE_BKG_STROKE = "SpriteEquipmentImageBkgStroke";

		private const string SPRITE_EQUIPMENT_ITEM_BAR_OUTLINE = "SpriteEquipmentItemBarOutline";

		private const string SPRITE_EQUIPMENT_IMAGE_BKG_GLOW = "SpriteEquipmentImageBkgGlow";

		private const string INACTIVE_CARD = "EquipmentItemCardQ{0}";

		private const float EQUIPMENT_GLOW_ALPHA = 0.4f;

		private const string INACTIVE_PROGRESS_BAR = "pBarEquipmentItemFrag";

		private const string INACTIVE_PROGRESS_BAR_SPRITE = "SpriteEquipmentItempBarFrag";

		private const int ACTIVE_GRID_MAX_CARDS_NO_SCROLL = 5;

		private const string EMPTY_ACTIVATE_INSTRUCTION_CARD = "EMPTY";

		private const int EFFECTIVE_MAX_INDEX = 500000;

		private const int SCROLL_POSITION_INVALID = -1;

		private static Dictionary<ShardQuality, Color> qualityColor;

		private static readonly Color quality1 = new Color(0.5411765f, 0.549019635f, 0.431372553f);

		private static readonly Color quality2 = new Color(0.211764708f, 0.34117648f, 1f);

		private static readonly Color quality3 = new Color(0.8784314f, 0.5411765f, 0.0392156877f);

		private UXLabel titleLabel;

		private UXLabel currentCapacityLabel;

		private UXLabel instructionsLabel;

		private UXGrid activeGrid;

		private UXGrid inactiveGrid;

		private BuildingTypeVO buildingInfo;

		private EquipmentTabHelper equipmentTabs;

		private int activeCardID;

		private int minimumInactiveSize;

		private float inactiveScrollPosition;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public ArmoryScreen(Entity armoryBuilding) : base("gui_armory")
		{
			this.buildingInfo = ((armoryBuilding != null) ? armoryBuilding.Get<BuildingComponent>().BuildingType : null);
			this.equipmentTabs = new EquipmentTabHelper();
			ArmoryScreen.qualityColor = new Dictionary<ShardQuality, Color>();
			ArmoryScreen.qualityColor.Add(ShardQuality.Basic, ArmoryScreen.quality1);
			ArmoryScreen.qualityColor.Add(ShardQuality.Advanced, ArmoryScreen.quality2);
			ArmoryScreen.qualityColor.Add(ShardQuality.Elite, ArmoryScreen.quality3);
			this.activeCardID = 0;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.EquipmentActivated);
			eventManager.RegisterObserver(this, EventId.EquipmentDeactivated);
		}

		public override void OnDestroyElement()
		{
			this.ResetScreen();
			this.equipmentTabs = null;
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.EquipmentActivated);
			eventManager.UnregisterObserver(this, EventId.EquipmentDeactivated);
			base.OnDestroyElement();
		}

		protected override void OnScreenLoaded()
		{
			this.InitLabels();
			this.InitActiveGrid();
			this.InitInactiveGrid();
			this.InitTabs();
			base.InitButtons();
		}

		private void InitLabels()
		{
			ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
			this.titleLabel = base.GetElement<UXLabel>("LabelTitle");
			this.titleLabel.Text = this.lang.Get("BUILDING_INFO", new object[]
			{
				LangUtils.GetBuildingDisplayName(this.buildingInfo),
				this.buildingInfo.Lvl
			});
			this.currentCapacityLabel = base.GetElement<UXLabel>("LabelEquipmentActive");
			this.currentCapacityLabel.Text = this.lang.Get("ARMORY_CAPACITY", new object[]
			{
				ArmoryUtils.GetCurrentActiveEquipmentCapacity(activeArmory),
				activeArmory.MaxCapacity
			});
			this.instructionsLabel = base.GetElement<UXLabel>("LabelEquipment");
			this.instructionsLabel.Text = this.lang.Get("ARMORY_CTA", new object[0]);
		}

		private void InitActiveGrid()
		{
			this.ResetMinimumInactiveSize();
			this.activeGrid = base.GetElement<UXGrid>("GridEquipmentActive");
			this.activeGrid.SetTemplateItem("EquipmentActiveItemTemplate");
			this.activeGrid.DupeOrdersAllowed = true;
			this.activeGrid.SetSortModeCustom();
			this.activeGrid.SetSortComparisonCallback(new Comparison<UXElement>(this.SortActiveGrid));
			this.PopulateActiveGrid();
			this.ShowInstructionalTextOnFirstEmptyCard(this.activeGrid);
			this.activeGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.ActiveGridRepositionComplete));
		}

		private void ActiveGridRepositionComplete(AbstractUXList list)
		{
			this.activeGrid.Scroll(0f);
		}

		private void InitInactiveGrid()
		{
			this.ResetMinimumInactiveSize();
			this.inactiveGrid = base.GetElement<UXGrid>("EquipmentGrid");
			this.inactiveGrid.SetTemplateItem("EquipmentItemTemplate");
			this.inactiveGrid.DupeOrdersAllowed = true;
			this.inactiveGrid.SetSortModeCustom();
			this.inactiveGrid.SetSortComparisonCallback(new Comparison<UXElement>(this.SortInactiveGrid));
			List<EquipmentVO> equipmentList = this.GenerateInactiveEquipmentList();
			this.PopulateInactiveGridWithList(equipmentList);
			this.inactiveScrollPosition = 0f;
			this.inactiveGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.RepositionInactiveGridItemsCallback));
		}

		private int SortActiveGrid(UXElement elementA, UXElement elementB)
		{
			SortableEquipment a = elementA.Tag as SortableEquipment;
			SortableEquipment b = elementB.Tag as SortableEquipment;
			return ArmorySortUtils.SortWithList(a, b, new List<EquipmentSortMethod>
			{
				EquipmentSortMethod.EmptyEquipment,
				EquipmentSortMethod.DecrementingIndex,
				EquipmentSortMethod.IncrementingEmptyIndex
			});
		}

		private int SortInactiveGrid(UXElement elementA, UXElement elementB)
		{
			SortableEquipment a = elementA.Tag as SortableEquipment;
			SortableEquipment b = elementB.Tag as SortableEquipment;
			return ArmorySortUtils.SortWithList(a, b, new List<EquipmentSortMethod>
			{
				EquipmentSortMethod.UnlockedEquipment,
				EquipmentSortMethod.RequirementsMet,
				EquipmentSortMethod.CurrentPlanet,
				EquipmentSortMethod.Quality,
				EquipmentSortMethod.CapacitySize,
				EquipmentSortMethod.Alphabetical
			});
		}

		private List<SortableEquipment> AddEmptyCardsToSortableEquipmentList(UXGrid grid, int minimumGridSize, List<SortableEquipment> equipmentList)
		{
			int count = equipmentList.Count;
			if (count >= minimumGridSize)
			{
				return equipmentList;
			}
			for (int i = count; i < minimumGridSize; i++)
			{
				equipmentList.Add(new SortableEquipment(null));
			}
			return equipmentList;
		}

		private bool IsElementInGrid(UXGrid grid, string cardUid)
		{
			List<UXElement> elementList = grid.GetElementList();
			int i = 0;
			int count = elementList.Count;
			while (i < count)
			{
				SortableEquipment sortableEquipment = elementList[i].Tag as SortableEquipment;
				if (sortableEquipment.Equipment != null && sortableEquipment.Equipment.Uid == cardUid)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		private UXElement CreateEmptyCardInternal(UXGrid grid, string index)
		{
			UXElement uXElement = grid.CloneTemplateItem(index);
			uXElement.Tag = new SortableEquipment(null);
			UXButton subElement = grid.GetSubElement<UXButton>(index, "BtnEquipmentActiveItemCard");
			subElement.Enabled = false;
			UXLabel subElement2 = grid.GetSubElement<UXLabel>(index, "LabelEquipmentActiveName");
			subElement2.Visible = false;
			UXLabel subElement3 = this.activeGrid.GetSubElement<UXLabel>(index, "LabelEquipmentActiveInstructions");
			subElement3.Visible = false;
			UXLabel subElement4 = grid.GetSubElement<UXLabel>(index, "LabelEquipmentActiveLevel");
			subElement4.Visible = false;
			UXSprite subElement5 = grid.GetSubElement<UXSprite>(index, "SpriteEquipmentActiveItemImage");
			subElement5.Visible = false;
			UXButton subElement6 = grid.GetSubElement<UXButton>(index, "BtnEquipmentActiveCancel");
			subElement6.Visible = false;
			UXUtils.HideGridQualityCards(grid, index, "EquipmentActiveItemCardQ{0}");
			grid.GetSubElement<UXSprite>(index, "SpriteEquipmentActiveImageBkg").Visible = false;
			grid.GetSubElement<UXSprite>(index, "SpriteEquipmentActiveGradientBottom").Visible = false;
			grid.GetSubElement<UXSprite>(index, "SpriteEquipmentActiveImageBkgGlow").Visible = false;
			grid.GetSubElement<UXSprite>(index, "SpriteEquipmentActiveImageBkgStroke").Visible = false;
			return uXElement;
		}

		private List<EquipmentVO> GenerateInactiveEquipmentList()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			IDictionary<string, int> levels = currentPlayer.UnlockedLevels.Equipment.Levels;
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			List<string> iDCollection = equipmentUpgradeCatalog.GetIDCollection();
			List<SortableEquipment> list = new List<SortableEquipment>();
			EquipmentTab currentTab = (EquipmentTab)this.equipmentTabs.CurrentTab;
			int i = 0;
			int count = iDCollection.Count;
			while (i < count)
			{
				string text = iDCollection[i];
				int level = 1;
				if (levels.ContainsKey(text))
				{
					level = levels[text];
				}
				EquipmentVO byLevel = equipmentUpgradeCatalog.GetByLevel(text, level);
				if (currentPlayer.Faction == byLevel.Faction && !currentPlayer.ActiveArmory.Equipment.Contains(byLevel.Uid))
				{
					if (this.equipmentTabs.IsEquipmentValidForTab(byLevel, currentTab))
					{
						list.Add(new SortableEquipment(currentPlayer, byLevel));
					}
				}
				i++;
			}
			ArmorySortUtils.SortWithPriorityList(list, new List<EquipmentSortMethod>
			{
				EquipmentSortMethod.UnlockedEquipment,
				EquipmentSortMethod.RequirementsMet,
				EquipmentSortMethod.Quality,
				EquipmentSortMethod.CurrentPlanet,
				EquipmentSortMethod.CapacitySize,
				EquipmentSortMethod.Alphabetical
			});
			return ArmorySortUtils.RemoveWrapper(list);
		}

		private void PopulateInactiveGridWithList(List<EquipmentVO> equipmentList)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int i = 0;
			int count = equipmentList.Count;
			while (i < count)
			{
				UXElement item = this.CreateInactiveCard(this.inactiveGrid, equipmentList[i], currentPlayer);
				this.inactiveGrid.AddItem(item, i);
				i++;
			}
		}

		private int GetCurrentActiveEquipmentSpace()
		{
			ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
			return activeArmory.MaxCapacity - ArmoryUtils.GetCurrentActiveEquipmentCapacity(activeArmory);
		}

		private bool ListHasEmptyFirstCard(UXGrid grid)
		{
			UXElement item = grid.GetItem(0);
			if (item == null)
			{
				return false;
			}
			SortableEquipment sortableEquipment = item.Tag as SortableEquipment;
			return sortableEquipment != null && !sortableEquipment.HasEquipment();
		}

		private void PopulateActiveGrid()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			StaticDataController staticDataController = Service.StaticDataController;
			List<SortableEquipment> list = new List<SortableEquipment>();
			int i = 0;
			int count = currentPlayer.ActiveArmory.Equipment.Count;
			while (i < count)
			{
				EquipmentVO equipment = staticDataController.Get<EquipmentVO>(currentPlayer.ActiveArmory.Equipment[i]);
				list.Add(new SortableEquipment(equipment, this.activeCardID++));
				i++;
			}
			int num = list.Count;
			if (this.GetCurrentActiveEquipmentSpace() >= this.minimumInactiveSize)
			{
				num++;
			}
			list = this.AddEmptyCardsToSortableEquipmentList(this.activeGrid, num, list);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				UXElement item;
				if (list[j].HasEquipment())
				{
					item = this.CreateActiveCard(this.activeGrid, list[j].Equipment, currentPlayer);
				}
				else
				{
					item = this.CreateEmptyCard(this.activeGrid);
				}
				this.activeGrid.AddItem(item, j);
				j++;
			}
		}

		private void ShowInstructionalTextOnFirstEmptyCard(UXGrid grid)
		{
			if (!this.ListHasEmptyFirstCard(grid))
			{
				return;
			}
			List<UXElement> elementList = grid.GetElementList();
			int num = 500000;
			int i = 0;
			int count = elementList.Count;
			while (i < count)
			{
				SortableEquipment sortableEquipment = elementList[i].Tag as SortableEquipment;
				if (sortableEquipment.EmptyIndex < num)
				{
					num = sortableEquipment.EmptyIndex;
				}
				i++;
			}
			StringBuilder stringBuilder = new StringBuilder("EMPTY");
			string itemUid = stringBuilder.Append(num).ToString();
			UXLabel subElement = grid.GetSubElement<UXLabel>(itemUid, "LabelEquipmentActiveInstructions");
			subElement.Visible = true;
		}

		private UXElement CreateEmptyCard(UXGrid grid)
		{
			SortableEquipment sortableEquipment = new SortableEquipment(null);
			sortableEquipment.EmptyIndex = 0;
			StringBuilder stringBuilder = new StringBuilder("EMPTY");
			string text = stringBuilder.Append(sortableEquipment.EmptyIndex).ToString();
			UXElement uXElement = this.CreateEmptyCardInternal(grid, text);
			uXElement.Tag = sortableEquipment;
			UXLabel subElement = this.activeGrid.GetSubElement<UXLabel>(text, "LabelEquipmentActiveInstructions");
			subElement.Text = this.lang.Get("ACTIVATE_INSTRUCTION", new object[0]);
			subElement.Visible = false;
			return uXElement;
		}

		private UXElement CreateCommonEquipmentCard(UXGrid grid, EquipmentVO equipment, string labelName, string labelLevel, string icon, string cardName, bool shouldAnimate, bool closeup)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string uid = equipment.Uid;
			UXElement uXElement = grid.CloneTemplateItem(uid);
			uXElement.Tag = new SortableEquipment(equipment);
			UXLabel subElement = grid.GetSubElement<UXLabel>(uid, labelName);
			subElement.Text = LangUtils.GetEquipmentDisplayName(equipment);
			UXLabel subElement2 = grid.GetSubElement<UXLabel>(uid, labelLevel);
			if (ArmoryUtils.IsEquipmentOwned(currentPlayer, equipment))
			{
				subElement2.Text = LangUtils.GetLevelText(equipment.Lvl);
			}
			else
			{
				subElement2.Visible = false;
			}
			UXSprite subElement3 = grid.GetSubElement<UXSprite>(uid, icon);
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateEquipmentConfig(equipment, subElement3, closeup);
			if (shouldAnimate)
			{
				projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			}
			ProjectorUtils.GenerateProjector(projectorConfig);
			UXElement uXElement2 = UXUtils.SetCardQuality(this, grid, uid, (int)equipment.Quality, cardName);
			if (uXElement2 != null)
			{
				uXElement2.SkipBoundsCalculations(true);
			}
			return uXElement;
		}

		private UXElement CreateActiveCard(UXGrid grid, EquipmentVO equipment, CurrentPlayer player)
		{
			UXElement uXElement = this.CreateCommonEquipmentCard(this.activeGrid, equipment, "LabelEquipmentActiveName", "LabelEquipmentActiveLevel", "SpriteEquipmentActiveItemImage", "EquipmentActiveItemCardQ{0}", true, false);
			SortableEquipment sortableEquipment = uXElement.Tag as SortableEquipment;
			sortableEquipment.IncrementingIndex = this.activeCardID++;
			UXLabel subElement = this.activeGrid.GetSubElement<UXLabel>(equipment.Uid, "LabelEquipmentActiveInstructions");
			subElement.Visible = false;
			UXButton subElement2 = this.activeGrid.GetSubElement<UXButton>(equipment.Uid, "BtnEquipmentActiveCancel");
			subElement2.OnClicked = new UXButtonClickedDelegate(this.OnCancelButtonClicked);
			subElement2.Tag = uXElement;
			UXButton subElement3 = this.activeGrid.GetSubElement<UXButton>(equipment.Uid, "BtnEquipmentActiveItemCard");
			subElement3.Tag = equipment;
			subElement3.OnClicked = new UXButtonClickedDelegate(this.OnActiveCardButtonClicked);
			UXSprite subElement4 = grid.GetSubElement<UXSprite>(equipment.Uid, "SpriteEquipmentActiveImageBkgStroke");
			UXSprite subElement5 = grid.GetSubElement<UXSprite>(equipment.Uid, "SpriteEquipmentActiveImageBkgGlow");
			subElement4.Color = ArmoryScreen.qualityColor[equipment.Quality];
			subElement5.Color = ArmoryScreen.qualityColor[equipment.Quality];
			subElement5.Alpha = 0.4f;
			this.activeGrid.GetSubElement<UXSprite>(equipment.Uid, "SpriteEquipmentActiveImageEmptySlot").Visible = false;
			return uXElement;
		}

		private UXElement CreateInactiveCard(UXGrid grid, EquipmentVO equipment, CurrentPlayer currentPlayer)
		{
			UXElement uXElement = this.CreateCommonEquipmentCard(grid, equipment, "LabelEquipmentName", "LabelEquipmentLevel", "SpriteEquipmentItemImage", "EquipmentItemCardQ{0}", false, true);
			(uXElement.Tag as SortableEquipment).Player = currentPlayer;
			UXButton subElement = this.inactiveGrid.GetSubElement<UXButton>(equipment.Uid, "BtnEquipmentInfo");
			subElement.OnClicked = new UXButtonClickedDelegate(this.OnInfoButtonClicked);
			subElement.Tag = equipment;
			UXButton subElement2 = this.inactiveGrid.GetSubElement<UXButton>(equipment.Uid, "BtnEquipmentItemCard");
			subElement2.OnClicked = new UXButtonClickedDelegate(this.OnCardButtonClicked);
			subElement2.Tag = uXElement;
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			UXSlider subElement3 = this.inactiveGrid.GetSubElement<UXSlider>(equipment.Uid, "pBarEquipmentItemFrag");
			UXSprite subElement4 = this.inactiveGrid.GetSubElement<UXSprite>(equipment.Uid, "SpriteEquipmentItempBarFrag");
			UXLabel subElement5 = grid.GetSubElement<UXLabel>(equipment.Uid, "LabelFragProgress");
			UXElement subElement6 = this.inactiveGrid.GetSubElement<UXElement>(equipment.Uid, "IconUpgrade");
			UXSprite subElement7 = grid.GetSubElement<UXSprite>(equipment.Uid, "SpriteEquipmentImageBkgStroke");
			UXSprite subElement8 = grid.GetSubElement<UXSprite>(equipment.Uid, "SpriteEquipmentItemBarOutline");
			UXSprite subElement9 = grid.GetSubElement<UXSprite>(equipment.Uid, "SpriteEquipmentImageBkgGlow");
			subElement7.Color = ArmoryScreen.qualityColor[equipment.Quality];
			subElement8.Color = ArmoryScreen.qualityColor[equipment.Quality];
			subElement9.Color = ArmoryScreen.qualityColor[equipment.Quality];
			subElement9.Alpha = 0.4f;
			float sliderProgressValue = this.GetSliderProgressValue(equipment, currentPlayer.GetShards(equipment.EquipmentID));
			UXSprite subElement10 = this.inactiveGrid.GetSubElement<UXSprite>(equipment.Uid, "SpriteIconFragment");
			UXUtils.SetupFragmentIconSprite(subElement10, (int)equipment.Quality);
			UXUtils.SetShardProgressBarValue(subElement3, subElement4, sliderProgressValue);
			subElement6.Visible = false;
			if (ArmoryUtils.IsAtMaxLevel(equipmentUpgradeCatalog, equipment))
			{
				subElement5.Text = this.lang.Get("MAX_LEVEL", new object[0]);
			}
			else
			{
				int shards = currentPlayer.GetShards(equipment.EquipmentID);
				int shardsRequiredForNextUpgrade = ArmoryUtils.GetShardsRequiredForNextUpgrade(currentPlayer, equipmentUpgradeCatalog, equipment);
				if (shards >= shardsRequiredForNextUpgrade)
				{
					subElement5.Text = this.lang.Get("ARMORY_UPGRADE_NOW", new object[0]);
				}
				else
				{
					subElement5.Text = this.lang.Get("OBJECTIVE_PROGRESS", new object[]
					{
						shards,
						shardsRequiredForNextUpgrade
					});
				}
			}
			if (ArmoryUtils.IsEquipmentOwned(currentPlayer, equipment))
			{
				EquipmentVO nextLevel = equipmentUpgradeCatalog.GetNextLevel(equipment);
				if (nextLevel != null)
				{
					if (Service.ISupportController.FindFirstContractWithProductUid(nextLevel.Uid) != null)
					{
						subElement5.Visible = false;
						subElement3.Visible = false;
					}
					else if (currentPlayer.GetShards(equipment.EquipmentID) >= nextLevel.UpgradeShards)
					{
						subElement6.Visible = true;
					}
				}
			}
			this.SetDimmerBasedOnRequirements(currentPlayer, equipment);
			return uXElement;
		}

		private void UpdateMinimumInactiveSize(int requiredSpace)
		{
			this.minimumInactiveSize = Math.Min(this.minimumInactiveSize, requiredSpace);
			if (!this.ListHasEmptyFirstCard(this.activeGrid) && this.GetCurrentActiveEquipmentSpace() > this.minimumInactiveSize)
			{
				UXElement item = this.CreateEmptyCard(this.activeGrid);
				this.activeGrid.AddItem(item, 0);
				this.ShowInstructionalTextOnFirstEmptyCard(this.activeGrid);
				this.activeGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.ActiveGridRepositionComplete));
			}
		}

		private void SetDimmerBasedOnRequirements(CurrentPlayer player, EquipmentVO equipment)
		{
			UXSprite subElement = this.inactiveGrid.GetSubElement<UXSprite>(equipment.Uid, "SpriteDim");
			UXSprite subElement2 = this.inactiveGrid.GetSubElement<UXSprite>(equipment.Uid, "SpriteDimFull");
			UXLabel subElement3 = this.inactiveGrid.GetSubElement<UXLabel>(equipment.Uid, "LabelEquipmentRequirement");
			UXSprite subElement4 = this.inactiveGrid.GetSubElement<UXSprite>(equipment.Uid, "SpriteLockIcon");
			UXElement subElement5 = this.inactiveGrid.GetSubElement<UXElement>(equipment.Uid, "PlanetLocked");
			subElement2.Visible = false;
			subElement4.Visible = false;
			subElement5.Visible = false;
			bool flag = ArmoryUtils.IsEquipmentOwned(player, equipment);
			if (ArmoryUtils.IsBuildingRequirementMet(equipment) && flag && ArmoryUtils.IsEquipmentValidForPlanet(equipment, player.PlanetId))
			{
				subElement3.Text = string.Empty;
				if (ArmoryUtils.HasEnoughCapacityToActivateEquipment(player.ActiveArmory, equipment))
				{
					this.UpdateMinimumInactiveSize(equipment.Size);
					subElement.Visible = false;
					return;
				}
				subElement3.Text = this.lang.Get("ARMORY_INACTIVE_CAPACITY_REACHED", new object[0]);
				subElement.Visible = true;
				return;
			}
			else
			{
				if (!ArmoryUtils.IsBuildingRequirementMet(equipment))
				{
					StaticDataController staticDataController = Service.StaticDataController;
					BuildingTypeVO buildingTypeVO = staticDataController.Get<BuildingTypeVO>(equipment.BuildingRequirement);
					subElement3.Text = this.lang.Get("BUILDING_REQUIREMENT", new object[]
					{
						buildingTypeVO.Lvl,
						LangUtils.GetBuildingDisplayName(buildingTypeVO)
					});
					subElement2.Visible = true;
					subElement.Visible = false;
					return;
				}
				UXButton subElement6 = this.inactiveGrid.GetSubElement<UXButton>(equipment.Uid, "BtnEquipmentItemCard");
				subElement6.Enabled = false;
				if (!ArmoryUtils.IsEquipmentOnValidPlanet(player, equipment) && flag)
				{
					subElement.Visible = false;
					subElement2.Visible = true;
					string planetDisplayName = LangUtils.GetPlanetDisplayName(player.PlanetId);
					subElement3.Text = this.lang.Get("BASE_ON_INCORRECT_PLANET", new object[]
					{
						planetDisplayName
					});
					subElement5.Visible = true;
					return;
				}
				subElement.Visible = false;
				subElement2.Visible = true;
				subElement4.Visible = true;
				if (player.Shards.ContainsKey(equipment.EquipmentID))
				{
					subElement3.Text = this.lang.Get("EQUIPMENT_LOCKED", new object[]
					{
						equipment.UpgradeShards - player.Shards[equipment.EquipmentID]
					});
				}
				else
				{
					subElement3.Text = this.lang.Get("EQUIPMENT_LOCKED", new object[]
					{
						equipment.UpgradeShards
					});
				}
				return;
			}
		}

		private float GetSliderProgressValue(EquipmentVO equipment, int currentShards)
		{
			EquipmentVO nextLevel = Service.EquipmentUpgradeCatalog.GetNextLevel(equipment);
			if (nextLevel == null)
			{
				return 1f;
			}
			EquipmentVO equipmentVO;
			if (Service.CurrentPlayer.UnlockedLevels.Equipment.Has(equipment))
			{
				equipmentVO = nextLevel;
			}
			else
			{
				equipmentVO = equipment;
			}
			if (equipmentVO.UpgradeShards == 0)
			{
				Service.Logger.ErrorFormat("CMS Error: Shards required for {0} is zero", new object[]
				{
					equipment.Uid
				});
				return 0f;
			}
			float num = (float)currentShards / (float)equipmentVO.UpgradeShards;
			return (num <= 1f) ? num : 1f;
		}

		private void InitTabs()
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
			this.equipmentTabs.CreateTabs(this, new Action(this.OnEquipmentTabChanged), dictionary, 0);
			this.equipmentTabs.SetSelectable(true);
		}

		private void ResetScreen()
		{
			this.equipmentTabs.Destroy();
		}

		private void OnEquipmentTabChanged()
		{
			this.RecreateInactiveGrid();
		}

		private void OnActiveCardButtonClicked(UXButton button)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<string> equipment = Service.CurrentPlayer.ActiveArmory.Equipment;
			List<EquipmentVO> list = new List<EquipmentVO>();
			int i = 0;
			int count = equipment.Count;
			while (i < count)
			{
				list.Add(staticDataController.Get<EquipmentVO>(equipment[i]));
				i++;
			}
			EquipmentVO selectedEquipment = button.Tag as EquipmentVO;
			Service.ScreenController.AddScreen(new EquipmentInfoScreen(selectedEquipment, list, null, false, true));
		}

		private void OnCancelButtonClicked(UXButton button)
		{
			UXElement uXElement = button.Tag as UXElement;
			EquipmentVO equipment = (uXElement.Tag as SortableEquipment).Equipment;
			Service.ArmoryController.DeactivateEquipment(equipment.EquipmentID);
		}

		private void OnCardButtonClicked(UXButton button)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			EquipmentVO equipmentVOFromCard = this.GetEquipmentVOFromCard(button.Tag as UXElement);
			if (!ArmoryUtils.IsEquipmentValidForPlanet(equipmentVOFromCard, currentPlayer.PlanetId))
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(this.lang.Get("BASE_ON_INCORRECT_PLANET", new object[0]));
				return;
			}
			if (!ArmoryUtils.HasEnoughCapacityToActivateEquipment(currentPlayer.ActiveArmory, equipmentVOFromCard))
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(this.lang.Get("ARMORY_FULL", new object[0]));
				return;
			}
			Service.ArmoryController.ActivateEquipment(equipmentVOFromCard.EquipmentID);
		}

		private void OnInfoButtonClicked(UXButton button)
		{
			EquipmentVO selectedEquipment = button.Tag as EquipmentVO;
			List<EquipmentVO> list = new List<EquipmentVO>();
			int i = 0;
			int count = this.inactiveGrid.Count;
			while (i < count)
			{
				EquipmentVO equipmentVOFromCard = this.GetEquipmentVOFromCard(this.inactiveGrid.GetItem(i));
				list.Add(equipmentVOFromCard);
				i++;
			}
			Service.ScreenController.AddScreen(new EquipmentInfoScreen(selectedEquipment, list, null, false, true));
		}

		private EquipmentVO GetEquipmentVOFromCard(UXElement card)
		{
			string uid = (card.Tag as SortableEquipment).Equipment.Uid;
			return Service.StaticDataController.Get<EquipmentVO>(uid);
		}

		public void RecreateInactiveGrid()
		{
			this.ResetMinimumInactiveSize();
			this.RemoveAnEmptyCard(this.activeGrid);
			this.activeGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.ActiveGridRepositionComplete));
			List<EquipmentVO> equipmentList = this.GenerateInactiveEquipmentList();
			this.inactiveGrid.Clear();
			this.PopulateInactiveGridWithList(equipmentList);
			this.inactiveScrollPosition = 0f;
			this.inactiveGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.RepositionInactiveGridItemsCallback));
		}

		private void RemoveCardFromGrid(UXGrid grid, UXElement card)
		{
			grid.RemoveItem(card);
			base.DestroyElement(card);
		}

		private void RemoveCardFromGridByUid(UXGrid grid, string cardUid)
		{
			List<UXElement> elementList = grid.GetElementList();
			int i = 0;
			int count = elementList.Count;
			while (i < count)
			{
				SortableEquipment sortableEquipment = elementList[i].Tag as SortableEquipment;
				if (cardUid == sortableEquipment.Equipment.Uid)
				{
					this.RemoveCardFromGrid(grid, elementList[i]);
					return;
				}
				i++;
			}
		}

		private void RemoveAnEmptyCard(UXGrid grid)
		{
			List<UXElement> elementList = grid.GetElementList();
			UXElement uXElement = null;
			int i = 0;
			int count = elementList.Count;
			while (i < count)
			{
				UXElement uXElement2 = elementList[i];
				if ((elementList[i].Tag as SortableEquipment).Equipment == null)
				{
					uXElement = uXElement2;
					break;
				}
				i++;
			}
			if (uXElement != null)
			{
				this.RemoveCardFromGrid(grid, uXElement);
			}
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			ActiveArmory activeArmory = currentPlayer.ActiveArmory;
			EquipmentVO equipmentVO = cookie as EquipmentVO;
			float currentScrollPosition = this.activeGrid.GetCurrentScrollPosition(false);
			this.inactiveScrollPosition = this.inactiveGrid.GetCurrentScrollPosition(false);
			if (id != EventId.EquipmentActivated)
			{
				if (id == EventId.EquipmentDeactivated)
				{
					UXButton subElement = this.activeGrid.GetSubElement<UXButton>(equipmentVO.Uid, "BtnEquipmentActiveCancel");
					UXElement uXElement = subElement.Tag as UXElement;
					this.RemoveCardFromGrid(this.activeGrid, uXElement);
					this.activeGrid.RepositionItems(false);
					this.ShowInstructionalTextOnFirstEmptyCard(this.activeGrid);
					if (this.activeGrid.Count > 5)
					{
						this.activeGrid.Scroll(currentScrollPosition);
					}
					else
					{
						this.activeGrid.Scroll(0f);
					}
					SortableEquipment sortableEquipment = uXElement.Tag as SortableEquipment;
					if (this.equipmentTabs.IsEquipmentValidForTab(sortableEquipment.Equipment, (EquipmentTab)this.equipmentTabs.CurrentTab))
					{
						UXElement item = this.CreateInactiveCard(this.inactiveGrid, sortableEquipment.Equipment, currentPlayer);
						this.inactiveGrid.AddItem(item, this.inactiveGrid.Count + 1);
						this.inactiveGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.RepositionInactiveGridItemsCallback));
					}
					this.currentCapacityLabel.Text = this.lang.Get("ARMORY_CAPACITY", new object[]
					{
						ArmoryUtils.GetCurrentActiveEquipmentCapacity(activeArmory),
						activeArmory.MaxCapacity
					});
					this.RefreshInactiveCardStatusesBasedOnOverallCapacity();
				}
			}
			else
			{
				this.RemoveCardFromGridByUid(this.inactiveGrid, equipmentVO.Uid);
				this.inactiveGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.RepositionInactiveGridItemsCallback));
				this.RemoveAnEmptyCard(this.activeGrid);
				UXElement item2 = this.CreateActiveCard(this.activeGrid, equipmentVO, currentPlayer);
				this.activeGrid.AddItem(item2, this.activeCardID++);
				this.activeGrid.RepositionItems(false);
				this.ShowInstructionalTextOnFirstEmptyCard(this.activeGrid);
				if (this.activeGrid.Count > 5)
				{
					this.activeGrid.Scroll(currentScrollPosition);
				}
				else
				{
					this.activeGrid.Scroll(0f);
				}
				this.currentCapacityLabel.Text = this.lang.Get("ARMORY_CAPACITY", new object[]
				{
					ArmoryUtils.GetCurrentActiveEquipmentCapacity(activeArmory),
					activeArmory.MaxCapacity
				});
				this.RefreshInactiveCardStatusesBasedOnOverallCapacity();
			}
			return base.OnEvent(id, cookie);
		}

		private void RepositionInactiveGridItemsCallback(AbstractUXList list)
		{
			if (this.inactiveScrollPosition != -1f)
			{
				this.inactiveGrid.Scroll(this.inactiveScrollPosition);
			}
			this.inactiveScrollPosition = -1f;
		}

		private void ResetMinimumInactiveSize()
		{
			ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
			this.minimumInactiveSize = activeArmory.MaxCapacity + 1;
		}

		private void RefreshInactiveCardStatusesBasedOnOverallCapacity()
		{
			this.ResetMinimumInactiveSize();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			List<EquipmentVO> list = this.GenerateInactiveEquipmentList();
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				this.SetDimmerBasedOnRequirements(currentPlayer, list[i]);
				i++;
			}
		}

		public override void Close(object modalResult)
		{
			if (this.inactiveGrid != null)
			{
				this.inactiveGrid.Visible = false;
			}
			if (this.activeGrid != null)
			{
				this.activeGrid.Visible = false;
			}
			base.Close(modalResult);
		}
	}
}
