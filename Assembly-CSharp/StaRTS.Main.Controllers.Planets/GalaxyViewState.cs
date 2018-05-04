using System;

namespace StaRTS.Main.Controllers.Planets
{
	public enum GalaxyViewState
	{
		Loading = 0,
		ManualRotate = 1,
		PlanetTransitionWithinGalaxy = 2,
		PlanetTransitionTowardCamera = 3,
		PlanetTransitionTowardGalaxy = 4,
		PlanetTransitionTowardLeft = 5,
		PlanetTransitionFromLeftTowardGalaxy = 6,
		PlanetTransitionTowardCenter = 7,
		LeftView = 8,
		PlanetViewSwitching = 9,
		PlanetTransitionInstantStart = 10,
		PlanetTransitionPanTo = 11,
		PlanetView = 12
	}
}
