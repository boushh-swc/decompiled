using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatDeployablesRequest : PlayerIdRequest
	{
		private Dictionary<string, int> deployables;

		public CheatDeployablesRequest(Dictionary<string, int> deployables)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.deployables = deployables;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddDictionary<int>("amount", this.deployables);
			return serializer.End().ToString();
		}
	}
}
