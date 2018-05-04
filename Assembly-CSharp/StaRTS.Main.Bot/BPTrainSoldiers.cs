using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Bot
{
	public class BPTrainSoldiers : BotPerformer
	{
		private SmartEntity building;

		public override void Perform()
		{
			Service.BotRunner.Log("Training Troops!", new object[0]);
			int num;
			int num2;
			GameUtils.GetStarportTroopCounts(out num, out num2);
			int num3 = 4;
			if (num + num3 > num2)
			{
				base.Perform();
				return;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string text = (currentPlayer.Faction != FactionType.Empire) ? "Soldier" : "Storm";
			int level = currentPlayer.UnlockedLevels.Troops.GetLevel(text);
			TroopTypeVO byLevel = Service.TroopUpgradeCatalog.GetByLevel(text, level);
			this.building = (SmartEntity)Service.BuildingLookupController.BarracksNodeList.Head.Entity;
			ISupportController iSupportController = Service.ISupportController;
			for (int i = 0; i < num3; i++)
			{
				iSupportController.StartTroopTrainContract(byLevel, this.building);
			}
			iSupportController.BuyoutAllTroopTrainContracts(this.building);
			base.Perform();
		}
	}
}
