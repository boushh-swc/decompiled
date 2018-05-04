using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatClient : IPhotonPeerListener
	{
		private const int FriendRequestListMax = 1024;

		private string chatRegion = "EU";

		public int MessageLimit;

		public readonly Dictionary<string, ChatChannel> PublicChannels;

		public readonly Dictionary<string, ChatChannel> PrivateChannels;

		private readonly HashSet<string> PublicChannelsUnsubscribing;

		private readonly IChatClientListener listener;

		public ChatPeer chatPeer;

		private bool didAuthenticate;

		private int msDeltaForServiceCalls = 50;

		private int msTimestampOfLastServiceCall;

		private const string ChatAppName = "chat";

		public string NameServerAddress
		{
			get;
			private set;
		}

		public string FrontendAddress
		{
			get;
			private set;
		}

		public string ChatRegion
		{
			get
			{
				return this.chatRegion;
			}
			set
			{
				this.chatRegion = value;
			}
		}

		public ChatState State
		{
			get;
			private set;
		}

		public ChatDisconnectCause DisconnectedCause
		{
			get;
			private set;
		}

		public bool CanChat
		{
			get
			{
				return this.State == ChatState.ConnectedToFrontEnd && this.HasPeer;
			}
		}

		private bool HasPeer
		{
			get
			{
				return this.chatPeer != null;
			}
		}

		public string AppVersion
		{
			get;
			private set;
		}

		public string AppId
		{
			get;
			private set;
		}

		public AuthenticationValues AuthValues
		{
			get;
			set;
		}

		public string UserId
		{
			get
			{
				return (this.AuthValues == null) ? null : this.AuthValues.UserId;
			}
			private set
			{
				if (this.AuthValues == null)
				{
					this.AuthValues = new AuthenticationValues();
				}
				this.AuthValues.UserId = value;
			}
		}

		public DebugLevel DebugOut
		{
			get
			{
				return this.chatPeer.DebugOut;
			}
			set
			{
				this.chatPeer.DebugOut = value;
			}
		}

		public ChatClient(IChatClientListener listener, ConnectionProtocol protocol = ConnectionProtocol.Udp)
		{
			this.listener = listener;
			this.State = ChatState.Uninitialized;
			this.chatPeer = new ChatPeer(this, protocol);
			this.PublicChannels = new Dictionary<string, ChatChannel>();
			this.PrivateChannels = new Dictionary<string, ChatChannel>();
			this.PublicChannelsUnsubscribing = new HashSet<string>();
		}

		public bool CanChatInChannel(string channelName)
		{
			return this.CanChat && this.PublicChannels.ContainsKey(channelName) && !this.PublicChannelsUnsubscribing.Contains(channelName);
		}

		public bool Connect(string appId, string appVersion, AuthenticationValues authValues)
		{
			this.chatPeer.TimePingInterval = 3000;
			this.DisconnectedCause = ChatDisconnectCause.None;
			if (authValues == null)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "Connect failed: no authentication values specified");
				}
				return false;
			}
			this.AuthValues = authValues;
			if (this.AuthValues.UserId == null || this.AuthValues.UserId == string.Empty)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "Connect failed: no UserId specified in authentication values");
				}
				return false;
			}
			this.AppId = appId;
			this.AppVersion = appVersion;
			this.didAuthenticate = false;
			this.msDeltaForServiceCalls = 100;
			this.chatPeer.QuickResendAttempts = 2;
			this.chatPeer.SentCountAllowance = 7;
			this.PublicChannels.Clear();
			this.PrivateChannels.Clear();
			this.PublicChannelsUnsubscribing.Clear();
			this.NameServerAddress = this.chatPeer.NameServerAddress;
			bool flag = this.chatPeer.Connect();
			if (flag)
			{
				this.State = ChatState.ConnectingToNameServer;
			}
			return flag;
		}

		public void Service()
		{
			if (this.HasPeer && (Environment.TickCount - this.msTimestampOfLastServiceCall > this.msDeltaForServiceCalls || this.msTimestampOfLastServiceCall == 0))
			{
				this.msTimestampOfLastServiceCall = Environment.TickCount;
				this.chatPeer.Service();
			}
		}

		public void Disconnect()
		{
			if (this.HasPeer && this.chatPeer.PeerState != PeerStateValue.Disconnected)
			{
				this.chatPeer.Disconnect();
			}
		}

		public void StopThread()
		{
			if (this.HasPeer)
			{
				this.chatPeer.StopThread();
			}
		}

		public bool Subscribe(string[] channels)
		{
			return this.Subscribe(channels, 0);
		}

		public bool Subscribe(string[] channels, int messagesFromHistory)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "Subscribe called while not connected to front end server.");
				}
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, "Subscribe can't be called for empty or null channels-list.");
				}
				return false;
			}
			return this.SendChannelOperation(channels, 0, messagesFromHistory);
		}

		public bool Unsubscribe(string[] channels)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "Unsubscribe called while not connected to front end server.");
				}
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, "Unsubscribe can't be called for empty or null channels-list.");
				}
				return false;
			}
			for (int i = 0; i < channels.Length; i++)
			{
				string item = channels[i];
				this.PublicChannelsUnsubscribing.Add(item);
			}
			return this.SendChannelOperation(channels, 1, 0);
		}

		public bool PublishMessage(string channelName, object message)
		{
			return this.publishMessage(channelName, message, true);
		}

		internal bool PublishMessageUnreliable(string channelName, object message)
		{
			return this.publishMessage(channelName, message, false);
		}

		private bool publishMessage(string channelName, object message, bool reliable)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "PublishMessage called while not connected to front end server.");
				}
				return false;
			}
			if (string.IsNullOrEmpty(channelName) || message == null)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, "PublishMessage parameters must be non-null and not empty.");
				}
				return false;
			}
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
			{
				{
					1,
					channelName
				},
				{
					3,
					message
				}
			};
			return this.chatPeer.OpCustom(2, customOpParameters, reliable);
		}

		public bool SendPrivateMessage(string target, object message)
		{
			return this.SendPrivateMessage(target, message, false);
		}

		public bool SendPrivateMessage(string target, object message, bool encrypt)
		{
			return this.sendPrivateMessage(target, message, encrypt, true);
		}

		internal bool SendPrivateMessageUnreliable(string target, object message, bool encrypt)
		{
			return this.sendPrivateMessage(target, message, encrypt, false);
		}

		private bool sendPrivateMessage(string target, object message, bool encrypt, bool reliable)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "SendPrivateMessage called while not connected to front end server.");
				}
				return false;
			}
			if (string.IsNullOrEmpty(target) || message == null)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, "SendPrivateMessage parameters must be non-null and not empty.");
				}
				return false;
			}
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
			{
				{
					225,
					target
				},
				{
					3,
					message
				}
			};
			return this.chatPeer.OpCustom(3, customOpParameters, reliable, 0, encrypt);
		}

		private bool SetOnlineStatus(int status, object message, bool skipMessage)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "SetOnlineStatus called while not connected to front end server.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>
			{
				{
					10,
					status
				}
			};
			if (skipMessage)
			{
				dictionary[12] = true;
			}
			else
			{
				dictionary[3] = message;
			}
			return this.chatPeer.OpCustom(5, dictionary, true);
		}

		public bool SetOnlineStatus(int status)
		{
			return this.SetOnlineStatus(status, null, true);
		}

		public bool SetOnlineStatus(int status, object message)
		{
			return this.SetOnlineStatus(status, message, false);
		}

		public bool AddFriends(string[] friends)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "AddFriends called while not connected to front end server.");
				}
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, "AddFriends can't be called for empty or null list.");
				}
				return false;
			}
			if (friends.Length > 1024)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, string.Concat(new object[]
					{
						"AddFriends max list size exceeded: ",
						friends.Length,
						" > ",
						1024
					}));
				}
				return false;
			}
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
			{
				{
					11,
					friends
				}
			};
			return this.chatPeer.OpCustom(6, customOpParameters, true);
		}

		public bool RemoveFriends(string[] friends)
		{
			if (!this.CanChat)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "RemoveFriends called while not connected to front end server.");
				}
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends can't be called for empty or null list.");
				}
				return false;
			}
			if (friends.Length > 1024)
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, string.Concat(new object[]
					{
						"RemoveFriends max list size exceeded: ",
						friends.Length,
						" > ",
						1024
					}));
				}
				return false;
			}
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
			{
				{
					11,
					friends
				}
			};
			return this.chatPeer.OpCustom(7, customOpParameters, true);
		}

		public string GetPrivateChannelNameByUser(string userName)
		{
			return string.Format("{0}:{1}", this.UserId, userName);
		}

		public bool TryGetChannel(string channelName, bool isPrivate, out ChatChannel channel)
		{
			if (!isPrivate)
			{
				return this.PublicChannels.TryGetValue(channelName, out channel);
			}
			return this.PrivateChannels.TryGetValue(channelName, out channel);
		}

		public bool TryGetChannel(string channelName, out ChatChannel channel)
		{
			bool flag = this.PublicChannels.TryGetValue(channelName, out channel);
			return flag || this.PrivateChannels.TryGetValue(channelName, out channel);
		}

		public void SendAcksOnly()
		{
			if (this.chatPeer != null)
			{
				this.chatPeer.SendAcksOnly();
			}
		}

		void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
		{
			this.listener.DebugReturn(level, message);
		}

		void IPhotonPeerListener.OnEvent(EventData eventData)
		{
			switch (eventData.Code)
			{
			case 0:
				this.HandleChatMessagesEvent(eventData);
				break;
			case 2:
				this.HandlePrivateMessageEvent(eventData);
				break;
			case 4:
				this.HandleStatusUpdate(eventData);
				break;
			case 5:
				this.HandleSubscribeEvent(eventData);
				break;
			case 6:
				this.HandleUnsubscribeEvent(eventData);
				break;
			}
		}

		void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
		{
			byte operationCode = operationResponse.OperationCode;
			switch (operationCode)
			{
			case 0:
			case 1:
			case 2:
			case 3:
				break;
			default:
				if (operationCode == 230)
				{
					this.HandleAuthResponse(operationResponse);
					return;
				}
				break;
			}
			if (operationResponse.ReturnCode != 0 && this.DebugOut >= DebugLevel.ERROR)
			{
				if (operationResponse.ReturnCode == -2)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, string.Format("Chat Operation {0} unknown on server. Check your AppId and make sure it's for a Chat application.", operationResponse.OperationCode));
				}
				else
				{
					this.listener.DebugReturn(DebugLevel.ERROR, string.Format("Chat Operation {0} failed (Code: {1}). Debug Message: {2}", operationResponse.OperationCode, operationResponse.ReturnCode, operationResponse.DebugMessage));
				}
			}
		}

		void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
		{
			if (statusCode != StatusCode.Connect)
			{
				if (statusCode != StatusCode.Disconnect)
				{
					if (statusCode != StatusCode.EncryptionEstablished)
					{
						if (statusCode == StatusCode.EncryptionFailedToEstablish)
						{
							this.State = ChatState.Disconnecting;
							this.chatPeer.Disconnect();
						}
					}
					else if (!this.didAuthenticate)
					{
						this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
						if (!this.didAuthenticate && this.DebugOut >= DebugLevel.ERROR)
						{
							((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
						}
					}
				}
				else if (this.State == ChatState.Authenticated)
				{
					this.ConnectToFrontEnd();
				}
				else
				{
					this.State = ChatState.Disconnected;
					this.listener.OnChatStateChange(ChatState.Disconnected);
					this.listener.OnDisconnected();
				}
			}
			else
			{
				if (!this.chatPeer.IsProtocolSecure)
				{
					this.chatPeer.EstablishEncryption();
				}
				else if (!this.didAuthenticate)
				{
					this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
					if (!this.didAuthenticate && this.DebugOut >= DebugLevel.ERROR)
					{
						((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
					}
				}
				if (this.State == ChatState.ConnectingToNameServer)
				{
					this.State = ChatState.ConnectedToNameServer;
					this.listener.OnChatStateChange(this.State);
				}
				else if (this.State == ChatState.ConnectingToFrontEnd)
				{
					this.AuthenticateOnFrontEnd();
				}
			}
		}

		private bool SendChannelOperation(string[] channels, byte operation, int historyLength)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>
			{
				{
					0,
					channels
				}
			};
			if (historyLength != 0)
			{
				dictionary.Add(14, historyLength);
			}
			return this.chatPeer.OpCustom(operation, dictionary, true);
		}

		private void HandlePrivateMessageEvent(EventData eventData)
		{
			object message = eventData.Parameters[3];
			string text = (string)eventData.Parameters[5];
			string privateChannelNameByUser;
			if (this.UserId != null && this.UserId.Equals(text))
			{
				string userName = (string)eventData.Parameters[225];
				privateChannelNameByUser = this.GetPrivateChannelNameByUser(userName);
			}
			else
			{
				privateChannelNameByUser = this.GetPrivateChannelNameByUser(text);
			}
			ChatChannel chatChannel;
			if (!this.PrivateChannels.TryGetValue(privateChannelNameByUser, out chatChannel))
			{
				chatChannel = new ChatChannel(privateChannelNameByUser);
				chatChannel.IsPrivate = true;
				chatChannel.MessageLimit = this.MessageLimit;
				this.PrivateChannels.Add(chatChannel.Name, chatChannel);
			}
			chatChannel.Add(text, message);
			this.listener.OnPrivateMessage(text, message, privateChannelNameByUser);
		}

		private void HandleChatMessagesEvent(EventData eventData)
		{
			object[] messages = (object[])eventData.Parameters[2];
			string[] senders = (string[])eventData.Parameters[4];
			string text = (string)eventData.Parameters[1];
			ChatChannel chatChannel;
			if (!this.PublicChannels.TryGetValue(text, out chatChannel))
			{
				if (this.DebugOut >= DebugLevel.WARNING)
				{
					this.listener.DebugReturn(DebugLevel.WARNING, "Channel " + text + " for incoming message event not found.");
				}
				return;
			}
			chatChannel.Add(senders, messages);
			this.listener.OnGetMessages(text, senders, messages);
		}

		private void HandleSubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData.Parameters[0];
			bool[] array2 = (bool[])eventData.Parameters[15];
			for (int i = 0; i < array.Length; i++)
			{
				if (array2[i])
				{
					string text = array[i];
					if (!this.PublicChannels.ContainsKey(text))
					{
						ChatChannel chatChannel = new ChatChannel(text);
						chatChannel.MessageLimit = this.MessageLimit;
						this.PublicChannels.Add(chatChannel.Name, chatChannel);
					}
				}
			}
			this.listener.OnSubscribed(array, array2);
		}

		private void HandleUnsubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData[0];
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				this.PublicChannels.Remove(text);
				this.PublicChannelsUnsubscribing.Remove(text);
			}
			this.listener.OnUnsubscribed(array);
		}

		private void HandleAuthResponse(OperationResponse operationResponse)
		{
			if (this.DebugOut >= DebugLevel.INFO)
			{
				this.listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " on: " + this.chatPeer.NameServerAddress);
			}
			if (operationResponse.ReturnCode == 0)
			{
				if (this.State == ChatState.ConnectedToNameServer)
				{
					this.State = ChatState.Authenticated;
					this.listener.OnChatStateChange(this.State);
					if (operationResponse.Parameters.ContainsKey(221))
					{
						if (this.AuthValues == null)
						{
							this.AuthValues = new AuthenticationValues();
						}
						this.AuthValues.Token = (operationResponse[221] as string);
						this.FrontendAddress = (string)operationResponse[230];
						this.chatPeer.Disconnect();
					}
					else if (this.DebugOut >= DebugLevel.ERROR)
					{
						this.listener.DebugReturn(DebugLevel.ERROR, "No secret in authentication response.");
					}
				}
				else if (this.State == ChatState.ConnectingToFrontEnd)
				{
					this.msDeltaForServiceCalls *= 4;
					this.State = ChatState.ConnectedToFrontEnd;
					this.listener.OnChatStateChange(this.State);
					this.listener.OnConnected();
				}
			}
			else
			{
				short returnCode = operationResponse.ReturnCode;
				switch (returnCode)
				{
				case 32755:
					this.DisconnectedCause = ChatDisconnectCause.CustomAuthenticationFailed;
					break;
				case 32756:
					this.DisconnectedCause = ChatDisconnectCause.InvalidRegion;
					break;
				case 32757:
					this.DisconnectedCause = ChatDisconnectCause.MaxCcuReached;
					break;
				default:
					if (returnCode != -3)
					{
						if (returnCode == 32767)
						{
							this.DisconnectedCause = ChatDisconnectCause.InvalidAuthentication;
						}
					}
					else
					{
						this.DisconnectedCause = ChatDisconnectCause.OperationNotAllowedInCurrentState;
					}
					break;
				}
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "Authentication request error: " + operationResponse.ReturnCode + ". Disconnecting.");
				}
				this.State = ChatState.Disconnecting;
				this.chatPeer.Disconnect();
			}
		}

		private void HandleStatusUpdate(EventData eventData)
		{
			string user = (string)eventData.Parameters[5];
			int status = (int)eventData.Parameters[10];
			object message = null;
			bool flag = eventData.Parameters.ContainsKey(3);
			if (flag)
			{
				message = eventData.Parameters[3];
			}
			this.listener.OnStatusUpdate(user, status, flag, message);
		}

		private void ConnectToFrontEnd()
		{
			this.State = ChatState.ConnectingToFrontEnd;
			if (this.DebugOut >= DebugLevel.INFO)
			{
				this.listener.DebugReturn(DebugLevel.INFO, "Connecting to frontend " + this.FrontendAddress);
			}
			this.chatPeer.Connect(this.FrontendAddress, "chat");
		}

		private bool AuthenticateOnFrontEnd()
		{
			if (this.AuthValues == null)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Authentication Values are not set");
				}
				return false;
			}
			if (this.AuthValues.Token == null || this.AuthValues.Token == string.Empty)
			{
				if (this.DebugOut >= DebugLevel.ERROR)
				{
					this.listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Secret is not set");
				}
				return false;
			}
			Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>
			{
				{
					221,
					this.AuthValues.Token
				}
			};
			return this.chatPeer.OpCustom(230, customOpParameters, true);
		}
	}
}
