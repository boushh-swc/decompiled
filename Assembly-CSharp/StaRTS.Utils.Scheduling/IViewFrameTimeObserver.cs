using System;

namespace StaRTS.Utils.Scheduling
{
	public interface IViewFrameTimeObserver
	{
		void OnViewFrameTime(float dt);
	}
}
