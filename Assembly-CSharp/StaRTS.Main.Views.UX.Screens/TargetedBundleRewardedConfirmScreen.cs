using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TargetedBundleRewardedConfirmScreen : ClosableScreen
	{
		private const string TITLE_LABEL = "LabelTitle";

		private const string DESCRIPTION_LABEL = "LabelDescription";

		private const string ITEM1_LABEL = "LabelItem1";

		private const string ITEM2_LABEL = "LabelItem2";

		private const string ITEM3_LABEL = "LabelItem3";

		private const string ITEM1_SPRITE = "SpriteRewardItem1";

		private const string ITEM2_SPRITE = "SpriteRewardItem2";

		private const string ITEM3_SPRITE = "SpriteRewardItem3";

		private const string CONFIRM_BUTTON = "BtnPrimary";

		private const string NAME_AND_AMOUNT = "AMOUNT_AND_NAME";

		private const int MAX_ITEMS = 3;

		private UXLabel titleLabel;

		private UXLabel descriptionLabel;

		private UXLabel itemLabel1;

		private UXLabel itemLabel2;

		private UXLabel itemLabel3;

		private UXLabel itemLabelTitle1;

		private UXLabel itemLabelTitle2;

		private UXLabel itemLabelTitle3;

		private UXLabel[] itemLabels;

		private UXSprite itemSprite1;

		private UXSprite itemSprite2;

		private UXSprite itemSprite3;

		private UXSprite[] itemSprites;

		private UXButton confirmButton;

		private TargetedBundleVO currentOffer;

		public TargetedBundleRewardedConfirmScreen(TargetedBundleVO offerVO) : base("gui_promotional_confirmation")
		{
			this.itemLabels = new UXLabel[3];
			this.itemSprites = new UXSprite[3];
			this.currentOffer = null;
			this.currentOffer = offerVO;
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
			this.itemLabels[0] = element;
			this.itemLabels[1] = element2;
			this.itemLabels[2] = element3;
			this.itemSprites[0] = element4;
			this.itemSprites[1] = element5;
			this.itemSprites[2] = element6;
			this.confirmButton = base.GetElement<UXButton>("BtnPrimary");
			this.confirmButton.OnClicked = new UXButtonClickedDelegate(this.OnConfirmButtonClicked);
			this.UpdateElements();
		}

		private void UpdateElements()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			if (this.currentOffer != null)
			{
				UXLabel element = base.GetElement<UXLabel>("LabelTitle");
				element.Text = this.lang.Get(this.currentOffer.Title, new object[0]);
				UXLabel element2 = base.GetElement<UXLabel>("LabelDescription");
				element2.Text = this.lang.Get(this.currentOffer.ConfirmationString, new object[0]);
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
							RewardUtils.SetRewardIcon(this.itemSprites[i], rewardComponentTag.RewardGeometryConfig, AnimationPreference.NoAnimation);
							j++;
						}
					}
					i++;
				}
				this.confirmButton.Tag = this.currentOffer;
			}
		}

		private void OnConfirmButtonClicked(UXButton button)
		{
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			targetedBundleController.LogTargetedBundleBI("purchase_complete", this.currentOffer);
			Service.EventManager.SendEvent(EventId.TargetedBundleRewardRedeemed, null);
			targetedBundleController.ResetOffer();
			this.Close(null);
		}
	}
}
