using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class GetSquadInvitesSentRequest : PlayerIdRequest
	{
		private List<string> friendIds;

		public GetSquadInvitesSentRequest(List<string> ids)
		{
			this.friendIds = ids;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddArrayOfPrimitives<string>("friendIds", this.friendIds);
			return serializer.End().ToString();
		}
	}
}
