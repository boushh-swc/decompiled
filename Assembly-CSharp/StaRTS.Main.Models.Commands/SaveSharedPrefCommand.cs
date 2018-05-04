using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class SaveSharedPrefCommand : GameCommand<SharedPrefRequest, DefaultResponse>
	{
		public const string ACTION = "player.preferences.set";

		public SaveSharedPrefCommand(string key, string value) : base("player.preferences.set", new SharedPrefRequest(key, value), new DefaultResponse())
		{
		}
	}
}
