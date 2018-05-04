using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetEquipmentCommand : GameActionCommand<CheatSetEquipmentRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.equipment.set";

		public CheatSetEquipmentCommand(CheatSetEquipmentRequest request) : base("cheats.equipment.set", request, new DefaultResponse())
		{
		}
	}
}
