using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeDefaultResponse : AbstractResponse
	{
		public object responseData;

		public override ISerializable FromObject(object obj)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.responseData = obj;
			currentPlayer.UpdateEpisodeProgressInfo(obj);
			Service.EpisodeController.ForceRefreshState();
			return this;
		}
	}
}
