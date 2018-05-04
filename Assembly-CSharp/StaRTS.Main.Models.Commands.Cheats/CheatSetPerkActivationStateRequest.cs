using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetPerkActivationStateRequest : PlayerIdRequest
	{
		private string perkUid;

		private bool deactivate;

		public CheatSetPerkActivationStateRequest(string perkUid, bool deactivate)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.perkUid = perkUid;
			this.deactivate = deactivate;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("perkId", this.perkUid);
			serializer.AddBool("deactivate", this.deactivate);
			return serializer.End().ToString();
		}
	}
}
