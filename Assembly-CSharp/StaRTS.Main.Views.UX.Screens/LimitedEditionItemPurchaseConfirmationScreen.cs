using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class LimitedEditionItemPurchaseConfirmationScreen : AlertScreen
	{
		private const string SECONDARY_BUTTON = "CANCEL";

		private const string PURCHASE_LEI = "PURCHASE_LEI";

		public LimitedEditionItemVO LeiVO;

		private LimitedEditionItemPurchaseConfirmationScreen() : base(false, null, null, null, false)
		{
		}

		public static AlertScreen ShowModal(LimitedEditionItemVO leiVO, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			LimitedEditionItemPurchaseConfirmationScreen limitedEditionItemPurchaseConfirmationScreen = new LimitedEditionItemPurchaseConfirmationScreen();
			limitedEditionItemPurchaseConfirmationScreen.LeiVO = leiVO;
			limitedEditionItemPurchaseConfirmationScreen.geometry = leiVO;
			limitedEditionItemPurchaseConfirmationScreen.title = LangUtils.GetLEIDisplayName(leiVO.Uid);
			limitedEditionItemPurchaseConfirmationScreen.message = limitedEditionItemPurchaseConfirmationScreen.lang.Get("PURCHASE_LEI", new object[]
			{
				limitedEditionItemPurchaseConfirmationScreen.title
			});
			limitedEditionItemPurchaseConfirmationScreen.OnModalResult = onModalResult;
			limitedEditionItemPurchaseConfirmationScreen.ModalResultCookie = modalResultCookie;
			limitedEditionItemPurchaseConfirmationScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(limitedEditionItemPurchaseConfirmationScreen);
			return limitedEditionItemPurchaseConfirmationScreen;
		}

		protected override void SetupControls()
		{
			base.SetupControls();
			this.primaryButton.Visible = false;
			this.payRightLabel.Visible = false;
			this.payRightButton.OnClicked = new UXButtonClickedDelegate(base.OnPrimaryButtonClicked);
			this.secondary2Option.Text = this.lang.Get("CANCEL", new object[0]);
			this.secondary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnSecondButtonClicked);
			this.CloseButton.OnClicked = new UXButtonClickedDelegate(this.OnSecondButtonClicked);
			this.payRightButton.Visible = true;
			this.secondary2Option.Visible = true;
			this.secondary2OptionButton.Visible = true;
			UXUtils.SetupCostElements(this, "CostOptionPay2", null, this.LeiVO.Credits, this.LeiVO.Materials, this.LeiVO.Contraband, this.LeiVO.Crystals, false, null);
		}

		private void OnSecondButtonClicked(UXButton cancelButton)
		{
			this.OnCloseButtonClicked(cancelButton);
		}
	}
}
