using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;

public class InAppPurchaseReceipt
{
	public bool useSandbox;

	public uint errorCode;

	public string price;

	public string transactionId;

	public string rawData;

	public string productId;

	public string signature;

	public string userId;

	public static InAppPurchaseReceipt Parse(string value)
	{
		InAppPurchaseReceipt inAppPurchaseReceipt = new InAppPurchaseReceipt();
		IDictionary<string, object> dictionary = new JsonParser(value).Parse() as Dictionary<string, object>;
		if (dictionary.ContainsKey("userSandbox") && dictionary["useSandbox"] != null)
		{
			inAppPurchaseReceipt.useSandbox = (bool)dictionary["useSandbox"];
		}
		inAppPurchaseReceipt.errorCode = Convert.ToUInt32(dictionary["errorCode"] as string);
		inAppPurchaseReceipt.price = (dictionary["price"] as string);
		inAppPurchaseReceipt.transactionId = (dictionary["transactionId"] as string);
		inAppPurchaseReceipt.rawData = (dictionary["rawData"] as string);
		inAppPurchaseReceipt.productId = (dictionary["productId"] as string);
		if (dictionary.ContainsKey("signature"))
		{
			inAppPurchaseReceipt.signature = (dictionary["signature"] as string);
		}
		return inAppPurchaseReceipt;
	}

	public string GetManimalReceiptString()
	{
		string empty = string.Empty;
		Serializer serializer = Serializer.Start();
		serializer.AddString("signedData", this.rawData.Replace("\"", "\\\""));
		serializer.AddString("signature", this.signature);
		serializer.End();
		return serializer.ToString();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("TransactionID: ");
		stringBuilder.Append(this.transactionId);
		stringBuilder.Append(": Error Code:");
		stringBuilder.Append(this.errorCode);
		stringBuilder.Append(": ProductID:");
		stringBuilder.Append(this.productId);
		stringBuilder.Append(": Price: ");
		stringBuilder.Append(this.price);
		stringBuilder.Append(": Receipt rawData: ");
		stringBuilder.Append(this.rawData);
		if (this.signature != null)
		{
			stringBuilder.Append(": Signature:");
			stringBuilder.Append(this.signature);
		}
		return stringBuilder.ToString();
	}
}
