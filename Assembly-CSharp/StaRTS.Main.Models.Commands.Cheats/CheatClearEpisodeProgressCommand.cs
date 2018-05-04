using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Episodes;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatClearEpisodeProgressCommand : GameCommand<PlayerIdRequest, EpisodeDefaultResponse>
	{
		public const string ACTION = "cheats.episodes.progress.clear";

		public CheatClearEpisodeProgressCommand(PlayerIdRequest request) : base("cheats.episodes.progress.clear", request, new EpisodeDefaultResponse())
		{
		}
	}
}
