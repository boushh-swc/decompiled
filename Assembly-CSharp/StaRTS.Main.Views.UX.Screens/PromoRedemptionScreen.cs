using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class PromoRedemptionScreen : AlertScreen
	{
		private string iapUID;

		public PromoRedemptionScreen(string uid, int amount) : base(false, null, null, null, false)
		{
			this.iapUID = uid;
			this.title = this.lang.Get("promo_redeem_title", new object[0]);
			InAppPurchaseTypeVO inAppPurchaseTypeVO = Service.StaticDataController.Get<InAppPurchaseTypeVO>(uid);
			string id = inAppPurchaseTypeVO.RedemptionStringEmpire;
			if (Service.CurrentPlayer.Faction == FactionType.Rebel)
			{
				id = inAppPurchaseTypeVO.RedemptionStringRebel;
			}
			this.message = this.lang.Get(id, new object[]
			{
				amount
			});
			base.AllowFUEBackButton = true;
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			this.sprite.Visible = true;
			Service.InAppPurchaseController.SetIAPRewardIcon(this.sprite, this.iapUID);
		}

		protected override void SetupControls()
		{
			base.SetupControls();
			this.centerLabel.Visible = false;
			this.rightLabel.Visible = true;
			this.rightLabel.Text = this.message;
			this.primaryButton.OnClicked = new UXButtonClickedDelegate(this.OnButtonClicked);
			this.primaryLabel.Text = this.lang.Get("OK", new object[0]);
			base.InitDefaultBackDelegate();
			Service.UserInputInhibitor.AlwaysAllowElement(this.primaryButton);
			Service.UserInputInhibitor.AlwaysAllowElement(this.CloseButton);
		}

		private void OnButtonClicked(UXButton button)
		{
			this.Close(base.OnModalResult);
		}

		public override void OnDestroyElement()
		{
			base.OnDestroyElement();
			this.Visible = false;
		}
	}
}
