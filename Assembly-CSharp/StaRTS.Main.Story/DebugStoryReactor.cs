using System;

namespace StaRTS.Main.Story
{
	public class DebugStoryReactor : IStoryReactor
	{
		public void ChildPrepared(IStoryAction child)
		{
			child.Execute();
		}

		public void ChildComplete(IStoryAction child)
		{
		}
	}
}
