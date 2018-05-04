using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class OpenCrateCommand : GameActionCommand<OpenCrateRequest, OpenCrateResponse>
	{
		public const string ACTION = "player.crate.open";

		public OpenCrateCommand(OpenCrateRequest request) : base("player.crate.open", request, new OpenCrateResponse())
		{
		}
	}
}
