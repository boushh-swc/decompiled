using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkCancelCommand : GameActionCommand<PlayerPerkCancelRequest, PlayerPerksDataResponse>
	{
		public const string ACTION = "player.perks.cancel";

		public PlayerPerkCancelCommand(PlayerPerkCancelRequest request) : base("player.perks.cancel", request, new PlayerPerksDataResponse())
		{
		}
	}
}
