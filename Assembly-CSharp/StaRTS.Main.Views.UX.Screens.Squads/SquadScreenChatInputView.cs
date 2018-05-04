using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenChatInputView : AbstractSquadScreenViewModule, IEventObserver
	{
		private const string CHAT_PANEL = "ChatPanel";

		private const string CHAT_TABLE = "ChatTable";

		private const string ELLIPSE = "...";

		private const string S_CHAT = "s_Chat";

		private const string REQUEST_TROOPS_DEFAULT = "REQUEST_TROOPS_DEFAULT";

		private const string BATTLE_REPLAY_SHARE_DEFAULT = "BATTLE_REPLAY_SHARE_DEFAULT";

		private const string SQUAD_OFFENSE = "SQUAD_OFFENSE";

		private const string SQUAD_DEFENSE = "SQUAD_DEFENSE";

		private const string PERCENTAGE = "PERCENTAGE";

		private const string INVALID_TEXT = "INVALID_TEXT";

		private const int CHAT_PANEL_REPLAY_OFFSET = -195;

		private const int CHAT_PANEL_UPDATE_OFFSET = -71;

		private const int CHAT_PANEL_NORMAL_OFFSET = -141;

		private const string INPUT_MODE_CHAT = "ChatInput";

		private const string INPUT_MODE_REQUEST = "RequestInput";

		private const string INPUT_MODE_SHARE = "ChatInputShare";

		private const string BTN_SENDCHAT = "BtnSendChat";

		private const string CHAT_INPUT = "LabelChatInput";

		private const string BTN_REQUESTTROOPS = "BtnRequestTroops";

		private const string BTN_RE_REQUESTTROOPS = "BtnResendRequest";

		private const string LABEL_RE_REQUESTCOST = "LabelBtnBtnResendRequestCost";

		private const string REQUEST_INPUT = "LabelRequestInput";

		private const string BTN_SHAREREPLAY = "BtnShareReplay";

		private const string SHARE_INPUT = "LabeShareReplayInput";

		private const string LABEL_SHARE_OPPONENTNAME = "LabelOpponentNameShare";

		private const string LABEL_SHARE_REPLAYTYPE = "LabelReplayTypeShare";

		private const string LABEL_SHARE_DMGPERCENT = "LabelDamageShare";

		private const string SPR_SHARE_1STAR = "SpriteStar1Share";

		private const string SPR_SHARE_2STAR = "SpriteStar2Share";

		private const string SPR_SHARE_3STAR = "SpriteStar3Share";

		private const string LABEL_SHARE_MEDALS = "LabelReplayMedalsShare";

		private const float GRAY_COLOR = 0.157f;

		private const float SHARE_DISABLE_SEC = 5f;

		private readonly Color grayOut = new Color(0.157f, 0.157f, 0.157f);

		private UXInput chatInputBox;

		private UIInput chatInputScript;

		private UXInput requestInputBox;

		private UIInput requestInputScript;

		private string defaultShareText;

		private UXInput shareInputBox;

		private UIInput shareInputScript;

		private UXButton requestTroopBtn;

		private UXButton resendRequestTroopBtn;

		private UXLabel resendRequestTroopCostLabel;

		private UXButton shareReplayButton;

		private UXLabel shareOppNameLabel;

		private UXLabel shareTypeLabel;

		private UXLabel shareDamagePercentLabel;

		private UXLabel shareMedalsLabel;

		private UXSprite share1StarSprite;

		private UXSprite share2StarSprite;

		private UXSprite share3StarSprite;

		private UXElement inputModeChat;

		private UXElement inputModeShare;

		private UXElement inputModeRequest;

		private UXElement chatPanel;

		private UXTable chatItemTable;

		private uint shareButtonTimer;

		public SquadScreenChatInputView(SquadSlidingScreen screen) : base(screen)
		{
		}

		public override void OnScreenLoaded()
		{
			string initText = this.lang.Get("s_Chat", new object[0]) + "...";
			this.chatInputBox = this.screen.GetElement<UXInput>("LabelChatInput");
			this.chatInputBox.InitText(initText);
			this.chatInputScript = this.chatInputBox.GetUIInputComponent();
			this.chatInputScript.onValidate = new UIInput.OnValidate(LangUtils.OnValidateWNewLines);
			this.chatInputScript.label.maxLineCount = 1;
			this.requestInputBox = this.screen.GetElement<UXInput>("LabelRequestInput");
			this.requestInputBox.InitText(this.lang.Get("REQUEST_TROOPS_DEFAULT", new object[0]));
			this.requestInputScript = this.requestInputBox.GetUIInputComponent();
			this.requestInputScript.onValidate = new UIInput.OnValidate(LangUtils.OnValidateWNewLines);
			this.requestInputScript.label.maxLineCount = 1;
			this.defaultShareText = this.lang.Get("BATTLE_REPLAY_SHARE_DEFAULT", new object[0]);
			this.shareInputBox = this.screen.GetElement<UXInput>("LabeShareReplayInput");
			this.shareInputBox.InitText(this.defaultShareText);
			this.shareInputScript = this.shareInputBox.GetUIInputComponent();
			this.shareInputScript.onValidate = new UIInput.OnValidate(LangUtils.OnValidateWNewLines);
			this.shareInputScript.label.maxLineCount = 1;
			this.shareOppNameLabel = this.screen.GetElement<UXLabel>("LabelOpponentNameShare");
			this.shareTypeLabel = this.screen.GetElement<UXLabel>("LabelReplayTypeShare");
			this.shareDamagePercentLabel = this.screen.GetElement<UXLabel>("LabelDamageShare");
			this.shareMedalsLabel = this.screen.GetElement<UXLabel>("LabelReplayMedalsShare");
			this.share1StarSprite = this.screen.GetElement<UXSprite>("SpriteStar1Share");
			this.share2StarSprite = this.screen.GetElement<UXSprite>("SpriteStar2Share");
			this.share3StarSprite = this.screen.GetElement<UXSprite>("SpriteStar3Share");
			UXButton element = this.screen.GetElement<UXButton>("BtnSendChat");
			element.OnClicked = new UXButtonClickedDelegate(this.OnChatMessageSend);
			this.requestTroopBtn = this.screen.GetElement<UXButton>("BtnRequestTroops");
			this.requestTroopBtn.OnClicked = new UXButtonClickedDelegate(this.OnTroopRequestClicked);
			this.resendRequestTroopBtn = this.screen.GetElement<UXButton>("BtnResendRequest");
			this.resendRequestTroopBtn.OnClicked = new UXButtonClickedDelegate(this.OnTroopRequestClicked);
			this.resendRequestTroopCostLabel = this.screen.GetElement<UXLabel>("LabelBtnBtnResendRequestCost");
			this.shareReplayButton = this.screen.GetElement<UXButton>("BtnShareReplay");
			this.shareReplayButton.OnClicked = new UXButtonClickedDelegate(this.OnShareReplayClicked);
			this.inputModeChat = this.screen.GetElement<UXElement>("ChatInput");
			this.inputModeShare = this.screen.GetElement<UXElement>("ChatInputShare");
			this.inputModeRequest = this.screen.GetElement<UXElement>("RequestInput");
			this.chatPanel = this.screen.GetElement<UXElement>("ChatPanel");
			this.chatItemTable = this.screen.GetElement<UXTable>("ChatTable");
		}

		public override void ShowView()
		{
			this.RefreshView();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.SquadChatFilterUpdated);
		}

		public override void HideView()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.SquadChatFilterUpdated);
		}

		public override void RefreshView()
		{
			SquadController squadController = Service.SquadController;
			this.HideAllInputs();
			int panelUnifiedAnchorTopOffset = -141;
			switch (squadController.StateManager.GetSquadScreenChatFilterType())
			{
			case ChatFilterType.ShowAll:
				this.inputModeChat.Visible = true;
				panelUnifiedAnchorTopOffset = -141;
				break;
			case ChatFilterType.Messages:
				this.inputModeChat.Visible = true;
				panelUnifiedAnchorTopOffset = -141;
				break;
			case ChatFilterType.Requests:
				this.inputModeRequest.Visible = true;
				panelUnifiedAnchorTopOffset = -141;
				this.UpdateTroopRequestMode();
				break;
			case ChatFilterType.Replays:
			{
				this.inputModeShare.Visible = true;
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				BattleEntry latestValidPvPBattle = currentPlayer.BattleHistory.GetLatestValidPvPBattle();
				if (latestValidPvPBattle != null)
				{
					this.FillOutReplayShare(latestValidPvPBattle);
					panelUnifiedAnchorTopOffset = -195;
				}
				else
				{
					this.inputModeShare.Visible = false;
					panelUnifiedAnchorTopOffset = -71;
				}
				break;
			}
			case ChatFilterType.Updates:
				panelUnifiedAnchorTopOffset = -71;
				break;
			}
			this.chatPanel.SetPanelUnifiedAnchorTopOffset(panelUnifiedAnchorTopOffset);
			this.chatItemTable.RepositionItems();
		}

		public override void OnDestroyElement()
		{
			if (this.shareButtonTimer != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.shareButtonTimer);
				this.shareButtonTimer = 0u;
			}
		}

		private void UpdateTroopRequestMode()
		{
			SmartEntity smartEntity = (SmartEntity)Service.BuildingLookupController.GetCurrentSquadBuilding();
			if (smartEntity != null)
			{
				int storage = smartEntity.BuildingComp.BuildingType.Storage;
				uint serverTime = Service.ServerAPI.ServerTime;
				uint troopRequestDate = Service.SquadController.StateManager.TroopRequestDate;
				int troopRequestCrystalCost = SquadUtils.GetTroopRequestCrystalCost(serverTime, troopRequestDate);
				if (SquadUtils.GetDonatedTroopStorageUsedByCurrentPlayer() < storage && GameUtils.CanAffordCrystals(troopRequestCrystalCost))
				{
					if (SquadUtils.CanSendFreeTroopRequest(serverTime, troopRequestDate))
					{
						this.requestTroopBtn.Visible = true;
						this.resendRequestTroopBtn.Visible = false;
					}
					else
					{
						this.requestTroopBtn.Visible = false;
						this.resendRequestTroopBtn.Visible = true;
						this.resendRequestTroopCostLabel.Text = troopRequestCrystalCost.ToString();
					}
				}
				else
				{
					this.resendRequestTroopBtn.Visible = false;
					this.requestTroopBtn.Visible = true;
					this.requestTroopBtn.Enabled = false;
				}
			}
		}

		private void HideAllInputs()
		{
			this.inputModeChat.Visible = false;
			this.inputModeShare.Visible = false;
			this.inputModeRequest.Visible = false;
		}

		private void OnChatMessageSend(UXButton button)
		{
			Service.EventManager.SendEvent(EventId.SquadSend, null);
			string text = this.chatInputBox.Text;
			SquadController squadController = Service.SquadController;
			squadController.PublishChatMessage(text);
			this.chatInputBox.Text = string.Empty;
		}

		private void OnChatSubmit()
		{
			UXButton element = this.screen.GetElement<UXButton>("BtnSendChat");
			this.OnChatMessageSend(element);
		}

		private void OnTroopRequestClicked(UXButton btn)
		{
			SquadController squadController = Service.SquadController;
			uint serverTime = Service.ServerAPI.ServerTime;
			uint troopRequestDate = squadController.StateManager.TroopRequestDate;
			bool flag = !SquadUtils.CanSendFreeTroopRequest(serverTime, troopRequestDate);
			string text = this.requestInputBox.Text;
			if (!Service.ProfanityController.IsValid(text, false))
			{
				AlertScreen.ShowModal(false, null, this.lang.Get("INVALID_TEXT", new object[0]), null, null);
				return;
			}
			if (flag)
			{
				squadController.ShowTroopRequestScreen(text, false);
			}
			else
			{
				squadController.SendTroopRequest(text, false);
			}
		}

		private void OnShareReplayClicked(UXButton btn)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			BattleEntry latestValidPvPBattle = currentPlayer.BattleHistory.GetLatestValidPvPBattle();
			if (latestValidPvPBattle != null)
			{
				string text = this.shareInputBox.Text;
				if (string.IsNullOrEmpty(text))
				{
					text = this.defaultShareText;
				}
				SquadMsg message = SquadMsgUtils.CreateSendReplayMessage(latestValidPvPBattle.RecordID, text);
				SquadController squadController = Service.SquadController;
				squadController.TakeAction(message);
				this.shareInputBox.Text = string.Empty;
				this.shareInputBox.InitText(this.defaultShareText);
				btn.Enabled = false;
				this.shareButtonTimer = Service.ViewTimerManager.CreateViewTimer(5f, false, new TimerDelegate(this.OnShareButtonTimer), btn);
			}
		}

		private void OnShareButtonTimer(uint id, object cookie)
		{
			if (cookie != null)
			{
				this.shareButtonTimer = 0u;
				UXButton uXButton = (UXButton)cookie;
				uXButton.Enabled = true;
			}
		}

		private void FillOutReplayShare(BattleEntry latest)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			bool flag = latest.AttackerID == currentPlayer.PlayerId;
			string text = (!flag) ? latest.Attacker.PlayerName : latest.Defender.PlayerName;
			string text2 = (!flag) ? this.lang.Get("SQUAD_DEFENSE", new object[0]) : this.lang.Get("SQUAD_OFFENSE", new object[0]);
			BattleParticipant battleParticipant = (!flag) ? latest.Defender : latest.Attacker;
			int num = GameUtils.CalcuateMedals(battleParticipant.AttackRating, battleParticipant.DefenseRating);
			int num2 = GameUtils.CalcuateMedals(battleParticipant.AttackRating + battleParticipant.AttackRatingDelta, battleParticipant.DefenseRating + battleParticipant.DefenseRatingDelta);
			int value = num2 - num;
			this.shareOppNameLabel.Text = text;
			this.shareTypeLabel.Text = text2;
			this.shareDamagePercentLabel.Text = this.lang.Get("PERCENTAGE", new object[]
			{
				latest.DamagePercent
			});
			this.shareMedalsLabel.Text = this.lang.ThousandsSeparated(value);
			this.share1StarSprite.Visible = true;
			this.share2StarSprite.Visible = true;
			this.share3StarSprite.Visible = true;
			this.share1StarSprite.Color = ((latest.EarnedStars <= 0) ? this.grayOut : Color.white);
			this.share2StarSprite.Color = ((latest.EarnedStars <= 1) ? this.grayOut : Color.white);
			this.share3StarSprite.Color = ((latest.EarnedStars <= 2) ? this.grayOut : Color.white);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SquadChatFilterUpdated)
			{
				this.RefreshView();
			}
			return EatResponse.NotEaten;
		}

		public override bool IsVisible()
		{
			return this.chatInputBox.Visible;
		}
	}
}
