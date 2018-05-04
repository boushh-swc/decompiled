using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetLeaderboardPlayersCommand : SquadGameCommand<PlayerLeaderboardRequest, LeaderboardResponse>
	{
		public const string ACTION = "player.leaderboard.getLeaders";

		public GetLeaderboardPlayersCommand(PlayerLeaderboardRequest request) : base("player.leaderboard.getLeaders", request, new LeaderboardResponse())
		{
		}
	}
}
