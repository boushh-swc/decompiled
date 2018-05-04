using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetLeaderboardSquadsCommand : SquadGameCommand<PlayerIdRequest, LeaderboardResponse>
	{
		public const string ACTION = "guild.leaderboard.getLeaders";

		public GetLeaderboardSquadsCommand(PlayerIdRequest request) : base("guild.leaderboard.getLeaders", request, new LeaderboardResponse())
		{
		}
	}
}
