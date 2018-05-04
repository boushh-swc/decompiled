using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Identity
{
	public class PlayerIdentityGetResponse : AbstractResponse
	{
		public PlayerIdentityInfo Info
		{
			get;
			private set;
		}

		public PlayerIdentityGetResponse()
		{
			this.Info = new PlayerIdentityInfo();
		}

		public override ISerializable FromObject(object obj)
		{
			this.Info.FromObject(obj);
			return this;
		}
	}
}
