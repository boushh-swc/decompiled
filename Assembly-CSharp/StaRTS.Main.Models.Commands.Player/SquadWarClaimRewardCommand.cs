using StaRTS.Main.Models.Commands.Crates;
using StaRTS.Main.Models.Commands.Squads;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SquadWarClaimRewardCommand : SquadGameCommand<SquadWarClaimRewardRequest, CrateDataResponse>
	{
		public const string ACTION = "guild.war.claim";

		public SquadWarClaimRewardCommand(SquadWarClaimRewardRequest request) : base("guild.war.claim", request, new CrateDataResponse())
		{
		}
	}
}
