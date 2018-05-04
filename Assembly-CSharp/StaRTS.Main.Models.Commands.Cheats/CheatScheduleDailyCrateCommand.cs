using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatScheduleDailyCrateCommand : GameCommand<CheatScheduleDailyCrateRequest, CheatScheduleDailyCrateResponse>
	{
		public const string ACTION = "cheats.crate.daily.schedule";

		public CheatScheduleDailyCrateCommand(CheatScheduleDailyCrateRequest request) : base("cheats.crate.daily.schedule", request, new CheatScheduleDailyCrateResponse())
		{
		}
	}
}
