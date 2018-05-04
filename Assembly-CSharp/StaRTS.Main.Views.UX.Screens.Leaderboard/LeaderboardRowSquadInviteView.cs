using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public class LeaderboardRowSquadInviteView : LeaderboardRowSquadView
	{
		private const string SQUAD_INVITED_BY = "SQUAD_INVITED_BY";

		private const string BUTTON_ACCEPT = "BUTTON_ACCEPT";

		private const string BUTTON_DECLINE = "BUTTON_DECLINE";

		private const string INFO = "context_Info";

		private SquadInvite invite;

		public LeaderboardRowSquadInviteView(AbstractLeaderboardScreen screen, UXGrid grid, UXElement templateItem, SocialTabs tab, FactionToggle faction, int position, SquadInvite invite, Squad squad) : base(screen, grid, templateItem, tab, faction, position, squad)
		{
			this.invite = invite;
			base.InitBaseView();
			this.InitFullView();
		}

		private void InitFullView()
		{
			Lang lang = Service.Lang;
			this.typeLabel.Visible = false;
			this.memberNumberLabel.Text = lang.Get("SQUAD_INVITED_BY", new object[]
			{
				this.invite.SenderName
			});
			this.primaryButtonLabel.Text = lang.Get("BUTTON_ACCEPT", new object[0]);
			this.primaryButton.OnClicked = new UXButtonClickedDelegate(this.OnAcceptClicked);
			this.joinModule = new SquadJoinActionModule(this.squad, this.screen, this.primaryButton);
			this.secondaryButtonLabel.Text = lang.Get("BUTTON_DECLINE", new object[0]);
			this.secondaryButton.OnClicked = new UXButtonClickedDelegate(this.OnRejectClicked);
			this.secondaryButton.Visible = true;
			this.tertiaryButtonLabel.Text = lang.Get("context_Info", new object[0]);
			this.tertiaryButton.OnClicked = new UXButtonClickedDelegate(this.screen.ViewSquadInfoInviteClicked);
			this.tertiaryButton.Tag = this.invite;
			this.tertiaryButton.Visible = true;
			base.UpdateButtonContainerTween(this.buttonContainer, 3);
		}

		private void OnAcceptClicked(UXButton button)
		{
			SquadMsg message = SquadMsgUtils.CreateAcceptSquadInviteMessage(this.invite.SquadId, new SquadController.ActionCallback(this.OnAcceptSquadInviteComplete), null);
			Service.SquadController.TakeAction(message);
			Service.EventManager.SendEvent(EventId.SquadNext, null);
			ProcessingScreen.Show();
		}

		private void OnAcceptSquadInviteComplete(bool success, object cookie)
		{
			ProcessingScreen.Hide();
			if (success)
			{
				this.joinModule.OnSquadJoined();
				Service.UXController.HUD.UpdateSquadJewelCount();
				this.screen.RemoveAndDestroyRow(this);
			}
		}

		private void OnRejectClicked(UXButton button)
		{
			SquadMsg message = SquadMsgUtils.CreateRejectSquadInviteMessage(this.invite.SquadId, new SquadController.ActionCallback(this.OnRejectSquadInviteComplete), null);
			Service.SquadController.TakeAction(message);
			Service.EventManager.SendEvent(EventId.SquadNext, null);
			ProcessingScreen.Show();
		}

		private void OnRejectSquadInviteComplete(bool success, object cookie)
		{
			ProcessingScreen.Hide();
			if (success)
			{
				Service.UXController.HUD.UpdateSquadJewelCount();
				this.screen.RemoveAndDestroyRow(this);
				List<SquadInvite> squadInvites = Service.SquadController.StateManager.SquadInvites;
				if (squadInvites.Count == 0 && this.tab == SocialTabs.Invites)
				{
					this.screen.ForceSwitchTab(SocialTabs.Featured);
				}
			}
		}
	}
}
