using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Utils;
using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkInvestCommand : GameActionCommand<PlayerPerkInvestRequest, PlayerPerkInvestResponse>
	{
		public const string ACTION = "player.perks.invest";

		public PlayerPerkInvestCommand(PlayerPerkInvestRequest request) : base("player.perks.invest", request, new PlayerPerkInvestResponse())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			if (!GameUtils.IsPerkCommandStatusFatal(status))
			{
				return OnCompleteAction.Ok;
			}
			return base.OnFailure(status, data);
		}
	}
}
