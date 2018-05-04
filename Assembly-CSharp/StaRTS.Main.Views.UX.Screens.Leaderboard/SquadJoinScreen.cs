using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public class SquadJoinScreen : AbstractLeaderboardScreen, IEventObserver
	{
		private const string SEARCH_SQUAD_INPUT = "LabelInputNameSquad";

		private const string SEARCH_INSTRUCTIONS_LABEL = "LabelSearchInstructions";

		private const string SEARCH_WIN_SEARCH_BTN = "BtnSearchWindow";

		private const string SEARCH_WIN_SEARCH_LABEL = "LabelBtnSearchWindow";

		private const string JOIN_SQUAD_FRIENDS = "JoinSquad_Friends";

		private const string JOIN_SQUAD_SEARCH = "JoinSquad_Search";

		private const string JOIN_SQUAD_FEATURED = "JoinSquad_Featured";

		private const string FILTER_OPTIONS = "FilterOptions";

		private const string BTN_FILTER = "BtnFilter";

		private const int MIN_SEARCH_STRING_LENGTH = 3;

		private const int ANCHOR_OFFSET = -10;

		private const string JOIN_A_SQUAD = "JOIN_A_SQUAD";

		private const string FEATURED = "s_Featured";

		private const string FRIENDS = "s_Friends";

		private const string SEARCH = "s_Search";

		private const string INVITES = "s_Invites";

		private const string SEARCH_INSTRUCTIONS = "SEARCH_INSTRUCTIONS";

		private const string SEARCH_LENGTH_ISSUE = "SEARCH_LENGTH_ISSUE";

		private const string SEARCH_NO_RESULTS = "SEARCH_NO_RESULTS";

		private const string GENERIC_SEARCH_ISSUE = "GENERIC_SEARCH_ISSUE";

		private UXLabel searchInstructionsLabel;

		private UXInput searchInput;

		private UXButton searchBtnOverlay;

		private JewelControl invitesJewel;

		private List<string> squadIdsRequiringDetails;

		public SquadJoinScreen()
		{
			this.initialTab = SocialTabs.Featured;
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			UIPanel component = this.panel.Root.GetComponent<UIPanel>();
			component.topAnchor.absolute = -10;
			base.TabClicked(true, this.initialTab);
			this.titleLabel.Text = this.lang.Get("JOIN_A_SQUAD", new object[0]);
			this.InitSearch();
			base.GetElement<UXElement>("FilterOptions").Visible = false;
			base.GetElement<UXButton>("BtnFilter").Visible = false;
			this.navigationRow.Visible = false;
			if (Service.LeaderboardController.ShouldRefreshData(PlayerListType.FeaturedSquads, null))
			{
				Service.LeaderboardController.UpdateFeaturedSquads(new LeaderboardController.OnUpdateData(this.OnGetFeaturedSquads));
			}
			if (GameConstants.SQUAD_INVITES_ENABLED)
			{
				EventManager eventManager = Service.EventManager;
				eventManager.RegisterObserver(this, EventId.SquadJoinInviteReceived);
				eventManager.RegisterObserver(this, EventId.SquadJoinInviteRemoved);
				this.squadIdsRequiringDetails = new List<string>();
				this.invitesJewel = JewelControl.Create(this, "Invites");
				List<SquadInvite> squadInvites = Service.SquadController.StateManager.SquadInvites;
				if (squadInvites != null)
				{
					this.UpdateSquadDataForInvites(squadInvites);
					this.UpdateInvitesJewel();
				}
				else
				{
					eventManager.RegisterObserver(this, EventId.SquadJoinInvitesReceived);
					this.invitesJewel.Value = 0;
				}
			}
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			UXCheckbox element = base.GetElement<UXCheckbox>("BtnSquads");
			element.OnSelected = new UXCheckboxSelectedDelegate(this.FeaturedTabClicked);
			element.Selected = true;
			SocialTabInfo tabInfo = base.GetTabInfo(SocialTabs.Featured);
			tabInfo.TabButton = element;
			tabInfo.TabLabel = base.GetElement<UXLabel>("LabelSquads");
			tabInfo.TabLabel.Text = this.lang.Get("s_Featured", new object[0]);
			element = base.GetElement<UXCheckbox>("BtnFriends");
			element.OnSelected = new UXCheckboxSelectedDelegate(this.SquadFriendsTabClicked);
			element.Selected = false;
			tabInfo = base.GetTabInfo(SocialTabs.Friends);
			tabInfo.TabButton = element;
			tabInfo.TabLabel = base.GetElement<UXLabel>("LabelBtnFriends");
			tabInfo.TabLabel.Text = this.lang.Get("s_Friends", new object[0]);
			element = base.GetElement<UXCheckbox>("BtnLeaders");
			element.OnSelected = new UXCheckboxSelectedDelegate(this.SearchTabClicked);
			element.Selected = false;
			tabInfo = base.GetTabInfo(SocialTabs.Search);
			tabInfo.TabButton = element;
			tabInfo.TabLabel = base.GetElement<UXLabel>("LabelBtnLeaders");
			tabInfo.TabLabel.Text = this.lang.Get("s_Search", new object[0]);
			element = base.GetElement<UXCheckbox>("BtnTournament");
			element.OnSelected = new UXCheckboxSelectedDelegate(this.InvitesTabClicked);
			element.Selected = false;
			element.Visible = GameConstants.SQUAD_INVITES_ENABLED;
			tabInfo = base.GetTabInfo(SocialTabs.Invites);
			tabInfo.TabButton = element;
			tabInfo.TabLabel = base.GetElement<UXLabel>("LabelBtnTournament");
			tabInfo.TabLabel.Text = this.lang.Get("s_Invites", new object[0]);
		}

		protected override void InitTabInfo()
		{
			SocialTabInfo value = new SocialTabInfo(new Action(this.LoadFeatureSquads), EventId.UISquadJoinTabShown, "featured", PlayerListType.FeaturedSquads);
			this.tabs.Add(SocialTabs.Featured, value);
			value = new SocialTabInfo(new Action(base.LoadFriends), EventId.UISquadJoinTabShown, "friends", PlayerListType.Friends);
			this.tabs.Add(SocialTabs.Friends, value);
			value = new SocialTabInfo(new Action(this.LoadSearchTab), EventId.UISquadJoinTabShown, "search", PlayerListType.SearchedSquads);
			this.tabs.Add(SocialTabs.Search, value);
			value = new SocialTabInfo(new Action(this.LoadSquadInvites), EventId.UISquadJoinTabShown, "invites", PlayerListType.Invites);
			this.tabs.Add(SocialTabs.Invites, value);
		}

		private void OnGetFeaturedSquads(bool success)
		{
			if (success && this.curTab == SocialTabs.Featured)
			{
				this.LoadFeatureSquads();
			}
		}

		protected void SquadFriendsTabClicked(UXCheckbox box, bool selected)
		{
			base.TabClicked(selected, SocialTabs.Friends);
		}

		protected override void InitIndividualGrids(UXGrid baseGrid)
		{
			GridLoadHelper tabGridLoadHelper = base.CreateGridLoadHelperByCloningGrid(baseGrid, "LBSquadGrid");
			base.GetTabInfo(SocialTabs.Featured).TabGridLoadHelper = tabGridLoadHelper;
			tabGridLoadHelper = base.CreateGridLoadHelperByCloningGrid(baseGrid, "LBFriendsGrid");
			base.GetTabInfo(SocialTabs.Friends).TabGridLoadHelper = tabGridLoadHelper;
			tabGridLoadHelper = base.CreateGridLoadHelperByCloningGrid(baseGrid, "LBPlayersGrid");
			base.GetTabInfo(SocialTabs.Search).TabGridLoadHelper = tabGridLoadHelper;
			tabGridLoadHelper = base.CreateGridLoadHelperByCloningGrid(baseGrid, "LBTournamentGrid");
			base.GetTabInfo(SocialTabs.Invites).TabGridLoadHelper = tabGridLoadHelper;
		}

		private void UpdateInvitesJewel()
		{
			List<SquadInvite> squadInvites = Service.SquadController.StateManager.SquadInvites;
			this.invitesJewel.Value = ((squadInvites == null) ? 0 : squadInvites.Count);
		}

		private void InitSearch()
		{
			this.searchBtnOverlay = base.GetElement<UXButton>("BtnSearchWindow");
			this.searchBtnOverlay.OnClicked = new UXButtonClickedDelegate(this.SearchClicked);
			this.searchBtnOverlay.Enabled = false;
			UXLabel element = base.GetElement<UXLabel>("LabelBtnSearchWindow");
			element.Text = this.lang.Get("s_Search", new object[0]);
			this.searchInput = base.GetElement<UXInput>("LabelInputNameSquad");
			this.searchInstructionsLabel = base.GetElement<UXLabel>("LabelSearchInstructions");
			this.searchInstructionsLabel.Text = this.lang.Get("SEARCH_INSTRUCTIONS", new object[0]);
			this.searchInstructionsLabel.Visible = true;
			this.searchInput.Text = string.Empty;
			this.searchInput.InitText(this.lang.Get("s_Search", new object[0]));
			UIInput uIInputComponent = this.searchInput.GetUIInputComponent();
			uIInputComponent.onValidate = new UIInput.OnValidate(LangUtils.OnValidateWSpaces);
			EventDelegate item = new EventDelegate(new EventDelegate.Callback(this.OnChange));
			uIInputComponent.onChange.Add(item);
		}

		private void RefreshCurrentTab()
		{
			switch (this.curTab)
			{
			case SocialTabs.Featured:
				this.LoadFeatureSquads();
				break;
			case SocialTabs.Friends:
				base.LoadFriends();
				break;
			case SocialTabs.Search:
				this.LoadSearchTab();
				break;
			case SocialTabs.Invites:
				this.LoadSquadInvites();
				break;
			}
		}

		private void LoadFeatureSquads()
		{
			base.ResetGrid();
			this.AddCreateSquadItem(SocialTabs.Featured);
			base.AddItemsToGrid<Squad>(Service.LeaderboardController.FeaturedSquads.List, true, false);
			UXUtils.SetSpriteTopAnchorPoint(this.scrollUp, -4);
		}

		private void FeaturedTabClicked(UXCheckbox box, bool selected)
		{
			base.TabClicked(selected, SocialTabs.Featured);
		}

		private void LoadSquadInvites()
		{
			base.ResetGrid();
			List<SquadInvite> squadInvites = Service.SquadController.StateManager.SquadInvites;
			if (squadInvites != null && this.squadIdsRequiringDetails.Count == 0)
			{
				base.AddItemsToGrid<SquadInvite>(squadInvites, true, false);
			}
			else
			{
				ProcessingScreen.Show();
			}
			UXUtils.SetSpriteTopAnchorPoint(this.scrollUp, -4);
		}

		private void InvitesTabClicked(UXCheckbox box, bool selected)
		{
			base.TabClicked(selected, SocialTabs.Invites);
		}

		public override void OnVisitClicked(UXButton button)
		{
			string playerId = button.Tag as string;
			bool isFriend = false;
			string tabName = null;
			switch (this.curTab)
			{
			case SocialTabs.Featured:
				tabName = "JoinSquad_Featured";
				break;
			case SocialTabs.Friends:
				isFriend = true;
				tabName = "JoinSquad_Friends";
				break;
			case SocialTabs.Search:
				tabName = "JoinSquad_Search";
				break;
			}
			PlayerVisitTag cookie = new PlayerVisitTag(false, isFriend, tabName, playerId);
			Service.EventManager.SendEvent(EventId.VisitPlayer, cookie);
			base.OnVisitClicked(button);
		}

		private void OnChange()
		{
			string text = this.searchInput.Text;
			if (!string.IsNullOrEmpty(text) && text.Length >= 3)
			{
				this.searchInstructionsLabel.Visible = false;
				this.searchBtnOverlay.Enabled = true;
			}
			else
			{
				this.searchInstructionsLabel.Visible = true;
				string id = (!string.IsNullOrEmpty(text)) ? "SEARCH_LENGTH_ISSUE" : "SEARCH_INSTRUCTIONS";
				this.searchInstructionsLabel.Text = this.lang.Get(id, new object[0]);
				this.searchBtnOverlay.Enabled = false;
			}
		}

		private void SearchResultsEmpty()
		{
			this.searchInstructionsLabel.Visible = true;
			this.searchInstructionsLabel.Text = this.lang.Get("SEARCH_NO_RESULTS", new object[0]);
		}

		private void LoadSearchTab()
		{
			base.ResetGrid();
			this.backButtonHelper.AddElementToTopLayer(this.searchOverlay);
			UXUtils.SetSpriteTopAnchorPoint(this.scrollUp, -175);
			this.AddEmptySpacingItem();
		}

		private void SearchTabClicked(UXCheckbox box, bool selected)
		{
			if (!selected)
			{
				this.backButtonHelper.RemoveElementFromTopLayer(this.searchOverlay);
				this.searchInstructionsLabel.Visible = false;
				return;
			}
			this.OnChange();
			base.TabClicked(selected, SocialTabs.Search);
		}

		private void SearchClicked(UXButton btn)
		{
			ProcessingScreen.Show();
			this.PrepareForNextSearch();
			this.searchBtnOverlay.Enabled = false;
			Service.LeaderboardController.SearchSquadsByName(this.searchInput.Text, new LeaderboardController.OnUpdateData(this.OnSquadsSearched));
		}

		private void PrepareForNextSearch()
		{
			Service.LeaderboardController.SearchedSquads.List.Clear();
		}

		private void OnSquadsSearched(bool success)
		{
			if (!this.Visible)
			{
				return;
			}
			this.searchBtnOverlay.Enabled = true;
			ProcessingScreen.Hide();
			List<Squad> list = Service.LeaderboardController.SearchedSquads.List;
			if (success && list.Count == 0)
			{
				this.SearchResultsEmpty();
			}
			if (this.curTab == SocialTabs.Search)
			{
				base.ResetGrid();
				this.AddEmptySpacingItem();
				if (success)
				{
					base.AddItemsToGrid<Squad>(list, true, false);
				}
				else
				{
					AlertScreen.ShowModal(false, null, this.lang.Get("GENERIC_SEARCH_ISSUE", new object[0]), null, null);
				}
			}
		}

		private void AddEmptySpacingItem()
		{
			LeaderboardRowFacebookView leaderboardRowFacebookView = new LeaderboardRowFacebookView(this, this.gridLoadHelper.GetGrid(), this.fbItemTemplate, this.curTab);
			leaderboardRowFacebookView.Hide();
			this.rowViews.Add(leaderboardRowFacebookView);
			UXElement item = leaderboardRowFacebookView.GetItem();
			this.gridLoadHelper.AddElement(item);
		}

		protected override FactionToggle GetSelectedFaction()
		{
			return FactionToggle.All;
		}

		protected override string GetSelectedPlanetId()
		{
			return null;
		}

		private void AddCreateSquadItem(SocialTabs socialTab)
		{
			int nexElementPosition = this.gridLoadHelper.GetNexElementPosition();
			LeaderboardRowCreateSquadView leaderboardRowCreateSquadView = new LeaderboardRowCreateSquadView(this, this.gridLoadHelper.GetGrid(), this.createSquadItemTemplate, nexElementPosition);
			this.rowViews.Add(leaderboardRowCreateSquadView);
			UXElement item = leaderboardRowCreateSquadView.GetItem();
			this.gridLoadHelper.AddElement(item);
		}

		public override void OnDestroyElement()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is WarBoardState)
			{
				Service.UXController.HUD.SetSquadScreenAlwaysOnTop(true);
			}
			this.searchInput.GetUIInputComponent().onChange.Clear();
			base.OnDestroyElement();
		}

		private void UpdateSquadDataForInvites(List<SquadInvite> invites)
		{
			LeaderboardController leaderboardController = Service.LeaderboardController;
			int i = 0;
			int count = invites.Count;
			while (i < count)
			{
				this.UpdateSquadDataForInvite(invites[i], leaderboardController);
				i++;
			}
		}

		private void UpdateSquadDataForInvite(SquadInvite invite, LeaderboardController leaderboardController)
		{
			this.squadIdsRequiringDetails.Add(invite.SquadId);
			leaderboardController.UpdateSquadDetails(invite.SquadId, new LeaderboardController.OnUpdateSquadData(this.OnSquadDataUpdated));
		}

		private void OnSquadDataUpdated(Squad squad, bool success)
		{
			this.squadIdsRequiringDetails.Remove(squad.SquadID);
			if (this.squadIdsRequiringDetails.Count == 0 && this.curTab == SocialTabs.Invites)
			{
				ProcessingScreen.Hide();
				this.LoadSquadInvites();
			}
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.SquadJoinInviteRemoved:
				this.UpdateInvitesJewel();
				break;
			case EventId.SquadJoinInviteReceived:
				this.UpdateSquadDataForInvite((SquadInvite)cookie, Service.LeaderboardController);
				this.UpdateInvitesJewel();
				break;
			case EventId.SquadJoinInvitesReceived:
				this.UpdateSquadDataForInvites((List<SquadInvite>)cookie);
				this.UpdateInvitesJewel();
				Service.EventManager.UnregisterObserver(this, EventId.SquadJoinInvitesReceived);
				break;
			}
			return base.OnEvent(id, cookie);
		}
	}
}
