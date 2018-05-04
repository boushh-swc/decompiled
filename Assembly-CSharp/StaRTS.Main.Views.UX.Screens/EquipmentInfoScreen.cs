using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class EquipmentInfoScreen : SelectedBuildingScreen, IViewFrameTimeObserver
	{
		private const int NUM_DETAIL_LEFT_ROWS = 4;

		private const int NUM_BARS = 3;

		private const string S_TIME_REMAINING = "s_TimeRemaining";

		protected const string BUTTON_BACK = "BtnBack";

		private const string BUTTON_TROOP_NEXT = "BtnTroopNext";

		private const string BUTTON_TROOP_PREV = "BtnTroopPrev";

		private const string LABEL_NAME = "DialogBldgUpgradeTitle";

		private const string LABEL_INFO = "LabelTroopInfo";

		private const string LABEL_REQUIREMENT = "LabelRequirement";

		private const string UPGRADE_GROUP = "ContainerUpgradeTime";

		private const string CONTEXT_FINISH_GROUP = "FinishCost";

		private const string BUTTON_PURCHASE = "ButtonPrimaryAction";

		private const string BUTTON_FINISH = "BtnFinish";

		private const string BUTTON_NORMAL = "BtnNormal";

		private const string TROOP_IMAGE = "TroopImage";

		private const string TROOP_IMAGE_FORMAT = "TroopImageQ{0}";

		private const string TROOP_IMAGE_BACKGROUND_FORMAT = "SpriteTroopImageBkgGridQ{0}";

		private const string PLANET_PANEL = "PanelPlanetAvailability";

		private const string LABEL_PLANET_AVAILABILITY = "LabelPlanetAvailability";

		private const string GRID_PLANET_AVAILABILITY = "GridPlanetAvailability";

		private const string TEMPLATE_PLANET = "TemplatePlanet";

		private const string LABEL_AVAILABLE_PLANET = "LabelAvailablePlanet";

		private const string TEXTURE_AVAILABLE_PLANET = "TextureAvailablePlanet";

		private const string BUILDING_REQUIREMENT = "BUILDING_REQUIREMENT";

		private const string LABEL_UPGRADE_TIME = "LabelUpgradeTime";

		private const string LABEL_UPGRADE_TIME_STATIC = "LabelUpgradeTimeStatic";

		private const string DAMAGE = "DAMAGE";

		private const string HEALTH = "HEALTH";

		private const string ICON_UPGRADE = "IconUpgrade";

		private const string SLIDER_NAME = "pBar{0}";

		private const string LABEL_PBAR = "LabelpBar{0}";

		private const string LABEL_PBAR_CUR = "LabelpBar{0}Current";

		private const string LABEL_PBAR_NEXT = "LabelpBar{0}Next";

		private const string SLIDER_CURRENT = "pBarCurrent{0}";

		private const string SLIDER_NEXT = "pBarNext{0}";

		private const string SLIDER_BG = "SpritepBarBkg{0}";

		private const string MOVEMENT_SPEED_GROUP = "MovementSpeed";

		private const string RANGE_GROUP = "Range";

		private const string TRAINING_TIME_GROUP = "TrainingTime";

		private const string TRAINING_COST_GROUP = "TrainingCost";

		private const string UNIT_CAPACITY_GROUP = "UnitCapacity";

		private const string SPRITE_EQUIPMENT = "SpriteTroopSelectedItemImageQ{0}";

		private const string LEFT_OWN_LABEL = "LabelQuantityOwnQ{0}";

		private const string LEFT_INFO_PROGRESS_LABEL = "LabelProgress";

		private const string SHARD_PROGRESS_BAR = "pBarFrag";

		private const string SHARD_PROGRESS_BAR_SPRITE = "SpritepBarFrag";

		private const string LEFT_INFO_GROUP = "InfoRow{0}";

		private const string LEFT_INFO_TITLE = "InfoTitle{0}";

		private const string LEFT_INFO_DETAIL = "InfoDetail{0}";

		private const string LEFT_INFO_GROUP3_ALT = "InfoRow3alt";

		private const string LEFT_LABEL_QUALITY = "LabelQualityQ{0}";

		private const string LEFT_UPGRADE_INSTRUCTIONS = "ItemStatus";

		private const string LABEL_NORMAL_INTRO = "LabelNormalIntro";

		private const string RESEARCH_LAB_ACTIVE_PLANETS = "RESEARCH_LAB_ACTIVE_PLANETS";

		private const string PLUS_DAMAGE_PERCENT = "perkEffect_descMod_PosPct";

		private const string LABEL_REWARD_UPGRADE = "LABEL_REWARD_UPGRADE";

		private const string EQUIPMENT_REQUIRES_BUILDING = "EQUIPMENT_REQUIRES_BUILDING";

		private const string BUILDING_UPGRADE = "BUILDING_UPGRADE";

		private const string EQUIPMENT_LOCKED = "EQUIPMENT_LOCKED";

		private const string MAX_LEVEL = "MAX_LEVEL";

		private const string BUILDING_INFO = "BUILDING_INFO";

		private const string FRACTION = "FRACTION";

		private const string S_UPGRADE_TIME = "s_upgradeTime";

		private const string PERCENTAGE = "PERCENTAGE";

		private const string AFFECTED_UNIT = "EQUIPMENT_INFO_AFFECTED_UNIT";

		private const string CAPACITY = "EQUIPMENT_INFO_CAPACITY";

		private const string ARMORY_ACTIVATE = "ARMORY_ACTIVATE";

		private const string ARMORY_DEACTIVATE = "ARMORY_DEACTIVATE";

		private const string ARMORY_INVALID_EQUIPMENT_PLANET = "ARMORY_INVALID_EQUIPMENT_PLANET";

		private const string ARMORY_NOT_ENOUGH_CAPACITY = "ARMORY_NOT_ENOUGH_CAPACITY";

		private const string EQUIPMENT_UPGRADE_REQ = "EQUIPMENT_UPGRADE_LOCKED";

		private const string UPGRADE_EQUIPMENT_IN_LAB = "ARMORY_UPGRADE_CTA";

		protected EquipmentVO selectedEquipment;

		private List<EquipmentVO> equipmentList;

		private EquipmentVO nextEquipmentVoUpgrade;

		protected GeometryProjector equipmentImage;

		protected bool wantsTransition;

		protected bool shouldCloseParent;

		protected bool forResearchLab;

		protected bool forArmoryScreen;

		private Contract activeContract;

		private bool timerActive;

		private float accumulatedUpdateDt;

		protected UXLabel labelUpgradeTime;

		protected override bool WantTransitions
		{
			get
			{
				return this.wantsTransition;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public EquipmentInfoScreen(EquipmentVO selectedEquipment, List<EquipmentVO> equipmentList, SmartEntity selectedBuilding, bool forResearchLab, bool forArmoryScreen) : base("gui_troop_info", selectedBuilding)
		{
			this.wantsTransition = false;
			this.shouldCloseParent = true;
			this.selectedEquipment = selectedEquipment;
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			this.nextEquipmentVoUpgrade = equipmentUpgradeCatalog.GetNextLevel(selectedEquipment);
			this.equipmentList = equipmentList;
			this.forResearchLab = forResearchLab;
			this.forArmoryScreen = forArmoryScreen;
			if (forResearchLab)
			{
				this.CheckActiveContract();
			}
			this.accumulatedUpdateDt = 0f;
		}

		public override void Close(object modalResult)
		{
			if (this.shouldCloseParent)
			{
				this.wantsTransition = true;
				ClosableScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<ArmoryScreen>();
				if (highestLevelScreen == null)
				{
					highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<TroopUpgradeScreen>();
				}
				if (highestLevelScreen != null)
				{
					highestLevelScreen.Close(null);
				}
			}
			base.Close(modalResult);
		}

		public override void OnDestroyElement()
		{
			this.labelUpgradeTime = null;
			Service.EventManager.UnregisterObserver(this, EventId.ContractCompleted);
			this.DisableTimers();
			if (this.equipmentImage != null)
			{
				this.equipmentImage.Destroy();
				this.equipmentImage = null;
			}
			this.equipmentList = null;
			this.selectedEquipment = null;
			this.nextEquipmentVoUpgrade = null;
			base.OnDestroyElement();
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			this.ToggleParentScreenVisibility(false);
			this.InitButtons();
			UXButton element = base.GetElement<UXButton>("BtnTroopPrev");
			UXButton element2 = base.GetElement<UXButton>("BtnTroopNext");
			if (this.equipmentList != null && this.equipmentList.Count > 1)
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
			this.labelUpgradeTime = base.GetElement<UXLabel>("LabelUpgradeTime");
			Service.EventManager.RegisterObserver(this, EventId.ContractCompleted);
			this.Redraw();
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ContractCompleted)
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				ContractType contractType = contractEventData.Contract.ContractTO.ContractType;
				if (contractType == ContractType.Research)
				{
					this.CheckActiveContract();
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
			if (this.nextEquipmentVoUpgrade != null && this.nextEquipmentVoUpgrade.Uid == this.activeContract.ProductUid)
			{
				int remainingTimeForView = this.activeContract.GetRemainingTimeForView();
				if (remainingTimeForView > 0)
				{
					this.labelUpgradeTime.Text = GameUtils.GetTimeLabelFromSeconds(remainingTimeForView);
					int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(this.activeContract);
					UXUtils.SetupCostElements(this, "FinishCost", null, 0, 0, 0, crystalCostToFinishContract, 0, !ArmoryUtils.IsBuildingRequirementMet(this.nextEquipmentVoUpgrade), null);
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

		private void Redraw()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			UXUtils.HideQualityCards(this, "TroopImage", "TroopImageQ{0}");
			int quality = (int)this.selectedEquipment.Quality;
			base.GetElement<UXElement>(string.Format("TroopImageQ{0}", quality)).Visible = true;
			base.GetElement<UXElement>(string.Format("SpriteTroopImageBkgGridQ{0}", quality)).Visible = true;
			base.GetElement<UXElement>("TroopImage").Visible = false;
			int quality2 = (int)this.selectedEquipment.Quality;
			base.GetElement<UXLabel>(string.Format("LabelQualityQ{0}", quality2)).Text = LangUtils.GetShardQuality(this.selectedEquipment.Quality);
			UXSprite element = base.GetElement<UXSprite>(string.Format("SpriteTroopSelectedItemImageQ{0}", quality2));
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateEquipmentConfig(this.selectedEquipment, element, true);
			projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			this.equipmentImage = ProjectorUtils.GenerateProjector(projectorConfig);
			this.SetProgressBarElements(currentPlayer, quality2);
			base.SetupFragmentSprite(quality2);
			base.GetElement<UXLabel>("LabelTroopInfo").Text = this.lang.Get(this.selectedEquipment.EquipmentDescription, new object[0]);
			base.GetElement<UXLabel>(string.Format("LabelQuantityOwnQ{0}", quality2)).Visible = false;
			this.PopulateAvailablePlanetsPanel(staticDataController);
			this.SetUpUpgradeElements(false);
			base.GetElement<UXElement>("MovementSpeed").Visible = false;
			base.GetElement<UXElement>("Range").Visible = false;
			base.GetElement<UXElement>("UnitCapacity").Visible = false;
			base.GetElement<UXElement>("TrainingTime").Visible = false;
			base.GetElement<UXElement>("TrainingCost").Visible = false;
			base.GetElement<UXElement>("InfoRow3alt").Visible = false;
			base.GetElement<UXButton>("BtnFinish").Visible = false;
			base.GetElement<UXButton>("ButtonPrimaryAction").Visible = false;
			this.DisplayBarsForEquipmentBuffs(staticDataController);
			if (this.forResearchLab)
			{
				this.SetUpResearchLabScreenInfo(staticDataController, currentPlayer);
			}
			else if (this.forArmoryScreen)
			{
				this.SetUpArmoryScreenInfo(currentPlayer);
			}
			else
			{
				base.GetElement<UXButton>("BtnNormal").Visible = false;
				base.GetElement<UXLabel>("LabelRequirement").Visible = false;
				UXLabel element2 = base.GetElement<UXLabel>("DialogBldgUpgradeTitle");
				this.SetTitleText(element2, "BUILDING_INFO", this.selectedEquipment.EquipmentName, this.selectedEquipment.Lvl);
			}
			for (int i = 0; i < 4; i++)
			{
				this.SetupLeftTableItem(i, null, null);
			}
			string affectedUnit = this.GetAffectedUnit();
			if (affectedUnit != null)
			{
				this.SetupLeftTableItem(0, "EQUIPMENT_INFO_AFFECTED_UNIT", affectedUnit);
			}
			this.SetupLeftTableItem(1, "EQUIPMENT_INFO_CAPACITY", this.selectedEquipment.Size.ToString());
		}

		private void SetUpUpgradeElements(bool upgradeVisible)
		{
			base.GetElement<UXElement>("ContainerUpgradeTime").Visible = upgradeVisible;
			if (!upgradeVisible)
			{
				return;
			}
			UXLabel element = base.GetElement<UXLabel>("LabelUpgradeTime");
			base.GetElement<UXLabel>("LabelUpgradeTimeStatic").Text = this.lang.Get("s_upgradeTime", new object[0]);
			element.Text = GameUtils.GetTimeLabelFromSeconds(this.nextEquipmentVoUpgrade.UpgradeTime);
		}

		private void SetProgressBarElements(CurrentPlayer player, int quality)
		{
			UXSlider element = base.GetElement<UXSlider>("pBarFrag");
			UXSprite element2 = base.GetElement<UXSprite>("SpritepBarFrag");
			UXLabel element3 = base.GetElement<UXLabel>("LabelProgress");
			UXLabel element4 = base.GetElement<UXLabel>("ItemStatus");
			EquipmentVO nextLevel = Service.EquipmentUpgradeCatalog.GetNextLevel(this.selectedEquipment);
			UXElement element5 = base.GetElement<UXElement>("IconUpgrade");
			string equipmentID = this.selectedEquipment.EquipmentID;
			element.Visible = true;
			element4.Visible = false;
			if (nextLevel == null)
			{
				element3.Text = this.lang.Get("MAX_LEVEL", new object[0]);
				element.Value = 0f;
				element5.Visible = false;
				return;
			}
			bool flag;
			if (this.forResearchLab)
			{
				flag = (this.activeContract != null && this.activeContract.ProductUid.Equals(nextLevel.Uid));
			}
			else
			{
				Contract contract = Service.ISupportController.FindFirstContractWithProductUid(nextLevel.Uid);
				flag = (contract != null);
			}
			if (flag)
			{
				element.Visible = false;
				element5.Visible = false;
				return;
			}
			int num = (!player.Shards.ContainsKey(equipmentID)) ? 0 : player.Shards[equipmentID];
			int upgradeShards;
			if (player.UnlockedLevels.Equipment.Has(this.selectedEquipment))
			{
				upgradeShards = nextLevel.UpgradeShards;
				element5.Visible = (num >= upgradeShards);
				if (!this.forResearchLab)
				{
					element4.Visible = true;
					element4.Text = ((num < upgradeShards) ? this.lang.Get("EQUIPMENT_UPGRADE_LOCKED", new object[]
					{
						this.CalculateFragmentsNeededForUnlock(nextLevel.UpgradeShards, this.selectedEquipment.EquipmentID)
					}) : this.lang.Get("ARMORY_UPGRADE_CTA", new object[0]));
				}
			}
			else
			{
				upgradeShards = this.selectedEquipment.UpgradeShards;
				element5.Visible = false;
			}
			element3.Text = this.lang.Get("FRACTION", new object[]
			{
				num,
				upgradeShards
			});
			if (upgradeShards == 0)
			{
				Service.Logger.ErrorFormat("CMS Error: Shards required for {0} is zero", new object[]
				{
					nextLevel.Uid
				});
				return;
			}
			float sliderValue = (float)num / (float)upgradeShards;
			UXUtils.SetShardProgressBarValue(element, element2, sliderValue);
		}

		private void DisplayBarsForEquipmentBuffs(StaticDataController sdc)
		{
			int num = 1;
			int num2 = this.selectedEquipment.EffectUids.Length;
			int i = 0;
			int num3 = num2;
			while (i < num3)
			{
				EquipmentEffectVO equipmentEffectVO = sdc.Get<EquipmentEffectVO>(this.selectedEquipment.EffectUids[i]);
				EquipmentEffectVO equipmentEffectVO2 = null;
				if (this.nextEquipmentVoUpgrade != null && this.nextEquipmentVoUpgrade.EffectUids.Length > 0 && this.nextEquipmentVoUpgrade.EffectUids.Length > i)
				{
					equipmentEffectVO2 = sdc.Get<EquipmentEffectVO>(this.nextEquipmentVoUpgrade.EffectUids[i]);
				}
				ArmorType armorType = ArmorType.Invalid;
				if (equipmentEffectVO.AffectedBuildingIds != null && equipmentEffectVO.AffectedBuildingIds.Length > 0)
				{
					BuildingTypeVO minLevel = Service.BuildingUpgradeCatalog.GetMinLevel(equipmentEffectVO.AffectedBuildingIds[0]);
					if (minLevel != null)
					{
						armorType = minLevel.ArmorType;
					}
				}
				else if (equipmentEffectVO.AffectedTroopIds != null && equipmentEffectVO.AffectedTroopIds.Length > 0)
				{
					TroopTypeVO minLevel2 = Service.TroopUpgradeCatalog.GetMinLevel(equipmentEffectVO.AffectedTroopIds[0]);
					if (minLevel2 != null)
					{
						armorType = minLevel2.ArmorType;
					}
				}
				int j = 0;
				int num4 = equipmentEffectVO.BuffUids.Length;
				while (j < num4)
				{
					BuffTypeVO buffTypeVO = sdc.Get<BuffTypeVO>(equipmentEffectVO.BuffUids[j]);
					BuffTypeVO buffTypeVO2 = null;
					if (equipmentEffectVO2 != null && equipmentEffectVO2.BuffUids.Length > 0 && equipmentEffectVO2.BuffUids.Length > j)
					{
						buffTypeVO2 = sdc.Get<BuffTypeVO>(equipmentEffectVO2.BuffUids[j]);
					}
					int num5 = buffTypeVO.Values[(int)armorType];
					int nextValue = num5;
					if (ArmoryUtils.IsEquipmentOwned(Service.CurrentPlayer, this.selectedEquipment))
					{
						nextValue = ((buffTypeVO2 != null) ? buffTypeVO2.Values[(int)armorType] : num5);
					}
					string buffString = this.GetBuffString(buffTypeVO.Modify);
					this.SetupBar(num, buffString, num5, nextValue, 1, false);
					num++;
					if (num >= 3)
					{
						break;
					}
					j++;
				}
				if (num >= 3)
				{
					break;
				}
				i++;
			}
			for (int k = num; k <= 3; k++)
			{
				base.GetElement<UXElement>(string.Format("pBar{0}", num)).Visible = false;
				num++;
			}
		}

		private void SetUpArmoryScreenInfo(CurrentPlayer player)
		{
			this.SetTitleText(base.GetElement<UXLabel>("DialogBldgUpgradeTitle"), "BUILDING_INFO", this.selectedEquipment.EquipmentName, this.selectedEquipment.Lvl);
			UXLabel element = base.GetElement<UXLabel>("LabelRequirement");
			element.Visible = true;
			UXLabel element2 = base.GetElement<UXLabel>("LabelNormalIntro");
			UXButton element3 = base.GetElement<UXButton>("BtnNormal");
			element3.Visible = true;
			element3.OnClicked = new UXButtonClickedDelegate(this.OnMainButtonClicked);
			if (!ArmoryUtils.IsEquipmentOwned(player, this.selectedEquipment))
			{
				element3.Enabled = false;
				element2.Text = this.lang.Get("ARMORY_ACTIVATE", new object[0]);
				element.Text = this.lang.Get("EQUIPMENT_LOCKED", new object[]
				{
					this.CalculateFragmentsNeededForUnlock(this.selectedEquipment.UpgradeShards, this.selectedEquipment.EquipmentID)
				});
				return;
			}
			if (ArmoryUtils.IsEquipmentActive(player, this.selectedEquipment))
			{
				element2.Text = this.lang.Get("ARMORY_DEACTIVATE", new object[0]);
				element3.Enabled = true;
			}
			else
			{
				bool flag = ArmoryUtils.HasEnoughCapacityToActivateEquipment(player.ActiveArmory, this.selectedEquipment);
				bool flag2 = ArmoryUtils.IsEquipmentValidForPlanet(this.selectedEquipment, player.PlanetId);
				element3.Enabled = (flag && flag2);
				element2.Text = this.lang.Get("ARMORY_ACTIVATE", new object[0]);
				if (!flag2)
				{
					string planetDisplayName = LangUtils.GetPlanetDisplayName(player.PlanetId);
					element.Text = this.lang.Get("ARMORY_INVALID_EQUIPMENT_PLANET", new object[]
					{
						planetDisplayName
					});
					return;
				}
				if (!flag)
				{
					element.Text = this.lang.Get("ARMORY_NOT_ENOUGH_CAPACITY", new object[0]);
					return;
				}
			}
			element.Text = string.Empty;
		}

		private void SetUpResearchLabScreenInfo(StaticDataController sdc, CurrentPlayer player)
		{
			UXLabel element = base.GetElement<UXLabel>("DialogBldgUpgradeTitle");
			UXButton element2 = base.GetElement<UXButton>("BtnNormal");
			element2.OnClicked = new UXButtonClickedDelegate(this.OnMainButtonClicked);
			UXButton element3 = base.GetElement<UXButton>("BtnFinish");
			element3.OnClicked = new UXButtonClickedDelegate(this.OnFinishClicked);
			UXLabel element4 = base.GetElement<UXLabel>("LabelRequirement");
			element4.Visible = true;
			element4.Text = string.Empty;
			if (this.nextEquipmentVoUpgrade != null)
			{
				ArmoryController armoryController = Service.ArmoryController;
				this.SetTitleText(element, "BUILDING_UPGRADE", this.selectedEquipment.EquipmentName, this.nextEquipmentVoUpgrade.Lvl);
				element2.Visible = true;
				base.GetElement<UXLabel>("LabelNormalIntro").Text = this.lang.Get("LABEL_REWARD_UPGRADE", new object[0]);
				if (this.activeContract != null)
				{
					if (this.activeContract.ProductUid == this.nextEquipmentVoUpgrade.Uid)
					{
						element3.Visible = true;
						element2.Visible = false;
						element4.Visible = false;
						base.GetElement<UXLabel>("LabelUpgradeTimeStatic").Text = this.lang.Get("s_TimeRemaining", new object[0]);
						base.GetElement<UXElement>("ContainerUpgradeTime").Visible = true;
						this.UpdateContractTimers();
					}
					else
					{
						element2.VisuallyDisableButton();
					}
				}
				else if (armoryController.IsEquipmentUpgradeable(this.selectedEquipment, this.nextEquipmentVoUpgrade))
				{
					element2.VisuallyEnableButton();
					element2.Enabled = true;
					this.SetUpUpgradeElements(true);
				}
				else
				{
					this.SetTitleText(element, "BUILDING_INFO", this.selectedEquipment.EquipmentName, this.selectedEquipment.Lvl);
					element2.Enabled = false;
					BuildingTypeVO buildingTypeVO = (this.nextEquipmentVoUpgrade.BuildingRequirement != null) ? sdc.Get<BuildingTypeVO>(this.nextEquipmentVoUpgrade.BuildingRequirement) : null;
					if (buildingTypeVO != null && !ArmoryUtils.IsBuildingRequirementMet(this.nextEquipmentVoUpgrade))
					{
						element4.Text = this.lang.Get("EQUIPMENT_REQUIRES_BUILDING", new object[]
						{
							LangUtils.GetBuildingDisplayName(buildingTypeVO),
							buildingTypeVO.Lvl
						});
					}
					else if (!ArmoryUtils.IsEquipmentOwned(player, this.selectedEquipment))
					{
						element4.Text = this.lang.Get("EQUIPMENT_LOCKED", new object[]
						{
							this.CalculateFragmentsNeededForUnlock(this.selectedEquipment.UpgradeShards, this.selectedEquipment.EquipmentID)
						});
					}
					else if (!ArmoryUtils.CanAffordEquipment(player, this.nextEquipmentVoUpgrade))
					{
						element4.Text = this.lang.Get("EQUIPMENT_UPGRADE_LOCKED", new object[]
						{
							this.CalculateFragmentsNeededForUnlock(this.nextEquipmentVoUpgrade.UpgradeShards, this.selectedEquipment.EquipmentID)
						});
					}
				}
			}
			else
			{
				element2.Visible = false;
				EquipmentVO maxLevel = Service.EquipmentUpgradeCatalog.GetMaxLevel(this.selectedEquipment);
				if (maxLevel == this.selectedEquipment)
				{
					this.SetTitleText(element, "BUILDING_INFO", this.selectedEquipment.EquipmentName, this.selectedEquipment.Lvl);
					element4.Text = this.lang.Get("MAX_LEVEL", new object[0]);
				}
			}
		}

		private void SetTitleText(UXLabel titleLabel, string titleKey, string nameKey, int level)
		{
			titleLabel.Text = this.lang.Get(titleKey, new object[]
			{
				this.lang.Get(nameKey, new object[0]),
				level
			});
		}

		private int CalculateFragmentsNeededForUnlock(int cost, string equipmentID)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (!currentPlayer.Shards.ContainsKey(equipmentID))
			{
				return cost;
			}
			return cost - currentPlayer.Shards[equipmentID];
		}

		private void PopulateAvailablePlanetsPanel(StaticDataController sdc)
		{
			UXElement element = base.GetElement<UXElement>("PanelPlanetAvailability");
			element.Visible = true;
			base.GetElement<UXLabel>("LabelPlanetAvailability").Text = this.lang.Get("RESEARCH_LAB_ACTIVE_PLANETS", new object[0]);
			UXGrid element2 = base.GetElement<UXGrid>("GridPlanetAvailability");
			element2.SetTemplateItem("TemplatePlanet");
			element2.Clear();
			int i = 0;
			int num = this.selectedEquipment.PlanetIDs.Length;
			while (i < num)
			{
				string uid = this.selectedEquipment.PlanetIDs[i];
				PlanetVO planetVO = sdc.Get<PlanetVO>(uid);
				UXElement item = element2.CloneTemplateItem(planetVO.Uid);
				element2.AddItem(item, planetVO.Order);
				element2.GetSubElement<UXLabel>(planetVO.Uid, "LabelAvailablePlanet").Text = LangUtils.GetPlanetDisplayName(uid);
				element2.GetSubElement<UXTexture>(planetVO.Uid, "TextureAvailablePlanet").LoadTexture(planetVO.LeaderboardButtonTexture);
				i++;
			}
			element2.RepositionItemsFrameDelayed();
		}

		private string GetAffectedUnit()
		{
			string[] effectUids = this.selectedEquipment.EffectUids;
			if (effectUids != null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				int i = 0;
				int num = effectUids.Length;
				while (i < num)
				{
					EquipmentEffectVO equipmentEffectVO = staticDataController.Get<EquipmentEffectVO>(effectUids[i]);
					if (equipmentEffectVO.AffectedTroopIds != null && equipmentEffectVO.AffectedTroopIds.Length > 0)
					{
						return this.lang.Get("trp_title_" + equipmentEffectVO.AffectedTroopIds[0], new object[0]);
					}
					if (equipmentEffectVO.AffectedSpecialAttackIds != null && equipmentEffectVO.AffectedSpecialAttackIds.Length > 0)
					{
						return this.lang.Get("shp_title_" + equipmentEffectVO.AffectedSpecialAttackIds[0], new object[0]);
					}
					if (equipmentEffectVO.AffectedBuildingIds != null && equipmentEffectVO.AffectedBuildingIds.Length > 0)
					{
						return this.lang.Get("bld_title_" + equipmentEffectVO.AffectedBuildingIds[0], new object[0]);
					}
					i++;
				}
			}
			return null;
		}

		private string GetBuffString(BuffModify buffType)
		{
			string text = string.Empty;
			if (buffType != BuffModify.Damage)
			{
				if (buffType != BuffModify.MaxHealth)
				{
					return text;
				}
				text = "HEALTH";
			}
			else
			{
				text = "DAMAGE";
			}
			return this.lang.Get(text, new object[]
			{
				string.Empty
			});
		}

		protected override void SetupPerksButton()
		{
			base.GetElement<UXButton>("btnPerks").Visible = false;
		}

		private void SetupLeftTableItem(int index, string title, string desc)
		{
			bool flag = !string.IsNullOrEmpty(title);
			base.GetElement<UXElement>(string.Format("InfoRow{0}", index)).Visible = flag;
			if (flag)
			{
				base.GetElement<UXLabel>(string.Format("InfoTitle{0}", index)).Text = this.lang.Get(title, new object[0]);
				base.GetElement<UXLabel>(string.Format("InfoDetail{0}", index)).Text = desc;
			}
		}

		private void OnMainButtonClicked(UXButton button)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (this.forResearchLab)
			{
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
				Service.ISupportController.StartEquipmentUpgrade(this.nextEquipmentVoUpgrade, this.selectedBuilding);
				this.CloseFromResearchScreen();
			}
			else
			{
				if (ArmoryUtils.IsEquipmentActive(currentPlayer, this.selectedEquipment))
				{
					Service.ArmoryController.DeactivateEquipment(this.selectedEquipment.EquipmentID);
				}
				else
				{
					Service.ArmoryController.ActivateEquipment(this.selectedEquipment.EquipmentID);
				}
				this.OnBackButtonClicked(null);
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
					string type = "speed_up_upgrade";
					string subType = "equipment";
					Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, buildingID, itemCount, type, subType);
				}
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
			Service.UXController.HUD.ShowContextButtons(this.selectedBuilding);
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
				deployableInfoParentScreen.Visible = visible;
			}
		}

		private void OnPrevOrNextButtonClicked(UXButton button)
		{
			int num = (int)button.Tag;
			int count = this.equipmentList.Count;
			int index = (num >= 0) ? 0 : (count - 1);
			EquipmentVO equipmentVO = this.equipmentList[index];
			for (int i = count - 1; i >= 0; i--)
			{
				int index2 = (num >= 0) ? i : (count - 1 - i);
				EquipmentVO equipmentVO2 = this.equipmentList[index2];
				if (equipmentVO2 == this.selectedEquipment)
				{
					break;
				}
				equipmentVO = equipmentVO2;
			}
			this.selectedEquipment = equipmentVO;
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			this.nextEquipmentVoUpgrade = equipmentUpgradeCatalog.GetNextLevel(this.selectedEquipment);
			this.Redraw();
		}

		protected void SetupBar(int index, string labelString, int currentValue, int nextValue, int maxValue, bool showBars)
		{
			bool flag = !string.IsNullOrEmpty(labelString);
			base.GetElement<UXElement>(string.Format("pBar{0}", index)).Visible = flag;
			if (!flag)
			{
				return;
			}
			UXLabel element = base.GetElement<UXLabel>(string.Format("LabelpBar{0}", index));
			element.Text = labelString;
			UXSlider element2 = base.GetElement<UXSlider>(string.Format("pBarCurrent{0}", index));
			UXSlider element3 = base.GetElement<UXSlider>(string.Format("pBarNext{0}", index));
			element2.Visible = showBars;
			element3.Visible = showBars;
			base.GetElement<UXSprite>(string.Format("SpritepBarBkg{0}", index)).Visible = showBars;
			element3.Visible = (nextValue > currentValue && showBars);
			element2.Value = MathUtils.NormalizeRange((float)currentValue, 0f, (float)maxValue);
			element3.Value = MathUtils.NormalizeRange((float)nextValue, 0f, (float)maxValue);
			UXLabel element4 = base.GetElement<UXLabel>(string.Format("LabelpBar{0}Current", index));
			element4.Text = this.lang.Get("perkEffect_descMod_PosPct", new object[]
			{
				this.lang.ThousandsSeparated(currentValue)
			});
			UXLabel element5 = base.GetElement<UXLabel>(string.Format("LabelpBar{0}Next", index));
			element5.Visible = (this.forResearchLab && nextValue > currentValue);
			element5.Text = this.lang.Get("perkEffect_descMod_PosPct", new object[]
			{
				this.lang.ThousandsSeparated(nextValue - currentValue)
			});
		}
	}
}
