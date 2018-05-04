using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.Leaderboard;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadIntroScreen : ClosableScreen
	{
		private const string BUTTON_CREATE = "ButtonCreateSquad";

		private const string BUTTON_JOIN = "ButtonJoinSquad";

		private const string LABEL_TITLE = "LabelTitle";

		private const string LABEL_CREATE = "LabelCreateSquad";

		private const string LABEL_JOIN = "LabelJoinSquad";

		private const string LABEL_INTRO = "LabelSquadIntro";

		private const string TEXTURE = "SquadImage";

		private const string SQUAD_JOIN_OR_CREATE = "SQUAD_JOIN_OR_CREATE";

		private const string SQUAD_INTRO = "SQUAD_INTRO";

		private const string CREATE_SQUAD = "CREATE_SQUAD";

		private const string JOIN_A_SQUAD = "JOIN_A_SQUAD";

		private const string TEXTURE_NAME_REBEL = "SquadIntro_r";

		private const string TEXTURE_NAME_EMPIRE = "SquadIntro_e";

		public SquadIntroScreen() : base("gui_squad_intro")
		{
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			UXTexture element = base.GetElement<UXTexture>("SquadImage");
			FactionType faction = Service.CurrentPlayer.Faction;
			FactionType factionType = faction;
			if (factionType != FactionType.Empire)
			{
				if (factionType == FactionType.Rebel)
				{
					element.LoadTexture("SquadIntro_r");
				}
			}
			else
			{
				element.LoadTexture("SquadIntro_e");
			}
			UXLabel element2 = base.GetElement<UXLabel>("LabelTitle");
			element2.Text = this.lang.Get("SQUAD_JOIN_OR_CREATE", new object[0]);
			UXLabel element3 = base.GetElement<UXLabel>("LabelSquadIntro");
			element3.Text = this.lang.Get("SQUAD_INTRO", new object[0]);
			UXLabel element4 = base.GetElement<UXLabel>("LabelCreateSquad");
			element4.Text = this.lang.Get("CREATE_SQUAD", new object[0]);
			UXLabel element5 = base.GetElement<UXLabel>("LabelJoinSquad");
			element5.Text = this.lang.Get("JOIN_A_SQUAD", new object[0]);
		}

		protected override void InitButtons()
		{
			UXButton element = base.GetElement<UXButton>("ButtonCreateSquad");
			element.OnClicked = new UXButtonClickedDelegate(this.OnCreateClicked);
			UXButton element2 = base.GetElement<UXButton>("ButtonJoinSquad");
			element2.OnClicked = new UXButtonClickedDelegate(this.OnJoinClicked);
			base.InitButtons();
		}

		private void OnCreateClicked(UXButton button)
		{
			Service.ScreenController.AddScreen(new SquadCreateScreen(true));
			this.Close(null);
		}

		private void OnJoinClicked(UXButton button)
		{
			Service.ScreenController.AddScreen(new SquadJoinScreen());
			this.Close(null);
		}
	}
}
