using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class CheateAddCrateCommand : GameCommand<CheatAddCrateRequest, CheatAddCrateResponse>
	{
		public const string ACTION = "cheats.crate.add";

		public CheateAddCrateCommand(CheatAddCrateRequest request) : base("cheats.crate.add", request, new CheatAddCrateResponse())
		{
		}
	}
}
