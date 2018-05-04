using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class CrateRewardModalScreen : ClosableScreen
	{
		private const string LANG_CRATE_REWARD_POPUP_TITLE = "CRATE_REWARD_POPUP_TITLE";

		private const string LANG_CRATE_REWARD_POPUP_DESC = "CRATE_REWARD_POPUP_DESC";

		private const string LANG_CRATE_REWARD_POPUP_CTA = "CRATE_REWARD_POPUP_CTA";

		private const string LABEL_TITLE = "LabelTitleRewardNotif";

		private const string SPRITE_REWARD = "SpriteReward";

		private const string LABEL_REWARD_DESCRIPTION = "LabelRewardDescription";

		private const string BUTTON_PRIMARY = "BtnPrimary";

		private const string LABEL_BUTTON_PRIMARY = "LabelBtnPrimary";

		private CrateVO targetCrateVO;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		public CrateRewardModalScreen(CrateVO crateVO) : base("gui_modal_rewardnotif")
		{
			this.targetCrateVO = crateVO;
		}

		protected override void OnScreenLoaded()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelTitleRewardNotif");
			element.Text = this.lang.Get("CRATE_REWARD_POPUP_TITLE", new object[0]);
			UXSprite element2 = base.GetElement<UXSprite>("SpriteReward");
			RewardUtils.SetCrateIcon(element2, this.targetCrateVO, AnimState.Closed);
			string crateDisplayName = LangUtils.GetCrateDisplayName(this.targetCrateVO);
			UXLabel element3 = base.GetElement<UXLabel>("LabelRewardDescription");
			element3.Text = this.lang.Get("CRATE_REWARD_POPUP_DESC", new object[]
			{
				crateDisplayName
			});
			UXButton element4 = base.GetElement<UXButton>("BtnPrimary");
			element4.OnClicked = new UXButtonClickedDelegate(this.OnPrimaryButtonClicked);
			UXLabel element5 = base.GetElement<UXLabel>("LabelBtnPrimary");
			element5.Text = this.lang.Get("CRATE_REWARD_POPUP_CTA", new object[0]);
			this.InitButtons();
		}

		private void OnPrimaryButtonClicked(UXButton button)
		{
			ScreenBase screenBase = Service.UXController.HUD.CreatePrizeInventoryScreen();
			if (screenBase != null)
			{
				Service.ScreenController.AddScreen(screenBase);
			}
			this.Close(null);
		}
	}
}
