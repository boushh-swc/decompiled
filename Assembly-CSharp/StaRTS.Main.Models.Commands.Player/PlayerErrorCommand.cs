using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class PlayerErrorCommand : GameCommand<PlayerErrorRequest, DefaultResponse>
	{
		public const string ACTION = "player.error";

		public PlayerErrorCommand(PlayerErrorRequest request) : base("player.error", request, new DefaultResponse())
		{
		}
	}
}
