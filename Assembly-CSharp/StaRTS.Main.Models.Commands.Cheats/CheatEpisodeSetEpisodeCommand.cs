using StaRTS.Main.Models.Commands.Episodes;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEpisodeSetEpisodeCommand : GameCommand<CheatEpisodeSetEpisodeRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "cheats.episodes.episode.set";

		public CheatEpisodeSetEpisodeCommand(CheatEpisodeSetEpisodeRequest request) : base("cheats.episodes.episode.set", request, new EpisodeDefaultResponse())
		{
		}
	}
}
