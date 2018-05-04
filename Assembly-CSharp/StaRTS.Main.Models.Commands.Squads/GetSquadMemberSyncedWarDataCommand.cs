using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadMemberSyncedWarDataCommand : SquadGameActionCommand<PlayerIdChecksumRequest, SquadMemberWarDataResponse>
	{
		public const string ACTION = "guild.war.getSyncedParticipant";

		public GetSquadMemberSyncedWarDataCommand(PlayerIdChecksumRequest request) : base("guild.war.getSyncedParticipant", request, new SquadMemberWarDataResponse())
		{
		}
	}
}
