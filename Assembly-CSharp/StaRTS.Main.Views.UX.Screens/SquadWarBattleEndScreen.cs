using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarBattleEndScreen : AbstractBattleEndScreen
	{
		private const string ATTACKER_BUFFS_LABEL = "LabelBuffsRightSquadWar";

		private const string DEFENDER_BUFFS_LABEL = "LabelBuffsLeftSquadWar";

		private const string PERCENT_LABEL = "LabelPercentageSquadWar";

		private const string POINTS_LABEL = "LabelResultsSquadWar";

		private const string TITLE_LABEL = "LabelTitleSquadWar";

		private const string TROOPS_LABEL = "LabelTroopsExpendedSquadWar";

		private const string TURNS_LABEL = "LabelAttacksLeftSquadWar";

		private const string LABEL_BUFF_BASE_CAPTURED_SQUADWAR = "LabelBuffBaseCapturedSquadWar";

		private const string BUFF_BASE_UPGRADE_RESULT = "LabelBuffBaseUpgradeResultSquadWar";

		private const string REPLAY_GROUP = "ReplayResults";

		private const string BUTTON_REPLAY_BATTLE = "ButtonReplayBattle";

		private const string VICTORY_POINT_1_SPRITE = "SpriteIconVictoryPointLeftSquadWar";

		private const string VICTORY_POINT_1_CHECK_SPRITE = "SpriteIconCheckLeftSquadWar";

		private const string VICTORY_POINT_1_LABEL = "LabelVictoryPointReqLeftSquadWar";

		private const string VICTORY_POINT_2_SPRITE = "SpriteIconVictoryPointMiddleSquadWar";

		private const string VICTORY_POINT_2_CHECK_SPRITE = "SpriteIconCheckMiddleSquadWar";

		private const string VICTORY_POINT_2_LABEL = "LabelVictoryPointReqMiddleSquadWar";

		private const string VICTORY_POINT_3_SPRITE = "SpriteIconVictoryPointRightSquadWar";

		private const string VICTORY_POINT_3_CHECK_SPRITE = "SpriteIconCheckRightSquadWar";

		private const string VICTORY_POINT_3_LABEL = "LabelVictoryPointReqRightSquadWar";

		private const string STARS_HOLDER = "StarsHolderSquadWar";

		private const string TROOP_GRID = "GridTroopsExpendedSquadWar";

		private const string TROOP_ITEM_TEMPLATE = "TemplateTroopsExpendedSquadWar";

		private const string TROOP_ITEM_CARD = "TroopsExpendedCardSquadWar";

		private const string TROOP_ITEM_CARD_DEFAULT = "TroopsExpendBgDefault";

		private const string TROOP_ITEM_CARD_QUALITY = "TroopsExpendBgQ{0}";

		private const string TROOP_ITEM_AMOUNT_LABEL = "LabelTroopsCountSquadWar";

		private const string TROOP_ITEM_IMAGE_SPRITE = "SpriteTroopsImageSquadWar";

		private const string TROOP_ITEM_LEVEL_LABEL = "LabelTroopLevelSquadWar";

		private const string TROOP_ITEM_HERO_DECAL = "SpriteHeroDecalSquadWar";

		private const string ATTACKER_BUFFS_GRID = "GridBuffsRightSquadWar";

		private const string ATTACKER_BUFFS_TEMPLATE = "SpriteIconBuffRightSquadWar";

		private const string DEFENDER_BUFFS_GRID = "GridBuffsLeftSquadWar";

		private const string DEFENDER_BUFFS_TEMPLATE = "SpriteIconBuffLeftSquadWar";

		private const string TEXTURE_FACTION_CURRENT = "TextureFactionIconCurrent";

		private const string TEXTURE_FACTION_NEW = "TextureFactionIconNew";

		private const string TEXTURE_REWARD_EMPIRE = "squadwars_end_celeb_empire";

		private const string TEXTURE_REWARD_REBEL = "squadwars_end_celeb_rebel";

		private const string ANIMATIONS = "EndAnimations";

		private const string BOOL_NONE_DISABLED = "NoneDisabled";

		private const string BOOL_DISABLE1 = "Disable1";

		private const string BOOL_DISABLE_1_2 = "Disable1and2";

		private const string BOOL_DISABLE_ALL = "DisableAll";

		private const string BOOL_NONE_DISABLED_1 = "NoneDisabled_1Disabled";

		private const string BOOL_DISABLE2 = "Disable2";

		private const string BOOL_DISABLE_2_3 = "Disable2and3";

		private const string BOOL_NONE_DISABLED_2 = "NoneDisabled_2Disabled";

		private const string BOOL_DISABLE3 = "Disable3";

		private const string TRIGGER_BASE_NOT_CAPTURED_NO_OWNER = "BaseNotCaptureNoOwner";

		private const string TRIGGER_BASE_CAPTURED = "BaseCaptured";

		private const string TRIGGER_BASE_CHANGE_OWNER = "BaseChangeOwner";

		private const string TRIGGER_DISABLE_UPLINK = "DisableUplink";

		private const float AUDIO_DELAY_UPLINKS_AT_START_ZERO = 0.183f;

		private const float AUDIO_DELAY_UPLINKS_AT_START_NOT_ZERO = 0.433f;

		private const float AUDIO_UPLINK_ANIM_INTERVAL = 1.35f;

		private const float AUDIO_DELAY_TRIGGER_BASE_CAPTURED = 1f;

		private const float AUDIO_DELAY_TRIGGER_BASE_CHANGE_OWNER = 2f;

		private const int HASH_FACTOR = 10;

		protected const string LANG_100_DAMAGE = "DAMAGE_100";

		protected const string LANG_50_DAMAGE = "DAMAGE_50";

		protected const string LANG_ATTACKER_BUFFS = "ATTACKER_BUFFS";

		protected const string LANG_ATTACKS_LEFT = "WAR_PLAYER_DETAILS_TURNS_LEFT";

		protected const string LANG_ATTACK_RESULT = "ATTACK_RESULT";

		protected const string LANG_DEFENDER_BUFFS = "DEFENDER_BUFFS";

		protected const string LANG_DESTROY_HQ = "DESTROY_HQ";

		protected const string LANG_POINTS_EARNED = "VICTORY_POINTS_EARNED";

		protected const string WAR_BUFF_BASE_UPGRADED = "WAR_BUFF_BASE_UPGRADED";

		protected const string WAR_BATTLE_END_BUFF_BASE_CAPTURED = "WAR_BATTLE_END_BUFF_BASE_CAPTURED";

		private UXLabel troopsLabel;

		private UXGrid attackerBuffsGrid;

		private UXGrid defenderBuffsGrid;

		private UXTexture currentOwnerTexture;

		private UXTexture newOwnerTexture;

		private UXLabel labelBuffBaseCapturedSquadWar;

		private UXLabel buffBaseUpgradeLabel;

		private BattleEntry lastBattle;

		private BattleType battleType;

		private static Dictionary<int, string> flagMap = new Dictionary<int, string>
		{
			{
				0,
				"NoneDisabled"
			},
			{
				1,
				"Disable1"
			},
			{
				2,
				"Disable1and2"
			},
			{
				3,
				"DisableAll"
			},
			{
				11,
				"NoneDisabled_1Disabled"
			},
			{
				12,
				"Disable2"
			},
			{
				13,
				"Disable2and3"
			},
			{
				22,
				"NoneDisabled_2Disabled"
			},
			{
				23,
				"Disable3"
			}
		};

		protected override string StarsPlaceHolderName
		{
			get
			{
				return "StarsHolderSquadWar";
			}
		}

		protected override string ReplayPrefix
		{
			get
			{
				return string.Empty;
			}
		}

		protected override string TroopCardName
		{
			get
			{
				return "TroopsExpendedCardSquadWar";
			}
		}

		protected override string TroopCardIconName
		{
			get
			{
				return "SpriteTroopsImageSquadWar";
			}
		}

		protected override string TroopCardAmountName
		{
			get
			{
				return "LabelTroopsCountSquadWar";
			}
		}

		protected override string TroopCardLevelName
		{
			get
			{
				return "LabelTroopLevelSquadWar";
			}
		}

		protected override string TroopHeroDecalName
		{
			get
			{
				return "SpriteHeroDecalSquadWar";
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

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public SquadWarBattleEndScreen(bool isReplay) : base("gui_squadwar_battle_end_screen", isReplay)
		{
		}

		public override void OnDestroyElement()
		{
			if (this.attackerBuffsGrid != null)
			{
				this.attackerBuffsGrid.Clear();
				this.attackerBuffsGrid = null;
			}
			if (this.defenderBuffsGrid != null)
			{
				this.defenderBuffsGrid.Clear();
				this.defenderBuffsGrid = null;
			}
			Service.SquadController.WarManager.ReleaseCurrentlyScoutedBuffBase();
			Service.AudioManager.ClearAudioRepeatTimers();
			base.OnDestroyElement();
		}

		protected override void SetupView()
		{
			base.GetElement<UXElement>("ReplayResults").Visible = false;
			base.GetElement<UXButton>("ButtonReplayBattle").Visible = false;
			this.lastBattle = null;
			this.battleType = BattleType.ClientBattle;
			if (this.isReplay)
			{
				this.lastBattle = Service.BattlePlaybackController.CurrentBattleEntry;
				BattleRecord currentBattleRecord = Service.BattlePlaybackController.CurrentBattleRecord;
				this.battleType = ((currentBattleRecord == null) ? BattleType.ClientBattle : currentBattleRecord.BattleType);
			}
			else
			{
				CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
				this.lastBattle = currentBattle;
				this.battleType = ((currentBattle == null) ? BattleType.ClientBattle : currentBattle.Type);
			}
			if (this.lastBattle == null)
			{
				Service.Logger.Error("Last battle is null");
				return;
			}
			base.GetElement<UXLabel>("LabelPercentageSquadWar").Text = this.lang.Get("PERCENTAGE", new object[]
			{
				this.lastBattle.DamagePercent
			});
			bool flag = this.battleType == BattleType.PvpAttackSquadWar;
			UXLabel element = base.GetElement<UXLabel>("LabelResultsSquadWar");
			if (flag)
			{
				int num = Service.SquadController.WarManager.CalculateVictoryPointsTaken(this.lastBattle);
				element.Text = this.lang.Get("VICTORY_POINTS_EARNED", new object[]
				{
					num
				});
			}
			else
			{
				bool flag2 = this.lastBattle.Defender.PlayerFaction != FactionType.Smuggler;
				bool won = this.lastBattle.Won;
				string text = (this.lastBattle.Defender.PlayerFaction != FactionType.Empire) ? "squadwars_end_celeb_rebel" : "squadwars_end_celeb_empire";
				string text2 = (this.lastBattle.Attacker.PlayerFaction != FactionType.Empire) ? "squadwars_end_celeb_rebel" : "squadwars_end_celeb_empire";
				this.currentOwnerTexture = base.GetElement<UXTexture>("TextureFactionIconCurrent");
				this.currentOwnerTexture.LoadTexture((flag2 || !won) ? text : text2);
				this.newOwnerTexture = base.GetElement<UXTexture>("TextureFactionIconNew");
				this.newOwnerTexture.LoadTexture((!won) ? text : text2);
				if (won)
				{
					SquadWarManager warManager = Service.SquadController.WarManager;
					string currentlyScoutedBuffBaseId = warManager.GetCurrentlyScoutedBuffBaseId();
					WarBuffVO warBuffVO = Service.StaticDataController.Get<WarBuffVO>(currentlyScoutedBuffBaseId);
					SquadWarBuffBaseData currentlyScoutedBuffBaseData = warManager.GetCurrentlyScoutedBuffBaseData();
					string text3 = this.lang.Get(warBuffVO.BuffBaseName, new object[0]);
					string text4 = currentlyScoutedBuffBaseData.GetDisplayBaseLevel().ToString();
					this.buffBaseUpgradeLabel.Visible = true;
					this.buffBaseUpgradeLabel.Text = this.lang.Get("WAR_BUFF_BASE_UPGRADED", new object[]
					{
						text3,
						text4
					});
					this.labelBuffBaseCapturedSquadWar.Visible = true;
					this.labelBuffBaseCapturedSquadWar.Text = this.lang.Get("WAR_BATTLE_END_BUFF_BASE_CAPTURED", new object[]
					{
						text3
					});
				}
			}
			element.Visible = flag;
			UXLabel element2 = base.GetElement<UXLabel>("LabelAttacksLeftSquadWar");
			bool visible = false;
			if (this.lastBattle.AttackerID == Service.CurrentPlayer.PlayerId)
			{
				SquadWarParticipantState currentParticipantState = Service.SquadController.WarManager.GetCurrentParticipantState();
				if (currentParticipantState != null)
				{
					element2.Text = this.lang.Get("WAR_PLAYER_DETAILS_TURNS_LEFT", new object[]
					{
						currentParticipantState.TurnsLeft
					});
					visible = true;
				}
			}
			element2.Visible = visible;
			this.InitVictoryPoints(this.lastBattle);
			this.attackerBuffsGrid = base.GetElement<UXGrid>("GridBuffsRightSquadWar");
			SquadWarBuffIconHelper.SetupBuffIcons(this.attackerBuffsGrid, "SpriteIconBuffRightSquadWar", this.lastBattle.AttackerWarBuffs);
			this.defenderBuffsGrid = base.GetElement<UXGrid>("GridBuffsLeftSquadWar");
			SquadWarBuffIconHelper.SetupBuffIcons(this.defenderBuffsGrid, "SpriteIconBuffLeftSquadWar", this.lastBattle.DefenderWarBuffs);
			if (this.lastBattle.Won)
			{
				base.AnimateStars(this.lastBattle.EarnedStars);
			}
			base.InitTroopGrid("GridTroopsExpendedSquadWar", "TemplateTroopsExpendedSquadWar", this.lastBattle);
		}

		protected override void OnAllStarsComplete(uint id, object cookie)
		{
			UXElement element = base.GetElement<UXElement>("EndAnimations");
			Animator component = element.Root.GetComponent<Animator>();
			string text = "BaseNotCaptureNoOwner";
			int num = 0;
			int num2 = 0;
			if (this.battleType == BattleType.PveBuffBase)
			{
				bool flag = this.lastBattle.Defender.PlayerFaction != FactionType.Smuggler;
				if (flag && this.lastBattle.Won)
				{
					text = "BaseChangeOwner";
				}
				if (flag && !this.lastBattle.Won)
				{
					text = "BaseCaptured";
				}
				if (!flag && this.lastBattle.Won)
				{
					text = "BaseCaptured";
				}
				if (!flag && !this.lastBattle.Won)
				{
					text = "BaseNotCaptureNoOwner";
				}
			}
			else if (this.battleType == BattleType.PvpAttackSquadWar)
			{
				text = "DisableUplink";
				num = 3 - this.lastBattle.WarVictoryPointsAvailable;
				int num3 = Math.Max(num, this.lastBattle.EarnedStars);
				string name = SquadWarBattleEndScreen.flagMap[num * 10 + num3];
				component.SetBool(name, true);
				num2 = num3 - num;
			}
			if (text == "BaseCaptured")
			{
				Service.AudioManager.PlayAudioDelayed("sfx_stinger_factory_outpost_captured", 1f);
			}
			else if (text == "BaseChangeOwner")
			{
				Service.AudioManager.PlayAudioDelayed("sfx_stinger_factory_outpost_captured", 2f);
			}
			else if (text == "DisableUplink" && num2 > 0)
			{
				float delay = (num <= 0) ? 0.183f : 0.433f;
				Service.AudioManager.PlayAudioRepeat("sfx_stinger_uplink_disabled", num2, delay, 1.35f);
			}
			component.SetTrigger(text);
		}

		protected override void InitElements()
		{
			base.InitStars();
		}

		protected override void InitLabels()
		{
			base.GetElement<UXLabel>("LabelTitleSquadWar").Text = this.lang.Get("ATTACK_RESULT", new object[0]);
			base.GetElement<UXLabel>("LabelBuffsLeftSquadWar").Text = this.lang.Get("DEFENDER_BUFFS", new object[0]);
			base.GetElement<UXLabel>("LabelBuffsRightSquadWar").Text = this.lang.Get("ATTACKER_BUFFS", new object[0]);
			this.troopsLabel = base.GetElement<UXLabel>("LabelTroopsExpendedSquadWar");
			this.troopsLabel.Text = this.lang.Get("EXPENDED", new object[0]);
			this.buffBaseUpgradeLabel = base.GetElement<UXLabel>("LabelBuffBaseUpgradeResultSquadWar");
			this.buffBaseUpgradeLabel.Visible = false;
			this.labelBuffBaseCapturedSquadWar = base.GetElement<UXLabel>("LabelBuffBaseCapturedSquadWar");
			this.labelBuffBaseCapturedSquadWar.Visible = false;
			base.InitLabels();
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.primaryActionButton.OnClicked = new UXButtonClickedDelegate(this.OnSquadWarBattleEndButtonClicked);
		}

		private void OnSquadWarBattleEndButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.replayBattleButton.Enabled = false;
			Service.BattlePlaybackController.DiscardLastReplay();
			Service.SquadController.WarManager.StartTranstionFromWarBaseToWarBoard();
		}

		private void InitVictoryPoints(BattleEntry battle)
		{
			int damagePercent = battle.DamagePercent;
			int earnedStars = battle.EarnedStars;
			bool flag = earnedStars >= 3 || (earnedStars >= 2 && damagePercent >= 50) || (earnedStars >= 1 && damagePercent < 50);
			base.GetElement<UXSprite>("SpriteIconVictoryPointLeftSquadWar").Color = ((earnedStars < 1) ? Color.gray : Color.white);
			base.GetElement<UXSprite>("SpriteIconVictoryPointMiddleSquadWar").Color = ((earnedStars < 2) ? Color.gray : Color.white);
			base.GetElement<UXSprite>("SpriteIconVictoryPointRightSquadWar").Color = ((earnedStars < 3) ? Color.gray : Color.white);
			base.GetElement<UXSprite>("SpriteIconCheckLeftSquadWar").Visible = (earnedStars >= 1);
			base.GetElement<UXSprite>("SpriteIconCheckMiddleSquadWar").Visible = (earnedStars >= 2);
			base.GetElement<UXSprite>("SpriteIconCheckRightSquadWar").Visible = (earnedStars >= 3);
			if (flag)
			{
				base.GetElement<UXLabel>("LabelVictoryPointReqLeftSquadWar").Text = this.lang.Get("DESTROY_HQ", new object[0]);
				base.GetElement<UXLabel>("LabelVictoryPointReqMiddleSquadWar").Text = this.lang.Get("DAMAGE_50", new object[0]);
				base.GetElement<UXLabel>("LabelVictoryPointReqRightSquadWar").Text = this.lang.Get("DAMAGE_100", new object[0]);
			}
			else
			{
				base.GetElement<UXLabel>("LabelVictoryPointReqLeftSquadWar").Text = this.lang.Get("DAMAGE_50", new object[0]);
				base.GetElement<UXLabel>("LabelVictoryPointReqMiddleSquadWar").Text = this.lang.Get("DAMAGE_100", new object[0]);
				base.GetElement<UXLabel>("LabelVictoryPointReqRightSquadWar").Text = this.lang.Get("DESTROY_HQ", new object[0]);
			}
		}
	}
}
