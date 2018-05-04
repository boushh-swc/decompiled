using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class BattleEntry : ISerializable
	{
		private Dictionary<string, object> troopsExpended;

		private Dictionary<string, object> attackerGuildTroopsDeployed;

		public string RecordID
		{
			get;
			set;
		}

		public BattleParticipant Attacker
		{
			get;
			set;
		}

		public BattleParticipant Defender
		{
			get;
			set;
		}

		public int LootCreditsAvailable
		{
			get;
			set;
		}

		public int LootMaterialsAvailable
		{
			get;
			set;
		}

		public int LootContrabandAvailable
		{
			get;
			set;
		}

		public int LootCreditsEarned
		{
			get;
			set;
		}

		public int LootMaterialsEarned
		{
			get;
			set;
		}

		public int LootContrabandEarned
		{
			get;
			set;
		}

		public int LootCreditsDeducted
		{
			get;
			set;
		}

		public int LootMaterialsDeducted
		{
			get;
			set;
		}

		public int LootContrabandDeducted
		{
			get;
			set;
		}

		public int PotentialMedalsToGain
		{
			get;
			set;
		}

		public uint CampaignPointsEarn
		{
			get;
			set;
		}

		public int EarnedStars
		{
			get;
			set;
		}

		public int DamagePercent
		{
			get;
			set;
		}

		public string MissionId
		{
			get;
			set;
		}

		public string BattleVersion
		{
			get;
			set;
		}

		public string PlanetId
		{
			get;
			set;
		}

		public string ManifestVersion
		{
			get;
			set;
		}

		public string CmsVersion
		{
			get;
			set;
		}

		public List<ConditionVO> VictoryConditions
		{
			get;
			set;
		}

		public ConditionVO FailureCondition
		{
			get;
			set;
		}

		public BattleDeploymentData AttackerDeployedData
		{
			get;
			set;
		}

		public BattleDeploymentData DefenderDeployedData
		{
			get;
			set;
		}

		public string DefenseEncounterProfile
		{
			get;
			set;
		}

		public string BattleScript
		{
			get;
			set;
		}

		public bool AllowReplay
		{
			get;
			set;
		}

		public int WarVictoryPointsAvailable
		{
			get;
			set;
		}

		public List<string> AttackerWarBuffs
		{
			get;
			set;
		}

		public List<string> DefenderWarBuffs
		{
			get;
			set;
		}

		public List<string> AttackerEquipment
		{
			get;
			set;
		}

		public List<string> DefenderEquipment
		{
			get;
			set;
		}

		public string FailedConditionUid
		{
			get;
			set;
		}

		public bool Revenged
		{
			get;
			set;
		}

		public uint EndBattleServerTime
		{
			get;
			set;
		}

		public bool Won
		{
			get;
			set;
		}

		public string SharerPlayerId
		{
			get;
			set;
		}

		public string AttackerID
		{
			get
			{
				return (this.Attacker != null) ? this.Attacker.PlayerId : string.Empty;
			}
		}

		public string DefenderID
		{
			get
			{
				return (this.Defender != null) ? this.Defender.PlayerId : string.Empty;
			}
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.BattleVersion = ((!dictionary.ContainsKey("battleVersion")) ? "30.0".ToString() : Convert.ToString(dictionary["battleVersion"]));
			this.CmsVersion = ((!dictionary.ContainsKey("cmsVersion")) ? Service.ContentManager.GetFileVersion("patches/base.json").ToString() : Convert.ToString(dictionary["cmsVersion"]));
			this.MissionId = ((!dictionary.ContainsKey("missionId")) ? null : Convert.ToString(dictionary["missionId"]));
			this.EarnedStars = Convert.ToInt32(dictionary["stars"]);
			this.DamagePercent = Convert.ToInt32(dictionary["baseDamagePercent"]);
			this.ManifestVersion = Convert.ToString(dictionary["manifestVersion"]);
			this.RecordID = Convert.ToString(dictionary["battleId"]);
			this.Attacker = BattleParticipant.CreateFromObject(dictionary["attacker"]);
			this.Defender = BattleParticipant.CreateFromObject(dictionary["defender"]);
			this.EndBattleServerTime = Convert.ToUInt32(dictionary["attackDate"]);
			if (dictionary.ContainsKey("planetId"))
			{
				this.PlanetId = Convert.ToString(dictionary["planetId"]);
			}
			if (dictionary.ContainsKey("potentialMedalGain"))
			{
				this.PotentialMedalsToGain = Convert.ToInt32(dictionary["potentialMedalGain"]);
			}
			if (dictionary.ContainsKey("troopsExpended"))
			{
				this.troopsExpended = (dictionary["troopsExpended"] as Dictionary<string, object>);
			}
			if (dictionary.ContainsKey("attackerGuildTroopsExpended"))
			{
				this.attackerGuildTroopsDeployed = (dictionary["attackerGuildTroopsExpended"] as Dictionary<string, object>);
			}
			this.DefenderDeployedData = null;
			if (dictionary.ContainsKey("looted"))
			{
				Dictionary<string, object> dictionary2 = dictionary["looted"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					if (dictionary2.ContainsKey("credits"))
					{
						this.LootCreditsDeducted = Convert.ToInt32(dictionary2["credits"]);
					}
					if (dictionary2.ContainsKey("materials"))
					{
						this.LootMaterialsDeducted = Convert.ToInt32(dictionary2["materials"]);
					}
					if (dictionary2.ContainsKey("contraband"))
					{
						this.LootContrabandDeducted = Convert.ToInt32(dictionary2["contraband"]);
					}
				}
			}
			bool flag = false;
			if (dictionary.ContainsKey("earned"))
			{
				Dictionary<string, object> dictionary3 = dictionary["earned"] as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					flag = true;
					if (dictionary3.ContainsKey("credits"))
					{
						this.LootCreditsEarned = Convert.ToInt32(dictionary3["credits"]);
					}
					if (dictionary3.ContainsKey("materials"))
					{
						this.LootMaterialsEarned = Convert.ToInt32(dictionary3["materials"]);
					}
					if (dictionary3.ContainsKey("contraband"))
					{
						this.LootContrabandEarned = Convert.ToInt32(dictionary3["contraband"]);
					}
				}
			}
			if (!flag)
			{
				this.LootCreditsEarned = this.LootCreditsDeducted;
				this.LootMaterialsEarned = this.LootMaterialsDeducted;
				this.LootContrabandEarned = this.LootContrabandDeducted;
			}
			if (dictionary.ContainsKey("maxLootable"))
			{
				Dictionary<string, object> dictionary4 = dictionary["maxLootable"] as Dictionary<string, object>;
				if (dictionary4 != null)
				{
					if (dictionary4.ContainsKey("credits"))
					{
						this.LootCreditsAvailable = Convert.ToInt32(dictionary4["credits"]);
					}
					if (dictionary4.ContainsKey("materials"))
					{
						this.LootMaterialsAvailable = Convert.ToInt32(dictionary4["materials"]);
					}
					if (dictionary4.ContainsKey("contraband"))
					{
						this.LootContrabandAvailable = Convert.ToInt32(dictionary4["contraband"]);
					}
				}
			}
			this.Revenged = false;
			if (dictionary.ContainsKey("revenged"))
			{
				this.Revenged = Convert.ToBoolean(dictionary["revenged"]);
			}
			string playerId = Service.CurrentPlayer.PlayerId;
			this.Won = ((this.AttackerID == playerId && this.EarnedStars > 0) || (this.DefenderID == playerId && this.EarnedStars == 0));
			if (dictionary.ContainsKey("defenseEncounterProfile"))
			{
				this.DefenseEncounterProfile = (dictionary["defenseEncounterProfile"] as string);
			}
			if (dictionary.ContainsKey("battleScript"))
			{
				this.BattleScript = (dictionary["battleScript"] as string);
			}
			if (dictionary.ContainsKey("attackerEquipment"))
			{
				this.AttackerEquipment = new List<string>();
				List<object> list = dictionary["attackerEquipment"] as List<object>;
				if (list != null)
				{
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						this.AttackerEquipment.Add((string)list[i]);
						i++;
					}
				}
			}
			if (dictionary.ContainsKey("defenderEquipment"))
			{
				this.DefenderEquipment = new List<string>();
				List<object> list2 = dictionary["defenderEquipment"] as List<object>;
				if (list2 != null)
				{
					int j = 0;
					int count2 = list2.Count;
					while (j < count2)
					{
						this.DefenderEquipment.Add((string)list2[j]);
						j++;
					}
				}
			}
			return this;
		}

		public void SetupExpendedTroops()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
			Dictionary<string, int> dictionary5 = new Dictionary<string, int>();
			if (this.troopsExpended != null)
			{
				foreach (string current in this.troopsExpended.Keys)
				{
					int value = Convert.ToInt32(this.troopsExpended[current]);
					TroopTypeVO optional = staticDataController.GetOptional<TroopTypeVO>(current);
					if (optional != null)
					{
						TroopType type = optional.Type;
						if (type != TroopType.Hero)
						{
							if (type != TroopType.Champion)
							{
								dictionary.Add(current, value);
							}
							else
							{
								dictionary4.Add(current, value);
							}
						}
						else
						{
							dictionary3.Add(current, value);
						}
					}
					else
					{
						SpecialAttackTypeVO optional2 = staticDataController.GetOptional<SpecialAttackTypeVO>(current);
						if (optional2 != null)
						{
							dictionary2.Add(current, value);
						}
					}
				}
			}
			if (this.attackerGuildTroopsDeployed != null)
			{
				foreach (string current2 in this.attackerGuildTroopsDeployed.Keys)
				{
					int value2 = Convert.ToInt32(this.attackerGuildTroopsDeployed[current2]);
					TroopTypeVO optional3 = staticDataController.GetOptional<TroopTypeVO>(current2);
					if (optional3 != null)
					{
						dictionary5.Add(current2, value2);
					}
				}
			}
			this.AttackerDeployedData = BattleDeploymentData.CreateEmpty();
			this.AttackerDeployedData.TroopData = dictionary;
			this.AttackerDeployedData.SpecialAttackData = dictionary2;
			this.AttackerDeployedData.HeroData = dictionary3;
			this.AttackerDeployedData.ChampionData = dictionary4;
			this.AttackerDeployedData.SquadData = dictionary5;
		}

		public bool IsPvP()
		{
			return string.IsNullOrEmpty(this.MissionId);
		}

		public bool IsSpecOps()
		{
			return GameUtils.IsMissionSpecOps(this.MissionId);
		}

		public bool IsSquadDeployAllowedInRaid()
		{
			return this.IsRaidDefense() && Service.RaidDefenseController.SquadTroopDeployAllowed();
		}

		public bool IsRaidDefense()
		{
			return GameUtils.IsMissionRaidDefense(this.MissionId);
		}

		public bool IsDefense()
		{
			return GameUtils.IsMissionDefense(this.MissionId);
		}

		public BattleEntry Clone()
		{
			return (BattleEntry)base.MemberwiseClone();
		}
	}
}
