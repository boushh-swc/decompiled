using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsChaptersViewModule : AbstractPlanetDetailsViewModule
	{
		private const string CAMPAIGN_SELECT_GROUP = "PanelCampaignSelect";

		private const string CAMPAIGN_DETAILS_GROUP = "PanelCampaignDetails";

		private const string CAMPAIGN_GRID = "CampaignGrid";

		private const string CAMPAIGN_ITEM_TEMPLATE = "CampaignTemplate";

		public const string CAMPAIGN_ITEM_BUTTON = "ButtonCampaignCard";

		private const string CAMPAIGN_ITEM_NAME_LABEL = "LabelCampaignName";

		private const string CAMPAIGN_ITEM_DESCRIPTION_LABEL = "LabelChapterDescription";

		private const string CAMPAIGN_ITEM_MISSIONS_LABEL = "LabelObjectivesCount";

		private const string CAMPAIGN_ITEM_REWARD_SPRITE = "SpriteChapterRewardItem";

		private const string CAMPAIGN_ITEM_STARS_LABEL = "LabelStarsCount";

		private const string CAMPAIGN_ITEM_UNLOCKS_LABEL = "LabelUnlocks";

		private const string CAMPAIGN_ITEM_COMPLETE_LABEL = "LabelCampaignComplete";

		private const string CAMPAIGN_ITEM_CHECK_SPRITE = "SpriteIconCheck";

		private const string CAMPAIGN_ITEM_BG_SPRITE = "SpriteCampaignImage";

		private const string CAMPAIGN_ITEM_PROGRESS_SLIDER = "ChapterPbar";

		private const string CAMPAIGN_ITEM_CHAPTER_LABEL = "LabelChapterNumberSelect";

		private const string CAMPAIGN_ITEM_CHAPTER_REWARD_LABEL = "LabelChapterReward";

		private const string CAMPAIGN_ITEM_CHAPTER_REWARDED_LABEL = "LabelChapterRewarded";

		private const string CS_OBJECTIVES_COUNT = "LabelObjectivesCount";

		private const string CAMPAIGN_ITEM_LOCK_SPRITE = "SpriteIcoChapterLocked";

		private const string CHAPTER_BACK_BUTTON = "BtnBackChapter";

		private const string CLOSE_CAMPAIGN_BUTTON = "BtnCloseCampaign";

		private UXButton backButtonChapter;

		private UXButton closeCampaignButton;

		private UXElement campaignDetailsGroup;

		private UXElement campaignSelectGroup;

		private UXGrid campaignGrid;

		private bool campaignGroupInitialized;

		public PlanetDetailsChaptersViewModule(PlanetDetailsScreen screen) : base(screen)
		{
		}

		public void OnScreenLoaded()
		{
			this.campaignSelectGroup = this.screen.GetElement<UXElement>("PanelCampaignSelect");
			this.campaignSelectGroup.Visible = false;
			this.campaignDetailsGroup = this.screen.GetElement<UXElement>("PanelCampaignDetails");
			this.backButtonChapter = this.screen.GetElement<UXButton>("BtnBackChapter");
			this.backButtonChapter.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.closeCampaignButton = this.screen.GetElement<UXButton>("BtnCloseCampaign");
			this.closeCampaignButton.OnClicked = new UXButtonClickedDelegate(this.CloseCampaignClicked);
			this.screen.BackButtons.Add(this.backButtonChapter);
		}

		public void RefreshScreenForPlanetChange()
		{
		}

		public void InitCampaignGrid()
		{
			this.screen.CurrentBackButton = this.backButtonChapter;
			this.screen.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.campaignSelectGroup.Visible = true;
			if (this.campaignGroupInitialized)
			{
				return;
			}
			this.campaignGroupInitialized = true;
			this.campaignGrid = this.screen.GetElement<UXGrid>("CampaignGrid");
			this.campaignGrid.SetTemplateItem("CampaignTemplate");
			foreach (CampaignVO current in base.Sdc.GetAll<CampaignVO>())
			{
				if (!current.Timed && current.Faction == base.Player.Faction)
				{
					string uid = current.Uid;
					UXElement uXElement = this.campaignGrid.CloneTemplateItem(uid);
					uXElement.Tag = current;
					int totalCampaignStarsEarned = base.Player.CampaignProgress.GetTotalCampaignStarsEarned(current);
					int totalMasteryStars = current.TotalMasteryStars;
					UXLabel subElement = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelCampaignName");
					subElement.Text = LangUtils.GetCampaignTitle(current);
					UXLabel subElement2 = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelChapterDescription");
					subElement2.Text = LangUtils.GetCampaignDescription(current);
					UXLabel subElement3 = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelStarsCount");
					subElement3.Text = base.LangController.Get("LABEL_CAMPAIGN_STARS", new object[]
					{
						totalCampaignStarsEarned,
						totalMasteryStars
					});
					UXButton subElement4 = this.campaignGrid.GetSubElement<UXButton>(uid, "ButtonCampaignCard");
					subElement4.Tag = current;
					subElement4.OnClicked = new UXButtonClickedDelegate(this.OnCampaignItemButtonClicked);
					UXTexture subElement5 = this.campaignGrid.GetSubElement<UXTexture>(uid, "SpriteCampaignImage");
					subElement5.LoadTexture(current.Uid);
					if (!base.Player.CampaignProgress.HasCampaign(current))
					{
						subElement4.Enabled = false;
					}
					UXSprite subElement6 = this.campaignGrid.GetSubElement<UXSprite>(uid, "SpriteIcoChapterLocked");
					subElement6.Visible = !subElement4.Enabled;
					UXLabel subElement7 = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelChapterNumberSelect");
					this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelChapterReward").Text = base.LangController.Get("s_Rewards", new object[0]);
					subElement7.Text = base.LangController.Get("CHAPTER_NUMBER", new object[]
					{
						current.UnlockOrder
					});
					UXLabel subElement8 = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelObjectivesCount");
					UXLabel subElement9 = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelCampaignComplete");
					UXSprite subElement10 = this.campaignGrid.GetSubElement<UXSprite>(uid, "SpriteIconCheck");
					UXLabel subElement11 = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelUnlocks");
					UXSprite subElement12 = this.campaignGrid.GetSubElement<UXSprite>(uid, "SpriteChapterRewardItem");
					UXLabel subElement13 = this.campaignGrid.GetSubElement<UXLabel>(uid, "LabelChapterRewarded");
					int totalCampaignMissionsCompleted = base.Player.CampaignProgress.GetTotalCampaignMissionsCompleted(current);
					int totalMissions = current.TotalMissions;
					IGeometryVO config = null;
					RewardType rewardType = RewardType.Invalid;
					base.RManager.GetFirstRewardAssetName(current.Reward, ref rewardType, out config);
					RewardUtils.SetRewardIcon(subElement12, config, AnimationPreference.NoAnimation);
					if (totalCampaignMissionsCompleted > 0 && totalCampaignMissionsCompleted == totalMissions)
					{
						subElement8.Visible = false;
						subElement9.Text = base.LangController.Get("LABEL_CAMPAIGN_COMPLETE", new object[0]);
						subElement11.Text = base.RManager.GetRewardString(current.Reward);
						bool flag = totalCampaignStarsEarned >= totalMasteryStars;
						subElement13.Visible = flag;
						subElement12.Visible = !flag;
						subElement11.Visible = !flag;
					}
					else
					{
						subElement9.Visible = false;
						subElement10.Visible = false;
						subElement8.Text = base.LangController.Get("LABEL_CAMPAIGN_MISSIONS", new object[]
						{
							totalCampaignMissionsCompleted,
							totalMissions
						});
						subElement11.Text = base.RManager.GetRewardString(current.Reward);
					}
					UXSlider subElement14 = this.campaignGrid.GetSubElement<UXSlider>(uid, "ChapterPbar");
					subElement14.Value = ((totalMissions != 0) ? ((float)totalCampaignMissionsCompleted / (float)totalMissions) : 0f);
					this.campaignGrid.AddItem(uXElement, current.UnlockOrder);
				}
			}
			this.campaignGrid.RepositionItems();
		}

		private void OnCampaignItemButtonClicked(UXButton button)
		{
			this.screen.SelectCampaign((CampaignVO)button.Tag);
			this.campaignSelectGroup.Visible = false;
		}

		private void OnBackButtonClicked(UXButton button)
		{
			if (this.campaignSelectGroup.Visible)
			{
				this.campaignDetailsGroup.Visible = true;
				this.campaignSelectGroup.Visible = false;
				this.screen.CurrentBackDelegate = new UXButtonClickedDelegate(this.screen.pveView.BackButtonClickedHelper);
			}
			base.EvtManager.SendEvent(EventId.BackButtonClicked, null);
		}

		private void CloseCampaignClicked(UXButton button)
		{
			this.screen.pveView.ReturnToMainSelect();
			this.screen.UpdateCurrentPlanet(this.screen.CurrentPlanet);
		}

		public void Destroy()
		{
			if (this.campaignGrid != null)
			{
				this.campaignGrid.Clear();
				this.campaignGrid = null;
			}
		}
	}
}
