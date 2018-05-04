using StaRTS.Externals.EnvironmentManager;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.VictoryConditions;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Commands.Missions;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class CampaignProgress : ISerializable
	{
		private Dictionary<string, Campaign> campaigns;

		private Dictionary<string, Mission> missions;

		private float[] starsToPortion;

		private EnvironmentController env;

		public bool FueInProgress
		{
			get;
			set;
		}

		public IDictionary<string, Mission> Missions
		{
			get
			{
				return this.missions;
			}
		}

		public IDictionary<string, Campaign> Campaigns
		{
			get
			{
				return this.campaigns;
			}
		}

		public CampaignProgress()
		{
			this.campaigns = new Dictionary<string, Campaign>();
			this.missions = new Dictionary<string, Mission>();
			this.FueInProgress = true;
			this.env = Service.EnvironmentController;
			this.starsToPortion = new float[]
			{
				0f,
				0.3f,
				0.6f,
				1f
			};
		}

		public void CheckForNewMissions(ref bool newChapterMission)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (string current in this.missions.Keys)
			{
				Mission mission = this.missions[current];
				if (mission.Status == MissionStatus.Default)
				{
					CampaignMissionVO optional = staticDataController.GetOptional<CampaignMissionVO>(mission.Uid);
					if (optional != null)
					{
						CampaignVO optional2 = staticDataController.GetOptional<CampaignVO>(optional.CampaignUid);
						if (optional2 != null)
						{
							if (!optional2.Timed)
							{
								newChapterMission = true;
							}
							if (newChapterMission)
							{
								break;
							}
						}
					}
				}
			}
		}

		public bool HasCampaign(CampaignVO campaignType)
		{
			return this.campaigns.ContainsKey(campaignType.Uid);
		}

		public int GetOffsetSeconds(ITimedEventVO vo)
		{
			if (!vo.UseTimeZoneOffset)
			{
				return 0;
			}
			if (this.campaigns.ContainsKey(vo.Uid))
			{
				return (int)(this.campaigns[vo.Uid].TimeZone * 3600f);
			}
			return this.env.GetTimezoneOffsetSeconds();
		}

		public bool IsNewSpecOp(CampaignVO vo)
		{
			if (!this.HasCampaign(vo))
			{
				return true;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (Mission current in this.missions.Values)
			{
				CampaignMissionVO optional = staticDataController.GetOptional<CampaignMissionVO>(current.Uid);
				if (current.CampaignUid == vo.Uid && optional != null && optional.UnlockOrder == 1)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsGrindComplete(CampaignMissionVO vo)
		{
			if (!this.missions.ContainsKey(vo.Uid))
			{
				return false;
			}
			Mission mission = this.missions[vo.Uid];
			return mission.GrindMissionRetries >= GameConstants.GRIND_MISSION_MAXIMUM;
		}

		public int GetRetriesLeft(CampaignMissionVO vo)
		{
			if (!this.missions.ContainsKey(vo.Uid))
			{
				return GameConstants.GRIND_MISSION_MAXIMUM;
			}
			Mission mission = this.missions[vo.Uid];
			return GameConstants.GRIND_MISSION_MAXIMUM - mission.GrindMissionRetries;
		}

		public bool HasSeenIntro(string campaignUid)
		{
			return Service.CurrentPlayer.SpecOpIntros.Contains(campaignUid);
		}

		public bool IsCampaignCollected(string campaignUid)
		{
			return this.campaigns.ContainsKey(campaignUid) && this.campaigns[campaignUid].Collected;
		}

		public bool CanReplay(CampaignMissionVO missionType)
		{
			return missionType.Replayable || missionType.Grind || this.GetMissionEarnedStars(missionType) < missionType.MasteryStars;
		}

		public int GetTotalCampaignStarsEarned(CampaignVO campaignType)
		{
			if (!this.campaigns.ContainsKey(campaignType.Uid))
			{
				return 0;
			}
			int num = 0;
			foreach (Mission current in this.missions.Values)
			{
				if (current.CampaignUid == campaignType.Uid)
				{
					num += current.EarnedStars;
				}
			}
			return num;
		}

		public int GetTotalAttackDefendCampaignStarsEarned(CampaignVO campaignType)
		{
			if (!this.campaigns.ContainsKey(campaignType.Uid))
			{
				return 0;
			}
			int num = 0;
			foreach (Mission current in this.missions.Values)
			{
				if (current.CampaignUid == campaignType.Uid)
				{
					CampaignMissionVO campaignMissionVO = Service.StaticDataController.Get<CampaignMissionVO>(current.Uid);
					if (campaignMissionVO.MissionType == MissionType.Attack || campaignMissionVO.MissionType == MissionType.Defend)
					{
						num += current.EarnedStars;
					}
				}
			}
			return num;
		}

		public int GetTotalCampaignStarsEarnedInAllCampaigns()
		{
			int num = 0;
			Dictionary<string, CampaignVO>.ValueCollection all = Service.StaticDataController.GetAll<CampaignVO>();
			foreach (CampaignVO current in all)
			{
				if (current.Faction != FactionType.Smuggler)
				{
					num += this.GetTotalAttackDefendCampaignStarsEarned(current);
				}
			}
			return num;
		}

		public List<CampaignMissionVO> GetAllMissionsInProgress()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<CampaignMissionVO> list = new List<CampaignMissionVO>();
			foreach (Mission current in this.missions.Values)
			{
				CampaignMissionVO optional = staticDataController.GetOptional<CampaignMissionVO>(current.Uid);
				if (optional != null)
				{
					if (this.IsMissionInProgress(optional) && !optional.IsCombatMission())
					{
						list.Add(optional);
					}
				}
			}
			return list;
		}

		public int GetTotalCampaignMissionsCompleted(CampaignVO campaignType)
		{
			if (!this.campaigns.ContainsKey(campaignType.Uid))
			{
				return 0;
			}
			int num = 0;
			foreach (Mission current in this.missions.Values)
			{
				if (current.CampaignUid == campaignType.Uid && current.Completed)
				{
					num++;
				}
			}
			return num;
		}

		public void AddMission(string uid, Mission mission)
		{
			this.missions.Add(uid, mission);
		}

		public void StartMission(CampaignMissionVO vo)
		{
			if (!string.IsNullOrEmpty(vo.Uid) && this.missions.ContainsKey(vo.Uid))
			{
				Mission mission = this.missions[vo.Uid];
				if (vo.Grind)
				{
					mission.GrindMissionRetries++;
				}
				if (!mission.Activated)
				{
					MissionIdRequest request = new MissionIdRequest(vo.Uid);
					Service.ServerAPI.Sync(new ActivateMissionCommand(request));
					mission.Activated = true;
				}
			}
		}

		public bool CompleteMission(CampaignMissionVO vo, int starsEarned)
		{
			if (vo.Uid != null && this.missions.ContainsKey(vo.Uid))
			{
				Mission mission = this.missions[vo.Uid];
				int earnedStars = mission.EarnedStars;
				if (starsEarned > earnedStars)
				{
					float[] array = this.starsToPortion;
					if (vo.StarsToPortion != null)
					{
						array = vo.StarsToPortion;
					}
					if (!vo.Grind)
					{
						mission.EarnedStars = starsEarned;
					}
					int campaignPoints = vo.CampaignPoints;
					if (campaignPoints > 0)
					{
						int num = (int)(array[earnedStars] * (float)campaignPoints);
						int num2 = (int)(array[starsEarned] * (float)campaignPoints);
						uint num3 = (uint)(num2 - num);
						if (this.campaigns.ContainsKey(vo.CampaignUid))
						{
							this.campaigns[vo.CampaignUid].Points += num3;
						}
						CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
						currentBattle.CampaignPointsEarn = num3;
					}
					Service.AchievementController.TryUnlockAchievementByValue(AchievementType.PveStars, this.GetTotalCampaignStarsEarnedInAllCampaigns());
				}
				if (!mission.Completed)
				{
					mission.Completed = true;
					return true;
				}
			}
			return false;
		}

		public int RemainingCampaignPointsForMission(CampaignMissionVO vo)
		{
			if (this.missions.ContainsKey(vo.Uid))
			{
				float[] array = this.starsToPortion;
				if (vo.StarsToPortion != null)
				{
					array = vo.StarsToPortion;
				}
				int earnedStars = this.missions[vo.Uid].EarnedStars;
				int campaignPoints = vo.CampaignPoints;
				if (campaignPoints > 0)
				{
					int num = campaignPoints - (int)(array[earnedStars] * (float)campaignPoints);
					if (num > 0)
					{
						return num;
					}
					return 0;
				}
			}
			return vo.CampaignPoints;
		}

		public bool CollectMission(string uid)
		{
			if (this.missions.ContainsKey(uid))
			{
				Mission mission = this.missions[uid];
				if (!mission.Collected)
				{
					mission.Collected = true;
					return true;
				}
			}
			return false;
		}

		public bool UpdateMissionLoot(string uid, CurrentBattle battle)
		{
			if (this.missions.ContainsKey(uid))
			{
				this.missions[uid].SetLootRemaining(battle.LootCreditsAvailable - battle.LootCreditsEarned, battle.LootMaterialsAvailable - battle.LootMaterialsEarned, battle.LootContrabandAvailable - battle.LootContrabandEarned);
				return true;
			}
			return false;
		}

		public void AddCampaign(string uid, Campaign campaign)
		{
			if (!this.campaigns.ContainsKey(uid))
			{
				this.campaigns.Add(uid, campaign);
			}
		}

		public bool CompleteCampaign(string uid)
		{
			if (!string.IsNullOrEmpty(uid) && this.campaigns.ContainsKey(uid) && !this.campaigns[uid].Completed)
			{
				this.campaigns[uid].Completed = true;
				return true;
			}
			return false;
		}

		public bool CollectCampaign(string uid)
		{
			if (this.campaigns.ContainsKey(uid) && !this.campaigns[uid].Collected)
			{
				this.campaigns[uid].Collected = true;
				return true;
			}
			return false;
		}

		public int GetMissionLootCreditsRemaining(CampaignMissionVO missionType)
		{
			return this.GetMissionLootCurrencyRemaining(CurrencyType.Credits, missionType);
		}

		public int GetMissionLootMaterialsRemaining(CampaignMissionVO missionType)
		{
			return this.GetMissionLootCurrencyRemaining(CurrencyType.Materials, missionType);
		}

		public int GetMissionLootContrabandRemaining(CampaignMissionVO missionType)
		{
			return this.GetMissionLootCurrencyRemaining(CurrencyType.Contraband, missionType);
		}

		private int GetMissionLootCurrencyRemaining(CurrencyType type, CampaignMissionVO missionType)
		{
			if (this.missions.ContainsKey(missionType.Uid))
			{
				Mission mission = this.missions[missionType.Uid];
				if (mission.LootRemaining != null && mission.LootRemaining[(int)type] >= 0)
				{
					return mission.LootRemaining[(int)type];
				}
				if (missionType.TotalLoot != null && missionType.TotalLoot[(int)type] >= 0)
				{
					return missionType.TotalLoot[(int)type];
				}
			}
			return 0;
		}

		public int GetMissionEarnedStars(CampaignMissionVO missionType)
		{
			return (!this.missions.ContainsKey(missionType.Uid)) ? 0 : this.missions[missionType.Uid].EarnedStars;
		}

		public Dictionary<string, int> GetMissionCounters(CampaignMissionVO missionType)
		{
			return (!this.missions.ContainsKey(missionType.Uid)) ? null : this.missions[missionType.Uid].Counters;
		}

		public bool UpdateMissionCounter(CampaignMissionVO missionType, string key, int delta)
		{
			if (!this.missions.ContainsKey(missionType.Uid))
			{
				return false;
			}
			this.missions[missionType.Uid].AddToCounter(key, delta);
			return true;
		}

		public void GetMissionProgress(CampaignMissionVO mission, out int current, out int total)
		{
			if (!this.missions.ContainsKey(mission.Uid))
			{
				current = 0;
				total = 1;
			}
			current = 0;
			total = 0;
			for (int i = 0; i < mission.Conditions.Count; i++)
			{
				Dictionary<string, int> counters = this.missions[mission.Uid].Counters;
				ConditionVO conditionVO = mission.Conditions[i];
				int startingValue = (counters == null || !counters.ContainsKey(conditionVO.Uid)) ? 0 : counters[conditionVO.Uid];
				AbstractCondition abstractCondition = ConditionFactory.GenerateCondition(conditionVO, null, startingValue);
				int num;
				int num2;
				abstractCondition.GetProgress(out num, out num2);
				current += num;
				total += num2;
				abstractCondition.Destroy();
			}
		}

		public bool IsMissionLocked(CampaignMissionVO missionType)
		{
			return !this.missions.ContainsKey(missionType.Uid) || this.missions[missionType.Uid].Locked;
		}

		public bool IsMissionCompleted(CampaignMissionVO missionType)
		{
			return this.missions.ContainsKey(missionType.Uid) && this.missions[missionType.Uid].Completed;
		}

		public bool IsMissionCollected(CampaignMissionVO missionType)
		{
			return this.missions.ContainsKey(missionType.Uid) && this.missions[missionType.Uid].Collected;
		}

		public bool IsMissionInProgress(CampaignMissionVO missionType)
		{
			if (missionType.IsCombatMission())
			{
				return false;
			}
			if (this.missions.ContainsKey(missionType.Uid))
			{
				Mission mission = this.missions[missionType.Uid];
				if (!mission.Completed && !mission.Collected)
				{
					return mission.Activated;
				}
			}
			return false;
		}

		public Campaign GetTimedEvent(string eventUid)
		{
			if (this.campaigns.ContainsKey(eventUid))
			{
				return this.campaigns[eventUid];
			}
			return null;
		}

		public string ToJson()
		{
			return "{}";
		}

		public void RemoveMissingMissionData()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<string> list = new List<string>();
			foreach (string current in this.campaigns.Keys)
			{
				if (staticDataController.GetOptional<CampaignVO>(current) == null)
				{
					list.Add(current);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.campaigns.Remove(list[i]);
			}
			list.Clear();
			foreach (string current2 in this.missions.Keys)
			{
				if (staticDataController.GetOptional<CampaignMissionVO>(current2) == null)
				{
					list.Add(current2);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				this.missions.Remove(list[j]);
			}
			list.Clear();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("campaigns"))
			{
				Dictionary<string, object> dictionary2 = dictionary["campaigns"] as Dictionary<string, object>;
				foreach (KeyValuePair<string, object> current in dictionary2)
				{
					Campaign campaign = new Campaign();
					campaign.FromObject(current.Value);
					this.campaigns.Add(current.Key, campaign);
				}
			}
			if (dictionary.ContainsKey("missions"))
			{
				Dictionary<string, object> dictionary3 = dictionary["missions"] as Dictionary<string, object>;
				foreach (KeyValuePair<string, object> current2 in dictionary3)
				{
					Mission mission = new Mission();
					mission.FromObject(current2.Value);
					this.missions.Add(current2.Key, mission);
				}
			}
			if (dictionary.ContainsKey("isFueInProgress"))
			{
				this.FueInProgress = (bool)dictionary["isFueInProgress"];
			}
			return this;
		}

		public void EraseMission(CampaignMissionVO missionType)
		{
			if (this.missions.ContainsKey(missionType.Uid))
			{
				this.missions.Remove(missionType.Uid);
			}
		}
	}
}
