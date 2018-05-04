using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SetFactionCommand : GameCommand<SetFactionRequest, DefaultResponse>
	{
		public const string ACTION = "player.faction.set";

		public SetFactionCommand(SetFactionRequest request) : base("player.faction.set", request, new DefaultResponse())
		{
		}
	}
}
