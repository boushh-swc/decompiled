using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class PromoPopupManager
	{
		private IPromoPopupHelper helper;

		public PromoPopupManager()
		{
			Service.PromoPopupManager = this;
			this.helper = new DefaultPromoPopupHelper();
		}

		public void ShowPromoDialog(OnScreenModalResult modalResult, object result)
		{
			this.helper.ShowPromoDialog(modalResult, result);
		}
	}
}
