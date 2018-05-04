using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetPublicSquadCommand : SquadGameCommand<SquadIDRequest, SquadResponse>
	{
		public const string ACTION = "guild.get.public";

		public GetPublicSquadCommand(SquadIDRequest request) : base("guild.get.public", request, new SquadResponse())
		{
		}
	}
}
