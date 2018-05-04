using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers.Missions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Campaign;
using StaRTS.Main.Models.Commands.Missions;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Commands.Pve;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class CampaignController
	{
		private CurrentPlayer player;

		private CampaignProgress progress;

		private ServerAPI server;

		private StaticDataController sdc;

		private RewardManager rm;

		private HUD hud;

		private bool newChapterMissionFlag;

		private List<MissionConductor> activeMissionConductors;

		public bool HasNewChapterMission
		{
			get
			{
				return this.newChapterMissionFlag;
			}
			set
			{
				bool flag = this.newChapterMissionFlag != value;
				this.newChapterMissionFlag = value;
				if (flag)
				{
					if (!this.newChapterMissionFlag)
					{
						this.UpdateMissionsViewedPref(1);
					}
					this.hud.RefreshView();
				}
			}
		}

		public CampaignController()
		{
			Service.CampaignController = this;
			this.player = Service.CurrentPlayer;
			this.progress = this.player.CampaignProgress;
			this.server = Service.ServerAPI;
			this.sdc = Service.StaticDataController;
			this.rm = Service.RewardManager;
			this.hud = Service.UXController.HUD;
			this.activeMissionConductors = new List<MissionConductor>();
			this.progress.RemoveMissingMissionData();
			List<CampaignMissionVO> allMissionsInProgress = this.progress.GetAllMissionsInProgress();
			int i = 0;
			int count = allMissionsInProgress.Count;
			while (i < count)
			{
				this.ResumeMission(allMissionsInProgress[i]);
				i++;
			}
			this.progress.CheckForNewMissions(ref this.newChapterMissionFlag);
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			if (Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.ChapterMissionViewed)) > 0)
			{
				this.newChapterMissionFlag = false;
			}
		}

		public int PlayButtonCount()
		{
			return Convert.ToInt32(this.newChapterMissionFlag);
		}

		private void UpdateMissionsViewedPref(int amount)
		{
			ServerPref pref = ServerPref.ChapterMissionViewed;
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			int num = Convert.ToInt32(serverPlayerPrefs.GetPref(pref));
			num = amount;
			serverPlayerPrefs.SetPref(pref, num.ToString());
			Service.ServerAPI.Enqueue(new SetPrefsCommand(false));
		}

		public void StartMission(CampaignMissionVO missionVO)
		{
			this.CancelDuplicateMissionConductors(missionVO);
			MissionConductor missionConductor = new MissionConductor(missionVO);
			this.activeMissionConductors.Add(missionConductor);
			missionConductor.Start();
			this.progress.StartMission(missionVO);
		}

		public void ResumeMission(CampaignMissionVO missionVO)
		{
			this.CancelDuplicateMissionConductors(missionVO);
			Service.Logger.DebugFormat("Resuming mission {0}", new object[]
			{
				missionVO.Uid
			});
			MissionConductor missionConductor = new MissionConductor(missionVO);
			this.activeMissionConductors.Add(missionConductor);
			missionConductor.Resume();
		}

		private void CancelDuplicateMissionConductors(CampaignMissionVO missionVO)
		{
			List<MissionConductor> list = null;
			int i = 0;
			int count = this.activeMissionConductors.Count;
			while (i < count)
			{
				MissionConductor missionConductor = this.activeMissionConductors[i];
				if (missionConductor.MissionVO.Uid == missionVO.Uid || (missionConductor.MissionVO.MissionType == MissionType.Pvp && missionVO.MissionType == MissionType.Pvp))
				{
					if (list == null)
					{
						list = new List<MissionConductor>();
					}
					list.Add(missionConductor);
				}
				i++;
			}
			if (list != null)
			{
				int j = 0;
				int count2 = list.Count;
				while (j < count2)
				{
					list[j].CancelMission();
					j++;
				}
			}
		}

		private void UnlockNextMission(CampaignMissionVO missionType)
		{
			CampaignVO campaignVO = this.sdc.Get<CampaignVO>(missionType.CampaignUid);
			CampaignVO campaignVO2 = null;
			CampaignMissionVO campaignMissionVO = null;
			CampaignMissionVO campaignMissionVO2 = null;
			foreach (CampaignVO current in this.sdc.GetAll<CampaignVO>())
			{
				if (current.UnlockOrder == campaignVO.UnlockOrder + 1 && current.Faction == campaignVO.Faction)
				{
					campaignVO2 = current;
					break;
				}
			}
			foreach (CampaignMissionVO current2 in this.sdc.GetAll<CampaignMissionVO>())
			{
				if (campaignVO2 != null && current2.CampaignUid == campaignVO2.Uid && current2.UnlockOrder == 1)
				{
					campaignMissionVO2 = current2;
				}
				if (current2.CampaignUid == missionType.CampaignUid && current2.UnlockOrder == missionType.UnlockOrder + 1)
				{
					campaignMissionVO = current2;
					break;
				}
			}
			if (campaignMissionVO != null)
			{
				Service.Logger.Debug("Unlocking next mission in current campaign!");
				this.UnlockMission(campaignMissionVO);
			}
			else if (campaignMissionVO2 != null)
			{
				Service.Logger.Debug("Unlocking first mission in next campaign!");
				this.UnlockMission(campaignMissionVO2);
			}
			else
			{
				Service.Logger.Debug("All missions for all campaigns are unlocked!");
			}
		}

		private void UnlockMission(CampaignMissionVO missionType)
		{
			if (!this.progress.IsMissionLocked(missionType))
			{
				Service.Logger.Error("Unexpected attempt to add a mission that is already unlocked!");
				return;
			}
			Mission mission = Mission.CreateFromCampaignMissionVO(missionType);
			this.newChapterMissionFlag = true;
			this.UpdateMissionsViewedPref(0);
			this.progress.AddMission(missionType.Uid, mission);
			if (missionType.UnlockOrder == 1 && !this.progress.Campaigns.ContainsKey(missionType.CampaignUid))
			{
				Campaign campaign = new Campaign();
				campaign.Uid = missionType.CampaignUid;
				campaign.Completed = false;
				campaign.Points = 0u;
				campaign.TimeZone = (float)Service.EnvironmentController.GetTimezoneOffset();
				this.progress.AddCampaign(missionType.CampaignUid, campaign);
			}
			Service.EventManager.SendEvent(EventId.MissionUnlocked, missionType);
		}

		public void CompleteMission(CampaignMissionVO missionType, int earnedStars)
		{
			if (this.progress.CompleteMission(missionType, earnedStars))
			{
				if (missionType == this.GetLastMission(missionType.CampaignUid) && !this.progress.CompleteCampaign(missionType.CampaignUid))
				{
					Service.Logger.WarnFormat("Unable to complete campaign {0}", new object[]
					{
						missionType.CampaignUid
					});
				}
				this.UnlockNextMission(missionType);
				if (!string.IsNullOrEmpty(missionType.Rewards) && this.rm.IsRewardOnlySoftCurrency(missionType.Rewards))
				{
					this.CollectMission(missionType);
				}
			}
			Service.EventManager.SendEvent(EventId.MissionCompleted, missionType);
			if (missionType.CampaignUid != null)
			{
				CampaignVO campaignVO = this.sdc.Get<CampaignVO>(missionType.CampaignUid);
				if (!this.progress.IsCampaignCollected(campaignVO.Uid))
				{
					int totalCampaignStarsEarned = this.progress.GetTotalCampaignStarsEarned(campaignVO);
					if (totalCampaignStarsEarned >= campaignVO.TotalMasteryStars)
					{
						this.CollectCampaign(campaignVO, missionType);
					}
				}
			}
			this.RemoveActiveMissionConductors(missionType);
		}

		public void OnMissionCancelled(CampaignMissionVO missionVO)
		{
			this.RemoveActiveMissionConductors(missionVO);
		}

		private void RemoveActiveMissionConductors(CampaignMissionVO missionVO)
		{
			for (int i = this.activeMissionConductors.Count - 1; i >= 0; i--)
			{
				MissionConductor missionConductor = this.activeMissionConductors[i];
				if (missionConductor.MissionVO.Uid == missionVO.Uid)
				{
					this.activeMissionConductors.Remove(missionConductor);
				}
			}
		}

		public void UpdateCounter(CampaignMissionVO missionType, string key, int delta)
		{
			this.progress.UpdateMissionCounter(missionType, key, delta);
		}

		public Dictionary<string, int> GetCounters(CampaignMissionVO missionType)
		{
			return this.progress.GetMissionCounters(missionType);
		}

		public void CollectMission(CampaignMissionVO missionType)
		{
			if (!this.progress.IsMissionCollected(missionType))
			{
				Service.EventManager.SendEvent(EventId.MissionCollecting, missionType);
				this.rm.TryAndGrantReward(missionType.Rewards, new RewardManager.SuccessCallback(this.OnMissionCollectSuccess), missionType);
				MissionIdRequest request = new MissionIdRequest(missionType.Uid);
				if (this.IsPveMission(missionType.MissionType))
				{
					this.server.Enqueue(new PveMissionCollectCommand(request));
				}
				else
				{
					this.server.Enqueue(new ClaimMissionCommand(request));
				}
			}
		}

		private void OnMissionCollectSuccess(object cookie)
		{
			CampaignMissionVO campaignMissionVO = (CampaignMissionVO)cookie;
			if (this.progress.CollectMission(campaignMissionVO.Uid))
			{
				Service.EventManager.SendEvent(EventId.MissionCollected, campaignMissionVO);
			}
		}

		public void CollectCampaign(CampaignVO campaignType, CampaignMissionVO lastMissionCompleted)
		{
			this.rm.TryAndGrantReward(campaignType.Reward, new RewardManager.SuccessCallback(this.OnCampaignCollectSuccess), campaignType);
			ClaimCampaignRequest request = new ClaimCampaignRequest(campaignType.Uid, lastMissionCompleted.Uid);
			Service.ServerAPI.Enqueue(new ClaimCampaignCommand(request));
		}

		public void OnCampaignCollectSuccess(object cookie)
		{
			CampaignVO campaignVO = (CampaignVO)cookie;
			this.progress.CollectCampaign(campaignVO.Uid);
		}

		public int GetTotalStarsEarned()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			int num = 0;
			foreach (CampaignVO current in staticDataController.GetAll<CampaignVO>())
			{
				if (!current.Timed && current.Faction == this.player.Faction)
				{
					num += this.progress.GetTotalCampaignStarsEarned(current);
				}
			}
			return num;
		}

		private bool CanCurrencyRewardFit(int amount, string rewardKey)
		{
			if (amount == 0)
			{
				return true;
			}
			CurrencyType currencyType = StringUtils.ParseEnum<CurrencyType>(rewardKey);
			return this.player.Inventory.CanStoreAll(amount, currencyType);
		}

		public CampaignMissionVO GetLastMission(string campaignUid)
		{
			CampaignMissionVO campaignMissionVO = null;
			foreach (CampaignMissionVO current in Service.StaticDataController.GetAll<CampaignMissionVO>())
			{
				if (!(current.CampaignUid != campaignUid))
				{
					if (campaignMissionVO == null || current.UnlockOrder > campaignMissionVO.UnlockOrder)
					{
						campaignMissionVO = current;
					}
				}
			}
			return campaignMissionVO;
		}

		public void UnlockMissionCheat(CampaignMissionVO missionType)
		{
			this.UnlockMission(missionType);
		}

		public void EraseMissionCheat(CampaignMissionVO missionType)
		{
			this.progress.EraseMission(missionType);
		}

		public void StartCampaignProgress()
		{
			string uid = string.Empty;
			FactionType faction = this.player.Faction;
			if (faction != FactionType.Rebel)
			{
				if (faction == FactionType.Empire)
				{
					uid = GameConstants.NEW_PLAYER_INITIAL_MISSION_EMPIRE;
				}
			}
			else
			{
				uid = GameConstants.NEW_PLAYER_INITIAL_MISSION_REBEL;
			}
			CampaignMissionVO missionType = Service.StaticDataController.Get<CampaignMissionVO>(uid);
			this.UnlockMission(missionType);
		}

		private bool IsPveMission(MissionType missionType)
		{
			return missionType == MissionType.Attack || missionType == MissionType.Defend || missionType == MissionType.RaidDefend;
		}
	}
}
