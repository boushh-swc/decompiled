using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSaveBattleRecordCommand : GameCommand<CheatSaveBattleRecordRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.saveBattleRecord";

		public CheatSaveBattleRecordCommand(CheatSaveBattleRecordRequest request) : base("cheats.saveBattleRecord", request, new DefaultResponse())
		{
		}
	}
}
