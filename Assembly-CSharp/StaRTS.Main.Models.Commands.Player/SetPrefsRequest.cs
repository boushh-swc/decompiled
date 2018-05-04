using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SetPrefsRequest : PlayerIdRequest
	{
		public SetPrefsRequest()
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("value", Service.ServerPlayerPrefs.Serialize());
			return serializer.End().ToString();
		}
	}
}
