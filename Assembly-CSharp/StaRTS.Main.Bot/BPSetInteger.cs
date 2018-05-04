using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Bot
{
	public class BPSetInteger : BotPerformer
	{
		public override void Perform()
		{
			KeyValuePair<string, int> keyValuePair = (KeyValuePair<string, int>)this.arg;
			Service.BotRunner.BotProperties[keyValuePair.Key] = keyValuePair.Value;
			Service.BotRunner.Log("Setting '{0}' to {1}", new object[]
			{
				keyValuePair.Key,
				keyValuePair.Value
			});
			base.Perform();
		}
	}
}
