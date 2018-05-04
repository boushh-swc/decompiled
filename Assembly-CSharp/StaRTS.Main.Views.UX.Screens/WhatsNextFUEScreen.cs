using StaRTS.Main.Models;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class WhatsNextFUEScreen : ScreenBase
	{
		private const string NEXT_BUTTON = "BtnNext";

		private const string LABEL_DESCRIPTION = "LabelWhatNext";

		private const string FACTION_TEXTURE_HOLDER = "SpriteNextImage";

		private const string NEXT_BUTTON_LABEL = "LabelBtnNext";

		private const string TITLE_LABEL = "LabelTitle";

		private const string TEXTURE_EMPIRE = "WhatsNextEmpire";

		private const string TEXTURE_REBEL = "WhatsNextRebel";

		public WhatsNextFUEScreen() : base("gui_what_next")
		{
		}

		protected override void OnScreenLoaded()
		{
			base.GetElement<UXLabel>("LabelTitle").Text = this.lang.Get("s_WhatsNextTitle", new object[0]);
			base.GetElement<UXLabel>("LabelBtnNext").Text = this.lang.Get("SPEC_OPS_CONTINUE", new object[0]);
			UXButton element = base.GetElement<UXButton>("BtnNext");
			element.OnClicked = new UXButtonClickedDelegate(this.Close);
			base.CurrentBackDelegate = element.OnClicked;
			base.CurrentBackButton = element;
			base.AllowFUEBackButton = true;
			FactionType faction = Service.CurrentPlayer.Faction;
			UXLabel element2 = base.GetElement<UXLabel>("LabelWhatNext");
			element2.Text = LangUtils.ProcessStringWithNewlines(this.lang.Get("s_WhatsNextDescription_" + faction.ToString(), new object[0]));
			FactionType factionType = faction;
			string assetName;
			if (factionType != FactionType.Empire)
			{
				assetName = "WhatsNextRebel";
			}
			else
			{
				assetName = "WhatsNextEmpire";
			}
			base.GetElement<UXTexture>("SpriteNextImage").LoadTexture(assetName);
		}
	}
}
