using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetTaskRequest : PlayerIdRequest
	{
		private string taskId;

		public CheatEpisodeSetTaskRequest(string taskId)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.taskId = taskId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("currentTaskUid", this.taskId);
			return serializer.End().ToString();
		}
	}
}
