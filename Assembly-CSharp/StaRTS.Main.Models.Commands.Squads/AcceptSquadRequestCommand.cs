using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class AcceptSquadRequestCommand : SquadGameCommand<MemberIdRequest, SquadMemberResponse>
	{
		public const string ACTION = "guild.join.accept";

		public AcceptSquadRequestCommand(MemberIdRequest request) : base("guild.join.accept", request, new SquadMemberResponse())
		{
		}
	}
}
