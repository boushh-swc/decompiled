using StaRTS.Main.Controllers.Objectives;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsLargeObjectivesViewModule : AbstractPlanetDetailsViewModule, IViewClockTimeObserver
	{
		private const int MAX_OBJECTIVES = 3;

		private const string OBJECTIVES_DETAILS = "ObjectivesDetails";

		private const string LABEL_HEADER = "LabelObjectivesDetailsHeader";

		private const string SPECIAL_EVENT_ICON = "TextureObjectiveDetailsSpecial";

		private const string SPECIAL_EVENT_FRAME = "TextureObjectiveDetailsFrameSpecial";

		private const string LABEL_TIMER = "LabelObjectivesDetailsTimer";

		private const string BTN_BACK = "BtnBackToPlanet";

		private const string GRID_OBJECTIVE = "GridObjective";

		private const string TEMPLATE_OBJECTIVE = "TemplateObjective";

		private const string SPRITE_CHECKMARK = "SpriteCheckmark";

		private const string LABEL_OBJ_NAME = "LabelObjectiveDetailName";

		private const string LABEL_OBJ_STATUS = "LabelObjectiveDetailStatus";

		private const string BTN_SUPPLY_CRATE = "BtnSupplyCrate";

		private const string SPRITE_SUPPLY_CRATE = "SpriteSupplyCrate";

		private const string SPRITE_OBJECTIVE_ICON = "SpriteUnit";

		private const string SLIDER = "PBarObjectiveDetail";

		private const string LABEL_RELOCATION = "LabelRelocationRequirement";

		private const string OBJECTIVE_BACKGROUND_COMPLETE = "TemplateObjectiveCompleteBg";

		private const string OBJECTIVE_BACKGROUND_EXPIRED = "TemplateObjectiveExpiredBg";

		private const string OBJECTIVE_BACKGROUND_PROGRESS = "TemplateObjectiveProgressBg";

		private const string OBJECTIVE_BACKGROUND_COLLECTED = "TemplateObjectiveCollectedBg";

		private const string SPECIAL_OBJECTIVE_FX = "CrateDetailEffect";

		private const string OBJECTIVE_DETAIL_EXTRA_BACK_BUTTON = "BtnBackToPlanetFullScreen";

		private const string CONTAINER = "TemplateObjectiveProgressBg";

		private const string CONTAINER_LEI = "TemplateObjectiveSpecialBg";

		private const string RELOCATE_TO_PLANET_MESSAGE = "RELOCATE_TO_PLANET_MESSAGE";

		private const string OBJECTIVE_DETAILS_HEADER = "OBJECTIVE_DETAILS_HEADER";

		private const string OBJECTIVE_DETAILS_HEADER_EXPIRED = "OBJECTIVES_DETAILS_HEADER_EXPIRED";

		private const string NO_OBJECTIVES_AVAILABLE = "NO_OBJECTIVES_AVAILABLE";

		private UXElement details;

		private UXButton btnBack;

		private UXGrid grid;

		private UXLabel header;

		private UXLabel timer;

		private UXLabel relocation;

		private UXElement graceIndicator;

		private UXTexture specialEventIcon;

		private UXTexture specialEventFrame;

		private ObjectiveController objectiveController;

		private CurrentPlayer player;

		private Lang lang;

		private string tempTimeString = string.Empty;

		private bool tempIsGrace;

		public PlanetDetailsLargeObjectivesViewModule(PlanetDetailsScreen screen) : base(screen)
		{
			this.player = Service.CurrentPlayer;
			this.objectiveController = Service.ObjectiveController;
			this.lang = Service.Lang;
		}

		public void OnScreenLoaded()
		{
			this.details = this.screen.GetElement<UXElement>("ObjectivesDetails");
			if (!this.objectiveController.ShouldShowObjectives())
			{
				return;
			}
			Service.ViewTimerManager.CreateViewTimer(0f, false, new TimerDelegate(this.OnDelayedScreenLoad), null);
		}

		private void OnDelayedScreenLoad(uint timerId, object cookie)
		{
			this.screen.GetElement<UXButton>("BtnBackToPlanetFullScreen").OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.header = this.screen.GetElement<UXLabel>("LabelObjectivesDetailsHeader");
			this.timer = this.screen.GetElement<UXLabel>("LabelObjectivesDetailsTimer");
			this.relocation = this.screen.GetElement<UXLabel>("LabelRelocationRequirement");
			this.specialEventIcon = this.screen.GetElement<UXTexture>("TextureObjectiveDetailsSpecial");
			this.specialEventFrame = this.screen.GetElement<UXTexture>("TextureObjectiveDetailsFrameSpecial");
			this.specialEventIcon.Visible = false;
			this.specialEventFrame.Visible = false;
			this.btnBack = this.screen.GetElement<UXButton>("BtnBackToPlanet");
			this.btnBack.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.grid = this.screen.GetElement<UXGrid>("GridObjective");
			this.grid.Clear();
			this.grid.SetTemplateItem("TemplateObjective");
			for (int i = 0; i < 3; i++)
			{
				string itemUid = i.ToString();
				UXElement uXElement = this.grid.CloneTemplateItem(itemUid);
				ObjectiveViewData objectiveViewData = new ObjectiveViewData();
				objectiveViewData.BtnSupplyCrate = this.grid.GetSubElement<UXButton>(itemUid, "BtnSupplyCrate");
				objectiveViewData.BtnSupplyCrate.OnClicked = new UXButtonClickedDelegate(this.objectiveController.OnCrateClickedFromDetail);
				objectiveViewData.BtnSupplyCrate.Tag = objectiveViewData;
				objectiveViewData.SpriteCheckmark = this.grid.GetSubElement<UXSprite>(itemUid, "SpriteCheckmark");
				objectiveViewData.SpriteSupplyCrate = this.grid.GetSubElement<UXSprite>(itemUid, "SpriteSupplyCrate");
				objectiveViewData.ObjectiveBgComplete = this.grid.GetSubElement<UXElement>(itemUid, "TemplateObjectiveCompleteBg");
				objectiveViewData.ObjectiveBgCollected = this.grid.GetSubElement<UXElement>(itemUid, "TemplateObjectiveCollectedBg");
				objectiveViewData.ObjectiveBgActive = this.grid.GetSubElement<UXElement>(itemUid, "TemplateObjectiveProgressBg");
				objectiveViewData.SpecailObjectiveFx = this.grid.GetSubElement<UXElement>(itemUid, "CrateDetailEffect");
				objectiveViewData.ObjectiveBgExpired = this.grid.GetSubElement<UXElement>(itemUid, "TemplateObjectiveExpiredBg");
				objectiveViewData.StatusLabel = this.grid.GetSubElement<UXLabel>(itemUid, "LabelObjectiveDetailStatus");
				objectiveViewData.TitleLabel = this.grid.GetSubElement<UXLabel>(itemUid, "LabelObjectiveDetailName");
				objectiveViewData.ProgressSlider = this.grid.GetSubElement<UXSlider>(itemUid, "PBarObjectiveDetail");
				objectiveViewData.SpriteObjectiveIcon = this.grid.GetSubElement<UXSprite>(itemUid, "SpriteUnit");
				objectiveViewData.ObjectiveContainer = this.grid.GetSubElement<UXElement>(itemUid, "TemplateObjectiveProgressBg");
				objectiveViewData.ObjectiveContainerLEI = this.grid.GetSubElement<UXElement>(itemUid, "TemplateObjectiveSpecialBg");
				uXElement.Tag = objectiveViewData;
				this.grid.AddItem(uXElement, i);
			}
			this.grid.RepositionItems();
			this.details.Visible = false;
		}

		public void RefreshScreenForPlanetChange()
		{
			if (!this.objectiveController.ShouldShowObjectives())
			{
				return;
			}
			if (this.specialEventIcon == null || this.specialEventFrame == null || this.player.Objectives == null || this.screen.viewingPlanetVO == null || this.grid == null || this.relocation == null)
			{
				return;
			}
			this.tempIsGrace = true;
			this.specialEventIcon.Visible = false;
			this.specialEventFrame.Visible = false;
			string planetDisplayName = LangUtils.GetPlanetDisplayName(this.screen.viewingPlanetVO);
			if (this.player.Objectives.ContainsKey(this.screen.viewingPlanetVO.Uid))
			{
				this.objectiveController.GetTimeData(this.lang, this.player.Objectives[this.screen.viewingPlanetVO.Uid], ref this.tempIsGrace, ref this.tempTimeString);
				ObjectiveGroup objectiveGroup = this.player.Objectives[this.screen.viewingPlanetVO.Uid];
				if (objectiveGroup == null)
				{
					Service.Logger.WarnFormat("Player objectives for planet {0} are null", new object[]
					{
						this.screen.viewingPlanetVO.Uid
					});
					return;
				}
				ObjectiveSeriesVO objectiveSeriesVO = Service.StaticDataController.Get<ObjectiveSeriesVO>(objectiveGroup.GroupSeriesId);
				if (objectiveGroup.ProgressObjects != null && objectiveGroup.ProgressObjects.Count > 0)
				{
					this.specialEventIcon.Visible = objectiveSeriesVO.SpecialEvent;
					this.specialEventFrame.Visible = objectiveSeriesVO.SpecialEvent;
				}
				if (objectiveSeriesVO.SpecialEvent)
				{
					this.specialEventIcon.LoadTexture(objectiveSeriesVO.EventIcon);
					this.specialEventFrame.LoadTexture(objectiveSeriesVO.EventDetailsArt);
				}
				int i = 0;
				int num = 3;
				while (i < num)
				{
					UXElement item = this.grid.GetItem(i);
					ObjectiveViewData objectiveViewData = item.Tag as ObjectiveViewData;
					if (i >= objectiveGroup.ProgressObjects.Count)
					{
						item.Visible = false;
					}
					else
					{
						item.Visible = true;
						objectiveViewData.SpecailObjectiveFx.Visible = objectiveSeriesVO.SpecialEvent;
						objectiveViewData.Objective = objectiveGroup.ProgressObjects[i];
						this.objectiveController.UpdateObjectiveEntry(objectiveViewData, this.tempIsGrace);
					}
					i++;
				}
			}
			else
			{
				this.header.Text = this.lang.Get("OBJECTIVE_DETAILS_HEADER", new object[]
				{
					planetDisplayName
				});
				int j = 0;
				int num2 = 3;
				while (j < num2)
				{
					UXElement item2 = this.grid.GetItem(j);
					item2.Visible = false;
					j++;
				}
			}
			if (this.screen.viewingPlanetVO == this.player.Planet || this.tempIsGrace)
			{
				this.relocation.Text = string.Empty;
			}
			else
			{
				this.relocation.Text = this.lang.Get("RELOCATE_TO_PLANET_MESSAGE", new object[]
				{
					planetDisplayName
				});
			}
			this.OnViewClockTime(0f);
		}

		public void OnBackButtonClicked(UXButton button)
		{
			this.screen.HideObjectivesUI();
		}

		public void Show()
		{
			this.details.Visible = true;
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		public void Hide()
		{
			this.details.Visible = false;
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
		}

		public void OnViewClockTime(float dt)
		{
			string planetDisplayName = LangUtils.GetPlanetDisplayName(this.screen.viewingPlanetVO);
			if (!this.player.Objectives.ContainsKey(this.screen.viewingPlanetVO.Uid) || this.player.Objectives[this.screen.viewingPlanetVO.Uid].ProgressObjects.Count <= 0)
			{
				this.timer.Text = this.lang.Get("NO_OBJECTIVES_AVAILABLE", new object[0]);
				this.timer.TextColor = ObjectiveController.TEXT_WHITE_COLOR;
				this.header.Text = this.lang.Get("OBJECTIVE_DETAILS_HEADER", new object[]
				{
					planetDisplayName
				});
				return;
			}
			ObjectiveGroup objectiveGroup = this.player.Objectives[this.screen.viewingPlanetVO.Uid];
			ObjectiveSeriesVO objectiveSeriesVO = Service.StaticDataController.Get<ObjectiveSeriesVO>(objectiveGroup.GroupSeriesId);
			this.objectiveController.GetTimeData(this.lang, objectiveGroup, ref this.tempIsGrace, ref this.tempTimeString);
			if (objectiveSeriesVO.SpecialEvent)
			{
				this.header.Text = ((!this.tempIsGrace) ? this.lang.Get(objectiveSeriesVO.ObjectiveString, new object[0]) : this.lang.Get(objectiveSeriesVO.ObjectiveExpiringString, new object[0]));
			}
			else
			{
				this.header.Text = ((!this.tempIsGrace) ? this.lang.Get("OBJECTIVE_DETAILS_HEADER", new object[]
				{
					planetDisplayName
				}) : this.lang.Get("OBJECTIVES_DETAILS_HEADER_EXPIRED", new object[]
				{
					planetDisplayName
				}));
			}
			this.timer.Text = this.tempTimeString;
			this.timer.TextColor = ((!this.tempIsGrace) ? ObjectiveController.TEXT_RED_COLOR : ObjectiveController.TEXT_YELLOW_COLOR);
		}

		public void OnClose()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			if (this.grid != null)
			{
				for (int i = 0; i < 3; i++)
				{
					UXElement item = this.grid.GetItem(i);
					if (item != null && item.Tag != null)
					{
						ObjectiveViewData objectiveViewData = item.Tag as ObjectiveViewData;
						objectiveViewData.BtnSupplyCrate = null;
						objectiveViewData.Objective = null;
						objectiveViewData.SpriteCheckmark = null;
						objectiveViewData.SpriteSupplyCrate = null;
						objectiveViewData.ObjectiveBgComplete = null;
						objectiveViewData.ObjectiveBgActive = null;
						objectiveViewData.ObjectiveBgCollected = null;
						objectiveViewData.ObjectiveBgExpired = null;
						if (objectiveViewData.GeoControlCrate != null)
						{
							objectiveViewData.GeoControlCrate.Destroy();
						}
						objectiveViewData.GeoControlCrate = null;
						objectiveViewData.ObjectiveContainer = null;
						objectiveViewData.ObjectiveContainerLEI = null;
					}
				}
				this.grid.Clear();
			}
		}
	}
}
