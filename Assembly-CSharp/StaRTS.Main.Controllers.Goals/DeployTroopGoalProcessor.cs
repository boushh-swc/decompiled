using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class DeployTroopGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		public DeployTroopGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			Service.EventManager.RegisterObserver(this, EventId.TroopPlacedOnBoard);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.TroopPlacedOnBoard)
			{
				if (this.IsEventValidForGoal())
				{
					SmartEntity smartEntity = (SmartEntity)cookie;
					if (smartEntity != null)
					{
						TeamComponent teamComponent = smartEntity.Get<TeamComponent>();
						SpawnComponent spawnComponent = smartEntity.Get<SpawnComponent>();
						TroopComponent troopComponent = smartEntity.Get<TroopComponent>();
						if (troopComponent.TroopType.Type != TroopType.Hero)
						{
							if (troopComponent != null && teamComponent != null && teamComponent.TeamType == TeamType.Attacker && (spawnComponent == null || !spawnComponent.IsSummoned()))
							{
								this.parent.Progress(this, troopComponent.TroopType.Size);
							}
						}
					}
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.TroopPlacedOnBoard);
			base.Destroy();
		}
	}
}
