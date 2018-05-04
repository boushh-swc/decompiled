using System;

namespace StaRTS.Utils.Scheduling
{
	public class Timer
	{
		private bool isKilled;

		public uint Id
		{
			get;
			private set;
		}

		public uint Delay
		{
			get;
			private set;
		}

		public bool Repeat
		{
			get;
			private set;
		}

		public TimerDelegate Callback
		{
			get;
			private set;
		}

		public object Cookie
		{
			get;
			private set;
		}

		public uint TimeFire
		{
			get;
			private set;
		}

		public bool IsKilled
		{
			get
			{
				return this.isKilled;
			}
		}

		public Timer(uint id, uint delay, bool repeat, TimerDelegate callback, object cookie, uint now)
		{
			this.isKilled = false;
			this.Id = id;
			this.Delay = delay;
			this.Repeat = repeat;
			this.Callback = callback;
			this.Cookie = cookie;
			this.TimeFire = now + this.Delay;
		}

		public void DecTimeFire(uint delta)
		{
			this.TimeFire -= delta;
		}

		public uint IncTimeFireByDelay()
		{
			return this.TimeFire += this.Delay;
		}

		public void Kill()
		{
			this.Cookie = null;
			this.Callback = null;
			this.isKilled = true;
		}
	}
}
