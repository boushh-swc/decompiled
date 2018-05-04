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
	public class DeployTroopIdGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private string troopId;

		public DeployTroopIdGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			this.troopId = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(this.troopId))
			{
				Service.Logger.ErrorFormat("Troop ID not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
			Service.EventManager.RegisterObserver(this, EventId.TroopPlacedOnBoard);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.TroopPlacedOnBoard)
			{
				if (this.IsEventValidForGoal())
				{
					SmartEntity smartEntity = (SmartEntity)cookie;
					TroopComponent troopComponent = smartEntity.Get<TroopComponent>();
					TeamComponent teamComponent = smartEntity.Get<TeamComponent>();
					SpawnComponent spawnComponent = smartEntity.Get<SpawnComponent>();
					if (teamComponent != null && teamComponent.TeamType == TeamType.Attacker && troopComponent != null && troopComponent.TroopType.TroopID == this.troopId && (spawnComponent == null || !spawnComponent.IsSummoned()))
					{
						this.parent.Progress(this, 1);
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
