using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class GetPlayerPvpStatusCommand : GameCommand<PlayerIdRequest, GetPlayerPvpStatusResponse>
	{
		public const string ACTION = "player.pvp.status";

		public GetPlayerPvpStatusCommand(PlayerIdRequest request) : base("player.pvp.status", request, new GetPlayerPvpStatusResponse())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			base.OnFailure(status, data);
			if (status != 2101u)
			{
				Service.Engine.Reload();
			}
			return OnCompleteAction.Ok;
		}
	}
}
