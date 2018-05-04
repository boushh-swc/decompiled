using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Missions;
using System;

namespace StaRTS.Main.Models.Commands.Pve
{
	public class PveMissionCollectCommand : GameActionCommand<MissionIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.pve.collect";

		public PveMissionCollectCommand(MissionIdRequest request) : base("player.pve.collect", request, new DefaultResponse())
		{
		}
	}
}
