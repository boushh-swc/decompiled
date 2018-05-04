using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class RejectSquadRequestCommand : SquadGameCommand<MemberIdRequest, DefaultResponse>
	{
		public const string ACTION = "guild.join.reject";

		public RejectSquadRequestCommand(MemberIdRequest request) : base("guild.join.reject", request, new DefaultResponse())
		{
		}
	}
}
