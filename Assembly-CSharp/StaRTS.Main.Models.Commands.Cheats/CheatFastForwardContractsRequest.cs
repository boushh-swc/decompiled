using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatFastForwardContractsRequest : PlayerIdRequest
	{
		public double TimeOffset
		{
			get;
			private set;
		}

		public CheatFastForwardContractsRequest(double timeOffset)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.TimeOffset = timeOffset;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("offset", this.TimeOffset.ToString());
			return serializer.End().ToString();
		}
	}
}
