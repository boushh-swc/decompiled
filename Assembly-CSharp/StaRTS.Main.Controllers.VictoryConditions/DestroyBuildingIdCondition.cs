using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class DestroyBuildingIdCondition : AbstractCondition, IEventObserver
	{
		private const int AMOUNT_ARG = 0;

		private const int BUILDING_ID_ARG = 1;

		private const string ANY_ID = "any";

		private bool any;

		private bool byPercent;

		private string buildingMatchId;

		private int buildingsToDestroy;

		private int buildingsDestroyed;

		public DestroyBuildingIdCondition(ConditionVO vo, IConditionParent parent) : base(vo, parent)
		{
			this.buildingMatchId = this.prepareArgs[1].ToLower();
			this.any = (this.buildingMatchId == "any");
			this.buildingsDestroyed = 0;
			this.buildingsToDestroy = 0;
			string text = this.prepareArgs[0];
			if (text.Contains("%"))
			{
				this.byPercent = true;
				text = text.Substring(0, text.Length - 1);
				int percent = Convert.ToInt32(text);
				if (this.any && this.byPercent)
				{
					this.buildingsToDestroy = percent;
					return;
				}
				EntityController entityController = Service.EntityController;
				NodeList<BuildingNode> nodeList = entityController.GetNodeList<BuildingNode>();
				int num = 0;
				for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
				{
					if (this.IsBuildingValid(buildingNode.BuildingComp))
					{
						num++;
					}
				}
				this.buildingsToDestroy = IntMath.GetPercent(percent, num);
			}
			else
			{
				this.buildingsToDestroy = Convert.ToInt32(text);
			}
		}

		public override void Start()
		{
			if (this.any && this.byPercent)
			{
				this.events.RegisterObserver(this, EventId.DamagePercentUpdated, EventPriority.Default);
			}
			else
			{
				this.events.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.EntityKilled)
			{
				if (id == EventId.DamagePercentUpdated)
				{
					this.buildingsDestroyed = (int)cookie;
					this.EvaluateAmount();
				}
			}
			else
			{
				Entity entity = (Entity)cookie;
				BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
				if (buildingComponent != null && this.IsBuildingValid(buildingComponent))
				{
					this.buildingsDestroyed++;
					this.EvaluateAmount();
				}
			}
			return EatResponse.NotEaten;
		}

		private bool IsBuildingValid(BuildingComponent component)
		{
			return (this.any && GameUtils.IsBuildingTypeValidForBattleConditions(component.BuildingType.Type)) || component.BuildingType.BuildingID.ToLower() == this.buildingMatchId;
		}

		private void EvaluateAmount()
		{
			if (this.IsConditionSatisfied())
			{
				this.parent.ChildSatisfied(this);
			}
		}

		public override void Destroy()
		{
			this.events.UnregisterObserver(this, EventId.EntityKilled);
			this.events.UnregisterObserver(this, EventId.DamagePercentUpdated);
		}

		public override bool IsConditionSatisfied()
		{
			return this.buildingsDestroyed >= this.buildingsToDestroy;
		}

		public override void GetProgress(out int current, out int total)
		{
			current = this.buildingsDestroyed;
			total = this.buildingsToDestroy;
		}
	}
}
