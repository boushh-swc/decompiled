using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class StarportInfoScreen : BuildingInfoScreen, IEventObserver
	{
		protected const int STARPORT_SLIDER_HITPOINTS = 0;

		protected const int STARPORT_SLIDER_CAPACITY = 1;

		protected const int STARPORT_SLIDER_COUNT = 2;

		public StarportInfoScreen(Entity starportBuilding) : base(starportBuilding)
		{
			this.useStorageGroup = true;
		}

		protected override void InitLabels()
		{
			base.InitLabels();
			this.labelStorage.Text = this.lang.Get("ALL_TROOPS", new object[0]);
		}

		protected override void OnLoaded()
		{
			base.InitControls(2);
			this.InitHitpoints(0);
			this.sliders[1].DescLabel.Text = this.lang.Get("STARPORT_CAPACITY", new object[0]);
			this.UpdateHousingSpace();
			this.SetupTroopItemGrid();
			Service.EventManager.RegisterObserver(this, EventId.InventoryTroopUpdated, EventPriority.Default);
		}

		public override void OnDestroyElement()
		{
			Service.EventManager.UnregisterObserver(this, EventId.InventoryTroopUpdated);
			base.OnDestroyElement();
		}

		private void UpdateHousingSpace()
		{
			int num;
			int num2;
			GameUtils.GetStarportTroopCounts(out num, out num2);
			UXLabel currentLabel = this.sliders[1].CurrentLabel;
			currentLabel.Text = this.lang.Get("FRACTION", new object[]
			{
				this.lang.ThousandsSeparated(num),
				this.lang.ThousandsSeparated(num2)
			});
			UXSlider currentSlider = this.sliders[1].CurrentSlider;
			float num3 = (num2 != 0) ? ((float)num / (float)num2) : 0f;
			currentSlider.Value = num3;
			this.projector.Config.MeterValue = num3;
		}

		protected void SetupTroopItemGrid()
		{
			base.InitGrid();
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (TroopTypeVO current in staticDataController.GetAll<TroopTypeVO>())
			{
				int worldOwnerTroopCount = GameUtils.GetWorldOwnerTroopCount(current.Uid);
				if (worldOwnerTroopCount > 0)
				{
					base.AddTroopItem(current, worldOwnerTroopCount, null);
				}
			}
			base.RepositionGridItems();
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.InventoryTroopUpdated)
			{
				this.UpdateHousingSpace();
				this.SetupTroopItemGrid();
			}
			return base.OnEvent(id, cookie);
		}
	}
}
