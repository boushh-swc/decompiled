using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenAdvancementView : AbstractSquadScreenViewModule, IEventObserver
	{
		private const string PERK_CONTAINER_GROUP = "PerksContainer";

		private const string PERK_MODAL_DIALOG_GROUP = "PanelContainerModalPerks";

		private const string PERK_TABS_GROUP = "TabGroupPerks";

		private const string PERK_UPGRADE_GROUP = "UpgradeMainGroupPerks";

		private const string PERK_CURRENCY_TRAY_GROUP = "PanelContainerCurrencyTrayPerks";

		public const string ACTIVATE_TOGGLE_BTN = "TabActivatePerks";

		public const string UPGRADE_TOGGLE_BTN = "TabUpgradePerks";

		private const string PERK_TITLE_LABEL = "LabelTitlePerks";

		private const string PERK_LEVEL_BUTTON = "SquadLevelMainGroupPerks";

		private const string SQUAD_LEVEL_LABEL = "LabelSquadLvlPerks";

		private const string SQUAD_REP_PROGRESSBAR = "PBarSquadLevelMainPerks";

		private const string SQUAD_REP_PROGRESS_LABEL = "LabelSquadLvlProgressPerks";

		private const string PERK_TAB_CHECKBOX = "SocialPerksBtn";

		private const string ACTIVATE_TAB_TOGGLE_LABEL = "LabelTabActPerks";

		private const string UPGRADE_TAB_TOGGLE_LABEL = "LabelTabUpPerks";

		private const string PERK_CONTEXT_ACTIVATE = "PERK_CONTEXT_ACTIVATE";

		private const string PERK_CONTEXT_UPGRADE = "PERK_CONTEXT_UPGRADE";

		private const string SQUAD_MAX_LEVEL = "MAX_LEVEL";

		private const string PERK_MAIN_TITLE = "PERK_MAIN_TITLE";

		private UXElement perkContainerGroup;

		private UXElement perkModalDialogGroup;

		private UXElement perkTabsGroup;

		private UXLabel squadTitleLabel;

		private UXLabel squadLevelLabel;

		private UXLabel squadLevelProgressLabel;

		private UXSlider squadLevelProgressBar;

		private UXElement perkUpgradeGroup;

		private UXElement perkCurrencyTrayGroup;

		private UXCheckbox tabButton;

		private JewelControl perkTabBadge;

		private SquadAdvancementBaseTab activeAdvancementTab;

		private UXCheckbox tabActivate;

		private UXCheckbox tabUpgrade;

		private List<SquadAdvancementBaseTab> allAdvancementTabs;

		public SquadScreenAdvancementView(SquadSlidingScreen screen) : base(screen)
		{
			this.allAdvancementTabs = new List<SquadAdvancementBaseTab>();
		}

		public override void OnScreenLoaded()
		{
			this.perkContainerGroup = this.screen.GetElement<UXElement>("PerksContainer");
			this.perkContainerGroup.Visible = false;
			UXElement element = this.screen.GetElement<UXElement>(SquadScreenBasePerkInfoView.PERK_ACTIVATION_INFO_VIEW);
			element.Visible = false;
			this.perkCurrencyTrayGroup = this.screen.GetElement<UXElement>("PanelContainerCurrencyTrayPerks");
			UXElement element2 = this.screen.GetElement<UXElement>(SquadScreenConfirmPerkUpgradeView.PERK_INVEST_CONFIRM_VIEW);
			element2.Visible = false;
			this.squadTitleLabel = this.screen.GetElement<UXLabel>("LabelTitlePerks");
			this.squadTitleLabel.Text = this.lang.Get("PERK_MAIN_TITLE", new object[0]);
			UXButton element3 = this.screen.GetElement<UXButton>("SquadLevelMainGroupPerks");
			element3.OnClicked = new UXButtonClickedDelegate(this.OnSquadLevelSelected);
			this.squadLevelLabel = this.screen.GetElement<UXLabel>("LabelSquadLvlPerks");
			this.squadLevelProgressLabel = this.screen.GetElement<UXLabel>("LabelSquadLvlProgressPerks");
			this.squadLevelProgressBar = this.screen.GetElement<UXSlider>("PBarSquadLevelMainPerks");
			this.tabButton = this.screen.GetElement<UXCheckbox>("SocialPerksBtn");
			this.tabButton.OnSelected = new UXCheckboxSelectedDelegate(this.OnTabButtonSelected);
			this.perkTabBadge = JewelControl.Create(this.screen, "Perks");
			this.InitPerkTabs();
			this.RefreshSquadLevel();
			this.RegisterEvents();
		}

		private void OnSquadLevelSelected(UXButton button)
		{
			if (this.activeAdvancementTab != null && !this.activeAdvancementTab.ShouldBlockInput() && Service.PerkManager.HasPlayerSeenPerkTutorial())
			{
				Service.UXController.MiscElementsManager.ShowSquadLevelTooltip(button);
			}
		}

		public virtual EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.SquadPerkUpdated:
			case EventId.PerkInvestment:
			case EventId.PerkUnlocked:
			case EventId.PerkUpgraded:
				this.RefreshSquadLevel();
				this.screen.UpdateBadges();
				break;
			case EventId.SquadLeveledUp:
				this.RefreshSquadLevel();
				if (this.IsVisible())
				{
					Service.PerkViewController.ShowSquadLevelUpIfPending();
				}
				this.screen.UpdateBadges();
				break;
			case EventId.PerkCelebClosed:
				this.screen.UpdateBadges();
				break;
			default:
				if (id == EventId.SquadScreenOpenedOrClosed)
				{
					if (!(bool)cookie)
					{
						this.SetDefaultTabActive();
					}
				}
				break;
			}
			return EatResponse.NotEaten;
		}

		public override bool IsVisible()
		{
			return this.perkContainerGroup.Visible;
		}

		public override void ShowView()
		{
			PerkViewController perkViewController = Service.PerkViewController;
			this.perkContainerGroup.Visible = true;
			this.activeAdvancementTab.Visible = true;
			this.perkCurrencyTrayGroup.Visible = true;
			this.RefreshView();
			perkViewController.ShowSquadLevelUpIfPending();
			this.UpdateBadge();
			Service.EventManager.SendEvent(EventId.SquadAdvancementTabSelected, this);
			Service.EventManager.SendEvent(EventId.SquadSelect, null);
			perkViewController.UpdateLastViewedPerkTime();
		}

		public override void HideView()
		{
			this.perkContainerGroup.Visible = false;
			this.activeAdvancementTab.Visible = false;
			this.perkCurrencyTrayGroup.Visible = false;
		}

		public override void RefreshView()
		{
			this.activeAdvancementTab.RefreshPerkStates();
		}

		public override void OnDestroyElement()
		{
			this.UnregisterEvents();
			this.HandleTabDestruction();
		}

		private void HandleTabDestruction()
		{
			int count = this.allAdvancementTabs.Count;
			for (int i = 0; i < count; i++)
			{
				this.allAdvancementTabs[i].OnDestroyElement();
			}
			this.allAdvancementTabs.Clear();
		}

		public CurrencyTrayType GetDisplayCurrencyTrayType()
		{
			if (this.activeAdvancementTab is SquadAdvancementUpgradeTab)
			{
				return CurrencyTrayType.Reputation;
			}
			if (this.activeAdvancementTab is SquadAdvancementActivateTab)
			{
				return CurrencyTrayType.All;
			}
			return CurrencyTrayType.Default;
		}

		private void RegisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.SquadLeveledUp);
			eventManager.RegisterObserver(this, EventId.SquadPerkUpdated);
			eventManager.RegisterObserver(this, EventId.PerkCelebClosed);
			eventManager.RegisterObserver(this, EventId.PerkUnlocked);
			eventManager.RegisterObserver(this, EventId.PerkUpgraded);
			eventManager.RegisterObserver(this, EventId.PerkInvestment);
			eventManager.RegisterObserver(this, EventId.SquadScreenOpenedOrClosed);
		}

		private void UnregisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.SquadLeveledUp);
			eventManager.UnregisterObserver(this, EventId.SquadPerkUpdated);
			eventManager.UnregisterObserver(this, EventId.PerkCelebClosed);
			eventManager.UnregisterObserver(this, EventId.PerkUnlocked);
			eventManager.UnregisterObserver(this, EventId.PerkUpgraded);
			eventManager.UnregisterObserver(this, EventId.PerkInvestment);
			eventManager.UnregisterObserver(this, EventId.SquadScreenOpenedOrClosed);
		}

		private SquadAdvancementBaseTab CreateAndAddActivateTab(SquadSlidingScreen screen)
		{
			Lang lang = Service.Lang;
			string tabLabelString = lang.Get("PERK_CONTEXT_ACTIVATE", new object[0]);
			SquadAdvancementBaseTab squadAdvancementBaseTab = new SquadAdvancementActivateTab(screen, "LabelTabActPerks", tabLabelString);
			this.allAdvancementTabs.Add(squadAdvancementBaseTab);
			return squadAdvancementBaseTab;
		}

		private SquadAdvancementBaseTab CreateAndAddUpgradeTab(SquadSlidingScreen screen)
		{
			Lang lang = Service.Lang;
			string tabLabelString = lang.Get("PERK_CONTEXT_UPGRADE", new object[0]);
			SquadAdvancementBaseTab squadAdvancementBaseTab = new SquadAdvancementUpgradeTab(screen, "LabelTabUpPerks", tabLabelString);
			this.allAdvancementTabs.Add(squadAdvancementBaseTab);
			return squadAdvancementBaseTab;
		}

		private void InitPerkTabs()
		{
			this.allAdvancementTabs.Clear();
			this.tabActivate = this.screen.GetElement<UXCheckbox>("TabActivatePerks");
			this.tabUpgrade = this.screen.GetElement<UXCheckbox>("TabUpgradePerks");
			this.tabActivate.OnSelected = new UXCheckboxSelectedDelegate(this.OnAdvancementTabSelected);
			this.tabUpgrade.OnSelected = new UXCheckboxSelectedDelegate(this.OnAdvancementTabSelected);
			this.perkTabsGroup = this.screen.GetElement<UXElement>("TabGroupPerks");
			this.perkTabsGroup.Visible = true;
			this.perkUpgradeGroup = this.screen.GetElement<UXElement>("UpgradeMainGroupPerks");
			this.perkUpgradeGroup.Visible = false;
			this.activeAdvancementTab = this.CreateAndAddActivateTab(this.screen);
			this.activeAdvancementTab.Visible = true;
			this.tabActivate.Tag = this.activeAdvancementTab;
			SquadAdvancementBaseTab squadAdvancementBaseTab = this.CreateAndAddUpgradeTab(this.screen);
			squadAdvancementBaseTab.Visible = false;
			this.tabUpgrade.Tag = squadAdvancementBaseTab;
		}

		private void OnAdvancementTabSelected(UXCheckbox checkbox, bool selected)
		{
			if (this.activeAdvancementTab != null && this.activeAdvancementTab.ShouldBlockTabChanges())
			{
				checkbox.Selected = !selected;
				return;
			}
			if (selected)
			{
				SquadAdvancementBaseTab squadAdvancementBaseTab = (SquadAdvancementBaseTab)checkbox.Tag;
				if (squadAdvancementBaseTab != this.activeAdvancementTab)
				{
					this.activeAdvancementTab.Visible = false;
					squadAdvancementBaseTab.Visible = true;
					this.activeAdvancementTab = squadAdvancementBaseTab;
					Service.EventManager.SendEvent(EventId.SquadAdvancementTabSelected, this);
				}
			}
		}

		private void SetDefaultTabActive()
		{
			SquadAdvancementUpgradeTab squadAdvancementUpgradeTab = (SquadAdvancementUpgradeTab)this.tabUpgrade.Tag;
			squadAdvancementUpgradeTab.CleanUpConfirmInfoView();
			this.tabActivate.Selected = true;
			this.tabUpgrade.Selected = false;
			SquadAdvancementBaseTab squadAdvancementBaseTab = (SquadAdvancementBaseTab)this.tabActivate.Tag;
			if (squadAdvancementBaseTab != this.activeAdvancementTab)
			{
				this.activeAdvancementTab.Visible = false;
				this.activeAdvancementTab = squadAdvancementBaseTab;
			}
		}

		private void RefreshSquadLevel()
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return;
			}
			this.squadLevelLabel.Text = currentSquad.Level.ToString();
			int totalRepInvested = currentSquad.TotalRepInvested;
			int reputationReqForSquadLevel = SquadUtils.GetReputationReqForSquadLevel(currentSquad.Level);
			int reputationReqForSquadLevel2 = SquadUtils.GetReputationReqForSquadLevel(currentSquad.Level + 1);
			if (reputationReqForSquadLevel2 < 0)
			{
				this.squadLevelProgressLabel.Text = this.lang.Get("MAX_LEVEL", new object[0]);
				this.squadLevelProgressBar.Value = 1f;
				return;
			}
			int num = totalRepInvested - reputationReqForSquadLevel;
			int num2 = reputationReqForSquadLevel2 - reputationReqForSquadLevel;
			this.squadLevelProgressLabel.Text = num + " / " + num2;
			this.squadLevelProgressBar.Value = (float)num / (float)num2;
		}

		private void OnTabButtonSelected(UXCheckbox button, bool selected)
		{
			if (selected)
			{
				SquadController squadController = Service.SquadController;
				squadController.StateManager.SquadScreenState = SquadScreenState.Advancement;
				this.screen.RefreshViews();
			}
		}

		public int UpdateBadge()
		{
			SquadController squadController = Service.SquadController;
			PerkViewController perkViewController = Service.PerkViewController;
			int num = 0;
			if (squadController.HavePendingSquadLevelCelebration())
			{
				num++;
			}
			num += perkViewController.GetBadgedPerkCount();
			this.perkTabBadge.Value = num;
			return num;
		}
	}
}
