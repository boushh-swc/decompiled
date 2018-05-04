using StaRTS.Externals.Manimal;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Text;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsPvPViewModule : AbstractPlanetDetailsViewModule
	{
		private const string ATTACK_RIVAL_TEXT = "s_AttackRival";

		private const string BATTLE_COST_TITLE_LABEL = "LabelBattleCost";

		private const string BATTLE_COST_VALUE_LABEL = "LabelBattleCostAmount";

		private const string BATTLE_ACTION_BUTTON = "BtnBattleAction";

		private const string BATTLE_ACTION_BUTTON_LABEL = "LabelBtnBattleAction";

		private const string LABEL_BATTLE_LOCKED = "LabelBattleLocked";

		private const string MESH_PVP_IMAGE = "MeshPVPImage";

		private const string PVP_BATTLE_TITLE_LABEL = "LabelBattleTitle";

		private const string PVP_MEDALS_LABEL = "LabelMedalCount";

		private const string PVP_TOURNAMENT_MEDALS_GROUP = "TournamentMedalCount";

		private const string PVP_TOURNAMENT_MEDALS_LABEL = "LabelTournamentMedalCount";

		private const string PVP_TOURNAMENT_MEDALS_SPRITE = "SpriteTournamentMedalCount";

		private const string CHAPTER_ACTION_BUTTON = "BtnChapterAction";

		private const string COST_STRING = "s_Cost";

		private const string FIND_MATCH_STRING = "s_FindMatch";

		private const string NOT_ENOUGH_TROOPS_TITLE_STRING = "NOT_ENOUGH_TROOPS_TITLE";

		private const string NOT_ENOUGH_TROOPS_FOR_ATTACK_STRING = "NOT_ENOUGH_TROOPS_FOR_ATTACK";

		private const string PVP_BATTLE_CONTEXT = "PvP_battle";

		private const string PLANET_PVP_BACKGROUND_PREFIX = "PlanetPVP-";

		private const string BATTLE_COST = "BattleCost";

		private UXButton pveContinueButton;

		private UXButton pvpAttackButton;

		private UXMeshRenderer pvpMeshTexture;

		private UXElement tournamentMedalsGroup;

		private UXLabel tournamentMedalsLabel;

		private UXSprite tournamentMedalsSprite;

		private UXLabel battleActionButtonLabel;

		private UXLabel labelBattleLocked;

		private UXElement battleCost;

		private UXLabel labelBattleTitle;

		public PlanetDetailsPvPViewModule(PlanetDetailsScreen screen) : base(screen)
		{
		}

		public void OnScreenLoaded()
		{
			this.InitPvpPanel();
			this.RefreshScreenForPlanetChange();
		}

		public void RefreshScreenForPlanetChange()
		{
			if (Service.CurrentPlayer.IsCurrentPlanet(this.screen.viewingPlanetVO))
			{
				this.pvpAttackButton.Enabled = true;
				this.pvpAttackButton.Visible = true;
				this.battleActionButtonLabel.TextColor = UXUtils.COLOR_ENABLED;
				this.pvpAttackButton.OnClicked = new UXButtonClickedDelegate(this.OnPvpButtonClicked);
				this.labelBattleLocked.Visible = false;
				this.battleCost.Visible = true;
				this.pvpMeshTexture.LoadTexture("PlanetPVP-" + this.screen.viewingPlanetVO.Abbreviation);
				this.pvpMeshTexture.SetShader("Unlit/Premultiplied Colored");
			}
			else
			{
				this.pvpAttackButton.Enabled = false;
				this.pvpAttackButton.Visible = false;
				this.battleActionButtonLabel.TextColor = UXUtils.COLOR_LABEL_DISABLED;
				this.labelBattleLocked.Visible = true;
				this.battleCost.Visible = false;
				this.pvpMeshTexture.LoadTexture("PlanetPanelLocked");
				this.pvpMeshTexture.SetShader("Unlit/Premultiplied Colored");
			}
		}

		private void InitPvpPanel()
		{
			this.screen.GetElement<UXLabel>("LabelBattleCost").Text = base.LangController.Get("s_Cost", new object[0]);
			this.battleActionButtonLabel = this.screen.GetElement<UXLabel>("LabelBtnBattleAction");
			this.battleActionButtonLabel.Text = base.LangController.Get("s_FindMatch", new object[0]);
			int pvpMatchCost = Service.PvpManager.GetPvpMatchCost();
			UXLabel element = this.screen.GetElement<UXLabel>("LabelBattleCostAmount");
			element.Text = base.LangController.ThousandsSeparated(pvpMatchCost);
			this.pveContinueButton = this.screen.GetElement<UXButton>("BtnChapterAction");
			this.pvpAttackButton = this.screen.GetElement<UXButton>("BtnBattleAction");
			this.battleCost = this.screen.GetElement<UXElement>("BattleCost");
			this.labelBattleLocked = this.screen.GetElement<UXLabel>("LabelBattleLocked");
			this.labelBattleTitle = this.screen.GetElement<UXLabel>("LabelBattleTitle");
			this.pvpMeshTexture = this.screen.GetElement<UXMeshRenderer>("MeshPVPImage");
			int playerMedals = base.Player.PlayerMedals;
			this.screen.GetElement<UXLabel>("LabelMedalCount").Text = playerMedals.ToString();
			this.tournamentMedalsGroup = this.screen.GetElement<UXElement>("TournamentMedalCount");
			this.tournamentMedalsLabel = this.screen.GetElement<UXLabel>("LabelTournamentMedalCount");
			this.tournamentMedalsSprite = this.screen.GetElement<UXSprite>("SpriteTournamentMedalCount");
		}

		public void UpdatePvpPanel(bool showTournamentRating, TournamentVO tournamentVO)
		{
			this.tournamentMedalsGroup.Visible = showTournamentRating;
			this.labelBattleTitle.Text = base.LangController.Get("s_AttackRival", new object[0]);
			if (showTournamentRating && !string.IsNullOrEmpty(tournamentVO.PlanetId))
			{
				int tournamentRating = Service.TournamentController.GetTournamentRating(tournamentVO);
				this.tournamentMedalsLabel.Text = tournamentRating.ToString();
				this.tournamentMedalsSprite.SpriteName = GameUtils.GetTournamentPointIconName(tournamentVO.PlanetId);
				if (Service.TournamentController.IsPlayerInTournament(tournamentVO))
				{
					this.labelBattleTitle.Text = LangUtils.GetTournamentTitle(tournamentVO);
				}
			}
		}

		private void OnPvpButtonClicked(UXButton button)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TournamentVO currentPlanetActiveTournament = Service.TournamentController.CurrentPlanetActiveTournament;
			stringBuilder.Append((currentPlanetActiveTournament == null) ? "no_tournament" : currentPlanetActiveTournament.Uid);
			if (!GameUtils.HasAvailableTroops(false, null))
			{
				AlertScreen.ShowModal(false, base.LangController.Get("NOT_ENOUGH_TROOPS_TITLE", new object[0]), base.LangController.Get("NOT_ENOUGH_TROOPS_FOR_ATTACK", new object[0]), null, null);
				stringBuilder.Append("|");
				stringBuilder.Append("no_troops");
				Service.EventManager.SendEvent(EventId.UIAttackScreenSelection, new ActionMessageBIData("PvP", stringBuilder.ToString()));
				return;
			}
			if (!GameUtils.CanAffordCredits(Service.PvpManager.GetPvpMatchCost()))
			{
				PayMeScreen.ShowIfNotEnoughCurrency(Service.PvpManager.GetPvpMatchCost(), 0, 0, "PvP_battle", new OnScreenModalResult(this.OnNotEnoughCreditsModalResult));
				return;
			}
			if (base.Player.ProtectedUntil > ServerTime.Time)
			{
				DisableProtectionAlertScreen.ShowModal(new OnScreenModalResult(this.OnConfirmInvalidation), null);
			}
			else
			{
				this.PurchaseNextBattle();
				base.EvtManager.SendEvent(EventId.UIAttackScreenSelection, new ActionMessageBIData("PvP", stringBuilder.ToString()));
			}
		}

		private void OnNotEnoughCreditsModalResult(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				this.OnPvpButtonClicked(null);
			}
		}

		private void OnConfirmInvalidation(object result, object cookie)
		{
			if (result != null)
			{
				this.PurchaseNextBattle();
			}
		}

		private void PurchaseNextBattle()
		{
			this.pvpAttackButton.Enabled = false;
			this.pveContinueButton.Enabled = false;
			Service.PvpManager.PurchaseNextBattle();
		}

		public void OnClose()
		{
			this.pvpMeshTexture.Visible = false;
		}
	}
}
