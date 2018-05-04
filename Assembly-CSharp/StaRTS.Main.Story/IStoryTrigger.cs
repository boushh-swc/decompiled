using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Story
{
	public interface IStoryTrigger : ISerializable
	{
		bool HasReaction
		{
			get;
		}

		string Reaction
		{
			get;
		}

		StoryTriggerVO VO
		{
			get;
		}

		void Activate();

		void Destroy();
	}
}
