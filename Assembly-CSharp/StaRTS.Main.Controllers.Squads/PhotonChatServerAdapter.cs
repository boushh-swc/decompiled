using Midcore.Chat.Filter;
using Midcore.Chat.Photon;
using Midcore.Chat.Photon.Encryption;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Chat;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using StaRTS.Utils.Json;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Squads
{
	public class PhotonChatServerAdapter : AbstractSquadServerAdapter
	{
		private const string CENSORED_WORDS_FILE = "censored-words/bad-words-{0}";

		public const string CENSORED_WORD_REPLACEMENT = "**** ";

		private PhotonChatWrapper photonChatWrapper;

		private uint publishTimerId;

		private const float PUBLISH_TIMER_DELAY_DEFAULT = 2f;

		private float publishTimerDelay;

		private Queue<SquadMsg> queuedMessagesToPublish;

		private SquadMsg latestMsg;

		public string encryptedPlayerId;

		private string messageEncryptionKey = string.Empty;

		private SwearFilter filter;

		public PhotonChatServerAdapter()
		{
			this.photonChatWrapper = new PhotonChatWrapper(new PhotonChatWrapper.OnGetMessagesCallback(this.GetMessagesCallback), new PhotonChatWrapper.OnConnectedCallback(this.OnConnectedCallback), new PhotonChatWrapper.OnDisconnectedCallback(this.OnDisconnectedCallback));
		}

		public void Connect(string photonAppId, string photonAppVersion, int historyLengthToFetch, float keepAliveTick, string playerID)
		{
			ChatCryptographyUtils.StaticInit();
			this.queuedMessagesToPublish = new Queue<SquadMsg>();
			this.encryptedPlayerId = ChatCryptographyUtils.GetSHA256Hash(playerID);
			this.photonChatWrapper.InitAndConnect(photonAppId, photonAppVersion, historyLengthToFetch, keepAliveTick, this.encryptedPlayerId);
			this.InitChatFilter();
		}

		private void InitChatFilter()
		{
			string langId = Service.Lang.ExtractLanguageFromLocale();
			string[] wordsFromFileForLocale = LangUtils.GetWordsFromFileForLocale("censored-words/bad-words-{0}", langId);
			if (wordsFromFileForLocale != null)
			{
				Logger logger = Service.Logger;
				this.filter = new SwearFilter(wordsFromFileForLocale, "**** ", logger);
			}
		}

		public override void Disable()
		{
			if (this.photonChatWrapper.SessionState != PhotonChatSessionState.Disconnected)
			{
				this.photonChatWrapper.Reset();
			}
			if (this.filter != null)
			{
				this.filter.ResetTrie();
			}
			this.latestMsg = null;
			base.Disable();
		}

		public override void Enable(SquadController.SquadMsgsCallback callback)
		{
			this.publishTimerDelay = ((GameConstants.PUBLISH_TIMER_DELAY != 0) ? ((float)GameConstants.PUBLISH_TIMER_DELAY) : 2f);
			base.Enable(callback);
		}

		public void StartSession(ChatType chatType, string channelId)
		{
			string sHA256Hash = ChatCryptographyUtils.GetSHA256Hash(channelId);
			this.photonChatWrapper.StartSession(chatType, sHA256Hash);
		}

		public PhotonChatSessionState GetChatSessionState()
		{
			return this.photonChatWrapper.SessionState;
		}

		public void SetMessageEncryptionKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Service.Logger.Error("Failed to update message encryption key, it's null or blank!");
				return;
			}
			this.messageEncryptionKey = key;
		}

		public bool CanPublishMessage()
		{
			return this.photonChatWrapper.SelectedChannelName != null;
		}

		public void PublishMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			string message2 = message;
			if (this.filter != null)
			{
				message2 = this.filter.Filter(message);
			}
			SquadMsg item = SquadMsgUtils.GenerateMessageFromChatMessage(message2);
			this.list.Clear();
			this.list.Add(item);
			this.callback(this.list);
			this.queuedMessagesToPublish.Enqueue(item);
			if (this.photonChatWrapper.SessionState == PhotonChatSessionState.Disconnected)
			{
				this.ReconnectSession();
			}
			else if (this.photonChatWrapper.SessionState == PhotonChatSessionState.Connected)
			{
				this.StartPublishTimer();
			}
		}

		private void StartPublishTimer()
		{
			if (this.publishTimerId == 0u)
			{
				this.publishTimerId = Service.ViewTimerManager.CreateViewTimer(this.publishTimerDelay, true, new TimerDelegate(this.OnPublishTimer), null);
			}
		}

		private void OnPublishTimer(uint id, object cookie)
		{
			if (this.queuedMessagesToPublish.Count == 0)
			{
				Service.ViewTimerManager.KillViewTimer(this.publishTimerId);
				this.publishTimerId = 0u;
				return;
			}
			if (this.photonChatWrapper.SessionState != PhotonChatSessionState.Connected)
			{
				Service.Logger.Error("Chat is not connected, messages cannot be published.");
				return;
			}
			SquadMsg squadMsg = this.queuedMessagesToPublish.Dequeue();
			if (string.IsNullOrEmpty(this.messageEncryptionKey))
			{
				Service.Logger.Error("Failed to send message, message encryption key is null or blank!");
				return;
			}
			string serializedMessage = new PhotonChatMessageTO
			{
				UserName = squadMsg.OwnerData.PlayerName,
				Text = squadMsg.ChatData.Message,
				TimeStamp = squadMsg.TimeSent.ToString()
			}.GetSerializedMessage();
			string encryptedMessageWithIV = ChatCryptographyUtils.GetEncryptedMessageWithIV(serializedMessage, this.messageEncryptionKey);
			if (string.IsNullOrEmpty(encryptedMessageWithIV))
			{
				Service.Logger.Warn("Failed to send message, encryptedMessageWithIV is blank!");
				return;
			}
			this.photonChatWrapper.PublishMessage(encryptedMessageWithIV);
			Service.EventManager.SendEvent(EventId.SquadChatSent, null);
		}

		public void OnConnectedCallback()
		{
			if (this.queuedMessagesToPublish.Count > 0)
			{
				this.StartPublishTimer();
			}
		}

		public void OnDisconnectedCallback()
		{
		}

		public void ReconnectSession()
		{
			if (GameConstants.PHOTON_CHAT_DISABLED)
			{
				return;
			}
			this.photonChatWrapper.Reconnect();
		}

		protected override void Poll()
		{
		}

		private void GetMessagesCallback(string channelName, string[] senders, object[] messages)
		{
			this.list.Clear();
			this.PopulateSquadMsgsReceivedFromPhoton(channelName, senders, messages);
			if (this.callback != null)
			{
				this.callback(this.list);
			}
		}

		protected override void PopulateSquadMsgsReceived(object response)
		{
			Service.Logger.Error("PopulateSquadMsgsReceived called, use PopulateSquadMsgsReceivedFromPhoton!");
		}

		private void PopulateSquadMsgsReceivedFromPhoton(string channelName, string[] senders, object[] messages)
		{
			if (senders == null && messages == null && senders.Length != messages.Length)
			{
				Service.Logger.Error("Cannot populate squad messages, senders and messages count mismatch!");
				return;
			}
			if (messages.Length == 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.messageEncryptionKey))
			{
				Service.Logger.Error("Failed to update received messages, message encryption key is null or blank!");
				return;
			}
			int i = 0;
			int num = messages.Length;
			while (i < num)
			{
				string messageAndIV = messages[i] as string;
				string text = ChatCryptographyUtils.DecryptMessageWithIV(messageAndIV, this.messageEncryptionKey);
				object obj = null;
				if (!string.IsNullOrEmpty(text) && text.StartsWith("{"))
				{
					obj = new JsonParser(text).Parse();
				}
				if (obj == null)
				{
					Service.Logger.WarnFormat("Decrypted chat message is invalid:{0}", new object[]
					{
						(text != null) ? text : string.Empty
					});
				}
				else
				{
					PhotonChatMessageTO photonChatMessageTO = new PhotonChatMessageTO(obj);
					if (this.filter != null)
					{
						photonChatMessageTO.Text = this.filter.Filter(photonChatMessageTO.Text);
					}
					SquadMsg squadMsg = SquadMsgUtils.GenerateMessageFromPhotonChatMessage(senders[i], photonChatMessageTO);
					if (squadMsg != null)
					{
						if (this.IsMsgValid(squadMsg))
						{
							this.list.Add(squadMsg);
							if (this.latestMsg == null || squadMsg.TimeSent > this.latestMsg.TimeSent)
							{
								this.latestMsg = squadMsg;
							}
						}
					}
				}
				i++;
			}
		}

		private bool IsMsgValid(SquadMsg msg)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (msg.OwnerData != null && this.encryptedPlayerId == msg.OwnerData.PlayerId && currentPlayer.LoginTime < msg.TimeSent)
			{
				return false;
			}
			if (GameConstants.PHOTON_CHAT_COMPLEX_COMPARE_ENABLED)
			{
				if (this.latestMsg != null)
				{
					bool flag = msg.TimeSent < this.latestMsg.TimeSent;
					bool flag2 = msg.TimeSent == this.latestMsg.TimeSent;
					bool flag3 = msg.ChatData != null && this.latestMsg.ChatData != null && msg.ChatData.Message == this.latestMsg.ChatData.Message;
					bool flag4 = msg.OwnerData != null && this.latestMsg.OwnerData != null && msg.OwnerData.PlayerId == this.latestMsg.OwnerData.PlayerId;
					if (flag || (flag2 && flag3 && flag4))
					{
						return false;
					}
				}
			}
			else if (this.latestMsg != null && msg.TimeSent <= this.latestMsg.TimeSent)
			{
				return false;
			}
			return true;
		}
	}
}
