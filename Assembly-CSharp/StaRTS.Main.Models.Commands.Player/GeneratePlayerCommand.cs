using Source.StaRTS.Main.Models.Commands.Player;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class GeneratePlayerCommand : GameCommand<GeneratePlayerRequest, GeneratePlayerResponse>
	{
		private const string ACTION = "auth.preauth.generatePlayer";

		public GeneratePlayerCommand(GeneratePlayerRequest request) : base("auth.preauth.generatePlayer", request, new GeneratePlayerResponse())
		{
		}
	}
}
