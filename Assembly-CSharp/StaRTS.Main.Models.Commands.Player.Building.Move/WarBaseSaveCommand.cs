using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Move
{
	public class WarBaseSaveCommand : SquadGameCommand<WarBaseSaveRequest, DefaultResponse>
	{
		public const string ACTION = "guild.war.base.save";

		public WarBaseSaveCommand(WarBaseSaveRequest request) : base("guild.war.base.save", request, new DefaultResponse())
		{
		}
	}
}
