using StaRTS.Main.Controllers.Objectives;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsObjectivesViewModule : AbstractPlanetDetailsViewModule, IViewClockTimeObserver
	{
		private const int MAX_OBJECTIVES = 3;

		private const string OBJECTIVES_PANEL = "Objectives";

		private const string LABEL_OBJECTIVES_TITLE = "LabelObjectivesTitle";

		private const string LABEL_OBJECTIVES_TIMER = "LabelObjectivesTimer";

		private const string SPECIAL_OBJECTIVE_ICON = "TextureObjectiveSpecial";

		private const string SPECIAL_OBJECTIVE_FRAME = "TextureObjectiveFrameSpecial";

		private const string BTN_DETAILS = "BtnObjectiveDetails";

		private const string LABEL_BTN_DETAILS = "LabelBtnDetails";

		private const string CONTAINER_JEWEL_OBJECTIVES = "ContainerJewelObjectives";

		private const string BTN_SUPPLY_CRATE = "BtnSupplyCrate";

		private const string SPRITE_CHECKMARK = "SpriteCheckmark";

		private const string SPRITE_SUPPLY_CRATE = "SpriteSupplyCrate";

		private const string LABEL_OPERATIONS_LOCKED = "LabelOperationsLocked";

		private const string SPRITE_PROGRESS = "SpriteObjectiveProgress";

		private const string LABEL_OBJECTIVE_EXPIRED = "LabelObjectiveExpired";

		private const string BG_OBJECTIVE_COMPLETE = "CrateBgComplete";

		private const string BG_OBJECTIVE_EXPIRED = "CrateBgExpired";

		private const string BG_OBJECTIVE_COLLECTED = "CrateBgCollected";

		private const string BG_OBJECTIVE_PROGRESS = "CrateBgProgress";

		private const string SPECIAL_OBJECTIVE_FX = "CrateEffect";

		private const string CONTAINER_CRATE_BG_PROGRESS3 = "ContainerCrateBgProgress3";

		private const string CONTAINER_CRATE_BG_PROGRESS3_LEI = "ContainerCrateBgProgressSpecial3";

		private const string ONLY_LIMITED_EDITION_CRATE_POSSIBLE = "3";

		private const string PLANET_VIEW_OBJECTIVES = "OBJECTIVES";

		private const string PLANET_VIEW_OBJECTIVES_DETAILS = "objective_details";

		private const string EXPIRES_IN = "expires_in";

		private const string GRACE_PERIOD = "grace_period";

		private const string BACK_TO_PLANET = "BACK_TO_PLANET";

		private const string NO_OBJECTIVES_AVAILABLE = "NO_OBJECTIVES_AVAILABLE";

		private UXTexture specialObjectiveIcon;

		private UXTexture specialObjectiveFrame;

		private UXElement objectivesPanel;

		private UXLabel labelObjectivesTitle;

		private UXLabel labelObjectivesTimer;

		private UXButton btnDetails;

		private UXLabel labelBtnDetails;

		private UXElement containerJewelObjectives;

		private UXLabel labelOperationsLocked;

		private UXSprite spriteObjectivesExpired;

		private List<ObjectiveViewData> data;

		private ObjectiveController objectiveController;

		private CurrentPlayer player;

		private Lang lang;

		private string tempTimeString = string.Empty;

		private bool tempIsGrace;

		public PlanetDetailsObjectivesViewModule(PlanetDetailsScreen screen) : base(screen)
		{
			this.data = new List<ObjectiveViewData>();
			this.player = Service.CurrentPlayer;
			this.lang = Service.Lang;
		}

		public void OnScreenLoaded()
		{
			this.objectiveController = Service.ObjectiveController;
			this.objectivesPanel = this.screen.GetElement<UXElement>("Objectives");
			this.labelOperationsLocked = this.screen.GetElement<UXLabel>("LabelOperationsLocked");
			if (!this.objectiveController.ShouldShowObjectives())
			{
				this.objectivesPanel.Visible = false;
				return;
			}
			this.objectiveController = Service.ObjectiveController;
			this.labelObjectivesTitle = this.screen.GetElement<UXLabel>("LabelObjectivesTitle");
			this.labelObjectivesTitle.Text = Service.Lang.Get("OBJECTIVES", new object[0]);
			this.labelObjectivesTimer = this.screen.GetElement<UXLabel>("LabelObjectivesTimer");
			this.btnDetails = this.screen.GetElement<UXButton>("BtnObjectiveDetails");
			this.btnDetails.OnClicked = new UXButtonClickedDelegate(this.OnObjectivesDetailsClicked);
			this.labelBtnDetails = this.screen.GetElement<UXLabel>("LabelBtnDetails");
			this.labelBtnDetails.Text = this.lang.Get("objective_details", new object[0]);
			this.containerJewelObjectives = this.screen.GetElement<UXElement>("ContainerJewelObjectives");
			this.containerJewelObjectives.Visible = false;
			this.specialObjectiveIcon = this.screen.GetElement<UXTexture>("TextureObjectiveSpecial");
			this.specialObjectiveFrame = this.screen.GetElement<UXTexture>("TextureObjectiveFrameSpecial");
			this.specialObjectiveIcon.Visible = false;
			this.specialObjectiveFrame.Visible = false;
			for (int i = 0; i < 3; i++)
			{
				ObjectiveViewData objectiveViewData = new ObjectiveViewData();
				this.data.Add(objectiveViewData);
				string text = (i + 1).ToString();
				objectiveViewData.BtnSupplyCrate = this.screen.GetElement<UXButton>("BtnSupplyCrate" + text);
				objectiveViewData.SpriteCheckmark = this.screen.GetElement<UXSprite>("SpriteCheckmark" + text);
				objectiveViewData.SpritePreview = this.screen.GetElement<UXSprite>("SpriteSupplyCrate" + text);
				objectiveViewData.RadialProgress = this.screen.GetElement<UXSprite>("SpriteObjectiveProgress" + text);
				objectiveViewData.ObjectiveBgComplete = this.screen.GetElement<UXElement>("CrateBgComplete" + text);
				objectiveViewData.ObjectiveBgActive = this.screen.GetElement<UXElement>("CrateBgProgress" + text);
				objectiveViewData.ObjectiveBgExpired = this.screen.GetElement<UXElement>("CrateBgExpired" + text);
				objectiveViewData.ObjectiveBgCollected = this.screen.GetElement<UXElement>("CrateBgCollected" + text);
				objectiveViewData.SpecailObjectiveFx = this.screen.GetElement<UXElement>("CrateEffect" + text);
				objectiveViewData.BtnSupplyCrate.OnClicked = new UXButtonClickedDelegate(this.OnPreviewIconClicked);
				objectiveViewData.BtnSupplyCrate.Tag = objectiveViewData;
				objectiveViewData.ExpiredLabel = this.screen.GetElement<UXLabel>("LabelObjectiveExpired" + text);
				objectiveViewData.ObjectiveContainer = this.screen.GetElement<UXElement>("ContainerCrateBgProgress3");
				objectiveViewData.ObjectiveContainerLEI = ((!(text == "3")) ? this.screen.GetElement<UXElement>("ContainerCrateBgProgress3") : this.screen.GetElement<UXElement>("ContainerCrateBgProgressSpecial3"));
			}
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			this.RefreshScreenForPlanetChange();
		}

		private bool CanRefresh(CurrentPlayer player)
		{
			return this.screen != null && this.screen.viewingPlanetVO != null && player.Objectives.ContainsKey(this.screen.viewingPlanetVO.Uid) && this.objectiveController.ShouldShowObjectives();
		}

		public void RefreshScreenForPlanetChange()
		{
			if (!this.CanRefresh(this.player))
			{
				this.objectivesPanel.Visible = false;
				if (this.objectiveController.ShouldShowObjectives())
				{
					this.labelOperationsLocked.Visible = true;
					this.labelOperationsLocked.Text = this.lang.Get("NO_OBJECTIVES_AVAILABLE", new object[0]);
				}
				return;
			}
			if (!this.player.Objectives.ContainsKey(this.screen.viewingPlanetVO.Uid) || this.player.Objectives[this.screen.viewingPlanetVO.Uid].ProgressObjects.Count == 0)
			{
				this.objectivesPanel.Visible = false;
				this.labelOperationsLocked.Visible = true;
				this.labelOperationsLocked.Text = this.lang.Get("NO_OBJECTIVES_AVAILABLE", new object[0]);
				this.specialObjectiveIcon.Visible = false;
				this.specialObjectiveFrame.Visible = false;
				return;
			}
			this.objectiveController.GetTimeData(this.lang, this.player.Objectives[this.screen.viewingPlanetVO.Uid], ref this.tempIsGrace, ref this.tempTimeString);
			this.objectivesPanel.Visible = true;
			ObjectiveGroup objectiveGroup = this.player.Objectives[this.screen.viewingPlanetVO.Uid];
			ObjectiveSeriesVO objectiveSeriesVO = Service.StaticDataController.Get<ObjectiveSeriesVO>(objectiveGroup.GroupSeriesId);
			this.specialObjectiveIcon.Visible = objectiveSeriesVO.SpecialEvent;
			this.specialObjectiveFrame.Visible = objectiveSeriesVO.SpecialEvent;
			if (objectiveSeriesVO.SpecialEvent)
			{
				this.specialObjectiveIcon.LoadTexture(objectiveSeriesVO.EventIcon);
				this.specialObjectiveFrame.LoadTexture(objectiveSeriesVO.EventPlayArt);
				this.labelObjectivesTitle.Text = Service.Lang.Get(objectiveSeriesVO.ObjectiveString, new object[0]);
			}
			else
			{
				this.labelObjectivesTitle.Text = Service.Lang.Get("OBJECTIVES", new object[0]);
			}
			int i = 0;
			int num = 3;
			while (i < num)
			{
				if (i >= objectiveGroup.ProgressObjects.Count)
				{
					this.data[i].Objective = null;
					this.data[i].BtnSupplyCrate.Visible = false;
				}
				else
				{
					ObjectiveProgress objective = objectiveGroup.ProgressObjects[i];
					this.data[i].Objective = objective;
					this.data[i].BtnSupplyCrate.Visible = true;
					this.data[i].SpecailObjectiveFx.Visible = objectiveSeriesVO.SpecialEvent;
					this.objectiveController.UpdateObjectiveEntry(this.data[i], this.tempIsGrace);
				}
				i++;
			}
			this.OnViewClockTime(0f);
		}

		public void OnViewClockTime(float dt)
		{
			if (!this.player.Objectives.ContainsKey(this.screen.viewingPlanetVO.Uid))
			{
				return;
			}
			this.objectiveController.GetTimeData(this.lang, this.player.Objectives[this.screen.viewingPlanetVO.Uid], ref this.tempIsGrace, ref this.tempTimeString);
			this.labelObjectivesTimer.Text = this.tempTimeString;
			this.labelObjectivesTimer.TextColor = ((!this.tempIsGrace) ? ObjectiveController.TEXT_RED_COLOR : ObjectiveController.TEXT_YELLOW_COLOR);
		}

		private void OnPreviewIconClicked(UXButton button)
		{
			ObjectiveViewData objectiveViewData = button.Tag as ObjectiveViewData;
			ObjectiveProgress objective = objectiveViewData.Objective;
			if (objective.State == ObjectiveState.Complete)
			{
				this.objectiveController.HandleCrateClicked(objective, button);
			}
			else
			{
				this.OnObjectivesDetailsClicked(button);
			}
		}

		private void OnObjectivesDetailsClicked(UXButton button)
		{
			if (this.screen.ShowingObjectivesUI)
			{
				return;
			}
			this.screen.ShowingObjectivesUI = true;
			this.screen.ShowObjectivesUI();
			this.SendObjectiveDetailsClickedBiLog(this.screen.viewingPlanetVO);
		}

		public void SendObjectiveDetailsClickedBiLog(PlanetVO planetVo)
		{
			string cookie = planetVo.PlanetBIName + "|" + this.player.GetPlanetStatus(planetVo.Uid);
			Service.EventManager.SendEvent(EventId.ObjectiveDetailsClicked, cookie);
		}

		public void OnClose()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			int i = 0;
			int count = this.data.Count;
			while (i < count)
			{
				ObjectiveViewData objectiveViewData = this.data[i];
				objectiveViewData.BtnSupplyCrate = null;
				objectiveViewData.Objective = null;
				objectiveViewData.SpriteCheckmark = null;
				objectiveViewData.RadialProgress = null;
				objectiveViewData.ExpiredLabel = null;
				objectiveViewData.SpritePreview = null;
				objectiveViewData.ObjectiveBgComplete = null;
				objectiveViewData.ObjectiveBgActive = null;
				objectiveViewData.ObjectiveBgCollected = null;
				objectiveViewData.SpecailObjectiveFx = null;
				objectiveViewData.ObjectiveBgExpired = null;
				if (objectiveViewData.GeoControlCrate != null)
				{
					objectiveViewData.GeoControlCrate.Destroy();
				}
				objectiveViewData.GeoControlCrate = null;
				if (objectiveViewData.GeoControlIcon != null)
				{
					objectiveViewData.GeoControlIcon.Destroy();
				}
				objectiveViewData.GeoControlIcon = null;
				objectiveViewData.ObjectiveContainer = null;
				objectiveViewData.ObjectiveContainerLEI = null;
				i++;
			}
		}
	}
}
