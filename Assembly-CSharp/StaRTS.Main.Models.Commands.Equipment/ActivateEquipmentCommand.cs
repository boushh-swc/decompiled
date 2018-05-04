using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Equipment
{
	public class ActivateEquipmentCommand : GameActionCommand<EquipmentIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.equipment.activate";

		public ActivateEquipmentCommand(EquipmentIdRequest request) : base("player.equipment.activate", request, new DefaultResponse())
		{
		}
	}
}
