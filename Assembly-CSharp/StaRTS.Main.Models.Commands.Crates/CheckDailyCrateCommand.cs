using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class CheckDailyCrateCommand : GameCommand<CheckDailyCrateRequest, CheckDailyCrateResponse>
	{
		public const string ACTION = "player.crate.checkDaily";

		public CheckDailyCrateCommand(CheckDailyCrateRequest request) : base("player.crate.checkDaily", request, new CheckDailyCrateResponse())
		{
		}
	}
}
