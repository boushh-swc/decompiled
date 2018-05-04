using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player
{
	public class MoneyReceiptVerifyResponse : AbstractResponse
	{
		private CrateData crateDataTO;

		public object Result
		{
			get;
			protected set;
		}

		public uint Status
		{
			get;
			set;
		}

		public List<Data> DataList
		{
			get;
			protected set;
		}

		public string TransactionId
		{
			get;
			set;
		}

		public override ISerializable FromObject(object obj)
		{
			bool flag = false;
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			Dictionary<string, object> dictionary2 = dictionary["iap"] as Dictionary<string, object>;
			Dictionary<string, object> dictionary3 = null;
			bool isPromo = (bool)dictionary["isPromo"];
			string currencyCode = "USD";
			double price = 0.0;
			string uid = string.Empty;
			double num = 1.0;
			string offerUid = null;
			if (dictionary.ContainsKey("sale"))
			{
				dictionary3 = (dictionary["sale"] as Dictionary<string, object>);
			}
			if (dictionary3 != null && dictionary3.ContainsKey("bonusMultiplier"))
			{
				num = Convert.ToDouble(dictionary3["bonusMultiplier"]);
				if (flag)
				{
					Service.Logger.Debug("MoneyReceiptVerifyResponse: Bonus Multiplier: " + num);
				}
			}
			if (dictionary2.ContainsKey("uid"))
			{
				uid = (dictionary2["uid"] as string);
			}
			if (dictionary2.ContainsKey("price"))
			{
				price = Convert.ToDouble(dictionary2["price"]);
			}
			if (dictionary.ContainsKey("targetedOffer"))
			{
				offerUid = (dictionary["targetedOffer"] as string);
			}
			if (dictionary.ContainsKey("crateData"))
			{
				this.crateDataTO = new CrateData();
				this.crateDataTO.FromObject(dictionary["crateData"]);
			}
			Kochava.FireEvent("paymentAction", "1");
			Kochava.FireEvent("revenueAmount", price.ToString());
			if (this.Status == 0u)
			{
				Service.InAppPurchaseController.HandleReceiptVerificationResponse(uid, this.TransactionId, currencyCode, price, num, isPromo, offerUid, this.crateDataTO);
			}
			return this;
		}
	}
}
