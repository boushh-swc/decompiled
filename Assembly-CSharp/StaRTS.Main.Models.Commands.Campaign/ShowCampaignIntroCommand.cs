using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Campaign
{
	public class ShowCampaignIntroCommand : GameActionCommand<CampaignIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.missions.showIntro";

		public ShowCampaignIntroCommand(CampaignIdRequest request) : base("player.missions.showIntro", request, new DefaultResponse())
		{
		}
	}
}
