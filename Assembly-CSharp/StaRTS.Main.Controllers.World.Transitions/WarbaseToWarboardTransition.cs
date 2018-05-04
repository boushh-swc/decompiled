using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.World.Transitions
{
	public class WarbaseToWarboardTransition : AbstractTransition
	{
		public WarbaseToWarboardTransition(IState transitionToState, IMapDataLoader mapDataLoader, TransitionCompleteDelegate onTransitionComplete, bool skipTransitions, bool zoomOut) : base(transitionToState, mapDataLoader, onTransitionComplete, skipTransitions, zoomOut, WipeTransition.FromStoryToLoadingScreen, WipeTransition.FromLoadingScreenToWarboard)
		{
		}

		protected override void StartTransitionIn()
		{
			this.StartTransitionInContinueSetup();
		}
	}
}
