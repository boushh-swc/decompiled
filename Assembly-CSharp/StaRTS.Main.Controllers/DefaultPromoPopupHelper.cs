using StaRTS.Main.Views.UX.Screens;
using System;

namespace StaRTS.Main.Controllers
{
	public class DefaultPromoPopupHelper : IPromoPopupHelper
	{
		public void ShowPromoDialog(OnScreenModalResult modalResult, object result)
		{
			if (modalResult != null)
			{
				modalResult(result, null);
			}
		}
	}
}
