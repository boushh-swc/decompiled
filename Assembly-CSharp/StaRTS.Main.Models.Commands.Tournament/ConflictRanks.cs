using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class ConflictRanks : AbstractResponse
	{
		public Dictionary<string, object> tournamentRankResponse;

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				this.tournamentRankResponse = new Dictionary<string, object>();
				Dictionary<string, object> dictionary2 = dictionary;
				foreach (KeyValuePair<string, object> current in dictionary2)
				{
					this.tournamentRankResponse.Add(current.Key, current.Value);
				}
			}
			return this;
		}
	}
}
