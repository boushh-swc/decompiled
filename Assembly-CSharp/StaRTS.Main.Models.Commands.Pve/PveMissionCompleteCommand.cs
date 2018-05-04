using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Pve
{
	public class PveMissionCompleteCommand : GameActionCommand<BattleEndRequest, DefaultResponse>
	{
		public const string ACTION = "player.pve.complete";

		public PveMissionCompleteCommand(BattleEndRequest request) : base("player.pve.complete", request, new DefaultResponse())
		{
		}
	}
}
