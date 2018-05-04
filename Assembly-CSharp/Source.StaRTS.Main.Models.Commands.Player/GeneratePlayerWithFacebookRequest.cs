using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace Source.StaRTS.Main.Models.Commands.Player
{
	public class GeneratePlayerWithFacebookRequest : AbstractRequest
	{
		public string LocalePreference
		{
			get;
			set;
		}

		public string Network
		{
			get;
			set;
		}

		public string ViewNetwork
		{
			get;
			set;
		}

		public string FacebookID
		{
			get;
			set;
		}

		public string FacebookAuthToken
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			if (this.LocalePreference != null)
			{
				serializer.AddString("locale", this.LocalePreference);
			}
			if (this.FacebookID != null)
			{
				serializer.AddString("facebookId", this.FacebookID);
			}
			if (this.FacebookAuthToken != null)
			{
				serializer.AddString("facebookAuthToken", this.FacebookAuthToken);
			}
			return serializer.End().ToString();
		}
	}
}
