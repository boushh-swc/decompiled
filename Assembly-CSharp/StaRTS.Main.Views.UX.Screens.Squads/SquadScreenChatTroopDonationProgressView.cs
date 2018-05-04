using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenChatTroopDonationProgressView : AbstractSquadScreenViewModule, IEventObserver, IViewClockTimeObserver
	{
		private const string DONATION_COMPLETE_GROUP = "GroupDailyRepComplete";

		private const string DONATION_COMPLETE_TITLE = "LabelDailyRepCompleteTitle";

		private const string DONATION_COUNTDOWN_TIMER_LABEL = "LabelDailyRepTimer";

		private const string DONATION_PROGRESS_GROUP = "GroupDailyRepProgress";

		private const string DONATION_PROGRESS_TITLE_LABEL = "LabelDailyRepProgressTitle";

		private const string DONATION_PROGRESS_INFO_LABEL = "LabelEarnDailyRep";

		private const string DONATION_PROGRESS_AMOUNT_LABEL = "LabelDailyRepProgress";

		private const string DONATION_PROGRESS_REWARD_AMOUNT_LABEL = "LabelRepAmount";

		private const string DONATION_PROGRESS_BAR = "PBarDonateRep";

		private const string TITLE_STRING = "PERK_CHAT_DONATE_TITLE";

		private const string TITLE_COMPLETE_STRING = "PERK_CHAT_DONATE_TITLE_COMPLETE";

		private const string DESC_STRING = "PERK_CHAT_DONATE_DESC";

		private const string COUNTDOWN_TIMER_STRING = "PERK_CHAT_DONATE_TIMER";

		private const string PERK_CHAT_DONATE_PROGRESS = "PERK_CHAT_DONATE_PROGRESS";

		private UXElement completeGroup;

		private UXElement progressGroup;

		private UXLabel progressAmountLabel;

		private UXLabel progressRewardAmountLabel;

		private UXLabel countdownTimerLabel;

		private UXSlider progressBar;

		public SquadScreenChatTroopDonationProgressView(SquadSlidingScreen screen) : base(screen)
		{
		}

		public override void OnScreenLoaded()
		{
			this.completeGroup = this.screen.GetElement<UXElement>("GroupDailyRepComplete");
			this.progressGroup = this.screen.GetElement<UXElement>("GroupDailyRepProgress");
			UXLabel element = this.screen.GetElement<UXLabel>("LabelDailyRepCompleteTitle");
			element.Text = this.lang.Get("PERK_CHAT_DONATE_TITLE_COMPLETE", new object[0]);
			this.countdownTimerLabel = this.screen.GetElement<UXLabel>("LabelDailyRepTimer");
			UXLabel element2 = this.screen.GetElement<UXLabel>("LabelDailyRepProgressTitle");
			element2.Text = this.lang.Get("PERK_CHAT_DONATE_TITLE", new object[0]);
			UXLabel element3 = this.screen.GetElement<UXLabel>("LabelEarnDailyRep");
			element3.Text = this.lang.Get("PERK_CHAT_DONATE_DESC", new object[]
			{
				GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD,
				GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD
			});
			this.progressAmountLabel = this.screen.GetElement<UXLabel>("LabelDailyRepProgress");
			this.progressRewardAmountLabel = this.screen.GetElement<UXLabel>("LabelRepAmount");
			this.progressRewardAmountLabel.Text = GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD.ToString();
			this.progressBar = this.screen.GetElement<UXSlider>("PBarDonateRep");
			this.RefreshTroopDonationProgress();
		}

		public override void ShowView()
		{
			this.RefreshView();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.TroopDonationTrackProgressUpdated);
			this.RefreshTroopDonationProgress();
		}

		public override void HideView()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.TroopDonationTrackProgressUpdated);
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
		}

		public override void OnDestroyElement()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.TroopDonationTrackProgressUpdated);
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.TroopDonationTrackProgressUpdated)
			{
				this.RefreshTroopDonationProgress();
			}
			return EatResponse.NotEaten;
		}

		public void OnViewClockTime(float dt)
		{
			TroopDonationProgress troopDonationProgress = Service.CurrentPlayer.TroopDonationProgress;
			uint donationCooldownEndTime = (uint)troopDonationProgress.DonationCooldownEndTime;
			if (donationCooldownEndTime <= 0u)
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				this.RefreshTroopDonationProgress();
				return;
			}
			this.UpdateCountDownTimer();
		}

		private void RefreshTroopDonationProgress()
		{
			TroopDonationTrackController troopDonationTrackController = Service.TroopDonationTrackController;
			bool flag = troopDonationTrackController.IsTroopDonationProgressComplete();
			this.completeGroup.Visible = flag;
			this.progressGroup.Visible = !flag;
			if (flag)
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				this.UpdateCountDownTimer();
			}
			else
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				this.UpdateInProgress();
			}
		}

		private void UpdateInProgress()
		{
			TroopDonationTrackController troopDonationTrackController = Service.TroopDonationTrackController;
			int num = troopDonationTrackController.GetTroopDonationProgressAmount();
			int sQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD = GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD;
			if (num > sQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD)
			{
				num = sQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD;
			}
			this.progressAmountLabel.Text = this.lang.Get("PERK_CHAT_DONATE_PROGRESS", new object[]
			{
				num,
				sQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD
			});
			this.progressBar.Value = (float)num / (float)sQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD;
		}

		private void UpdateCountDownTimer()
		{
			TroopDonationTrackController troopDonationTrackController = Service.TroopDonationTrackController;
			int timeRemainingUntilNextProgressTrack = troopDonationTrackController.GetTimeRemainingUntilNextProgressTrack();
			if (timeRemainingUntilNextProgressTrack <= 0)
			{
				this.RefreshTroopDonationProgress();
				return;
			}
			string text = LangUtils.FormatTime((long)timeRemainingUntilNextProgressTrack);
			this.countdownTimerLabel.Text = this.lang.Get("PERK_CHAT_DONATE_TIMER", new object[]
			{
				text
			});
		}
	}
}
