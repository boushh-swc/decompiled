using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class ConditionFactory
	{
		private const string DELIMITER = "|";

		private const string DESTROY_BUILDING_TYPE = "DestroyBuildingType";

		private const string DESTROY_BUILDING_ID = "DestroyBuildingId";

		private const string DESTROY_BUILDING_UID = "DestroyBuildingUid";

		private const string DESTROY_UNIT_TYPE = "DestroyUnitType";

		private const string DESTROY_UNIT_ID = "DestroyUnitId";

		private const string DESTROY_UNIT_UID = "DestroyUnitUid";

		private const string RETAIN_UNIT_UID = "RetainUnitUid";

		private const string RETAIN_UNIT_ID_LEVEL = "RetainUnitIdLevel";

		private const string RETAIN_UNIT_TYPE_LEVEL = "RetainUnitTypeLevel";

		private const string RETAIN_BUILDING_TYPE = "RetainBuildingType";

		private const string RETAIN_BUILDING_TYPE_LEVEL = "RetainBuildingTypeLevel";

		private const string RETAIN_BUILDING_ID_LEVEL = "RetainBuildingIdLevel";

		private const string RETAIN_BUILDING_UID = "RetainBuildingUid";

		private const string DEPLOY_UNIT_TYPE = "DeployUnitType";

		private const string DEPLOY_UNIT_ID = "DeployUnitId";

		private const string DEPLOY_UNIT_UID = "DeployUnitUid";

		private const string OWN_BUILDING_UID = "OwnBuildingUid";

		private const string OWN_BUILDING_ID_LEVEL = "OwnBuildingIdLevel";

		private const string OWN_BUILDING_TYPE_LEVEL = "OwnBuildingTypeLevel";

		private const string OWN_UNIT_UID = "OwnUnitUid";

		private const string OWN_UNIT_ID_LEVEL = "OwnUnitIdLevel";

		private const string OWN_UNIT_TYPE_LEVEL = "OwnUnitTypeLevel";

		private const string OWN_HERO_UID = "OwnHeroUid";

		private const string OWN_HERO_ID_LEVEL = "OwnHeroIdLevel";

		private const string OWN_HERO_TYPE_LEVEL = "OwnHeroTypeLevel";

		private const string OWN_RESOURCE = "OwnResource";

		private const string TRAIN_UNIT_UID = "TrainUnitUid";

		private const string TRAIN_UNIT_ID_LEVEL = "TrainUnitIdLevel";

		private const string TRAIN_UNIT_TYPE_LEVEL = "TrainUnitTypeLevel";

		private const string TRAIN_HERO_UID = "TrainHeroUid";

		private const string TRAIN_HERO_ID_LEVEL = "TrainHeroIdLevel";

		private const string TRAIN_HERO_TYPE_LEVEL = "TrainHeroTypeLevel";

		private const string COUNT_EVENTS = "CountEvents";

		private const string COLLECT_CURRENCY = "CollectCurrency";

		public const string PVP_START = "PvpStart";

		public const string PVP_WIN = "PvpWin";

		private const string LOOT_CURRENCY = "LootCurrency";

		public static AbstractCondition GenerateCondition(ConditionVO vo, IConditionParent parent)
		{
			return ConditionFactory.GenerateCondition(vo, parent, 0);
		}

		public static AbstractCondition GenerateCondition(ConditionVO vo, IConditionParent parent, int startingValue)
		{
			AbstractCondition result;
			try
			{
				string conditionType = vo.ConditionType;
				switch (conditionType)
				{
				case "DestroyBuildingType":
					result = new DestroyBuildingTypeCondition(vo, parent);
					return result;
				case "DestroyBuildingId":
					result = new DestroyBuildingIdCondition(vo, parent);
					return result;
				case "DestroyBuildingUid":
					result = new DestroyBuildingUidCondition(vo, parent);
					return result;
				case "DestroyUnitType":
					result = new DestroyUnitTypeCondition(vo, parent);
					return result;
				case "DestroyUnitId":
					result = new DestroyUnitIdCondition(vo, parent);
					return result;
				case "DestroyUnitUid":
					result = new DestroyUnitUidCondition(vo, parent);
					return result;
				case "RetainBuildingType":
				case "RetainBuildingTypeLevel":
					result = new RetainBuildingCondition(vo, parent, ConditionMatchType.Type);
					return result;
				case "RetainBuildingIdLevel":
					result = new RetainBuildingCondition(vo, parent, ConditionMatchType.Type);
					return result;
				case "RetainBuildingUid":
					result = new RetainBuildingCondition(vo, parent, ConditionMatchType.Type);
					return result;
				case "RetainUnitTypeLevel":
					result = new RetainUnitCondition(vo, parent, ConditionMatchType.Type);
					return result;
				case "RetainUnitIdLevel":
					result = new RetainUnitCondition(vo, parent, ConditionMatchType.Id);
					return result;
				case "RetainUnitUid":
					result = new RetainUnitCondition(vo, parent, ConditionMatchType.Uid);
					return result;
				case "OwnBuildingUid":
					result = new OwnBuildingCondition(vo, parent, ConditionMatchType.Uid);
					return result;
				case "OwnBuildingIdLevel":
					result = new OwnBuildingCondition(vo, parent, ConditionMatchType.Id);
					return result;
				case "OwnBuildingTypeLevel":
					result = new OwnBuildingCondition(vo, parent, ConditionMatchType.Type);
					return result;
				case "OwnUnitUid":
					result = new OwnUnitCondition(vo, parent, ConditionMatchType.Uid, TroopType.Infantry);
					return result;
				case "OwnUnitIdLevel":
					result = new OwnUnitCondition(vo, parent, ConditionMatchType.Id, TroopType.Infantry);
					return result;
				case "OwnUnitTypeLevel":
					result = new OwnUnitCondition(vo, parent, ConditionMatchType.Type, TroopType.Infantry);
					return result;
				case "OwnHeroUid":
					result = new OwnUnitCondition(vo, parent, ConditionMatchType.Uid, TroopType.Hero);
					return result;
				case "OwnHeroIdLevel":
					result = new OwnUnitCondition(vo, parent, ConditionMatchType.Id, TroopType.Hero);
					return result;
				case "OwnHeroTypeLevel":
					result = new OwnUnitCondition(vo, parent, ConditionMatchType.Type, TroopType.Hero);
					return result;
				case "OwnResource":
					result = new OwnResourceCondition(vo, parent);
					return result;
				case "DeployUnitId":
					result = new DeployUnitIdCondition(vo, parent);
					return result;
				case "DeployUnitType":
					result = new DeployUnitTypeCondition(vo, parent);
					return result;
				case "DeployUnitUid":
					result = new DeployUnitUidCondition(vo, parent);
					return result;
				case "CountEvents":
					result = new CountEventsCondition(vo, parent, startingValue);
					return result;
				case "CollectCurrency":
					result = new CollectCurrencyCondition(vo, parent, startingValue);
					return result;
				case "LootCurrency":
					result = new LootCurrencyCondition(vo, parent, startingValue);
					return result;
				case "PvpStart":
					result = new CountEventsCondition(vo, parent, startingValue, "pvp_battle_started");
					return result;
				case "PvpWin":
					result = new CountEventsCondition(vo, parent, startingValue, "pvp_battle_won");
					return result;
				case "TrainUnitUid":
					result = new TrainUnitCondition(vo, parent, startingValue, ConditionMatchType.Uid);
					return result;
				case "TrainUnitIdLevel":
					result = new TrainUnitCondition(vo, parent, startingValue, ConditionMatchType.Id);
					return result;
				case "TrainUnitTypeLevel":
					result = new TrainUnitCondition(vo, parent, startingValue, ConditionMatchType.Type);
					return result;
				case "TrainHeroUid":
					result = new TrainUnitCondition(vo, parent, startingValue, ConditionMatchType.Uid);
					return result;
				case "TrainHeroIdLevel":
					result = new TrainUnitCondition(vo, parent, startingValue, ConditionMatchType.Id);
					return result;
				case "TrainHeroTypeLevel":
					result = new TrainUnitCondition(vo, parent, startingValue, ConditionMatchType.Type);
					return result;
				}
				Service.Logger.ErrorFormat("Unrecognized condition {0} in {1}", new object[]
				{
					vo.ConditionType,
					vo.Uid
				});
				result = new DegenerateCondition(vo, parent);
			}
			catch (Exception ex)
			{
				Service.Logger.ErrorFormat("Invalid condition detected in uid {0}. {1}:{2}", new object[]
				{
					vo.Uid,
					vo.ConditionType,
					vo.PrepareString
				});
				throw ex;
			}
			return result;
		}
	}
}
