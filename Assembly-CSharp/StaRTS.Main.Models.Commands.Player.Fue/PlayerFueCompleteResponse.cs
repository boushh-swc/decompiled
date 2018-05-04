using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Fue
{
	public class PlayerFueCompleteResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			Dictionary<string, object> dictionary2 = dictionary["playerModel"] as Dictionary<string, object>;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (dictionary2.ContainsKey("episodeProgressInfo"))
			{
				currentPlayer.UpdateEpisodeProgressInfoNoRefreshEvent(dictionary2["episodeProgressInfo"]);
				Service.EventManager.SendEvent(EventId.FueCompleted, null);
			}
			return this;
		}
	}
}
