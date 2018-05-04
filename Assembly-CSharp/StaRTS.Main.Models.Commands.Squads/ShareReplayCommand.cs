using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class ShareReplayCommand : SquadGameCommand<ShareReplayRequest, DefaultResponse>
	{
		public const string ACTION = "guild.battle.share";

		public ShareReplayCommand(ShareReplayRequest request) : base("guild.battle.share", request, new DefaultResponse())
		{
		}
	}
}
