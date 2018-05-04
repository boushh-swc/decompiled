using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Account.External
{
	public class UnregisterExternalAccountRequest : AbstractRequest
	{
		public AccountProvider Provider
		{
			get;
			set;
		}

		public string PlayerId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", this.PlayerId);
			serializer.AddString("providerId", this.Provider.ToString());
			return serializer.End().ToString();
		}
	}
}
