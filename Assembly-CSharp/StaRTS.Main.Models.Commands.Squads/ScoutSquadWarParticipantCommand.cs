using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class ScoutSquadWarParticipantCommand : SquadGameActionCommand<SquadWarParticipantIdRequest, SquadMemberWarDataResponse>
	{
		public const string ACTION = "guild.war.scoutPlayer";

		public ScoutSquadWarParticipantCommand(SquadWarParticipantIdRequest request) : base("guild.war.scoutPlayer", request, new SquadMemberWarDataResponse())
		{
		}
	}
}
