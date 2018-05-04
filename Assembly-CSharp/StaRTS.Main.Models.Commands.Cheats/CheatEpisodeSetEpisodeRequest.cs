using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetEpisodeRequest : PlayerIdRequest
	{
		private string episodeId;

		public CheatEpisodeSetEpisodeRequest(string episodeId)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.episodeId = episodeId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("currentEpisodeUid", this.episodeId);
			return serializer.End().ToString();
		}
	}
}
