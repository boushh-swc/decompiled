using StaRTS.Assets;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ScreenBase : UXFactory
	{
		private const string TRANSITION_IN = "guiAnim_dialogIn";

		private const string TRANSITION_OUT = "guiAnim_dialogOut";

		private const string MAIN_WIDGET_POSTFIX = "_main_widget";

		private const string CURRENCY_TRAY = "CurrencyTray";

		private object modalResult;

		private string assetName;

		private AssetHandle assetHandle;

		protected Lang lang;

		private UIPanel rootPanel;

		private ScreenTransition transition;

		private bool closing;

		public List<UXButton> BackButtons;

		private bool isWaitingToShow;

		public OnScreenModalResult OnModalResult
		{
			get;
			set;
		}

		public object ModalResultCookie
		{
			get;
			set;
		}

		public OnTransInComplete OnTransitionInComplete
		{
			get;
			set;
		}

		public bool TransitionedIn
		{
			get;
			private set;
		}

		public string AssetName
		{
			get
			{
				return this.assetName;
			}
		}

		public bool IsClosing
		{
			get
			{
				return this.closing;
			}
		}

		protected virtual bool IsFullScreen
		{
			get
			{
				return false;
			}
		}

		protected virtual bool WantTransitions
		{
			get
			{
				return true;
			}
		}

		protected virtual bool AllowGarbageCollection
		{
			get
			{
				return true;
			}
		}

		public virtual bool ShowCurrencyTray
		{
			get
			{
				return false;
			}
		}

		public bool AllowFUEBackButton
		{
			get;
			set;
		}

		public bool IsAlwaysOnTop
		{
			get;
			set;
		}

		public UXButtonClickedDelegate CurrentBackDelegate
		{
			get;
			set;
		}

		public UXButton CurrentBackButton
		{
			get;
			set;
		}

		public ScreenBase(string assetName) : base(Service.CameraManager.UXCamera)
		{
			this.isWaitingToShow = false;
			this.assetName = assetName;
			this.lang = Service.Lang;
			this.transition = null;
			this.closing = false;
			this.AllowFUEBackButton = false;
			this.BackButtons = new List<UXButton>();
			this.TransitionedIn = false;
			base.Load(ref this.assetHandle, assetName, new UXFactoryLoadDelegate(this.OnScreenLoadSuccess), null, null);
		}

		public override void SetupRootCollider()
		{
			this.SetupRootPanel();
			if (this.WantTransitions)
			{
				Animation animation = this.root.GetComponent<Animation>();
				if (animation == null && Service.UXController != null)
				{
					animation = Service.UXController.MiscElementsManager.AddScreenTransitionAnimation(this.root);
				}
				if (animation != null)
				{
					this.transition = new ScreenTransition(animation);
				}
			}
			this.TransitionIn();
		}

		private void SetupRootPanel()
		{
			this.rootPanel = this.root.GetComponent<UIPanel>();
			if (this.rootPanel == null)
			{
				this.rootPanel = this.root.GetComponentInChildren<UIPanel>();
				if (this.rootPanel == null)
				{
					this.rootPanel = this.root.GetComponentInParent<UIPanel>();
				}
			}
		}

		public int GetRootPanelDepth()
		{
			if (this.rootPanel == null)
			{
				this.SetupRootPanel();
			}
			return (!(this.rootPanel == null)) ? this.rootPanel.depth : 0;
		}

		private void TransitionIn()
		{
			if (this.transition != null)
			{
				this.transition.PlayTransition("guiAnim_dialogIn", new OnScreenTransitionComplete(this.TransitionInComplete), true);
			}
			else
			{
				this.TransitionInComplete();
			}
		}

		private void TransitionOut()
		{
			if (this.IsFullScreen && Service.CameraManager != null)
			{
				Service.CameraManager.MainCamera.Enable();
			}
			this.TransitionedIn = false;
			if (this.transition != null && this.WantTransitions)
			{
				this.transition.PlayTransition("guiAnim_dialogOut", new OnScreenTransitionComplete(this.TransitionOutComplete), false);
			}
			else
			{
				this.TransitionOutComplete();
			}
		}

		protected float GetAlpha()
		{
			return (!(this.rootPanel != null)) ? 1f : this.rootPanel.alpha;
		}

		private void DestroyTransition()
		{
			if (this.transition != null)
			{
				this.transition.Destroy();
				this.transition = null;
			}
		}

		private void OnScreenLoadSuccess(object cookie)
		{
			if (!this.isWaitingToShow)
			{
				this.HandleScreenLoaded();
			}
		}

		public void OnScreenAddedToQueue()
		{
			this.isWaitingToShow = true;
		}

		public void OnScreeenPoppedFromQueue()
		{
			this.isWaitingToShow = false;
			this.HandleScreenLoaded();
		}

		private void HandleScreenLoaded()
		{
			if (base.IsLoaded())
			{
				this.OnScreenLoaded();
				if (this.ShowCurrencyTray)
				{
					this.UpdateCurrencyTrayAttachment();
				}
				Service.EventManager.SendEvent(EventId.ScreenLoaded, this);
			}
		}

		public void UpdateCurrencyTrayAttachment()
		{
			UXElement uXElement = base.GetOptionalElement<UXElement>("CurrencyTray");
			if (uXElement == null)
			{
				if (base.HasCollider())
				{
					uXElement = this;
				}
				else
				{
					uXElement = base.GetElement<UXElement>(this.root.name + "_main_widget");
				}
			}
			if (uXElement != null)
			{
				Service.UXController.MiscElementsManager.AttachCurrencyTrayToScreen(uXElement, this.GetDisplayCurrencyTrayType());
			}
			else
			{
				Service.Logger.Warn("Cannot attach currency tray");
			}
		}

		protected virtual CurrencyTrayType GetDisplayCurrencyTrayType()
		{
			return CurrencyTrayType.Default;
		}

		protected virtual void OnScreenLoaded()
		{
		}

		public void CloseNoTransition(object modalResult)
		{
			this.DestroyTransition();
			this.Close(modalResult);
		}

		public virtual void Close(object modalResult)
		{
			if (this.closing)
			{
				return;
			}
			this.closing = true;
			if (Service.ScreenController != null && !(Service.GameStateMachine.CurrentState is ApplicationLoadState) && !(Service.GameStateMachine.CurrentState is GalaxyState))
			{
				Service.ScreenController.RecalculateHudVisibility();
			}
			if (Service.EventManager != null)
			{
				Service.EventManager.SendEvent(EventId.ScreenClosing, this);
			}
			this.modalResult = modalResult;
			this.TransitionOut();
		}

		private void TransitionInComplete()
		{
			if (this.AllowGarbageCollection)
			{
				Service.Engine.ForceGarbageCollection(null);
			}
			if (this.IsFullScreen && Service.CameraManager != null)
			{
				Service.CameraManager.MainCamera.Disable();
			}
			this.TransitionedIn = true;
			if (this.OnTransitionInComplete != null)
			{
				this.OnTransitionInComplete();
				this.OnTransitionInComplete = null;
			}
		}

		private void TransitionOutComplete()
		{
			if (this.OnModalResult != null)
			{
				this.OnModalResult(this.modalResult, this.ModalResultCookie);
			}
			if (Service.EventManager != null)
			{
				Service.EventManager.SendEvent(EventId.ScreenClosed, this);
			}
			if (Service.ViewTimerManager == null)
			{
				this.DestroyScreen();
				return;
			}
			Service.ViewTimerManager.CreateViewTimer(0.001f, false, new TimerDelegate(this.DestroyScreenOnTimer), null);
		}

		protected void CleanupScreenTransition(bool visible)
		{
			if (!this.WantTransitions)
			{
				Animation component = base.Root.GetComponent<Animation>();
				if (component != null)
				{
					if (component.isPlaying)
					{
						component.Stop();
					}
					ScreenTransition.ForceAlpha(component, (float)((!visible) ? 0 : 1));
				}
			}
		}

		private void DestroyScreenOnTimer(uint timerId, object cookie)
		{
			this.DestroyScreen();
		}

		public override void OnDestroyElement()
		{
			this.DestroyTransition();
			if (this.assetHandle != AssetHandle.Invalid)
			{
				base.Unload(this.assetHandle, this.assetName);
				this.assetHandle = AssetHandle.Invalid;
			}
			if (Service.ScreenController != null)
			{
				Service.ScreenController.RemoveScreen(this);
				Service.ScreenController.RecalculateCurrencyTrayVisibility();
			}
			base.OnDestroyElement();
		}

		public void DestroyScreen()
		{
			base.DestroyFactory();
		}
	}
}
