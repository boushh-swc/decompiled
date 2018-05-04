using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Bot
{
	public class BNWhilePropNonZero : BotNotifier
	{
		public override bool EvaluateUpdate()
		{
			int num = (int)Service.BotRunner.BotProperties[(string)this.arg];
			bool flag = num > 0;
			Service.BotRunner.Log("'{0}': {1} > 0 = {2}", new object[]
			{
				(string)this.arg,
				num,
				flag
			});
			return flag;
		}
	}
}
