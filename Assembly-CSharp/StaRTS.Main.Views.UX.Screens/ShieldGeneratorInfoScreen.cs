using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ShieldGeneratorInfoScreen : BuildingInfoScreen
	{
		private const int GENERATOR_SLIDER_HITPOINTS = 0;

		private const int GENERATOR_SLIDER_SHIELD_HEALTH = 1;

		private const int GENERATOR_SLIDER_SHIELD_RANGE = 2;

		private const int GENERATOR_SLIDER_COUNT = 3;

		public ShieldGeneratorInfoScreen(Entity generatorBuilding) : base(generatorBuilding)
		{
		}

		protected override void OnLoaded()
		{
			base.InitControls(3);
			this.InitHitpoints(0);
			this.sliders[1].DescLabel.Text = this.lang.Get("SHIELD_HEALTH", new object[0]);
			this.UpdateShieldHealth(1);
			this.sliders[2].DescLabel.Text = this.lang.Get("SHIELD_RANGE", new object[0]);
			this.UpdateShieldRange(2);
		}

		private void UpdateShieldHealth(int sliderIndex)
		{
			ShieldController shieldController = Service.ShieldController;
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			BuildingTypeVO maxLevel = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup);
			int num = shieldController.PointsToHealth[this.buildingInfo.ShieldHealthPoints];
			int num2 = shieldController.PointsToHealth[maxLevel.ShieldHealthPoints];
			this.sliders[sliderIndex].CurrentSlider.Value = ((num2 != 0) ? ((float)num / (float)num2) : 0f);
			if (this.useUpgradeGroup)
			{
				BuildingTypeVO nextLevel = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo);
				int num3 = shieldController.PointsToHealth[nextLevel.ShieldHealthPoints];
				this.sliders[sliderIndex].CurrentLabel.Text = this.lang.ThousandsSeparated(num);
				this.sliders[sliderIndex].NextLabel.Text = this.lang.Get("PLUS", new object[]
				{
					this.lang.ThousandsSeparated(num3 - num)
				});
				this.sliders[sliderIndex].NextSlider.Value = ((num2 != 0) ? ((float)num3 / (float)num2) : 0f);
			}
			else
			{
				this.sliders[sliderIndex].CurrentLabel.Text = this.lang.Get("FRACTION", new object[]
				{
					this.lang.ThousandsSeparated(num),
					this.lang.ThousandsSeparated(num2)
				});
			}
		}

		private void UpdateShieldRange(int sliderIndex)
		{
			ShieldController shieldController = Service.ShieldController;
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			BuildingTypeVO maxLevel = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup);
			int num = shieldController.PointsToRange[this.buildingInfo.ShieldRangePoints];
			int num2 = shieldController.PointsToRange[maxLevel.ShieldRangePoints];
			this.sliders[sliderIndex].CurrentSlider.Value = ((num2 != 0) ? ((float)num / (float)num2) : 0f);
			if (this.useUpgradeGroup)
			{
				BuildingTypeVO nextLevel = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo);
				int num3 = shieldController.PointsToRange[nextLevel.ShieldRangePoints];
				this.sliders[sliderIndex].CurrentLabel.Text = this.lang.ThousandsSeparated(num);
				this.sliders[sliderIndex].NextLabel.Text = this.lang.Get("PLUS", new object[]
				{
					this.lang.ThousandsSeparated(num3 - num)
				});
				this.sliders[sliderIndex].NextSlider.Value = ((num2 != 0) ? ((float)num3 / (float)num2) : 0f);
			}
			else
			{
				this.sliders[sliderIndex].CurrentLabel.Text = this.lang.Get("FRACTION", new object[]
				{
					this.lang.ThousandsSeparated(num),
					this.lang.ThousandsSeparated(num2)
				});
			}
		}
	}
}
