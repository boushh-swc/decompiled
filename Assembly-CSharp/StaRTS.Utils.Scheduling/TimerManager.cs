using StaRTS.DataStructures;
using System;

namespace StaRTS.Utils.Scheduling
{
	public class TimerManager
	{
		public const uint ONE_DAY = 86400000u;

		public const uint MAX_DELAY_MILLIS = 432000000u;

		public const uint MAX_DELAY_SECONDS = 432000u;

		private const uint REBASE_TIME = 60000u;

		protected const uint INFINITY = 4294967295u;

		private uint idLast;

		private TimerDynamicArray dynamicArray;

		private uint lastTimeFire;

		private int lastIndex;

		private uint timeNow;

		private uint timeNext;

		public TimerManager()
		{
			this.idLast = 0u;
			this.dynamicArray = new TimerDynamicArray(4);
			this.lastTimeFire = 4294967295u;
			this.lastIndex = -1;
			this.timeNow = 0u;
			this.timeNext = 4294967295u;
		}

		protected uint CreateTimer(uint delay, bool repeat, TimerDelegate callback, object cookie)
		{
			if (delay > 432000000u)
			{
				throw new Exception(string.Format("Timer delay {0} exceeds maximum {1}", delay, 432000000u));
			}
			if (delay == 0u)
			{
				delay = 1u;
			}
			if (callback == null)
			{
				throw new Exception("Null timer callback not supported nor useful");
			}
			uint next = TimerId.GetNext(ref this.idLast);
			Timer timer = new Timer(next, delay, repeat, callback, cookie, this.timeNow);
			uint num = this.timeNow + delay;
			if (num == this.lastTimeFire)
			{
				this.dynamicArray.Insert(++this.lastIndex, timer);
			}
			else
			{
				this.lastTimeFire = num;
				this.lastIndex = this.Add(timer);
			}
			if (this.lastIndex == 0)
			{
				this.timeNext = num;
			}
			return next;
		}

		protected void KillTimer(uint id)
		{
			Timer[] array = this.dynamicArray.Array;
			int length = this.dynamicArray.Length;
			for (int i = 0; i < length; i++)
			{
				if (array[i].Id == id)
				{
					this.KillTimerAt(i);
					break;
				}
			}
		}

		protected void TriggerKillTimer(uint id)
		{
			Timer[] array = this.dynamicArray.Array;
			int length = this.dynamicArray.Length;
			for (int i = 0; i < length; i++)
			{
				Timer timer = array[i];
				if (timer.Id == id)
				{
					timer.Callback(timer.Id, timer.Cookie);
					break;
				}
			}
			this.KillTimer(id);
		}

		private void KillTimerAt(int i)
		{
			this.lastTimeFire = 4294967295u;
			this.lastIndex = -1;
			Timer timer = this.dynamicArray.Array[i];
			timer.Kill();
			this.dynamicArray.RemoveAt(i);
			if (i == 0)
			{
				this.SetTimeNext();
			}
		}

		public void EnsureTimerKilled(ref uint id)
		{
			if (id != 0u)
			{
				this.KillTimer(id);
				id = 0u;
			}
		}

		private void SetTimeNext()
		{
			this.timeNext = ((this.dynamicArray.Length != 0) ? this.dynamicArray.Array[0].TimeFire : 4294967295u);
		}

		protected void OnDeltaTime(uint dt)
		{
			this.timeNow += dt;
			if (this.timeNow >= 60000u)
			{
				if (this.timeNext == 4294967295u)
				{
					this.timeNow -= 60000u;
				}
				else if (this.timeNext >= 60000u)
				{
					this.timeNow -= 60000u;
					this.timeNext -= 60000u;
					this.Rebase(60000u);
				}
			}
			this.lastTimeFire = 4294967295u;
			this.lastIndex = -1;
			while (this.timeNext <= this.timeNow)
			{
				Timer timer = this.dynamicArray.Array[0];
				if (timer.TimeFire > this.timeNow)
				{
					break;
				}
				timer.Callback(timer.Id, timer.Cookie);
				if (timer.Repeat)
				{
					while (!timer.IsKilled)
					{
						if (timer.IncTimeFireByDelay() > this.timeNow)
						{
							this.ReprioritizeFirst();
							this.SetTimeNext();
							break;
						}
						timer.Callback(timer.Id, timer.Cookie);
					}
				}
				else if (!timer.IsKilled)
				{
					this.KillTimer(timer.Id);
				}
			}
		}

		private int Add(Timer timer)
		{
			uint timeFire = timer.TimeFire;
			Timer[] array = this.dynamicArray.Array;
			int length = this.dynamicArray.Length;
			for (int i = 0; i < length; i++)
			{
				if (timeFire < array[i].TimeFire)
				{
					this.dynamicArray.Insert(i, timer);
					return i;
				}
			}
			this.dynamicArray.Add(timer);
			return length;
		}

		private void ReprioritizeFirst()
		{
			Timer[] array = this.dynamicArray.Array;
			int length = this.dynamicArray.Length;
			Timer timer = array[0];
			uint timeFire = timer.TimeFire;
			for (int i = 1; i < length; i++)
			{
				Timer timer2 = array[i];
				if (timeFire < timer2.TimeFire)
				{
					return;
				}
				array[i] = timer;
				array[i - 1] = timer2;
			}
		}

		private void Rebase(uint amount)
		{
			Timer[] array = this.dynamicArray.Array;
			int length = this.dynamicArray.Length;
			for (int i = 0; i < length; i++)
			{
				array[i].DecTimeFire(amount);
			}
		}
	}
}
