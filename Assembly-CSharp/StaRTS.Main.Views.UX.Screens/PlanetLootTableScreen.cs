using Facebook.Unity;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.Player;
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
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class PlanetLootTableScreen : ClosableScreen, IViewClockTimeObserver
	{
		private const string PLANET_NAME_LABEL = "LabelTitlePlanetName";

		private const string PLANET_DESC_LABEL = "LabelPlanetDescription";

		private const string PLANET_LOCKED_SPRITE = "SpriteIconTitleLockedPlanet";

		private const string FEATURED_REWARDS_LABEL = "LabelFeaturedRewards";

		private const string FORCES_LABEL = "LabelForces";

		private const string FORCES_COUNT_LABEL = "LabelForcesValue";

		private const string SQUADMATES_LABEL = "LabelSquadmates";

		private const string SQUADMATES_COUNT_LABEL = "LabelSquadmatesValue";

		private const string REWARDS_GRID = "RewardGrid";

		private const string REWARDS_TEMPLATE = "RewardItemTemplate";

		private const string REWARD_ITEM_CARD_PREFIX = "RewardItemCardQ{0}";

		private const string REWARD_ITEM_CARD_BKG_PREFIX = "SpriteTroopImageBkgGridQ{0}";

		private const string REWARD_ITEM_CARD_DEFAULT = "RewardItemDefault";

		private const string REWARD_PROGRESSBAR = "pBarRewardItemFrag";

		private const string REWARD_PROGRESSBAR_SPRITE = "SpriteRewardItempBarFrag";

		private const string REWARD_FRAGMENT_SPRITE = "SpriteIconFragmentLootTable";

		private const string REWARD_NAME_LABEL = "LabelRewardName";

		private const string REWARD_GATE_LABEL = "LabelGate";

		private const string REWARD_PROGRESS_LABEL = "LabelFragProgress";

		private const string REWARD_INFO_BTN = "BtnRewardInfo";

		private const string REWARD_ITEM_CARD_BTN = "BtnRewardItemCard";

		private const string REWARD_UPGRADE_ICON = "IconUpgrade";

		private const string REWARD_SPRITE_IMAGE = "SpriteRewardItemImage";

		private const string SPRITE_LOCK_ICON = "SpriteLockIcon";

		private const string LABEL_EQUIPMENT_REQUIREMENT = "LabelEquipmentRequirement";

		private const string SPRITE_DIM_LOCK = "SpriteDimLock";

		private const string REWARD_TYPE_LABEL = "LabelRewardTypeLootTable";

		private const string REWARD_EXPIRATION_LABEL = "LabelRewardTypeLootTimer";

		private const string BTN_RELOCATE = "BtnRelocate";

		private const string BTN_RELOCATE_LABEL = "LabelBtnRelocate";

		private const string BTN_FACEBOOK = "BtnFacebookGoogle";

		private const string FIND_COMMANDERS_LABEL = "LabelFindCommanders";

		private const string FACEBOOK_BUTTON_LABEL = "LabelBtnFacebookGoogle";

		private const string BTN_BACK = "BtnBack";

		private const string ALLIES_STRING = "s_FriendsHere";

		private const string FACEBOOK_FIND_COMMANDERS = "PLANETS_FACEBOOK_FIND_COMMANDERS";

		private const string FACEBOOK_RECRUIT_COMMANDERS = "PLANETS_FACEBOOK_RECRUIT_COMMANDERS";

		private const string POPULATION_STRING = "s_PlanetStats";

		private const string SQUADMATES_HERE_STRING = "s_SquadmatesHere";

		private const string NO_SQUAD_DATA_STRING = "Planets_Info_No_Squad";

		private const string LOGIN_TO_FACEBOOK = "SETTINGS_NOTCONNECTED";

		private const string INVITE_FRIENDS = "INVITE_FRIENDS";

		private const string FEATURED_REWARDS = "PLANET_INFO_FEATURED_REWARDS";

		private const string RELOCATE_BUTTON_STRING = "Planets_Relocate_Button";

		private const string MAX_LEVEL = "MAX_LEVEL";

		private const string FRACTION = "FRACTION";

		private const string ENDS_IN_STRING = "PLANET_INFO_ITEM_TIMER";

		private const string POSTFIX_DESCRIPTION = "_desc";

		private const string EQUIPMENT_ARMORY_REQUIRED = "EQUIPMENT_ARMORY_REQUIRED";

		private const int MAX_TIMER_LABEL_WIDTH = 200;

		private Planet currentPlanet;

		private UXSprite lockedSprite;

		private UXButton facebookButton;

		private UXButton relocateButton;

		private UXLabel facebookLabel;

		private UXLabel facebookButtonLabel;

		private UXLabel forcesCountLabel;

		private UXLabel squadmatesCountLabel;

		private UXGrid rewardsGrid;

		private List<GeometryProjector> projectors;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public PlanetLootTableScreen(Planet planet) : base("gui_loot_table")
		{
			this.currentPlanet = planet;
			this.projectors = new List<GeometryProjector>();
		}

		public void RefreshWithNewPlanet(Planet planet)
		{
			if (base.IsLoaded())
			{
				this.currentPlanet = planet;
				this.InitLabels();
				this.InitButtons();
				this.SetupCurrentPlanetElements();
				this.RefreshGrid();
			}
		}

		public override void OnDestroyElement()
		{
			if (this.rewardsGrid != null)
			{
				this.rewardsGrid.Clear();
				this.rewardsGrid = null;
			}
			int i = 0;
			int count = this.projectors.Count;
			while (i < count)
			{
				this.projectors[i].Destroy();
				this.projectors[i] = null;
				i++;
			}
			this.projectors.Clear();
			this.projectors = null;
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			base.OnDestroyElement();
		}

		public void OnViewClockTime(float dt)
		{
			bool flag = false;
			int i = 0;
			int count = this.rewardsGrid.Count;
			while (i < count)
			{
				UXElement item = this.rewardsGrid.GetItem(i);
				PlanetLootEntryVO lootEntry = (PlanetLootEntryVO)item.Tag;
				if (this.DoesLootEntryHaveCountdown(lootEntry))
				{
					flag = true;
					this.UpdateTimeRemainingLabel(lootEntry);
				}
				i++;
			}
			if (!flag)
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				this.RefreshGrid();
			}
			this.currentPlanet.UpdateThrashingPopulation(dt);
			this.forcesCountLabel.Text = this.currentPlanet.ThrashingPopulation.ToString();
		}

		protected override void OnScreenLoaded()
		{
			this.lockedSprite = base.GetElement<UXSprite>("SpriteIconTitleLockedPlanet");
			this.InitLabels();
			this.InitButtons();
			this.SetupCurrentPlanetElements();
			this.InitFeaturedRewardsGrid();
		}

		private void SetupCurrentPlanetElements()
		{
			if (!base.IsLoaded())
			{
				Service.Logger.Error("Trying to setup planet locked ui, before screen load");
				return;
			}
			bool flag = Service.CurrentPlayer.IsPlanetUnlocked(this.currentPlanet.VO.Uid);
			this.lockedSprite.Visible = !flag;
		}

		private void RefreshGrid()
		{
			if (this.rewardsGrid != null)
			{
				this.rewardsGrid.Clear();
			}
			int i = 0;
			int count = this.projectors.Count;
			while (i < count)
			{
				this.projectors[i].Destroy();
				this.projectors[i] = null;
				i++;
			}
			this.projectors.Clear();
			this.InitFeaturedRewardsGrid();
		}

		protected override void InitButtons()
		{
			UXButton element = base.GetElement<UXButton>("BtnBack");
			element.OnClicked = new UXButtonClickedDelegate(this.HandleClose);
			this.SetupRelocateButton();
			this.SetupFacebookConnectButton();
		}

		private void InitLabels()
		{
			Lang lang = Service.Lang;
			base.GetElement<UXLabel>("LabelTitlePlanetName").Text = lang.Get(this.currentPlanet.VO.LoadingScreenText, new object[0]);
			base.GetElement<UXLabel>("LabelPlanetDescription").Text = lang.Get(this.currentPlanet.VO.Uid + "_desc", new object[0]);
			base.GetElement<UXLabel>("LabelFeaturedRewards").Text = lang.Get("PLANET_INFO_FEATURED_REWARDS", new object[0]);
			UXLabel element = base.GetElement<UXLabel>("LabelForces");
			element.Text = lang.Get("s_PlanetStats", new object[0]);
			this.forcesCountLabel = base.GetElement<UXLabel>("LabelForcesValue");
			this.forcesCountLabel.Text = this.currentPlanet.ThrashingPopulation.ToString();
			UXLabel element2 = base.GetElement<UXLabel>("LabelSquadmates");
			element2.Text = lang.Get("s_SquadmatesHere", new object[0]);
			this.squadmatesCountLabel = base.GetElement<UXLabel>("LabelSquadmatesValue");
			this.squadmatesCountLabel.Text = this.currentPlanet.NumSquadmatesOnPlanet.ToString();
		}

		private void InitFeaturedRewardsGrid()
		{
			this.rewardsGrid = base.GetElement<UXGrid>("RewardGrid");
			this.rewardsGrid.SetTemplateItem("RewardItemTemplate");
			InventoryCrateRewardController inventoryCrateRewardController = Service.InventoryCrateRewardController;
			List<PlanetLootEntryVO> featuredLootEntriesForPlanet = inventoryCrateRewardController.GetFeaturedLootEntriesForPlanet(this.currentPlanet);
			bool flag = false;
			int i = 0;
			int count = featuredLootEntriesForPlanet.Count;
			while (i < count)
			{
				this.AddFeaturedRewardItemToGrid(featuredLootEntriesForPlanet[i], i);
				if (this.DoesLootEntryHaveCountdown(featuredLootEntriesForPlanet[i]))
				{
					flag = true;
				}
				i++;
			}
			this.rewardsGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.RewardsGridRepositionComplete));
			if (flag)
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
		}

		private void RewardsGridRepositionComplete(AbstractUXList list)
		{
			this.rewardsGrid.Scroll(0f);
		}

		private void AddFeaturedRewardItemToGrid(PlanetLootEntryVO lootEntry, int order)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			InventoryCrateRewardController inventoryCrateRewardController = Service.InventoryCrateRewardController;
			Lang lang = Service.Lang;
			CrateSupplyVO optional = staticDataController.GetOptional<CrateSupplyVO>(lootEntry.SupplyDataUid);
			if (optional == null)
			{
				Service.Logger.ErrorFormat("Couldn't find CrateSupplyVO: {0} specified in PlanetLootEntryVO: {1}", new object[]
				{
					lootEntry.SupplyDataUid,
					lootEntry.Uid
				});
				return;
			}
			string uid = lootEntry.Uid;
			UXElement uXElement = this.rewardsGrid.CloneTemplateItem(uid);
			uXElement.Tag = lootEntry;
			UXLabel subElement = this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelRewardName");
			subElement.Text = inventoryCrateRewardController.GetCrateSupplyRewardName(optional);
			UXUtils.ClampUILabelWidth(subElement, 200, 0);
			UXLabel subElement2 = this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelGate");
			subElement2.Text = string.Empty;
			if (!string.IsNullOrEmpty(lootEntry.NotesString))
			{
				subElement2.Text = lang.Get(lootEntry.NotesString, new object[0]);
				UXUtils.ClampUILabelWidth(subElement2, 200, 0);
			}
			UXUtils.HideGridQualityCards(this.rewardsGrid, uid, "RewardItemCardQ{0}");
			this.rewardsGrid.GetSubElement<UXElement>(uid, "RewardItemDefault").Visible = false;
			this.rewardsGrid.GetSubElement<UXElement>(uid, "pBarRewardItemFrag").Visible = false;
			this.rewardsGrid.GetSubElement<UXElement>(uid, "IconUpgrade").Visible = false;
			UXLabel subElement3 = this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelRewardTypeLootTimer");
			UXUtils.ClampUILabelWidth(subElement3, 200, 0);
			bool flag = this.DoesLootEntryHaveCountdown(lootEntry);
			subElement3.Visible = flag;
			if (flag)
			{
				this.UpdateTimeRemainingLabel(lootEntry);
			}
			UXButton subElement4 = this.rewardsGrid.GetSubElement<UXButton>(uid, "BtnRewardInfo");
			UXButton subElement5 = this.rewardsGrid.GetSubElement<UXButton>(uid, "BtnRewardItemCard");
			subElement5.InitTweenComponent();
			subElement4.Visible = false;
			if (optional.Type == SupplyType.Hero || optional.Type == SupplyType.Troop || optional.Type == SupplyType.SpecialAttack || optional.Type == SupplyType.Shard || optional.Type == SupplyType.ShardTroop || optional.Type == SupplyType.ShardSpecialAttack)
			{
				subElement4.Visible = true;
				subElement4.Tag = optional;
				subElement4.OnClicked = new UXButtonClickedDelegate(this.OnInfoButtonClicked);
				subElement5.Tag = optional;
				subElement5.OnClicked = new UXButtonClickedDelegate(this.OnInfoButtonClicked);
			}
			else
			{
				subElement5.DisablePlayTween();
			}
			UXLabel subElement6 = this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelRewardTypeLootTable");
			subElement6.Visible = true;
			this.rewardsGrid.GetSubElement<UXSprite>(uid, "SpriteIconFragmentLootTable").Visible = false;
			if (optional.Type == SupplyType.ShardTroop || optional.Type == SupplyType.ShardSpecialAttack)
			{
				this.SetupShardRewardItemElements(uid, optional);
			}
			else if (optional.Type == SupplyType.Shard)
			{
				this.SetupEquipmentShardRewardItemElements(uid, optional);
			}
			else
			{
				this.rewardsGrid.GetSubElement<UXElement>(uid, "RewardItemDefault").Visible = true;
				this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelFragProgress").Text = string.Empty;
				UXSprite subElement7 = this.rewardsGrid.GetSubElement<UXSprite>(uid, "SpriteRewardItemImage");
				IGeometryVO geometryVO = null;
				if (optional.Type == SupplyType.Currency)
				{
					subElement6.Visible = false;
					geometryVO = UXUtils.GetDefaultCurrencyIconVO(optional.RewardUid);
				}
				else if (optional.Type == SupplyType.Hero || optional.Type == SupplyType.Troop)
				{
					geometryVO = staticDataController.Get<TroopTypeVO>(optional.RewardUid);
				}
				else if (optional.Type == SupplyType.SpecialAttack)
				{
					geometryVO = staticDataController.Get<SpecialAttackTypeVO>(optional.RewardUid);
				}
				if (geometryVO != null)
				{
					ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(geometryVO, subElement7, false);
					GeometryProjector item = ProjectorUtils.GenerateProjector(config);
					this.projectors.Add(item);
				}
			}
			this.SetupLockedRewardItemElements(uid, lootEntry);
			if (subElement6.Visible)
			{
				string typeStringID = lootEntry.TypeStringID;
				if (!string.IsNullOrEmpty(typeStringID))
				{
					subElement6.Visible = true;
					subElement6.Text = lang.Get(typeStringID, new object[0]);
				}
				else
				{
					subElement6.Visible = false;
				}
			}
			subElement6.InitTweenComponent();
			subElement3.InitTweenComponent();
			if (flag && subElement6.Visible)
			{
				subElement6.EnablePlayTween();
				subElement3.EnablePlayTween();
			}
			else
			{
				subElement6.ResetPlayTweenTarget();
				subElement3.ResetPlayTweenTarget();
				subElement6.TextColor = Color.white;
				subElement3.TextColor = Color.white;
			}
			this.rewardsGrid.AddItem(uXElement, order);
		}

		private void SetupLockedRewardItemElements(string itemUID, PlanetLootEntryVO lootEntry)
		{
			UXSprite subElement = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteLockIcon");
			UXSprite subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteDimLock");
			UXLabel subElement3 = this.rewardsGrid.GetSubElement<UXLabel>(itemUID, "LabelEquipmentRequirement");
			subElement.Visible = false;
			subElement2.Visible = false;
			subElement3.Visible = false;
			bool flag = lootEntry.ReqArmory && !ArmoryUtils.PlayerHasArmory();
			if (flag)
			{
				subElement2.Visible = true;
				subElement.Visible = true;
				subElement3.Visible = true;
				subElement3.Text = this.lang.Get("EQUIPMENT_ARMORY_REQUIRED", new object[0]);
			}
		}

		private void SetupEquipmentShardRewardItemElements(string itemUID, CrateSupplyVO crateSupply)
		{
			EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(crateSupply.RewardUid);
			int quality = (int)currentEquipmentDataByID.Quality;
			string name = string.Format("SpriteTroopImageBkgGridQ{0}", quality);
			this.rewardsGrid.GetSubElement<UXElement>(itemUID, name).Visible = true;
			UXSprite subElement = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteIconFragmentLootTable");
			UXUtils.SetupFragmentIconSprite(subElement, quality);
			this.SetupEquipmentShardProgress(itemUID, currentEquipmentDataByID);
			UXSprite subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteRewardItemImage");
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateEquipmentConfig(currentEquipmentDataByID, subElement2, true);
			projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			GeometryProjector item = ProjectorUtils.GenerateProjector(projectorConfig);
			this.projectors.Add(item);
			UXUtils.SetCardQuality(this, this.rewardsGrid, itemUID, quality, "RewardItemCardQ{0}");
		}

		private void SetupShardRewardItemElements(string itemUID, CrateSupplyVO crateSupply)
		{
			ShardVO shardVO = Service.StaticDataController.Get<ShardVO>(crateSupply.RewardUid);
			int quality = (int)shardVO.Quality;
			UXSprite subElement = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteIconFragmentLootTable");
			UXUtils.SetupFragmentIconSprite(subElement, quality);
			UXSprite subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteRewardItemImage");
			IDeployableVO deployableVOFromShard = Service.DeployableShardUnlockController.GetDeployableVOFromShard(shardVO);
			if (deployableVOFromShard == null)
			{
				Service.Logger.Error("SetupShardRewardItemElements Unable to find deployable");
				return;
			}
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(deployableVOFromShard, subElement2, true);
			projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			GeometryProjector item = ProjectorUtils.GenerateProjector(projectorConfig);
			this.projectors.Add(item);
			this.SetupDeployableShardProgress(itemUID, shardVO);
			UXUtils.SetCardQuality(this, this.rewardsGrid, itemUID, quality, "RewardItemCardQ{0}");
		}

		private void SetupEquipmentShardProgress(string itemUID, EquipmentVO eqp)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			UXSlider subElement = this.rewardsGrid.GetSubElement<UXSlider>(itemUID, "pBarRewardItemFrag");
			UXSprite subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteRewardItempBarFrag");
			subElement.Visible = true;
			UXLabel subElement3 = this.rewardsGrid.GetSubElement<UXLabel>(itemUID, "LabelFragProgress");
			UXElement subElement4 = this.rewardsGrid.GetSubElement<UXElement>(itemUID, "IconUpgrade");
			EquipmentVO nextLevel = Service.EquipmentUpgradeCatalog.GetNextLevel(eqp);
			string equipmentID = eqp.EquipmentID;
			if (nextLevel == null)
			{
				subElement3.Text = this.lang.Get("MAX_LEVEL", new object[0]);
				subElement.Value = 1f;
				return;
			}
			int num = (!currentPlayer.Shards.ContainsKey(equipmentID)) ? 0 : currentPlayer.Shards[equipmentID];
			int upgradeShards;
			if (currentPlayer.UnlockedLevels.Equipment.Has(eqp))
			{
				upgradeShards = nextLevel.UpgradeShards;
				subElement4.Visible = (num >= upgradeShards);
			}
			else
			{
				upgradeShards = eqp.UpgradeShards;
			}
			subElement3.Text = this.lang.Get("FRACTION", new object[]
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
			UXUtils.SetShardProgressBarValue(subElement, subElement2, sliderValue);
		}

		private void SetupDeployableShardProgress(string itemUID, ShardVO shard)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			TroopUpgradeCatalog troopUpgradeCatalog = Service.TroopUpgradeCatalog;
			StarshipUpgradeCatalog starshipUpgradeCatalog = Service.StarshipUpgradeCatalog;
			UXSlider subElement = this.rewardsGrid.GetSubElement<UXSlider>(itemUID, "pBarRewardItemFrag");
			UXSprite subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUID, "SpriteRewardItempBarFrag");
			subElement.Visible = true;
			UXLabel subElement3 = this.rewardsGrid.GetSubElement<UXLabel>(itemUID, "LabelFragProgress");
			UXElement subElement4 = this.rewardsGrid.GetSubElement<UXElement>(itemUID, "IconUpgrade");
			IDeployableVO deployableVO;
			IDeployableVO byLevel;
			if (shard.TargetType == "specialAttack")
			{
				int nextLevel = currentPlayer.UnlockedLevels.Starships.GetNextLevel(shard.TargetGroupId);
				deployableVO = starshipUpgradeCatalog.GetByLevel(shard.TargetGroupId, nextLevel);
				byLevel = starshipUpgradeCatalog.GetByLevel(shard.TargetGroupId, nextLevel - 1);
			}
			else
			{
				int nextLevel = currentPlayer.UnlockedLevels.Troops.GetNextLevel(shard.TargetGroupId);
				deployableVO = troopUpgradeCatalog.GetByLevel(shard.TargetGroupId, nextLevel);
				byLevel = troopUpgradeCatalog.GetByLevel(shard.TargetGroupId, nextLevel - 1);
			}
			if (deployableVO == null)
			{
				subElement3.Text = this.lang.Get("MAX_LEVEL", new object[0]);
				subElement.Value = 1f;
				return;
			}
			bool flag = Service.UnlockController.IsMinLevelUnlocked(deployableVO);
			if (!flag)
			{
				deployableVO = byLevel;
			}
			int shardAmount = Service.DeployableShardUnlockController.GetShardAmount(deployableVO.UpgradeShardUid);
			int upgradeShardCount = deployableVO.UpgradeShardCount;
			subElement3.Text = this.lang.Get("FRACTION", new object[]
			{
				shardAmount,
				upgradeShardCount
			});
			if (upgradeShardCount == 0)
			{
				Service.Logger.ErrorFormat("CMS Error: Shards required for {0} is zero", new object[]
				{
					deployableVO.Uid
				});
				return;
			}
			float sliderValue = (float)shardAmount / (float)upgradeShardCount;
			UXUtils.SetShardProgressBarValue(subElement, subElement2, sliderValue);
			subElement4.Visible = (flag && shardAmount >= upgradeShardCount);
		}

		private void SetupRelocateButton()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Lang lang = Service.Lang;
			this.relocateButton = base.GetElement<UXButton>("BtnRelocate");
			this.relocateButton.OnClicked = new UXButtonClickedDelegate(this.OnRelocateClicked);
			UXLabel element = base.GetElement<UXLabel>("LabelBtnRelocate");
			element.Text = lang.Get("Planets_Relocate_Button", new object[0]);
			if (currentPlayer.IsPlanetUnlocked(this.currentPlanet.VO.Uid))
			{
				if (GameUtils.IsPlanetCurrentOne(this.currentPlanet.VO.Uid))
				{
					this.relocateButton.Visible = false;
				}
				else
				{
					this.relocateButton.Visible = true;
					this.relocateButton.Enabled = true;
				}
			}
			else
			{
				this.relocateButton.Visible = true;
				this.relocateButton.Enabled = false;
			}
		}

		private void UpdateTimeRemainingLabel(PlanetLootEntryVO lootEntry)
		{
			UXLabel subElement = this.rewardsGrid.GetSubElement<UXLabel>(lootEntry.Uid, "LabelRewardTypeLootTimer");
			int totalSeconds = (int)((long)lootEntry.HideDateTimeStamp - (long)((ulong)ServerTime.Time));
			subElement.Text = Service.Lang.Get("PLANET_INFO_ITEM_TIMER", new object[]
			{
				GameUtils.GetTimeLabelFromSeconds(totalSeconds)
			});
		}

		private void SetupFacebookConnectButton()
		{
			this.facebookButton = base.GetElement<UXButton>("BtnFacebookGoogle");
			this.facebookLabel = base.GetElement<UXLabel>("LabelFindCommanders");
			this.facebookButtonLabel = base.GetElement<UXLabel>("LabelBtnFacebookGoogle");
			if (!Service.ISocialDataController.IsLoggedIn)
			{
				this.facebookLabel.Text = this.lang.Get("PLANETS_FACEBOOK_FIND_COMMANDERS", new object[0]);
				this.facebookButtonLabel.Text = this.lang.Get("SETTINGS_NOTCONNECTED", new object[0]);
				this.facebookButton.OnClicked = new UXButtonClickedDelegate(this.OnConnectToFacebook);
			}
			else if (GameConstants.FACEBOOK_INVITES_ENABLED)
			{
				this.facebookLabel.Text = this.lang.Get("PLANETS_FACEBOOK_RECRUIT_COMMANDERS", new object[0]);
				this.facebookButtonLabel.Text = this.lang.Get("INVITE_FRIENDS", new object[0]);
				this.facebookButton.OnClicked = new UXButtonClickedDelegate(this.OnInvite);
			}
			else
			{
				this.facebookButton.Visible = false;
			}
		}

		private void OnConnectToFacebook(UXButton button)
		{
			Service.EventManager.SendEvent(EventId.SquadFB, null);
			Service.ISocialDataController.Login(new OnAllDataFetchedDelegate(this.SetupFacebookConnectButton));
		}

		private void OnInvite(UXButton button)
		{
			Service.EventManager.SendEvent(EventId.SquadFB, null);
			Service.ISocialDataController.InviteFriends(new OnRequestDelegate(this.InviteFriendsCallback));
		}

		private void InviteFriendsCallback(IAppRequestResult result)
		{
			Service.GalaxyPlanetController.UpdatePlanetsFriendData();
			this.squadmatesCountLabel.Text = this.currentPlanet.FriendsOnPlanet.Count.ToString();
		}

		private void OnRelocateClicked(UXButton button)
		{
			PlanetVO vO = this.currentPlanet.VO;
			Service.EventManager.SendEvent(EventId.LootTableRelocateTapped, vO.Uid);
			ConfirmRelocateScreen.ShowModal(vO, null, null);
		}

		private void OnInfoButtonClicked(UXButton button)
		{
			CrateSupplyVO crateSupply = (CrateSupplyVO)button.Tag;
			GameUtils.HandleCrateSupplyRewardClicked(crateSupply);
		}

		private bool DoesLootEntryHaveCountdown(PlanetLootEntryVO lootEntry)
		{
			int time = (int)ServerTime.Time;
			return lootEntry.ShowDateTimeStamp <= time && (lootEntry.HideDateTimeStamp > 0 && time < lootEntry.HideDateTimeStamp);
		}
	}
}
