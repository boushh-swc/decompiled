using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SetPrefsCommand : GameCommand<SetPrefsRequest, DefaultResponse>
	{
		public const string ACTION = "player.prefs.set";

		private bool reloadOnResponse;

		public SetPrefsCommand(bool reloadOnResponse) : base("player.prefs.set", new SetPrefsRequest(), new DefaultResponse())
		{
			this.reloadOnResponse = reloadOnResponse;
		}

		public override void OnSuccess()
		{
			if (this.reloadOnResponse)
			{
				Service.Engine.Reload();
			}
		}
	}
}
