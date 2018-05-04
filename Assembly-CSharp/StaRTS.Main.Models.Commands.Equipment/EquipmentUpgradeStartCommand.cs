using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Equipment
{
	public class EquipmentUpgradeStartCommand : GameActionCommand<EquipmentUpgradeStartRequest, DefaultResponse>
	{
		public const string ACTION = "player.equipment.upgrade.start";

		public EquipmentUpgradeStartCommand(EquipmentUpgradeStartRequest request) : base("player.equipment.upgrade.start", request, new DefaultResponse())
		{
		}
	}
}
