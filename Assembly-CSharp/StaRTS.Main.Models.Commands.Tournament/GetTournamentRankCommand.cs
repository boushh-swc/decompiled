using System;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class GetTournamentRankCommand : GameCommand<TournamentRankRequest, TournamentRankResponse>
	{
		public const string ACTION = "player.leaderboard.tournament.getRank";

		public GetTournamentRankCommand(TournamentRankRequest request) : base("player.leaderboard.tournament.getRank", request, new TournamentRankResponse())
		{
		}
	}
}
