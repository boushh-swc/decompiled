using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Commands.Missions;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Controllers.Missions
{
	public class OffensiveCombatMissionProcessor : AbstractMissionProcessor, IEventObserver
	{
		private const float GOAL_DELAY = 0.5f;

		private const float GOAL_DURATION = 4f;

		private const float GOAL_FADE = 0.5f;

		private BattleInitializationData data;

		public OffensiveCombatMissionProcessor(MissionConductor parent) : base(parent)
		{
		}

		public override void Start()
		{
			if (this.parent.MissionVO.Grind)
			{
				MissionIdRequest request = new MissionIdRequest(this.parent.MissionVO.Uid);
				GetMissionMapCommand getMissionMapCommand = new GetMissionMapCommand(request);
				getMissionMapCommand.AddSuccessCallback(new AbstractCommand<MissionIdRequest, GetMissionMapResponse>.OnSuccessCallback(this.OnServerGrindSelectionCompleteOnSuccess));
				Service.ServerAPI.Sync(getMissionMapCommand);
			}
			else
			{
				this.data = BattleInitializationData.CreateFromCampaignMissionVO(this.parent.MissionVO.Uid);
				this.LoadBattle();
			}
		}

		public override void Resume()
		{
			this.Start();
		}

		private void OnServerGrindSelectionCompleteOnSuccess(GetMissionMapResponse response, object cookie)
		{
			this.data = BattleInitializationData.CreateFromCampaignMissionAndBattle(this.parent.MissionVO.Uid, response.BattleUid);
			this.LoadBattle();
		}

		private void LoadBattle()
		{
			BattleStartState.GoToBattleStartState(this.data, new TransitionCompleteDelegate(this.OnWorldLoaded));
		}

		private void OnWorldLoaded()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.BattleEndFullyProcessed, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			if (this.parent.OnIntroHook())
			{
				base.PauseBattle();
			}
			else
			{
				this.StartMission();
			}
		}

		public override void OnIntroHookComplete()
		{
			base.ResumeBattle();
			this.StartMission();
		}

		private void StartMission()
		{
			Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.ShowGoal), null);
			Service.EventManager.SendEvent(EventId.MissionStarted, null);
		}

		private void ShowGoal(uint id, object cookie)
		{
			string missionGoal = LangUtils.GetMissionGoal(this.parent.MissionVO);
			if (!string.IsNullOrEmpty(missionGoal))
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(missionGoal, 4f, 0.5f);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.BattleEndFullyProcessed)
			{
				if (id == EventId.GameStateChanged)
				{
					if (Service.GameStateMachine.CurrentState is HomeState)
					{
						this.parent.CancelMission();
					}
				}
			}
			else
			{
				this.RemoveListeners();
				CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				currentPlayer.CampaignProgress.UpdateMissionLoot(this.parent.MissionVO.Uid, currentBattle);
				if (currentBattle.Won)
				{
					this.parent.CompleteMission(currentBattle.EarnedStars);
					if (this.parent.OnSuccessHook())
					{
						base.PauseBattle();
					}
				}
				else if (this.parent.OnFailureHook())
				{
					base.PauseBattle();
				}
			}
			return EatResponse.NotEaten;
		}

		public override void OnSuccessHookComplete()
		{
			base.ResumeBattle();
		}

		public override void OnFailureHookComplete()
		{
			base.ResumeBattle();
		}

		public override void OnGoalFailureHookComplete()
		{
			base.ResumeBattle();
		}

		private void RemoveListeners()
		{
			Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
			Service.EventManager.UnregisterObserver(this, EventId.BattleEndFullyProcessed);
		}

		public override void OnCancel()
		{
			this.RemoveListeners();
			Service.BattleController.CancelBattle();
		}
	}
}
