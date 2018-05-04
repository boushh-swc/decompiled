using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Campaign
{
	public class ClaimCampaignCommand : GameActionCommand<ClaimCampaignRequest, DefaultResponse>
	{
		public const string ACTION = "player.missions.claimCampaign";

		public ClaimCampaignCommand(ClaimCampaignRequest request) : base("player.missions.claimCampaign", request, new DefaultResponse())
		{
		}
	}
}
