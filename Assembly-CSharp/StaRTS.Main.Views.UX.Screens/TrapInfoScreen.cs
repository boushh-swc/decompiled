using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TrapInfoScreen : BuildingInfoScreen
	{
		private const int TRAP_SLIDER_DPS = 0;

		private const int TRAP_SLIDER_RADIUS = 1;

		private const int TRAP_SLIDER_COUNT = 2;

		private TrapTypeVO trapVO;

		private TrapTypeVO nextTrapVO;

		private TrapTypeVO maxTrapVO;

		public TrapInfoScreen(SmartEntity trap, bool upgradeGroup) : base(trap, upgradeGroup)
		{
		}

		protected override void OnLoaded()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			this.trapVO = staticDataController.Get<TrapTypeVO>(this.buildingInfo.TrapUid);
			if (this.useUpgradeGroup && this.nextBuildingInfo != null)
			{
				this.nextTrapVO = staticDataController.Get<TrapTypeVO>(this.nextBuildingInfo.TrapUid);
			}
			if (this.maxBuildingInfo != null)
			{
				this.maxTrapVO = staticDataController.Get<TrapTypeVO>(this.maxBuildingInfo.TrapUid);
			}
			base.InitControls(2);
			this.UpdateDps();
			this.UpdateRadius();
		}

		private void UpdateDps()
		{
			SliderControl sliderControl = this.sliders[0];
			TrapController trapController = Service.TrapController;
			int trapDamageForUIDisplay = trapController.GetTrapDamageForUIDisplay(this.trapVO);
			int trapDamageForUIDisplay2 = trapController.GetTrapDamageForUIDisplay(this.maxTrapVO);
			sliderControl.DescLabel.Text = this.lang.Get("DAMAGE_DPS", new object[]
			{
				string.Empty
			});
			sliderControl.CurrentLabel.Text = this.lang.ThousandsSeparated(trapDamageForUIDisplay);
			sliderControl.CurrentSlider.Value = ((trapDamageForUIDisplay2 != 0) ? ((float)trapDamageForUIDisplay / (float)trapDamageForUIDisplay2) : 0f);
			if (this.useUpgradeGroup && this.nextTrapVO != null)
			{
				int trapDamageForUIDisplay3 = trapController.GetTrapDamageForUIDisplay(this.nextTrapVO);
				sliderControl.NextLabel.Text = this.lang.Get("PLUS", new object[]
				{
					this.lang.ThousandsSeparated(trapDamageForUIDisplay3 - trapDamageForUIDisplay)
				});
				sliderControl.NextSlider.Value = ((trapDamageForUIDisplay2 != 0) ? ((float)trapDamageForUIDisplay3 / (float)trapDamageForUIDisplay2) : 0f);
			}
		}

		private void UpdateRadius()
		{
			int trapMaxRadius = (int)TrapUtils.GetTrapMaxRadius(this.trapVO);
			int trapMaxRadius2 = (int)TrapUtils.GetTrapMaxRadius(this.maxTrapVO);
			SliderControl sliderControl = this.sliders[1];
			sliderControl.DescLabel.Text = this.lang.Get("TRAP_TRIGGER_RADIUS", new object[0]);
			sliderControl.CurrentLabel.Text = this.lang.ThousandsSeparated(trapMaxRadius);
			sliderControl.CurrentSlider.Value = ((trapMaxRadius2 != 0) ? ((float)trapMaxRadius / (float)trapMaxRadius2) : 0f);
			if (this.useUpgradeGroup && this.nextTrapVO != null)
			{
				int trapMaxRadius3 = (int)TrapUtils.GetTrapMaxRadius(this.maxTrapVO);
				sliderControl.NextLabel.Text = this.lang.Get("PLUS", new object[]
				{
					this.lang.ThousandsSeparated(trapMaxRadius3 - trapMaxRadius)
				});
				sliderControl.NextSlider.Value = ((trapMaxRadius2 != 0) ? ((float)trapMaxRadius3 / (float)trapMaxRadius2) : 0f);
			}
		}
	}
}
