using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Utils.Scheduling
{
	public class SimTimeEngine
	{
		private const int MAX_SIM_FRAMES_LAG_BEFORE_RESET = 30;

		private const uint ROLLOVER_MILLISECONDS = 60000u;

		private const float ROLLOVER_SECONDS = 60f;

		private const float LOW_SIM_TIME_PER_FRAME = 0.0055f;

		private uint timePerFrame;

		private uint maxLag;

		private uint timeLast;

		private uint timeAccumulator;

		private float rolloverNext;

		private List<ISimTimeObserver> observers;

		private MutableIterator miter;

		private uint scale;

		private uint frameCount;

		public SimTimeEngine(uint timePerFrame)
		{
			Service.SimTimeEngine = this;
			this.timePerFrame = timePerFrame;
			this.ScaleTime(1u);
			this.timeLast = this.Now();
			this.timeAccumulator = 0u;
			this.rolloverNext = 0f;
			this.observers = new List<ISimTimeObserver>();
			this.miter = new MutableIterator();
			this.frameCount = 0u;
		}

		public void RegisterSimTimeObserver(ISimTimeObserver observer)
		{
			if (observer == null)
			{
				return;
			}
			if (this.observers.IndexOf(observer) < 0)
			{
				this.observers.Add(observer);
			}
		}

		public void UnregisterSimTimeObserver(ISimTimeObserver observer)
		{
			int num = this.observers.IndexOf(observer);
			if (num >= 0)
			{
				this.observers.RemoveAt(num);
				this.miter.OnRemove(num);
			}
		}

		public void UnregisterAll()
		{
			this.observers.Clear();
			this.miter.Reset();
		}

		public void ScaleTime(uint scale)
		{
			this.scale = scale;
			this.maxLag = this.timePerFrame * 30u * scale;
		}

		public bool IsPaused()
		{
			return this.scale == 0u;
		}

		public uint GetFrameCount()
		{
			return this.frameCount;
		}

		public void OnUpdate()
		{
			uint num = this.Now();
			uint num2 = (num >= this.timeLast) ? (num - this.timeLast) : (num + 60000u - this.timeLast);
			this.timeLast = num;
			num2 *= this.scale;
			this.timeAccumulator += num2;
			float realTimeSinceStartUp = UnityUtils.GetRealTimeSinceStartUp();
			int num3 = 0;
			while (this.timeAccumulator >= this.timePerFrame)
			{
				this.miter.Init(this.observers);
				while (this.miter.Active())
				{
					ISimTimeObserver simTimeObserver = this.observers[this.miter.Index];
					simTimeObserver.OnSimTime(this.timePerFrame);
					this.miter.Next();
				}
				this.miter.Reset();
				this.frameCount += 1u;
				this.timeAccumulator -= this.timePerFrame;
				if ((long)(++num3) == (long)((ulong)this.scale))
				{
					if (UnityUtils.GetRealTimeSinceStartUp() - realTimeSinceStartUp > 0.0055f)
					{
						break;
					}
				}
				else if ((long)num3 == (long)((ulong)(this.scale + 1u)))
				{
					break;
				}
			}
			if (this.timeAccumulator > this.maxLag)
			{
				this.timeAccumulator = 0u;
			}
		}

		private uint Now()
		{
			float time = Time.time;
			float num = time - this.rolloverNext;
			if (num >= 60f)
			{
				num -= 60f;
				this.rolloverNext += 60f;
			}
			return (uint)Mathf.Floor(num * 1000f);
		}
	}
}
