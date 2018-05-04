using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Campaign
{
	public class CampaignStoreBuyCommand : GameActionCommand<CampaignStoreBuyRequest, DefaultResponse>
	{
		public const string ACTION = "player.campaignStore.buy";

		public CampaignStoreBuyCommand(CampaignStoreBuyRequest request) : base("player.campaignStore.buy", request, new DefaultResponse())
		{
		}
	}
}
