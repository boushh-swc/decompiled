using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story;
using StaRTS.Main.Story.Actions;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

public class PanToPlanetStoryAction : AbstractStoryAction, IEventObserver
{
	private Planet planet;

	public PanToPlanetStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
	{
	}

	public EatResponse OnEvent(EventId id, object cookie)
	{
		if (id != EventId.GalaxyStatePanToPlanetComplete)
		{
			if (id == EventId.PlanetsLoadingComplete)
			{
				Service.EventManager.UnregisterObserver(this, EventId.PlanetsLoadingComplete);
				Service.EventManager.RegisterObserver(this, EventId.GalaxyStatePanToPlanetComplete, EventPriority.Default);
				Service.GalaxyViewController.PanToPlanet(this.planet);
			}
		}
		else
		{
			string value = cookie as string;
			if (!this.planet.VO.Uid.Equals(value))
			{
				return EatResponse.NotEaten;
			}
			Service.EventManager.UnregisterObserver(this, EventId.GalaxyStatePanToPlanetComplete);
			Service.EventManager.UnregisterObserver(this, EventId.PlanetsLoadingComplete);
			this.parent.ChildComplete(this);
		}
		return EatResponse.NotEaten;
	}

	public override void Prepare()
	{
		string planetUID = Service.CurrentPlayer.PlanetId;
		if (!string.IsNullOrEmpty(this.vo.PrepareString))
		{
			if (this.vo.PrepareString.Equals("1stPlaName"))
			{
				string firstPlanetUnlockedUID = Service.CurrentPlayer.GetFirstPlanetUnlockedUID();
				if (!string.IsNullOrEmpty(firstPlanetUnlockedUID))
				{
					planetUID = firstPlanetUnlockedUID;
				}
			}
			else
			{
				planetUID = this.vo.PrepareString;
			}
		}
		this.planet = Service.GalaxyPlanetController.GetPlanet(planetUID);
		IState currentState = Service.GameStateMachine.CurrentState;
		if (this.planet == null)
		{
			if (currentState is GalaxyState)
			{
				Service.Logger.Error("PanToPlanetStoryAction: No Valid planet specified for: " + this.vo.Uid + ": prepare: " + this.vo.PrepareString);
			}
			else
			{
				Service.Logger.Error("PanToPlanetStoryAction: Can't do PanToPlanetStoryAction when not in Galaxy view");
			}
			this.parent.ChildComplete(this);
			return;
		}
		if (!(currentState is GalaxyState))
		{
			Service.Logger.Error("PanToPlanetStoryAction: We're not in Galaxy State");
			this.parent.ChildComplete(this);
			return;
		}
		this.parent.ChildPrepared(this);
	}

	public override void Execute()
	{
		base.Execute();
		if (Service.GalaxyPlanetController.AreAllPlanetsLoaded())
		{
			Service.EventManager.RegisterObserver(this, EventId.GalaxyStatePanToPlanetComplete, EventPriority.Default);
			Service.GalaxyViewController.PanToPlanet(this.planet);
		}
		else
		{
			Service.EventManager.RegisterObserver(this, EventId.PlanetsLoadingComplete, EventPriority.Default);
		}
	}
}
