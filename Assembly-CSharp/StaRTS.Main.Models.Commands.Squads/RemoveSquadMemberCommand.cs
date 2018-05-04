using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class RemoveSquadMemberCommand : SquadGameCommand<MemberIdRequest, DefaultResponse>
	{
		public const string ACTION = "guild.eject";

		public RemoveSquadMemberCommand(MemberIdRequest request) : base("guild.eject", request, new DefaultResponse())
		{
		}
	}
}
