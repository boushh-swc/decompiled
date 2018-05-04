using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class GoToHomeStateRUFTask : AbstractRUFTask
	{
		private bool continueProcessing;

		public GoToHomeStateRUFTask()
		{
			base.Priority = 50;
			base.ShouldProcess = true;
			base.ShouldPurgeQueue = false;
			base.ShouldPlayFromLoadState = true;
		}

		public override void Process(bool continueProcessing)
		{
			this.continueProcessing = continueProcessing;
			if (base.ShouldProcess)
			{
				Service.RUFManager.UpdateProcessingLoadState(false);
				if (Service.GameStateMachine.CurrentState is HomeState)
				{
					base.Process(continueProcessing);
					return;
				}
				HomeState.GoToHomeState(new TransitionCompleteDelegate(this.OnHomeLoaded), true);
			}
		}

		private void OnHomeLoaded()
		{
			base.Process(this.continueProcessing);
		}
	}
}
