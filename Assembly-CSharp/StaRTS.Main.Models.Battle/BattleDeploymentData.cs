using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle
{
	public class BattleDeploymentData : ISerializable
	{
		public Dictionary<string, int> TroopData
		{
			get;
			set;
		}

		public Dictionary<string, int> SpecialAttackData
		{
			get;
			set;
		}

		public Dictionary<string, int> HeroData
		{
			get;
			set;
		}

		public Dictionary<string, int> ChampionData
		{
			get;
			set;
		}

		public Dictionary<string, int> SquadData
		{
			get;
			set;
		}

		public List<DeploymentRecord> TroopDataList
		{
			get;
			set;
		}

		public List<DeploymentRecord> SpecialAttackDataList
		{
			get;
			set;
		}

		public List<DeploymentRecord> HeroDataList
		{
			get;
			set;
		}

		public List<DeploymentRecord> ChampionDataList
		{
			get;
			set;
		}

		public List<DeploymentRecord> SquadDataList
		{
			get;
			set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.TroopData = new Dictionary<string, int>();
			this.SpecialAttackData = new Dictionary<string, int>();
			this.HeroData = new Dictionary<string, int>();
			this.ChampionData = new Dictionary<string, int>();
			this.SquadData = new Dictionary<string, int>();
			this.TroopDataList = new List<DeploymentRecord>();
			this.SpecialAttackDataList = new List<DeploymentRecord>();
			this.HeroDataList = new List<DeploymentRecord>();
			this.ChampionDataList = new List<DeploymentRecord>();
			if (dictionary.ContainsKey("troopList"))
			{
				List<object> list = dictionary["troopList"] as List<object>;
				if (list != null)
				{
					foreach (object current in list)
					{
						this.TroopDataList.Add((DeploymentRecord)new DeploymentRecord().FromObject(current));
					}
				}
			}
			if (dictionary.ContainsKey("troop"))
			{
				Dictionary<string, object> dictionary2 = dictionary["troop"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (KeyValuePair<string, object> current2 in dictionary2)
					{
						this.TroopData.Add(current2.Key, Convert.ToInt32(current2.Value));
					}
				}
			}
			if (dictionary.ContainsKey("specialAttackList"))
			{
				List<object> list2 = dictionary["specialAttackList"] as List<object>;
				if (list2 != null)
				{
					foreach (object current3 in list2)
					{
						this.SpecialAttackDataList.Add((DeploymentRecord)new DeploymentRecord().FromObject(current3));
					}
				}
			}
			if (dictionary.ContainsKey("specialAttack"))
			{
				Dictionary<string, object> dictionary3 = dictionary["specialAttack"] as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					foreach (KeyValuePair<string, object> current4 in dictionary3)
					{
						this.SpecialAttackData.Add(current4.Key, Convert.ToInt32(current4.Value));
					}
				}
			}
			if (dictionary.ContainsKey("heroList"))
			{
				List<object> list3 = dictionary["heroList"] as List<object>;
				if (list3 != null)
				{
					foreach (object current5 in list3)
					{
						this.HeroDataList.Add((DeploymentRecord)new DeploymentRecord().FromObject(current5));
					}
				}
			}
			if (dictionary.ContainsKey("hero"))
			{
				Dictionary<string, object> dictionary4 = dictionary["hero"] as Dictionary<string, object>;
				if (dictionary4 != null)
				{
					foreach (KeyValuePair<string, object> current6 in dictionary4)
					{
						this.HeroData.Add(current6.Key, Convert.ToInt32(current6.Value));
					}
				}
			}
			if (dictionary.ContainsKey("championList"))
			{
				List<object> list4 = dictionary["championList"] as List<object>;
				if (list4 != null)
				{
					foreach (object current7 in list4)
					{
						this.ChampionDataList.Add((DeploymentRecord)new DeploymentRecord().FromObject(current7));
					}
				}
			}
			if (dictionary.ContainsKey("champion"))
			{
				Dictionary<string, object> dictionary5 = dictionary["champion"] as Dictionary<string, object>;
				if (dictionary5 != null)
				{
					foreach (KeyValuePair<string, object> current8 in dictionary5)
					{
						this.ChampionData.Add(current8.Key, Convert.ToInt32(current8.Value));
					}
				}
			}
			return this;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			if (this.TroopData != null)
			{
				serializer.AddDictionary<int>("troop", this.TroopData);
			}
			if (this.TroopDataList != null)
			{
				serializer.AddArray<DeploymentRecord>("troopList", this.TroopDataList);
			}
			if (this.SpecialAttackData != null)
			{
				serializer.AddDictionary<int>("specialAttack", this.SpecialAttackData);
			}
			if (this.SpecialAttackDataList != null)
			{
				serializer.AddArray<DeploymentRecord>("specialAttackList", this.SpecialAttackDataList);
			}
			if (this.HeroData != null)
			{
				serializer.AddDictionary<int>("hero", this.HeroData);
			}
			if (this.HeroDataList != null)
			{
				serializer.AddArray<DeploymentRecord>("heroList", this.HeroDataList);
			}
			if (this.ChampionData != null)
			{
				serializer.AddDictionary<int>("champion", this.ChampionData);
			}
			if (this.ChampionDataList != null)
			{
				serializer.AddArray<DeploymentRecord>("championList", this.ChampionDataList);
			}
			return serializer.End().ToString();
		}

		public static BattleDeploymentData CreateEmpty()
		{
			return new BattleDeploymentData
			{
				TroopData = null,
				SpecialAttackData = null,
				HeroData = null,
				ChampionData = null,
				TroopDataList = null,
				SpecialAttackDataList = null,
				HeroDataList = null,
				ChampionDataList = null,
				SquadData = null
			};
		}

		public static BattleDeploymentData Copy(BattleDeploymentData deploymentData)
		{
			BattleDeploymentData battleDeploymentData = new BattleDeploymentData();
			if (deploymentData.TroopData != null)
			{
				battleDeploymentData.TroopData = new Dictionary<string, int>(deploymentData.TroopData);
			}
			if (deploymentData.SpecialAttackData != null)
			{
				battleDeploymentData.SpecialAttackData = new Dictionary<string, int>(deploymentData.SpecialAttackData);
			}
			if (deploymentData.HeroData != null)
			{
				battleDeploymentData.HeroData = new Dictionary<string, int>(deploymentData.HeroData);
			}
			if (deploymentData.ChampionData != null)
			{
				battleDeploymentData.ChampionData = new Dictionary<string, int>(deploymentData.ChampionData);
			}
			if (deploymentData.SquadData != null)
			{
				battleDeploymentData.SquadData = new Dictionary<string, int>(deploymentData.SquadData);
			}
			if (deploymentData.TroopDataList != null)
			{
				battleDeploymentData.TroopDataList = new List<DeploymentRecord>(deploymentData.TroopDataList);
			}
			if (deploymentData.SpecialAttackDataList != null)
			{
				battleDeploymentData.SpecialAttackDataList = new List<DeploymentRecord>(deploymentData.SpecialAttackDataList);
			}
			if (deploymentData.HeroDataList != null)
			{
				battleDeploymentData.HeroDataList = new List<DeploymentRecord>(deploymentData.HeroDataList);
			}
			if (deploymentData.ChampionDataList != null)
			{
				battleDeploymentData.ChampionDataList = new List<DeploymentRecord>(deploymentData.ChampionDataList);
			}
			return battleDeploymentData;
		}
	}
}
