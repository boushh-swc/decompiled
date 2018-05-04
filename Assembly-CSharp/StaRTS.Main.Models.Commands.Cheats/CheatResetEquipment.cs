using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatResetEquipment : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.equipment.reset";

		public CheatResetEquipment(PlayerIdRequest request) : base("cheats.equipment.reset", request, new DefaultResponse())
		{
		}
	}
}
