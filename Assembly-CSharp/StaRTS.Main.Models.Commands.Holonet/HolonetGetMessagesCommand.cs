using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Holonet
{
	public class HolonetGetMessagesCommand : GameCommand<PlayerIdRequest, HolonetGetMessagesResponse>
	{
		public const string ACTION = "player.holonet.getEventMessage";

		public HolonetGetMessagesCommand(PlayerIdRequest request) : base("player.holonet.getEventMessage", request, new HolonetGetMessagesResponse())
		{
		}
	}
}
