using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.VictoryConditions;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Missions
{
	public class OwnMissionProcessor : AbstractMissionProcessor, IConditionParent
	{
		private int conditionCount;

		private int successes;

		private List<AbstractCondition> conditions;

		public OwnMissionProcessor(MissionConductor parent) : base(parent)
		{
		}

		public override void Start()
		{
			PlanetDetailsScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<PlanetDetailsScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.Close(null);
			}
			if (Service.GameStateMachine.CurrentState is GalaxyState)
			{
				Service.GalaxyViewController.GoToHome();
			}
			if (this.parent.OnIntroHook())
			{
				Service.UserInputInhibitor.DenyAll();
			}
			else
			{
				this.StartCounting();
			}
		}

		public override void Resume()
		{
			this.StartCounting();
		}

		public override void OnIntroHookComplete()
		{
			Service.UserInputInhibitor.AllowAll();
			this.StartCounting();
		}

		private void StartCounting()
		{
			this.successes = 0;
			this.conditionCount = this.parent.MissionVO.Conditions.Count;
			this.conditions = new List<AbstractCondition>();
			for (int i = 0; i < this.conditionCount; i++)
			{
				AbstractCondition abstractCondition = ConditionFactory.GenerateCondition(this.parent.MissionVO.Conditions[i], this);
				this.conditions.Add(abstractCondition);
				abstractCondition.Start();
			}
		}

		public void ChildSatisfied(AbstractCondition child)
		{
			this.successes++;
			if (this.successes >= this.conditionCount)
			{
				this.parent.CompleteMission(3);
				this.parent.OnSuccessHook();
			}
		}

		public void ChildFailed(AbstractCondition child)
		{
		}

		public void ChildUpdated(AbstractCondition child, int delta)
		{
		}

		public override void Destroy()
		{
			for (int i = 0; i < this.conditions.Count; i++)
			{
				this.conditions[i].Destroy();
			}
			this.conditions.Clear();
			this.conditions = null;
		}
	}
}
