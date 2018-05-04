using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.World.Transitions
{
	public class GalaxyMapToBaseTransition : AbstractTransition
	{
		public GalaxyMapToBaseTransition(IState transitionToState, IMapDataLoader mapDataLoader, TransitionCompleteDelegate onTransitionComplete, bool skipTransitions, bool zoomOut) : base(transitionToState, mapDataLoader, onTransitionComplete, skipTransitions, zoomOut, WipeTransition.FromGalaxyToLoadingScreen, WipeTransition.FromLoadingScreenToBase)
		{
		}
	}
}
