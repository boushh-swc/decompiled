using StaRTS.DataStructures;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Bot
{
	public class BPDeploySquadTroops : BotPerformer
	{
		public override void Perform()
		{
			Service.BotRunner.Performing = true;
			Service.BotRunner.Log("Deploying Squad Troops", new object[0]);
			IntPosition boardPos = new IntPosition(22, 22);
			Service.SquadTroopAttackController.DeploySquadTroops(boardPos);
			Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.DelayComplete), null);
		}

		private void DelayComplete(uint id, object cookie)
		{
			Service.BotRunner.Performing = false;
			base.Perform();
		}
	}
}
