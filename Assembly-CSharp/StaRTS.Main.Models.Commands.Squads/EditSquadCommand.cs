using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class EditSquadCommand : SquadGameCommand<EditSquadRequest, DefaultResponse>
	{
		public const string ACTION = "guild.edit";

		public EditSquadCommand(EditSquadRequest request) : base("guild.edit", request, new DefaultResponse())
		{
		}
	}
}
