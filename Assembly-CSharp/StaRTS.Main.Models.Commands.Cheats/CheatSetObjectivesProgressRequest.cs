using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetObjectivesProgressRequest : PlayerIdRequest
	{
		private string uid;

		private int count;

		public CheatSetObjectivesProgressRequest(string uid, int count)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.uid = uid;
			this.count = count;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("uid", this.uid);
			serializer.Add<int>("count", this.count);
			return serializer.End().ToString();
		}
	}
}
