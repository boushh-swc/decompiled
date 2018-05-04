using System;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpBattleEndCommand : GameActionCommand<BattleEndRequest, PvpBattleEndResponse>
	{
		public const string ACTION = "player.pvp.battle.complete";

		public PvpBattleEndCommand(BattleEndRequest request) : base("player.pvp.battle.complete", request, new PvpBattleEndResponse())
		{
		}
	}
}
