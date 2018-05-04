using System;

namespace StaRTS.Main.Story
{
	public interface ITriggerReactor
	{
		void SatisfyTrigger(IStoryTrigger trigger);
	}
}
