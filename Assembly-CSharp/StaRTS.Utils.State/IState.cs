using System;

namespace StaRTS.Utils.State
{
	public interface IState
	{
		void OnEnter();

		void OnExit(IState nextState);
	}
}
