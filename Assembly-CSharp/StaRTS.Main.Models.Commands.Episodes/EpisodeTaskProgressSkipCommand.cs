using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskProgressSkipCommand : GameCommand<PlayerIdChecksumRequest, EpisodeTaskProgressSkipResponse>
	{
		public const string ACTION = "player.episodes.tasks.progress.skip";

		public EpisodeTaskProgressSkipCommand(PlayerIdChecksumRequest request) : base("player.episodes.tasks.progress.skip", request, new EpisodeTaskProgressSkipResponse())
		{
		}
	}
}
