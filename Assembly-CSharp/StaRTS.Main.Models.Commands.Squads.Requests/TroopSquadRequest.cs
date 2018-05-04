using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class TroopSquadRequest : PlayerIdRequest
	{
		public bool PayToSkip
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddBool("payToSkip", this.PayToSkip);
			serializer.AddString("message", this.Message);
			return serializer.End().ToString();
		}
	}
}
