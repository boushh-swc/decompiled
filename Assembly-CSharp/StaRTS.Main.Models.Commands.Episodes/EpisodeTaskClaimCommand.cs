using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskClaimCommand : GameCommand<PlayerIdRequest, EpisodeTaskClaimResponse>
	{
		public const string ACTION = "player.episodes.tasks.claim";

		public EpisodeTaskClaimCommand(PlayerIdRequest request) : base("player.episodes.tasks.claim", request, new EpisodeTaskClaimResponse())
		{
		}
	}
}
