using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Account.External
{
	public class GetExternalAccountsResponse : AbstractResponse
	{
		public string DerivedFacebookAccountId
		{
			get;
			set;
		}

		public string DerivedGameCenterAccountId
		{
			get;
			set;
		}

		public string DerivedGooglePlayAccountId
		{
			get;
			set;
		}

		public string DerivedRecoveryId
		{
			get;
			set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				string key = AccountProvider.FACEBOOK.ToString();
				string key2 = AccountProvider.GAMECENTER.ToString();
				string key3 = AccountProvider.GOOGLEPLAY.ToString();
				string key4 = AccountProvider.RECOVERY.ToString();
				if (dictionary.ContainsKey(key))
				{
					List<object> list = dictionary[key] as List<object>;
					if (list != null && list.Count > 0)
					{
						this.DerivedFacebookAccountId = (list[0] as string);
					}
				}
				if (dictionary.ContainsKey(key2))
				{
					List<object> list = dictionary[key2] as List<object>;
					if (list != null && list.Count > 0)
					{
						this.DerivedGameCenterAccountId = (list[0] as string);
					}
				}
				if (dictionary.ContainsKey(key3))
				{
					List<object> list = dictionary[key3] as List<object>;
					if (list != null && list.Count > 0)
					{
						this.DerivedGooglePlayAccountId = (list[0] as string);
					}
				}
				if (dictionary.ContainsKey(key4))
				{
					List<object> list = dictionary[key4] as List<object>;
					if (list != null && list.Count > 0)
					{
						this.DerivedRecoveryId = (list[0] as string);
					}
				}
			}
			return this;
		}
	}
}
