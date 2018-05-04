using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class FactionFlipScreen : ClosableScreen
	{
		private const string LABEL_TITLE = "LabelFactionSecondTitle";

		private const string LABEL_LOCKED = "LabelLockedCondition";

		private const string LABEL_CURRENT = "LabelFactionCurrent";

		private const string LABEL_CURRENT_CALL_SIGN = "LabelCallsignCurrent";

		private const string LABEL_CURRENT_CALL_SIGN_NAME = "LabelCallsignNameCurrent";

		private const string LABEL_CURRENT_HQ_LEVEL = "LabelHQLEVELOther";

		private const string LABEL_CURRENT_HQ_LEVEL_COUNT = "LabelHQLEVELCountOther";

		private const string LABEL_CURRENT_MEDAL = "LabelMedalOther";

		private const string LABEL_CURRENT_MEDAL_COUNT = "LabelMedalCountOther";

		private const string LABEL_CURRENT_SQUAD = "LabelSquadOther";

		private const string LABEL_CURRENT_SQUAD_NAME = "LabelSquadNameOther";

		private const string LABEL_INSTRUCTIONS = "LabelFactionInstructions";

		private const string LABEL_CALL_SIGN = "LabelCallsign";

		private const string LABEL_CALL_SIGN_NAME = "LabelCallsignName";

		private const string LABEL_HQ_LEVEL = "LabelHQLEVEL";

		private const string LABEL_MEDAL_COUNT = "LabelMedalCount";

		private const string LABEL_SQUAD = "LabelSquad";

		private const string LABEL_SQUAD_NAME = "LabelSquadName";

		private const string LABEL_LEARN_MORE = "LabelLearnMore";

		private const string LABEL_BUTTON_SWAP = "LabelBtnFactionSwap";

		private const string LABEL_BUTTON_LEARN_MORE = "LabelBtnLearnMore";

		private const string SPRITE_BUTTON_FACTION = "SpriteBkgBtnFactionSwap";

		private const string SPRITE_LOCKED = "SpriteFactionLockedIcon";

		private const string ICON_BUTTON_EMPIRE = "btnEmpire";

		private const string ICON_BUTTON_REBEL = "btnRebel";

		private const string BUTTON_BACK = "BtnBack";

		private const string BUTTON_LEARN_MORE = "BtnLearnMore";

		private const string BUTTON_SWAP = "BtnFactionSwap";

		private const string GROUP_FACTION = "Faction";

		private const string GROUP_FACTION_SWITCH = "FactionSwitch";

		private const string GROUP_LEARN_MORE = "LearnMore";

		private const string GROUP_STATS = "BaseStats";

		private const string TEXTURE_HOLDER = "SpriteFactionSwitchImage";

		private const string TEXTURE_EMPIRE = "FactionEmpire";

		private const string TEXTURE_REBEL = "FactionRebel";

		private const string REBEL = "Rebel";

		private const string EMPIRE = "Empire";

		private UXButton backButton;

		private UXButton swapButton;

		private UXElement groupFactionSwitch;

		private UXElement groupLearnMore;

		private UXElement groupStats;

		private UXLabel titleLabel;

		private bool locked;

		private PlayerIdentityInfo oppositePlayerInfo;

		private FactionType currentFaction;

		private FactionType oppositeFaction;

		private string titleText;

		private string factionSuffix;

		public FactionFlipScreen() : base("gui_faction_second")
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.NumIdentities == 1)
			{
				int num = currentPlayer.Map.FindHighestHqLevel();
				this.locked = (num < GameConstants.FACTION_FLIPPING_UNLOCK_LEVEL);
			}
			this.currentFaction = currentPlayer.Faction;
			this.oppositeFaction = ((this.currentFaction != FactionType.Empire) ? FactionType.Empire : FactionType.Rebel);
			if (!this.locked && currentPlayer.NumIdentities > 1)
			{
				Service.PlayerIdentityController.GetOtherPlayerIdentity(new PlayerIdentityController.GetOtherPlayerIdentityCallback(this.OnGetOtherPlayerIdentity));
			}
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.InitCurrentPlayerInfo();
			this.groupFactionSwitch = base.GetElement<UXElement>("FactionSwitch");
			this.groupLearnMore = base.GetElement<UXElement>("LearnMore");
			this.groupLearnMore.Visible = false;
			this.titleLabel = base.GetElement<UXLabel>("LabelFactionSecondTitle");
			base.GetElement<UXLabel>("LabelBtnLearnMore").Text = this.lang.Get("FACTION_FLIP_LEARN_MORE", new object[0]);
			base.GetElement<UXLabel>("LabelLearnMore").Text = this.lang.Get("FACTION_FLIP_LEARN_MORE_INFO", new object[0]);
			this.factionSuffix = string.Empty;
			FactionType factionType = this.oppositeFaction;
			if (factionType != FactionType.Empire)
			{
				if (factionType == FactionType.Rebel)
				{
					this.factionSuffix = "Rebel";
					this.titleText = this.lang.Get("FACTION_FLIP_REBEL_TITLE", new object[0]);
					this.titleLabel.Text = this.titleText;
					base.GetElement<UXLabel>("LabelBtnFactionSwap").Text = this.lang.Get("FACTION_FLIP_PLAY_REBEL", new object[0]);
					base.GetElement<UXSprite>("SpriteBkgBtnFactionSwap").SpriteName = "btnRebel";
					base.GetElement<UXTexture>("SpriteFactionSwitchImage" + this.factionSuffix).LoadTexture("FactionRebel");
					base.GetElement<UXElement>("FactionEmpire").Visible = false;
				}
			}
			else
			{
				this.factionSuffix = "Empire";
				this.titleText = this.lang.Get("FACTION_FLIP_EMPIRE_TITLE", new object[0]);
				this.titleLabel.Text = this.titleText;
				base.GetElement<UXLabel>("LabelBtnFactionSwap").Text = this.lang.Get("FACTION_FLIP_PLAY_EMPIRE", new object[0]);
				base.GetElement<UXSprite>("SpriteBkgBtnFactionSwap").SpriteName = "btnEmpire";
				base.GetElement<UXTexture>("SpriteFactionSwitchImage" + this.factionSuffix).LoadTexture("FactionEmpire");
				base.GetElement<UXElement>("FactionRebel").Visible = false;
			}
			this.groupStats = base.GetElement<UXElement>("BaseStats" + this.factionSuffix);
			this.groupStats.Visible = false;
			UXLabel element = base.GetElement<UXLabel>("LabelLockedCondition" + this.factionSuffix);
			if (this.locked)
			{
				element.Text = this.lang.Get("FACTION_FLIP_LOCKED", new object[]
				{
					this.lang.Get(this.oppositeFaction.ToString().ToLower(), new object[0]),
					this.lang.Get(this.currentFaction.ToString().ToLower(), new object[0]),
					GameConstants.FACTION_FLIPPING_UNLOCK_LEVEL
				});
			}
			else
			{
				base.GetElement<UXElement>("SpriteFactionLockedIcon" + this.factionSuffix).Visible = false;
				element.Visible = false;
				if (this.oppositePlayerInfo != null)
				{
					this.InitPlayerInfo(this.oppositePlayerInfo);
				}
			}
			if (Service.CurrentPlayer.NumIdentities > 1 || this.locked)
			{
				base.GetElement<UXLabel>("LabelFactionInstructions" + this.factionSuffix).Visible = false;
			}
			else
			{
				base.GetElement<UXLabel>("LabelFactionInstructions" + this.factionSuffix).Text = this.lang.Get("FACTION_FLIP_INSTRUCTIONS", new object[0]);
			}
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.swapButton = base.GetElement<UXButton>("BtnFactionSwap");
			this.swapButton.OnClicked = new UXButtonClickedDelegate(this.OnSwapButtonClicked);
			this.swapButton.Enabled = ((!this.locked && Service.CurrentPlayer.NumIdentities == 1) || this.oppositePlayerInfo != null);
			this.backButton = base.GetElement<UXButton>("BtnBack");
			this.backButton.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			base.GetElement<UXButton>("BtnLearnMore").OnClicked = new UXButtonClickedDelegate(this.OnLearnMoreButtonClicked);
		}

		private void OnLearnMoreButtonClicked(UXButton button)
		{
			this.groupFactionSwitch.Visible = false;
			this.groupLearnMore.Visible = true;
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			base.CurrentBackButton = this.backButton;
			this.titleLabel.Text = this.lang.Get("FACTION_FLIP_LEARN_MORE", new object[0]);
		}

		private void OnBackButtonClicked(UXButton button)
		{
			this.groupFactionSwitch.Visible = true;
			this.groupLearnMore.Visible = false;
			base.InitDefaultBackDelegate();
			this.titleLabel.Text = this.titleText;
		}

		private void OnSwapButtonClicked(UXButton button)
		{
			string pref = Service.ServerPlayerPrefs.GetPref(ServerPref.FactionFlippingSkipConfirmation);
			if (pref == "0")
			{
				Service.ScreenController.AddScreen(new FactionFlipConfirmationScreen(this.currentFaction, this.oppositeFaction, new OnScreenModalResult(this.OnFactionFlipConfirmed)));
				this.Visible = false;
			}
			else
			{
				this.OnFactionFlipConfirmed(true, null);
				Service.EventManager.SendEvent(EventId.UIFactionFlipAction, "faction");
			}
		}

		private void OnFactionFlipConfirmed(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			if (!Convert.ToBoolean(result))
			{
				this.Visible = true;
				return;
			}
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			if (serverPlayerPrefs.GetPref(ServerPref.FactionFlipped) == "0")
			{
				serverPlayerPrefs.SetPref(ServerPref.FactionFlipped, "1");
				Service.ServerAPI.Enqueue(new SetPrefsCommand(false));
			}
			if (this.oppositePlayerInfo != null && !string.IsNullOrEmpty(this.oppositePlayerInfo.PlayerId))
			{
				Service.PlayerIdentityController.SwitchIdentity(this.oppositePlayerInfo.PlayerId);
			}
			else
			{
				Service.PlayerIdentityController.SwitchToNewIdentity();
			}
		}

		private void OnGetOtherPlayerIdentity(PlayerIdentityInfo info)
		{
			this.oppositePlayerInfo = info;
			if (base.IsLoaded())
			{
				this.InitPlayerInfo(this.oppositePlayerInfo);
			}
		}

		private void InitCurrentPlayerInfo()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string str = (this.currentFaction != FactionType.Empire) ? "Rebel" : "Empire";
			Color textColor = base.GetElement<UXLabel>("LabelCallsignName" + str).TextColor;
			base.GetElement<UXLabel>("LabelFactionCurrent").Text = ((this.currentFaction != FactionType.Empire) ? this.lang.Get("FACTION_FLIP_CURRENT_BASE_REBEL", new object[0]) : this.lang.Get("FACTION_FLIP_CURRENT_BASE_EMPIRE", new object[0]));
			base.GetElement<UXLabel>("LabelCallsignCurrent").Text = this.lang.Get("FACTION_FLIP_CALL_SIGN", new object[0]);
			UXLabel element = base.GetElement<UXLabel>("LabelCallsignNameCurrent");
			element.Text = currentPlayer.PlayerName;
			element.TextColor = textColor;
			base.GetElement<UXLabel>("LabelHQLEVELOther").Text = this.lang.Get("FACTION_FLIP_HQ_LEVEL", new object[]
			{
				string.Empty
			});
			UXLabel element2 = base.GetElement<UXLabel>("LabelHQLEVELCountOther");
			element2.Text = currentPlayer.Map.FindHighestHqLevel().ToString();
			element2.TextColor = textColor;
			base.GetElement<UXLabel>("LabelMedalOther").Text = this.lang.Get("FACTION_FLIP_MEDALS_CURRENT", new object[0]);
			UXLabel element3 = base.GetElement<UXLabel>("LabelMedalCountOther");
			element3.Text = currentPlayer.PlayerMedals.ToString();
			element3.TextColor = textColor;
			base.GetElement<UXLabel>("LabelSquadOther").Text = this.lang.Get("FACTION_FLIP_SQUAD", new object[0]);
			UXLabel element4 = base.GetElement<UXLabel>("LabelSquadNameOther");
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			element4.Text = ((currentSquad == null) ? this.lang.Get("general_none", new object[0]) : currentSquad.SquadName);
			element4.TextColor = textColor;
		}

		private void InitPlayerInfo(PlayerIdentityInfo playerInfo)
		{
			base.GetElement<UXLabel>("LabelCallsign" + this.factionSuffix).Text = this.lang.Get("FACTION_FLIP_CALL_SIGN", new object[0]);
			base.GetElement<UXLabel>("LabelCallsignName" + this.factionSuffix).Text = ((playerInfo.PlayerName == null) ? this.lang.Get("general_none", new object[0]) : playerInfo.PlayerName);
			base.GetElement<UXLabel>("LabelHQLEVEL" + this.factionSuffix).Text = this.lang.Get("FACTION_FLIP_HQ_LEVEL", new object[]
			{
				playerInfo.HQLevel
			});
			base.GetElement<UXLabel>("LabelMedalCount" + this.factionSuffix).Text = this.lang.Get("FACTION_FLIP_MEDALS", new object[]
			{
				playerInfo.Medals
			});
			base.GetElement<UXLabel>("LabelSquad" + this.factionSuffix).Text = this.lang.Get("FACTION_FLIP_SQUAD", new object[0]);
			base.GetElement<UXLabel>("LabelSquadName" + this.factionSuffix).Text = ((playerInfo.SquadName == null) ? this.lang.Get("general_none", new object[0]) : playerInfo.SquadName);
			this.swapButton.Enabled = true;
			this.groupStats.Visible = true;
		}

		public override void Close(object modalResult)
		{
			Service.EventManager.SendEvent(EventId.UIFactionFlipAction, "close");
			base.Close(modalResult);
		}

		public override void OnDestroyElement()
		{
			this.oppositePlayerInfo = null;
			base.OnDestroyElement();
		}
	}
}
