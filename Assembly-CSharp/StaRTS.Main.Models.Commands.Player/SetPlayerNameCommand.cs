using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SetPlayerNameCommand : GameCommand<SetPlayerNameRequest, DefaultResponse>
	{
		public const string ACTION = "player.name.set";

		public SetPlayerNameCommand(string name) : base("player.name.set", new SetPlayerNameRequest(name), new DefaultResponse())
		{
		}
	}
}
