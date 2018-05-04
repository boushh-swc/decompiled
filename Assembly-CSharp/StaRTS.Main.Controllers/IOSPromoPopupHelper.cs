using StaRTS.Externals.Manimal;
using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Globalization;

namespace StaRTS.Main.Controllers
{
	public class IOSPromoPopupHelper : IPromoPopupHelper
	{
		public void ShowPromoDialog(OnScreenModalResult modalResult, object result)
		{
			int num = 0;
			string iOS_PROMO_END_DATE = GameConstants.IOS_PROMO_END_DATE;
			if (!string.IsNullOrEmpty(iOS_PROMO_END_DATE))
			{
				DateTime date = DateTime.ParseExact(iOS_PROMO_END_DATE, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
				num = DateUtils.GetSecondsFromEpoch(date);
			}
			if ((ulong)ServerTime.Time < (ulong)((long)num))
			{
				Service.ScreenController.AddScreen(new ApplePromoScreen(modalResult, result));
			}
			else if (modalResult != null)
			{
				modalResult(result, null);
			}
		}
	}
}
