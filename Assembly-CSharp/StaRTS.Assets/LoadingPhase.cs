using System;

namespace StaRTS.Assets
{
	public enum LoadingPhase
	{
		Initialized = 0,
		PreLoading = 1,
		LazyLoading = 2,
		OnDemand = 3
	}
}
