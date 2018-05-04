using Facebook.Unity;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Leaderboard;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public class LeaderboardRowPlayerView : AbstractLeaderboardRowView
	{
		private const string FRIENDS_PREFIX = "friend_";

		private const string PLAYER_PREFIX = "player_";

		private const string TOURNAMENT_PLAYER_PREFIX = "tournament_player_";

		private const string LEADERBOARD_YOUR_NAME = "LEADERBOARD_YOUR_NAME";

		private const string LEADERBOARD_PLAYER_NAME = "LEADERBOARD_PLAYER_NAME";

		private const string PLAYER_NAME_AND_REAL_NAME = "PLAYER_NAME_AND_REAL_NAME";

		private const string VISIT = "s_Visit";

		private const string SQUAD = "s_Squad";

		private const string SQUAD_INVITE_SENT = "SQUAD_INVITE_SENT";

		private const string SQUAD_INVITE = "SQUAD_INVITE";

		private const string SQUAD_INVITE_SUCCESS = "SQUAD_INVITE_SUCCESS";

		private const string ATTACKS_WON = "ATTACKS_WON";

		private const string DEFENSES_WON = "DEFENSES_WON";

		private PlayerLBEntity player;

		private string planetUid;

		private Texture2D facebookPhotoTexture;

		private bool destroyed;

		public LeaderboardRowPlayerView(AbstractLeaderboardScreen screen, UXGrid grid, UXElement templateItem, SocialTabs tab, FactionToggle faction, int position, PlayerLBEntity player, string planetUid) : base(screen, grid, templateItem, tab, faction, position, true)
		{
			this.player = player;
			this.planetUid = planetUid;
			this.InitView();
		}

		protected override void CreateItem()
		{
			string playerElementPrefix = this.GetPlayerElementPrefix(this.tab);
			this.id = string.Format("{0}{1}", playerElementPrefix, this.position);
			this.item = this.grid.CloneItem(this.id, this.templateItem);
		}

		private void InitView()
		{
			if (this.player.PlayerID == Service.CurrentPlayer.PlayerId)
			{
				this.nameLabel.Text = Service.Lang.Get("LEADERBOARD_YOUR_NAME", new object[]
				{
					this.player.PlayerName
				});
				this.arrowSprite.Visible = false;
				base.ToggleHighlight(true);
			}
			else
			{
				this.nameLabel.Text = Service.Lang.Get("LEADERBOARD_PLAYER_NAME", new object[]
				{
					this.player.PlayerName
				});
				base.ToggleHighlight(false);
			}
			this.typeLabel.Text = this.player.SquadName;
			this.squadSymbolSprite.SpriteName = this.player.Symbol;
			this.rankLabel.Text = this.player.Rank.ToString();
			this.memberNumberLabel.Visible = false;
			this.activeMemberNumberLabel.Visible = false;
			this.squadLevelGroup.Visible = false;
			LeaderboardBattleHistory leaderboardBattleHistory = null;
			this.InitButtons(this.id);
			this.InitPlanetStats(this.id);
			this.InitBattleStats(this.id, out leaderboardBattleHistory);
			int rating = (leaderboardBattleHistory == null) ? 0 : GameUtils.CalculateBattleHistoryVictoryRating(leaderboardBattleHistory);
			this.InitIcons(this.id, this.player.Faction, rating);
			this.InitFacebookData();
		}

		private void InitButtons(string id)
		{
			this.primaryButtonLabel.Text = Service.Lang.Get("s_Visit", new object[0]);
			this.primaryButton.OnClicked = new UXButtonClickedDelegate(this.screen.OnVisitClicked);
			this.primaryButton.Tag = this.player.PlayerID;
			if (this.player.PlayerID == Service.CurrentPlayer.PlayerId)
			{
				this.buttonContainer.Visible = false;
			}
			else
			{
				SquadStateManager stateManager = Service.SquadController.StateManager;
				Squad currentSquad = stateManager.GetCurrentSquad();
				bool flag = currentSquad != null && currentSquad.SquadID == this.player.SquadID;
				if (this.CanPlayerBeInvitedToJoinSquad())
				{
					bool flag2 = stateManager.PlayersInvitedToSquad.Contains(this.player.PlayerID);
					this.secondaryButtonLabel.Text = ((!flag2) ? Service.Lang.Get("SQUAD_INVITE", new object[0]) : Service.Lang.Get("SQUAD_INVITE_SENT", new object[0]));
					this.secondaryButton.Enabled = !flag2;
					this.secondaryButton.OnClicked = new UXButtonClickedDelegate(this.OnInviteToSquadClicked);
					this.secondaryButton.Visible = true;
				}
				else if (!string.IsNullOrEmpty(this.player.SquadID) && !flag)
				{
					this.secondaryButtonLabel.Text = Service.Lang.Get("s_Squad", new object[0]);
					this.secondaryButton.OnClicked = new UXButtonClickedDelegate(this.screen.ViewSquadInfoClicked);
					this.secondaryButton.Tag = this.player.SquadID;
					this.secondaryButton.Visible = true;
				}
				else
				{
					this.secondaryButton.Visible = false;
					base.UpdateButtonContainerTween(this.buttonContainer, 1);
				}
			}
		}

		private bool CanPlayerBeInvitedToJoinSquad()
		{
			if (!GameConstants.SQUAD_INVITES_ENABLED)
			{
				return false;
			}
			if (this.player.Faction != Service.CurrentPlayer.Faction)
			{
				return false;
			}
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			return currentSquad != null && string.IsNullOrEmpty(this.player.SquadID) && (this.tab == SocialTabs.Friends || GameConstants.SQUAD_INVITES_TO_LEADERS_ENABLED);
		}

		private void InitPlanetStats(string id)
		{
			PlanetVO optional = Service.StaticDataController.GetOptional<PlanetVO>(this.player.Planet);
			if (optional != null && this.tab != SocialTabs.Tournament)
			{
				this.planetLabel.Text = LangUtils.GetPlanetDisplayName(optional);
				this.planetBgTexture.LoadTexture(optional.LeaderboardTileTexture);
			}
			else
			{
				this.planetLabel.Visible = false;
				this.planetBgTexture.Visible = false;
			}
		}

		private void InitBattleStats(string id, out LeaderboardBattleHistory battleHistory)
		{
			battleHistory = null;
			bool flag = this.tab == SocialTabs.Tournament;
			this.medalGroup.Visible = !flag;
			this.tournamentMedalGroup.Visible = flag;
			if (flag)
			{
				if (!string.IsNullOrEmpty(this.planetUid))
				{
					this.tournamentMedalLabel.Text = Service.Lang.ThousandsSeparated(this.player.BattleScore);
					this.tournamentMedalSprite.SpriteName = GameUtils.GetTournamentPointIconName(this.planetUid);
				}
				else
				{
					this.tournamentMedalGroup.Visible = false;
				}
				if (this.player.TournamentBattleHistory != null && !string.IsNullOrEmpty(this.planetUid))
				{
					TournamentVO activeTournamentOnPlanet = TournamentController.GetActiveTournamentOnPlanet(this.planetUid);
					if (activeTournamentOnPlanet != null)
					{
						this.player.TournamentBattleHistory.TryGetValue(activeTournamentOnPlanet.Uid, out battleHistory);
					}
				}
			}
			else
			{
				this.medalLabel.Text = Service.Lang.ThousandsSeparated(this.player.BattleScore);
				battleHistory = this.player.BattleHistory;
			}
			int num = (battleHistory == null) ? 0 : battleHistory.AttacksWon;
			int num2 = (battleHistory == null) ? 0 : battleHistory.DefensesWon;
			this.attacksLabel.Text = Service.Lang.Get("ATTACKS_WON", new object[]
			{
				num
			});
			this.defensesLabel.Text = Service.Lang.Get("DEFENSES_WON", new object[]
			{
				num2
			});
		}

		private void InitIcons(string id, FactionType faction, int rating)
		{
			bool flag = Service.FactionIconUpgradeController.UseUpgradeImage(rating);
			this.squadFactionSprite.Visible = !flag;
			this.playerFactionSprite.Visible = flag;
			if (flag)
			{
				this.playerFactionSprite.SpriteName = Service.FactionIconUpgradeController.GetIcon(faction, rating);
			}
			else if (faction == FactionType.Empire)
			{
				this.squadFactionSprite.SpriteName = "FactionEmpire";
			}
			else if (faction == FactionType.Rebel)
			{
				this.squadFactionSprite.SpriteName = "FactionRebel";
			}
		}

		private void InitFacebookData()
		{
			this.friendTexture.Visible = false;
			ISocialDataController iSocialDataController = Service.ISocialDataController;
			Dictionary<string, SocialFriendData> playerIdToFriendData = iSocialDataController.PlayerIdToFriendData;
			SocialFriendData socialFriendData = null;
			if (this.player.PlayerID == Service.CurrentPlayer.PlayerId && iSocialDataController.FacebookId != null)
			{
				iSocialDataController.GetSelfPicture(new OnGetProfilePicture(this.OnGetProfilePicture), this.friendTexture);
			}
			else if (playerIdToFriendData != null && playerIdToFriendData.TryGetValue(this.player.PlayerID, out socialFriendData))
			{
				if (!string.IsNullOrEmpty(socialFriendData.Name))
				{
					string[] array = socialFriendData.Name.Split(new char[]
					{
						' '
					});
					this.nameLabel.Text = Service.Lang.Get("PLAYER_NAME_AND_REAL_NAME", new object[]
					{
						this.player.PlayerName,
						array[0]
					});
				}
				if (this.tab == SocialTabs.Friends)
				{
					iSocialDataController.GetFriendPicture(socialFriendData, new OnGetProfilePicture(this.OnGetProfilePicture), this.friendTexture);
				}
			}
		}

		private void OnGetProfilePicture(Texture2D tex, object cookie)
		{
			if (this.destroyed)
			{
				Service.ISocialDataController.DestroyFriendPicture(tex);
				return;
			}
			this.facebookPhotoTexture = tex;
			this.friendTexture.MainTexture = tex;
			this.friendTexture.Visible = true;
			this.squadSymbolSprite.Visible = false;
		}

		public override void Destroy()
		{
			if (this.facebookPhotoTexture != null)
			{
				Service.ISocialDataController.DestroyFriendPicture(this.facebookPhotoTexture);
				this.facebookPhotoTexture = null;
			}
			this.destroyed = true;
		}

		private string GetPlayerElementPrefix(SocialTabs selectedTab)
		{
			string result;
			if (selectedTab != SocialTabs.Friends)
			{
				if (selectedTab != SocialTabs.Tournament)
				{
					result = "player_";
				}
				else
				{
					result = "tournament_player_";
				}
			}
			else
			{
				result = "friend_";
			}
			return result;
		}

		private void OnInviteToSquadClicked(UXButton button)
		{
			this.secondaryButton.Enabled = false;
			string fbFriendId = null;
			string fbAccessToken = null;
			Dictionary<string, SocialFriendData> playerIdToFriendData = Service.ISocialDataController.PlayerIdToFriendData;
			SocialFriendData socialFriendData = null;
			if (playerIdToFriendData != null && playerIdToFriendData.TryGetValue(this.player.PlayerID, out socialFriendData))
			{
				fbFriendId = socialFriendData.Id;
				fbAccessToken = AccessToken.CurrentAccessToken.TokenString;
			}
			SquadMsg message = SquadMsgUtils.CreateSendInviteMessage(this.player.PlayerID, fbFriendId, fbAccessToken, new SquadController.ActionCallback(this.OnInviteToSquadComplete), null);
			Service.SquadController.TakeAction(message);
			Service.EventManager.SendEvent(EventId.SquadNext, null);
			ProcessingScreen.Show();
		}

		private void OnInviteToSquadComplete(bool success, object cookie)
		{
			ProcessingScreen.Hide();
			if (success)
			{
				string message = Service.Lang.Get("SQUAD_INVITE_SUCCESS", new object[0]);
				AlertScreen.ShowModal(false, null, message, null, null);
				this.secondaryButtonLabel.Text = Service.Lang.Get("SQUAD_INVITE_SENT", new object[0]);
				Service.SquadController.StateManager.PlayersInvitedToSquad.Add(this.player.PlayerID);
			}
			else
			{
				this.secondaryButton.Enabled = true;
			}
		}
	}
}
