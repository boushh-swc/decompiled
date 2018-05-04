using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class VisitNeighborCommand : GameCommand<VisitNeighborRequest, VisitNeighborResponse>
	{
		public const string ACTION = "player.neighbor.visit";

		public VisitNeighborCommand(VisitNeighborRequest request) : base("player.neighbor.visit", request, new VisitNeighborResponse())
		{
		}
	}
}
