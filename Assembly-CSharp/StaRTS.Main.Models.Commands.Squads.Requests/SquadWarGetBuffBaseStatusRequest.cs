using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class SquadWarGetBuffBaseStatusRequest : PlayerIdRequest
	{
		private string buffBaseUID;

		public SquadWarGetBuffBaseStatusRequest(string playerId, string buffBaseId)
		{
			base.PlayerId = playerId;
			this.buffBaseUID = buffBaseId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("buffBaseUid", this.buffBaseUID);
			return serializer.End().ToString();
		}
	}
}
