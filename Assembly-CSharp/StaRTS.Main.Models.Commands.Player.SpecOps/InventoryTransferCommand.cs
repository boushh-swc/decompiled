using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.SpecOps
{
	public class InventoryTransferCommand : GameActionCommand<PlayerIdChecksumRequest, DefaultResponse>
	{
		public const string ACTION = "player.inventory.transfer";

		public InventoryTransferCommand(InventoryTransferRequest request) : base("player.inventory.transfer", request, new DefaultResponse())
		{
		}
	}
}
