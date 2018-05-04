using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Leaderboard
{
	public class PlayerLBEntity : IComparable<PlayerLBEntity>, ISerializable
	{
		public string PlayerID
		{
			get;
			set;
		}

		public string PlayerName
		{
			get;
			set;
		}

		public string SocialID
		{
			get;
			set;
		}

		public string Planet
		{
			get;
			set;
		}

		public int BattleScore
		{
			get;
			set;
		}

		public int Rank
		{
			get;
			set;
		}

		public int Percentile
		{
			get;
			set;
		}

		public LeaderboardBattleHistory BattleHistory
		{
			get;
			private set;
		}

		public FactionType Faction
		{
			get;
			set;
		}

		public string SquadName
		{
			get;
			set;
		}

		public string SquadID
		{
			get;
			set;
		}

		public string Symbol
		{
			get;
			set;
		}

		public Dictionary<string, LeaderboardBattleHistory> TournamentBattleHistory
		{
			get;
			private set;
		}

		public PlayerLBEntity(string playerId)
		{
			this.PlayerName = string.Empty;
			this.PlayerID = playerId;
			this.SocialID = string.Empty;
			this.Rank = 0;
			this.BattleScore = 0;
			this.Faction = FactionType.Smuggler;
			this.SquadName = string.Empty;
			this.SquadID = string.Empty;
			this.Symbol = string.Empty;
			this.Planet = string.Empty;
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			try
			{
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				if (dictionary.ContainsKey("_id"))
				{
					this.PlayerID = Convert.ToString(dictionary["_id"]);
				}
				if (dictionary.ContainsKey("snid"))
				{
					this.SocialID = Convert.ToString(dictionary["snid"]);
				}
				if (dictionary.ContainsKey("rank"))
				{
					this.Rank = Convert.ToInt32(dictionary["rank"]);
				}
				if (dictionary.ContainsKey("value"))
				{
					this.BattleScore = Convert.ToInt32(dictionary["value"]);
				}
				if (dictionary.ContainsKey("account"))
				{
					Dictionary<string, object> dictionary2 = dictionary["account"] as Dictionary<string, object>;
					if (dictionary2.ContainsKey("manimal"))
					{
						Dictionary<string, object> dictionary3 = dictionary2["manimal"] as Dictionary<string, object>;
						if (dictionary3.ContainsKey("data"))
						{
							Dictionary<string, object> dictionary4 = dictionary3["data"] as Dictionary<string, object>;
							if (dictionary4 == null)
							{
								return null;
							}
							if (dictionary4.ContainsKey("scalars"))
							{
								this.BattleHistory = new LeaderboardBattleHistory(dictionary4["scalars"]);
							}
							if (dictionary4.ContainsKey("name"))
							{
								this.PlayerName = Convert.ToString(dictionary4["name"]);
							}
							if (dictionary4.ContainsKey("playerModel"))
							{
								Dictionary<string, object> dictionary5 = dictionary4["playerModel"] as Dictionary<string, object>;
								if (dictionary5.ContainsKey("faction"))
								{
									string name = Convert.ToString(dictionary5["faction"]);
									this.Faction = StringUtils.ParseEnum<FactionType>(name);
								}
								if (dictionary5.ContainsKey("guildInfo"))
								{
									Dictionary<string, object> dictionary6 = dictionary5["guildInfo"] as Dictionary<string, object>;
									if (dictionary6.ContainsKey("guildId"))
									{
										this.SquadID = Convert.ToString(dictionary6["guildId"]);
									}
									if (dictionary6.ContainsKey("guildName"))
									{
										this.SquadName = WWW.UnEscapeURL(Convert.ToString(dictionary6["guildName"]));
									}
									if (dictionary6.ContainsKey("icon"))
									{
										this.Symbol = Convert.ToString(dictionary6["icon"]);
									}
								}
								if (dictionary5.ContainsKey("map"))
								{
									Dictionary<string, object> dictionary7 = dictionary5["map"] as Dictionary<string, object>;
									if (dictionary7.ContainsKey("planet"))
									{
										this.Planet = Convert.ToString(dictionary7["planet"]);
									}
								}
								this.ParseTournamentData(dictionary5);
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return this;
		}

		private void ParseTournamentData(Dictionary<string, object> playerDict)
		{
			if (playerDict.ContainsKey("tournaments"))
			{
				Dictionary<string, object> dictionary = playerDict["tournaments"] as Dictionary<string, object>;
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, object> current in dictionary)
					{
						if (this.TournamentBattleHistory == null)
						{
							this.TournamentBattleHistory = new Dictionary<string, LeaderboardBattleHistory>();
						}
						LeaderboardBattleHistory value = new LeaderboardBattleHistory(current.Value);
						this.TournamentBattleHistory.Add(current.Key, value);
					}
				}
			}
		}

		public int CompareTo(PlayerLBEntity comparePlayer)
		{
			if (comparePlayer == null)
			{
				return -1;
			}
			return comparePlayer.BattleScore - this.BattleScore;
		}
	}
}
