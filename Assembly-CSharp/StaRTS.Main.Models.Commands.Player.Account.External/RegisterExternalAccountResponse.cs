using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Account.External
{
	public class RegisterExternalAccountResponse : AbstractResponse
	{
		public Dictionary<string, PlayerIdentityInfo> PlayerIdentities
		{
			get;
			private set;
		}

		public string Secret
		{
			get;
			private set;
		}

		public int LastSyncedTimeStamp
		{
			get;
			private set;
		}

		public string DerivedExternalAccountId
		{
			get;
			set;
		}

		public int ExternalAccountReward
		{
			get;
			set;
		}

		public RegisterExternalAccountResponse()
		{
			this.PlayerIdentities = new Dictionary<string, PlayerIdentityInfo>();
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("identities"))
				{
					Dictionary<string, object> dictionary2 = dictionary["identities"] as Dictionary<string, object>;
					if (dictionary2 != null)
					{
						foreach (KeyValuePair<string, object> current in dictionary2)
						{
							PlayerIdentityInfo playerIdentityInfo = new PlayerIdentityInfo();
							playerIdentityInfo.FromObject(current.Value);
							this.PlayerIdentities.Add(current.Key, playerIdentityInfo);
						}
					}
				}
				if (dictionary.ContainsKey("secret"))
				{
					this.Secret = (string)dictionary["secret"];
				}
				if (dictionary.ContainsKey("registrationTime"))
				{
					this.LastSyncedTimeStamp = Convert.ToInt32((string)dictionary["registrationTime"]);
				}
				if (dictionary.ContainsKey("derivedExternalAccountId"))
				{
					this.DerivedExternalAccountId = (string)dictionary["derivedExternalAccountId"];
				}
				if (dictionary.ContainsKey("registrationReward"))
				{
					this.ExternalAccountReward = Convert.ToInt32((string)dictionary["registrationReward"]);
				}
			}
			return this;
		}
	}
}
