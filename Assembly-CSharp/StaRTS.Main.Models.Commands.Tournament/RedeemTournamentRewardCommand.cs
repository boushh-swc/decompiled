using System;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class RedeemTournamentRewardCommand : GameActionCommand<PlayerIdChecksumRequest, TournamentResponse>
	{
		public const string ACTION = "player.store.tournament.redeem";

		public RedeemTournamentRewardCommand(PlayerIdChecksumRequest request) : base("player.store.tournament.redeem", request, new TournamentResponse())
		{
		}
	}
}
