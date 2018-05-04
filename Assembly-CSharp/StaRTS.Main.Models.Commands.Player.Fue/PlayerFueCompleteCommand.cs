using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Player.Fue
{
	public class PlayerFueCompleteCommand : GameActionCommand<PlayerIdChecksumRequest, PlayerFueCompleteResponse>
	{
		public const string ACTION = "player.fue.complete";

		public PlayerFueCompleteCommand(PlayerIdChecksumRequest request) : base("player.fue.complete", request, new PlayerFueCompleteResponse())
		{
		}

		public override void OnSuccess()
		{
			Service.NotificationController.Init();
		}
	}
}
