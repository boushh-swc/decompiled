using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarTroopDonateCommand : SquadGameCommand<TroopDonateRequest, TroopDonateResponse>
	{
		public const string ACTION = "guild.war.troops.donate";

		public SquadWarTroopDonateCommand(TroopDonateRequest request) : base("guild.war.troops.donate", request, new TroopDonateResponse())
		{
		}
	}
}
