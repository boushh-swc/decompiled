using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace Midcore.Chat.Photon
{
	public class PhotonChatMessageTO
	{
		private const string USER_NAME = "userName";

		private const string TEXT = "text";

		private const string TIMESTAMP = "timestamp";

		public string UserName;

		public string Text;

		public string TimeStamp;

		private string SerializedMessage;

		public PhotonChatMessageTO()
		{
		}

		public PhotonChatMessageTO(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("userName"))
				{
					this.UserName = Convert.ToString(dictionary["userName"]);
				}
				if (dictionary.ContainsKey("text"))
				{
					this.Text = Convert.ToString(dictionary["text"]);
				}
				if (dictionary.ContainsKey("timestamp"))
				{
					this.TimeStamp = Convert.ToString(dictionary["timestamp"]);
				}
			}
		}

		public string GetSerializedMessage()
		{
			if (!string.IsNullOrEmpty(this.SerializedMessage))
			{
				return this.SerializedMessage;
			}
			Serializer serializer = Serializer.Start();
			serializer.AddString("userName", this.UserName);
			serializer.AddString("text", this.Text);
			serializer.AddString("timestamp", this.TimeStamp);
			this.SerializedMessage = serializer.End().ToString();
			return this.SerializedMessage;
		}
	}
}
