using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class BattleEndScreen : AbstractBattleEndScreen
	{
		private const string RESULT_LABEL = "LabelBattleResult";

		private const string RESULT_FAILURE_TITLE_LABEL = "LabelMissionResultTitle";

		private const string RESULT_FAILURE_LABEL = "LabelMissionResult";

		private const string PERCENT_LABEL = "LabelBattleDescription";

		private const string TROOPS_LABEL = "LabelTroops";

		private const string REWARD_SUCCESS_LABEL = "LabelRewardSuccess";

		private const string STARS_HOLDER = "StarsHolder";

		private const string MISSION_REWARDS_GROUP = "MissionRewards";

		private const string SQUAD_BONUS_GROUP = "SquadBonus";

		private const string ATTACK_LABEL_GAINED = "AttackerLabelGained";

		private const string DEFEND_LABEL_AVAILABLE = "DefenderLabelAvailable";

		private const string ATTACK_MEDALS_LABEL = "AttackerLabelMedals";

		private const string DEFEND_MEDALS_LABEL = "LabelDefenderMedals";

		private const string ATTACK_MEDALS_ICON = "AttackerSpriteIcoMedals";

		private const string DEFEND_MEDALS_ICON = "LabelSpriteIcoDefenderMedals";

		private const string BATTALE_DAMAGE_LABEL = "LabelDamage";

		private const string SEIZED_LABEL = "LabelLootText";

		private const string SEIZED_TABLE = "SeizedTable";

		private const string SEIZED_ITEM_TEMPLATE = "SeizedTemplate";

		private const string SEIZED_ITEM_LABEL = "SeizedLabel";

		private const string SEIZED_ITEM_ICON = "SeizedIcon";

		private const string SEIZED_ICON_CREDITS = "icoCollectCredit";

		private const string SEIZED_ICON_MATERIALS = "icoMaterials";

		private const string SEIZED_ICON_CONTRABAND = "icoContraband";

		private const string SEIZED_ICON_CRYSTALS = "icoCrystals";

		private const string SEIZED_ICON_POINTS = "icoCampaignPoints";

		private const string SEIZED_ICON_MEDALS = "icoMedal";

		private const int SEIZED_ORDER_CREDITS = 1;

		private const int SEIZED_ORDER_MATERIALS = 2;

		private const int SEIZED_ORDER_CONTRABAND = 3;

		private const int SEIZED_ORDER_CRYSTALS = 4;

		private const int SEIZED_ORDER_POINTS = 5;

		private const int SEIZED_ORDER_MEDALS = 6;

		private const int SEIZED_ORDER_TOURNAMENTMEDALS = 7;

		private const string TROOP_GRID = "TroopsExpendedGrid";

		private const string TROOP_ITEM_TEMPLATE = "TroopsExpendedTemplate";

		private const string TROOP_ITEM_CARD = "TroopsExpendedCard";

		private const string TROOP_ITEM_CARD_DEFAULT = "TroopsExpendBgDefault";

		private const string TROOP_ITEM_CARD_QUALITY = "TroopsExpendBgQ{0}";

		private const string TROOP_ITEM_AMOUNT_LABEL = "LabelTroopsExpended";

		private const string TROOP_ITEM_IMAGE_SPRITE = "SpriteTroopsImage";

		private const string TROOP_ITEM_LEVEL_LABEL = "LabelTroopLevel";

		private const string TROOP_ITEM_HERO_DECAL = "SpriteHeroDecal";

		private const string REPLAY_GROUP = "ReplayResults";

		private const string BATTLE_GROUP = "BattleResults";

		private const string MISSION_RESULT_GROUP = "MissionResult";

		private const string REPLAY_TITLE_BAR = "TitleBar";

		private const string REPLAY_TITLE_BAR_RESULT_LABEL = "TitleBarLabelResult";

		private const string REPLAY_TITLE_BAR_LABEL_STARS = "TitleBarLabelStars";

		private const string REPLAY_TITLE_BAR_STARS = "TitleBarSpriteStar{0}";

		private const string REPLAY_CREDITS_LABEL = "AttackerLabelCurrencyValueHome";

		private const string REPLAY_MATERIALS_LABEL = "AttackerLabelMaterialValueHome";

		private const string REPLAY_CONTRABAND_LABEL = "AttackerLabelContrabandValueHome";

		private const string REPLAY_PERCENT_LABEL = "ReplayDamageAmount";

		private const string REPLAY_PREFIX = "Replay";

		private const string REPLAY_AVAILABLE_CREDITS_LABEL = "DefenderLabelCurrencyValueHome";

		private const string REPLAY_AVAILABLE_MATERIALS_LABEL = "LabelLabelMaterialValueHome";

		private const string REPLAY_AVAILABLE_CONTRABAND_LABEL = "DefenderLabelContrabandValueHome";

		private const string REPLAY_DEFENDER_NAME_LABEL = "DefenderLabelName";

		private const string REPLAY_ATTACKER_NAME_LABEL = "AttackerLabelName";

		private const string REPLAY_DEFENDER_TOURNAMENT_RATING = "DefenderTournamentMedals";

		private const string REPLAY_DEFENDER_TOURNAMENT_RATING_LABEL = "LabelDefenderTournamentMedals";

		private const string REPLAY_DEFENDER_TOURNAMENT_RATING_ICON = "LabelSpriteIcoDefenderTournamentMedals";

		private const string REPLAY_ATTACKER_TOURNAMENT_RATING = "AttackerTournamentMedals";

		private const string REPLAY_ATTACKER_TOURNAMENT_RATING_LABEL = "AttackerLabelTournamentMedals";

		private const string REPLAY_ATTACKER_TOURNAMENT_RATING_ICON = "AttackerSpriteIcoTournamentMedals";

		private const string INSTANT_BATTLE_BUTTON = "ButtonInstantBattle";

		private const string LANG_ATTACKED_BY = "ATTACKED_BY";

		private const string LANG_ATTACKER_EARNED = "s_Attacker_Earned";

		private const string LANG_CAMPAIGN_POINTS = "CAMPAIGN_POINTS";

		private const string LANG_CONFLICT_TIER_BONUS = "CONFLICT_TIER_BONUS";

		private const string LANG_DAMAGE = "s_Damage";

		private const string LANG_DEFEAT = "DEFEAT";

		private const string LANG_MINUS = "MINUS";

		private const string LANG_VICTORY = "VICTORY";

		private UXLabel resultLabel;

		private UXLabel resultFailureTitleLabel;

		private UXLabel percentLabel;

		private UXLabel creditsLabel;

		private UXLabel materialLabel;

		private UXLabel contrabandLabel;

		private UXLabel troopsLabel;

		private UXLabel rewardLabel;

		private UXElement missionRewardGroup;

		private UXElement squadBonusGroup;

		private UXLabel attackMedalsLabel;

		private UXLabel defendMedalsLabel;

		private UXSprite attackMedalsIcon;

		private UXSprite defendMedalsIcon;

		private UXLabel seizedLabel;

		private UXTable seizedTable;

		private UXLabel replayDefenderName;

		private UXLabel replayAttackerName;

		private UXLabel replayLostCredits;

		private UXLabel replayLostMaterials;

		private UXLabel replayLostContraband;

		private UXElement replayDefenderTournamentRating;

		private UXElement replayAttackerTournamentRating;

		private UXLabel replayDefenderTournamentRatingLabel;

		private UXSprite replayDefenderTournamentRatingIcon;

		private UXLabel replayAttackerTournamentRatingLabel;

		private UXSprite replayAttackerTournamentRatingIcon;

		private UXLabel replayTitleBarResultLabel;

		protected override string StarsPlaceHolderName
		{
			get
			{
				return "StarsHolder";
			}
		}

		protected override string ReplayPrefix
		{
			get
			{
				return "Replay";
			}
		}

		protected override string TroopCardName
		{
			get
			{
				return "TroopsExpendedCard";
			}
		}

		protected override string TroopCardDefaultName
		{
			get
			{
				return "TroopsExpendBgDefault";
			}
		}

		protected override string TroopCardQualityName
		{
			get
			{
				return "TroopsExpendBgQ{0}";
			}
		}

		protected override string TroopCardIconName
		{
			get
			{
				return "SpriteTroopsImage";
			}
		}

		protected override string TroopCardAmountName
		{
			get
			{
				return "LabelTroopsExpended";
			}
		}

		protected override string TroopCardLevelName
		{
			get
			{
				return "LabelTroopLevel";
			}
		}

		protected override string TroopHeroDecalName
		{
			get
			{
				return "SpriteHeroDecal";
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public BattleEndScreen(bool isReplay) : base("gui_battle_end_screen", isReplay)
		{
		}

		public override void OnDestroyElement()
		{
			if (this.seizedTable != null)
			{
				this.seizedTable.Clear();
				this.seizedTable = null;
			}
			base.OnDestroyElement();
		}

		protected override void InitElements()
		{
			this.missionRewardGroup = base.GetElement<UXElement>("MissionRewards");
			this.missionRewardGroup.Visible = false;
			this.squadBonusGroup = base.GetElement<UXElement>("SquadBonus");
			this.squadBonusGroup.Visible = false;
			this.attackMedalsLabel = base.GetElement<UXLabel>("AttackerLabelMedals");
			this.defendMedalsLabel = base.GetElement<UXLabel>("LabelDefenderMedals");
			this.attackMedalsIcon = base.GetElement<UXSprite>("AttackerSpriteIcoMedals");
			this.defendMedalsIcon = base.GetElement<UXSprite>("LabelSpriteIcoDefenderMedals");
			if (this.isNonAttackerReplayView)
			{
				this.replayAttackerTournamentRating = base.GetElement<UXElement>("AttackerTournamentMedals");
				this.replayDefenderTournamentRating = base.GetElement<UXElement>("DefenderTournamentMedals");
				this.replayTitleBarResultLabel = base.GetElement<UXLabel>("TitleBarLabelResult");
			}
			else
			{
				this.seizedLabel = base.GetElement<UXLabel>("LabelLootText");
				this.seizedTable = base.GetElement<UXTable>("SeizedTable");
				this.seizedTable.SetTemplateItem("SeizedTemplate");
				base.InitStars();
			}
			base.GetElement<UXElement>("MissionResult").Visible = false;
		}

		protected override void InitButtons()
		{
			base.GetElement<UXButton>("ButtonInstantBattle").Visible = false;
			base.InitButtons();
		}

		protected override void InitLabels()
		{
			base.GetElement<UXLabel>("AttackerLabelGained").Visible = false;
			base.GetElement<UXLabel>("DefenderLabelAvailable").Visible = false;
			base.GetElement<UXLabel>("LabelDamage").Text = this.lang.Get("s_Damage", new object[0]);
			this.resultLabel = base.GetElement<UXLabel>("LabelBattleResult");
			this.resultFailureTitleLabel = base.GetElement<UXLabel>("LabelMissionResultTitle");
			this.troopsLabel = base.GetElement<UXLabel>("LabelTroops");
			this.rewardLabel = base.GetElement<UXLabel>("LabelRewardSuccess");
			this.rewardLabel.Visible = false;
			if (this.isNonAttackerReplayView)
			{
				base.GetElement<UXLabel>("TitleBarLabelStars").Text = this.lang.Get("s_Attacker_Earned", new object[0]);
				this.percentLabel = base.GetElement<UXLabel>("ReplayDamageAmount");
				this.creditsLabel = base.GetElement<UXLabel>("AttackerLabelCurrencyValueHome");
				this.materialLabel = base.GetElement<UXLabel>("AttackerLabelMaterialValueHome");
				this.contrabandLabel = base.GetElement<UXLabel>("AttackerLabelContrabandValueHome");
				this.replayLostCredits = base.GetElement<UXLabel>("DefenderLabelCurrencyValueHome");
				this.replayLostMaterials = base.GetElement<UXLabel>("LabelLabelMaterialValueHome");
				this.replayLostContraband = base.GetElement<UXLabel>("DefenderLabelContrabandValueHome");
				this.replayAttackerName = base.GetElement<UXLabel>("AttackerLabelName");
				this.replayDefenderName = base.GetElement<UXLabel>("DefenderLabelName");
				this.replayAttackerTournamentRatingLabel = base.GetElement<UXLabel>("AttackerLabelTournamentMedals");
				this.replayDefenderTournamentRatingLabel = base.GetElement<UXLabel>("LabelDefenderTournamentMedals");
				this.replayAttackerTournamentRatingIcon = base.GetElement<UXSprite>("AttackerSpriteIcoTournamentMedals");
				this.replayDefenderTournamentRatingIcon = base.GetElement<UXSprite>("LabelSpriteIcoDefenderTournamentMedals");
			}
			else
			{
				this.percentLabel = base.GetElement<UXLabel>("LabelBattleDescription");
			}
			base.InitLabels();
		}

		protected override void SetupView()
		{
			base.GetElement<UXElement>("BattleResults").Visible = !this.isNonAttackerReplayView;
			base.GetElement<UXElement>("ReplayResults").Visible = this.isNonAttackerReplayView;
			BattleEntry battleEntry;
			BattleType battleType;
			if (this.isReplay)
			{
				battleEntry = Service.BattlePlaybackController.CurrentBattleEntry;
				battleType = BattleType.Pvp;
			}
			else
			{
				CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
				battleEntry = currentBattle;
				battleType = currentBattle.Type;
			}
			if (battleEntry == null)
			{
				Service.Logger.Error("Last battle is null");
				return;
			}
			this.resultLabel.Text = ((!battleEntry.Won) ? this.lang.Get("DEFEAT", new object[0]) : this.lang.Get("VICTORY", new object[0]));
			this.resultLabel.Visible = !this.isNonAttackerReplayView;
			UXLabel element = base.GetElement<UXLabel>("LabelMissionResult");
			if (battleEntry.FailedConditionUid != null)
			{
				this.resultLabel.Text = string.Empty;
				this.resultFailureTitleLabel.Text = this.lang.Get("DEFEAT", new object[0]);
				CampaignMissionVO mission = Service.StaticDataController.Get<CampaignMissionVO>(battleEntry.MissionId);
				element.Text = LangUtils.GetMissionFailureMessage(mission);
			}
			else
			{
				this.resultFailureTitleLabel.Text = string.Empty;
				element.Text = string.Empty;
			}
			this.percentLabel.Text = this.lang.Get("PERCENTAGE", new object[]
			{
				battleEntry.DamagePercent
			});
			bool flag = battleType == BattleType.Pvp;
			bool flag2 = battleEntry.IsSpecOps();
			this.attackMedalsLabel.Visible = flag;
			this.defendMedalsLabel.Visible = flag;
			this.attackMedalsIcon.Visible = flag;
			this.defendMedalsIcon.Visible = flag;
			if (this.seizedTable != null)
			{
				this.seizedTable.Clear();
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			BattleParticipant attacker = battleEntry.Attacker;
			BattleParticipant defender = battleEntry.Defender;
			bool flag3 = flag && attacker.TournamentRatingDelta != 0;
			if (flag)
			{
				this.attackMedalsLabel.Text = this.CalculateMedalsGained(attacker).ToString();
				this.defendMedalsLabel.Text = this.CalculateMedalsGained(defender).ToString();
				bool flag4 = currentPlayer.PlayerId == attacker.PlayerId || battleEntry.SharerPlayerId == attacker.PlayerId;
				this.AddSeizedItem("icoMedal", (!flag4) ? this.defendMedalsLabel.Text : this.attackMedalsLabel.Text, 6);
				if (flag3)
				{
					this.AddSeizedItem(GameUtils.GetTournamentPointIconName(battleEntry.PlanetId), attacker.TournamentRatingDelta.ToString(), 7);
				}
			}
			switch (battleType)
			{
			case BattleType.Pvp:
			case BattleType.PveAttack:
			case BattleType.PveFue:
			case BattleType.ClientBattle:
				this.AddSeizedItem("icoCollectCredit", this.lang.ThousandsSeparated(battleEntry.LootCreditsEarned), 1);
				this.AddSeizedItem("icoMaterials", this.lang.ThousandsSeparated(battleEntry.LootMaterialsEarned), 2);
				if (this.ShouldShowContrabandLoot(currentPlayer, battleType, battleEntry))
				{
					this.AddSeizedItem("icoContraband", this.lang.ThousandsSeparated(battleEntry.LootContrabandEarned), 3);
				}
				this.troopsLabel.Text = this.lang.Get("EXPENDED", new object[0]);
				break;
			case BattleType.PveDefend:
				this.troopsLabel.Text = this.lang.Get("ATTACKED_BY", new object[0]);
				this.seizedLabel.Visible = false;
				break;
			}
			if (flag2)
			{
				if (battleType == BattleType.PveDefend)
				{
					this.AddSeizedItem("icoCampaignPoints", this.lang.Get("CAMPAIGN_POINTS", new object[]
					{
						this.lang.ThousandsSeparated((int)battleEntry.CampaignPointsEarn)
					}), 5);
				}
				else
				{
					this.AddSeizedItem("icoCampaignPoints", this.lang.Get("CAMPAIGN_POINTS", new object[]
					{
						this.lang.ThousandsSeparated((int)battleEntry.CampaignPointsEarn)
					}), 5);
				}
			}
			if (this.seizedTable != null)
			{
				this.seizedTable.RepositionItemsFrameDelayed(new UXDragDelegate(this.OnSeizedRepositioned));
			}
			if (this.isNonAttackerReplayView)
			{
				this.SetupNonAttackerReplayView(battleEntry, attacker, defender, flag3);
			}
			else if (battleEntry.Won)
			{
				base.AnimateStars(battleEntry.EarnedStars);
			}
			base.InitTroopGrid("TroopsExpendedGrid", "TroopsExpendedTemplate", battleEntry);
			this.troopsLabel.Visible = (this.troopGrid.Count > 0);
			this.replayBattleButton.Visible = battleEntry.AllowReplay;
		}

		private void SetupNonAttackerReplayView(BattleEntry lastBattle, BattleParticipant attacker, BattleParticipant defender, bool isConflictBattle)
		{
			this.replayTitleBarResultLabel.Text = ((!lastBattle.Won) ? this.lang.Get("DEFEAT", new object[0]) : this.lang.Get("VICTORY", new object[0]));
			this.replayTitleBarResultLabel.TextColor = ((!lastBattle.Won) ? UXUtils.COLOR_REPLAY_DEFEAT : UXUtils.COLOR_REPLAY_VICTORY);
			for (int i = 1; i <= 3; i++)
			{
				base.GetElement<UXSprite>(string.Format("TitleBarSpriteStar{0}", i)).Visible = true;
				if (i <= lastBattle.EarnedStars)
				{
					base.GetElement<UXSprite>(string.Format("TitleBarSpriteStar{0}", i)).Color = Color.white;
				}
			}
			this.creditsLabel.Text = this.lang.ThousandsSeparated(lastBattle.LootCreditsEarned);
			this.materialLabel.Text = this.lang.ThousandsSeparated(lastBattle.LootMaterialsEarned);
			this.contrabandLabel.Text = this.lang.ThousandsSeparated(lastBattle.LootContrabandEarned);
			this.replayAttackerName.Text = attacker.PlayerName;
			this.replayDefenderName.Text = defender.PlayerName;
			this.replayLostCredits.Text = this.lang.Get("MINUS", new object[]
			{
				this.lang.ThousandsSeparated(lastBattle.LootCreditsDeducted)
			});
			this.replayLostMaterials.Text = this.lang.Get("MINUS", new object[]
			{
				this.lang.ThousandsSeparated(lastBattle.LootMaterialsDeducted)
			});
			this.replayLostContraband.Text = this.lang.Get("MINUS", new object[]
			{
				this.lang.ThousandsSeparated(lastBattle.LootContrabandDeducted)
			});
			if (isConflictBattle)
			{
				this.replayAttackerTournamentRatingLabel.Text = attacker.TournamentRatingDelta.ToString();
				this.replayDefenderTournamentRatingLabel.Text = defender.TournamentRatingDelta.ToString();
				this.replayAttackerTournamentRatingIcon.SpriteName = GameUtils.GetTournamentPointIconName(lastBattle.PlanetId);
				this.replayDefenderTournamentRatingIcon.SpriteName = GameUtils.GetTournamentPointIconName(lastBattle.PlanetId);
			}
			this.replayAttackerTournamentRating.Visible = isConflictBattle;
			this.replayDefenderTournamentRating.Visible = isConflictBattle;
		}

		private int CalculateMedalsGained(BattleParticipant participant)
		{
			int result = 0;
			if (participant != null)
			{
				int num = GameUtils.CalcuateMedals(participant.AttackRating, participant.DefenseRating);
				int num2 = GameUtils.CalcuateMedals(participant.AttackRating + participant.AttackRatingDelta, participant.DefenseRating + participant.DefenseRatingDelta);
				result = num2 - num;
			}
			return result;
		}

		private bool ShouldShowContrabandLoot(GamePlayer player, BattleType type, BattleEntry battle)
		{
			if (type == BattleType.Pvp)
			{
				return player.IsContrabandUnlocked;
			}
			return player.IsContrabandUnlocked && battle.LootContrabandEarned > 0;
		}

		private void OnSeizedRepositioned(AbstractUXList list)
		{
			this.seizedTable.AutoCenter();
		}

		private void AddSeizedItem(string iconName, string text, int order)
		{
			if (this.seizedTable == null)
			{
				return;
			}
			UXElement item = this.seizedTable.CloneTemplateItem(iconName);
			UXSprite subElement = this.seizedTable.GetSubElement<UXSprite>(iconName, "SeizedIcon");
			subElement.SpriteName = iconName;
			UXLabel subElement2 = this.seizedTable.GetSubElement<UXLabel>(iconName, "SeizedLabel");
			subElement2.Text = text;
			this.seizedTable.AddItem(item, order);
		}

		private void SetupSeizedIcon(string itemId, string iconName)
		{
			UXSprite subElement = this.seizedTable.GetSubElement<UXSprite>(itemId, iconName);
			subElement.Visible = (itemId == iconName);
		}

		private int GetBonusCurrencyOrder(string currency)
		{
			if (currency == "credits")
			{
				return 1;
			}
			if (currency == "materials")
			{
				return 2;
			}
			if (currency == "contraband")
			{
				return 3;
			}
			if (currency == "crystals")
			{
				return 4;
			}
			return 0;
		}
	}
}
