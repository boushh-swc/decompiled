using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class PromoteSquadMemberCommand : SquadGameCommand<MemberIdRequest, DefaultResponse>
	{
		public const string ACTION = "guild.promote";

		public PromoteSquadMemberCommand(MemberIdRequest request) : base("guild.promote", request, new DefaultResponse())
		{
		}
	}
}
