using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadSlidingScreen : PersistentAnimatedScreen, IEventObserver
	{
		private const string CHAT_HOLDER_PANEL = "ChatHolderPanel";

		private const string OPEN_CLOSE_ANIMATION = "ChatHolder";

		private const string OPEN_CLOSE_STATE_ANIM = "anim_chat_display";

		private const string BTN_SQUAD_SCREEN_SLIDE = "ButtonChat";

		private const string SQUAD_MEMBERS = "Squad_Members";

		private const string BTN_CLOSE_SQUAD = "BtnCloseSquad";

		private const string BG_DIALOG = "BgDialog";

		private const string SQUAD_NAVIGATION = "SquadNavigation";

		private SquadScreenChatView chatView;

		private SquadScreenMembersView membersView;

		private SquadScreenOverviewView overviewView;

		private SquadScreenTroopDonationView troopDonationView;

		private SquadScreenWarButtonView warButtonView;

		private SquadScreenWarLogView warLogView;

		private SquadScreenAdvancementView advancementView;

		private UXElement chatHolderPanel;

		private UXButton squadSlideBtn;

		private UXElement bgDialog;

		private UXButton closeSquadBtn;

		private UXElement squadNavigation;

		private JewelControl squadBadge;

		public override bool ShowCurrencyTray
		{
			get
			{
				return true;
			}
		}

		public SquadSlidingScreen() : base("gui_squad")
		{
			this.chatView = new SquadScreenChatView(this);
			this.membersView = new SquadScreenMembersView(this);
			this.overviewView = new SquadScreenOverviewView(this);
			this.troopDonationView = new SquadScreenTroopDonationView(this);
			this.warButtonView = new SquadScreenWarButtonView(this);
			this.warLogView = new SquadScreenWarLogView(this);
			this.advancementView = new SquadScreenAdvancementView(this);
		}

		protected override void OnScreenLoaded()
		{
			Service.BuildingController.CancelEditModeTimer();
			base.InitAnimations("ChatHolder", "anim_chat_display");
			this.chatHolderPanel = base.GetElement<UXElement>("ChatHolderPanel");
			this.squadSlideBtn = base.GetElement<UXButton>("ButtonChat");
			this.squadSlideBtn.OnClicked = new UXButtonClickedDelegate(this.OnSquadSlideClicked);
			this.bgDialog = base.GetElement<UXElement>("BgDialog");
			this.bgDialog.Visible = false;
			this.squadNavigation = base.GetElement<UXElement>("SquadNavigation");
			this.squadNavigation.Visible = false;
			this.squadBadge = JewelControl.Create(this, "Chat");
			this.closeSquadBtn = base.GetElement<UXButton>("BtnCloseSquad");
			this.closeSquadBtn.OnClicked = new UXButtonClickedDelegate(this.OnCloseSquad);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.CurrentPlayerMemberDataUpdated);
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
			eventManager.RegisterObserver(this, EventId.HUDVisibilityChanged);
			eventManager.RegisterObserver(this, EventId.SquadLeft);
			eventManager.RegisterObserver(this, EventId.SquadUpdated);
			eventManager.RegisterObserver(this, EventId.WarRewardClaimed);
			this.chatView.OnScreenLoaded();
			this.membersView.OnScreenLoaded();
			this.overviewView.OnScreenLoaded();
			this.troopDonationView.OnScreenLoaded();
			this.warButtonView.OnScreenLoaded();
			this.warLogView.OnScreenLoaded();
			this.advancementView.OnScreenLoaded();
			this.HideAllViews();
		}

		private void OnCloseSquad(UXButton closeBtrn)
		{
			if (base.IsOpen())
			{
				this.HideAllViews();
				base.AnimateClosed(false, null);
			}
		}

		public void OnSquadSlideClicked(UXButton slideButton)
		{
			this.ToggleSquadSlideScren();
		}

		public void ToggleSquadSlideScren()
		{
			if (base.IsClosed())
			{
				SquadController squadController = Service.SquadController;
				squadController.StateManager.SquadScreenState = SquadScreenState.Chat;
				this.AnimateOpen();
			}
			else if (base.IsOpen())
			{
				this.HideAllViews();
				base.AnimateClosed(false, null);
			}
		}

		public override void AnimateOpen()
		{
			base.AnimateOpen();
			this.UpdateBadges();
		}

		protected override void OnOpening()
		{
			this.bgDialog.Visible = true;
			this.squadNavigation.Visible = true;
			this.RefreshViews();
			base.OnOpening();
		}

		protected override void OnClosing()
		{
			base.OnClosing();
			this.UpdateBadges();
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			this.chatHolderPanel.RefreshPanel();
			SquadController squadController = Service.SquadController;
			squadController.StateManager.SetSquadScreenOpen(true);
			if (!this.chatView.ChatDisplaySetup)
			{
				this.chatView.SetupChatDisplay();
			}
		}

		protected override void OnClose()
		{
			base.OnClose();
			this.HideAllViews();
			this.bgDialog.Visible = false;
			this.squadNavigation.Visible = false;
			Service.SquadController.WarManager.MatchMakingPrepMode = false;
			SquadController squadController = Service.SquadController;
			squadController.StateManager.SetSquadScreenOpen(false);
		}

		private void HideAllOtherVisibleContainerViews(AbstractSquadScreenViewModule view)
		{
			if (view != this.chatView && this.chatView.IsVisible())
			{
				this.chatView.HideView();
			}
			if (view != this.membersView && this.membersView.IsVisible())
			{
				this.membersView.HideView();
			}
			if (view != this.overviewView && this.overviewView.IsVisible())
			{
				this.overviewView.HideView();
			}
			if (view != this.troopDonationView && this.troopDonationView.IsVisible())
			{
				this.troopDonationView.HideView();
			}
			if (view != this.warLogView && this.warLogView.IsVisible())
			{
				this.warLogView.HideView();
			}
			if (view != this.advancementView && this.advancementView.IsVisible())
			{
				this.advancementView.HideView();
			}
		}

		public void RefreshViews()
		{
			SquadController squadController = Service.SquadController;
			switch (squadController.StateManager.SquadScreenState)
			{
			case SquadScreenState.Chat:
				this.HideAllOtherVisibleContainerViews(this.chatView);
				if (!this.chatView.IsVisible())
				{
					this.chatView.ShowView();
				}
				else
				{
					this.chatView.RefreshView();
				}
				break;
			case SquadScreenState.Members:
				this.HideAllOtherVisibleContainerViews(this.membersView);
				if (!this.membersView.IsVisible())
				{
					this.membersView.ShowView();
				}
				else
				{
					this.membersView.RefreshView();
				}
				break;
			case SquadScreenState.Overview:
				this.HideAllOtherVisibleContainerViews(this.overviewView);
				if (!this.overviewView.IsVisible())
				{
					this.overviewView.ShowView();
				}
				else
				{
					this.overviewView.RefreshView();
				}
				break;
			case SquadScreenState.Donation:
				this.HideAllOtherVisibleContainerViews(this.chatView);
				if (!this.chatView.IsVisible())
				{
					this.chatView.ShowView();
				}
				else
				{
					this.chatView.RefreshView();
				}
				if (!this.troopDonationView.IsVisible())
				{
					this.troopDonationView.ShowView();
				}
				else
				{
					this.troopDonationView.RefreshView();
				}
				break;
			case SquadScreenState.WarLog:
				this.HideAllOtherVisibleContainerViews(this.warLogView);
				if (!this.warLogView.IsVisible())
				{
					this.warLogView.ShowView();
				}
				else
				{
					this.warLogView.RefreshView();
				}
				break;
			case SquadScreenState.Advancement:
				this.HideAllOtherVisibleContainerViews(this.advancementView);
				if (!this.advancementView.IsVisible())
				{
					this.advancementView.ShowView();
				}
				else
				{
					this.advancementView.RefreshView();
				}
				break;
			}
			this.warButtonView.RefreshView();
		}

		protected override CurrencyTrayType GetDisplayCurrencyTrayType()
		{
			if (this.advancementView != null && this.advancementView.IsVisible())
			{
				return this.advancementView.GetDisplayCurrencyTrayType();
			}
			return base.GetDisplayCurrencyTrayType();
		}

		private void HideAllViews()
		{
			this.chatView.HideView();
			this.membersView.HideView();
			this.overviewView.HideView();
			this.troopDonationView.HideView();
			this.warButtonView.HideView();
			this.warLogView.HideView();
			this.advancementView.HideView();
		}

		public void RefreshVisibility()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			HUD hUD = Service.UXController.HUD;
			HudConfig currentHudConfig = hUD.CurrentHudConfig;
			SquadController squadController = Service.SquadController;
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			bool visible = (currentState is GalaxyState || currentHudConfig.Has("SquadScreen")) && currentSquad != null;
			this.Visible = visible;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.SquadUpdated && id != EventId.SquadLeft && id != EventId.GameStateChanged && id != EventId.HUDVisibilityChanged)
			{
				if (id == EventId.CurrentPlayerMemberDataUpdated || id == EventId.WarRewardClaimed)
				{
					this.UpdateBadges();
				}
			}
			else
			{
				this.RefreshVisibility();
			}
			return base.OnEvent(id, cookie);
		}

		public override void OnDestroyElement()
		{
			this.HideAllViews();
			this.chatView.OnDestroyElement();
			this.membersView.OnDestroyElement();
			this.overviewView.OnDestroyElement();
			this.troopDonationView.OnDestroyElement();
			this.warButtonView.OnDestroyElement();
			this.warLogView.OnDestroyElement();
			this.advancementView.OnDestroyElement();
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.CurrentPlayerMemberDataUpdated);
			eventManager.UnregisterObserver(this, EventId.GameStateChanged);
			eventManager.UnregisterObserver(this, EventId.HUDVisibilityChanged);
			eventManager.UnregisterObserver(this, EventId.SquadLeft);
			eventManager.UnregisterObserver(this, EventId.SquadUpdated);
			eventManager.UnregisterObserver(this, EventId.WarRewardClaimed);
			base.OnDestroyElement();
		}

		protected override void OnAnimationComplete(uint id, object cookie)
		{
			base.OnAnimationComplete(id, cookie);
			if (!this.shouldCloseOnAnimComplete)
			{
				if (base.IsClosed())
				{
					this.RefreshVisibility();
				}
			}
			else
			{
				base.ClearCloseOnAnimFlags();
				Service.UXController.HUD.PrepForSquadScreenCreate();
			}
		}

		public void ShowSquadSlideButton()
		{
			this.squadSlideBtn.Visible = true;
		}

		public void HideSquadSlideButton()
		{
			this.squadSlideBtn.Visible = false;
		}

		public void OpenDonationView(string recipientId, string recipientUserName, int alreadyDonatedSize, int totalCapacity, int currentPlayerDonationCount, string requestId, bool isWarRequest, int troopDonationLimit, TroopDonationProgress donationProgress)
		{
			this.troopDonationView.InitView(recipientId, recipientUserName, alreadyDonatedSize, totalCapacity, currentPlayerDonationCount, requestId, isWarRequest, troopDonationLimit, donationProgress);
			SquadController squadController = Service.SquadController;
			squadController.StateManager.SquadScreenState = SquadScreenState.Donation;
			this.RefreshViews();
		}

		public void CloseDonationView()
		{
			SquadController squadController = Service.SquadController;
			if (squadController.StateManager.SquadScreenState == SquadScreenState.Donation)
			{
				this.troopDonationView.CloseView();
			}
		}

		public void UpdateBadges()
		{
			int num = this.chatView.UpdateBadges();
			int num2 = this.advancementView.UpdateBadge();
			int num3 = this.warLogView.RefreshBadge();
			bool flag = num > 0 || num3 > 0 || num2 > 0;
			if ((base.IsAnimClosing() || base.IsClosed()) && flag)
			{
				if (num3 > 0)
				{
					this.squadBadge.Text = "!";
				}
				else
				{
					this.squadBadge.Value = num + num2;
				}
			}
			else
			{
				this.squadBadge.Value = 0;
			}
		}
	}
}
