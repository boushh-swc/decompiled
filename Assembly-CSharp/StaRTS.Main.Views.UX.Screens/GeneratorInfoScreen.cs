using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class GeneratorInfoScreen : BuildingInfoScreen
	{
		private const string PERK_EFFECT = "PerkEffectResource";

		protected const int GENERATOR_SLIDER_HITPOINTS = 0;

		protected const int GENERATOR_SLIDER_PRODUCTION = 1;

		protected const int GENERATOR_SLIDER_CAPACITY = 2;

		protected const int GENERATOR_SLIDER_COUNT = 3;

		private ICurrencyController currencyController;

		public GeneratorInfoScreen(Entity generatorBuilding) : base(generatorBuilding)
		{
			this.currencyController = Service.ICurrencyController;
		}

		protected override void OnLoaded()
		{
			base.InitControls(3);
			this.InitHitpoints(0);
			this.sliders[1].DescLabel.Text = this.lang.Get("PRODUCTION_RATE", new object[0]);
			this.UpdateProductionRate(1);
			this.sliders[2].DescLabel.Text = this.lang.Get("GENERATOR_CAPACITY", new object[0]);
			this.UpdateCurrentAmount(2);
			if (!this.observingClockViewTime && !this.useUpgradeGroup)
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				this.observingClockViewTime = true;
			}
			if (Service.PerkManager.IsPerkAppliedToBuilding(this.buildingInfo))
			{
				base.GetElement<UXElement>("PerkEffectResource").Visible = true;
			}
		}

		protected void UpdateProductionRate(int sliderIndex)
		{
			int num;
			if (this.useUpgradeGroup)
			{
				num = this.currencyController.CurrencyPerHour(this.buildingInfo);
			}
			else
			{
				num = ResourceGenerationPerkUtils.GetCurrentCurrencyGenerationRate(this.buildingInfo, Service.PerkManager.GetPlayerActivePerks());
			}
			int num2 = this.currencyController.CurrencyPerHour(this.maxBuildingInfo);
			this.sliders[sliderIndex].CurrentSlider.Value = ((num2 != 0) ? ((float)num / (float)num2) : 0f);
			if (this.useUpgradeGroup)
			{
				int num3 = this.currencyController.CurrencyPerHour(this.nextBuildingInfo);
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

		private void UpdateCurrentAmount(int sliderIndex)
		{
			BuildingComponent buildingComponent = this.selectedBuilding.Get<BuildingComponent>();
			int accruedCurrency = buildingComponent.BuildingTO.AccruedCurrency;
			int storage = this.buildingInfo.Storage;
			UXLabel currentLabel = this.sliders[sliderIndex].CurrentLabel;
			currentLabel.Text = this.lang.Get("FRACTION", new object[]
			{
				this.lang.ThousandsSeparated(accruedCurrency),
				this.lang.ThousandsSeparated(storage)
			});
			UXSlider currentSlider = this.sliders[sliderIndex].CurrentSlider;
			float num = (storage != 0) ? ((float)accruedCurrency / (float)storage) : 0f;
			currentSlider.Value = num;
			this.projector.Config.MeterValue = num;
		}

		public override void OnViewClockTime(float dt)
		{
			this.UpdateCurrentAmount(2);
			if (Service.PostBattleRepairController.IsEntityInRepair(this.selectedBuilding))
			{
				this.UpdateHitpoints();
			}
		}
	}
}
