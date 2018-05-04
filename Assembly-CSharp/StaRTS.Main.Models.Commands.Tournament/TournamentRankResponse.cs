using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class TournamentRankResponse : AbstractResponse
	{
		public TournamentRank Rank
		{
			get;
			private set;
		}

		public Tournament TournamentData
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.Rank = new TournamentRank();
			this.Rank.FromObject(obj);
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("tournament"))
			{
				this.TournamentData = new Tournament();
				this.TournamentData.FromObject(dictionary["tournament"]);
			}
			return this;
		}
	}
}
