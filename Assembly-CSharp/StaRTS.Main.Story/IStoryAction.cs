using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Story
{
	public interface IStoryAction
	{
		StoryActionVO VO
		{
			get;
		}

		string Reaction
		{
			get;
		}

		void Prepare();

		void Execute();

		void Destroy();
	}
}
