using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads
{
	public class SquadMember : IComparable<SquadMember>, ISerializable
	{
		public string MemberName
		{
			get;
			set;
		}

		public string MemberID
		{
			get;
			set;
		}

		public string Planet
		{
			get;
			set;
		}

		public SquadRole Role
		{
			get;
			set;
		}

		public int Score
		{
			get;
			set;
		}

		public uint JoinDate
		{
			get;
			set;
		}

		public uint LastLoginTime
		{
			get;
			set;
		}

		public int TroopsDonated
		{
			get;
			set;
		}

		public int TroopsReceived
		{
			get;
			set;
		}

		public int ReputationInvested
		{
			get;
			set;
		}

		public int AttacksWon
		{
			get;
			set;
		}

		public int DefensesWon
		{
			get;
			set;
		}

		public Dictionary<string, int> TournamentScore
		{
			get;
			set;
		}

		public int BaseScore
		{
			get;
			set;
		}

		public int WarParty
		{
			get;
			set;
		}

		public int HQLevel
		{
			get;
			set;
		}

		public SquadMember()
		{
			this.MemberName = string.Empty;
			this.MemberID = "0";
			this.Score = 0;
			this.TroopsDonated = 0;
			this.TroopsReceived = 0;
			this.ReputationInvested = 0;
			this.AttacksWon = 0;
			this.DefensesWon = 0;
			this.LastLoginTime = 0u;
			this.TournamentScore = new Dictionary<string, int>();
			this.JoinDate = 0u;
			this.BaseScore = 0;
			this.Role = SquadRole.Member;
			this.WarParty = 0;
			this.HQLevel = 0;
		}

		public int CompareTo(SquadMember compareMember)
		{
			if (compareMember == null)
			{
				return -1;
			}
			return compareMember.Score - this.Score;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("name"))
			{
				this.MemberName = Convert.ToString(dictionary["name"]);
			}
			bool flag = false;
			if (dictionary.ContainsKey("isOwner"))
			{
				flag = Convert.ToBoolean(dictionary["isOwner"]);
			}
			bool flag2 = false;
			if (dictionary.ContainsKey("isOfficer"))
			{
				flag2 = Convert.ToBoolean(dictionary["isOfficer"]);
			}
			if (flag)
			{
				this.Role = SquadRole.Owner;
			}
			else if (flag2)
			{
				this.Role = SquadRole.Officer;
			}
			else
			{
				this.Role = SquadRole.Member;
			}
			if (dictionary.ContainsKey("score"))
			{
				this.Score = Convert.ToInt32(dictionary["score"]);
			}
			if (dictionary.ContainsKey("joinDate"))
			{
				this.JoinDate = Convert.ToUInt32(dictionary["joinDate"]);
			}
			if (dictionary.ContainsKey("lastLoginTime"))
			{
				this.LastLoginTime = Convert.ToUInt32(dictionary["lastLoginTime"]);
			}
			if (dictionary.ContainsKey("troopsDonated"))
			{
				this.TroopsDonated = Convert.ToInt32(dictionary["troopsDonated"]);
			}
			if (dictionary.ContainsKey("reputationInvested"))
			{
				this.ReputationInvested = Convert.ToInt32(dictionary["reputationInvested"]);
			}
			if (dictionary.ContainsKey("troopsReceived"))
			{
				this.TroopsReceived = Convert.ToInt32(dictionary["troopsReceived"]);
			}
			if (dictionary.ContainsKey("attacksWon"))
			{
				this.AttacksWon = Convert.ToInt32(dictionary["attacksWon"]);
			}
			if (dictionary.ContainsKey("defensesWon"))
			{
				this.DefensesWon = Convert.ToInt32(dictionary["defensesWon"]);
			}
			if (dictionary.ContainsKey("tournamentScores"))
			{
				object obj2 = dictionary["tournamentScores"];
				if (obj2 != null)
				{
					Dictionary<string, object> dictionary2 = obj2 as Dictionary<string, object>;
					if (dictionary2 != null)
					{
						foreach (KeyValuePair<string, object> current in dictionary2)
						{
							this.TournamentScore.Add(current.Key, Convert.ToInt32(current.Value));
						}
					}
				}
			}
			if (dictionary.ContainsKey("playerId"))
			{
				this.MemberID = Convert.ToString(dictionary["playerId"]);
			}
			if (dictionary.ContainsKey("planet"))
			{
				this.Planet = Convert.ToString(dictionary["planet"]);
			}
			if (dictionary.ContainsKey("warParty"))
			{
				this.WarParty = Convert.ToInt32(dictionary["warParty"]);
			}
			if (dictionary.ContainsKey("hqLevel"))
			{
				this.HQLevel = Convert.ToInt32(dictionary["hqLevel"]);
			}
			if (dictionary.ContainsKey("xp"))
			{
				this.BaseScore = Convert.ToInt32(dictionary["xp"]);
			}
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}
	}
}
