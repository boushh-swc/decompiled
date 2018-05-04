using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TrainingInfoScreen : BuildingInfoScreen, IEventObserver
	{
		protected const int TRAINING_SLIDER_HITPOINTS = 0;

		protected const int TRAINING_SLIDER_CAPACITY = 1;

		protected const int TRAINING_SLIDER_COUNT = 2;

		protected DeliveryType deliveryType;

		public TrainingInfoScreen(SmartEntity trainingBuilding) : base(trainingBuilding)
		{
		}

		protected override void SetSelectedBuilding(SmartEntity newSelectedBuilding)
		{
			base.SetSelectedBuilding(newSelectedBuilding);
			this.deliveryType = ContractUtils.GetTroopContractTypeByBuilding(this.buildingInfo);
		}

		protected override void OnLoaded()
		{
			base.InitControls(2);
			this.InitHitpoints(0);
			UXLabel descLabel = this.sliders[1].DescLabel;
			DeliveryType deliveryType = this.deliveryType;
			if (deliveryType != DeliveryType.Vehicle)
			{
				if (deliveryType != DeliveryType.Mercenary)
				{
					descLabel.Text = this.lang.Get("TRAINING_CAPACITY", new object[0]);
				}
				else
				{
					descLabel.Text = this.lang.Get("HIRE_CAPACITY", new object[0]);
				}
			}
			else
			{
				descLabel.Text = this.lang.Get("CONSTRUCTION_CAPACITY", new object[0]);
			}
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
		}

		private void UpdateHousingSpace()
		{
			int num = ContractUtils.CalculateSpaceOccupiedByQueuedTroops(this.selectedBuilding);
			int storage = this.buildingInfo.Storage;
			UXLabel currentLabel = this.sliders[1].CurrentLabel;
			currentLabel.Text = this.lang.Get("FRACTION", new object[]
			{
				this.lang.ThousandsSeparated(num),
				this.lang.ThousandsSeparated(storage)
			});
			UXSlider currentSlider = this.sliders[1].CurrentSlider;
			currentSlider.Value = ((storage != 0) ? ((float)num / (float)storage) : 0f);
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
