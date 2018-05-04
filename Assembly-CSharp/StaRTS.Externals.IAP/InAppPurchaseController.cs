using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Commands.TargetedBundleOffers;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Externals.IAP
{
	public class InAppPurchaseController : IEventObserver
	{
		private const int MAX_NUM_VALIDATIONS_ATTEMPTS = 3;

		private const int MAX_STORE_RETRY_ATTEMPTS = 10;

		private const int SCHEDULED_VALIDATION_DELAY = 10;

		private const string TARGETED_OFFER_STRING = "targetedOffer";

		private IInAppPurchaseManager iapManager;

		private EventManager eventManager;

		private int numTimesValidatedItems;

		private int numStoreRetryAttempts;

		private int expectedIAPCount;

		private Dictionary<string, InAppPurchaseProductInfo> products;

		private Dictionary<string, InAppPurchaseTypeVO> validIAPTypes;

		public bool AreProductIdsReady
		{
			get;
			private set;
		}

		public InAppPurchaseController()
		{
			Service.InAppPurchaseController = this;
			this.numTimesValidatedItems = 0;
			this.AreProductIdsReady = false;
			this.iapManager = new AndroidIAPManager();
			this.iapManager.Init();
			this.products = new Dictionary<string, InAppPurchaseProductInfo>();
			this.validIAPTypes = new Dictionary<string, InAppPurchaseTypeVO>();
			this.eventManager = Service.EventManager;
			this.eventManager.RegisterObserver(this, EventId.SuccessfullyResumed, EventPriority.Default);
			if (!Service.EnvironmentController.IsRestrictedProfile())
			{
				this.eventManager.RegisterObserver(this, EventId.InitializeGeneralSystemsEnd, EventPriority.Default);
				this.eventManager.RegisterObserver(this, EventId.StoreScreenReady, EventPriority.Default);
				this.eventManager.RegisterObserver(this, EventId.StoreCategorySelected, EventPriority.Default);
				this.eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
				this.eventManager.RegisterObserver(this, EventId.TargetedBundleReserve, EventPriority.Default);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.InitializeGeneralSystemsEnd:
				this.CheckExpectedIAPCount();
				this.GetValidProductsFromStore();
				this.ScheduleValidateIAPItems();
				this.eventManager.UnregisterObserver(this, EventId.InitializeGeneralSystemsEnd);
				return EatResponse.NotEaten;
			case EventId.InitializeWorldStart:
			case EventId.InitializeWorldEnd:
			{
				IL_1E:
				if (id == EventId.StoreCategorySelected)
				{
					StoreTab storeTab = (StoreTab)((int)cookie);
					if (storeTab == StoreTab.Treasure && this.products.Count == 0)
					{
						Service.BILoggingController.TrackGameAction("iap", "get_products_treasure_empty", "no_valid_products", string.Empty);
					}
					return EatResponse.NotEaten;
				}
				if (id == EventId.StoreScreenReady)
				{
					if (this.numStoreRetryAttempts < 10)
					{
						if (this.products.Count == 0)
						{
							this.numStoreRetryAttempts++;
							Service.BILoggingController.TrackGameAction("iap", "get_products_store_retry", string.Empty, string.Empty);
							this.ValidateIAPItems(true);
						}
					}
					else if (this.products.Count == 0)
					{
						Service.BILoggingController.TrackGameAction("iap", "get_products_store_fail", this.numStoreRetryAttempts.ToString(), string.Empty);
					}
					return EatResponse.NotEaten;
				}
				if (id == EventId.WorldLoadComplete)
				{
					this.eventManager.UnregisterObserver(this, EventId.WorldLoadComplete);
					if (!this.AreProductIdsReady)
					{
						this.eventManager.RegisterObserver(this, EventId.IAPProductIDsReady, EventPriority.Default);
					}
					else
					{
						this.RestorePurchases();
					}
					return EatResponse.NotEaten;
				}
				if (id == EventId.IAPProductIDsReady)
				{
					this.eventManager.UnregisterObserver(this, EventId.IAPProductIDsReady);
					this.RestorePurchases();
					return EatResponse.NotEaten;
				}
				if (id != EventId.TargetedBundleReserve)
				{
					return EatResponse.NotEaten;
				}
				string productId = (string)cookie;
				this.ReserveTargetBundleOnServer(productId);
				return EatResponse.NotEaten;
			}
			case EventId.SuccessfullyResumed:
				Service.InAppPurchaseController.OnApplicationResume();
				return EatResponse.NotEaten;
			}
			goto IL_1E;
		}

		private void ReserveTargetBundleOnServer(string productId)
		{
			ReserveTargetedOfferIDRequest reserveTargetedOfferIDRequest = new ReserveTargetedOfferIDRequest();
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			if (targetedBundleController.CurrentTargetedOffer != null)
			{
				reserveTargetedOfferIDRequest.PlayerId = Service.CurrentPlayer.PlayerId;
				reserveTargetedOfferIDRequest.ProductId = productId;
				reserveTargetedOfferIDRequest.OfferId = targetedBundleController.CurrentTargetedOffer.Uid;
				ReserveTargetedOfferCommand reserveTargetedOfferCommand = new ReserveTargetedOfferCommand(reserveTargetedOfferIDRequest);
				reserveTargetedOfferCommand.AddFailureCallback(new AbstractCommand<ReserveTargetedOfferIDRequest, DefaultResponse>.OnFailureCallback(this.OnFailure));
				Service.ServerAPI.Sync(reserveTargetedOfferCommand);
			}
		}

		private void OnFailure(uint status, object cookie)
		{
			Service.Logger.Error("Failed in reserving targeted bundle offers.  Status = " + status.ToString());
		}

		private void ScheduleValidateIAPItems()
		{
			Service.ViewTimerManager.CreateViewTimer(10f, false, new TimerDelegate(this.ScheduledCallback), 0);
		}

		private void ScheduledCallback(uint id, object cookie)
		{
			this.ValidateIAPItems(true);
		}

		public bool IsIAPForCurrentPlatform(InAppPurchaseTypeVO iapVO)
		{
			bool result = false;
			if (iapVO.Type == "a")
			{
				result = true;
			}
			return result;
		}

		private void CheckExpectedIAPCount()
		{
			foreach (InAppPurchaseTypeVO current in this.GetAllIAPTypes())
			{
				if (!current.IsPromo)
				{
					if (this.IsIAPForCurrentPlatform(current))
					{
						this.expectedIAPCount++;
					}
				}
			}
		}

		public void GetValidProductsFromStore()
		{
			if (!Service.EnvironmentController.IsRestrictedProfile())
			{
				this.iapManager.GetProducts();
			}
			else
			{
				Service.BILoggingController.TrackGameAction("iap", "iap_restricted_account", "restricted_user", string.Empty);
			}
		}

		public void OnGetInfoForProducts(string value, List<InAppPurchaseProductInfo> productInfoList = null)
		{
			if (productInfoList == null)
			{
				this.ParseProductsFromJson(value);
			}
			else
			{
				this.ParseProductsFromList(productInfoList);
			}
			this.ValidateIAPItems(false);
			if (this.products.Count == this.expectedIAPCount)
			{
				if (this.numStoreRetryAttempts < 1)
				{
					int num = this.numStoreRetryAttempts - 1;
					Service.BILoggingController.TrackGameAction("iap", "get_products_init_success", num.ToString(), string.Empty);
				}
				else
				{
					Service.BILoggingController.TrackGameAction("iap", "get_products_store_success", this.numStoreRetryAttempts.ToString(), string.Empty);
				}
				this.AreProductIdsReady = true;
				Service.EventManager.SendEvent(EventId.IAPProductIDsReady, null);
			}
		}

		private void ParseProductsFromJson(string value)
		{
			Dictionary<string, InAppPurchaseTypeVO> allIAPTypesByProductID = this.GetAllIAPTypesByProductID();
			IDictionary<string, object> dictionary = new JsonParser(value).Parse() as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("products"))
			{
				List<object> list = dictionary["products"] as List<object>;
				if (list != null)
				{
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						InAppPurchaseProductInfo inAppPurchaseProductInfo = InAppPurchaseProductInfo.Parse(list[i]);
						Service.Logger.Debug("IAP Product: " + inAppPurchaseProductInfo.ToString());
						if (allIAPTypesByProductID.ContainsKey(inAppPurchaseProductInfo.AppStoreId))
						{
							InAppPurchaseTypeVO inAppPurchaseTypeVO = allIAPTypesByProductID[inAppPurchaseProductInfo.AppStoreId];
							if (!inAppPurchaseTypeVO.IsPromo)
							{
								if (!this.products.ContainsKey(inAppPurchaseProductInfo.AppStoreId))
								{
									this.products.Add(inAppPurchaseProductInfo.AppStoreId, inAppPurchaseProductInfo);
								}
								if (!this.validIAPTypes.ContainsKey(inAppPurchaseTypeVO.ProductId))
								{
									this.validIAPTypes.Add(inAppPurchaseTypeVO.ProductId, inAppPurchaseTypeVO);
								}
							}
						}
						else
						{
							Service.Logger.Debug("IAP Item no longer supported: " + inAppPurchaseProductInfo.AppStoreId);
						}
					}
					Service.Logger.Debug("Number of valid products: " + count);
				}
			}
		}

		private void ParseProductsFromList(List<InAppPurchaseProductInfo> productInfoList)
		{
			Dictionary<string, InAppPurchaseTypeVO> allIAPTypesByProductID = this.GetAllIAPTypesByProductID();
			int count = productInfoList.Count;
			for (int i = 0; i < count; i++)
			{
				InAppPurchaseProductInfo inAppPurchaseProductInfo = productInfoList[i];
				Service.Logger.Debug("IAP Product: " + inAppPurchaseProductInfo.ToString());
				if (allIAPTypesByProductID.ContainsKey(inAppPurchaseProductInfo.AppStoreId))
				{
					InAppPurchaseTypeVO inAppPurchaseTypeVO = allIAPTypesByProductID[inAppPurchaseProductInfo.AppStoreId];
					if (!inAppPurchaseTypeVO.IsPromo)
					{
						if (!this.products.ContainsKey(inAppPurchaseProductInfo.AppStoreId))
						{
							this.products.Add(inAppPurchaseProductInfo.AppStoreId, inAppPurchaseProductInfo);
						}
						if (!this.validIAPTypes.ContainsKey(inAppPurchaseTypeVO.ProductId))
						{
							this.validIAPTypes.Add(inAppPurchaseTypeVO.ProductId, inAppPurchaseTypeVO);
						}
					}
				}
				else
				{
					Service.Logger.Debug("IAP Item no longer supported: " + inAppPurchaseProductInfo.AppStoreId);
				}
			}
			Service.Logger.Debug("Number of valid products: " + count);
		}

		public void Consume(string consumeId)
		{
			this.iapManager.Consume(consumeId);
		}

		public void RestorePurchases()
		{
			this.iapManager.RestorePurchases();
		}

		public void PurchaseProduct(string productID)
		{
			Service.GameIdleController.Enabled = false;
			this.iapManager.Purchase(productID);
		}

		public void OnPurchaseProductResponse(string jsonString, bool gameIdleControllerEnabled)
		{
			InAppPurchaseReceipt receipt = InAppPurchaseReceipt.Parse(jsonString);
			this.OnPurchaseProductResponse(receipt, gameIdleControllerEnabled);
		}

		public void OnPurchaseProductResponse(InAppPurchaseReceipt receipt, bool gameIdleControllerEnabled)
		{
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = gameIdleControllerEnabled;
			}
			if (receipt.errorCode != 0u)
			{
				return;
			}
			MoneyReceiptVerifyRequest moneyReceiptVerifyRequest = new MoneyReceiptVerifyRequest();
			moneyReceiptVerifyRequest.Receipt = receipt.GetManimalReceiptString();
			moneyReceiptVerifyRequest.VendorKey = "googleV3";
			if (!string.IsNullOrEmpty(receipt.userId))
			{
				if (moneyReceiptVerifyRequest.ExtraParams == null)
				{
					moneyReceiptVerifyRequest.ExtraParams = new Dictionary<string, string>();
				}
				moneyReceiptVerifyRequest.ExtraParams.Add("userId", receipt.userId);
			}
			moneyReceiptVerifyRequest.PlayerId = Service.CurrentPlayer.PlayerId;
			ServerAPI serverAPI = Service.ServerAPI;
			MoneyReceiptVerifyCommand moneyReceiptVerifyCommand = new MoneyReceiptVerifyCommand(moneyReceiptVerifyRequest);
			moneyReceiptVerifyCommand.ProductId = receipt.productId;
			moneyReceiptVerifyCommand.SetTransactionId(receipt.transactionId);
			if (!serverAPI.Enabled && Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				serverAPI.Enabled = true;
				serverAPI.Sync(moneyReceiptVerifyCommand);
				serverAPI.Enabled = false;
			}
			else
			{
				serverAPI.Sync(moneyReceiptVerifyCommand);
			}
		}

		public void HandleReceiptVerificationResponse(string uid, string transactionId, string currencyCode, double price, double bonusMultiplier, bool isPromo, string offerUid, CrateData crateData)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			StaticDataController staticDataController = Service.StaticDataController;
			InAppPurchaseTypeVO inAppPurchaseTypeVO = staticDataController.Get<InAppPurchaseTypeVO>(uid);
			if (inAppPurchaseTypeVO.IsPromo)
			{
				isPromo = true;
			}
			bool flag = !string.IsNullOrEmpty(offerUid);
			string text = inAppPurchaseTypeVO.RewardEmpire;
			if (currentPlayer.Faction == FactionType.Rebel)
			{
				text = inAppPurchaseTypeVO.RewardRebel;
			}
			if (string.IsNullOrEmpty(text) && !flag)
			{
				Service.Logger.Error("MoneyReceiptVerifyResponse:" + inAppPurchaseTypeVO.Uid + " faction specific reward uids do not exist");
				return;
			}
			bool flag2 = false;
			TargetedBundleVO targetedBundleVO = null;
			if (flag)
			{
				targetedBundleVO = staticDataController.GetOptional<TargetedBundleVO>(offerUid);
				if (targetedBundleVO != null)
				{
					Service.TargetedBundleController.GrantTargetedBundleRewards(targetedBundleVO);
					flag2 = true;
				}
				else
				{
					Service.Logger.Error("MoneyReceiptVerifyResponse: targeted offer " + offerUid + " does not exist");
				}
			}
			else
			{
				RewardVO optional = staticDataController.GetOptional<RewardVO>(text);
				if (optional == null)
				{
					Service.Logger.Error("MoneyReceiptVerifyResponse:" + text + " does not exist");
				}
				else if (inAppPurchaseTypeVO.CurrencyType.Equals("hard"))
				{
					RewardUtils.GrantReward(currentPlayer, optional, bonusMultiplier);
				}
				else
				{
					RewardUtils.GrantInAppPurchaseRewardToHQInventory(optional);
				}
			}
			if (inAppPurchaseTypeVO.ProductId.Contains("promo"))
			{
				isPromo = true;
			}
			int amount;
			if (inAppPurchaseTypeVO.CurrencyType.Equals("hard"))
			{
				amount = (int)Math.Floor(bonusMultiplier * (double)inAppPurchaseTypeVO.Amount);
			}
			else
			{
				amount = inAppPurchaseTypeVO.Amount;
			}
			this.Consume(inAppPurchaseTypeVO.ProductId);
			string text2 = uid;
			bool flag3 = false;
			SaleTypeVO currentActiveSale = SaleUtils.GetCurrentActiveSale();
			if (currentActiveSale != null && !isPromo)
			{
				text2 = text2 + "_" + currentActiveSale.Uid;
			}
			if (isPromo)
			{
				text2 += "_promo";
				flag3 = true;
			}
			if (GameConstants.IAP_FORCE_POPUP_ENABLED)
			{
				flag3 = true;
			}
			if (flag2)
			{
				Service.TargetedBundleController.HandleTargetedOfferSuccess(crateData, targetedBundleVO);
			}
			else if (flag3)
			{
				this.ShowRedemptionScreen(amount, uid);
				Service.EventManager.SendEvent(EventId.InAppPurchaseMade, null);
			}
			Service.DMOAnalyticsController.LogPaymentAction(currencyCode, price, inAppPurchaseTypeVO.ProductId, 1, text2);
		}

		private void ShowRedemptionScreen(int amount, string uid)
		{
			PromoRedemptionScreen promoRedemptionScreen = new PromoRedemptionScreen(uid, amount);
			promoRedemptionScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(promoRedemptionScreen, true, true);
		}

		public void SetIAPRewardIcon(UXSprite iconSprite, string uid)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			InAppPurchaseTypeVO inAppPurchaseTypeVO = staticDataController.Get<InAppPurchaseTypeVO>(uid);
			string rewardUid = string.Empty;
			if (Service.CurrentPlayer.Faction == FactionType.Empire)
			{
				rewardUid = inAppPurchaseTypeVO.RewardEmpire;
			}
			else
			{
				rewardUid = inAppPurchaseTypeVO.RewardRebel;
			}
			if (inAppPurchaseTypeVO.CurrencyType.Equals("hard") || inAppPurchaseTypeVO.CurrencyType.Equals("pack"))
			{
				UXUtils.SetupGeometryForIcon(iconSprite, inAppPurchaseTypeVO.CurrencyIconId);
			}
			else
			{
				RewardType rewardType = RewardType.Invalid;
				IGeometryVO config;
				Service.RewardManager.GetFirstRewardAssetName(rewardUid, ref rewardType, out config);
				RewardUtils.SetRewardIcon(iconSprite, config, AnimationPreference.NoAnimation);
			}
		}

		public void OnApplicationResume()
		{
			this.iapManager.OnApplicationResume();
		}

		public List<InAppPurchaseTypeVO> GetValidIAPTypes()
		{
			List<InAppPurchaseTypeVO> list = new List<InAppPurchaseTypeVO>();
			foreach (KeyValuePair<string, InAppPurchaseTypeVO> current in this.validIAPTypes)
			{
				if (!current.Value.HideFromStore)
				{
					list.Add(current.Value);
				}
			}
			list.Sort(new Comparison<InAppPurchaseTypeVO>(InAppPurchaseController.CompareIAPs));
			return list;
		}

		public int GetNumberOfValidIapItems()
		{
			return this.validIAPTypes.Count;
		}

		public void ValidateIAPItems(bool bypassCheck)
		{
			if (Service.EnvironmentController.IsRestrictedProfile())
			{
				return;
			}
			if (!bypassCheck)
			{
				if (this.numTimesValidatedItems >= 3)
				{
					Service.BILoggingController.TrackGameAction("iap", "get_products_init_fail", this.numTimesValidatedItems.ToString(), string.Empty);
					return;
				}
				this.numTimesValidatedItems++;
			}
			if (this.validIAPTypes.Count < this.expectedIAPCount)
			{
				Service.Logger.ErrorFormat("Unexpected number of IAP store items. Expected {0}, got {1}.", new object[]
				{
					this.expectedIAPCount,
					this.validIAPTypes.Count
				});
				this.GetValidProductsFromStore();
			}
		}

		private static int CompareIAPs(InAppPurchaseTypeVO a, InAppPurchaseTypeVO b)
		{
			if (a == b)
			{
				return 0;
			}
			return (a.Order >= b.Order) ? -1 : 1;
		}

		public InAppPurchaseProductInfo GetIAPProduct(string productID)
		{
			if (this.products.ContainsKey(productID))
			{
				return this.products[productID];
			}
			return null;
		}

		public Dictionary<string, InAppPurchaseTypeVO>.ValueCollection GetAllIAPTypes()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			return staticDataController.GetAll<InAppPurchaseTypeVO>();
		}

		private Dictionary<string, InAppPurchaseTypeVO> GetAllIAPTypesByProductID()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			Dictionary<string, InAppPurchaseTypeVO>.ValueCollection all = staticDataController.GetAll<InAppPurchaseTypeVO>();
			Dictionary<string, InAppPurchaseTypeVO> dictionary = new Dictionary<string, InAppPurchaseTypeVO>();
			foreach (InAppPurchaseTypeVO current in all)
			{
				if (this.IsIAPForCurrentPlatform(current))
				{
					dictionary.Add(current.ProductId, current);
				}
			}
			return dictionary;
		}
	}
}
