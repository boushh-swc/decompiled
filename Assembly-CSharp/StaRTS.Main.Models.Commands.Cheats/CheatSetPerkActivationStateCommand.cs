using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetPerkActivationStateCommand : GameCommand<CheatSetPerkActivationStateRequest, CheatSetPerkActivationStateResponse>
	{
		public const string ACTION = "cheats.perks.setPerkActivationState";

		public CheatSetPerkActivationStateCommand(CheatSetPerkActivationStateRequest request) : base("cheats.perks.setPerkActivationState", request, new CheatSetPerkActivationStateResponse())
		{
		}
	}
}
