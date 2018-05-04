using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Text;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadCreateScreen : ClosableScreen
	{
		private const string CONTAINER_SYMBOLS = "ContainerSquadIcons";

		private const string CONTAINER_COST = "Cost";

		private const string LABEL_TITLE = "LabelTitleSquadCreate";

		private const string LABEL_INSTRUCTIONS = "LabelInstructions";

		private const string LABEL_SYMBOL = "LabelSelectSymbol";

		private const string LABEL_SYMBOL_SELECT = "LabelSelectSymbolInner";

		private const string LABEL_SCORE_TITLE = "LabelMinTrophy";

		private const string LABEL_SCORE_VALUE = "LabelTrophyValue";

		private const string LABEL_SQUAD_TYPE = "LabelSquadType";

		private const string LABEL_SQUAD_TYPE_INVITE = "LabelInvite";

		private const string LABEL_SQUAD_TYPE_OPEN = "LabelOpen";

		private const string LABEL_PURCHASE = "CostLabel";

		private const string LABEL_DONE = "LabelBtnDone";

		private const string LABEL_CONFIRM = "LabelBtnConfirm";

		private const string INPUT_NAME = "LabelInputNameSquad";

		private const string INPUT_DESC = "LabelInputSquadDesc";

		private const string BUTTON_PURCHASE = "BtnPay";

		private const string BUTTON_DONE = "BtnDone";

		private const string BUTTON_CONFIRM = "BtnConfirm";

		private const string BUTTON_TYPE_BACK = "BtnScrollBack";

		private const string BUTTON_TYPE_FORWARD = "BtnScrollForward";

		private const string BUTTON_SYMBOL = "BtnSelectSymbol";

		private const string BUTTON_SCORE_DECREMENT = "BtnDecrement";

		private const string BUTTON_SCORE_INCREMENT = "BtnIncrement";

		private const string SPRITE_SYMBOL = "SpriteBtnSelectSymbol";

		private const string SYMBOL_NAME_PREFIX = "SquadSymbols_";

		private const string SYMBOL_NAME_SPRITE_FORMAT = "SquadSymbols_{0:D2}";

		private const string SYMBOL_NAME_CHECKBOX_PREFIX = "Toggle";

		private const string SYMBOL_NAME_CHECKBOX_FORMAT = "Toggle{0:D2}";

		private const string SYMBOL_NAME_MAKER_REBEL = "MakerRebel";

		private const string SYMBOL_NAME_MAKER_EMPIRE = "MakerEmpire";

		private const string ALERT = "ALERT";

		private const string CREATE_LEAVE_SQUAD_ALERT = "CREATE_LEAVE_SQUAD_ALERT";

		private const string CREATE_CTA = "CREATE_CTA";

		private const string ACCOUNT_CONFLICT_CONFIRM_CANCEL = "ACCOUNT_CONFLICT_CONFIRM_CANCEL";

		private const int NUM_SYMBOLS = 16;

		private const int SCORE_DECREMENT_AMOUNT = -100;

		private const int SCORE_INCREMENT_AMOUNT = 100;

		private const string SCORE_REQ = "MIN_TROPHY_REQ";

		private const string ENROLLMENT_TYPE = "SQUAD_TYPE_LABEL";

		private const string REQUEST_ONLY = "SQUAD_INVITE_ONLY";

		private const string OPEN_TO_ALL = "SQUAD_OPEN_TO_ALL";

		private const string SELECT_SQUAD_SYMBOL = "SELECT_SQUAD_SYMBOL";

		private const string SELECT_SYMBOL = "SELECT_SYMBOL";

		private const string CONFIRM = "s_Confirm";

		private const string CREATE_SQUAD = "CREATE_SQUAD";

		private const string CREATE_INSTRUCTIONS = "SQUAD_CREATE_INSTRUCTIONS";

		private const string DEFAULT_INPUT_NAME = "INIT_SQUAD_NAME_INPUT";

		private const string DEFAULT_INPUT_DESC = "INIT_SQUAD_DESC_INPUT";

		private const string EDIT_SQUAD = "EDIT_SQUAD";

		private const string DONE = "BUTTON_DONE";

		private const string INVALID_SQUAD_NAME = "INVALID_SQUAD_NAME";

		private const string INVALID_SQUAD_DESC = "INVALID_SQUAD_DESC";

		private UXElement containerSymbols;

		private UXLabel labelInstructions;

		private UXLabel labelTypeInvite;

		private UXLabel labelTypeOpen;

		private UXLabel labelScore;

		private UXInput inputName;

		private UXInput inputDescription;

		private UXButton buttonPurchase;

		private UXButton buttonEdit;

		private UXButton buttonScoreDecrement;

		private UXButton buttonScoreIncrement;

		private UXButton buttonTypeBack;

		private UXButton buttonTypeForward;

		private UXSprite spriteSelectedSymbol;

		private bool showCreateView;

		private int scoreReq;

		private int prevScoreReq;

		private bool openEnrollment;

		private string symbolName;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public SquadCreateScreen(bool showCreateView) : base("gui_squad_create")
		{
			this.showCreateView = showCreateView;
		}

		protected override void OnScreenLoaded()
		{
			this.labelInstructions = base.GetElement<UXLabel>("LabelInstructions");
			this.inputName = base.GetElement<UXInput>("LabelInputNameSquad");
			this.inputDescription = base.GetElement<UXInput>("LabelInputSquadDesc");
			this.InitButtons();
			this.InitScoreReqSelector();
			this.InitEnrollmentTypeSelector();
			this.InitSymbolSelector();
			if (this.showCreateView)
			{
				this.InitCreateView();
			}
			else
			{
				this.InitEditView();
			}
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.buttonPurchase = base.GetElement<UXButton>("BtnPay");
			this.buttonEdit = base.GetElement<UXButton>("BtnDone");
		}

		private void InitScoreReqSelector()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelMinTrophy");
			element.Text = this.lang.Get("MIN_TROPHY_REQ", new object[0]);
			this.labelScore = base.GetElement<UXLabel>("LabelTrophyValue");
			this.buttonScoreDecrement = base.GetElement<UXButton>("BtnDecrement");
			this.buttonScoreDecrement.Tag = -100;
			this.buttonScoreDecrement.OnClicked = new UXButtonClickedDelegate(this.OnScoreChangeClicked);
			this.buttonScoreIncrement = base.GetElement<UXButton>("BtnIncrement");
			this.buttonScoreIncrement.Tag = 100;
			this.buttonScoreIncrement.OnClicked = new UXButtonClickedDelegate(this.OnScoreChangeClicked);
			this.UpdateScoreReq(0);
			this.prevScoreReq = this.scoreReq;
		}

		private void OnScoreChangeClicked(UXButton button)
		{
			int delta = (int)button.Tag;
			this.UpdateScoreReq(delta);
			Service.EventManager.SendEvent(EventId.SquadNext, null);
		}

		private void UpdateScoreReq(int delta)
		{
			if (this.scoreReq == 0 && delta > 0)
			{
				delta = GameConstants.SQUAD_CREATE_MIN_TROPHY_REQ;
			}
			else if (this.scoreReq == GameConstants.SQUAD_CREATE_MIN_TROPHY_REQ && delta < 0)
			{
				delta = -GameConstants.SQUAD_CREATE_MIN_TROPHY_REQ;
			}
			this.scoreReq += delta;
			int num = 0;
			int sQUAD_CREATE_MAX_TROPHY_REQ = GameConstants.SQUAD_CREATE_MAX_TROPHY_REQ;
			if (this.scoreReq < num)
			{
				this.scoreReq = num;
			}
			else if (this.scoreReq > sQUAD_CREATE_MAX_TROPHY_REQ)
			{
				this.scoreReq = sQUAD_CREATE_MAX_TROPHY_REQ;
			}
			if (this.scoreReq <= num)
			{
				this.buttonScoreIncrement.Enabled = true;
				this.buttonScoreDecrement.Enabled = false;
			}
			else if (this.scoreReq >= sQUAD_CREATE_MAX_TROPHY_REQ)
			{
				this.buttonScoreIncrement.Enabled = false;
				this.buttonScoreDecrement.Enabled = true;
			}
			else
			{
				this.buttonScoreIncrement.Enabled = true;
				this.buttonScoreDecrement.Enabled = true;
			}
			this.labelScore.Text = this.scoreReq.ToString();
		}

		private void InitEnrollmentTypeSelector()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelSquadType");
			element.Text = this.lang.Get("SQUAD_TYPE_LABEL", new object[0]);
			this.labelTypeInvite = base.GetElement<UXLabel>("LabelInvite");
			this.labelTypeInvite.Text = this.lang.Get("SQUAD_INVITE_ONLY", new object[0]);
			this.labelTypeOpen = base.GetElement<UXLabel>("LabelOpen");
			this.labelTypeOpen.Text = this.lang.Get("SQUAD_OPEN_TO_ALL", new object[0]);
			this.buttonTypeBack = base.GetElement<UXButton>("BtnScrollBack");
			this.buttonTypeBack.Tag = false;
			this.buttonTypeBack.OnClicked = new UXButtonClickedDelegate(this.OnEnrollmentTypeChangeClicked);
			this.buttonTypeForward = base.GetElement<UXButton>("BtnScrollForward");
			this.buttonTypeForward.Tag = true;
			this.buttonTypeForward.OnClicked = new UXButtonClickedDelegate(this.OnEnrollmentTypeChangeClicked);
			this.UpdateEnrollmentType(true);
		}

		private void OnEnrollmentTypeChangeClicked(UXButton button)
		{
			bool flag = (bool)button.Tag;
			this.UpdateEnrollmentType(flag);
			Service.EventManager.SendEvent(EventId.SquadNext, null);
		}

		private void UpdateEnrollmentType(bool openEnrollment)
		{
			this.openEnrollment = openEnrollment;
			this.labelTypeOpen.Visible = openEnrollment;
			this.labelTypeInvite.Visible = !openEnrollment;
			this.buttonTypeBack.Enabled = openEnrollment;
			this.buttonTypeForward.Enabled = !openEnrollment;
			int num = this.scoreReq;
			this.scoreReq = ((!openEnrollment) ? 0 : this.prevScoreReq);
			this.prevScoreReq = num;
			this.labelScore.Text = this.scoreReq.ToString();
			this.buttonScoreIncrement.Visible = openEnrollment;
			this.buttonScoreDecrement.Visible = openEnrollment;
		}

		private void InitSymbolSelector()
		{
			this.containerSymbols = base.GetElement<UXElement>("ContainerSquadIcons");
			UXLabel element = base.GetElement<UXLabel>("LabelSelectSymbolInner");
			element.Text = this.lang.Get("SELECT_SQUAD_SYMBOL", new object[0]);
			UXLabel element2 = base.GetElement<UXLabel>("LabelSelectSymbol");
			element2.Text = this.lang.Get("SELECT_SYMBOL", new object[0]);
			UXLabel element3 = base.GetElement<UXLabel>("LabelBtnConfirm");
			element3.Text = this.lang.Get("s_Confirm", new object[0]);
			UXButton element4 = base.GetElement<UXButton>("BtnConfirm");
			element4.OnClicked = new UXButtonClickedDelegate(this.OnConfirmSymbolClicked);
			UXButton element5 = base.GetElement<UXButton>("BtnSelectSymbol");
			element5.OnClicked = new UXButtonClickedDelegate(this.OnShowSymbolsClicked);
			this.spriteSelectedSymbol = base.GetElement<UXSprite>("SpriteBtnSelectSymbol");
			for (int i = 1; i <= 16; i++)
			{
				string name = string.Format("Toggle{0:D2}", i);
				string tag = string.Format("SquadSymbols_{0:D2}", i);
				UXCheckbox element6 = base.GetElement<UXCheckbox>(name);
				element6.Tag = tag;
				element6.OnSelected = new UXCheckboxSelectedDelegate(this.OnSymbolSelected);
			}
			string str;
			string str2;
			if (Service.CurrentPlayer.Faction == FactionType.Empire)
			{
				str = "MakerEmpire";
				str2 = "MakerRebel";
			}
			else
			{
				str = "MakerRebel";
				str2 = "MakerEmpire";
			}
			UXCheckbox element7 = base.GetElement<UXCheckbox>("Toggle" + str);
			element7.Tag = "SquadSymbols_" + str;
			element7.OnSelected = new UXCheckboxSelectedDelegate(this.OnSymbolSelected);
			UXCheckbox element8 = base.GetElement<UXCheckbox>("Toggle" + str2);
			element8.Visible = false;
		}

		private void OnShowSymbolsClicked(UXButton btn)
		{
			this.ToggleSymbolsView(true);
		}

		private void OnSymbolSelected(UXCheckbox symbol, bool selected)
		{
			if (selected)
			{
				this.symbolName = (string)symbol.Tag;
			}
		}

		private void OnConfirmSymbolClicked(UXButton btn)
		{
			this.spriteSelectedSymbol.SpriteName = this.symbolName;
			this.ToggleSymbolsView(false);
		}

		private void ToggleSymbolsView(bool show)
		{
			this.containerSymbols.Visible = show;
			this.CloseButton.Visible = !show;
			if (!this.showCreateView)
			{
				this.buttonEdit.Visible = !show;
			}
		}

		private string ConvertSymbolNameToCheckboxId(string symbolName)
		{
			int length = "SquadSymbols_".Length;
			string str = symbolName.Substring(length);
			return "Toggle" + str;
		}

		private void InitCreateView()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelTitleSquadCreate");
			element.Text = this.lang.Get("CREATE_SQUAD", new object[0]);
			this.labelInstructions.Text = this.lang.Get("SQUAD_CREATE_INSTRUCTIONS", new object[0]);
			this.inputName.EnableInput = true;
			UIInput uIInputComponent = this.inputName.GetUIInputComponent();
			uIInputComponent.onValidate = new UIInput.OnValidate(this.OnNameValidate);
			uIInputComponent.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnNameChange)));
			this.inputName.InitText(this.lang.Get("INIT_SQUAD_NAME_INPUT", new object[0]));
			this.inputDescription.InitText(this.lang.Get("INIT_SQUAD_DESC_INPUT", new object[0]));
			UXLabel element2 = base.GetElement<UXLabel>("CostLabel");
			element2.Text = GameConstants.SQUAD_CREATE_COST.ToString();
			this.SetupCreateCostButton();
			this.buttonPurchase.OnClicked = new UXButtonClickedDelegate(this.OnCreateClicked);
			this.buttonPurchase.Enabled = false;
			this.buttonEdit.Visible = false;
		}

		private void SetupCreateCostButton()
		{
			UXUtils.SetupSingleCostElement(this, "Cost", GameConstants.SQUAD_CREATE_COST, 0, 0, 0, 0, false, null);
		}

		private void OnNameChange()
		{
			string text = this.inputName.Text;
			if (!string.IsNullOrEmpty(text) && text.Length >= GameConstants.SQUAD_NAME_LENGTH_MIN)
			{
				this.buttonPurchase.Enabled = true;
				this.labelInstructions.Visible = false;
			}
			else
			{
				this.buttonPurchase.Enabled = false;
				this.labelInstructions.Visible = true;
			}
		}

		public char OnNameValidate(string text, int charIndex, char addedChar)
		{
			if (text.Length >= GameConstants.SQUAD_NAME_LENGTH_MAX)
			{
				return '\0';
			}
			return LangUtils.OnValidateWSpaces(text, charIndex, addedChar);
		}

		private void OnCreateClicked(UXButton button)
		{
			if (!GameUtils.CanAffordCredits(GameConstants.SQUAD_CREATE_COST))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("create");
				stringBuilder.Append("|");
				string value = (!this.openEnrollment) ? "private" : "public";
				stringBuilder.Append(value);
				stringBuilder.Append("|");
				stringBuilder.Append(this.scoreReq);
				PayMeScreen.ShowIfNotEnoughCurrency(GameConstants.SQUAD_CREATE_COST, 0, 0, stringBuilder.ToString(), new OnScreenModalResult(this.OnPurchaseSoftCurrency));
			}
			else
			{
				this.CreateSquad();
			}
		}

		private void OnPurchaseSoftCurrency(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				this.SetupCreateCostButton();
				this.CreateSquad();
			}
		}

		private void CreateSquad()
		{
			bool flag = this.CheckForValidInput(true);
			if (flag)
			{
				Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
				if (currentSquad != null)
				{
					YesNoScreen.ShowModal(this.lang.Get("ALERT", new object[0]), this.lang.Get("CREATE_LEAVE_SQUAD_ALERT", new object[]
					{
						currentSquad.SquadName
					}), false, this.lang.Get("CREATE_CTA", new object[0]), this.lang.Get("ACCOUNT_CONFLICT_CONFIRM_CANCEL", new object[0]), new OnScreenModalResult(this.OnLeaveAndCreateSquad), null);
					Service.EventManager.SendEvent(EventId.UISquadLeaveConfirmation, currentSquad.SquadID + "|create|");
				}
				else
				{
					this.ActualCreateSquad();
				}
			}
		}

		private void OnCreateComplete(bool success, object cookie)
		{
			ProcessingScreen.Hide();
			base.AllowClose = true;
			if (success)
			{
				string text = (string)cookie;
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(this.lang.Get("CREATE_SQUAD_CONFIRMATION", new object[]
				{
					text
				}), 1f, 5f);
				Service.ScreenController.CloseAll();
			}
			else
			{
				this.buttonPurchase.Enabled = true;
			}
		}

		private void OnLeaveAndCreateSquad(object result, object cookie)
		{
			if (result != null)
			{
				ProcessingScreen.Show();
				SquadMsg message = SquadMsgUtils.CreateLeaveSquadMessage(new SquadController.ActionCallback(this.OnLeaveComplete), null);
				Service.SquadController.TakeAction(message);
			}
		}

		private void OnLeaveComplete(bool success, object cookie)
		{
			if (success)
			{
				this.ActualCreateSquad();
			}
			else
			{
				Service.ScreenController.CloseAll();
			}
		}

		private void ActualCreateSquad()
		{
			Service.EventManager.SendEvent(EventId.SquadCredits, null);
			string text = this.inputName.Text;
			string text2 = this.inputDescription.Text;
			SquadMsg message = SquadMsgUtils.CreateNewSquadMessage(text, text2, this.symbolName, this.scoreReq, this.openEnrollment, new SquadController.ActionCallback(this.OnCreateComplete), text);
			Service.SquadController.TakeAction(message);
			this.buttonPurchase.Enabled = false;
			base.AllowClose = false;
			ProcessingScreen.Show();
		}

		private void InitEditView()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelTitleSquadCreate");
			element.Text = this.lang.Get("EDIT_SQUAD", new object[0]);
			this.labelInstructions.Visible = false;
			UXLabel element2 = base.GetElement<UXLabel>("LabelBtnDone");
			element2.Text = this.lang.Get("BUTTON_DONE", new object[0]);
			this.buttonPurchase.Visible = false;
			this.buttonEdit.OnClicked = new UXButtonClickedDelegate(this.OnEditClicked);
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			this.inputName.Text = currentSquad.SquadName;
			this.inputName.EnableInput = false;
			this.inputDescription.Text = currentSquad.Description;
			this.spriteSelectedSymbol.SpriteName = currentSquad.Symbol;
			this.symbolName = currentSquad.Symbol;
			string name = this.ConvertSymbolNameToCheckboxId(currentSquad.Symbol);
			UXCheckbox element3 = base.GetElement<UXCheckbox>(name);
			element3.DelayedSelect(true);
			this.UpdateEnrollmentType(currentSquad.InviteType == 1);
			this.scoreReq = currentSquad.RequiredTrophies;
			this.UpdateScoreReq(0);
		}

		private void OnEditClicked(UXButton button)
		{
			bool flag = this.CheckForValidInput(false);
			if (flag)
			{
				Service.EventManager.SendEvent(EventId.SquadEdited, null);
				string text = this.inputDescription.Text;
				SquadMsg message = SquadMsgUtils.CreateEditSquadMessage(text, this.symbolName, this.scoreReq, this.openEnrollment, new SquadController.ActionCallback(this.OnEditComplete), null);
				Service.SquadController.TakeAction(message);
				this.buttonEdit.Enabled = false;
				base.AllowClose = false;
				ProcessingScreen.Show();
			}
		}

		private void OnEditComplete(bool success, object cookie)
		{
			ProcessingScreen.Hide();
			base.AllowClose = true;
			if (success)
			{
				string squadName = Service.SquadController.StateManager.GetCurrentSquad().SquadName;
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(this.lang.Get("EDIT_SQUAD_CONFIRMATION", new object[]
				{
					squadName
				}), 1f, 5f);
				IState currentState = Service.GameStateMachine.CurrentState;
				if (currentState is WarBoardState)
				{
					this.Close(null);
				}
				else
				{
					Service.ScreenController.CloseAll();
				}
			}
			else
			{
				this.buttonEdit.Enabled = true;
			}
		}

		private bool CheckForValidInput(bool checkName)
		{
			ProfanityController profanityController = Service.ProfanityController;
			if (checkName && (this.inputName.Text == this.lang.Get("INIT_SQUAD_NAME_INPUT", new object[0]) || !profanityController.IsValid(this.inputName.Text, true)))
			{
				AlertScreen.ShowModal(false, null, this.lang.Get("INVALID_SQUAD_NAME", new object[0]), null, null);
				return false;
			}
			if (!profanityController.IsValid(this.inputDescription.Text, false))
			{
				AlertScreen.ShowModal(false, null, this.lang.Get("INVALID_SQUAD_DESC", new object[0]), null, null);
				return false;
			}
			return true;
		}
	}
}
