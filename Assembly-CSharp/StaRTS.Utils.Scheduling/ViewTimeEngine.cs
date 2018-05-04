using StaRTS.DataStructures;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Utils.Scheduling
{
	public class ViewTimeEngine
	{
		private const int MAX_PHYS_FRAMES_PER_VIEW_FRAME = 5;

		private float timePerFrame;

		private float timeLast;

		private ViewPhysicsTimeObserverDynamicArray physTimeObservers;

		private ViewFrameTimeObserverDynamicArray frameTimeObservers;

		private ViewClockTimeObserverDynamicArray clockTimeObservers;

		private MutableIterator physMiter;

		private MutableIterator frameMiter;

		private MutableIterator clockMiter;

		public ViewTimeEngine(float timePerFrame)
		{
			Service.ViewTimeEngine = this;
			this.timePerFrame = timePerFrame;
			this.timeLast = this.Now();
			this.physTimeObservers = new ViewPhysicsTimeObserverDynamicArray(4);
			this.frameTimeObservers = new ViewFrameTimeObserverDynamicArray(4);
			this.clockTimeObservers = new ViewClockTimeObserverDynamicArray(4);
			this.physMiter = new MutableIterator();
			this.frameMiter = new MutableIterator();
			this.clockMiter = new MutableIterator();
		}

		public void RegisterPhysicsTimeObserver(IViewPhysicsTimeObserver observer)
		{
			if (observer == null)
			{
				return;
			}
			if (this.physTimeObservers.IndexOf(observer) < 0)
			{
				this.physTimeObservers.Add(observer);
			}
		}

		public void UnregisterPhysicsTimeObserver(IViewPhysicsTimeObserver observer)
		{
			int num = this.physTimeObservers.IndexOf(observer);
			if (num >= 0)
			{
				this.physTimeObservers.RemoveAt(num);
				this.physMiter.OnRemove(num);
			}
		}

		public void RegisterFrameTimeObserver(IViewFrameTimeObserver observer)
		{
			if (observer == null)
			{
				return;
			}
			if (this.frameTimeObservers.IndexOf(observer) < 0)
			{
				this.frameTimeObservers.Add(observer);
			}
		}

		public void UnregisterFrameTimeObserver(IViewFrameTimeObserver observer)
		{
			int num = this.frameTimeObservers.IndexOf(observer);
			if (num >= 0)
			{
				this.frameTimeObservers.RemoveAt(num);
				this.frameMiter.OnRemove(num);
			}
		}

		public void RegisterClockTimeObserver(IViewClockTimeObserver observer, float tickSize)
		{
			if (observer == null)
			{
				return;
			}
			float accumulator = 0f;
			bool flag = false;
			int i = 0;
			int length = this.clockTimeObservers.Length;
			while (i < length)
			{
				ViewClockTimeObserver viewClockTimeObserver = this.clockTimeObservers.Array[i];
				if (viewClockTimeObserver.Observer == observer)
				{
					return;
				}
				if (!flag && viewClockTimeObserver.TickSize == tickSize)
				{
					accumulator = viewClockTimeObserver.Accumulator;
					flag = true;
				}
				i++;
			}
			if (!flag)
			{
				accumulator = MathUtils.FloatMod(this.timeLast, tickSize);
			}
			this.clockTimeObservers.Add(new ViewClockTimeObserver(observer, tickSize, accumulator));
		}

		public void UnregisterClockTimeObserver(IViewClockTimeObserver observer)
		{
			int i = 0;
			int length = this.clockTimeObservers.Length;
			while (i < length)
			{
				if (this.clockTimeObservers.Array[i].Observer == observer)
				{
					this.clockTimeObservers.RemoveAt(i);
					this.clockMiter.OnRemove(i);
					break;
				}
				i++;
			}
		}

		public void UnregisterAll()
		{
			this.physTimeObservers.Clear();
			this.frameTimeObservers.Clear();
			this.clockTimeObservers.Clear();
			this.physMiter.Reset();
			this.frameMiter.Reset();
			this.clockMiter.Reset();
		}

		public void OnUpdate()
		{
			float num = this.Now();
			float num2 = num - this.timeLast;
			this.timeLast = num;
			float num3 = num2;
			int num4 = 0;
			while (num3 > 0f)
			{
				float num5 = (num3 <= this.timePerFrame) ? num3 : this.timePerFrame;
				this.physMiter.Init(this.physTimeObservers.Length);
				while (this.physMiter.Active())
				{
					IViewPhysicsTimeObserver viewPhysicsTimeObserver = this.physTimeObservers.Array[this.physMiter.Index];
					viewPhysicsTimeObserver.OnViewPhysicsTime(num5);
					this.physMiter.Next();
				}
				this.physMiter.Reset();
				if (++num4 == 5)
				{
					break;
				}
				num3 -= num5;
			}
			this.frameMiter.Init(this.frameTimeObservers.Length);
			while (this.frameMiter.Active())
			{
				IViewFrameTimeObserver viewFrameTimeObserver = this.frameTimeObservers.Array[this.frameMiter.Index];
				viewFrameTimeObserver.OnViewFrameTime(num2);
				this.frameMiter.Next();
			}
			this.frameMiter.Reset();
			this.clockMiter.Init(this.clockTimeObservers.Length);
			while (this.clockMiter.Active())
			{
				ViewClockTimeObserver viewClockTimeObserver = this.clockTimeObservers.Array[this.clockMiter.Index];
				float num6 = viewClockTimeObserver.Accumulator + num2;
				float tickSize = viewClockTimeObserver.TickSize;
				while (num6 >= tickSize)
				{
					viewClockTimeObserver.Observer.OnViewClockTime(tickSize);
					num6 -= tickSize;
				}
				viewClockTimeObserver.Accumulator = num6;
				this.clockMiter.Next();
			}
			this.clockMiter.Reset();
		}

		private float Now()
		{
			return Time.time;
		}
	}
}
