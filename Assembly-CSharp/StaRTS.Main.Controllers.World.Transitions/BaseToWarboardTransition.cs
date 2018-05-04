using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.World.Transitions
{
	public class BaseToWarboardTransition : AbstractTransition
	{
		private const float WIPE_DIRECTION = 1.57079637f;

		public BaseToWarboardTransition(IState transitionToState, IMapDataLoader mapDataLoader, TransitionCompleteDelegate onTransitionComplete) : base(transitionToState, mapDataLoader, onTransitionComplete, false, false, WipeTransition.FromBaseToWarboard, WipeTransition.FromBaseToWarboard)
		{
		}

		public override void StartTransition()
		{
			Service.UserInputInhibitor.DenyAll();
			this.transitioning = true;
			this.worldLoaded = false;
			this.preloadsLoaded = false;
			this.wipeDirection = 1.57079637f;
			EventManager eventManager = Service.EventManager;
			eventManager.SendEvent(EventId.HideAllHolograms, null);
			eventManager.RegisterObserver(this, EventId.WarBoardLoadComplete);
			eventManager.RegisterObserver(this, EventId.WipeCameraSnapshotTaken);
			Service.CameraManager.WipeCamera.TakeSnapshotForDelayedWipe(this.startWipeTransition, this.wipeDirection, new WipeCompleteDelegate(this.OnTransitionInComplete), null);
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.WipeCameraSnapshotTaken)
			{
				if (id == EventId.WarBoardLoadComplete)
				{
					Service.EventManager.UnregisterObserver(this, EventId.WarBoardLoadComplete);
					Service.CameraManager.WipeCamera.ContinueWipe();
					Service.AudioManager.PlayAudio("sfx_ui_squadwar_warboard_open");
				}
			}
			else
			{
				Service.EventManager.UnregisterObserver(this, EventId.WipeCameraSnapshotTaken);
				if (this.transitionToState != null)
				{
					Service.GameStateMachine.SetState(this.transitionToState);
				}
			}
			return EatResponse.NotEaten;
		}

		protected override void OnTransitionInComplete(object cookie)
		{
			CameraManager cameraManager = Service.CameraManager;
			cameraManager.WarBoardCamera.Enable();
			cameraManager.WarBoardCamera.GroundOffset = -10000f;
			cameraManager.MainCamera.Disable();
			Service.UserInputManager.SetActiveWorldCamera(cameraManager.WarBoardCamera);
			Service.UserInputInhibitor.AllowAll();
			base.OnTransitionInComplete(cookie);
		}
	}
}
