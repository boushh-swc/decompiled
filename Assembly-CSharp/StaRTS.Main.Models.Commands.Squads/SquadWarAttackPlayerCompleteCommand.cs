using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarAttackPlayerCompleteCommand : SquadGameActionCommand<BattleEndRequest, DefaultResponse>
	{
		public const string ACTION = "guild.war.attackPlayer.complete";

		public SquadWarAttackPlayerCompleteCommand(BattleEndRequest request) : base("guild.war.attackPlayer.complete", request, new DefaultResponse())
		{
		}
	}
}
