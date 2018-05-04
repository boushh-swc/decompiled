using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Episodes
{
	public class EpisodeTaskIntroViewedCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.episodes.tasks.intro.viewed";

		public EpisodeTaskIntroViewedCommand(PlayerIdRequest request) : base("player.episodes.tasks.intro.viewed", request, new DefaultResponse())
		{
		}
	}
}
