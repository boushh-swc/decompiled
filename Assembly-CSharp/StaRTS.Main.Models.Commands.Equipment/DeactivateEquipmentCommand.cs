using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Equipment
{
	public class DeactivateEquipmentCommand : GameActionCommand<EquipmentIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.equipment.deactivate";

		public DeactivateEquipmentCommand(EquipmentIdRequest request) : base("player.equipment.deactivate", request, new DefaultResponse())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			if (status == 2604u)
			{
				return base.EatFailure(status, data);
			}
			return base.OnFailure(status, data);
		}
	}
}
