using StaRTS.Main.Controllers.Goals;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public static class GoalFactory
	{
		public static BaseGoalProcessor GetProcessor(IValueObject vo, AbstractGoalManager parent)
		{
			switch (parent.GetGoalType(vo))
			{
			case GoalType.Invalid:
				Service.Logger.ErrorFormat("Goal type not found for goal {0}", new object[]
				{
					vo.Uid
				});
				break;
			case GoalType.Loot:
				return new LootGoalProcessor(vo, parent);
			case GoalType.DestroyBuilding:
				return new DestroyBuildingGoalProcessor(vo, parent);
			case GoalType.DestroyBuildingType:
				return new DestroyBuildingTypeGoalProcessor(vo, parent);
			case GoalType.DestroyBuildingID:
				return new DestroyBuildingIdGoalProcessor(vo, parent);
			case GoalType.DeployTroop:
				return new DeployTroopGoalProcessor(vo, parent);
			case GoalType.DeployTroopType:
				return new DeployTroopTypeGoalProcessor(vo, parent);
			case GoalType.DeployTroopID:
				return new DeployTroopIdGoalProcessor(vo, parent);
			case GoalType.TrainTroop:
				return new TrainTroopGoalProcessor(vo, parent);
			case GoalType.TrainTroopType:
				return new TrainTroopTypeGoalProcessor(vo, parent);
			case GoalType.TrainTroopID:
				return new TrainTroopIdGoalProcessor(vo, parent);
			case GoalType.ReceiveDonatedTroops:
				return new ReceiveSquadUnitGoalProcessor(vo, parent);
			case GoalType.DeploySpecialAttack:
				return new DeploySpecialAttackGoalProcessor(vo, parent);
			case GoalType.DeploySpecialAttackID:
				return new DeploySpecialAttackIdGoalProcessor(vo, parent);
			case GoalType.TrainSpecialAttack:
				return new TrainSpecialAttackGoalProcessor(vo, parent);
			case GoalType.TrainSpecialAttackID:
				return new TrainSpecialAttackIdGoalProcessor(vo, parent);
			case GoalType.DonateTroopType:
				return new DonateTroopTypeGoalProcessor(vo, parent);
			case GoalType.DonateTroopID:
				return new DonateTroopIdGoalProcessor(vo, parent);
			case GoalType.DonateTroop:
				return new DonateTroopGoalProcessor(vo, parent);
			case GoalType.EpisodePoint:
				return new EpisodePointGoalProcessor(vo, parent);
			}
			return new BaseGoalProcessor(vo, parent);
		}
	}
}
