using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetTaskActionRequest : PlayerIdRequest
	{
		private string actionId;

		public CheatEpisodeSetTaskActionRequest(string actionId)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.actionId = actionId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("currentTaskActionUid", this.actionId);
			return serializer.End().ToString();
		}
	}
}
