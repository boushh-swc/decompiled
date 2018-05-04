using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class GetSyncContentCommand : GameCommand<PlayerIdRequest, GetSyncContentResponse>
	{
		public const string ACTION = "player.getSyncContent";

		public GetSyncContentCommand(PlayerIdRequest request) : base("player.getSyncContent", request, new GetSyncContentResponse())
		{
		}
	}
}
