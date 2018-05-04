using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class KeepAliveCommand : GameCommand<KeepAliveRequest, DefaultResponse>
	{
		public const string ACTION = "player.keepAlive";

		public KeepAliveCommand(KeepAliveRequest request) : base("player.keepAlive", request, new DefaultResponse())
		{
		}

		protected override bool IsAddToken()
		{
			return false;
		}
	}
}
