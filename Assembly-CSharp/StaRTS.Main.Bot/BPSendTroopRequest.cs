using StaRTS.Main.Controllers.Squads;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Bot
{
	public class BPSendTroopRequest : BotPerformer
	{
		public override void Perform()
		{
			Service.BotRunner.Log("Sending troop request", new object[0]);
			SquadController squadController = Service.SquadController;
			squadController.SendTroopRequest(null, false);
			base.Perform();
		}
	}
}
