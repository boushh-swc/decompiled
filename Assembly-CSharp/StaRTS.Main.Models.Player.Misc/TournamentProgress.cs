using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Commands.Tournament;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class TournamentProgress : ISerializable
	{
		private Dictionary<string, Tournament> tournaments;

		public TournamentProgress()
		{
			this.tournaments = new Dictionary<string, Tournament>();
		}

		public bool HasTournament(TournamentVO tournamentVO)
		{
			return this.tournaments.ContainsKey(tournamentVO.Uid);
		}

		public void AddTournament(string uid, Tournament tournament)
		{
			this.tournaments.Add(uid, tournament);
		}

		public Tournament GetTournament(string uid)
		{
			Tournament result = null;
			if (this.tournaments.ContainsKey(uid))
			{
				result = this.tournaments[uid];
			}
			return result;
		}

		public TournamentRank GetTournamentCurrentRank(string uid)
		{
			if (this.tournaments.ContainsKey(uid))
			{
				Tournament tournament = this.tournaments[uid];
				return tournament.CurrentRank;
			}
			return null;
		}

		public TournamentRank GetTournamentFinalRank(string uid)
		{
			if (this.tournaments.ContainsKey(uid))
			{
				Tournament tournament = this.tournaments[uid];
				return tournament.FinalRank;
			}
			return null;
		}

		public AbstractTimedEvent GetTimedEvent(string eventUid)
		{
			if (this.tournaments.ContainsKey(eventUid))
			{
				return this.tournaments[eventUid];
			}
			return null;
		}

		public string ToJson()
		{
			return "{}";
		}

		public void RemoveMissingTournamentData()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<string> list = new List<string>();
			foreach (string current in this.tournaments.Keys)
			{
				if (staticDataController.GetOptional<TournamentVO>(current) == null)
				{
					list.Add(current);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.tournaments.Remove(list[i]);
			}
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("tournaments"))
			{
				Dictionary<string, object> dictionary2 = dictionary["tournaments"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						Tournament tournament = new Tournament();
						tournament.FromObject(current.Value);
						this.tournaments.Add(current.Key, tournament);
					}
				}
			}
			return this;
		}
	}
}
