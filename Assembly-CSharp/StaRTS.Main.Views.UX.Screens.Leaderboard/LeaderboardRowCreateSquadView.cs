using StaRTS.Main.Models;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public class LeaderboardRowCreateSquadView : AbstractLeaderboardRowView
	{
		private const string CREATE_BUTTON = "BtnConnect";

		private const string CREATE_BUTTON_LABEL = "LabelBtnConnect";

		private const string CREATE_DESC_LABEL = "LabelConnectFacebook";

		private const string CREATE_ROW_ICON = "SpriteFacebookIcon";

		private const string PREFIX = "create_squad_";

		private const string SQUAD_CREATE = "SQUAD_CREATE";

		private const string SQUAD_CREATE_DESC = "SQUAD_CREATE_DESC";

		private const string IN_WAR_CANT_LEAVE_SQUAD = "IN_WAR_CANT_LEAVE_SQUAD";

		public LeaderboardRowCreateSquadView(AbstractLeaderboardScreen screen, UXGrid grid, UXElement templateItem, int position) : base(screen, grid, templateItem, SocialTabs.Empty, FactionToggle.All, position, false)
		{
			this.InitView();
		}

		protected override void CreateItem()
		{
			this.id = string.Format("{0}{1}", "create_squad_", this.position);
			this.item = this.grid.CloneItem(this.id, this.templateItem);
		}

		protected void InitView()
		{
			UXButton subElement = this.grid.GetSubElement<UXButton>(this.id, "BtnConnect");
			subElement.OnClicked = new UXButtonClickedDelegate(this.OnCreateSquadClicked);
			UXLabel subElement2 = this.grid.GetSubElement<UXLabel>(this.id, "LabelBtnConnect");
			subElement2.Text = Service.Lang.Get("SQUAD_CREATE", new object[0]);
			UXLabel subElement3 = this.grid.GetSubElement<UXLabel>(this.id, "LabelConnectFacebook");
			subElement3.Text = Service.Lang.Get("SQUAD_CREATE_DESC", new object[0]);
			UXSprite subElement4 = this.grid.GetSubElement<UXSprite>(this.id, "SpriteFacebookIcon");
			FactionType faction = Service.CurrentPlayer.Faction;
			if (faction != FactionType.Empire)
			{
				if (faction == FactionType.Rebel)
				{
					subElement4.SpriteName = "FactionRebel";
				}
			}
			else
			{
				subElement4.SpriteName = "FactionEmpire";
			}
		}

		private void OnCreateSquadClicked(UXButton button)
		{
			if (!SquadUtils.CanLeaveSquad())
			{
				string message = Service.Lang.Get("IN_WAR_CANT_LEAVE_SQUAD", new object[0]);
				AlertScreen.ShowModal(false, null, message, null, null, true);
			}
			else
			{
				Service.ScreenController.AddScreen(new SquadCreateScreen(true));
			}
			this.screen.Close(null);
		}

		public override void Destroy()
		{
		}
	}
}
