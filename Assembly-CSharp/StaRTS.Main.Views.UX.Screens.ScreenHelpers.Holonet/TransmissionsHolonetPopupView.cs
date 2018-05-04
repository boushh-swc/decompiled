using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Tournament;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Story;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet
{
	public class TransmissionsHolonetPopupView
	{
		private const string SHOW_CONFLICT_END = "Show";

		private const string BTN_GOLD = "BtnGold";

		private const string BTN_BLUE = "BtnBlue";

		private const string HOLONET_CHARACTER_HOLDER = "TransmissionHoloHolder";

		private const string TRANSMISSIONS_GROUP = "IncomingTransmissionsGroup";

		private const string BTN_PREV_TRANSMISSION = "BtnTransmissionPrev";

		private const string BTN_NEXT_TRANSMISSION = "BtnTransmissionNext";

		private const string LABEL_TRANSMISSION_HEADER = "CurrentTransmissionLabel";

		private const string GENERIC_TRANSMISSION_GROUP = "TransmissionGenericGroup";

		private const string LABEL_GENERIC_TRANSMISSION_TITLE = "TransmissionMessageTitle";

		private const string LABEL_GENERIC_TRANSMISSION_BODY = "TransmissionMessageBody";

		private const string TRANSMISSION_BUTTON_TABLE = "TransmissionButtonTable";

		private const string TRANSMISSTION_BUTTON_TEMPLATE = "TransmissionButtonTableItem";

		private const string LABEL_TRANSMISSION_BUTTON = "LabelBtnPrimary";

		private const string SPRITE_TRANSMISSION_BUTTON = "SpriteBkgPrimary";

		private const string TRANSMISSION_MESSAGE_TEXT = "TransmissionMessageText";

		private const string TRANSMISSION_BTN_MSG = "LabelBtnMessage";

		private const string TRANSMISSION_CELEB_MSG_LABEL = "LabelCelebrationMessage";

		private const string CONFLICT_CONTAINER = "EndConflictContainer";

		private const string LABEL_CONFLICT_MESSAGE = "EndConflictLabelMessage";

		private const string LABEL_TIER = "LabelFinalTierTitle";

		private const string ICON_TIER = "SpriteFinalTierIcon";

		private const string LABEL_FINAL_TIER = "LabelFinalTier";

		private const string LABEL_TIER_PERCENT = "LabelFinalTierPercent";

		private const string REWARD_TITLE = "LabelRewardTitle";

		private const string REWARD_ICON = "SpriteReward";

		private const string REWARD_NAME = "LabelReward";

		private const string LABEL_CONFLICT_BONUS = "EndConflictBonusLabel";

		private const string SPRITE_CONFLICT_BONUS = "EndConflictBonusIcon";

		private const string BATTLE_TRANSMISSION_GROUP = "TransmissionBattleLogGroup";

		private const string LABEL_BATTLE_TRANSMISSION_TITLE = "TransmissionBattleLogTitle";

		private const string LABEL_BATTLE_TRANSMISSION_BODY = "TransmissionBattleLogBody";

		private const string GRID_BATTLE_TRANSMISSION = "GridBattleList";

		private const string TEMPLATE_BATTLE_TRANSMISSION_ITEM = "ItemBattleResult";

		private const string LABEL_BATTLE_TRANSMISSION_MEDALS = "LabelMedalResult";

		private const string CONFLICT_RESULT_GROUP = "ConflictResult1";

		private const string LABEL_BATTLE_TRANSMISSION_CONFLICT_PTS = "LabelConflictResult1";

		private const string SPRITE_BATTLE_TRANSMISSION_CONFLICT = "SpriteConflictResult1";

		private const string BTN_BATTLE_TRANSMISSION_DISMISS = "TransmissionBattlelogBtnPrimary";

		private const string LABEL_BATTLE_TRANSMISSION_DISMISS = "TransmissionBattlelogLabelBtnPrimary";

		private const string BTN_BATTLE_TRANSMISSION_BATTLE_LOG = "TransmissionBattlelogBtnSecondary";

		private const string LABEL_BATTLE_TRANSMISSION_BATTLE_LOG = "TransmissionBattlelogLabelBtnSecondary";

		private const string LABEL_BATTLE_TRANSMISSION_OPPONENT_NAME = "LabelOpponentName";

		private const string LABEL_BATTLE_TRANSMISSION_BATTLE_TIME = "LabelBattleTime";

		private const string CONFLICT_PTS_ITEM_GROUP = "CampaignPoints";

		private const string LABEL_BATTLE_TRANSMISSION_OPPONENT_LEVEL = "LabelOpponentLevel";

		private const string LABEL_BATTLE_TRANSMISSION_MEDALS_DELTA = "LabelMedalCount";

		private const string LABEL_BATTLE_TRANSMISSION_CAMPAIGN_PTS_DELTA = "LabelCampaignPoints";

		private const string SPRITE_BATTLE_TRANSMISSION_CAMPAIGN_PTS = "SpriteCampaignPoints";

		private const string SQUAD_WAR_ENDED_CONTAINER = "EndSquadWarContainer";

		private const string SQUAD_WAR_ENDED_LABEL = "EndSquadWarLabelMessage";

		private const string SQUAD_WAR_ENDED_TITLE = "EndSquadWarLabelTitle";

		private const string SQUAD_WAR_ENDED_BUTTON_NOTE = "EndSquadWarLabelButtonMessage";

		private const string DAILY_CRATE_REWARD_CTA_BTN_ID = "DailyCrateReward";

		private const string BATTLE = "battle";

		private const string DISMISS_BTN_ID_POSTFIX = "_DISMISS";

		private const string CTA_BTN_ID_POSTFIX = "_CTA";

		private const string DISMISS_TEXT = "s_Dismiss";

		private const string CONFLICT_CTA_TEXT = "hn_conflict_reward_cta";

		private const string NEXT_TEXT = "s_Next";

		private const string BATTLE_LOG = "s_BattleLog";

		private const string INCOMING_HEADER = "hn_inc_trans_header";

		private const string TIME_AGO = "TIME_AGO";

		private const string CONFLICT_END_TEXT_PREFIX = "hn_conflict_end_message";

		private const string CONFLICT_TIER_PERCENTILE = "CONFLICT_TIER_PERCENTILE";

		private const string CONFLICT_LEAGUE_AND_DIVISION = "CONFLICT_LEAGUE_AND_DIVISION";

		private const string CONFLICT_END_PRIZE = "CONFLICT_END_PRIZE";

		private const string CONFLICT_TOUR_TIER = "s_YourTier";

		private const string GO_TO_WAR_BOARD_TEXT = "WAR_END_TRANSMISSION_CTA";

		private const string WAR_END_TRANSMISSION_TITLE = "WAR_END_TRANSMISSION_TITLE";

		private const string WAR_END_RESULTS_REBELS_WIN = "WAR_END_RESULTS_REBELS_WIN";

		private const string WAR_END_RESULTS_EMPIRE_WIN = "WAR_END_RESULTS_EMPIRE_WIN";

		private const string WAR_END_RESULTS_DRAW = "WAR_END_RESULTS_DRAW";

		private const string WAR_END_TRANSMISSION_CTA_DESC = "WAR_END_TRANSMISSION_CTA_DESC";

		private const string CONFLICT_BONUS_DESC_TEXT = "hn_conflict_bonus_desc";

		private const string REWARD_DURATION = "REWARD_DURATION";

		private const float ANIM_DELAY = 0.5f;

		private UXElement transmissionsGroups;

		private UXElement genericTransmission;

		private UXElement battleLogTransmission;

		private UXButton previousTransmission;

		private UXButton nextTransmission;

		private UXLabel transmissionsHeader;

		private UXLabel genericTransmissionTitle;

		private UXLabel genericTransmissionBody;

		private UXTable genericTransmissionButtonTable;

		private UXElement genericTransmissionTextGroup;

		private UXLabel genericBtnMsg;

		private UXLabel genericCelebMsg;

		private UXElement conflictContainer;

		private UXLabel conflictMessage;

		private UXSprite conflictTierIcon;

		private UXLabel conflictTierTitle;

		private UXLabel conflictTierLevel;

		private UXLabel conflictTierPercent;

		private UXElement conflictResultGroup;

		private UXLabel battleTransmissionTitle;

		private UXLabel battleTransmissionBody;

		private UXGrid battleTransmissionBattlesGrid;

		private UXLabel battleTransmissionMedalsTotalDelta;

		private UXLabel battleTransmissionConflictPtsTotalDelta;

		private UXSprite battleTransmissionConflictPtsSprite;

		private UXButton battleTransmissionDismiss;

		private UXLabel battleTransmissionDismissLabel;

		private UXButton battleTransmissionBattleLog;

		private UXLabel battleTransmissionBattleLogLabel;

		private UXElement squadWarEndedContainer;

		private HolonetScreen holonetScreen;

		private HoloCharacter currentCharacter;

		private UXElement holoPositioner;

		private TransmissionVO currentTransmission;

		private int currentIndex;

		private int maxIndex;

		private Lang lang;

		private EventManager eventManager;

		private TournamentVO conflictEndVO;

		public TransmissionsHolonetPopupView(HolonetScreen screen)
		{
			this.conflictEndVO = null;
			this.lang = Service.Lang;
			this.eventManager = Service.EventManager;
			this.holonetScreen = screen;
			this.holoPositioner = null;
			this.currentCharacter = null;
			this.currentTransmission = null;
			this.currentIndex = 1;
			this.maxIndex = 0;
			this.InitView();
		}

		private void InitView()
		{
			this.holonetScreen.HideAllTabs();
			this.holoPositioner = this.holonetScreen.GetElement<UXElement>("TransmissionHoloHolder");
			this.transmissionsGroups = this.holonetScreen.GetElement<UXElement>("IncomingTransmissionsGroup");
			this.previousTransmission = this.holonetScreen.GetElement<UXButton>("BtnTransmissionPrev");
			this.previousTransmission.OnClicked = new UXButtonClickedDelegate(this.OnPreviousTransmission);
			this.nextTransmission = this.holonetScreen.GetElement<UXButton>("BtnTransmissionNext");
			this.nextTransmission.OnClicked = new UXButtonClickedDelegate(this.OnNextTransmission);
			this.transmissionsHeader = this.holonetScreen.GetElement<UXLabel>("CurrentTransmissionLabel");
			this.genericTransmission = this.holonetScreen.GetElement<UXElement>("TransmissionGenericGroup");
			this.genericTransmissionTitle = this.holonetScreen.GetElement<UXLabel>("TransmissionMessageTitle");
			this.genericTransmissionBody = this.holonetScreen.GetElement<UXLabel>("TransmissionMessageBody");
			this.genericTransmissionButtonTable = this.holonetScreen.GetElement<UXTable>("TransmissionButtonTable");
			this.genericTransmissionButtonTable.SetTemplateItem("TransmissionButtonTableItem");
			this.genericBtnMsg = this.holonetScreen.GetElement<UXLabel>("LabelBtnMessage");
			this.genericCelebMsg = this.holonetScreen.GetElement<UXLabel>("LabelCelebrationMessage");
			this.genericTransmissionTextGroup = this.holonetScreen.GetElement<UXElement>("TransmissionMessageText");
			this.conflictContainer = this.holonetScreen.GetElement<UXElement>("EndConflictContainer");
			this.conflictMessage = this.holonetScreen.GetElement<UXLabel>("EndConflictLabelMessage");
			this.conflictTierIcon = this.holonetScreen.GetElement<UXSprite>("SpriteFinalTierIcon");
			this.conflictTierTitle = this.holonetScreen.GetElement<UXLabel>("LabelFinalTierTitle");
			this.conflictTierLevel = this.holonetScreen.GetElement<UXLabel>("LabelFinalTier");
			this.conflictTierPercent = this.holonetScreen.GetElement<UXLabel>("LabelFinalTierPercent");
			this.battleLogTransmission = this.holonetScreen.GetElement<UXElement>("TransmissionBattleLogGroup");
			this.conflictResultGroup = this.holonetScreen.GetElement<UXElement>("ConflictResult1");
			this.battleTransmissionTitle = this.holonetScreen.GetElement<UXLabel>("TransmissionBattleLogTitle");
			this.battleTransmissionBody = this.holonetScreen.GetElement<UXLabel>("TransmissionBattleLogBody");
			this.battleTransmissionBattlesGrid = this.holonetScreen.GetElement<UXGrid>("GridBattleList");
			this.battleTransmissionMedalsTotalDelta = this.holonetScreen.GetElement<UXLabel>("LabelMedalResult");
			this.battleTransmissionConflictPtsTotalDelta = this.holonetScreen.GetElement<UXLabel>("LabelConflictResult1");
			this.battleTransmissionConflictPtsSprite = this.holonetScreen.GetElement<UXSprite>("SpriteConflictResult1");
			this.battleTransmissionDismiss = this.holonetScreen.GetElement<UXButton>("TransmissionBattlelogBtnPrimary");
			this.battleTransmissionDismiss.OnClicked = new UXButtonClickedDelegate(this.OnDismissTransmissionClicked);
			this.battleTransmissionDismissLabel = this.holonetScreen.GetElement<UXLabel>("TransmissionBattlelogLabelBtnPrimary");
			this.battleTransmissionBattleLog = this.holonetScreen.GetElement<UXButton>("TransmissionBattlelogBtnSecondary");
			this.battleTransmissionBattleLog.OnClicked = new UXButtonClickedDelegate(this.OnBattleLog);
			this.battleTransmissionBattleLogLabel = this.holonetScreen.GetElement<UXLabel>("TransmissionBattlelogLabelBtnSecondary");
			this.squadWarEndedContainer = this.holonetScreen.GetElement<UXElement>("EndSquadWarContainer");
			Service.HolonetController.TransmissionsController.OnTransmissionPopupIntialized(this);
			this.eventManager.SendEvent(EventId.HolonetTabOpened, "incoming_transmission");
		}

		private void OnNextTransmission(UXButton btn)
		{
			this.eventManager.SendEvent(EventId.HolonetIncomingTransmission, "next|" + this.GetBiUid());
			if (this.maxIndex > 1)
			{
				this.eventManager.SendEvent(EventId.HolonetNextPrevTransmision, null);
				this.currentIndex++;
				if (this.currentIndex > this.maxIndex)
				{
					this.currentIndex = this.maxIndex;
				}
				Service.HolonetController.UpdateIncomingTransmission(this.currentIndex);
			}
		}

		private void OnPreviousTransmission(UXButton btn)
		{
			this.eventManager.SendEvent(EventId.HolonetIncomingTransmission, "previous|" + this.GetBiUid());
			if (this.maxIndex > 1)
			{
				this.eventManager.SendEvent(EventId.HolonetNextPrevTransmision, null);
				this.currentIndex--;
				if (this.currentIndex < 1)
				{
					this.currentIndex = 1;
				}
				Service.HolonetController.UpdateIncomingTransmission(this.currentIndex);
			}
		}

		private void OnDismissTransmissionClicked(UXButton btn)
		{
			this.DismissTransmission();
			this.eventManager.SendEvent(EventId.HolonetIncomingTransmission, "dismiss|" + this.GetBiUid());
		}

		private void CloseTransmission()
		{
			this.eventManager.SendEvent(EventId.HoloEvent, "sfx_ui_hologram_off");
			this.CleanUp();
			this.ReturnToHolonet();
		}

		private void OnGenericCTA(UXButton btn)
		{
			this.CloseTransmission();
			string cookie = (!(this.currentTransmission.Btn1Action.ToLower() == "reward")) ? this.currentTransmission.TransData : this.currentTransmission.Uid;
			Service.HolonetController.HandleCallToActionButton(this.currentTransmission.Btn1Action.ToLower(), this.currentTransmission.Btn1Data, cookie);
			string cookie2 = this.currentTransmission.Btn1Action + "|" + this.GetBiUid() + "|cta_button";
			this.eventManager.SendEvent(EventId.HolonetIncomingTransmission, cookie2);
		}

		private void OnBattleLog(UXButton btn)
		{
			this.CloseTransmission();
			Service.HolonetController.HandleCallToActionButton("battle", this.currentTransmission.Btn1Data);
			this.eventManager.SendEvent(EventId.HolonetIncomingTransmission, "battle_log|" + this.GetBiUid() + "|cta_button");
		}

		private void DismissTransmission()
		{
			this.CloseTransmission();
		}

		public void CleanUp()
		{
			Service.HolonetController.TransmissionsController.OnTransmissionPopupClosed();
			this.eventManager.SendEvent(EventId.HolonetTabClosed, "incoming_transmission");
			if (this.currentCharacter != null)
			{
				this.currentCharacter.Destroy();
				this.currentCharacter = null;
			}
			if (this.battleTransmissionBattlesGrid != null)
			{
				this.battleTransmissionBattlesGrid.Clear();
				this.battleTransmissionBattlesGrid = null;
			}
		}

		private void ReturnToHolonet()
		{
			if (this.holonetScreen != null)
			{
				this.transmissionsGroups.Visible = false;
				this.holonetScreen.ShowDefaultTabs();
				this.holonetScreen = null;
			}
		}

		private void DisableNextPrevButtons()
		{
			this.previousTransmission.Enabled = false;
			this.nextTransmission.Enabled = false;
		}

		private void EnableNextPrevButtons()
		{
			this.previousTransmission.Enabled = true;
			this.nextTransmission.Enabled = true;
		}

		private void ShowHoloCharacter(string characterId)
		{
			if (this.currentCharacter != null)
			{
				this.currentCharacter.ChangeCharacter(characterId);
			}
			else
			{
				Vector3 vector = this.holoPositioner.LocalPosition;
				vector = this.holonetScreen.UXCamera.Camera.ScreenToViewportPoint(vector);
				this.currentCharacter = new HoloCharacter(characterId, vector);
			}
			this.currentCharacter.OnDoneLoading = new Action(this.EnableNextPrevButtons);
		}

		private void SetupBattleTransmission(TransmissionVO transmission)
		{
			this.battleLogTransmission.Visible = true;
			this.genericTransmission.Visible = false;
			this.SetupNextOrDismissBattleButton();
			if (this.battleTransmissionBattlesGrid.Count > 0)
			{
				return;
			}
			this.conflictResultGroup.Visible = false;
			this.battleTransmissionTitle.Text = LangUtils.GetHolonetTransmissionCharacterName(transmission);
			this.battleTransmissionBody.Text = LangUtils.GetHolonetBattleTransmissionDescText(transmission);
			this.battleTransmissionMedalsTotalDelta.Text = transmission.TotalPvpRatingDelta.ToString();
			TournamentController tournamentController = Service.TournamentController;
			bool flag = false;
			int count = transmission.AttackerData.Count;
			string planetId = string.Empty;
			int num = 0;
			this.battleTransmissionBattlesGrid.SetTemplateItem("ItemBattleResult");
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = transmission.AttackerData[i];
				string itemUid = battleEntry.AttackerID + i.ToString();
				UXElement item = this.battleTransmissionBattlesGrid.CloneTemplateItem(itemUid);
				BattleParticipant defender = battleEntry.Defender;
				int num2 = GameUtils.CalcuateMedals(defender.AttackRating, defender.DefenseRating);
				int num3 = GameUtils.CalcuateMedals(defender.AttackRating + defender.AttackRatingDelta, defender.DefenseRating + defender.DefenseRatingDelta);
				int delta = num3 - num2;
				string deltaString = LangUtils.GetDeltaString(delta);
				this.battleTransmissionBattlesGrid.GetSubElement<UXLabel>(itemUid, "LabelOpponentLevel").Visible = false;
				this.battleTransmissionBattlesGrid.GetSubElement<UXLabel>(itemUid, "LabelOpponentName").Text = battleEntry.Attacker.PlayerName;
				this.battleTransmissionBattlesGrid.GetSubElement<UXLabel>(itemUid, "LabelMedalCount").Text = deltaString;
				string timeLabelFromSeconds = GameUtils.GetTimeLabelFromSeconds((int)(ServerTime.Time - battleEntry.EndBattleServerTime));
				this.battleTransmissionBattlesGrid.GetSubElement<UXLabel>(itemUid, "LabelBattleTime").Text = this.lang.Get("TIME_AGO", new object[]
				{
					timeLabelFromSeconds
				});
				bool flag2 = defender.TournamentRatingDelta != 0 && tournamentController.IsBattleInCurrentTournament(battleEntry);
				this.battleTransmissionBattlesGrid.GetSubElement<UXElement>(itemUid, "CampaignPoints").Visible = flag2;
				if (flag2 && !string.IsNullOrEmpty(battleEntry.PlanetId))
				{
					flag = true;
					planetId = battleEntry.PlanetId;
					num += defender.TournamentRatingDelta;
					string deltaString2 = LangUtils.GetDeltaString(defender.TournamentRatingDelta);
					this.battleTransmissionBattlesGrid.GetSubElement<UXLabel>(itemUid, "LabelCampaignPoints").Text = deltaString2;
					this.battleTransmissionBattlesGrid.GetSubElement<UXSprite>(itemUid, "SpriteCampaignPoints").SpriteName = GameUtils.GetTournamentPointIconName(planetId);
				}
				this.battleTransmissionBattlesGrid.AddItem(item, i);
			}
			if (flag)
			{
				this.conflictResultGroup.Visible = true;
				this.battleTransmissionConflictPtsTotalDelta.Text = LangUtils.GetDeltaString(num);
				this.battleTransmissionConflictPtsSprite.SpriteName = GameUtils.GetTournamentPointIconName(planetId);
			}
			this.battleTransmissionBattlesGrid.RepositionItems();
			this.battleTransmissionBattleLogLabel.Text = this.lang.Get("s_BattleLog", new object[0]);
		}

		private void ShowGenericTransmissionGroup(bool showBtnMsg)
		{
			this.genericTransmission.Visible = true;
			this.genericTransmissionTitle.Visible = true;
			this.genericBtnMsg.Visible = showBtnMsg;
			this.genericCelebMsg.Visible = false;
		}

		private void SetupGenericTransmission(TransmissionVO transmission)
		{
			this.battleLogTransmission.Visible = false;
			this.ShowGenericTransmissionGroup(false);
			this.conflictContainer.Visible = false;
			this.squadWarEndedContainer.Visible = false;
			this.genericTransmissionTextGroup.Visible = true;
			this.SetupTransmissionButtons(transmission);
			this.genericTransmissionTitle.Text = this.lang.Get(transmission.TitleText, new object[0]);
			this.genericTransmissionBody.Text = this.lang.Get(transmission.BodyText, new object[0]);
		}

		private void SetupTransmissionButtons(TransmissionVO transmission)
		{
			this.genericTransmissionButtonTable.Clear();
			string btnID = transmission.Uid + "_DISMISS";
			string displayText = string.Empty;
			UXButtonClickedDelegate onClicked;
			if (this.currentIndex >= this.maxIndex)
			{
				displayText = this.lang.Get("s_Dismiss", new object[0]);
				onClicked = new UXButtonClickedDelegate(this.OnDismissTransmissionClicked);
			}
			else
			{
				displayText = this.lang.Get("s_Next", new object[0]);
				onClicked = new UXButtonClickedDelegate(this.OnNextTransmission);
			}
			this.CreateGenericTableButton(btnID, displayText, "BtnGold", onClicked, 1);
			if (!string.IsNullOrEmpty(transmission.Btn1Action))
			{
				string btnID2 = transmission.Uid + "_CTA";
				string cTAButtonText = this.GetCTAButtonText(transmission);
				this.CreateGenericTableButton(btnID2, cTAButtonText, "BtnBlue", new UXButtonClickedDelegate(this.OnGenericCTA), 0);
			}
			this.genericTransmissionButtonTable.RepositionItems();
		}

		private void CreateGenericTableButton(string btnID, string displayText, string spriteName, UXButtonClickedDelegate onClicked, int index)
		{
			UXButton uXButton = (UXButton)this.genericTransmissionButtonTable.CloneTemplateItem(btnID);
			uXButton.OnClicked = onClicked;
			UXLabel subElement = this.genericTransmissionButtonTable.GetSubElement<UXLabel>(btnID, "LabelBtnPrimary");
			subElement.Text = displayText;
			UXSprite subElement2 = this.genericTransmissionButtonTable.GetSubElement<UXSprite>(btnID, "SpriteBkgPrimary");
			subElement2.SpriteName = spriteName;
			this.genericTransmissionButtonTable.AddItem(uXButton, index);
		}

		private string GetCTAButtonText(TransmissionVO vo)
		{
			string result = string.Empty;
			switch (vo.Type)
			{
			case TransmissionType.Conflict:
				result = this.lang.Get("hn_conflict_reward_cta", new object[0]);
				return result;
			case TransmissionType.WarPreparation:
			case TransmissionType.WarStart:
			case TransmissionType.WarEnded:
				result = this.lang.Get("WAR_END_TRANSMISSION_CTA", new object[0]);
				return result;
			case TransmissionType.GuildLevelUp:
				result = this.lang.Get("hn_perks_squad_level_up_cta", new object[0]);
				return result;
			case TransmissionType.DailyCrateReward:
				result = this.lang.Get("hn_daily_crate_reward_cta", new object[0]);
				return result;
			}
			result = this.lang.Get(vo.Btn1, new object[0]);
			return result;
		}

		private void SetupNextOrDismissBattleButton()
		{
			if (this.currentIndex >= this.maxIndex)
			{
				this.battleTransmissionDismissLabel.Text = this.lang.Get("s_Dismiss", new object[0]);
				this.battleTransmissionDismiss.OnClicked = new UXButtonClickedDelegate(this.OnDismissTransmissionClicked);
			}
			else
			{
				this.battleTransmissionDismissLabel.Text = this.lang.Get("s_Next", new object[0]);
				this.battleTransmissionDismiss.OnClicked = new UXButtonClickedDelegate(this.OnNextTransmission);
			}
		}

		private void SetupConflictTransmission(TransmissionVO transmission)
		{
			this.battleLogTransmission.Visible = false;
			this.ShowGenericTransmissionGroup(false);
			this.conflictContainer.Visible = true;
			this.squadWarEndedContainer.Visible = false;
			this.genericTransmissionTextGroup.Visible = false;
			this.SetupTransmissionButtons(transmission);
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.conflictEndVO = staticDataController.GetOptional<TournamentVO>(transmission.TransData);
			TournamentTierVO optional = staticDataController.GetOptional<TournamentTierVO>(transmission.Btn1Data);
			if (this.conflictEndVO == null || optional == null)
			{
				return;
			}
			Tournament tournament = currentPlayer.TournamentProgress.GetTournament(this.conflictEndVO.Uid);
			if (tournament == null)
			{
				Service.Logger.ErrorFormat("Tournament doesn't exist for player. tournament {0}", new object[]
				{
					this.conflictEndVO.Uid
				});
				this.conflictMessage.Visible = false;
				this.conflictTierIcon.Visible = false;
				return;
			}
			TournamentRank finalRank = tournament.FinalRank;
			string text = LangUtils.AppendPlayerFactionToKey("hn_conflict_end_message");
			this.conflictMessage.Visible = true;
			this.conflictTierIcon.Visible = true;
			this.conflictMessage.Text = this.lang.Get(text.ToString(), new object[]
			{
				LangUtils.GetPlanetDisplayName(this.conflictEndVO.PlanetId)
			});
			this.conflictTierIcon.SpriteName = Service.TournamentController.GetTierIconName(optional);
			this.conflictTierTitle.Text = this.lang.Get("s_YourTier", new object[0]);
			this.conflictTierLevel.Text = this.lang.Get("CONFLICT_LEAGUE_AND_DIVISION", new object[]
			{
				this.lang.Get(optional.RankName, new object[0]),
				this.lang.Get(optional.DivisionSmall, new object[0])
			});
			this.conflictTierPercent.Text = this.lang.Get("CONFLICT_TIER_PERCENTILE", new object[]
			{
				Math.Round(finalRank.Percentile, 2)
			});
			bool flag = TimedEventPrizeUtils.TrySetupConflictEndedRewardView(tournament.RedeemedRewards, this.holonetScreen.GetElement<UXLabel>("LabelReward"), this.holonetScreen.GetElement<UXSprite>("SpriteReward"));
			if (flag)
			{
				this.holonetScreen.GetElement<UXLabel>("LabelRewardTitle").Text = this.lang.Get("CONFLICT_END_PRIZE", new object[0]);
			}
			else
			{
				Service.Logger.ErrorFormat("There is no reward given to player for tournament {0}", new object[]
				{
					this.conflictEndVO.Uid
				});
			}
			Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.ShowReward), null);
		}

		private void ShowReward(uint timerId, object cookie)
		{
			Animator component = this.conflictContainer.Root.GetComponent<Animator>();
			if (component != null && component.isActiveAndEnabled)
			{
				component.SetTrigger("Show");
			}
		}

		private void SetupSquadWarPreparedTransmission(TransmissionVO transmission)
		{
			this.battleLogTransmission.Visible = false;
			this.ShowGenericTransmissionGroup(false);
			this.conflictContainer.Visible = false;
			this.squadWarEndedContainer.Visible = false;
			this.genericTransmissionTextGroup.Visible = true;
			this.SetupTransmissionButtons(transmission);
			this.genericTransmissionTitle.Text = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_prep_title"), new object[0]);
			this.genericTransmissionBody.Text = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_prep_body"), new object[0]);
		}

		private void SetupSquadWarStartedTransmission(TransmissionVO transmission)
		{
			this.battleLogTransmission.Visible = false;
			this.ShowGenericTransmissionGroup(false);
			this.conflictContainer.Visible = false;
			this.squadWarEndedContainer.Visible = false;
			this.genericTransmissionTextGroup.Visible = true;
			this.SetupTransmissionButtons(transmission);
			this.genericTransmissionTitle.Text = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_start_title"), new object[0]);
			this.genericTransmissionBody.Text = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_start_body"), new object[0]);
		}

		private void SetupSquadLevelUpTransmission(TransmissionVO transmission)
		{
			this.genericTransmissionButtonTable.Clear();
			this.battleLogTransmission.Visible = false;
			this.ShowGenericTransmissionGroup(true);
			this.conflictContainer.Visible = false;
			this.squadWarEndedContainer.Visible = false;
			this.genericTransmissionTextGroup.Visible = true;
			string btnID = transmission.Uid + "_CTA";
			string cTAButtonText = this.GetCTAButtonText(transmission);
			this.CreateGenericTableButton(btnID, cTAButtonText, "BtnGold", new UXButtonClickedDelegate(this.OnGenericCTA), 0);
			this.genericTransmissionTitle.Visible = false;
			this.genericTransmissionBody.Text = this.lang.Get("hn_perks_squad_level_up_body", new object[]
			{
				transmission.SquadLevel
			});
			this.genericBtnMsg.Text = this.lang.Get("hn_perks_squad_level_up_cta_desc", new object[0]);
		}

		private void SetupDailyCrateRewardTransmission(TransmissionVO transmission)
		{
			this.genericTransmissionButtonTable.Clear();
			this.battleLogTransmission.Visible = false;
			this.ShowGenericTransmissionGroup(true);
			this.conflictContainer.Visible = false;
			this.squadWarEndedContainer.Visible = false;
			this.genericTransmissionTextGroup.Visible = true;
			string btnID = "DailyCrateReward_CTA";
			string cTAButtonText = this.GetCTAButtonText(transmission);
			this.CreateGenericTableButton(btnID, cTAButtonText, "BtnGold", new UXButtonClickedDelegate(this.OnGenericCTA), 0);
			this.genericTransmissionTitle.Text = this.lang.Get("hn_daily_crate_reward_title", new object[0]);
			CrateVO crateVO = Service.StaticDataController.Get<CrateVO>(transmission.CrateId);
			this.genericTransmissionBody.Text = this.lang.Get("hn_daily_crate_reward_body", new object[]
			{
				LangUtils.GetCrateDisplayName(crateVO)
			});
			this.genericBtnMsg.Visible = false;
		}

		private void SetupSquadWarEndedTransmission(TransmissionVO transmission)
		{
			this.genericTransmissionButtonTable.Clear();
			this.battleLogTransmission.Visible = false;
			this.ShowGenericTransmissionGroup(false);
			this.conflictContainer.Visible = false;
			this.squadWarEndedContainer.Visible = true;
			this.genericTransmissionTextGroup.Visible = false;
			int empireScore = transmission.EmpireScore;
			int rebelScore = transmission.RebelScore;
			string text = this.lang.Get("WAR_END_RESULTS_DRAW", new object[0]);
			if (empireScore > rebelScore)
			{
				text = this.lang.Get("WAR_END_RESULTS_EMPIRE_WIN", new object[]
				{
					transmission.EmpireSquadName
				});
			}
			else if (rebelScore > empireScore)
			{
				text = this.lang.Get("WAR_END_RESULTS_REBELS_WIN", new object[]
				{
					transmission.RebelSquadName
				});
			}
			UXLabel element = this.holonetScreen.GetElement<UXLabel>("EndSquadWarLabelMessage");
			element.Text = text;
			UXLabel element2 = this.holonetScreen.GetElement<UXLabel>("EndSquadWarLabelTitle");
			element2.Text = this.lang.Get("WAR_END_TRANSMISSION_TITLE", new object[0]);
			UXLabel element3 = this.holonetScreen.GetElement<UXLabel>("EndSquadWarLabelButtonMessage");
			element3.Text = this.lang.Get("WAR_END_TRANSMISSION_CTA_DESC", new object[0]);
			string btnID = transmission.Uid + "_CTA";
			string cTAButtonText = this.GetCTAButtonText(transmission);
			this.CreateGenericTableButton(btnID, cTAButtonText, "BtnGold", new UXButtonClickedDelegate(this.OnGenericCTA), 0);
			this.genericTransmissionButtonTable.RepositionItems();
		}

		public void SetMaxTransmissionIndex(int maxTransmissionIndex)
		{
			this.maxIndex = maxTransmissionIndex;
		}

		public void IncrementMaxTransmissionsIndex()
		{
			this.maxIndex++;
		}

		public void UpdateNextPrevTransmissionButtonsVisibility()
		{
			this.previousTransmission.Visible = true;
			this.nextTransmission.Visible = true;
			if (this.maxIndex == 1)
			{
				this.previousTransmission.Visible = false;
				this.nextTransmission.Visible = false;
			}
			else if (this.currentIndex <= 1)
			{
				this.previousTransmission.Visible = false;
			}
			else if (this.currentIndex >= this.maxIndex)
			{
				this.nextTransmission.Visible = false;
			}
		}

		public void RefreshView(TransmissionVO transmission)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.transmissionsGroups.Visible = true;
			this.currentTransmission = transmission;
			this.transmissionsHeader.Text = this.lang.Get("hn_inc_trans_header", new object[]
			{
				this.currentIndex,
				this.maxIndex
			});
			if (string.IsNullOrEmpty(transmission.CharacterID))
			{
				transmission.CharacterID = GameUtils.GetTransmissionHoloId(currentPlayer.Faction, currentPlayer.Planet.Uid);
			}
			this.UpdateNextPrevTransmissionButtonsVisibility();
			this.DisableNextPrevButtons();
			this.ShowHoloCharacter(transmission.CharacterID);
			switch (transmission.Type)
			{
			case TransmissionType.Generic:
				this.SetupGenericTransmission(transmission);
				return;
			case TransmissionType.Battle:
				this.SetupBattleTransmission(transmission);
				return;
			case TransmissionType.Conflict:
				this.SetupConflictTransmission(transmission);
				return;
			case TransmissionType.WarPreparation:
				this.SetupSquadWarPreparedTransmission(transmission);
				return;
			case TransmissionType.WarStart:
				this.SetupSquadWarStartedTransmission(transmission);
				return;
			case TransmissionType.WarEnded:
				this.SetupSquadWarEndedTransmission(transmission);
				return;
			case TransmissionType.GuildLevelUp:
				this.SetupSquadLevelUpTransmission(transmission);
				return;
			case TransmissionType.DailyCrateReward:
				this.SetupDailyCrateRewardTransmission(transmission);
				return;
			}
			Service.Logger.ErrorFormat("Unkown Transmission Type {0}", new object[]
			{
				transmission.Type
			});
		}

		public string GetBITabName()
		{
			return "incoming_transmission|" + this.GetBiUid();
		}

		public string GetBiUid()
		{
			string result = string.Empty;
			if (this.currentTransmission.Type == TransmissionType.Generic)
			{
				result = this.currentTransmission.Uid;
			}
			else
			{
				result = this.currentTransmission.Type.ToString().ToLower();
			}
			return result;
		}
	}
}
