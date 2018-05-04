using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story
{
	public class DebugTriggerReactor : ITriggerReactor
	{
		public void SatisfyTrigger(IStoryTrigger trigger)
		{
			Service.Logger.Debug("Test Story Trigger Satisfied!");
		}
	}
}
