using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetCampaignPointsCommand : GameCommand<CheatPointsRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.setCampaignPoints";

		public CheatSetCampaignPointsCommand(CheatPointsRequest request) : base("cheats.setCampaignPoints", request, new DefaultResponse())
		{
		}
	}
}
