using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatDeployableUpgradeRequest : PlayerIdRequest
	{
		private List<string> uids;

		public CheatDeployableUpgradeRequest(List<string> uids)
		{
			this.uids = uids;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddArrayOfPrimitives<string>("uids", this.uids);
			return serializer.End().ToString();
		}
	}
}
