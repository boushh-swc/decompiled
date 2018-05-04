using System;

namespace StaRTS.Main.Views.Cameras
{
	public enum WipeTransition
	{
		FromIntroToBase = 0,
		FromBaseToBase = 1,
		FromStoryToLoadingScreen = 2,
		FromLoadingScreenToBase = 3,
		FromGalaxyToHyperspace = 4,
		FromHyperspaceToBase = 5,
		FromBaseToGalaxy = 6,
		FromGalaxyToBase = 7,
		FromGalaxyToLoadingScreen = 8,
		FromLoadingScreenToWarboard = 9,
		FromWarboardToLoadingScreen = 10,
		FromBaseToWarboard = 11,
		FromWarboardToBase = 12
	}
}
