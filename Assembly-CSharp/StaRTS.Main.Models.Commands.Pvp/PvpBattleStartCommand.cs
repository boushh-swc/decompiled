using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpBattleStartCommand : GameActionCommand<PvpBattleStartRequest, DefaultResponse>
	{
		public const string ACTION = "player.pvp.battle.start";

		public PvpBattleStartCommand(PvpBattleStartRequest request) : base("player.pvp.battle.start", request, new DefaultResponse())
		{
		}
	}
}
