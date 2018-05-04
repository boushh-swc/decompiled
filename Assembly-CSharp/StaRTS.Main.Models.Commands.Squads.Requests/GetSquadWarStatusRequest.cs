using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class GetSquadWarStatusRequest : PlayerIdRequest
	{
		public string WarId
		{
			get;
			set;
		}

		public GetSquadWarStatusRequest(string playerId, string warId)
		{
			base.PlayerId = playerId;
			this.WarId = warId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("warId", this.WarId);
			return serializer.End().ToString();
		}
	}
}
