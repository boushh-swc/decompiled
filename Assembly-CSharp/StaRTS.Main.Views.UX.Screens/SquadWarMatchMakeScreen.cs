using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarMatchMakeScreen : ClosableScreen
	{
		private const string LABEL_TITLE = "LabelTitle";

		private const string BTN_INFO = "BtnInfo";

		private const string LABEL_SQUAD_QUEUE = "LabelSquadQueue";

		private const string BUTTON_CANCEL = "ButtonCancel";

		private const string LABEL_BUTTON_CANCEL = "LabelButtonCancel";

		private const string BUTTON_BACK = "ButtonBack";

		private const string LABEL_BUTTON_BACK = "LabelButtonBack";

		private const string BUTTON_CONFIRM = "ButtonConfirm";

		private const string LABEL_BUTTON_OK = "LabelButtonOk";

		private const string QUEUE_TEXTURE = "QueueTexture";

		private const string WAR_MATCHMAKE_TITLE = "WAR_MATCHMAKE_TITLE";

		private const string WAR_CANCEL_MATCHMAKE = "WAR_CANCEL_MATCHMAKE";

		private const string WAR_CANCEL_MATCHMAKE_BACK = "WAR_CANCEL_MATCHMAKE_BACK";

		private const string WAR_MATCHMAKE_TEXT = "WAR_MATCHMAKE_TEXT";

		private const string WAR_MATCHMAKE_CANCEL_CONFIRM_TEXT = "WAR_MATCHMAKE_CANCEL_CONFIRM_TEXT";

		private const string WAR_OK_STRING = "WAR_OK";

		private const string GUI_TEXTURES_CANCEL_MATCHMAKING = "gui_textures_cancel_matchmaking";

		private UXLabel labelTitle;

		private UXButton btnInfo;

		private UXLabel labelSquadQueue;

		private UXButton buttonCancel;

		private UXLabel labelButtonCancel;

		private UXButton buttonBack;

		private UXLabel labelButtonBack;

		private UXButton buttonConfirm;

		private UXLabel labelButtonOk;

		private UXTexture queueTexture;

		private bool finalConfirm;

		public SquadWarMatchMakeScreen() : base("gui_squadwar_queue")
		{
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.labelTitle = base.GetElement<UXLabel>("LabelTitle");
			this.btnInfo = base.GetElement<UXButton>("BtnInfo");
			this.labelSquadQueue = base.GetElement<UXLabel>("LabelSquadQueue");
			this.buttonCancel = base.GetElement<UXButton>("ButtonCancel");
			this.labelButtonCancel = base.GetElement<UXLabel>("LabelButtonCancel");
			this.buttonBack = base.GetElement<UXButton>("ButtonBack");
			this.labelButtonBack = base.GetElement<UXLabel>("LabelButtonBack");
			this.buttonConfirm = base.GetElement<UXButton>("ButtonConfirm");
			this.labelButtonOk = base.GetElement<UXLabel>("LabelButtonOk");
			this.queueTexture = base.GetElement<UXTexture>("QueueTexture");
			StaticDataController staticDataController = Service.StaticDataController;
			TextureVO textureVO = staticDataController.Get<TextureVO>("gui_textures_cancel_matchmaking");
			this.queueTexture.LoadTexture(textureVO.AssetName);
			this.labelTitle.Text = this.lang.Get("WAR_MATCHMAKE_TITLE", new object[0]);
			this.btnInfo.OnClicked = new UXButtonClickedDelegate(Service.SquadController.WarManager.ShowInfoScreen);
			this.buttonCancel.OnClicked = new UXButtonClickedDelegate(this.OnCancelButtonClicked);
			this.labelButtonCancel.Text = this.lang.Get("WAR_CANCEL_MATCHMAKE", new object[0]);
			this.buttonBack.OnClicked = new UXButtonClickedDelegate(this.OnBackClicked);
			this.labelButtonBack.Text = this.lang.Get("WAR_CANCEL_MATCHMAKE_BACK", new object[0]);
			this.buttonConfirm.OnClicked = new UXButtonClickedDelegate(this.OnConfirmClicked);
			this.labelButtonOk.Text = this.lang.Get("WAR_OK", new object[0]);
			this.finalConfirm = false;
			this.Refresh();
		}

		private void Refresh()
		{
			if (!this.finalConfirm)
			{
				SquadRole role = Service.SquadController.StateManager.Role;
				bool visible = role == SquadRole.Owner || role == SquadRole.Officer;
				this.buttonCancel.Visible = visible;
				this.buttonBack.Visible = false;
				this.buttonConfirm.Visible = false;
				this.labelSquadQueue.Text = this.lang.Get("WAR_MATCHMAKE_TEXT", new object[0]);
			}
			else
			{
				this.buttonCancel.Visible = false;
				this.buttonBack.Visible = true;
				this.buttonConfirm.Visible = true;
				this.labelSquadQueue.Text = this.lang.Get("WAR_MATCHMAKE_CANCEL_CONFIRM_TEXT", new object[0]);
			}
		}

		private void OnCancelButtonClicked(UXButton button)
		{
			this.finalConfirm = true;
			this.Refresh();
		}

		private void OnBackClicked(UXButton button)
		{
			this.finalConfirm = false;
			this.Refresh();
		}

		private void OnConfirmClicked(UXButton button)
		{
			this.finalConfirm = false;
			this.Close(null);
			Service.SquadController.WarManager.CancelMatchMakingTakeAction();
		}
	}
}
