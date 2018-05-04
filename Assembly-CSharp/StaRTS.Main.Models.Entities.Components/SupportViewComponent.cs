using Net.RichardLord.Ash.Core;
using StaRTS.Main.Views;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class SupportViewComponent : ComponentBase
	{
		private BuildingTooltip buildingTooltip;

		private TooltipHelper tooltipHelper;

		public SupportViewComponentState State
		{
			get;
			private set;
		}

		public BuildingTooltip BuildingTooltip
		{
			get
			{
				return this.buildingTooltip;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.tooltipHelper.Enabled;
			}
		}

		public SupportViewComponent()
		{
			this.buildingTooltip = null;
			this.State = SupportViewComponentState.Dormant;
			this.tooltipHelper = new TooltipHelper();
		}

		public void SetupElements(BuildingTooltip buildingTooltip, SupportViewComponentState s)
		{
			if (this.State != SupportViewComponentState.Dormant)
			{
				return;
			}
			this.buildingTooltip = buildingTooltip;
			this.State = s;
			this.tooltipHelper.SetupElements(this.Entity.Get<GameObjectViewComponent>(), buildingTooltip.TooltipElement, 0f, true, false);
		}

		public void SetEnabled(bool enable)
		{
			this.tooltipHelper.Enabled = enable;
		}

		public void UpdateLocation()
		{
			if (this.State == SupportViewComponentState.Dormant)
			{
				return;
			}
			this.tooltipHelper.UpdateLocation(false);
		}

		public void UpdateSelected(bool selected)
		{
			this.Refresh();
			if (this.buildingTooltip != null)
			{
				this.buildingTooltip.SetSelected(selected);
			}
		}

		public void Refresh()
		{
			this.TeardownElements();
			Service.BuildingTooltipController.EnsureBuildingTooltip((SmartEntity)this.Entity);
		}

		public void UpdateTime(int timeLeft, int timeTotal, bool updateProgress)
		{
			if (this.buildingTooltip != null)
			{
				this.buildingTooltip.SetTime(timeLeft);
				if (updateProgress)
				{
					this.buildingTooltip.SetProgress(timeLeft, timeTotal);
				}
			}
		}

		public void TeardownElements()
		{
			if (this.buildingTooltip != null)
			{
				this.buildingTooltip.DestroyTooltip();
				this.buildingTooltip = null;
			}
			this.State = SupportViewComponentState.Dormant;
			this.tooltipHelper.TeardownElements(false);
		}

		public override void OnRemove()
		{
			this.TeardownElements();
		}
	}
}
