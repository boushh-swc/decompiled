using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Missions
{
	public class ActivateMissionCommand : GameActionCommand<MissionIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.missions.activateMission";

		public ActivateMissionCommand(MissionIdRequest request) : base("player.missions.activateMission", request, new DefaultResponse())
		{
		}
	}
}
