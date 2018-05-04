using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadUpgradeScreen : SquadBuildingScreen
	{
		private const string UPGRADE_FIELD_CAPACITY = "UPGRADE_FIELD_CAPACITY";

		public SquadUpgradeScreen(Entity selectedBuilding) : base(selectedBuilding)
		{
			this.useUpgradeGroup = true;
		}

		protected override void OnLoaded()
		{
			base.InitControls(3);
			this.InitHitpoints(0);
			this.UpdateCapacity(1);
			this.InitReputation();
			base.InitTroopGrid();
		}

		private void UpdateSlider(int sliderIndex, string descText, int capacity, int capacityNext, int capacityTotal)
		{
			SliderControl sliderControl = this.sliders[sliderIndex];
			sliderControl.DescLabel.Text = descText;
			sliderControl.CurrentLabel.Text = this.lang.ThousandsSeparated(capacity);
			sliderControl.NextLabel.Text = this.lang.Get("PLUS", new object[]
			{
				this.lang.ThousandsSeparated(capacityNext - capacity)
			});
			sliderControl.CurrentSlider.Value = ((capacityTotal != 0) ? ((float)capacity / (float)capacityTotal) : 0f);
			sliderControl.NextSlider.Value = ((capacityTotal != 0) ? ((float)capacityNext / (float)capacityTotal) : 0f);
		}

		private void UpdateCapacity(int sliderIndex)
		{
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			int storage = this.buildingInfo.Storage;
			int storage2 = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo).Storage;
			int storage3 = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup).Storage;
			this.UpdateSlider(sliderIndex, this.lang.Get("UPGRADE_FIELD_CAPACITY", new object[0]), storage, storage2, storage3);
		}

		protected override void UpdateReputation(int sliderIndex)
		{
			Inventory inventory = Service.CurrentPlayer.Inventory;
			if (!inventory.HasItem("reputation"))
			{
				this.sliders[sliderIndex].HideAll();
				Service.Logger.WarnFormat("No reputation found in your inventory", new object[0]);
				return;
			}
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			int itemCapacity = inventory.GetItemCapacity("reputation");
			int reputationCapacityForLevel = GameUtils.GetReputationCapacityForLevel(buildingUpgradeCatalog.GetNextLevel(this.buildingInfo).Lvl);
			int reputationCapacityForLevel2 = GameUtils.GetReputationCapacityForLevel(buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup).Lvl);
			this.UpdateSlider(sliderIndex, this.lang.Get("BUILDING_REPUTATION", new object[0]), itemCapacity, reputationCapacityForLevel, reputationCapacityForLevel2);
		}
	}
}
