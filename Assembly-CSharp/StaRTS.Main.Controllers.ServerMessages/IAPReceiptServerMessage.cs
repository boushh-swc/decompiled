using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.ServerMessages
{
	public class IAPReceiptServerMessage : AbstractMessage
	{
		public override object MessageCookie
		{
			get
			{
				return this;
			}
		}

		public override EventId MessageEventId
		{
			get
			{
				return EventId.IAPReceiptServerMessage;
			}
		}

		public string TransactionId
		{
			get;
			set;
		}

		public string IapUID
		{
			get;
			private set;
		}

		public double Price
		{
			get;
			private set;
		}

		public string OfferUID
		{
			get;
			private set;
		}

		public double BonusMultiplier
		{
			get;
			private set;
		}

		public bool IsPromo
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = null;
			Dictionary<string, object> dictionary2 = null;
			this.IapUID = string.Empty;
			this.Price = 0.0;
			this.BonusMultiplier = 1.0;
			this.OfferUID = null;
			Dictionary<string, object> dictionary3 = obj as Dictionary<string, object>;
			if (dictionary3.ContainsKey("sale"))
			{
				dictionary = (dictionary3["iap"] as Dictionary<string, object>);
			}
			if (dictionary3.ContainsKey("isPromo"))
			{
				this.IsPromo = (bool)dictionary3["isPromo"];
			}
			if (dictionary3.ContainsKey("sale"))
			{
				dictionary2 = (dictionary3["sale"] as Dictionary<string, object>);
			}
			if (dictionary2 != null && dictionary2.ContainsKey("bonusMultiplier"))
			{
				this.BonusMultiplier = Convert.ToDouble(dictionary2["bonusMultiplier"]);
			}
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("uid"))
				{
					this.IapUID = (dictionary["uid"] as string);
				}
				if (dictionary.ContainsKey("price"))
				{
					this.Price = Convert.ToDouble(dictionary["price"]);
				}
			}
			if (dictionary3.ContainsKey("targetedOffer"))
			{
				this.OfferUID = (dictionary3["targetedOffer"] as string);
			}
			return this;
		}
	}
}
