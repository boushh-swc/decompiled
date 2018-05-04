using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarPlayerDetailsScreen : ClosableScreen
	{
		private SquadWarParticipantState participantState;

		private const string BUTTON_PLAYER_NEXT = "BtnPlayerNext";

		private const string BUTTON_PLAYER_PREV = "BtnPlayerPrev";

		private const string LABEL_PLAYER_NAME = "LabelPlayerName";

		private const string LABEL_ATTACKS_REMAINING = "LabelAttacksRemaining";

		private const string LABEL_UPLINKS_AVAILABLE = "LabelUplinksAvailable";

		private const string LABEL_STAR_REQUIREMENT = "LabelStarRequirement{0}";

		private const string SPRITE_UPLINK = "SpriteUplink{0}";

		private const string SPRITE_CHECK = "SpriteCheck{0}";

		private const string GROUP_DAMAGE_STARS = "DamageStars{0}";

		private const string SPRITE_FACTION = "SpriteFactionIcon";

		private const string SPRITE_FACTION_DEFAULT = "SpriteFactionIconZero";

		private const string BUTTON_SCOUT = "BtnScout";

		private const string LABEL_BUTTON_SCOUT = "LabelBtnScout";

		private const string TEXTURE_PLAYER_DETAIL = "TexturePlayerDetails";

		private const string TEXTURE_PLAYER_DETAIL_BG = "squadwars_playerdetails_bg";

		private const string WAR_PLAYER_DETAILS_NAME = "WAR_PLAYER_DETAILS_NAME";

		private const string WAR_PLAYER_DETAILS_TURNS_LEFT = "WAR_PLAYER_DETAILS_TURNS_LEFT";

		private const string WAR_PLAYER_DETAILS_POINTS_LEFT = "WAR_PLAYER_DETAILS_POINTS_LEFT";

		private const string WAR_PLAYER_DETAILS_REQ_1 = "WAR_PLAYER_DETAILS_REQ_1";

		private const string WAR_PLAYER_DETAILS_REQ_2 = "WAR_PLAYER_DETAILS_REQ_2";

		private const string WAR_PLAYER_DETAILS_REQ_3 = "WAR_PLAYER_DETAILS_REQ_3";

		private const string WAR_PLAYER_DETAILS_REQ_COMPLETE = "WAR_PLAYER_DETAILS_REQ_COMPLETE";

		private const string SCOUT = "SCOUT";

		private UXButton playerNextButton;

		private UXButton playerPrevButton;

		private UXLabel playerNameLabel;

		private UXLabel attacksRemainingLabel;

		private UXLabel uplinksAvailableLabel;

		private List<UXLabel> requirements;

		private List<UXSprite> uplinks;

		private List<UXSprite> checks;

		private List<UXElement> stars;

		private UXSprite factionSprite;

		private UXSprite factionDefaultSprite;

		private UXButton scoutButton;

		private UXLabel scoutButtonLabel;

		private UXTexture playerDetailTexture;

		public SquadWarPlayerDetailsScreen(SquadWarParticipantState participantState) : base("gui_squadwar_playerdetails")
		{
			this.participantState = participantState;
			this.requirements = new List<UXLabel>();
			this.uplinks = new List<UXSprite>();
			this.checks = new List<UXSprite>();
			this.stars = new List<UXElement>();
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.playerNameLabel = base.GetElement<UXLabel>("LabelPlayerName");
			this.attacksRemainingLabel = base.GetElement<UXLabel>("LabelAttacksRemaining");
			this.uplinksAvailableLabel = base.GetElement<UXLabel>("LabelUplinksAvailable");
			this.factionSprite = base.GetElement<UXSprite>("SpriteFactionIcon");
			this.factionDefaultSprite = base.GetElement<UXSprite>("SpriteFactionIconZero");
			this.playerDetailTexture = base.GetElement<UXTexture>("TexturePlayerDetails");
			StaticDataController staticDataController = Service.StaticDataController;
			TextureVO optional = staticDataController.GetOptional<TextureVO>("squadwars_playerdetails_bg");
			if (optional != null)
			{
				this.playerDetailTexture.LoadTexture(optional.AssetName);
			}
			for (int i = 1; i <= 3; i++)
			{
				this.requirements.Add(base.GetElement<UXLabel>(string.Format("LabelStarRequirement{0}", i)));
				this.uplinks.Add(base.GetElement<UXSprite>(string.Format("SpriteUplink{0}", i)));
				this.checks.Add(base.GetElement<UXSprite>(string.Format("SpriteCheck{0}", i)));
				this.stars.Add(base.GetElement<UXElement>(string.Format("DamageStars{0}", i)));
			}
			this.RefreshView();
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.playerNextButton = base.GetElement<UXButton>("BtnPlayerNext");
			this.playerNextButton.Tag = 1;
			this.playerNextButton.OnClicked = new UXButtonClickedDelegate(this.OnPlayerChangeClicked);
			this.playerPrevButton = base.GetElement<UXButton>("BtnPlayerPrev");
			this.playerPrevButton.Tag = -1;
			this.playerPrevButton.OnClicked = new UXButtonClickedDelegate(this.OnPlayerChangeClicked);
			this.scoutButton = base.GetElement<UXButton>("BtnScout");
			this.scoutButton.OnClicked = new UXButtonClickedDelegate(this.OnScoutClicked);
			this.scoutButtonLabel = base.GetElement<UXLabel>("LabelBtnScout");
			this.scoutButtonLabel.Text = this.lang.Get("SCOUT", new object[0]);
		}

		private void OnPlayerChangeClicked(UXButton button)
		{
			int direction = (int)button.Tag;
			this.participantState = this.GetAdjacentParticipant(this.participantState, direction);
			this.RefreshView();
		}

		private void OnScoutClicked(UXButton button)
		{
			if (this.participantState != null)
			{
				SquadWarManager warManager = Service.SquadController.WarManager;
				string empty = string.Empty;
				if (warManager.CanScoutWarMember(this.participantState.SquadMemberId, ref empty))
				{
					string squadMemberId = this.participantState.SquadMemberId;
					if (warManager.ScoutWarMember(squadMemberId))
					{
						this.Close(null);
					}
				}
				else
				{
					Service.UXController.MiscElementsManager.ShowPlayerInstructions(empty);
				}
			}
		}

		private SquadWarParticipantState GetAdjacentParticipant(SquadWarParticipantState start, int direction)
		{
			SquadWarData currentSquadWar = Service.SquadController.WarManager.CurrentSquadWar;
			if (currentSquadWar == null)
			{
				return start;
			}
			SquadWarSquadData squadWarSquadData = null;
			int num = -1;
			int i = 0;
			int num2 = currentSquadWar.Squads.Length;
			while (i < num2)
			{
				num = currentSquadWar.Squads[i].Participants.IndexOf(start);
				if (num != -1)
				{
					squadWarSquadData = currentSquadWar.Squads[i];
					break;
				}
				i++;
			}
			if (num == -1)
			{
				return start;
			}
			int count = squadWarSquadData.Participants.Count;
			int index = (count + num + direction) % count;
			return squadWarSquadData.Participants[index];
		}

		public override void OnDestroyElement()
		{
			this.requirements.Clear();
			this.uplinks.Clear();
			this.checks.Clear();
			this.stars.Clear();
			base.OnDestroyElement();
		}

		public override void RefreshView()
		{
			if (!base.IsLoaded())
			{
				return;
			}
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarSquadType participantSquad = warManager.GetParticipantSquad(this.participantState.SquadMemberId);
			SquadWarSquadData squadData = warManager.GetSquadData(participantSquad);
			this.playerNameLabel.Text = this.lang.Get("WAR_PLAYER_DETAILS_NAME", new object[]
			{
				this.participantState.SquadMemberName,
				this.participantState.HQLevel
			});
			this.attacksRemainingLabel.Text = this.lang.Get("WAR_PLAYER_DETAILS_TURNS_LEFT", new object[]
			{
				this.participantState.TurnsLeft
			});
			this.uplinksAvailableLabel.Text = this.lang.Get("WAR_PLAYER_DETAILS_POINTS_LEFT", new object[]
			{
				this.participantState.VictoryPointsLeft
			});
			FactionIconUpgradeController factionIconUpgradeController = Service.FactionIconUpgradeController;
			int rating = GameUtils.CalculateVictoryRating(this.participantState.AttacksWon, this.participantState.DefensesWon);
			string icon = factionIconUpgradeController.GetIcon(squadData.Faction, rating);
			if (factionIconUpgradeController.UseUpgradeImage(rating))
			{
				this.factionSprite.SpriteName = icon;
				this.factionSprite.Visible = true;
				this.factionDefaultSprite.Visible = false;
			}
			else
			{
				this.factionSprite.Visible = false;
				this.factionDefaultSprite.Visible = true;
				this.factionDefaultSprite.SpriteName = icon;
			}
			string empty = string.Empty;
			if (warManager.CanScoutWarMember(this.participantState.SquadMemberId, ref empty))
			{
				this.scoutButton.VisuallyEnableButton();
				this.scoutButtonLabel.TextColor = this.scoutButtonLabel.OriginalTextColor;
			}
			else
			{
				this.scoutButton.VisuallyDisableButton();
				this.scoutButtonLabel.TextColor = UXUtils.COLOR_LABEL_DISABLED;
			}
			this.UpdateUplinkHelper(0, this.participantState.VictoryPointsLeft >= 3, "WAR_PLAYER_DETAILS_REQ_1");
			this.UpdateUplinkHelper(1, this.participantState.VictoryPointsLeft >= 2, "WAR_PLAYER_DETAILS_REQ_2");
			this.UpdateUplinkHelper(2, this.participantState.VictoryPointsLeft >= 1, "WAR_PLAYER_DETAILS_REQ_3");
		}

		private void UpdateUplinkHelper(int index, bool active, string reqKey)
		{
			UXUtils.UpdateUplinkHelper(this.uplinks[index], active, false);
			if (active)
			{
				this.requirements[index].Text = this.lang.Get(reqKey, new object[0]);
			}
			else
			{
				this.requirements[index].Text = this.lang.Get("WAR_PLAYER_DETAILS_REQ_COMPLETE", new object[0]);
			}
			this.checks[index].Visible = !active;
			this.stars[index].Visible = active;
		}
	}
}
