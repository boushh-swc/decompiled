using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player
{
	public class MoneyReceiptVerifyRequest : PlayerIdRequest
	{
		public string VendorKey
		{
			get;
			set;
		}

		public string Receipt
		{
			get;
			set;
		}

		public Dictionary<string, string> ExtraParams
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("receipt", this.Receipt);
			serializer.AddString("vendorKey", this.VendorKey);
			if (this.ExtraParams != null)
			{
				serializer.AddDictionary<string>("extraParams", this.ExtraParams);
			}
			serializer.End();
			return serializer.ToString();
		}
	}
}
