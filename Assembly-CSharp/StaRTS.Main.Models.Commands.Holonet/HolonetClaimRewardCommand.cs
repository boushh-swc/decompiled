using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Holonet
{
	public class HolonetClaimRewardCommand : GameCommand<HolonetClaimRewardRequest, DefaultResponse>
	{
		public const string ACTION = "player.holonet.claimReward";

		public HolonetClaimRewardCommand(HolonetClaimRewardRequest request) : base("player.holonet.claimReward", request, new DefaultResponse())
		{
		}
	}
}
