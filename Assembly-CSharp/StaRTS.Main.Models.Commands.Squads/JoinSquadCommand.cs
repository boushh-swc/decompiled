using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class JoinSquadCommand : SquadGameCommand<SquadIDRequest, SquadResponse>
	{
		public const string ACTION = "guild.join";

		public JoinSquadCommand(SquadIDRequest request) : base("guild.join", request, new SquadResponse())
		{
		}
	}
}
