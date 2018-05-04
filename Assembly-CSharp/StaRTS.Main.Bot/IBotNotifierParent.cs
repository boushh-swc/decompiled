using System;

namespace StaRTS.Main.Bot
{
	public interface IBotNotifierParent
	{
		BotNotifier NextNotifier
		{
			get;
			set;
		}

		void AddNotifier(BotNotifier notifier);
	}
}
