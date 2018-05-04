using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace Source.StaRTS.Main.Models.Commands.Player
{
	internal class GetAuthTokenRequest : PlayerIdRequest
	{
		public string RequestToken
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("requestToken", this.RequestToken);
			return serializer.End().ToString();
		}
	}
}
