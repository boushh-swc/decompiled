using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class ResetStateCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.state.reset";

		public ResetStateCommand(PlayerIdRequest request) : base("player.state.reset", request, new DefaultResponse())
		{
		}

		public override void OnSuccess()
		{
			Service.Logger.Debug("Reset successful.");
		}
	}
}
