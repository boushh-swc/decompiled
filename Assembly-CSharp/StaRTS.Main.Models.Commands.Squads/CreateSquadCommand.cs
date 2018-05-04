using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class CreateSquadCommand : SquadGameCommand<CreateSquadRequest, SquadResponse>
	{
		public const string ACTION = "guild.create";

		public CreateSquadCommand(CreateSquadRequest request) : base("guild.create", request, new SquadResponse())
		{
		}
	}
}
