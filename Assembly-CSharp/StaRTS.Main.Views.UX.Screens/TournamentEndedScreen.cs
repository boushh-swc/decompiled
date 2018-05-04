using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Tournament;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TournamentEndedScreen : ClosableScreen
	{
		private const string LABEL_TITLE = "DialogTournamentsTitle";

		private const string REWARD_MESSAGE = "LabelBody";

		private const string LABEL_TIER = "LabelFinalTierTitle";

		private const string ICON_TIER = "SpriteFinalTierIcon";

		private const string LABEL_FINAL_TIER = "LabelFinalTier";

		private const string LABEL_TIER_PERCENT = "LabelFinalTierPercent";

		private const string REWARD_TITLE = "LabelRewardTitle";

		private const string REWARD_ICON = "SpriteReward";

		private const string REWARD_NAME = "LabelReward";

		private const string BTN_VIEW_LEADERBOARD = "BtnViewLeaderboard";

		private const string LABEL_VIEW_LEADERBOARD = "LabelViewLeaderboard";

		private const string TEXTURE_BACKGROUND = "TextureTournamentBackground";

		private const string TEXTURE_FOREGROUND_LEFT = "TextureTournamentForegroundLeft";

		private const string TEXTURE_FOREGROUND_RIGHT = "TextureTournamentForegroundRight";

		private const string ANIM_SHOW = "Show";

		private const float ANIM_DELAY = 0.5f;

		private Tournament tournament;

		private TournamentVO tournamentVO;

		private TournamentTierVO tierVO;

		private TournamentRank playerRank;

		private uint timerId;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public TournamentEndedScreen(Tournament tournament) : base("gui_tournaments")
		{
			this.tournament = tournament;
			this.tournamentVO = Service.StaticDataController.Get<TournamentVO>(tournament.Uid);
			this.playerRank = tournament.FinalRank;
			this.tierVO = Service.StaticDataController.Get<TournamentTierVO>(this.playerRank.TierUid);
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.InitLabels();
			this.InitImages();
			this.UpdateRewards();
			base.GetElement<UXButton>("BtnViewLeaderboard").OnClicked = new UXButtonClickedDelegate(this.OnViewLeaderboardClicked);
			this.timerId = Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.ShowReward), null);
		}

		private void ShowReward(uint timerId, object cookie)
		{
			Animator component = base.Root.GetComponent<Animator>();
			component.SetTrigger("Show");
		}

		private void OnViewLeaderboardClicked(UXButton button)
		{
			this.Close(null);
			Service.UXController.HUD.OpenConflictLeaderBoardWithPlanet(this.tournamentVO.PlanetId);
		}

		private void InitLabels()
		{
			string planetDisplayName = LangUtils.GetPlanetDisplayName(this.tournamentVO.PlanetId);
			base.GetElement<UXLabel>("DialogTournamentsTitle").Text = this.lang.Get("CONFLICT_END_POPUP_TITLE", new object[]
			{
				planetDisplayName
			});
			base.GetElement<UXLabel>("LabelBody").Text = this.lang.Get("CONFLICT_ENDED", new object[0]);
			base.GetElement<UXLabel>("LabelFinalTierTitle").Text = this.lang.Get("s_YourTier", new object[0]);
			base.GetElement<UXLabel>("LabelFinalTier").Text = this.lang.Get("CONFLICT_LEAGUE_AND_DIVISION", new object[]
			{
				this.lang.Get(this.tierVO.RankName, new object[0]),
				this.lang.Get(this.tierVO.DivisionSmall, new object[0])
			});
			base.GetElement<UXLabel>("LabelFinalTierPercent").Text = this.lang.Get("CONFLICT_TIER_PERCENTILE", new object[]
			{
				Math.Round(this.playerRank.Percentile, 2)
			});
			base.GetElement<UXLabel>("LabelViewLeaderboard").Text = this.lang.Get("CONFLICT_VIEW_LEADERS", new object[0]);
		}

		private void UpdateRewards()
		{
			base.GetElement<UXLabel>("LabelRewardTitle").Text = this.lang.Get("CONFLICT_END_PRIZE", new object[0]);
			TimedEventPrizeUtils.TrySetupConflictEndedRewardView(this.tournament.RedeemedRewards, base.GetElement<UXLabel>("LabelReward"), base.GetElement<UXSprite>("SpriteReward"));
		}

		private void InitImages()
		{
			if (!string.IsNullOrEmpty(this.tournamentVO.BackgroundTextureName))
			{
				base.GetElement<UXTexture>("TextureTournamentBackground").LoadTexture(this.tournamentVO.BackgroundTextureName);
			}
			base.GetElement<UXSprite>("SpriteFinalTierIcon").SpriteName = Service.TournamentController.GetTierIconName(this.tierVO);
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimerManager.KillViewTimer(this.timerId);
			base.OnDestroyElement();
		}

		public override void Close(object modalResult)
		{
			base.Close(modalResult);
			Service.EventManager.SendEvent(EventId.UITournamentEndSelection, new ActionMessageBIData("close", this.tournamentVO.Uid));
		}
	}
}
