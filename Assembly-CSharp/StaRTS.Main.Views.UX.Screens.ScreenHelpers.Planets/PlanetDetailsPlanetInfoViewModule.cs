using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsPlanetInfoViewModule : AbstractPlanetDetailsViewModule
	{
		private const string PLANET_INFO_BUTTON = "BtnActionRewards";

		private const string PLANET_INFO_LABEL = "LabelBtnActionRewards";

		private const string PLANET_NAME_LABEL = "LabelPlanetName";

		private const string NEW_LOOT_BADGE = "ContainerJewelRewards";

		private const string NEW_LOOT_BADGE_LABEL = "LabelJewelRewards";

		private const string PLANET_INFO_STRING = "PLANET_PLAYSCREEN_INFO";

		private const string NEW_BADGE_STRING = "HQ_BADGE";

		private UXButton planetInfoButton;

		private UXLabel planetInfoLabel;

		private UXLabel planetNameLabel;

		private UXElement newBadge;

		private string currentFeaturedLootHash;

		private PlanetLootTableScreen lootScreen;

		public PlanetDetailsPlanetInfoViewModule(PlanetDetailsScreen screen) : base(screen)
		{
		}

		public void OnScreenLoaded()
		{
			this.planetInfoButton = this.screen.GetElement<UXButton>("BtnActionRewards");
			this.planetInfoButton.OnClicked = new UXButtonClickedDelegate(this.OnPlanetInfoButtonClicked);
			this.planetInfoButton.Enabled = true;
			this.planetNameLabel = this.screen.GetElement<UXLabel>("LabelPlanetName");
			this.planetInfoLabel = this.screen.GetElement<UXLabel>("LabelBtnActionRewards");
			this.planetInfoLabel.Text = base.LangController.Get("PLANET_PLAYSCREEN_INFO", new object[0]);
			this.newBadge = this.screen.GetElement<UXElement>("ContainerJewelRewards");
			this.newBadge.Visible = false;
			UXLabel element = this.screen.GetElement<UXLabel>("LabelJewelRewards");
			element.Text = base.LangController.Get("HQ_BADGE", new object[0]);
			this.RefreshScreenForPlanetChange();
		}

		public void RefreshScreenForPlanetChange()
		{
			this.planetNameLabel.Text = base.LangController.Get(this.screen.viewingPlanetVO.LoadingScreenText, new object[0]);
			this.currentFeaturedLootHash = Service.InventoryCrateRewardController.CalculatePlanetRewardChecksum(this.screen.CurrentPlanet);
			this.newBadge.Visible = this.ShouldBadgeForNewLoot();
			if (this.lootScreen != null)
			{
				this.lootScreen.RefreshWithNewPlanet(this.screen.CurrentPlanet);
				if (this.newBadge.Visible)
				{
					this.SaveLastFeaturedLootHash();
				}
			}
		}

		private void CleanUpLootScreen()
		{
			if (this.lootScreen != null)
			{
				this.lootScreen.OnModalResult = null;
				this.lootScreen.CloseNoTransition(null);
				this.lootScreen = null;
			}
		}

		public void Destroy()
		{
			this.CleanUpLootScreen();
		}

		public void OnClose()
		{
			this.CleanUpLootScreen();
		}

		private void OnPlanetInfoButtonClicked(UXButton button)
		{
			button.Enabled = false;
			Service.EventManager.SendEvent(EventId.LootTableButtonTapped, null);
			Planet currentPlanet = this.screen.CurrentPlanet;
			this.lootScreen = new PlanetLootTableScreen(currentPlanet);
			this.lootScreen.OnModalResult = new OnScreenModalResult(this.OnScreenClosed);
			Service.ScreenController.AddScreen(this.lootScreen, false);
			if (this.newBadge.Visible)
			{
				this.SaveLastFeaturedLootHash();
			}
			this.screen.ShowPlanetInfoUI();
			this.screen.UpdateCurrentPlanet(this.screen.CurrentPlanet);
			Service.GalaxyViewController.SwitchToObjectiveDetails(false);
		}

		private void OnScreenClosed(object result, object cookie)
		{
			if (this.planetInfoButton != null)
			{
				this.planetInfoButton.Enabled = true;
			}
			if (this.lootScreen != null)
			{
				this.lootScreen = null;
				Service.GalaxyPlanetController.UpdateEventEffectStatus(this.screen.CurrentPlanet);
				this.screen.HidePlanetInfoUI();
				Service.GalaxyViewController.SwitchFromObjectiveDetails();
			}
			else
			{
				this.screen.HidePlanetInfoUI();
			}
			this.newBadge.Visible = false;
		}

		private void SaveLastFeaturedLootHash()
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			string prefName = "plh_" + this.screen.CurrentPlanet.VO.Uid;
			sharedPlayerPrefs.SetPref(prefName, this.currentFeaturedLootHash);
		}

		private bool ShouldBadgeForNewLoot()
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			string prefName = "plh_" + this.screen.CurrentPlanet.VO.Uid;
			string pref = sharedPlayerPrefs.GetPref<string>(prefName);
			return pref != this.currentFeaturedLootHash;
		}
	}
}
