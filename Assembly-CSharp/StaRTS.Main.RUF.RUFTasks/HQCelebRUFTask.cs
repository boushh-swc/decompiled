using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class HQCelebRUFTask : AbstractRUFTask
	{
		public HQCelebRUFTask()
		{
			base.Priority = 80;
			base.ShouldProcess = true;
			base.ShouldPurgeQueue = true;
		}

		public override void Process(bool continueProcessing)
		{
			if (base.ShouldProcess)
			{
			}
			base.Process(continueProcessing);
		}
	}
}
