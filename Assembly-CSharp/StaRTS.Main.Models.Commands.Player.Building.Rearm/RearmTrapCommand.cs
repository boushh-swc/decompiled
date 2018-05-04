using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Rearm
{
	public class RearmTrapCommand : GameActionCommand<RearmTrapRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.rearm";

		public RearmTrapCommand(RearmTrapRequest request) : base("player.building.rearm", request, new DefaultResponse())
		{
		}
	}
}
