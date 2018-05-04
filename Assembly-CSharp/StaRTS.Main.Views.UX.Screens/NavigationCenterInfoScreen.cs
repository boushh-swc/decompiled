using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class NavigationCenterInfoScreen : BuildingInfoScreen
	{
		private const string PERK_EFFECT_RELOCATION = "PerkEffectRelocation";

		private UXGrid planetGrid;

		private UXLabel labelUnlockPlanetTimer;

		private UXButton buttonTutorialConfirm;

		public NavigationCenterInfoScreen(Entity selectedBuilding) : base(selectedBuilding)
		{
			this.buildingInfo = selectedBuilding.Get<BuildingComponent>().BuildingType;
		}

		protected override void InitGroups()
		{
			base.InitGroups();
			base.GetElement<UXElement>("NavigationCenter").Visible = true;
			base.GetElement<UXElement>("SelectPlanet").Visible = false;
			base.GetElement<UXElement>("Info").Visible = false;
			if (Service.PerkManager.IsPerkAppliedToBuilding(this.buildingInfo))
			{
				base.GetElement<UXElement>("PerkEffectRelocation").Visible = true;
			}
		}

		protected override void InitLabels()
		{
			base.InitLabels();
			base.GetElement<UXElement>("LabelInfo").Visible = false;
			base.GetElement<UXElement>("LowLayoutGroup").Visible = true;
			base.GetElement<UXLabel>("LabelUpgradeUnlockPlanet").Text = string.Empty;
			base.GetElement<UXElement>("UpgradeTime").Visible = false;
			base.GetElement<UXLabel>("LabelInfoBottom").Text = LangUtils.GetBuildingDescription(this.buildingInfo);
			this.labelHQUpgradeDesc.Visible = false;
			this.labelViewGalaxyMap.Text = this.lang.Get("s_ViewGalaxyMap", new object[0]);
			this.labelUnlockPlanetTimer = base.GetElement<UXLabel>("LabelUnlockPlanetTimer");
			this.labelUnlockPlanetTimer.Text = string.Empty;
			this.labelUnlockPlanetTimer.Visible = false;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.HandleClose);
			base.GetElement<UXButton>("BuildingImage").Visible = false;
			this.buttonTutorialConfirm = base.GetElement<UXButton>("ButtonTutorialConfirm");
			this.buttonTutorialConfirm.Visible = false;
			bool enabled = Service.BuildingLookupController.HasNavigationCenter();
			this.buttonViewGalaxyMap.Enabled = enabled;
			this.buttonViewGalaxyMap.OnClicked = new UXButtonClickedDelegate(this.ViewGalaxyMapClicked);
		}

		protected void ViewGalaxyMapClicked(UXButton button)
		{
			base.CloseNoTransition(null);
			GameUtils.ExitEditState();
			Service.GalaxyViewController.GoToGalaxyView();
			Service.EventManager.SendEvent(EventId.GalaxyOpenByInfoScreen, null);
		}

		protected override void InitImages()
		{
			IGeometryVO imageGeometryConfig = this.GetImageGeometryConfig();
			ProjectorConfig config = ProjectorUtils.GenerateBuildingConfig(imageGeometryConfig as BuildingTypeVO, base.GetElement<UXSprite>("BldgImageBottomFrame"));
			this.projector = ProjectorUtils.GenerateProjector(config);
			base.GetElement<UXSprite>("SpriteSquadSymbol").Visible = false;
		}

		protected override void OnLoaded()
		{
			base.InitControls(2);
			this.InitHitpoints(3);
			this.InitPlanetSlider(4);
			this.InitRelocationStarSlider(5);
			this.InitUnlockedPlanets();
		}

		private void InitUnlockedPlanets()
		{
			this.planetGrid = base.GetElement<UXGrid>("TopUnlockItemsGrid");
			this.planetGrid.SetTemplateItem("TopUnlockItemsTemplate");
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			PlanetVO planet = currentPlayer.Map.Planet;
			StaticDataController staticDataController = Service.StaticDataController;
			int i = 0;
			int count = currentPlayer.UnlockedPlanets.Count;
			while (i < count)
			{
				string uid = currentPlayer.UnlockedPlanets[i];
				PlanetVO planetVO = staticDataController.Get<PlanetVO>(uid);
				UXElement item = this.planetGrid.CloneTemplateItem(planetVO.Uid);
				UXLabel subElement = this.planetGrid.GetSubElement<UXLabel>(planetVO.Uid, "LabelTopUnlockItemsCurrent");
				if (planetVO.Uid != planet.Uid)
				{
					subElement.Visible = false;
				}
				else
				{
					subElement.Text = Service.Lang.Get("s_Current", new object[0]);
				}
				UXTexture subElement2 = this.planetGrid.GetSubElement<UXTexture>(planetVO.Uid, "TextureTopUnlockItemsImage");
				subElement2.LoadTexture("PlanetEnvIcon-" + planetVO.Abbreviation);
				UXSprite subElement3 = this.planetGrid.GetSubElement<UXSprite>(planetVO.Uid, "SpriteTopUnlockItemsLock");
				subElement3.Visible = false;
				UXLabel subElement4 = this.planetGrid.GetSubElement<UXLabel>(planetVO.Uid, "LabelTopUnlockItemsLock");
				subElement4.Visible = false;
				UXLabel subElement5 = this.planetGrid.GetSubElement<UXLabel>(planetVO.Uid, "LabelTopUnlockItemsName");
				subElement5.Text = LangUtils.GetPlanetDisplayName(planetVO);
				this.planetGrid.AddItem(item, i);
				UXButton subElement6 = this.planetGrid.GetSubElement<UXButton>(planetVO.Uid, "TopUnlockItemsCard");
				subElement6.Enabled = false;
				subElement6.VisuallyEnableButton();
				i++;
			}
			if (this.nextBuildingInfo != null)
			{
				for (int j = this.nextBuildingInfo.Lvl; j <= this.maxBuildingInfo.Lvl; j++)
				{
					string itemUid = "NavCenter" + j + "Required";
					UXElement item2 = this.planetGrid.CloneTemplateItem(itemUid);
					UXLabel subElement5 = this.planetGrid.GetSubElement<UXLabel>(itemUid, "LabelTopUnlockItemsName");
					subElement5.Visible = false;
					UXLabel subElement = this.planetGrid.GetSubElement<UXLabel>(itemUid, "LabelTopUnlockItemsCurrent");
					subElement.Visible = false;
					string text = this.lang.Get("PLANETS_GNC_UPGRADE_REQUIRED", new object[]
					{
						j
					});
					UXLabel subElement4 = this.planetGrid.GetSubElement<UXLabel>(itemUid, "LabelTopUnlockItemsLock");
					subElement4.Text = text;
					this.planetGrid.AddItem(item2, j);
				}
			}
			this.planetGrid.RepositionItems();
			this.planetGrid.IsScrollable = true;
			this.planetGrid.ScrollToItem(0);
		}

		protected virtual void InitPlanetSlider(int sliderIndex)
		{
			float num = (float)this.maxBuildingInfo.Lvl + 1f;
			SliderControl sliderControl = this.sliders[sliderIndex];
			sliderControl.DescLabel.Visible = true;
			sliderControl.DescLabel.Text = this.lang.Get("PLANETS_GNC_SLOTS", new object[0]);
			sliderControl.NextLabel.Visible = false;
			sliderControl.NextSlider.Visible = false;
			sliderControl.CurrentSlider.Visible = true;
			sliderControl.CurrentLabel.Visible = true;
			sliderControl.CurrentSlider.Value = (float)(this.buildingInfo.Lvl + 1) / num;
			sliderControl.CurrentLabel.Text = this.buildingInfo.Lvl + 1 + "/" + num;
			sliderControl.Background.Visible = true;
		}

		protected virtual void InitRelocationStarSlider(int sliderIndex)
		{
			SliderControl sliderControl = this.sliders[sliderIndex];
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			sliderControl.DescLabel.Visible = true;
			sliderControl.NextLabel.Visible = false;
			sliderControl.NextSlider.Visible = false;
			sliderControl.CurrentSlider.Visible = true;
			sliderControl.Background.Visible = true;
			int displayRelocationStarsCount = currentPlayer.GetDisplayRelocationStarsCount();
			int requiredRelocationStars = currentPlayer.GetRequiredRelocationStars();
			if (displayRelocationStarsCount < requiredRelocationStars)
			{
				sliderControl.CurrentLabel.Visible = true;
				float num = (float)currentPlayer.GetRequiredRelocationStars();
				sliderControl.CurrentSlider.Value = ((num > 0f) ? ((float)displayRelocationStarsCount / num) : 1f);
				sliderControl.CurrentLabel.Text = displayRelocationStarsCount + "/" + currentPlayer.GetRequiredRelocationStars();
				sliderControl.DescLabel.Text = this.lang.Get("PLANETS_GNC_RELOCATION_STATUS", new object[0]);
				base.GetElement<UXSprite>("SpritepBar3MasteryStar").Visible = true;
			}
			else
			{
				sliderControl.CurrentSlider.Value = 1f;
				sliderControl.CurrentLabel.Enabled = false;
				sliderControl.DescLabel.Text = this.lang.Get("PLANETS_GNC_RELOCATION_STATUS_AVAILABLE", new object[0]);
				base.GetElement<UXSprite>("SpritepBar3MasteryStar").Visible = false;
			}
		}

		public override void OnDestroyElement()
		{
			if (this.planetGrid != null)
			{
				this.planetGrid.Clear();
				this.planetGrid = null;
			}
			this.buttonViewGalaxyMap.OnClicked = null;
			base.OnDestroyElement();
		}
	}
}
