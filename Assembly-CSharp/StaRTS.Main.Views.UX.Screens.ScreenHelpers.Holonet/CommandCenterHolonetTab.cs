using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Holonet;
using StaRTS.Main.Controllers.ShardShop;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet
{
	public class CommandCenterHolonetTab : AbstractHolonetTab, IViewFrameTimeObserver, IEventObserver, IViewClockTimeObserver
	{
		private const int devNoteLength = 120;

		private const float WARBOARD_TRANSITION_DELAY = 0.002f;

		private const float DAILY_CRATE_PARTICLE_DELAY = 1.5f;

		private const int MAX_SHARD_SHOP_ITEMS = 3;

		private ScreenParticleFXCookie dailyParticleFx;

		private const string FRAGMENT_STORE_GROUP = "FragmentStoreGroup";

		private const string LABEL_TITLE_FRAGMENT_STORE = "LabelFragmentStore";

		private const string LABEL_FRAGMENT_STORE_EXPIRATION = "LabelFragmentStoreExpiration";

		private const string BTN_LABEL_FRAGMENT_GO_TO_STORE = "BtnLabelNotesPanelFragmentStore";

		private const string BTN_FRAGMENT_STORE_SEE_DEALS = "BtnFragmentStoreSeeDeals";

		private const string GRID_FRAGMENT_CONTAINER = "GridFragmentContainer";

		private const string TEMPLATE_FRAGMENT_CARD = "TemplateFragmentCard";

		private const string SPRITE_FRAGMENT_IMAGE = "SpriteFragmentImage";

		private const string TEXTURE_FRAGMENT_STORE_BG = "TextureFragmentStoreBackground";

		private const string SHARD_SHOP_HOLO_OFFER_EXPIRES_STRINGID = "shard_shop_holo_offer_expires";

		private const string SHARD_SHOP_HOLO_GO_TO_STORE_STRINGID = "shard_shop_holo_go_to_store";

		private const string DAILY_CRATE_PANEL_GROUP = "PanelGroupDailyCrate";

		private const string DAILY_CRATE_PANEL_BG = "PanelBgDailyCrate";

		private const string DAILY_CRATE_PROJ_SPRITE = "SpriteDailyCrateImage";

		private const string DAILY_CRATE_BG_TEXTURE = "TextureBgDailyCrate";

		private const string DAILY_CRATE_ROOM_IMAGE = "supplycratefloor";

		private const string DAILY_CRATE_TITLE_LABEL = "LabelTitleDailyCrate";

		private const string DAILY_CRATE_TIMER_LABEL = "LabelCountdownDailyCrate";

		private const string DAILY_CRATE_GET_MORE_BTN = "BtnGetMoreDailyCrate";

		private const string DAILY_CRATE_GET_MORE_LABEL = "LabelBtnGetMoreDailyCrate";

		private const string DAILY_CRATE_OPEN_CRATE_BTN = "BtnPrimaryDailyCrate";

		private const string DAILY_CRATE_OPEN_CRATE_LABEL = "LabelBtnPrimaryDailyCrate";

		private const string DAILY_CRATE_SHADOW_TEXTURE = "TextureShadowDailyCrate";

		private const string DAILY_CRATE_AVAILABLE_OPEN_TITLE_STRINGID = "hn_ui_daily_crate_available";

		private const string DAILY_CRATE_AVAILABLE_OPEN_BTN_STRINGID = "hn_ui_daily_crate_open_cta";

		private const string DAILY_CRATE_AVAILABLE_NEXT_TITLE_STRINGID = "hn_ui_daily_crate_next_available";

		private const string DAILY_CRATE_AVAILABLE_NEXT_BTN_STRINGID = "hn_ui_daily_crate_store_cta";

		private const string DAILY_CRATE_ANOTHER_WIDGET = "WidgetOpenAnotherCrate";

		private const string DAILY_CRATE_ANOTHER_BTN = "BtnOpenAnotherCrate";

		private const string DAILY_CRATE_ANOTHER_LABEL = "LabelBtnOpenAnotherCrate";

		private const string DAILY_CRATE_ANOTHER_STRINGID = "s_OpenAnother";

		private const string DEFAULT_DAILY_CRATE_SHADOW = "shadow_dailycrate";

		private const string PAGE_DOT_GRID = "PageDotGrid";

		private const string PAGE_DOT = "PageDot";

		private const string FEATURE_PANEL_GRID = "FeaturePanelGrid";

		private const string NEWS_ITEM_FULL = "NewsItemFull";

		private const string NEWS_ITEM_HALF = "NewsItemHalf";

		private const string NEWS_ITEM_QUARTER = "NewsItemQuarter";

		private const string NEWS_ITEM_FULL_TITLE_LABEL = "NewsItemFullTitleLabel";

		private const string NEWS_ITEM_FULL_BODY_LABEL = "NewsItemFullBodyLabel";

		private const string NEWS_ITEM_FULL_BTN_ACTION_LABEL = "NewsItemFullBtnActionLabel";

		private const string NEWS_ITEM_FULL_BTN_ACTION = "NewsItemFullBtnAction";

		private const string NEWS_ITEM_FULL_IMAGE = "NewsItemFullImage";

		private const string NEWS_ITEM_HALF_TITLE_LABEL = "NewsItemHalfTitleLabel";

		private const string NEWS_ITEM_HALF_BODY_LABEL = "NewsItemHalfBodyLabel";

		private const string NEWS_ITEM_HALF_BTN_ACTION_LABEL = "NewsItemHalfBtnActionLabel";

		private const string NEWS_ITEM_HALF_BTN_ACTION = "NewsItemHalfBtnAction";

		private const string NEWS_ITEM_HALF_IMAGE = "NewsItemHalfImage";

		private const string NEWS_ITEM_QUARTER_TITLE_LABEL = "NewsItemQuarterTitleLabel";

		private const string NEWS_ITEM_QUARTER_BODY_LABEL = "NewsItemQuarterBodyLabel";

		private const string NEWS_ITEM_QUARTER_BTN_ACTION_LABEL = "NewsItemQuarterBtnActionLabel";

		private const string NEWS_ITEM_QUARTER_BTN_ICON_OPTION_LABEL = "NewsItemQuarterBtnActionIconOptionLabel";

		private const string NEWS_ITEM_QUARTER_BTN_OPTION_SPRITE = "NewsItemQuarterBtnActionIconOptionSprite";

		private const string NEWS_ITEM_QUARTER_BTN_ACTION = "NewsItemQuarterBtnAction";

		private const string NEWS_ITEM_QUARTER_BTN_ACTION_ICON_OPTION = "NewsItemQuarterBtnActionIconOption";

		private const string NEWS_ITEM_QUARTER_IMAGE = "NewsItemQuarterImage";

		private const string NEWS_ITEM_QUARTER_TABLE = "NewsItemQuarterTable";

		private const string NEWS_ITEM_QUARTER_BUTTON1 = "_1_";

		private const string NEWS_ITEM_QUARTER_BUTTON2 = "_2_";

		private const string NEWS_HN_FACEBOOK_CTA = "hn_facebook_cta";

		private const string NEWS_HN_TWITTER_CTA = "hn_twitter_cta";

		private const string FACTION_SPRITE_EMPIRE = "FactionEmpire";

		private const string FACTION_SPRITE_REBEL = "FactionRebel";

		private const string HOLONET_DEVELOPER_NOTES = "hn_cc_developer_notes";

		private const string HOLONET_READ_MORE = "hn_cc_read_more";

		private const string IMG_TAG = "[img]";

		private const string SOURCE_TAG = "src=";

		private const string SQUAD_WAR_GROUP = "SquadWarGroup";

		private const string SQUAD_WAR_LABEL_TITLE = "SquadWarTitleLabel";

		private const string SQUAD_WAR_BTN_LABEL = "BtnLabelSquadWar";

		private const string SQUAD_WAR_BTN = "BtnSquadWar";

		private const string SQUAD_WAR_TEXTURE = "SquadWarTexture";

		private const string SQUAD_WAR_START = "WarStart";

		private const string SQUAD_WAR_PREP_ACTION = "WarPrepAction";

		private const string SQUAD_WAR_TIMER_PREP_ACTION = "SquadWarTimerLabelPrepAction";

		private const string SQUAD_WAR_LABEL_LEFT_NAME = "SquadWarLabelLeftName";

		private const string SQUAD_WAR_LABEL_LEFT_SCORE = "SquadWarLabelLeftScore";

		private const string SQUAD_WAR_SPRITE_LEFT = "SpriteSquadWarLeft";

		private const string SQUAD_WAR_LABEL_RIGHT_NAME = "SquadWarLabelRightName";

		private const string SQUAD_WAR_LABEL_RIGHT_SCORE = "SquadWarLabelRightScore";

		private const string SQUAD_WAR_SPRITE_RIGHT = "SpriteSquadWarRight";

		private const string SQUAD_WAR_REWARD = "WarReward";

		private const string SQUAD_WAR_REWARD_LABEL = "SquadWarLabelWinner";

		private const string SQUAD_WAR_REWARD_DIRECTIONS = "SquadWarLabelDirections";

		private const string SQUAD_WAR_TITLE = "WAR_INTERSTITIAL_TITLE";

		private const string SQUAD_WAR_BTN_TEXT = "WAR_START_TITLE";

		private const string SQUAD_WAR_PREP_TIME_REMAINING = "WAR_BASE_PREP_TIME_REMAINING";

		private const string SQUAD_WAR_ACTION_TIME_REMAINING = "WAR_BASE_ACTION_TIME_REMAINING";

		private const string SQUAD_WAR_PREP_GRACE_TIME_REMAINING = "WAR_BOARD_PREP_GRACE_PHASE";

		private const string SQUAD_WAR_ACTION_GRACE_TIME_REMAINING = "WAR_BOARD_ACTION_GRACE_PHASE";

		private const string SQUAD_WAR_PREPARE = "WAR_HOLONET_PREPARE";

		private const string SQUAD_WAR_ACTION = "WAR_HOLONET_ACTION";

		private const string WAR_END_NEWSPAPER_TITLE = "WAR_END_NEWSPAPER_TITLE";

		private const string WAR_END_NEWSPAPER_DESC = "WAR_END_NEWSPAPER_DESC";

		private const string WAR_END_NEWSPAPER_CTA_DESC = "WAR_END_NEWSPAPER_CTA_DESC";

		private const string WAR_END_NEWSPAPER_CTA = "WAR_END_NEWSPAPER_CTA";

		private const int BOTTOM_RIGHT_VIEW_SHARD_SHOP = 0;

		private const int BOTTOM_RIGHT_VIEW_SQUAD_WAR = 1;

		private const string ICON_QUALITY_INT = "Quality";

		private UXElement notesGroup;

		private UXElement squadWarGroup;

		private UXTexture squadWarTexture;

		private UXElement fragmentStoreGroup;

		private UXLabel labelFragmentStore;

		private UXLabel labelFragmentStoreExpiration;

		private UXLabel btnLabelFragmentGoToStore;

		private UXButton btnFragmentStoreSeeDeals;

		private UXGrid gridFragmentContainer;

		private UXTexture fragmentStoreTextureBG;

		private UXElement squadWarStart;

		private UXElement squadWarPrepAction;

		private UXLabel squadWarTimerLabelPrepAction;

		private UXLabel squadWarLabelLeftName;

		private UXLabel squadWarLabelLeftScore;

		private UXLabel squadWarLabelRightName;

		private UXLabel squadWarLabelRightScore;

		private UXSprite squadWarLeftIcon;

		private UXSprite squadWarRightIcon;

		private UXElement squadWarReward;

		private UXGrid pageDotGrid;

		private UXGrid featureGrid;

		private UXElement newsItemFullTemplate;

		private UXElement newsItemHalfTemplate;

		private UXElement newsItemQuarterTemplate;

		private UXLabel newsItemFullTitleLabel;

		private UXLabel newsItemFullBodyLabel;

		private UXLabel newsItemFullBtnActionLabel;

		private UXButton newsItemFullBtnAction;

		private UXTexture newsItemFullImage;

		private float featureSwipeTimer;

		private UXElement nextAutoElement;

		private int previousCarouselIndex;

		private Dictionary<string, List<UXButton>> ctaButtonList;

		private Dictionary<string, List<UXLabel>> ctaLabelList;

		private int endActionPhaseTime;

		private int endPrepPhaseTime;

		private int endActionGracePhaseTime;

		private int endPrepGracePhaseTime;

		private UXElement dailyCratePanel;

		private UXElement dailyCratePanelBG;

		private UXLabel nextCrateTimeLabel;

		private CrateData dailyCrate;

		private UXSprite dailyCrateSprite;

		private GeometryProjector dailyCrateProj;

		private UXTexture crateShadow;

		private ScreenParticleHandler particleHandler;

		private List<GeometryProjector> projectorCleanupList;

		private int bottomRightViewType;

		public CommandCenterHolonetTab(HolonetScreen screen, HolonetControllerType holonetControllerType) : base(screen, holonetControllerType)
		{
			InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
			base.InitializeTab("CommandCenterTab", "hn_commandcenter_tab");
			this.dailyCrate = crates.GetDailyCrateIfAvailable();
			this.ctaButtonList = new Dictionary<string, List<UXButton>>();
			this.ctaLabelList = new Dictionary<string, List<UXLabel>>();
			this.projectorCleanupList = new List<GeometryProjector>();
			this.HideUnusedVideoUI();
			this.dailyCratePanel = screen.GetElement<UXElement>("PanelGroupDailyCrate");
			this.dailyCratePanelBG = screen.GetElement<UXElement>("PanelBgDailyCrate");
			this.dailyCrateSprite = screen.GetElement<UXSprite>("SpriteDailyCrateImage");
			this.nextCrateTimeLabel = screen.GetElement<UXLabel>("LabelCountdownDailyCrate");
			this.dailyCratePanel.Visible = false;
			this.dailyCratePanelBG.Visible = false;
			this.featureSwipeTimer = 0f;
			this.squadWarGroup = screen.GetElement<UXElement>("SquadWarGroup");
			this.fragmentStoreGroup = screen.GetElement<UXElement>("FragmentStoreGroup");
			this.fragmentStoreTextureBG = screen.GetElement<UXTexture>("TextureFragmentStoreBackground");
			this.SetupBottomRightPanel();
			this.pageDotGrid = screen.GetElement<UXGrid>("PageDotGrid");
			this.pageDotGrid.SetTemplateItem("PageDot");
			this.featureGrid = screen.GetElement<UXGrid>("FeaturePanelGrid");
			this.featureGrid.SetCenteredFinishedCallback(new UXGrid.OnCentered(this.OnCenteredFinished));
			this.newsItemFullTemplate = screen.GetElement<UXElement>("NewsItemFull");
			this.newsItemHalfTemplate = screen.GetElement<UXElement>("NewsItemHalf");
			this.newsItemQuarterTemplate = screen.GetElement<UXElement>("NewsItemQuarter");
			screen.GetElement<UXButton>("NewsItemQuarterBtnAction").Visible = false;
			this.newsItemFullTemplate.Visible = false;
			this.newsItemHalfTemplate.Visible = false;
			this.newsItemQuarterTemplate.Visible = false;
			List<CommandCenterVO> featuredItems = Service.HolonetController.CommandCenterController.FeaturedItems;
			int i = 0;
			int count = featuredItems.Count;
			while (i < count)
			{
				CommandCenterVO commandCenterVO = featuredItems[i];
				UXElement uXElement;
				switch (commandCenterVO.Layout)
				{
				case 1:
				{
					uXElement = this.featureGrid.CloneItem(commandCenterVO.Uid, this.newsItemFullTemplate);
					this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemFullTitleLabel").Text = this.lang.Get(commandCenterVO.TitleText, new object[0]);
					this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemFullBodyLabel").Text = this.lang.Get(commandCenterVO.BodyText, new object[0]);
					UXTexture subElement = this.featureGrid.GetSubElement<UXTexture>(commandCenterVO.Uid, "NewsItemFullImage");
					base.DeferTexture(subElement, commandCenterVO.Image);
					this.ctaButtonList.Add(commandCenterVO.Uid, new List<UXButton>());
					this.ctaLabelList.Add(commandCenterVO.Uid, new List<UXLabel>());
					UXButton subElement2 = this.featureGrid.GetSubElement<UXButton>(commandCenterVO.Uid, "NewsItemFullBtnAction");
					UXLabel subElement3 = this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemFullBtnActionLabel");
					this.ctaButtonList[commandCenterVO.Uid].Add(subElement2);
					this.ctaLabelList[commandCenterVO.Uid].Add(subElement3);
					base.PrepareButton(commandCenterVO, 1, subElement2, subElement3);
					break;
				}
				case 2:
				{
					uXElement = this.featureGrid.CloneItem(commandCenterVO.Uid, this.newsItemHalfTemplate);
					this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemHalfTitleLabel").Text = this.lang.Get(commandCenterVO.TitleText, new object[0]);
					this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemHalfBodyLabel").Text = this.lang.Get(commandCenterVO.BodyText, new object[0]);
					this.ctaButtonList.Add(commandCenterVO.Uid, new List<UXButton>());
					this.ctaLabelList.Add(commandCenterVO.Uid, new List<UXLabel>());
					UXButton subElement4 = this.featureGrid.GetSubElement<UXButton>(commandCenterVO.Uid, "NewsItemHalfBtnAction");
					UXLabel subElement5 = this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemHalfBtnActionLabel");
					this.ctaButtonList[commandCenterVO.Uid].Add(subElement4);
					this.ctaLabelList[commandCenterVO.Uid].Add(subElement5);
					base.PrepareButton(commandCenterVO, 1, subElement4, subElement5);
					UXTexture subElement6 = this.featureGrid.GetSubElement<UXTexture>(commandCenterVO.Uid, "NewsItemHalfImage");
					base.DeferTexture(subElement6, commandCenterVO.Image);
					break;
				}
				case 3:
				{
					uXElement = this.featureGrid.CloneItem(commandCenterVO.Uid, this.newsItemQuarterTemplate);
					this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemQuarterTitleLabel").Text = this.lang.Get(commandCenterVO.TitleText, new object[0]);
					this.featureGrid.GetSubElement<UXLabel>(commandCenterVO.Uid, "NewsItemQuarterBodyLabel").Text = this.lang.Get(commandCenterVO.BodyText, new object[0]);
					UXTexture subElement7 = this.featureGrid.GetSubElement<UXTexture>(commandCenterVO.Uid, "NewsItemQuarterImage");
					base.DeferTexture(subElement7, commandCenterVO.Image);
					UXTable subElement8 = this.featureGrid.GetSubElement<UXTable>(commandCenterVO.Uid, "NewsItemQuarterTable");
					if (!string.IsNullOrEmpty(commandCenterVO.Btn1))
					{
						this.QuarterButtonSetup(commandCenterVO, subElement8, 1, commandCenterVO.Btn1);
					}
					if (!string.IsNullOrEmpty(commandCenterVO.Btn2))
					{
						this.QuarterButtonSetup(commandCenterVO, subElement8, 2, commandCenterVO.Btn2);
					}
					subElement8.RepositionItemsFrameDelayed();
					break;
				}
				default:
					Service.Logger.Warn("Holonet Command Layout: " + featuredItems[i].Layout + " not supported.");
					uXElement = null;
					break;
				}
				if (uXElement != null)
				{
					this.featureGrid.AddItem(uXElement, i);
					uXElement.Tag = commandCenterVO;
					UXElement uXElement2 = this.pageDotGrid.CloneTemplateItem(commandCenterVO.Uid);
					this.pageDotGrid.AddItem(uXElement2, i);
					UXCheckbox uXCheckbox = (UXCheckbox)uXElement2;
					uXCheckbox.OnSelected = new UXCheckboxSelectedDelegate(this.FeaturedItemDotClicked);
					uXCheckbox.Tag = i;
					if (i == 0)
					{
						string cookie = commandCenterVO.Uid + "|auto";
						this.eventManager.SendEvent(EventId.HolonetCommandCenterFeature, cookie);
					}
				}
				i++;
			}
			screen.GetElement<UXTexture>("TextureBgDailyCrate").LoadTexture("supplycratefloor");
			this.pageDotGrid.RepositionItems();
			this.pageDotGrid.CenterElementsInPanel();
			this.featureGrid.RepositionItemsFrameDelayed();
		}

		private void HideUnusedVideoUI()
		{
			this.screen.GetElement<UXLabel>("VideoPanelFeaturedLabel").Visible = false;
			this.screen.GetElement<UXLabel>("VideoPanelTitleLabel").Visible = false;
			this.screen.GetElement<UXLabel>("VideoPanelCreatorLabel").Visible = false;
			this.screen.GetElement<UXElement>("VideoClippingPanel").Visible = false;
			this.screen.GetElement<UXSprite>("VideoPanelBGUserTypeMoreVideos").Visible = false;
			this.screen.GetElement<UXLabel>("VideoPanelLabelUserTypeMoreVideos").Visible = false;
			this.screen.GetElement<UXElement>("MakerContainerErrorMsgCC").Visible = false;
			this.screen.GetElement<UXButton>("BtnMoreVideosCCTab").Visible = false;
			this.screen.GetElement<UXElement>("LeadersPanelContainer").Visible = false;
		}

		private void SetupDailyCratePanel()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			Service.EventManager.RegisterObserver(this, EventId.CrateInventoryUpdated);
			this.particleHandler.HideParticleElement(this.dailyParticleFx);
			bool flag = this.dailyCrate != null;
			UXLabel element = this.screen.GetElement<UXLabel>("LabelTitleDailyCrate");
			UXButton element2 = this.screen.GetElement<UXButton>("BtnGetMoreDailyCrate");
			UXLabel element3 = this.screen.GetElement<UXLabel>("LabelBtnGetMoreDailyCrate");
			UXButton element4 = this.screen.GetElement<UXButton>("BtnPrimaryDailyCrate");
			UXLabel element5 = this.screen.GetElement<UXLabel>("LabelBtnPrimaryDailyCrate");
			UXElement element6 = this.screen.GetElement<UXElement>("WidgetOpenAnotherCrate");
			UXButton element7 = this.screen.GetElement<UXButton>("BtnOpenAnotherCrate");
			UXLabel element8 = this.screen.GetElement<UXLabel>("LabelBtnOpenAnotherCrate");
			element5.Text = this.lang.Get("hn_ui_daily_crate_open_cta", new object[0]);
			element3.Text = this.lang.Get("hn_ui_daily_crate_store_cta", new object[0]);
			element8.Text = this.lang.Get("s_OpenAnother", new object[0]);
			this.nextCrateTimeLabel.Visible = false;
			element2.Visible = false;
			element3.Visible = false;
			element4.Visible = false;
			element5.Visible = false;
			element6.Visible = false;
			element7.Visible = false;
			element.Visible = true;
			this.dailyCratePanel.Visible = true;
			this.dailyCratePanelBG.Visible = true;
			this.dailyCrateSprite.Visible = true;
			string text = string.Empty;
			bool flag2 = false;
			if (Service.MobileConnectorAdsController != null)
			{
				flag2 = Service.MobileConnectorAdsController.IsMobileConnectorAdAvailable();
			}
			string text2;
			if (!flag)
			{
				if (!flag2)
				{
					InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
					text2 = crates.GetNextDailyCrateId();
				}
				else
				{
					text2 = GameConstants.MOBILE_CONNECTOR_VIDEO_REWARD_CRATE;
				}
				text = this.lang.Get("hn_ui_daily_crate_next_available", new object[0]);
			}
			else
			{
				text = this.lang.Get("hn_ui_daily_crate_available", new object[0]);
				text2 = this.dailyCrate.CrateId;
			}
			element.Text = text;
			if (string.IsNullOrEmpty(text2))
			{
				Service.Logger.Error("CommandCenterHolonetTab.SetupDailyCratePanel Daily Crate Data missing crate CMS UID");
				return;
			}
			CrateVO optional = staticDataController.GetOptional<CrateVO>(text2);
			if (optional == null)
			{
				Service.Logger.Error("CommandCenterHolonetTab.SetupDailyCratePanel Daily Crate Data has invalid crate CMS UID " + text2);
				return;
			}
			string text3 = optional.HoloCrateShadowTextureName;
			this.crateShadow = this.screen.GetElement<UXTexture>("TextureShadowDailyCrate");
			if (string.IsNullOrEmpty(text3))
			{
				text3 = "shadow_dailycrate";
			}
			if (flag || flag2)
			{
				this.crateShadow.LoadTexture(text3);
			}
			if (this.dailyCrateProj != null)
			{
				this.dailyCrateProj.Destroy();
			}
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(optional, this.dailyCrateSprite);
			projectorConfig.AnimState = AnimState.Closed;
			projectorConfig.AnimPreference = AnimationPreference.AnimationAlways;
			projectorConfig.EnableCrateHoloShaderSwap = (!flag && !flag2);
			projectorConfig.CameraPosition = optional.HoloNetIconCameraPostion;
			projectorConfig.CameraInterest = optional.HoloNetIconLookAtPostion;
			if (projectorConfig.EnableCrateHoloShaderSwap && !string.IsNullOrEmpty(optional.UIColor))
			{
				projectorConfig.Tint = FXUtils.ConvertHexStringToColorObject(optional.UIColor);
			}
			this.dailyCrateProj = ProjectorUtils.GenerateProjector(projectorConfig);
			if (!flag)
			{
				if (flag2)
				{
					this.nextCrateTimeLabel.Visible = true;
					element7.Visible = true;
					element6.Visible = true;
					this.crateShadow.Visible = true;
					this.UpdateNextDailyCrateTimeLabel();
					element7.OnClicked = new UXButtonClickedDelegate(this.OnAnotherCrateClicked);
					this.ShowDailyCrateParticleFX(optional);
					if (this.screen.AutoDisplayMCACrate && Service.ScreenController.GetHighestLevelScreen<CrateInfoModalScreen>() == null)
					{
						this.OnAnotherCrateClicked(null);
					}
				}
				else
				{
					this.nextCrateTimeLabel.Visible = true;
					element2.Visible = true;
					element3.Visible = true;
					this.crateShadow.Visible = false;
					this.UpdateNextDailyCrateTimeLabel();
					element2.OnClicked = new UXButtonClickedDelegate(this.OnGetCrateClicked);
				}
			}
			else
			{
				element4.Visible = true;
				element5.Visible = true;
				this.crateShadow.Visible = true;
				element4.OnClicked = new UXButtonClickedDelegate(this.OnOpenCrateClicked);
				this.ShowDailyCrateParticleFX(optional);
			}
		}

		private void UpdateNextDailyCrateTimeLabel()
		{
			InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
			uint nextDailyCrateTime = crates.NextDailyCrateTime;
			uint time = ServerTime.Time;
			string text = string.Empty;
			if (nextDailyCrateTime >= time)
			{
				uint num = nextDailyCrateTime - time;
				text = LangUtils.FormatTime((long)((ulong)num));
			}
			this.nextCrateTimeLabel.Text = text;
		}

		private void OnGetCrateClicked(UXButton btn)
		{
			this.screen.Close(null);
			GameUtils.OpenStoreWithTab(StoreTab.Treasure);
		}

		private void OnOpenCrateClicked(UXButton btn)
		{
			GameUtils.OpenInventoryCrateModal(this.dailyCrate, new OnScreenModalResult(this.OnCrateInfoModalClosed));
		}

		private void OnAnotherCrateClicked(UXButton btn)
		{
			string planetId = Service.CurrentPlayer.PlanetId;
			string mOBILE_CONNECTOR_VIDEO_REWARD_CRATE = GameConstants.MOBILE_CONNECTOR_VIDEO_REWARD_CRATE;
			CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForMobileConnectorAd(mOBILE_CONNECTOR_VIDEO_REWARD_CRATE, planetId);
			crateInfoModalScreen.OnModalResult = new OnScreenModalResult(this.OnMobileConnectorCrateInfoClosed);
			crateInfoModalScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(crateInfoModalScreen, true, false);
		}

		private void OnCrateInfoModalClosed(object result, object cookie)
		{
			if (cookie != null || this.dailyCrate == null)
			{
				Service.HolonetController.RegisterForHolonetToReopenAfterCrateReward();
				this.screen.Close(null);
			}
		}

		private void OnMobileConnectorCrateInfoClosed(object result, object cookie)
		{
			if (cookie != null)
			{
				Service.HolonetController.RegisterForHolonetToReopenAfterCrateReward();
				this.screen.Close(null);
			}
		}

		private void ShowDailyCrateParticleFX(CrateVO crateVO)
		{
			string holoParticleEffectId = crateVO.HoloParticleEffectId;
			if (string.IsNullOrEmpty(holoParticleEffectId))
			{
				Service.Logger.ErrorFormat("Daily Crate missing holonet FX in crate:{0}", new object[]
				{
					crateVO.Uid
				});
				return;
			}
			if (this.dailyParticleFx != null)
			{
				this.particleHandler.HideParticleElement(this.dailyParticleFx);
			}
			this.dailyParticleFx = new ScreenParticleFXCookie(1.5f, holoParticleEffectId);
			this.particleHandler.ScheduleParticleFX(this.dailyParticleFx);
		}

		private void SetupBottomRightPanel()
		{
			this.squadWarGroup.Visible = false;
			this.fragmentStoreGroup.Visible = false;
			int num = Service.CurrentPlayer.Map.FindHighestHqLevel();
			int sHARD_SHOP_MINIMUM_HQ = GameConstants.SHARD_SHOP_MINIMUM_HQ;
			ShardShopController shardShopController = Service.ShardShopController;
			if (num >= sHARD_SHOP_MINIMUM_HQ && shardShopController.CurrentShopData != null && shardShopController.CurrentShopData.Expiration > DateUtils.GetNowSeconds())
			{
				this.bottomRightViewType = 0;
				this.SetupShardShopView();
			}
			else
			{
				this.bottomRightViewType = 1;
				this.SetupSquadWarInfo();
			}
			Service.EventManager.RegisterObserver(this, EventId.ShardOfferChanged);
		}

		private void SetupSquadWarInfo()
		{
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarData currentSquadWar = warManager.CurrentSquadWar;
			SquadWarStatusType currentStatus = warManager.GetCurrentStatus();
			bool flag = Service.CurrentPlayer.Faction == FactionType.Empire;
			this.squadWarTexture = this.screen.GetElement<UXTexture>("SquadWarTexture");
			UXLabel element = this.screen.GetElement<UXLabel>("SquadWarTitleLabel");
			UXLabel element2 = this.screen.GetElement<UXLabel>("BtnLabelSquadWar");
			UXButton element3 = this.screen.GetElement<UXButton>("BtnSquadWar");
			this.squadWarStart = this.screen.GetElement<UXElement>("WarStart");
			this.squadWarPrepAction = this.screen.GetElement<UXElement>("WarPrepAction");
			this.squadWarTimerLabelPrepAction = this.screen.GetElement<UXLabel>("SquadWarTimerLabelPrepAction");
			this.squadWarLabelLeftName = this.screen.GetElement<UXLabel>("SquadWarLabelLeftName");
			this.squadWarLabelLeftScore = this.screen.GetElement<UXLabel>("SquadWarLabelLeftScore");
			this.squadWarLabelRightName = this.screen.GetElement<UXLabel>("SquadWarLabelRightName");
			this.squadWarLabelRightScore = this.screen.GetElement<UXLabel>("SquadWarLabelRightScore");
			this.squadWarLeftIcon = this.screen.GetElement<UXSprite>("SpriteSquadWarLeft");
			this.squadWarRightIcon = this.screen.GetElement<UXSprite>("SpriteSquadWarRight");
			this.squadWarReward = this.screen.GetElement<UXElement>("WarReward");
			this.squadWarGroup.Visible = true;
			element.Text = this.lang.Get("WAR_INTERSTITIAL_TITLE", new object[0]);
			element3.OnClicked = new UXButtonClickedDelegate(this.OnSquadWarBtnClicked);
			element3.Visible = true;
			this.squadWarStart.Visible = false;
			this.squadWarPrepAction.Visible = false;
			this.squadWarReward.Visible = false;
			StaticDataController staticDataController = Service.StaticDataController;
			string text = null;
			switch (currentStatus)
			{
			case SquadWarStatusType.PhaseOpen:
				this.squadWarStart.Visible = true;
				element2.Text = this.lang.Get("WAR_START_TITLE", new object[0]);
				text = ((!flag) ? GameConstants.HOLONET_TEXTURE_WAR_REBEL_OPEN : GameConstants.HOLONET_TEXTURE_WAR_EMPIRE_OPEN);
				break;
			case SquadWarStatusType.PhasePrep:
			case SquadWarStatusType.PhasePrepGrace:
			case SquadWarStatusType.PhaseAction:
			case SquadWarStatusType.PhaseActionGrace:
			{
				uint serverTime = Service.ServerAPI.ServerTime;
				SquadWarSquadData squadWarSquadData = currentSquadWar.Squads[0];
				SquadWarSquadData squadWarSquadData2 = currentSquadWar.Squads[1];
				this.squadWarPrepAction.Visible = true;
				SquadWarSquadData squadWarSquadData3 = squadWarSquadData;
				SquadWarSquadData squadWarSquadData4 = squadWarSquadData2;
				SquadWarSquadType squadType = SquadWarSquadType.PLAYER_SQUAD;
				SquadWarSquadType squadType2 = SquadWarSquadType.OPPONENT_SQUAD;
				this.squadWarLeftIcon.SpriteName = ((squadWarSquadData.Faction != FactionType.Rebel) ? "FactionEmpire" : "FactionRebel");
				this.squadWarRightIcon.SpriteName = ((squadWarSquadData2.Faction != FactionType.Rebel) ? "FactionEmpire" : "FactionRebel");
				int currentSquadScore = warManager.GetCurrentSquadScore(squadType);
				int currentSquadScore2 = warManager.GetCurrentSquadScore(squadType2);
				this.squadWarLabelLeftName.Text = squadWarSquadData3.SquadName;
				this.squadWarLabelLeftScore.Text = currentSquadScore.ToString();
				this.squadWarLabelRightName.Text = squadWarSquadData4.SquadName;
				this.squadWarLabelRightScore.Text = currentSquadScore2.ToString();
				switch (currentStatus)
				{
				case SquadWarStatusType.PhasePrep:
					this.endPrepPhaseTime = currentSquadWar.PrepGraceStartTimeStamp - (int)serverTime;
					break;
				case SquadWarStatusType.PhasePrepGrace:
					this.endPrepGracePhaseTime = currentSquadWar.PrepEndTimeStamp - (int)serverTime;
					break;
				case SquadWarStatusType.PhaseAction:
					this.endActionPhaseTime = currentSquadWar.ActionGraceStartTimeStamp - (int)serverTime;
					break;
				case SquadWarStatusType.PhaseActionGrace:
					this.endActionGracePhaseTime = currentSquadWar.ActionEndTimeStamp - (int)serverTime;
					break;
				}
				if (currentStatus == SquadWarStatusType.PhasePrep || currentStatus == SquadWarStatusType.PhasePrepGrace)
				{
					element2.Text = this.lang.Get("WAR_HOLONET_PREPARE", new object[0]);
					text = ((!flag) ? GameConstants.HOLONET_TEXTURE_WAR_REBEL_PREP : GameConstants.HOLONET_TEXTURE_WAR_EMPIRE_PREP);
				}
				else
				{
					element2.Text = this.lang.Get("WAR_HOLONET_ACTION", new object[0]);
					text = ((!flag) ? GameConstants.HOLONET_TEXTURE_WAR_REBEL_ACTION : GameConstants.HOLONET_TEXTURE_WAR_EMPIRE_ACTION);
				}
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				break;
			}
			case SquadWarStatusType.PhaseCooldown:
			{
				this.squadWarReward.Visible = true;
				text = ((!flag) ? GameConstants.HOLONET_TEXTURE_WAR_REBEL_COOLDOWN : GameConstants.HOLONET_TEXTURE_WAR_EMPIRE_COOLDOWN);
				element.Text = this.lang.Get("WAR_END_NEWSPAPER_TITLE", new object[0]);
				UXLabel element4 = this.screen.GetElement<UXLabel>("SquadWarLabelWinner");
				element4.Text = this.lang.Get("WAR_END_NEWSPAPER_DESC", new object[0]);
				UXLabel element5 = this.screen.GetElement<UXLabel>("SquadWarLabelDirections");
				element5.Text = this.lang.Get("WAR_END_NEWSPAPER_CTA_DESC", new object[0]);
				element2.Text = this.lang.Get("WAR_END_NEWSPAPER_CTA", new object[0]);
				break;
			}
			}
			if (!string.IsNullOrEmpty(text))
			{
				TextureVO optional = staticDataController.GetOptional<TextureVO>(text);
				if (optional != null)
				{
					this.squadWarTexture.LoadTexture(optional.AssetName);
				}
			}
		}

		private void SetupShardShopView()
		{
			ShardShopController shardShopController = Service.ShardShopController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			ShardShopData currentShopData = shardShopController.CurrentShopData;
			this.fragmentStoreGroup.Visible = true;
			int playerHq = currentPlayer.Map.FindHighestHqLevel();
			int num = Math.Min(currentShopData.ShardOffers.Count, 3);
			this.labelFragmentStore = this.screen.GetElement<UXLabel>("LabelFragmentStore");
			this.labelFragmentStore.Text = this.lang.Get(currentShopData.ActiveSeries.InfonetTitle, new object[0]);
			string shardShopTextureBG = currentShopData.ActiveSeries.ShardShopTextureBG;
			if (!string.IsNullOrEmpty(shardShopTextureBG))
			{
				this.fragmentStoreTextureBG.Visible = true;
				this.fragmentStoreTextureBG.LoadTexture(shardShopTextureBG);
			}
			else
			{
				this.fragmentStoreTextureBG.Visible = false;
			}
			this.btnLabelFragmentGoToStore = this.screen.GetElement<UXLabel>("BtnLabelNotesPanelFragmentStore");
			this.btnLabelFragmentGoToStore.Text = this.lang.Get("shard_shop_holo_go_to_store", new object[0]);
			this.btnFragmentStoreSeeDeals = this.screen.GetElement<UXButton>("BtnFragmentStoreSeeDeals");
			this.btnFragmentStoreSeeDeals.OnClicked = new UXButtonClickedDelegate(this.OnFragmentStoreClicked);
			this.labelFragmentStoreExpiration = this.screen.GetElement<UXLabel>("LabelFragmentStoreExpiration");
			CountdownControl countdownControl = new CountdownControl(this.labelFragmentStoreExpiration, this.lang.Get("shard_shop_holo_offer_expires", new object[0]), (int)shardShopController.CurrentShopData.Expiration);
			countdownControl.SetOffsetMinutes((int)currentShopData.OffsetMinutes);
			this.gridFragmentContainer = this.screen.GetElement<UXGrid>("GridFragmentContainer");
			this.gridFragmentContainer.Clear();
			this.CleanupProjectors();
			this.gridFragmentContainer.SetTemplateItem("TemplateFragmentCard");
			this.screen.GetElement<UXElement>("TemplateFragmentCard").Visible = true;
			for (int i = 0; i < num; i++)
			{
				string shardSlotId = GameUtils.GetShardSlotId(i);
				CrateSupplyVO crateSupplyVO = currentShopData.ShardOffers[shardSlotId];
				UXElement uXElement = this.gridFragmentContainer.CloneTemplateItem(shardSlotId);
				UXSprite subElement = this.gridFragmentContainer.GetSubElement<UXSprite>(shardSlotId, "SpriteFragmentImage");
				IGeometryVO geometryVO = GameUtils.GetIconVOFromCrateSupply(crateSupplyVO, playerHq);
				if (geometryVO != null)
				{
					if (crateSupplyVO.Type == SupplyType.Shard)
					{
						geometryVO = ProjectorUtils.DetermineVOForEquipment((EquipmentVO)geometryVO);
					}
					ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(geometryVO, subElement);
					GeometryProjector item = ProjectorUtils.GenerateProjector(config);
					this.projectorCleanupList.Add(item);
				}
				else
				{
					Service.Logger.ErrorFormat("CommandCenterHolonetTab: Could not generate geometry for crate supply {0}", new object[]
					{
						crateSupplyVO.Uid
					});
				}
				this.screen.RevertToOriginalNameRecursively(uXElement.Root, shardSlotId);
				this.gridFragmentContainer.AddItem(uXElement, i);
				int value = 0;
				if (crateSupplyVO.Type == SupplyType.ShardTroop || crateSupplyVO.Type == SupplyType.ShardSpecialAttack)
				{
					ShardVO optional = Service.StaticDataController.GetOptional<ShardVO>(crateSupplyVO.RewardUid);
					value = (int)optional.Quality;
				}
				else if (crateSupplyVO.Type == SupplyType.Shard)
				{
					EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(crateSupplyVO.RewardUid);
					value = (int)currentEquipmentDataByID.Quality;
				}
				Animator component = uXElement.Root.GetComponent<Animator>();
				component.SetInteger("Quality", value);
			}
			this.screen.GetElement<UXElement>("TemplateFragmentCard").Visible = false;
		}

		private void OnFragmentStoreClicked(UXButton btn)
		{
			this.screen.Close(null);
			Service.EventManager.SendEvent(EventId.GoToShardShopClickedFromHolonet, null);
			GameUtils.OpenStoreWithTab(StoreTab.Fragments);
		}

		private void OnSquadWarBtnClicked(UXButton btn)
		{
			this.screen.Close(null);
			Service.ViewTimerManager.CreateViewTimer(0.002f, false, new TimerDelegate(this.TransitionToWarBoard), null);
		}

		private void TransitionToWarBoard(uint timerId, object cookie)
		{
			Service.EventManager.SendEvent(EventId.WarLaunchFlow, null);
		}

		private void QuarterButtonSetup(CommandCenterVO vo, UXTable buttonsTable, int index, string btn)
		{
			if (!this.ctaButtonList.ContainsKey(vo.Uid))
			{
				this.ctaButtonList.Add(vo.Uid, new List<UXButton>());
			}
			if (!this.ctaLabelList.ContainsKey(vo.Uid))
			{
				this.ctaLabelList.Add(vo.Uid, new List<UXLabel>());
			}
			string text = vo.Uid + index;
			UXButton uXButton;
			string name;
			if (btn == "hn_facebook_cta" || btn == "hn_twitter_cta")
			{
				string templateItem = UXUtils.FormatAppendedName("NewsItemQuarterBtnActionIconOption", vo.Uid);
				buttonsTable.SetTemplateItem(templateItem);
				uXButton = (buttonsTable.CloneTemplateItem(text) as UXButton);
				name = UXUtils.FormatAppendedName("NewsItemQuarterBtnActionIconOptionLabel", vo.Uid);
				string text2 = UXUtils.FormatAppendedName("NewsItemQuarterBtnActionIconOptionSprite", vo.Uid);
				text2 = UXUtils.FormatAppendedName(text2, text);
				UXSprite element = this.screen.GetElement<UXSprite>(text2);
				if (btn == "hn_facebook_cta")
				{
					element.SpriteName = "icoFacebook";
				}
				else if (btn == "hn_twitter_cta")
				{
					element.SpriteName = "icoTwitter";
				}
			}
			else
			{
				string templateItem = UXUtils.FormatAppendedName("NewsItemQuarterBtnAction", vo.Uid);
				buttonsTable.SetTemplateItem(templateItem);
				uXButton = (buttonsTable.CloneTemplateItem(text) as UXButton);
				name = UXUtils.FormatAppendedName("NewsItemQuarterBtnActionLabel", vo.Uid);
			}
			UXLabel subElement = buttonsTable.GetSubElement<UXLabel>(text, name);
			this.ctaButtonList[vo.Uid].Add(uXButton);
			this.ctaLabelList[vo.Uid].Add(subElement);
			buttonsTable.AddItem(uXButton, buttonsTable.Count);
			base.PrepareButton(vo, index, uXButton, subElement);
		}

		public void OnCenteredFinished(UXElement element, int index)
		{
			if (this.nextAutoElement != null)
			{
				this.nextAutoElement = null;
			}
		}

		protected override void FeaturedButton1Clicked(UXButton button)
		{
			CommandCenterVO commandCenterVO = (CommandCenterVO)button.Tag;
			Service.HolonetController.HandleCallToActionButton(commandCenterVO.Btn1Action, commandCenterVO.Btn1Data, commandCenterVO.Uid);
			this.RefreshButtons(commandCenterVO);
			base.SendCallToActionBI(commandCenterVO.Btn1Action, commandCenterVO.Uid, EventId.HolonetCommandCenterTab);
		}

		protected override void FeaturedButton2Clicked(UXButton button)
		{
			CommandCenterVO commandCenterVO = (CommandCenterVO)button.Tag;
			Service.HolonetController.HandleCallToActionButton(commandCenterVO.Btn2Action, commandCenterVO.Btn2Data, commandCenterVO.Uid);
			this.RefreshButtons(commandCenterVO);
			base.SendCallToActionBI(commandCenterVO.Btn2Action, commandCenterVO.Uid, EventId.HolonetCommandCenterTab);
		}

		private void RefreshButtons(CommandCenterVO vo)
		{
			if (!this.ctaButtonList.ContainsKey(vo.Uid) || this.ctaButtonList[vo.Uid] == null)
			{
				return;
			}
			if (this.ctaButtonList[vo.Uid].Count > 0)
			{
				base.PrepareButton(vo, 1, this.ctaButtonList[vo.Uid][0], this.ctaLabelList[vo.Uid][0]);
			}
			if (this.ctaButtonList[vo.Uid].Count > 1)
			{
				base.PrepareButton(vo, 2, this.ctaButtonList[vo.Uid][1], this.ctaLabelList[vo.Uid][1]);
			}
		}

		private void LinkToDevNotes(UXButton btn)
		{
			this.eventManager.SendEvent(EventId.HolonetDevNotes, "featured");
			this.screen.OpenTab(HolonetControllerType.DevNotes);
		}

		private void CleanupProjectors()
		{
			int count = this.projectorCleanupList.Count;
			for (int i = 0; i < count; i++)
			{
				this.projectorCleanupList[i].Destroy();
			}
			this.projectorCleanupList.Clear();
		}

		public override void OnDestroyTab()
		{
			if (this.particleHandler != null)
			{
				this.particleHandler.Destroy();
			}
			if (this.gridFragmentContainer != null)
			{
				this.gridFragmentContainer.Clear();
				this.gridFragmentContainer = null;
			}
			if (this.ctaButtonList != null)
			{
				foreach (KeyValuePair<string, List<UXButton>> current in this.ctaButtonList)
				{
					current.Value.Clear();
				}
				this.ctaButtonList.Clear();
			}
			this.ctaButtonList = null;
			if (this.ctaLabelList != null)
			{
				foreach (KeyValuePair<string, List<UXLabel>> current2 in this.ctaLabelList)
				{
					current2.Value.Clear();
				}
				this.ctaLabelList.Clear();
			}
			this.ctaLabelList = null;
			if (this.featureGrid != null)
			{
				this.featureGrid.Clear();
				this.featureGrid = null;
			}
			if (this.pageDotGrid != null)
			{
				this.pageDotGrid.Clear();
				this.pageDotGrid = null;
			}
			this.CleanupProjectors();
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.eventManager.UnregisterObserver(this, EventId.CrateInventoryUpdated);
			this.eventManager.UnregisterObserver(this, EventId.ShardOfferChanged);
			this.eventManager.UnregisterObserver(this, EventId.ScreenLoaded);
			this.eventManager.UnregisterObserver(this, EventId.ScreenClosing);
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
		}

		private void FeaturedItemDotClicked(UXCheckbox checkbox, bool selected)
		{
			if (selected)
			{
				this.featureGrid.SmoothScrollToItem((int)checkbox.Tag);
			}
		}

		public override void OnTabOpen()
		{
			base.OnTabOpen();
			this.particleHandler = new ScreenParticleHandler(this.screen);
			this.featureSwipeTimer = 0f;
			this.nextAutoElement = null;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.eventManager.RegisterObserver(this, EventId.ScreenLoaded);
			this.eventManager.RegisterObserver(this, EventId.ScreenClosing);
			this.eventManager.SendEvent(EventId.HolonetCommandCenterTab, "view");
			this.SetupDailyCratePanel();
		}

		public override void OnTabClose()
		{
			base.OnTabClose();
			if (this.particleHandler != null)
			{
				this.particleHandler.Destroy();
			}
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.eventManager.UnregisterObserver(this, EventId.ScreenLoaded);
			this.eventManager.UnregisterObserver(this, EventId.ScreenClosing);
		}

		public void OnViewFrameTime(float dt)
		{
			UXElement centeredElement = this.featureGrid.GetCenteredElement();
			if (centeredElement == null)
			{
				return;
			}
			List<UXElement> elementList = this.pageDotGrid.GetElementList();
			int num = this.featureGrid.GetElementList().IndexOf(centeredElement);
			if (0 <= num && num < elementList.Count)
			{
				UXCheckbox uXCheckbox = (UXCheckbox)elementList[num];
				uXCheckbox.Selected = true;
			}
			if (this.previousCarouselIndex != num)
			{
				this.previousCarouselIndex = num;
				if (this.nextAutoElement == null)
				{
					string cookie = (centeredElement.Tag as CommandCenterVO).Uid + "|manual";
					this.eventManager.SendEvent(EventId.HolonetCommandCenterFeature, cookie);
				}
			}
			if (Service.UserInputManager.IsPressed())
			{
				this.featureSwipeTimer = 0f;
				this.nextAutoElement = null;
			}
			else
			{
				this.featureSwipeTimer += dt;
			}
			float carouselAutoSwipe = (centeredElement.Tag as CommandCenterVO).CarouselAutoSwipe;
			float num2 = (carouselAutoSwipe <= 0f) ? GameConstants.HOLONET_FEATURE_CAROUSEL_AUTO_SWIPE : carouselAutoSwipe;
			if (this.featureSwipeTimer >= num2)
			{
				this.featureSwipeTimer = 0f;
				this.nextAutoElement = this.featureGrid.ScrollToNextElement();
				if (this.nextAutoElement != null)
				{
					string cookie2 = (this.nextAutoElement.Tag as CommandCenterVO).Uid + "|auto";
					this.eventManager.SendEvent(EventId.HolonetCommandCenterFeature, cookie2);
				}
			}
			if (this.dailyCrate == null)
			{
				this.UpdateNextDailyCrateTimeLabel();
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.CrateInventoryUpdated)
			{
				if (id == EventId.ShardOfferChanged)
				{
					this.SetupBottomRightPanel();
				}
			}
			else
			{
				InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
				this.dailyCrate = crates.GetDailyCrateIfAvailable();
				this.SetupDailyCratePanel();
			}
			return EatResponse.NotEaten;
		}

		public void OnViewClockTime(float dt)
		{
			if (this.bottomRightViewType == 1)
			{
				SquadWarManager warManager = Service.SquadController.WarManager;
				SquadWarStatusType currentStatus = warManager.GetCurrentStatus();
				string id = string.Empty;
				int num = 0;
				switch (currentStatus)
				{
				case SquadWarStatusType.PhasePrep:
					num = --this.endPrepPhaseTime;
					id = "WAR_BASE_PREP_TIME_REMAINING";
					break;
				case SquadWarStatusType.PhasePrepGrace:
					num = --this.endPrepGracePhaseTime;
					id = "WAR_BOARD_PREP_GRACE_PHASE";
					break;
				case SquadWarStatusType.PhaseAction:
					num = --this.endActionPhaseTime;
					id = "WAR_BASE_ACTION_TIME_REMAINING";
					break;
				case SquadWarStatusType.PhaseActionGrace:
					num = --this.endActionGracePhaseTime;
					id = "WAR_BOARD_ACTION_GRACE_PHASE";
					break;
				case SquadWarStatusType.PhaseCooldown:
					this.SetupBottomRightPanel();
					return;
				}
				if (num < 0)
				{
					num = 0;
				}
				this.squadWarTimerLabelPrepAction.Text = this.lang.Get(id, new object[]
				{
					GameUtils.GetTimeLabelFromSeconds(num)
				});
			}
		}

		public override string GetBITabName()
		{
			UXElement centeredElement = this.featureGrid.GetCenteredElement();
			string str = string.Empty;
			if (centeredElement != null && centeredElement.Tag != null)
			{
				str = "|" + (centeredElement.Tag as CommandCenterVO).Uid;
			}
			return "command_center" + str;
		}
	}
}
