using System;

namespace StaRTS.Utils.Scheduling
{
	public interface IViewClockTimeObserver
	{
		void OnViewClockTime(float dt);
	}
}
