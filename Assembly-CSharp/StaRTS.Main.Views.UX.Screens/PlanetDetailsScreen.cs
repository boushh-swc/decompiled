using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class PlanetDetailsScreen : ClosableScreen
	{
		private const string BACK_TO_GALAXY_BUTTON = "BtnBackToGalaxy";

		private const string SPRITE_BACK_TO_GALAXY_ICON = "SpriteBackToGalaxyIcon";

		private const string LABEL_BACK_TO_GALAXY = "LabelBackToGalaxy";

		private const string PLANETS_PLAY_BUILD_PC = "PLANETS_PLAY_BUILD_PC";

		private const string PLANETS_GO_TO_GALAXY = "PLANETS_GO_TO_GALAXY";

		private const string PLANETS_GALAXY_LOCKED = "PLANETS_GALAXY_LOCKED";

		private const string PLANETS_PLAYSCREEN_PREVIOUS_WORLD = "PLANETS_PLAYSCREEN_PREVIOUS_WORLD";

		private const string PLANETS_PLAYSCREEN_NEXT_WORLD = "PLANETS_PLAYSCREEN_NEXT_WORLD";

		private const string LABEL_PLANET_PREV = "LabelBtnPlanetPrev";

		private const string LABEL_PLANET_NEXT = "LabelBtnPlanetNext";

		private const string PLANET_POS_ELEMENT = "PlanetSize";

		private const string LABEL_GALAXY_LOCKED = "LabelGalaxyLocked";

		private const string BUTTON_PLANET_ACTION = "BtnPlanetAction";

		private const string CLOSE_BUTTON_PLANET_VIEW = "BtnClosePlanetView";

		private const string CAMPAIGN_DETAILS_GROUP = "PanelCampaignDetails";

		private const string CAMPAIGN_SELECT_GROUP = "PanelCampaignSelect";

		private const string LOCKED_PLANET_INFO = "LockedInfo";

		public const string PLANET_PANEL_LOCKED = "PlanetPanelLocked";

		public const string PLANET_DETAILS_TOP = "PlanetDetailsTop";

		public const string PLANET_DETAILS_BOTTOM = "PlanetDetailsBottom";

		public const string BUTTON_PLANET_PREV = "BtnPlanetPrev";

		public const string BUTTON_PLANET_NEXT = "BtnPlanetNext";

		private const string SHOW_ANIM_TRIGGER = "Show";

		private const string SHOW_UI_ANIM_TRIGGER = "ShowUI";

		private const string HIDE_UI_ANIM_TRIGGER = "HideUI";

		private const string SHOW_OBJECTIVES_UI_ANIM_TRIGGER = "ShowObjectives";

		private const string HIDE_OBJECTIVES_UI_ANIM_TRIGGER = "HideObjectives";

		private const string OFF_OBJECTIVES_UI_ANIM_TRIGGER = "OffObjectives";

		private const float ANIM_DELAY = 0.2f;

		private EventManager eventManager;

		private bool autoTransition;

		private bool largeObjectivesShowing;

		private bool planetDetailsShowing;

		public PlanetDetailsChaptersViewModule chaptersView;

		public PlanetDetailsFeaturedViewModule featuredView;

		public PlanetDetailsMissionViewModule pveView;

		public PlanetDetailsPvEViewModule pveHomeView;

		public PlanetDetailsObjectivesViewModule objectivesView;

		public PlanetDetailsLargeObjectivesViewModule largeObjectivesView;

		public PlanetDetailsPvPViewModule pvpView;

		public PlanetDetailsPlanetInfoViewModule planetInfoView;

		public PlanetDetailsRelocateViewModule relocateView;

		public CampaignScreenSection currentSection;

		private UXElement campaignDetailsGroup;

		private UXElement campaignDetailsTimeLeft;

		private UXElement campaignDescriptionGroup;

		private UXElement campaignSelectGroup;

		private UXElement planetDetailsTop;

		private UXElement planetDetailsBottom;

		private UXElement planetLockedInfo;

		private UXElement objectivesDetails;

		private UXLabel labelGalaxyLocked;

		private UXLabel labelPlanetPrev;

		private UXLabel labelPlanetNext;

		private UXButton backToGalaxyButton;

		private UXButton buttonPlanetAction;

		public bool ShowingObjectivesUI;

		private JewelControl galaxyJewel;

		private TournamentController tournamentController;

		private uint animDelayTimerId;

		public PlanetVO viewingPlanetVO
		{
			get;
			set;
		}

		public Planet CurrentPlanet
		{
			get;
			private set;
		}

		protected override bool AllowGarbageCollection
		{
			get
			{
				return !this.autoTransition;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return false;
			}
		}

		public PlanetDetailsScreen(Planet planet, bool automaticallyTransition) : base("gui_planet_view")
		{
			this.viewingPlanetVO = planet.VO;
			this.eventManager = Service.EventManager;
			this.tournamentController = Service.TournamentController;
			this.chaptersView = new PlanetDetailsChaptersViewModule(this);
			this.featuredView = new PlanetDetailsFeaturedViewModule(this);
			this.pveView = new PlanetDetailsMissionViewModule(this);
			this.pveHomeView = new PlanetDetailsPvEViewModule(this);
			this.objectivesView = new PlanetDetailsObjectivesViewModule(this);
			this.largeObjectivesView = new PlanetDetailsLargeObjectivesViewModule(this);
			this.pvpView = new PlanetDetailsPvPViewModule(this);
			this.planetInfoView = new PlanetDetailsPlanetInfoViewModule(this);
			this.relocateView = new PlanetDetailsRelocateViewModule(this);
			this.CurrentPlanet = planet;
			this.autoTransition = automaticallyTransition;
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.InitElements();
			this.pvpView.OnScreenLoaded();
			this.pveHomeView.OnScreenLoaded();
			this.objectivesView.OnScreenLoaded();
			this.largeObjectivesView.OnScreenLoaded();
			this.pveView.OnScreenLoaded();
			this.chaptersView.OnScreenLoaded();
			this.featuredView.OnScreenLoaded();
			this.planetInfoView.OnScreenLoaded();
			this.relocateView.OnScreenLoaded();
			this.GoToMainSelectScreen();
			base.Root.GetComponent<Animator>().SetTrigger("Show");
			if (this.autoTransition)
			{
				this.Transition();
			}
			this.ShowingObjectivesUI = false;
		}

		public void UpdateCurrentPlanet(Planet planet)
		{
			this.CurrentPlanet = planet;
			this.viewingPlanetVO = this.CurrentPlanet.VO;
			this.planetLockedInfo.Visible = !Service.CurrentPlayer.IsPlanetUnlocked(this.viewingPlanetVO.Uid);
			this.largeObjectivesView.RefreshScreenForPlanetChange();
			this.pvpView.RefreshScreenForPlanetChange();
			this.pveView.RefreshScreenForPlanetChange();
			this.pveHomeView.RefreshScreenForPlanetChange();
			this.chaptersView.RefreshScreenForPlanetChange();
			this.objectivesView.RefreshScreenForPlanetChange();
			this.featuredView.RefreshScreenForPlanetChange();
			this.planetInfoView.RefreshScreenForPlanetChange();
			this.relocateView.RefreshScreenForPlanetChange();
			if (!this.largeObjectivesShowing && !this.planetDetailsShowing)
			{
				this.GoToMainSelectScreen();
			}
		}

		public void CloseSubScreenAndReturnToMainSelect()
		{
			CampaignScreenSection campaignScreenSection = this.currentSection;
			if (campaignScreenSection != CampaignScreenSection.Campaign)
			{
				this.GoToMainSelectScreen();
			}
			else
			{
				this.pveView.ReturnToMainSelect();
			}
		}

		public void AnimateShowUI()
		{
			if (base.Root != null && this.CurrentPlanet.PlanetGameObject != null)
			{
				Service.GalaxyPlanetController.UpdateEventEffectStatus(this.CurrentPlanet);
				this.CurrentPlanet.PlanetGameObject.SetActive(true);
				this.planetDetailsTop.Visible = true;
				this.planetDetailsBottom.Visible = true;
				base.Root.GetComponent<Animator>().SetTrigger("ShowUI");
			}
			MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
			miscElementsManager.ShowEventsTickerView();
			this.featuredView.OnAnimateShowUI();
		}

		public void AnimateHideUI()
		{
			if (base.Root != null)
			{
				base.Root.GetComponent<Animator>().SetTrigger("HideUI");
				Service.ViewTimerManager.CreateViewTimer(0.2f, false, new TimerDelegate(this.OnTimerCallback), null);
			}
			MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
			miscElementsManager.HideEventsTickerView();
		}

		public void ShowObjectivesUI()
		{
			this.largeObjectivesShowing = true;
			if (base.Root != null)
			{
				this.largeObjectivesView.Show();
				base.CurrentBackDelegate = new UXButtonClickedDelegate(this.largeObjectivesView.OnBackButtonClicked);
				base.Root.GetComponent<Animator>().SetTrigger("ShowObjectives");
				this.animDelayTimerId = Service.ViewTimerManager.CreateViewTimer(0.2f, false, new TimerDelegate(this.OnTimerCallback), null);
				this.UpdateCurrentPlanet(this.CurrentPlanet);
				Service.GalaxyViewController.SwitchToObjectiveDetails(false);
			}
		}

		public void HideObjectivesUI()
		{
			this.largeObjectivesShowing = false;
			if (base.Root != null && this.CurrentPlanet.PlanetGameObject != null)
			{
				if (this.animDelayTimerId != 0u)
				{
					Service.ViewTimerManager.KillViewTimer(this.animDelayTimerId);
				}
				this.largeObjectivesView.Hide();
				base.CurrentBackDelegate = new UXButtonClickedDelegate(this.HandleClose);
				Service.GalaxyPlanetController.UpdateEventEffectStatus(this.CurrentPlanet);
				this.planetDetailsTop.Visible = true;
				base.Root.GetComponent<Animator>().SetTrigger("HideObjectives");
				Service.GalaxyViewController.SwitchFromObjectiveDetails();
			}
		}

		public void ShowPlanetInfoUI()
		{
			this.planetDetailsShowing = true;
			if (base.Root != null)
			{
				base.Root.GetComponent<Animator>().SetTrigger("ShowObjectives");
				this.animDelayTimerId = Service.ViewTimerManager.CreateViewTimer(0.2f, false, new TimerDelegate(this.OnTimerCallback), null);
				this.UpdateCurrentPlanet(this.CurrentPlanet);
				this.largeObjectivesView.Hide();
				Service.GalaxyViewController.SwitchToObjectiveDetails(false);
			}
		}

		public void HidePlanetInfoUI()
		{
			this.planetDetailsShowing = false;
			if (base.Root != null && this.CurrentPlanet.PlanetGameObject != null)
			{
				if (this.animDelayTimerId != 0u)
				{
					Service.ViewTimerManager.KillViewTimer(this.animDelayTimerId);
				}
				Service.GalaxyPlanetController.UpdateEventEffectStatus(this.CurrentPlanet);
				this.planetDetailsTop.Visible = true;
				base.Root.GetComponent<Animator>().SetTrigger("HideObjectives");
				Service.GalaxyViewController.SwitchFromObjectiveDetails();
			}
		}

		private void OnTimerCallback(uint id, object cookie)
		{
			if (base.Root != null && this.CurrentPlanet != null && this.CurrentPlanet.PlanetGameObject != null)
			{
				this.planetDetailsTop.Visible = false;
				this.ShowingObjectivesUI = false;
			}
		}

		public float GetPlanetFrustumDistance(float height)
		{
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			UXElement element = base.GetElement<UXElement>("PlanetSize");
			Vector3 screenPoint = element.Position - Vector3.up * element.Height * 0.5f;
			Vector3 screenPoint2 = element.Position + Vector3.up * element.Height * 0.5f;
			Ray ray = mainCamera.ScreenPointToRay(screenPoint);
			Ray ray2 = mainCamera.ScreenPointToRay(screenPoint2);
			float num = Vector3.Angle(ray.direction, ray2.direction);
			return height / Mathf.Tan(num * 0.0174532924f);
		}

		public Quaternion GetTransitionLookOffset()
		{
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			UXElement element = base.GetElement<UXElement>("PlanetSize");
			Vector3 vector = base.Position / this.uxCamera.Scale + Vector3.forward;
			Vector3 vector2 = element.Position / this.uxCamera.Scale + Vector3.forward;
			vector = mainCamera.Camera.ScreenToWorldPoint(vector);
			vector -= mainCamera.Camera.transform.position;
			vector2 = mainCamera.Camera.ScreenToWorldPoint(vector2);
			return Quaternion.FromToRotation((vector2 - mainCamera.Camera.transform.position).normalized, vector.normalized);
		}

		public void Transition()
		{
			Service.GalaxyViewController.StartPlanetTransition();
		}

		private void InitElements()
		{
			this.campaignDetailsGroup = base.GetElement<UXElement>("PanelCampaignDetails");
			this.campaignSelectGroup = base.GetElement<UXElement>("PanelCampaignSelect");
			this.planetDetailsTop = base.GetElement<UXElement>("PlanetDetailsTop");
			this.planetDetailsBottom = base.GetElement<UXElement>("PlanetDetailsBottom");
			this.galaxyJewel = JewelControl.Create(this, "Galaxy");
			this.labelPlanetPrev = base.GetElement<UXLabel>("LabelBtnPlanetPrev");
			this.labelPlanetPrev.Text = this.lang.Get("PLANETS_PLAYSCREEN_PREVIOUS_WORLD", new object[0]);
			this.labelPlanetNext = base.GetElement<UXLabel>("LabelBtnPlanetNext");
			this.labelPlanetNext.Text = this.lang.Get("PLANETS_PLAYSCREEN_NEXT_WORLD", new object[0]);
			this.planetLockedInfo = base.GetElement<UXElement>("LockedInfo");
		}

		protected override void InitButtons()
		{
			this.backToGalaxyButton = base.GetElement<UXButton>("BtnBackToGalaxy");
			this.labelGalaxyLocked = base.GetElement<UXLabel>("LabelGalaxyLocked");
			this.labelGalaxyLocked.Text = this.lang.Get("PLANETS_GALAXY_LOCKED", new object[0]);
			bool flag = Service.BuildingLookupController.HasNavigationCenter();
			UXButton element = base.GetElement<UXButton>("BtnPlanetPrev");
			UXButton element2 = base.GetElement<UXButton>("BtnPlanetNext");
			if (flag)
			{
				this.backToGalaxyButton.OnClicked = new UXButtonClickedDelegate(this.OnGalaxyButtonClicked);
				this.backToGalaxyButton.Visible = true;
				base.GetElement<UXLabel>("LabelBackToGalaxy").Text = this.lang.Get("PLANETS_GO_TO_GALAXY", new object[0]);
				element.Visible = true;
				element.OnClicked = new UXButtonClickedDelegate(this.OnPlanetPrevClicked);
				element2.Visible = true;
				element2.OnClicked = new UXButtonClickedDelegate(this.OnPlanetNextClicked);
				this.labelGalaxyLocked.Visible = false;
			}
			else
			{
				this.backToGalaxyButton.Visible = false;
				base.GetElement<UXElement>("SpriteBackToGalaxyIcon").Visible = false;
				base.GetElement<UXLabel>("LabelBackToGalaxy").Text = this.lang.Get("PLANETS_PLAY_BUILD_PC", new object[0]);
				element.Visible = false;
				element2.Visible = false;
				this.labelGalaxyLocked.Visible = true;
			}
			this.buttonPlanetAction = base.GetElement<UXButton>("BtnPlanetAction");
			this.buttonPlanetAction.OnClicked = new UXButtonClickedDelegate(this.ButtonPlanetActionClicked);
			this.CloseButton = base.GetElement<UXButton>("BtnClosePlanetView");
			this.CloseButton.Enabled = true;
			base.CurrentBackButton = this.CloseButton;
			this.CloseButton.OnClicked = new UXButtonClickedDelegate(this.HandleClose);
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.HandleClose);
		}

		public void UpdatePvpPanel(bool showTournamentRating, TournamentVO tournamentVO)
		{
			this.pvpView.UpdatePvpPanel(showTournamentRating, tournamentVO);
			if (tournamentVO != null && GameUtils.ConflictStartsInBadgePeriod(tournamentVO))
			{
				this.tournamentController.OnTournamentViewed(tournamentVO.Uid);
			}
			if (tournamentVO == null)
			{
				this.featuredView.ShowNoActiveEventsAvailable();
			}
			this.galaxyJewel.Value = this.tournamentController.NumberOfTournamentsNotViewed();
		}

		public void GoToMainSelectScreen()
		{
			Service.UXController.HUD.SetSquadScreenVisibility(true);
			this.planetDetailsTop.Visible = true;
			this.planetDetailsBottom.Visible = true;
			this.campaignDetailsGroup.Visible = false;
			this.campaignSelectGroup.Visible = false;
			this.buttonPlanetAction.Enabled = Service.CurrentPlayer.IsCurrentPlanet(this.CurrentPlanet.VO);
		}

		public void GoToAllCampaigns()
		{
			this.chaptersView.InitCampaignGrid();
		}

		public void SelectCampaign(CampaignVO campaignType)
		{
			this.pveView.SelectCampaign(campaignType);
		}

		public override void OnDestroyElement()
		{
			this.currentSection = CampaignScreenSection.Main;
			this.GoToMainSelectScreen();
			this.pveView.Destroy();
			this.chaptersView.Destroy();
			this.planetInfoView.Destroy();
			this.featuredView.Destroy();
			base.OnDestroyElement();
		}

		private void ButtonPlanetActionClicked(UXButton button)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (this.viewingPlanetVO == currentPlayer.Planet)
			{
				this.HandleClose(button);
				return;
			}
			if (currentPlayer.IsPlanetUnlocked(this.viewingPlanetVO.Uid))
			{
				this.relocateView.OnRelocateButton(null);
			}
		}

		protected override void HandleClose(UXButton button)
		{
			if (!this.allowClose)
			{
				return;
			}
			base.Root.GetComponent<Animator>().ResetTrigger("OffObjectives");
			base.Root.GetComponent<Animator>().SetTrigger("OffObjectives");
			base.CloseNoTransition(null);
			Service.GalaxyViewController.GoToHome();
		}

		public void GoToGalaxyFromPlanetScreen()
		{
			this.GoToGalaxyFromPlanetScreen(null);
		}

		protected void GoToGalaxyFromPlanetScreen(UXButton button)
		{
			base.HandleClose(button);
			this.tournamentController.OnGalaxyViewed();
			Service.GalaxyViewController.TranstionPlanetToGalaxy();
			Service.EventManager.SendEvent(EventId.GalaxyOpenByPlayScreen, null);
		}

		protected void OnGalaxyButtonClicked(UXButton button)
		{
			this.GoToGalaxyFromPlanetScreen(button);
		}

		protected void OnPlanetPrevClicked(UXButton button)
		{
			Planet planet = Service.GalaxyViewController.TransitionToPrevPlanet();
			Service.EventManager.SendEvent(EventId.GalaxyTransitionToPreviousPlanet, null);
			if (this.largeObjectivesShowing)
			{
				this.objectivesView.SendObjectiveDetailsClickedBiLog(planet.VO);
			}
		}

		protected void OnPlanetNextClicked(UXButton button)
		{
			Planet planet = Service.GalaxyViewController.TransitionToNextPlanet();
			Service.EventManager.SendEvent(EventId.GalaxyTransitionToNextPlanet, null);
			if (this.largeObjectivesShowing)
			{
				this.objectivesView.SendObjectiveDetailsClickedBiLog(planet.VO);
			}
		}

		public override void Close(object modalResult)
		{
			base.Close(modalResult);
			this.pveHomeView.OnClose();
			this.pvpView.OnClose();
			this.objectivesView.OnClose();
			this.largeObjectivesView.OnClose();
			this.planetInfoView.OnClose();
			this.eventManager.SendEvent(EventId.PlanetDetailsScreenClosed, EventPriority.Default);
		}
	}
}
