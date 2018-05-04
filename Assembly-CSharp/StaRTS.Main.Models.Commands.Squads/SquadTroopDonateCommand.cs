using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadTroopDonateCommand : SquadGameCommand<TroopDonateRequest, TroopDonateResponse>
	{
		public const string ACTION = "guild.troops.donate";

		public SquadTroopDonateCommand(TroopDonateRequest request) : base("guild.troops.donate", request, new TroopDonateResponse())
		{
		}
	}
}
