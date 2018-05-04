using System;

namespace StaRTS.Utils.Scheduling
{
	public class ViewClockTimeObserver
	{
		public IViewClockTimeObserver Observer
		{
			get;
			private set;
		}

		public float TickSize
		{
			get;
			private set;
		}

		public float Accumulator
		{
			get;
			set;
		}

		public ViewClockTimeObserver(IViewClockTimeObserver observer, float tickSize, float accumulator)
		{
			this.Observer = observer;
			this.TickSize = tickSize;
			this.Accumulator = accumulator;
		}
	}
}
