using System;

namespace StaRTS.Main.Controllers.Performance
{
	public class NamedSample
	{
		public string name;

		public int count;

		public float totalTime;

		public float lastTime;

		public float average;

		public float peakTime;

		public NamedSample(string sampleName)
		{
			this.name = sampleName;
			this.count = 0;
			this.totalTime = 0f;
			this.lastTime = 0f;
			this.average = 0f;
			this.peakTime = 0f;
		}
	}
}
