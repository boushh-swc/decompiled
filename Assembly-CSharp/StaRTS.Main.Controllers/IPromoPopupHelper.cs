using StaRTS.Main.Views.UX.Screens;
using System;

namespace StaRTS.Main.Controllers
{
	public interface IPromoPopupHelper
	{
		void ShowPromoDialog(OnScreenModalResult modalResult, object result);
	}
}
