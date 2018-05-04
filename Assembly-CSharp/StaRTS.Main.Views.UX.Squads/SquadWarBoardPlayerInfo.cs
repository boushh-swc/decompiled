using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Squads
{
	public class SquadWarBoardPlayerInfo : AbstractSquadWarBoardInfoElement
	{
		public SquadWarParticipantState participantState;

		private const string WAR_BOARD_HQ_LABEL = "WAR_BOARD_HQ_LABEL";

		private const string LABEL_TROOP_TOOLTIP = "LabelTroopTooltip";

		private const string SPRITE_POINT_1 = "SpriteStar1";

		private const string SPRITE_POINT_2 = "SpriteStar2";

		private const string SPRITE_POINT_3 = "SpriteStar3";

		private const string GROUP_STARS = "GroupStars";

		private const string STAR_BG = "SpriteTroopTooltip";

		private const string PLAYER_HIGHLIGHT = "SpritePlayerHighlight";

		private UXLabel labelTroopTooltip;

		private UXSprite spritePoint1;

		private UXSprite spritePoint2;

		private UXSprite spritePoint3;

		private UXSprite starBG;

		private UXElement groupStars;

		private UXSprite playerHighlight;

		private bool internalDisplay;

		private bool isCurrentPlayer;

		public SquadWarBoardPlayerInfo(SquadWarParticipantState participantState, Transform transformToTrack) : base("gui_squadwar_playerinfo", transformToTrack)
		{
			this.participantState = participantState;
			this.isCurrentPlayer = (participantState.SquadMemberId == Service.CurrentPlayer.PlayerId);
		}

		protected override void SetupView()
		{
			this.labelTroopTooltip = base.GetElement<UXLabel>("LabelTroopTooltip");
			this.labelTroopTooltip.Text = Service.Lang.Get("WAR_BOARD_HQ_LABEL", new object[]
			{
				this.participantState.SquadMemberName,
				this.participantState.HQLevel
			});
			this.spritePoint1 = base.GetElement<UXSprite>("SpriteStar1");
			this.spritePoint2 = base.GetElement<UXSprite>("SpriteStar2");
			this.spritePoint3 = base.GetElement<UXSprite>("SpriteStar3");
			this.groupStars = base.GetElement<UXElement>("GroupStars");
			this.starBG = base.GetElement<UXSprite>("SpriteTroopTooltip");
			this.playerHighlight = base.GetElement<UXSprite>("SpritePlayerHighlight");
			this.playerHighlight.Visible = this.isCurrentPlayer;
			Service.EventManager.RegisterObserver(this, EventId.WarVictoryPointsUpdated);
			this.UpdateView();
		}

		protected override void UpdateView()
		{
			UXUtils.UpdateUplinkHelper(this.spritePoint1, this.participantState.VictoryPointsLeft >= 3, true);
			UXUtils.UpdateUplinkHelper(this.spritePoint2, this.participantState.VictoryPointsLeft >= 2, true);
			UXUtils.UpdateUplinkHelper(this.spritePoint3, this.participantState.VictoryPointsLeft >= 1, true);
			this.ToggleDisplay(this.internalDisplay);
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.WarVictoryPointsUpdated)
			{
				SquadWarParticipantState squadWarParticipantState = (SquadWarParticipantState)cookie;
				if (squadWarParticipantState != null && squadWarParticipantState.SquadMemberId == this.participantState.SquadMemberId)
				{
					this.UpdateView();
				}
			}
			return base.OnEvent(id, cookie);
		}

		public override void Destroy()
		{
			base.Destroy();
			Service.EventManager.UnregisterObserver(this, EventId.WarVictoryPointsUpdated);
			this.participantState = null;
			this.labelTroopTooltip = null;
			this.spritePoint1 = null;
			this.spritePoint2 = null;
			this.spritePoint3 = null;
		}

		public Vector3 GetPositionOfCenterStar()
		{
			return this.spritePoint2.Root.transform.position;
		}

		public void ToggleDisplay(bool flag)
		{
			this.internalDisplay = flag;
			if (base.IsLoaded())
			{
				this.groupStars.Visible = flag;
				this.labelTroopTooltip.Visible = flag;
				this.playerHighlight.Visible = (flag && this.isCurrentPlayer);
			}
		}

		protected override void FadeElements(float proportion)
		{
			this.Visible = (proportion > 0f);
			if (!this.Visible || this.labelTroopTooltip == null)
			{
				return;
			}
			this.labelTroopTooltip.TextColor = new Color(this.labelTroopTooltip.TextColor.r, this.labelTroopTooltip.TextColor.g, this.labelTroopTooltip.TextColor.b, proportion);
			this.spritePoint1.Alpha = proportion;
			this.spritePoint2.Alpha = proportion;
			this.spritePoint3.Alpha = proportion;
			this.starBG.Alpha = proportion;
			this.playerHighlight.Alpha = proportion;
		}
	}
}
