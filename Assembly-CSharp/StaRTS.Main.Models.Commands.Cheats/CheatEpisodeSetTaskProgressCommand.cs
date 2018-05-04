using StaRTS.Main.Models.Commands.Episodes;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetTaskProgressCommand : GameCommand<CheatEpisodeSetTaskProgressRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "cheats.episodes.task.progress.set";

		public CheatEpisodeSetTaskProgressCommand(CheatEpisodeSetTaskProgressRequest request) : base("cheats.episodes.task.progress.set", request, new EpisodeDefaultResponse())
		{
		}
	}
}
