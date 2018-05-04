using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.World
{
	public class WorldTransitioner
	{
		private const string START_TRANSITION_ERROR = "Transition in progress, unable to set another transition.";

		private const string TRANSITION_IN_PROGRESS_WIPE_ERROR = "Transition in progress, unable to start another wipe.";

		private const string TRANSITION_NOT_SET_ERROR = "WorldTransitioner.currentTransition is not set.";

		public const string DEFAULT_PLANET = "planet1";

		private AbstractTransition currentTransition;

		public WorldTransitioner()
		{
			Service.WorldTransitioner = this;
		}

		public void StartTransition(AbstractTransition transition)
		{
			if (this.currentTransition != null && this.currentTransition.IsTransitioning())
			{
				Service.Logger.Error("Transition in progress, unable to set another transition.");
				return;
			}
			this.currentTransition = transition;
			this.currentTransition.StartTransition();
		}

		public void StartWipe(AbstractTransition transition)
		{
			this.StartWipe(transition, Service.StaticDataController.Get<PlanetVO>("planet1"));
		}

		public void StartWipe(AbstractTransition transition, PlanetVO planetVO)
		{
			if (this.currentTransition != null && this.currentTransition.IsTransitioning())
			{
				Service.Logger.Error("Transition in progress, unable to start another wipe.");
				return;
			}
			this.currentTransition = transition;
			this.currentTransition.StartWipe(planetVO);
		}

		public void ContinueWipe(IState transitionToState, IMapDataLoader mapDataLoader, TransitionCompleteDelegate onTransitionComplete)
		{
			if (!this.IsCurrentTransitionSet())
			{
				return;
			}
			this.currentTransition.ContinueWipe(transitionToState, mapDataLoader, onTransitionComplete);
		}

		public AbstractTransition GetCurrentTransition()
		{
			return this.currentTransition;
		}

		public void FinishWipe()
		{
			if (!this.IsCurrentTransitionSet())
			{
				return;
			}
			this.currentTransition.FinishWipe();
		}

		public bool IsSoftWiping()
		{
			return this.IsCurrentTransitionSet() && this.currentTransition.IsSoftWipeing();
		}

		public bool IsEverythingLoaded()
		{
			return this.IsCurrentTransitionSet() && this.currentTransition.IsEverythingLoaded();
		}

		public bool IsCurrentWorldHome()
		{
			return this.IsCurrentTransitionSet() && this.currentTransition.IsCurrentWorldHome();
		}

		public bool IsCurrentWorldUserWarBase()
		{
			return this.IsCurrentTransitionSet() && this.currentTransition.IsCurrentWorldUserWarBase();
		}

		public string GetCurrentWorldName()
		{
			if (!this.IsCurrentTransitionSet())
			{
				return string.Empty;
			}
			return this.currentTransition.GetCurrentWorldName();
		}

		public string GetCurrentWorldFactionAssetName()
		{
			if (!this.IsCurrentTransitionSet())
			{
				return string.Empty;
			}
			return this.currentTransition.GetCurrentWorldFactionAssetName();
		}

		public void SetTransitionInStartCallback(TransitionInStartDelegate startCallback)
		{
			if (!this.IsCurrentTransitionSet())
			{
				return;
			}
			this.currentTransition.SetOnTransitionInStart(startCallback);
		}

		public void SetAlertMessage(string alertMessage)
		{
			if (!this.IsCurrentTransitionSet())
			{
				return;
			}
			this.currentTransition.SetAlertMessage(alertMessage);
		}

		public void SetSkipTransitions(bool skipTransitions)
		{
			if (!this.IsCurrentTransitionSet())
			{
				return;
			}
			this.currentTransition.SetSkipTransitions(skipTransitions);
		}

		public IMapDataLoader GetMapDataLoader()
		{
			if (!this.IsCurrentTransitionSet())
			{
				return null;
			}
			return this.currentTransition.GetMapDataLoader();
		}

		public bool IsTransitioning()
		{
			return this.IsCurrentTransitionSet() && this.currentTransition.IsTransitioning();
		}

		private bool IsCurrentTransitionSet()
		{
			if (this.currentTransition == null)
			{
				Service.Logger.Error("WorldTransitioner.currentTransition is not set.");
				return false;
			}
			return true;
		}
	}
}
