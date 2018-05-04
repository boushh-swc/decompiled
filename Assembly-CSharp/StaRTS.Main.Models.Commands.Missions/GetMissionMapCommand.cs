using System;

namespace StaRTS.Main.Models.Commands.Missions
{
	public class GetMissionMapCommand : GameActionCommand<MissionIdRequest, GetMissionMapResponse>
	{
		public const string ACTION = "player.missions.missionMap";

		public GetMissionMapCommand(MissionIdRequest request) : base("player.missions.missionMap", request, new GetMissionMapResponse())
		{
		}
	}
}
