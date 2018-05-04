using StaRTS.Main.Models.Cee.Serializables;
using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle.Replay
{
	public class BattleRecord : ISerializable
	{
		public const string ACTION_ID_KEY = "actionId";

		private const string COMBAT_ENCOUNTER_KEY = "combatEncounter";

		private const string BATTLE_ATTRIBUTES_KEY = "battleAttributes";

		private const string BATTLE_ACTIONS_KEY = "battleActions";

		private const string BATTLE_TYPE_KEY = "battleType";

		private const string BATTLE_LENGTH_KEY = "battleLength";

		private const string ATTACKER_DEPLOYMENT_DATA_KEY = "attackerDeploymentData";

		private const string DEFENDER_DEPLOYMENT_DATA_KEY = "defenderDeploymentData";

		private const string PLANET_ID_KEY = "planetId";

		private const string LOOT_CREDITS_AVAILABLE_KEY = "lootCreditsAvailable";

		private const string LOOT_MATERIALS_AVAILABLE_KEY = "lootMaterialsAvailable";

		private const string LOOT_CONTRABAND_AVAILABLE_KEY = "lootContrabandAvailable";

		private const string LOOT_BUILDING_CREDITS_MAP_KEY = "lootBuildingCreditsMap";

		private const string LOOT_BUILDING_MATERIALS_MAP_KEY = "lootBuildingMaterialsMap";

		private const string LOOT_BUILDING_CONTRABAND_MAP_KEY = "lootBuildingContrabandMap";

		private const string BATTLE_VERSION_KEY = "battleVersion";

		private const string BATTLE_PLANET_ID_KEY = "planetId";

		private const string CMS_VERSION_KEY = "manifestVersion";

		private const string LOWEST_FPS_KEY = "lowFPS";

		private const string LOWEST_FPS_TIME_KEY = "lowFPSTime";

		private const string VICTORY_CONDITIONS = "victoryConditions";

		private const string FAILURE_CONDITION = "failureCondition";

		private const string DEFENDER_GUILD_TROOPS = "donatedTroops";

		private const string ATTACKER_GUILD_TROOPS = "donatedTroopsAttacker";

		private const string DEFENDER_CHAMPIONS = "champions";

		private const string DEFENSE_ENCOUNTER_PROFILE = "defenseEncounterProfile";

		private const string BATTLE_SCRIPT = "battleScript";

		private const string DISABLED_BUILDINGS = "disabledBuildings";

		private const string VIEW_TIME_PRE_BATTLE = "viewTimePreBattle";

		private List<IBattleAction> battleActions;

		public List<string> victoryConditionsUids;

		public string failureConditionUid;

		public string DefenseEncounterProfile;

		public string BattleScript;

		public RandSimSeed SimSeed = default(RandSimSeed);

		public string RecordId
		{
			get;
			set;
		}

		public CombatEncounter CombatEncounter
		{
			get;
			set;
		}

		public BattleDeploymentData AttackerDeploymentData
		{
			get;
			set;
		}

		public BattleDeploymentData DefenderDeploymentData
		{
			get;
			set;
		}

		public Dictionary<string, int> DefenderGuildTroops
		{
			get;
			set;
		}

		public Dictionary<string, int> AttackerGuildTroops
		{
			get;
			set;
		}

		public Dictionary<string, int> DefenderChampions
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

		public Dictionary<string, int> BuildingLootCreditsMap
		{
			get;
			set;
		}

		public Dictionary<string, int> BuildingLootMaterialsMap
		{
			get;
			set;
		}

		public Dictionary<string, int> BuildingLootContrabandMap
		{
			get;
			set;
		}

		public BattleType BattleType
		{
			get;
			set;
		}

		public List<IBattleAction> BattleActions
		{
			get
			{
				return this.battleActions;
			}
		}

		public string MissionId
		{
			get;
			set;
		}

		public string PlanetId
		{
			get;
			set;
		}

		public string BattleVersion
		{
			get;
			set;
		}

		public string CmsVersion
		{
			get;
			set;
		}

		public BattleAttributes BattleAttributes
		{
			get;
			set;
		}

		public int BattleLength
		{
			get;
			set;
		}

		public List<string> DisabledBuildings
		{
			get;
			set;
		}

		public float LowestFPS
		{
			get;
			set;
		}

		public uint LowestFPSTime
		{
			get;
			set;
		}

		public float ViewTimePassedPreBattle
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

		public BattleRecord()
		{
			this.battleActions = new List<IBattleAction>();
			this.BattleAttributes = new BattleAttributes();
		}

		public void Add(IBattleAction battleAction)
		{
			this.battleActions.Add(battleAction);
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.CombatEncounter = (new CombatEncounter().FromObject(dictionary["combatEncounter"]) as CombatEncounter);
			List<object> list = dictionary["battleActions"] as List<object>;
			foreach (object current in list)
			{
				Dictionary<string, object> dictionary2 = current as Dictionary<string, object>;
				string actionId = dictionary2["actionId"] as string;
				this.BattleActions.Add(BattleRecord.CreateBattleAction(actionId, current));
			}
			if (dictionary.ContainsKey("attackerDeploymentData"))
			{
				object obj2 = dictionary["attackerDeploymentData"];
				if (obj2 != null)
				{
					this.AttackerDeploymentData = (new BattleDeploymentData().FromObject(obj2) as BattleDeploymentData);
				}
			}
			if (dictionary.ContainsKey("defenderDeploymentData"))
			{
				object obj3 = dictionary["defenderDeploymentData"];
				if (obj3 != null)
				{
					this.DefenderDeploymentData = (new BattleDeploymentData().FromObject(obj3) as BattleDeploymentData);
				}
			}
			if (dictionary.ContainsKey("battleType"))
			{
				this.BattleType = StringUtils.ParseEnum<BattleType>(dictionary["battleType"] as string);
			}
			else
			{
				this.BattleType = BattleType.Pvp;
			}
			this.BattleLength = Convert.ToInt32(dictionary["battleLength"]);
			if (dictionary.ContainsKey("lowFPS"))
			{
				this.LowestFPS = (float)Convert.ToInt32(dictionary["lowFPS"]);
			}
			if (dictionary.ContainsKey("lowFPSTime"))
			{
				this.LowestFPSTime = (uint)Convert.ToInt32(dictionary["lowFPSTime"]);
			}
			if (dictionary.ContainsKey("missionId"))
			{
				this.MissionId = (dictionary["missionId"] as string);
			}
			if (dictionary.ContainsKey("planetId"))
			{
				this.PlanetId = (dictionary["planetId"] as string);
			}
			if (this.PlanetId == null && this.CombatEncounter.map.Planet != null)
			{
				this.PlanetId = this.CombatEncounter.map.Planet.Uid;
			}
			if (dictionary.ContainsKey("battleVersion"))
			{
				this.BattleVersion = (dictionary["battleVersion"] as string);
			}
			this.CmsVersion = (dictionary["manifestVersion"] as string);
			if (dictionary.ContainsKey("battleAttributes"))
			{
				this.BattleAttributes = new BattleAttributes();
				this.BattleAttributes.FromObject(dictionary["battleAttributes"]);
			}
			this.LootCreditsAvailable = Convert.ToInt32(dictionary["lootCreditsAvailable"]);
			this.LootMaterialsAvailable = Convert.ToInt32(dictionary["lootMaterialsAvailable"]);
			if (dictionary.ContainsKey("lootContrabandAvailable"))
			{
				this.LootContrabandAvailable = Convert.ToInt32(dictionary["lootContrabandAvailable"]);
			}
			this.BuildingLootCreditsMap = new Dictionary<string, int>();
			if (dictionary.ContainsKey("lootBuildingCreditsMap"))
			{
				Dictionary<string, object> dictionary3 = dictionary["lootBuildingCreditsMap"] as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					foreach (KeyValuePair<string, object> current2 in dictionary3)
					{
						this.BuildingLootCreditsMap.Add(current2.Key, Convert.ToInt32(current2.Value));
					}
				}
			}
			this.BuildingLootMaterialsMap = new Dictionary<string, int>();
			if (dictionary.ContainsKey("lootBuildingMaterialsMap"))
			{
				Dictionary<string, object> dictionary4 = dictionary["lootBuildingMaterialsMap"] as Dictionary<string, object>;
				if (dictionary4 != null)
				{
					foreach (KeyValuePair<string, object> current3 in dictionary4)
					{
						this.BuildingLootMaterialsMap.Add(current3.Key, Convert.ToInt32(current3.Value));
					}
				}
			}
			this.BuildingLootContrabandMap = new Dictionary<string, int>();
			if (dictionary.ContainsKey("lootBuildingContrabandMap"))
			{
				Dictionary<string, object> dictionary5 = dictionary["lootBuildingContrabandMap"] as Dictionary<string, object>;
				if (dictionary5 != null)
				{
					foreach (KeyValuePair<string, object> current4 in dictionary5)
					{
						this.BuildingLootContrabandMap.Add(current4.Key, Convert.ToInt32(current4.Value));
					}
				}
			}
			this.victoryConditionsUids = new List<string>();
			List<object> list2 = dictionary["victoryConditions"] as List<object>;
			int i = 0;
			int count = list2.Count;
			while (i < count)
			{
				this.victoryConditionsUids.Add(list2[i] as string);
				i++;
			}
			this.failureConditionUid = (dictionary["failureCondition"] as string);
			this.DefenderGuildTroops = new Dictionary<string, int>();
			if (dictionary.ContainsKey("donatedTroops"))
			{
				Dictionary<string, object> dictionary6 = dictionary["donatedTroops"] as Dictionary<string, object>;
				if (dictionary6 != null)
				{
					foreach (KeyValuePair<string, object> current5 in dictionary6)
					{
						this.DefenderGuildTroops.Add(current5.Key, Convert.ToInt32(current5.Value));
					}
				}
			}
			this.AttackerGuildTroops = new Dictionary<string, int>();
			if (dictionary.ContainsKey("donatedTroopsAttacker"))
			{
				Dictionary<string, object> dictionary7 = dictionary["donatedTroopsAttacker"] as Dictionary<string, object>;
				if (dictionary7 != null)
				{
					foreach (KeyValuePair<string, object> current6 in dictionary7)
					{
						this.AttackerGuildTroops.Add(current6.Key, Convert.ToInt32(current6.Value));
					}
				}
			}
			this.DefenderChampions = new Dictionary<string, int>();
			if (dictionary.ContainsKey("champions"))
			{
				Dictionary<string, object> dictionary8 = dictionary["champions"] as Dictionary<string, object>;
				if (dictionary8 != null)
				{
					foreach (KeyValuePair<string, object> current7 in dictionary8)
					{
						this.DefenderChampions.Add(current7.Key, Convert.ToInt32(current7.Value));
					}
				}
			}
			if (dictionary.ContainsKey("defenseEncounterProfile"))
			{
				this.DefenseEncounterProfile = (dictionary["defenseEncounterProfile"] as string);
			}
			if (dictionary.ContainsKey("battleScript"))
			{
				this.BattleScript = (dictionary["battleScript"] as string);
			}
			this.DisabledBuildings = new List<string>();
			if (dictionary.ContainsKey("disabledBuildings"))
			{
				List<object> list3 = dictionary["disabledBuildings"] as List<object>;
				for (int j = 0; j < list3.Count; j++)
				{
					this.DisabledBuildings.Add((string)list3[j]);
				}
			}
			if (dictionary.ContainsKey("simSeedA"))
			{
				this.SimSeed.SimSeedA = uint.Parse(dictionary["simSeedA"] as string);
			}
			if (dictionary.ContainsKey("simSeedB"))
			{
				this.SimSeed.SimSeedB = uint.Parse(dictionary["simSeedB"] as string);
			}
			if (dictionary.ContainsKey("viewTimePreBattle"))
			{
				this.ViewTimePassedPreBattle = float.Parse(dictionary["viewTimePreBattle"] as string);
			}
			this.AttackerWarBuffs = this.ParseStringList(dictionary, "attackerWarBuffs");
			this.DefenderWarBuffs = this.ParseStringList(dictionary, "defenderWarBuffs");
			this.AttackerEquipment = this.ParseStringList(dictionary, "attackerEquipment");
			this.DefenderEquipment = this.ParseStringList(dictionary, "defenderEquipment");
			return this;
		}

		private List<string> ParseStringList(Dictionary<string, object> dictionary, string key)
		{
			List<string> list = null;
			if (dictionary.ContainsKey(key))
			{
				List<object> list2 = dictionary[key] as List<object>;
				if (list2 != null)
				{
					list = new List<string>();
					int i = 0;
					int count = list2.Count;
					while (i < count)
					{
						list.Add(Convert.ToString(list2[i]));
						i++;
					}
				}
			}
			return list;
		}

		private static IBattleAction CreateBattleAction(string actionId, object action)
		{
			IBattleAction result = null;
			switch (actionId)
			{
			case "TroopPlaced":
				result = (new TroopPlacedAction().FromObject(action) as TroopPlacedAction);
				break;
			case "SpecialAttackDeployed":
				result = (new SpecialAttackDeployedAction().FromObject(action) as SpecialAttackDeployedAction);
				break;
			case "HeroDeployed":
				result = (new HeroDeployedAction().FromObject(action) as HeroDeployedAction);
				break;
			case "HeroAbilityActivated":
				result = (new HeroAbilityAction().FromObject(action) as HeroAbilityAction);
				break;
			case "ChampionDeployed":
				result = (new ChampionDeployedAction().FromObject(action) as ChampionDeployedAction);
				break;
			case "SquadTroopPlaced":
				result = (new SquadTroopPlacedAction().FromObject(action) as SquadTroopPlacedAction);
				break;
			case "BattleCanceled":
				result = (new BattleCanceledAction().FromObject(action) as BattleCanceledAction);
				break;
			}
			return result;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddObject<CombatEncounter>("combatEncounter", this.CombatEncounter);
			serializer.AddArray<IBattleAction>("battleActions", this.battleActions);
			serializer.AddObject<BattleDeploymentData>("attackerDeploymentData", this.AttackerDeploymentData);
			serializer.AddObject<BattleDeploymentData>("defenderDeploymentData", this.DefenderDeploymentData);
			serializer.Add<int>("lootCreditsAvailable", this.LootCreditsAvailable);
			serializer.Add<int>("lootMaterialsAvailable", this.LootMaterialsAvailable);
			serializer.Add<int>("lootContrabandAvailable", this.LootContrabandAvailable);
			if (this.BuildingLootCreditsMap != null)
			{
				serializer.AddDictionary<int>("lootBuildingCreditsMap", this.BuildingLootCreditsMap);
			}
			if (this.BuildingLootMaterialsMap != null)
			{
				serializer.AddDictionary<int>("lootBuildingMaterialsMap", this.BuildingLootMaterialsMap);
			}
			if (this.BuildingLootContrabandMap != null)
			{
				serializer.AddDictionary<int>("lootBuildingContrabandMap", this.BuildingLootContrabandMap);
			}
			serializer.AddString("battleType", this.BattleType.ToString());
			serializer.Add<int>("battleLength", this.BattleLength);
			serializer.Add<int>("lowFPS", (int)this.LowestFPS);
			serializer.Add<int>("lowFPSTime", (int)this.LowestFPSTime);
			serializer.AddString("battleVersion", this.BattleVersion);
			serializer.AddString("planetId", this.PlanetId);
			serializer.AddString("manifestVersion", this.CmsVersion);
			serializer.AddObject<BattleAttributes>("battleAttributes", this.BattleAttributes);
			if (this.victoryConditionsUids != null)
			{
				serializer.AddArrayOfPrimitives<string>("victoryConditions", this.victoryConditionsUids);
			}
			serializer.AddString("failureCondition", this.failureConditionUid);
			if (this.DefenderGuildTroops != null)
			{
				serializer.AddDictionary<int>("donatedTroops", this.DefenderGuildTroops);
			}
			if (this.AttackerGuildTroops != null)
			{
				serializer.AddDictionary<int>("donatedTroopsAttacker", this.AttackerGuildTroops);
			}
			if (this.DefenderChampions != null)
			{
				serializer.AddDictionary<int>("champions", this.DefenderChampions);
			}
			if (!string.IsNullOrEmpty(this.DefenseEncounterProfile))
			{
				serializer.AddString("defenseEncounterProfile", this.DefenseEncounterProfile);
			}
			if (!string.IsNullOrEmpty(this.BattleScript))
			{
				serializer.AddString("battleScript", this.BattleScript);
			}
			if (this.DisabledBuildings != null)
			{
				serializer.AddArrayOfPrimitives<string>("disabledBuildings", this.DisabledBuildings);
			}
			serializer.Add<uint>("simSeedA", this.SimSeed.SimSeedA);
			serializer.Add<uint>("simSeedB", this.SimSeed.SimSeedB);
			serializer.Add<float>("viewTimePreBattle", this.ViewTimePassedPreBattle);
			if (this.AttackerWarBuffs != null)
			{
				serializer.AddArrayOfPrimitives<string>("attackerWarBuffs", this.AttackerWarBuffs);
			}
			if (this.DefenderWarBuffs != null)
			{
				serializer.AddArrayOfPrimitives<string>("defenderWarBuffs", this.DefenderWarBuffs);
			}
			if (this.AttackerEquipment != null)
			{
				serializer.AddArrayOfPrimitives<string>("attackerEquipment", this.AttackerEquipment);
			}
			if (this.DefenderEquipment != null)
			{
				serializer.AddArrayOfPrimitives<string>("defenderEquipment", this.DefenderEquipment);
			}
			return serializer.End().ToString();
		}
	}
}
