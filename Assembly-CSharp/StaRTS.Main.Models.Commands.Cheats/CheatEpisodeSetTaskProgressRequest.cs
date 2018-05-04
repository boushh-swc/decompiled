using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetTaskProgressRequest : PlayerIdRequest
	{
		private int count;

		public CheatEpisodeSetTaskProgressRequest(int count)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.count = count;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<int>("count", this.count);
			return serializer.End().ToString();
		}
	}
}
