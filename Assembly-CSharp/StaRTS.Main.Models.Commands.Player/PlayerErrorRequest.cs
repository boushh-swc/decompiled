using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class PlayerErrorRequest : PlayerIdRequest
	{
		public string Prefix
		{
			get;
			set;
		}

		public string ClientCheckSumString
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("prefix", this.Prefix);
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("clientState", this.ClientCheckSumString);
			return serializer.End().ToString();
		}
	}
}
