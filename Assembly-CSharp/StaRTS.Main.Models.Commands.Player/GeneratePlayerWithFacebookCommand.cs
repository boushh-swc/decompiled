using Source.StaRTS.Main.Models.Commands.Player;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class GeneratePlayerWithFacebookCommand : GameCommand<GeneratePlayerWithFacebookRequest, GeneratePlayerResponse>
	{
		private const string ACTION = "auth.preauth.generatePlayerWithFacebook";

		public GeneratePlayerWithFacebookCommand(GeneratePlayerWithFacebookRequest request) : base("auth.preauth.generatePlayerWithFacebook", request, new GeneratePlayerResponse())
		{
		}
	}
}
