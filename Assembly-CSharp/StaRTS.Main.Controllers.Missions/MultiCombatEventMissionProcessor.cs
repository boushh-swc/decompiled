using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.VictoryConditions;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Missions
{
	public class MultiCombatEventMissionProcessor : AbstractMissionProcessor, IConditionParent, IEventObserver
	{
		private List<string> successes;

		private int conditionCount;

		private List<AbstractCondition> conditions;

		private Dictionary<string, int> startingValues;

		public MultiCombatEventMissionProcessor(MissionConductor parent) : base(parent)
		{
			this.successes = new List<string>();
			this.startingValues = new Dictionary<string, int>();
			foreach (ConditionVO current in parent.MissionVO.Conditions)
			{
				this.startingValues.Add(current.Uid, 0);
			}
		}

		public override void Start()
		{
			if (this.parent.OnIntroHook())
			{
				Service.UserInputInhibitor.DenyAll();
			}
			else
			{
				this.StartCounting();
			}
		}

		public override void OnIntroHookComplete()
		{
			Service.UserInputInhibitor.AllowAll();
			this.StartCounting();
		}

		public override void Resume()
		{
			Dictionary<string, int> counters = this.parent.GetCounters();
			if (counters != null)
			{
				foreach (string current in counters.Keys)
				{
					this.startingValues[current] = counters[current];
				}
			}
			this.StartCounting();
		}

		public void ChildUpdated(AbstractCondition child, int delta)
		{
			this.parent.UpdateCounter(child.GetConditionVo().Uid, delta);
		}

		private void StartCounting()
		{
			this.conditions = new List<AbstractCondition>();
			this.conditionCount = this.parent.MissionVO.Conditions.Count;
			for (int i = 0; i < this.conditionCount; i++)
			{
				AbstractCondition abstractCondition = ConditionFactory.GenerateCondition(this.parent.MissionVO.Conditions[i], this, this.startingValues[this.parent.MissionVO.Conditions[i].Uid]);
				this.conditions.Add(abstractCondition);
				abstractCondition.Start();
			}
		}

		public void ChildSatisfied(AbstractCondition child)
		{
			if (!this.successes.Contains(child.GetConditionVo().Uid))
			{
				this.successes.Add(child.GetConditionVo().Uid);
			}
			if (this.successes.Count >= this.conditionCount)
			{
				if (Service.GameStateMachine.CurrentState is HomeState)
				{
					this.parent.OnSuccessHook();
					this.parent.CompleteMission(3);
				}
				else
				{
					Service.EventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
				}
			}
		}

		public void ChildFailed(AbstractCondition child)
		{
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.GameStateChanged)
			{
				if (Service.GameStateMachine.CurrentState is HomeState)
				{
					Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
					this.parent.OnSuccessHook();
					this.parent.CompleteMission(3);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
			for (int i = 0; i < this.conditionCount; i++)
			{
				this.conditions[i].Destroy();
			}
		}
	}
}
