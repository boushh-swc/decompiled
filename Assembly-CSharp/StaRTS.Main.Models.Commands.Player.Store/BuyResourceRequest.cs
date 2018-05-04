using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Store
{
	public class BuyResourceRequest : PlayerIdChecksumRequest
	{
		private const string TYPE_RESOURCES = "Resources";

		private const string UID_DROIDS = "droids";

		private const string UID_PROTECTION = "protection";

		private string uid;

		private string currency;

		private string type;

		private int count;

		private string purchaseContext;

		private BuyResourceRequest(string uid, int amount)
		{
			this.uid = uid;
			this.count = amount;
			this.currency = "crystals";
			this.type = "Resources";
		}

		public static BuyResourceRequest MakeBuyResourceRequest(CurrencyType resourceToBuy, int amount)
		{
			string text = null;
			switch (resourceToBuy)
			{
			case CurrencyType.Credits:
				text = "credits";
				break;
			case CurrencyType.Materials:
				text = "materials";
				break;
			case CurrencyType.Contraband:
				text = "contraband";
				break;
			}
			return new BuyResourceRequest(text, amount);
		}

		public static BuyResourceRequest MakeBuyDroidRequest(int amount)
		{
			return new BuyResourceRequest("droids", amount);
		}

		public static BuyResourceRequest MakeBuyProtectionRequest(int packNumber)
		{
			return new BuyResourceRequest("protection", packNumber);
		}

		public void setPurchaseContext(string value)
		{
			this.purchaseContext = value;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("uid", this.uid);
			startedSerializer.AddString("currency", this.currency);
			startedSerializer.AddString("type", this.type);
			startedSerializer.Add<int>("count", this.count);
			if (this.purchaseContext != null && !this.purchaseContext.Equals(string.Empty))
			{
				startedSerializer.AddString("purchaseContext", this.purchaseContext);
			}
			return startedSerializer.End().ToString();
		}
	}
}
