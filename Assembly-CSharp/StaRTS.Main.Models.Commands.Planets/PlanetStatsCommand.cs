using System;

namespace StaRTS.Main.Models.Commands.Planets
{
	public class PlanetStatsCommand : GameCommand<PlanetStatsRequest, PlanetStatsResponse>
	{
		public const string ACTION = "player.planet.stats";

		public PlanetStatsCommand(PlanetStatsRequest request) : base("player.planet.stats", request, new PlanetStatsResponse())
		{
		}
	}
}
