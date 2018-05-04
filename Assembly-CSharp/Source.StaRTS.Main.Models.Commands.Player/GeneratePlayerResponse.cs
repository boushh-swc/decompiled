using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace Source.StaRTS.Main.Models.Commands.Player
{
	public class GeneratePlayerResponse : AbstractResponse
	{
		public string PlayerId
		{
			get;
			private set;
		}

		public string Secret
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.PlayerId = (string)dictionary["playerId"];
			this.Secret = (string)dictionary["secret"];
			return this;
		}
	}
}
