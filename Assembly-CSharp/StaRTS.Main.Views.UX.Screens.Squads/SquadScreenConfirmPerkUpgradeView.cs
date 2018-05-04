using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenConfirmPerkUpgradeView
	{
		private const int INVEST_AMT_PER_PRESS = 1;

		private const string PERK_UPGRADE_POPUP_BACK = "PERK_UPGRADE_POPUP_BACK";

		private const string PERK_UPGRADE_POPUP_TITLE = "PERK_UPGRADE_POPUP_TITLE";

		private const string PERK_UPGRADE_POPUP_LVL_REQ_SQUAD = "PERK_UPGRADE_POPUP_LVL_REQ";

		private const string PERK_UPGRADE_POPUP_LVL_REQ = "PERK_UPGRADE_POPUP_LVL_REQ2";

		private const string PERK_UPGRADE_POPUP_CTA = "PERK_UPGRADE_POPUP_CTA";

		private const string PERK_UPGRADE_POPUP_CTA_DESC = "PERK_UPGRADE_POPUP_CTA_DESC";

		private const string PERK_UPGRADE_POPUP_QUESTION = "PERK_UPGRADE_POPUP_DESC";

		private const string PERK_UPGRADE_POPUP_DESC_UNLOCK = "PERK_UPGRADE_POPUP_DESC_UNLOCK";

		private const string PERK_UPGRADE_POPUP_REQUIRED = "PERK_UPGRADE_LVL_REQ";

		private const string PERK_UPGRADE_POPUP_PROGRESS = "PERK_UPGRADE_POPUP_PROGRESS";

		private const string PERK_UPGRADE_POPUP_MAX_TIER = "PERK_UPGRADE_MAX_TIER";

		private const string PERK_UPGRADE_INVEST_NOT_ENOUGH_REPUTATION = "PERK_UPGRADE_INVEST_NOT_ENOUGH_REPUTATION";

		private const string LANG_PERK_UPGRADE_LVL_REQ = "PERK_UPGRADE_LVL_REQ";

		private const string PERK_UPGRADE_CONFIRM_BACK_BTN = "BtnBack";

		private const string PERK_UPGRADE_CONFIRM_BACK_LABEL = "LabelBack";

		private const string PERK_UPGRADE_CONFIRM_PRIMARY_TITLE_LABEL = "LabelTitlePrimaryUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_DESC_LABEL = "LabelModalUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_SECONDARY_TITLE_LABEL = "LabelTitleSecondaryUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_STAT_GRID = "GridUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_STAT_GRID_TEMPLATE = "TemplateUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_STAT_INFO_LABEL = "LabelUpSelectedStatsInfoPerks";

		private const string PERK_UPGRADE_CONFIRM_STAT_VALUE_LABEL = "LabelUpSelectedStatsValuePerks";

		private const string PERK_UPGRADE_CONFIRM_LOCKED_GROUP = "GroupInvestmentLocked";

		private const string PERK_UPGRADE_CONFIRM_LOCKED_LEVEL_NUM_LABEL = "LabelSquadLvlUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_LOCKED_LEVEL_REQ_LABEL = "LabelSecondaryUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_LOCKED_LEVEL_SQUAD_LABEL = "LabelPrimaryUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_PERK_IMAGE = "TexturePerkArtUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_SQUAD_PROG_VALUE_LABEL = "LabelSquadProgressValueUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_SQUAD_PROGRESS_SLIDER = "pBarSquadInvestAmtUpSelectedPerks";

		private const string PERK_UPGRADE_CONFIRM_INVEST_GROUP = "GroupSetInvestment";

		private const string PERK_UPGRADE_CONFIRM_REDUCE_AMT_BTN = "BtnScrollBack";

		private const string PERK_UPGRADE_CONFIRM_INC_AMT_BTN = "BtnScrollForward";

		private const string PERK_UPGRADE_CONFIRM_INVEST_BTN = "BtnInvestPerks";

		private const string PERK_UPGRADE_CONFIRM_INVEST_LABEL = "LabelBtnInvestPerks";

		private const string PERK_UPGRADE_CONFIRM_AMT_LABEL = "LabelAmount";

		private const string PERK_UPGRADE_CONFIRM_SET_INVEST_LABEL = "LabelSetInvestment";

		private const string PERK_UPGRADE_CONFIRM_MAX_TIER = "LabelMaxLockedUpSelectedPerks";

		private const string PERK_TITLE_GROUP = "TitleGroupPerks";

		private const string PERK_TABS_GROUP = "TabGroupPerks";

		public static readonly string PERK_INVEST_CONFIRM_VIEW = "UpgradeSelectedGroupPerks";

		private UXButton backBtn;

		private UXLabel backLabel;

		private UXLabel titleLabel;

		private UXLabel descLabel;

		private UXLabel subTitleLabel;

		private UXGrid statGrid;

		private UXElement lockedGroup;

		private UXLabel lockedLevelLabel;

		private UXLabel lockedReqLabel;

		private UXLabel lockedSquadLabel;

		private UXTexture perkImage;

		private UXLabel progressValueLabel;

		private UXSlider progressSlider;

		private UXElement rootConfirmView;

		private UXElement investGroup;

		private UXButton reduceInvestAmtBtn;

		private UXButton incInvestAmtBtn;

		private UXButton investBtn;

		private UXLabel investBtnLabel;

		private UXLabel investAmtLabel;

		private UXLabel setInvestLabel;

		private UXLabel maxTierLabel;

		private UXElement titleGroup;

		private UXElement tabsGroup;

		private PerkVO perkToInvestIn;

		private SquadSlidingScreen squadScreen;

		private PerkUpgradeConfirmState viewState;

		private Action onCloseCB;

		private int perkTotalCost;

		private int perkRemainingCost;

		private int internalInvestCount;

		private int nextInvestAmt;

		public SquadScreenConfirmPerkUpgradeView(PerkVO perkData, SquadSlidingScreen screen, PerkUpgradeConfirmState state, Action onCloseCB)
		{
			this.perkToInvestIn = perkData;
			this.squadScreen = screen;
			this.viewState = state;
			this.onCloseCB = onCloseCB;
			this.InitUI();
		}

		private void InitUI()
		{
			Lang lang = Service.Lang;
			PerkViewController perkViewController = Service.PerkViewController;
			this.perkTotalCost = this.perkToInvestIn.ReputationCost;
			int squadLevelUnlock = this.perkToInvestIn.SquadLevelUnlock;
			this.rootConfirmView = this.squadScreen.GetElement<UXElement>(SquadScreenConfirmPerkUpgradeView.PERK_INVEST_CONFIRM_VIEW);
			this.backBtn = this.squadScreen.GetElement<UXButton>("BtnBack");
			this.backLabel = this.squadScreen.GetElement<UXLabel>("LabelBack");
			this.titleLabel = this.squadScreen.GetElement<UXLabel>("LabelTitlePrimaryUpSelectedPerks");
			this.descLabel = this.squadScreen.GetElement<UXLabel>("LabelModalUpSelectedPerks");
			this.subTitleLabel = this.squadScreen.GetElement<UXLabel>("LabelTitleSecondaryUpSelectedPerks");
			this.statGrid = this.squadScreen.GetElement<UXGrid>("GridUpSelectedPerks");
			this.lockedGroup = this.squadScreen.GetElement<UXElement>("GroupInvestmentLocked");
			this.lockedSquadLabel = this.squadScreen.GetElement<UXLabel>("LabelPrimaryUpSelectedPerks");
			this.lockedReqLabel = this.squadScreen.GetElement<UXLabel>("LabelSecondaryUpSelectedPerks");
			this.lockedLevelLabel = this.squadScreen.GetElement<UXLabel>("LabelSquadLvlUpSelectedPerks");
			this.perkImage = this.squadScreen.GetElement<UXTexture>("TexturePerkArtUpSelectedPerks");
			this.progressValueLabel = this.squadScreen.GetElement<UXLabel>("LabelSquadProgressValueUpSelectedPerks");
			this.progressSlider = this.squadScreen.GetElement<UXSlider>("pBarSquadInvestAmtUpSelectedPerks");
			this.investGroup = this.squadScreen.GetElement<UXElement>("GroupSetInvestment");
			this.reduceInvestAmtBtn = this.squadScreen.GetElement<UXButton>("BtnScrollBack");
			this.incInvestAmtBtn = this.squadScreen.GetElement<UXButton>("BtnScrollForward");
			this.investBtn = this.squadScreen.GetElement<UXButton>("BtnInvestPerks");
			this.investBtnLabel = this.squadScreen.GetElement<UXLabel>("LabelBtnInvestPerks");
			this.investAmtLabel = this.squadScreen.GetElement<UXLabel>("LabelAmount");
			this.setInvestLabel = this.squadScreen.GetElement<UXLabel>("LabelSetInvestment");
			this.maxTierLabel = this.squadScreen.GetElement<UXLabel>("LabelMaxLockedUpSelectedPerks");
			this.reduceInvestAmtBtn.OnClicked = new UXButtonClickedDelegate(this.OnReduceInvestAmtClicked);
			this.incInvestAmtBtn.OnClicked = new UXButtonClickedDelegate(this.OnIncInvestAmtClicked);
			this.investBtn.OnClicked = new UXButtonClickedDelegate(this.OnInvestClicked);
			this.investBtnLabel.Text = lang.Get("PERK_UPGRADE_POPUP_CTA", new object[0]);
			this.setInvestLabel.Text = lang.Get("PERK_UPGRADE_POPUP_CTA_DESC", new object[0]);
			this.titleGroup = this.squadScreen.GetElement<UXElement>("TitleGroupPerks");
			this.tabsGroup = this.squadScreen.GetElement<UXElement>("TabGroupPerks");
			this.backBtn.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.backLabel.Text = lang.Get("PERK_UPGRADE_POPUP_BACK", new object[0]);
			this.titleLabel.Text = lang.Get("PERK_UPGRADE_POPUP_TITLE", new object[]
			{
				perkViewController.GetPerkNameForGroup(this.perkToInvestIn.PerkGroup),
				this.perkToInvestIn.PerkTier
			});
			this.descLabel.Text = perkViewController.GetPerkDescForGroup(this.perkToInvestIn.PerkGroup);
			string text = string.Empty;
			bool useUpgrade = true;
			if (this.perkToInvestIn.PerkTier > 1)
			{
				text = lang.Get("PERK_UPGRADE_POPUP_DESC", new object[]
				{
					this.perkToInvestIn.PerkTier
				});
			}
			else
			{
				useUpgrade = false;
				text = lang.Get("PERK_UPGRADE_POPUP_DESC_UNLOCK", new object[0]);
			}
			this.subTitleLabel.Text = text;
			perkViewController.SetupStatGridForPerk(this.perkToInvestIn, this.statGrid, "TemplateUpSelectedPerks", "LabelUpSelectedStatsInfoPerks", "LabelUpSelectedStatsValuePerks", useUpgrade);
			perkViewController.SetPerkImage(this.perkImage, this.perkToInvestIn);
			this.subTitleLabel.Visible = true;
			if (this.viewState == PerkUpgradeConfirmState.Unlocked)
			{
				this.maxTierLabel.Visible = false;
				this.lockedGroup.Visible = false;
				this.progressSlider.Visible = true;
				this.progressValueLabel.Visible = true;
				this.investGroup.Visible = true;
				this.RefreshProgress();
			}
			else
			{
				this.maxTierLabel.Visible = false;
				this.investGroup.Visible = false;
				this.lockedGroup.Visible = true;
				this.progressSlider.Visible = false;
				this.progressValueLabel.Visible = false;
				if (this.viewState == PerkUpgradeConfirmState.Locked)
				{
					this.lockedSquadLabel.Text = lang.Get("PERK_UPGRADE_POPUP_LVL_REQ", new object[0]);
					this.lockedReqLabel.Text = lang.Get("PERK_UPGRADE_LVL_REQ", new object[0]);
					this.lockedLevelLabel.Text = squadLevelUnlock.ToString();
				}
				else
				{
					this.lockedGroup.Visible = false;
					this.maxTierLabel.Visible = true;
					this.subTitleLabel.Visible = false;
					this.maxTierLabel.Text = lang.Get("PERK_UPGRADE_MAX_TIER", new object[0]);
				}
			}
		}

		private void SetupDefaultInvestAmt()
		{
			int currentReputation = this.GetCurrentReputation();
			int num = Math.Min(this.perkRemainingCost, currentReputation);
			this.nextInvestAmt = num;
			this.UpdateInvestUIBasedOnNewAmt();
		}

		private int GetCurrentReputation()
		{
			Inventory inventory = Service.CurrentPlayer.Inventory;
			return inventory.GetItemAmount("reputation");
		}

		private void OnReduceInvestAmtClicked(UXButton btn)
		{
			this.nextInvestAmt--;
			if (this.nextInvestAmt < 0)
			{
				this.nextInvestAmt = 0;
			}
			this.UpdateInvestUIBasedOnNewAmt();
		}

		private void OnIncInvestAmtClicked(UXButton btn)
		{
			this.nextInvestAmt++;
			if (this.nextInvestAmt > this.perkRemainingCost)
			{
				this.nextInvestAmt = this.perkRemainingCost;
			}
			if (this.nextInvestAmt > this.GetCurrentReputation())
			{
				this.nextInvestAmt = this.GetCurrentReputation();
			}
			this.UpdateInvestUIBasedOnNewAmt();
		}

		private void UpdateInvestUIBasedOnNewAmt()
		{
			this.investAmtLabel.Text = this.nextInvestAmt.ToString();
			this.investBtn.Enabled = true;
			this.investBtn.VisuallyEnableButton();
			this.incInvestAmtBtn.Enabled = true;
			this.reduceInvestAmtBtn.Enabled = true;
			if (this.nextInvestAmt <= 0)
			{
				this.reduceInvestAmtBtn.Enabled = false;
				this.investBtn.VisuallyDisableButton();
			}
			if (this.nextInvestAmt >= this.perkRemainingCost || this.nextInvestAmt >= this.GetCurrentReputation())
			{
				this.incInvestAmtBtn.Enabled = false;
			}
		}

		private void UpdateRemainingPerkCost()
		{
			this.perkRemainingCost = this.perkTotalCost - this.internalInvestCount;
		}

		private void OnInvestClicked(UXButton btn)
		{
			Lang lang = Service.Lang;
			if (this.nextInvestAmt <= 0)
			{
				if (this.GetCurrentReputation() < 1)
				{
					string instructions = lang.Get("PERK_UPGRADE_INVEST_NOT_ENOUGH_REPUTATION", new object[0]);
					Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(instructions);
				}
				return;
			}
			this.internalInvestCount += this.nextInvestAmt;
			this.UpdateRemainingPerkCost();
			Squad squad = Service.CurrentPlayer.Squad;
			if (squad == null)
			{
				return;
			}
			SquadPerks perks = squad.Perks;
			string squadLevelUIDFromSquad = GameUtils.GetSquadLevelUIDFromSquad(squad);
			Service.PerkManager.InvestInPerk(this.nextInvestAmt, this.perkToInvestIn.Uid, perks.Available, squadLevelUIDFromSquad);
			Service.EventManager.SendEvent(EventId.PerkInvested, null);
			this.nextInvestAmt = 0;
			this.UpdateInvestUIBasedOnNewAmt();
		}

		private void OnBackButtonClicked(UXButton btn)
		{
			this.HideAndCleanUp();
			if (this.onCloseCB != null)
			{
				this.onCloseCB();
			}
			Service.EventManager.SendEvent(EventId.BackButtonClicked, null);
		}

		private void UpdateInternalInvestCountFromServerData()
		{
			Squad squad = Service.CurrentPlayer.Squad;
			if (squad == null)
			{
				return;
			}
			Dictionary<string, int> inProgress = squad.Perks.InProgress;
			int perkInvestedProgress = Service.PerkManager.GetPerkInvestedProgress(this.perkToInvestIn, inProgress);
			if (perkInvestedProgress > this.internalInvestCount)
			{
				this.internalInvestCount = perkInvestedProgress;
			}
			this.UpdateRemainingPerkCost();
		}

		public void RefreshProgressFromServer()
		{
			this.UpdateInternalInvestCountFromServerData();
			this.RefreshProgress();
		}

		public void RefreshProgress()
		{
			string str = this.perkTotalCost.ToString();
			string str2 = this.internalInvestCount.ToString();
			this.progressValueLabel.Text = str2 + "/" + str;
			this.progressSlider.Value = (float)this.internalInvestCount / (float)this.perkTotalCost;
		}

		public void Show()
		{
			this.UpdateInternalInvestCountFromServerData();
			this.SetupDefaultInvestAmt();
			this.squadScreen.CurrentBackButton = this.backBtn;
			this.squadScreen.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.rootConfirmView.Visible = true;
			this.titleGroup.Visible = false;
			this.tabsGroup.Visible = false;
			this.RefreshProgress();
		}

		public void HideAndCleanUp()
		{
			this.squadScreen.SetDefaultBackDelegate();
			this.statGrid.Clear();
			this.rootConfirmView.Visible = false;
			this.titleGroup.Visible = true;
			this.tabsGroup.Visible = true;
		}

		public bool IsVisible()
		{
			return this.rootConfirmView.Visible;
		}

		public PerkVO GetPerkDataShown()
		{
			return this.perkToInvestIn;
		}
	}
}
