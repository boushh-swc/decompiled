using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Planets;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UserInput
{
	public class BackButtonManager : IEventObserver, IBackButtonManager, IViewFrameTimeObserver
	{
		private List<IBackButtonHandler> backButtonHandlers;

		public BackButtonManager()
		{
			Service.IBackButtonManager = this;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.backButtonHandlers = new List<IBackButtonHandler>();
		}

		public void RegisterBackButtonHandler(IBackButtonHandler handler)
		{
			if (!this.backButtonHandlers.Contains(handler))
			{
				this.backButtonHandlers.Add(handler);
			}
		}

		public void UnregisterBackButtonHandler(IBackButtonHandler handler)
		{
			if (this.backButtonHandlers.Contains(handler))
			{
				this.backButtonHandlers.Remove(handler);
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				this.HandleBackButton();
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ScreenLoaded)
			{
				if (id == EventId.NativeAlertBoxDismissed)
				{
					Service.EventManager.UnregisterObserver(this, EventId.NativeAlertBoxDismissed);
					if ((bool)cookie)
					{
						Application.Quit();
					}
				}
			}
			else
			{
				ClosableScreen closableScreen = cookie as ClosableScreen;
				if (closableScreen != null)
				{
					closableScreen.CloseButton.Visible = false;
					for (int i = 0; i < closableScreen.BackButtons.Count; i++)
					{
						closableScreen.BackButtons[i].Visible = false;
					}
				}
			}
			return EatResponse.NotEaten;
		}

		private void HandleBackButton()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is IntroCameraState)
			{
				IntroCameraAnimation intro = Service.UXController.Intro;
				if (intro != null)
				{
					intro.FinishUp(true);
				}
				return;
			}
			for (int i = this.backButtonHandlers.Count - 1; i >= 0; i--)
			{
				if (this.backButtonHandlers[i].HandleBackButtonPress())
				{
					return;
				}
			}
			UICamera.selectedObject = null;
			UICamera.hoveredObject = null;
			if (TouchScreenKeyboard.visible)
			{
				return;
			}
			ScreenController screenController = null;
			if (Service.ScreenController != null)
			{
				screenController = Service.ScreenController;
				AlertScreen highestLevelScreen = screenController.GetHighestLevelScreen<AlertScreen>();
				if (highestLevelScreen != null && highestLevelScreen.IsAlwaysOnTop)
				{
					this.HandleScreenBack(highestLevelScreen);
					return;
				}
			}
			if (Service.HoloController != null)
			{
				HoloController holoController = Service.HoloController;
				if (holoController.HasAnyCharacter())
				{
					Service.EventManager.SendEvent(EventId.StoryNextButtonClicked, null);
					return;
				}
			}
			if (screenController != null)
			{
				ScreenBase highestLevelScreen2 = screenController.GetHighestLevelScreen<ScreenBase>();
				ClosableScreen highestLevelScreen3 = screenController.GetHighestLevelScreen<ClosableScreen>();
				if (Service.CurrentPlayer.CampaignProgress.FueInProgress || Service.UserInputInhibitor.IsDenying())
				{
					if (highestLevelScreen2 != null && highestLevelScreen2.AllowFUEBackButton && this.HandleScreenBack(highestLevelScreen2))
					{
						return;
					}
					this.TryQuit();
					return;
				}
				else
				{
					if (highestLevelScreen2 != null && this.HandleScreenBack(highestLevelScreen2))
					{
						return;
					}
					if (highestLevelScreen3 != null && this.HandleScreenBack(highestLevelScreen3))
					{
						return;
					}
				}
			}
			if (currentState is EditBaseState)
			{
				HomeState.GoToHomeState(null, false);
				return;
			}
			if (Service.BuildingController != null)
			{
				BuildingController buildingController = Service.BuildingController;
				if (buildingController.SelectedBuilding != null)
				{
					buildingController.EnsureDeselectSelectedBuilding();
					return;
				}
			}
			if (currentState is BaseLayoutToolState)
			{
				UXController uXController = Service.UXController;
				uXController.HUD.BaseLayoutToolView.CancelBaseLayoutTool();
				return;
			}
			if (Service.GalaxyViewController != null && currentState is GalaxyState)
			{
				GalaxyViewController galaxyViewController = Service.GalaxyViewController;
				galaxyViewController.GoToHome();
				return;
			}
			this.TryQuit();
		}

		private bool HandleScreenBack(ScreenBase screen)
		{
			bool result = false;
			if (screen.CurrentBackDelegate != null)
			{
				result = true;
				screen.CurrentBackDelegate(screen.CurrentBackButton);
				if (screen.CurrentBackButton != null)
				{
					Service.EventManager.SendEvent(EventId.ButtonClicked, screen.CurrentBackButton.Root.name);
				}
			}
			return result;
		}

		private void TryQuit()
		{
			Service.EventManager.RegisterObserver(this, EventId.NativeAlertBoxDismissed, EventPriority.Default);
			string titleText = Service.Lang.Get("EXIT_APP_SCREEN_TITLE", new object[0]);
			string messageText = Service.Lang.Get("EXIT_APP_SCREEN_MESSAGE", new object[0]);
			string yesButtonText = Service.Lang.Get("EXIT_APP_SCREEN_YES_BUTTON", new object[0]);
			string noButtonText = Service.Lang.Get("EXIT_APP_SCREEN_NO_BUTTON", new object[0]);
			Service.EnvironmentController.ShowAlert(titleText, messageText, yesButtonText, noButtonText);
		}
	}
}
