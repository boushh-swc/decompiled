using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskTimeGateStartCommand : GameCommand<PlayerIdRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "player.episodes.tasks.timegate.start";

		public EpisodeTaskTimeGateStartCommand(PlayerIdRequest request) : base("player.episodes.tasks.timegate.start", request, new EpisodeDefaultResponse())
		{
		}
	}
}
