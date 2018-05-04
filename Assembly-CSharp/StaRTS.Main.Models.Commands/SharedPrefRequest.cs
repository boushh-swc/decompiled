using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands
{
	public class SharedPrefRequest : PlayerIdRequest
	{
		private string key;

		private string value;

		public SharedPrefRequest(string key, string value)
		{
			this.key = key;
			this.value = value;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", Service.CurrentPlayer.PlayerId);
			serializer.AddDictionary<string>("sharedPrefs", new Dictionary<string, string>
			{
				{
					this.key,
					this.value
				}
			});
			return serializer.End().ToString();
		}
	}
}
