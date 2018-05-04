using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class ResetPatchesCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "test.player.content.reset";

		public ResetPatchesCommand(PlayerIdRequest request) : base("test.player.content.reset", request, new DefaultResponse())
		{
		}
	}
}
