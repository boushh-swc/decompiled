using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Fue
{
	public class FueUpdateStateCommand : GameActionCommand<FueUpdateStateRequest, DefaultResponse>
	{
		public const string ACTION = "player.fue.setQuest";

		public FueUpdateStateCommand() : base("player.fue.setQuest", new FueUpdateStateRequest(), new DefaultResponse())
		{
		}
	}
}
