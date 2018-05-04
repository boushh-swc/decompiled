using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class DemoteSquadMemberCommand : SquadGameCommand<MemberIdRequest, DefaultResponse>
	{
		public const string ACTION = "guild.demote";

		public DemoteSquadMemberCommand(MemberIdRequest request) : base("guild.demote", request, new DefaultResponse())
		{
		}
	}
}
