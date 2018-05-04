using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsMissionViewModule : AbstractPlanetDetailsViewModule, IEventObserver, IViewFrameTimeObserver
	{
		private const string ATTACKS_LEFT_STRING = "ATTACKS_LEFT";

		private const string ELITE_OPS_REWARD_ITEM_PREFIX = "RewardItem";

		private const string CHAPTER_JEWEL = "ContainerJewelChapter";

		private const string CAMPAIGN_ITEM_REWARD_SPRITE = "SpriteChapterRewardItem";

		private const string CAMPAIGN_REWARD_BUTTON = "BtnViewCampaignRewards";

		private const string CAMPAIGN_DETAILS_TIME_LEFT = "CampaignDetailsTimeLeft";

		private const string CAMPAIGN_DETAILS_TIME_LEFT_LABEL = "LabelCampaignTimeLeft";

		private const string CAMPAIGN_DETAIL_MISSIONS_STRING = "LABEL_CAMPAIGN_DETAIL_MISSIONS";

		private const string CAMPAIGNS_LABEL = "LabelCampaigns";

		private const string CAMPAIGN_TITLE_LABEL = "LabelDetailsTitle";

		private const string CAMPAIGN_STARS_LABEL = "LabelStarsCountDetails";

		private const string CAMPAIGN_IMAGE_SPRITE = "SpriteCampaignImageLarge";

		private const string CAMPAIGN_POINTS_ICON = "icoCampaignPoints";

		private const string CAMPAIGN_POINTS_STRING = "CAMPAIGN_POINTS";

		private const string CAMPAIGN_STARS_STRING = "LABEL_CAMPAIGN_STARS";

		private const string CAMPAIGN_DETAILS_GROUP = "PanelCampaignDetails";

		private const string CD_ALL_CHAPTERS_BUTTON = "BtnAllChapters";

		private const string CD_CAMPAIGN_INFO_BUTTON = "BtnChapterInfo";

		private const string CD_CAMPAIGN_DESCRIPTION_LABEL = "LabelCampaignDescription";

		private const string CD_MASTERY_UNLOCK_DETAILS_LABEL = "LabelUnlocksDetails";

		private const string CD_MASTERY_REWARD_GROUP = "MasteryReward";

		private const string CD_MASTERY_PROGRESS_GROUP = "MasteryProgress";

		private const string CD_CAMPAIGN_DESCRIPTION_GROUP = "ContainerChapterDescription";

		private const string CD_MASTERY_REWARD_SPRITE = "SpriteMasteryReward";

		private const string CURRENCY_VALUE_NAME_STRING = "CURRENCY_VALUE_NAME";

		private const string EVENT_INFO_GROUP = "EventInfo";

		private const string MISSION_DETAILS_GROUP = "ObjectiveDetails";

		private const string MISSION_REWARDS_GRID = "MissionRewardsGrid";

		private const string MISSION_REWARDS_ITEM_TEMPLATE = "MissionRewardsItem";

		private const string MISSION_REWARDS_ITEM_LABEL = "LabelMissionReward";

		private const string MISSION_REWARDS_ITEM_SPRITE = "SpriteMissionReward";

		private const string MISSION_GRID = "ObjectiveGrid";

		private const string MISSION_ITEM_TEMPLATE = "ObjectiveTemplate";

		public const string MISSION_ITEM_BUTTON = "ButtonObjectiveCard";

		private const string MISSION_ITEM_NUMBER_LABEL = "LabelObjectiveNumber";

		private const string MISSION_ITEM_STAR_GROUP = "ObjectiveStars";

		private const string MISSION_ITEM_STAR_SPRITE = "SpriteStar";

		private const string MISSION_ITEM_CHECK_SPRITE = "SpriteMissionCheck";

		private const string MISSION_ITEM_IMAGE_SPRITE = "SpriteObjectiveImage";

		private const string MISSION_ITEM_LOCK_SPRITE = "SpriteIcoMissionLocked";

		private const string MISSION_ITEM_PROGRESS_BAR = "MissionSelectPbar";

		private const string MISSION_DIFFICULTY_GRID = "ObjectiveGridDifficulty";

		private const string MISSION_DIFFICULTY_ITEM_TEMPLATE = "ObjectiveTemplateDifficulty";

		private const string MISSION_DIFFICULTY_ITEM_BUTTON = "ButtonObjectiveCardDifficulty";

		private const string MISSION_DIFFICULTY_ITEM_LABEL = "LabelDifficulty";

		private const string MISSION_ITEM_STAR_ON = "CampaignStarOn";

		private const string MISSION_ITEM_STAR_OFF = "CampaignStarOff";

		private const string MISSION_ACTION_BUTTON = "BtnAction";

		private const string MISSION_ACTION_BUTTON_LABEL = "LabelBtnAction";

		private const string MISSION_COLLECT_BUTTON = "BtnCollect";

		private const string MISSION_DESC_LABEL = "LabelObjectiveDetails";

		private const string MISSION_TITLE_LABEL = "LabelObjectiveName";

		private const string MISSION_REWARDS_PANEL = "PanelMissionRewards";

		private const string MISSION_REWARDS_LABEL = "LabelMissionReward";

		private const string MISSION_REWARDS_TITLE_LABEL = "LabelMissionRewardTitle";

		private const string MISSION_REWARDED_LABEL = "LabelRewarded";

		private const string MISSION_PROGRESS_LABEL = "LabelMissionProgress";

		private const string MISSION_PROGRESS_DESCRIPTION_LABEL = "LabelMissionProgressDescription";

		private const string MISSION_STATUS_LABEL = "LabelMissionStatus";

		private const string MISSION_PROGRESS_BAR = "MissionPbar";

		private const string MISSION_DETAIL_STAR_PREFIX = "MissionSpriteStar";

		private const string MISSION_COUNTDOWN_LABEL = "Countdown";

		private const string MISSION_ICON_BUILD = "IcoBuild";

		private const string MISSION_ICON_ATTACK = "IcoAttack";

		private const string MISSION_ICON_DEFEND = "icoDefend";

		private const string MISSION_ICON_GRIND = "icoRecBattle";

		private const string MISSION_ICON_PVP = "IcoWar";

		private const string MISSION_ICON_CHALLENGE = "IcoMissions";

		private const string NOT_ENOUGH_TROOPS_TITLE_STRING = "NOT_ENOUGH_TROOPS_TITLE";

		private const string NOT_ENOUGH_TROOPS_FOR_ATTACK_STRING = "NOT_ENOUGH_TROOPS_FOR_ATTACK";

		private const string PLANET_DETAILS_TOP = "PlanetDetailsTop";

		private const string PLANET_DETAILS_BOTTOM = "PlanetDetailsBottom";

		private const string LANG_ALL_CHAPTERS_LABEL = "s_ViewAllChapters";

		private const string CS_OBJECTIVES_COUNT = "LabelObjectivesCount";

		private const string CAMPAIGN_ITEM_LOCK_SPRITE = "SpriteIcoChapterLocked";

		private const string MISSION_ICON_HERO_DEFEND = "icoHeroDefend";

		private const int VISIBLE_MISSION_COUNT = 9;

		private const string BUTTON_BLUE_BACKGROUND = "BtnBlue";

		private const string BTN_CLOSE_CAMPAIGN_DETAILS = "BtnCloseCampaignDetails";

		private static readonly Color COMPLETION_COLOR = new Color(255f, 222f, 0f);

		private UXElement campaignDescriptionGroup;

		private UXElement campaignDetailsGroup;

		private UXElement campaignDetailsTimeLeft;

		private UXElement chapterJewel;

		private UXElement missionRewardsPanel;

		private UXElement planetDetailsTop;

		private UXElement planetDetailsBottom;

		private UXGrid missionGrid;

		private UXGrid rewardsGrid;

		private UXButton allChaptersButton;

		private UXButton backButtonChapter;

		private UXButton closeButton;

		public UXCheckbox selectedMissionCheckbox;

		private UXLabel missionCountdown;

		private UXLabel rewardedLabel;

		protected TimedEventCountdownHelper countdownHelper;

		public CampaignVO selectedCampaign;

		public CampaignMissionVO selectedMission;

		private int missionSelectIndexOnFrameDelay = -1;

		public PlanetDetailsMissionViewModule(PlanetDetailsScreen screen) : base(screen)
		{
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.MissionCompleted:
				this.OnMissionCompleted(cookie as CampaignMissionVO);
				break;
			case EventId.MissionCollected:
				this.OnRewardCollected(cookie as CampaignMissionVO);
				break;
			}
			return EatResponse.NotEaten;
		}

		public void OnViewFrameTime(float dt)
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.OnMissionFrameDelayed(this.missionSelectIndexOnFrameDelay);
			this.missionSelectIndexOnFrameDelay = -1;
		}

		private void OnMissionFrameDelayed(int i)
		{
			if (i < this.missionGrid.Count)
			{
				if (i >= 9)
				{
					this.missionGrid.ScrollToItem(i);
				}
				CampaignMissionVO campaignMissionVO = (CampaignMissionVO)this.missionGrid.GetItem(i).Tag;
				UXCheckbox uXCheckbox = (!this.selectedCampaign.IsMiniCampaign()) ? this.missionGrid.GetSubElement<UXCheckbox>(campaignMissionVO.Uid, "ButtonObjectiveCard") : this.missionGrid.GetSubElement<UXCheckbox>(campaignMissionVO.Uid, "ButtonObjectiveCardDifficulty");
				uXCheckbox.Selected = true;
				this.OnMissionItemSelected(uXCheckbox, true);
			}
		}

		public void OnScreenLoaded()
		{
			this.closeButton = this.screen.GetElement<UXButton>("BtnCloseCampaignDetails");
			this.closeButton.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.screen.BackButtons.Add(this.closeButton);
			this.campaignDetailsGroup = this.screen.GetElement<UXElement>("PanelCampaignDetails");
			this.campaignDetailsGroup.Visible = false;
			this.eventInfoGroup = this.screen.GetElement<UXElement>("EventInfo");
			this.planetDetailsTop = this.screen.GetElement<UXElement>("PlanetDetailsTop");
			this.planetDetailsBottom = this.screen.GetElement<UXElement>("PlanetDetailsBottom");
			this.chapterJewel = this.screen.GetElement<UXElement>("ContainerJewelChapter");
			this.allChaptersButton = this.screen.GetElement<UXButton>("BtnAllChapters");
			this.allChaptersButton.OnClicked = new UXButtonClickedDelegate(this.OnAllChaptersButtonClicked);
			this.missionRewardsPanel = this.screen.GetElement<UXElement>("PanelMissionRewards");
			base.EvtManager.RegisterObserver(this, EventId.MissionCollected, EventPriority.Default);
			base.EvtManager.RegisterObserver(this, EventId.MissionCompleted, EventPriority.Default);
		}

		public void RefreshScreenForPlanetChange()
		{
		}

		public void InitCampaignDetailScreen()
		{
			UXLabel element = this.screen.GetElement<UXLabel>("LabelCampaignDescription");
			element.Text = LangUtils.GetCampaignDescription(this.selectedCampaign);
			this.campaignDescriptionGroup = this.screen.GetElement<UXElement>("ContainerChapterDescription");
			this.campaignDescriptionGroup.Visible = false;
			UXButton element2 = this.screen.GetElement<UXButton>("BtnChapterInfo");
			element2.OnClicked = new UXButtonClickedDelegate(this.OnCampaignInfoButtonClicked);
			UXLabel element3 = this.screen.GetElement<UXLabel>("LabelCampaigns");
			element3.Text = base.LangController.Get("s_ViewAllChapters", new object[0]);
			UXLabel element4 = this.screen.GetElement<UXLabel>("LabelDetailsTitle");
			element4.Text = LangUtils.GetCampaignTitle(this.selectedCampaign);
			this.missionCountdown = this.screen.GetElement<UXLabel>("Countdown");
			this.missionCountdown.Visible = false;
			UXButton element5 = this.screen.GetElement<UXButton>("BtnViewCampaignRewards");
			element5.Visible = false;
			this.campaignDetailsTimeLeft = this.screen.GetElement<UXElement>("CampaignDetailsTimeLeft");
			this.campaignDetailsTimeLeft.Visible = false;
			int totalCampaignStarsEarned = base.Player.CampaignProgress.GetTotalCampaignStarsEarned(this.selectedCampaign);
			UXLabel element6 = this.screen.GetElement<UXLabel>("LabelStarsCountDetails");
			element6.Text = base.LangController.Get("LABEL_CAMPAIGN_STARS", new object[]
			{
				totalCampaignStarsEarned,
				this.selectedCampaign.TotalMasteryStars
			});
			UXSprite element7 = this.screen.GetElement<UXSprite>("SpriteMasteryReward");
			RewardType rewardType = RewardType.Invalid;
			IGeometryVO config;
			base.RManager.GetFirstRewardAssetName(this.selectedCampaign.Reward, ref rewardType, out config);
			RewardUtils.SetRewardIcon(element7, config, AnimationPreference.NoAnimation);
			int totalCampaignMissionsCompleted = base.Player.CampaignProgress.GetTotalCampaignMissionsCompleted(this.selectedCampaign);
			UXLabel element8 = this.screen.GetElement<UXLabel>("LabelObjectivesCount");
			element8.Text = base.LangController.Get("LABEL_CAMPAIGN_DETAIL_MISSIONS", new object[]
			{
				totalCampaignMissionsCompleted,
				this.selectedCampaign.TotalMissions
			});
			UXLabel element9 = this.screen.GetElement<UXLabel>("LabelUnlocksDetails");
			if (totalCampaignMissionsCompleted > 0 && totalCampaignMissionsCompleted == this.selectedCampaign.TotalMissions)
			{
				element9.Text = base.RManager.GetRewardString(this.selectedCampaign.Reward);
				if (totalCampaignStarsEarned == this.selectedCampaign.TotalMasteryStars)
				{
					element9.TextColor = PlanetDetailsMissionViewModule.COMPLETION_COLOR;
				}
			}
			else
			{
				element9.Text = base.RManager.GetRewardString(this.selectedCampaign.Reward);
			}
			UXTexture element10 = this.screen.GetElement<UXTexture>("SpriteCampaignImageLarge");
			element10.LoadTexture(this.selectedCampaign.Uid);
			this.screen.GetElement<UXElement>("MasteryProgress").Visible = true;
			this.screen.GetElement<UXElement>("MasteryReward").Visible = true;
			this.allChaptersButton.Visible = true;
			this.InitMissionGrid();
		}

		private void InitMissionGrid()
		{
			CampaignMissionVO campaignMissionVO = null;
			List<CampaignMissionVO> list = new List<CampaignMissionVO>();
			foreach (CampaignMissionVO current in base.Sdc.GetAll<CampaignMissionVO>())
			{
				if (current.CampaignUid == this.selectedCampaign.Uid)
				{
					list.Add(current);
				}
			}
			list.Sort(new Comparison<CampaignMissionVO>(PlanetDetailsMissionViewModule.CompareMissions));
			if (this.selectedCampaign.IsMiniCampaign())
			{
				this.missionGrid = this.screen.GetElement<UXGrid>("ObjectiveGridDifficulty");
				this.missionGrid.SetTemplateItem("ObjectiveTemplateDifficulty");
				this.screen.GetElement<UXGrid>("ObjectiveGrid").Visible = false;
			}
			else
			{
				this.missionGrid = this.screen.GetElement<UXGrid>("ObjectiveGrid");
				this.missionGrid.SetTemplateItem("ObjectiveTemplate");
				this.screen.GetElement<UXGrid>("ObjectiveGridDifficulty").Visible = false;
			}
			this.missionGrid.Clear();
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				CampaignMissionVO campaignMissionVO2 = list[i];
				string uid = campaignMissionVO2.Uid;
				UXElement uXElement = this.missionGrid.CloneTemplateItem(uid);
				this.missionGrid.AddItem(uXElement, i);
				this.UpdateMissionItem(uid, campaignMissionVO2, false);
				uXElement.Tag = campaignMissionVO2;
				if (!base.Player.CampaignProgress.IsMissionLocked(campaignMissionVO2) && (campaignMissionVO == null || campaignMissionVO2.UnlockOrder > campaignMissionVO.UnlockOrder))
				{
					campaignMissionVO = campaignMissionVO2;
				}
				i++;
			}
			this.missionGrid.RepositionItems();
			int j = 0;
			int count2 = this.missionGrid.Count;
			while (j < count2)
			{
				if (campaignMissionVO == (CampaignMissionVO)this.missionGrid.GetItem(j).Tag)
				{
					this.missionSelectIndexOnFrameDelay = j;
					Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
					break;
				}
				j++;
			}
			this.InitMissionDetailGroup();
		}

		private void InitMissionDetailGroup()
		{
			this.screen.GetElement<UXElement>("ObjectiveDetails").Visible = (this.selectedMission != null);
			if (this.selectedMission == null)
			{
				return;
			}
			UXLabel element = this.screen.GetElement<UXLabel>("LabelMissionProgress");
			element.Visible = false;
			UXLabel element2 = this.screen.GetElement<UXLabel>("LabelMissionProgressDescription");
			element2.Visible = false;
			UXLabel element3 = this.screen.GetElement<UXLabel>("LabelMissionStatus");
			element3.Visible = false;
			UXSlider element4 = this.screen.GetElement<UXSlider>("MissionPbar");
			element4.Visible = false;
			UXButton element5 = this.screen.GetElement<UXButton>("BtnAction");
			bool flag = this.selectedMission.Grind && base.Player.CampaignProgress.IsGrindComplete(this.selectedMission);
			this.missionCountdown.Visible = this.selectedMission.Grind;
			if (this.selectedMission.Grind)
			{
				int retriesLeft = base.Player.CampaignProgress.GetRetriesLeft(this.selectedMission);
				this.missionCountdown.Text = base.LangController.Get("ATTACKS_LEFT", new object[]
				{
					retriesLeft
				});
			}
			if (flag)
			{
				element5.Visible = false;
			}
			else if (base.Player.CampaignProgress.IsMissionInProgress(this.selectedMission) && this.selectedMission.MissionType != MissionType.Pvp)
			{
				int num;
				int num2;
				base.Player.CampaignProgress.GetMissionProgress(this.selectedMission, out num, out num2);
				if (this.selectedMission.ProgressString != null)
				{
					element2.Text = base.LangController.Get(this.selectedMission.ProgressString, new object[]
					{
						num,
						num2
					});
					element2.Visible = true;
				}
				element4.Value = (float)num / (float)num2;
				element4.Visible = true;
				element.Visible = true;
				element5.Visible = false;
			}
			else if (base.Player.CampaignProgress.CanReplay(this.selectedMission))
			{
				element5.Visible = true;
				element5.Enabled = true;
				UXLabel element6 = this.screen.GetElement<UXLabel>("LabelBtnAction");
				element6.Text = LangUtils.GetMissionButtonDisplayText(this.selectedMission.MissionType);
				element5.OnClicked = new UXButtonClickedDelegate(this.OnActionButtonClicked);
			}
			else
			{
				element5.Visible = false;
			}
			UXButton element7 = this.screen.GetElement<UXButton>("BtnCollect");
			this.rewardedLabel = this.screen.GetElement<UXLabel>("LabelRewarded");
			bool flag2 = !this.selectedMission.Grind && base.Player.CampaignProgress.IsMissionCompleted(this.selectedMission);
			bool flag3 = base.Player.CampaignProgress.IsMissionCollected(this.selectedMission);
			bool flag4 = GameUtils.IsMissionSpecOps(this.selectedMission.Uid);
			int missionEarnedStars = base.Player.CampaignProgress.GetMissionEarnedStars(this.selectedMission);
			if (flag2 && !flag3 && flag4 && missionEarnedStars == 3)
			{
				flag3 = true;
			}
			bool flag5 = !string.IsNullOrEmpty(this.selectedMission.Rewards) && this.selectedMission.CampaignPoints == 0;
			if (flag3 || flag)
			{
				this.rewardedLabel.Visible = true;
				element7.Visible = false;
				this.missionRewardsPanel.Visible = false;
			}
			else
			{
				this.rewardedLabel.Visible = false;
				if (flag2 && flag5)
				{
					element7.Visible = true;
					element7.OnClicked = new UXButtonClickedDelegate(this.OnCollectButtonClicked);
				}
				else
				{
					element7.Visible = false;
				}
				this.missionRewardsPanel.Visible = true;
			}
			UXLabel element8 = this.screen.GetElement<UXLabel>("LabelObjectiveDetails");
			element8.Text = LangUtils.GetMissionDescription(this.selectedMission);
			UXLabel element9 = this.screen.GetElement<UXLabel>("LabelObjectiveName");
			element9.Text = LangUtils.GetMissionTitle(this.selectedMission);
			this.InitMissionRewardsGrid(this.selectedMission.Rewards, base.Player.CampaignProgress.RemainingCampaignPointsForMission(this.selectedMission));
			for (int i = 1; i <= 3; i++)
			{
				UXSprite element10 = this.screen.GetElement<UXSprite>("MissionSpriteStar" + i);
				element10.SpriteName = ((missionEarnedStars < i) ? "CampaignStarOff" : "CampaignStarOn");
			}
			if (!GameUtils.IsPlanetCurrentOne("planet1"))
			{
				element5.Enabled = false;
				element5.GetUIWidget.color = UXUtils.COLOR_COST_LOCKED;
			}
		}

		private void InitMissionRewardsGrid(string rewardUid, int campaignPoints)
		{
			this.rewardsGrid = this.screen.GetElement<UXGrid>("MissionRewardsGrid");
			this.rewardsGrid.Clear();
			this.rewardsGrid.SetTemplateItem("MissionRewardsItem");
			if (string.IsNullOrEmpty(rewardUid) && campaignPoints <= 0)
			{
				return;
			}
			if (campaignPoints > 0)
			{
				string itemUid = "RewardItem0";
				UXElement item = this.rewardsGrid.CloneTemplateItem(itemUid);
				UXLabel subElement = this.rewardsGrid.GetSubElement<UXLabel>(itemUid, "LabelMissionReward");
				subElement.Text = base.LangController.Get("CAMPAIGN_POINTS", new object[]
				{
					campaignPoints
				});
				UXSprite subElement2 = this.rewardsGrid.GetSubElement<UXSprite>(itemUid, "SpriteMissionReward");
				subElement2.Visible = true;
				subElement2.SpriteName = "icoCampaignPoints";
				this.rewardsGrid.AddItem(item, 0);
			}
			else
			{
				RewardVO rewardVO = base.Sdc.Get<RewardVO>(rewardUid);
				List<RewardComponentTag> rewardComponents = RewardUtils.GetRewardComponents(rewardVO);
				int i = 0;
				int count = rewardComponents.Count;
				while (i < count)
				{
					string itemUid2 = "RewardItem" + i;
					RewardComponentTag rewardComponentTag = rewardComponents[i];
					UXElement item2 = this.rewardsGrid.CloneTemplateItem(itemUid2);
					UXLabel subElement3 = this.rewardsGrid.GetSubElement<UXLabel>(itemUid2, "LabelMissionReward");
					subElement3.Text = base.LangController.Get("CURRENCY_VALUE_NAME", new object[]
					{
						rewardComponentTag.Quantity,
						rewardComponentTag.RewardName
					});
					UXSprite subElement4 = this.rewardsGrid.GetSubElement<UXSprite>(itemUid2, "SpriteMissionReward");
					RewardUtils.SetRewardIcon(subElement4, rewardComponentTag.RewardGeometryConfig, AnimationPreference.NoAnimation);
					this.rewardsGrid.AddItem(item2, i);
					i++;
				}
			}
			this.rewardsGrid.RepositionItems();
			this.rewardsGrid.Scroll(0.5f);
		}

		private void UpdateMissionItem(string itemUid, CampaignMissionVO missionType, bool selected)
		{
			UXSprite uXSprite = (!this.selectedCampaign.IsMiniCampaign()) ? this.missionGrid.GetSubElement<UXSprite>(itemUid, "SpriteObjectiveImage") : null;
			if (uXSprite != null)
			{
				if (missionType.Grind)
				{
					uXSprite.SpriteName = "icoRecBattle";
				}
				else if (missionType.IsChallengeMission())
				{
					uXSprite.SpriteName = "IcoMissions";
				}
				else if (missionType.IsCombatMission())
				{
					uXSprite.SpriteName = ((missionType.MissionType != MissionType.Attack) ? "icoDefend" : "IcoAttack");
				}
				else if (missionType.HasPvpCondition() || missionType.MissionType == MissionType.Pvp)
				{
					uXSprite.SpriteName = "IcoWar";
				}
				else
				{
					uXSprite.SpriteName = "IcoBuild";
				}
			}
			UXCheckbox uXCheckbox = (!this.selectedCampaign.IsMiniCampaign()) ? this.missionGrid.GetSubElement<UXCheckbox>(itemUid, "ButtonObjectiveCard") : this.missionGrid.GetSubElement<UXCheckbox>(itemUid, "ButtonObjectiveCardDifficulty");
			uXCheckbox.Tag = missionType;
			uXCheckbox.OnSelected = new UXCheckboxSelectedDelegate(this.OnMissionItemSelected);
			uXCheckbox.Selected = selected;
			uXCheckbox.RadioGroup = 0;
			if (this.selectedCampaign.IsMiniCampaign())
			{
				UXLabel subElement = this.missionGrid.GetSubElement<UXLabel>(itemUid, "LabelDifficulty");
				subElement.Text = LangUtils.GetMissionDifficultyLabel(itemUid);
				uXCheckbox.Visible = !base.Player.CampaignProgress.IsMissionLocked(missionType);
				return;
			}
			UXLabel subElement2 = this.missionGrid.GetSubElement<UXLabel>(itemUid, "LabelObjectiveNumber");
			subElement2.Text = missionType.UnlockOrder.ToString();
			UXSprite subElement3 = this.missionGrid.GetSubElement<UXSprite>(itemUid, "SpriteMissionCheck");
			UXSprite subElement4 = this.missionGrid.GetSubElement<UXSprite>(itemUid, "SpriteIcoMissionLocked");
			UXElement subElement5 = this.missionGrid.GetSubElement<UXElement>(itemUid, "ObjectiveStars");
			UXSlider subElement6 = this.missionGrid.GetSubElement<UXSlider>(itemUid, "MissionSelectPbar");
			if (base.Player.CampaignProgress.IsMissionInProgress(missionType) && missionType.MissionType != MissionType.Pvp)
			{
				int num;
				int num2;
				base.Player.CampaignProgress.GetMissionProgress(missionType, out num, out num2);
				subElement6.Value = (float)num / (float)num2;
			}
			else
			{
				subElement6.Visible = false;
			}
			if (base.Player.CampaignProgress.IsMissionLocked(missionType))
			{
				uXCheckbox.Enabled = false;
				uXSprite.Visible = false;
				subElement3.Visible = false;
				subElement5.Visible = false;
				subElement6.Visible = false;
				subElement4.Visible = true;
				return;
			}
			subElement4.Visible = false;
			subElement5.Visible = true;
			int missionEarnedStars = base.Player.CampaignProgress.GetMissionEarnedStars(missionType);
			for (int i = 1; i <= missionEarnedStars; i++)
			{
				UXSprite subElement7 = this.missionGrid.GetSubElement<UXSprite>(itemUid, "SpriteStar" + i);
				subElement7.SpriteName = "CampaignStarOn";
			}
			if (missionType.Grind && base.Player.CampaignProgress.IsGrindComplete(missionType))
			{
				subElement3.Visible = true;
				uXSprite.Visible = false;
			}
			else if (base.Player.CampaignProgress.IsMissionCompleted(missionType))
			{
				if (base.Player.CampaignProgress.IsMissionCollected(missionType))
				{
					subElement3.Visible = true;
					uXSprite.Visible = false;
				}
				else
				{
					uXSprite.Visible = true;
					subElement3.Visible = false;
				}
			}
			else
			{
				subElement3.Visible = false;
			}
		}

		private void OnMissionItemSelected(UXCheckbox checkbox, bool selected)
		{
			if (this.selectedMissionCheckbox == checkbox)
			{
				if (!selected)
				{
					this.selectedMission = null;
					this.selectedMissionCheckbox = null;
					this.InitMissionDetailGroup();
				}
				return;
			}
			if (!selected)
			{
				return;
			}
			UXCheckbox uXCheckbox = this.selectedMissionCheckbox;
			this.selectedMissionCheckbox = checkbox;
			if (uXCheckbox != null)
			{
				uXCheckbox.Selected = false;
			}
			this.selectedMission = (CampaignMissionVO)checkbox.Tag;
			this.InitMissionDetailGroup();
		}

		private void OnAllChaptersButtonClicked(UXButton button)
		{
			this.screen.currentSection = CampaignScreenSection.AllChapters;
			this.campaignDetailsGroup.Visible = false;
			this.screen.GoToAllCampaigns();
			base.EvtManager.SendEvent(EventId.UIPvESelection, new ActionMessageBIData("chapter", string.Empty));
		}

		private void OnCampaignInfoButtonClicked(UXButton button)
		{
			this.campaignDescriptionGroup.Visible = !this.campaignDescriptionGroup.Visible;
		}

		private void OnCollectButtonClicked(UXButton button)
		{
			base.CampController.CollectMission(this.selectedMission);
			button.Visible = false;
		}

		private void OnRewardCollected(CampaignMissionVO rewardedMission)
		{
			if (this.selectedMission != null && rewardedMission == this.selectedMission)
			{
				this.rewardedLabel.Visible = true;
				this.UpdateMissionItem(this.selectedMission.Uid, this.selectedMission, true);
				this.InitMissionDetailGroup();
				this.InitMissionGrid();
			}
		}

		private void OnMissionCompleted(CampaignMissionVO completedMission)
		{
			this.UpdateMissionItem(this.selectedMission.Uid, this.selectedMission, true);
			this.InitMissionDetailGroup();
			this.InitMissionGrid();
		}

		private void OnCampaignItemButtonClicked(UXButton button)
		{
			this.SelectCampaign((CampaignVO)button.Tag);
		}

		public void SelectCampaign(CampaignVO campaignType)
		{
			Service.UXController.HUD.SetSquadScreenVisibility(false);
			this.screen.currentSection = CampaignScreenSection.Campaign;
			this.screen.CurrentBackDelegate = new UXButtonClickedDelegate(this.BackButtonClickedHelper);
			this.selectedCampaign = campaignType;
			this.planetDetailsTop.Visible = false;
			this.planetDetailsBottom.Visible = false;
			this.campaignDetailsGroup.Visible = true;
			this.eventInfoGroup.Visible = false;
			this.InitCampaignDetailScreen();
		}

		private void OnActionButtonClicked(UXButton button)
		{
			if (this.selectedMission.MissionType == MissionType.Attack)
			{
				BattleTypeVO battle = null;
				if (this.selectedMission.Map != null && !this.selectedMission.Grind)
				{
					battle = base.Sdc.Get<BattleTypeVO>(this.selectedMission.Map);
				}
				if (!GameUtils.HasAvailableTroops(true, battle))
				{
					AlertScreen.ShowModal(false, base.LangController.Get("NOT_ENOUGH_TROOPS_TITLE", new object[0]), base.LangController.Get("NOT_ENOUGH_TROOPS_FOR_ATTACK", new object[0]), null, null);
					base.EvtManager.SendEvent(EventId.UIPvESelection, new ActionMessageBIData("start", "no_troops"));
					return;
				}
			}
			button.Enabled = false;
			base.CampController.StartMission(this.selectedMission);
			base.EvtManager.SendEvent(EventId.MissionActionButtonClicked, this.selectedMission);
			base.EvtManager.SendEvent(EventId.UIPvESelection, new ActionMessageBIData("start", string.Empty));
			if (this.selectedMission.MissionType == MissionType.Own || this.selectedMission.MissionType == MissionType.Collect)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.selectedMission.BIChapterId);
				stringBuilder.Append("|");
				stringBuilder.Append(this.selectedMission.BIMissionId);
				stringBuilder.Append("|");
				stringBuilder.Append(this.selectedMission.BIMissionName);
				stringBuilder.Append("|");
				stringBuilder.Append(this.selectedMission.Uid);
				base.EvtManager.SendEvent(EventId.UIPvEMissionStart, stringBuilder.ToString());
			}
		}

		public void ReturnToMainSelect()
		{
			this.screen.currentSection = CampaignScreenSection.Main;
			this.chapterJewel.Visible = false;
			this.selectedCampaign = null;
			this.selectedMission = null;
			this.selectedMissionCheckbox = null;
			this.eventInfoGroup.Visible = true;
			this.closeButton.Visible = true;
			this.screen.GoToMainSelectScreen();
			this.screen.AnimateShowUI();
			this.screen.InitDefaultBackDelegate();
		}

		private void OnBackButtonClicked(UXButton button)
		{
			this.BackButtonClickedHelper(button);
		}

		public void BackButtonClickedHelper(UXButton button)
		{
			this.ReturnToMainSelect();
			this.screen.UpdateCurrentPlanet(this.screen.CurrentPlanet);
			base.EvtManager.SendEvent(EventId.BackButtonClicked, null);
		}

		private static int CompareMissions(CampaignMissionVO missionA, CampaignMissionVO missionB)
		{
			if (missionA.UnlockOrder < missionB.UnlockOrder)
			{
				return -1;
			}
			if (missionA.UnlockOrder > missionB.UnlockOrder)
			{
				return 1;
			}
			return string.Compare(missionA.Uid, missionB.Uid);
		}

		public void Destroy()
		{
			if (this.missionGrid != null)
			{
				this.missionGrid.Clear();
				this.missionGrid = null;
			}
			if (this.rewardsGrid != null)
			{
				this.rewardsGrid.Clear();
				this.rewardsGrid = null;
			}
			base.EvtManager.UnregisterObserver(this, EventId.MissionCollected);
			base.EvtManager.UnregisterObserver(this, EventId.MissionCompleted);
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}
	}
}
