using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story;
using StaRTS.Main.Story.Trigger;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

public class GalaxyMapOpenStoryTrigger : AbstractStoryTrigger, IEventObserver
{
	public GalaxyMapOpenStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
	{
	}

	public EatResponse OnEvent(EventId id, object cookie)
	{
		if (id == EventId.GalaxyViewMapOpenComplete || id == EventId.ReturnToGalaxyViewMapComplete)
		{
			if (!Service.GalaxyViewController.IsPlanetDetailsScreenOpen())
			{
				Service.EventManager.UnregisterObserver(this, EventId.GalaxyViewMapOpenComplete);
				Service.EventManager.UnregisterObserver(this, EventId.ReturnToGalaxyViewMapComplete);
				this.parent.SatisfyTrigger(this);
			}
		}
		return EatResponse.NotEaten;
	}

	public override void Activate()
	{
		if (!(Service.GameStateMachine.CurrentState is GalaxyState))
		{
			Service.EventManager.RegisterObserver(this, EventId.GalaxyViewMapOpenComplete, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.ReturnToGalaxyViewMapComplete, EventPriority.Default);
			base.Activate();
			return;
		}
		if (Service.GalaxyViewController.IsPlanetDetailsScreenOpen())
		{
			Service.Logger.WarnFormat("GalaxyMapOpenStoryTrigger: {0} : You tried to do a GalaxyMapOpen whileThe PlanetDetailsScreen was open.", new object[]
			{
				this.vo.Uid
			});
			return;
		}
		this.parent.SatisfyTrigger(this);
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
