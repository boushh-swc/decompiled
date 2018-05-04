using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Bot
{
	public class BPStartNotifier : BotPerformer
	{
		public override void Perform()
		{
			Service.BotRunner.AddNotifier((BotNotifier)this.arg);
			base.Perform();
		}
	}
}
