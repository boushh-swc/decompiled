using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarGetBuffBaseStatusCommand : SquadGameCommand<SquadWarGetBuffBaseStatusRequest, SquadWarBuffBaseResponse>
	{
		public const string ACTION = "guild.war.getBaseStatus";

		public SquadWarGetBuffBaseStatusCommand(SquadWarGetBuffBaseStatusRequest request) : base("guild.war.getBaseStatus", request, new SquadWarBuffBaseResponse())
		{
		}
	}
}
