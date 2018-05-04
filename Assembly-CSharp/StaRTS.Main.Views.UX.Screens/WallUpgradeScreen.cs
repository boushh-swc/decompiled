using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Contracts;
using StaRTS.Main.Models.Commands.Player.Building.Upgrade;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class WallUpgradeScreen : BuildingInfoScreen
	{
		private const string UPGRADE_ALL_WALLS_BUTTON_COST = "SecondaryCost";

		private const string UPGRADE_ALL_WALLS_BUTTON_COST_LABEL = "SecondaryCostLabel";

		private List<Entity> wallsOfSameLevel;

		private int currentWallLevel;

		private int allWallSameLevelCount;

		public WallUpgradeScreen(Entity selectedBuilding) : base(selectedBuilding)
		{
			this.useUpgradeGroup = true;
			this.currentWallLevel = this.buildingInfo.Lvl;
			this.wallsOfSameLevel = new List<Entity>();
			string key = selectedBuilding.Get<BuildingComponent>().BuildingTO.Key;
			NodeList<WallNode> wallNodeList = Service.BuildingLookupController.WallNodeList;
			for (WallNode wallNode = wallNodeList.Head; wallNode != null; wallNode = wallNode.Next)
			{
				if (wallNode.BuildingComp.BuildingTO.Key != key && wallNode.BuildingComp.BuildingType.Lvl == this.currentWallLevel)
				{
					this.wallsOfSameLevel.Add(wallNode.Entity);
				}
			}
			this.allWallSameLevelCount = this.wallsOfSameLevel.Count + 1;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.buttonInstantBuy.Visible = false;
			this.buttonUpgradeAllWalls.Visible = true;
			this.buttonUpgradeAllWalls.Enabled = (this.reqMet && GameConstants.ENABLE_UPGRADE_ALL_WALLS);
			this.buttonUpgradeAllWalls.OnClicked = new UXButtonClickedDelegate(this.OnUpgradeAllWallsButton);
			int crystals = GameUtils.CrystalCostToUpgradeAllWalls(this.nextBuildingInfo.UpgradeMaterials, this.allWallSameLevelCount);
			UXUtils.SetupSingleCostElement(this, "SecondaryCost", 0, 0, 0, crystals, 0, !this.reqMet, null);
		}

		protected override void InitLabels()
		{
			base.InitLabels();
			if (this.reqMet)
			{
				this.labelPrimaryAction.Visible = true;
				this.labelSecondaryAction.Visible = true;
				this.labelPrimaryAction.Text = this.lang.Get("BUILDING_UPGRADE_SINGLE_WALL", new object[0]);
				this.labelSecondaryAction.Text = this.lang.Get("BUILDING_UPGRADE_ALL_WALLS", new object[]
				{
					this.currentWallLevel,
					this.allWallSameLevelCount
				});
			}
		}

		private void OnUpgradeAllWallsButton(UXButton button)
		{
			int num = GameUtils.CrystalCostToUpgradeAllWalls(this.nextBuildingInfo.UpgradeMaterials, this.allWallSameLevelCount);
			string title = this.lang.Get("upgrade_all_walls_confirm_title", new object[0]);
			string message = this.lang.Get("upgrade_all_walls_conifrm_desc", new object[]
			{
				this.allWallSameLevelCount,
				this.currentWallLevel + 1,
				num
			});
			AlertScreen.ShowModal(false, title, message, new OnScreenModalResult(this.ConfirmUpgradeAllWalls), null);
		}

		private void ConfirmUpgradeAllWalls(object result, object cookie)
		{
			string context = "UI_money_flow";
			string action = (result != null) ? "buy" : "close";
			string message = "upgrade_all_walls";
			Service.BILoggingController.TrackGameAction(context, action, message, null);
			if (result == null)
			{
				return;
			}
			EventManager eventManager = Service.EventManager;
			eventManager.SendEvent(EventId.MuteEvent, EventId.InventoryResourceUpdated);
			eventManager.SendEvent(EventId.MuteEvent, EventId.ContractAdded);
			int num = GameUtils.CrystalCostToUpgradeAllWalls(this.nextBuildingInfo.UpgradeMaterials, this.allWallSameLevelCount);
			if (!GameUtils.SpendCrystals(num))
			{
				eventManager.SendEvent(EventId.UnmuteEvent, EventId.InventoryResourceUpdated);
				eventManager.SendEvent(EventId.UnmuteEvent, EventId.ContractAdded);
				return;
			}
			int currencyAmount = -num;
			string itemType = "soft_currency_flow";
			string itemId = "materials";
			int itemCount = 1;
			string type = "currency_purchase";
			string subType = "durable";
			Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, itemId, itemCount, type, subType);
			string uid = this.buildingInfo.Uid;
			ISupportController iSupportController = Service.ISupportController;
			iSupportController.StartAllWallPartBuildingUpgrade(this.nextBuildingInfo, this.selectedBuilding, false, false);
			int count = this.wallsOfSameLevel.Count;
			for (int i = 0; i < count; i++)
			{
				iSupportController.StartAllWallPartBuildingUpgrade(this.nextBuildingInfo, this.wallsOfSameLevel[i], false, false);
			}
			BuildingUpgradeAllWallsCommand command = new BuildingUpgradeAllWallsCommand(new BuildingUpgradeAllWallsRequest
			{
				BuildingUid = uid,
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			Service.ServerAPI.Enqueue(command);
			for (int j = 0; j < count; j++)
			{
				iSupportController.FinishCurrentContract(this.wallsOfSameLevel[j], j != 0, false);
			}
			iSupportController.FinishCurrentContract(this.selectedBuilding, true, false);
			eventManager.SendEvent(EventId.UnmuteEvent, EventId.InventoryResourceUpdated);
			eventManager.SendEvent(EventId.UnmuteEvent, EventId.ContractAdded);
			eventManager.SendEvent(EventId.SimulateAudioEvent, new AudioEventData(EventId.InventoryResourceUpdated, "crystals"));
		}
	}
}
