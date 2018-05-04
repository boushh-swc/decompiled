using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Pvp;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatAttackPlayerCommand : GameCommand<PvpRevengeRequest, PvpTarget>
	{
		public const string ACTION = "cheats.pvp.match";

		public CheatAttackPlayerCommand(PvpRevengeRequest request) : base("cheats.pvp.match", request, new PvpTarget())
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
