using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class RedeemTournamentRewardRequest : PlayerIdRequest
	{
		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			return serializer.End().ToString();
		}
	}
}
