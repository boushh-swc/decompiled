using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskProgressCompleteResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("episodeProgressInfo"))
			{
				object rawEpisodeProgressInfo = dictionary["episodeProgressInfo"];
				currentPlayer.UpdateEpisodeProgressInfo(rawEpisodeProgressInfo);
			}
			return this;
		}
	}
}
