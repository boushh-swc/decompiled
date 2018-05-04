using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenBasePerkInfoView
	{
		protected const string BUTTON_CLOSE = "BtnClose";

		protected const string LABEL_MODAL_TITLE = "LabelModalTitlePerks";

		protected const string LABEL_PERK_DESCRIPTION = "LabelModalStoryPerks";

		protected const string LABEL_ACTIVATION_TICKER = "LabelTickerModalPerks";

		protected const string PERK_ACTIVATE_CONFIRM_PERK_IMAGE = "TexturePerkArtModalCardPerks";

		protected const string PERK_MODAL_LEVEL_LABEL = "LabelPerkLvlModalPerks";

		protected const string PERK_MODAL_LVL_LOCKED_GROUP = "GroupLvlLockedModalPerks";

		protected const string PERK_MODAL_REP_LOCKED_GROUP = "GroupRepLockedModalPerks";

		protected const string PERK_MODAL_REP_LOCKED_LABEL = "LabelPrimaryRepLockedModalPerks";

		protected const string PERK_MODAL_LVL_LOCKED_SQUAD_LABEL = "LabelPrimaryLvlLockedModalPerks";

		protected const string PERK_MODAL_LVL_LOCKED_LEVEL_LABEL = "LabelSquadLvlLockedModalPerks";

		protected const string PERK_MODAL_LVL_LOCKED_REQ_LABEL = "LabelSecondaryLvlLockedModalPerks";

		protected const string GRID_PERK_ACTIVATION_STAT = "GridModalStatsPerks";

		protected const string GRID_TEMPLATE_PERK_ACTIVATION_STAT = "TemplateModalStatsPerks";

		protected const string LABEL_PERK_ACTIVATION_STAT_INFO = "LabelModalStatsInfoPerks";

		protected const string LABEL_PERK_ACTIVATION_STAT_VALUE = "LabelModalStatsValuePerks";

		protected const string BUTTON_ONE_CURRENCY = "BtnModalOneCurrencyPerks";

		protected const string GROUP_COST_ONE_CURRENCY = "CostModalOnePerks";

		protected const string BUTTON_TWO_CURRENCY = "BtnModalTwoCurrencyPerks";

		protected const string GROUP_COST_TWO_CURRENCY_TOP = "CostModalTwoTopPerks";

		protected const string GROUP_COST_TWO_CURRENCY_BOT = "CostModalTwoBotPerks";

		protected const string BUTTON_CONTINUE_UPGRADE_CELEB = "BtnContinuePerkUpgradeCeleb";

		protected const string LABEL_CONTINUE_UPGRADE_CELEB = "LabelContinuePerkUpgradeCeleb";

		protected const string HIDE = "Hide";

		public static readonly string PERK_ACTIVATION_INFO_VIEW = "PanelContainerModalPerks";

		protected SquadSlidingScreen squadScreen;

		protected PerkVO targetPerkVO;

		protected UXElement rootInfoView;

		protected UXElement levelLockedGroup;

		protected UXElement repLockedGroup;

		protected UXButton closeBtn;

		protected UXGrid statGrid;

		public SquadScreenBasePerkInfoView(SquadSlidingScreen screen, PerkVO targetPerkVO)
		{
			this.squadScreen = screen;
			this.targetPerkVO = targetPerkVO;
		}

		protected virtual void InitUI()
		{
			this.levelLockedGroup = this.squadScreen.GetElement<UXElement>("GroupLvlLockedModalPerks");
			this.repLockedGroup = this.squadScreen.GetElement<UXElement>("GroupRepLockedModalPerks");
			this.levelLockedGroup.Visible = false;
			this.repLockedGroup.Visible = false;
			UXLabel element = this.squadScreen.GetElement<UXLabel>("LabelPerkLvlModalPerks");
			element.Text = StringUtils.GetRomanNumeral(this.targetPerkVO.PerkTier);
			this.rootInfoView = this.squadScreen.GetElement<UXElement>(SquadScreenBasePerkInfoView.PERK_ACTIVATION_INFO_VIEW);
			this.closeBtn = this.squadScreen.GetElement<UXButton>("BtnClose");
			this.closeBtn.OnClicked = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
			this.statGrid = this.squadScreen.GetElement<UXGrid>("GridModalStatsPerks");
		}

		public virtual void HideAndCleanUp()
		{
			this.squadScreen.SetDefaultBackDelegate();
			this.statGrid.Clear();
			this.rootInfoView.SetTrigger("Hide");
			Service.EventManager.SendEvent(EventId.ScreenOverlayClosing, base.GetType().Name);
		}

		protected void OnCloseButtonClicked(UXButton btn)
		{
			this.HideAndCleanUp();
		}
	}
}
