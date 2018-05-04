using AnimationOrTween;
using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.Leaderboard
{
	public abstract class AbstractLeaderboardRowView
	{
		protected const string NAME_LABEL = "LabelName";

		protected const string PLANET_LABEL = "LabelPlanet";

		protected const string RANK_LABEL = "LabelRank";

		protected const string TYPE_LABEL = "LabelType";

		protected const string MEMBER_NUMBER_LABEL = "LabelMemberNumber";

		protected const string ACTIVE_MEMBER_NUMBER_LABEL = "LabelMemberActiveNumber";

		protected const string ATTACKS_LABEL = "LabelAttacksWon";

		protected const string DEFFENSES_LABEL = "LabelDefensesWon";

		protected const string FACTION_SPRITE = "SpriteFactionIcon";

		protected const string SQUAD_SYMBOL_SPRITE = "SpriteSquadSymbol";

		protected const string PLAYER_FACTION_UPGRADE_SPRITE = "SpriteFactionUpgradeIcon";

		protected const string BUTTON_CONTAINER = "ButtonContainer";

		protected const string BACK_BUTTON = "BtnScrollBack";

		protected const string PRIMARY_BUTTON = "BtnPrimary";

		protected const string PRIMARY_BUTTON_LABEL = "LabelBtnPrimary";

		protected const string SECONDARY_BUTTON = "BtnSecondary";

		protected const string SECONDARY_BUTTON_LABEL = "LabelBtnSecondary";

		protected const string TERTIARY_BUTTON = "BtnTertiary";

		protected const string TERTIARY_BUTTON_LABEL = "LabelBtnTertiary";

		protected const string MEDAL_GROUP = "SquadInfoMedals";

		protected const string MEDAL_LABEL = "LabelScore";

		protected const string TOURNAMENT_MEDAL_GROUP = "SquadInfoTournamentMedals";

		protected const string TOURNAMENT_MEDAL_LABEL = "LabelScoreTournamentMedals";

		protected const string TOURNAMENT_MEDAL_SPRITE = "SpriteSquadInfoTournamentMedalIcon";

		protected const string BG_SPRITE = "SpriteLeaderboardBg";

		protected const string PLANET_BG_TEXTURE = "TexturePlanetBg";

		protected const string FRIEND_TEXTURE = "FriendPic";

		protected const string SPRITE_SCROLL_ARROW = "SpriteScrollBtn";

		protected const string GROUP_SQUAD_LEVEL = "GroupSquadLevel";

		protected const string LABEL_SQUAD_LEVEL = "LabelSquadLvl";

		protected const string SPRITE_BG_HIGHLIGHT = "BgGradientRowHighlight";

		protected const string SPRITE_BG_DEFAULT = "BgGradientRow";

		protected const float SPRITE_BG_HIGHLIGHT_ALPHA = 0.6f;

		protected const string EMPIRE_ICON_NAME = "FactionEmpire";

		protected const string REBEL_ICON_NAME = "FactionRebel";

		private const int ONE_BUTTON_TWEEN_X = 25;

		private const int THREE_BUTTON_TWEEN_X = -332;

		protected const string NEW_ELEMENT_FORMAT = "{0}{1}";

		protected UXLabel nameLabel;

		protected UXLabel typeLabel;

		protected UXSprite squadSymbolSprite;

		protected UXLabel memberNumberLabel;

		protected UXLabel activeMemberNumberLabel;

		protected UXLabel rankLabel;

		protected UXTexture friendTexture;

		protected UXSprite squadFactionSprite;

		protected UXSprite playerFactionSprite;

		protected UXButton backButton;

		protected UXElement buttonContainer;

		protected UXLabel primaryButtonLabel;

		protected UXButton primaryButton;

		protected UXLabel secondaryButtonLabel;

		protected UXButton secondaryButton;

		protected UXLabel tertiaryButtonLabel;

		protected UXButton tertiaryButton;

		protected UXLabel planetLabel;

		protected UXTexture planetBgTexture;

		protected UXElement medalGroup;

		protected UXElement tournamentMedalGroup;

		protected UXLabel tournamentMedalLabel;

		protected UXSprite tournamentMedalSprite;

		protected UXLabel medalLabel;

		protected UXLabel attacksLabel;

		protected UXLabel defensesLabel;

		protected UXSprite bgSprite;

		protected UXSprite arrowSprite;

		protected UXElement squadLevelGroup;

		protected UXLabel squadLevelLabel;

		protected AbstractLeaderboardScreen screen;

		protected UXGrid grid;

		protected UXElement templateItem;

		protected SocialTabs tab;

		protected FactionToggle faction;

		protected int position;

		protected UIPlayTween tween;

		protected string id;

		protected UXElement item;

		public AbstractLeaderboardRowView(AbstractLeaderboardScreen screen, UXGrid grid, UXElement templateItem, SocialTabs tab, FactionToggle faction, int position, bool initAllElements)
		{
			this.screen = screen;
			this.grid = grid;
			this.templateItem = templateItem;
			this.tab = tab;
			this.faction = faction;
			this.position = position;
			this.CreateItem();
			if (initAllElements)
			{
				this.InitElements();
			}
		}

		protected abstract void CreateItem();

		public abstract void Destroy();

		protected void InitElements()
		{
			this.nameLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelName");
			this.typeLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelType");
			this.squadSymbolSprite = this.grid.GetSubElement<UXSprite>(this.id, "SpriteSquadSymbol");
			this.memberNumberLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelMemberNumber");
			this.activeMemberNumberLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelMemberActiveNumber");
			this.rankLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelRank");
			this.friendTexture = this.grid.GetSubElement<UXTexture>(this.id, "FriendPic");
			this.squadFactionSprite = this.grid.GetSubElement<UXSprite>(this.id, "SpriteFactionIcon");
			this.playerFactionSprite = this.grid.GetSubElement<UXSprite>(this.id, "SpriteFactionUpgradeIcon");
			this.backButton = this.grid.GetSubElement<UXButton>(this.id, "BtnScrollBack");
			this.buttonContainer = this.grid.GetSubElement<UXElement>(this.id, "ButtonContainer");
			this.primaryButtonLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelBtnPrimary");
			this.primaryButton = this.grid.GetSubElement<UXButton>(this.id, "BtnPrimary");
			this.secondaryButtonLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelBtnSecondary");
			this.secondaryButton = this.grid.GetSubElement<UXButton>(this.id, "BtnSecondary");
			this.tertiaryButtonLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelBtnTertiary");
			this.tertiaryButton = this.grid.GetSubElement<UXButton>(this.id, "BtnTertiary");
			this.planetLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelPlanet");
			this.planetBgTexture = this.grid.GetSubElement<UXTexture>(this.id, "TexturePlanetBg");
			this.medalGroup = this.grid.GetSubElement<UXElement>(this.id, "SquadInfoMedals");
			this.tournamentMedalGroup = this.grid.GetSubElement<UXElement>(this.id, "SquadInfoTournamentMedals");
			this.tournamentMedalLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelScoreTournamentMedals");
			this.tournamentMedalSprite = this.grid.GetSubElement<UXSprite>(this.id, "SpriteSquadInfoTournamentMedalIcon");
			this.medalLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelScore");
			this.attacksLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelAttacksWon");
			this.defensesLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelDefensesWon");
			this.bgSprite = this.grid.GetSubElement<UXSprite>(this.id, "SpriteLeaderboardBg");
			this.arrowSprite = this.grid.GetSubElement<UXSprite>(this.id, "SpriteScrollBtn");
			this.squadLevelGroup = this.grid.GetSubElement<UXElement>(this.id, "GroupSquadLevel");
			this.squadLevelLabel = this.grid.GetSubElement<UXLabel>(this.id, "LabelSquadLvl");
			this.item.AddUXButton(this.screen.GetElement<UXButton>(this.item.Root.name));
			this.item.OnElementClicked = new UXButtonClickedDelegate(this.OnClicked);
			this.backButton.OnClicked = new UXButtonClickedDelegate(this.OnBackButtonClicked);
			this.tween = this.item.Root.GetComponent<UIPlayTween>();
			this.tween.playDirection = Direction.Forward;
			UIPlayTween component = this.backButton.Root.GetComponent<UIPlayTween>();
			component.tweenTarget = null;
			this.secondaryButton.Visible = false;
			this.tertiaryButton.Visible = false;
		}

		public UXElement GetItem()
		{
			return this.item;
		}

		protected void OnClicked(UXButton button)
		{
			Service.EventManager.SendEvent(EventId.SquadMore, null);
			this.screen.OnRowSelected(this);
		}

		private void OnBackButtonClicked(UXButton button)
		{
			this.screen.OnRowSelected(this);
		}

		public void Deselect()
		{
			if (this.tween != null)
			{
				this.tween.Play(false);
			}
		}

		protected void UpdateButtonContainerTween(UXElement buttonContainer, int numButtons)
		{
			if (numButtons != 1 && numButtons != 3)
			{
				return;
			}
			TweenPosition component = buttonContainer.Root.GetComponent<TweenPosition>();
			Vector3 to = component.to;
			if (numButtons == 1)
			{
				to.x = 25f;
			}
			else if (numButtons == 3)
			{
				to.x = -332f;
			}
			component.to = to;
		}

		protected void ToggleHighlight(bool highlight)
		{
			if (highlight)
			{
				this.bgSprite.SpriteName = "BgGradientRowHighlight";
				this.bgSprite.Color = Color.white;
				this.bgSprite.Alpha = 0.6f;
			}
			else
			{
				this.bgSprite.SpriteName = "BgGradientRow";
			}
		}
	}
}
