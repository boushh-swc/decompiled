using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class GetContentCommand : GameCommand<GetContentRequest, GetContentResponse>
	{
		public const string ACTION = "player.content.get";

		public GetContentCommand(GetContentRequest request) : base("player.content.get", request, new GetContentResponse())
		{
		}
	}
}
