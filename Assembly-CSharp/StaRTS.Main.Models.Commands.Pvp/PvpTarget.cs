using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.World;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpTarget : AbstractResponse
	{
		private const string BATTLE_ID = "battleId";

		private const string EQUIPMENT = "equipment";

		private const string PLAYER_ID = "playerId";

		private const string PLAYER_NAME = "name";

		private const string PLAYER_LEVEL = "level";

		private const string PLAYER_XP = "xp";

		private const string PLAYER_FACTION = "faction";

		private const string PLAYER_ATTACK_RATING = "attackRating";

		private const string PLAYER_DEFENSE_RATING = "defenseRating";

		private const string PLAYER_ATTACKS_WON = "attacksWon";

		private const string PLAYER_DEFENSES_WON = "defensesWon";

		private const string GUILD_ID = "guildId";

		private const string GUILD_NAME = "guildName";

		private const string GUILD_TROOPS = "guildTroops";

		private const string CHAMPIONS = "champions";

		private const string POTENTIAL_POINTS = "potentialPoints";

		private const string POTENTIAL_SCORE_WIN = "potentialScoreWin";

		private const string POTENTIAL_SCORE_LOSE = "potentialScoreLose";

		private const string POTENTIAL_TOURNAMENT_RATING_DELTA_WIN = "potentialPointsWin";

		private const string POTENTIAL_TOURNAMENT_RATING_DELTA_LOSE = "potentialPointsLose";

		private const string BASE_MAP = "map";

		private const string CREDITS = "credits";

		private const string MATERIALS = "materials";

		private const string CONTRABAND = "contraband";

		private const string BUILDING_LOOT_MAP = "resources";

		private const string CREDITS_CHARGED = "creditsCharged";

		public string BattleId
		{
			get;
			private set;
		}

		public string PlayerId
		{
			get;
			private set;
		}

		public string PlayerName
		{
			get;
			private set;
		}

		public int PlayerLevel
		{
			get;
			private set;
		}

		public int PlayerXp
		{
			get;
			private set;
		}

		public FactionType PlayerFaction
		{
			get;
			private set;
		}

		public int PlayerAttackRating
		{
			get;
			private set;
		}

		public int PlayerDefenseRating
		{
			get;
			private set;
		}

		public int PlayerAttacksWon
		{
			get;
			private set;
		}

		public int PlayerDefensesWon
		{
			get;
			private set;
		}

		public string GuildId
		{
			get;
			private set;
		}

		public string GuildName
		{
			get;
			private set;
		}

		public Dictionary<string, int> GuildDonatedTroops
		{
			get;
			private set;
		}

		public Dictionary<string, int> Champions
		{
			get;
			private set;
		}

		public int PotentialMedalsToGain
		{
			get;
			private set;
		}

		public int PotentialMedalsToLose
		{
			get;
			private set;
		}

		public int PotentialTournamentRatingDeltaWin
		{
			get;
			private set;
		}

		public int PotentialTournamentRatingDeltaLose
		{
			get;
			private set;
		}

		public Map BaseMap
		{
			get;
			private set;
		}

		public int AvailableCredits
		{
			get;
			private set;
		}

		public int AvailableMaterials
		{
			get;
			private set;
		}

		public int AvailableContraband
		{
			get;
			private set;
		}

		public Dictionary<string, int> BuildingLootCreditsMap
		{
			get;
			private set;
		}

		public Dictionary<string, int> BuildingLootMaterialsMap
		{
			get;
			private set;
		}

		public Dictionary<string, int> BuildingLootContrabandMap
		{
			get;
			private set;
		}

		public int CreditsCharged
		{
			get;
			private set;
		}

		public List<ContractTO> Contracts
		{
			get;
			private set;
		}

		public List<string> Equipment
		{
			get;
			private set;
		}

		public Dictionary<string, object> AttackerDeployableServerData
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				Service.Logger.Error("Attempted to create invalid PvpTarget.");
				return null;
			}
			if (dictionary.ContainsKey("battleId"))
			{
				this.BattleId = (string)dictionary["battleId"];
			}
			if (dictionary.ContainsKey("playerId"))
			{
				this.PlayerId = (string)dictionary["playerId"];
			}
			if (dictionary.ContainsKey("name"))
			{
				this.PlayerName = (string)dictionary["name"];
			}
			if (dictionary.ContainsKey("level"))
			{
				this.PlayerLevel = Convert.ToInt32(dictionary["level"]);
			}
			if (dictionary.ContainsKey("xp"))
			{
				this.PlayerXp = Convert.ToInt32(dictionary["xp"]);
			}
			if (dictionary.ContainsKey("attackRating"))
			{
				this.PlayerAttackRating = Convert.ToInt32(dictionary["attackRating"]);
			}
			if (dictionary.ContainsKey("defenseRating"))
			{
				this.PlayerDefenseRating = Convert.ToInt32(dictionary["defenseRating"]);
			}
			if (dictionary.ContainsKey("attacksWon"))
			{
				this.PlayerAttacksWon = Convert.ToInt32(dictionary["attacksWon"]);
			}
			if (dictionary.ContainsKey("defensesWon"))
			{
				this.PlayerDefensesWon = Convert.ToInt32(dictionary["defensesWon"]);
			}
			if (dictionary.ContainsKey("guildId"))
			{
				this.GuildId = (string)dictionary["guildId"];
			}
			if (dictionary.ContainsKey("guildName"))
			{
				this.GuildName = (string)dictionary["guildName"];
			}
			if (dictionary.ContainsKey("map"))
			{
				this.BaseMap = new Map();
				this.BaseMap.FromObject(dictionary["map"]);
				this.BaseMap.InitializePlanet();
			}
			if (dictionary.ContainsKey("resources"))
			{
				this.BuildingLootCreditsMap = new Dictionary<string, int>();
				this.BuildingLootMaterialsMap = new Dictionary<string, int>();
				this.BuildingLootContrabandMap = new Dictionary<string, int>();
				Dictionary<string, object> dictionary2 = dictionary["resources"] as Dictionary<string, object>;
				foreach (KeyValuePair<string, object> current in dictionary2)
				{
					Dictionary<string, object> dictionary3 = current.Value as Dictionary<string, object>;
					if (dictionary3 != null)
					{
						if (dictionary3.ContainsKey("credits"))
						{
							this.BuildingLootCreditsMap.Add(current.Key, Convert.ToInt32(dictionary3["credits"]));
							this.AvailableCredits += Convert.ToInt32(dictionary3["credits"]);
						}
						if (dictionary3.ContainsKey("materials"))
						{
							this.BuildingLootMaterialsMap.Add(current.Key, Convert.ToInt32(dictionary3["materials"]));
							this.AvailableMaterials += Convert.ToInt32(dictionary3["materials"]);
						}
						if (dictionary3.ContainsKey("contraband"))
						{
							this.BuildingLootContrabandMap.Add(current.Key, Convert.ToInt32(dictionary3["contraband"]));
							this.AvailableContraband += Convert.ToInt32(dictionary3["contraband"]);
						}
					}
				}
			}
			if (dictionary.ContainsKey("potentialPoints"))
			{
				Dictionary<string, object> dictionary4 = dictionary["potentialPoints"] as Dictionary<string, object>;
				if (dictionary4 != null)
				{
					if (dictionary4.ContainsKey("potentialScoreWin"))
					{
						this.PotentialMedalsToGain = Convert.ToInt32(dictionary4["potentialScoreWin"]);
					}
					if (dictionary4.ContainsKey("potentialScoreLose"))
					{
						this.PotentialMedalsToLose = Convert.ToInt32(dictionary4["potentialScoreLose"]);
					}
					if (dictionary4.ContainsKey("potentialPointsWin"))
					{
						this.PotentialTournamentRatingDeltaWin = Convert.ToInt32(dictionary4["potentialPointsWin"]);
					}
					if (dictionary4.ContainsKey("potentialPointsLose"))
					{
						this.PotentialTournamentRatingDeltaLose = Convert.ToInt32(dictionary4["potentialPointsLose"]);
					}
				}
			}
			if (dictionary.ContainsKey("guildTroops"))
			{
				this.GuildDonatedTroops = new Dictionary<string, int>();
				Dictionary<string, object> dictionary5 = dictionary["guildTroops"] as Dictionary<string, object>;
				if (dictionary5 != null)
				{
					foreach (KeyValuePair<string, object> current2 in dictionary5)
					{
						string key = current2.Key;
						int num = 0;
						Dictionary<string, object> dictionary6 = current2.Value as Dictionary<string, object>;
						if (dictionary6 != null)
						{
							foreach (KeyValuePair<string, object> current3 in dictionary6)
							{
								num += Convert.ToInt32(current3.Value);
							}
							this.GuildDonatedTroops.Add(key, num);
						}
					}
				}
			}
			if (dictionary.ContainsKey("champions"))
			{
				this.Champions = new Dictionary<string, int>(StringComparer.Ordinal);
				Dictionary<string, object> dictionary7 = dictionary["champions"] as Dictionary<string, object>;
				if (dictionary7 != null)
				{
					foreach (KeyValuePair<string, object> current4 in dictionary7)
					{
						string key2 = current4.Key;
						this.Champions.Add(key2, Convert.ToInt32(current4.Value));
					}
				}
			}
			if (dictionary.ContainsKey("creditsCharged"))
			{
				this.CreditsCharged = Convert.ToInt32(dictionary["creditsCharged"]);
			}
			this.Contracts = new List<ContractTO>();
			if (dictionary.ContainsKey("contracts"))
			{
				List<object> list = dictionary["contracts"] as List<object>;
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						ContractTO item = new ContractTO().FromObject(list[i]) as ContractTO;
						this.Contracts.Add(item);
					}
				}
			}
			if (dictionary.ContainsKey("faction"))
			{
				string name = Convert.ToString(dictionary["faction"]);
				this.PlayerFaction = StringUtils.ParseEnum<FactionType>(name);
			}
			if (dictionary.ContainsKey("attackerDeployables"))
			{
				this.AttackerDeployableServerData = (dictionary["attackerDeployables"] as Dictionary<string, object>);
			}
			if (dictionary.ContainsKey("equipment"))
			{
				List<object> list2 = dictionary["equipment"] as List<object>;
				if (list2 != null)
				{
					this.Equipment = new List<string>();
					int j = 0;
					int count = list2.Count;
					while (j < count)
					{
						this.Equipment.Add(list2[j] as string);
						j++;
					}
				}
			}
			return this;
		}
	}
}
