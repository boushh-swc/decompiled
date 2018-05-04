using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Controllers.Performance
{
	public class BIFrameMonitor : IPerformanceObserver, IViewFrameTimeObserver
	{
		private const int SECONDS_TO_CAPTURE = 5;

		private int secsToCapture;

		private float biFps;

		public BIFrameMonitor()
		{
			this.secsToCapture = 0;
			this.biFps = 0f;
		}

		public void LogFPS()
		{
			if (this.secsToCapture > 0)
			{
				return;
			}
			this.biFps = 0f;
			Service.PerformanceMonitor.RegisterFPSObserver(this);
			this.secsToCapture = 5;
		}

		public void OnViewFrameTime(float dt)
		{
		}

		public void OnPerformanceFPS(float fps)
		{
			if (fps < 1f)
			{
				return;
			}
			if (this.biFps == 0f)
			{
				this.biFps = fps;
			}
			else
			{
				this.biFps = (this.biFps + fps) / 2f;
			}
			if (this.biFps > 60f)
			{
				this.biFps = 60f;
			}
			if (--this.secsToCapture <= 0)
			{
				Service.PerformanceMonitor.UnregisterFPSObserver(this);
				this.FinishLogFPS();
			}
		}

		public void OnPerformanceFPeak(uint fpeak)
		{
		}

		public void OnPerformanceMemRsvd(uint memRsvd)
		{
		}

		public void OnPerformanceMemUsed(uint memUsed)
		{
		}

		public void OnPerformanceMemTexture(uint memTexture)
		{
		}

		public void OnPerformanceMemMesh(uint memMesh)
		{
		}

		public void OnPerformanceMemAudio(uint memAudio)
		{
		}

		public void OnPerformanceMemAnimation(uint memAnimation)
		{
		}

		public void OnPerformanceMemMaterials(uint memMaterials)
		{
		}

		public void OnPerformanceDeviceMemUsage(long memory)
		{
		}

		private void FinishLogFPS()
		{
			long totalMemory = GC.GetTotalMemory(false);
			float memoryUsed = (float)totalMemory / 1024f / 1024f;
			Service.BILoggingController.TrackPerformance(this.biFps, memoryUsed);
			this.biFps = 0f;
			this.secsToCapture = 0;
		}
	}
}
