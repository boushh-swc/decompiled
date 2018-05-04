using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadInvitesSentCommand : SquadGameCommand<GetSquadInvitesSentRequest, GetSquadInvitesSentResponse>
	{
		public const string ACTION = "guild.invite.sent";

		public GetSquadInvitesSentCommand(GetSquadInvitesSentRequest request) : base("guild.invite.sent", request, new GetSquadInvitesSentResponse())
		{
		}
	}
}
