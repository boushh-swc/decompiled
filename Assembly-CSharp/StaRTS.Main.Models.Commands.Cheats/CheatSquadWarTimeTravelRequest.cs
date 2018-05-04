using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSquadWarTimeTravelRequest : PlayerIdRequest
	{
		public string WarId
		{
			get;
			private set;
		}

		public double TimeOffset
		{
			get;
			private set;
		}

		public CheatSquadWarTimeTravelRequest(string warId, double timeOffset)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.WarId = warId;
			this.TimeOffset = timeOffset;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("warId", this.WarId);
			serializer.AddString("offset", this.TimeOffset.ToString());
			return serializer.End().ToString();
		}
	}
}
