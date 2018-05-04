using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Planets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Player.World;
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
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class DeployableInfoScreen : SelectedBuildingScreen, IViewFrameTimeObserver
	{
		private const int NUM_DETAIL_LEFT_ROWS = 4;

		private const string GO_TO_PLANET_ACTION = "planet";

		private const string GO_TO_GALAXY_ACTION = "galaxy";

		protected const string BUTTON_BACK = "BtnBack";

		private const string BUTTON_TROOP_NEXT = "BtnTroopNext";

		private const string BUTTON_TROOP_PREV = "BtnTroopPrev";

		private const string LABEL_TROOP_NAME = "DialogBldgUpgradeTitle";

		private const string LABEL_TROOP_INFO = "LabelTroopInfo";

		private const string UPGRADE_TIME_GROUP = "ContainerUpgradeTime";

		private const string LABEL_UPGRADE_TIME = "LabelUpgradeTime";

		private const string LABEL_UPGRADE_TIME_STATIC = "LabelUpgradeTimeStatic";

		private const string UPGRADE_REQUIREMENT_LABEL = "LabelRequirement";

		private const string BUTTON_PURCHASE = "ButtonPrimaryAction";

		private const string BUTTON_FINISH = "BtnFinish";

		protected const string CONTEXT_COST_GROUP = "Cost";

		private const string CONTEXT_FINISH_GROUP = "FinishCost";

		private const string ICON_UPGRADE = "IconUpgrade";

		private const string SLIDER_NAME = "pBar{0}";

		private const string SLIDER_CURRENT = "pBarCurrent{0}";

		private const string SLIDER_NEXT = "pBarNext{0}";

		private const string LABEL_PBAR = "LabelpBar{0}";

		private const string LABEL_PBAR_CUR = "LabelpBar{0}Current";

		private const string LABEL_PBAR_NEXT = "LabelpBar{0}Next";

		private const string ELEMENT_PBAR = "pBar{0}";

		private const string SHARD_PROGRESS_LABEL = "LabelProgressQ{0}";

		private const string TROOP_IMAGE = "TroopImage";

		private const string TROOP_QUALITY_PANEL = "TroopImageQ{0}";

		private const string TROOP_QUALITY_BACKGROUND = "SpriteTroopImageBkgGridQ{0}";

		private const string TRAINING_TIME_GROUP = "TrainingTime";

		private const string TRAINING_TIME_NAME_LABEL = "LabelTrainingTime";

		private const string TRAINING_TIME_VALUE_LABEL = "LabelTrainingTimeCurrent";

		private const string TRAINING_TIME_NEXT_LABEL = "LabelTrainingTimeIncrease";

		private const string TRAINING_COST_GROUP = "TrainingCost";

		private const string TRAINING_COST_NAME_LABEL = "LabelTrainingCost";

		private const string TRAINING_COST_ICON = "SpriteTrainingCostIcon";

		private const string TRAINING_COST_VALUE_LABEL = "LabelTrainingCostCurrent";

		private const string TRAINING_COST_NEXT_LABEL = "LabelTrainingCostIncrease";

		private const string ATTACK_RANGE_GROUP = "Range";

		private const string ATTACK_RANGE_NAME_LABEL = "LabelRange";

		private const string ATTACK_RANGE_VALUE_LABEL = "LabelRangeCurrent";

		private const string MOVEMENT_SPEED_GROUP = "MovementSpeed";

		private const string MOVEMENT_SPEED_NAME_LABEL = "LabelMovementSpeed";

		private const string MOVEMENT_SPEED_VALUE_LABEL = "LabelMovementSpeedCurrent";

		private const string UNIT_CAPACITY_GROUP = "UnitCapacity";

		private const string UNIT_CAPACITY_NAME_LABEL = "LabelUnitCapacity";

		private const string UNIT_CAPACITY_VALUE_LABEL = "LabelUnitCapacityCurrent";

		private const string STRING_DAMAGE = "s_Damage";

		private const string STRING_DAMAGE_BUFF = "s_DamageBuff";

		private const string STRING_HEALING_POWER = "HEALING_POWER";

		private const string STRING_HEALING_PERCENT = "s_HealerPercent";

		private const string SPRITE_TROOP_SELECTED = "SpriteTroopSelectedItemImage";

		private const string LEFT_OWN_LABEL = "LabelQuantityOwn";

		private const string QUALITY_OWN_LABEL = "LabelQuantityOwnQ{0}";

		private const string LEFT_INFO_GROUP = "InfoRow{0}";

		private const string LEFT_INFO_TITLE = "InfoTitle{0}";

		private const string LEFT_INFO_DETAIL = "InfoDetail{0}";

		private const string LEFT_ITEM_STATUS = "ItemStatus";

		private const string LEFT_LABEL_QUALITY = "LabelQualityQ{0}";

		private const string LEFT_INFO_GROUP3_ALT = "InfoRow3alt";

		private const string LEFT_INFO_TITLE3_ALT = "InfoTitle3alt";

		private const string LEFT_INFO_DETAIL3_ALT = "InfoDetail3alt";

		public const string LANG_ENTITY_TYPE_PREFIX = "entityType_";

		public const string TARGET_PREF_PREFIX = "target_pref_";

		private const string LANG_SPECIAL_ABILITY = "s_SpecialAbility";

		private const string ICON_CREDITS = "icoCollectCredit";

		private const string ICON_MATERIALS = "icoDialogMaterials";

		private const string ICON_CONTRABAND = "icoContraband";

		private const string PERK_EFFECT_COST = "PerkEffectTrainingCost";

		private const string PERK_EFFECT_TIME = "PerkEffectTrainingTime";

		private const string UNLOCK_IN_EVENT_ONLY = "UNLOCK_IN_EVENT_ONLY";

		private const string BUTTON_NORMAL = "BtnNormal";

		private const string STRING_REWARD_UPGRADE = "LABEL_REWARD_UPGRADE";

		private const string LABEL_NORMAL_INTRO = "LabelNormalIntro";

		private const string SPRITE_SHARD_TROOP_SELECTED = "SpriteTroopSelectedItemImageQ{0}";

		private const string SHARD_PROGRESS_BAR = "pBarFrag";

		private const string SHARD_PROGRESS_BAR_SPRITE = "SpritepBarFrag";

		private const string SHARD_INFO_PROGRESS_LABEL = "LabelProgress";

		private const string STRING_FRACTION = "FRACTION";

		private const string STRING_MAX_LEVEL = "MAX_LEVEL";

		private const string BTN_GALAXY = "BtnGalaxy";

		private const string LABEL_BTN_GALAXY = "LabelBtnGalaxy";

		private const string PLANETS_UNLOCK_GROUP = "PanelPlanetAvailability";

		private const string LABEL_PLANETS_UNLOCK = "LabelPlanetAvailability";

		private const string GRID_PLANETS_UNLOCK = "GridPlanetAvailability";

		private const string GRID_PLANETS_UNLOCK_TEMPLATE = "TemplatePlanet";

		private const string LABEL_PLANET_UNLOCK_ITEM = "LabelAvailablePlanet";

		private const string TEXTURE_PLANET_UNLOCK_ITEM = "TextureAvailablePlanet";

		private const string PLANETS_PLAY_BUILD_PC = "PLANETS_PLAY_BUILD_PC";

		protected TroopUpgradeTag selectedTroop;

		private IDeployableVO nextLevel;

		private IDeployableVO currentLevel;

		private List<TroopUpgradeTag> troopList;

		protected bool showUpgradeControls;

		private Contract activeContract;

		protected GeometryProjector troopImage;

		protected UXLabel labelUpgradeTime;

		protected UXElement attackRangeGroup;

		protected UXElement movementSpeedGroup;

		protected UXElement unitCapacityGroup;

		protected UXElement trainingTimeGroup;

		protected UXElement trainingCostGroup;

		protected UXLabel attackRangeNameLabel;

		protected UXLabel attackRangeValueLabel;

		protected UXLabel movementSpeedNameLabel;

		protected UXLabel movementSpeedValueLabel;

		protected UXLabel unitCapacityValueLabel;

		protected UXLabel trainingTimeNameLabel;

		protected UXLabel trainingTimeValueLabel;

		protected UXLabel trainingCostValueLabel;

		protected UXLabel trainingCostNextValueLabel;

		private UXSprite trainingCostIcon;

		private UXElement perkEffectCost;

		private UXElement perkEffectTime;

		private UXButton goToGalaxyButton;

		private UXLabel goToGalaxyLabel;

		private UXElement planetsUnlockGroup;

		private UXGrid planetsUnlockGrid;

		private UXLabel planetsUnlockLabel;

		private int maxTroopDps;

		private int maxHealth;

		private int maxSpecialAttackDps;

		private bool timerActive;

		private float accumulatedUpdateDt;

		private bool uiWasRefreshed;

		protected bool wantsTransition;

		protected bool shouldCloseParent;

		protected override bool WantTransitions
		{
			get
			{
				return this.wantsTransition;
			}
		}

		public override bool ShowCurrencyTray
		{
			get
			{
				return this.showUpgradeControls;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public DeployableInfoScreen(TroopUpgradeTag selectedTroop, List<TroopUpgradeTag> troopList, bool showUpgradeControls, SmartEntity selectedBuilding) : base("gui_troop_info", selectedBuilding)
		{
			TroopUpgradeCatalog troopUpgradeCatalog = Service.TroopUpgradeCatalog;
			this.wantsTransition = false;
			this.shouldCloseParent = true;
			this.accumulatedUpdateDt = 0f;
			this.showUpgradeControls = showUpgradeControls;
			this.selectedTroop = selectedTroop;
			this.currentLevel = selectedTroop.Troop;
			this.nextLevel = this.GetNextDeployable(selectedTroop.Troop);
			this.troopList = troopList;
			this.maxTroopDps = troopUpgradeCatalog.MaxTroopDps;
			this.maxHealth = troopUpgradeCatalog.MaxTroopHealth;
			this.maxSpecialAttackDps = Service.StarshipUpgradeCatalog.MaxSpecialAttackDps;
			if (showUpgradeControls)
			{
				this.CheckActiveContract();
			}
		}

		private IDeployableVO GetNextDeployable(IDeployableVO currentTroop)
		{
			if (currentTroop is SpecialAttackTypeVO)
			{
				return Service.StarshipUpgradeCatalog.GetNextLevel(currentTroop as SpecialAttackTypeVO);
			}
			return Service.TroopUpgradeCatalog.GetNextLevel(currentTroop as TroopTypeVO);
		}

		public override void Close(object modalResult)
		{
			if (this.shouldCloseParent)
			{
				this.wantsTransition = true;
				ClosableScreen deployableInfoParentScreen = ScreenUtils.GetDeployableInfoParentScreen();
				if (deployableInfoParentScreen != null)
				{
					deployableInfoParentScreen.Close(null);
				}
			}
			base.Close(modalResult);
		}

		public override void OnDestroyElement()
		{
			if (this.troopImage != null)
			{
				this.troopImage.Destroy();
				this.troopImage = null;
			}
			this.troopList = null;
			this.selectedTroop = null;
			this.currentLevel = null;
			this.nextLevel = null;
			this.labelUpgradeTime = null;
			Service.EventManager.UnregisterObserver(this, EventId.ContractCompleted);
			this.DisableTimers();
			base.OnDestroyElement();
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			this.ToggleParentScreenVisibility(false);
			this.InitButtons();
			this.uiWasRefreshed = false;
			UXButton element = base.GetElement<UXButton>("BtnTroopPrev");
			UXButton element2 = base.GetElement<UXButton>("BtnTroopNext");
			if (this.troopList != null && this.troopList.Count > 1)
			{
				element.Visible = true;
				element2.Visible = true;
				element.Tag = -1;
				element2.Tag = 1;
				element.OnClicked = new UXButtonClickedDelegate(this.OnPrevOrNextButtonClicked);
				element2.OnClicked = new UXButtonClickedDelegate(this.OnPrevOrNextButtonClicked);
			}
			else
			{
				element.Visible = false;
				element2.Visible = false;
			}
			UXButton element3 = base.GetElement<UXButton>("BtnBack");
			element3.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			base.CurrentBackButton = element3;
			base.GetElement<UXButton>("BtnFinish").OnClicked = new UXButtonClickedDelegate(this.OnFinishClicked);
			base.GetElement<UXButton>("ButtonPrimaryAction").OnClicked = new UXButtonClickedDelegate(this.OnPurchaseClicked);
			base.GetElement<UXButton>("BtnNormal").OnClicked = new UXButtonClickedDelegate(this.OnPurchaseClicked);
			this.labelUpgradeTime = base.GetElement<UXLabel>("LabelUpgradeTime");
			this.attackRangeGroup = base.GetElement<UXElement>("Range");
			this.movementSpeedGroup = base.GetElement<UXElement>("MovementSpeed");
			this.unitCapacityGroup = base.GetElement<UXElement>("UnitCapacity");
			this.trainingTimeGroup = base.GetElement<UXElement>("TrainingTime");
			this.trainingCostGroup = base.GetElement<UXElement>("TrainingCost");
			this.attackRangeNameLabel = base.GetElement<UXLabel>("LabelRange");
			this.attackRangeValueLabel = base.GetElement<UXLabel>("LabelRangeCurrent");
			this.movementSpeedNameLabel = base.GetElement<UXLabel>("LabelMovementSpeed");
			this.movementSpeedValueLabel = base.GetElement<UXLabel>("LabelMovementSpeedCurrent");
			this.unitCapacityValueLabel = base.GetElement<UXLabel>("LabelUnitCapacityCurrent");
			this.trainingTimeNameLabel = base.GetElement<UXLabel>("LabelTrainingTime");
			this.trainingTimeValueLabel = base.GetElement<UXLabel>("LabelTrainingTimeCurrent");
			this.trainingCostValueLabel = base.GetElement<UXLabel>("LabelTrainingCostCurrent");
			this.trainingCostNextValueLabel = base.GetElement<UXLabel>("LabelTrainingCostIncrease");
			this.trainingCostIcon = base.GetElement<UXSprite>("SpriteTrainingCostIcon");
			this.perkEffectCost = base.GetElement<UXElement>("PerkEffectTrainingCost");
			this.perkEffectTime = base.GetElement<UXElement>("PerkEffectTrainingTime");
			this.goToGalaxyButton = base.GetElement<UXButton>("BtnGalaxy");
			this.goToGalaxyButton.Visible = false;
			this.goToGalaxyLabel = base.GetElement<UXLabel>("LabelBtnGalaxy");
			this.goToGalaxyLabel.Visible = false;
			this.planetsUnlockGroup = base.GetElement<UXElement>("PanelPlanetAvailability");
			this.planetsUnlockGroup.Visible = false;
			this.planetsUnlockGrid = base.GetElement<UXGrid>("GridPlanetAvailability");
			this.planetsUnlockLabel = base.GetElement<UXLabel>("LabelPlanetAvailability");
			base.GetElement<UXElement>("BtnNormal").Visible = false;
			this.Redraw();
			Service.EventManager.RegisterObserver(this, EventId.ContractCompleted);
		}

		private void CheckActiveContract()
		{
			Contract contract = null;
			if (this.selectedBuilding != null)
			{
				BuildingComponent buildingComp = this.selectedBuilding.BuildingComp;
				contract = Service.ISupportController.FindCurrentContract(buildingComp.BuildingTO.Key);
			}
			if (contract != null)
			{
				this.activeContract = contract;
				this.EnableTimers();
			}
			else
			{
				this.activeContract = null;
				this.DisableTimers();
			}
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ContractCompleted)
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				ContractType contractType = contractEventData.Contract.ContractTO.ContractType;
				if (contractType == ContractType.Build || contractType == ContractType.Research || contractType == ContractType.Upgrade)
				{
					bool flag = this.activeContract != null;
					this.CheckActiveContract();
					bool flag2 = flag != (this.activeContract != null);
					if (contractEventData.Contract.ProductUid == this.nextLevel.Uid)
					{
						flag2 = true;
						IDeployableVO nextDeployable = this.GetNextDeployable(this.nextLevel);
						if (nextDeployable != null)
						{
							this.currentLevel = this.nextLevel;
							this.nextLevel = nextDeployable;
						}
					}
					if (flag2)
					{
						this.uiWasRefreshed = true;
						this.Redraw();
					}
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

		private void UpdateContractTimers()
		{
			if (this.activeContract == null || this.labelUpgradeTime == null)
			{
				return;
			}
			if (this.nextLevel != null && this.nextLevel.Uid == this.activeContract.ProductUid)
			{
				int remainingTimeForView = this.activeContract.GetRemainingTimeForView();
				if (remainingTimeForView > 0)
				{
					this.labelUpgradeTime.Text = GameUtils.GetTimeLabelFromSeconds(remainingTimeForView);
					int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(this.activeContract);
					UXUtils.SetupCostElements(this, "FinishCost", null, 0, 0, 0, crystalCostToFinishContract, 0, !this.selectedTroop.ReqMet, null);
				}
				else
				{
					this.activeContract = null;
					this.DisableTimers();
					this.Redraw();
				}
			}
		}

		private void EnableTimers()
		{
			if (!this.timerActive)
			{
				this.timerActive = true;
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
		}

		private void DisableTimers()
		{
			this.timerActive = false;
			this.activeContract = null;
			this.accumulatedUpdateDt = 0f;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		private void SetupAndEnableUnlockUnitGalaxyUI(IDeployableVO deployabVO)
		{
			this.goToGalaxyButton.Visible = true;
			this.goToGalaxyLabel.Visible = true;
			this.planetsUnlockGroup.Visible = true;
			if (Service.BuildingLookupController.GetCurrentNavigationCenter() == null)
			{
				this.goToGalaxyButton.VisuallyDisableButton();
			}
			else
			{
				this.goToGalaxyButton.VisuallyEnableButton();
			}
			DeployableInfoActionButtonTag deployableInfoActionButtonTag = new DeployableInfoActionButtonTag(deployabVO.EventButtonAction, deployabVO.EventButtonData);
			this.goToGalaxyButton.Tag = deployableInfoActionButtonTag;
			this.goToGalaxyButton.OnClicked = new UXButtonClickedDelegate(this.OnGoToGalaxyClicked);
			this.goToGalaxyLabel.Text = this.lang.Get(deployabVO.EventButtonString, new object[0]);
			this.planetsUnlockGrid.SetTemplateItem("TemplatePlanet");
			this.planetsUnlockLabel.Text = this.lang.Get(deployabVO.EventFeaturesString, new object[0]);
			this.FillPlanetsUnlockGrid(deployableInfoActionButtonTag.DataList);
		}

		private void FillPlanetsUnlockGrid(List<string> planetIds)
		{
			if (planetIds == null)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			this.planetsUnlockGrid.SetTemplateItem("TemplatePlanet");
			this.planetsUnlockGrid.Clear();
			int count = planetIds.Count;
			for (int i = 0; i < count; i++)
			{
				PlanetVO planetVO = staticDataController.Get<PlanetVO>(planetIds[i]);
				string itemUid = planetVO.Uid + i;
				UXElement item = this.planetsUnlockGrid.CloneTemplateItem(itemUid);
				UXTexture subElement = this.planetsUnlockGrid.GetSubElement<UXTexture>(itemUid, "TextureAvailablePlanet");
				subElement.LoadTexture(planetVO.LeaderboardButtonTexture);
				UXLabel subElement2 = this.planetsUnlockGrid.GetSubElement<UXLabel>(itemUid, "LabelAvailablePlanet");
				subElement2.Text = this.lang.Get(planetVO.LoadingScreenText, new object[0]);
				this.planetsUnlockGrid.AddItem(item, i);
			}
			this.planetsUnlockGrid.RepositionItems();
		}

		private void HideUnlockUnitGalaxyUI()
		{
			this.goToGalaxyButton.Visible = false;
			this.goToGalaxyLabel.Visible = false;
			this.planetsUnlockGroup.Visible = false;
		}

		private bool IsNotMaxLevel(IDeployableVO deployable)
		{
			return deployable != null && deployable.PlayerFacing;
		}

		private void Redraw()
		{
			UXElement element = base.GetElement<UXElement>("IconUpgrade");
			element.Visible = false;
			this.SetupTroopImage();
			UXButton element2 = base.GetElement<UXButton>("BtnFinish");
			UXButton element3 = base.GetElement<UXButton>("ButtonPrimaryAction");
			UXButton element4 = base.GetElement<UXButton>("BtnNormal");
			element4.Visible = false;
			UnlockController unlockController = Service.UnlockController;
			bool flag = unlockController.RequiresUnlockByEventReward(this.selectedTroop.Troop);
			bool flag2 = !unlockController.IsMinLevelUnlocked(this.selectedTroop.Troop);
			IDeployableVO deployableVO = this.nextLevel;
			bool flag3 = false;
			bool flag4 = false;
			if (this.IsNotMaxLevel(this.nextLevel))
			{
				if (flag)
				{
					if (flag2)
					{
						deployableVO = this.currentLevel;
						flag3 = false;
						flag4 = (!flag3 && this.showUpgradeControls);
					}
					else
					{
						flag3 = this.showUpgradeControls;
						flag4 = this.showUpgradeControls;
					}
				}
				else
				{
					flag3 = this.showUpgradeControls;
					flag4 = this.showUpgradeControls;
				}
			}
			base.GetElement<UXElement>("ContainerUpgradeTime").Visible = (flag3 && !flag2 && (this.activeContract == null || this.nextLevel.Uid == this.activeContract.ProductUid));
			element2.Visible = flag3;
			element3.Visible = flag3;
			IDeployableVO troop = this.selectedTroop.Troop;
			bool flag5 = (flag4 && !this.selectedTroop.ReqMet && this.IsNotMaxLevel(this.nextLevel)) || flag2;
			string text = this.selectedTroop.RequirementText;
			bool flag6 = Service.UnlockController.CanDeployableBeUpgraded(this.currentLevel, deployableVO);
			bool flag7 = this.nextLevel != null && Service.DeployableShardUnlockController.DoesUserHaveUpgradeShardRequirement(deployableVO);
			UXLabel element5 = base.GetElement<UXLabel>("LabelRequirement");
			if (flag)
			{
				bool flag8 = this.showUpgradeControls && !flag2;
				flag5 = ((!flag6 || !flag7) && this.IsNotMaxLevel(this.nextLevel) && !flag8);
				text = this.selectedTroop.ShortRequirementText;
			}
			element5.Visible = flag5;
			if (flag5)
			{
				element5.Text = text;
			}
			if (this.activeContract == null)
			{
				element3.VisuallyEnableButton();
			}
			else
			{
				element3.VisuallyDisableButton();
			}
			bool visible = true;
			if (this.activeContract == null || deployableVO == null || deployableVO.Uid != this.activeContract.ProductUid)
			{
				element2.Visible = false;
				base.GetElement<UXLabel>("LabelUpgradeTimeStatic").Text = this.lang.Get("s_upgradeTime", new object[0]);
				if (deployableVO != null)
				{
					this.labelUpgradeTime.Text = GameUtils.GetTimeLabelFromSeconds(deployableVO.UpgradeTime);
				}
				element3.Enabled = this.selectedTroop.ReqMet;
			}
			else
			{
				element2.Visible = true;
				element3.Visible = false;
				visible = false;
				element5.Visible = false;
				base.GetElement<UXLabel>("LabelUpgradeTimeStatic").Text = this.lang.Get("s_TimeRemaining", new object[0]);
				this.UpdateContractTimers();
			}
			if (flag3)
			{
				if (flag)
				{
					element3.Visible = false;
					element4.Visible = visible;
					element4.Enabled = (flag7 && flag6);
					base.GetElement<UXLabel>("LabelNormalIntro").Text = this.lang.Get("LABEL_REWARD_UPGRADE", new object[0]);
				}
				else
				{
					UXUtils.SetupSingleCostElement(this, "Cost", deployableVO.UpgradeCredits, deployableVO.UpgradeMaterials, deployableVO.UpgradeContraband, 0, 0, false, null);
				}
			}
			DeployableInfoUIType upgradeInfoLevel = DeployableInfoUIType.None;
			bool flag9 = this.IsNotMaxLevel(this.nextLevel) && !string.IsNullOrEmpty(deployableVO.UpgradeShardUid);
			if (flag4)
			{
				if (flag9 && flag7 && flag2)
				{
					upgradeInfoLevel = DeployableInfoUIType.AskOnly;
				}
				else if (flag9 && !flag7 && !flag2)
				{
					upgradeInfoLevel = DeployableInfoUIType.InfoOnly;
				}
				else if (flag7 || !flag2)
				{
					upgradeInfoLevel = DeployableInfoUIType.All;
				}
				if (flag9)
				{
					element4.Visible = visible;
					element4.Enabled = (flag7 && flag6);
					base.GetElement<UXLabel>("LabelNormalIntro").Text = this.lang.Get("LABEL_REWARD_UPGRADE", new object[0]);
				}
			}
			TroopUniqueAbilityDescVO descVO = null;
			if (troop is TroopTypeVO)
			{
				TroopTypeVO troopTypeVO = this.selectedTroop.Troop as TroopTypeVO;
				descVO = troopTypeVO.UniqueAbilityDescVO;
				this.RefreshTroopView(troopTypeVO, upgradeInfoLevel);
			}
			else if (troop is SpecialAttackTypeVO)
			{
				this.RefreshStarshipView(troop as SpecialAttackTypeVO, upgradeInfoLevel);
			}
			base.GetElement<UXElement>("ItemStatus").Visible = false;
			this.SetupLeftTableAltAbilityItem(descVO);
		}

		private void SetupTroopImage()
		{
			UXUtils.HideQualityCards(this, "TroopImage", "TroopImageQ{0}");
			IDeployableVO deployableVO = (this.nextLevel == null) ? this.selectedTroop.Troop : this.nextLevel;
			ShardVO shardVO = null;
			if (!string.IsNullOrEmpty(deployableVO.UpgradeShardUid))
			{
				shardVO = Service.StaticDataController.GetOptional<ShardVO>(deployableVO.UpgradeShardUid);
			}
			string name = "SpriteTroopSelectedItemImage";
			base.GetElement<UXLabel>("LabelProgress").Text = string.Empty;
			if (shardVO != null)
			{
				int quality = (int)shardVO.Quality;
				base.GetElement<UXElement>(string.Format("TroopImageQ{0}", quality)).Visible = true;
				base.GetElement<UXLabel>(string.Format("LabelQualityQ{0}", quality)).Text = LangUtils.GetShardQuality(shardVO.Quality);
				name = string.Format("SpriteTroopSelectedItemImageQ{0}", quality);
				this.SetupShardProgressBar(quality);
				base.SetupFragmentSprite(quality);
			}
			else
			{
				this.fragmentSprite.Visible = false;
				base.GetElement<UXSlider>("pBarFrag").Visible = false;
			}
			base.GetElement<UXElement>("TroopImage").Visible = (shardVO == null);
			UXSprite element = base.GetElement<UXSprite>(name);
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(deployableVO, element, true);
			projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			this.troopImage = ProjectorUtils.GenerateProjector(projectorConfig);
			FactionDecal.SetDeployableDecalVisibiliy(this, false);
		}

		private void SetupShardProgressBar(int quality)
		{
			UXSlider element = base.GetElement<UXSlider>("pBarFrag");
			UXSprite element2 = base.GetElement<UXSprite>("SpritepBarFrag");
			UXLabel element3 = base.GetElement<UXLabel>("LabelProgress");
			UnlockController unlockController = Service.UnlockController;
			bool flag = !unlockController.IsMinLevelUnlocked(this.selectedTroop.Troop);
			element.Visible = true;
			if (this.nextLevel == null)
			{
				element3.Text = this.lang.Get("MAX_LEVEL", new object[0]);
				element.Value = 0f;
				return;
			}
			IDeployableVO deployableVO = this.nextLevel;
			if (flag)
			{
				deployableVO = this.currentLevel;
			}
			int shardAmount = Service.DeployableShardUnlockController.GetShardAmount(this.nextLevel.UpgradeShardUid);
			int upgradeShardCount = deployableVO.UpgradeShardCount;
			element3.Text = this.lang.Get("FRACTION", new object[]
			{
				shardAmount,
				upgradeShardCount
			});
			if (upgradeShardCount == 0)
			{
				Service.Logger.ErrorFormat("CMS Error: Shards required for {0} is zero", new object[]
				{
					this.nextLevel.Uid
				});
				return;
			}
			float num = (float)shardAmount / (float)upgradeShardCount;
			UXUtils.SetShardProgressBarValue(element, element2, num);
			if (num >= 1f)
			{
				base.GetElement<UXElement>("IconUpgrade").Visible = true;
			}
		}

		private string GetOwnLabelID()
		{
			string result = "LabelQuantityOwn";
			IDeployableVO deployableVO = (this.nextLevel == null) ? this.selectedTroop.Troop : this.nextLevel;
			ShardVO shardVO = null;
			if (!string.IsNullOrEmpty(deployableVO.UpgradeShardUid))
			{
				shardVO = Service.StaticDataController.GetOptional<ShardVO>(deployableVO.UpgradeShardUid);
			}
			if (shardVO != null)
			{
				int quality = (int)shardVO.Quality;
				result = string.Format("LabelQuantityOwnQ{0}", quality);
			}
			return result;
		}

		private void RefreshStarshipViewUpgradeAsk(SpecialAttackTypeVO nextLevelVO)
		{
			base.GetElement<UXLabel>("DialogBldgUpgradeTitle").Text = this.lang.Get("BUILDING_UPGRADE", new object[]
			{
				LangUtils.GetStarshipDisplayName(nextLevelVO),
				this.nextLevel.Lvl
			});
			base.GetElement<UXLabel>(this.GetOwnLabelID()).Text = string.Empty;
		}

		private void RefreshStarshipViewNoUpgradeAsk(SpecialAttackTypeVO specialAttack)
		{
			base.GetElement<UXLabel>("DialogBldgUpgradeTitle").Text = this.lang.Get("BUILDING_INFO", new object[]
			{
				LangUtils.GetStarshipDisplayName(specialAttack),
				specialAttack.Lvl
			});
			int itemAmount = Service.CurrentPlayer.Inventory.SpecialAttack.GetItemAmount(specialAttack.Uid);
			base.GetElement<UXLabel>(this.GetOwnLabelID()).Text = this.lang.Get("numOwned", new object[]
			{
				this.lang.ThousandsSeparated(itemAmount)
			});
			base.GetElement<UXLabel>("LabelTrainingTimeIncrease").Text = string.Empty;
		}

		private void RefreshStarshipView(SpecialAttackTypeVO specialAttack, DeployableInfoUIType upgradeInfoLevel)
		{
			int nextValue = 0;
			SpecialAttackTypeVO specialAttackTypeVO = (SpecialAttackTypeVO)this.nextLevel;
			if (upgradeInfoLevel > DeployableInfoUIType.None)
			{
				if (upgradeInfoLevel == DeployableInfoUIType.InfoOnly)
				{
					this.RefreshStarshipViewNoUpgradeAsk(specialAttack);
				}
				else
				{
					this.RefreshStarshipViewUpgradeAsk(specialAttackTypeVO);
				}
				if (upgradeInfoLevel != DeployableInfoUIType.AskOnly)
				{
					int num = specialAttackTypeVO.TrainingTime - specialAttack.TrainingTime;
					string id = (num < 0) ? "MINUS" : "PLUS";
					base.GetElement<UXLabel>("LabelTrainingTimeIncrease").Text = this.lang.Get(id, new object[]
					{
						GameUtils.GetTimeLabelFromSeconds(num)
					});
					nextValue = specialAttackTypeVO.DPS;
				}
			}
			else
			{
				this.RefreshStarshipViewNoUpgradeAsk(specialAttack);
			}
			base.GetElement<UXLabel>("LabelTroopInfo").Text = LangUtils.GetStarshipDescription(specialAttack);
			StaticDataController staticDataController = Service.StaticDataController;
			SpecialAttackTypeVO maxLevel = Service.StarshipUpgradeCatalog.GetMaxLevel(specialAttack);
			this.maxSpecialAttackDps = maxLevel.DPS;
			if (specialAttack.IsDropship)
			{
				bool flag = upgradeInfoLevel > DeployableInfoUIType.None;
				TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(specialAttack.LinkedUnit);
				TroopTypeVO troopTypeVO2 = null;
				TroopTypeVO troopTypeVO3 = staticDataController.Get<TroopTypeVO>(maxLevel.LinkedUnit);
				if (specialAttackTypeVO != null)
				{
					troopTypeVO2 = staticDataController.Get<TroopTypeVO>(specialAttackTypeVO.LinkedUnit);
				}
				else
				{
					flag = false;
				}
				if (flag && troopTypeVO2 == null)
				{
					Service.Logger.ErrorFormat("Invaild Dropship Troop: {0}, on Special Attack: {1}", new object[]
					{
						specialAttackTypeVO.LinkedUnit,
						specialAttackTypeVO.Uid
					});
				}
				this.SetupBar(1, this.lang.Get("TROOP_UNITS", new object[0]), (int)specialAttack.UnitCount, (int)((!flag) ? 0u : specialAttackTypeVO.UnitCount), (int)maxLevel.UnitCount);
				this.SetupBar(2, this.lang.Get("PER_UNIT_DAMAGE", new object[0]), troopTypeVO.DPS, (!flag) ? 0 : troopTypeVO2.DPS, troopTypeVO3.DPS);
				this.SetupBar(3, this.lang.Get("PER_UNIT_HEALTH", new object[0]), troopTypeVO.Health, (!flag) ? 0 : troopTypeVO2.Health, troopTypeVO3.Health);
				this.movementSpeedNameLabel.Visible = false;
				this.movementSpeedValueLabel.Visible = false;
				this.SetAttackRange(0u, 0u);
			}
			else
			{
				string id2 = "s_Damage";
				if (specialAttack.InfoUIType == InfoUIType.Healer)
				{
					id2 = "HEALING_POWER";
				}
				else if (specialAttack.InfoUIType == InfoUIType.HealerPercent)
				{
					id2 = "s_HealerPercent";
				}
				else if (specialAttack.InfoUIType == InfoUIType.DamageBuff)
				{
					id2 = "s_DamageBuff";
				}
				this.SetupBar(1, this.lang.Get(id2, new object[0]), specialAttack.DPS, nextValue, this.maxSpecialAttackDps);
				this.SetupBar(2, null, 0, 0, 0);
				this.SetupBar3();
				this.movementSpeedNameLabel.Visible = true;
				this.movementSpeedValueLabel.Visible = true;
				this.movementSpeedValueLabel.Text = this.lang.ThousandsSeparated(specialAttack.MaxSpeed);
				this.attackRangeNameLabel.Text = this.lang.Get("PREFERRED_TARGET", new object[0]);
				this.attackRangeValueLabel.Text = this.lang.Get("target_pref_" + specialAttack.FavoriteTargetType, new object[0]);
			}
			this.unitCapacityValueLabel.Text = this.lang.ThousandsSeparated(specialAttack.Size);
			this.trainingTimeValueLabel.Text = GameUtils.GetTimeLabelFromSeconds(specialAttack.TrainingTime);
			int credits = specialAttack.Credits;
			int materials = specialAttack.Materials;
			int contraband = specialAttack.Contraband;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			BuildingTypeVO minBuildingRequirement = buildingLookupController.GetMinBuildingRequirement(specialAttack);
			float contractCostMultiplier = Service.PerkManager.GetContractCostMultiplier(minBuildingRequirement);
			GameUtils.MultiplyCurrency(contractCostMultiplier, ref credits, ref materials, ref contraband);
			this.trainingCostValueLabel.Text = this.lang.ThousandsSeparated(credits);
			for (int i = 0; i < 4; i++)
			{
				this.SetupLeftTableItem(i, null, null);
			}
			if (specialAttack.UnlockedByEvent)
			{
				this.SetupAndEnableUnlockUnitGalaxyUI(specialAttack);
			}
			else
			{
				this.HideUnlockUnitGalaxyUI();
			}
			this.SetTrainingCost(specialAttack, upgradeInfoLevel > DeployableInfoUIType.None);
		}

		private void RefreshTroopViewUpgradeAsk(TroopTypeVO nextLevelVO)
		{
			base.GetElement<UXLabel>("DialogBldgUpgradeTitle").Text = this.lang.Get("BUILDING_UPGRADE", new object[]
			{
				LangUtils.GetTroopDisplayName(nextLevelVO),
				nextLevelVO.Lvl
			});
			base.GetElement<UXLabel>(this.GetOwnLabelID()).Text = string.Empty;
		}

		private void RefreshTroopViewNoUpgradeAsk(TroopTypeVO troop)
		{
			base.GetElement<UXLabel>("DialogBldgUpgradeTitle").Text = this.lang.Get("BUILDING_INFO", new object[]
			{
				LangUtils.GetTroopDisplayName(troop),
				troop.Lvl
			});
			Inventory inventory = Service.CurrentPlayer.Inventory;
			TroopType type = troop.Type;
			int itemAmount;
			if (type != TroopType.Hero)
			{
				if (type != TroopType.Champion)
				{
					itemAmount = inventory.Troop.GetItemAmount(troop.Uid);
				}
				else
				{
					itemAmount = inventory.Champion.GetItemAmount(troop.Uid);
				}
			}
			else
			{
				itemAmount = inventory.Hero.GetItemAmount(troop.Uid);
			}
			base.GetElement<UXLabel>(this.GetOwnLabelID()).Text = this.lang.Get("numOwned", new object[]
			{
				this.lang.ThousandsSeparated(itemAmount)
			});
			base.GetElement<UXLabel>("LabelTrainingTimeIncrease").Text = string.Empty;
		}

		private void RefreshTroopView(TroopTypeVO troop, DeployableInfoUIType upgradeInfoLevel)
		{
			int nextValue = 0;
			int nextValue2 = 0;
			TroopTypeVO troopTypeVO = (TroopTypeVO)this.nextLevel;
			if (upgradeInfoLevel > DeployableInfoUIType.None)
			{
				if (upgradeInfoLevel == DeployableInfoUIType.InfoOnly)
				{
					this.RefreshTroopViewNoUpgradeAsk(troop);
				}
				else
				{
					this.RefreshTroopViewUpgradeAsk(troopTypeVO);
				}
				if (upgradeInfoLevel != DeployableInfoUIType.AskOnly)
				{
					int num = troopTypeVO.TrainingTime - troop.TrainingTime;
					string id = (num < 0) ? "MINUS" : "PLUS";
					base.GetElement<UXLabel>("LabelTrainingTimeIncrease").Text = this.lang.Get(id, new object[]
					{
						GameUtils.GetTimeLabelFromSeconds(num)
					});
					nextValue = troopTypeVO.DPS;
					nextValue2 = troopTypeVO.Health;
				}
			}
			else
			{
				this.RefreshTroopViewNoUpgradeAsk(troop);
			}
			base.GetElement<UXLabel>("LabelTroopInfo").Text = LangUtils.GetTroopDescription(troop);
			TroopTypeVO maxLevel = Service.TroopUpgradeCatalog.GetMaxLevel(troop);
			this.maxTroopDps = maxLevel.DPS;
			this.maxHealth = maxLevel.Health;
			string id2 = "s_Damage";
			if (troop.InfoUIType == InfoUIType.Healer)
			{
				id2 = "HEALING_POWER";
			}
			else if (troop.InfoUIType == InfoUIType.HealerPercent)
			{
				id2 = "s_HealerPercent";
			}
			else if (troop.InfoUIType == InfoUIType.DamageBuff)
			{
				id2 = "s_DamageBuff";
			}
			this.SetupBar(1, this.lang.Get(id2, new object[0]), troop.DPS, nextValue, this.maxTroopDps);
			this.SetupBar(2, this.lang.Get("HEALTH", new object[]
			{
				string.Empty
			}), troop.Health, nextValue2, this.maxHealth);
			this.SetupBar3();
			this.SetupLeftTableItem(0, this.lang.Get("TRAINING_CLASS_TYPE", new object[0]), this.lang.Get("trp_class_" + troop.TroopRole.ToString(), new object[0]));
			this.SetupLeftTableItem(1, this.lang.Get("DAMAGE_TYPE", new object[0]), this.lang.Get((troop.ProjectileType.SplashRadius <= 0) ? "DAMAGE_TYPE_STANDARD" : "DAMAGE_TYPE_SPLASH", new object[0]));
			this.SetupLeftTableItem(2, this.lang.Get("FAVORITE_TARGET", new object[0]), this.lang.Get("target_pref_" + troop.FavoriteTargetType, new object[0]));
			if (troop.Type == TroopType.Hero && !string.IsNullOrEmpty(troop.Ability))
			{
				TroopAbilityVO abilityInfo = Service.StaticDataController.Get<TroopAbilityVO>(troop.Ability);
				this.SetupLeftTableItem(3, this.lang.Get("s_HeroPower", new object[0]), LangUtils.GetHeroAbilityDisplayName(abilityInfo));
			}
			else
			{
				this.SetupLeftTableItem(3, null, null);
			}
			this.SetAttackRange(troop.MinAttackRange, troop.MaxAttackRange);
			this.movementSpeedValueLabel.Text = this.lang.ThousandsSeparated(troop.MaxSpeed);
			this.unitCapacityValueLabel.Text = this.lang.ThousandsSeparated(troop.Size);
			float contractTimeReductionMultiplier = Service.PerkManager.GetContractTimeReductionMultiplier(troop);
			int num2 = Mathf.FloorToInt((float)troop.TrainingTime * contractTimeReductionMultiplier);
			this.trainingTimeValueLabel.Text = GameUtils.GetTimeLabelFromSeconds(num2);
			if (num2 != troop.TrainingTime)
			{
				if (troop.Type == TroopType.Champion)
				{
					this.perkEffectCost.Visible = true;
				}
				else
				{
					this.perkEffectTime.Visible = true;
				}
			}
			if (troop.UnlockedByEvent)
			{
				this.SetupAndEnableUnlockUnitGalaxyUI(troop);
			}
			else
			{
				this.HideUnlockUnitGalaxyUI();
			}
			this.SetTrainingCost(troop, upgradeInfoLevel > DeployableInfoUIType.None);
		}

		protected override void SetupPerksButton()
		{
			UXButton element = base.GetElement<UXButton>("btnPerks");
			element.Visible = false;
		}

		private void SetupLeftTableAltAbilityItem(TroopUniqueAbilityDescVO descVO)
		{
			bool flag = descVO != null;
			base.GetElement<UXElement>("InfoRow3alt").Visible = flag;
			if (flag)
			{
				base.GetElement<UXLabel>("InfoTitle3alt").Text = this.lang.Get("s_SpecialAbility", new object[0]);
				string text = this.lang.Get(descVO.AbilityTitle1, new object[0]);
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(descVO.AbilityTitle2))
				{
					text = text + "\n\n" + this.lang.Get(descVO.AbilityTitle2, new object[0]);
				}
				base.GetElement<UXLabel>("InfoDetail3alt").Text = text;
			}
		}

		private void SetTrainingCost(IDeployableVO vo, bool showUpgradeCostIncrease)
		{
			int num = 0;
			int contraband = vo.Contraband;
			int materials = vo.Materials;
			int credits = vo.Credits;
			int num2 = (this.nextLevel != null) ? this.nextLevel.Contraband : 0;
			int num3 = (this.nextLevel != null) ? this.nextLevel.Materials : 0;
			int num4 = (this.nextLevel != null) ? this.nextLevel.Credits : 0;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			PerkManager perkManager = Service.PerkManager;
			BuildingTypeVO minBuildingRequirement = buildingLookupController.GetMinBuildingRequirement(vo);
			float contractCostMultiplier = perkManager.GetContractCostMultiplier(minBuildingRequirement);
			GameUtils.MultiplyCurrency(contractCostMultiplier, ref credits, ref materials, ref contraband);
			GameUtils.MultiplyCurrency(contractCostMultiplier, ref num4, ref num3, ref num2);
			if (vo.Contraband > 0)
			{
				this.trainingCostIcon.SpriteName = "icoContraband";
				this.trainingCostValueLabel.Text = this.lang.ThousandsSeparated(contraband);
				if (showUpgradeCostIncrease)
				{
					num = num2 - contraband;
				}
			}
			else if (vo.Materials > 0)
			{
				this.trainingCostIcon.SpriteName = "icoDialogMaterials";
				this.trainingCostValueLabel.Text = this.lang.ThousandsSeparated(materials);
				if (showUpgradeCostIncrease)
				{
					num = num3 - materials;
				}
			}
			else
			{
				this.trainingCostIcon.SpriteName = "icoCollectCredit";
				this.trainingCostValueLabel.Text = this.lang.ThousandsSeparated(credits);
				if (showUpgradeCostIncrease)
				{
					num = num4 - credits;
				}
			}
			if (perkManager.IsContractCostMultiplierAppliedToBuilding(minBuildingRequirement))
			{
				this.perkEffectCost.Visible = true;
			}
			this.trainingCostNextValueLabel.Visible = showUpgradeCostIncrease;
			if (showUpgradeCostIncrease)
			{
				string id = (num < 0) ? "MINUS" : "PLUS";
				base.GetElement<UXLabel>("LabelTrainingCostIncrease").Text = this.lang.Get(id, new object[]
				{
					num
				});
			}
		}

		private void SetAttackRange(uint minAttackRange, uint maxAttackRange)
		{
			UXLabel uXLabel = this.attackRangeNameLabel;
			UXLabel uXLabel2 = this.attackRangeValueLabel;
			if (maxAttackRange == 0u)
			{
				uXLabel.Text = string.Empty;
				uXLabel2.Text = string.Empty;
			}
			else
			{
				uXLabel.Text = this.lang.Get("RANGE", new object[0]);
				uXLabel2.Text = ((minAttackRange != 0u) ? this.lang.Get("TILE_RANGE", new object[]
				{
					minAttackRange,
					maxAttackRange
				}) : this.lang.Get("TILE_COUNT", new object[]
				{
					maxAttackRange
				}));
			}
		}

		protected virtual void SetupBar3()
		{
			this.SetupBar(3, null, 0, 0, 0);
		}

		protected void SetupBar(int index, string labelString, int currentValue, int nextValue, int maxValue)
		{
			bool flag = labelString != null;
			base.GetElement<UXElement>(string.Format("pBar{0}", index)).Visible = flag;
			if (!flag)
			{
				return;
			}
			UXLabel element = base.GetElement<UXLabel>(string.Format("LabelpBar{0}", index));
			element.Text = labelString;
			UXSlider element2 = base.GetElement<UXSlider>(string.Format("pBarCurrent{0}", index));
			UXSlider element3 = base.GetElement<UXSlider>(string.Format("pBarNext{0}", index));
			element3.Visible = (nextValue > currentValue);
			element2.Value = MathUtils.NormalizeRange((float)currentValue, 0f, (float)maxValue);
			element3.Value = MathUtils.NormalizeRange((float)nextValue, 0f, (float)maxValue);
			UXLabel element4 = base.GetElement<UXLabel>(string.Format("LabelpBar{0}Current", index));
			UXLabel element5 = base.GetElement<UXLabel>(string.Format("LabelpBar{0}Next", index));
			element5.Visible = (nextValue > currentValue);
			element4.Text = this.lang.ThousandsSeparated(currentValue);
			element5.Text = this.lang.Get("PLUS", new object[]
			{
				this.lang.ThousandsSeparated(nextValue - currentValue)
			});
		}

		private void SetupLeftTableItem(int index, string title, string desc)
		{
			bool flag = !string.IsNullOrEmpty(title);
			base.GetElement<UXElement>(string.Format("InfoRow{0}", index)).Visible = flag;
			if (flag)
			{
				base.GetElement<UXLabel>(string.Format("InfoTitle{0}", index)).Text = title;
				base.GetElement<UXLabel>(string.Format("InfoDetail{0}", index)).Text = desc;
			}
		}

		private void OnBackButtonClicked(UXButton button)
		{
			this.ToggleParentScreenVisibility(true);
			this.shouldCloseParent = false;
			this.Close(null);
		}

		private void ToggleParentScreenVisibility(bool visible)
		{
			ClosableScreen deployableInfoParentScreen = ScreenUtils.GetDeployableInfoParentScreen();
			if (deployableInfoParentScreen != null)
			{
				deployableInfoParentScreen.SetVisibilityAndRefresh(visible, this.uiWasRefreshed);
			}
		}

		private void OnPrevOrNextButtonClicked(UXButton button)
		{
			int num = (int)button.Tag;
			int count = this.troopList.Count;
			int index = (num >= 0) ? 0 : (count - 1);
			TroopUpgradeTag troopUpgradeTag = this.troopList[index];
			for (int i = count - 1; i >= 0; i--)
			{
				int index2 = (num >= 0) ? i : (count - 1 - i);
				TroopUpgradeTag troopUpgradeTag2 = this.troopList[index2];
				if (troopUpgradeTag2.Troop == this.selectedTroop.Troop)
				{
					break;
				}
				troopUpgradeTag = troopUpgradeTag2;
			}
			this.selectedTroop = troopUpgradeTag;
			this.currentLevel = this.selectedTroop.Troop;
			this.nextLevel = this.GetNextDeployable(this.selectedTroop.Troop);
			this.Redraw();
		}

		private void StartUpgrading()
		{
			if (this.nextLevel is TroopTypeVO)
			{
				Service.ISupportController.StartTroopUpgrade((TroopTypeVO)this.nextLevel, this.selectedBuilding);
			}
			else if (this.nextLevel is SpecialAttackTypeVO)
			{
				Service.ISupportController.StartStarshipUpgrade((SpecialAttackTypeVO)this.nextLevel, this.selectedBuilding);
			}
			this.CloseFromResearchScreen();
		}

		private void CloseFromResearchScreen()
		{
			this.Close(this.selectedBuilding.ID);
			TroopUpgradeScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<TroopUpgradeScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.Close(this.selectedBuilding.ID);
			}
			this.RefreshResearchContextButtons();
		}

		private void RefreshResearchContextButtons()
		{
			SmartEntity selectedBuilding = Service.BuildingController.SelectedBuilding;
			if (selectedBuilding == null)
			{
				return;
			}
			BuildingComponent buildingComp = selectedBuilding.BuildingComp;
			if (buildingComp == null)
			{
				return;
			}
			BuildingTypeVO buildingType = buildingComp.BuildingType;
			if (buildingType.Type == BuildingType.TroopResearch)
			{
				Service.UXController.HUD.ShowContextButtons(selectedBuilding);
			}
		}

		protected virtual void OnPurchaseClicked(UXButton button)
		{
			if (this.selectedTroop == null)
			{
				return;
			}
			if (this.activeContract != null)
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(this.lang.Get("UPGRADE_CONTRACT_ACTIVE", new object[0]));
				return;
			}
			if (this.selectedBuilding == null)
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(this.lang.Get("UPGRADE_RESEARCH_CENTER_ACTIVE", new object[0]));
				return;
			}
			IUpgradeableVO upgradeableVO = this.nextLevel;
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			string value = string.Empty;
			int value2 = 0;
			if (upgradeableVO is SpecialAttackTypeVO)
			{
				text = (upgradeableVO as SpecialAttackTypeVO).SpecialAttackID;
				value = text;
				value2 = (upgradeableVO as SpecialAttackTypeVO).Lvl;
			}
			else if (upgradeableVO is TroopTypeVO)
			{
				text = (upgradeableVO as TroopTypeVO).Type.ToString();
				value = (upgradeableVO as TroopTypeVO).TroopID;
				value2 = (upgradeableVO as TroopTypeVO).Lvl;
			}
			string value3 = StringUtils.ToLowerCaseUnderscoreSeperated(text);
			stringBuilder.Append(value3);
			stringBuilder.Append("|");
			stringBuilder.Append(value);
			stringBuilder.Append("|");
			stringBuilder.Append(value2);
			stringBuilder.Append("|research");
			if (!PayMeScreen.ShowIfNotEnoughCurrency(upgradeableVO.UpgradeCredits, upgradeableVO.UpgradeMaterials, upgradeableVO.UpgradeContraband, stringBuilder.ToString(), new OnScreenModalResult(this.OnPayMeForCurrencyResult)))
			{
				this.StartUpgrading();
			}
		}

		private void OnGoToGalaxyClicked(UXButton btn)
		{
			Service.EventManager.SendEvent(EventId.UnitInfoGoToGalaxy, this.selectedTroop.Troop.Uid);
			DeployableInfoActionButtonTag tag = (DeployableInfoActionButtonTag)btn.Tag;
			bool flag = Service.BuildingLookupController.GetCurrentNavigationCenter() != null;
			IState currentState = Service.GameStateMachine.CurrentState;
			if (!flag)
			{
				string instructions = this.lang.Get("PLANETS_PLAY_BUILD_PC", new object[0]);
				Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(instructions);
			}
			else if (currentState is HomeState)
			{
				Service.ScreenController.CloseAll();
				this.GoToGalaxyFromHomeState(tag);
			}
			else if (currentState is GalaxyState)
			{
				this.GoToGalaxyFromGalaxyState(tag);
			}
			else
			{
				Service.Logger.Error("Attempt to go to galaxy from invalide state " + currentState.ToString());
			}
		}

		private void GoToGalaxyFromHomeState(DeployableInfoActionButtonTag tag)
		{
			GalaxyViewController galaxyViewController = Service.GalaxyViewController;
			string actionId = tag.ActionId;
			string planetUID = tag.DataList[0];
			this.Close(null);
			if (actionId == "planet")
			{
				galaxyViewController.GoToPlanetView(planetUID, CampaignScreenSection.Main);
			}
			else if (actionId == "galaxy")
			{
				galaxyViewController.GoToGalaxyView(planetUID);
			}
		}

		private void GoToGalaxyFromGalaxyState(DeployableInfoActionButtonTag tag)
		{
			GalaxyViewController galaxyViewController = Service.GalaxyViewController;
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			ScreenController screenController = Service.ScreenController;
			string actionId = tag.ActionId;
			string planetUID = tag.DataList[0];
			base.CloseNoTransition(null);
			galaxyViewController.TransitionToPlanet(galaxyPlanetController.GetPlanet(planetUID), true);
			TournamentTiersScreen highestLevelScreen = screenController.GetHighestLevelScreen<TournamentTiersScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.CloseNoTransition(null);
			}
			if (actionId == "planet")
			{
				PlanetLootTableScreen highestLevelScreen2 = screenController.GetHighestLevelScreen<PlanetLootTableScreen>();
				if (highestLevelScreen2 != null)
				{
					highestLevelScreen2.CloseNoTransition(null);
				}
			}
			else if (actionId == "galaxy")
			{
				PlanetDetailsScreen highestLevelScreen3 = screenController.GetHighestLevelScreen<PlanetDetailsScreen>();
				if (highestLevelScreen3 != null)
				{
					highestLevelScreen3.GoToGalaxyFromPlanetScreen();
				}
			}
		}

		private void OnFinishClicked(UXButton button)
		{
			if (this.activeContract == null)
			{
				return;
			}
			int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(this.activeContract);
			if (crystalCostToFinishContract >= GameConstants.CRYSTAL_SPEND_WARNING_MINIMUM)
			{
				FinishNowScreen.ShowModal(this.selectedBuilding, new OnScreenModalResult(this.FinishContract), null);
			}
			else
			{
				this.FinishContract(this.selectedBuilding, null);
			}
		}

		private void FinishContract(object result, object cookie)
		{
			if (this.activeContract == null || result == null)
			{
				return;
			}
			int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(this.activeContract);
			if (!GameUtils.SpendCrystals(crystalCostToFinishContract))
			{
				return;
			}
			Service.ISupportController.BuyOutCurrentBuildingContract(this.selectedBuilding, true);
			BuildingComponent buildingComp = this.selectedBuilding.BuildingComp;
			if (buildingComp != null)
			{
				BuildingTypeVO buildingType = buildingComp.BuildingType;
				if (buildingType != null)
				{
					int currencyAmount = -crystalCostToFinishContract;
					string itemType = StringUtils.ToLowerCaseUnderscoreSeperated(buildingType.Type.ToString());
					string buildingID = buildingType.BuildingID;
					int itemCount = 1;
					string type = "speed_up_building";
					string subType = "consumable";
					Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, buildingID, itemCount, type, subType);
				}
			}
			this.CloseFromResearchScreen();
		}

		private void OnPayMeForCurrencyResult(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				this.StartUpgrading();
			}
		}
	}
}
