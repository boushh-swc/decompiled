using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class RaidInfoScreen : ClosableScreen, IViewClockTimeObserver
	{
		private const string LABEL_RAID_INFO_TITLE = "LabelDialogTitleNR";

		private const string LABEL_MESSAGE = "LabelTopMessageNR";

		private const string SPRITE_MESSAGE_STROKE = "SpriteBGMessageStrokeNR";

		private const string LABEL_REWARDS_TITLE = "LabelRewardsTitleNR";

		private const string BTN_LEFT_CONTAINER = "CrateContainerLeftNR";

		private const string BTN_MID_CONTAINER = "CrateContainerMiddleNR";

		private const string BTN_RIGHT_CONTAINER = "CrateContainerRightNR";

		private const string SPRITE_CRATE_LEFT = "SpriteCrateLeftNR";

		private const string SPRITE_CRATE_MID = "SpriteCrateMiddleNR";

		private const string SPRITE_CRATE_RIGHT = "SpriteCrateRightNR";

		private const string LABEL_ONE_STAR = "LabelRequirementsOneStarNR";

		private const string LABEL_TWO_STAR = "LabelRequirementsTwoStarNR";

		private const string LABEL_THREE_STAR = "LabelRequirementsThreeStarNR";

		private const string TEXTURE_REWARD_BG = "TextureBGRewardsNR";

		private const string LABEL_RAID_TIMER = "TickerNR";

		private const string BTN_OK = "ButtonOneOptionNR";

		private const string LABEL_OK = "LabelOneOptionNR";

		private const string BTN_CANCEL = "ButtonTwoOptionLeftNR";

		private const string LABEL_CANCEL = "LabelTwoOptionLeftNR";

		private const string BTN_CONFIRM = "ButtonTwoOptionRightNR";

		private const string LABEL_CONFIRM = "LabelTwoOptionRightNR";

		private const string RAID_TIME_REMAINING_ACTIVE = "RAID_TIME_REMAINING_ACTIVE";

		private const string RAID_TIME_REMAINING_INACTIVE = "RAID_TIME_REMAINING_INACTIVE";

		private const string REWARDS_TITLE = "s_Rewards";

		private const string RAID_TITLE_ACTIVE = "RAID_TITLE_ACTIVE";

		private const string RAID_TITLE_INACTIVE = "RAID_TITLE_INACTIVE";

		private const string OK = "s_Ok";

		private const string RAID_NOTIFY_ME = "RAID_NOTIFY_ME";

		private const string RAID_WAIT = "RAID_WAIT";

		private const string RAID_START = "RAID_START";

		private UXLabel title;

		private UXLabel message;

		private UXSprite messageBorderStroke;

		private UXLabel rewardSectionTitle;

		private UXButton leftContainerBtn;

		private UXButton midContainerBtn;

		private UXButton rightContainerBtn;

		private UXSprite leftCrateSprite;

		private UXSprite midCrateSprite;

		private UXSprite rightCrateSprite;

		private UXLabel oneStarLabel;

		private UXLabel twoStarLabel;

		private UXLabel threeStarLabel;

		private UXTexture rewardBGTexture;

		private UXLabel raidTimer;

		private UXButton okBtn;

		private UXLabel okLabel;

		private UXButton confirmBtn;

		private UXLabel confirmLabel;

		private UXButton cancelBtn;

		private UXLabel cancelLabel;

		private bool raidAvailable;

		private RaidMissionPoolVO raidPool;

		private CampaignMissionVO raidMission;

		private string raidTimerText;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public RaidInfoScreen() : base("gui_nightraids")
		{
			base.OnModalResult = new OnScreenModalResult(this.OnRaidInfoScreenClosed);
		}

		protected override void OnScreenLoaded()
		{
			this.title = base.GetElement<UXLabel>("LabelDialogTitleNR");
			this.message = base.GetElement<UXLabel>("LabelTopMessageNR");
			this.messageBorderStroke = base.GetElement<UXSprite>("SpriteBGMessageStrokeNR");
			this.rewardSectionTitle = base.GetElement<UXLabel>("LabelRewardsTitleNR");
			this.leftContainerBtn = base.GetElement<UXButton>("CrateContainerLeftNR");
			this.midContainerBtn = base.GetElement<UXButton>("CrateContainerMiddleNR");
			this.rightContainerBtn = base.GetElement<UXButton>("CrateContainerRightNR");
			this.leftCrateSprite = base.GetElement<UXSprite>("SpriteCrateLeftNR");
			this.midCrateSprite = base.GetElement<UXSprite>("SpriteCrateMiddleNR");
			this.rightCrateSprite = base.GetElement<UXSprite>("SpriteCrateRightNR");
			this.oneStarLabel = base.GetElement<UXLabel>("LabelRequirementsOneStarNR");
			this.twoStarLabel = base.GetElement<UXLabel>("LabelRequirementsTwoStarNR");
			this.threeStarLabel = base.GetElement<UXLabel>("LabelRequirementsThreeStarNR");
			this.rewardBGTexture = base.GetElement<UXTexture>("TextureBGRewardsNR");
			this.raidTimer = base.GetElement<UXLabel>("TickerNR");
			this.okBtn = base.GetElement<UXButton>("ButtonOneOptionNR");
			this.okLabel = base.GetElement<UXLabel>("LabelOneOptionNR");
			this.confirmBtn = base.GetElement<UXButton>("ButtonTwoOptionRightNR");
			this.confirmLabel = base.GetElement<UXLabel>("LabelTwoOptionRightNR");
			this.cancelBtn = base.GetElement<UXButton>("ButtonTwoOptionLeftNR");
			this.cancelLabel = base.GetElement<UXLabel>("LabelTwoOptionLeftNR");
			this.okLabel.Text = this.lang.Get("s_Ok", new object[0]);
			this.okBtn.OnClicked = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
			this.leftContainerBtn.OnClicked = new UXButtonClickedDelegate(this.OnCrateClicked);
			this.midContainerBtn.OnClicked = new UXButtonClickedDelegate(this.OnCrateClicked);
			this.rightContainerBtn.OnClicked = new UXButtonClickedDelegate(this.OnCrateClicked);
			this.InitButtons();
			this.RefreshRaidView();
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		private void SetupRaidData()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.raidPool = Service.StaticDataController.Get<RaidMissionPoolVO>(currentPlayer.CurrentRaidPoolId);
			this.raidMission = currentPlayer.CurrentRaid;
			this.raidAvailable = Service.RaidDefenseController.IsRaidAvailable();
		}

		private void SetupRaidRewardsBG()
		{
			string raidBriefingBGTextureName = this.raidMission.RaidBriefingBGTextureName;
			if (!string.IsNullOrEmpty(raidBriefingBGTextureName))
			{
				this.rewardBGTexture.LoadTexture(raidBriefingBGTextureName);
			}
		}

		private void SetupRaidRewards()
		{
			this.oneStarLabel.Text = this.lang.Get(this.raidPool.Condition1StarRewardStringId, new object[0]);
			this.twoStarLabel.Text = this.lang.Get(this.raidPool.Condition2StarRewardStringId, new object[0]);
			this.threeStarLabel.Text = this.lang.Get(this.raidPool.Condition3StarRewardStringId, new object[0]);
			CrateVO crateVO = Service.StaticDataController.Get<CrateVO>(this.raidPool.Crate1StarRewardId);
			CrateVO crateVO2 = Service.StaticDataController.Get<CrateVO>(this.raidPool.Crate2StarRewardId);
			CrateVO crateVO3 = Service.StaticDataController.Get<CrateVO>(this.raidPool.Crate3StarRewardId);
			this.InitCrateProjector(this.leftCrateSprite, crateVO);
			this.InitCrateProjector(this.midCrateSprite, crateVO2);
			this.InitCrateProjector(this.rightCrateSprite, crateVO3);
			this.leftContainerBtn.Tag = crateVO.Uid;
			this.midContainerBtn.Tag = crateVO2.Uid;
			this.rightContainerBtn.Tag = crateVO3.Uid;
		}

		private void InitCrateProjector(UXSprite sprite, CrateVO crateVO)
		{
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(crateVO, sprite);
			projectorConfig.AnimState = AnimState.Closed;
			ProjectorUtils.GenerateProjector(projectorConfig);
		}

		private void SetupRaidInfoText()
		{
			string id;
			if (this.raidAvailable)
			{
				id = "RAID_TITLE_ACTIVE";
				this.raidTimerText = "RAID_TIME_REMAINING_ACTIVE";
			}
			else
			{
				id = "RAID_TITLE_INACTIVE";
				this.raidTimerText = "RAID_TIME_REMAINING_INACTIVE";
			}
			this.UpdateRaidTimer();
			this.rewardSectionTitle.Text = this.lang.Get("s_Rewards", new object[0]);
			this.title.Text = this.lang.Get(id, new object[0]);
			this.message.Text = this.lang.Get(this.raidMission.RaidDescriptionID, new object[0]);
		}

		private void UpdateRaidTimer()
		{
			RaidDefenseController raidDefenseController = Service.RaidDefenseController;
			this.raidTimer.Text = this.lang.Get(this.raidTimerText, new object[]
			{
				LangUtils.FormatTime((long)raidDefenseController.GetRaidTimeSeconds())
			});
		}

		private void SetupRaidStateColors()
		{
			RaidDefenseController raidDefenseController = Service.RaidDefenseController;
			Color textColor;
			Color color;
			if (this.raidAvailable)
			{
				textColor = raidDefenseController.ActiveRaidColor;
				color = raidDefenseController.ActiveRaidColor;
			}
			else
			{
				textColor = Color.white;
				color = raidDefenseController.InactiveColor;
			}
			this.title.TextColor = textColor;
			this.raidTimer.TextColor = color;
			this.messageBorderStroke.Color = color;
		}

		private void HideActionButtons()
		{
			this.okBtn.Visible = false;
			this.cancelBtn.Visible = false;
			this.confirmBtn.Visible = false;
		}

		private void SetupRaidButtons()
		{
			this.HideActionButtons();
			if (!this.raidAvailable)
			{
				if (!Service.NotificationController.HasAgreedToNotifications())
				{
					this.cancelBtn.Visible = true;
					this.cancelLabel.Text = this.lang.Get("s_Ok", new object[0]);
					this.cancelBtn.OnClicked = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
					this.confirmBtn.Visible = true;
					this.confirmLabel.Text = this.lang.Get("RAID_NOTIFY_ME", new object[0]);
					this.confirmBtn.OnClicked = new UXButtonClickedDelegate(this.OnNotifyClicked);
				}
				else
				{
					this.okBtn.Visible = true;
				}
			}
			else
			{
				this.cancelBtn.Visible = true;
				this.cancelLabel.Text = this.lang.Get("RAID_WAIT", new object[0]);
				this.cancelBtn.OnClicked = new UXButtonClickedDelegate(this.OnWaitButtonClicked);
				this.confirmBtn.Visible = true;
				this.confirmLabel.Text = this.lang.Get("RAID_START", new object[0]);
				this.confirmBtn.OnClicked = new UXButtonClickedDelegate(this.OnDefendButtonClicked);
			}
		}

		public void OnViewClockTime(float dt)
		{
			if (this.raidAvailable != Service.RaidDefenseController.IsRaidAvailable())
			{
				this.RefreshRaidView();
			}
			this.UpdateRaidTimer();
		}

		public void RefreshRaidView()
		{
			if (base.IsLoaded())
			{
				this.SetupRaidData();
				this.SetupRaidRewardsBG();
				this.SetupRaidInfoText();
				this.SetupRaidStateColors();
				this.SetupRaidButtons();
				this.SetupRaidRewards();
			}
		}

		private void OnCrateClicked(UXButton crateButton)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string crateUid = crateButton.Tag as string;
			string planetId = currentPlayer.PlanetId;
			CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForInfo(crateUid, planetId);
			crateInfoModalScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(crateInfoModalScreen, true, false);
		}

		private void OnNotifyClicked(UXButton notifyButton)
		{
			Service.EventManager.SendEvent(EventId.RaidNotifyRequest, null);
			this.Close(null);
		}

		private void OnWaitButtonClicked(UXButton notifyButton)
		{
			this.Close(null);
		}

		private void OnDefendButtonClicked(UXButton notifyButton)
		{
			Service.BILoggingController.TrackGameAction("UI_raid", "start", "briefing", string.Empty, 1);
			RaidDefenseController raidDefenseController = Service.RaidDefenseController;
			this.Close(true);
			raidDefenseController.StartCurrentRaidDefense();
		}

		private void OnRaidInfoScreenClosed(object result, object cookie)
		{
			if (result == null)
			{
				RaidDefenseController raidDefenseController = Service.RaidDefenseController;
				raidDefenseController.AttemptToShowRaidWaitConfirmation();
				Service.BILoggingController.TrackGameAction("UI_raid_briefing", "close", string.Empty, string.Empty, 1);
			}
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			base.OnDestroyElement();
		}
	}
}
