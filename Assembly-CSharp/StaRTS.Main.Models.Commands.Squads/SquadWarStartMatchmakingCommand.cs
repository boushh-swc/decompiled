using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarStartMatchmakingCommand : SquadGameCommand<SquadWarStartMatchmakingRequest, DefaultResponse>
	{
		public const string ACTION = "guild.war.matchmaking.start";

		public SquadWarStartMatchmakingCommand(SquadWarStartMatchmakingRequest request) : base("guild.war.matchmaking.start", request, new DefaultResponse())
		{
		}
	}
}
