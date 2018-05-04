using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowScreenStoryAction : AbstractStoryAction, IEventObserver
	{
		private const string PLANET_DETAILS_SCREEN = "PlanetDetails";

		private const string GALAXY_MAP = "GalaxyMap";

		private string screenType;

		private string param;

		private EventManager eventManager;

		public ShowScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
			this.eventManager = Service.EventManager;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				ScreenBase screen = (ScreenBase)cookie;
				this.OnScreenReady(screen);
			}
			return EatResponse.NotEaten;
		}

		public override void Prepare()
		{
			if (string.IsNullOrEmpty(this.vo.PrepareString))
			{
				Service.Logger.Error("ShowScreenStoryAction: " + this.vo.Uid + " : lacks a prepare string");
				return;
			}
			this.screenType = this.prepareArgs[0];
			if (this.prepareArgs.Length > 1)
			{
				this.param = this.prepareArgs[1];
			}
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (this.screenType == "PlanetDetails")
			{
				this.eventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
				IState currentState = Service.GameStateMachine.CurrentState;
				if (currentState is GalaxyState)
				{
					if (string.IsNullOrEmpty(this.param))
					{
						Service.GalaxyViewController.OpenPlanetDetailsForPlanet(currentPlayer.Planet.Uid);
					}
					else
					{
						Service.GalaxyViewController.OpenPlanetDetailsForPlanet(this.param);
					}
				}
				else
				{
					Service.GalaxyViewController.GoToPlanetView(currentPlayer.Planet.Uid, CampaignScreenSection.Main);
				}
				this.screenType = "PlanetDetailsScreen";
			}
			else if (this.screenType == "GalaxyMap")
			{
				Service.GalaxyViewController.GoToGalaxyView();
				this.OnScreenReady(null);
			}
			else
			{
				Service.Logger.Error("ShowScreenStoryAction: " + this.vo.Uid + " : invalid screen type provided: " + this.screenType);
			}
		}

		private void OnScreenReady(ScreenBase screen)
		{
			if (screen != null)
			{
				string name = screen.GetType().Name;
				if (name != this.screenType)
				{
					return;
				}
			}
			this.eventManager.UnregisterObserver(this, EventId.ScreenLoaded);
			this.parent.ChildComplete(this);
		}
	}
}
