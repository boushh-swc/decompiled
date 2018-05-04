using StaRTS.Externals.IAP;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Tags
{
	public class StoreItemTag
	{
		public BuildingTypeVO BuildingInfo
		{
			get;
			set;
		}

		public UXElement MainElement
		{
			get;
			set;
		}

		public UXButton MainButton
		{
			get;
			set;
		}

		public UXLabel InfoLabel
		{
			get;
			set;
		}

		public UXButton InfoGroup
		{
			get;
			set;
		}

		public int CurQuantity
		{
			get;
			set;
		}

		public int MaxQuantity
		{
			get;
			set;
		}

		public bool CanPurchase
		{
			get;
			set;
		}

		public BuildingTypeVO ReqBuilding
		{
			get;
			set;
		}

		public bool ReqMet
		{
			get;
			set;
		}

		public int Amount
		{
			get;
			set;
		}

		public int Price
		{
			get;
			set;
		}

		public CurrencyType Currency
		{
			get;
			set;
		}

		public string IconName
		{
			get;
			set;
		}

		public string Uid
		{
			get;
			set;
		}

		public InAppPurchaseTypeVO IAPType
		{
			get;
			set;
		}

		public InAppPurchaseProductInfo IAPProduct
		{
			get;
			set;
		}
	}
}
