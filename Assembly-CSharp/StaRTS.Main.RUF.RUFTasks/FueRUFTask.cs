using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Story;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class FueRUFTask : AbstractRUFTask
	{
		public FueRUFTask()
		{
			base.Priority = 20;
			base.ShouldProcess = false;
			base.ShouldPurgeQueue = true;
			base.ShouldPlayFromLoadState = true;
			if (Service.CurrentPlayer.HasNotCompletedFirstFueStep())
			{
				base.ShouldProcess = true;
			}
		}

		public override void Process(bool continueProcessing)
		{
			if (base.ShouldProcess)
			{
				new ActionChain(GameConstants.FUE_QUEST_UID);
				Service.GameStateMachine.SetState(new IntroCameraState());
			}
			base.Process(continueProcessing);
		}
	}
}
