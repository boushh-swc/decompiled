using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player
{
	public class RegisterDeviceCommand : GameCommand<RegisterDeviceRequest, RegisterDeviceResponse>
	{
		public const string ACTION = "player.device.register";

		public RegisterDeviceCommand(RegisterDeviceRequest request) : base("player.device.register", request, new RegisterDeviceResponse())
		{
		}

		public override void OnSuccess()
		{
			PlayerResourceResponse responseResult = base.ResponseResult;
			if (responseResult != null)
			{
				List<ICommand> queue = Service.ServerAPI.GetQueue();
				int i = 0;
				int count = queue.Count;
				while (i < count)
				{
					PlayerIdChecksumRequest playerIdChecksumRequest = queue[i].Request as PlayerIdChecksumRequest;
					if (playerIdChecksumRequest != null)
					{
						playerIdChecksumRequest.RecalculateChecksumAfterResourceChange(responseResult.CrystalsDelta);
					}
					i++;
				}
			}
		}
	}
}
