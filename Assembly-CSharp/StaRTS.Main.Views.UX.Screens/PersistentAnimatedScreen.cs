using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class PersistentAnimatedScreen : ScreenBase
	{
		private PersistentScreenState AnimatingState;

		private Animation openCloseAnim;

		private AnimationState animState;

		private uint animationTimer;

		protected bool shouldCloseOnAnimComplete;

		protected object closeModalResult;

		protected PersistentAnimatedScreen(string assetName) : base(assetName)
		{
			this.animationTimer = 0u;
			this.AnimatingState = PersistentScreenState.Closed;
			this.shouldCloseOnAnimComplete = false;
			this.closeModalResult = null;
		}

		protected void InitAnimations(string animationElementName, string animationName)
		{
			UXElement element = base.GetElement<UXElement>(animationElementName);
			this.openCloseAnim = element.Root.GetComponent<Animation>();
			this.animState = this.openCloseAnim[animationName];
		}

		private void PlayAnimation(bool openScreen)
		{
			if (openScreen)
			{
				this.AnimatingState = PersistentScreenState.Opening;
				this.animState.speed = 1f;
				this.animState.time = 0f;
			}
			else
			{
				this.AnimatingState = PersistentScreenState.Closing;
				this.animState.speed = -1f;
				this.animState.time = this.animState.length;
			}
			this.animationTimer = Service.ViewTimerManager.CreateViewTimer(this.animState.length, false, new TimerDelegate(this.OnAnimationComplete), null);
			this.openCloseAnim.Play();
			Service.EventManager.SendEvent(EventId.UpdateScrim, null);
		}

		public void InstantClose(bool shouldCloseOnAnimComplete, object modalResult)
		{
			this.ClearDefaultBackDelegate();
			this.shouldCloseOnAnimComplete = shouldCloseOnAnimComplete;
			this.closeModalResult = modalResult;
			this.AnimatingState = PersistentScreenState.Closing;
			this.OnAnimationComplete(0u, null);
			Service.EventManager.SendEvent(EventId.UpdateScrim, null);
		}

		protected virtual void OnAnimationComplete(uint id, object cookie)
		{
			PersistentScreenState animatingState = this.AnimatingState;
			if (animatingState != PersistentScreenState.Closing)
			{
				if (animatingState == PersistentScreenState.Opening)
				{
					this.AnimatingState = PersistentScreenState.Open;
					this.openCloseAnim.Play();
					this.animState.time = this.animState.length;
					this.openCloseAnim.Sample();
					this.openCloseAnim.Stop();
					this.OnOpen();
				}
			}
			else
			{
				this.AnimatingState = PersistentScreenState.Closed;
				this.openCloseAnim.Play();
				this.animState.time = 0f;
				this.openCloseAnim.Sample();
				this.openCloseAnim.Stop();
				this.OnClose();
				if (this.shouldCloseOnAnimComplete)
				{
					object modalResult = this.closeModalResult;
					this.Close(modalResult);
				}
			}
			this.animationTimer = 0u;
		}

		protected void ClearCloseOnAnimFlags()
		{
			this.closeModalResult = null;
			this.shouldCloseOnAnimComplete = false;
		}

		public virtual void AnimateOpen()
		{
			if (this.IsClosed())
			{
				this.OnOpening();
				this.PlayAnimation(true);
			}
		}

		public void AnimateClosed(bool closeOnFinish, object modalResult)
		{
			if (this.IsOpen())
			{
				this.PlayAnimation(false);
				this.ClearDefaultBackDelegate();
				this.shouldCloseOnAnimComplete = closeOnFinish;
				this.closeModalResult = modalResult;
				this.OnClosing();
			}
		}

		public override void SetupRootCollider()
		{
		}

		public bool IsOpen()
		{
			return this.AnimatingState == PersistentScreenState.Open;
		}

		public bool IsOpening()
		{
			return this.AnimatingState == PersistentScreenState.Opening;
		}

		public bool IsClosed()
		{
			return this.AnimatingState == PersistentScreenState.Closed;
		}

		public bool IsAnimClosing()
		{
			return this.AnimatingState == PersistentScreenState.Closing;
		}

		private void HandleBackButton(UXButton btn)
		{
			this.AnimateClosed(false, null);
		}

		public void SetDefaultBackDelegate()
		{
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.HandleBackButton);
			base.CurrentBackButton = null;
		}

		protected void ClearDefaultBackDelegate()
		{
			base.CurrentBackDelegate = null;
			base.CurrentBackButton = null;
		}

		protected virtual void OnOpening()
		{
			this.SetDefaultBackDelegate();
			Service.UXController.HUD.Visible = false;
			Service.UserInputInhibitor.DenyAll();
		}

		protected virtual void OnOpen()
		{
			Service.UserInputInhibitor.AllowAll();
		}

		protected virtual void OnClosing()
		{
			this.ClearDefaultBackDelegate();
			Service.UserInputInhibitor.DenyAll();
		}

		protected virtual void OnClose()
		{
			if (Service.GameStateMachine.CurrentState is HomeState)
			{
				Service.UXController.HUD.Visible = true;
			}
			Service.UserInputInhibitor.AllowAll();
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimerManager.EnsureTimerKilled(ref this.animationTimer);
			base.OnDestroyElement();
		}

		public override void Close(object modalResult)
		{
			this.ClearDefaultBackDelegate();
			base.Close(modalResult);
		}
	}
}
