using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatFastForwardContractsCommand : GameCommand<CheatFastForwardContractsRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.contracts.timeTravel";

		public CheatFastForwardContractsCommand(CheatFastForwardContractsRequest request) : base("cheats.contracts.timeTravel", request, new DefaultResponse())
		{
		}
	}
}
