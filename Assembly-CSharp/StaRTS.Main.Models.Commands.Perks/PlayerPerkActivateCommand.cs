using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkActivateCommand : GameActionCommand<PlayerPerkActivateRequest, PlayerPerksDataResponse>
	{
		public const string ACTION = "player.perks.activate";

		public PlayerPerkActivateCommand(PlayerPerkActivateRequest request) : base("player.perks.activate", request, new PlayerPerksDataResponse())
		{
		}
	}
}
