using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace Source.StaRTS.Main.Models.Commands.Player
{
	public class GeneratePlayerRequest : AbstractRequest
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

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			if (this.LocalePreference != null)
			{
				serializer.AddString("locale", this.LocalePreference);
			}
			return serializer.End().ToString();
		}
	}
}
