using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class PlanetSelectionDropDown
	{
		public UXFactory uxFactory;

		private const int NAVIGATION_RIGHT_BG_SMALL = 120;

		private const int NAVIGATION_RIGHT_BG_LARGE = 242;

		private const string WIDGET_PLANET_FILTER = "PlanetFilter";

		private const string PLANET_FILTER_BUTTON = "BtnPlanetFilter";

		private const string LABEL_PLANET_FILTER_BUTTON = "LabelBtnPlanetFilter";

		private const string SPRITE_PLANET_FILTER_BUTTON = "SpriteBtnPlanetFilterSymbol";

		private const string TEXTURE_PLANET_FILTER_BUTTON = "TextureBtnPlanetFilterSymbol";

		private const string PLANET_FILTER_OPTIONS = "PlanetFilterOptions";

		private const string PLANET_FILTER_OPTIONS_GRID = "PlanetFilterGrid";

		private const string FILTER_PLANET_TEMPLATE = "BtnPlanet";

		private const string LABEL_PLANET_BUTTON = "LabelBtnPlanet";

		private const string SPRITE_PLANET_BUTTON_SELECT = "SpriteBtnPlanetSelect";

		private const string SPRITE_PLANET_BUTTON_BACKGROUND = "SpriteBtnPlanetSymbol";

		private const string TEXTURE_PLANET_BUTTON_SELECT = "TextureBtnPlanetSymbol";

		private const string SPRITE_PLANET_OPTIONS_BACKGROUND = "SpritePlanetFilterBg";

		private const string NAVIGATION_ROW_BG_RIGHT = "NavigationRowBtnBgRight";

		private const string ALL_PLANET_BUTTON = "BtnAll";

		private PlanetVO selectedPlanet;

		private UXButton currentPlanetBtn;

		private UXLabel cuttentPlanetBtnLabel;

		private UXSprite cuttentPlanetBtnBackground;

		private UXGrid planetGrid;

		private UXElement planetOptionPanel;

		private UXSprite planetOptionPanelBackground;

		private UXTexture cuttentPlanetBtnTexture;

		private Lang lang;

		public event Action<PlanetVO> PlanetSelectCallBack
		{
			add
			{
				Action<PlanetVO> action = this.PlanetSelectCallBack;
				Action<PlanetVO> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<PlanetVO>>(ref this.PlanetSelectCallBack, (Action<PlanetVO>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<PlanetVO> action = this.PlanetSelectCallBack;
				Action<PlanetVO> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<PlanetVO>>(ref this.PlanetSelectCallBack, (Action<PlanetVO>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public PlanetSelectionDropDown(UXFactory uxFactory)
		{
			this.uxFactory = uxFactory;
			this.lang = Service.Lang;
		}

		public void Init()
		{
			this.selectedPlanet = null;
			this.currentPlanetBtn = this.uxFactory.GetElement<UXButton>("BtnPlanetFilter");
			this.cuttentPlanetBtnLabel = this.uxFactory.GetElement<UXLabel>("LabelBtnPlanetFilter");
			this.cuttentPlanetBtnBackground = this.uxFactory.GetElement<UXSprite>("SpriteBtnPlanetFilterSymbol");
			this.cuttentPlanetBtnTexture = this.uxFactory.GetElement<UXTexture>("TextureBtnPlanetFilterSymbol");
			this.currentPlanetBtn.OnClicked = new UXButtonClickedDelegate(this.CurrentPlanetButtonClicked);
			this.planetGrid = this.uxFactory.GetElement<UXGrid>("PlanetFilterGrid");
			this.planetGrid.SetTemplateItem("BtnPlanet");
			this.planetOptionPanel = this.uxFactory.GetElement<UXElement>("PlanetFilterOptions");
			this.planetOptionPanel.Visible = false;
			this.planetOptionPanelBackground = this.uxFactory.GetElement<UXSprite>("SpritePlanetFilterBg");
			this.UpdateCurrentPlanetButton();
			this.currentPlanetBtn.Visible = false;
		}

		private void UpdateCurrentPlanetButton()
		{
			if (this.selectedPlanet != null)
			{
				this.cuttentPlanetBtnBackground.Visible = false;
				this.cuttentPlanetBtnTexture.Visible = true;
				this.cuttentPlanetBtnLabel.Text = LangUtils.GetPlanetDisplayName(this.selectedPlanet);
				this.cuttentPlanetBtnTexture.LoadTexture(this.selectedPlanet.LeaderboardButtonTexture);
			}
			else
			{
				this.cuttentPlanetBtnBackground.Visible = true;
				this.cuttentPlanetBtnTexture.Visible = false;
				this.cuttentPlanetBtnLabel.Text = this.lang.Get("s_ShowAll", new object[0]);
			}
		}

		private void TogglePlanetOptions()
		{
			this.planetOptionPanel.Visible = !this.planetOptionPanel.Visible;
			this.planetGrid.RepositionItems();
		}

		private void CurrentPlanetButtonClicked(UXButton button)
		{
			this.TogglePlanetOptions();
		}

		private void NewPlanetOptionClicked(UXCheckbox box, bool selected)
		{
			if (selected)
			{
				this.selectedPlanet = (box.Tag as PlanetVO);
				this.planetOptionPanel.Visible = false;
				this.UpdateCurrentPlanetButton();
				if (this.PlanetSelectCallBack != null)
				{
					this.PlanetSelectCallBack(this.selectedPlanet);
				}
			}
		}

		private void AddAllPlanetButton()
		{
			UXCheckbox uXCheckbox = (UXCheckbox)this.planetGrid.CloneTemplateItem("BtnAll");
			uXCheckbox.OnSelected = new UXCheckboxSelectedDelegate(this.NewPlanetOptionClicked);
			uXCheckbox.Tag = null;
			uXCheckbox.Selected = true;
			UXLabel subElement = this.planetGrid.GetSubElement<UXLabel>("BtnAll", "LabelBtnPlanet");
			subElement.Text = this.lang.Get("s_ShowAll", new object[0]);
			this.planetGrid.AddItem(uXCheckbox, 2000);
		}

		public void SetStatePlanetOptions(SocialTabs type)
		{
			switch (type)
			{
			case SocialTabs.Friends:
				this.DisablePlanetSelection();
				goto IL_68;
			case SocialTabs.Squads:
				this.DisablePlanetSelection();
				goto IL_68;
			case SocialTabs.Leaders:
				this.InitPlanetOptionPlayerTab();
				this.ActivatePlanetSelection();
				goto IL_68;
			case SocialTabs.Tournament:
				this.InitPlanetOptionTournamentTab();
				this.ActivatePlanetSelection();
				goto IL_68;
			}
			this.DisablePlanetSelection();
			IL_68:
			this.UpdateCurrentPlanetButton();
		}

		public void DisablePlanetSelection()
		{
			this.currentPlanetBtn.Enabled = false;
			this.currentPlanetBtn.Visible = false;
			this.planetOptionPanel.Visible = false;
		}

		public void ActivatePlanetSelection()
		{
			this.currentPlanetBtn.Visible = true;
			this.currentPlanetBtn.Enabled = true;
			this.planetOptionPanel.Visible = false;
		}

		private void AddPlanetButton(PlanetVO planet, bool createdForTournament, bool setSelected)
		{
			string uid = planet.Uid;
			UXCheckbox uXCheckbox = (UXCheckbox)this.planetGrid.CloneTemplateItem(uid);
			uXCheckbox.OnSelected = new UXCheckboxSelectedDelegate(this.NewPlanetOptionClicked);
			uXCheckbox.Tag = planet;
			uXCheckbox.Selected = setSelected;
			UXLabel subElement = this.planetGrid.GetSubElement<UXLabel>(uid, "LabelBtnPlanet");
			subElement.Text = LangUtils.GetPlanetDisplayName(planet);
			UXSprite subElement2 = this.planetGrid.GetSubElement<UXSprite>(uid, "SpriteBtnPlanetSymbol");
			subElement2.Visible = false;
			UXTexture subElement3 = this.planetGrid.GetSubElement<UXTexture>(uid, "TextureBtnPlanetSymbol");
			subElement3.LoadTexture(planet.LeaderboardButtonTexture);
			this.planetGrid.AddItem(uXCheckbox, planet.Order);
		}

		private void InitPlanetOptionTournamentTab()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			bool flag = false;
			this.planetGrid.Clear();
			this.planetGrid.Visible = true;
			List<TournamentVO> allLiveAndClosingTournaments = TournamentController.GetAllLiveAndClosingTournaments();
			int num = 2147483647;
			foreach (TournamentVO current in allLiveAndClosingTournaments)
			{
				PlanetVO planetVO = staticDataController.Get<PlanetVO>(current.PlanetId);
				if (planetVO != null)
				{
					if (planetVO.Uid == currentPlayer.Planet.Uid)
					{
						flag = true;
					}
					if (current.EndTimestamp < num)
					{
						this.selectedPlanet = planetVO;
						num = current.EndTimestamp;
					}
					this.AddPlanetButton(planetVO, true, planetVO.Uid == currentPlayer.Planet.Uid);
				}
				if (flag)
				{
					this.selectedPlanet = currentPlayer.Planet;
				}
			}
			this.planetOptionPanelBackground.Height = (float)this.planetGrid.Count * this.planetGrid.CellHeight + 20f;
			this.planetGrid.RepositionItems();
		}

		private void InitPlanetOptionPlayerTab()
		{
			this.planetGrid.Clear();
			this.planetGrid.Visible = true;
			this.selectedPlanet = null;
			List<PlanetVO> allPlayerFacingPlanets = PlanetUtils.GetAllPlayerFacingPlanets();
			foreach (PlanetVO current in allPlayerFacingPlanets)
			{
				if (current != null)
				{
					this.AddPlanetButton(current, false, false);
				}
			}
			this.AddAllPlanetButton();
			this.planetOptionPanelBackground.Height = (float)this.planetGrid.Count * this.planetGrid.CellHeight + 20f;
			this.planetGrid.RepositionItems();
		}

		public void SelectPlanet(PlanetVO planet)
		{
			if (this.planetGrid != null && planet != null)
			{
				UXCheckbox uXCheckbox = null;
				List<UXElement> elementList = this.planetGrid.GetElementList();
				int i = 0;
				int count = elementList.Count;
				while (i < count)
				{
					UXElement uXElement = elementList[i];
					PlanetVO planetVO = uXElement.Tag as PlanetVO;
					if (planetVO != null && planetVO.Uid == planet.Uid)
					{
						uXCheckbox = (uXElement as UXCheckbox);
						break;
					}
					i++;
				}
				if (uXCheckbox != null)
				{
					uXCheckbox.Selected = true;
					this.NewPlanetOptionClicked(uXCheckbox, true);
				}
			}
		}

		public PlanetVO GetSelectedPlanet()
		{
			return this.selectedPlanet;
		}

		public string GetSelectedPlanetId()
		{
			return (this.selectedPlanet != null) ? this.selectedPlanet.Uid : null;
		}

		public string GetSelectedPlanetTabName()
		{
			return (this.selectedPlanet != null) ? this.selectedPlanet.PlanetBIName.ToLower() : "all";
		}

		public void DestroyGrid()
		{
			this.planetGrid.Clear();
		}
	}
}
