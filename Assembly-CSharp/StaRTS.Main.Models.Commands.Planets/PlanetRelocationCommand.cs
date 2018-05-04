using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Planets
{
	public class PlanetRelocationCommand : GameActionCommand<RelocatePlanetRequest, DefaultResponse>
	{
		public const string ACTION = "player.planet.relocate";

		public PlanetRelocationCommand(RelocatePlanetRequest request) : base("player.planet.relocate", request, new DefaultResponse())
		{
		}
	}
}
