using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadWarStatusCommand : SquadGameCommand<GetSquadWarStatusRequest, GetSquadWarStatusResponse>
	{
		public const string ACTION = "guild.war.status";

		public GetSquadWarStatusCommand(GetSquadWarStatusRequest request) : base("guild.war.status", request, new GetSquadWarStatusResponse())
		{
		}
	}
}
