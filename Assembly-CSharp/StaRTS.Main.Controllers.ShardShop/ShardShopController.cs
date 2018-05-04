using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Shards;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.ShardShop
{
	public class ShardShopController : IEventObserver
	{
		public const int SHARD_SLOT_MAX = 5;

		private int[] adjustedOfferQuantity;

		private uint expirationTimer;

		public ShardShopData CurrentShopData
		{
			get;
			private set;
		}

		public int ClientPredictionId
		{
			get;
			private set;
		}

		public ShardShopController()
		{
			Service.ShardShopController = this;
			this.adjustedOfferQuantity = new int[5];
			Service.EventManager.RegisterObserver(this, EventId.WorldLoadComplete);
		}

		public ShardShopViewTO GenerateViewTO(int index, CurrentPlayer player, ShardShopData data)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			int playerHQLevel = player.Map.FindHighestHqLevel();
			ShardShopViewTO shardShopViewTO = new ShardShopViewTO();
			string shardSlotId = GameUtils.GetShardSlotId(index);
			int num = 0;
			foreach (int current in data.Purchases[index].Values)
			{
				num += current;
			}
			int num2 = this.adjustedOfferQuantity[index] - num;
			shardShopViewTO.SlotIndex = index;
			shardShopViewTO.SupplyVO = data.ShardOffers[shardSlotId];
			shardShopViewTO.RemainingShardsForSale = num2;
			if (num2 > 0)
			{
				shardShopViewTO.CostOfNextShard = ShardShopController.CalculateCost(index, 1, data);
				shardShopViewTO.CostOfAllShards = ShardShopController.CalculateCost(index, num2, data);
			}
			shardShopViewTO.CanAffordSingle = CostUtils.HasRequiredCurrency(player, shardShopViewTO.CostOfNextShard);
			shardShopViewTO.CanAffordAll = CostUtils.HasRequiredCurrency(player, shardShopViewTO.CostOfAllShards);
			shardShopViewTO.PlayerHQLevel = playerHQLevel;
			shardShopViewTO.ItemName = GameUtils.GetShardShopNameWithoutQuantity(shardShopViewTO.SupplyVO, staticDataController);
			shardShopViewTO.Quality = GameUtils.GetShardQualityNumeric(shardShopViewTO.SupplyVO);
			shardShopViewTO.UpgradeShardsEarned = GameUtils.GetUpgradeShardsOwned(shardShopViewTO.SupplyVO, player);
			shardShopViewTO.UpgradeShardsRequired = GameUtils.GetUpgradeShardsRequired(shardShopViewTO.SupplyVO, player, staticDataController);
			shardShopViewTO.State = "unlocked";
			SupplyType type = shardShopViewTO.SupplyVO.Type;
			if (this.adjustedOfferQuantity[index] < 1)
			{
				shardShopViewTO.State = "maxedOut";
			}
			else if (num2 < 1)
			{
				shardShopViewTO.State = "soldOut";
			}
			else if (type == SupplyType.Shard && !ArmoryUtils.PlayerHasArmory())
			{
				shardShopViewTO.State = "requiresArmory";
			}
			else if (type == SupplyType.ShardTroop)
			{
				ShardVO shardVO = staticDataController.Get<ShardVO>(shardShopViewTO.SupplyVO.RewardUid);
				TroopUpgradeCatalog troopUpgradeCatalog = Service.TroopUpgradeCatalog;
				TroopTypeVO byLevel = troopUpgradeCatalog.GetByLevel(shardVO.TargetGroupId, 1);
				if (byLevel != null && byLevel.Type == TroopType.Mercenary && !Service.BuildingLookupController.HasCantina())
				{
					shardShopViewTO.State = "requiresCantina";
				}
			}
			if (type == SupplyType.Shard)
			{
				EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
				EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(shardShopViewTO.SupplyVO.RewardUid);
				int shards = player.GetShards(shardShopViewTO.SupplyVO.RewardUid);
				int shardsRequiredForNextUpgrade = ArmoryUtils.GetShardsRequiredForNextUpgrade(player, equipmentUpgradeCatalog, currentEquipmentDataByID);
				shardShopViewTO.Upgradeable = (shards >= shardsRequiredForNextUpgrade && shardsRequiredForNextUpgrade > 0);
			}
			else if (type == SupplyType.ShardTroop || type == SupplyType.ShardSpecialAttack)
			{
				shardShopViewTO.Upgradeable = Service.DeployableShardUnlockController.IsShardDeployableReadyToUpgrade(shardShopViewTO.SupplyVO.RewardUid);
			}
			return shardShopViewTO;
		}

		public void GetOffering(Action callback)
		{
			this.GetOffering(callback, true);
		}

		public void GetOffering(Action callback, bool immediate)
		{
			GetShardOfferingCommand getShardOfferingCommand = new GetShardOfferingCommand(new GetShardOfferingRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getShardOfferingCommand.Context = callback;
			getShardOfferingCommand.AddSuccessCallback(new AbstractCommand<GetShardOfferingRequest, GetShardOfferingResponse>.OnSuccessCallback(this.OnOfferingUpdated));
			Service.ServerAPI.Enqueue(getShardOfferingCommand);
			if (immediate)
			{
				Service.ServerAPI.Sync();
			}
		}

		private void OnOfferingUpdated(GetShardOfferingResponse response, object cookie)
		{
			bool flag;
			if (this.CurrentShopData == null)
			{
				flag = (response.ShopData != null);
			}
			else
			{
				flag = this.CurrentShopData.HasOfferChanged(response.ShopData);
			}
			if (flag)
			{
				this.CurrentShopData = response.ShopData;
				Service.EventManager.SendEvent(EventId.ShardOfferChanged, null);
			}
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			if (this.expirationTimer != 0u)
			{
				viewTimerManager.KillViewTimer(this.expirationTimer);
			}
			int num = (int)(Service.ServerAPI.ServerTime + (uint)((int)(this.CurrentShopData.OffsetMinutes * 60f)));
			float num2 = (float)((ulong)this.CurrentShopData.Expiration - (ulong)((long)num));
			num2 = Math.Max(1f, Math.Min(num2, 432000f));
			this.expirationTimer = viewTimerManager.CreateViewTimer(num2, false, new TimerDelegate(this.OnOfferExpired), null);
			if (cookie != null)
			{
				Action action = (Action)cookie;
				action();
			}
			this.AdjustOfferQuantities();
		}

		private void AdjustOfferQuantities()
		{
			int count = this.CurrentShopData.ShardOffers.Count;
			for (int i = 0; i < count; i++)
			{
				ShardShopPoolVO pool = this.CurrentShopData.ActiveSeries.GetPool(i);
				string shardSlotId = GameUtils.GetShardSlotId(i);
				CrateSupplyVO crateSupplyVO = this.CurrentShopData.ShardOffers[shardSlotId];
				int shardCountToReachMaxLevel = GameUtils.GetShardCountToReachMaxLevel(crateSupplyVO.RewardUid, crateSupplyVO.Type);
				int spentShardCount = GameUtils.GetSpentShardCount(crateSupplyVO.RewardUid, crateSupplyVO.Type);
				int upgradeShardsOwned = GameUtils.GetUpgradeShardsOwned(crateSupplyVO, Service.CurrentPlayer);
				int val = shardCountToReachMaxLevel - (spentShardCount + upgradeShardsOwned);
				this.adjustedOfferQuantity[i] = Math.Min(val, pool.GetTotalQuantity());
			}
		}

		private void OnOfferExpired(uint id, object cookie)
		{
			this.expirationTimer = 0u;
			this.GetOffering(null, true);
		}

		public void ResetClientPredictionId()
		{
			this.ClientPredictionId = 0;
		}

		public bool BuyShards(int slotIndex, int quantity, Action<int, bool> deferredSuccess, Action<object> serverCallback)
		{
			CostVO costVO = ShardShopController.CalculateCost(slotIndex, quantity, this.CurrentShopData);
			MissingCurrencyTypes missingCurrencyTypes = CostUtils.CheckForMissingCurrency(Service.CurrentPlayer, costVO);
			switch (missingCurrencyTypes)
			{
			case MissingCurrencyTypes.Soft:
			{
				ShardShopPurchaseCookie purchaseCookie = new ShardShopPurchaseCookie(slotIndex, quantity, deferredSuccess, serverCallback, this.ClientPredictionId, costVO);
				PayMeScreen.ShowIfNotEnoughCurrency(costVO.Credits, costVO.Materials, costVO.Contraband, "shard", purchaseCookie, new OnScreenModalResult(this.OnPurchaseMissingSoftCurrency));
				break;
			}
			case MissingCurrencyTypes.Hard:
				GameUtils.PromptToBuyCrystals();
				break;
			case MissingCurrencyTypes.MultipleSoft:
			{
				ShardShopPurchaseCookie purchaseCookie2 = new ShardShopPurchaseCookie(slotIndex, quantity, deferredSuccess, serverCallback, this.ClientPredictionId, costVO);
				MultiResourcePayMeScreen.ShowIfNotEnoughMultipleCurrencies(costVO, "shard", purchaseCookie2, new OnScreenModalResult(this.OnPurchaseMissingSoftCurrency));
				break;
			}
			}
			if (missingCurrencyTypes != MissingCurrencyTypes.None)
			{
				return false;
			}
			this.ClientPredictionId++;
			this.PurchasePoolIdShard(slotIndex, quantity, serverCallback, this.ClientPredictionId, costVO);
			return true;
		}

		private void PurchasePoolIdShard(int slotIndex, int quantity, Action<object> callback, object cookie, CostVO cost)
		{
			Dictionary<string, int> dictionary = this.CurrentShopData.Purchases[slotIndex];
			string shardSlotId = GameUtils.GetShardSlotId(slotIndex);
			CostUtils.DeductCost(Service.CurrentPlayer, cost);
			CrateSupplyVO crateSupplyVO = this.CurrentShopData.ShardOffers[shardSlotId];
			Service.InventoryCrateRewardController.GrantShopSupply(crateSupplyVO, quantity);
			string uid = crateSupplyVO.Uid;
			if (dictionary.ContainsKey(uid))
			{
				dictionary[uid] += quantity;
			}
			else
			{
				dictionary.Add(uid, quantity);
			}
			BuyShardRequest request = new BuyShardRequest(shardSlotId, quantity);
			BuyShardCommand buyShardCommand = new BuyShardCommand(request);
			buyShardCommand.Context = new KeyValuePair<object, object>(callback, cookie);
			buyShardCommand.AddSuccessCallback(new AbstractCommand<BuyShardRequest, GetShardOfferingResponse>.OnSuccessCallback(this.OnBuyShardSuccess));
			Service.ServerAPI.Sync(buyShardCommand);
		}

		private void OnPurchaseMissingSoftCurrency(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				ShardShopPurchaseCookie shardShopPurchaseCookie = null;
				if (cookie is CurrencyTag)
				{
					CurrencyTag currencyTag = (CurrencyTag)cookie;
					shardShopPurchaseCookie = (ShardShopPurchaseCookie)currencyTag.Cookie;
				}
				else if (cookie is MultiCurrencyTag)
				{
					MultiCurrencyTag multiCurrencyTag = (MultiCurrencyTag)cookie;
					shardShopPurchaseCookie = (ShardShopPurchaseCookie)multiCurrencyTag.Cookie;
				}
				if (shardShopPurchaseCookie != null)
				{
					shardShopPurchaseCookie.DeferredSuccessCallback(shardShopPurchaseCookie.Quantity, true);
					CostVO cost = shardShopPurchaseCookie.Cost;
					this.PurchasePoolIdShard(shardShopPurchaseCookie.SlotIndex, shardShopPurchaseCookie.Quantity, shardShopPurchaseCookie.ServerCallback, shardShopPurchaseCookie.Cookie, cost);
				}
				else
				{
					Service.Logger.Error("Shard Shop Missing soft currency purchase has null purchase cookie");
				}
			}
		}

		public static CostVO CalculateCost(int slotIndex, int quantity, ShardShopData data)
		{
			Dictionary<string, int> dictionary = data.Purchases[slotIndex];
			ShardShopPoolVO pool = data.ActiveSeries.GetPool(slotIndex);
			int num = 0;
			foreach (int current in dictionary.Values)
			{
				num += current;
			}
			CostVO costVO = pool.GetCost(num);
			if (quantity > 1)
			{
				for (int i = 1; i < quantity; i++)
				{
					CostVO cost = pool.GetCost(num + i);
					costVO = CostUtils.Combine(costVO, cost);
				}
			}
			return CostUtils.CombineCurrenciesForShards(costVO);
		}

		private void OnBuyShardSuccess(GetShardOfferingResponse response, object cookie)
		{
			bool flag;
			if (this.CurrentShopData == null)
			{
				flag = (response.ShopData != null);
			}
			else
			{
				flag = this.CurrentShopData.HasOfferChanged(response.ShopData);
			}
			if (cookie != null)
			{
				int num = (int)((KeyValuePair<object, object>)cookie).Value;
				if (num == this.ClientPredictionId)
				{
					this.CurrentShopData = response.ShopData;
				}
			}
			if (flag)
			{
				Service.EventManager.SendEvent(EventId.ShardOfferChanged, null);
			}
			if (cookie != null)
			{
				KeyValuePair<object, object> keyValuePair = (KeyValuePair<object, object>)cookie;
				Action<object> action = (Action<object>)keyValuePair.Key;
				action(keyValuePair.Value);
			}
		}

		public bool IsShardShopUnlocked()
		{
			int num = Service.CurrentPlayer.Map.FindHighestHqLevel();
			int sHARD_SHOP_MINIMUM_HQ = GameConstants.SHARD_SHOP_MINIMUM_HQ;
			return num >= sHARD_SHOP_MINIMUM_HQ;
		}

		public void SaveShardShopExpiration()
		{
			if (this.CurrentShopData == null)
			{
				Service.Logger.Warn("Trying to save Shard Shop expiration time, CurrentShopData is NULL!");
				return;
			}
			PlayerSettings.SetShardShopExpiration(this.CurrentShopData.Expiration.ToString("D"));
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			IGameState gameState = Service.GameStateMachine.CurrentState as IGameState;
			if (id == EventId.WorldLoadComplete)
			{
				if (gameState is ApplicationLoadState || gameState is HomeState)
				{
					List<Contract> list = Service.ISupportController.FindAllContractsOfType(ContractType.Research);
					if (list.Count > 0)
					{
						this.AdjustOfferQuantities();
					}
					Service.EventManager.UnregisterObserver(this, EventId.WorldLoadComplete);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
