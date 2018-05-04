using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Bot
{
	public class BPDelay : BotPerformer
	{
		public override void Perform()
		{
			Service.BotRunner.Performing = true;
			Service.BotRunner.Log("Delaying for {0}", new object[]
			{
				this.arg
			});
			Service.ViewTimerManager.CreateViewTimer((float)this.arg, false, new TimerDelegate(this.DelayComplete), null);
		}

		private void DelayComplete(uint id, object cookie)
		{
			Service.BotRunner.Performing = false;
			base.Perform();
		}
	}
}
