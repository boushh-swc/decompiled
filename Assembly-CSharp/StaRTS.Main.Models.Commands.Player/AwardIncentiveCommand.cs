using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class AwardIncentiveCommand : GameActionCommand<PlayerIdChecksumRequest, DefaultResponse>
	{
		public const string ACTION = "player.award.rateMyApp";

		public AwardIncentiveCommand(PlayerIdChecksumRequest request) : base("player.award.rateMyApp", request, new DefaultResponse())
		{
		}
	}
}
