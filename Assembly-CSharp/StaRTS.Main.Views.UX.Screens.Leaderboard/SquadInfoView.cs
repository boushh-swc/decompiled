using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public class SquadInfoView
	{
		private const string SQUAD_INFO_CONTAINER = "ContainerSquadInfo";

		private const string SQUAD_INFO_GRID = "SquadInfoGrid";

		private const string SQUAD_INFO_TEMPLATE_ITEM = "SquadInfoItem";

		private const string SI_SQUAD_INFO_DETAILS_LABEL = "LabelSquadUserDetails";

		private const string SI_JOIN_SQUAD_BTN = "BtnJoinSquad";

		private const string SI_JOIN_SQUAD_LABEL = "LabelBtnJoinSquad";

		private const string SI_SQUAD_RANK_LABEL = "LabelSquadRankTop";

		private const string SI_SQUAD_SCORE_LABEL = "LabelSquadScoreTop";

		private const string SI_SQUAD_DESC_LABEL = "LabelSquadInfoDescription";

		private const string SI_SQUAD_TO_JOIN_LABEL = "LabelSquadInfoJoin";

		private const string SI_SQUAD_NAME_LABEL = "LabelSquadNameTop";

		private const string SI_SQUAD_SPRITE = "SpriteSquadIcon";

		private const string SI_TEMP_VISIT_BTN = "BtnVisit";

		private const string SI_TEMP_VISIT_BTN_LABEL = "LabelBtnVisit";

		private const string SI_USER_NAME_LABEL = "LabelSquadInfoMemberName";

		private const string SI_USER_RANK_LABEL = "LabelSquadInfoMemberRank";

		private const string SI_USER_SCORE_LABEL = "LabelSquadInfoMemberScore";

		private const string SI_USER_ROLE_LABEL = "LabelSquadInfoMemberType";

		private const string SI_TROOPS_DONATED_LABEL = "LabelSquadInfoTroopsDonated";

		private const string SI_TROOPS_RECEIVED_LABEL = "LabelSquadInfoTroopsReceived";

		private const string SQUAD_RANK = "SQUAD_RANK";

		private const string SQUAD_BATTLESCORE = "SQUAD_BATTLESCORE";

		private const string JOIN = "JOIN";

		private const string APPLY = "APPLY";

		private const string APPLY_TO_JOIN = "APPLY_TO_JOIN";

		private const string VISIT = "s_Visit";

		private const string TROOPS_DONATED = "TROOPS_DONATED";

		private const string TROOPS_RECEIVED = "TROOPS_RECEIVED";

		private const string MEMBER_PREFIX = "member_";

		private AbstractLeaderboardScreen screen;

		private SquadJoinActionModule joinModule;

		private UXGrid squadMemberGrid;

		private UXElement squadInfoOverlay;

		private UXLabel squadNameLabel;

		private UXLabel squadRankLabel;

		private UXLabel squadScoreLabel;

		private UXLabel squadDescLabel;

		private UXLabel squadToJoinLabel;

		private UXButton requestBtn;

		private UXLabel requestBtnLabel;

		private UXSprite squadSymbolSprite;

		private UXLabel squadDetailsAlertLabel;

		public UXElement Container
		{
			get
			{
				return this.squadInfoOverlay;
			}
		}

		public SquadInfoView(AbstractLeaderboardScreen screen)
		{
			this.screen = screen;
		}

		public void OnScreenLoaded()
		{
			this.squadInfoOverlay = this.screen.GetElement<UXElement>("ContainerSquadInfo");
			this.squadInfoOverlay.Visible = false;
			this.squadDetailsAlertLabel = this.screen.GetElement<UXLabel>("LabelSquadUserDetails");
			this.squadDetailsAlertLabel.Visible = true;
			this.squadMemberGrid = this.screen.GetElement<UXGrid>("SquadInfoGrid");
			this.squadMemberGrid.SetTemplateItem("SquadInfoItem");
			this.requestBtn = this.screen.GetElement<UXButton>("BtnJoinSquad");
			this.requestBtnLabel = this.screen.GetElement<UXLabel>("LabelBtnJoinSquad");
			this.joinModule = new SquadJoinActionModule(null, this.screen, this.requestBtn);
			this.requestBtn.OnClicked = new UXButtonClickedDelegate(this.OnJoinClicked);
			this.squadNameLabel = this.screen.GetElement<UXLabel>("LabelSquadNameTop");
			this.squadRankLabel = this.screen.GetElement<UXLabel>("LabelSquadRankTop");
			this.squadScoreLabel = this.screen.GetElement<UXLabel>("LabelSquadScoreTop");
			this.squadDescLabel = this.screen.GetElement<UXLabel>("LabelSquadInfoDescription");
			this.squadToJoinLabel = this.screen.GetElement<UXLabel>("LabelSquadInfoJoin");
			this.squadSymbolSprite = this.screen.GetElement<UXSprite>("SpriteSquadIcon");
		}

		public void ToggleInfoVisibility(bool visible)
		{
			this.squadSymbolSprite.Visible = visible;
			this.squadNameLabel.Visible = visible;
			this.squadDescLabel.Visible = visible;
			this.squadRankLabel.Visible = visible;
			this.squadScoreLabel.Visible = visible;
			this.squadDetailsAlertLabel.Visible = visible;
			this.squadToJoinLabel.Visible = visible;
			this.requestBtn.Visible = visible;
			this.squadMemberGrid.Visible = visible;
		}

		public void DisplaySquadInfo(Squad squad, bool isJoinButtonVisible, string alertLabel)
		{
			Lang lang = Service.Lang;
			this.squadMemberGrid.Clear();
			this.ToggleInfoVisibility(true);
			this.squadSymbolSprite.SpriteName = squad.Symbol;
			this.squadNameLabel.Text = squad.SquadName;
			this.squadDescLabel.Text = squad.Description;
			this.squadRankLabel.Text = lang.Get("SQUAD_RANK", new object[]
			{
				lang.ThousandsSeparated(squad.Rank)
			});
			this.squadScoreLabel.Text = lang.Get("SQUAD_BATTLESCORE", new object[0]) + lang.ThousandsSeparated(squad.BattleScore);
			if (squad.InviteType == 1)
			{
				this.requestBtnLabel.Text = lang.Get("JOIN", new object[0]);
				this.squadToJoinLabel.Text = string.Empty;
				this.requestBtn.Enabled = true;
			}
			else
			{
				this.requestBtnLabel.Text = lang.Get("APPLY", new object[0]);
				this.squadToJoinLabel.Text = lang.Get("APPLY_TO_JOIN", new object[0]);
				SquadStateManager stateManager = Service.SquadController.StateManager;
				this.requestBtn.Enabled = !stateManager.SquadJoinRequestsPending.Contains(squad.SquadID);
			}
			this.squadDetailsAlertLabel.Visible = true;
			this.squadDetailsAlertLabel.Text = alertLabel;
			this.requestBtn.Visible = isJoinButtonVisible;
			this.squadToJoinLabel.Visible = isJoinButtonVisible;
			this.joinModule.SetSquad(squad);
			int i = 0;
			int count = squad.MemberList.Count;
			while (i < count)
			{
				if (squad.MemberList[i] != null)
				{
					this.AddSquadMember(squad.MemberList[i]);
				}
				i++;
			}
			this.squadMemberGrid.RepositionItemsFrameDelayed();
		}

		private void AddSquadMember(SquadMember member)
		{
			int num = this.squadMemberGrid.Count + 1;
			string itemUid = "member_" + num;
			UXElement item = this.squadMemberGrid.CloneTemplateItem(itemUid);
			Lang lang = Service.Lang;
			UXButton subElement = this.squadMemberGrid.GetSubElement<UXButton>(itemUid, "BtnVisit");
			if (member.MemberID != Service.CurrentPlayer.PlayerId)
			{
				subElement.OnClicked = new UXButtonClickedDelegate(this.screen.OnVisitClicked);
				subElement.Tag = member.MemberID;
				UXLabel subElement2 = this.squadMemberGrid.GetSubElement<UXLabel>(itemUid, "LabelBtnVisit");
				subElement2.Text = lang.Get("s_Visit", new object[0]);
			}
			else
			{
				subElement.Visible = false;
			}
			UXLabel subElement3 = this.squadMemberGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadInfoMemberName");
			subElement3.Text = member.MemberName;
			UXLabel subElement4 = this.squadMemberGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadInfoMemberRank");
			subElement4.Text = num.ToString();
			UXLabel subElement5 = this.squadMemberGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadInfoMemberType");
			subElement5.Text = LangUtils.GetSquadRoleDisplayName(member.Role);
			UXLabel subElement6 = this.squadMemberGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadInfoMemberScore");
			subElement6.Text = lang.ThousandsSeparated(member.Score);
			UXLabel subElement7 = this.squadMemberGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadInfoTroopsDonated");
			subElement7.Text = lang.Get("TROOPS_DONATED", new object[]
			{
				lang.ThousandsSeparated(member.TroopsDonated)
			});
			UXLabel subElement8 = this.squadMemberGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadInfoTroopsReceived");
			subElement8.Text = lang.Get("TROOPS_RECEIVED", new object[]
			{
				lang.ThousandsSeparated(member.TroopsReceived)
			});
			this.squadMemberGrid.AddItem(item, num);
		}

		private void OnJoinClicked(UXButton button)
		{
			this.joinModule.JoinSquad(this.screen.GetTabString());
		}

		public void Destroy()
		{
			if (this.squadMemberGrid != null)
			{
				this.squadMemberGrid.Clear();
			}
		}
	}
}
