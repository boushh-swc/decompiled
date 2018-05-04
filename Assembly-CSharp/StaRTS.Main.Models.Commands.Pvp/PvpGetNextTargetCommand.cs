using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpGetNextTargetCommand : GameCommand<PvpGetNextTargetRequest, PvpTarget>
	{
		public const string ACTION = "player.pvp.getNextTarget";

		public PvpGetNextTargetCommand(PvpGetNextTargetRequest request) : base("player.pvp.getNextTarget", request, new PvpTarget())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			base.OnFailure(status, data);
			if (!GameUtils.IsPvpTargetSearchFailureRequiresReload(status))
			{
				Service.PvpManager.OnPvpGetNextTargetFailure();
				return OnCompleteAction.Ok;
			}
			return OnCompleteAction.Desync;
		}
	}
}
