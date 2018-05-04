using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskProgressCompleteCommand : GameCommand<PlayerIdChecksumRequest, EpisodeTaskProgressCompleteResponse>
	{
		public const string ACTION = "player.episodes.tasks.progress.complete";

		public EpisodeTaskProgressCompleteCommand(PlayerIdChecksumRequest request) : base("player.episodes.tasks.progress.complete", request, new EpisodeTaskProgressCompleteResponse())
		{
		}
	}
}
