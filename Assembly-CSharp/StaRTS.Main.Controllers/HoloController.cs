using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Story.Actions;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class HoloController : IEventObserver
	{
		private EventManager events;

		private HolocommScreen holocommScreen;

		private List<HoloCommand> commandBuffer;

		public HoloController()
		{
			Service.HoloController = this;
			this.commandBuffer = new List<HoloCommand>();
			this.events = Service.EventManager;
			this.events.RegisterObserver(this, EventId.HoloCommScreenDestroyed);
			this.events.RegisterObserver(this, EventId.ShowHologram);
			this.events.RegisterObserver(this, EventId.ShowTranscript);
			this.events.RegisterObserver(this, EventId.PlayHologramAnimation);
			this.events.RegisterObserver(this, EventId.HideHologram);
			this.events.RegisterObserver(this, EventId.HideTranscript);
			this.events.RegisterObserver(this, EventId.ShowNextButton);
			this.events.RegisterObserver(this, EventId.ShowStoreNextButton);
			this.events.RegisterObserver(this, EventId.HideAllHolograms);
			this.events.RegisterObserver(this, EventId.ScreenLoaded);
			this.events.RegisterObserver(this, EventId.ShowInfoPanel);
			this.events.RegisterObserver(this, EventId.HideInfoPanel);
		}

		private void SafeRunThroughCommandBuffer()
		{
			List<HoloCommand> list = new List<HoloCommand>();
			list.AddRange(this.commandBuffer);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null)
				{
					this.OnEvent(list[i].EventId, list[i].Cookie);
				}
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ShowHologram:
			{
				ShowHologramStoryAction showHologramStoryAction = (ShowHologramStoryAction)cookie;
				if (this.EnsureScreenActive())
				{
					this.holocommScreen.ShowHoloCharacter(showHologramStoryAction.Character);
				}
				else
				{
					this.StoreCommandInBuffer(id, cookie);
				}
				return EatResponse.NotEaten;
			}
			case EventId.ShowHologramComplete:
			case EventId.ShowAttackButton:
			case EventId.HideHologramComplete:
				IL_44:
				if (id == EventId.ScreenLoaded)
				{
					if (cookie is HolocommScreen)
					{
						HolocommScreen holocommScreen = (HolocommScreen)cookie;
						if (!holocommScreen.HiddenInQueue)
						{
							this.SafeRunThroughCommandBuffer();
							this.commandBuffer.Clear();
						}
					}
					return EatResponse.NotEaten;
				}
				if (id != EventId.HoloCommScreenDestroyed)
				{
					return EatResponse.NotEaten;
				}
				this.holocommScreen = null;
				if (!(Service.GameStateMachine.CurrentState is GalaxyState))
				{
					Service.UXController.HUD.Visible = true;
				}
				return EatResponse.NotEaten;
			case EventId.ShowTranscript:
			{
				ShowTranscriptStoryAction showTranscriptStoryAction = (ShowTranscriptStoryAction)cookie;
				if (this.EnsureScreenActive())
				{
					this.holocommScreen.AddDialogue(showTranscriptStoryAction.Text, showTranscriptStoryAction.Title);
				}
				else
				{
					this.StoreCommandInBuffer(id, cookie);
				}
				return EatResponse.NotEaten;
			}
			case EventId.PlayHologramAnimation:
			{
				PlayHoloAnimationStoryAction playHoloAnimationStoryAction = (PlayHoloAnimationStoryAction)cookie;
				this.holocommScreen.PlayHoloAnimation(playHoloAnimationStoryAction.AnimName);
				return EatResponse.NotEaten;
			}
			case EventId.HideHologram:
				if (this.holocommScreen != null && this.holocommScreen.IsLoaded())
				{
					this.holocommScreen.CloseAndDestroyHoloCharacter();
				}
				return EatResponse.NotEaten;
			case EventId.HideTranscript:
				if (this.holocommScreen != null && this.holocommScreen.IsLoaded())
				{
					this.holocommScreen.RemoveDialogue();
				}
				return EatResponse.NotEaten;
			case EventId.ShowNextButton:
				if (this.EnsureScreenActive())
				{
					this.holocommScreen.ShowButton("BtnNext");
				}
				else
				{
					this.StoreCommandInBuffer(id, cookie);
				}
				return EatResponse.NotEaten;
			case EventId.ShowStoreNextButton:
				if (this.EnsureScreenActive())
				{
					this.holocommScreen.ShowButton("ButtonStore");
				}
				else
				{
					this.StoreCommandInBuffer(id, cookie);
				}
				return EatResponse.NotEaten;
			case EventId.HideAllHolograms:
				if (this.holocommScreen != null && this.holocommScreen.IsLoaded())
				{
					this.holocommScreen.HideAllElements();
					this.holocommScreen.CloseAndDestroyHoloCharacter();
				}
				return EatResponse.NotEaten;
			case EventId.ShowInfoPanel:
				if (this.EnsureScreenActive())
				{
					ShowHologramInfoStoryAction showHologramInfoStoryAction = (ShowHologramInfoStoryAction)cookie;
					this.holocommScreen.ShowInfoPanel(showHologramInfoStoryAction.ImageName, showHologramInfoStoryAction.DisplayText, showHologramInfoStoryAction.TitleText, showHologramInfoStoryAction.PlanetPanel);
				}
				else
				{
					this.StoreCommandInBuffer(id, cookie);
				}
				return EatResponse.NotEaten;
			case EventId.HideInfoPanel:
				if (this.holocommScreen != null && this.holocommScreen.IsLoaded())
				{
					this.holocommScreen.HideInfoPanel();
				}
				return EatResponse.NotEaten;
			}
			goto IL_44;
		}

		private void StoreCommandInBuffer(EventId id, object cookie)
		{
			HoloCommand holoCommand = new HoloCommand();
			holoCommand.EventId = id;
			holoCommand.Cookie = cookie;
			this.commandBuffer.Add(holoCommand);
		}

		private bool EnsureScreenActive()
		{
			if (this.holocommScreen == null)
			{
				this.holocommScreen = new HolocommScreen();
				if (Service.ScreenController.GetHighestLevelScreen<HQCelebScreen>() != null)
				{
					Service.ScreenController.AddScreen(this.holocommScreen, true, QueueScreenBehavior.QueueAndDeferTillClosed);
				}
				else
				{
					Service.ScreenController.AddScreen(this.holocommScreen, true);
				}
				Service.UXController.HUD.Visible = false;
				return false;
			}
			return this.holocommScreen.IsLoaded() && !this.holocommScreen.HiddenInQueue;
		}

		public Camera GetActiveCamera()
		{
			if (this.holocommScreen != null && this.holocommScreen.IsLoaded())
			{
				return this.holocommScreen.GetHoloCamera();
			}
			return null;
		}

		public bool HasAnyCharacter()
		{
			return this.holocommScreen != null && this.holocommScreen.IsLoaded() && this.holocommScreen.HasCharacterShowing();
		}

		public bool CharacterAlreadyShowing(string characterId)
		{
			return this.holocommScreen != null && this.holocommScreen.IsLoaded() && this.holocommScreen.CharacterAlreadyShowing(characterId);
		}
	}
}
