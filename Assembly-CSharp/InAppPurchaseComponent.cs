using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InAppPurchaseComponent : MonoBehaviour
{
	public void OnGetInfoForProducts(string jsonString)
	{
		if (!string.IsNullOrEmpty(jsonString))
		{
			if (Service.Logger != null)
			{
				Service.Logger.Debug("ProductInfoString: " + jsonString);
			}
			if (Service.InAppPurchaseController != null)
			{
				Service.InAppPurchaseController.OnGetInfoForProducts(jsonString, null);
			}
			else if (Service.Logger != null)
			{
				Service.Logger.Error("ProductInfoString: InAppPurchaseController is not set");
			}
		}
		else if (Service.Logger != null)
		{
			Service.Logger.Error("ProductInfoString: IsNullOrEmpty");
		}
	}

	public void OnPurchaseProduct(string jsonString)
	{
		if (Service.Logger != null)
		{
			Service.Logger.Debug("OnPurchaseProduct: " + jsonString);
		}
		if (!string.IsNullOrEmpty(jsonString) && Service.InAppPurchaseController != null)
		{
			Service.InAppPurchaseController.OnPurchaseProductResponse(jsonString, true);
		}
	}

	public void onConsumePurchase(string jsonString)
	{
		if (!string.IsNullOrEmpty(jsonString))
		{
			Service.Logger.Debug("OnConsumeProduct: " + jsonString);
		}
	}

	public void OnBILog(string biLog)
	{
		if (string.IsNullOrEmpty(biLog))
		{
			return;
		}
		string text = biLog;
		string action = string.Empty;
		int num = text.IndexOf('|');
		if (num >= 0)
		{
			action = text.Substring(num + 1);
			text = text.Substring(0, num);
		}
		if (Service.BILoggingController != null)
		{
			Service.BILoggingController.TrackIAPGameAction("iap", action, text);
		}
	}

	public void OnRestorePurchases(string jsonString)
	{
		if (!string.IsNullOrEmpty(jsonString))
		{
			Service.Logger.Debug("OnRestorePurchases: " + jsonString);
			this.RestoreAndroidPurchases(jsonString);
		}
	}

	private void RestoreAndroidPurchases(string jsonString)
	{
		if (!string.IsNullOrEmpty(jsonString))
		{
			IDictionary<string, object> dictionary = new JsonParser(jsonString).Parse() as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("purchases"))
			{
				List<object> list = dictionary["purchases"] as List<object>;
				for (int i = 0; i < list.Count; i++)
				{
					Serializer serializer = new Serializer();
					IDictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						Service.Logger.Debug(string.Concat(new object[]
						{
							"key: ",
							current.Key,
							" value: ",
							current.Value
						}));
						serializer.AddString(current.Key, current.Value as string);
					}
					serializer.End();
					Service.Logger.Debug("Restored Purcase:" + serializer.ToString());
					Service.InAppPurchaseController.OnPurchaseProductResponse(serializer.ToString(), true);
				}
			}
		}
	}

	private void RestoreIOSPurchases(string jsonString)
	{
		if (string.IsNullOrEmpty(jsonString))
		{
			return;
		}
		if (Service.InAppPurchaseController != null)
		{
			Service.InAppPurchaseController.OnPurchaseProductResponse(jsonString, true);
		}
	}
}
