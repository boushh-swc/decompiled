using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Test.Config
{
	public class AuthTokenIsPlayerIdResponse : AbstractResponse
	{
		public bool AuthIsPlayerId
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.AuthIsPlayerId = (bool)obj;
			Service.Logger.Debug("AuthTokenIsPlayerId returned " + this.AuthIsPlayerId);
			return this;
		}
	}
}
