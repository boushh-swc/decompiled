using StaRTS.Main.Models.Leaderboard;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models
{
	public class SocialFriendData : ISerializable
	{
		private const string KEY_ID = "id";

		private const string KEY_NAME = "name";

		private const string KEY_PICTURE = "picture";

		private const string KEY_DATA = "data";

		private const string KEY_URL = "url";

		private const string KEY_INSTALLED = "installed";

		public string Id
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string PictureURL
		{
			get;
			private set;
		}

		public bool Installed
		{
			get;
			private set;
		}

		public PlayerLBEntity PlayerData
		{
			get;
			set;
		}

		public string ToJson()
		{
			return string.Empty;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Id = (dictionary["id"] as string);
			this.Name = (dictionary["name"] as string);
			if (dictionary.ContainsKey("installed"))
			{
				this.Installed = Convert.ToBoolean(dictionary["installed"]);
			}
			else
			{
				this.Installed = false;
			}
			if (dictionary.ContainsKey("picture"))
			{
				Dictionary<string, object> dictionary2 = dictionary["picture"] as Dictionary<string, object>;
				Dictionary<string, object> dictionary3 = dictionary2["data"] as Dictionary<string, object>;
				this.PictureURL = WWW.UnEscapeURL((string)dictionary3["url"]);
			}
			return this;
		}
	}
}
