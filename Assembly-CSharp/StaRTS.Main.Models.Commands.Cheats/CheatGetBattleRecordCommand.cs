using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatGetBattleRecordCommand : GameCommand<CheatGetBattleRecordRequest, CheatGetBattleRecordResponse>
	{
		public const string ACTION = "cheats.getBattleRecord";

		public CheatGetBattleRecordCommand(string battleId) : base("cheats.getBattleRecord", new CheatGetBattleRecordRequest(battleId), new CheatGetBattleRecordResponse(battleId))
		{
		}
	}
}
