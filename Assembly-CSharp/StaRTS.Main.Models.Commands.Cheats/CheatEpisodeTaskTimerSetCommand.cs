using StaRTS.Main.Models.Commands.Episodes;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeTaskTimerSetCommand : GameCommand<CheatEpisodeTaskTimerSetRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "cheats.episodes.task.timegate.set";

		public CheatEpisodeTaskTimerSetCommand(CheatEpisodeTaskTimerSetRequest request) : base("cheats.episodes.task.timegate.set", request, new EpisodeDefaultResponse())
		{
		}
	}
}
