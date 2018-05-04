using StaRTS.Assets;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Animations
{
	public class InventoryCrateAnimation : IEventObserver
	{
		private const float LAND_TO_OPEN_DELAY = 2.1f;

		private static readonly Vector3 RIG_OFFSET = new Vector3(3000f, 1000f, 0f);

		private const int RENDER_WIDTH_HEIGHT = 256;

		private const string CRATE_LOCATOR = "crateHolder";

		private const string CRATE_VFX = "vfx_crate_specific";

		public const string SUPPLY_REWARD_FORMAT = "SupplyRewardFormat";

		public const string DROP_CRATE_ANIM = "DropCrate";

		public const string SHOW_REWARD_ANIM = "ShowReward";

		public const string SKIP_REWARD_ANIM = "ShowRewardEnd";

		public const string SHOW_SHARD_REWARD_END = "ShowDiscSpinEnd";

		private const string CRATE_NAME_USED_TO_BE_FOUND = "crate";

		private const string REWARD_PATH_BASE = "reward_node/";

		private const string CONTRABAND_REWARD_ID = "crate_reward_contraband";

		private const string REPUTATION_REWARD_ID = "crate_reward_reputation";

		private const string CRYSTAL_REWARD_ID = "crate_reward_crystal";

		private const string CREDIT_REWARD_ID = "crate_reward_credit";

		private const string MATERIAL_REWARD_ID = "crate_reward_alloy";

		private const string UNIT_REWARD_ID = "crate_reward_unit";

		private const string SHARD_REWARD_ID = "crate_reward_shard";

		private const string BACK_VFX_PATH_BASE = "reward_vfx_locator/";

		private const string CREDIT_BACK_VFX = "vfx_back_credit";

		private const string CONTRABAND_BACK_VFX = "vfx_back_contraband";

		private const string MATERIAL_BACK_VFX = "vfx_back_alloy";

		private const string REPUTATION_BACK_VFX = "vfx_back_reputation";

		private const string CRYSTAL_BACK_VFX = "vfx_back_crystal";

		private const string UNIT_BACK_VFX = "vfx_back_unit";

		private const string SHARD_BACK_VFX = "vfx_back_shard";

		private const string BEAM_VFX = "beams";

		private const string SPARKLE_VFX = "sparkles";

		private const string OUTLINE_OUTER_PARAM = "_Outline";

		private const string OUTLINE_INNER_PARAM = "_OutlineInnerWidth";

		private const string REWARD_RENDER_MESH_TARGET = "datacardscreenmesh";

		private const string REWARD_RARITY_LEVEL1_MESH = "rarity_star_01";

		private const string REWARD_RARITY_LEVEL2_MESH = "rarity_star_02";

		private const string REWARD_RARITY_LEVEL3_MESH = "rarity_star_03";

		private const string REWARD_RARITY_LEVEL10_MESH = "rarity_star_10";

		private const string DISK_LEVEL1_MESH = "disc_body_01";

		private const string DISK_LEVEL2_MESH = "disc_body_02";

		private const string DISK_LEVEL3_MESH = "disc_body_03";

		private const string DISK_LEVEL10_MESH = "disc_body_10";

		private GameObject crateRayEffect;

		private Transform beamsEffect;

		private Transform sparkleEffect;

		private readonly Color ELITE_COLOR = new Color(1f, 0.5f, 0f);

		private readonly Color ELITE_BURST_COLOR = new Color(1f, 0.25f, 0f);

		private readonly Color ADVANCED_COLOR = new Color(0f, 0.45f, 1f);

		private readonly Color BASIC_COLOR = new Color(0.5f, 0.5f, 0.5f);

		private readonly Color UNIT_COLOR = new Color(0.46f, 0.1f, 0.83f);

		private List<SupplyCrateTag> supplyCrateDataList;

		private SupplyCrateTag currentCrateItem;

		private int hq;

		private InventoryCrateCollectionScreen screen;

		private AssetMeshDataMonoBehaviour assetMeshDataMonoBehavior;

		private List<AssetHandle> assetHandles;

		private Dictionary<string, int> shardsOriginal;

		private Dictionary<string, int> equipmentOriginal;

		private Dictionary<string, int> troopUpgradeOrignal;

		private Dictionary<string, int> specialAttackUpgradeOrignal;

		private GameObject crateController;

		private GameObject crateObj;

		private CrateVO crateVO;

		private Animator crateAnimator;

		private ParticleSystem crateVfxParticles;

		private ParticleSystem crateBurstParticles;

		private GameObject contrabandCurrencyReward;

		private GameObject reputationReward;

		private GameObject crystalCurrencyReward;

		private GameObject creditCurrencyReward;

		private GameObject materialCurrencyReward;

		private GameObject unitReward;

		private GameObject shardReward;

		private Animator shardRewardAnimator;

		private GameObject contrabandBackVfx;

		private GameObject reputationBackVfx;

		private GameObject crystalBackVfx;

		private GameObject creditBackVfx;

		private GameObject materialBackVfx;

		private GameObject unitBackVfx;

		private GameObject shardBackVfx;

		private int rewardIndex;

		private InventoryCrateAnimationState crateAnimState;

		private uint crateLandTimer;

		private bool playOpenOnLanding;

		private bool isSkippingReward;

		private bool rewardAnimStarted;

		private bool rewardAnimEventReceived;

		public InventoryCrateCollectionScreen InvCrateCollectionScreen
		{
			get
			{
				return this.screen;
			}
		}

		public bool IsLoaded
		{
			get;
			private set;
		}

		public InventoryCrateAnimation(List<CrateSupplyVO> supplyTableDataList, CrateData crateData, Dictionary<string, int> shardsOriginal, Dictionary<string, int> equipmentOriginal, Dictionary<string, int> troopUpgradeOriginalCopy, Dictionary<string, int> specialAttackUpgradeOriginalCopy)
		{
			List<string> assetNames = new List<string>();
			List<object> cookies = new List<object>();
			this.assetHandles = new List<AssetHandle>();
			this.supplyCrateDataList = new List<SupplyCrateTag>();
			this.hq = crateData.HQLevel;
			this.screen = new InventoryCrateCollectionScreen(this, crateData);
			StaticDataController staticDataController = Service.StaticDataController;
			this.crateVO = staticDataController.Get<CrateVO>(crateData.CrateId);
			this.shardsOriginal = shardsOriginal;
			this.equipmentOriginal = equipmentOriginal;
			this.troopUpgradeOrignal = troopUpgradeOriginalCopy;
			this.specialAttackUpgradeOrignal = specialAttackUpgradeOriginalCopy;
			this.crateAnimState = InventoryCrateAnimationState.Falling;
			this.crateLandTimer = 0u;
			this.playOpenOnLanding = false;
			this.isSkippingReward = false;
			this.rewardAnimStarted = false;
			this.rewardAnimEventReceived = false;
			this.IsLoaded = false;
			int i = 0;
			int count = supplyTableDataList.Count;
			while (i < count)
			{
				SupplyCrateTag supplyCrateTag = new SupplyCrateTag();
				CrateSupplyVO crateSupplyVO = supplyTableDataList[i];
				SupplyType type = crateSupplyVO.Type;
				supplyCrateTag.CrateSupply = crateSupplyVO;
				this.supplyCrateDataList.Add(supplyCrateTag);
				if (type != SupplyType.Currency)
				{
					IGeometryVO iconVOFromCrateSupply = GameUtils.GetIconVOFromCrateSupply(crateSupplyVO, this.hq);
					this.AddAssetToLoadList(assetNames, cookies, iconVOFromCrateSupply.IconAssetName);
					ProjectorConfig projectorConfig;
					if (iconVOFromCrateSupply is EquipmentVO)
					{
						EquipmentVO equipmentVO = (EquipmentVO)iconVOFromCrateSupply;
						supplyCrateTag.Equipment = equipmentVO;
						supplyCrateTag.ShardQuailty = equipmentVO.Quality;
						projectorConfig = ProjectorUtils.GenerateEquipmentConfig(iconVOFromCrateSupply as EquipmentVO, new Action<RenderTexture, ProjectorConfig>(this.RenderTextureCompleteCallback), 256f, 256f);
					}
					else
					{
						if (type == SupplyType.ShardSpecialAttack || type == SupplyType.ShardTroop)
						{
							ShardVO shardVO = staticDataController.Get<ShardVO>(crateSupplyVO.RewardUid);
							supplyCrateTag.ShardQuailty = shardVO.Quality;
							supplyCrateTag.UnlockShard = shardVO;
						}
						projectorConfig = ProjectorUtils.GenerateGeometryConfig(iconVOFromCrateSupply, new Action<RenderTexture, ProjectorConfig>(this.RenderTextureCompleteCallback), 256f, 256f);
					}
					projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
					supplyCrateTag.Config = projectorConfig;
					supplyCrateTag.Projector = ProjectorUtils.GenerateProjector(projectorConfig);
				}
				i++;
			}
			this.AddAssetToLoadList(assetNames, cookies, this.crateVO.RewardAnimationAssetName);
			this.AddAssetToLoadList(assetNames, cookies, "crate_controller");
			Service.AssetManager.MultiLoad(this.assetHandles, assetNames, new AssetSuccessDelegate(this.LoadSuccess), new AssetFailureDelegate(this.LoadFailed), cookies, new AssetsCompleteDelegate(this.AssetsCompleteDelegate), null);
		}

		public int GetTotalRewardsCount()
		{
			return this.supplyCrateDataList.Count;
		}

		public int GetCurrentRewardIndex()
		{
			return this.rewardIndex;
		}

		public int GetCrateHQLevel()
		{
			return this.hq;
		}

		public List<SupplyCrateTag> GetRewardList()
		{
			return this.supplyCrateDataList;
		}

		private bool IsCurrentItemRewardAShard()
		{
			if (this.currentCrateItem == null || this.currentCrateItem.CrateSupply == null)
			{
				string text = (this.crateVO != null) ? this.crateVO.Uid : "unknown";
				Service.Logger.ErrorFormat("Reward data for crate {0} not found or has been cleaned up while using it", new object[]
				{
					text
				});
				return false;
			}
			CrateSupplyVO crateSupply = this.currentCrateItem.CrateSupply;
			SupplyType type = crateSupply.Type;
			return type == SupplyType.Shard || type == SupplyType.ShardSpecialAttack || type == SupplyType.ShardTroop;
		}

		private void HandleShardRewardItemShow()
		{
			int shardsFrom;
			int num;
			int shardsNeededForLevel;
			bool showLevel;
			EquipmentVO equipmentVO;
			string key;
			if (this.GetEquipmentShardUIInfoForCurrentItem(out shardsFrom, out num, out shardsNeededForLevel, out showLevel, out equipmentVO))
			{
				key = equipmentVO.EquipmentID;
				this.screen.ShowEquipmentPBarUI(this.currentCrateItem, shardsFrom, num, shardsNeededForLevel, showLevel);
			}
			else
			{
				ShardVO shardVO;
				if (!this.GetUnlockShardUIInfoForCurrentItem(out shardsFrom, out num, out shardsNeededForLevel, out showLevel, out shardVO))
				{
					Service.Logger.Error("Inventory Crate Animation: HandleRewardItemShow Unknown Shard RewardType");
					return;
				}
				key = shardVO.Uid;
				this.screen.ShowUnlockShardPBarUI(this.currentCrateItem, shardsFrom, num, shardsNeededForLevel, showLevel);
			}
			this.shardsOriginal[key] = num;
		}

		private bool GetUnlockShardUIInfoForCurrentItem(out int currentShardAmount, out int increasedShardAmount, out int shardsNeededForLevel, out bool showLevel, out ShardVO unlockShardToDisplay)
		{
			currentShardAmount = 0;
			increasedShardAmount = 0;
			shardsNeededForLevel = 0;
			unlockShardToDisplay = null;
			showLevel = false;
			CrateSupplyVO crateSupply = this.currentCrateItem.CrateSupply;
			SupplyType type = crateSupply.Type;
			if (type != SupplyType.ShardSpecialAttack && type != SupplyType.ShardTroop)
			{
				return false;
			}
			ShardVO unlockShard = this.currentCrateItem.UnlockShard;
			if (unlockShard == null)
			{
				return false;
			}
			unlockShardToDisplay = unlockShard;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			DeployableShardUnlockController deployableShardUnlockController = Service.DeployableShardUnlockController;
			string uid = unlockShard.Uid;
			string targetGroupId = unlockShard.TargetGroupId;
			string targetType = unlockShard.TargetType;
			int num = 0;
			if (targetType == "hero" || targetType == "troop")
			{
				if (this.troopUpgradeOrignal.ContainsKey(targetGroupId))
				{
					num = this.troopUpgradeOrignal[targetGroupId];
				}
			}
			else if (targetType == "specialAttack" && this.specialAttackUpgradeOrignal.ContainsKey(targetGroupId))
			{
				num = this.specialAttackUpgradeOrignal[targetGroupId];
			}
			int hqLevel = currentPlayer.Map.FindHighestHqLevel();
			int num2 = (!this.shardsOriginal.ContainsKey(uid)) ? 0 : this.shardsOriginal[uid];
			int rewardAmount = Service.InventoryCrateRewardController.GetRewardAmount(crateSupply, hqLevel);
			currentShardAmount = num2;
			increasedShardAmount = num2 + rewardAmount;
			showLevel = (num > 0);
			if (type == SupplyType.ShardSpecialAttack)
			{
				shardsNeededForLevel = deployableShardUnlockController.GetNumShardsForDeployableToReachLevelInGroup<SpecialAttackTypeVO>(num, num + 1, targetGroupId);
			}
			else if (type == SupplyType.ShardTroop)
			{
				shardsNeededForLevel = deployableShardUnlockController.GetNumShardsForDeployableToReachLevelInGroup<TroopTypeVO>(num, num + 1, targetGroupId);
			}
			return true;
		}

		private bool GetEquipmentShardUIInfoForCurrentItem(out int currentShardAmount, out int increasedShardAmount, out int shardsNeededForLevel, out bool showLevel, out EquipmentVO equipmentVoToDisplay)
		{
			currentShardAmount = 0;
			increasedShardAmount = 0;
			shardsNeededForLevel = 0;
			equipmentVoToDisplay = null;
			showLevel = false;
			CrateSupplyVO crateSupply = this.currentCrateItem.CrateSupply;
			if (crateSupply.Type != SupplyType.Shard)
			{
				return false;
			}
			EquipmentVO equipment = this.currentCrateItem.Equipment;
			if (equipment == null)
			{
				return false;
			}
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			string equipmentID = equipment.EquipmentID;
			int num = (!this.shardsOriginal.ContainsKey(equipmentID)) ? 0 : this.shardsOriginal[equipmentID];
			currentShardAmount = num;
			RewardVO reward = Service.InventoryCrateRewardController.GenerateRewardFromSupply(crateSupply, this.hq);
			int shardsRewarded = RewardUtils.GetShardsRewarded(reward);
			int num2 = num + shardsRewarded;
			increasedShardAmount = num2;
			EquipmentVO equipmentVO = equipment;
			showLevel = true;
			if (this.equipmentOriginal.ContainsKey(equipmentID))
			{
				int num3 = this.equipmentOriginal[equipmentID];
				int level = Mathf.Min(num3 + 1, equipmentUpgradeCatalog.GetMaxLevel(equipmentID).Lvl);
				equipmentVoToDisplay = equipmentUpgradeCatalog.GetByLevel(equipment, num3);
				equipmentVO = equipmentUpgradeCatalog.GetByLevel(equipmentVoToDisplay, level);
			}
			else
			{
				equipmentVoToDisplay = equipmentUpgradeCatalog.GetMinLevel(equipmentVO.EquipmentID);
				equipmentVO = equipmentVoToDisplay;
				showLevel = false;
			}
			shardsNeededForLevel = equipmentVO.UpgradeShards;
			return true;
		}

		private void AddAssetToLoadList(List<string> assetNames, List<object> cookies, string assetName)
		{
			assetNames.Add(assetName);
			this.assetHandles.Add(AssetHandle.Invalid);
			cookies.Add(null);
		}

		private void RenderTextureCompleteCallback(RenderTexture renderTexture, ProjectorConfig config)
		{
			int i = 0;
			int count = this.supplyCrateDataList.Count;
			while (i < count)
			{
				SupplyCrateTag supplyCrateTag = this.supplyCrateDataList[i];
				if (config == supplyCrateTag.Config)
				{
					supplyCrateTag.RenderTexture = renderTexture;
					break;
				}
				i++;
			}
		}

		private void LoadFailed(object cookie)
		{
		}

		private void AssetsCompleteDelegate(object cookie)
		{
			ProcessingScreen.Hide();
			Service.EventManager.RegisterObserver(this, EventId.PlayCrateAnimVfx);
			Service.EventManager.RegisterObserver(this, EventId.CrateRewardAnimDone);
			Service.UXController.HideAll();
			Service.ScreenController.AddScreen(this.screen, true, false);
			this.assetMeshDataMonoBehavior = this.crateController.GetComponent<AssetMeshDataMonoBehaviour>();
			Transform transform = this.assetMeshDataMonoBehavior.OtherGameObjects[0].transform;
			Transform transform2 = this.assetMeshDataMonoBehavior.OtherGameObjects[1].transform;
			this.crateVfxParticles = transform2.gameObject.GetComponent<ParticleSystem>();
			this.crateBurstParticles = this.assetMeshDataMonoBehavior.OtherGameObjects[2].GetComponent<ParticleSystem>();
			this.crateRayEffect = transform2.Find(this.crateObj.name).gameObject;
			this.beamsEffect = this.crateRayEffect.transform.Find("beams");
			this.sparkleEffect = this.crateRayEffect.transform.Find("sparkles");
			this.crateRayEffect.SetActive(true);
			this.crateObj.transform.SetParent(transform);
			this.crateObj.transform.localPosition = Vector3.zero;
			Renderer[] componentsInChildren = this.crateObj.GetComponentsInChildren<Renderer>(true);
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				Renderer renderer = componentsInChildren[i];
				if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_OutlineInnerWidth") && renderer.sharedMaterial.HasProperty("_Outline"))
				{
					renderer.sharedMaterial.SetFloat("_Outline", 0f);
					renderer.sharedMaterial.SetFloat("_OutlineInnerWidth", 0f);
				}
				i++;
			}
			Transform[] componentsInChildren2 = this.crateObj.GetComponentsInChildren<Transform>(true);
			GameObject gameObject = null;
			if (componentsInChildren2 != null)
			{
				int num2 = componentsInChildren2.Length;
				for (int j = 0; j < num2; j++)
				{
					GameObject gameObject2 = componentsInChildren2[j].gameObject;
					string name = gameObject2.name;
					if ("crate" == name)
					{
						gameObject = gameObject2;
						break;
					}
				}
			}
			this.crateObj.name = "crate";
			if (gameObject != null)
			{
				transform2.SetParent(gameObject.transform);
			}
			else
			{
				transform2.SetParent(this.crateObj.transform);
			}
			transform2.localPosition = Vector3.zero;
			this.crateController.SetActive(true);
			this.crateAnimator = this.crateController.GetComponent<Animator>();
			this.crateAnimator.SetTrigger("DropCrate");
			this.FindChildAnims();
			this.crateLandTimer = Service.ViewTimerManager.CreateViewTimer(this.crateVO.CrateRewardAnimLandTime, false, new TimerDelegate(this.OnCrateLanded), null);
			this.IsLoaded = true;
			this.UpdateCrateAnimState(InventoryCrateAnimationState.Falling);
		}

		private void FindChildAnims()
		{
			string text = string.Empty;
			text = "reward_node/crate_reward_contraband";
			this.contrabandCurrencyReward = this.crateController.transform.Find(text).gameObject;
			if (this.contrabandCurrencyReward == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_node/crate_reward_reputation";
			this.reputationReward = this.crateController.transform.Find(text).gameObject;
			if (this.reputationReward == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_node/crate_reward_crystal";
			this.crystalCurrencyReward = this.crateController.transform.Find(text).gameObject;
			if (this.crystalCurrencyReward == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_node/crate_reward_credit";
			this.creditCurrencyReward = this.crateController.transform.Find(text).gameObject;
			if (this.creditCurrencyReward == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_node/crate_reward_alloy";
			this.materialCurrencyReward = this.crateController.transform.Find(text).gameObject;
			if (this.materialCurrencyReward == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_node/crate_reward_unit";
			this.unitReward = this.crateController.transform.Find(text).gameObject;
			if (this.unitReward == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_node/crate_reward_shard";
			this.shardReward = this.crateController.transform.Find(text).gameObject;
			if (this.shardReward == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			else
			{
				this.shardRewardAnimator = this.shardReward.GetComponent<Animator>();
			}
			text = "reward_vfx_locator/vfx_back_contraband";
			this.contrabandBackVfx = this.crateController.transform.Find(text).gameObject;
			if (this.contrabandBackVfx == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_vfx_locator/vfx_back_reputation";
			this.reputationBackVfx = this.crateController.transform.Find(text).gameObject;
			if (this.reputationBackVfx == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_vfx_locator/vfx_back_crystal";
			this.crystalBackVfx = this.crateController.transform.Find(text).gameObject;
			if (this.crystalBackVfx == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_vfx_locator/vfx_back_credit";
			this.creditBackVfx = this.crateController.transform.Find(text).gameObject;
			if (this.creditBackVfx == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_vfx_locator/vfx_back_alloy";
			this.materialBackVfx = this.crateController.transform.Find(text).gameObject;
			if (this.materialBackVfx == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_vfx_locator/vfx_back_unit";
			this.unitBackVfx = this.crateController.transform.Find(text).gameObject;
			if (this.unitBackVfx == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
			text = "reward_vfx_locator/vfx_back_shard";
			this.shardBackVfx = this.crateController.transform.Find(text).gameObject;
			if (this.shardBackVfx == null)
			{
				Service.Logger.ErrorFormat("Could not find gameObject {0} in crate controller", new object[]
				{
					text
				});
			}
		}

		public bool AvailableToAnimate()
		{
			return this.crateAnimator != null;
		}

		private void UpdateCrateAnimState(InventoryCrateAnimationState nextState)
		{
			this.crateAnimState = nextState;
			this.PlayAnimStateSFX(this.crateAnimState);
		}

		private void PlayAnimStateSFX(InventoryCrateAnimationState nextState)
		{
			Service.EventManager.SendEvent(EventId.InventoryCrateAnimationStateChange, nextState);
		}

		private void PlayVFXTriggerSFX(string sfxName)
		{
			Service.EventManager.SendEvent(EventId.InventoryCrateAnimVFXTriggered, sfxName);
		}

		private void OnCrateLanded(uint TimerId, object cookie)
		{
			this.UpdateCrateAnimState(InventoryCrateAnimationState.Landed);
			this.screen.ShowOpenButton();
			if (this.playOpenOnLanding)
			{
				this.playOpenOnLanding = false;
				this.crateLandTimer = Service.ViewTimerManager.CreateViewTimer(2.1f, false, new TimerDelegate(this.UpdateToOpenAfterDelay), null);
			}
		}

		private void UpdateToOpenAfterDelay(uint TimerId, object cookie)
		{
			this.UpdateCrateAnimState(InventoryCrateAnimationState.Open);
		}

		private void HideAllRewards()
		{
			if (this.contrabandCurrencyReward != null)
			{
				this.contrabandCurrencyReward.SetActive(false);
			}
			else
			{
				Service.Logger.Error("contrabandCurrencyReward has been destroyed during possible use");
			}
			if (this.reputationReward != null)
			{
				this.reputationReward.SetActive(false);
			}
			else
			{
				Service.Logger.Error("reputationReward has been destroyed during possible use");
			}
			if (this.crystalCurrencyReward != null)
			{
				this.crystalCurrencyReward.SetActive(false);
			}
			else
			{
				Service.Logger.Error("crystalCurrencyReward has been destroyed during possible use");
			}
			if (this.creditCurrencyReward != null)
			{
				this.creditCurrencyReward.SetActive(false);
			}
			else
			{
				Service.Logger.Error("creditCurrencyReward has been destroyed during possible use");
			}
			if (this.materialCurrencyReward != null)
			{
				this.materialCurrencyReward.SetActive(false);
			}
			else
			{
				Service.Logger.Error("materialCurrencyReward has been destroyed during possible use");
			}
			if (this.unitReward != null)
			{
				this.unitReward.SetActive(false);
			}
			else
			{
				Service.Logger.Error("unitReward has been destroyed during possible use");
			}
			if (this.shardReward != null)
			{
				this.shardReward.SetActive(false);
			}
			else
			{
				Service.Logger.Error("shardReward has been destroyed during possible use");
			}
			if (this.contrabandBackVfx != null)
			{
				this.contrabandBackVfx.SetActive(false);
			}
			else
			{
				Service.Logger.Error("contrabandBackVfx has been destroyed during possible use");
			}
			if (this.reputationBackVfx != null)
			{
				this.reputationBackVfx.SetActive(false);
			}
			else
			{
				Service.Logger.Error("reputationBackVfx has been destroyed during possible use");
			}
			if (this.crystalBackVfx != null)
			{
				this.crystalBackVfx.SetActive(false);
			}
			else
			{
				Service.Logger.Error("crystalBackVfx has been destroyed during possible use");
			}
			if (this.creditBackVfx != null)
			{
				this.creditBackVfx.SetActive(false);
			}
			else
			{
				Service.Logger.Error("creditBackVfx has been destroyed during possible use");
			}
			if (this.materialBackVfx != null)
			{
				this.materialBackVfx.SetActive(false);
			}
			else
			{
				Service.Logger.Error("materialBackVfx has been destroyed during possible use");
			}
			if (this.unitBackVfx != null)
			{
				this.unitBackVfx.SetActive(false);
			}
			else
			{
				Service.Logger.Error("unitBackVfx has been destroyed during possible use");
			}
			if (this.shardBackVfx != null)
			{
				this.shardBackVfx.SetActive(false);
			}
			else
			{
				Service.Logger.Error("shardBackVfx has been destroyed during possible use");
			}
		}

		private void LoadSuccess(object asset, object cookie)
		{
			if (asset is GameObject)
			{
				GameObject gameObject = asset as GameObject;
				if (gameObject.name == "crate_controller")
				{
					this.crateController = gameObject;
					this.crateController.transform.position = InventoryCrateAnimation.RIG_OFFSET;
					this.crateController.SetActive(false);
				}
				else if (this.crateVO.RewardAnimationAssetName == gameObject.name)
				{
					this.crateObj = gameObject;
				}
				else
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}

		public void ShowNextReward()
		{
			this.isSkippingReward = false;
			if (this.crateAnimState == InventoryCrateAnimationState.Landed)
			{
				this.UpdateCrateAnimState(InventoryCrateAnimationState.Open);
			}
			else if (this.crateAnimState == InventoryCrateAnimationState.Falling)
			{
				this.playOpenOnLanding = true;
			}
			else
			{
				this.PlayAnimStateSFX(InventoryCrateAnimationState.Hop);
			}
			this.rewardAnimStarted = true;
			this.rewardAnimEventReceived = false;
			this.crateAnimator.SetTrigger("ShowReward");
		}

		private void SetupCrateRewardItem(SupplyCrateTag crateTag)
		{
			CrateSupplyVO crateSupply = crateTag.CrateSupply;
			this.HideAllRewards();
			switch (crateSupply.Type)
			{
			case SupplyType.Currency:
				this.ActivateCurrencyRewardItem(crateSupply.RewardUid);
				break;
			case SupplyType.Shard:
			case SupplyType.ShardTroop:
			case SupplyType.ShardSpecialAttack:
				if (this.shardReward != null)
				{
					this.shardReward.SetActive(true);
					if (this.isSkippingReward)
					{
						this.shardRewardAnimator.SetTrigger("ShowDiscSpinEnd");
					}
					this.SetupRenderTargetMesh(this.shardReward, crateTag);
					this.SetupShardQualityDisplay(this.shardReward, crateTag);
				}
				if (this.shardBackVfx != null)
				{
					this.shardBackVfx.SetActive(true);
				}
				break;
			case SupplyType.Troop:
			case SupplyType.Hero:
			case SupplyType.SpecialAttack:
				if (this.unitReward != null)
				{
					this.unitReward.SetActive(true);
					this.SetupRenderTargetMesh(this.unitReward, crateTag);
				}
				if (this.unitBackVfx != null)
				{
					this.unitBackVfx.SetActive(true);
				}
				break;
			}
		}

		private void SetupRenderTargetMesh(GameObject rootReward, SupplyCrateTag crateTag)
		{
			Transform transform = this.assetMeshDataMonoBehavior.OtherGameObjects[7].transform;
			GameObject gameObject = transform.Find("datacardscreenmesh").gameObject;
			Renderer component = gameObject.GetComponent<Renderer>();
			component.sharedMaterial.mainTexture = crateTag.RenderTexture;
			component.sharedMaterial.shader = Shader.Find("Unlit/Transparent Colored");
		}

		private void SetupShardQualityDisplay(GameObject rootReward, SupplyCrateTag crateTag)
		{
			Transform transform = this.assetMeshDataMonoBehavior.OtherGameObjects[7].transform;
			GameObject gameObject = transform.Find("rarity_star_01").gameObject;
			GameObject gameObject2 = transform.Find("rarity_star_02").gameObject;
			GameObject gameObject3 = transform.Find("rarity_star_03").gameObject;
			GameObject gameObject4 = transform.Find("rarity_star_10").gameObject;
			gameObject.SetActive(false);
			gameObject2.SetActive(false);
			gameObject3.SetActive(false);
			gameObject4.SetActive(false);
			GameObject gameObject5 = this.assetMeshDataMonoBehavior.OtherGameObjects[3];
			GameObject gameObject6 = this.assetMeshDataMonoBehavior.OtherGameObjects[4];
			GameObject gameObject7 = this.assetMeshDataMonoBehavior.OtherGameObjects[5];
			GameObject gameObject8 = this.assetMeshDataMonoBehavior.OtherGameObjects[6];
			gameObject5.SetActive(false);
			gameObject6.SetActive(false);
			gameObject7.SetActive(false);
			gameObject8.SetActive(false);
			ShardQuality shardQuailty = crateTag.ShardQuailty;
			switch (shardQuailty)
			{
			case ShardQuality.Basic:
				gameObject.SetActive(true);
				gameObject5.SetActive(true);
				break;
			case ShardQuality.Advanced:
				gameObject2.SetActive(true);
				gameObject6.SetActive(true);
				break;
			case ShardQuality.Elite:
				gameObject3.SetActive(true);
				gameObject7.SetActive(true);
				break;
			default:
				if (shardQuailty == ShardQuality.Unit)
				{
					gameObject4.SetActive(true);
					gameObject8.SetActive(true);
				}
				break;
			}
			this.ChangeCrateRayColor();
		}

		private void ChangeCrateRayColor()
		{
			if (this.crateRayEffect != null)
			{
				List<SupplyCrateTag> rewardList = this.GetRewardList();
				int index = this.GetCurrentRewardIndex() - 1;
				Color color = this.BASIC_COLOR;
				if (rewardList[index].ShardQuailty == ShardQuality.Advanced)
				{
					color = this.ADVANCED_COLOR;
				}
				else if (rewardList[index].ShardQuailty == ShardQuality.Elite)
				{
					color = this.ELITE_COLOR;
				}
				else if (rewardList[index].ShardQuailty == ShardQuality.Unit)
				{
					color = this.UNIT_COLOR;
				}
				if (this.beamsEffect != null)
				{
					this.beamsEffect.GetComponent<Renderer>().sharedMaterial.SetColor("_CrateRayColor", color);
				}
				if (this.sparkleEffect != null)
				{
					this.sparkleEffect.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);
				}
				if (rewardList[index].ShardQuailty == ShardQuality.Elite)
				{
					color = this.ELITE_BURST_COLOR;
				}
				ParticleSystem.MainModule main = this.crateBurstParticles.main;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				startColor = color;
				main.startColor = startColor;
			}
		}

		private void ActivateCurrencyRewardItem(string rewardId)
		{
			switch (StringUtils.ParseEnum<CurrencyType>(rewardId))
			{
			case CurrencyType.Credits:
				if (this.creditCurrencyReward != null)
				{
					this.creditCurrencyReward.SetActive(true);
				}
				if (this.creditBackVfx != null)
				{
					this.creditBackVfx.SetActive(true);
				}
				break;
			case CurrencyType.Materials:
				if (this.materialCurrencyReward != null)
				{
					this.materialCurrencyReward.SetActive(true);
				}
				if (this.materialBackVfx != null)
				{
					this.materialBackVfx.SetActive(true);
				}
				break;
			case CurrencyType.Contraband:
				if (this.contrabandCurrencyReward != null)
				{
					this.contrabandCurrencyReward.SetActive(true);
				}
				if (this.contrabandBackVfx != null)
				{
					this.contrabandBackVfx.SetActive(true);
				}
				break;
			case CurrencyType.Reputation:
				if (this.reputationReward != null)
				{
					this.reputationReward.SetActive(true);
				}
				if (this.reputationBackVfx != null)
				{
					this.reputationBackVfx.SetActive(true);
				}
				break;
			case CurrencyType.Crystals:
				if (this.crystalCurrencyReward != null)
				{
					this.crystalCurrencyReward.SetActive(true);
				}
				if (this.crystalBackVfx != null)
				{
					this.crystalBackVfx.SetActive(true);
				}
				break;
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			bool flag = cookie != null && (bool)cookie;
			if (id == EventId.PlayCrateAnimVfx && !flag)
			{
				if (!this.isSkippingReward && this.rewardIndex >= this.supplyCrateDataList.Count - 1)
				{
					this.screen.ChangePrimaryButtonToClose();
				}
				if (this.rewardIndex >= this.supplyCrateDataList.Count)
				{
					if (!this.isSkippingReward)
					{
						this.rewardIndex++;
						this.HideAllRewards();
					}
				}
				else
				{
					this.currentCrateItem = this.supplyCrateDataList[this.rewardIndex];
					if (!this.isSkippingReward)
					{
						this.rewardIndex++;
						this.SetupCrateRewardItem(this.currentCrateItem);
						this.crateVfxParticles.Play();
						this.PlayVFXTriggerSFX(this.currentCrateItem.CrateSupply.RewardAnimationTierSfx);
						this.rewardAnimEventReceived = true;
					}
				}
			}
			else if (id == EventId.CrateRewardAnimDone)
			{
				if (this.rewardIndex > this.supplyCrateDataList.Count)
				{
					return EatResponse.NotEaten;
				}
				if (this.IsCurrentItemRewardAShard())
				{
					this.HandleShardRewardItemShow();
				}
				else
				{
					this.screen.ShowNameAndAmountUI(this.currentCrateItem);
				}
			}
			return EatResponse.NotEaten;
		}

		public void SkipToEndOfCrateRewardAnim()
		{
			if (this.IsSkippingAllowed())
			{
				this.isSkippingReward = true;
				this.rewardAnimStarted = false;
				this.rewardAnimEventReceived = false;
				this.crateAnimator.SetTrigger("ShowRewardEnd");
			}
		}

		private bool IsSkippingAllowed()
		{
			return !this.isSkippingReward && this.rewardAnimStarted && this.rewardAnimEventReceived;
		}

		public void CleanUp()
		{
			Service.EventManager.UnregisterObserver(this, EventId.PlayCrateAnimVfx);
			Service.EventManager.UnregisterObserver(this, EventId.CrateRewardAnimDone);
			if (this.crateLandTimer != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.crateLandTimer);
				this.crateLandTimer = 0u;
			}
			int i = 0;
			int count = this.supplyCrateDataList.Count;
			while (i < count)
			{
				this.supplyCrateDataList[i].CleanUp();
				this.supplyCrateDataList[i] = null;
				i++;
			}
			this.supplyCrateDataList.Clear();
			UnityEngine.Object.Destroy(this.crateObj);
			UnityEngine.Object.Destroy(this.crateController);
			int count2 = this.assetHandles.Count;
			for (int j = 0; j < count2; j++)
			{
				Service.AssetManager.Unload(this.assetHandles[j]);
				this.assetHandles[j] = AssetHandle.Invalid;
			}
			this.assetHandles.Clear();
			Service.UXController.RestoreVisibilityToAll();
		}
	}
}
