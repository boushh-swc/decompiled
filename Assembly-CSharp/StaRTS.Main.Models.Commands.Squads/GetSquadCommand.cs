using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadCommand : SquadGameCommand<PlayerIdRequest, SquadResponse>
	{
		public const string ACTION = "guild.get";

		public GetSquadCommand(PlayerIdRequest request) : base("guild.get", request, new SquadResponse())
		{
		}
	}
}
