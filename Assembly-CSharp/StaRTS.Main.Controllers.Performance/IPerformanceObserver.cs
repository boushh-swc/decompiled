using System;

namespace StaRTS.Main.Controllers.Performance
{
	public interface IPerformanceObserver
	{
		void OnPerformanceFPS(float fps);

		void OnPerformanceFPeak(uint fpeak);

		void OnPerformanceMemRsvd(uint memRsvd);

		void OnPerformanceMemUsed(uint memUsed);

		void OnPerformanceMemTexture(uint memTexture);

		void OnPerformanceMemMesh(uint memMesh);

		void OnPerformanceMemAudio(uint memAudio);

		void OnPerformanceMemAnimation(uint memAnimation);

		void OnPerformanceMemMaterials(uint memMaterials);

		void OnPerformanceDeviceMemUsage(long memory);
	}
}
