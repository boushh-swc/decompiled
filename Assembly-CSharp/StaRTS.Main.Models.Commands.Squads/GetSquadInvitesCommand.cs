using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadInvitesCommand : SquadGameCommand<PlayerIdRequest, GetSquadInvitesResponse>
	{
		public const string ACTION = "guild.invite.get";

		public GetSquadInvitesCommand(PlayerIdRequest request) : base("guild.invite.get", request, new GetSquadInvitesResponse())
		{
		}
	}
}
