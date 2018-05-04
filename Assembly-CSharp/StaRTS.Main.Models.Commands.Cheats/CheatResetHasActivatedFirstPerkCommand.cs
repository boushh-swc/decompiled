using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatResetHasActivatedFirstPerkCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.perks.resetHasActivatedFirstPerk";

		public CheatResetHasActivatedFirstPerkCommand(PlayerIdRequest request) : base("cheats.perks.resetHasActivatedFirstPerk", request, new DefaultResponse())
		{
		}
	}
}
