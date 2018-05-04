using System;

namespace StaRTS.Main.Story
{
	public interface IStoryReactor
	{
		void ChildPrepared(IStoryAction child);

		void ChildComplete(IStoryAction child);
	}
}
