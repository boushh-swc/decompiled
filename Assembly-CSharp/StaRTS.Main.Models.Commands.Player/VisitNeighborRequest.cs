using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class VisitNeighborRequest : AbstractRequest
	{
		private string neighborId;

		private string playerId;

		public VisitNeighborRequest(string neighborId)
		{
			this.neighborId = neighborId;
			this.playerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", this.playerId);
			serializer.AddString("neighborId", this.neighborId);
			return serializer.End().ToString();
		}
	}
}
