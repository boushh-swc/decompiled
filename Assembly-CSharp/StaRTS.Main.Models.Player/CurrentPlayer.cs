using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Episodes;
using StaRTS.Main.Models.MobileConnectorAds;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Player
{
	public class CurrentPlayer : GamePlayer
	{
		private const int FREE_RELOCATION = -1;

		private const string CURRENT = "current";

		private const string UNLOCKED = "unlocked";

		private const string LOCKED = "locked";

		private string playerName;

		private string nextRaidId;

		private int attackRating;

		private int defenseRating;

		private FactionType faction;

		public BattleHistory BattleHistory
		{
			get;
			set;
		}

		public List<IStoryTrigger> ActiveSaveTriggers
		{
			get;
			private set;
		}

		public List<string> SpecOpIntros
		{
			get;
			set;
		}

		public string CurrentQuest
		{
			get;
			set;
		}

		public string RestoredQuest
		{
			get;
			set;
		}

		public uint FirstLoginTime
		{
			get;
			set;
		}

		public string PlayerId
		{
			get;
			private set;
		}

		public bool PlayerNameInvalid
		{
			get;
			set;
		}

		public uint RaidStartTime
		{
			get;
			private set;
		}

		public uint NextRaidStartTime
		{
			get;
			private set;
		}

		public uint RaidEndTime
		{
			get;
			private set;
		}

		public Dictionary<string, ObjectiveGroup> Objectives
		{
			get;
			set;
		}

		public List<string> UnlockedPlanets
		{
			get;
			private set;
		}

		public List<string> HolonetRewards
		{
			get;
			private set;
		}

		public CampaignMissionVO CurrentRaid
		{
			get;
			private set;
		}

		public string CurrentRaidPoolId
		{
			get;
			private set;
		}

		public string CurrentRaidId
		{
			get;
			private set;
		}

		private int RelocationStarsCount
		{
			get;
			set;
		}

		public PerksInfo PerksInfo
		{
			get;
			private set;
		}

		public EpisodeProgressInfo EpisodeProgressInfo
		{
			get;
			private set;
		}

		public MobileConnectorAdsInfo MobileConnectorAdsInfo
		{
			get;
			private set;
		}

		public ActiveArmory ActiveArmory
		{
			get;
			private set;
		}

		public ArmoryInfo ArmoryInfo
		{
			get;
			private set;
		}

		public Dictionary<string, int> Shards
		{
			get;
			set;
		}

		public TroopDonationProgress TroopDonationProgress
		{
			get;
			private set;
		}

		public override string PlayerName
		{
			get
			{
				return this.playerName;
			}
			set
			{
				this.playerName = value;
				this.PlayerNameInvalid = false;
				Service.EventManager.SendEvent(EventId.PlayerNameChanged, this.faction);
			}
		}

		public string PlanetId
		{
			get
			{
				return base.Map.PlanetId();
			}
		}

		public PlanetVO Planet
		{
			get
			{
				return base.Map.Planet;
			}
		}

		public override int AttackRating
		{
			get
			{
				return this.attackRating;
			}
			set
			{
				bool flag = this.attackRating != value;
				if (flag)
				{
					this.attackRating = value;
					Service.EventManager.SendEvent(EventId.PvpRatingChanged, null);
				}
			}
		}

		public override int DefenseRating
		{
			get
			{
				return this.defenseRating;
			}
			set
			{
				bool flag = this.defenseRating != value;
				if (flag)
				{
					this.defenseRating = value;
					Service.EventManager.SendEvent(EventId.PvpRatingChanged, null);
				}
			}
		}

		public bool CurrentlyDefending
		{
			get;
			set;
		}

		public uint CurrentlyDefendingExpireTime
		{
			get;
			set;
		}

		public CampaignProgress CampaignProgress
		{
			get;
			set;
		}

		public TournamentProgress TournamentProgress
		{
			get;
			set;
		}

		public PrizeInventory Prizes
		{
			get;
			set;
		}

		public bool FirstTimePlayer
		{
			get;
			set;
		}

		public uint ProtectedUntil
		{
			get;
			set;
		}

		public uint ProtectionFrom
		{
			get;
			set;
		}

		private Dictionary<string, uint> ProtectionCooldownUntil
		{
			get;
			set;
		}

		public uint LastLoginTime
		{
			get;
			set;
		}

		public uint LoginTime
		{
			get;
			set;
		}

		public int SessionCountToday
		{
			get;
			set;
		}

		public uint InstallDate
		{
			get;
			set;
		}

		public Dictionary<string, int> DamagedBuildings
		{
			get;
			set;
		}

		public bool IsConnectedAccount
		{
			get;
			set;
		}

		public bool IsRateIncentivized
		{
			get;
			set;
		}

		public bool IsPushIncentivized
		{
			get;
			set;
		}

		public int NumIdentities
		{
			get;
			set;
		}

		public List<string> Patches
		{
			get;
			set;
		}

		public uint LastWarParticipationTime
		{
			get;
			set;
		}

		public string OfferId
		{
			get;
			set;
		}

		public uint TriggerDate
		{
			get;
			set;
		}

		public Dictionary<string, object> AbTests
		{
			get;
			set;
		}

		public uint LastPaymentTime
		{
			get;
			set;
		}

		public override FactionType Faction
		{
			get
			{
				return this.faction;
			}
			set
			{
				this.faction = value;
				Service.EventManager.SendEvent(EventId.PlayerFactionChanged, this.faction);
			}
		}

		public int CurrentCrystalsAmount
		{
			get
			{
				return base.Inventory.GetItemAmount("crystals");
			}
		}

		public int CurrentDroidsAmount
		{
			get
			{
				return base.Inventory.GetItemAmount("droids");
			}
		}

		public int MaxDroidsAmount
		{
			get
			{
				return base.Inventory.GetItemCapacity("droids");
			}
		}

		public CurrentPlayer()
		{
			Service.CurrentPlayer = this;
			this.SetPlayerId();
		}

		public bool HasNotCompletedFirstFueStep()
		{
			bool flag = this.FirstTimePlayer || this.RestoredQuest == GameConstants.FUE_QUEST_UID;
			if (flag)
			{
				flag = Service.PlayerIdentityController.IsFirstIdentity(this.PlayerId);
			}
			return flag;
		}

		public void Init()
		{
			base.Map = new Map();
			base.Inventory = new Inventory();
			base.UnlockedLevels = new UnlockedLevelData();
			this.Prizes = new PrizeInventory();
			this.ActiveSaveTriggers = new List<IStoryTrigger>();
			this.FirstTimePlayer = true;
			this.CampaignProgress = new CampaignProgress();
			this.TournamentProgress = new TournamentProgress();
			this.BattleHistory = new BattleHistory();
			this.UnlockedPlanets = new List<string>();
			this.HolonetRewards = new List<string>();
			this.RelocationStarsCount = 0;
			this.NumIdentities = 1;
			this.Objectives = new Dictionary<string, ObjectiveGroup>();
			this.Shards = new Dictionary<string, int>();
		}

		private void SetPlayerId()
		{
			string text = null;
			if (PlayerPrefs.HasKey("prefPlayerId"))
			{
				text = PlayerPrefs.GetString("prefPlayerId");
			}
			if (string.IsNullOrEmpty(text))
			{
				text = Service.EnvironmentController.GetDeviceId();
			}
			this.PlayerId = text;
			Service.EventManager.SendEvent(EventId.PlayerIdSet, this.PlayerId);
		}

		public IEnumerable<KeyValuePair<string, InventoryEntry>> GetAllTroops()
		{
			return base.Inventory.Troop.GetInternalStorage();
		}

		public IEnumerable<KeyValuePair<string, InventoryEntry>> GetAllSpecialAttacks()
		{
			return base.Inventory.SpecialAttack.GetInternalStorage();
		}

		public IEnumerable<KeyValuePair<string, InventoryEntry>> GetAllHeroes()
		{
			return base.Inventory.Hero.GetInternalStorage();
		}

		public IEnumerable<KeyValuePair<string, InventoryEntry>> GetAllChampions()
		{
			return base.Inventory.Champion.GetInternalStorage();
		}

		public bool SetTroopCount(string uid, int amount)
		{
			TroopTypeVO optional = Service.StaticDataController.GetOptional<TroopTypeVO>(uid);
			if (optional == null)
			{
				Service.Logger.Error("Troop does not exist: " + uid);
				return false;
			}
			return this.SetDeployableCount(uid, amount, base.Inventory.Troop, optional.Size);
		}

		public void SetCurrentRaid(string raidUid)
		{
			this.nextRaidId = raidUid;
			if (!string.IsNullOrEmpty(this.nextRaidId) && Service.StaticDataController != null)
			{
				this.CurrentRaid = Service.StaticDataController.Get<CampaignMissionVO>(this.nextRaidId);
			}
		}

		public bool SetSpecialAttackCount(string uid, int amount)
		{
			SpecialAttackTypeVO optional = Service.StaticDataController.GetOptional<SpecialAttackTypeVO>(uid);
			if (optional == null)
			{
				Service.Logger.Error("Special Attack does not exist: " + uid);
				return false;
			}
			return this.SetDeployableCount(uid, amount, base.Inventory.SpecialAttack, optional.Size);
		}

		public bool SetHeroCount(string uid, int amount)
		{
			TroopTypeVO optional = Service.StaticDataController.GetOptional<TroopTypeVO>(uid);
			if (optional == null)
			{
				Service.Logger.Error("Hero does not exist: " + uid);
				return false;
			}
			return this.SetDeployableCount(uid, amount, base.Inventory.Hero, optional.Size);
		}

		public bool SetChampionCount(string uid, int amount)
		{
			TroopTypeVO optional = Service.StaticDataController.GetOptional<TroopTypeVO>(uid);
			if (optional == null)
			{
				Service.Logger.Error("Champion does not exist: " + uid);
				return false;
			}
			return this.SetDeployableCount(uid, amount, base.Inventory.Champion, optional.Size);
		}

		private bool SetDeployableCount(string uid, int amount, InventoryStorage storage, int size)
		{
			if (amount < 0)
			{
				Service.Logger.Debug("Cannot set a deployable count less than zero. uid: " + uid);
				return false;
			}
			int num = storage.GetTotalStorageAmount() + amount * size;
			if (num > storage.GetTotalStorageCapacity())
			{
				Service.Logger.Debug("Not enough capacity for deployable. uid: " + uid);
				return false;
			}
			int delta = amount - storage.GetItemAmount(uid);
			storage.ModifyItemAmount(uid, delta);
			return true;
		}

		public void AddProtectionCooldownUntil(string key, uint time)
		{
			if (this.ProtectionCooldownUntil == null)
			{
				this.ProtectionCooldownUntil = new Dictionary<string, uint>();
			}
			if (this.ProtectionCooldownUntil.ContainsKey(key))
			{
				this.ProtectionCooldownUntil[key] = time;
			}
			else
			{
				this.ProtectionCooldownUntil.Add(key, time);
			}
		}

		public void AddProtectionCooldownUntil(int packNumber, uint time)
		{
			string key = "protection" + packNumber;
			this.AddProtectionCooldownUntil(key, time);
		}

		public uint GetProtectionPurchaseCooldown(int packNumber)
		{
			string key = "protection" + packNumber;
			if (this.ProtectionCooldownUntil != null && this.ProtectionCooldownUntil.ContainsKey(key))
			{
				return this.ProtectionCooldownUntil[key];
			}
			return 0u;
		}

		public void RemoveTroop(string uid, int delta)
		{
			this.RemoveDeployable(uid, base.Inventory.Troop, delta);
		}

		public void RemoveSpecialAttack(string uid, int delta)
		{
			this.RemoveDeployable(uid, base.Inventory.SpecialAttack, delta);
		}

		public void RemoveHero(string uid, int delta)
		{
			this.RemoveDeployable(uid, base.Inventory.Hero, delta);
		}

		public void OnChampionKilled(string championUid)
		{
			this.RemoveChampionFromInventory(championUid);
			base.Inventory.Champion.SetItemCapacity(championUid, 0);
		}

		public void OnChampionRepaired(string championUid)
		{
			this.RemoveChampionFromInventory(championUid);
			base.Inventory.Champion.SetItemCapacity(championUid, 1);
			this.AddChampionToInventoryIfAlive(championUid);
		}

		public void RemoveChampionFromInventory(string championUid)
		{
			InventoryStorage champion = base.Inventory.Champion;
			champion.ModifyItemAmount(championUid, -champion.GetItemAmount(championUid));
		}

		public void AddChampionToInventoryIfAlive(string championUid)
		{
			InventoryStorage champion = base.Inventory.Champion;
			if (champion.GetItemCapacity(championUid) != 0)
			{
				champion.ModifyItemAmount(championUid, 1 - champion.GetItemAmount(championUid));
			}
		}

		private void RemoveDeployable(string uid, InventoryStorage storage, int delta)
		{
			int deployableCount = GameUtils.GetDeployableCount(uid, storage);
			if (deployableCount - delta < 0)
			{
				delta = deployableCount;
				Service.Logger.WarnFormat("Can not set a deployable count less than zero. uid: {0}, amount{1}, delta: {2}", new object[]
				{
					uid,
					deployableCount,
					delta
				});
			}
			storage.ModifyItemAmount(uid, -delta);
		}

		public Dictionary<string, int> RemoveAllDeployables()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Dictionary<string, InventoryEntry> internalStorage = base.Inventory.Troop.GetInternalStorage();
			foreach (KeyValuePair<string, InventoryEntry> current in internalStorage)
			{
				dictionary.Add(current.Key, 0);
				base.Inventory.Troop.ClearItemAmount(current.Key);
			}
			Dictionary<string, InventoryEntry> internalStorage2 = base.Inventory.SpecialAttack.GetInternalStorage();
			foreach (KeyValuePair<string, InventoryEntry> current2 in internalStorage2)
			{
				dictionary.Add(current2.Key, 0);
				base.Inventory.SpecialAttack.ClearItemAmount(current2.Key);
			}
			Dictionary<string, InventoryEntry> internalStorage3 = base.Inventory.Hero.GetInternalStorage();
			foreach (KeyValuePair<string, InventoryEntry> current3 in internalStorage3)
			{
				dictionary.Add(current3.Key, 0);
				base.Inventory.Hero.ClearItemAmount(current3.Key);
			}
			Dictionary<string, InventoryEntry> internalStorage4 = base.Inventory.Champion.GetInternalStorage();
			foreach (KeyValuePair<string, InventoryEntry> current4 in internalStorage4)
			{
				dictionary.Add(current4.Key, 0);
				base.Inventory.Champion.ClearItemAmount(current4.Key);
			}
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is HomeState || currentState is EditBaseState)
			{
				StorageSpreadUtils.UpdateAllStarportFullnessMeters();
			}
			return dictionary;
		}

		public void AddHolonetReward(string uid)
		{
			if (!string.IsNullOrEmpty(uid) && !this.HolonetRewards.Contains(uid))
			{
				this.HolonetRewards.Add(uid);
			}
		}

		public void UpdatePerksInfo(object rawPerksInfo)
		{
			if (rawPerksInfo != null)
			{
				this.PerksInfo = new PerksInfo();
				this.PerksInfo.FromObject(rawPerksInfo);
			}
		}

		public void UpdateEpisodeProgressInfo(object rawEpisodeProgressInfo)
		{
			this.UpdateEpisodeProgressInfoNoRefreshEvent(rawEpisodeProgressInfo);
			Service.EventManager.SendEvent(EventId.EpisodeProgressInfoRefreshed, null);
		}

		public void UpdateEpisodeProgressInfoNoRefreshEvent(object rawEpisodeProgressInfo)
		{
			if (rawEpisodeProgressInfo != null)
			{
				this.EpisodeProgressInfo = new EpisodeProgressInfo();
				this.EpisodeProgressInfo.FromObject(rawEpisodeProgressInfo);
			}
		}

		public void UpdateMobileConnectorAdsInfo(object rawMCAInfo)
		{
			if (rawMCAInfo != null)
			{
				this.MobileConnectorAdsInfo = new MobileConnectorAdsInfo();
				this.MobileConnectorAdsInfo.FromObject(rawMCAInfo);
			}
		}

		public void UpdateActiveArmory(object data)
		{
			if (data != null)
			{
				this.ActiveArmory = new ActiveArmory();
				this.ActiveArmory.FromObject(data);
			}
		}

		public void UpdateArmoryInfo(object data)
		{
			if (data != null)
			{
				this.ArmoryInfo = new ArmoryInfo();
				this.ArmoryInfo.FromObject(data);
			}
		}

		public void UpdateShardsInfo(object rawShardInfo)
		{
			Dictionary<string, object> dictionary = rawShardInfo as Dictionary<string, object>;
			if (dictionary != null)
			{
				foreach (string current in dictionary.Keys)
				{
					this.Shards.Add(current, Convert.ToInt32(dictionary[current]));
				}
			}
		}

		public int GetShards(string equipmentID)
		{
			return (!this.Shards.ContainsKey(equipmentID)) ? 0 : this.Shards[equipmentID];
		}

		public void SetTroopDonationProgress(object rawDonationInfo)
		{
			if (rawDonationInfo != null)
			{
				this.TroopDonationProgress = new TroopDonationProgress();
				this.TroopDonationProgress.FromObject(rawDonationInfo);
			}
		}

		public void UpdateTroopDonationProgress(int donationCount, int lastTrackedDonationTime, int repDonationCooldownTime)
		{
			this.TroopDonationProgress.DonationCount = donationCount;
			this.TroopDonationProgress.LastTrackedDonationTime = lastTrackedDonationTime;
			this.TroopDonationProgress.DonationCooldownEndTime = repDonationCooldownTime;
		}

		public void UpdateCurrentRaid(object raidData)
		{
			if (raidData != null)
			{
				Dictionary<string, object> dictionary = raidData as Dictionary<string, object>;
				string key = base.Map.PlanetId();
				object obj = null;
				if (dictionary.TryGetValue(key, out obj))
				{
					Dictionary<string, object> raidData2 = obj as Dictionary<string, object>;
					this.SetupRaidFromDictionary(raidData2);
				}
			}
		}

		public void SetupRaidFromDictionary(Dictionary<string, object> raidData)
		{
			if (raidData.ContainsKey("startTime"))
			{
				this.RaidStartTime = Convert.ToUInt32(raidData["startTime"]);
			}
			else
			{
				Service.Logger.Error("Raid Data Missing Start Time");
			}
			if (raidData.ContainsKey("nextRaidStartTime"))
			{
				this.NextRaidStartTime = Convert.ToUInt32(raidData["nextRaidStartTime"]);
			}
			else
			{
				Service.Logger.Error("Raid Data Missing Next Start Time");
			}
			if (raidData.ContainsKey("endTime"))
			{
				object obj = raidData["endTime"];
				if (obj == null)
				{
					this.RaidEndTime = 0u;
				}
				else
				{
					this.RaidEndTime = Convert.ToUInt32(obj);
				}
			}
			else
			{
				Service.Logger.Error("Raid Data Missing End Time");
			}
			if (raidData.ContainsKey("raidPoolId"))
			{
				string currentRaidPoolId = string.Empty;
				object obj2 = raidData["raidPoolId"];
				if (obj2 != null)
				{
					currentRaidPoolId = (obj2 as string);
				}
				this.CurrentRaidPoolId = currentRaidPoolId;
			}
			else
			{
				Service.Logger.Error("Raid Data Missing Pool ID");
			}
			if (raidData.ContainsKey("raidId"))
			{
				string currentRaidId = string.Empty;
				object obj3 = raidData["raidId"];
				if (obj3 != null)
				{
					currentRaidId = (obj3 as string);
				}
				this.CurrentRaidId = currentRaidId;
			}
			else
			{
				Service.Logger.Error("Raid Data Missing Raid ID");
			}
			if (raidData.ContainsKey("raidMissionId"))
			{
				string currentRaid = string.Empty;
				object obj4 = raidData["raidMissionId"];
				if (obj4 != null)
				{
					currentRaid = (obj4 as string);
				}
				this.SetCurrentRaid(currentRaid);
			}
			else
			{
				Service.Logger.Error("Raid Data Missing Mission ID");
			}
		}

		public void UpdateHolonetRewardsFromServer(object holonetRewards)
		{
			if (holonetRewards != null)
			{
				List<object> list = holonetRewards as List<object>;
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					this.HolonetRewards.Add((string)list[i]);
				}
			}
		}

		public void UpdateUnlockedPlanetsFromServer(object serverPlanetsUnlockData)
		{
			if (serverPlanetsUnlockData != null)
			{
				List<object> list = serverPlanetsUnlockData as List<object>;
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.UnlockedPlanets.Add((string)list[i]);
					i++;
				}
				if (!this.UnlockedPlanets.Contains("planet1"))
				{
					this.UnlockedPlanets.Add("planet1");
				}
			}
		}

		public void AddUnlockedPlanet(string planetId)
		{
			if (!string.IsNullOrEmpty(planetId))
			{
				if (!this.UnlockedPlanets.Contains(planetId))
				{
					this.UnlockedPlanets.Add(planetId);
				}
				Service.EventManager.SendEvent(EventId.PlanetUnlocked, planetId);
			}
		}

		public void AddRelocationStars(int stars)
		{
			if (this.RelocationStarsCount == -1)
			{
				return;
			}
			int requiredRelocationStars = this.GetRequiredRelocationStars();
			this.RelocationStarsCount += stars;
			if (this.RelocationStarsCount > requiredRelocationStars)
			{
				this.RelocationStarsCount = requiredRelocationStars;
			}
		}

		public void SetRelocationStartsCount(int value)
		{
			this.RelocationStarsCount = value;
		}

		public int GetRawRelocationStarsCount()
		{
			return this.RelocationStarsCount;
		}

		public int GetDisplayRelocationStarsCount()
		{
			if (this.RelocationStarsCount == -1)
			{
				return this.GetRequiredRelocationStars();
			}
			return Mathf.Clamp(this.RelocationStarsCount, 0, this.GetRequiredRelocationStars());
		}

		public int GetRequiredRelocationStars()
		{
			int num = Service.CurrentPlayer.Map.FindHighestHqLevel();
			int num2 = num - 1;
			if (num2 < 0 || num2 >= GameConstants.StarsPerRelocation.Length)
			{
				Service.Logger.Warn("StarsPerRelocation index out of bounds! index: " + num2);
				return 50;
			}
			int num3 = GameConstants.StarsPerRelocation[num2];
			int relocationCostDiscount = Service.PerkManager.GetRelocationCostDiscount();
			int val = num3 - relocationCostDiscount;
			return Math.Max(0, val);
		}

		public void ResetRelocationStars()
		{
			this.RelocationStarsCount = 0;
		}

		public bool IsRelocationRequirementMet()
		{
			return this.RelocationStarsCount >= this.GetRequiredRelocationStars() || this.RelocationStarsCount == -1;
		}

		public void SetFreeRelocation()
		{
			this.RelocationStarsCount = -1;
		}

		public string GetFirstPlanetUnlockedUID()
		{
			if (this.UnlockedPlanets != null && this.UnlockedPlanets.Count > 0)
			{
				int count = this.UnlockedPlanets.Count;
				for (int i = 0; i < count; i++)
				{
					if (!this.UnlockedPlanets[i].Equals("planet1"))
					{
						return this.UnlockedPlanets[i];
					}
				}
			}
			return string.Empty;
		}

		public bool IsPlanetUnlocked(string planetId)
		{
			return this.UnlockedPlanets != null && this.UnlockedPlanets.Count > 0 && this.UnlockedPlanets.Contains(planetId);
		}

		public bool IsCurrentPlanet(string planetUid)
		{
			return planetUid == this.Planet.Uid;
		}

		public bool IsCurrentPlanet(PlanetVO planetVO)
		{
			return planetVO == this.Planet;
		}

		public string GetPlanetStatus(string planetUid)
		{
			if (this.IsCurrentPlanet(planetUid))
			{
				return "current";
			}
			if (this.IsPlanetUnlocked(planetUid))
			{
				return "unlocked";
			}
			return "locked";
		}

		public bool IsRelocationFree()
		{
			return this.RelocationStarsCount == -1;
		}

		public int GetCrystalRelocationCost()
		{
			int num = this.GetRequiredRelocationStars() - this.RelocationStarsCount;
			int num2 = Service.CurrentPlayer.Map.FindHighestHqLevel();
			int num3 = num2 - 1;
			if (num3 < 0 || num3 >= GameConstants.CrystalsPerRelocationStar.Length)
			{
				Service.Logger.Warn("CrystalsPerRelocationStar index out of bounds! index: " + num3);
				return 15;
			}
			int num4 = GameConstants.CrystalsPerRelocationStar[num3];
			return num * num4;
		}

		public void ModifyShardAmount(string shardId, int newAmount)
		{
			if (this.Shards.ContainsKey(shardId))
			{
				this.Shards[shardId] = newAmount;
			}
			else
			{
				this.Shards.Add(shardId, newAmount);
			}
		}

		public void DoPostContentInitialization()
		{
			this.SetCurrentRaid(this.nextRaidId);
		}
	}
}
