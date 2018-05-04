using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public class LeaderboardRowSquadView : AbstractLeaderboardRowView
	{
		protected const string SQUAD_PREFIX = "squad_";

		private const string SQUAD_INVITE_PREFIX = "squad_invite_";

		private const string SQUAD_SEARCH_PREFIX = "squad_search_";

		private const string LEADERBOARD_PLAYER_NAME = "LEADERBOARD_PLAYER_NAME";

		private const string JOIN = "JOIN";

		private const string APPLY = "APPLY";

		private const string SQUAD_OPEN_TO_ALL = "SQUAD_OPEN_TO_ALL";

		private const string SQUAD_INVITE_ONLY = "SQUAD_INVITE_ONLY";

		private const string INFO = "context_Info";

		private const string SQUAD_MEMBERS = "SQUAD_MEMBERS";

		private const string SQUAD_ACTIVE_MEMBERS = "SQUAD_ACTIVE_MEMBERS";

		private const string FRACTION = "FRACTION";

		protected Squad squad;

		protected SquadJoinActionModule joinModule;

		public LeaderboardRowSquadView(AbstractLeaderboardScreen screen, UXGrid grid, UXElement templateItem, SocialTabs tab, FactionToggle faction, int position, Squad squad) : base(screen, grid, templateItem, tab, faction, position, true)
		{
			this.squad = squad;
			this.InitBaseView();
			this.InitFullView();
		}

		protected override void CreateItem()
		{
			string squadElementPrefix = this.GetSquadElementPrefix(this.tab);
			this.id = string.Format("{0}{1}", squadElementPrefix, this.position);
			this.item = this.grid.CloneItem(this.id, this.templateItem);
		}

		public override void Destroy()
		{
			this.joinModule = null;
		}

		protected void InitBaseView()
		{
			FactionType faction = this.squad.Faction;
			if (faction != FactionType.Empire)
			{
				if (faction != FactionType.Rebel)
				{
					this.squadFactionSprite.Visible = false;
				}
				else
				{
					this.squadFactionSprite.SpriteName = "FactionRebel";
					this.squadFactionSprite.Visible = true;
				}
			}
			else
			{
				this.squadFactionSprite.SpriteName = "FactionEmpire";
				this.squadFactionSprite.Visible = true;
			}
			this.planetLabel.Visible = false;
			this.nameLabel.Text = Service.Lang.Get("LEADERBOARD_PLAYER_NAME", new object[]
			{
				this.squad.SquadName
			});
			this.rankLabel.Text = this.squad.Rank.ToString();
			this.medalLabel.Text = Service.Lang.ThousandsSeparated(this.squad.BattleScore);
			this.medalGroup.Visible = true;
			this.tournamentMedalGroup.Visible = false;
			this.attacksLabel.Visible = false;
			this.defensesLabel.Visible = false;
			this.squadSymbolSprite.SpriteName = this.squad.Symbol;
		}

		private void InitFullView()
		{
			Lang lang = Service.Lang;
			string text;
			if (!SquadUtils.CanCurrentPlayerJoinSquad(Service.CurrentPlayer, Service.SquadController.StateManager.GetCurrentSquad(), this.squad, lang, out text))
			{
				this.primaryButton.VisuallyDisableButton();
			}
			else if (this.squad.InviteType == 0 && Service.SquadController.StateManager.SquadJoinRequestsPending.Contains(this.squad.SquadID))
			{
				this.primaryButton.Enabled = false;
			}
			this.primaryButton.OnClicked = new UXButtonClickedDelegate(this.OnJoinClicked);
			this.joinModule = new SquadJoinActionModule(this.squad, this.screen, this.primaryButton);
			this.secondaryButton.OnClicked = new UXButtonClickedDelegate(this.screen.ViewSquadInfoClicked);
			this.secondaryButton.Tag = this.squad.SquadID;
			this.secondaryButton.Visible = true;
			this.secondaryButtonLabel.Text = lang.Get("context_Info", new object[0]);
			if (this.squad.InviteType == 1)
			{
				this.primaryButtonLabel.Text = lang.Get("JOIN", new object[0]);
				this.typeLabel.Text = lang.Get("SQUAD_OPEN_TO_ALL", new object[0]);
			}
			else
			{
				this.primaryButtonLabel.Text = lang.Get("APPLY", new object[0]);
				this.typeLabel.Text = lang.Get("SQUAD_INVITE_ONLY", new object[0]);
			}
			this.playerFactionSprite.Visible = false;
			this.memberNumberLabel.Text = lang.Get("SQUAD_MEMBERS", new object[0]) + " " + lang.Get("FRACTION", new object[]
			{
				this.squad.MemberCount,
				this.squad.MemberMax
			});
			this.activeMemberNumberLabel.Text = lang.Get("SQUAD_ACTIVE_MEMBERS", new object[0]) + " " + lang.Get("FRACTION", new object[]
			{
				this.squad.ActiveMemberCount,
				this.squad.MemberMax
			});
			this.squadLevelGroup.Visible = true;
			this.squadLevelLabel.Text = this.squad.Level.ToString();
			this.friendTexture.Visible = false;
			this.planetLabel.Visible = false;
			this.planetBgTexture.Visible = false;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			base.ToggleHighlight(currentSquad != null && currentSquad.SquadID == this.squad.SquadID);
		}

		private void OnJoinClicked(UXButton button)
		{
			if (this.tab == SocialTabs.Squads)
			{
				Service.EventManager.SendEvent(EventId.UILeaderboardJoin, "squad");
			}
			this.joinModule.JoinSquad(this.screen.GetTabString());
		}

		private string GetSquadElementPrefix(SocialTabs selectedTab)
		{
			string result;
			if (selectedTab != SocialTabs.Search)
			{
				if (selectedTab != SocialTabs.Invites)
				{
					result = "squad_";
				}
				else
				{
					result = "squad_invite_";
				}
			}
			else
			{
				result = "squad_search_";
			}
			return result;
		}
	}
}
