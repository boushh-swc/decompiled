using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Crates;
using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Controllers
{
	public class InventoryCrateRewardController : IEventObserver
	{
		private const string CRATE_EXPIRATION_WARNING_TOAST_TITLE = "CRATE_EXPIRATION_WARNING_TOAST_TITLE";

		private const string CRATE_EXPIRATION_WARNING_TOAST_DESC = "CRATE_EXPIRATION_WARNING_TOAST_DESC";

		private const uint RETRY_TOAST_CLAMP_SEC = 60u;

		private uint nextCrateExpireTime;

		private uint expireToastTimerId;

		private uint expireBadgeTimerId;

		private uint nextDailyCrateTimerId;

		public bool IsCrateAnimationShowingOrPending
		{
			get;
			private set;
		}

		public InventoryCrateRewardController()
		{
			Service.InventoryCrateRewardController = this;
			Service.EventManager.RegisterObserver(this, EventId.CrateInventoryUpdated);
			Service.EventManager.RegisterObserver(this, EventId.InventoryCrateCollectionClosed);
			this.expireToastTimerId = 0u;
			this.expireBadgeTimerId = 0u;
			this.nextDailyCrateTimerId = 0u;
			this.nextCrateExpireTime = 0u;
			this.ScheduleCrateExpireBadgeUpdate();
			this.ScheduleCrateExpireToast();
			this.IsCrateAnimationShowingOrPending = false;
		}

		public InventoryCrateAnimation ShowInventoryCrateAnimation(List<CrateSupplyVO> crateSupplyDataList, CrateData crateData, Dictionary<string, int> shardsOriginal, Dictionary<string, int> equipmentOriginal, Dictionary<string, int> troopUpgradeOriginal, Dictionary<string, int> specialAttackUpgradeOriginal)
		{
			if (crateSupplyDataList == null || crateSupplyDataList.Count <= 0)
			{
				return null;
			}
			if (crateData == null)
			{
				return null;
			}
			return new InventoryCrateAnimation(crateSupplyDataList, crateData, shardsOriginal, equipmentOriginal, troopUpgradeOriginal, specialAttackUpgradeOriginal);
		}

		public InventoryCrateAnimation GrantInventoryCrateReward(List<string> crateSupplyDataList, CrateData crateData)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (crateSupplyDataList == null || crateSupplyDataList.Count <= 0)
			{
				return null;
			}
			if (crateData == null)
			{
				return null;
			}
			this.IsCrateAnimationShowingOrPending = true;
			UnlockedLevelData unlockedLevels = Service.CurrentPlayer.UnlockedLevels;
			Dictionary<string, int> shardsOriginal = new Dictionary<string, int>(currentPlayer.Shards);
			Dictionary<string, int> troopUpgradeOriginal = new Dictionary<string, int>(unlockedLevels.Troops.Levels);
			Dictionary<string, int> specialAttackUpgradeOriginal = new Dictionary<string, int>(unlockedLevels.Starships.Levels);
			Dictionary<string, int> equipmentOriginal = new Dictionary<string, int>(currentPlayer.UnlockedLevels.Equipment.Levels);
			List<CrateSupplyVO> list = new List<CrateSupplyVO>();
			int hQLevel = crateData.HQLevel;
			int i = 0;
			int count = crateSupplyDataList.Count;
			while (i < count)
			{
				list.Add(this.GrantSingleSupplyCrateReward(crateSupplyDataList[i], hQLevel));
				i++;
			}
			return this.ShowInventoryCrateAnimation(list, crateData, shardsOriginal, equipmentOriginal, troopUpgradeOriginal, specialAttackUpgradeOriginal);
		}

		public void GrantShopSupply(CrateSupplyVO supplyData, int quantity)
		{
			RewardVO rewardVO = new RewardVO();
			rewardVO.Uid = supplyData.RewardUid;
			string[] array = new string[]
			{
				supplyData.RewardUid + ":" + quantity
			};
			switch (supplyData.Type)
			{
			case SupplyType.Currency:
				rewardVO.CurrencyRewards = array;
				Service.RewardManager.TryAndGrantReward(rewardVO, null, null, false);
				break;
			case SupplyType.Shard:
				rewardVO.ShardRewards = array;
				Service.RewardManager.TryAndGrantReward(rewardVO, null, null, false);
				break;
			case SupplyType.Troop:
				rewardVO.TroopRewards = array;
				GameUtils.AddRewardToInventory(rewardVO);
				break;
			case SupplyType.Hero:
				rewardVO.HeroRewards = array;
				GameUtils.AddRewardToInventory(rewardVO);
				break;
			case SupplyType.SpecialAttack:
				rewardVO.SpecialAttackRewards = array;
				GameUtils.AddRewardToInventory(rewardVO);
				break;
			case SupplyType.ShardTroop:
				Service.DeployableShardUnlockController.GrantUnlockShards(rewardVO.Uid, quantity);
				break;
			case SupplyType.ShardSpecialAttack:
				Service.DeployableShardUnlockController.GrantUnlockShards(rewardVO.Uid, quantity);
				break;
			}
		}

		private CrateSupplyVO GrantSingleSupplyCrateReward(string crateSupplyId, int hqLevel)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CrateSupplyVO crateSupplyVO = staticDataController.Get<CrateSupplyVO>(crateSupplyId);
			this.GrantSingleSupplyCrateReward(crateSupplyVO, hqLevel);
			return crateSupplyVO;
		}

		public void GrantSingleSupplyCrateReward(CrateSupplyVO crateSupplyData, int hqLevel)
		{
			switch (crateSupplyData.Type)
			{
			case SupplyType.Currency:
			case SupplyType.Shard:
			{
				RewardVO vo = this.GenerateRewardFromSupply(crateSupplyData, hqLevel);
				Service.RewardManager.TryAndGrantReward(vo, null, null, false);
				break;
			}
			case SupplyType.Troop:
			case SupplyType.Hero:
			case SupplyType.SpecialAttack:
			{
				RewardVO vo2 = this.GenerateRewardFromSupply(crateSupplyData, hqLevel);
				GameUtils.AddRewardToInventory(vo2);
				break;
			}
			case SupplyType.ShardTroop:
			case SupplyType.ShardSpecialAttack:
				Service.DeployableShardUnlockController.GrantUnlockShards(crateSupplyData.RewardUid, this.GetRewardAmount(crateSupplyData, hqLevel));
				break;
			case SupplyType.Invalid:
				Service.Logger.Error("Supply Type Invalid: " + crateSupplyData.Uid);
				break;
			}
		}

		public RewardVO GenerateRewardFromSupply(CrateSupplyVO supplyData, int hqLevel)
		{
			RewardVO rewardVO = new RewardVO();
			rewardVO.Uid = ((!string.IsNullOrEmpty(supplyData.ScalingUid)) ? supplyData.ScalingUid : supplyData.RewardUid);
			switch (supplyData.Type)
			{
			case SupplyType.Currency:
				rewardVO.CurrencyRewards = this.GetUnitRewardUid(supplyData, hqLevel);
				break;
			case SupplyType.Shard:
				rewardVO.ShardRewards = this.GetUnitRewardUid(supplyData, hqLevel);
				break;
			case SupplyType.Troop:
				rewardVO.TroopRewards = this.GetUnitRewardUid(supplyData, hqLevel);
				break;
			case SupplyType.Hero:
				rewardVO.HeroRewards = this.GetUnitRewardUid(supplyData, hqLevel);
				break;
			case SupplyType.SpecialAttack:
				rewardVO.SpecialAttackRewards = this.GetUnitRewardUid(supplyData, hqLevel);
				break;
			}
			return rewardVO;
		}

		public string GetCrateSupplyRewardName(CrateSupplyVO supplyData)
		{
			string text = string.Empty;
			if (supplyData == null)
			{
				return text;
			}
			Lang lang = Service.Lang;
			StaticDataController staticDataController = Service.StaticDataController;
			string rewardUid = supplyData.RewardUid;
			SupplyType type = supplyData.Type;
			switch (type)
			{
			case SupplyType.Currency:
				text = lang.Get(supplyData.RewardUid.ToUpper(), new object[0]);
				break;
			case SupplyType.Shard:
			{
				EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(rewardUid);
				if (currentEquipmentDataByID != null)
				{
					text = lang.Get(currentEquipmentDataByID.EquipmentName, new object[0]);
				}
				break;
			}
			case SupplyType.Troop:
			case SupplyType.Hero:
			{
				TroopTypeVO optional = staticDataController.GetOptional<TroopTypeVO>(rewardUid);
				if (optional != null)
				{
					text = LangUtils.GetTroopDisplayName(optional);
				}
				break;
			}
			case SupplyType.SpecialAttack:
			{
				SpecialAttackTypeVO optional2 = staticDataController.GetOptional<SpecialAttackTypeVO>(rewardUid);
				if (optional2 != null)
				{
					text = LangUtils.GetStarshipDisplayName(optional2);
				}
				break;
			}
			case SupplyType.ShardTroop:
			{
				ShardVO optional3 = staticDataController.GetOptional<ShardVO>(rewardUid);
				if (optional3 != null)
				{
					text = LangUtils.GetTroopDisplayNameFromTroopID(optional3.TargetGroupId);
				}
				break;
			}
			case SupplyType.ShardSpecialAttack:
			{
				ShardVO optional4 = staticDataController.GetOptional<ShardVO>(rewardUid);
				if (optional4 != null)
				{
					text = LangUtils.GetStarshipDisplayNameFromAttackID(optional4.TargetGroupId);
				}
				break;
			}
			}
			if (string.IsNullOrEmpty(text))
			{
				Service.Logger.ErrorFormat("CrateSupplyVO Uid:{0}, Cannot find reward for RewardUid:{1}, Type:{2}", new object[]
				{
					supplyData.Uid,
					rewardUid,
					type
				});
			}
			return text;
		}

		public int GetRewardAmount(CrateSupplyVO supplyData, int hqLevel)
		{
			if (supplyData == null)
			{
				Service.Logger.ErrorFormat("Crate reward given to GetRewardAmount is null", new object[0]);
				return 0;
			}
			CrateSupplyScaleVO crateSupplyScaleVO = null;
			if (!string.IsNullOrEmpty(supplyData.ScalingUid))
			{
				crateSupplyScaleVO = Service.StaticDataController.GetOptional<CrateSupplyScaleVO>(supplyData.ScalingUid);
			}
			int result = 0;
			switch (supplyData.Type)
			{
			case SupplyType.Currency:
			case SupplyType.Shard:
			case SupplyType.ShardTroop:
			case SupplyType.ShardSpecialAttack:
				if (crateSupplyScaleVO == null)
				{
					Service.Logger.ErrorFormat("Crate reward {0} requires HQ scaling data, but none found", new object[]
					{
						supplyData.Uid,
						supplyData.ScalingUid
					});
				}
				else
				{
					result = crateSupplyScaleVO.GetHQScaling(hqLevel);
				}
				break;
			case SupplyType.Troop:
			case SupplyType.Hero:
			case SupplyType.SpecialAttack:
				result = supplyData.Amount;
				break;
			}
			return result;
		}

		public string CalculatePlanetRewardChecksum(Planet currentPlanet)
		{
			List<PlanetLootEntryVO> featuredLootEntriesForPlanet = Service.InventoryCrateRewardController.GetFeaturedLootEntriesForPlanet(currentPlanet);
			featuredLootEntriesForPlanet.Sort(new Comparison<PlanetLootEntryVO>(this.SortFeaturedLootEntries));
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int count = featuredLootEntriesForPlanet.Count;
			while (i < count)
			{
				stringBuilder.Append(featuredLootEntriesForPlanet[i].Uid);
				i++;
			}
			return CryptographyUtils.ComputeMD5Hash(stringBuilder.ToString());
		}

		public List<PlanetLootEntryVO> GetFeaturedLootEntriesForPlanet(Planet currentPlanet)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			List<PlanetLootEntryVO> list = new List<PlanetLootEntryVO>();
			PlanetLootVO planetLootVO = staticDataController.Get<PlanetLootVO>(currentPlanet.VO.PlanetLootUid);
			string[] array;
			if (currentPlayer.Faction == FactionType.Empire)
			{
				array = planetLootVO.EmpirePlanetLootEntryUids;
			}
			else
			{
				array = planetLootVO.RebelPlanetLootEntryUids;
			}
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				PlanetLootEntryVO optional = staticDataController.GetOptional<PlanetLootEntryVO>(array[i]);
				if (optional == null)
				{
					Service.Logger.ErrorFormat("Couldn't find PlanetLootEntryVO: {0} specified in PlanetLoot: {1}", new object[]
					{
						array[i],
						currentPlanet.VO.PlanetLootUid
					});
				}
				else if (this.IsPlanetLootEntryValidToShow(currentPlayer, optional))
				{
					list.Add(optional);
					if (list.Count >= GameConstants.PLANET_REWARDS_ITEM_THROTTLE)
					{
						break;
					}
				}
				i++;
			}
			return list;
		}

		public List<PlanetLootEntryVO> GetFeaturedLootEntriesForEpisodeTask(EpisodeTaskVO vo, int maxEntries)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			List<PlanetLootEntryVO> list = new List<PlanetLootEntryVO>();
			string[] array;
			if (currentPlayer.Faction == FactionType.Empire)
			{
				array = vo.EmpireRewardItemIds;
			}
			else
			{
				array = vo.RebelRewardItemIds;
			}
			if (array == null)
			{
				return list;
			}
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				PlanetLootEntryVO optional = staticDataController.GetOptional<PlanetLootEntryVO>(array[i]);
				if (optional == null)
				{
					Service.Logger.ErrorFormat("Couldn't find PlanetLootEntryVO: {0} specified in EpisodeTaskVO: {1}", new object[]
					{
						array[i],
						vo.Uid
					});
				}
				else if (this.IsPlanetLootEntryValidToShow(currentPlayer, optional))
				{
					list.Add(optional);
					if (list.Count >= maxEntries)
					{
						break;
					}
				}
				i++;
			}
			return list;
		}

		private int SortFeaturedLootEntries(PlanetLootEntryVO vo1, PlanetLootEntryVO vo2)
		{
			return string.Compare(vo1.Uid, vo2.Uid);
		}

		private bool IsPlanetLootEntryValidToShow(CurrentPlayer player, PlanetLootEntryVO lootEntry)
		{
			int num = player.Map.FindHighestHqLevel();
			if (lootEntry.MinHQ > 0 && num < lootEntry.MinHQ)
			{
				return false;
			}
			if (lootEntry.MaxHQ > 0 && num > lootEntry.MaxHQ)
			{
				return false;
			}
			int time = (int)ServerTime.Time;
			return lootEntry.ShowDateTimeStamp <= time && (lootEntry.HideDateTimeStamp <= 0 || time < lootEntry.HideDateTimeStamp);
		}

		private string[] GetUnitRewardUid(CrateSupplyVO supplyData, int hqLevel)
		{
			return new string[]
			{
				supplyData.RewardUid + ":" + this.GetRewardAmount(supplyData, hqLevel)
			};
		}

		private CrateData GetCrateSoonestToExpire(uint afterTime)
		{
			InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
			CrateData crateData = null;
			foreach (CrateData current in crates.Available.Values)
			{
				if (current.DoesExpire)
				{
					if (current.ExpiresTimeStamp >= afterTime)
					{
						if (crateData == null || crateData.ExpiresTimeStamp > current.ExpiresTimeStamp)
						{
							crateData = current;
						}
					}
				}
			}
			return crateData;
		}

		private void ScheduleCrateExpireBadgeUpdate()
		{
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			uint time = ServerTime.Time;
			if (this.expireBadgeTimerId != 0u)
			{
				viewTimerManager.KillViewTimer(this.expireBadgeTimerId);
			}
			if (Service.CurrentPlayer == null)
			{
				return;
			}
			CrateData crateSoonestToExpire = this.GetCrateSoonestToExpire(time);
			if (crateSoonestToExpire == null)
			{
				return;
			}
			uint expiresTimeStamp = crateSoonestToExpire.ExpiresTimeStamp;
			uint num = expiresTimeStamp - time;
			if (num <= 432000u)
			{
				this.expireBadgeTimerId = viewTimerManager.CreateViewTimer(num, false, new TimerDelegate(this.UpdateBadgesAfterCrateExpire), null);
			}
		}

		private void UpdateBadgesAfterCrateExpire(uint timerId, object cookie)
		{
			InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
			crates.UpdateBadgingBasedOnAvailableCrates();
			this.ScheduleCrateExpireBadgeUpdate();
		}

		public void ScheduleGivingNextDailyCrate()
		{
			this.CancelDailyCrateScheduledTimer();
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
			uint nextDailCrateTimeWithSyncBuffer = crates.GetNextDailCrateTimeWithSyncBuffer();
			uint time = ServerTime.Time;
			uint num = 0u;
			if (nextDailCrateTimeWithSyncBuffer > time)
			{
				num = nextDailCrateTimeWithSyncBuffer - time;
			}
			if (num <= 432000u)
			{
				this.nextDailyCrateTimerId = viewTimerManager.CreateViewTimer(num, false, new TimerDelegate(this.UpdateDailyCrateInventory), null);
			}
		}

		public void CancelDailyCrateScheduledTimer()
		{
			if (this.nextDailyCrateTimerId != 0u)
			{
				ViewTimerManager viewTimerManager = Service.ViewTimerManager;
				viewTimerManager.KillViewTimer(this.nextDailyCrateTimerId);
				this.nextDailyCrateTimerId = 0u;
			}
		}

		private void UpdateDailyCrateInventory(uint timerId, object cookie)
		{
			this.nextDailyCrateTimerId = 0u;
			CheckDailyCrateRequest request = new CheckDailyCrateRequest();
			CheckDailyCrateCommand command = new CheckDailyCrateCommand(request);
			Service.ServerAPI.Sync(command);
		}

		private void ScheduleCrateExpireToast()
		{
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			uint num = (uint)(GameConstants.CRATE_EXPIRATION_WARNING_TOAST * 60);
			uint time = ServerTime.Time;
			uint afterTime = time + num;
			if (this.expireToastTimerId != 0u)
			{
				viewTimerManager.KillViewTimer(this.expireToastTimerId);
			}
			if (Service.CurrentPlayer == null)
			{
				return;
			}
			CrateData crateSoonestToExpire = this.GetCrateSoonestToExpire(afterTime);
			if (crateSoonestToExpire == null)
			{
				return;
			}
			uint num2 = crateSoonestToExpire.ExpiresTimeStamp - num;
			uint num3 = num2 - time;
			if (num3 <= 432000u)
			{
				this.nextCrateExpireTime = crateSoonestToExpire.ExpiresTimeStamp;
				this.expireToastTimerId = viewTimerManager.CreateViewTimer(num3, false, new TimerDelegate(this.AttemptShowCrateExpireToast), null);
			}
		}

		private bool IsValidGameStateForToast()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			bool flag = Service.UXController.MiscElementsManager.CanShowToast(currentState);
			bool flag2 = currentState is HomeState || currentState is EditBaseState || currentState is BaseLayoutToolState || currentState is GalaxyState || currentState is WarBoardState;
			if (flag && flag2)
			{
				SelectedBuildingScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<SelectedBuildingScreen>();
				return highestLevelScreen == null;
			}
			return false;
		}

		private void ShowCrateExpireToast()
		{
			uint num = this.nextCrateExpireTime - 60u;
			this.nextCrateExpireTime = 0u;
			if (num < ServerTime.Time)
			{
				return;
			}
			Lang lang = Service.Lang;
			string toast = lang.Get("CRATE_EXPIRATION_WARNING_TOAST_TITLE", new object[0]);
			string status = lang.Get("CRATE_EXPIRATION_WARNING_TOAST_DESC", new object[0]);
			Service.UXController.MiscElementsManager.ShowToast(toast, status, string.Empty);
			this.ScheduleCrateExpireToast();
		}

		private void AttemptShowCrateExpireToast(uint TimerId, object cookie)
		{
			if (this.IsValidGameStateForToast())
			{
				this.ShowCrateExpireToast();
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.WorldInTransitionComplete);
				Service.EventManager.RegisterObserver(this, EventId.GameStateChanged);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.WorldInTransitionComplete && id != EventId.GameStateChanged)
			{
				if (id != EventId.CrateInventoryUpdated)
				{
					if (id == EventId.InventoryCrateCollectionClosed)
					{
						this.IsCrateAnimationShowingOrPending = false;
					}
				}
				else
				{
					this.ScheduleCrateExpireToast();
					this.ScheduleCrateExpireBadgeUpdate();
				}
			}
			else if (this.IsValidGameStateForToast())
			{
				Service.EventManager.UnregisterObserver(this, EventId.WorldInTransitionComplete);
				Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
				this.ShowCrateExpireToast();
			}
			return EatResponse.NotEaten;
		}
	}
}
