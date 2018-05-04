using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadAdvancementInfoScreen : ClosableScreen
	{
		private const string PERK_INFO_TITLE = "SQUAD_ADVANCEMENT_OVERVIEW_TITLE";

		private const string PERK_INFO_DESC = "SQUAD_ADVANCEMENT_OVERVIEW_DESC";

		private const string TITLE_LABEL = "LabelTitle";

		private const string OVERVIEW_LABEL = "LabelOverview";

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public SquadAdvancementInfoScreen() : base("gui_perks_info")
		{
			base.IsAlwaysOnTop = true;
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			UXLabel element = base.GetElement<UXLabel>("LabelTitle");
			UXLabel element2 = base.GetElement<UXLabel>("LabelOverview");
			element.Text = this.lang.Get("SQUAD_ADVANCEMENT_OVERVIEW_TITLE", new object[0]);
			element2.Text = this.lang.Get("SQUAD_ADVANCEMENT_OVERVIEW_DESC", new object[0]);
		}
	}
}
