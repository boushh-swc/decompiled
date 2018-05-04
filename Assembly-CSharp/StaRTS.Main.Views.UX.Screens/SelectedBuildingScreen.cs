using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SelectedBuildingScreen : ClosableScreen, IEventObserver
	{
		protected const string BUILDING_PERKS_TITLE_GROUP = "TitleGroupPerks";

		protected const string BUILDING_TITLE_GROUP = "TitleGroup";

		protected const string BUILDING_PERKS_BUTTON = "btnPerks";

		protected const string FRAGMENT_ICON = "SpriteIconFragmentUpgrade";

		protected SmartEntity selectedBuilding;

		protected BuildingTypeVO buildingInfo;

		protected UXSprite fragmentSprite;

		protected SelectedBuildingScreen(string assetName, SmartEntity selectedBuilding) : base(assetName)
		{
			this.SetSelectedBuilding(selectedBuilding);
		}

		protected override void OnScreenLoaded()
		{
			this.fragmentSprite = base.GetElement<UXSprite>("SpriteIconFragmentUpgrade");
			this.fragmentSprite.Visible = false;
			base.OnScreenLoaded();
		}

		protected void SetupFragmentSprite(int quality)
		{
			UXUtils.SetupFragmentIconSprite(this.fragmentSprite, quality);
		}

		protected override void InitButtons()
		{
			Service.EventManager.RegisterObserver(this, EventId.ActivePerksUpdated, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.EntityDestroyed, EventPriority.Default);
			base.InitButtons();
			this.SetupPerksButton();
		}

		protected virtual void SetupPerksButton()
		{
			UXElement element = base.GetElement<UXElement>("TitleGroup");
			element.Visible = true;
			UXElement element2 = base.GetElement<UXElement>("TitleGroupPerks");
			element2.Visible = false;
			UXButton element3 = base.GetElement<UXButton>("btnPerks");
			element3.Tag = this.buildingInfo;
			element3.OnClicked = new UXButtonClickedDelegate(this.OnPerksButtonClicked);
			if (Service.PerkManager.IsPerkAppliedToBuilding(this.buildingInfo))
			{
				element2.Visible = true;
				element.Visible = false;
			}
		}

		protected void OnPerksButtonClicked(UXButton button)
		{
			Service.PerkViewController.ShowActivePerksScreen((BuildingTypeVO)button.Tag);
		}

		public override void OnDestroyElement()
		{
			this.UnregisterEvents();
			base.OnDestroyElement();
		}

		private void UnregisterEvents()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ActivePerksUpdated);
			Service.EventManager.UnregisterObserver(this, EventId.EntityDestroyed);
		}

		protected virtual void SetSelectedBuilding(SmartEntity newSelectedBuilding)
		{
			this.selectedBuilding = newSelectedBuilding;
			this.buildingInfo = ((this.selectedBuilding != null) ? this.selectedBuilding.BuildingComp.BuildingType : null);
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.EntityDestroyed)
			{
				if (id == EventId.ActivePerksUpdated)
				{
					this.SetupPerksButton();
				}
			}
			else if (this.selectedBuilding != null && this.selectedBuilding.ID == (uint)cookie)
			{
				this.UnregisterEvents();
				this.selectedBuilding = Service.BuildingController.SelectedBuilding;
				base.CloseNoTransition(null);
			}
			return base.OnEvent(id, cookie);
		}
	}
}
