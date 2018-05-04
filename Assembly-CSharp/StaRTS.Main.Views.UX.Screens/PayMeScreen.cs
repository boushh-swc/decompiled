using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class PayMeScreen : AlertScreen, IEventObserver
	{
		public static readonly object MODALRESULT_CONFIRMED = new object();

		public static readonly object MODALRESULT_CONFIRMED_ALTERNATE = new object();

		private int crystals;

		private bool forDroids;

		private bool closing;

		private PayMeScreen(int crystals, bool forDroids, string title, string message, string spriteName) : base(false, title, message, spriteName, false)
		{
			this.crystals = crystals;
			this.forDroids = forDroids;
			this.closing = false;
			if (forDroids)
			{
				Service.EventManager.RegisterObserver(this, EventId.ContractCompleted, EventPriority.Default);
				Service.EventManager.RegisterObserver(this, EventId.ContractCanceled, EventPriority.Default);
			}
		}

		private static void ShowModal(int crystals, bool forDroids, string title, string message, string spriteName, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			PayMeScreen payMeScreen = new PayMeScreen(crystals, forDroids, title, message, spriteName);
			payMeScreen.OnModalResult = onModalResult;
			payMeScreen.ModalResultCookie = modalResultCookie;
			Service.ScreenController.AddScreen(payMeScreen);
		}

		public static bool ShowIfNoFreeDroids(OnScreenModalResult onModalResult, object modalResultCookie)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int num = ContractUtils.CalculateDroidsInUse();
			if (num >= currentPlayer.CurrentDroidsAmount)
			{
				Lang lang = Service.Lang;
				string title = lang.Get("droid_title_AllDroidsBusy", new object[0]);
				string message = (currentPlayer.CurrentDroidsAmount >= currentPlayer.MaxDroidsAmount) ? lang.Get("droid_desc_CompletePreviousBuilding", new object[0]) : lang.Get("droid_desc_CompletePreviousBuildingOrBuy", new object[0]);
				int num2 = ContractUtils.MinimumCostToFinish();
				PayMeScreen.ShowModal(num2, true, title, message, null, onModalResult, modalResultCookie);
				return true;
			}
			return false;
		}

		public static bool ShowIfNotEnoughCurrency(int credits, int materials, int contraband, string purchaseContext, OnScreenModalResult onModalResult)
		{
			return PayMeScreen.ShowIfNotEnoughCurrency(credits, materials, contraband, purchaseContext, null, onModalResult);
		}

		public static bool ShowIfNotEnoughCurrency(int credits, int materials, int contraband, string purchaseContext, object purchaseCookie, OnScreenModalResult onModalResult)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			CurrencyType currencyType = GameUtils.GetCurrencyType(credits, materials, contraband);
			int num = 0;
			if (currencyType != CurrencyType.Credits)
			{
				if (currencyType != CurrencyType.Materials)
				{
					if (currencyType == CurrencyType.Contraband)
					{
						num = contraband - currentPlayer.CurrentContrabandAmount;
					}
				}
				else
				{
					num = materials - currentPlayer.CurrentMaterialsAmount;
				}
			}
			else
			{
				num = credits - currentPlayer.CurrentCreditsAmount;
			}
			if (num > 0)
			{
				if (GameUtils.HasEnoughCurrencyStorage(currencyType, num))
				{
					Lang lang = Service.Lang;
					string currencyStringId = LangUtils.GetCurrencyStringId(currencyType);
					string text = lang.Get(currencyStringId, new object[0]);
					string title = lang.Get("NEED_MORE", new object[]
					{
						text
					});
					string message = lang.Get("NEED_MORE_BUY_MISSING", new object[]
					{
						lang.ThousandsSeparated(num),
						text
					});
					int num2 = 0;
					if (currencyType != CurrencyType.Credits)
					{
						if (currencyType != CurrencyType.Materials)
						{
							if (currencyType == CurrencyType.Contraband)
							{
								num2 = GameUtils.ContrabandCrystalCost(num);
							}
						}
						else
						{
							num2 = GameUtils.MaterialsCrystalCost(num);
						}
					}
					else
					{
						num2 = GameUtils.CreditsCrystalCost(num);
					}
					string currencyItemAssetName = UXUtils.GetCurrencyItemAssetName(currencyType.ToString());
					CurrencyTag modalResultCookie = new CurrencyTag(currencyType, num, num2, purchaseContext, purchaseCookie);
					PayMeScreen.ShowModal(num2, false, title, message, currencyItemAssetName, onModalResult, modalResultCookie);
				}
				else
				{
					onModalResult(null, null);
				}
				return true;
			}
			return false;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ContractCompleted || id == EventId.ContractCanceled)
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				if (ContractUtils.ContractTypeConsumesDroid(contractEventData.Contract.ContractTO.ContractType) && !this.closing)
				{
					this.Close(null);
				}
			}
			return base.OnEvent(id, cookie);
		}

		public override void OnDestroyElement()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ContractCompleted);
			Service.EventManager.UnregisterObserver(this, EventId.ContractCanceled);
			base.OnDestroyElement();
		}

		protected override void SetupControls()
		{
			base.GetElement<UXLabel>("TickerDialogSmall").Visible = false;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (this.forDroids && currentPlayer.CurrentDroidsAmount < currentPlayer.MaxDroidsAmount)
			{
				this.payLeftLabel.Text = this.lang.Get("FINISH", new object[0]);
				this.payLeftButton.Visible = true;
				this.payLeftButton.OnClicked = new UXButtonClickedDelegate(this.OnPayButtonClicked);
				UXUtils.SetupCostElements(this, "CostOptionPay1", null, 0, 0, 0, this.crystals, false, null);
				this.payRightLabel.Text = this.lang.Get("ADD_DROID", new object[0]);
				this.payRightButton.Visible = true;
				this.payRightButton.OnClicked = new UXButtonClickedDelegate(this.OnAlternatePayButtonClicked);
				int num = GameUtils.DroidCrystalCost(currentPlayer.CurrentDroidsAmount);
				UXUtils.SetupCostElements(this, "CostOptionPay2", null, 0, 0, 0, num, false, null);
				this.centerLabel.Text = this.message;
				this.centerLabel.Visible = true;
				this.rightLabel.Visible = false;
			}
			else
			{
				this.payButton.Visible = true;
				this.payButton.OnClicked = new UXButtonClickedDelegate(this.OnPayButtonClicked);
				UXUtils.SetupCostElements(this, "Cost", null, 0, 0, 0, this.crystals, false, null);
				if (this.forDroids)
				{
					this.centerLabel.Text = this.message;
					this.centerLabel.Visible = true;
					this.rightLabel.Visible = false;
				}
				else
				{
					this.rightLabel.Text = this.message;
					this.centerLabel.Visible = false;
					this.rightLabel.Visible = true;
					this.sprite.SpriteName = this.spriteName;
					UXUtils.SetupGeometryForIcon(this.sprite, this.spriteName);
				}
			}
			this.titleLabel.Text = this.title;
		}

		public void OnPayButtonClicked(UXButton button)
		{
			this.closing = true;
			object modalResult = null;
			button.Enabled = false;
			if (!this.forDroids)
			{
				Service.EventManager.SendEvent(EventId.UINotEnoughSoftCurrencyBuy, base.ModalResultCookie);
				modalResult = PayMeScreen.MODALRESULT_CONFIRMED;
			}
			else if (ContractUtils.InstantFreeupDroid())
			{
				Service.EventManager.SendEvent(EventId.UINotEnoughDroidSpeedUp, base.ModalResultCookie);
				modalResult = PayMeScreen.MODALRESULT_CONFIRMED;
			}
			this.Close(modalResult);
		}

		public void OnAlternatePayButtonClicked(UXButton button)
		{
			this.closing = true;
			object modalResult = null;
			button.Enabled = false;
			if (!this.forDroids)
			{
				Service.EventManager.SendEvent(EventId.UINotEnoughSoftCurrencyBuy, base.ModalResultCookie);
				modalResult = PayMeScreen.MODALRESULT_CONFIRMED_ALTERNATE;
			}
			else if (GameUtils.BuyNextDroid(true))
			{
				Service.EventManager.SendEvent(EventId.UINotEnoughDroidBuy, base.ModalResultCookie);
				modalResult = PayMeScreen.MODALRESULT_CONFIRMED_ALTERNATE;
			}
			this.Close(modalResult);
		}

		public override void Close(object modalResult)
		{
			base.Close(modalResult);
			if (modalResult != null)
			{
				return;
			}
			if (this.forDroids)
			{
				Service.EventManager.SendEvent(EventId.UINotEnoughDroidClose, base.ModalResultCookie);
			}
			else
			{
				Service.EventManager.SendEvent(EventId.UINotEnoughSoftCurrencyClose, base.ModalResultCookie);
			}
		}
	}
}
