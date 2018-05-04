using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Bot
{
	public class BPDecrementInteger : BotPerformer
	{
		public override void Perform()
		{
			int num = (int)Service.BotRunner.BotProperties[(string)this.arg] - 1;
			Service.BotRunner.BotProperties[(string)this.arg] = num;
			Service.BotRunner.Log("Decrementing '{0}' to {1}", new object[]
			{
				this.arg,
				num
			});
			base.Perform();
		}
	}
}
