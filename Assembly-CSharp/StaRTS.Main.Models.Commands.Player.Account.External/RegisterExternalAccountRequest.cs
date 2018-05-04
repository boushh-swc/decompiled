using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Account.External
{
	public class RegisterExternalAccountRequest : AbstractRequest
	{
		public AccountProvider Provider
		{
			get;
			set;
		}

		public string ExternalAccountId
		{
			get;
			set;
		}

		public string ExternalAccountSecurityToken
		{
			get;
			set;
		}

		public bool OverrideExistingAccountRegistration
		{
			get;
			set;
		}

		public AccountProvider OtherLinkedProvider
		{
			get;
			set;
		}

		public string PlayerId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", this.PlayerId);
			serializer.AddString("providerId", this.Provider.ToString());
			serializer.AddString("externalAccountId", this.ExternalAccountId);
			serializer.AddString("externalAccountSecurityToken", this.ExternalAccountSecurityToken);
			serializer.AddBool("overrideExistingAccountRegistration", this.OverrideExistingAccountRegistration);
			serializer.AddString("otherLinkedProviderId", this.OtherLinkedProvider.ToString());
			return serializer.End().ToString();
		}
	}
}
