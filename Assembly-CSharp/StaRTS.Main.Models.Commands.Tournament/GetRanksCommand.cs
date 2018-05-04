using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class GetRanksCommand : GameCommand<PlayerIdRequest, ConflictRanks>
	{
		public const string ACTION = "player.leaderboard.tournament.getRanks";

		public GetRanksCommand(PlayerIdRequest request) : base("player.leaderboard.tournament.getRanks", request, new ConflictRanks())
		{
		}
	}
}
