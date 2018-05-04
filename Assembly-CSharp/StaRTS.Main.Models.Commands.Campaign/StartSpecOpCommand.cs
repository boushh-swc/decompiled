using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Campaign
{
	public class StartSpecOpCommand : GameActionCommand<CampaignIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.missions.startSpecop";

		public StartSpecOpCommand(CampaignIdRequest request) : base("player.missions.startSpecop", request, new DefaultResponse())
		{
		}
	}
}
