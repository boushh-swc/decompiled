using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CampaignMissionVO : IValueObject
	{
		public static int COLUMN_missionType
		{
			get;
			private set;
		}

		public static int COLUMN_waves
		{
			get;
			private set;
		}

		public static int COLUMN_map
		{
			get;
			private set;
		}

		public static int COLUMN_campaignUid
		{
			get;
			private set;
		}

		public static int COLUMN_title
		{
			get;
			private set;
		}

		public static int COLUMN_unlockOrder
		{
			get;
			private set;
		}

		public static int COLUMN_description
		{
			get;
			private set;
		}

		public static int COLUMN_rewards
		{
			get;
			private set;
		}

		public static int COLUMN_introStory
		{
			get;
			private set;
		}

		public static int COLUMN_winStory
		{
			get;
			private set;
		}

		public static int COLUMN_loseStory
		{
			get;
			private set;
		}

		public static int COLUMN_goalFailStory
		{
			get;
			private set;
		}

		public static int COLUMN_opponentName
		{
			get;
			private set;
		}

		public static int COLUMN_goalString
		{
			get;
			private set;
		}

		public static int COLUMN_goalFailString
		{
			get;
			private set;
		}

		public static int COLUMN_progressString
		{
			get;
			private set;
		}

		public static int COLUMN_replay
		{
			get;
			private set;
		}

		public static int COLUMN_grind
		{
			get;
			private set;
		}

		public static int COLUMN_battleMusic
		{
			get;
			private set;
		}

		public static int COLUMN_ambientMusic
		{
			get;
			private set;
		}

		public static int COLUMN_campaignPoints
		{
			get;
			private set;
		}

		public static int COLUMN_fixedWaves
		{
			get;
			private set;
		}

		public static int COLUMN_totalLoot
		{
			get;
			private set;
		}

		public static int COLUMN_victoryConditions
		{
			get;
			private set;
		}

		public static int COLUMN_failureCondition
		{
			get;
			private set;
		}

		public static int COLUMN_bi_chap_id
		{
			get;
			private set;
		}

		public static int COLUMN_bi_context
		{
			get;
			private set;
		}

		public static int COLUMN_bi_enemy_tier
		{
			get;
			private set;
		}

		public static int COLUMN_bi_mission_id
		{
			get;
			private set;
		}

		public static int COLUMN_bi_mission_name
		{
			get;
			private set;
		}

		public static int COLUMN_raidDesc
		{
			get;
			private set;
		}

		public static int COLUMN_raidImage
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string CampaignUid
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public int UnlockOrder
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Rewards
		{
			get;
			set;
		}

		public int MasteryStars
		{
			get;
			set;
		}

		public int[] TotalLoot
		{
			get;
			set;
		}

		public MissionType MissionType
		{
			get;
			set;
		}

		public string Waves
		{
			get;
			set;
		}

		public string Map
		{
			get;
			set;
		}

		public List<ConditionVO> Conditions
		{
			get;
			set;
		}

		public string FailureCondition
		{
			get;
			set;
		}

		public string IntroStory
		{
			get;
			set;
		}

		public string SuccessStory
		{
			get;
			set;
		}

		public string FailureStory
		{
			get;
			set;
		}

		public string GoalFailureStory
		{
			get;
			set;
		}

		public string OpponentName
		{
			get;
			set;
		}

		public string GoalString
		{
			get;
			set;
		}

		public string GoalFailureString
		{
			get;
			set;
		}

		public string ProgressString
		{
			get;
			set;
		}

		public bool Replayable
		{
			get;
			set;
		}

		public bool Grind
		{
			get;
			set;
		}

		public string BattleMusic
		{
			get;
			set;
		}

		public string AmbientMusic
		{
			get;
			set;
		}

		public int CampaignPoints
		{
			get;
			set;
		}

		public float[] StarsToPortion
		{
			get;
			set;
		}

		public bool FixedWaves
		{
			get;
			set;
		}

		public string RaidDescriptionID
		{
			get;
			private set;
		}

		public string RaidBriefingBGTextureName
		{
			get;
			private set;
		}

		public string BIContext
		{
			get;
			set;
		}

		public string BIChapterId
		{
			get;
			set;
		}

		public string BIMissionId
		{
			get;
			set;
		}

		public string BIMissionName
		{
			get;
			set;
		}

		public int BIEnemyTier
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.MissionType = StringUtils.ParseEnum<MissionType>(row.TryGetString(CampaignMissionVO.COLUMN_missionType));
			this.Waves = row.TryGetString(CampaignMissionVO.COLUMN_waves);
			this.Map = row.TryGetString(CampaignMissionVO.COLUMN_map);
			this.CampaignUid = row.TryGetString(CampaignMissionVO.COLUMN_campaignUid);
			this.Title = row.TryGetString(CampaignMissionVO.COLUMN_title);
			this.UnlockOrder = row.TryGetInt(CampaignMissionVO.COLUMN_unlockOrder);
			this.Description = row.TryGetString(CampaignMissionVO.COLUMN_description);
			this.Rewards = row.TryGetString(CampaignMissionVO.COLUMN_rewards);
			this.MasteryStars = 3;
			this.IntroStory = row.TryGetString(CampaignMissionVO.COLUMN_introStory);
			this.SuccessStory = row.TryGetString(CampaignMissionVO.COLUMN_winStory);
			this.FailureStory = row.TryGetString(CampaignMissionVO.COLUMN_loseStory);
			this.GoalFailureStory = row.TryGetString(CampaignMissionVO.COLUMN_goalFailStory);
			this.OpponentName = row.TryGetString(CampaignMissionVO.COLUMN_opponentName);
			this.GoalString = row.TryGetString(CampaignMissionVO.COLUMN_goalString);
			this.GoalFailureString = row.TryGetString(CampaignMissionVO.COLUMN_goalFailString);
			this.ProgressString = row.TryGetString(CampaignMissionVO.COLUMN_progressString);
			this.Replayable = row.TryGetBool(CampaignMissionVO.COLUMN_replay);
			this.Grind = row.TryGetBool(CampaignMissionVO.COLUMN_grind);
			this.BattleMusic = row.TryGetString(CampaignMissionVO.COLUMN_battleMusic);
			this.AmbientMusic = row.TryGetString(CampaignMissionVO.COLUMN_ambientMusic);
			this.CampaignPoints = this.ParseCampaignPoints(row.TryGetString(CampaignMissionVO.COLUMN_campaignPoints));
			this.FixedWaves = row.TryGetBool(CampaignMissionVO.COLUMN_fixedWaves);
			this.RaidDescriptionID = row.TryGetString(CampaignMissionVO.COLUMN_raidDesc);
			this.RaidBriefingBGTextureName = row.TryGetString(CampaignMissionVO.COLUMN_raidImage);
			this.TotalLoot = null;
			ValueObjectController valueObjectController = Service.ValueObjectController;
			List<StrIntPair> strIntPairs = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(CampaignMissionVO.COLUMN_totalLoot));
			if (strIntPairs != null)
			{
				int num = 6;
				this.TotalLoot = new int[num];
				for (int i = 0; i < num; i++)
				{
					this.TotalLoot[i] = -1;
				}
				int j = 0;
				int count = strIntPairs.Count;
				while (j < count)
				{
					StrIntPair strIntPair = strIntPairs[j];
					this.TotalLoot[(int)StringUtils.ParseEnum<CurrencyType>(strIntPair.StrKey)] = strIntPair.IntVal;
					j++;
				}
			}
			StaticDataController staticDataController = Service.StaticDataController;
			this.Conditions = new List<ConditionVO>();
			string[] array = row.TryGetStringArray(CampaignMissionVO.COLUMN_victoryConditions);
			for (int k = 0; k < array.Length; k++)
			{
				this.Conditions.Add(staticDataController.Get<ConditionVO>(array[k]));
			}
			if (!string.IsNullOrEmpty(this.CampaignUid))
			{
				CampaignVO optional = staticDataController.GetOptional<CampaignVO>(this.CampaignUid);
				if (optional != null)
				{
					optional.TotalMissions++;
					optional.TotalMasteryStars += this.MasteryStars;
				}
				else
				{
					Service.Logger.ErrorFormat("CampaignMissionVO {0} that references a CampaignVO Uid {1} that doesn't exist", new object[]
					{
						this.Uid,
						this.CampaignUid
					});
				}
			}
			this.FailureCondition = row.TryGetString(CampaignMissionVO.COLUMN_failureCondition);
			this.BIChapterId = row.TryGetString(CampaignMissionVO.COLUMN_bi_chap_id);
			this.BIContext = row.TryGetString(CampaignMissionVO.COLUMN_bi_context);
			this.BIEnemyTier = row.TryGetInt(CampaignMissionVO.COLUMN_bi_enemy_tier);
			this.BIMissionId = row.TryGetString(CampaignMissionVO.COLUMN_bi_mission_id);
			this.BIMissionName = row.TryGetString(CampaignMissionVO.COLUMN_bi_mission_name);
		}

		public bool IsRaidDefense()
		{
			return this.MissionType == MissionType.RaidDefend;
		}

		public bool IsCombatMission()
		{
			return this.MissionType == MissionType.Attack || this.MissionType == MissionType.Defend || this.MissionType == MissionType.RaidDefend;
		}

		public bool HasPvpCondition()
		{
			int i = 0;
			int count = this.Conditions.Count;
			while (i < count)
			{
				if (this.Conditions[i].IsPvpType())
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public bool IsChallengeMission()
		{
			if (this.Map != null && !this.Grind && this.MissionType != MissionType.RaidDefend)
			{
				BattleTypeVO battleTypeVO = Service.StaticDataController.Get<BattleTypeVO>(this.Map);
				return battleTypeVO.OverridePlayerUnits;
			}
			return false;
		}

		private int ParseCampaignPoints(string raw)
		{
			if (string.IsNullOrEmpty(raw))
			{
				return 0;
			}
			if (!raw.Contains("|"))
			{
				return Convert.ToInt32(raw);
			}
			string[] array = raw.Split(new char[]
			{
				'|'
			});
			string[] array2 = array[1].Split(new char[]
			{
				','
			});
			this.StarsToPortion = new float[4];
			this.StarsToPortion[0] = 0f;
			float num = 0f;
			int i = 0;
			int num2 = array2.Length;
			while (i < num2)
			{
				num += Convert.ToSingle(array2[i]) / 100f;
				this.StarsToPortion[i + 1] = num;
				i++;
			}
			if (num != 1f)
			{
				Service.Logger.WarnFormat("The campaign point distribution for mission {0} does not add up to 100: {1}", new object[]
				{
					this.Uid,
					raw
				});
			}
			return Convert.ToInt32(array[0]);
		}
	}
}
