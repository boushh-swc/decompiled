using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadJoinRequestScreen : AbstractSquadRequestScreen
	{
		private const string APPLY_REQUEST = "APPLY_REQUEST";

		private const string APPLY_REQUEST_DEFAULT = "APPLY_REQUEST_DEFAULT";

		private const string SQUAD_JOIN_REQUEST_INFO = "SQUAD_JOIN_REQUEST_INFO";

		private const string ALERT = "ALERT";

		private const string APPLY_LEAVE_SQUAD_ALERT = "APPLY_LEAVE_SQUAD_ALERT";

		private const string APPLY_CTA = "APPLY_CTA";

		private const string ACCOUNT_CONFLICT_CONFIRM_CANCEL = "ACCOUNT_CONFLICT_CONFIRM_CANCEL";

		private Squad squad;

		private SquadController.ActionCallback callback;

		private object cookie;

		public SquadJoinRequestScreen(Squad squad, SquadController.ActionCallback callback, object cookie)
		{
			this.squad = squad;
			this.callback = callback;
			this.cookie = cookie;
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			this.requestLabel.Text = this.lang.Get("APPLY_REQUEST", new object[]
			{
				this.squad.SquadName
			});
			this.instructionsLabel.Text = this.lang.Get("SQUAD_JOIN_REQUEST_INFO", new object[0]);
			this.input.InitText(this.lang.Get("APPLY_REQUEST_DEFAULT", new object[0]));
			this.CloseButton.OnClicked = new UXButtonClickedDelegate(this.OnCancel);
		}

		protected override void OnClicked(UXButton button)
		{
			if (!base.CheckForValidInput())
			{
				return;
			}
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad != null)
			{
				YesNoScreen.ShowModal(this.lang.Get("ALERT", new object[0]), this.lang.Get("APPLY_LEAVE_SQUAD_ALERT", new object[]
				{
					this.squad.SquadName,
					currentSquad.SquadName
				}), false, this.lang.Get("APPLY_CTA", new object[0]), this.lang.Get("ACCOUNT_CONFLICT_CONFIRM_CANCEL", new object[0]), new OnScreenModalResult(this.OnLeaveConfirmation), null);
				Service.EventManager.SendEvent(EventId.UISquadLeaveConfirmation, currentSquad.SquadID + "|apply|" + this.squad.SquadID);
			}
			else
			{
				this.SendRequest();
			}
		}

		private void OnLeaveConfirmation(object result, object cookie)
		{
			if (result != null)
			{
				this.SendRequest();
			}
			else
			{
				this.OnCancel(null);
			}
		}

		private void SendRequest()
		{
			Service.EventManager.SendEvent(EventId.SquadEdited, null);
			string text = this.input.Text;
			if (string.IsNullOrEmpty(text))
			{
				text = this.lang.Get("APPLY_REQUEST_DEFAULT", new object[0]);
			}
			SquadMsg message = SquadMsgUtils.CreateApplyToJoinSquadMessage(this.squad.SquadID, text, this.callback, this.cookie);
			Service.SquadController.TakeAction(message);
			Service.ScreenController.CloseAll();
		}

		private void OnCancel(UXButton button)
		{
			if (this.callback != null)
			{
				this.callback(false, this.cookie);
			}
			this.OnCloseButtonClicked(button);
		}
	}
}
