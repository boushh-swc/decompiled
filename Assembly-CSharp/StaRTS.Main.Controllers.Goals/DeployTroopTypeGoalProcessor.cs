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
	public class DeployTroopTypeGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private TroopType troopType;

		public DeployTroopTypeGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			string goalItem = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(goalItem))
			{
				Service.Logger.ErrorFormat("Troop type not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
			else
			{
				this.troopType = StringUtils.ParseEnum<TroopType>(goalItem);
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
					if (teamComponent != null && teamComponent.TeamType == TeamType.Attacker && troopComponent != null && troopComponent.TroopType.Type == this.troopType && (spawnComponent == null || !spawnComponent.IsSummoned()))
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
