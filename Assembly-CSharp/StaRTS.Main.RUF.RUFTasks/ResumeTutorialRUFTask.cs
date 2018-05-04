using StaRTS.Main.Story;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class ResumeTutorialRUFTask : AbstractRUFTask
	{
		private string story;

		public ResumeTutorialRUFTask(string story)
		{
			base.Priority = 70;
			base.ShouldPurgeQueue = true;
			base.ShouldProcess = true;
			this.story = story;
		}

		public override void Process(bool continueProcessing)
		{
			if (base.ShouldProcess)
			{
				new ActionChain(this.story);
			}
			base.Process(continueProcessing);
		}
	}
}
