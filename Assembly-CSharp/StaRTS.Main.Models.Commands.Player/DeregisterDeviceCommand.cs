using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class DeregisterDeviceCommand : GameCommand<DeregisterDeviceRequest, DefaultResponse>
	{
		public const string ACTION = "player.device.deregister";

		public DeregisterDeviceCommand(DeregisterDeviceRequest request) : base("player.device.deregister", request, new DefaultResponse())
		{
		}
	}
}
