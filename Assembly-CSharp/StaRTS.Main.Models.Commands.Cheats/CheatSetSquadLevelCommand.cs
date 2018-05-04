using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetSquadLevelCommand : GameCommand<CheatSetSquadLevelRequest, DefaultResponse>
	{
		private const string ACTION = "cheats.guild.setLevel";

		public CheatSetSquadLevelCommand(CheatSetSquadLevelRequest request) : base("cheats.guild.setLevel", request, new DefaultResponse())
		{
		}
	}
}
