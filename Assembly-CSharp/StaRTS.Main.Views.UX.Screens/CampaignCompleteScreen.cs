using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class CampaignCompleteScreen : ScreenBase
	{
		private const string CAMPAIGN_TITLE_LABEL = "LabelCampaignTitle";

		private const string NEXT_CHAPTER_LABEL = "LabelNextChapter";

		private const string NEXT_CHAPTER_INTRO_LABEL = "LabelNextChapterIntro";

		private const string STARS_LABEL = "LabelStars";

		private const string STARS_FEEDBACK_LABEL = "LabelStarsFeedback";

		private const string REWARD_COLLECT_BUTTON = "BtnCollect";

		private const string REWARD_COLLECT_BUTTON_LABEL = "LabelBtnCollect";

		private const string NEXT_CHAPTER_BUTTON = "BtnNextChapter";

		private const string REWARDS_GROUP = "Rewards";

		private const string REWARD_GRID = "GridRewards";

		private const string REWARD_ITEM = "RewardItem";

		private const string REWARD_ITEM_LABEL = "LabelReward";

		private const string REWARD_ITEM_COUNT_LABEL = "LabelRewardCount";

		private const string REWARD_ITEM_REWARD = "SpriteReward";

		private const string REWARD_ITEM_TROOP = "SpriteTroop";

		private const string CONTINUE_BUTTON = "BtnContinue";

		private const string CONTINUE_BUTTON_TEXT = "LabelReplayBtn";

		private CampaignVO meta;

		private CampaignVO nextMeta;

		private CurrentPlayer cp;

		private int earnedStars;

		private int totalStars;

		private UXGrid rewardsGrid;

		public CampaignCompleteScreen(CampaignVO meta) : base("gui_chapter_complete")
		{
			this.meta = meta;
			this.cp = Service.CurrentPlayer;
			this.nextMeta = this.GetNextCampaign(meta);
			this.earnedStars = this.cp.CampaignProgress.GetTotalCampaignStarsEarned(meta);
			this.totalStars = meta.TotalMasteryStars;
		}

		protected override void OnScreenLoaded()
		{
			this.InitElements();
			this.InitLabels();
			this.InitButtons();
		}

		private void InitElements()
		{
			UXElement element = base.GetElement<UXElement>("Rewards");
			bool flag = this.earnedStars >= this.totalStars;
			element.Visible = flag;
			if (flag)
			{
				this.InitRewardsGrid();
			}
		}

		private void InitLabels()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelCampaignTitle");
			element.Text = LangUtils.GetCampaignTitle(this.meta);
			UXLabel element2 = base.GetElement<UXLabel>("LabelStars");
			element2.Text = this.lang.Get("LABEL_CAMPAIGN_STARS", new object[]
			{
				this.earnedStars,
				this.totalStars
			});
			UXLabel element3 = base.GetElement<UXLabel>("LabelStarsFeedback");
			if (this.earnedStars < this.totalStars)
			{
				element3.Text = this.lang.Get("CAMPAIGN_CHAPTER_COMPLETE", new object[0]);
			}
			else
			{
				element3.Text = this.lang.Get("CAMPAIGN_ALLSTARS_COMPLETE", new object[0]);
			}
			UXLabel element4 = base.GetElement<UXLabel>("LabelNextChapter");
			element4.Text = LangUtils.GetCampaignTitle(this.nextMeta);
			element4.Visible = (this.meta != this.nextMeta);
			UXLabel element5 = base.GetElement<UXLabel>("LabelNextChapterIntro");
			element5.Visible = (this.meta != this.nextMeta);
		}

		private void InitButtons()
		{
			UXButton element = base.GetElement<UXButton>("BtnNextChapter");
			element.OnClicked = new UXButtonClickedDelegate(this.OnNextChapterButtonClicked);
			element.Visible = (this.meta != this.nextMeta);
			base.GetElement<UXButton>("BtnContinue").OnClicked = new UXButtonClickedDelegate(this.Close);
			base.GetElement<UXLabel>("LabelReplayBtn").Text = this.lang.Get("OK", new object[0]);
		}

		private void OnNextChapterButtonClicked(UXButton button)
		{
			this.Close(true);
			Service.UXController.HUD.OpenPlanetViewScreen(CampaignScreenSection.PvE);
		}

		private void InitRewardsGrid()
		{
			this.rewardsGrid = base.GetElement<UXGrid>("GridRewards");
			this.rewardsGrid.SetTemplateItem("RewardItem");
			RewardVO rewardVO = Service.StaticDataController.Get<RewardVO>(this.meta.Reward);
			List<RewardComponentTag> rewardComponents = RewardUtils.GetRewardComponents(rewardVO);
			for (int i = 0; i < rewardComponents.Count; i++)
			{
				RewardComponentTag rewardComponentTag = rewardComponents[i];
				string itemUid = rewardVO.Uid + i;
				UXElement uXElement = this.rewardsGrid.CloneTemplateItem(itemUid);
				uXElement.Tag = rewardComponentTag;
				UXLabel subElement = this.rewardsGrid.GetSubElement<UXLabel>(itemUid, "LabelRewardCount");
				subElement.Text = rewardComponentTag.Quantity;
				RewardType type = rewardComponentTag.Type;
				UXSprite subElement2;
				if (type != RewardType.Building && type != RewardType.Currency)
				{
					subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUid, "SpriteTroop");
				}
				else
				{
					subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUid, "SpriteReward");
				}
				RewardUtils.SetRewardIcon(subElement2, rewardComponentTag.RewardGeometryConfig, AnimationPreference.NoAnimation);
				this.rewardsGrid.AddItem(uXElement, rewardComponentTag.Order);
				this.rewardsGrid.RepositionItems();
				this.rewardsGrid.Scroll(0.5f);
			}
			this.rewardsGrid.RepositionItems();
			this.rewardsGrid.Scroll(0.5f);
		}

		private CampaignVO GetNextCampaign(CampaignVO prev)
		{
			foreach (CampaignVO current in Service.StaticDataController.GetAll<CampaignVO>())
			{
				if (current.Faction == this.meta.Faction)
				{
					if (!current.Timed)
					{
						if (current.UnlockOrder == prev.UnlockOrder + 1)
						{
							return current;
						}
					}
				}
			}
			return prev;
		}

		public override void OnDestroyElement()
		{
			if (this.rewardsGrid != null)
			{
				this.rewardsGrid.Clear();
				this.rewardsGrid = null;
			}
			base.OnDestroyElement();
		}
	}
}
