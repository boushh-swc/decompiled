using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeGetProgressCommand : GameCommand<PlayerIdRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "player.episodes.progress.get";

		public EpisodeGetProgressCommand(PlayerIdRequest request) : base("player.episodes.progress.get", request, new EpisodeDefaultResponse())
		{
		}
	}
}
