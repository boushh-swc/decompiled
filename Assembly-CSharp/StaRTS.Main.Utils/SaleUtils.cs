using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Utils
{
	public static class SaleUtils
	{
		public static SaleTypeVO GetCurrentActiveSale()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (SaleTypeVO current in staticDataController.GetAll<SaleTypeVO>())
			{
				if (TimedEventUtils.IsTimedEventActive(current))
				{
					return current;
				}
			}
			return null;
		}

		public static List<SaleItemTypeVO> GetSaleItems(string[] saleItemUids)
		{
			List<SaleItemTypeVO> list = new List<SaleItemTypeVO>();
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (SaleItemTypeVO current in staticDataController.GetAll<SaleItemTypeVO>())
			{
				if (Array.IndexOf<string>(saleItemUids, current.Uid) >= 0)
				{
					list.Add(current);
				}
			}
			return list;
		}
	}
}
