using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Holonet
{
	public class HolonetGetCommandCenterEntriesCommand : GameCommand<PlayerIdRequest, HolonetGetCommandCenterEntriesResponse>
	{
		public const string ACTION = "player.holonet.getCommandCenterEntry";

		public HolonetGetCommandCenterEntriesCommand(int lastTimeViewed, PlayerIdRequest request) : base("player.holonet.getCommandCenterEntry", request, new HolonetGetCommandCenterEntriesResponse())
		{
			base.Context = lastTimeViewed;
		}
	}
}
