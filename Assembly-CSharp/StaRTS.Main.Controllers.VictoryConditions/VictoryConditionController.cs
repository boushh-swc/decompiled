using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class VictoryConditionController : IConditionParent
	{
		private const string DEFAULT_VICTORY_CONDITION1 = "DestroyHQ";

		private const string DEFAULT_VICTORY_CONDITION2 = "DestroyHalfBuildings";

		private const string DEFAULT_VICTORY_CONDITION3 = "DestroyAllBuildings";

		private List<AbstractCondition> currentVictoryConditions;

		private EventManager events;

		private AbstractCondition failureCondition;

		private StaticDataController sdc;

		public List<string> Successes;

		public List<string> Failures;

		public ConditionVO FailureConditionVO
		{
			get
			{
				if (this.failureCondition != null)
				{
					return this.failureCondition.GetConditionVo();
				}
				return null;
			}
		}

		public List<AbstractCondition> ActiveConditions
		{
			get
			{
				return this.currentVictoryConditions;
			}
		}

		public VictoryConditionController()
		{
			Service.VictoryConditionController = this;
			this.events = Service.EventManager;
			this.sdc = Service.StaticDataController;
			this.Successes = new List<string>();
			this.Failures = new List<string>();
		}

		public void ActivateConditionSet(List<ConditionVO> voSet)
		{
			this.CancelCurrentConditions();
			this.Successes = new List<string>();
			this.Failures = new List<string>();
			this.currentVictoryConditions = new List<AbstractCondition>();
			AbstractCondition abstractCondition = null;
			for (int i = 0; i < voSet.Count; i++)
			{
				abstractCondition = ConditionFactory.GenerateCondition(voSet[i], this);
				this.currentVictoryConditions.Add(abstractCondition);
				abstractCondition.Start();
			}
			for (int j = voSet.Count; j < 3; j++)
			{
				abstractCondition = ConditionFactory.GenerateCondition(abstractCondition.GetConditionVo(), this);
				this.currentVictoryConditions.Add(abstractCondition);
				abstractCondition.Start();
			}
		}

		public void ActivateFailureCondition(ConditionVO condition)
		{
			this.failureCondition = ConditionFactory.GenerateCondition(condition, this);
			this.failureCondition.Start();
		}

		public void CancelCurrentConditions()
		{
			if (this.currentVictoryConditions != null)
			{
				for (int i = 0; i < this.currentVictoryConditions.Count; i++)
				{
					this.currentVictoryConditions[i].Destroy();
				}
			}
			this.currentVictoryConditions = null;
			this.Successes.Clear();
			this.Failures.Clear();
			if (this.failureCondition != null)
			{
				this.failureCondition.Destroy();
				this.failureCondition = null;
			}
		}

		public List<ConditionVO> GetDefaultConditions()
		{
			List<ConditionVO> list = new List<ConditionVO>();
			ConditionVO item = this.sdc.Get<ConditionVO>("DestroyHQ");
			list.Add(item);
			item = this.sdc.Get<ConditionVO>("DestroyHalfBuildings");
			list.Add(item);
			item = this.sdc.Get<ConditionVO>("DestroyAllBuildings");
			list.Add(item);
			return list;
		}

		public bool HasCurrentActiveConditions()
		{
			return this.currentVictoryConditions != null;
		}

		public void ChildSatisfied(AbstractCondition child)
		{
			if (child == this.failureCondition)
			{
				return;
			}
			ConditionVO conditionVo = child.GetConditionVo();
			child.Destroy();
			this.Successes.Add(conditionVo.Uid);
			this.events.SendEvent(EventId.VictoryConditionSuccess, conditionVo);
		}

		public void ChildFailed(AbstractCondition child)
		{
			ConditionVO conditionVo = child.GetConditionVo();
			child.Destroy();
			this.Failures.Add(conditionVo.Uid);
			this.events.SendEvent(EventId.VictoryConditionFailure, conditionVo);
		}

		public void ChildUpdated(AbstractCondition child, int delta)
		{
		}
	}
}
