using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetTournamentLeaderboardPlayersCommand : SquadGameCommand<PlayerLeaderboardRequest, LeaderboardResponse>
	{
		public const string ACTION = "player.leaderboard.tournament.getLeaders";

		public GetTournamentLeaderboardPlayersCommand(PlayerLeaderboardRequest request) : base("player.leaderboard.tournament.getLeaders", request, new LeaderboardResponse())
		{
		}
	}
}
