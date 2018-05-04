using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Externals.IAP
{
	public class AndroidIAPManager : IInAppPurchaseManager
	{
		private AndroidJavaObject iapHandler;

		private AndroidJavaObject pluginActivity;

		public void Init()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.starts.PluginActivity");
			this.pluginActivity = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			this.iapHandler = this.pluginActivity.Get<AndroidJavaObject>("iapHandler");
			Service.Logger.Debug("Android IAP Init: MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvR0OFW4ydPpr27ptG5U7LG5v6XHsIeGmRv46oHk/RP+V4NjuQrrWj/LWz/uoH/7B5bSiiZPFTXpCmmD9Zqi4EIn79A6IZf1l9oMKX0H/PqNp3PyJOwh+Egkp10UrM7KBjbiDf5YZhRKG3L8FpQl/Y9TBvfUyxb4HLAkmYUaqMgscN4GTSMHUVcjuSGkgmURYKMLSWYa1leDE8vZZ5vZCoB20Kh6PN1IcvUq/FZE1NxV1cNX44lE3DIzQuUJy+VB1Mg5aCIk6A9/GTD+BdeDKAgtf6ktiLK2oJRwe2c5quhI7cLNX3+jQJFcEdz5+pmcNeRLkkFelAd5vYU1c8WWotwIDAQAB");
			this.iapHandler.Call("Configure", new object[]
			{
				"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvR0OFW4ydPpr27ptG5U7LG5v6XHsIeGmRv46oHk/RP+V4NjuQrrWj/LWz/uoH/7B5bSiiZPFTXpCmmD9Zqi4EIn79A6IZf1l9oMKX0H/PqNp3PyJOwh+Egkp10UrM7KBjbiDf5YZhRKG3L8FpQl/Y9TBvfUyxb4HLAkmYUaqMgscN4GTSMHUVcjuSGkgmURYKMLSWYa1leDE8vZZ5vZCoB20Kh6PN1IcvUq/FZE1NxV1cNX44lE3DIzQuUJy+VB1Mg5aCIk6A9/GTD+BdeDKAgtf6ktiLK2oJRwe2c5quhI7cLNX3+jQJFcEdz5+pmcNeRLkkFelAd5vYU1c8WWotwIDAQAB"
			});
		}

		public void GetProducts()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<string, InAppPurchaseTypeVO>.ValueCollection allIAPTypes = Service.InAppPurchaseController.GetAllIAPTypes();
			if (allIAPTypes != null)
			{
				foreach (InAppPurchaseTypeVO current in allIAPTypes)
				{
					if (current.Type == "a")
					{
						stringBuilder.Append(current.ProductId);
						stringBuilder.Append(",");
					}
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			this.iapHandler.Call("GetInfoForProducts", new object[]
			{
				stringBuilder.ToString()
			});
		}

		public void Purchase(string productID)
		{
			bool flag = this.iapHandler.Call<bool>("PurchaseProduct", new object[]
			{
				productID,
				"true"
			});
			Service.Logger.Debug(string.Concat(new object[]
			{
				"PurchaseProduct: ",
				productID,
				":",
				flag
			}));
		}

		public void Consume(string productID)
		{
			this.iapHandler.Call("ConsumeProduct", new object[]
			{
				productID
			});
		}

		public void RestorePurchases()
		{
			bool flag = this.iapHandler.Call<bool>("RestorePurchases", new object[0]);
			Service.Logger.Debug("RestorePurchases: " + flag);
		}

		public void OnApplicationResume()
		{
		}
	}
}
