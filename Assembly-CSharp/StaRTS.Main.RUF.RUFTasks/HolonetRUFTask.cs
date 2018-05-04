using StaRTS.Main.Models.Player.Misc;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class HolonetRUFTask : AbstractRUFTask
	{
		private List<BattleEntry> newBattles;

		private bool continueProcessing;

		public HolonetRUFTask()
		{
			base.Priority = 100;
			base.ShouldProcess = false;
			base.ShouldPurgeQueue = true;
			base.PriorityPurgeThreshold = 120;
			this.newBattles = Service.PvpManager.GetBattlesThatHappenOffline();
			if (this.newBattles.Count > 0 || Service.HolonetController.CalculateBadgeCount() > 0)
			{
				base.ShouldProcess = true;
			}
		}

		public override void Process(bool continueProcessing)
		{
			this.continueProcessing = continueProcessing;
			if (base.ShouldProcess)
			{
				Service.ViewTimerManager.CreateViewTimer(2f, false, new TimerDelegate(this.ShowHolonetOnTimer), this.newBattles);
			}
			else
			{
				base.Process(continueProcessing);
			}
		}

		public void ShowHolonetOnTimer(uint timerId, object data)
		{
			if (!Service.PerkManager.WillShowPerkTutorial())
			{
				List<BattleEntry> battles = (List<BattleEntry>)data;
				Service.HolonetController.InitBattlesTransmission(battles);
				Service.HolonetController.OpenHolonet();
			}
			base.Process(this.continueProcessing);
		}
	}
}
