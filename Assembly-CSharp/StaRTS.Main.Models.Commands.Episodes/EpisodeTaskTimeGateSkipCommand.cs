using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskTimeGateSkipCommand : GameCommand<PlayerIdChecksumRequest, EpisodeTaskTimeGateSkipResponse>
	{
		public const string ACTION = "player.episodes.tasks.timegate.skip";

		public EpisodeTaskTimeGateSkipCommand(PlayerIdChecksumRequest request) : base("player.episodes.tasks.timegate.skip", request, new EpisodeTaskTimeGateSkipResponse())
		{
		}
	}
}
