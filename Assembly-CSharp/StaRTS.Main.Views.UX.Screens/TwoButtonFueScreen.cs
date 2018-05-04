using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TwoButtonFueScreen : ClosableScreen
	{
		private const string FACTION_CHOICE_GROUP = "FactionChoice";

		private const string FACTION_CHOICE_TEXTURE = "TextureCharacterFaction";

		private const string FACTION_CHOICE_LABEL_TITLE = "LabelTitleFaction";

		private const string FACTION_CHOICE_LABEL_DESC = "TextFaction";

		private const string FACTION_CHOICE_BUTTON_YES = "BtnPrimary_2optionFaction";

		private const string FACTION_CHOICE_BUTTON_LABEL_YES = "LabelBtnPrimary_2optionFaction";

		private const string FACTION_CHOICE_BUTTON_NO = "BtnSecondaryFaction";

		private const string FACTION_CHOICE_BUTTON_LABEL_NO = "LabelBtnSecondaryFaction";

		private const string FACTION_CHOICE_CLOSE = "BtnCloseFaction";

		private const string GENERIC_GROUP = "Generic";

		private const string TEXTURE = "TextureCharacter";

		private const string LABEL_TITLE = "LabelTitle";

		private const string LABEL_DESC = "TextGeneric";

		private const string BUTTON_YES = "BtnPrimary_2option";

		private const string BUTTON_LABEL_YES = "LabelBtnPrimary_2option";

		private const string BUTTON_NO = "BtnSecondary";

		private const string BUTTON_LABEL_NO = "LabelBtnSecondary";

		private const string CRYSTAL_GROUP = "CrystalIncentive";

		private const string CRYSTAL_TEXT_LEFT = "LabelCrystalIncentive";

		private const string CRYSTAL_TEXT_RIGHT = "LabelCrystalIncentive2";

		private const string IMAGE_SAPAONZA = "CharacterPopUp_Sapz";

		private const string IMAGE_LEIA = "CharacterPopUp_Leia";

		private const string IMAGE_VADER = "CharacterPopUp_Vader";

		private const string IMAGE_FOLDER = "images/characters";

		private bool useFaction;

		private FactionType faction;

		private UXLabel title;

		private UXLabel desc;

		private UXButton buttonYes;

		private UXButton buttonNo;

		private UXLabel buttonYesLabel;

		private UXLabel buttonNoLabel;

		private UXTexture image;

		private string descriptionText;

		private bool showIncent;

		public TwoButtonFueScreen(bool forFactionChoice, OnScreenModalResult onModalResult, object modalResultCookie, string descriptionText, bool showIncent) : base("gui_characterpopup")
		{
			this.descriptionText = descriptionText;
			this.showIncent = showIncent;
			this.useFaction = forFactionChoice;
			if (this.useFaction)
			{
				this.faction = (FactionType)((int)modalResultCookie);
			}
			base.AllowFUEBackButton = true;
			base.OnModalResult = onModalResult;
			base.ModalResultCookie = modalResultCookie;
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			base.GetElement<UXElement>("FactionChoice").Visible = this.useFaction;
			base.GetElement<UXElement>("Generic").Visible = !this.useFaction;
			this.image = base.GetElement<UXTexture>((!this.useFaction) ? "TextureCharacter" : "TextureCharacterFaction");
			this.title = base.GetElement<UXLabel>((!this.useFaction) ? "LabelTitle" : "LabelTitleFaction");
			this.desc = base.GetElement<UXLabel>((!this.useFaction) ? "TextGeneric" : "TextFaction");
			this.buttonYes = base.GetElement<UXButton>((!this.useFaction) ? "BtnPrimary_2option" : "BtnPrimary_2optionFaction");
			this.buttonNo = base.GetElement<UXButton>((!this.useFaction) ? "BtnSecondary" : "BtnSecondaryFaction");
			this.buttonYesLabel = base.GetElement<UXLabel>((!this.useFaction) ? "LabelBtnPrimary_2option" : "LabelBtnPrimary_2optionFaction");
			this.buttonNoLabel = base.GetElement<UXLabel>((!this.useFaction) ? "LabelBtnSecondary" : "LabelBtnSecondaryFaction");
			this.buttonYes.OnClicked = new UXButtonClickedDelegate(this.OnButton);
			this.buttonNo.OnClicked = new UXButtonClickedDelegate(this.OnButton);
			base.GetElement<UXButton>("BtnCloseFaction").OnClicked = new UXButtonClickedDelegate(this.OnButton);
			if (this.showIncent)
			{
				base.GetElement<UXLabel>("LabelCrystalIncentive").Text = this.lang.Get("crystal_incentive_1", new object[]
				{
					GameConstants.PUSH_NOTIFICATION_CRYSTAL_REWARD_AMOUNT
				});
				base.GetElement<UXLabel>("LabelCrystalIncentive2").Text = this.lang.Get("crystal_incentive_2", new object[0]);
			}
			else
			{
				base.GetElement<UXElement>("CrystalIncentive").Visible = false;
			}
			if (!this.useFaction)
			{
				this.image.LoadTexture("CharacterPopUp_Sapz");
				this.title.Text = this.lang.Get("notif_auth_alert_title", new object[0]);
				this.desc.Text = this.lang.Get(this.descriptionText, new object[0]);
				this.buttonYesLabel.Text = this.lang.Get("notif_auth_alert_yes", new object[0]);
				this.buttonNoLabel.Text = this.lang.Get("notif_auth_alert_no", new object[0]);
			}
			else if (this.faction == FactionType.Empire)
			{
				this.image.LoadTexture("CharacterPopUp_Vader");
				this.title.Text = this.lang.Get("FACTION_CONFIRMATION_TITLE", new object[0]);
				this.desc.Text = this.lang.Get("FACTION_CONFIRMATION_DESCRIPTION", new object[]
				{
					this.lang.Get("FACTION_CONFIRMATION_EMPIRE", new object[0])
				});
				this.buttonYesLabel.Text = this.lang.Get("FACTION_CONFIRMATION_JOIN_EMPIRE", new object[0]);
				this.buttonNoLabel.Text = this.lang.Get("ACCOUNT_CONFLICT_CONFIRM_CANCEL", new object[0]);
			}
			else if (this.faction == FactionType.Rebel)
			{
				this.image.LoadTexture("CharacterPopUp_Leia");
				this.title.Text = this.lang.Get("FACTION_CONFIRMATION_TITLE", new object[0]);
				this.desc.Text = this.lang.Get("FACTION_CONFIRMATION_DESCRIPTION", new object[]
				{
					this.lang.Get("FACTION_CONFIRMATION_REBEL", new object[0])
				});
				this.buttonYesLabel.Text = this.lang.Get("FACTION_CONFIRMATION_JOIN_REBEL", new object[0]);
				this.buttonNoLabel.Text = this.lang.Get("ACCOUNT_CONFLICT_CONFIRM_CANCEL", new object[0]);
			}
		}

		private void OnButton(UXButton button)
		{
			if (button == this.buttonYes)
			{
				this.Close(true);
			}
			else
			{
				this.Close(null);
			}
		}
	}
}
