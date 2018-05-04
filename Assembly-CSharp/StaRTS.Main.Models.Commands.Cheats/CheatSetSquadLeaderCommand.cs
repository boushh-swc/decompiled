using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetSquadLeaderCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		private const string ACTION = "cheats.guild.setLeader";

		public CheatSetSquadLeaderCommand(PlayerIdRequest request) : base("cheats.guild.setLeader", request, new DefaultResponse())
		{
		}
	}
}
