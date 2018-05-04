using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class GetPlayerPvpStatusResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			return this;
		}
	}
}
