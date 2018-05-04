using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpRevengeCommand : GameActionCommand<PvpRevengeRequest, PvpTarget>
	{
		public const string ACTION = "player.pvp.getRevengeTarget";

		public PvpRevengeCommand(PvpRevengeRequest request) : base("player.pvp.getRevengeTarget", request, new PvpTarget())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			base.OnFailure(status, data);
			if (!GameUtils.IsPvpTargetSearchFailureRequiresReload(status))
			{
				Service.PvpManager.OnPvpRevengeFailure(status);
				return OnCompleteAction.Ok;
			}
			return OnCompleteAction.Desync;
		}
	}
}
