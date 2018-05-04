using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class OwnUnitCondition : AbstractCondition, IEventObserver
	{
		private const string ANY_STRING = "any";

		private const int COUNT_ARG = 0;

		private const int BUILDING_ID_ARG = 1;

		private const int MIN_LEVEL_ARG = 2;

		private int threshold;

		private string unitId;

		private int level;

		private bool any;

		private ConditionMatchType matchType;

		private TroopType troopType;

		private bool observingEvents;

		public OwnUnitCondition(ConditionVO vo, IConditionParent parent, ConditionMatchType matchType, TroopType troopType) : base(vo, parent)
		{
			this.threshold = Convert.ToInt32(this.prepareArgs[0]);
			this.unitId = this.prepareArgs[1];
			this.matchType = matchType;
			this.troopType = troopType;
			this.any = (this.unitId == "any");
			if (matchType == ConditionMatchType.Uid)
			{
				this.level = Service.StaticDataController.Get<TroopTypeVO>(this.unitId).Lvl;
			}
			else if (!this.any && this.prepareArgs.Length > 2)
			{
				this.level = Convert.ToInt32(this.prepareArgs[2]);
			}
			else
			{
				this.level = 0;
			}
		}

		public override void Start()
		{
			if (this.IsConditionSatisfied())
			{
				this.parent.ChildSatisfied(this);
			}
			else
			{
				this.events.RegisterObserver(this, EventId.InventoryTroopUpdated, EventPriority.Default);
				this.events.RegisterObserver(this, EventId.InventorySpecialAttackUpdated, EventPriority.Default);
				this.events.RegisterObserver(this, EventId.InventoryHeroUpdated, EventPriority.Default);
				this.events.RegisterObserver(this, EventId.InventoryChampionUpdated, EventPriority.Default);
				this.observingEvents = true;
			}
		}

		public override void Destroy()
		{
			if (this.observingEvents)
			{
				this.events.UnregisterObserver(this, EventId.InventoryTroopUpdated);
				this.events.UnregisterObserver(this, EventId.InventorySpecialAttackUpdated);
				this.events.UnregisterObserver(this, EventId.InventoryHeroUpdated);
				this.events.UnregisterObserver(this, EventId.InventoryChampionUpdated);
			}
		}

		public override void GetProgress(out int current, out int total)
		{
			current = 0;
			total = this.threshold;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			TroopType troopType = this.troopType;
			InventoryStorage inventoryStorage;
			if (troopType != TroopType.Hero)
			{
				if (troopType != TroopType.Champion)
				{
					inventoryStorage = currentPlayer.Inventory.Troop;
				}
				else
				{
					inventoryStorage = currentPlayer.Inventory.Champion;
				}
			}
			else
			{
				inventoryStorage = currentPlayer.Inventory.Hero;
			}
			if (!this.any)
			{
				List<TroopTypeVO> list = new List<TroopTypeVO>();
				switch (this.matchType)
				{
				case ConditionMatchType.Uid:
				{
					TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(this.unitId);
					list = Service.TroopUpgradeCatalog.GetUpgradeGroupLevels(troopTypeVO.UpgradeGroup);
					break;
				}
				case ConditionMatchType.Id:
					list = Service.TroopUpgradeCatalog.GetUpgradeGroupLevels(this.unitId);
					break;
				case ConditionMatchType.Type:
				{
					StaticDataController staticDataController = Service.StaticDataController;
					TroopType troopType2 = StringUtils.ParseEnum<TroopType>(this.unitId);
					foreach (TroopTypeVO current2 in staticDataController.GetAll<TroopTypeVO>())
					{
						if (current2.Type == troopType2 && current2.Lvl >= this.level && inventoryStorage.HasItem(current2.Uid))
						{
							current += inventoryStorage.GetItemAmount(current2.Uid);
						}
					}
					break;
				}
				}
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					if (list[i].Lvl >= this.level && inventoryStorage.HasItem(list[i].Uid))
					{
						current += inventoryStorage.GetItemAmount(list[i].Uid);
					}
					i++;
				}
			}
			else
			{
				Dictionary<string, InventoryEntry> internalStorage = inventoryStorage.GetInternalStorage();
				foreach (InventoryEntry current3 in internalStorage.Values)
				{
					current += current3.Amount;
				}
			}
		}

		public override bool IsConditionSatisfied()
		{
			int num;
			int num2;
			this.GetProgress(out num, out num2);
			return num >= num2;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (this.IsConditionSatisfied())
			{
				this.parent.ChildSatisfied(this);
			}
			return EatResponse.NotEaten;
		}
	}
}
