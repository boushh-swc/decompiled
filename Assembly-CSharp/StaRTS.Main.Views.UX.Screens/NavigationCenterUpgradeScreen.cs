using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Contracts;
using StaRTS.Main.Models.Commands.Player.Building.Upgrade;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class NavigationCenterUpgradeScreen : BuildingInfoScreen, IViewFrameTimeObserver
	{
		protected PlanetVO selectedPlanet;

		protected bool tutorialMode;

		protected UXLabel labelTutorialConfirm;

		protected OnScreenModalResult callback;

		protected UXLabel labelUnlockPlanetTimer;

		protected UXButton buttonTutorialConfirm;

		private List<UXCheckbox> unlockablePlanetList = new List<UXCheckbox>();

		public NavigationCenterUpgradeScreen(Entity selectedBuilding) : base(selectedBuilding)
		{
			this.tutorialMode = false;
			this.useUpgradeGroup = true;
		}

		public NavigationCenterUpgradeScreen(Entity selectedBuilding, BuildingTypeVO buildingTypeVO, OnScreenModalResult callback) : base(selectedBuilding)
		{
			this.buildingInfo = buildingTypeVO;
			this.callback = callback;
			this.tutorialMode = true;
			this.useUpgradeGroup = false;
		}

		protected override void InitGroups()
		{
			base.InitGroups();
			base.GetElement<UXElement>("NavigationCenter").Visible = true;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		protected override void InitLabels()
		{
			base.InitLabels();
			this.labelViewGalaxyMap.Text = this.lang.Get("s_ViewGalaxyMap", new object[0]);
			base.GetElement<UXElement>("LabelInfo").Visible = false;
			base.GetElement<UXElement>("LowLayoutGroup").Visible = false;
			base.GetElement<UXLabel>("LabelUpgradeUnlockPlanet").Text = string.Empty;
			this.labelUpgradeUnlockPlanet = base.GetElement<UXLabel>("LabelUpgradeUnlockPlanet");
			if (this.tutorialMode)
			{
				base.GetElement<UXLabel>("LabelSelectPlanet").Text = this.lang.Get("PLANETS_GNC_SELECT_UNLOCK", new object[0]);
			}
			else if (this.reqMet)
			{
				base.GetElement<UXLabel>("LabelSelectPlanet").Text = this.lang.Get("PLANETS_GNC_SELECT_UPGRADE", new object[0]);
			}
			else
			{
				base.GetElement<UXLabel>("LabelSelectPlanet").Text = this.lang.Get("PLANETS_GNC_HQ_UPGRADE_REQUIRED", new object[]
				{
					this.reqBuildingInfo.Lvl
				});
				this.labelHQUpgradeDesc.Visible = false;
			}
			base.GetElement<UXLabel>("LabelUpgradeTime").Text = GameUtils.GetTimeLabelFromSeconds((!this.tutorialMode) ? this.nextBuildingInfo.Time : this.buildingInfo.Time);
			base.GetElement<UXElement>("UpgradeTime").Visible = true;
			base.GetElement<UXLabel>("LabelUpgrade").Text = this.lang.Get("PLANETS_GNC_CONSTR_TIME", new object[0]);
			this.buttonPrimaryAction.Enabled = false;
			base.GetElement<UXLabel>("CostLabelConfirm").Text = this.lang.Get("PLANETS_GNC_UNLOCK_MODAL_BUTTON_CONFIRM", new object[0]);
			this.labelUnlockPlanetTimer = base.GetElement<UXLabel>("LabelUnlockPlanetTimer");
			this.labelUnlockPlanetTimer.Text = string.Empty;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.HandleClose);
			this.buttonTutorialConfirm = base.GetElement<UXButton>("ButtonTutorialConfirm");
			this.labelTutorialConfirm = base.GetElement<UXLabel>("CostLabelConfirm");
			if (this.tutorialMode)
			{
				this.buttonTutorialConfirm.OnClicked = new UXButtonClickedDelegate(this.OnTutorialConfirmClicked);
				this.buttonTutorialConfirm.Enabled = false;
				this.labelTutorialConfirm.TextColor = UXUtils.COLOR_LABEL_DISABLED;
				this.buttonViewGalaxyMap.Enabled = false;
			}
			else
			{
				this.buttonTutorialConfirm.Visible = false;
				this.buttonTutorialConfirm.Enabled = false;
				this.buttonViewGalaxyMap.Enabled = true;
				this.buttonViewGalaxyMap.OnClicked = new UXButtonClickedDelegate(this.ViewGalaxyMapClicked);
			}
			this.buttonInstantBuy.Enabled = false;
		}

		protected void ViewGalaxyMapClicked(UXButton button)
		{
			base.CloseNoTransition(null);
			GameUtils.ExitEditState();
			Service.GalaxyViewController.GoToGalaxyView();
			Service.EventManager.SendEvent(EventId.GalaxyOpenByInfoScreen, null);
		}

		protected override void OnLoaded()
		{
			base.GetElement<UXSprite>("SpriteHighlightPlanetPanel").Visible = false;
			base.InitControls(2);
			this.InitPlanetSlots();
			this.InitUpgradePlanetSlider(1);
			bool useUpgradeGroup = this.useUpgradeGroup;
			this.useUpgradeGroup = true;
			this.InitHitpoints(0);
			this.useUpgradeGroup = useUpgradeGroup;
		}

		protected void InitUpgradePlanetSlider(int sliderIndex)
		{
			float num = (float)this.maxBuildingInfo.Lvl + 1f;
			this.sliders[sliderIndex].DescLabel.Visible = true;
			this.sliders[sliderIndex].DescLabel.Text = this.lang.Get("PLANETS_GNC_SLOTS", new object[0]);
			this.sliders[sliderIndex].NextLabel.Visible = true;
			this.sliders[sliderIndex].NextLabel.Text = this.lang.Get("PLUS", new object[]
			{
				"1"
			});
			this.sliders[sliderIndex].NextSlider.Visible = true;
			this.sliders[sliderIndex].CurrentSlider.Visible = true;
			this.sliders[sliderIndex].CurrentLabel.Visible = true;
			if (this.tutorialMode)
			{
				this.sliders[sliderIndex].NextSlider.Value = (float)(1 + this.buildingInfo.Lvl) / num;
				this.sliders[sliderIndex].CurrentSlider.Value = (float)this.buildingInfo.Lvl / num;
				this.sliders[sliderIndex].CurrentLabel.Text = this.buildingInfo.Lvl.ToString();
			}
			else
			{
				this.sliders[sliderIndex].NextSlider.Value = (float)(1 + this.buildingInfo.Lvl + 1) / num;
				this.sliders[sliderIndex].CurrentSlider.Value = (float)(this.buildingInfo.Lvl + 1) / num;
				this.sliders[sliderIndex].CurrentLabel.Text = (this.buildingInfo.Lvl + 1).ToString();
			}
		}

		protected void InitPlanetSlots()
		{
			this.selectPlanetGrid = base.GetElement<UXGrid>("SelectPlanetGrid");
			UXElement element = base.GetElement<UXElement>("SelectPlanetTemplate");
			element.Visible = true;
			this.selectPlanetGrid.SetTemplateItem("SelectPlanetCard");
			element.Visible = false;
			this.unlockablePlanetList.Clear();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			foreach (PlanetVO current in Service.StaticDataController.GetAll<PlanetVO>())
			{
				if (current.PlayerFacing && !currentPlayer.UnlockedPlanets.Contains(current.Uid) && currentPlayer.Map.Planet.Uid != current.Uid)
				{
					UXCheckbox uXCheckbox = (UXCheckbox)this.selectPlanetGrid.CloneTemplateItem(current.Uid);
					UXLabel subElement = this.selectPlanetGrid.GetSubElement<UXLabel>(current.Uid, "LabelSelectPlanetName");
					if (!this.reqMet && !this.tutorialMode)
					{
						uXCheckbox.RadioGroup = 0;
						uXCheckbox.SetSelectable(false);
						uXCheckbox.Selected = false;
					}
					uXCheckbox.OnSelected = new UXCheckboxSelectedDelegate(this.PlanetSelected);
					UXTexture subElement2 = this.selectPlanetGrid.GetSubElement<UXTexture>(current.Uid, "SpriteSelectPlanetImagePlanet");
					subElement2.LoadTexture("PlanetEnvIcon-" + current.Abbreviation);
					subElement.Text = LangUtils.GetPlanetDisplayName(current);
					uXCheckbox.Tag = current;
					uXCheckbox.RadioGroup = 0;
					uXCheckbox.DelayedSelect(false);
					this.selectPlanetGrid.AddItem(uXCheckbox, current.Order);
					this.unlockablePlanetList.Add(uXCheckbox);
				}
			}
			if (this.unlockablePlanetList.Count == 0)
			{
				base.GetElement<UXElement>("ButtonPrimary").Visible = false;
				base.GetElement<UXElement>("BtnInstantBuy").Visible = false;
				this.buttonTutorialConfirm.Visible = false;
			}
			this.selectPlanetGrid.RepositionItems();
			this.selectPlanetGrid.IsScrollable = true;
			this.selectPlanetGrid.ScrollToItem(0);
		}

		protected void PlanetSelected(UXCheckbox checkbox, bool selected)
		{
			if (selected && (this.reqMet || this.tutorialMode))
			{
				this.selectedPlanet = (PlanetVO)checkbox.Tag;
				string planetDisplayName = LangUtils.GetPlanetDisplayName(this.selectedPlanet);
				if (this.tutorialMode)
				{
					this.labelUpgradeUnlockPlanet.Text = this.lang.Get("PLANETS_GNC_TUT_UNLOCK_MODAL_TITLE", new object[]
					{
						planetDisplayName
					});
					this.buttonTutorialConfirm.Enabled = true;
					this.labelTutorialConfirm.TextColor = UXUtils.COLOR_ENABLED;
				}
				else
				{
					this.labelUpgradeUnlockPlanet.Text = this.lang.Get("PLANETS_GNC_UPGRADE_MODAL_DESC", new object[]
					{
						planetDisplayName
					});
					this.buttonPrimaryAction.Enabled = true;
					this.buttonInstantBuy.Enabled = true;
				}
				for (int i = 0; i < this.unlockablePlanetList.Count; i++)
				{
					this.unlockablePlanetList[i].RadioGroup = 55;
				}
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.selectedPlanet == null)
			{
				return;
			}
			TournamentVO activeTournamentOnPlanet = TournamentController.GetActiveTournamentOnPlanet(this.selectedPlanet.Uid);
			if (activeTournamentOnPlanet != null)
			{
				TimedEventState state = TimedEventUtils.GetState(activeTournamentOnPlanet);
				if (state == TimedEventState.Live)
				{
					int secondsRemaining = TimedEventUtils.GetSecondsRemaining(activeTournamentOnPlanet);
					string text = LangUtils.FormatTime((long)secondsRemaining);
					this.labelUnlockPlanetTimer.Text = this.lang.Get("PLANETS_GNC_UPGRADE_CONFLICT_ENDS", new object[]
					{
						text
					});
				}
				else if (state == TimedEventState.Upcoming)
				{
					int secondsRemaining2 = TimedEventUtils.GetSecondsRemaining(activeTournamentOnPlanet);
					string text2 = LangUtils.FormatTime((long)secondsRemaining2);
					this.labelUnlockPlanetTimer.Text = this.lang.Get("PLANETS_GNC_UPGRADE_NEXT_CONFLICT_BEGINS", new object[]
					{
						text2
					});
				}
				else
				{
					this.labelUnlockPlanetTimer.Text = string.Empty;
				}
			}
			else
			{
				this.labelUnlockPlanetTimer.Text = string.Empty;
			}
		}

		protected override void HandleClose(UXButton button)
		{
			if (!this.allowClose)
			{
				return;
			}
			if (this.tutorialMode && this.callback != null)
			{
				this.callback(null, null);
				this.callback = null;
			}
			this.Close(null);
		}

		private void OnTutorialConfirmClicked(UXButton button)
		{
			string planetDisplayName = LangUtils.GetPlanetDisplayName(this.selectedPlanet);
			string planetDisplayNameKey = LangUtils.GetPlanetDisplayNameKey(this.selectedPlanet.Uid);
			Service.SharedPlayerPrefs.SetPref("1stPlaName", planetDisplayNameKey);
			Service.SharedPlayerPrefs.SetPref("1stPlaUid", this.selectedPlanet.Uid);
			AlertScreen alertScreen = AlertScreen.ShowModal(false, this.lang.Get("PLANETS_GNC_UNLOCK_MODAL_TITLE", new object[0]), this.lang.Get("PLANETS_GNC_TUTORIAL_MESSAGE", new object[]
			{
				planetDisplayName
			}), new OnScreenModalResult(this.OnConfirmation), this.selectedPlanet);
			alertScreen.AllowFUEBackButton = true;
			alertScreen.Tag = "Tutorial";
			alertScreen.SetPrimaryLabelText(this.lang.Get("s_Confirm", new object[0]));
			alertScreen.SetTextureInset("PlanetEnvIcon-" + this.selectedPlanet.Abbreviation);
		}

		private void OnConfirmation(object result, object cookie)
		{
			bool flag = result != null;
			if (flag)
			{
				string uid = ((PlanetVO)cookie).Uid;
				if (this.callback != null)
				{
					this.callback(flag, cookie);
				}
				if (this.useUpgradeGroup)
				{
					this.ConfirmUpgrade();
				}
				if (this.tutorialMode)
				{
					Service.CurrentPlayer.AddUnlockedPlanet(uid);
				}
				Service.EventManager.SendEvent(EventId.PlanetScoutingStart, uid);
				this.Close(null);
			}
		}

		protected override void ConfirmUpgrade()
		{
			Service.ISupportController.StartBuildingUpgrade(this.nextBuildingInfo, this.selectedBuilding, false, this.selectedPlanet.Uid);
			if (this.nextBuildingInfo.Time > 0)
			{
				Service.BuildingController.EnsureDeselectSelectedBuilding();
				Service.BuildingController.SelectedBuilding = this.selectedBuilding;
			}
			this.Close(this.selectedBuilding.ID);
		}

		public override void OnDestroyElement()
		{
			if (this.selectPlanetGrid != null)
			{
				this.selectPlanetGrid.Clear();
			}
			this.unlockablePlanetList.Clear();
			if (!this.tutorialMode)
			{
				this.buttonViewGalaxyMap.OnClicked = null;
			}
			this.DeactivateHighlight();
			this.selectedPlanet = null;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.buttonViewGalaxyMap.OnClicked = null;
			base.OnDestroyElement();
		}

		protected override void OnPayMeForCurrencyResult(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie) && !PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
			{
				this.OnUpgradeButtonClicked(null);
			}
		}

		protected override void OnPayMeForDroidResult(object result, object cookie)
		{
			if (result != null)
			{
				this.OnUpgradeButtonClicked(null);
			}
		}

		protected void OnUpgradeButtonClickedRetry(object result, object cookie)
		{
			bool flag = result != null;
			if (flag)
			{
				this.OnUpgradeButtonClicked(null);
			}
		}

		protected override void OnUpgradeButtonClicked(UXButton button)
		{
			int upgradeCredits = this.nextBuildingInfo.UpgradeCredits;
			int upgradeMaterials = this.nextBuildingInfo.UpgradeMaterials;
			int upgradeContraband = this.nextBuildingInfo.UpgradeContraband;
			string buildingPurchaseContext = GameUtils.GetBuildingPurchaseContext(this.nextBuildingInfo, this.buildingInfo, true, false, this.selectedPlanet);
			if (PayMeScreen.ShowIfNotEnoughCurrency(upgradeCredits, upgradeMaterials, upgradeContraband, buildingPurchaseContext, new OnScreenModalResult(this.OnPayMeForCurrencyResult)))
			{
				return;
			}
			if (PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
			{
				return;
			}
			string planetDisplayName = LangUtils.GetPlanetDisplayName(this.selectedPlanet);
			AlertScreen alertScreen = AlertScreen.ShowModal(false, this.lang.Get("PLANETS_GNC_UNLOCK_MODAL_TITLE", new object[0]), this.lang.Get("PLANETS_GNC_UNLOCK_MODAL_DESC", new object[]
			{
				planetDisplayName
			}), new OnScreenModalResult(this.OnConfirmation), this.selectedPlanet);
			alertScreen.SetPrimaryLabelText(this.lang.Get("s_Confirm", new object[0]));
			alertScreen.SetTextureInset("PlanetEnvIcon-" + this.selectedPlanet.Abbreviation);
		}

		protected override void OnInstantUpgradeButtonClicked(UXButton button)
		{
			if (!base.HasEnoughResourceCapacityToUpgrade(this.nextBuildingInfo))
			{
				CurrencyType currencyType = GameUtils.GetCurrencyType(this.nextBuildingInfo.UpgradeCredits, this.nextBuildingInfo.UpgradeMaterials, this.nextBuildingInfo.UpgradeContraband);
				Service.ICurrencyController.HandleUnableToCollect(currencyType);
				return;
			}
			int num = GameUtils.CrystalCostToInstantUpgrade(this.nextBuildingInfo);
			string planetDisplayName = LangUtils.GetPlanetDisplayName(this.selectedPlanet);
			string text = this.lang.ThousandsSeparated(num);
			FinishNowScreen finishNowScreen = FinishNowScreen.ShowModalWithNoContract(this.selectedBuilding, new OnScreenModalResult(base.ConfirmInstantUpgrade), null, num, this.lang.Get("PLANETS_GNC_UNLOCK_MODAL_TITLE", new object[0]), this.lang.Get("PLANETS_GNC_INSTANT_MODAL_DESC", new object[]
			{
				text,
				planetDisplayName
			}), true);
			finishNowScreen.SetTextureInset("PlanetEnvIcon-" + this.selectedPlanet.Abbreviation);
		}

		protected override void HandleInstantUpgradeRequest()
		{
			if (this.instantUpgradeBuildingKey != null && this.instantUpgradeBuildingUid != null && this.selectedPlanet != null)
			{
				Service.CurrentPlayer.AddUnlockedPlanet(this.selectedPlanet.Uid);
				BuildingInstantUpgradeRequest request = new BuildingInstantUpgradeRequest(this.instantUpgradeBuildingKey, this.instantUpgradeBuildingUid, this.selectedPlanet.Uid);
				BuildingInstantUpgradeCommand command = new BuildingInstantUpgradeCommand(request);
				Service.ServerAPI.Enqueue(command);
				this.instantUpgradeBuildingKey = null;
				this.instantUpgradeBuildingUid = null;
			}
		}

		public void ActivateHighlight()
		{
			base.GetElement<UXSprite>("SpriteHighlightPlanetPanel").Visible = true;
			UXElement element = base.GetElement<UXElement>("SelectPlanetArrowPlaceholder");
			Service.UXController.MiscElementsManager.AlignArrow(element.Position);
		}

		public void DeactivateHighlight()
		{
			base.GetElement<UXSprite>("SpriteHighlightPlanetPanel").Visible = false;
			Service.UXController.MiscElementsManager.HideArrow();
		}
	}
}
