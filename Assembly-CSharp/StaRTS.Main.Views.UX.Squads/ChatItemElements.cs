using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Squads
{
	public class ChatItemElements
	{
		public const string CI_PRIMARY_BTN = "BtnPrimary";

		public const string CI_PRIMARY_BTN_LABEL = "LabelBtnPrimary";

		public const string CI_SECONDARY_BTN = "BtnSecondary";

		public const string CI_SECONDARY_BTN_LABEL = "LabelBtnSecondary";

		public const string CI_DONATE_PBAR = "PbarDonate";

		public const string CI_DONATE_PBAR_LABEL = "LabelPbarDonate";

		public const string CI_MESSAGE_BG = "SpritePlayerMessage";

		public const string CI_REPLAY_SECTION = "Replay";

		public const string CI_RP_DAMAGE_LABEL = "LabelDamage";

		public const string CI_RP_OPPONENT_LABEL = "LabelOpponentName";

		public const string CI_RP_TYPE_LABEL = "LabelReplayType";

		public const string CI_RP_STAR_1 = "SpriteStar1";

		public const string CI_RP_STAR_2 = "SpriteStar2";

		public const string CI_RP_STAR_3 = "SpriteStar3";

		public const string CI_PLAYER_MESSAGE_LABEL = "LabelPlayerMessage";

		public const string CI_PLAYER_NAME_LABEL = "LabelPlayerName";

		public const string CI_PLAYER_ROLE_LABEL = "LabelPlayerRole";

		public const string CI_TIMESTAMP_LABEL = "LabelTimeStamp";

		public const string CI_STATUS_LABEL = "LabelSquadUpdate";

		public const string CI_MESSSAGE_ARROW_SPRITE = "SpritePlayerMessageArrow";

		public const string CI_LABLE_ITEM_REPLAYMEDALS = "LabelReplayMedals";

		public const string CI_REWARD_LABEL = "LabelDonateReward";

		public const string CI_CONTAINER_CHAT = "ContainerChat";

		public const string CI_CONTAINER_CHAT_WAR = "ContainerChatWar";

		public const string CI_SPRITE_WAR_ICON = "SpriteWarIcon";

		public const string CI_WAR_REQUEST_TEXTURE = "TextureWarRequest";

		public const string WAR_REQUEST_TEXTURE_NAME = "squadwars_chatrequest_row";

		public UXElement parent;

		public UXButton PrimaryButton
		{
			get;
			set;
		}

		public UXLabel PrimaryButtonLabel
		{
			get;
			set;
		}

		public UXButton SecondaryButton
		{
			get;
			set;
		}

		public UXLabel SecondaryButtonLabel
		{
			get;
			set;
		}

		public UXSlider DonateProgBar
		{
			get;
			set;
		}

		public UXLabel DonateProgBarLabel
		{
			get;
			set;
		}

		public UXLabel DonateRewardLabel
		{
			get;
			set;
		}

		public UXSprite MessageBG
		{
			get;
			set;
		}

		public UXElement ReplayParent
		{
			get;
			set;
		}

		public UXLabel ReplayTypeLabel
		{
			get;
			set;
		}

		public UXLabel ReplayDamageLabel
		{
			get;
			set;
		}

		public UXLabel ReplayOpponentNameLabel
		{
			get;
			set;
		}

		public UXLabel ReplayMedals
		{
			get;
			set;
		}

		public UXSprite ReplayStar1
		{
			get;
			set;
		}

		public UXSprite ReplayStar2
		{
			get;
			set;
		}

		public UXSprite ReplayStar3
		{
			get;
			set;
		}

		public UXLabel PlayerMessageLabel
		{
			get;
			set;
		}

		public UXLabel PlayerNameLabel
		{
			get;
			set;
		}

		public UXLabel PlayerRoleLabel
		{
			get;
			set;
		}

		public UXLabel LabelSquadUpdate
		{
			get;
			set;
		}

		public UXSprite SpriteMessageArrow
		{
			get;
			set;
		}

		public UXLabel TimestampLabel
		{
			get;
			set;
		}

		public UXElement ContainerChat
		{
			get;
			set;
		}

		public UXElement ContainerChatWar
		{
			get;
			set;
		}

		public UXSprite SpriteWarIcon
		{
			get;
			set;
		}

		public UXTexture WarRequestTexture
		{
			get;
			set;
		}
	}
}
