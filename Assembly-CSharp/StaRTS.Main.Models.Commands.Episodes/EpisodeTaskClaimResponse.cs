using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskClaimResponse : AbstractResponse
	{
		public CrateData CrateDataTO
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("episodeProgressInfo"))
			{
				object rawEpisodeProgressInfo = dictionary["episodeProgressInfo"];
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				currentPlayer.UpdateEpisodeProgressInfo(rawEpisodeProgressInfo);
			}
			if (dictionary.ContainsKey("crateData"))
			{
				object obj2 = dictionary["crateData"];
				this.CrateDataTO = new CrateData();
				this.CrateDataTO.FromObject(obj2);
			}
			Service.EpisodeController.ForceRefreshState();
			return this;
		}
	}
}
