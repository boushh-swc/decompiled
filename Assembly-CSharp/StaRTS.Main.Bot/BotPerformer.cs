using System;

namespace StaRTS.Main.Bot
{
	public class BotPerformer
	{
		protected object arg;

		public BotPerformer NextPerformer;

		public virtual BotPerformer Init(object arg)
		{
			this.arg = arg;
			return this;
		}

		public virtual void Perform()
		{
			if (this.NextPerformer != null)
			{
				this.NextPerformer.Perform();
			}
		}
	}
}
