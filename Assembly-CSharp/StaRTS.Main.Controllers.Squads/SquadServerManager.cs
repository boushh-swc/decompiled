using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.ServerMessages;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Chat;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Commands.Squads;
using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Squads
{
	public class SquadServerManager : IEventObserver
	{
		private SquadController controller;

		private SquadNotifAdapter notifAdapter;

		private PhotonChatServerAdapter chatAdapter;

		private List<SquadController.SquadMsgsCallback> callbacks;

		private List<SquadMsg> serverMessages;

		public SquadServerManager(SquadController controller)
		{
			this.controller = controller;
			this.notifAdapter = new SquadNotifAdapter();
			this.chatAdapter = new PhotonChatServerAdapter();
			this.callbacks = new List<SquadController.SquadMsgsCallback>();
		}

		public void Init()
		{
			this.notifAdapter.SetNotifStartDate(this.controller.StateManager.JoinDate);
			this.ConnectToChat();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.SquadServerMessage);
			eventManager.RegisterObserver(this, EventId.SquadUpdated);
			eventManager.RegisterObserver(this, EventId.SquadScreenOpenedOrClosed);
			eventManager.RegisterObserver(this, EventId.WarPhaseChanged);
			eventManager.RegisterObserver(this, EventId.SuccessfullyResumed);
		}

		public void EnablePolling()
		{
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			if (currentSquad != null)
			{
				float pollFrequency = this.GetPollFrequency();
				this.notifAdapter.EnableAndPoll(new SquadController.SquadMsgsCallback(this.OnNewSquadMsgs), pollFrequency);
				if (this.chatAdapter.GetChatSessionState() == PhotonChatSessionState.Disconnected)
				{
					this.ConnectToChat();
				}
			}
		}

		private void ConnectToChat()
		{
			if (GameConstants.PHOTON_CHAT_DISABLED)
			{
				this.chatAdapter.Disable();
				return;
			}
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return;
			}
			string pHOTON_CHAT_APP_VERSION = GameConstants.PHOTON_CHAT_APP_VERSION;
			if (string.IsNullOrEmpty(pHOTON_CHAT_APP_VERSION))
			{
				Service.Logger.Error("Connecting to Photon failed. photon app version not set in game constants");
				return;
			}
			string photonAppId = AppServerEnvironmentController.GetPhotonAppId();
			string playerId = Service.CurrentPlayer.PlayerId;
			string squadID = currentSquad.SquadID;
			this.chatAdapter.Connect(photonAppId, pHOTON_CHAT_APP_VERSION, GameConstants.PHOTON_CHAT_HISTORY_LENGTH, GameConstants.PHOTON_CHAT_KEEP_ALIVE_TICK, playerId);
			GetSquadChatKeyCommand getSquadChatKeyCommand = new GetSquadChatKeyCommand(new SquadIDRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId,
				SquadId = squadID
			});
			getSquadChatKeyCommand.AddSuccessCallback(new AbstractCommand<SquadIDRequest, GetSquadChatKeyResponse>.OnSuccessCallback(this.OnGetChatKeySuccess));
			getSquadChatKeyCommand.AddFailureCallback(new AbstractCommand<SquadIDRequest, GetSquadChatKeyResponse>.OnFailureCallback(this.OnGetChatKeyFailure));
			Service.ServerAPI.Sync(getSquadChatKeyCommand);
		}

		private void OnGetChatKeySuccess(GetSquadChatKeyResponse response, object cookie)
		{
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			if (currentSquad != null && response != null && !string.IsNullOrEmpty(response.ChatMessageEncryptionKey))
			{
				this.chatAdapter.SetMessageEncryptionKey(response.ChatMessageEncryptionKey);
				this.chatAdapter.Enable(new SquadController.SquadMsgsCallback(this.OnNewSquadMsgs));
				this.chatAdapter.StartSession(ChatType.Alliance, currentSquad.SquadID);
			}
			else
			{
				this.chatAdapter.Disable();
			}
		}

		private void OnGetChatKeyFailure(uint status, object cookie)
		{
			Service.Logger.Error("Failed to get chat key for squad!");
			this.chatAdapter.Disable();
		}

		private void DisablePolling()
		{
			this.notifAdapter.Disable();
			this.chatAdapter.Disable();
		}

		public void UpdatePollFrequency()
		{
			float pollFrequency = this.GetPollFrequency();
			this.notifAdapter.AdjustPollFrequency(pollFrequency);
		}

		public void AddSquadMsgCallback(SquadController.SquadMsgsCallback callback)
		{
			if (!this.callbacks.Contains(callback))
			{
				this.callbacks.Add(callback);
			}
		}

		private void OnNewSquadMsgs(List<SquadMsg> msgs)
		{
			int i = 0;
			int count = this.callbacks.Count;
			while (i < count)
			{
				if (this.callbacks[i] != null)
				{
					this.callbacks[i](msgs);
				}
				i++;
			}
		}

		private float GetPollFrequency()
		{
			return (!this.controller.StateManager.SquadScreenOpen) ? this.controller.PullFrequencyChatClosed : this.controller.PullFrequencyChatOpen;
		}

		public void Destroy()
		{
			this.DisablePolling();
			this.callbacks.Clear();
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.SquadServerMessage);
			eventManager.UnregisterObserver(this, EventId.SquadUpdated);
			eventManager.UnregisterObserver(this, EventId.SquadScreenOpenedOrClosed);
			eventManager.UnregisterObserver(this, EventId.WarPhaseChanged);
			eventManager.UnregisterObserver(this, EventId.GameStateChanged);
			eventManager.UnregisterObserver(this, EventId.SuccessfullyResumed);
		}

		public void TakeAction(SquadMsg message)
		{
			switch (message.ActionData.Type)
			{
			case SquadAction.Create:
				this.SendCommandRequest<CreateSquadRequest, SquadResponse>(new CreateSquadCommand(SquadMsgUtils.GenerateCreateSquadRequest(message)), message);
				break;
			case SquadAction.Join:
				this.SendCommandRequest<SquadIDRequest, SquadResponse>(new JoinSquadCommand(SquadMsgUtils.GenerateSquadIdRequest(message)), message);
				break;
			case SquadAction.Leave:
				this.SendCommandRequest<PlayerIdRequest, DefaultResponse>(new LeaveSquadCommand(SquadMsgUtils.GeneratePlayerIdRequest(message)), message);
				break;
			case SquadAction.Edit:
				this.SendCommandRequest<EditSquadRequest, DefaultResponse>(new EditSquadCommand(SquadMsgUtils.GenerateEditSquadRequest(message)), message);
				break;
			case SquadAction.ApplyToJoin:
				this.SendCommandRequest<ApplyToSquadRequest, DefaultResponse>(new ApplyToSquadCommand(SquadMsgUtils.GenerateApplyToSquadRequest(message)), message);
				break;
			case SquadAction.AcceptApplicationToJoin:
				this.SendCommandRequest<MemberIdRequest, SquadMemberResponse>(new AcceptSquadRequestCommand(SquadMsgUtils.GenerateMemberIdRequest(message)), message);
				break;
			case SquadAction.RejectApplicationToJoin:
				this.SendCommandRequest<MemberIdRequest, DefaultResponse>(new RejectSquadRequestCommand(SquadMsgUtils.GenerateMemberIdRequest(message)), message);
				break;
			case SquadAction.SendInviteToJoin:
				this.SendCommandRequest<SendSquadInviteRequest, DefaultResponse>(new SendSquadInviteCommand(SquadMsgUtils.GenerateSendInviteRequest(message)), message);
				break;
			case SquadAction.AcceptInviteToJoin:
				this.SendCommandRequest<SquadIDRequest, DefaultResponse>(new AcceptSquadInviteCommand(SquadMsgUtils.GenerateSquadIdRequest(message)), message);
				break;
			case SquadAction.RejectInviteToJoin:
				this.SendCommandRequest<SquadIDRequest, DefaultResponse>(new RejectSquadInviteCommand(SquadMsgUtils.GenerateSquadIdRequest(message)), message);
				break;
			case SquadAction.PromoteMember:
				this.SendCommandRequest<MemberIdRequest, DefaultResponse>(new PromoteSquadMemberCommand(SquadMsgUtils.GenerateMemberIdRequest(message)), message);
				break;
			case SquadAction.DemoteMember:
				this.SendCommandRequest<MemberIdRequest, DefaultResponse>(new DemoteSquadMemberCommand(SquadMsgUtils.GenerateMemberIdRequest(message)), message);
				break;
			case SquadAction.RemoveMember:
				this.SendCommandRequest<MemberIdRequest, DefaultResponse>(new RemoveSquadMemberCommand(SquadMsgUtils.GenerateMemberIdRequest(message)), message);
				break;
			case SquadAction.RequestTroops:
				this.SendCommandRequest<TroopSquadRequest, DefaultResponse>(new SquadTroopRequestCommand(SquadMsgUtils.GenerateTroopRequest(message)), message);
				break;
			case SquadAction.DonateTroops:
				this.SendCommandRequest<TroopDonateRequest, TroopDonateResponse>(new SquadTroopDonateCommand(SquadMsgUtils.GenerateTroopDonateRequest(message)), message);
				break;
			case SquadAction.RequestWarTroops:
				this.SendCommandRequest<TroopSquadRequest, DefaultResponse>(new SquadWarTroopRequestCommand(SquadMsgUtils.GenerateTroopRequest(message)), message);
				break;
			case SquadAction.DonateWarTroops:
				this.SendCommandRequest<TroopDonateRequest, TroopDonateResponse>(new SquadWarTroopDonateCommand(SquadMsgUtils.GenerateTroopDonateRequest(message)), message);
				break;
			case SquadAction.ShareReplay:
				this.SendCommandRequest<ShareReplayRequest, DefaultResponse>(new ShareReplayCommand(SquadMsgUtils.GenerateShareReplayRequest(message)), message);
				break;
			case SquadAction.StartWarMatchmaking:
				this.SendCommandRequest<SquadWarStartMatchmakingRequest, DefaultResponse>(new SquadWarStartMatchmakingCommand(SquadMsgUtils.GenerateStartWarMatchmakingRequest(message)), message);
				break;
			case SquadAction.CancelWarMatchmaking:
				this.SendCommandRequest<PlayerIdChecksumRequest, DefaultResponse>(new SquadWarCancelMatchmakingCommand(SquadMsgUtils.GeneratePlayerIdChecksumRequest(message)), message);
				break;
			}
		}

		private void SendCommandRequest<TRequest, TResponse>(AbstractCommand<TRequest, TResponse> command, object context) where TRequest : AbstractRequest where TResponse : AbstractResponse
		{
			if (command != null)
			{
				command.AddSuccessCallback(new AbstractCommand<TRequest, TResponse>.OnSuccessCallback(this.OnActionCommandSuccess));
				command.AddFailureCallback(new AbstractCommand<TRequest, TResponse>.OnFailureCallback(this.OnActionCommandFailure));
				command.Context = context;
				Service.ServerAPI.Sync(command);
			}
		}

		private void OnActionCommandSuccess(AbstractResponse response, object cookie)
		{
			SquadMsg squadMsg = (SquadMsg)cookie;
			SqmActionData actionData = squadMsg.ActionData;
			SquadAction type = actionData.Type;
			SquadMsg squadMsg2;
			switch (type)
			{
			case SquadAction.Create:
			case SquadAction.Join:
				squadMsg2 = SquadMsgUtils.GenerateMessageFromSquadResponse((SquadResponse)response, Service.LeaderboardController);
				goto IL_B5;
			case SquadAction.Leave:
				squadMsg2 = squadMsg;
				this.controller.WarManager.ClearSquadWarData();
				goto IL_B5;
			case SquadAction.Edit:
			case SquadAction.ApplyToJoin:
				IL_35:
				switch (type)
				{
				case SquadAction.DonateTroops:
				case SquadAction.DonateWarTroops:
					squadMsg2 = SquadMsgUtils.GenerateMessageFromTroopDonateResponse((TroopDonateResponse)response);
					Service.TroopDonationTrackController.UpdateTroopDonationProgress((TroopDonateResponse)response);
					goto IL_B5;
				}
				squadMsg2 = squadMsg;
				goto IL_B5;
			case SquadAction.AcceptApplicationToJoin:
				squadMsg2 = SquadMsgUtils.GenerateMessageFromSquadMemberResponse((SquadMemberResponse)response);
				goto IL_B5;
			}
			goto IL_35;
			IL_B5:
			squadMsg2.BISource = squadMsg.BISource;
			this.controller.OnPlayerActionSuccess(actionData.Type, squadMsg2);
			if (actionData.Callback != null)
			{
				actionData.Callback(true, actionData.Cookie);
			}
		}

		private void OnActionCommandFailure(uint status, object cookie)
		{
			SquadMsg squadMsg = (SquadMsg)cookie;
			this.controller.OnPlayerActionFailure(squadMsg, status);
			if (squadMsg.ActionData.Callback != null)
			{
				squadMsg.ActionData.Callback(false, cookie);
			}
		}

		public bool CanPublishMessage()
		{
			return this.chatAdapter.CanPublishMessage();
		}

		public void PublishChatMessage(string message)
		{
			this.chatAdapter.PublishMessage(message);
		}

		public void UpdateSquadInvitesReceived()
		{
			GetSquadInvitesCommand getSquadInvitesCommand = new GetSquadInvitesCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getSquadInvitesCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, GetSquadInvitesResponse>.OnSuccessCallback(this.OnGetSquadInvitesSuccess));
			Service.ServerAPI.Sync(getSquadInvitesCommand);
		}

		private void OnGetSquadInvitesSuccess(GetSquadInvitesResponse response, object cookie)
		{
			this.controller.HandleSquadInvitesReceived(response.SquadInvites);
		}

		public void UpdateSquadInvitesSentToPlayers(List<string> playerIds, Action callback)
		{
			GetSquadInvitesSentRequest request = new GetSquadInvitesSentRequest(playerIds);
			GetSquadInvitesSentCommand getSquadInvitesSentCommand = new GetSquadInvitesSentCommand(request);
			getSquadInvitesSentCommand.Context = callback;
			getSquadInvitesSentCommand.AddSuccessCallback(new AbstractCommand<GetSquadInvitesSentRequest, GetSquadInvitesSentResponse>.OnSuccessCallback(this.OnGetSquadInvitesSentSuccess));
			getSquadInvitesSentCommand.AddFailureCallback(new AbstractCommand<GetSquadInvitesSentRequest, GetSquadInvitesSentResponse>.OnFailureCallback(this.OnGetSquadInvitesSentFailure));
			Service.ServerAPI.Sync(getSquadInvitesSentCommand);
		}

		private void OnGetSquadInvitesSentSuccess(GetSquadInvitesSentResponse response, object cookie)
		{
			this.controller.HandleSquadInvitesSentToPlayers(response.PlayersWithPendingInvites, (Action)cookie);
		}

		private void OnGetSquadInvitesSentFailure(uint status, object cookie)
		{
			this.controller.HandleSquadInvitesSentToPlayers(null, (Action)cookie);
		}

		public void UpdateCurrentSquad()
		{
			GetSquadCommand getSquadCommand = new GetSquadCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getSquadCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, SquadResponse>.OnSuccessCallback(this.OnUpdateSquadSuccess));
			Service.ServerAPI.Sync(getSquadCommand);
		}

		private void OnUpdateSquadSuccess(SquadResponse response, object cookie)
		{
			Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
			if (currentSquad != null)
			{
				string currentWarId = currentSquad.CurrentWarId;
				currentSquad.FromObject(response.SquadData);
				this.controller.SyncCurrentPlayerRole();
				bool flag = !string.IsNullOrEmpty(currentSquad.CurrentWarId);
				if (flag && (this.controller.WarManager.CurrentSquadWar == null || currentSquad.CurrentWarId != currentWarId))
				{
					this.UpdateSquadWar(currentSquad.CurrentWarId, false);
				}
				else
				{
					SquadWarStatusType currentStatus = this.controller.WarManager.GetCurrentStatus();
					if (flag && currentStatus == SquadWarStatusType.PhasePrep)
					{
						this.UpdateCurrentSquadWar();
					}
					else
					{
						Service.EventManager.SendEvent(EventId.SquadUpdateCompleted, null);
					}
				}
			}
			else
			{
				Service.EventManager.SendEvent(EventId.SquadUpdateCompleted, null);
			}
			Service.EventManager.SendEvent(EventId.SquadDetailsUpdated, null);
		}

		public void UpdateCurrentSquadWar()
		{
			if (this.IsValidUpdateGameState())
			{
				SquadWarData currentSquadWar = this.controller.WarManager.CurrentSquadWar;
				if (currentSquadWar != null)
				{
					this.UpdateSquadWar(currentSquadWar.WarId, true);
				}
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.GameStateChanged);
			}
		}

		private void UpdateSquadWar(string warId, bool updateMemberWarData)
		{
			if (updateMemberWarData)
			{
				this.QueueUpdateCurrentMemberWarData();
			}
			GetSquadWarStatusRequest request = new GetSquadWarStatusRequest(Service.CurrentPlayer.PlayerId, warId);
			GetSquadWarStatusCommand getSquadWarStatusCommand = new GetSquadWarStatusCommand(request);
			getSquadWarStatusCommand.AddSuccessCallback(new AbstractCommand<GetSquadWarStatusRequest, GetSquadWarStatusResponse>.OnSuccessCallback(this.OnUpdateSquadWarSuccess));
			Service.ServerAPI.Sync(getSquadWarStatusCommand);
		}

		private void OnUpdateSquadWarSuccess(GetSquadWarStatusResponse response, object cookie)
		{
			SquadMsg squadMsg = SquadMsgUtils.GenerateMessageFromGetSquadWarStatusResponse(response);
			this.controller.InitSquadWarState(squadMsg.CurrentSquadWarData);
			Service.EventManager.SendEvent(EventId.SquadUpdateCompleted, null);
		}

		public void QueueUpdateCurrentMemberWarData()
		{
			GetSquadMemberWarDataCommand getSquadMemberWarDataCommand = new GetSquadMemberWarDataCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getSquadMemberWarDataCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, SquadMemberWarDataResponse>.OnSuccessCallback(this.OnUpdateCurrentMemberWarDataSuccess));
			Service.ServerAPI.Enqueue(getSquadMemberWarDataCommand);
		}

		private void OnUpdateCurrentMemberWarDataSuccess(SquadMemberWarDataResponse response, object cookie)
		{
			this.controller.WarManager.UpdateCurrentMemberWarData(response);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.SquadScreenOpenedOrClosed)
			{
				if (id != EventId.SquadUpdated)
				{
					if (id != EventId.SquadServerMessage)
					{
						if (id != EventId.WarPhaseChanged)
						{
							if (id != EventId.SuccessfullyResumed)
							{
								if (id == EventId.GameStateChanged)
								{
									SquadWarManager warManager = this.controller.WarManager;
									if (this.IsValidUpdateGameState() && warManager.WarExists())
									{
										this.UpdateSquadWar(warManager.CurrentSquadWar.WarId, true);
										Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
									}
								}
							}
							else
							{
								Squad currentSquad = this.controller.StateManager.GetCurrentSquad();
								if (currentSquad != null)
								{
									this.chatAdapter.ReconnectSession();
								}
							}
						}
						else
						{
							SquadWarStatusType squadWarStatusType = (SquadWarStatusType)((int)cookie);
							this.UpdateCurrentSquadWar();
							if (squadWarStatusType == SquadWarStatusType.PhaseCooldown)
							{
								this.controller.UpdateCurrentSquad();
							}
						}
					}
					else
					{
						SquadServerMessage squadServerMessage = (SquadServerMessage)cookie;
						if (squadServerMessage.Messages != null)
						{
							if (this.serverMessages == null)
							{
								this.serverMessages = new List<SquadMsg>();
							}
							else
							{
								this.serverMessages.Clear();
							}
							uint num = 0u;
							int i = 0;
							int count = squadServerMessage.Messages.Count;
							while (i < count)
							{
								SquadMsg squadMsg = SquadMsgUtils.GenerateMessageFromServerMessageObject(squadServerMessage.Messages[i]);
								if (squadMsg != null)
								{
									this.serverMessages.Add(squadMsg);
									if (!string.IsNullOrEmpty(squadMsg.NotifId) && squadMsg.TimeSent > num)
									{
										num = squadMsg.TimeSent;
									}
								}
								i++;
							}
							if (num > 0u)
							{
								this.notifAdapter.ResetPollTimer(num);
							}
							this.OnNewSquadMsgs(this.serverMessages);
						}
					}
				}
				else if (cookie == null)
				{
					this.DisablePolling();
				}
				else
				{
					this.EnablePolling();
				}
			}
			else
			{
				this.UpdatePollFrequency();
			}
			return EatResponse.NotEaten;
		}

		private bool IsValidUpdateGameState()
		{
			IGameState gameState = (IGameState)Service.GameStateMachine.CurrentState;
			return gameState != null && gameState.CanUpdateHomeContracts();
		}
	}
}
