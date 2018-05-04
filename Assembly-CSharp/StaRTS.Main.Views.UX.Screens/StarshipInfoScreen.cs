using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class StarshipInfoScreen : BuildingInfoScreen, IEventObserver
	{
		private const int STARSHIP_SLIDER_HITPOINTS = 0;

		private const int STARSHIP_SLIDER_CAPACITY = 1;

		private const int STARSHIP_SLIDER_COUNT = 2;

		public StarshipInfoScreen(Entity starshipBuilding) : base(starshipBuilding)
		{
			this.useStorageGroup = true;
		}

		protected override void InitLabels()
		{
			base.InitLabels();
			this.labelStorage.Text = this.lang.Get("ALL_STARSHIPS", new object[0]);
		}

		protected override void OnLoaded()
		{
			base.InitControls(2);
			this.InitHitpoints(0);
			this.sliders[1].DescLabel.Text = this.lang.Get("MOBILIZATION_CAPACITY", new object[0]);
			Service.EventManager.RegisterObserver(this, EventId.InventoryTroopUpdated, EventPriority.Default);
		}

		public override void OnDestroyElement()
		{
			Service.EventManager.UnregisterObserver(this, EventId.InventoryTroopUpdated);
			base.OnDestroyElement();
		}

		public override void RefreshView()
		{
			if (!base.IsLoaded())
			{
				return;
			}
			this.UpdateHousingSpace();
			this.SetupTroopItemGrid();
		}

		private void UpdateHousingSpace()
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			int totalStorageAmount = worldOwner.Inventory.SpecialAttack.GetTotalStorageAmount();
			int storage = this.buildingInfo.Storage;
			UXLabel currentLabel = this.sliders[1].CurrentLabel;
			currentLabel.Text = this.lang.Get("FRACTION", new object[]
			{
				this.lang.ThousandsSeparated(totalStorageAmount),
				this.lang.ThousandsSeparated(storage)
			});
			UXSlider currentSlider = this.sliders[1].CurrentSlider;
			currentSlider.Value = ((storage != 0) ? ((float)totalStorageAmount / (float)storage) : 0f);
		}

		private void SetupTroopItemGrid()
		{
			base.InitGrid();
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (SpecialAttackTypeVO current in staticDataController.GetAll<SpecialAttackTypeVO>())
			{
				int worldOwnerSpecialAttackCount = GameUtils.GetWorldOwnerSpecialAttackCount(current.Uid);
				if (worldOwnerSpecialAttackCount > 0)
				{
					base.AddTroopItem(current, worldOwnerSpecialAttackCount, null);
				}
			}
			base.RepositionGridItems();
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.InventoryTroopUpdated)
			{
				this.RefreshView();
			}
			return base.OnEvent(id, cookie);
		}
	}
}
