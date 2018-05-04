using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TargetedBundleScreen : ClosableScreen, IViewClockTimeObserver
	{
		private const string TITLE_LABEL = "LabelTitle";

		private const string DESCRIPTION_LABEL = "LabelDescription";

		private const string VALUE_LABEL = "LabelValueStatement";

		private const string VALUE2_LABEL = "LabelValueStatement2";

		private const string TIMER_LABEL = "LabelTimer";

		private const string ITEM1_LABEL = "LabelItem1";

		private const string ITEM2_LABEL = "LabelItem2";

		private const string ITEM3_LABEL = "LabelItem3";

		private const string ITEM1_SPRITE = "SpriteRewardItem1";

		private const string ITEM2_SPRITE = "SpriteRewardItem2";

		private const string ITEM3_SPRITE = "SpriteRewardItem3";

		private const string CHARACTER1_SPRITE = "SpriteCharacter1";

		private const string CHARACTER2_SPRITE = "SpriteCharacter2";

		private const string PURCHASE_BUTTON_LABEL = "LabelBtnPrimary";

		private const string PURCHASE_BUTTON = "BtnPrimary";

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

		private const string CURRENCY_COST_LABEL = "LabelBtnSecondary";

		private const string CURRENCY_COST_SPRITE = "SpriteCurrency";

		private const string NAME_AND_AMOUNT = "AMOUNT_AND_NAME";

		private const string ANIM_SHOW = "show";

		private const string TEXT_TITLE = "SPECIAL_PROMOTION_TITLE";

		private const string TEXT_DESCRIPTION = "SPECIAL_PROMOTION_DESCRIPTION";

		private const string TEXT_PURCHASE = "TARGETED_BUNDLE_PURCHASE";

		private const string EXPIRES_IN = "expires_in";

		private const string TEXT_DISCOUNT = "TARGETED_BUNDLE_DISCOUNT";

		private const string TEXT_PERCENT = "PERCENTAGE";

		private const int MAX_ITEMS = 3;

		private UXLabel itemLabel1;

		private UXLabel itemLabel2;

		private UXLabel itemLabel3;

		private UXLabel itemLabelTitle1;

		private UXLabel itemLabelTitle2;

		private UXLabel itemLabelTitle3;

		private UXLabel[] itemLabels;

		private UXLabel titleLabel;

		private UXLabel descriptionLabel;

		private UXLabel timerLabel;

		private UXLabel valueLabel;

		private UXLabel value2Label;

		private UXSprite itemSprite1;

		private UXSprite itemSprite2;

		private UXSprite itemSprite3;

		private UXSprite[] itemSprites;

		private UXSprite characterSprite1;

		private UXSprite characterSprite2;

		private UXTexture promotionalTexture;

		private UXButton purchaseButton;

		private UXLabel purchaseIAPButtonLabel;

		private UXLabel purchaseWithCurrencyButtonLabel;

		private UXSprite purchaseWithCurrencySprite;

		private int timeRemaining;

		private GeometryProjector charGeometry1;

		private GeometryProjector charGeometry2;

		private TargetedBundleVO currentOffer;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public TargetedBundleScreen() : base("gui_promotional_info")
		{
			this.itemLabels = new UXLabel[3];
			this.itemSprites = new UXSprite[3];
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			UXLabel element = base.GetElement<UXLabel>("LabelItem1");
			UXLabel element2 = base.GetElement<UXLabel>("LabelItem2");
			UXLabel element3 = base.GetElement<UXLabel>("LabelItem3");
			UXSprite element4 = base.GetElement<UXSprite>("SpriteRewardItem1");
			UXSprite element5 = base.GetElement<UXSprite>("SpriteRewardItem2");
			UXSprite element6 = base.GetElement<UXSprite>("SpriteRewardItem3");
			this.characterSprite1 = base.GetElement<UXSprite>("SpriteCharacter1");
			this.characterSprite2 = base.GetElement<UXSprite>("SpriteCharacter2");
			this.itemLabels[0] = element;
			this.itemLabels[1] = element2;
			this.itemLabels[2] = element3;
			this.itemSprites[0] = element4;
			this.itemSprites[1] = element5;
			this.itemSprites[2] = element6;
			this.purchaseButton = base.GetElement<UXButton>("BtnPrimary");
			this.purchaseButton.OnClicked = new UXButtonClickedDelegate(this.OnPurchaseButtonClicked);
			this.purchaseIAPButtonLabel = base.GetElement<UXLabel>("LabelBtnPrimary");
			this.purchaseWithCurrencyButtonLabel = base.GetElement<UXLabel>("LabelBtnSecondary");
			this.purchaseWithCurrencySprite = base.GetElement<UXSprite>("SpriteCurrency");
			this.timerLabel = base.GetElement<UXLabel>("LabelTimer");
			this.valueLabel = base.GetElement<UXLabel>("LabelValueStatement");
			this.value2Label = base.GetElement<UXLabel>("LabelValueStatement2");
			this.UpdateElements();
			Animator component = base.Root.GetComponent<Animator>();
			if (component != null)
			{
				component.SetTrigger("show");
			}
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			base.OnDestroyElement();
		}

		public void OnViewClockTime(float dt)
		{
			this.timeRemaining = Mathf.Max(0, this.timeRemaining - 1);
			this.timerLabel.Text = this.lang.Get("expires_in", new object[]
			{
				GameUtils.GetTimeLabelFromSeconds(this.timeRemaining)
			});
			if (this.timeRemaining <= 0)
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				this.Close(null);
			}
		}

		private void UpdateElements()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			this.currentOffer = targetedBundleController.CurrentTargetedOffer;
			if (this.currentOffer != null)
			{
				UXLabel element = base.GetElement<UXLabel>("LabelTitle");
				element.Text = this.lang.Get(this.currentOffer.Title, new object[0]);
				UXLabel element2 = base.GetElement<UXLabel>("LabelDescription");
				element2.Text = this.lang.Get(this.currentOffer.Description, new object[0]);
				TextureVO optional = staticDataController.GetOptional<TextureVO>("gui_textures_promotional");
				if (optional != null)
				{
					UXTexture element3 = base.GetElement<UXTexture>("TexturePromoArt");
					element3.LoadTexture(optional.AssetName);
				}
				optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_entry");
				if (optional != null)
				{
					UXTexture element4 = base.GetElement<UXTexture>("TextureEnvironmentEntry");
					element4.LoadTexture(optional.AssetName);
				}
				optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_treads");
				if (optional != null)
				{
					UXTexture element5 = base.GetElement<UXTexture>("TextureEnvironmentRight");
					element5.LoadTexture(optional.AssetName);
				}
				optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_treads");
				if (optional != null)
				{
					UXTexture element6 = base.GetElement<UXTexture>("TextureEnvironmentLeft");
					element6.LoadTexture(optional.AssetName);
				}
				optional = staticDataController.GetOptional<TextureVO>("targeted_bundle_dust");
				if (optional != null)
				{
					UXTexture element7 = base.GetElement<UXTexture>("TextureEnvironmentDustRight");
					element7.LoadTexture(optional.AssetName);
					element7 = base.GetElement<UXTexture>("TextureEnvironmentDustLeft");
					element7.LoadTexture(optional.AssetName);
				}
				int i = 0;
				int count = this.currentOffer.RewardUIDs.Count;
				while (i < count)
				{
					RewardVO rewardVO = staticDataController.Get<RewardVO>(this.currentOffer.RewardUIDs[i]);
					if (!RewardUtils.SetupTargetedOfferCrateRewardDisplay(rewardVO, this.itemLabels[i], this.itemSprites[i]))
					{
						List<RewardComponentTag> rewardComponents = RewardUtils.GetRewardComponents(rewardVO);
						int j = 0;
						int count2 = rewardComponents.Count;
						while (j < count2)
						{
							RewardComponentTag rewardComponentTag = rewardComponents[j];
							this.itemLabels[i].Text = this.lang.Get("AMOUNT_AND_NAME", new object[]
							{
								rewardComponentTag.RewardName,
								rewardComponentTag.Quantity
							});
							RewardUtils.SetRewardIcon(this.itemSprites[i], rewardComponentTag.RewardGeometryConfig, AnimationPreference.AnimationAlways);
							j++;
						}
					}
					i++;
				}
				if (targetedBundleController.IsCurrencyCostOffer(this.currentOffer))
				{
					this.SetupCurrencyCostOffer(this.currentOffer);
				}
				else
				{
					this.SetupIAPLinkedOffer(this.currentOffer);
				}
				UXUtils.SetupAnimatedCharacter(this.characterSprite1, this.currentOffer.Character1Image, ref this.charGeometry1);
				UXUtils.SetupAnimatedCharacter(this.characterSprite2, this.currentOffer.Character2Image, ref this.charGeometry2);
				uint serverTime = Service.ServerAPI.ServerTime;
				this.timeRemaining = (int)(targetedBundleController.OfferExpiresAt - serverTime);
				this.timerLabel.Text = this.lang.Get("expires_in", new object[]
				{
					GameUtils.GetTimeLabelFromSeconds(this.timeRemaining)
				});
				this.valueLabel.Text = this.lang.Get("PERCENTAGE", new object[]
				{
					this.currentOffer.Discount.ToString()
				});
				this.value2Label.Text = this.lang.Get("TARGETED_BUNDLE_DISCOUNT", new object[]
				{
					this.currentOffer.Discount.ToString()
				});
			}
			else
			{
				Service.Logger.Error("No current offer available for targeted bundle screen");
			}
		}

		private void SetupCurrencyCostOffer(TargetedBundleVO currentOffer)
		{
			this.purchaseIAPButtonLabel.Visible = false;
			this.purchaseWithCurrencySprite.Visible = true;
			this.purchaseWithCurrencyButtonLabel.Visible = true;
			UXUtils.SetupTargetedOfferCostUI(currentOffer, this.purchaseWithCurrencyButtonLabel, this.purchaseWithCurrencySprite);
		}

		private void SetupIAPLinkedOffer(TargetedBundleVO currentOffer)
		{
			this.purchaseWithCurrencySprite.Visible = false;
			this.purchaseWithCurrencyButtonLabel.Visible = false;
			this.purchaseIAPButtonLabel.Visible = true;
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			bool flag = false;
			if (currentOffer != null)
			{
				string text;
				string text2;
				flag = targetedBundleController.FindAvailableIAP(currentOffer, out text, out text2);
				if (flag && !string.IsNullOrEmpty(text))
				{
					this.purchaseIAPButtonLabel.Text = this.lang.Get("TARGETED_BUNDLE_PURCHASE", new object[]
					{
						text2
					});
					this.purchaseButton.Tag = text;
				}
			}
			if (!flag)
			{
				this.purchaseButton.Visible = false;
				this.purchaseIAPButtonLabel.Visible = false;
				Service.Logger.Error("No iap available for targeted bundle screen: " + currentOffer.Uid);
			}
		}

		private void OnPurchaseButtonClicked(UXButton button)
		{
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			object tag = button.Tag;
			string iapId = null;
			if (tag != null)
			{
				iapId = button.Tag.ToString();
			}
			targetedBundleController.MakeTargetedBundlePurchase(this.currentOffer, iapId);
			this.Close(null);
		}
	}
}
