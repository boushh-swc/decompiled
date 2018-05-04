using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class GeneratorUpgradeScreen : GeneratorInfoScreen
	{
		public GeneratorUpgradeScreen(Entity selectedBuilding) : base(selectedBuilding)
		{
			this.useUpgradeGroup = true;
		}

		protected override void OnLoaded()
		{
			base.InitControls(3);
			this.InitHitpoints(0);
			string currencyStringId = LangUtils.GetCurrencyStringId(this.buildingInfo.Currency);
			this.sliders[1].DescLabel.Text = this.lang.Get("PRODUCTION_PER_HOUR", new object[]
			{
				this.lang.Get(currencyStringId, new object[0])
			});
			base.UpdateProductionRate(1);
			this.UpdateCapacity(2);
		}

		private void UpdateCapacity(int sliderIndex)
		{
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			int storage = this.buildingInfo.Storage;
			int storage2 = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo).Storage;
			int storage3 = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup).Storage;
			this.sliders[sliderIndex].DescLabel.Text = this.lang.Get("UPGRADE_FIELD_STORAGE", new object[0]);
			this.sliders[sliderIndex].CurrentLabel.Text = this.lang.ThousandsSeparated(storage);
			this.sliders[sliderIndex].NextLabel.Text = this.lang.Get("PLUS", new object[]
			{
				this.lang.ThousandsSeparated(storage2 - storage)
			});
			this.sliders[sliderIndex].CurrentSlider.Value = ((storage3 != 0) ? ((float)storage / (float)storage3) : 0f);
			this.sliders[sliderIndex].NextSlider.Value = ((storage3 != 0) ? ((float)storage2 / (float)storage3) : 0f);
			BuildingComponent buildingComponent = this.selectedBuilding.Get<BuildingComponent>();
			int accruedCurrency = buildingComponent.BuildingTO.AccruedCurrency;
			int storage4 = this.buildingInfo.Storage;
			float meterValue = (storage4 != 0) ? ((float)accruedCurrency / (float)storage4) : 0f;
			this.projector.Config.MeterValue = meterValue;
		}
	}
}
