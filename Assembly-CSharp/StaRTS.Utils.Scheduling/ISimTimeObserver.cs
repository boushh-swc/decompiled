using System;

namespace StaRTS.Utils.Scheduling
{
	public interface ISimTimeObserver
	{
		void OnSimTime(uint dt);
	}
}
