using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarCancelMatchmakingCommand : SquadGameCommand<PlayerIdChecksumRequest, DefaultResponse>
	{
		public const string ACTION = "guild.war.matchmaking.cancel";

		public SquadWarCancelMatchmakingCommand(PlayerIdChecksumRequest request) : base("guild.war.matchmaking.cancel", request, new DefaultResponse())
		{
		}
	}
}
