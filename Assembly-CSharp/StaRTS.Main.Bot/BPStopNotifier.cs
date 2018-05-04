using System;

namespace StaRTS.Main.Bot
{
	public class BPStopNotifier : BotPerformer
	{
		public override void Perform()
		{
			BotNotifier botNotifier = (BotNotifier)this.arg;
			botNotifier.Parent.NextNotifier = null;
			botNotifier.Parent = null;
			base.Perform();
		}
	}
}
