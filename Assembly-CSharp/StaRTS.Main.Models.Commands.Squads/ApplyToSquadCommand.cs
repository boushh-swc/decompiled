using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class ApplyToSquadCommand : SquadGameCommand<ApplyToSquadRequest, DefaultResponse>
	{
		public const string ACTION = "guild.join.request";

		public ApplyToSquadCommand(ApplyToSquadRequest request) : base("guild.join.request", request, new DefaultResponse())
		{
		}
	}
}
