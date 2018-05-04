using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatClearMobileConnectorAdsCommand : GameCommand<PlayerIdRequest, CheatClearMobileConnectorAdsResponse>
	{
		public const string ACTION = "cheats.mca.records.clear";

		public CheatClearMobileConnectorAdsCommand(PlayerIdRequest request) : base("cheats.mca.records.clear", request, new CheatClearMobileConnectorAdsResponse())
		{
		}
	}
}
