using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarAttackBuffBaseCompleteCommand : SquadGameActionCommand<BattleEndRequest, DefaultResponse>
	{
		public const string ACTION = "guild.war.attackBase.complete";

		public SquadWarAttackBuffBaseCompleteCommand(BattleEndRequest request) : base("guild.war.attackBase.complete", request, new DefaultResponse())
		{
		}
	}
}
