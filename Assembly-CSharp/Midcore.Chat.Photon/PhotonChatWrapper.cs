using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using StaRTS.Main.Models.Chat;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace Midcore.Chat.Photon
{
	public class PhotonChatWrapper : IChatClientListener, IViewClockTimeObserver
	{
		public delegate void OnGetMessagesCallback(string channelName, string[] senders, object[] messages);

		public delegate void OnDisconnectedCallback();

		public delegate void OnConnectedCallback();

		private const string CHAT_REGION = "USW";

		private const int DEFAULT_HISTORY_LENGTH = 50;

		private const float DEFAULT_KEEP_ALIVE_TICK = 0.1f;

		private const string LOGGER_PREFIX = "Photon Chat: ";

		private List<string> channelsToJoinOnConnect;

		private ChatClient chatClient;

		private string appID;

		private string appVersion;

		private int historyLengthToFetch = 50;

		private string userID;

		private PhotonChatWrapper.OnGetMessagesCallback getMessagesCallback;

		private PhotonChatWrapper.OnConnectedCallback connectedCallback;

		private PhotonChatWrapper.OnDisconnectedCallback disconnectedCallback;

		private float keepAliveTick = 0.1f;

		public PhotonChatSessionState SessionState
		{
			get;
			private set;
		}

		public string SelectedChannelName
		{
			get;
			private set;
		}

		public PhotonChatWrapper(PhotonChatWrapper.OnGetMessagesCallback getMessagesCallback, PhotonChatWrapper.OnConnectedCallback connectedCallback, PhotonChatWrapper.OnDisconnectedCallback disconnectedCallback)
		{
			this.getMessagesCallback = getMessagesCallback;
			this.connectedCallback = connectedCallback;
			this.disconnectedCallback = disconnectedCallback;
			this.SessionState = PhotonChatSessionState.Disconnected;
		}

		public void InitAndConnect(string appId, string appVersion, int historyLengthToFetch, float keepAliveTick, string userID)
		{
			this.appID = appId;
			this.appVersion = appVersion;
			this.userID = userID;
			if (keepAliveTick > 0f)
			{
				this.keepAliveTick = keepAliveTick;
			}
			if (historyLengthToFetch > 0)
			{
				this.historyLengthToFetch = historyLengthToFetch;
			}
			this.SelectedChannelName = null;
			this.channelsToJoinOnConnect = new List<string>();
			this.Connect();
		}

		public void Connect()
		{
			this.chatClient = new ChatClient(this, ConnectionProtocol.Udp);
			this.chatClient.ChatRegion = "USW";
			this.chatClient.Connect(this.appID, this.appVersion, new AuthenticationValues(this.userID));
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, this.keepAliveTick);
			this.SessionState = PhotonChatSessionState.Connecting;
		}

		public void Reconnect()
		{
			if (this.chatClient == null)
			{
				this.LogWarning("Photon ChatClient is NULL! Reconnecting failed.");
				return;
			}
			if ((this.SessionState == PhotonChatSessionState.Disconnected || this.chatClient.State == ChatState.Disconnected) && this.userID != null)
			{
				this.Connect();
			}
		}

		public void Reset()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Disconnect();
			}
			this.SessionState = PhotonChatSessionState.Disconnected;
			this.SelectedChannelName = null;
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			if (this.channelsToJoinOnConnect != null)
			{
				this.channelsToJoinOnConnect.Clear();
				this.channelsToJoinOnConnect = null;
			}
		}

		public void OnViewClockTime(float dt)
		{
			if (this.chatClient != null)
			{
				this.chatClient.Service();
			}
		}

		public void StartSession(ChatType chatType, string channelId)
		{
			if (this.channelsToJoinOnConnect == null)
			{
				this.LogError("Trying to subscribe to channels when channelsToJoinOnConnect is null. Chat is disabled");
				return;
			}
			PhotonChatSessionState sessionState = this.SessionState;
			if (sessionState != PhotonChatSessionState.Connected)
			{
				if (sessionState != PhotonChatSessionState.Connecting)
				{
					if (sessionState == PhotonChatSessionState.Disconnected)
					{
						this.LogError("Trying to subscribe to a channel after disconnecting from chat");
					}
				}
				else if (!this.channelsToJoinOnConnect.Contains(channelId))
				{
					this.channelsToJoinOnConnect.Add(channelId);
				}
			}
			else
			{
				this.channelsToJoinOnConnect.Add(channelId);
				this.SubscribeToChannelId(channelId);
			}
		}

		private void SubscribeToChannelId(string channelId)
		{
			string[] channels = new string[]
			{
				channelId
			};
			if (this.SelectedChannelName == null)
			{
				this.SelectedChannelName = channelId;
			}
			this.chatClient.Subscribe(channels, this.historyLengthToFetch);
		}

		public void PublishMessage(string messageJson)
		{
			if (this.SelectedChannelName == null)
			{
				this.LogWarning("Failed to publish message, not subscribed to a channel");
				return;
			}
			this.chatClient.PublishMessage(this.SelectedChannelName, messageJson);
		}

		private void LogError(string message)
		{
			Service.Logger.Error("Photon Chat: " + message);
		}

		private void LogWarning(string message)
		{
			Service.Logger.Warn("Photon Chat: " + message);
		}

		private void LogDebug(string message)
		{
			Service.Logger.Debug("Photon Chat: " + message);
		}

		public void DebugReturn(DebugLevel level, string message)
		{
			if (level == DebugLevel.ERROR)
			{
				this.LogError(message);
			}
			else if (level == DebugLevel.WARNING)
			{
				this.LogWarning(message);
			}
			else
			{
				this.LogDebug(message);
			}
		}

		public void OnDisconnected()
		{
			this.SessionState = PhotonChatSessionState.Disconnected;
			if (this.disconnectedCallback != null)
			{
				this.disconnectedCallback();
			}
		}

		public void OnConnected()
		{
			this.SessionState = PhotonChatSessionState.Connected;
			if (this.channelsToJoinOnConnect == null || this.channelsToJoinOnConnect.Count == 0)
			{
				return;
			}
			int i = 0;
			int count = this.channelsToJoinOnConnect.Count;
			while (i < count)
			{
				this.SubscribeToChannelId(this.channelsToJoinOnConnect[i]);
				i++;
			}
			if (this.connectedCallback != null)
			{
				this.connectedCallback();
			}
		}

		public void OnChatStateChange(ChatState state)
		{
			this.LogDebug("OnChatStateChange: " + state.ToString());
		}

		public void OnGetMessages(string channelName, string[] senders, object[] messages)
		{
			if (senders.Length != messages.Length)
			{
				this.LogError(string.Format("OnGetMessages: number of senders:{0} not equal to number of messages:{1}", senders.Length, messages.Length));
				return;
			}
			if (this.getMessagesCallback != null)
			{
				this.getMessagesCallback(channelName, senders, messages);
			}
		}

		public void OnSubscribed(string[] channels, bool[] results)
		{
			if (channels == null || results == null || channels.Length != results.Length)
			{
				this.LogError("Chat Client sent empty or invalid channel subscriptions");
				return;
			}
			int i = 0;
			int num = channels.Length;
			while (i < num)
			{
				this.LogDebug("OnSubscribed: " + channels[i] + " Status:" + ((!results[i]) ? "false" : "true"));
				if (this.SelectedChannelName != null && channels[i] == this.SelectedChannelName && !results[i])
				{
					this.SelectedChannelName = null;
					break;
				}
				i++;
			}
		}

		public void OnUnsubscribed(string[] channels)
		{
			if (channels == null)
			{
				this.LogError("Chat Client failed to send valid channel list on unsubscribe");
				return;
			}
			int i = 0;
			int num = channels.Length;
			while (i < num)
			{
				this.LogDebug("OnUnsubscribed: " + channels[i]);
				i++;
			}
		}

		public void OnPrivateMessage(string sender, object message, string channelName)
		{
		}

		public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
		{
		}
	}
}
