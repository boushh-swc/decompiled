using Net.RichardLord.Ash.Core;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Objectives;
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

namespace StaRTS.Main.Views.UX.Screens
{
	public class CrateInfoModalScreen : ClosableScreen, IEventObserver, IViewClockTimeObserver
	{
		private const string LANG_CRATE_FLYOUT_LIST_TITLE = "CRATE_FLYOUT_LIST_TITLE";

		private const string LANG_LCFLY_QUANTITY = "lcfly_quant";

		private const string LANG_TWO_OPTION_CONJUNCTION = "OR";

		private const string LANG_CRATE_FLYOUT_OPEN_CTA = "CRATE_FLYOUT_OPEN_CTA";

		private const string LANG_CRATE_FLYOUT_STORE_OPEN_CTA = "CRATE_FLYOUT_STORE_OPEN_CTA";

		private const string LANG_CRATE_FREE = "CRATE_FREE";

		private const string LANG_CRATE_REWARD_COUNT = "CRATE_FLYOUT_REWARD_COUNT";

		private const string LANG_EQUIPMENT_ARMORY_REQUIRED = "EQUIPMENT_ARMORY_REQUIRED";

		private const string LANG_WATCH_TO_OPEN = "s_WatchToOpen";

		private const string EXPIRES_IN = "expires_in";

		private const string TEXT_PURCHASE = "TARGETED_BUNDLE_PURCHASE";

		private const string TEXT_DISCOUNT = "TARGETED_BUNDLE_DISCOUNT";

		private const string TEXT_PERCENT = "PERCENTAGE";

		private const int MAX_FLYOUT_ROW_ITEMS = 4;

		private const string ANIM_SHOW = "Show";

		private const string ANIM_HIDE = "Hide";

		private const float ANIM_HIDE_DURATION = 1.16f;

		private const string LABEL_CRATE_TITLE = "LabelCrateTitle";

		private const string BUTTON_CLOSE_CRATE_INFO = "BtnCloseCrateInfo";

		private const string PLANET_BG_TEXTURE = "TexturePlanetBgCrateInfo";

		private const string LABEL_PLANET_NAME_CRATE = "LabelPlanetNameCrate";

		private const string LABEL_REWARDS_LIST_TITLE = "LabelRewardsListTitle";

		private const string LABEL_REWARD_CHANCE = "LabelChanceTitle";

		private const string LABEL_CRATE_PULL_AMOUNT = "LabelCratePullAmt";

		private const string SPRITE_CRATE_IMAGE = "SpriteCrateImage";

		private const string GROUP_REWARD_DETAILS = "PanelGroupRewardPreview";

		private const string REWARD_ITEM_QUALITY = "RewardItemCardQ{0}";

		private const string REWARD_ITEM_QUALITY_BACKGROUND = "SpriteTroopImageBkgGridQ{0}";

		private const string REWARD_ITEM_DEFAULT = "RewardItemDefault";

		private const string LABEL_REWARD_NAME = "LabelRewardNamePreview";

		private const string LABEL_REWARD_AMOUNT = "LabelRewardAmt";

		private const string SPRITE_REWARD_IMAGE = "SpriteRewardItemImage";

		private const string SPRITE_REWARD_ICON = "SpriteRewardIcon";

		private const string LABEL_REWARD_TYPE = "LabelRewardTypePreview";

		private const string SPRITE_LOCK_ICON = "SpriteLockIcon";

		private const string LABEL_EQUIPMENT_REQUIREMENT = "LabelEquipmentRequirement";

		private const string SPRITE_REWARD_DIM = "SpriteRewardDim";

		private const string GRID_REWARD_LIST = "GridRewardsList";

		private const string TEMPLATE_REWARDS = "TemplateRewards";

		private const string GRID_ITEM_ROW_HIGHLIGHT = "GroupRowHighlight";

		private const string GRID_ITEM_LABEL_CHANCE = "LabelChance";

		private const string GRID_ITEM_LABEL_REWARD_NAME = "LabelRewardName";

		private const string GRID_ITEM_SPRITE_REWARD = "SpriteGridReward";

		private const string GRID_ITEM_GROUP_OPTION_2 = "Group2Option";

		private const string GRID_ITEM_SPRITE_OPTION_2A = "Sprite2OptionA";

		private const string GRID_ITEM_LABEL_CONJUNCTION = "LabelConjunction";

		private const string GRID_ITEM_SPRITE_OPTION_2B = "Sprite2OptionB";

		private const string LABEL_CRATE_EXPIRE_TIMER = "LabelCrateExpireTimer";

		private const string BUTTON_CRATE_INFO_CTA = "BtnCrateInfoCTA";

		private const string LABEL_BTN_CRATE_INFO_CTA = "LabelBtnCrateInfoCTA";

		private const string BUTTON_CRATE_INFO_PAY = "BtnCrateInfoPay";

		private const string LABEL_PURCHASE = "LabelPurchase";

		private const string LABEL_PURCHASE_COST = "PurchaseCostLabel";

		private const string SPRITE_CRYSTAL_COST_ICON = "PurchaseCostCrystal";

		private const string LABEL_CENTERED_PAY = "LabelBtnPayCentered";

		private const string CONTAINER = "ContainerCrateCircle";

		private const string CONTAINER_LEI = "ContainerCrateCircleSpecial";

		private const string GROUP_DISCOUNT = "GroupDiscountBadge";

		private const string LABEL_PERCENTAGE = "LabelValueStatement";

		private const string LABEL_DISCOUNT = "LabelValueStatement2";

		private const string CHARACTER1_SPRITE = "SpriteCharacter1";

		private const string CHARACTER2_SPRITE = "SpriteCharacter2";

		private const string PROMO_ART_TEXTURE = "TexturePromoArt";

		private const string PROMO_ART_TEXTURE_ASSET = "gui_textures_promotional";

		private const string PROMO_BG_TEXTURE_ENTRY = "TextureEnvironmentEntry";

		private const string PROMO_BG_TEXTURE_LEFT = "TextureEnvironmentLeft";

		private const string PROMO_BG_TEXTURE_RIGHT = "TextureEnvironmentRight";

		private const string PROMO_BG_ENTRY_TEXTURE_ASSET = "targeted_bundle_entry";

		private const string PROMO_BG_LEFT_TEXTURE_ASSET = "targeted_bundle_treads";

		private const string PROMO_BG_RIGHT_TEXTURE_ASSET = "targeted_bundle_treads";

		private const string PROMO_DUST_TEXTURE_LEFT = "TextureEnvironmentDustLeft";

		private const string PROMO_DUST_TEXTURE_RIGHT = "TextureEnvironmentDustRight";

		private const string PROMO_DUST_TEXTURE_ASSET = "targeted_bundle_dust";

		private const string PROMO_PAY_BUTTON_CURRENCY_LABEL = "LabelBtnPayCenteredCurrency";

		private const string PROMO_PAY_BUTTON_CURRENCY_SPRITE = "SpriteBtnPayCenteredCurrency";

		private const string WIDGET_OPEN_ANOTHER = "WidgetOpenAnotherCrate";

		private const string LABEL_OPEN_ANOTHER = "LabelBtnOpenAnotherCrate";

		private const string BUTTON_OPEN_ANOTHER = "BtnOpenAnotherCrate";

		private UXLabel labelRewardChance;

		private UXLabel labelRewardName;

		private UXLabel labelRewardAmt;

		private UXSprite spriteRewardIcon;

		private UXSprite spriteRewardImage;

		private UXLabel labelRewardType;

		private UXElement discountGroup;

		private UXButton crateInfoCTAButton;

		private UXButton crateInfoPayButton;

		private UXLabel expirationLabel;

		private UXElement widgetOpenAnother;

		private UXLabel labelOpenAnother;

		private UXButton buttonOpenAnother;

		private UXGrid rewardsGrid;

		private CrateVO targetCrateVO;

		private LimitedEditionItemVO targetLEIVO;

		private PlanetVO planetVO;

		private CrateData crateData;

		private int hqLevel;

		private bool playerHasArmory;

		public CrateInfoReason ModalReason;

		private int selectedRowIndex = -1;

		private List<CrateFlyoutItemVO> filteredFlyoutItems;

		private uint autoScrollFlyoutRepeatTimerId;

		private uint autoScrollFlyoutResumeTimerId;

		public TargetedBundleVO CurrentOffer;

		private UXSprite characterSprite1;

		private UXSprite characterSprite2;

		private GeometryProjector charGeometry1;

		private GeometryProjector charGeometry2;

		private UXSprite btnCurrencySprite;

		private UXLabel btnCurrencyLabel;

		private string currentIapId;

		private string currentRealCost;

		protected bool ignoreExpirationAutoClose;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		public CrateInfoModalScreen(string crateUid, string planetID, int hqLevel, FactionType faction, bool playerHasArmory) : base("gui_modal_crateinfo")
		{
			StaticDataController staticDataController = Service.StaticDataController;
			this.targetCrateVO = staticDataController.Get<CrateVO>(crateUid);
			string text = null;
			if (faction != FactionType.Empire)
			{
				if (faction == FactionType.Rebel)
				{
					text = this.targetCrateVO.RebelLEIUid;
				}
			}
			else
			{
				text = this.targetCrateVO.EmpireLEIUid;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.targetLEIVO = staticDataController.Get<LimitedEditionItemVO>(text);
			}
			this.planetVO = staticDataController.GetOptional<PlanetVO>(planetID);
			this.playerHasArmory = playerHasArmory;
			this.hqLevel = hqLevel;
			this.filteredFlyoutItems = new List<CrateFlyoutItemVO>();
			string[] array = (faction != FactionType.Empire) ? this.targetCrateVO.FlyoutRebelItems : this.targetCrateVO.FlyoutEmpireItems;
			if (array != null)
			{
				int i = 0;
				int num = array.Length;
				while (i < num)
				{
					string text2 = array[i];
					CrateFlyoutItemVO optional = staticDataController.GetOptional<CrateFlyoutItemVO>(text2);
					if (optional == null)
					{
						Service.Logger.ErrorFormat("CrateInfoModalScreen: FlyoutItemVO Uid {0} not found", new object[]
						{
							text2
						});
					}
					else
					{
						bool flag = UXUtils.IsValidRewardItem(optional, this.planetVO, hqLevel);
						bool flag2 = UXUtils.ShouldDisplayCrateFlyoutItem(optional, CrateFlyoutDisplayType.Flyout);
						if (flag && flag2 && this.filteredFlyoutItems.Count < 4)
						{
							this.filteredFlyoutItems.Add(optional);
						}
					}
					i++;
				}
			}
		}

		public static CrateInfoModalScreen CreateForInfo(string crateUid, string planetID)
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			Entity currentHQ = buildingLookupController.GetCurrentHQ();
			BuildingTypeVO buildingType = currentHQ.Get<BuildingComponent>().BuildingType;
			FactionType faction = Service.CurrentPlayer.Faction;
			bool flag = ArmoryUtils.PlayerHasArmory();
			return new CrateInfoModalScreen(crateUid, planetID, buildingType.Lvl, faction, flag)
			{
				ModalReason = CrateInfoReason.Reason_Info
			};
		}

		public static CrateInfoModalScreen CreateForTargetedOfferTest(TargetedBundleVO offer, CrateVO crate)
		{
			CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForTargetedOffer(offer, crate);
			crateInfoModalScreen.ignoreExpirationAutoClose = true;
			return crateInfoModalScreen;
		}

		public static CrateInfoModalScreen CreateForTargetedOffer(TargetedBundleVO offer, CrateVO crate)
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			Entity currentHQ = buildingLookupController.GetCurrentHQ();
			BuildingTypeVO buildingType = currentHQ.Get<BuildingComponent>().BuildingType;
			string planetId = Service.CurrentPlayer.PlanetId;
			FactionType faction = Service.CurrentPlayer.Faction;
			bool flag = ArmoryUtils.PlayerHasArmory();
			return new CrateInfoModalScreen(crate.Uid, planetId, buildingType.Lvl, faction, flag)
			{
				ModalReason = CrateInfoReason.Reason_Targeted_Offer,
				CurrentOffer = offer
			};
		}

		public static CrateInfoModalScreen CreateForObjectiveProgressInfo(string crateUid, ObjectiveProgress progress)
		{
			return new CrateInfoModalScreen(crateUid, progress.PlanetId, progress.HQ, Service.CurrentPlayer.Faction, ArmoryUtils.PlayerHasArmory())
			{
				ModalReason = CrateInfoReason.Reason_Info
			};
		}

		public static CrateInfoModalScreen CreateForInventory(CrateData crateData)
		{
			return new CrateInfoModalScreen(crateData.CrateId, crateData.PlanetId, crateData.HQLevel, Service.CurrentPlayer.Faction, ArmoryUtils.PlayerHasArmory())
			{
				crateData = crateData,
				ModalReason = CrateInfoReason.Reason_Inventory_Open
			};
		}

		public static CrateInfoModalScreen CreateForStore(string crateUid, string planetID)
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			Entity currentHQ = buildingLookupController.GetCurrentHQ();
			BuildingTypeVO buildingType = currentHQ.Get<BuildingComponent>().BuildingType;
			FactionType faction = Service.CurrentPlayer.Faction;
			bool flag = ArmoryUtils.PlayerHasArmory();
			return new CrateInfoModalScreen(crateUid, planetID, buildingType.Lvl, faction, flag)
			{
				ModalReason = CrateInfoReason.Reason_Store_Buy
			};
		}

		public static CrateInfoModalScreen CreateForMobileConnectorAd(string crateUid, string planetID)
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			Entity currentHQ = buildingLookupController.GetCurrentHQ();
			BuildingTypeVO buildingType = currentHQ.Get<BuildingComponent>().BuildingType;
			FactionType faction = Service.CurrentPlayer.Faction;
			bool flag = ArmoryUtils.PlayerHasArmory();
			return new CrateInfoModalScreen(crateUid, planetID, buildingType.Lvl, faction, flag)
			{
				ModalReason = CrateInfoReason.Reason_Mobile_Connector_Ad
			};
		}

		public void ResetAutoScrollTimers()
		{
			if (this.autoScrollFlyoutRepeatTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.autoScrollFlyoutRepeatTimerId);
				this.autoScrollFlyoutRepeatTimerId = 0u;
			}
			if (this.autoScrollFlyoutResumeTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.autoScrollFlyoutResumeTimerId);
				this.autoScrollFlyoutResumeTimerId = 0u;
			}
		}

		public override void OnDestroyElement()
		{
			this.ResetAutoScrollTimers();
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			if (this.rewardsGrid != null)
			{
				this.rewardsGrid.Clear();
				this.rewardsGrid = null;
			}
			base.OnDestroyElement();
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.OpeningPurchasedCrate)
			{
				Service.EventManager.UnregisterObserver(this, EventId.OpeningPurchasedCrate);
				ProcessingScreen.Hide();
				StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
				if (this.ModalReason == CrateInfoReason.Reason_Store_Buy && highestLevelScreen != null)
				{
					highestLevelScreen.RegisterForCrateFlyoutReOpen(this.targetCrateVO.Uid);
				}
				this.OnCloseCrateInfoButtonClicked(null);
			}
			return base.OnEvent(id, cookie);
		}

		protected override void OnScreenLoaded()
		{
			base.InitAnimator();
			string crateDisplayName = LangUtils.GetCrateDisplayName(this.targetCrateVO);
			UXLabel element = base.GetElement<UXLabel>("LabelCrateTitle");
			element.Text = crateDisplayName;
			UXSprite element2 = base.GetElement<UXSprite>("SpriteCrateImage");
			RewardUtils.SetCrateIcon(element2, this.targetCrateVO, AnimState.Idle);
			UXLabel element3 = base.GetElement<UXLabel>("LabelRewardsListTitle");
			element3.Text = this.lang.Get("CRATE_FLYOUT_LIST_TITLE", new object[0]);
			UXButton element4 = base.GetElement<UXButton>("BtnCloseCrateInfo");
			element4.OnClicked = new UXButtonClickedDelegate(this.OnCloseCrateInfoButtonClicked);
			UXLabel element5 = base.GetElement<UXLabel>("LabelCratePullAmt");
			int num = this.targetCrateVO.SupplyPoolUIDs.Length;
			element5.Text = this.lang.Get("CRATE_FLYOUT_REWARD_COUNT", new object[]
			{
				num
			});
			UXElement element6 = base.GetElement<UXElement>("ContainerCrateCircle");
			UXElement element7 = base.GetElement<UXElement>("ContainerCrateCircleSpecial");
			element7.Visible = (this.targetLEIVO != null);
			element6.Visible = !element7.Visible;
			this.labelRewardType = base.GetElement<UXLabel>("LabelRewardTypePreview");
			this.InitPlanetUI();
			this.InitExpirationLabel();
			this.labelRewardChance = base.GetElement<UXLabel>("LabelChanceTitle");
			this.labelRewardName = base.GetElement<UXLabel>("LabelRewardNamePreview");
			this.labelRewardAmt = base.GetElement<UXLabel>("LabelRewardAmt");
			this.spriteRewardIcon = base.GetElement<UXSprite>("SpriteRewardIcon");
			this.spriteRewardImage = base.GetElement<UXSprite>("SpriteRewardItemImage");
			this.btnCurrencyLabel = base.GetElement<UXLabel>("LabelBtnPayCenteredCurrency");
			this.btnCurrencySprite = base.GetElement<UXSprite>("SpriteBtnPayCenteredCurrency");
			this.discountGroup = base.GetElement<UXElement>("GroupDiscountBadge");
			this.discountGroup.Visible = false;
			this.InitRewardsGrid();
			base.SetTrigger("Show");
			if (this.filteredFlyoutItems.Count > 0)
			{
				this.SelectRowItemAtIndex(0);
				this.AutoScrollFlyoutRowItem();
			}
			if (this.ModalReason == CrateInfoReason.Reason_Targeted_Offer)
			{
				this.SetupTargetedOfferElements();
				this.discountGroup.Visible = true;
				UXLabel element8 = base.GetElement<UXLabel>("LabelValueStatement");
				UXLabel element9 = base.GetElement<UXLabel>("LabelValueStatement2");
				element8.Text = this.lang.Get("PERCENTAGE", new object[]
				{
					this.CurrentOffer.Discount.ToString()
				});
				element9.Text = this.lang.Get("TARGETED_BUNDLE_DISCOUNT", new object[0]);
			}
			this.InitCTAUI();
			Service.EventManager.SendEvent(EventId.ObjectiveCrateInfoScreenOpened, null);
		}

		private void SetupTargetedOfferElements()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			this.characterSprite1 = base.GetElement<UXSprite>("SpriteCharacter1");
			this.characterSprite2 = base.GetElement<UXSprite>("SpriteCharacter2");
			TextureVO optional = staticDataController.GetOptional<TextureVO>("gui_textures_promotional");
			if (optional != null)
			{
				UXTexture element = base.GetElement<UXTexture>("TexturePromoArt");
				element.LoadTexture(optional.AssetName);
			}
			optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_entry");
			if (optional != null)
			{
				UXTexture element2 = base.GetElement<UXTexture>("TextureEnvironmentEntry");
				element2.LoadTexture(optional.AssetName);
			}
			optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_treads");
			if (optional != null)
			{
				UXTexture element3 = base.GetElement<UXTexture>("TextureEnvironmentRight");
				element3.LoadTexture(optional.AssetName);
			}
			optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_treads");
			if (optional != null)
			{
				UXTexture element4 = base.GetElement<UXTexture>("TextureEnvironmentLeft");
				element4.LoadTexture(optional.AssetName);
			}
			optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_dust");
			if (optional != null)
			{
				UXTexture element5 = base.GetElement<UXTexture>("TextureEnvironmentDustRight");
				element5.LoadTexture(optional.AssetName);
				element5 = base.GetElement<UXTexture>("TextureEnvironmentDustLeft");
				element5.LoadTexture(optional.AssetName);
			}
			UXUtils.SetupAnimatedCharacter(this.characterSprite1, this.CurrentOffer.Character1Image, ref this.charGeometry1);
			UXUtils.SetupAnimatedCharacter(this.characterSprite2, this.CurrentOffer.Character2Image, ref this.charGeometry2);
		}

		private void InitPlanetUI()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelPlanetNameCrate");
			UXTexture element2 = base.GetElement<UXTexture>("TexturePlanetBgCrateInfo");
			if (this.planetVO != null)
			{
				element.Text = LangUtils.GetPlanetDisplayName(this.planetVO.Uid);
				element2.LoadTexture(this.planetVO.LeaderboardTileTexture);
			}
			else
			{
				element.Visible = false;
				element2.Visible = false;
			}
		}

		private void InitExpirationLabel()
		{
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			this.expirationLabel = base.GetElement<UXLabel>("LabelCrateExpireTimer");
			if (this.ModalReason == CrateInfoReason.Reason_Store_Buy && this.targetLEIVO != null)
			{
				UXUtils.SetLeiExpirationTimerLabel(this.targetLEIVO, this.expirationLabel, this.lang);
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			else if (this.ModalReason == CrateInfoReason.Reason_Inventory_Open && this.crateData != null)
			{
				UXUtils.SetCrateExpirationTimerLabel(this.crateData, this.expirationLabel, this.lang);
				if (this.crateData.DoesExpire)
				{
					Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				}
			}
			else if (this.ModalReason == CrateInfoReason.Reason_Targeted_Offer)
			{
				UXUtils.SetCrateTargetedOfferTimerLabel(targetedBundleController.OfferExpiresAt, this.expirationLabel, this.lang);
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			else
			{
				this.expirationLabel.Visible = false;
			}
		}

		private void InitCTAUI()
		{
			this.crateInfoCTAButton = base.GetElement<UXButton>("BtnCrateInfoCTA");
			this.crateInfoPayButton = base.GetElement<UXButton>("BtnCrateInfoPay");
			this.widgetOpenAnother = base.GetElement<UXElement>("WidgetOpenAnotherCrate");
			this.labelOpenAnother = base.GetElement<UXLabel>("LabelBtnOpenAnotherCrate");
			this.buttonOpenAnother = base.GetElement<UXButton>("BtnOpenAnotherCrate");
			this.crateInfoPayButton.Visible = false;
			this.crateInfoCTAButton.Visible = false;
			this.widgetOpenAnother.Visible = false;
			this.buttonOpenAnother.Visible = false;
			this.btnCurrencyLabel.Visible = false;
			this.btnCurrencySprite.Visible = false;
			bool flag = CrateUtils.IsVisibleInStore(this.targetCrateVO) && CrateUtils.IsPurchasableInStore(this.targetCrateVO);
			UXLabel element = base.GetElement<UXLabel>("LabelPurchase");
			UXSprite element2 = base.GetElement<UXSprite>("PurchaseCostCrystal");
			UXLabel element3 = base.GetElement<UXLabel>("PurchaseCostLabel");
			UXLabel element4 = base.GetElement<UXLabel>("LabelBtnPayCentered");
			if (this.ModalReason == CrateInfoReason.Reason_Targeted_Offer)
			{
				element.Visible = false;
				element2.Visible = true;
				element3.Visible = true;
				element4.Visible = false;
				this.crateInfoPayButton.Visible = true;
				this.btnCurrencyLabel.Visible = false;
				this.btnCurrencySprite.Visible = false;
				if (this.CurrentOffer != null)
				{
					this.crateInfoPayButton.OnClicked = new UXButtonClickedDelegate(this.OnTargetedOfferPurchaseClicked);
					string[] cost = this.CurrentOffer.Cost;
					if (cost != null && cost.Length > 0)
					{
						element.Visible = true;
						element.Text = this.lang.Get("CRATE_FLYOUT_STORE_OPEN_CTA", new object[0]);
						UXUtils.SetupTargetedOfferCostUI(this.CurrentOffer, element3, element2);
					}
					else
					{
						if (this.CurrentOffer.LinkedIAPs.Count <= 0)
						{
							Service.Logger.Error("CrateModalInfoScreen::InitCTAUI No Cost on Offer: " + this.CurrentOffer.Uid);
							return;
						}
						this.btnCurrencySprite.Visible = false;
						this.currentIapId = null;
						this.currentRealCost = null;
						if (!Service.TargetedBundleController.FindAvailableIAP(this.CurrentOffer, out this.currentIapId, out this.currentRealCost))
						{
							this.discountGroup.Visible = false;
							this.crateInfoPayButton.Visible = false;
							this.btnCurrencyLabel.Visible = false;
							Service.Logger.Error("CrateModalInfoScreen::InitCTAUI LinkedIAP invalid on Offer: " + this.CurrentOffer.Uid);
							return;
						}
						element4.Text = this.lang.Get("TARGETED_BUNDLE_PURCHASE", new object[]
						{
							this.currentRealCost
						});
						element4.Visible = true;
						element2.Visible = false;
						element3.Visible = false;
					}
				}
			}
			else if (this.ModalReason == CrateInfoReason.Reason_Store_Buy && (flag || this.targetLEIVO != null))
			{
				this.crateInfoPayButton.Visible = true;
				this.crateInfoPayButton.OnClicked = new UXButtonClickedDelegate(this.OnCrateInfoPayButtonClicked);
				element.Text = this.lang.Get("CRATE_FLYOUT_STORE_OPEN_CTA", new object[0]);
				element4.Visible = false;
				int num = (this.targetLEIVO != null) ? this.targetLEIVO.Crystals : this.targetCrateVO.Crystals;
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				if (currentPlayer.ArmoryInfo.FirstCratePurchased || this.targetLEIVO != null)
				{
					element3.Text = this.lang.ThousandsSeparated(num);
					UXUtils.UpdateCostColor(element3, null, 0, 0, 0, num, 0, false);
					element2.Visible = true;
				}
				else
				{
					element4.Text = this.lang.Get("CRATE_FLYOUT_STORE_OPEN_CTA", new object[0]);
					element4.Visible = true;
					element.Visible = false;
					element3.Visible = false;
					element2.Visible = false;
				}
			}
			else if (this.ModalReason == CrateInfoReason.Reason_Inventory_Open && this.crateData != null)
			{
				this.crateInfoCTAButton.Visible = true;
				this.crateInfoCTAButton.OnClicked = new UXButtonClickedDelegate(this.OnCrateInfoCTAButtonClicked);
				UXLabel element5 = base.GetElement<UXLabel>("LabelBtnCrateInfoCTA");
				element5.Text = this.lang.Get("CRATE_FLYOUT_OPEN_CTA", new object[0]);
			}
			else if (this.ModalReason == CrateInfoReason.Reason_Mobile_Connector_Ad)
			{
				this.widgetOpenAnother.Visible = true;
				this.labelOpenAnother.Text = this.lang.Get("s_WatchToOpen", new object[0]);
				this.buttonOpenAnother.Visible = true;
				this.buttonOpenAnother.OnClicked = new UXButtonClickedDelegate(this.OnViewMobileConnectorAdButtonClicked);
			}
		}

		private void InitRewardsGrid()
		{
			this.rewardsGrid = base.GetElement<UXGrid>("GridRewardsList");
			this.rewardsGrid.SetTemplateItem("TemplateRewards");
			if (this.filteredFlyoutItems.Count == 0)
			{
				base.GetElement<UXElement>("PanelGroupRewardPreview").Visible = false;
				this.rewardsGrid.Visible = false;
				this.labelRewardChance.Visible = false;
				return;
			}
			int i = 0;
			int count = this.filteredFlyoutItems.Count;
			while (i < count)
			{
				CrateFlyoutItemVO crateFlyoutItemVO = this.filteredFlyoutItems[i];
				string uid = crateFlyoutItemVO.Uid;
				UXElement uXElement = this.rewardsGrid.CloneTemplateItem(uid);
				UXLabel subElement = this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelChance");
				subElement.Text = this.lang.Get(crateFlyoutItemVO.ListChanceString, new object[0]);
				UXLabel subElement2 = this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelRewardName");
				subElement2.Text = this.lang.Get(crateFlyoutItemVO.ListDescString, new object[0]);
				UXElement subElement3 = this.rewardsGrid.GetSubElement<UXElement>(uid, "Group2Option");
				UXSprite subElement4 = this.rewardsGrid.GetSubElement<UXSprite>(uid, "Sprite2OptionA");
				UXLabel subElement5 = this.rewardsGrid.GetSubElement<UXLabel>(uid, "LabelConjunction");
				UXSprite subElement6 = this.rewardsGrid.GetSubElement<UXSprite>(uid, "Sprite2OptionB");
				UXSprite subElement7 = this.rewardsGrid.GetSubElement<UXSprite>(uid, "SpriteGridReward");
				string[] listIcons = crateFlyoutItemVO.ListIcons;
				if (listIcons != null)
				{
					if (listIcons.Length == 1)
					{
						subElement7.SpriteName = listIcons[0];
						subElement3.Visible = false;
					}
					else if (listIcons.Length == 2)
					{
						subElement4.SpriteName = listIcons[0];
						subElement6.SpriteName = listIcons[1];
						subElement5.Text = this.lang.Get("OR", new object[0]);
						subElement7.Visible = false;
					}
				}
				else
				{
					subElement3.Visible = false;
					subElement7.Visible = false;
				}
				this.UpdateRowHighlight(uid, false);
				UXButton uXButton = (UXButton)uXElement;
				uXButton.Tag = i;
				uXButton.OnClicked = new UXButtonClickedDelegate(this.OnGridItemButtonClicked);
				this.rewardsGrid.AddItem(uXElement, i);
				i++;
			}
			this.rewardsGrid.RepositionItems();
		}

		public void OnViewClockTime(float dt)
		{
			if (this.ignoreExpirationAutoClose)
			{
				return;
			}
			if (this.crateData != null)
			{
				if (this.crateData.ExpiresTimeStamp <= ServerTime.Time)
				{
					this.OnCloseCrateInfoButtonClicked(null);
					return;
				}
			}
			else if (this.targetLEIVO != null)
			{
				if ((long)this.targetLEIVO.EndTime <= (long)((ulong)ServerTime.Time))
				{
					this.OnCloseCrateInfoButtonClicked(null);
					return;
				}
			}
			else if (this.ModalReason == CrateInfoReason.Reason_Targeted_Offer)
			{
				TargetedBundleController targetedBundleController = Service.TargetedBundleController;
				if (targetedBundleController.OfferExpiresAt <= ServerTime.Time)
				{
					this.OnCloseCrateInfoButtonClicked(null);
					return;
				}
			}
		}

		private void UpdateRowHighlight(string itemUid, bool show)
		{
			UXElement subElement = this.rewardsGrid.GetSubElement<UXElement>(itemUid, "GroupRowHighlight");
			subElement.Visible = show;
		}

		private void UpdateRewardUI(CrateFlyoutItemVO crateFlyoutItemVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			InventoryCrateRewardController inventoryCrateRewardController = Service.InventoryCrateRewardController;
			string crateSupplyUid = crateFlyoutItemVO.CrateSupplyUid;
			CrateSupplyVO optional = staticDataController.GetOptional<CrateSupplyVO>(crateSupplyUid);
			if (optional != null)
			{
				UXUtils.HideQualityCards(this, "RewardItemDefault", "RewardItemCardQ{0}");
				if (optional.Type == SupplyType.ShardTroop || optional.Type == SupplyType.ShardSpecialAttack)
				{
					ShardVO optional2 = Service.StaticDataController.GetOptional<ShardVO>(optional.RewardUid);
					int quality = (int)optional2.Quality;
					base.GetElement<UXElement>(string.Format("RewardItemCardQ{0}", quality)).Visible = true;
				}
				else if (optional.Type == SupplyType.Shard)
				{
					EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(optional.RewardUid);
					int quality2 = (int)currentEquipmentDataByID.Quality;
					base.GetElement<UXElement>(string.Format("RewardItemCardQ{0}", quality2)).Visible = true;
					base.GetElement<UXElement>(string.Format("SpriteTroopImageBkgGridQ{0}", quality2)).Visible = true;
				}
				else if (optional.Type == SupplyType.Troop || optional.Type == SupplyType.Hero || optional.Type == SupplyType.SpecialAttack)
				{
					int upgradeQualityForDeployableUID = Service.DeployableShardUnlockController.GetUpgradeQualityForDeployableUID(optional.RewardUid);
					if (upgradeQualityForDeployableUID > 0)
					{
						base.GetElement<UXElement>(string.Format("RewardItemCardQ{0}", upgradeQualityForDeployableUID)).Visible = true;
					}
				}
				else
				{
					base.GetElement<UXElement>("RewardItemDefault").Visible = true;
				}
				IGeometryVO iconVOFromCrateSupply = GameUtils.GetIconVOFromCrateSupply(optional, this.hqLevel);
				if (iconVOFromCrateSupply != null)
				{
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(iconVOFromCrateSupply, this.spriteRewardImage, true);
					projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
					ProjectorUtils.GenerateProjector(projectorConfig);
				}
				else
				{
					Service.Logger.ErrorFormat("CrateInfoModalScreen: Could not generate geometry for crate supply {0}", new object[]
					{
						optional.Uid
					});
				}
				this.labelRewardChance.Text = this.lang.Get(crateFlyoutItemVO.DetailChanceString, new object[0]);
				this.labelRewardName.Text = this.lang.Get(crateFlyoutItemVO.DetailDescString, new object[0]);
				string[] listIcons = crateFlyoutItemVO.ListIcons;
				if (listIcons != null && listIcons.Length > 0)
				{
					this.spriteRewardIcon.SpriteName = listIcons[listIcons.Length - 1];
				}
				else
				{
					this.spriteRewardIcon.Visible = false;
				}
				string detailTypeStringId = crateFlyoutItemVO.DetailTypeStringId;
				if (!string.IsNullOrEmpty(detailTypeStringId))
				{
					this.labelRewardType.Visible = true;
					string text = this.lang.Get(detailTypeStringId, new object[0]);
					this.labelRewardType.Text = text;
				}
				else
				{
					this.labelRewardType.Visible = false;
				}
				int rewardAmount = inventoryCrateRewardController.GetRewardAmount(optional, this.hqLevel);
				string text2 = this.lang.ThousandsSeparated(rewardAmount);
				string quantityString = crateFlyoutItemVO.QuantityString;
				if (!string.IsNullOrEmpty(quantityString))
				{
					this.labelRewardAmt.Text = this.lang.Get(quantityString, new object[]
					{
						text2
					});
				}
				else if (rewardAmount > 1)
				{
					this.labelRewardAmt.Text = this.lang.Get("lcfly_quant", new object[]
					{
						text2
					});
				}
				else
				{
					this.labelRewardAmt.Visible = false;
				}
				UXSprite element = base.GetElement<UXSprite>("SpriteLockIcon");
				UXSprite element2 = base.GetElement<UXSprite>("SpriteRewardDim");
				UXLabel element3 = base.GetElement<UXLabel>("LabelEquipmentRequirement");
				element.Visible = false;
				element2.Visible = false;
				element3.Visible = false;
				bool flag = crateFlyoutItemVO.ReqArmory && !this.playerHasArmory;
				if (flag)
				{
					element2.Visible = true;
					element.Visible = true;
					element3.Visible = true;
					element3.Text = this.lang.Get("EQUIPMENT_ARMORY_REQUIRED", new object[0]);
				}
				this.UpdateRowHighlight(crateFlyoutItemVO.Uid, true);
			}
			else
			{
				Service.Logger.ErrorFormat("CrateInfoModalScreen: Could not find crate supply {0} for crate flyout {1}", new object[]
				{
					crateSupplyUid,
					crateFlyoutItemVO.Uid
				});
			}
		}

		private void SelectRowItemAtIndex(int index)
		{
			if (this.selectedRowIndex >= 0 && this.selectedRowIndex < this.filteredFlyoutItems.Count)
			{
				CrateFlyoutItemVO crateFlyoutItemVO = this.filteredFlyoutItems[this.selectedRowIndex];
				string uid = crateFlyoutItemVO.Uid;
				this.UpdateRowHighlight(uid, false);
			}
			CrateFlyoutItemVO crateFlyoutItemVO2 = this.filteredFlyoutItems[index];
			this.UpdateRewardUI(crateFlyoutItemVO2);
			this.selectedRowIndex = index;
		}

		private void CloseScreenOnHideComplete(uint id, object cookie)
		{
			UXButton button = (cookie == null) ? null : (cookie as UXButton);
			base.OnCloseButtonClicked(button);
		}

		public override void Close(object modalResult)
		{
			if (this.ModalReason == CrateInfoReason.Reason_Store_Buy)
			{
				Service.EventManager.SendEvent(EventId.CrateStoreCancel, this.targetCrateVO.Uid);
			}
			base.Close(modalResult);
		}

		private void AutoScrollFlyoutItemResumeCallback(uint id, object cookie)
		{
			this.AutoScrollFlyoutRowItem();
		}

		public float GetFlyoutItemSelectDuration()
		{
			return GameConstants.CRATE_FLYOUT_ITEM_AUTO_SELECT_RESUME;
		}

		private void AutoScrollFlyoutRowItem()
		{
			if (this.filteredFlyoutItems.Count > 1)
			{
				this.autoScrollFlyoutRepeatTimerId = Service.ViewTimerManager.CreateViewTimer(this.GetFlyoutItemSelectDuration(), true, new TimerDelegate(this.AutoScrollFlyoutItemRepeatCallback), null);
			}
		}

		private void AutoScrollFlyoutItemRepeatCallback(uint id, object cookie)
		{
			int num = this.selectedRowIndex + 1;
			if (num >= this.filteredFlyoutItems.Count)
			{
				num = 0;
			}
			this.SelectRowItemAtIndex(num);
		}

		private void OnTargetedOfferPurchaseClicked(UXButton button)
		{
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			targetedBundleController.MakeTargetedBundlePurchase(this.CurrentOffer, this.currentIapId);
			this.Close(null);
		}

		private void OnGridItemButtonClicked(UXButton button)
		{
			int num = (int)button.Tag;
			if (num != this.selectedRowIndex)
			{
				this.ResetAutoScrollTimers();
				this.SelectRowItemAtIndex(num);
				this.autoScrollFlyoutResumeTimerId = Service.ViewTimerManager.CreateViewTimer(this.GetFlyoutItemSelectDuration(), false, new TimerDelegate(this.AutoScrollFlyoutItemResumeCallback), null);
			}
		}

		private void OnCloseCrateInfoButtonClicked(UXButton button)
		{
			if (button != null)
			{
				button.Enabled = false;
			}
			this.ResetAutoScrollTimers();
			if (this.animator != null)
			{
				this.animator.SetTrigger("Hide");
				Service.ViewTimerManager.CreateViewTimer(1.16f, false, new TimerDelegate(this.CloseScreenOnHideComplete), button);
			}
			else
			{
				this.CloseScreenOnHideComplete(0u, button);
			}
		}

		private void OnCrateInfoCTAButtonClicked(UXButton button)
		{
			button.Enabled = false;
			GameUtils.OpenCrate(this.crateData);
			base.ModalResultCookie = this.crateData.CrateId;
			this.OnCloseCrateInfoButtonClicked(null);
		}

		private void OnCrateInfoPayButtonClicked(UXButton button)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			bool flag;
			if (this.targetLEIVO != null)
			{
				flag = GameUtils.BuyLEI(currentPlayer, this.targetLEIVO);
			}
			else
			{
				flag = GameUtils.BuyCrate(currentPlayer, this.targetCrateVO);
			}
			if (flag)
			{
				Service.EventManager.RegisterObserver(this, EventId.OpeningPurchasedCrate);
				ProcessingScreen.Show();
			}
			else
			{
				this.OnCloseCrateInfoButtonClicked(null);
			}
			button.Enabled = false;
		}

		private void OnViewMobileConnectorAdButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.OnCloseCrateInfoButtonClicked(null);
			base.ModalResultCookie = GameConstants.MOBILE_CONNECTOR_VIDEO_REWARD_CRATE;
			Service.MobileConnectorAdsController.ShowRewardedVideoAd();
		}
	}
}
