using StaRTS.Main.Models.Player;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class PromoRUFTask : AbstractRUFTask
	{
		public PromoRUFTask()
		{
			base.Priority = 10;
			base.ShouldProcess = true;
			base.ShouldPurgeQueue = false;
			base.ShouldPlayFromLoadState = true;
		}

		public override void Process(bool continueProcessing)
		{
			if (base.ShouldProcess)
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				bool flag = currentPlayer.FirstTimePlayer && currentPlayer.NumIdentities == 1;
				if (flag)
				{
					Service.PromoPopupManager.ShowPromoDialog(new OnScreenModalResult(this.OnPromoDialogViewed), continueProcessing);
				}
				else
				{
					base.Process(continueProcessing);
				}
			}
		}

		public void OnPromoDialogViewed(object result, object cookie)
		{
			base.Process((bool)result);
		}
	}
}
