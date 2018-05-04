using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeTaskTimerSetRequest : PlayerIdRequest
	{
		private long endTimeSecs;

		public CheatEpisodeTaskTimerSetRequest(long endTimeSecs)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.endTimeSecs = endTimeSecs;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<long>("endTimeSecs", this.endTimeSecs);
			return serializer.End().ToString();
		}
	}
}
