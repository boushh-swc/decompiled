using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Squads
{
	public class Squad : IComparable<Squad>, ISerializable
	{
		public const string DEFAULT_SQUAD_SYMBOL = "SquadSymbols_01";

		public string SquadName
		{
			get;
			private set;
		}

		public List<SquadMember> MemberList
		{
			get;
			private set;
		}

		public int MemberCount
		{
			get;
			set;
		}

		public int ActiveMemberCount
		{
			get;
			private set;
		}

		public int MemberMax
		{
			get;
			private set;
		}

		public int BattleScore
		{
			get;
			set;
		}

		public int Rank
		{
			get;
			private set;
		}

		public int HighestRank
		{
			get;
			private set;
		}

		public int InviteType
		{
			get;
			set;
		}

		public int RequiredTrophies
		{
			get;
			set;
		}

		public FactionType Faction
		{
			get;
			private set;
		}

		public string Symbol
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string SquadID
		{
			get;
			private set;
		}

		public string CurrentWarId
		{
			get;
			set;
		}

		public int WarSignUpTime
		{
			get;
			set;
		}

		public List<SquadWarHistoryEntry> WarHistory
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			set;
		}

		public int TotalRepInvested
		{
			get;
			set;
		}

		public SquadPerks Perks
		{
			get;
			private set;
		}

		public Squad(string squadID)
		{
			this.SquadID = squadID;
			this.MemberCount = 0;
			this.ActiveMemberCount = 0;
			this.MemberList = new List<SquadMember>();
			this.MemberMax = GameConstants.SQUAD_MEMBER_LIMIT;
			this.Rank = 0;
			this.HighestRank = 0;
			this.BattleScore = 0;
			this.Level = 1;
			this.TotalRepInvested = 0;
			this.WarHistory = new List<SquadWarHistoryEntry>();
			this.Perks = new SquadPerks();
			this.Perks.Default();
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("id"))
			{
				this.SquadID = Convert.ToString(dictionary["id"]);
			}
			if (dictionary.ContainsKey("name"))
			{
				this.SquadName = WWW.UnEscapeURL(Convert.ToString(dictionary["name"]));
			}
			if (dictionary.ContainsKey("description"))
			{
				this.Description = WWW.UnEscapeURL(Convert.ToString(dictionary["description"]));
			}
			if (dictionary.ContainsKey("icon"))
			{
				this.Symbol = Convert.ToString(dictionary["icon"]);
				if (string.IsNullOrEmpty(this.Symbol))
				{
					this.Symbol = "SquadSymbols_01";
				}
			}
			if (dictionary.ContainsKey("rank"))
			{
				int num = Convert.ToInt32(dictionary["rank"]);
				if (num > 0)
				{
					this.Rank = num;
				}
			}
			this.HighestRank = this.Rank;
			if (dictionary.ContainsKey("highestRankAchieved"))
			{
				object obj2 = dictionary["highestRankAchieved"];
				if (obj2 != null)
				{
					this.HighestRank = Convert.ToInt32(obj2);
				}
			}
			if (dictionary.ContainsKey("score"))
			{
				this.BattleScore = Convert.ToInt32(dictionary["score"]);
			}
			else if (dictionary.ContainsKey("value"))
			{
				this.BattleScore = Convert.ToInt32(dictionary["value"]);
			}
			if (dictionary.ContainsKey("memberCount"))
			{
				this.DeserializeMemberCount(Convert.ToInt32(dictionary["memberCount"]));
			}
			if (dictionary.ContainsKey("currentWarId"))
			{
				this.CurrentWarId = Convert.ToString(dictionary["currentWarId"]);
			}
			if (dictionary.ContainsKey("warSignUpTime"))
			{
				this.WarSignUpTime = Convert.ToInt32(dictionary["warSignUpTime"]);
			}
			Dictionary<string, object> dictionary2 = dictionary["membershipRestrictions"] as Dictionary<string, object>;
			if (dictionary2 != null)
			{
				if (dictionary2.ContainsKey("maxSize"))
				{
					this.MemberMax = Convert.ToInt32(dictionary2["maxSize"]);
				}
				if (dictionary2.ContainsKey("minScoreAtEnrollment"))
				{
					this.RequiredTrophies = Convert.ToInt32(dictionary2["minScoreAtEnrollment"]);
				}
				if (dictionary2.ContainsKey("faction"))
				{
					string name = Convert.ToString(dictionary2["faction"]);
					this.Faction = StringUtils.ParseEnum<FactionType>(name);
				}
				if (dictionary2.ContainsKey("openEnrollment"))
				{
					if (Convert.ToBoolean(dictionary2["openEnrollment"]))
					{
						this.InviteType = 1;
					}
					else
					{
						this.InviteType = 0;
					}
				}
			}
			this.DeserializeMemberList(dictionary["members"] as List<object>);
			if (dictionary.ContainsKey("memberCount"))
			{
				this.DeserializeMemberCount(Convert.ToInt32(dictionary["memberCount"]));
			}
			if (dictionary.ContainsKey("activeMemberCount"))
			{
				this.DeserializeActiveMemberCount(Convert.ToInt32(dictionary["activeMemberCount"]));
			}
			if (dictionary.ContainsKey("warHistory"))
			{
				this.DeserializeWarHistoryList(dictionary["warHistory"] as List<object>);
			}
			if (dictionary.ContainsKey("level"))
			{
				object obj3 = dictionary["level"];
				if (obj3 != null)
				{
					this.Level = Convert.ToInt32(obj3);
				}
			}
			if (dictionary.ContainsKey("totalRepInvested"))
			{
				object obj4 = dictionary["totalRepInvested"];
				if (obj4 != null)
				{
					this.TotalRepInvested = Convert.ToInt32(obj4);
				}
			}
			if (dictionary.ContainsKey("perks"))
			{
				this.UpdateSquadPerks(dictionary["perks"]);
			}
			return this;
		}

		public void UpdateSquadPerks(object perksDataObject)
		{
			if (perksDataObject != null)
			{
				this.Perks.FromObject(perksDataObject);
			}
		}

		public void UpdateSquadLevel(int squadLevel, int totalRepInvested)
		{
			this.Level = squadLevel;
			this.TotalRepInvested = totalRepInvested;
		}

		public ISerializable FromFeaturedObject(Dictionary<string, object> d)
		{
			if (d.ContainsKey("faction"))
			{
				string name = Convert.ToString(d["faction"]);
				this.Faction = StringUtils.ParseEnum<FactionType>(name);
			}
			if (d.ContainsKey("name"))
			{
				this.SquadName = WWW.UnEscapeURL(Convert.ToString(d["name"]));
			}
			if (d.ContainsKey("icon"))
			{
				this.Symbol = Convert.ToString(d["icon"]);
				if (string.IsNullOrEmpty(this.Symbol))
				{
					this.Symbol = "SquadSymbols_01";
				}
			}
			if (d.ContainsKey("minScore"))
			{
				this.RequiredTrophies = Convert.ToInt32(d["minScore"]);
			}
			if (d.ContainsKey("score"))
			{
				this.BattleScore = Convert.ToInt32(d["score"]);
			}
			if (d.ContainsKey("rank"))
			{
				this.Rank = Convert.ToInt32(d["rank"]);
			}
			if (d.ContainsKey("openEnrollment"))
			{
				this.InviteType = ((!Convert.ToBoolean(d["openEnrollment"])) ? 0 : 1);
			}
			if (d.ContainsKey("members"))
			{
				this.DeserializeMemberCount(Convert.ToInt32(d["members"]));
			}
			if (d.ContainsKey("activeMemberCount"))
			{
				this.DeserializeActiveMemberCount(Convert.ToInt32(d["activeMemberCount"]));
			}
			if (d.ContainsKey("warHistory"))
			{
				this.DeserializeWarHistoryList(d["warHistory"] as List<object>);
			}
			if (d.ContainsKey("level"))
			{
				object obj = d["level"];
				if (obj != null)
				{
					this.Level = Convert.ToInt32(obj);
				}
				if (this.Level <= 0)
				{
					this.Level = 1;
				}
			}
			return this;
		}

		public ISerializable FromLeaderboardObject(Dictionary<string, object> lbObj)
		{
			if (lbObj.ContainsKey("rank"))
			{
				this.Rank = Convert.ToInt32(lbObj["rank"]);
			}
			if (this.Rank < this.HighestRank)
			{
				this.HighestRank = this.Rank;
			}
			if (lbObj.ContainsKey("value"))
			{
				this.BattleScore = Convert.ToInt32(lbObj["value"]);
			}
			if (lbObj.ContainsKey("account"))
			{
				Dictionary<string, object> dictionary = lbObj["account"] as Dictionary<string, object>;
				if (dictionary != null && dictionary.ContainsKey("manimal"))
				{
					Dictionary<string, object> dictionary2 = dictionary["manimal"] as Dictionary<string, object>;
					if (dictionary2.ContainsKey("data"))
					{
						Dictionary<string, object> dictionary3 = dictionary2["data"] as Dictionary<string, object>;
						if (dictionary3.ContainsKey("name"))
						{
							this.SquadName = WWW.UnEscapeURL(Convert.ToString(dictionary3["name"]));
						}
						if (dictionary3.ContainsKey("level"))
						{
							this.Level = Convert.ToInt32(dictionary3["level"]);
							if (this.Level <= 0)
							{
								this.Level = 1;
							}
						}
						if (dictionary3.ContainsKey("icon"))
						{
							this.Symbol = Convert.ToString(dictionary3["icon"]);
							if (string.IsNullOrEmpty(this.Symbol))
							{
								this.Symbol = "SquadSymbols_01";
							}
						}
						if (dictionary3.ContainsKey("memberCount"))
						{
							this.DeserializeMemberCount(Convert.ToInt32(dictionary3["memberCount"]));
						}
						if (dictionary3.ContainsKey("activeMemberCount"))
						{
							this.DeserializeActiveMemberCount(Convert.ToInt32(dictionary3["activeMemberCount"]));
						}
						if (dictionary3.ContainsKey("warHistory"))
						{
							this.DeserializeWarHistoryList(dictionary3["warHistory"] as List<object>);
						}
						if (dictionary3.ContainsKey("membershipRestrictions"))
						{
							Dictionary<string, object> dictionary4 = dictionary3["membershipRestrictions"] as Dictionary<string, object>;
							if (dictionary4.ContainsKey("faction"))
							{
								string name = Convert.ToString(dictionary4["faction"]);
								this.Faction = StringUtils.ParseEnum<FactionType>(name);
							}
							if (dictionary4.ContainsKey("minScoreAtEnrollment"))
							{
								this.RequiredTrophies = Convert.ToInt32(dictionary4["minScoreAtEnrollment"]);
							}
							if (dictionary4.ContainsKey("openEnrollment"))
							{
								this.InviteType = ((!Convert.ToBoolean(dictionary4["openEnrollment"])) ? 0 : 1);
							}
						}
					}
				}
			}
			return this;
		}

		public ISerializable FromLoginObject(Dictionary<string, object> d)
		{
			if (d.ContainsKey("guildName"))
			{
				string s = d["guildName"] as string;
				this.SquadName = WWW.UnEscapeURL(s);
			}
			return this;
		}

		public ISerializable FromVisitNeighborObject(Dictionary<string, object> d)
		{
			if (d.ContainsKey("guildName"))
			{
				string s = d["guildName"] as string;
				this.SquadName = WWW.UnEscapeURL(s);
			}
			return this;
		}

		private void DeserializeMemberCount(int count)
		{
			this.MemberCount = count;
			if (this.MemberList.Count != 0 && this.MemberCount != this.MemberList.Count)
			{
				this.MemberCount = this.MemberList.Count;
			}
		}

		private void DeserializeActiveMemberCount(int count)
		{
			this.ActiveMemberCount = count;
			if (this.ActiveMemberCount > this.MemberCount)
			{
				this.ActiveMemberCount = this.MemberCount;
			}
		}

		private void DeserializeMemberList(List<object> memberList)
		{
			this.MemberList.Clear();
			if (memberList != null)
			{
				this.MemberCount = memberList.Count;
				int i = 0;
				int count = memberList.Count;
				while (i < count)
				{
					SquadMember squadMember = new SquadMember();
					squadMember.FromObject(memberList[i]);
					this.MemberList.Add(squadMember);
					i++;
				}
				this.MemberList.Sort();
			}
		}

		private void DeserializeWarHistoryList(List<object> warHistoryList)
		{
			if (warHistoryList != null && warHistoryList.Count > 0)
			{
				this.WarHistory.Clear();
				int i = 0;
				int count = warHistoryList.Count;
				while (i < count)
				{
					SquadWarHistoryEntry squadWarHistoryEntry = new SquadWarHistoryEntry();
					squadWarHistoryEntry.FromObject(warHistoryList[i]);
					this.WarHistory.Add(squadWarHistoryEntry);
					i++;
				}
				this.WarHistory.Sort();
			}
		}

		public int CompareTo(Squad compareSquad)
		{
			if (compareSquad == null)
			{
				return -1;
			}
			return compareSquad.BattleScore - this.BattleScore;
		}

		public void ClearSquadWarId()
		{
			this.CurrentWarId = null;
		}
	}
}
