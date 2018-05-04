using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Identity
{
	public class PlayerIdentityRequest : PlayerIdChecksumRequest
	{
		public int IdentityIndex
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.Add<int>("identityIndex", this.IdentityIndex);
			return startedSerializer.End().ToString();
		}
	}
}
