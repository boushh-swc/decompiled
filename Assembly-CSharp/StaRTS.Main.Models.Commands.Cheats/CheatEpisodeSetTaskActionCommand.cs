using StaRTS.Main.Models.Commands.Episodes;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetTaskActionCommand : GameCommand<CheatEpisodeSetTaskActionRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "cheats.episodes.task.action.set";

		public CheatEpisodeSetTaskActionCommand(CheatEpisodeSetTaskActionRequest request) : base("cheats.episodes.task.action.set", request, new EpisodeDefaultResponse())
		{
		}
	}
}
