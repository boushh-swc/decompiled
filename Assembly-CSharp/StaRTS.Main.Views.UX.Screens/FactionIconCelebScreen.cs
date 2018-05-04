using StaRTS.Main.Models.Player;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class FactionIconCelebScreen : ScreenBase
	{
		private const string SHOW = "Show";

		private const float ANIMATION_TIME = 8f;

		private const string TEXTURE_FACTION_ICON = "TextureFactionIcon";

		private const string TEXTURE_POSTFIX = "_C";

		private const string LABEL_TITLE_BATTLESWON_FACTIONICON = "LabelTitleBattlesWonFactionIcon";

		private const string LABEL_TITLE_DEFENSESWON_FACTIONICON = "LabelTitleDefensesWonFactionIcon";

		private const string BATTLES_WON_FACTION_CELEB = "FACTION_ICON_CELEB_BATTLES_WON";

		private const string DEFENSES_WON_FACTION_CELEB = "FACTION_ICON_CELEB_DEFENSES_WON";

		private UXLabel battlesWonLabel;

		private UXLabel defensesWonLabel;

		private UXTexture iconTexture;

		private string iconAssetName;

		private uint timerId;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public FactionIconCelebScreen(string image) : base("gui_FactionIcon_celebration")
		{
			this.timerId = 0u;
			this.iconAssetName = image + "_C";
		}

		protected override void OnScreenLoaded()
		{
			this.battlesWonLabel = base.GetElement<UXLabel>("LabelTitleBattlesWonFactionIcon");
			this.defensesWonLabel = base.GetElement<UXLabel>("LabelTitleDefensesWonFactionIcon");
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.battlesWonLabel.Text = this.lang.Get("FACTION_ICON_CELEB_BATTLES_WON", new object[]
			{
				currentPlayer.AttacksWon
			});
			this.defensesWonLabel.Text = this.lang.Get("FACTION_ICON_CELEB_DEFENSES_WON", new object[]
			{
				currentPlayer.DefensesWon
			});
			this.iconTexture = base.GetElement<UXTexture>("TextureFactionIcon");
			this.iconTexture.LoadTexture(this.iconAssetName, new Action(this.OnIconLoadComplete), new Action(this.OnIconLoadFailed));
		}

		public override void OnDestroyElement()
		{
			if (this.timerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.timerId);
				this.timerId = 0u;
			}
			base.OnDestroyElement();
		}

		private void HideAndCloseCelebration()
		{
			this.Visible = false;
			this.Close(null);
		}

		private void OnIconLoadFailed()
		{
			this.HideAndCloseCelebration();
		}

		private void OnAnimationFinishedTimer(uint id, object cookie)
		{
			this.timerId = 0u;
			this.HideAndCloseCelebration();
		}

		private void OnIconLoadComplete()
		{
			Animator componentInChildren = base.Root.GetComponentInChildren<Animator>();
			if (componentInChildren != null)
			{
				componentInChildren.SetTrigger("Show");
				this.timerId = Service.ViewTimerManager.CreateViewTimer(8f, false, new TimerDelegate(this.OnAnimationFinishedTimer), null);
			}
			else
			{
				this.HideAndCloseCelebration();
			}
		}
	}
}
