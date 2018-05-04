using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetObjectivesRequest : PlayerIdRequest
	{
		private List<string> uids;

		public CheatSetObjectivesRequest(List<string> uids)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.uids = uids;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			int num = Mathf.Min(this.uids.Count, 3);
			for (int i = 0; i < num; i++)
			{
				serializer.AddArrayOfPrimitives<string>("uids", this.uids);
			}
			return serializer.End().ToString();
		}
	}
}
