using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadTroopRequestCommand : SquadGameCommand<TroopSquadRequest, DefaultResponse>
	{
		public const string ACTION = "guild.troops.request";

		public SquadTroopRequestCommand(TroopSquadRequest request) : base("guild.troops.request", request, new DefaultResponse())
		{
		}
	}
}
