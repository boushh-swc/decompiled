using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.MobileConnectorAds
{
	public class ClaimMobileConnectorAdsRewardCommand : GameCommand<PlayerIdRequest, ClaimMobileConnectorAdsRewardResponse>
	{
		public const string ACTION = "player.mca.claim";

		public ClaimMobileConnectorAdsRewardCommand(PlayerIdRequest request) : base("player.mca.claim", request, new ClaimMobileConnectorAdsRewardResponse())
		{
		}
	}
}
