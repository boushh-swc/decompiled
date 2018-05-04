using StaRTS.Main.Models.Commands.Campaign;
using System;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class TournamentStoreBuyCommand : GameCommand<CampaignStoreBuyRequest, TournamentResponse>
	{
		public const string ACTION = "player.store.tournament.buy";

		public TournamentStoreBuyCommand(CampaignStoreBuyRequest request) : base("player.store.tournament.buy", request, new TournamentResponse())
		{
		}
	}
}
