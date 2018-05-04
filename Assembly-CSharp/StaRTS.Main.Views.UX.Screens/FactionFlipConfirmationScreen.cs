using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class FactionFlipConfirmationScreen : AlertScreen
	{
		private const string CONFIRMATION_BOX_GROUP = "buttonConfirmation";

		private const string CONFIRMATION_BOX_LABEL = "LabelConfirmation";

		private UXCheckbox checkbox;

		public FactionFlipConfirmationScreen(FactionType currentFaction, FactionType oppositeFaction, OnScreenModalResult onModalResult) : base(false, null, null, null, false)
		{
			base.OnModalResult = onModalResult;
			this.title = this.lang.Get("FACTION_FLIP_CONFIRMATION_TITLE", new object[0]);
			this.message = this.lang.Get("FACTION_FLIP_CONFIRMATION_DESCRIPTION", new object[]
			{
				this.lang.Get(oppositeFaction.ToString().ToLower(), new object[0]),
				this.lang.Get(currentFaction.ToString().ToLower(), new object[0])
			});
		}

		protected override void SetupControls()
		{
			base.SetupControls();
			this.primary2OptionButton.Visible = true;
			this.primary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnConfirmButtonClicked);
			this.primary2Option.Text = this.lang.Get("FACTION_FLIP_CONFIRM", new object[0]);
			this.secondary2OptionButton.Visible = true;
			this.secondary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.secondary2Option.Text = this.lang.Get("FACTION_FLIP_CANCEL", new object[0]);
			base.GetElement<UXLabel>("LabelConfirmation").Text = this.lang.Get("FACTION_FLIP_SKIP_FUTURE_CONFIRMATION", new object[0]);
			this.checkbox = base.GetElement<UXCheckbox>("buttonConfirmation");
			this.checkbox.Visible = true;
			this.checkbox.Selected = false;
			this.primaryButton.Visible = false;
		}

		private void OnConfirmButtonClicked(UXButton button)
		{
			if (this.checkbox.Selected)
			{
				Service.ServerPlayerPrefs.SetPref(ServerPref.FactionFlippingSkipConfirmation, "1");
				Service.ServerAPI.Enqueue(new SetPrefsCommand(false));
			}
			this.Close(true);
			Service.EventManager.SendEvent(EventId.UIFactionFlipConfirmAction, "faction");
		}

		private void OnBackButtonClicked(UXButton button)
		{
			this.Close(false);
		}

		public override void Close(object modalResult)
		{
			if (modalResult == null)
			{
				FactionFlipScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<FactionFlipScreen>();
				if (highestLevelScreen != null)
				{
					highestLevelScreen.Close(false);
				}
			}
			Service.EventManager.SendEvent(EventId.UIFactionFlipConfirmAction, "close");
			base.Close(modalResult);
		}
	}
}
