using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public interface IGameState : IState
	{
		bool CanUpdateHomeContracts();
	}
}
