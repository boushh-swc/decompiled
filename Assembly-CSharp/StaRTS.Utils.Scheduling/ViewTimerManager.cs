using StaRTS.Utils.Core;
using System;

namespace StaRTS.Utils.Scheduling
{
	public class ViewTimerManager : TimerManager, IViewFrameTimeObserver
	{
		public ViewTimerManager()
		{
			Service.ViewTimerManager = this;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public uint CreateViewTimer(float delay, bool repeat, TimerDelegate callback, object cookie)
		{
			delay *= 1000f;
			if (delay < 0f || delay >= 4.2949673E+09f)
			{
				throw new Exception(string.Format("Timer delay {0} is out of range.  Check against TimerManager.MAX_DELAY_SECONDS", delay));
			}
			return base.CreateTimer((uint)delay, repeat, callback, cookie);
		}

		public void KillViewTimer(uint id)
		{
			base.KillTimer(id);
		}

		public void TriggerKillViewTimer(uint id)
		{
			base.TriggerKillTimer(id);
		}

		public void OnViewFrameTime(float dt)
		{
			base.OnDeltaTime((uint)(dt * 1000f));
		}
	}
}
