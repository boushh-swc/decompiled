using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
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
	public class DestroyBuildingTypeCondition : AbstractCondition, IEventObserver
	{
		private const int AMOUNT_ARG = 0;

		private const int BUILDING_TYPE_ARG = 1;

		private bool any;

		private bool byPercent;

		private BuildingType buildingMatchType;

		protected int buildingsToDestroy;

		protected int buildingsDestroyed;

		public DestroyBuildingTypeCondition(ConditionVO vo, IConditionParent parent) : base(vo, parent)
		{
			this.buildingMatchType = StringUtils.ParseEnum<BuildingType>(this.prepareArgs[1]);
			this.any = (this.buildingMatchType == BuildingType.Any);
			this.buildingsDestroyed = 0;
			this.buildingsToDestroy = 0;
			if (this.prepareArgs[0].Contains("%"))
			{
				this.byPercent = true;
				string text = this.prepareArgs[0];
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
				this.buildingsToDestroy = Convert.ToInt32(this.prepareArgs[0]);
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
			return (this.any && GameUtils.IsBuildingTypeValidForBattleConditions(component.BuildingType.Type)) || component.BuildingType.Type == this.buildingMatchType;
		}

		protected virtual void EvaluateAmount()
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
