using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetTroopDonateRepCommand : GameCommand<CheatSetTroopDonateRepRequest, CheatSetTroopDonateRepResponse>
	{
		private const string ACTION = "cheats.setTroopDonationRep";

		public CheatSetTroopDonateRepCommand(CheatSetTroopDonateRepRequest request) : base("cheats.setTroopDonationRep", request, new CheatSetTroopDonateRepResponse())
		{
		}
	}
}
