using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Player.Misc
{
	public class BattleParticipant : ISerializable
	{
		public string PlayerId
		{
			get;
			set;
		}

		public string PlayerName
		{
			get;
			set;
		}

		public string GuildId
		{
			get;
			set;
		}

		public string GuildName
		{
			get;
			set;
		}

		public int AttackRating
		{
			get;
			set;
		}

		public int AttackRatingDelta
		{
			get;
			set;
		}

		public int DefenseRating
		{
			get;
			set;
		}

		public int DefenseRatingDelta
		{
			get;
			set;
		}

		public int TournamentRating
		{
			get;
			set;
		}

		public int TournamentRatingDelta
		{
			get;
			set;
		}

		public FactionType PlayerFaction
		{
			get;
			set;
		}

		public BattleParticipant(string id, string name, FactionType faction)
		{
			this.PlayerId = id;
			this.PlayerName = name;
			this.PlayerFaction = faction;
			if (string.IsNullOrEmpty(this.PlayerName))
			{
				this.PlayerName = this.PlayerId;
			}
		}

		public static BattleParticipant CreateFromObject(object obj)
		{
			return new BattleParticipant(null, null, FactionType.Invalid).FromObject(obj) as BattleParticipant;
		}

		public string ToJson()
		{
			return Serializer.Start().AddString("playerId", this.PlayerId).AddString("name", this.PlayerName).AddString("guildId", this.GuildId).AddString("guildName", this.GuildName).Add<int>("attackRating", this.AttackRating).Add<int>("attackRatingDelta", this.AttackRatingDelta).Add<int>("defenseRating", this.DefenseRating).Add<int>("defenseRatingDelta", this.DefenseRatingDelta).Add<int>("tournamentRating", this.TournamentRating).Add<int>("tournamentRatingDelta", this.TournamentRatingDelta).Add<string>("faction", this.PlayerFaction.ToString()).End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				this.PlayerId = Convert.ToString(dictionary["playerId"]);
				this.PlayerName = Convert.ToString(dictionary["name"]);
				this.GuildId = Convert.ToString(dictionary["guildId"]);
				this.GuildName = WWW.UnEscapeURL(Convert.ToString(dictionary["guildName"]));
				this.AttackRating = Convert.ToInt32(dictionary["attackRating"]);
				this.AttackRatingDelta = Convert.ToInt32(dictionary["attackRatingDelta"]);
				this.DefenseRating = Convert.ToInt32(dictionary["defenseRating"]);
				this.DefenseRatingDelta = Convert.ToInt32(dictionary["defenseRatingDelta"]);
				if (string.IsNullOrEmpty(this.PlayerName))
				{
					this.PlayerName = this.PlayerId;
				}
				if (string.IsNullOrEmpty(this.GuildName))
				{
					this.GuildName = this.GuildId;
				}
				if (dictionary.ContainsKey("faction"))
				{
					string name = Convert.ToString(dictionary["faction"]);
					this.PlayerFaction = StringUtils.ParseEnum<FactionType>(name);
				}
				if (dictionary.ContainsKey("tournamentRating"))
				{
					this.TournamentRating = Convert.ToInt32(dictionary["tournamentRating"]);
				}
				if (dictionary.ContainsKey("tournamentRatingDelta"))
				{
					this.TournamentRatingDelta = Convert.ToInt32(dictionary["tournamentRatingDelta"]);
				}
			}
			return this;
		}
	}
}
