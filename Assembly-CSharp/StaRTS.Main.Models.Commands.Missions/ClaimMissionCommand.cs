using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Missions
{
	public class ClaimMissionCommand : GameActionCommand<MissionIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.missions.claimMission";

		public ClaimMissionCommand(MissionIdRequest request) : base("player.missions.claimMission", request, new DefaultResponse())
		{
		}
	}
}
