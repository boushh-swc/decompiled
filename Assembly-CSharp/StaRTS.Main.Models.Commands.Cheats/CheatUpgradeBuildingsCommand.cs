using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatUpgradeBuildingsCommand : GameCommand<CheatUpgradeBuildingsRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.building.upgrade";

		public CheatUpgradeBuildingsCommand(CheatUpgradeBuildingsRequest request) : base("cheats.building.upgrade", request, new DefaultResponse())
		{
		}
	}
}
