using StaRTS.Main.Models.Commands.Missions;
using System;

namespace StaRTS.Main.Models.Commands.Pve
{
	public class PveMissionStartCommand : GameActionCommand<MissionIdRequest, BattleIdResponse>
	{
		public const string ACTION = "player.pve.start";

		public PveMissionStartCommand(MissionIdRequest request) : base("player.pve.start", request, new BattleIdResponse())
		{
		}
	}
}
