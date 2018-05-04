using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Leaderboard;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public abstract class AbstractLeaderboardScreen : ClosableScreen, IViewFrameTimeObserver
	{
		private const string TITLE_LABEL = "LabelLeaderboards";

		private const string CONTAINER = "LeaderboardsContainer";

		private const string PANEL = "LeaderboardsPanel";

		private const string SCROLL_UP = "SpriteLeaderboardsPanelScrollUp";

		private const string SCROLL_DOWN = "SpriteLeaderboardsPanelScrollDown";

		private const string NAVIGATION = "LeaderboardNavigation";

		private const string LEADERBOARD_NAVIGATION_ROW = "NavigationRow";

		private const string SEARCH_WINDOW = "ContainerSearchWindow";

		private const string GRID = "SquadGrid";

		private const string SQUAD_CREATE_TEMPLATE_ITEM = "SquadCreateItem";

		private const string TEMPLATE_ITEM = "SquadItem";

		private const string FB_TEMPLATE_ITEM = "SquadFacebookItem";

		protected const string BTN_SQUADS = "BtnSquads";

		protected const string BTN_SQUADS_LABEL = "LabelSquads";

		protected const string BTN_LEADERS = "BtnLeaders";

		protected const string BTN_LEADERS_LABEL = "LabelBtnLeaders";

		protected const string BTN_FRIENDS = "BtnFriends";

		protected const string BTN_FRIENDS_LABEL = "LabelBtnFriends";

		protected const string BTN_TOURNAMENT = "BtnTournament";

		protected const string BTN_TOURNAMENT_LABEL = "LabelBtnTournament";

		protected const string BTN_TOP_50 = "BtnSortTop";

		protected const string BTN_TOP_50_LABEL = "LabelBtnSortTop";

		protected const string BTN_FIND_ME = "BtnFindMe";

		protected const string BTN_FIND_ME_LABEL = "LabelBtnFindMe";

		protected const string LB_SQUAD_GRID = "LBSquadGrid";

		protected const string LB_FRIEND_GRID = "LBFriendsGrid";

		protected const string LB_PLAYER_GRID = "LBPlayersGrid";

		protected const string LB_TOURNAMENT_GRID = "LBTournamentGrid";

		protected const string FEATURE_SQUAD_BUTTON = "BtnFeaturedSquads";

		protected const string LABEL_FEATURE_SQUAD_BUTTON = "LabelBtnFeaturedSquads";

		private const int MAX_ELEMENTS = 51;

		private const float SCROLL_DELTA = 30f;

		protected const int DEFAULT_SCROLL_ARROW_ANCHOR = -4;

		protected const int SEARCH_SCROLL_ARROW_ANCHOR = -175;

		private static readonly Vector3 OFFSCREEN_GRID_POSITION = new Vector3(3000f, 0f, 0f);

		private const string GENERIC_SQUAD_INFO_ISSUE = "GENERIC_SQUAD_INFO_ISSUE";

		protected GridLoadHelper gridLoadHelper;

		protected UIScrollView scrollView;

		protected Dictionary<SocialTabs, SocialTabInfo> tabs;

		protected SocialTabs initialTab;

		protected PlanetVO initialPlanetVO;

		protected SquadInfoView squadInfoView;

		protected BackButtonHelper backButtonHelper;

		protected List<AbstractLeaderboardRowView> rowViews;

		protected AbstractLeaderboardRowView selectedRow;

		private LeaderboardRowFacebookView facebookRow;

		protected UXElement itemTemplate;

		protected UXElement createSquadItemTemplate;

		protected UXElement fbItemTemplate;

		protected UXLabel titleLabel;

		protected UXElement container;

		protected UXElement navigation;

		protected UXElement navigationRow;

		protected UXElement panel;

		protected UXElement searchOverlay;

		protected UXSprite scrollUp;

		protected UXSprite scrollDown;

		protected SocialTabs curTab = SocialTabs.Empty;

		private int currentPlayerTileIndex;

		private float currentScrollOffset;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public AbstractLeaderboardScreen() : base("gui_leaderboards")
		{
			Service.BuildingController.CancelEditModeTimer();
			this.backButtonHelper = new BackButtonHelper(this);
			this.squadInfoView = new SquadInfoView(this);
			this.backButtonHelper.BackButtonCallBack = new Action(this.CheckBackButton);
			this.rowViews = new List<AbstractLeaderboardRowView>();
			this.tabs = new Dictionary<SocialTabs, SocialTabInfo>();
			this.InitTabInfo();
		}

		protected abstract void InitTabInfo();

		public string GetTabString()
		{
			return this.curTab.ToString();
		}

		protected SocialTabInfo GetTabInfo(SocialTabs socialTab)
		{
			return this.tabs[socialTab];
		}

		private void CheckBackButton()
		{
			if (this.backButtonHelper.IsBackButtonEnabled())
			{
				base.CurrentBackDelegate = new UXButtonClickedDelegate(this.GoBackDelagate);
				base.CurrentBackButton = this.backButtonHelper.GetBackButton();
			}
			else
			{
				base.InitDefaultBackDelegate();
			}
		}

		private void GoBackDelagate(UXButton btn)
		{
			this.backButtonHelper.GoBack();
		}

		protected override void OnScreenLoaded()
		{
			this.squadInfoView.OnScreenLoaded();
			this.createSquadItemTemplate = base.GetElement<UXElement>("SquadCreateItem");
			this.fbItemTemplate = base.GetElement<UXElement>("SquadFacebookItem");
			this.itemTemplate = base.GetElement<UXElement>("SquadItem");
			this.createSquadItemTemplate.Visible = false;
			this.fbItemTemplate.Visible = false;
			this.itemTemplate.Visible = false;
			this.searchOverlay = base.GetElement<UXElement>("ContainerSearchWindow");
			this.searchOverlay.Visible = false;
			this.scrollUp = base.GetElement<UXSprite>("SpriteLeaderboardsPanelScrollUp");
			this.scrollDown = base.GetElement<UXSprite>("SpriteLeaderboardsPanelScrollDown");
			this.titleLabel = base.GetElement<UXLabel>("LabelLeaderboards");
			this.container = base.GetElement<UXElement>("LeaderboardsContainer");
			this.container.Visible = true;
			this.navigation = base.GetElement<UXElement>("LeaderboardNavigation");
			this.navigation.Visible = true;
			this.navigationRow = base.GetElement<UXElement>("NavigationRow");
			this.navigationRow.Visible = true;
			this.panel = base.GetElement<UXElement>("LeaderboardsPanel");
			this.scrollView = NGUITools.FindInParents<UIScrollView>(this.panel.Root);
			this.InitButtons();
			List<UXElement> elements = new List<UXElement>
			{
				this.navigation,
				this.container
			};
			this.backButtonHelper.InitWithMultipleElementsLayer(elements);
			UXGrid element = base.GetElement<UXGrid>("SquadGrid");
			element.BypassLocalPositionOnAdd = true;
			element.LocalPosition = AbstractLeaderboardScreen.OFFSCREEN_GRID_POSITION;
			element.Enabled = false;
			this.InitIndividualGrids(element);
			UIPanel component = this.panel.Root.GetComponent<UIPanel>();
			Vector2 clipOffset = component.clipOffset;
			clipOffset.x = 0f;
			component.clipOffset = clipOffset;
			SocialTabInfo tabInfo = this.GetTabInfo(this.initialTab);
			tabInfo.TabButton.SetSelected(true);
			this.gridLoadHelper = tabInfo.TabGridLoadHelper;
		}

		protected abstract void InitIndividualGrids(UXGrid baseGrid);

		protected GridLoadHelper CreateGridLoadHelperByCloningGrid(UXGrid originalGrid, string itemId)
		{
			UXGrid grid = base.CloneElement<UXGrid>(originalGrid, itemId, originalGrid.Root.transform.parent.gameObject);
			GridLoadHelper result = new GridLoadHelper(new GridLoadHelper.CreateUXElementFromGridItem(this.CreateUXElementFromGridItem), grid);
			this.EnableGridLoadHelper(result, false);
			Service.EventManager.SendEvent(EventId.AllUXElementsCreated, this);
			return result;
		}

		protected void EnableGridLoadHelper(GridLoadHelper gridLoadHelper, bool isEnabled)
		{
			UXGrid grid = gridLoadHelper.GetGrid();
			grid.Enabled = isEnabled;
			grid.Visible = isEnabled;
			grid.LocalPosition = ((!isEnabled) ? AbstractLeaderboardScreen.OFFSCREEN_GRID_POSITION : Vector3.zero);
		}

		protected void ResetGrid()
		{
			this.CleanupRowViews();
			this.facebookRow = null;
			this.gridLoadHelper.ResetGrid();
		}

		protected void TabClicked(bool selected, SocialTabs clickedTab)
		{
			if (!selected)
			{
				return;
			}
			if (this.curTab != clickedTab)
			{
				this.curTab = clickedTab;
				SocialTabInfo tabInfo = this.GetTabInfo(clickedTab);
				this.DoTabClickedReset(tabInfo.TabGridLoadHelper);
				tabInfo.LoadAction();
				string cookie = (tabInfo.EventActionId == null) ? this.GetSelectedFactionString() : tabInfo.EventActionId;
				Service.EventManager.SendEvent(tabInfo.TabEventId, cookie);
				Service.UXController.MiscElementsManager.TryCloseNonFatalAlertScreen();
				this.PositionScrollViewForTop50();
				foreach (KeyValuePair<SocialTabs, SocialTabInfo> current in this.tabs)
				{
					if (current.Key == clickedTab)
					{
						current.Value.TabLabel.TextColor = UXUtils.COLOR_NAV_TAB_ENABLED;
					}
					else
					{
						current.Value.TabLabel.TextColor = UXUtils.COLOR_NAV_TAB_DISABLED;
					}
				}
			}
		}

		public void ForceSwitchTab(SocialTabs tab)
		{
			foreach (KeyValuePair<SocialTabs, SocialTabInfo> current in this.tabs)
			{
				current.Value.TabButton.Selected = false;
			}
			this.GetTabInfo(tab).TabButton.Selected = true;
			this.TabClicked(true, tab);
		}

		private void DoTabClickedReset(GridLoadHelper switchingGridLoadHelper)
		{
			ProcessingScreen.Hide();
			this.EnableGridLoadHelper(this.gridLoadHelper, false);
			this.gridLoadHelper = switchingGridLoadHelper;
			this.EnableGridLoadHelper(this.gridLoadHelper, true);
			Service.EventManager.SendEvent(EventId.SquadSelect, null);
			this.ResetGrid();
			this.RepositionGridItems();
			this.scrollView.ResetPosition();
		}

		protected void AddItemsToGrid<T>(List<T> list, bool addOverTime, bool resetGrid)
		{
			this.currentPlayerTileIndex = 0;
			if (resetGrid)
			{
				this.ResetGrid();
			}
			if (list != null && list.Count > 0)
			{
				this.CullElementList<T>(list);
				GridDataCookie gridDataCookie = new GridDataCookie(this.curTab, this.GetSelectedFaction(), this.GetSelectedPlanetId());
				if (addOverTime)
				{
					this.gridLoadHelper.StartAddingItemOverTime<T>(list, gridDataCookie, null);
				}
				else
				{
					this.gridLoadHelper.AddItems<T>(list, gridDataCookie);
				}
			}
			this.scrollView.ResetPosition();
		}

		protected void RepositionGridItems()
		{
			this.gridLoadHelper.GetGrid().RepositionItemsFrameDelayed();
		}

		private UXElement CreateUXElementFromGridItem(object itemObject, object cookie, int position)
		{
			GridDataCookie gridDataCookie = (GridDataCookie)cookie;
			SocialTabs selectedTab = gridDataCookie.SelectedTab;
			FactionToggle selectedFaction = gridDataCookie.SelectedFaction;
			string selectedPlanet = gridDataCookie.SelectedPlanet;
			AbstractLeaderboardRowView abstractLeaderboardRowView = null;
			if (itemObject is PlayerLBEntity)
			{
				abstractLeaderboardRowView = this.AddPlayerRow((PlayerLBEntity)itemObject, selectedTab, selectedFaction, selectedPlanet, position);
			}
			else if (itemObject is Squad)
			{
				abstractLeaderboardRowView = this.AddSquadRow((Squad)itemObject, selectedTab, selectedFaction, position);
			}
			else if (itemObject is SquadInvite)
			{
				abstractLeaderboardRowView = this.AddSquadInviteRow((SquadInvite)itemObject, selectedTab, selectedFaction, position);
			}
			UXElement result = null;
			if (abstractLeaderboardRowView != null)
			{
				result = abstractLeaderboardRowView.GetItem();
				this.rowViews.Add(abstractLeaderboardRowView);
			}
			return result;
		}

		private AbstractLeaderboardRowView AddPlayerRow(PlayerLBEntity player, SocialTabs tab, FactionToggle faction, string planetUid, int position)
		{
			if (player == null || string.IsNullOrEmpty(player.PlayerName))
			{
				return null;
			}
			if (!this.IsFactionToggleValidFactionType(faction, player.Faction) || position >= 51)
			{
				return null;
			}
			if (player.PlayerID == Service.CurrentPlayer.PlayerId)
			{
				this.currentPlayerTileIndex = position;
			}
			return new LeaderboardRowPlayerView(this, this.gridLoadHelper.GetGrid(), this.itemTemplate, tab, faction, position, player, planetUid);
		}

		private AbstractLeaderboardRowView AddSquadRow(Squad squad, SocialTabs tab, FactionToggle faction, int position)
		{
			if (squad == null)
			{
				return null;
			}
			if (!this.IsFactionToggleValidFactionType(faction, squad.Faction) || position >= 51)
			{
				return null;
			}
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad != null && currentSquad.SquadID == squad.SquadID)
			{
				this.currentPlayerTileIndex = position;
			}
			return new LeaderboardRowSquadView(this, this.gridLoadHelper.GetGrid(), this.itemTemplate, tab, faction, position, squad);
		}

		private AbstractLeaderboardRowView AddSquadInviteRow(SquadInvite invite, SocialTabs tab, FactionToggle faction, int position)
		{
			if (invite == null || string.IsNullOrEmpty(invite.SquadId))
			{
				return null;
			}
			Squad orCreateSquad = Service.LeaderboardController.GetOrCreateSquad(invite.SquadId);
			if (!this.IsFactionToggleValidFactionType(faction, orCreateSquad.Faction) || position >= 51)
			{
				return null;
			}
			return new LeaderboardRowSquadInviteView(this, this.gridLoadHelper.GetGrid(), this.itemTemplate, tab, faction, position, invite, orCreateSquad);
		}

		public virtual void OnRowSelected(AbstractLeaderboardRowView row)
		{
			if (this.selectedRow != null && this.selectedRow == row)
			{
				this.selectedRow.Deselect();
			}
			int i = 0;
			int count = this.rowViews.Count;
			while (i < count)
			{
				if (this.rowViews[i] != row)
				{
					this.rowViews[i].Deselect();
				}
				i++;
			}
			if (this.selectedRow != row)
			{
				this.selectedRow = row;
			}
			else
			{
				this.selectedRow = null;
			}
		}

		protected abstract FactionToggle GetSelectedFaction();

		protected string GetSelectedFactionString()
		{
			return this.GetSelectedFaction().ToString().ToLower();
		}

		protected abstract string GetSelectedPlanetId();

		private bool IsFactionToggleValidFactionType(FactionToggle selectedFaction, FactionType faction)
		{
			return selectedFaction == FactionToggle.All || (faction == FactionType.Empire && selectedFaction == FactionToggle.Empire) || (faction == FactionType.Rebel && selectedFaction == FactionToggle.Rebel);
		}

		protected void LoadFriends()
		{
			this.ResetGrid();
			ProcessingScreen.Show();
			Service.ISocialDataController.FriendsDetailsCB = new OnFBFriendsDelegate(this.OnFriendsListLoaded);
			Service.ISocialDataController.UpdateFriends();
			UXUtils.SetSpriteTopAnchorPoint(this.scrollUp, -4);
		}

		private void OnFriendsListLoaded()
		{
			if (!this.Visible)
			{
				ProcessingScreen.Hide();
				return;
			}
			this.OnPlayersListLoaded(Service.LeaderboardController.Friends.List, new Action(this.PopulateFriendsOnGrid));
		}

		private void PopulateFriendsOnGrid()
		{
			ProcessingScreen.Hide();
			if (this.curTab == SocialTabs.Friends)
			{
				this.ResetGrid();
				this.AddFBConnectItem(SocialTabs.Friends);
				this.AddItemsToGrid<PlayerLBEntity>(Service.LeaderboardController.Friends.List, true, false);
			}
		}

		protected void OnPlayersListLoaded(List<PlayerLBEntity> playerList, Action populateItemsCallback)
		{
			if (playerList != null && GameConstants.SQUAD_INVITES_ENABLED && Service.SquadController.StateManager.GetCurrentSquad() != null)
			{
				FactionType faction = Service.CurrentPlayer.Faction;
				List<string> list = new List<string>();
				int i = 0;
				int count = playerList.Count;
				while (i < count)
				{
					PlayerLBEntity playerLBEntity = playerList[i];
					if (string.IsNullOrEmpty(playerLBEntity.SquadID) && playerLBEntity.Faction == faction)
					{
						list.Add(playerLBEntity.PlayerID);
					}
					i++;
				}
				Service.SquadController.CheckSquadInvitesSentToPlayers(list, populateItemsCallback);
			}
			else
			{
				populateItemsCallback();
			}
		}

		public virtual void OnVisitClicked(UXButton button)
		{
			GameUtils.ExitEditState();
			Service.EventManager.SendEvent(EventId.SquadNext, null);
			string text = button.Tag as string;
			this.Close(text);
			Service.UXController.HUD.DestroySquadScreen();
			Service.NeighborVisitManager.VisitNeighbor(text);
		}

		public virtual void ViewSquadInfoClicked(UXButton button)
		{
			string squadId = (string)button.Tag;
			this.squadInfoView.ToggleInfoVisibility(false);
			ProcessingScreen.Show();
			Service.LeaderboardController.UpdateSquadDetails(squadId, new LeaderboardController.OnUpdateSquadData(this.OnSquadDetailsUpdated));
			this.CheckBackButton();
			Service.EventManager.SendEvent(EventId.SquadNext, null);
		}

		public void ViewSquadInfoInviteClicked(UXButton button)
		{
			SquadInvite squadInvite = (SquadInvite)button.Tag;
			Squad orCreateSquad = Service.LeaderboardController.GetOrCreateSquad(squadInvite.SquadId);
			this.DisplaySquadInfoView(orCreateSquad, false, null);
			Service.EventManager.SendEvent(EventId.SquadNext, null);
		}

		private void OnSquadDetailsUpdated(Squad squad, bool success)
		{
			ProcessingScreen.Hide();
			if (this.IsClosedOrClosing())
			{
				return;
			}
			if (success)
			{
				if (squad != null)
				{
					string detailsString;
					bool showRequestButton = SquadUtils.CanCurrentPlayerJoinSquad(Service.CurrentPlayer, Service.SquadController.StateManager.GetCurrentSquad(), squad, Service.Lang, out detailsString);
					this.DisplaySquadInfoView(squad, showRequestButton, detailsString);
				}
				else
				{
					success = false;
				}
			}
			else
			{
				string message = this.lang.Get("GENERIC_SQUAD_INFO_ISSUE", new object[0]);
				AlertScreen.ShowModal(false, null, message, null, null);
			}
		}

		private void DisplaySquadInfoView(Squad squad, bool showRequestButton, string detailsString)
		{
			this.backButtonHelper.AddLayer(this.squadInfoView.Container);
			this.squadInfoView.DisplaySquadInfo(squad, showRequestButton, detailsString);
			this.CheckBackButton();
		}

		protected void AddFBConnectItem(SocialTabs socialTab)
		{
			if (GameConstants.NO_FB_FACTION_CHOICE_ANDROID)
			{
				return;
			}
			if (Service.EnvironmentController.IsRestrictedProfile())
			{
				return;
			}
			if (this.facebookRow != null)
			{
				return;
			}
			this.facebookRow = new LeaderboardRowFacebookView(this, this.gridLoadHelper.GetGrid(), this.fbItemTemplate, this.curTab);
			this.rowViews.Add(this.facebookRow);
			this.gridLoadHelper.AddElement(this.facebookRow.GetItem());
			this.RepositionGridItems();
			this.scrollView.ResetPosition();
		}

		public void OnFacebookLoggedIn()
		{
			if (this.curTab == SocialTabs.Friends)
			{
				this.CancelLoaderAndResetGrid();
				this.LoadFriends();
			}
			Service.EventManager.SendEvent(EventId.FacebookLoggedIn, "UI_leaderboard_friends");
		}

		private void CullElementList<T>(List<T> elementList)
		{
			if (elementList.Count >= 51)
			{
				int num = elementList.Count - 51 + 1;
				elementList.RemoveRange(elementList.Count - num, num);
			}
		}

		protected void PositionScrollViewForFindMe()
		{
			if (this.currentPlayerTileIndex > 0)
			{
				this.currentPlayerTileIndex--;
				UXGrid grid = this.gridLoadHelper.GetGrid();
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				this.currentScrollOffset = (float)this.currentPlayerTileIndex * (grid.CellHeight / this.uxCamera.Scale);
			}
		}

		protected void PositionScrollViewForTop50()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.currentScrollOffset = 0f;
		}

		public void OnViewFrameTime(float dt)
		{
			this.currentScrollOffset -= 30f;
			if (this.currentScrollOffset > 0f)
			{
				this.scrollView.MoveRelative(new Vector3(0f, 30f));
			}
			else
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		protected void DestroyGridLoadHelper(GridLoadHelper tmpGridLoadHelper, bool isDestroyGrid)
		{
			if (tmpGridLoadHelper != null)
			{
				tmpGridLoadHelper.OnDestroyElement();
				if (isDestroyGrid)
				{
					base.DestroyElement(tmpGridLoadHelper.GetGrid());
				}
			}
		}

		protected void CancelLoaderAndResetGrid()
		{
			ProcessingScreen.Hide();
			this.ResetGrid();
			this.RepositionGridItems();
			this.scrollView.ResetPosition();
		}

		public void RemoveAndDestroyRow(AbstractLeaderboardRowView rowView)
		{
			UXElement item = rowView.GetItem();
			this.gridLoadHelper.RemoveElement(item);
			base.DestroyElement(item);
			rowView.Destroy();
			this.rowViews.Remove(rowView);
		}

		private void CleanupRowViews()
		{
			int i = 0;
			int count = this.rowViews.Count;
			while (i < count)
			{
				this.rowViews[i].Destroy();
				i++;
			}
			this.rowViews.Clear();
		}

		public override void Close(object modalResult)
		{
			this.curTab = SocialTabs.Empty;
			base.Close(modalResult);
		}

		private bool IsClosedOrClosing()
		{
			return !this.Visible || this.curTab == SocialTabs.Empty;
		}

		public override void OnDestroyElement()
		{
			this.CleanupRowViews();
			this.squadInfoView.Destroy();
			foreach (KeyValuePair<SocialTabs, SocialTabInfo> current in this.tabs)
			{
				this.DestroyGridLoadHelper(current.Value.TabGridLoadHelper, true);
			}
			base.OnDestroyElement();
		}
	}
}
