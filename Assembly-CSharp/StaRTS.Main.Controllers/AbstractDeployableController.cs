using StaRTS.Main.Controllers.GameStates;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public abstract class AbstractDeployableController
	{
		protected void EnsureBattlePlayState()
		{
			GameStateMachine gameStateMachine = Service.GameStateMachine;
			if (gameStateMachine.CurrentState is BattleStartState)
			{
				gameStateMachine.SetState(new BattlePlayState());
			}
		}
	}
}
