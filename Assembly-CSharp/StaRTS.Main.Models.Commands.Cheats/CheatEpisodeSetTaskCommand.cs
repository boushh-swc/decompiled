using StaRTS.Main.Models.Commands.Episodes;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetTaskCommand : GameCommand<CheatEpisodeSetTaskRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "cheats.episodes.task.set";

		public CheatEpisodeSetTaskCommand(CheatEpisodeSetTaskRequest request) : base("cheats.episodes.task.set", request, new EpisodeDefaultResponse())
		{
		}
	}
}
