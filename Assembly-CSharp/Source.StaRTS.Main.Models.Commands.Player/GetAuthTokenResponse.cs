using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;

namespace Source.StaRTS.Main.Models.Commands.Player
{
	internal class GetAuthTokenResponse : AbstractResponse
	{
		public string AuthToken
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.AuthToken = (obj as string);
			return this;
		}
	}
}
