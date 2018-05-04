using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Commands.Crates;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Commands.Squads;
using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class SquadWarManager : IEventObserver, IViewClockTimeObserver
	{
		public const int MAX_VICTORY_POINTS_LEFT = 3;

		public const int PLAYER_SQUAD = 0;

		public const int OPPONENT_SQUAD = 1;

		public const int NUM_SQUADS_IN_WAR = 2;

		public const int NOT_IN_WAR_OR_MATCHMAKING = 0;

		public const int IN_WAR_OR_MATCHMAKING = 1;

		private const string BUFF_BASE_ATTACK_STARTED = "WAR_ATTACK_BUFF_BASE_STARTED";

		private const string BUFF_BASE_ATTACK_ENDED = "WAR_ATTACK_BUFF_BASE_ENDED";

		private const string BUFF_BASE_ATTACK_WON = "WAR_BUFF_BASE_CAPTURED";

		private const string PLAYER_ATTACK_STARTED = "WAR_ATTACK_PLAYER_STARTED";

		private const string PLAYER_ATTACKED = "WAR_PLAYER_ATTACKED";

		private const string BUFF_BASE_OWNERSHIP_PLAYER = "BUFF_BASE_OWNERSHIP_PLAYER";

		private const string BUFF_BASE_OWNERSHIP_OPPONENT = "BUFF_BASE_OWNERSHIP_OPPONENT";

		private const string NOT_ENOUGH_TROOPS_TITLE_STRING = "NOT_ENOUGH_TROOPS_TITLE";

		private const string NOT_ENOUGH_TROOPS_FOR_ATTACK_STRING = "NOT_ENOUGH_TROOPS_FOR_ATTACK";

		private const string WAR_BUFF_BASE_ALREADY_CAPTURED = "WAR_BUFF_BASE_ALREADY_CAPTURED";

		private const string WAR_NO_TURNS_NO_SCOUTING = "WAR_NO_TURNS_NO_SCOUTING";

		private const string WAR_TARGET_NO_POINTS = "WAR_TARGET_NO_POINTS";

		private const string WAR_PREVENT_ENEMY_SCOUT_WRONG_PHASE = "WAR_PREVENT_ENEMY_SCOUT_WRONG_PHASE";

		private const string WAR_PREVENT_ALLY_SCOUT_WRONG_PHASE = "WAR_PREVENT_ALLY_SCOUT_WRONG_PHASE";

		private const string WAR_PREVENT_BUFF_BASE_SCOUT_WRONG_PHASE = "WAR_PREVENT_BUFF_BASE_SCOUT_WRONG_PHASE";

		private const string WAR_BOARD_INSUFFICIENT_LEVEL_TITLE = "WAR_BOARD_INSUFFICIENT_LEVEL_TITLE";

		private const string WAR_BOARD_INSUFFICIENT_LEVEL_BODY = "WAR_BOARD_INSUFFICIENT_LEVEL_BODY";

		private const string WAR_DISABLED_TITLE = "WAR_DISABLED_TITLE";

		private const string WAR_DISABLED_BODY = "WAR_DISABLED_BODY";

		private const string WAR_MATCHMAKING_STARTED = "WAR_MATCHMAKING_STARTED";

		private const string WAR_BOARD_MATCHMAKING_EXIT_TITLE = "WAR_BOARD_MATCHMAKING_EXIT_TITLE";

		private const string WAR_BOARD_MATCHMAKING_EXIT_BODY = "WAR_BOARD_MATCHMAKING_EXIT_BODY";

		public int NumParticipants;

		public SquadWarData CurrentSquadWar;

		private SquadWarStatusType previousStatus;

		private string currentlyScoutedBuffBaseId;

		private SquadController controller;

		private SqmWarEventData eventDataToBeShown;

		private List<string> warParty;

		public bool MatchMakingPrepMode;

		private SquadMemberWarData currentMemberWarData;

		private SquadWarParticipantState currentParticipantState;

		public bool EnableSquadWarMode
		{
			get;
			set;
		}

		public SquadWarManager(SquadController controller)
		{
			this.EnableSquadWarMode = true;
			this.MatchMakingPrepMode = false;
			this.controller = controller;
			this.warParty = new List<string>();
			Service.EventManager.RegisterObserver(this, EventId.WarLaunchFlow);
			Service.EventManager.RegisterObserver(this, EventId.ContractCompleted);
			Service.EventManager.RegisterObserver(this, EventId.WarPhaseChanged);
		}

		public SquadWarStatusType GetCurrentStatus()
		{
			return SquadUtils.GetWarStatus(this.CurrentSquadWar, Service.ServerAPI.ServerTime);
		}

		public bool IsTimeWithinCurrentSquadWarPhase(int serverTime)
		{
			return SquadUtils.IsTimeWithinSquadWarPhase(this.CurrentSquadWar, (uint)serverTime);
		}

		public void UpdateSquadWar(SquadWarData squadwarData)
		{
			this.CurrentSquadWar = squadwarData;
			SquadWarSquadData squadWarSquadData = this.CurrentSquadWar.Squads[0];
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			squadWarSquadData.Faction = currentSquad.Faction;
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			this.NumParticipants = squadWarSquadData.Participants.Count;
			string playerId = Service.CurrentPlayer.PlayerId;
			for (int i = 0; i < this.NumParticipants; i++)
			{
				SquadWarParticipantState squadWarParticipantState = squadWarSquadData.Participants[i];
				if (squadWarParticipantState.SquadMemberId == playerId)
				{
					this.currentParticipantState = squadWarParticipantState;
					break;
				}
			}
		}

		public WarScheduleVO GetCurrentWarScheduleData()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (WarScheduleVO current in staticDataController.GetAll<WarScheduleVO>())
			{
				if (current.StartTime <= this.CurrentSquadWar.StartTimeStamp && this.CurrentSquadWar.StartTimeStamp <= current.EndTime)
				{
					return current;
				}
			}
			Service.Logger.Error("Could not find war schedule data for current squad war!");
			return null;
		}

		public SquadWarSquadData GetSquadData(SquadWarSquadType squadType)
		{
			SquadWarSquadData result;
			if (squadType == SquadWarSquadType.PLAYER_SQUAD)
			{
				result = this.CurrentSquadWar.Squads[0];
			}
			else
			{
				result = this.CurrentSquadWar.Squads[1];
			}
			return result;
		}

		public SquadWarSquadData GetSquadData(string squadId)
		{
			for (int i = 0; i < 2; i++)
			{
				if (this.CurrentSquadWar.Squads[i].SquadId == squadId)
				{
					return this.CurrentSquadWar.Squads[i];
				}
			}
			return null;
		}

		public SquadWarParticipantState GetParticipantState(int index, SquadWarSquadType squadType)
		{
			SquadWarParticipantState result;
			if (squadType == SquadWarSquadType.PLAYER_SQUAD)
			{
				result = this.CurrentSquadWar.Squads[0].Participants[index];
			}
			else
			{
				result = this.CurrentSquadWar.Squads[1].Participants[index];
			}
			return result;
		}

		public SquadWarSquadType GetParticipantSquad(string squadMemberId)
		{
			List<SquadWarParticipantState> participants = this.CurrentSquadWar.Squads[0].Participants;
			int i = 0;
			int count = participants.Count;
			while (i < count)
			{
				if (participants[i].SquadMemberId == squadMemberId)
				{
					return SquadWarSquadType.PLAYER_SQUAD;
				}
				i++;
			}
			return SquadWarSquadType.OPPONENT_SQUAD;
		}

		public void UpdateCurrentMemberWarData(SquadMemberWarDataResponse response)
		{
			this.currentMemberWarData = response.MemberWarData;
			if (this.currentMemberWarData != null)
			{
				Service.EventManager.SendEvent(EventId.CurrentPlayerMemberDataUpdated, null);
			}
		}

		public SquadMemberWarData GetCurrentMemberWarData()
		{
			return this.currentMemberWarData;
		}

		public SquadWarParticipantState GetCurrentParticipantState()
		{
			return this.currentParticipantState;
		}

		public SquadWarParticipantState GetCurrentOpponentState()
		{
			string playerId = Service.BattleController.GetCurrentBattle().Defender.PlayerId;
			if (string.IsNullOrEmpty(playerId))
			{
				return null;
			}
			return this.FindParticipantState(playerId);
		}

		public SquadWarRewardData GetCurrentPlayerCurrentWarReward()
		{
			if (this.currentMemberWarData != null && this.currentMemberWarData.WarReward != null && this.currentMemberWarData.WarReward.WarId == this.CurrentSquadWar.WarId)
			{
				return this.currentMemberWarData.WarReward;
			}
			return null;
		}

		private bool RemoveCurrentPlayerCurrentWarReward()
		{
			return this.currentMemberWarData.WarReward != null && this.currentMemberWarData.WarReward.WarId == this.CurrentSquadWar.WarId && this.currentMemberWarData.RemoveSquadWarReward();
		}

		public int GetCurrentSquadWarResult()
		{
			int currentSquadScore = this.GetCurrentSquadScore(SquadWarSquadType.PLAYER_SQUAD);
			int currentSquadScore2 = this.GetCurrentSquadScore(SquadWarSquadType.OPPONENT_SQUAD);
			return Math.Sign(currentSquadScore - currentSquadScore2);
		}

		public int GetCurrentSquadScore(SquadWarSquadType squadType)
		{
			int num = 0;
			SquadWarSquadData squadWarSquadData = this.CurrentSquadWar.Squads[(int)squadType];
			int i = 0;
			int count = squadWarSquadData.Participants.Count;
			while (i < count)
			{
				SquadWarParticipantState participantState = this.GetParticipantState(i, squadType);
				num += participantState.Score;
				i++;
			}
			return num;
		}

		public void ClaimSquadWarReward(string warId)
		{
			ProcessingScreen.Show();
			SquadWarClaimRewardRequest request = new SquadWarClaimRewardRequest(warId);
			SquadWarClaimRewardCommand squadWarClaimRewardCommand = new SquadWarClaimRewardCommand(request);
			squadWarClaimRewardCommand.Context = warId;
			squadWarClaimRewardCommand.AddSuccessCallback(new AbstractCommand<SquadWarClaimRewardRequest, CrateDataResponse>.OnSuccessCallback(this.OnSquadWarClaimRewardSuccess));
			squadWarClaimRewardCommand.AddFailureCallback(new AbstractCommand<SquadWarClaimRewardRequest, CrateDataResponse>.OnFailureCallback(this.OnSquadWarClaimRewardFailed));
			Service.ServerAPI.Sync(squadWarClaimRewardCommand);
		}

		private void OnSquadWarClaimRewardSuccess(CrateDataResponse response, object cookie)
		{
			ProcessingScreen.Hide();
			string text = cookie as string;
			if (this.currentMemberWarData == null || this.currentMemberWarData.WarRewards == null)
			{
				Service.Logger.ErrorFormat("Attempting to claim reward for war {0} but memberWarData contains no rewards", new object[]
				{
					text
				});
				return;
			}
			SquadWarRewardData squadWarRewardData = null;
			int i = 0;
			int count = this.currentMemberWarData.WarRewards.Count;
			while (i < count)
			{
				if (this.currentMemberWarData.WarRewards[i].WarId == text)
				{
					squadWarRewardData = this.currentMemberWarData.WarRewards[i];
					this.currentMemberWarData.WarRewards.RemoveAt(i);
					break;
				}
				i++;
			}
			if (squadWarRewardData == null)
			{
				Service.Logger.ErrorFormat("Could not find reward for war {0} in memberWarData", new object[]
				{
					text
				});
				return;
			}
			CrateData crateDataTO = response.CrateDataTO;
			if (crateDataTO != null)
			{
				this.ShowWarRewardSupplyCrateAnimation(squadWarRewardData, crateDataTO);
			}
			Service.EventManager.SendEvent(EventId.WarRewardClaimed, null);
		}

		private void OnSquadWarClaimRewardFailed(uint status, object data)
		{
			ProcessingScreen.Hide();
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is WarBoardState)
			{
				this.CloseWarEndedScreen();
			}
		}

		public bool ClaimCurrentPlayerCurrentWarReward()
		{
			SquadWarRewardData currentPlayerCurrentWarReward = this.GetCurrentPlayerCurrentWarReward();
			if (currentPlayerCurrentWarReward != null)
			{
				this.ClaimSquadWarReward(currentPlayerCurrentWarReward.WarId);
				return true;
			}
			Service.Logger.Error("Trying to claim a non existant squad war reward");
			return false;
		}

		private void ShowWarRewardSupplyCrateAnimation(SquadWarRewardData rewardData, CrateData crateData)
		{
			SquadWarEndCelebrationScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<SquadWarEndCelebrationScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.PlayCloseAnimation();
			}
			List<string> resolvedSupplyIdList = GameUtils.GetResolvedSupplyIdList(crateData);
			InventoryCrateRewardController inventoryCrateRewardController = Service.InventoryCrateRewardController;
			InventoryCrateAnimation inventoryCrateAnimation = inventoryCrateRewardController.GrantInventoryCrateReward(resolvedSupplyIdList, crateData);
			inventoryCrateAnimation.InvCrateCollectionScreen.OnModalResult = new OnScreenModalResult(this.OnCrateScreenClosed);
		}

		private void OnCrateScreenClosed(object result, object cookie)
		{
			this.CloseWarEndedScreen();
		}

		public bool ShowWarEndedScreen()
		{
			if (Service.ScreenController.GetHighestLevelScreen<SquadWarEndCelebrationScreen>() != null)
			{
				return false;
			}
			if (this.GetCurrentPlayerCurrentWarReward() == null)
			{
				return false;
			}
			SquadWarSquadData squadWarSquadData = this.CurrentSquadWar.Squads[0];
			FactionType faction = squadWarSquadData.Faction;
			string squadName = squadWarSquadData.SquadName;
			int currentSquadWarResult = this.GetCurrentSquadWarResult();
			SquadWarSquadData squadWarSquadData2 = this.CurrentSquadWar.Squads[1];
			FactionType faction2 = squadWarSquadData2.Faction;
			bool sameFaction = faction == faction2;
			SquadWarEndCelebrationScreen screen = new SquadWarEndCelebrationScreen(currentSquadWarResult, faction, squadName, sameFaction);
			Service.ScreenController.AddScreen(screen);
			Service.UXController.HUD.SlideSquadScreenClosedInstantly();
			Service.UXController.HUD.SetSquadScreenVisibility(false);
			return true;
		}

		private void CloseWarEndedScreen()
		{
			SquadWarEndCelebrationScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<SquadWarEndCelebrationScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.OnModalResult = new OnScreenModalResult(this.OnCelebrationScreenClosed);
				highestLevelScreen.Close(null);
			}
			Service.UXController.HUD.SetSquadScreenVisibility(true);
		}

		public SquadWarBuffBaseData GetBuffBaseData(int index)
		{
			return this.CurrentSquadWar.BuffBases[index];
		}

		public bool IsBuffBaseUnderAttack(SquadWarBuffBaseData buffBaseData)
		{
			bool result = false;
			if (buffBaseData.AttackExpirationDate > 0u)
			{
				result = (ServerTime.Time <= buffBaseData.AttackExpirationDate);
			}
			return result;
		}

		public SquadWarScoutState CanAttackBuffBase(SquadWarBuffBaseData buffBaseData)
		{
			if (this.GetCurrentStatus() != SquadWarStatusType.PhaseAction)
			{
				return SquadWarScoutState.NotInActionPhase;
			}
			if (this.currentParticipantState == null)
			{
				return SquadWarScoutState.NotPatricipantInWar;
			}
			if (this.currentParticipantState.TurnsLeft <= 0)
			{
				return SquadWarScoutState.NoTurnsLeft;
			}
			if (this.IsBuffBaseUnderAttack(buffBaseData))
			{
				return SquadWarScoutState.UnderAttack;
			}
			WarBuffVO warBuffVO = Service.StaticDataController.Get<WarBuffVO>(buffBaseData.BuffBaseId);
			string planetId = warBuffVO.PlanetId;
			if (!Service.CurrentPlayer.IsPlanetUnlocked(planetId))
			{
				return SquadWarScoutState.DestinationUnavailable;
			}
			return SquadWarScoutState.AttackAvailable;
		}

		public SquadWarBuffBaseData GetCurrentlyScoutedBuffBaseData()
		{
			return this.FindBuffBaseData(this.currentlyScoutedBuffBaseId);
		}

		public SquadWarScoutState CanAttackCurrentlyScoutedBuffBase()
		{
			if (string.IsNullOrEmpty(this.currentlyScoutedBuffBaseId))
			{
				return SquadWarScoutState.Invalid;
			}
			SquadWarBuffBaseData buffBaseData = this.FindBuffBaseData(this.currentlyScoutedBuffBaseId);
			return this.CanAttackBuffBase(buffBaseData);
		}

		public bool IsCurrentlyScoutingOwnedBuffBase()
		{
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return false;
			}
			SquadWarBuffBaseData squadWarBuffBaseData = this.FindBuffBaseData(this.currentlyScoutedBuffBaseId);
			return squadWarBuffBaseData != null && squadWarBuffBaseData.OwnerId != null && squadWarBuffBaseData.OwnerId == currentSquad.SquadID;
		}

		public bool IsOpponentUnderAttack(SquadWarParticipantState opponentState)
		{
			bool result = false;
			if (opponentState.DefendingAttackExpirationDate > 0u)
			{
				result = (ServerTime.Time <= opponentState.DefendingAttackExpirationDate);
			}
			return result;
		}

		public SquadWarScoutState CanAttackOpponent(SquadWarParticipantState opponentState)
		{
			if (this.GetCurrentStatus() != SquadWarStatusType.PhaseAction)
			{
				return SquadWarScoutState.NotInActionPhase;
			}
			if (this.currentParticipantState == null)
			{
				return SquadWarScoutState.NotPatricipantInWar;
			}
			if (this.currentParticipantState.TurnsLeft <= 0)
			{
				return SquadWarScoutState.NoTurnsLeft;
			}
			if (opponentState.VictoryPointsLeft <= 0)
			{
				return SquadWarScoutState.OpponentHasNoVictoryPointsLeft;
			}
			if (this.IsOpponentUnderAttack(opponentState))
			{
				return SquadWarScoutState.UnderAttack;
			}
			return SquadWarScoutState.AttackAvailable;
		}

		public SquadWarScoutState CanAttackCurrentlyScoutedOpponent()
		{
			string playerId = Service.BattleController.GetCurrentBattle().Defender.PlayerId;
			if (string.IsNullOrEmpty(playerId))
			{
				return SquadWarScoutState.Invalid;
			}
			SquadWarParticipantState opponentState = this.FindParticipantState(playerId);
			return this.CanAttackOpponent(opponentState);
		}

		public void ClearSquadWarData()
		{
			this.CurrentSquadWar = null;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad != null)
			{
				currentSquad.ClearSquadWarId();
			}
		}

		public void EndSquadWar()
		{
			this.ClearSquadWarData();
		}

		public void OnViewClockTime(float dt)
		{
			uint serverTime = Service.ServerAPI.ServerTime;
			SquadWarStatusType warStatus = SquadUtils.GetWarStatus(this.CurrentSquadWar, serverTime);
			if (warStatus != this.previousStatus)
			{
				Service.EventManager.SendEvent(EventId.WarPhaseChanged, warStatus);
				this.previousStatus = warStatus;
			}
		}

		public void HandleWarEventMsg(SquadMsg msg)
		{
			SqmOwnerData ownerData = msg.OwnerData;
			SqmWarEventData warEventData = msg.WarEventData;
			if (warEventData == null)
			{
				return;
			}
			string text = (ownerData == null) ? null : ownerData.PlayerId;
			string text2 = (ownerData == null) ? null : ownerData.PlayerName;
			string opponentId = warEventData.OpponentId;
			string opponentName = warEventData.OpponentName;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			bool flag = text == currentPlayer.PlayerId;
			string text3 = null;
			switch (msg.Type)
			{
			case SquadMsgType.WarBuffBaseAttackStart:
			{
				SquadWarSquadData squad = null;
				this.FindParticipantState(text, out squad);
				SquadWarBuffBaseData buffBase = this.FindBuffBaseData(warEventData.BuffBaseUid);
				this.UpdateAttackStateForBuffBase(buffBase, warEventData.AttackExpirationTime);
				if (!flag)
				{
					this.UpdateStringBasedOnSquad(squad, ref text2);
					text3 = Service.Lang.Get("WAR_ATTACK_BUFF_BASE_STARTED", new object[]
					{
						this.GetWarBuffDisplayName(warEventData.BuffBaseUid),
						text2
					});
				}
				Service.EventManager.SendEvent(EventId.WarAttackBuffBaseStarted, warEventData.BuffBaseUid);
				break;
			}
			case SquadMsgType.WarBuffBaseAttackComplete:
			{
				SquadWarSquadData squadWarSquadData = null;
				SquadWarParticipantState participant = this.FindParticipantState(text, out squadWarSquadData);
				this.DeductTurnFromParticipant(participant);
				SquadWarBuffBaseData buffBase2 = this.FindBuffBaseData(warEventData.BuffBaseUid);
				this.UpdateAttackStateForBuffBase(buffBase2, 0u);
				if (warEventData.BuffBaseCaptured)
				{
					this.CaptureBuffBase(buffBase2, squadWarSquadData);
					if (!flag)
					{
						this.UpdateStringBasedOnSquad(squadWarSquadData, ref text2);
						text3 = Service.Lang.Get("WAR_BUFF_BASE_CAPTURED", new object[]
						{
							this.GetWarBuffDisplayName(warEventData.BuffBaseUid),
							text2
						});
					}
				}
				Service.EventManager.SendEvent(EventId.WarAttackBuffBaseCompleted, warEventData.BuffBaseUid);
				break;
			}
			case SquadMsgType.WarPlayerAttackStart:
			{
				SquadWarSquadData squad2 = null;
				this.FindParticipantState(text, out squad2);
				SquadWarSquadData squad3 = null;
				SquadWarParticipantState participant2 = this.FindParticipantState(opponentId, out squad3);
				this.UpdateUnderAttackStateForParticipant(participant2, warEventData.AttackExpirationTime);
				if (!flag)
				{
					this.UpdateStringBasedOnSquad(squad3, ref opponentName);
					this.UpdateStringBasedOnSquad(squad2, ref text2);
					text3 = Service.Lang.Get("WAR_ATTACK_PLAYER_STARTED", new object[]
					{
						opponentName,
						text2
					});
				}
				Service.EventManager.SendEvent(EventId.WarAttackPlayerStarted, opponentId);
				break;
			}
			case SquadMsgType.WarPlayerAttackComplete:
			{
				SquadWarSquadData squadWarSquadData2 = null;
				SquadWarParticipantState squadWarParticipantState = this.FindParticipantState(text, out squadWarSquadData2);
				this.DeductTurnFromParticipant(squadWarParticipantState);
				SquadWarSquadData squad4 = null;
				SquadWarParticipantState squadWarParticipantState2 = this.FindParticipantState(opponentId, out squad4);
				this.UpdateUnderAttackStateForParticipant(squadWarParticipantState2, 0u);
				this.ExchangeVictoryPoints(squadWarParticipantState, squadWarParticipantState2, squadWarSquadData2, warEventData.VictoryPointsTaken);
				if (!flag)
				{
					this.UpdateStringBasedOnSquad(squad4, ref opponentName);
					this.UpdateStringBasedOnSquad(squadWarSquadData2, ref text2);
					text3 = Service.Lang.Get("WAR_PLAYER_ATTACKED", new object[]
					{
						opponentName,
						text2,
						warEventData.StarsEarned,
						warEventData.VictoryPointsTaken
					});
				}
				Service.EventManager.SendEvent(EventId.WarAttackPlayerCompleted, opponentId);
				break;
			}
			case SquadMsgType.WarEnded:
				this.controller.UpdateCurrentSquadWar();
				break;
			}
			if (text3 != null && Service.UXController != null)
			{
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(text3);
			}
		}

		private void UpdateStringBasedOnSquad(SquadWarSquadData squad, ref string text)
		{
			if (squad != null)
			{
				if (squad.Faction == FactionType.Empire)
				{
					text = UXUtils.WrapTextInColor(text, "c0d0ff");
				}
				else if (squad.Faction == FactionType.Rebel)
				{
					text = UXUtils.WrapTextInColor(text, "f0dfc1");
				}
			}
		}

		public void EnterWarBoardMode()
		{
			HomeMapDataLoader homeMapDataLoader = Service.HomeMapDataLoader;
			Service.WorldTransitioner.StartTransition(new BaseToWarboardTransition(new WarBoardState(), homeMapDataLoader, null));
			this.SetupWarBoardLighting(homeMapDataLoader, true);
		}

		public void ExitWarBoardMode(TransitionCompleteDelegate callback)
		{
			HomeMapDataLoader homeMapDataLoader = Service.HomeMapDataLoader;
			HomeState.GoToHomeState(callback, true);
			this.SetupWarBoardLightingRemoval(homeMapDataLoader, true);
		}

		public void SetupWarBoardLightingRemoval(HomeMapDataLoader dataLoader, bool softWipeTransition)
		{
			LightingEffectsController lightingEffectsController = Service.LightingEffectsController;
			EventId triggerEvent = EventId.WorldOutTransitionComplete;
			if (softWipeTransition)
			{
				triggerEvent = EventId.WipeCameraSnapshotTaken;
			}
			lightingEffectsController.LightingEffects.SetupDelayedLightingOverrideRemoval(triggerEvent);
		}

		public void SetupWarBoardLighting(HomeMapDataLoader dataLoader, bool softWipeTransition)
		{
			string warBoardLightingAsset = dataLoader.GetPlanetData().WarBoardLightingAsset;
			LightingEffectsController lightingEffectsController = Service.LightingEffectsController;
			EventId triggerEvent = EventId.WorldOutTransitionComplete;
			if (softWipeTransition)
			{
				triggerEvent = EventId.WipeCameraSnapshotTaken;
			}
			lightingEffectsController.LightingEffects.ApplyDelayedLightingDataOverride(triggerEvent, warBoardLightingAsset);
		}

		private void UpdateUnderAttackStateForParticipant(SquadWarParticipantState participant, uint expirationTime)
		{
			if (participant != null)
			{
				participant.DefendingAttackExpirationDate = expirationTime;
			}
		}

		private void UpdateAttackStateForBuffBase(SquadWarBuffBaseData buffBase, uint expirationTime)
		{
			if (buffBase != null)
			{
				buffBase.AttackExpirationDate = expirationTime;
			}
		}

		private void DeductTurnFromParticipant(SquadWarParticipantState participant)
		{
			if (participant != null && participant.TurnsLeft > 0)
			{
				participant.TurnsLeft--;
			}
		}

		private void ExchangeVictoryPoints(SquadWarParticipantState attacker, SquadWarParticipantState defender, SquadWarSquadData attackerSquad, int victoryPoints)
		{
			if (attacker != null)
			{
				attacker.Score += victoryPoints;
			}
			if (defender != null && defender.VictoryPointsLeft >= victoryPoints)
			{
				defender.VictoryPointsLeft -= victoryPoints;
			}
			Service.EventManager.SendEvent(EventId.WarVictoryPointsUpdated, attacker);
			Service.EventManager.SendEvent(EventId.WarVictoryPointsUpdated, defender);
		}

		private void CaptureBuffBase(SquadWarBuffBaseData buffBase, SquadWarSquadData attackerSquad)
		{
			if (buffBase == null)
			{
				return;
			}
			if (attackerSquad != null)
			{
				buffBase.OwnerId = attackerSquad.SquadId;
			}
			WarBuffVO warBuffVO = Service.StaticDataController.Get<WarBuffVO>(buffBase.BuffBaseId);
			string[] array = null;
			FactionType faction = Service.CurrentPlayer.Faction;
			if (faction != FactionType.Empire)
			{
				if (faction == FactionType.Rebel)
				{
					array = warBuffVO.RebelBattlesByLevel;
				}
			}
			else
			{
				array = warBuffVO.EmpireBattlesByLevel;
			}
			if (array != null && buffBase.BaseLevel < array.Length - 1)
			{
				buffBase.BaseLevel++;
			}
			Service.EventManager.SendEvent(EventId.WarBuffBaseCaptured, buffBase);
		}

		private SquadWarParticipantState FindParticipantState(string playerId)
		{
			SquadWarSquadData squadWarSquadData;
			return this.FindParticipantState(playerId, out squadWarSquadData);
		}

		private SquadWarParticipantState FindParticipantState(string playerId, out SquadWarSquadData squad)
		{
			SquadWarParticipantState result = null;
			squad = null;
			if (this.CurrentSquadWar == null)
			{
				return null;
			}
			for (int i = 0; i < 2; i++)
			{
				SquadWarSquadData squadWarSquadData = this.CurrentSquadWar.Squads[i];
				int j = 0;
				int count = squadWarSquadData.Participants.Count;
				while (j < count)
				{
					if (squadWarSquadData.Participants[j].SquadMemberId == playerId)
					{
						result = squadWarSquadData.Participants[j];
						squad = squadWarSquadData;
						break;
					}
					j++;
				}
			}
			return result;
		}

		private SquadWarBuffBaseData FindBuffBaseData(string buffBaseId)
		{
			if (this.CurrentSquadWar == null)
			{
				return null;
			}
			SquadWarBuffBaseData result = null;
			int i = 0;
			int count = this.CurrentSquadWar.BuffBases.Count;
			while (i < count)
			{
				if (this.CurrentSquadWar.BuffBases[i].BuffBaseId == buffBaseId)
				{
					result = this.CurrentSquadWar.BuffBases[i];
					break;
				}
				i++;
			}
			return result;
		}

		public List<string> GetBuffBasesOwnedBySquad(string squadId)
		{
			if (string.IsNullOrEmpty(squadId))
			{
				return null;
			}
			List<string> list = null;
			int i = 0;
			int count = this.CurrentSquadWar.BuffBases.Count;
			while (i < count)
			{
				if (this.CurrentSquadWar.BuffBases[i].OwnerId == squadId)
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(this.CurrentSquadWar.BuffBases[i].BuffBaseId);
				}
				i++;
			}
			return list;
		}

		public int CalculateVictoryPointsTaken(BattleEntry battle)
		{
			return Math.Max(0, battle.EarnedStars - GameConstants.WAR_VICTORY_POINTS + battle.WarVictoryPointsAvailable);
		}

		private void OnTransitionComplete()
		{
		}

		public bool CanScoutBuffBase(SquadWarBuffBaseData buffBaseData, ref string message)
		{
			message = string.Empty;
			SquadWarStatusType currentStatus = this.GetCurrentStatus();
			if (currentStatus != SquadWarStatusType.PhasePrep && currentStatus != SquadWarStatusType.PhaseAction)
			{
				message = Service.Lang.Get("WAR_PREVENT_BUFF_BASE_SCOUT_WRONG_PHASE", new object[0]);
				return false;
			}
			return this.currentParticipantState != null || true;
		}

		private void OnGetBuffBaseStatusSuccess(SquadWarBuffBaseResponse response, object cookie)
		{
			ProcessingScreen.Hide();
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return;
			}
			SquadWarBuffBaseData squadWarBuffBaseData = (response.SquadWarBuffBaseData == null) ? this.FindBuffBaseData(this.currentlyScoutedBuffBaseId) : response.SquadWarBuffBaseData;
			if (squadWarBuffBaseData == null)
			{
				return;
			}
			string ownerId = squadWarBuffBaseData.OwnerId;
			int baseLevel = squadWarBuffBaseData.BaseLevel;
			WarBuffVO warBuffVO = Service.StaticDataController.Get<WarBuffVO>(this.currentlyScoutedBuffBaseId);
			bool flag = ownerId != null && ownerId == currentSquad.SquadID;
			string[] array = null;
			SquadWarSquadData squadData = this.GetSquadData(SquadWarSquadType.PLAYER_SQUAD);
			SquadWarSquadData squadData2 = this.GetSquadData(SquadWarSquadType.OPPONENT_SQUAD);
			FactionType faction;
			if (flag)
			{
				faction = squadData.Faction;
			}
			else
			{
				faction = squadData2.Faction;
			}
			if (faction != FactionType.Empire)
			{
				if (faction == FactionType.Rebel)
				{
					array = warBuffVO.EmpireBattlesByLevel;
				}
			}
			else
			{
				array = warBuffVO.RebelBattlesByLevel;
			}
			string text = null;
			if (array != null && baseLevel < array.Length)
			{
				text = array[baseLevel];
			}
			if (string.IsNullOrEmpty(text))
			{
				Service.Logger.Error("Can't assign base name for:" + this.currentlyScoutedBuffBaseId);
				return;
			}
			this.LogScoutBIGameAction(squadWarBuffBaseData.BuffBaseId);
			CampaignMissionVO vo = Service.StaticDataController.Get<CampaignMissionVO>(text);
			BattleInitializationData data = BattleInitializationData.CreateBuffBaseBattleFromCampaignMissionVO(vo, squadWarBuffBaseData);
			BattleStartState.GoToBattleStartState(data, new TransitionCompleteDelegate(this.OnBattleReady));
		}

		private void OnGetBuffBaseStatusFailure(uint status, object cookie)
		{
			ProcessingScreen.Hide();
			this.ShowPlayerInstructionErrorBasedOnStatus(status, true);
			this.ReleaseCurrentlyScoutedBuffBase();
		}

		private void LogScoutBIGameAction(string id)
		{
			SquadWarStatusType currentStatus = this.GetCurrentStatus();
			if (currentStatus == SquadWarStatusType.PhasePrep || currentStatus == SquadWarStatusType.PhaseAction)
			{
				string text = "null";
				if (this.currentParticipantState != null)
				{
					text = this.currentParticipantState.TurnsLeft.ToString();
				}
				string message = string.Concat(new string[]
				{
					ServerTime.Time.ToString(),
					"|",
					id,
					"|",
					text
				});
				Service.BILoggingController.TrackGameAction(this.CurrentSquadWar.WarId, "squad_wars_scout", message, null, 1);
			}
		}

		private void OnStartAttackOnBuffBaseFailure(uint status, object cookie)
		{
			this.ShowPlayerInstructionErrorBasedOnStatus(status, true);
			Service.EventManager.SendEvent(EventId.WarAttackCommandFailed, null);
		}

		private void OnStartAttackOnWarMemberFailure(uint status, object cookie)
		{
			this.ShowPlayerInstructionErrorBasedOnStatus(status, false);
			Service.EventManager.SendEvent(EventId.WarAttackCommandFailed, null);
		}

		private void OnScoutWarMemberFailure(uint status)
		{
			this.ShowPlayerInstructionErrorBasedOnStatus(status, false);
		}

		private void ShowPlayerInstructionErrorBasedOnStatus(uint status, bool buffBaseBattle)
		{
			bool isPvp = !buffBaseBattle;
			string failureStringIdByStatus = SquadUtils.GetFailureStringIdByStatus(status, isPvp);
			string instructions = Service.Lang.Get(failureStringIdByStatus, new object[0]);
			Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(instructions);
		}

		public bool ScoutBuffBase(string buffBaseUid)
		{
			if (this.currentParticipantState != null && !this.EnsurePlayerHasTroops())
			{
				return false;
			}
			if (this.controller.StateManager.GetCurrentSquad() == null)
			{
				return false;
			}
			if (this.FindBuffBaseData(buffBaseUid) == null)
			{
				return false;
			}
			ProcessingScreen.Show();
			this.currentlyScoutedBuffBaseId = buffBaseUid;
			SquadWarGetBuffBaseStatusRequest request = new SquadWarGetBuffBaseStatusRequest(Service.CurrentPlayer.PlayerId, buffBaseUid);
			SquadWarGetBuffBaseStatusCommand squadWarGetBuffBaseStatusCommand = new SquadWarGetBuffBaseStatusCommand(request);
			squadWarGetBuffBaseStatusCommand.AddSuccessCallback(new AbstractCommand<SquadWarGetBuffBaseStatusRequest, SquadWarBuffBaseResponse>.OnSuccessCallback(this.OnGetBuffBaseStatusSuccess));
			squadWarGetBuffBaseStatusCommand.AddFailureCallback(new AbstractCommand<SquadWarGetBuffBaseStatusRequest, SquadWarBuffBaseResponse>.OnFailureCallback(this.OnGetBuffBaseStatusFailure));
			Service.ServerAPI.Sync(squadWarGetBuffBaseStatusCommand);
			return true;
		}

		public void StartAttackOnScoutedBuffBase()
		{
			SquadWarAttackBuffBaseRequest request = new SquadWarAttackBuffBaseRequest(this.currentlyScoutedBuffBaseId);
			SquadWarAttackBuffBaseCommand squadWarAttackBuffBaseCommand = new SquadWarAttackBuffBaseCommand(request);
			squadWarAttackBuffBaseCommand.AddSuccessCallback(new AbstractCommand<SquadWarAttackBuffBaseRequest, BattleIdResponse>.OnSuccessCallback(this.OnStartAttackOnBuffBaseSuccess));
			squadWarAttackBuffBaseCommand.AddFailureCallback(new AbstractCommand<SquadWarAttackBuffBaseRequest, BattleIdResponse>.OnFailureCallback(this.OnStartAttackOnBuffBaseFailure));
			Service.ServerAPI.Sync(squadWarAttackBuffBaseCommand);
		}

		private void OnStartAttackOnBuffBaseSuccess(BattleIdResponse response, object cookie)
		{
			Service.BattleController.GetCurrentBattle().RecordID = response.BattleId;
			Service.GameStateMachine.SetState(new BattlePlayState());
		}

		private void BattleInitializationShowBuffs(bool showOpponentBuffs)
		{
			Service.Logger.Debug("BattleInitializationShowBuffs");
			if (this.CurrentSquadWar != null)
			{
				Lang lang = Service.Lang;
				StaticDataController staticDataController = Service.StaticDataController;
				MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
				List<SquadWarBuffBaseData> buffBases = this.CurrentSquadWar.BuffBases;
				Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
				int i = 0;
				int count = buffBases.Count;
				while (i < count)
				{
					SquadWarBuffBaseData squadWarBuffBaseData = buffBases[i];
					WarBuffVO warBuffVO = staticDataController.Get<WarBuffVO>(squadWarBuffBaseData.BuffBaseId);
					string text = string.Empty;
					if (squadWarBuffBaseData.OwnerId == currentSquad.SquadID)
					{
						text = lang.Get("BUFF_BASE_OWNERSHIP_PLAYER", new object[]
						{
							lang.Get(warBuffVO.BuffStringTitle, new object[0])
						});
					}
					else if (showOpponentBuffs && !string.IsNullOrEmpty(squadWarBuffBaseData.OwnerId))
					{
						text = lang.Get("BUFF_BASE_OWNERSHIP_OPPONENT", new object[]
						{
							lang.Get(warBuffVO.BuffStringTitle, new object[0])
						});
					}
					if (!string.IsNullOrEmpty(text))
					{
						miscElementsManager.ShowPlayerInstructions(text);
					}
					i++;
				}
			}
		}

		public bool CanScoutWarMember(string memberId, ref string message)
		{
			SquadWarParticipantState squadWarParticipantState = this.FindParticipantState(memberId);
			if (squadWarParticipantState == null)
			{
				return false;
			}
			SquadWarStatusType currentStatus = this.GetCurrentStatus();
			SquadWarSquadType participantSquad = this.GetParticipantSquad(squadWarParticipantState.SquadMemberId);
			if (currentStatus != SquadWarStatusType.PhasePrep && currentStatus != SquadWarStatusType.PhaseAction)
			{
				if (participantSquad == SquadWarSquadType.OPPONENT_SQUAD)
				{
					message = Service.Lang.Get("WAR_PREVENT_ENEMY_SCOUT_WRONG_PHASE", new object[0]);
				}
				else
				{
					message = Service.Lang.Get("WAR_PREVENT_ALLY_SCOUT_WRONG_PHASE", new object[0]);
				}
				return false;
			}
			return this.currentParticipantState != null || true;
		}

		public bool ScoutWarMember(string memberId)
		{
			if (this.currentParticipantState != null && !this.EnsurePlayerHasTroops())
			{
				return false;
			}
			ProcessingScreen.Show();
			ScoutSquadWarParticipantCommand scoutSquadWarParticipantCommand = new ScoutSquadWarParticipantCommand(new SquadWarParticipantIdRequest
			{
				ParticipantId = memberId
			});
			scoutSquadWarParticipantCommand.AddSuccessCallback(new AbstractCommand<SquadWarParticipantIdRequest, SquadMemberWarDataResponse>.OnSuccessCallback(this.OnScoutWarMemberSuccess));
			Service.ServerAPI.Sync(scoutSquadWarParticipantCommand);
			return true;
		}

		private void OnScoutWarMemberSuccess(SquadMemberWarDataResponse response, object cookie)
		{
			ProcessingScreen.Hide();
			if (SquadUtils.IsNotFatalServerError(response.ScoutingStatus))
			{
				this.OnScoutWarMemberFailure(response.ScoutingStatus);
				return;
			}
			SquadMemberWarData memberWarData = response.MemberWarData;
			SquadWarSquadType participantSquad = this.GetParticipantSquad(memberWarData.SquadMemberId);
			SquadWarSquadData squadData = this.GetSquadData(participantSquad);
			BattleInitializationData battleInitializationData = BattleInitializationData.CreateFromMemberWarData(memberWarData, response.DonatedSquadTroops, response.Champions, squadData.Faction, squadData.SquadId, response.Equipment);
			TransitionCompleteDelegate onComplete = new TransitionCompleteDelegate(this.OnBattleReady);
			if (battleInitializationData.Attacker.PlayerFaction == battleInitializationData.Defender.PlayerFaction)
			{
				onComplete = new TransitionCompleteDelegate(this.OnSameFactionScoutReady);
			}
			this.LogScoutBIGameAction(memberWarData.SquadMemberId);
			BattleStartState.GoToBattleStartState(battleInitializationData, onComplete);
		}

		private bool EnsurePlayerHasTroops()
		{
			if (!GameUtils.HasAvailableTroops(false, null))
			{
				Lang lang = Service.Lang;
				AlertScreen.ShowModal(false, lang.Get("NOT_ENOUGH_TROOPS_TITLE", new object[0]), lang.Get("NOT_ENOUGH_TROOPS_FOR_ATTACK", new object[0]), null, null);
				Service.EventManager.SendEvent(EventId.UISquadWarScreen, new ActionMessageBIData("PvP_or_buffbase", "no_troops"));
				return false;
			}
			return true;
		}

		public void StartAttackOnScoutedWarMember()
		{
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			SquadWarAttackPlayerStartRequest request = new SquadWarAttackPlayerStartRequest(currentBattle.Defender.PlayerId);
			SquadWarAttackPlayerStartCommand squadWarAttackPlayerStartCommand = new SquadWarAttackPlayerStartCommand(request);
			squadWarAttackPlayerStartCommand.AddSuccessCallback(new AbstractCommand<SquadWarAttackPlayerStartRequest, BattleIdResponse>.OnSuccessCallback(this.OnStartAttackOnWarMemberSuccess));
			squadWarAttackPlayerStartCommand.AddFailureCallback(new AbstractCommand<SquadWarAttackPlayerStartRequest, BattleIdResponse>.OnFailureCallback(this.OnStartAttackOnWarMemberFailure));
			Service.ServerAPI.Sync(squadWarAttackPlayerStartCommand);
		}

		private void OnStartAttackOnWarMemberSuccess(BattleIdResponse response, object cookie)
		{
			Service.BattleController.GetCurrentBattle().RecordID = response.BattleId;
			Service.GameStateMachine.SetState(new BattlePlayState());
		}

		private void OnBattleReady()
		{
			this.BattleInitializationShowBuffs(true);
		}

		private void OnSameFactionScoutReady()
		{
			this.BattleInitializationShowBuffs(false);
		}

		public string GetCurrentlyScoutedBuffBaseId()
		{
			return this.currentlyScoutedBuffBaseId;
		}

		public void ReleaseCurrentlyScoutedBuffBase()
		{
			this.currentlyScoutedBuffBaseId = null;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.WorldInTransitionComplete)
			{
				if (id != EventId.ContractCompleted)
				{
					if (id != EventId.WarPhaseChanged)
					{
						if (id == EventId.WarLaunchFlow)
						{
							this.LaunchSquadWarFlow();
						}
					}
					else if ((SquadWarStatusType)cookie == SquadWarStatusType.PhaseOpen)
					{
						this.EndSquadWar();
					}
				}
				else if (this.currentParticipantState != null)
				{
					ContractEventData contractEventData = cookie as ContractEventData;
					ContractType contractType = contractEventData.Contract.ContractTO.ContractType;
					if (contractType == ContractType.Upgrade)
					{
						BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(contractEventData.Contract.ProductUid);
						if (buildingTypeVO.Type == BuildingType.Squad)
						{
							this.controller.UpdateCurrentSquad();
						}
					}
				}
			}
			else
			{
				this.CheckForWarboardForceExit();
				Service.EventManager.UnregisterObserver(this, EventId.WorldInTransitionComplete);
			}
			return EatResponse.NotEaten;
		}

		public bool WarExists()
		{
			return this.CurrentSquadWar != null && !string.IsNullOrEmpty(this.CurrentSquadWar.WarId);
		}

		public bool CanStartSquadWar()
		{
			SquadRole role = this.controller.StateManager.Role;
			return !this.WarExists() && (role == SquadRole.Officer || role == SquadRole.Owner);
		}

		public bool IsMemberInWarParty(string memberId)
		{
			return this.warParty.Contains(memberId);
		}

		public bool IsSquadMemberInWarOrMatchmaking(SquadMember squadMember)
		{
			return squadMember.WarParty == 1;
		}

		public int GetWarPartyCount()
		{
			return this.warParty.Count;
		}

		public bool IsEligibleForWarParty(SquadMember squadMember)
		{
			bool flag = squadMember.HQLevel >= GameConstants.WAR_PARTICIPANT_MIN_LEVEL;
			bool flag2 = this.warParty.Count < GameConstants.WAR_PARTICIPANT_COUNT;
			bool flag3 = !this.warParty.Contains(squadMember.MemberID);
			return flag && flag2 && flag3;
		}

		public bool WarPartyAdd(SquadMember squadMember)
		{
			if (this.IsEligibleForWarParty(squadMember))
			{
				this.warParty.Add(squadMember.MemberID);
				return true;
			}
			return false;
		}

		public bool WarPartyRemove(string memberId)
		{
			return !(memberId == Service.CurrentPlayer.PlayerId) && this.warParty.Remove(memberId);
		}

		public void StartMatchMakingPreparation()
		{
			if (!GameConstants.WAR_ALLOW_MATCHMAKING)
			{
				Lang lang = Service.Lang;
				AlertScreen.ShowModal(false, lang.Get("WAR_DISABLED_TITLE", new object[0]), lang.Get("WAR_DISABLED_BODY", new object[0]), null, null);
				return;
			}
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			if (!SquadUtils.CanStartMatchmakingPrep(this.controller, buildingLookupController))
			{
				return;
			}
			this.warParty.Clear();
			this.MatchMakingPrepMode = true;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			SquadMember squadMemberById = SquadUtils.GetSquadMemberById(currentSquad, currentPlayer.PlayerId);
			this.WarPartyAdd(squadMemberById);
			List<SquadMember> memberList = this.controller.StateManager.GetCurrentSquad().MemberList;
			List<SquadMember> list = new List<SquadMember>();
			int i = 0;
			int count = memberList.Count;
			while (i < count)
			{
				if (this.IsEligibleForWarParty(memberList[i]))
				{
					list.Add(memberList[i]);
				}
				i++;
			}
			list.Sort(new Comparison<SquadMember>(this.SortPotentialWarParty));
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				if (this.warParty.Count >= GameConstants.WAR_PARTICIPANT_COUNT)
				{
					break;
				}
				this.WarPartyAdd(list[j]);
				j++;
			}
			this.controller.StateManager.SquadScreenState = SquadScreenState.Members;
			Service.UXController.HUD.SlideSquadScreenOpen();
		}

		private int SortPotentialWarParty(SquadMember firstMember, SquadMember secondMember)
		{
			if (firstMember != null && secondMember == null)
			{
				return 1;
			}
			if (firstMember == null && secondMember != null)
			{
				return -1;
			}
			if (firstMember == null && secondMember == null)
			{
				return 0;
			}
			if (secondMember.BaseScore > firstMember.BaseScore)
			{
				return 1;
			}
			if (secondMember.BaseScore == firstMember.BaseScore)
			{
				if (secondMember.HQLevel > firstMember.HQLevel)
				{
					return 1;
				}
				if (secondMember.HQLevel == firstMember.HQLevel)
				{
					return secondMember.Score.CompareTo(firstMember.Score);
				}
			}
			return -1;
		}

		public bool IsCurrentSquadMatchmaking()
		{
			return this.controller.StateManager.GetCurrentSquad().WarSignUpTime != 0;
		}

		public void StartMatchMaking(bool allowSameFaction)
		{
			this.MatchMakingPrepMode = false;
			this.CurrentSquadWar = null;
			this.controller.StateManager.GetCurrentSquad().WarSignUpTime = (int)Service.ServerAPI.ServerTime;
			SquadMsg message = SquadMsgUtils.CreateStartMatchmakingMessage(this.warParty, allowSameFaction);
			this.controller.TakeAction(message);
		}

		public void CancelEnteringMatchmaking()
		{
			this.MatchMakingPrepMode = false;
			this.warParty.Clear();
		}

		public void CancelMatchMaking()
		{
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			int i = 0;
			int count = currentSquad.MemberList.Count;
			while (i < count)
			{
				currentSquad.MemberList[i].WarParty = 0;
				i++;
			}
			this.controller.StateManager.GetCurrentSquad().WarSignUpTime = 0;
			this.warParty.Clear();
		}

		public void CancelMatchMakingTakeAction()
		{
			this.CancelMatchMaking();
			SquadMsg message = SquadMsgUtils.CreateCancelMatchmakingMessage();
			this.controller.TakeAction(message);
		}

		public void OnWarMatchMakingBegin()
		{
			this.controller.StateManager.GetCurrentSquad().WarSignUpTime = (int)Service.ServerAPI.ServerTime;
			if (this.MatchMakingPrepMode)
			{
				this.CancelEnteringMatchmaking();
			}
			IGameState gameState = Service.GameStateMachine.CurrentState as IGameState;
			if (this.WarExists() && gameState is WarBoardState)
			{
				this.CheckForWarboardForceExit();
			}
			else
			{
				string instructions = Service.Lang.Get("WAR_MATCHMAKING_STARTED", new object[0]);
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(instructions);
				this.EndSquadWar();
			}
		}

		public void OnCelebrationScreenClosed(object result, object cookie)
		{
			this.CheckForWarboardForceExit();
		}

		public void CheckForWarboardForceExit()
		{
			ScreenController screenController = Service.ScreenController;
			if (!this.WarExists() || !this.IsCurrentSquadMatchmaking())
			{
				return;
			}
			if (Service.WorldTransitioner.IsTransitioning())
			{
				Service.EventManager.RegisterObserver(this, EventId.WorldInTransitionComplete);
				return;
			}
			SquadWarEndCelebrationScreen highestLevelScreen = screenController.GetHighestLevelScreen<SquadWarEndCelebrationScreen>();
			if (highestLevelScreen != null && !highestLevelScreen.IsClosing)
			{
				return;
			}
			InventoryCrateCollectionScreen highestLevelScreen2 = screenController.GetHighestLevelScreen<InventoryCrateCollectionScreen>();
			if (highestLevelScreen2 != null && !highestLevelScreen2.IsClosing)
			{
				return;
			}
			if (Service.ScreenController.GetHighestLevelScreen<AlertScreen>() != null)
			{
				return;
			}
			IGameState gameState = Service.GameStateMachine.CurrentState as IGameState;
			if (!(gameState is WarBoardState))
			{
				return;
			}
			this.EndSquadWar();
			Service.UXController.HUD.SlideSquadScreenClosedInstantly();
			Lang lang = Service.Lang;
			AlertScreen.ShowModal(false, lang.Get("WAR_BOARD_MATCHMAKING_EXIT_TITLE", new object[0]), lang.Get("WAR_BOARD_MATCHMAKING_EXIT_BODY", new object[0]), null, null, true);
		}

		public void ShowInfoScreen(UXButton button)
		{
			SquadWarInfoScreen screen = new SquadWarInfoScreen(-1);
			Service.ScreenController.AddScreen(screen);
		}

		private void LaunchSquadWarFlow()
		{
			if (this.controller.StateManager.GetCurrentSquad() == null)
			{
				SquadWarStartScreen screen = new SquadWarStartScreen();
				Service.ScreenController.AddScreen(screen);
				return;
			}
			if (Service.BuildingLookupController.GetHighestLevelHQ() < GameConstants.WAR_PARTICIPANT_MIN_LEVEL)
			{
				Lang lang = Service.Lang;
				AlertScreen.ShowModal(false, lang.Get("WAR_BOARD_INSUFFICIENT_LEVEL_TITLE", new object[0]), lang.Get("WAR_BOARD_INSUFFICIENT_LEVEL_BODY", new object[0]), new OnScreenModalResult(this.OnAcceptInsufficientLevel), null);
				return;
			}
			switch (this.GetCurrentStatus())
			{
			case SquadWarStatusType.PhaseOpen:
				if (this.IsCurrentSquadMatchmaking())
				{
					SquadWarMatchMakeScreen screen2 = new SquadWarMatchMakeScreen();
					Service.ScreenController.AddScreen(screen2);
				}
				else
				{
					SquadWarStartScreen screen3 = new SquadWarStartScreen();
					Service.ScreenController.AddScreen(screen3);
				}
				break;
			case SquadWarStatusType.PhasePrep:
			case SquadWarStatusType.PhasePrepGrace:
			case SquadWarStatusType.PhaseAction:
			case SquadWarStatusType.PhaseActionGrace:
			case SquadWarStatusType.PhaseCooldown:
				this.EnterWarBoardMode();
				break;
			}
		}

		private void OnAcceptInsufficientLevel(object result, object cookie)
		{
			Service.UXController.HUD.RefreshView();
		}

		public void StartTranstionFromWarBaseToWarBoard()
		{
			HomeMapDataLoader homeMapDataLoader = Service.HomeMapDataLoader;
			Service.WorldTransitioner.StartTransition(new WarbaseToWarboardTransition(new WarBoardState(), homeMapDataLoader, null, false, false));
			this.SetupWarBoardLighting(homeMapDataLoader, false);
		}

		public bool IsAlliedBuffBase(SquadWarBuffBaseData baseData)
		{
			SquadWarSquadData squadData = this.GetSquadData(SquadWarSquadType.PLAYER_SQUAD);
			string ownerId = baseData.OwnerId;
			return ownerId == squadData.SquadId;
		}

		public string GetWarBuffDisplayName(string buffUid)
		{
			Lang lang = Service.Lang;
			StaticDataController staticDataController = Service.StaticDataController;
			WarBuffVO optional = staticDataController.GetOptional<WarBuffVO>(buffUid);
			if (optional != null)
			{
				return lang.Get(optional.BuffBaseName, new object[0]);
			}
			return string.Empty;
		}

		public void Destroy()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			this.warParty.Clear();
			this.warParty = null;
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.WarLaunchFlow);
			eventManager.UnregisterObserver(this, EventId.ContractCompleted);
			eventManager.UnregisterObserver(this, EventId.WorldInTransitionComplete);
			eventManager.UnregisterObserver(this, EventId.WarPhaseChanged);
		}
	}
}
