using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarTroopRequestCommand : SquadGameCommand<TroopSquadRequest, DefaultResponse>
	{
		public const string ACTION = "guild.war.troops.request";

		public SquadWarTroopRequestCommand(TroopSquadRequest request) : base("guild.war.troops.request", request, new DefaultResponse())
		{
		}
	}
}
