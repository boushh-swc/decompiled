using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadMemberWarDataCommand : SquadGameCommand<PlayerIdRequest, SquadMemberWarDataResponse>
	{
		public const string ACTION = "guild.war.getParticipant";

		public GetSquadMemberWarDataCommand(PlayerIdRequest request) : base("guild.war.getParticipant", request, new SquadMemberWarDataResponse())
		{
		}
	}
}
