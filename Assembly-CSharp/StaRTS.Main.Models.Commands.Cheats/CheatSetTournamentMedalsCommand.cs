using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetTournamentMedalsCommand : GameCommand<CheatPointsRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.setTournamentMedal";

		public CheatSetTournamentMedalsCommand(CheatPointsRequest request) : base("cheats.setTournamentMedal", request, new DefaultResponse())
		{
		}
	}
}
