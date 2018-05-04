using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class MultiResourcePayMeScreen : AlertScreen
	{
		private int crystals;

		private List<string> multiItemSpriteNames;

		private List<string> multiItemLabelTexts;

		private MultiResourcePayMeScreen(int crystals, string title, string message, List<string> spriteNames, List<string> labelTexts) : base(false, title, message, string.Empty, false)
		{
			this.crystals = crystals;
			this.multiItemSpriteNames = spriteNames;
			this.multiItemLabelTexts = labelTexts;
		}

		public static bool ShowIfNotEnoughMultipleCurrencies(string[] cost, string purchaseContext, object purchaseCookie, OnScreenModalResult onModalResult)
		{
			int credits;
			int materials;
			int contraband;
			int reputation;
			GameUtils.GetHQScaledCurrency(cost, out credits, out materials, out contraband, out reputation);
			return MultiResourcePayMeScreen.ShowIfNotEnoughMultipleCurrenciesInternal(credits, materials, contraband, reputation, purchaseContext, purchaseCookie, onModalResult);
		}

		public static bool ShowIfNotEnoughMultipleCurrencies(CostVO cost, string purchaseContext, object purchaseCookie, OnScreenModalResult onModalResult)
		{
			return MultiResourcePayMeScreen.ShowIfNotEnoughMultipleCurrenciesInternal(cost.Credits, cost.Materials, cost.Contraband, cost.Reputation, purchaseContext, purchaseCookie, onModalResult);
		}

		public static bool ShowIfNotEnoughMultipleCurrencies(string[] cost, string purchaseContext, OnScreenModalResult onModalResult)
		{
			int credits;
			int materials;
			int contraband;
			int reputation;
			GameUtils.GetHQScaledCurrency(cost, out credits, out materials, out contraband, out reputation);
			return MultiResourcePayMeScreen.ShowIfNotEnoughMultipleCurrenciesInternal(credits, materials, contraband, reputation, purchaseContext, null, onModalResult);
		}

		public static bool ShowIfNotEnoughMultipleCurrencies(CostVO cost, string purchaseContext, OnScreenModalResult onModalResult)
		{
			return MultiResourcePayMeScreen.ShowIfNotEnoughMultipleCurrenciesInternal(cost.Credits, cost.Materials, cost.Contraband, cost.Reputation, purchaseContext, null, onModalResult);
		}

		private static bool ShowIfNotEnoughMultipleCurrenciesInternal(int credits, int materials, int contraband, int reputation, string purchaseContext, object purchaseCookie, OnScreenModalResult onModalResult)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			int num = credits - currentPlayer.CurrentCreditsAmount;
			int num2 = materials - currentPlayer.CurrentMaterialsAmount;
			int num3 = contraband - currentPlayer.CurrentContrabandAmount;
			if (num > 0)
			{
				dictionary.Add(CurrencyType.Credits, num);
			}
			else
			{
				num = 0;
			}
			if (num2 > 0)
			{
				dictionary.Add(CurrencyType.Materials, num2);
			}
			else
			{
				num2 = 0;
			}
			if (num3 > 0)
			{
				dictionary.Add(CurrencyType.Contraband, num3);
			}
			else
			{
				num3 = 0;
			}
			if (dictionary.Count > 0)
			{
				Lang lang = Service.Lang;
				bool flag = true;
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				foreach (KeyValuePair<CurrencyType, int> current in dictionary)
				{
					CurrencyType key = current.Key;
					int value = current.Value;
					flag &= GameUtils.HasEnoughCurrencyStorage(key, value);
					list.Add(UXUtils.GetCurrencyItemAssetName(key.ToString()));
					string currencyStringId = LangUtils.GetCurrencyStringId(key);
					string str = lang.Get(currencyStringId, new object[0]);
					list2.Add(lang.ThousandsSeparated(value) + " " + str);
				}
				if (flag)
				{
					string title = lang.Get("NEED_MORE_MULTI", new object[0]);
					string message = lang.Get("NEED_MORE_MULTI_BUY_MISSING", new object[0]);
					int num4 = GameUtils.MultiCurrencyCrystalCost(dictionary);
					MultiCurrencyTag modalResultCookie = new MultiCurrencyTag(num, num2, num3, num4, purchaseContext, purchaseCookie);
					MultiResourcePayMeScreen multiResourcePayMeScreen = new MultiResourcePayMeScreen(num4, title, message, list, list2);
					multiResourcePayMeScreen.OnModalResult = onModalResult;
					multiResourcePayMeScreen.ModalResultCookie = modalResultCookie;
					Service.ScreenController.AddScreen(multiResourcePayMeScreen);
				}
				else
				{
					onModalResult(null, null);
				}
				return true;
			}
			dictionary.Clear();
			dictionary = null;
			return false;
		}

		protected override void SetupControls()
		{
			base.GetElement<UXLabel>("TickerDialogSmall").Visible = false;
			this.titleLabel.Text = this.title;
			this.payButton.Visible = true;
			this.payButton.OnClicked = new UXButtonClickedDelegate(this.OnPayButtonClicked);
			UXUtils.SetupCostElements(this, "Cost", null, 0, 0, 0, this.crystals, false, null);
			if (this.multiItemSpriteNames != null && this.multiItemLabelTexts != null)
			{
				this.groupMultipleItems.Visible = true;
				this.centerLabel.Visible = false;
				this.rightLabel.Visible = false;
				this.multiItemMessageLabel.Text = this.message;
				this.sprite.Visible = false;
				this.textureImageInset.Enabled = false;
				if (this.multiItemTable != null && this.multiItemSpriteNames.Count == this.multiItemLabelTexts.Count)
				{
					this.multiItemTable.Clear();
					int i = 0;
					int count = this.multiItemSpriteNames.Count;
					while (i < count)
					{
						string itemUid = i.ToString();
						UXElement item = this.multiItemTable.CloneTemplateItem(itemUid);
						UXSprite subElement = this.multiItemTable.GetSubElement<UXSprite>(itemUid, "SpriteImageAndTextMultiple");
						UXLabel subElement2 = this.multiItemTable.GetSubElement<UXLabel>(itemUid, "LabelItemImageAndTextMultiple");
						UXUtils.SetupGeometryForIcon(subElement, this.multiItemSpriteNames[i]);
						subElement2.Text = this.multiItemLabelTexts[i];
						this.multiItemTable.AddItem(item, i);
						i++;
					}
				}
			}
		}

		private void OnPayButtonClicked(UXButton button)
		{
			button.Enabled = false;
			Service.EventManager.SendEvent(EventId.UINotEnoughSoftCurrencyBuy, base.ModalResultCookie);
			this.Close(true);
		}

		public override void Close(object modalResult)
		{
			base.Close(modalResult);
			if (modalResult != null)
			{
				return;
			}
			Service.EventManager.SendEvent(EventId.UINotEnoughSoftCurrencyClose, base.ModalResultCookie);
		}
	}
}
