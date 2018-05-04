using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Identity
{
	public class PlayerIdentitySwitchResponse : AbstractResponse
	{
		public string PlayerId
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.PlayerId = (obj as string);
			return this;
		}
	}
}
