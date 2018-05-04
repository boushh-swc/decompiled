using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenUpgradeCelebPerkInfoView : SquadScreenBasePerkInfoView
	{
		private const string SHOW_CELEB = "ShowCeleb";

		private const string PERK_UPGRADE_CELEBRATION_TITLE = "PERK_UPGRADE_CELEBRATION_TITLE";

		private const string PERK_UPGRADE_UNLOCK_CELEBRATION_TITLE = "PERK_UPGRADE_UNLOCK_CELEBRATION_TITLE";

		private const string CONTINUE = "CONTINUE";

		private UXButton continueBtn;

		private UXLabel modalTitle;

		public SquadScreenUpgradeCelebPerkInfoView(SquadSlidingScreen screen, PerkVO targetPerkVO) : base(screen, targetPerkVO)
		{
			this.InitUI();
		}

		protected override void InitUI()
		{
			base.InitUI();
			PerkViewController perkViewController = Service.PerkViewController;
			Lang lang = Service.Lang;
			UXTexture element = this.squadScreen.GetElement<UXTexture>("TexturePerkArtModalCardPerks");
			perkViewController.SetPerkImage(element, this.targetPerkVO);
			UXLabel element2 = this.squadScreen.GetElement<UXLabel>("LabelModalTitlePerks");
			UXLabel element3 = this.squadScreen.GetElement<UXLabel>("LabelModalStoryPerks");
			element3.Text = perkViewController.GetPerkDescForGroup(this.targetPerkVO.PerkGroup);
			bool useUpgrade = false;
			int perkTier = this.targetPerkVO.PerkTier;
			string perkGroup = this.targetPerkVO.PerkGroup;
			if (perkTier == 1)
			{
				element2.Text = lang.Get("PERK_UPGRADE_UNLOCK_CELEBRATION_TITLE", new object[]
				{
					perkViewController.GetPerkNameForGroup(perkGroup)
				});
			}
			else
			{
				useUpgrade = true;
				int num = perkTier - 1;
				element2.Text = lang.Get("PERK_UPGRADE_CELEBRATION_TITLE", new object[]
				{
					perkViewController.GetPerkNameForGroup(perkGroup),
					num,
					perkTier
				});
			}
			perkViewController.SetupStatGridForPerk(this.targetPerkVO, this.statGrid, "TemplateModalStatsPerks", "LabelModalStatsInfoPerks", "LabelModalStatsValuePerks", useUpgrade);
			this.continueBtn = this.squadScreen.GetElement<UXButton>("BtnContinuePerkUpgradeCeleb");
			this.continueBtn.OnClicked = new UXButtonClickedDelegate(this.OnContinueButtonClicked);
			UXLabel element4 = this.squadScreen.GetElement<UXLabel>("LabelContinuePerkUpgradeCeleb");
			element4.Text = lang.Get("CONTINUE", new object[0]);
		}

		public void Show()
		{
			this.squadScreen.CurrentBackButton = this.continueBtn;
			this.squadScreen.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnContinueButtonClicked);
			this.rootInfoView.Visible = true;
			this.continueBtn.Visible = true;
			this.rootInfoView.InitAnimator();
			this.rootInfoView.SetTrigger("ShowCeleb");
			Service.EventManager.SendEvent(EventId.PerkCelebStarted, null);
		}

		private void OnContinueButtonClicked(UXButton btn)
		{
			this.HideAndCleanUp();
		}

		public override void HideAndCleanUp()
		{
			this.continueBtn.Visible = false;
			Service.EventManager.SendEvent(EventId.PerkCelebClosed, null);
			base.HideAndCleanUp();
		}
	}
}
