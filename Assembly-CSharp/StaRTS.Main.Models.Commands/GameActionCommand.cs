using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using System;

namespace StaRTS.Main.Models.Commands
{
	public abstract class GameActionCommand<TRequest, TResponse> : GameCommand<TRequest, TResponse> where TRequest : PlayerIdChecksumRequest where TResponse : AbstractResponse
	{
		public const uint UNSYNCHRONIZED = 5000u;

		private uint status;

		public GameActionCommand(string action, TRequest request, TResponse response) : base(action, request, response)
		{
		}

		public override OnCompleteAction OnComplete(Data data, bool success)
		{
			this.status = data.Status;
			return base.OnComplete(data, success);
		}

		public override void OnSuccess()
		{
			if (this.status == 5001u)
			{
				this.SendPlayerError();
			}
			base.OnSuccess();
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			if (status == 5000u)
			{
				Logger arg_24_0 = Service.Logger;
				TRequest requestArgs = base.RequestArgs;
				arg_24_0.Debug(requestArgs.ChecksumInfoString);
				this.SendPlayerError();
			}
			return base.OnFailure(status, data);
		}

		private void SendPlayerError()
		{
			PlayerErrorRequest playerErrorRequest = new PlayerErrorRequest();
			playerErrorRequest.Prefix = "DESYNC:";
			playerErrorRequest.PlayerId = Service.CurrentPlayer.PlayerId;
			PlayerErrorRequest arg_36_0 = playerErrorRequest;
			TRequest requestArgs = base.RequestArgs;
			arg_36_0.ClientCheckSumString = requestArgs.ChecksumInfoString;
			PlayerErrorCommand command = new PlayerErrorCommand(playerErrorRequest);
			Service.ServerAPI.Async(command);
		}
	}
}
