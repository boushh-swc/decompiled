using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Bot
{
	public class BPDonateTroops : BotPerformer
	{
		public override void Perform()
		{
			Service.BotRunner.Log("Donating Troops", new object[0]);
			Service.BotRunner.Performing = true;
			SquadMsg squadMsg = (SquadMsg)Service.BotRunner.BotProperties[(string)this.arg];
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string text = (currentPlayer.Faction != FactionType.Empire) ? "Soldier" : "Storm";
			int level = currentPlayer.UnlockedLevels.Troops.GetLevel(text);
			TroopTypeVO byLevel = Service.TroopUpgradeCatalog.GetByLevel(text, level);
			int num = 4;
			dictionary.Add(byLevel.Uid, num);
			SquadMsg message = SquadMsgUtils.CreateDonateMessage(squadMsg.OwnerData.PlayerId, dictionary, num, squadMsg.NotifId, new SquadController.ActionCallback(this.OnComplete), squadMsg);
			SquadController squadController = Service.SquadController;
			squadController.TakeAction(message);
		}

		protected void OnComplete(bool success, object cookie)
		{
			Service.BotRunner.BotProperties[(string)this.arg] = null;
			Service.BotRunner.Performing = false;
			base.Perform();
		}
	}
}
