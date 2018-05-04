using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public class LeaderboardRowFacebookView : AbstractLeaderboardRowView
	{
		private const string SPRITE = "SpriteFacebookIcon";

		private const string LABEL = "LabelConnectFacebook";

		private const string BUTTON = "BtnConnect";

		private const string BUTTON_LABEL = "LabelBtnConnect";

		private const string FACEBOOK_PREFIX = "facebook_";

		private const string CONNECT_FB_DESC = "CONNECT_FB_DESC";

		private const string CONNECT_FB_INCENTIVE_DESC = "CONNECT_FB_INCENTIVE_DESC";

		private const string SETTINGS_NOTCONNECTED = "SETTINGS_NOTCONNECTED";

		private const string INVITE_FB_DESC = "INVITE_FB_DESC";

		private const string INVITE_FRIENDS = "INVITE_FRIENDS";

		private UXSprite sprite;

		private UXLabel label;

		private UXButton button;

		private UXLabel buttonLabel;

		public LeaderboardRowFacebookView(AbstractLeaderboardScreen screen, UXGrid grid, UXElement templateItem, SocialTabs tab) : base(screen, grid, templateItem, tab, FactionToggle.All, 0, false)
		{
			this.InitView();
		}

		protected override void CreateItem()
		{
			this.id = string.Format("{0}{1}", "facebook_", this.tab.ToString());
			this.item = this.grid.CloneItem(this.id, this.templateItem);
		}

		private void InitView()
		{
			Lang lang = Service.Lang;
			this.sprite = this.grid.GetSubElement<UXSprite>(this.id, "SpriteFacebookIcon");
			this.label = this.grid.GetSubElement<UXLabel>(this.id, "LabelConnectFacebook");
			this.button = this.grid.GetSubElement<UXButton>(this.id, "BtnConnect");
			this.buttonLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelBtnConnect");
			if (!Service.ISocialDataController.IsLoggedIn)
			{
				string text;
				if (Service.CurrentPlayer.IsConnectedAccount)
				{
					text = lang.Get("CONNECT_FB_DESC", new object[0]);
				}
				else
				{
					text = lang.Get("CONNECT_FB_INCENTIVE_DESC", new object[]
					{
						GameConstants.FB_CONNECT_REWARD
					});
				}
				this.label.Text = text;
				this.buttonLabel.Text = lang.Get("SETTINGS_NOTCONNECTED", new object[0]);
				this.button.OnClicked = new UXButtonClickedDelegate(this.ConnectToFacebook);
			}
			else if (GameConstants.FACEBOOK_INVITES_ENABLED)
			{
				this.label.Text = lang.Get("INVITE_FB_DESC", new object[0]);
				this.buttonLabel.Text = lang.Get("INVITE_FRIENDS", new object[0]);
				this.button.OnClicked = new UXButtonClickedDelegate(this.FacebookInviteFriends);
			}
		}

		public void Hide()
		{
			this.sprite.Visible = false;
			this.label.Visible = false;
			this.button.Visible = false;
		}

		private void ConnectToFacebook(UXButton btn)
		{
			Service.EventManager.SendEvent(EventId.SquadFB, null);
			Service.ISocialDataController.Login(new OnAllDataFetchedDelegate(this.screen.OnFacebookLoggedIn));
		}

		private void FacebookInviteFriends(UXButton btn)
		{
			Service.EventManager.SendEvent(EventId.SquadFB, null);
			Service.ISocialDataController.InviteFriends(null);
		}

		public override void Destroy()
		{
		}
	}
}
