using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Chat;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Squads
{
	public class SquadMsgChatDisplay : AbstractSquadMsgDisplay, IEventObserver
	{
		private const string CHAT_ITEM = "ChatItem";

		private const int MAX_PLAYER_NAME_LENGTH = 30;

		private const string ELLIPSE = "SQUAD_CHAT_PLAYER_NAME_SUFFIX";

		private const string NGUI_LINE_WRAP_ENDING_TEXT = "   ";

		public const int CHAT_ITEM_UPDATEONLY_SIZE = 36;

		public const int CHAT_ITEM_SMALL_SIZE = 110;

		public const int CHAT_ITEM_LARGE_SIZE = 170;

		private const string BUTTON_ACCEPT = "BUTTON_ACCEPT";

		private const string BUTTON_DECLINE = "BUTTON_DECLINE";

		private const string BUTTON_REPLAY = "BUTTON_REPLAY";

		private const string SQUAD_NOTIF_POSTFIX = "_SQUAD_NOTIF";

		private const string SQUAD_ROLE = "SQUAD_ROLE_EMPHASIS";

		private const string SQUAD_INVITE_ACCEPTED_CHAT = "SQUAD_INVITE_ACCEPTED_CHAT";

		private const string SQUAD_OFFENSE = "SQUAD_OFFENSE";

		private const string SQUAD_DEFENSE = "SQUAD_DEFENSE";

		private const string FRACTION = "FRACTION";

		private const string PERCENTAGE = "PERCENTAGE";

		private const string BUTTON_DONATE = "BUTTON_DONATE";

		private const string SQUAD_DONATE_EMPIRE = "SQUAD_DONATE_EMPIRE";

		private const string SQUAD_DONATE_REBEL = "SQUAD_DONATE_REBEL";

		private const string ONE_DONATOR = "X_DONATED_TROOPS_TO_Y";

		private const string TWO_DONATOR = "X_AND_Y_DONATED_TROOPS_TO_Z";

		private const string THREE_DONATOR = "X_Y_AND_Z_DONATED_TROOPS_TO_A";

		private const string FOUR_OR_MORE_DONATOR = "X_Y_Z_AND_A_DONATED_TROOPS_TO_B";

		private const string JOIN_REQUEST_ACCEPTED = "s_sqd_Welcome";

		private const string JOIN_REQUEST_REJECTED = "s_sqd_Rejected";

		private const string WAR_MATCHMAKING_STARTED = "WAR_MATCHMAKING_STARTED";

		private const string WAR_ENDED = "WAR_END_TRANSMISSION_TITLE";

		private const string WAR_BUFF_BASE_CAPTURED = "WAR_BUFF_BASE_CAPTURED";

		private const string WAR_PLAYER_ATTACKED = "WAR_PLAYER_ATTACKED";

		private const string COLORIZE_PLAYERNAME = "SQUAD_CHAT_PLAYER_NAME_COLORIZE";

		private const string SQUAD_LEVELED_UP = "PERK_SQUAD_CHAT_LEVEL_UP";

		private const string PERK_SQUAD_CHAT_PERK_UPGRADE = "PERK_SQUAD_CHAT_PERK_UPGRADE";

		private const string PERK_SQUAD_CHAT_PERK_UNLOCK = "PERK_SQUAD_CHAT_PERK_UNLOCK";

		private const string WAR_SUFFIX = "war";

		private int nextItemId = -1;

		private Dictionary<SquadMsg, ChatItemElements> msgToElementsMap;

		private Dictionary<SquadMsg, UXElement> joinRequestItems;

		private Dictionary<SquadMsg, UXElement> warTroopRequestItems;

		private Dictionary<string, List<SquadMsg>> donationMsgsByRequestId;

		private Dictionary<string, SquadMsg> lastRequestMessage;

		private SquadSlidingScreen parentScreen;

		private Dictionary<ChatFilterType, List<SquadMsgType>> chatFilterMappings;

		private bool readyForNewMessageSetup;

		public SquadMsgChatDisplay(SquadSlidingScreen parentScreen, UXTable table) : base(table)
		{
			table.BypassLocalPositionOnAdd = true;
			table.HideInactive = true;
			table.ChangeScrollDirection(true);
			this.parentScreen = parentScreen;
			this.msgToElementsMap = new Dictionary<SquadMsg, ChatItemElements>();
			this.lastRequestMessage = new Dictionary<string, SquadMsg>();
			this.chatFilterMappings = new Dictionary<ChatFilterType, List<SquadMsgType>>();
			this.chatFilterMappings[ChatFilterType.Messages] = new List<SquadMsgType>();
			this.chatFilterMappings[ChatFilterType.Messages].Add(SquadMsgType.Chat);
			this.chatFilterMappings[ChatFilterType.Requests] = new List<SquadMsgType>();
			this.chatFilterMappings[ChatFilterType.Requests].Add(SquadMsgType.JoinRequest);
			this.chatFilterMappings[ChatFilterType.Requests].Add(SquadMsgType.TroopRequest);
			this.chatFilterMappings[ChatFilterType.Requests].Add(SquadMsgType.Invite);
			this.chatFilterMappings[ChatFilterType.Replays] = new List<SquadMsgType>();
			this.chatFilterMappings[ChatFilterType.Replays].Add(SquadMsgType.ShareBattle);
			this.chatFilterMappings[ChatFilterType.Updates] = new List<SquadMsgType>();
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.Join);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.JoinRequestAccepted);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.JoinRequestRejected);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.InviteAccepted);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.Leave);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.Ejected);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.Promotion);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.Demotion);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.Invite);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.InviteRejected);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.WarMatchMakingBegin);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.WarStarted);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.WarPrepared);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.WarBuffBaseAttackStart);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.WarBuffBaseAttackComplete);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.WarPlayerAttackStart);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.WarPlayerAttackComplete);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.SquadLevelUp);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.PerkUnlocked);
			this.chatFilterMappings[ChatFilterType.Updates].Add(SquadMsgType.PerkUpgraded);
			Service.EventManager.RegisterObserver(this, EventId.SquadChatFilterUpdated);
			Service.EventManager.RegisterObserver(this, EventId.WarPhaseChanged);
			this.readyForNewMessageSetup = false;
		}

		public void OnExistingMessagesSetup()
		{
			this.readyForNewMessageSetup = true;
			this.table.RepositionItems();
		}

		public override void ProcessNewMessages(List<SquadMsg> messages)
		{
			bool flag = false;
			int i = 0;
			int count = messages.Count;
			while (i < count)
			{
				bool flag2 = this.ProcessMessage(messages[i]);
				flag = (flag || flag2);
				i++;
			}
			if (flag)
			{
				this.parentScreen.UpdateBadges();
				this.table.RepositionItems();
			}
		}

		protected override bool ProcessMessage(SquadMsg msg)
		{
			return this.ProcessMessage(msg, true);
		}

		public bool ProcessMessage(SquadMsg msg, bool isNew)
		{
			if (msg.OwnerData == null)
			{
				return false;
			}
			if (isNew && !this.readyForNewMessageSetup)
			{
				return false;
			}
			this.nextItemId++;
			PerkVO perkVO = null;
			if (msg.PerkData != null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				perkVO = staticDataController.GetOptional<PerkVO>(msg.PerkData.PerkUId);
				if (perkVO == null)
				{
					return false;
				}
			}
			string playerName = msg.OwnerData.PlayerName;
			string playerId = msg.OwnerData.PlayerId;
			UXElement uXElement = null;
			ChatItemElements chatItemElements = null;
			int desiredHeight = 170;
			switch (msg.Type)
			{
			case SquadMsgType.Chat:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupPlayerName(chatItemElements, playerName);
				this.SetupPlayerRole(chatItemElements, playerId);
				this.SetupChatMessage(chatItemElements, msg.ChatData);
				desiredHeight = 110;
				break;
			case SquadMsgType.Join:
			case SquadMsgType.Leave:
			case SquadMsgType.Ejected:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupJoinOrLeave(chatItemElements, playerName, msg.Type);
				desiredHeight = 36;
				break;
			case SquadMsgType.JoinRequest:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupPlayerName(chatItemElements, playerName);
				this.SetupJoinRequest(chatItemElements, msg);
				this.SetupChatMessage(chatItemElements, msg.ChatData);
				if (this.joinRequestItems == null)
				{
					this.joinRequestItems = new Dictionary<SquadMsg, UXElement>();
				}
				if (!this.joinRequestItems.ContainsKey(msg))
				{
					this.joinRequestItems.Add(msg, uXElement);
				}
				break;
			case SquadMsgType.JoinRequestAccepted:
			case SquadMsgType.JoinRequestRejected:
				if (msg.ApplyData != null)
				{
					uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
					this.SetupJoinOrLeave(chatItemElements, playerName, msg.Type);
					desiredHeight = 36;
				}
				break;
			case SquadMsgType.InviteAccepted:
				if (msg.FriendInviteData != null)
				{
					uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
					this.SetupInviteAccepted(chatItemElements, playerName, msg.FriendInviteData.SenderId);
					desiredHeight = 36;
				}
				break;
			case SquadMsgType.Promotion:
			case SquadMsgType.Demotion:
				if (SquadUtils.IsPlayerInSquad(playerId, Service.SquadController.StateManager.GetCurrentSquad()))
				{
					uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
					this.SetupRoleChange(chatItemElements, playerName, msg.MemberData.MemberRole, msg.Type);
					desiredHeight = 36;
					if (Service.CurrentPlayer.PlayerId == playerId)
					{
						this.UpdateExistingJoinRequestActionPermissions();
					}
				}
				break;
			case SquadMsgType.ShareBattle:
				if (msg.ReplayData != null && GameUtils.IsBattleVersionSupported(msg.ReplayData.CMSVersion, msg.ReplayData.BattleVersion) && !string.IsNullOrEmpty(msg.ReplayData.OpponentId) && !string.IsNullOrEmpty(msg.ReplayData.BattleId))
				{
					uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
					this.SetupPlayerName(chatItemElements, playerName);
					this.SetupPlayerRole(chatItemElements, playerId);
					this.SetupChatMessage(chatItemElements, msg.ChatData);
					this.SetupBattleReplay(chatItemElements, msg);
				}
				break;
			case SquadMsgType.TroopRequest:
				if (SquadUtils.IsPlayerInSquad(playerId, Service.SquadController.StateManager.GetCurrentSquad()))
				{
					bool isWarRequest = msg.RequestData.IsWarRequest;
					if (!isWarRequest || this.CanShowWarTroopRequest(msg.RequestData))
					{
						if (!this.IsRequestFull(msg))
						{
							string text = playerId;
							if (isWarRequest)
							{
								text += "war";
							}
							if (this.lastRequestMessage.ContainsKey(text))
							{
								SquadMsg squadMsg = this.lastRequestMessage[text];
								ChatItemElements chatItemElements2 = this.msgToElementsMap[squadMsg];
								this.RemoveItemForMsg(chatItemElements2.parent, squadMsg);
								this.lastRequestMessage[text] = msg;
							}
							else
							{
								this.lastRequestMessage.Add(text, msg);
							}
						}
						uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
						this.SetupPlayerName(chatItemElements, playerName);
						this.SetupPlayerRole(chatItemElements, playerId);
						this.SetupChatMessage(chatItemElements, msg.ChatData);
						bool flag = this.SetupTroopRequest(chatItemElements, msg);
						if (flag)
						{
							desiredHeight = 110;
						}
						if (isWarRequest)
						{
							if (this.warTroopRequestItems == null)
							{
								this.warTroopRequestItems = new Dictionary<SquadMsg, UXElement>();
							}
							if (!this.warTroopRequestItems.ContainsKey(msg))
							{
								this.warTroopRequestItems.Add(msg, uXElement);
							}
						}
					}
				}
				break;
			case SquadMsgType.TroopDonation:
				if (msg.DonationData != null)
				{
					chatItemElements = this.UpdateExistingTroopRequestWithDonation(msg);
					if (chatItemElements != null)
					{
						desiredHeight = 110;
						uXElement = chatItemElements.parent;
					}
				}
				break;
			case SquadMsgType.WarMatchMakingBegin:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupMatchmakingStarted(chatItemElements);
				desiredHeight = 36;
				break;
			case SquadMsgType.WarBuffBaseAttackComplete:
				if (msg.WarEventData != null && msg.WarEventData.BuffBaseCaptured)
				{
					uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
					this.SetupWarBuffBaseCaptured(chatItemElements, msg.WarEventData.BuffBaseUid, playerName);
					desiredHeight = 36;
				}
				break;
			case SquadMsgType.WarPlayerAttackComplete:
				if (msg.WarEventData != null)
				{
					uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
					this.SetupWarPlayerAttacked(chatItemElements, playerName, msg.WarEventData.OpponentName, msg.WarEventData.StarsEarned, msg.WarEventData.VictoryPointsTaken);
					desiredHeight = 36;
				}
				break;
			case SquadMsgType.WarEnded:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupWarEnded(chatItemElements);
				desiredHeight = 36;
				break;
			case SquadMsgType.SquadLevelUp:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupSquadLeveledUp(chatItemElements, msg.SquadData.Level);
				desiredHeight = 36;
				break;
			case SquadMsgType.PerkUnlocked:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupPerkUnlocked(chatItemElements, perkVO);
				desiredHeight = 36;
				break;
			case SquadMsgType.PerkUpgraded:
				uXElement = this.CreateNewChatItem(this.nextItemId.ToString(), "ChatItem", msg, ref chatItemElements);
				this.SetupPerkUpgraded(chatItemElements, perkVO);
				desiredHeight = 36;
				break;
			}
			if (uXElement != null && chatItemElements != null)
			{
				this.SetupTimestamp(chatItemElements, msg.TimeSent);
				this.ResizeItem(uXElement, chatItemElements.PlayerMessageLabel, desiredHeight);
				List<UXElement> elementList = this.table.GetElementList();
				if (elementList.Count > Service.SquadController.MessageLimit)
				{
					UXElement uXElement2 = elementList[0];
					this.table.RemoveItem(uXElement2);
					this.parentScreen.DestroyElement(uXElement2);
				}
			}
			return chatItemElements != null;
		}

		public override void RemoveMessage(SquadMsg msg)
		{
			if (this.msgToElementsMap.ContainsKey(msg))
			{
				ChatItemElements chatItemElements = this.msgToElementsMap[msg];
				if (chatItemElements != null && chatItemElements.parent != null)
				{
					this.RemoveItemForMsg(chatItemElements.parent, msg);
					if (this.joinRequestItems != null && this.joinRequestItems.ContainsKey(msg))
					{
						this.joinRequestItems.Remove(msg);
					}
					if (this.warTroopRequestItems != null && this.warTroopRequestItems.ContainsKey(msg))
					{
						this.warTroopRequestItems.Remove(msg);
					}
				}
			}
		}

		private void SetupPlayerName(ChatItemElements elements, string playerName)
		{
			if (playerName.Length > 30)
			{
				playerName = playerName.Remove(30, playerName.Length - 30);
				playerName += Service.Lang.Get("SQUAD_CHAT_PLAYER_NAME_SUFFIX", new object[0]) + "   ";
			}
			else
			{
				playerName += "   ";
			}
			elements.PlayerNameLabel.Visible = true;
			elements.PlayerNameLabel.Text = playerName;
		}

		private void SetupPlayerRole(ChatItemElements elements, string playerId)
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			SquadMember squadMemberById = SquadUtils.GetSquadMemberById(currentSquad, playerId);
			if (squadMemberById != null)
			{
				elements.PlayerRoleLabel.Visible = true;
				elements.PlayerRoleLabel.Text = Service.Lang.Get("SQUAD_ROLE_EMPHASIS", new object[]
				{
					LangUtils.GetSquadRoleDisplayName(squadMemberById.Role)
				});
			}
		}

		private void SetupChatMessage(ChatItemElements elements, SqmChatData chatData)
		{
			if (chatData != null)
			{
				elements.PlayerMessageLabel.Visible = true;
				elements.PlayerMessageLabel.Text = chatData.Message;
			}
			elements.MessageBG.Visible = true;
		}

		private void SetupJoinRequest(ChatItemElements elements, SquadMsg msg)
		{
			elements.PrimaryButton.Visible = true;
			elements.PrimaryButton.OnClicked = new UXButtonClickedDelegate(this.OnJoinAcceptClicked);
			elements.PrimaryButton.Tag = msg;
			elements.PrimaryButtonLabel.Text = Service.Lang.Get("BUTTON_ACCEPT", new object[0]);
			elements.SecondaryButton.Visible = true;
			elements.SecondaryButton.OnClicked = new UXButtonClickedDelegate(this.OnDeclineClicked);
			elements.SecondaryButton.Tag = msg;
			elements.SecondaryButtonLabel.Text = Service.Lang.Get("BUTTON_DECLINE", new object[0]);
			this.UpdateJoinRequestActionPermission(elements, msg);
		}

		private void UpdateJoinRequestActionPermission(ChatItemElements elements, SquadMsg msg)
		{
			string playerId = msg.OwnerData.PlayerId;
			string playerId2 = Service.CurrentPlayer.PlayerId;
			SquadRole role = Service.SquadController.StateManager.Role;
			bool enabled = playerId2 != playerId && role != SquadRole.Member;
			elements.PrimaryButton.Enabled = enabled;
			elements.SecondaryButton.Enabled = enabled;
		}

		private void UpdateExistingJoinRequestActionPermissions()
		{
			if (this.joinRequestItems != null)
			{
				foreach (KeyValuePair<SquadMsg, UXElement> current in this.joinRequestItems)
				{
					SquadMsg key = current.Key;
					ChatItemElements elements = this.msgToElementsMap[key];
					this.UpdateJoinRequestActionPermission(elements, key);
				}
			}
		}

		private void OnJoinAcceptClicked(UXButton button)
		{
			ProcessingScreen.Show();
			SquadMsg squadMsg = (SquadMsg)button.Tag;
			string playerId = squadMsg.OwnerData.PlayerId;
			SquadMsg message = SquadMsgUtils.CreateAcceptJoinRequestMessage(playerId, "chat", new SquadController.ActionCallback(this.OnJoinRequestAccepted), squadMsg);
			Service.SquadController.TakeAction(message);
		}

		private void OnJoinRequestAccepted(bool success, object cookie)
		{
			this.OnJoinRequestHandled(success, (SquadMsg)cookie, true);
		}

		private void OnDeclineClicked(UXButton button)
		{
			ProcessingScreen.Show();
			SquadMsg squadMsg = (SquadMsg)button.Tag;
			string playerId = squadMsg.OwnerData.PlayerId;
			SquadMsg message = SquadMsgUtils.CreateRejectJoinRequestMessage(playerId, new SquadController.ActionCallback(this.OnJoinRequestDeclined), squadMsg);
			Service.SquadController.TakeAction(message);
		}

		private void OnJoinRequestDeclined(bool success, object cookie)
		{
			this.OnJoinRequestHandled(success, (SquadMsg)cookie, false);
		}

		private void OnJoinRequestHandled(bool success, SquadMsg msg, bool accepted)
		{
			ProcessingScreen.Hide();
			if (!success)
			{
				return;
			}
			if (this.msgToElementsMap.ContainsKey(msg))
			{
				ChatItemElements chatItemElements = this.msgToElementsMap[msg];
				chatItemElements.PrimaryButton.Visible = false;
				chatItemElements.SecondaryButton.Visible = false;
				if (accepted)
				{
					chatItemElements.PlayerMessageLabel.Text = Service.Lang.Get("s_sqd_Welcome", new object[]
					{
						msg.OwnerData.PlayerName
					});
					chatItemElements.PlayerMessageLabel.TextColor = Color.green;
				}
				else
				{
					chatItemElements.PlayerMessageLabel.Text = Service.Lang.Get("s_sqd_Rejected", new object[]
					{
						msg.OwnerData.PlayerName
					});
					chatItemElements.PlayerMessageLabel.TextColor = Color.red;
				}
			}
		}

		private void SetupRoleChange(ChatItemElements elements, string memberName, SquadRole memberRole, SquadMsgType type)
		{
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = Service.Lang.Get(type.ToString() + "_SQUAD_NOTIF", new object[]
			{
				memberName,
				LangUtils.GetSquadRoleDisplayName(memberRole)
			});
		}

		private void SetupJoinOrLeave(ChatItemElements elements, string memberName, SquadMsgType type)
		{
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = Service.Lang.Get(type.ToString() + "_SQUAD_NOTIF", new object[]
			{
				memberName
			});
		}

		private void SetupInviteAccepted(ChatItemElements elements, string memberName, string inviterName)
		{
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = Service.Lang.Get("SQUAD_INVITE_ACCEPTED_CHAT", new object[]
			{
				memberName,
				inviterName
			});
		}

		private bool IsRequestFull(SquadMsg msg)
		{
			int num = msg.RequestData.TotalCapacity - msg.RequestData.StartingAvailableCapacity;
			bool result = false;
			if (this.donationMsgsByRequestId != null && this.donationMsgsByRequestId.ContainsKey(msg.NotifId))
			{
				List<SquadMsg> list = this.donationMsgsByRequestId[msg.NotifId];
				StaticDataController staticDataController = Service.StaticDataController;
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					SquadMsg squadMsg = list[i];
					if (squadMsg.DonationData != null && squadMsg.DonationData.Donations != null)
					{
						foreach (KeyValuePair<string, int> current in squadMsg.DonationData.Donations)
						{
							num += staticDataController.Get<TroopTypeVO>(current.Key).Size * current.Value;
						}
					}
					i++;
				}
				result = (num >= msg.RequestData.TotalCapacity);
			}
			return result;
		}

		private bool CanShowWarTroopRequest(SqmRequestData request)
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarData currentSquadWar = warManager.CurrentSquadWar;
			return currentSquadWar != null && currentSquadWar.WarId == request.WarId && warManager.GetCurrentStatus() == SquadWarStatusType.PhasePrep;
		}

		private bool SetupTroopRequest(ChatItemElements elements, SquadMsg msg)
		{
			bool flag = Service.CurrentPlayer.PlayerId == msg.OwnerData.PlayerId;
			bool flag2 = false;
			SqmRequestData requestData = msg.RequestData;
			int donationCount = SquadUtils.GetDonationCount(Service.SquadController.MsgManager.GetExistingMessages(), msg.NotifId, Service.CurrentPlayer.PlayerId);
			bool flag3 = donationCount > 0;
			bool flag4 = donationCount < requestData.TroopDonationLimit;
			int num = requestData.TotalCapacity - requestData.StartingAvailableCapacity;
			bool flag5 = true;
			string text = null;
			if (this.donationMsgsByRequestId != null && this.donationMsgsByRequestId.ContainsKey(msg.NotifId))
			{
				List<string> list = new List<string>();
				List<SquadMsg> list2 = this.donationMsgsByRequestId[msg.NotifId];
				int i = 0;
				int count = list2.Count;
				while (i < count)
				{
					SquadMsg squadMsg = list2[i];
					string item = Service.Lang.Get("SQUAD_CHAT_PLAYER_NAME_COLORIZE", new object[]
					{
						squadMsg.OwnerData.PlayerName
					});
					if (!list.Contains(item))
					{
						list.Add(item);
					}
					if (squadMsg.DonationData != null && squadMsg.DonationData.Donations != null)
					{
						StaticDataController staticDataController = Service.StaticDataController;
						foreach (KeyValuePair<string, int> current in squadMsg.DonationData.Donations)
						{
							num += staticDataController.Get<TroopTypeVO>(current.Key).Size * current.Value;
						}
					}
					i++;
				}
				flag2 = (num >= requestData.TotalCapacity);
				int count2 = list.Count;
				if (flag2 && count2 > 0)
				{
					string text2 = Service.Lang.Get("SQUAD_CHAT_PLAYER_NAME_COLORIZE", new object[]
					{
						msg.OwnerData.PlayerName
					});
					if (count2 == 1)
					{
						text = Service.Lang.Get("X_DONATED_TROOPS_TO_Y", new object[]
						{
							list[0],
							text2
						});
					}
					else if (count2 == 2)
					{
						text = Service.Lang.Get("X_AND_Y_DONATED_TROOPS_TO_Z", new object[]
						{
							list[0],
							list[1],
							text2
						});
					}
					else if (count2 == 3)
					{
						text = Service.Lang.Get("X_Y_AND_Z_DONATED_TROOPS_TO_A", new object[]
						{
							list[0],
							list[1],
							list[2],
							text2
						});
					}
					else if (count2 >= 4)
					{
						text = Service.Lang.Get("X_Y_Z_AND_A_DONATED_TROOPS_TO_B", new object[]
						{
							list[0],
							list[1],
							list[2],
							list[3],
							text2
						});
					}
				}
			}
			requestData.CurrentDonationSize = num;
			requestData.CurrentPlayerDonationCount = donationCount;
			if (flag2)
			{
				flag5 = false;
				elements.PrimaryButton.Visible = false;
				elements.PlayerMessageLabel.Visible = true;
				elements.PlayerMessageLabel.Text = text;
			}
			else if (flag)
			{
				flag5 = true;
				elements.PrimaryButton.Visible = false;
			}
			else if (flag4)
			{
				flag5 = true;
				elements.PrimaryButton.Visible = true;
				elements.PrimaryButton.OnClicked = new UXButtonClickedDelegate(this.OnDonateClicked);
				elements.PrimaryButton.Tag = msg;
				elements.PrimaryButton.Enabled = true;
				elements.PrimaryButtonLabel.Text = Service.Lang.Get("BUTTON_DONATE", new object[0]);
			}
			else if (flag3)
			{
				flag5 = true;
				elements.PrimaryButton.Visible = false;
			}
			elements.DonateProgBar.Visible = flag5;
			bool isWarRequest = requestData.IsWarRequest;
			elements.SpriteWarIcon.Visible = isWarRequest;
			elements.ContainerChatWar.Visible = (isWarRequest && !flag2);
			elements.ContainerChat.Visible = (!isWarRequest && !flag2);
			if (isWarRequest && !flag2)
			{
				elements.WarRequestTexture.LoadTexture("squadwars_chatrequest_row");
			}
			if (flag5)
			{
				elements.PlayerMessageLabel.Visible = true;
				elements.MessageBG.Visible = true;
				int totalCapacity = requestData.TotalCapacity;
				int num2 = num;
				elements.DonateProgBarLabel.Text = Service.Lang.Get("FRACTION", new object[]
				{
					num2,
					totalCapacity
				});
				float value = (totalCapacity <= 0) ? 0f : ((float)num2 / (float)totalCapacity);
				elements.DonateProgBar.Value = value;
			}
			return flag2;
		}

		private void OnDonateClicked(UXButton button)
		{
			Service.EventManager.SendEvent(EventId.SquadNext, null);
			SquadMsg squadMsg = (SquadMsg)button.Tag;
			SqmRequestData requestData = squadMsg.RequestData;
			TroopDonationProgress troopDonationProgress = Service.CurrentPlayer.TroopDonationProgress;
			this.parentScreen.OpenDonationView(squadMsg.OwnerData.PlayerId, squadMsg.OwnerData.PlayerName, requestData.CurrentDonationSize, requestData.TotalCapacity, requestData.CurrentPlayerDonationCount, squadMsg.NotifId, requestData.IsWarRequest, requestData.TroopDonationLimit, troopDonationProgress);
		}

		private ChatItemElements UpdateExistingTroopRequestWithDonation(SquadMsg msg)
		{
			ChatItemElements result = null;
			string requestId = msg.DonationData.RequestId;
			if (string.IsNullOrEmpty(requestId))
			{
				Service.Logger.Warn("Troop donation message does not have request Id");
				return result;
			}
			SquadMsg msgById = Service.SquadController.MsgManager.GetMsgById(requestId);
			if (msgById != null && msgById.RequestData.IsWarRequest && !this.CanShowWarTroopRequest(msgById.RequestData))
			{
				return result;
			}
			if (this.donationMsgsByRequestId == null)
			{
				this.donationMsgsByRequestId = new Dictionary<string, List<SquadMsg>>();
			}
			List<SquadMsg> list;
			if (this.donationMsgsByRequestId.ContainsKey(requestId))
			{
				list = this.donationMsgsByRequestId[requestId];
			}
			else
			{
				list = new List<SquadMsg>();
				this.donationMsgsByRequestId.Add(requestId, list);
			}
			list.Add(msg);
			if (msgById != null && this.msgToElementsMap.ContainsKey(msgById))
			{
				bool flag = this.SetupTroopRequest(this.msgToElementsMap[msgById], msgById);
				if (flag)
				{
					result = this.msgToElementsMap[msgById];
				}
			}
			return result;
		}

		private void SetupBattleReplay(ChatItemElements elements, SquadMsg msg)
		{
			SqmReplayData replayData = msg.ReplayData;
			Lang lang = Service.Lang;
			elements.ReplayParent.Visible = true;
			elements.PrimaryButton.Visible = true;
			elements.PrimaryButton.OnClicked = new UXButtonClickedDelegate(this.OnReplayClicked);
			elements.PrimaryButton.Tag = msg;
			elements.PrimaryButtonLabel.Text = lang.Get("BUTTON_REPLAY", new object[0]);
			elements.ReplayStar1.Color = Color.black;
			elements.ReplayStar2.Color = Color.black;
			elements.ReplayStar3.Color = Color.black;
			int stars = replayData.Stars;
			if (stars > 0)
			{
				elements.ReplayStar1.Color = Color.white;
				if (stars > 1)
				{
					elements.ReplayStar2.Color = Color.white;
					if (stars > 2)
					{
						elements.ReplayStar3.Color = Color.white;
					}
				}
			}
			elements.ReplayOpponentNameLabel.Text = replayData.OpponentName;
			elements.ReplayDamageLabel.Text = lang.Get("PERCENTAGE", new object[]
			{
				replayData.DamagePercent
			});
			elements.ReplayTypeLabel.Text = ((replayData.BattleType != SquadBattleReplayType.Attack) ? lang.Get("SQUAD_DEFENSE", new object[0]) : lang.Get("SQUAD_OFFENSE", new object[0]));
			elements.ReplayMedals.Text = lang.ThousandsSeparated(replayData.MedalDelta);
		}

		private void OnReplayClicked(UXButton button)
		{
			SquadMsg squadMsg = (SquadMsg)button.Tag;
			SqmOwnerData ownerData = squadMsg.OwnerData;
			SqmReplayData replayData = squadMsg.ReplayData;
			if (Service.GameStateMachine.CurrentState is GalaxyState)
			{
				Service.GalaxyViewController.GoToHome();
			}
			BattleParticipant defender;
			if (replayData.BattleType == SquadBattleReplayType.Attack)
			{
				defender = new BattleParticipant(replayData.OpponentId, replayData.OpponentName, replayData.OpponentFaction);
			}
			else
			{
				defender = new BattleParticipant(ownerData.PlayerId, ownerData.PlayerName, replayData.SharerFaction);
			}
			Service.PvpManager.ReplayBattle(replayData.BattleId, defender, ownerData.PlayerId);
			this.parentScreen.AnimateClosed(false, null);
		}

		private void SetupMatchmakingStarted(ChatItemElements elements)
		{
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = Service.Lang.Get("WAR_MATCHMAKING_STARTED", new object[0]);
		}

		private void SetupWarEnded(ChatItemElements elements)
		{
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = Service.Lang.Get("WAR_END_TRANSMISSION_TITLE", new object[0]);
		}

		private void SetupSquadLeveledUp(ChatItemElements elements, int level)
		{
			Lang lang = Service.Lang;
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = lang.Get("PERK_SQUAD_CHAT_LEVEL_UP", new object[]
			{
				level.ToString()
			});
		}

		private void SetupPerkUnlocked(ChatItemElements elements, PerkVO perkUnlocked)
		{
			Lang lang = Service.Lang;
			string perkNameForGroup = Service.PerkViewController.GetPerkNameForGroup(perkUnlocked.PerkGroup);
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = lang.Get("PERK_SQUAD_CHAT_PERK_UNLOCK", new object[]
			{
				perkNameForGroup
			});
		}

		private void SetupPerkUpgraded(ChatItemElements elements, PerkVO perkUpgraded)
		{
			Lang lang = Service.Lang;
			string perkNameForGroup = Service.PerkViewController.GetPerkNameForGroup(perkUpgraded.PerkGroup);
			int perkTier = perkUpgraded.PerkTier;
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = lang.Get("PERK_SQUAD_CHAT_PERK_UPGRADE", new object[]
			{
				perkNameForGroup,
				perkTier.ToString()
			});
		}

		private void SetupWarBuffBaseCaptured(ChatItemElements elements, string buffBaseUid, string playerName)
		{
			Lang lang = Service.Lang;
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = lang.Get("WAR_BUFF_BASE_CAPTURED", new object[]
			{
				Service.SquadController.WarManager.GetWarBuffDisplayName(buffBaseUid),
				playerName
			});
		}

		private void SetupWarPlayerAttacked(ChatItemElements elements, string playerName, string opponentName, int stars, int victoryPoints)
		{
			elements.LabelSquadUpdate.Visible = true;
			elements.LabelSquadUpdate.Text = Service.Lang.Get("WAR_PLAYER_ATTACKED", new object[]
			{
				opponentName,
				playerName,
				stars,
				victoryPoints
			});
		}

		private void SetupTimestamp(ChatItemElements elements, uint timestamp)
		{
			elements.TimestampLabel.Visible = true;
			this.UpdateTimestampLabel(elements, timestamp, Service.Lang);
		}

		public void UpdateAllTimestamps()
		{
			Lang lang = Service.Lang;
			foreach (KeyValuePair<SquadMsg, ChatItemElements> current in this.msgToElementsMap)
			{
				SquadMsg key = current.Key;
				if (key.ChatData != null)
				{
					this.UpdateTimestampLabel(current.Value, key.TimeSent, lang);
				}
			}
		}

		private void UpdateTimestampLabel(ChatItemElements elements, uint timestamp, Lang lang)
		{
			elements.TimestampLabel.Text = ChatTimeConversionUtils.GetFormattedAge(timestamp, lang);
		}

		private UXElement CreateNewChatItem(string id, string templateName, SquadMsg msg, ref ChatItemElements elements)
		{
			this.table.SetTemplateItem(templateName);
			UXElement uXElement = this.table.CloneTemplateItem(id);
			this.table.AddItem(uXElement, this.nextItemId);
			elements = new ChatItemElements();
			this.msgToElementsMap.Add(msg, elements);
			uXElement.Tag = elements;
			if (templateName == "ChatItem")
			{
				this.SetupChatItemElements(elements, id, uXElement);
			}
			ChatFilterType squadScreenChatFilterType = Service.SquadController.StateManager.GetSquadScreenChatFilterType();
			this.ProcessMessageByFilter(squadScreenChatFilterType, msg, elements);
			return uXElement;
		}

		private void SetupChatItemElements(ChatItemElements elements, string id, UXElement parent)
		{
			elements.parent = parent;
			elements.PrimaryButton = this.table.GetSubElement<UXButton>(id, "BtnPrimary");
			elements.PrimaryButton.Visible = false;
			elements.PrimaryButtonLabel = this.table.GetSubElement<UXLabel>(id, "LabelBtnPrimary");
			elements.SecondaryButton = this.table.GetSubElement<UXButton>(id, "BtnSecondary");
			elements.SecondaryButton.Visible = false;
			elements.SecondaryButtonLabel = this.table.GetSubElement<UXLabel>(id, "LabelBtnSecondary");
			elements.DonateProgBar = this.table.GetSubElement<UXSlider>(id, "PbarDonate");
			elements.DonateProgBar.Visible = false;
			elements.DonateProgBarLabel = this.table.GetSubElement<UXLabel>(id, "LabelPbarDonate");
			elements.DonateRewardLabel = this.table.GetSubElement<UXLabel>(id, "LabelDonateReward");
			elements.DonateRewardLabel.Visible = false;
			elements.MessageBG = this.table.GetSubElement<UXSprite>(id, "SpritePlayerMessage");
			elements.MessageBG.Visible = false;
			elements.ReplayParent = this.table.GetSubElement<UXElement>(id, "Replay");
			elements.ReplayParent.Visible = false;
			elements.ReplayTypeLabel = this.table.GetSubElement<UXLabel>(id, "LabelReplayType");
			elements.ReplayDamageLabel = this.table.GetSubElement<UXLabel>(id, "LabelDamage");
			elements.ReplayOpponentNameLabel = this.table.GetSubElement<UXLabel>(id, "LabelOpponentName");
			elements.ReplayMedals = this.table.GetSubElement<UXLabel>(id, "LabelReplayMedals");
			elements.ReplayStar1 = this.table.GetSubElement<UXSprite>(id, "SpriteStar1");
			elements.ReplayStar2 = this.table.GetSubElement<UXSprite>(id, "SpriteStar2");
			elements.ReplayStar3 = this.table.GetSubElement<UXSprite>(id, "SpriteStar3");
			elements.PlayerMessageLabel = this.table.GetSubElement<UXLabel>(id, "LabelPlayerMessage");
			elements.PlayerMessageLabel.Visible = false;
			elements.PlayerNameLabel = this.table.GetSubElement<UXLabel>(id, "LabelPlayerName");
			elements.PlayerNameLabel.Visible = false;
			elements.PlayerRoleLabel = this.table.GetSubElement<UXLabel>(id, "LabelPlayerRole");
			elements.PlayerRoleLabel.Visible = false;
			elements.LabelSquadUpdate = this.table.GetSubElement<UXLabel>(id, "LabelSquadUpdate");
			elements.LabelSquadUpdate.Visible = false;
			elements.SpriteMessageArrow = this.table.GetSubElement<UXSprite>(id, "SpritePlayerMessageArrow");
			elements.SpriteMessageArrow.Visible = false;
			elements.TimestampLabel = this.table.GetSubElement<UXLabel>(id, "LabelTimeStamp");
			elements.TimestampLabel.Visible = false;
			elements.ContainerChat = this.table.GetSubElement<UXElement>(id, "ContainerChat");
			elements.ContainerChat.Visible = true;
			elements.ContainerChatWar = this.table.GetSubElement<UXElement>(id, "ContainerChatWar");
			elements.ContainerChatWar.Visible = false;
			elements.SpriteWarIcon = this.table.GetSubElement<UXSprite>(id, "SpriteWarIcon");
			elements.SpriteWarIcon.Visible = false;
			elements.WarRequestTexture = this.table.GetSubElement<UXTexture>(id, "TextureWarRequest");
		}

		private void ResizeItem(UXElement item, UXLabel messageLabel, int desiredHeight)
		{
			float num = messageLabel.Height - messageLabel.LineHeight;
			item.Height = (float)desiredHeight * this.parentScreen.UXCamera.Scale + num;
		}

		private void RemoveItemForMsg(UXElement item, SquadMsg msg)
		{
			this.table.RemoveItem(item);
			this.parentScreen.DestroyElement(item);
			this.table.RepositionItems();
			this.msgToElementsMap.Remove(msg);
		}

		public void Cleanup()
		{
			this.msgToElementsMap.Clear();
			this.chatFilterMappings.Clear();
			if (this.joinRequestItems != null)
			{
				this.joinRequestItems.Clear();
			}
			if (this.warTroopRequestItems != null)
			{
				this.warTroopRequestItems.Clear();
			}
			Service.EventManager.UnregisterObserver(this, EventId.SquadChatFilterUpdated);
			Service.EventManager.UnregisterObserver(this, EventId.WarPhaseChanged);
		}

		public int RemoveElementsByCount(int numToRemove)
		{
			List<UXElement> elementList = this.table.GetElementList();
			int count = elementList.Count;
			int num = (count <= numToRemove) ? 0 : (count - numToRemove);
			for (int i = count - 1; i >= num; i--)
			{
				UXElement uXElement = elementList[i];
				this.table.RemoveItem(uXElement);
				this.parentScreen.DestroyElement(uXElement);
			}
			return num;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.SquadChatFilterUpdated)
			{
				if (id == EventId.WarPhaseChanged)
				{
					if ((SquadWarStatusType)cookie == SquadWarStatusType.PhaseAction && this.warTroopRequestItems != null)
					{
						foreach (KeyValuePair<SquadMsg, UXElement> current in this.warTroopRequestItems)
						{
							this.RemoveItemForMsg(current.Value, current.Key);
						}
					}
				}
			}
			else
			{
				ChatFilterType type = (ChatFilterType)cookie;
				foreach (KeyValuePair<SquadMsg, ChatItemElements> current2 in this.msgToElementsMap)
				{
					SquadMsg key = current2.Key;
					ChatItemElements value = current2.Value;
					this.ProcessMessageByFilter(type, key, value);
				}
				this.table.RepositionItems();
			}
			return EatResponse.NotEaten;
		}

		private void ProcessMessageByFilter(ChatFilterType type, SquadMsg msg, ChatItemElements elements)
		{
			if (type == ChatFilterType.ShowAll || this.chatFilterMappings[type].Contains(msg.Type))
			{
				elements.parent.Visible = true;
			}
			else
			{
				elements.parent.Visible = false;
			}
		}
	}
}
